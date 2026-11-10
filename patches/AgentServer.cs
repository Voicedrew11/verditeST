using System.Collections.Concurrent;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using RecompOne.Runtime.Dispatch;
using RecompOne.Runtime.Events;
using RecompOne.Runtime.Hardware;
using RecompOne.Runtime.Memory;

namespace St;

/// <summary>
/// The command channel: ST_SHELL=1 listens on 127.0.0.1:27904 (ST_SHELL=&lt;port&gt;
/// picks another). One request per line, one line of reply. Requests are read on a
/// client thread and run on the game thread, drained on VSync. See "Driving the game
/// without a person" in docs/DEVELOPMENT.md.
/// </summary>
public static class AgentServer
{
    const int DefaultPort = 27904;
    const int ReplyTimeoutMs = 5000;
    const int DefaultHoldMs = 150;
    const int MaxPeekBytes = 4096;
    const uint GameStackFloor = 0x80000000;

    // func_80044EE8 loads the area whose map number is in the byte at AreaRequest,
    // with the four bytes after it 0. A door writes the arrival point into the player
    // block while the module loads (func_80050768, at 0x800525E8); the warp writes it
    // with the request. See "Changing areas" in docs/GAME_INTERNALS.md.
    const uint AreaRequest = 0x801B5924;
    const uint LoadArea = 0x80044EE8;

    // The maps with an area module, FDAT.T entry 10n+2 (config/st.json); the others'
    // entries are not modules.
    static readonly HashSet<int> Maps =
    [
        0, 1, 2, 3, 4, 5, 6, 8, 9, 10, 11, 12, 13, 14, 15, 17, 18, 20,
        21, 23, 24, 25, 26, 27, 28, 30, 32, 33, 37, 38, 40, 41, 44, 45, 46, 47,
    ];

    static readonly Dictionary<string, ushort> Buttons = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Select"] = Controller.Select, ["Start"] = Controller.Start,
        ["Cross"] = Controller.Cross, ["Circle"] = Controller.Circle,
        ["Square"] = Controller.Square, ["Triangle"] = Controller.Triangle,
        ["L1"] = Controller.L1, ["R1"] = Controller.R1,
        ["L2"] = Controller.L2, ["R2"] = Controller.R2,
        ["Up"] = Controller.Up, ["Down"] = Controller.Down,
        ["Left"] = Controller.Left, ["Right"] = Controller.Right,
    };

    sealed record Request(string[] Words, TaskCompletionSource<string> Reply);

    static readonly ConcurrentQueue<Request> _requests = new();
    static IMemory _memory = null!;

    // Written and read on the game thread only.
    static ushort _heldBits;
    static long _heldUntilMs;
    static readonly Queue<(uint Address, uint A0, uint A1)> _calls = new();
    static bool _calling;

    public static void Install(string? spec, IMemory memory)
    {
        int port = ParsePort(spec);
        if (port == 0) return;
        _memory = memory;

        Event.AddListener<VSyncEvent>(OnVSync);
        Event.AddListener<PadReadEvent>(OnPadRead);

        var listener = new TcpListener(IPAddress.Loopback, port);
        try
        {
            listener.Start();
        }
        catch (SocketException ex)
        {
            Console.Error.WriteLine($"[ST] agent server: could not listen on port {port}: {ex.Message}");
            return;
        }
        new Thread(() => AcceptClients(listener)) { IsBackground = true, Name = "st-agent-server" }.Start();
        Console.WriteLine($"[ST] agent server: listening on 127.0.0.1:{port}");
    }

    static int ParsePort(string? spec)
    {
        string text = (spec ?? "").Trim().ToLowerInvariant();
        if (text is "" or "0" or "off") return 0;
        if (text is "1" or "on") return DefaultPort;
        if (!int.TryParse(text, out int port) || port is < 1 or > 65535)
            throw new ArgumentException($"ST_SHELL: cannot read '{spec}'");
        return port;
    }

    // Game thread.
    static void OnVSync(VSyncEvent e)
    {
        while (_requests.TryDequeue(out var request))
            request.Reply.TrySetResult(ExecuteGuarded(request.Words));
        RunQueuedCalls(e);
    }

    // Game thread. A routine that draws its own frames comes back through OnVSync, so
    // the guard stops a nested vblank from starting the next call early, and requests
    // keep being served while the routine runs. A vblank delivered from the interrupt
    // poll is on the exception stack, in the middle of whatever the game was doing, so
    // calls wait for one that comes from the game's own VSync.
    static void RunQueuedCalls(VSyncEvent e)
    {
        if (_calling || e.Context.SP < GameStackFloor) return;
        _calling = true;
        try
        {
            while (_calls.TryDequeue(out var call)) RunCall(e, call);
        }
        finally
        {
            _calling = false;
        }
    }

    static void RunCall(VSyncEvent e, (uint Address, uint A0, uint A1) call)
    {
        var saved = e.Context.Snapshot();
        e.Context.A0 = call.A0;
        e.Context.A1 = call.A1;
        Dispatcher.Call(e.Context, e.Memory, call.Address);
        e.Context.Restore(saved);
        Console.WriteLine($"[ST] agent server: call {call.Address:X8} returned");
    }

    // Game thread. The event's buttons are active-low with their two bytes swapped
    // against Controller's layout.
    static void OnPadRead(PadReadEvent e)
    {
        if (e.Port != 0 || _heldBits == 0) return;
        if (Environment.TickCount64 >= _heldUntilMs)
        {
            _heldBits = 0;
            return;
        }
        e.Buttons &= (ushort)~(ushort)((_heldBits >> 8) | (_heldBits << 8));
    }

    // Client threads below; nothing here touches game state.
    static void AcceptClients(TcpListener listener)
    {
        while (true)
        {
            TcpClient client;
            try
            {
                client = listener.AcceptTcpClient();
            }
            catch (SocketException)
            {
                continue;
            }
            new Thread(() => Serve(client)) { IsBackground = true, Name = "st-agent-client" }.Start();
        }
    }

    static void Serve(TcpClient client)
    {
        using (client)
        using (var reader = new StreamReader(client.GetStream(), new UTF8Encoding(false)))
        using (var writer = new StreamWriter(client.GetStream(), new UTF8Encoding(false)) { AutoFlush = true })
        {
            try
            {
                while (reader.ReadLine() is { } line)
                {
                    string[] words = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (words.Length > 0) writer.WriteLine(Submit(words));
                }
            }
            catch (IOException)
            {
                // The client went away; the game carries on.
            }
        }
    }

    static string Submit(string[] words)
    {
        var reply = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
        _requests.Enqueue(new Request(words, reply));
        return reply.Task.Wait(ReplyTimeoutMs) ? reply.Task.Result : "error: timed out, no vsync for 5s";
    }

    // Game thread from here down.
    static string ExecuteGuarded(string[] words)
    {
        try
        {
            return Execute(words);
        }
        catch (Exception ex) when (ex is FormatException or ArgumentException or OverflowException or IOException or IndexOutOfRangeException or InvalidOperationException)
        {
            return "error: " + ex.Message;
        }
    }

    static string Execute(string[] words) => words[0].ToLowerInvariant() switch
    {
        "state" => AgentBeacon.Snapshot(),
        "warp" => Warp(words),
        "press" => Press(words),
        "peek" => Peek(words),
        "poke" => Poke(words),
        "dump" => Dump(words),
        "call" => QueueCall(words),
        _ => "error: commands are state, press <a+b> [ms], warp <map> [x y z], peek <addr> [bytes], poke <addr> <hex>, dump <file>, call <addr> [a0] [a1]",
    };

    static string QueueCall(string[] words)
    {
        uint address = ParseAddress(words[1]);
        uint a0 = words.Length > 2 ? ParseAddress(words[2]) : 0;
        uint a1 = words.Length > 3 ? ParseAddress(words[3]) : 0;
        _calls.Enqueue((address, a0, a1));
        return $"queued {address:X8}({a0:X}, {a1:X})";
    }

    // warp <map> [x y z]. The area module loads over the next few frames; state's map
    // says when it is in. Without a point the player keeps their coordinates, which in
    // another map is usually no floor: map 0's start in map 1 is a death on arrival.
    static string Warp(string[] words)
    {
        int map = int.Parse(words[1], CultureInfo.InvariantCulture);
        if (!Maps.Contains(map)) throw new ArgumentException($"map {map} has no area module");
        if (AgentBeacon.Program != "st") throw new InvalidOperationException("warp needs ST.EXE running");
        if (words.Length is not (2 or 5)) throw new ArgumentException("warp takes a map and optionally x y z");

        string arrival = "where the player stands";
        if (words.Length == 5)
        {
            int x = int.Parse(words[2], CultureInfo.InvariantCulture);
            int y = int.Parse(words[3], CultureInfo.InvariantCulture);
            int z = int.Parse(words[4], CultureInfo.InvariantCulture);
            _memory.WriteU32(AgentBeacon.PosX, (uint)x);
            _memory.WriteU32(AgentBeacon.PosY, (uint)y);
            _memory.WriteU32(AgentBeacon.PosZ, (uint)z);
            arrival = $"({x}, {y}, {z})";
        }
        _memory.WriteU8(AreaRequest, (byte)map);
        _memory.WriteU32(AreaRequest + 1, 0);
        _calls.Enqueue((LoadArea, 0, 0));
        return $"warping to map {map} at {arrival}";
    }

    static string Press(string[] words)
    {
        ushort bits = 0;
        foreach (string name in words[1].Split('+'))
        {
            if (!Buttons.TryGetValue(name, out ushort bit)) throw new ArgumentException($"no button '{name}'");
            bits |= bit;
        }
        int hold_ms = words.Length > 2 ? int.Parse(words[2], CultureInfo.InvariantCulture) : DefaultHoldMs;
        _heldBits = bits;
        _heldUntilMs = Environment.TickCount64 + hold_ms;
        return $"pressed {words[1]} for {hold_ms} ms";
    }

    static uint ParseAddress(string text) => uint.Parse(text, NumberStyles.HexNumber, CultureInfo.InvariantCulture);

    static string Peek(string[] words)
    {
        uint address = ParseAddress(words[1]);
        int count = words.Length > 2 ? int.Parse(words[2], CultureInfo.InvariantCulture) : 16;
        if (count is < 1 or > MaxPeekBytes) throw new ArgumentException($"peek takes 1 to {MaxPeekBytes} bytes");

        var hex = new StringBuilder(count * 2);
        for (uint i = 0; i < count; i++) hex.Append(_memory.ReadU8(address + i).ToString("X2"));
        return hex.ToString();
    }

    static string Poke(string[] words)
    {
        uint address = ParseAddress(words[1]);
        byte[] bytes = Convert.FromHexString(words[2]);
        for (uint i = 0; i < bytes.Length; i++) _memory.WriteU8(address + i, bytes[i]);
        return $"wrote {bytes.Length} bytes";
    }

    static string Dump(string[] words)
    {
        const int RamBytes = 0x200000;
        var ram = new byte[RamBytes];
        for (uint i = 0; i < RamBytes; i++) ram[i] = _memory.ReadU8(0x80000000u + i);
        File.WriteAllBytes(words[1], ram);
        return $"wrote {RamBytes} bytes to {words[1]}";
    }
}
