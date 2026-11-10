using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_800176A8</c>: the camera: copies the eye's position (16 bytes at a0, stage 10's first buffer) to 0x8017BE58 and its three angles (a1, the second) to 0x8017BE68, builds the view matrix with RotMatrix into 0x8017BD78, then func_80049CD4 (the X angle, into 0x8017BE38) and func_800498CC (the matrix, into 0x8017BDB8). <c>ST_FRAMESETUP</c>.
/// </summary>
public static class CameraSetup
{
    public static readonly Literal R = new(typeof(CameraSetup), "camera setup", "CAMERASETUP", 0x800176A8, Run,
        "the camera: copies the eye's position (16 bytes at a0, stage 10's first buffer) to 0x8017BE58 and its three angles (a1, the second) to 0x8017BE68, builds the view matrix with RotMatrix into 0x8017BD78, then func_80049CD4 (the X angle, into 0x8017BE38) and func_800498CC (the matrix, into 0x8017BDB8).");

    static void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m) => R.Replace(orig, c, m);

    static void Run(CpuContext c, PSMemory mem)
    {
        c.SP = c.SP - 0x20u;
        c.A3 = c.A0;
        c.T0 = c.A1;
        mem.WriteU32((c.SP + 0x1Cu), c.RA);
        if (c.A3 == 0u) {
            mem.WriteU32((c.SP + 0x18u), c.S0);
            goto L800176F4;
        }
        mem.WriteU32((c.SP + 0x18u), c.S0);
        c.V1 = 0x00010000u;
        c.V1 = c.V1 | 0x8000u;
        c.V0 = 0x80160000u;
        c.V0 = c.V0 - 0x2F70u;
        c.V0 = c.V0 + c.V1;
        c.V1 = mem.ReadU32(c.A3);
        c.A0 = mem.ReadU32((c.A3 + 0x4u));
        c.A1 = mem.ReadU32((c.A3 + 0x8u));
        c.A2 = mem.ReadU32((c.A3 + 0xCu));
        mem.WriteU32((c.V0 + 0x6DC8u), c.V1);
        mem.WriteU32((c.V0 + 0x6DCCu), c.A0);
        mem.WriteU32((c.V0 + 0x6DD0u), c.A1);
        mem.WriteU32((c.V0 + 0x6DD4u), c.A2);
        L800176F4: ;
        if (c.T0 == 0u) {
            c.V1 = 0x00010000u;
            goto L8001772C;
        }
        c.V1 = 0x00010000u;
        c.V1 = c.V1 | 0x8000u;
        c.V0 = 0x80160000u;
        c.V0 = c.V0 - 0x2F70u;
        c.V0 = c.V0 + c.V1;
        c.V1 = mem.ReadWordLeft(c.V1, (c.T0 + 0x3u));
        c.V1 = mem.ReadWordRight(c.V1, c.T0);
        c.A0 = mem.ReadWordLeft(c.A0, (c.T0 + 0x7u));
        c.A0 = mem.ReadWordRight(c.A0, (c.T0 + 0x4u));
        mem.WriteWordLeft((c.V0 + 0x6DDBu), c.V1);
        mem.WriteWordRight((c.V0 + 0x6DD8u), c.V1);
        mem.WriteWordLeft((c.V0 + 0x6DDFu), c.A0);
        mem.WriteWordRight((c.V0 + 0x6DDCu), c.A0);
        L8001772C: ;
        c.V1 = 0x00010000u;
        c.V1 = c.V1 | 0x8000u;
        c.A0 = c.SP + 0x10u;
        c.V0 = 0x80160000u;
        c.V0 = c.V0 - 0x2F70u;
        c.V0 = c.V0 + c.V1;
        c.S0 = 0x80180000u;
        c.S0 = c.S0 - 0x4288u;
        c.V1 = mem.ReadU16((c.V0 + 0x6DD8u));
        c.A2 = mem.ReadU16((c.V0 + 0x6DDCu));
        c.V0 = mem.ReadU16((c.V0 + 0x6DDAu));
        c.A1 = c.S0;
        mem.WriteU16((c.SP + 0x10u), (ushort)c.V1);
        mem.WriteU16((c.SP + 0x14u), (ushort)c.A2);
        mem.WriteU16((c.SP + 0x12u), (ushort)c.V0);
        c.RA = 0x8001776Cu;
        ShadowTower_st.RotMatrix_st(c, mem);
        c.A1 = 0x80180000u;
        c.A0 = (uint)(short)mem.ReadU16((c.SP + 0x10u));
        c.A1 = c.A1 - 0x41C8u;
        c.RA = 0x8001777Cu;
        ShadowTower_st.func_80049CD4(c, mem);
        c.A0 = c.S0;
        c.A1 = 0x80180000u;
        c.A1 = c.A1 - 0x4248u;
        c.RA = 0x8001778Cu;
        ShadowTower_st.func_800498CC(c, mem);
        c.RA = mem.ReadU32((c.SP + 0x1Cu));
        c.S0 = mem.ReadU32((c.SP + 0x18u));
        c.SP = c.SP + 0x20u;
        return;
    }
}
