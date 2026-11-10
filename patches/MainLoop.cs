namespace St;

/// <summary>
/// ST.EXE's main loop at <c>0x80014BD4</c> (in <c>func_80014A30</c>): fourteen calls
/// an iteration, the last of which builds and draws the frame. One table, read by
/// every patch that needs a stage, so a stage is named once. See "The main loop" in
/// docs/GAME_INTERNALS.md.
/// </summary>
static class MainLoop
{
    /// <summary>A stage: the routine, and the address of the main loop's own
    /// <c>jal</c> to it. A call from anywhere else is not the main loop's.</summary>
    public readonly record struct Stage(int Number, uint Routine, uint CallSite)
    {
        /// <summary>The return address a call from the main loop arrives with.</summary>
        public uint ReturnAddress => CallSite + 8;
    }

    public const string Overlay = "st";

    public static readonly Stage[] Stages =
    [
        new(1, 0x800164B4, 0x80014BD4),   // once a second: the frames drawn in it, to 0x8017BD51
        new(2, 0x80016508, 0x80014BDC),
        new(3, 0x80042D74, 0x80014BE4),   // the menu and its screens run inside it
        new(4, 0x80050768, 0x80014BEC),   // the area's object records, doors among them
        new(5, 0x8004E324, 0x80014BF4),
        new(6, 0x80053AA4, 0x80014BFC),
        new(7, 0x8003BC40, 0x80014C04),
        new(8, 0x8005735C, 0x80014C0C),   // the loop clears player+0x21C just before
        new(9, 0x800456F8, 0x80014C14),   // the area loader's state machine
        new(10, 0x8003E734, 0x80014C20),  // (sp+0x18, sp+0x28)
        new(11, 0x80058288, 0x80014C2C),  // (sp+0x18, sp+0x28)
        new(12, 0x80045FFC, 0x80014C34),  // the streamed-image parser, 16 sectors a call
        new(13, 0x80058620, 0x80014C3C),
        new(14, 0x80026AFC, 0x80014C48),  // (sp+0x18, sp+0x28): builds and draws the frame
    ];

    /// <summary>The stage that draws: everything before it is the world's tick.</summary>
    public static Stage Draw => Stages[^1];

    /// <summary>The first stage, where an iteration starts.</summary>
    public static Stage First => Stages[0];

    /// <summary><c>func_8001535C</c>, called by stage 14 after the present: spins
    /// <c>VSync(0)</c> until the vblank handler has counted one since it last passed,
    /// then zeroes the count. A 60 Hz ceiling on the loop.</summary>
    public const uint FrameGate = 0x8001535C;

    /// <summary>u32, vblanks since the frame gate last passed (<c>func_800153B4</c>, the
    /// vblank handler, bumps it).</summary>
    public const uint GateCount = 0x8008820C;

    /// <summary>u32, the ordering table being built: <c>func_80017634</c> draws it from
    /// <c>+0x3FFC</c>, 0x1000 entries.</summary>
    public const uint OtPointer = 0x80165174;
    public const int OtWords = 0x1000;

    /// <summary>ST.EXE's libetc <c>VSync</c> and libgpu <c>DrawOTag</c>, bound to the
    /// runtime (docs/RECOMPILATION.md).</summary>
    public const uint VSync = 0x80077104;
    public const uint DrawOTag = 0x8007A054;
}
