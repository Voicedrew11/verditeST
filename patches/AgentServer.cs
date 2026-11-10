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
    static readonly Queue<(uint Address, uint[] Args)> _calls = new();
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
            Warp.Settle(e.Memory);
            while (_calls.TryDequeue(out var call)) RunCall(e, call);
        }
        finally
        {
            _calling = false;
        }
    }

    // A0 to A3 from the first four arguments, the rest on the stack from SP+0x10, as
    // the MIPS convention places them; the frame is below the game's own.
    static void RunCall(VSyncEvent e, (uint Address, uint[] Args) call)
    {
        var saved = e.Context.Snapshot();
        uint[] a = call.Args;
        e.Context.A0 = a.Length > 0 ? a[0] : 0;
        e.Context.A1 = a.Length > 1 ? a[1] : 0;
        e.Context.A2 = a.Length > 2 ? a[2] : 0;
        e.Context.A3 = a.Length > 3 ? a[3] : 0;
        e.Context.SP -= 0x80;
        for (int i = 4; i < a.Length; i++) e.Memory.WriteU32(e.Context.SP + 0x10 + (uint)(i - 4) * 4, a[i]);
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
        "warp" => WarpTo(words),
        "press" => Press(words),
        "peek" => Peek(words),
        "poke" => Poke(words),
        "dump" => Dump(words),
        "call" => QueueCall(words),
        _ => "error: commands are state, press <a+b> [ms], warp <map> [x y z], peek <addr> [bytes], poke <addr> <hex>, dump <file>, call <addr> [a0 a1 a2 a3 stack...]",
    };

    static string QueueCall(string[] words)
    {
        uint address = ParseAddress(words[1]);
        uint[] args = words.Skip(2).Select(ParseAddress).ToArray();
        _calls.Enqueue((address, args));
        return $"queued {address:X8}({string.Join(", ", args.Select(a => a.ToString("X")))})";
    }

    // warp <map> [x y z]: patches/Warp.cs.
    static string WarpTo(string[] words)
    {
        var (reply, request) = Warp.Start(words, _memory);
        if (request != null) _calls.Enqueue((Warp.RequestArea, request));
        return reply;
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
