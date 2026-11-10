using RecompOne.Runtime.Context;
using RecompOne.Runtime.Memory;
using RecompOne.Runtime.Dispatch;
using Recompiled;
using static St.Mips;

namespace St;

/// <summary>
/// <c>func_8001EEAC</c>: the scene's last step; wrote no packet in map 0. <c>ST_SCENESTEP1EEAC</c>.
/// </summary>
public static partial class SceneStep1EEAC
{
    public static readonly Literal R = new(typeof(SceneStep1EEAC), "scene step 1EEAC", "SCENESTEP1EEAC", 0x8001EEAC, Run,
        "the scene's last step; wrote no packet in map 0.");

    static void Replace(Action<CpuContext, IMemory> orig, CpuContext c, IMemory m) => R.Replace(orig, c, m);

    static void Run(CpuContext c, PSMemory mem)
    {
        AfterEntry(c, mem);
        if (c.V0 == 0u) {
            c.S0 = 0x1F800000u;
            goto L8001EF20;
        }
        c.S0 = 0x1F800000u;
        c.V0 = 0x80020000u;
        c.V0 = c.V0 - 0x11D8u;
        goto L8001EF30;
        L8001EF20: ;
        c.V0 = c.V1 & 0x0004u;
        if (c.V0 == 0u) {
            c.V0 = 0x80020000u;
            goto L8001EF34;
        }
        c.V0 = 0x80020000u;
        c.V0 = c.V0 - 0x1198u;
        L8001EF30: ;
        mem.WriteU32((c.SP + 0x34Cu), c.V0);
        L8001EF34: ;
        c.A3 = mem.ReadU32((c.SP + 0x348u));
        c.V0 = 0xFFFFFFFFu;
        c.A3 = c.A3 - 0x1u;
        if (c.A3 == c.V0) {
            mem.WriteU32((c.SP + 0x348u), c.A3);
            goto L8001F920;
        }
        mem.WriteU32((c.SP + 0x348u), c.A3);
        c.T0 = c.SP + 0x34u;
        c.T1 = c.SP + 0x4Cu;
        c.A3 = c.SP + 0x64u;
        mem.WriteU32((c.SP + 0x350u), c.T0);
        mem.WriteU32((c.SP + 0x354u), c.T1);
        mem.WriteU32((c.SP + 0x358u), c.A3);
        L8001EF60: ;
        Poll(c, mem);
        c.V1 = 0x801B0000u;
        c.T0 = mem.ReadU32((c.SP + 0x348u));
        c.V1 = c.V1 + 0x5948u;
        c.V0 = c.T0 << 1;
        c.V0 = c.V0 + c.V1;
        c.V0 = mem.ReadU16((c.V0 + 0x45CEu));
        c.V1 = c.V0 << 1;
        c.V1 = c.V1 + c.V0;
        c.V1 = c.V1 << 2;
        c.V0 = 0x80180000u;
        c.V0 = c.V0 - 0x4130u;
        c.A0 = c.V1 + c.V0;
        c.V0 = mem.ReadU8((c.A0 + 0x4u));
        c.V0 = c.V0 & 0x0003u;
        if (c.V0 != 0u) {
            goto L8001EFD8;
        }
        c.V1 = mem.ReadU8(c.A0);
        c.V0 = c.V1 << 2;
        c.V0 = c.V0 + c.V1;
        c.V0 = c.V0 << 3;
        c.V1 = mem.ReadU32((c.S0 + 0xCu));
        c.V0 = c.V0 + 0xCu;
        c.V1 = c.V1 + c.V0;
        mem.WriteU32((c.S0 + 0x20u), c.V1);
        c.V0 = mem.ReadU32((c.V1 + 0x4u));
        goto L8001F8FC;
        L8001EFD8: ;
        After8001EFD8(c, mem);
        if (c.V1 == c.V0) {
            goto L8001F8F0;
        }
        c.S1 = c.S4 + 0x1Cu;
        L8001F0F4: ;
        Poll(c, mem);
        After8001F0F4(c, mem);
        if ((int)c.V0 > 0) {
            goto L8001F1B0;
        }
        c.V0 = mem.ReadU32((c.S0 + 0x74u));
        if ((int)c.V0 >= 0) {
            goto L8001F8D4;
        }
        L8001F1B0: ;
        c.V0 = mem.ReadU32((c.S0 + 0x10u));
        c.V0 = c.V0 + 0x2Cu;
        mem.WriteU32(c.V0, (RecompOne.Runtime.Gte.Read(12)));
        c.V0 = (uint)(short)mem.ReadU16((c.S7 + 0x4u));
        mem.WriteU32((c.S0 + 0x84u), c.V0);
        mem.WriteU32((c.S0 + 0x80u), c.V0);
        c.V1 = (uint)(short)mem.ReadU16((c.S5 + 0x4u));
        c.V0 = (int)c.V0 < (int)c.V1 ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L8001F1EC;
        }
        mem.WriteU32((c.S0 + 0x84u), c.V1);
        c.V1 = (uint)(short)mem.ReadU16((c.S5 + 0x4u));
        L8001F1EC: ;
        c.V0 = mem.ReadU32((c.S0 + 0x80u));
        c.V0 = (int)c.V1 < (int)c.V0 ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L8001F204;
        }
        mem.WriteU32((c.S0 + 0x80u), c.V1);
        L8001F204: ;
        c.V1 = (uint)(short)mem.ReadU16((c.S6 + 0x4u));
        c.V0 = mem.ReadU32((c.S0 + 0x84u));
        c.V0 = (int)c.V0 < (int)c.V1 ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L8001F224;
        }
        mem.WriteU32((c.S0 + 0x84u), c.V1);
        c.V1 = (uint)(short)mem.ReadU16((c.S6 + 0x4u));
        L8001F224: ;
        c.V0 = mem.ReadU32((c.S0 + 0x80u));
        c.V0 = (int)c.V1 < (int)c.V0 ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L8001F23C;
        }
        mem.WriteU32((c.S0 + 0x80u), c.V1);
        L8001F23C: ;
        c.V1 = (uint)(short)mem.ReadU16((c.FP + 0x4u));
        c.V0 = mem.ReadU32((c.S0 + 0x84u));
        c.V0 = (int)c.V0 < (int)c.V1 ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L8001F25C;
        }
        mem.WriteU32((c.S0 + 0x84u), c.V1);
        c.V1 = (uint)(short)mem.ReadU16((c.FP + 0x4u));
        L8001F25C: ;
        c.V0 = mem.ReadU32((c.S0 + 0x80u));
        c.V0 = (int)c.V1 < (int)c.V0 ? 1u : 0u;
        if (c.V0 == 0u) {
            goto L8001F274;
        }
        mem.WriteU32((c.S0 + 0x80u), c.V1);
        L8001F274: ;
        c.V0 = mem.ReadU32((c.S0 + 0x84u));
        c.V1 = (uint)(sbyte)mem.ReadU8((c.S1 - 0x12u));
        c.V0 = (uint)((int)c.V0 >> 5);
        c.V1 = c.V0 + c.V1;
        c.V0 = (int)c.V1 < 4096 ? 1u : 0u;
        if (c.V0 == 0u) {
            mem.WriteU32((c.S0 + 0x88u), c.V1);
            goto L8001F8D4;
        }
        mem.WriteU32((c.S0 + 0x88u), c.V1);
        c.V0 = mem.ReadU32((c.S0 + 0x8Cu));
        c.V0 = (int)c.V1 < (int)c.V0 ? 1u : 0u;
        if (c.V0 != 0u) {
            goto L8001F8D4;
        }
        c.V0 = mem.ReadU32((c.S0 + 0x70u));
        c.V0 = (int)c.V0 < 16385 ? 1u : 0u;
        if (c.V0 == 0u) {
            c.V0 = 0x00000001u;
            goto L8001F2CC;
        }
        c.V0 = 0x00000001u;
        c.V0 = mem.ReadU32((c.S0 + 0x74u));
        c.V0 = (int)c.V0 < -16384 ? 1u : 0u;
        if (c.V0 == 0u) {
            c.V0 = 0x00000001u;
            goto L8001F6F8;
        }
        c.V0 = 0x00000001u;
        L8001F2CC: ;
        mem.WriteU32((c.SP + 0x10u), c.V0);
        c.V1 = mem.ReadU32((c.S0 + 0x70u));
        c.V0 = 0u | 0x8000u;
        c.V0 = (int)c.V0 < (int)c.V1 ? 1u : 0u;
        if (c.V0 != 0u) {
            c.V0 = 0x00000002u;
            goto L8001F2F8;
        }
        c.V0 = 0x00000002u;
        c.V0 = mem.ReadU32((c.S0 + 0x74u));
        c.V0 = (int)c.V0 < -32768 ? 1u : 0u;
        if (c.V0 == 0u) {
            c.V0 = 0x00000002u;
            goto L8001F328;
        }
        c.V0 = 0x00000002u;
        L8001F2F8: ;
        mem.WriteU32((c.SP + 0x10u), c.V0);
        c.V1 = mem.ReadU32((c.S0 + 0x70u));
        c.V0 = 0x00010000u;
        c.V0 = (int)c.V0 < (int)c.V1 ? 1u : 0u;
        if (c.V0 != 0u) {
            c.V0 = 0x00000003u;
            goto L8001F324;
        }
        c.V0 = 0x00000003u;
        c.V0 = mem.ReadU32((c.S0 + 0x74u));
        c.V1 = 0xFFFF0000u;
        c.V0 = (int)c.V0 < (int)c.V1 ? 1u : 0u;
        if (c.V0 == 0u) {
            c.V0 = 0x00000003u;
            goto L8001F328;
        }
        c.V0 = 0x00000003u;
        L8001F324: ;
        mem.WriteU32((c.SP + 0x10u), c.V0);
        L8001F328: ;
        After8001F328_2(c, mem);
        if (c.V0 == 0u) {
            goto L8001F4B8;
        }
        After8001F328(c, mem);
        L8001F4B8: ;
        After8001F4B8(c, mem);
        goto L8001F8DC;
        L8001F6F8: ;
        c.A1 = mem.ReadU32((c.S0 + 0x10u));
        c.V0 = mem.ReadU32((c.S0 + 0x14u));
        c.V1 = c.A1 + 0x34u;
        c.V0 = c.V0 < c.V1 ? 1u : 0u;
        if (c.V0 != 0u) {
            goto L8001F920;
        }
        After8001F6F8(c, mem);
        if (c.V0 == 0u) {
            goto L8001F8D4;
        }
        c.T1 = mem.ReadU32((c.SP + 0x34Cu));
        c.A0 = c.A2;
        c.RA = 0x8001F8A4u;
        Dispatcher.Call(c, mem, c.T1);
        c.A3 = mem.ReadU32((c.SP + 0x34Cu));
        c.A0 = c.S5;
        c.RA = 0x8001F8B4u;
        Dispatcher.Call(c, mem, c.A3);
        c.T0 = mem.ReadU32((c.SP + 0x34Cu));
        c.A0 = c.S3;
        c.RA = 0x8001F8C4u;
        Dispatcher.Call(c, mem, c.T0);
        c.T1 = mem.ReadU32((c.SP + 0x34Cu));
        c.A0 = c.S2;
        c.RA = 0x8001F8D4u;
        Dispatcher.Call(c, mem, c.T1);
        L8001F8D4: ;
        c.S1 = c.S1 + 0x20u;
        c.S4 = c.S4 + 0x20u;
        L8001F8DC: ;
        c.V1 = mem.ReadU32((c.S0 + 0x184u));
        c.V0 = 0xFFFFFFFFu;
        c.V1 = c.V1 - 0x1u;
        if (c.V1 != c.V0) {
            mem.WriteU32((c.S0 + 0x184u), c.V1);
            goto L8001F0F4;
        }
        mem.WriteU32((c.S0 + 0x184u), c.V1);
        L8001F8F0: ;
        c.V0 = mem.ReadU32((c.S0 + 0x20u));
        c.V0 = mem.ReadU32((c.V0 + 0x4u));
        L8001F8FC: ;
        c.V1 = mem.ReadU32((c.S0 + 0x34u));
        c.V0 = c.V0 << 3;
        c.V1 = c.V1 + c.V0;
        mem.WriteU32((c.S0 + 0x34u), c.V1);
        c.A3 = mem.ReadU32((c.SP + 0x348u));
        c.V0 = 0xFFFFFFFFu;
        c.A3 = c.A3 - 0x1u;
        if (c.A3 != c.V0) {
            mem.WriteU32((c.SP + 0x348u), c.A3);
            goto L8001EF60;
        }
        mem.WriteU32((c.SP + 0x348u), c.A3);
        L8001F920: ;
        c.RA = mem.ReadU32((c.SP + 0x384u));
        c.FP = mem.ReadU32((c.SP + 0x380u));
        c.S7 = mem.ReadU32((c.SP + 0x37Cu));
        c.S6 = mem.ReadU32((c.SP + 0x378u));
        c.S5 = mem.ReadU32((c.SP + 0x374u));
        c.S4 = mem.ReadU32((c.SP + 0x370u));
        c.S3 = mem.ReadU32((c.SP + 0x36Cu));
        c.S2 = mem.ReadU32((c.SP + 0x368u));
        c.S1 = mem.ReadU32((c.SP + 0x364u));
        c.S0 = mem.ReadU32((c.SP + 0x360u));
        c.SP = c.SP + 0x388u;
        return;
    }
}
