using RecompOne.Runtime;
using RecompOne.Runtime.Memory;

namespace St;

/// <summary>
/// Blended faces drawn by the backend where the table walk would have drawn their
/// packets (<c>RetainedScene.AddBlendFace</c>): each at the table entry the game links
/// it at, worked out from the corner depths the game projected the same frame, so the
/// order among the frame's other packets is the table's. The walk passes this game's
/// 0x1000-entry table from its far end, and the backend's slot test reads a key
/// <c>K</c> as the entry <c>0x1FFF - K</c> from that end (Verdite3's 0x2000 table), so
/// entry <c>k</c> here is key <c>k + 0x1000</c>. Faces the game drops by key are not
/// added. A blended face whose record or caller says so; a caller's forced blend makes
/// every face of the part one. See "Blended faces" in docs/GPU_RENDERER.md.
/// </summary>
static class RetainedBlend
{
    const int KeyOffset = 0x1000;

    /// <summary>How the game keys a face from its corners' depths.</summary>
    public enum Keying { Deepest, Mean }

    public static bool Enabled = Environment.GetEnvironmentVariable("ST_GPU_BLEND") != "0";
    public static long Faces, Dropped;

    /// <summary>The faces of the instance just added that are blended (all of them when
    /// <paramref name="forced"/>), at their keys: the depth from the corner buffer at
    /// <paramref name="corners"/> (8 bytes a vertex, SZ at +4) by <paramref name="keying"/>,
    /// shifted right 5, plus the record's bias at +0xA; kept when lo &lt; key &lt; hi.
    /// The mode is the record's page's, or-ed with <paramref name="modeBits"/>.</summary>
    public static void Add(IMemory m, RetainedMeshes.Mesh mesh, ReadOnlySpan<RetainedMeshes.Run> runs, uint corners,
                           Keying keying, int lo, int hi, bool forced, int modeBits)
    {
        foreach (var f in mesh.FaceList)
        {
            if (!f.Semi && !forced) continue;
            var run = runs[f.Run];
            uint rec = run.First + (uint)f.Index * run.Shape.Size;
            int deepest = 0, sum = 0;
            foreach (uint at in run.Shape.CornersAt)
            {
                int sz = m.ReadU16(corners + m.ReadU16(rec + at) + 4);
                deepest = Math.Max(deepest, sz);
                sum += sz;
            }
            int depth = keying == Keying.Deepest ? deepest
                      : run.Shape.Corners == 4 ? sum >> 2 : sum / 3;
            int key = (depth >> 5) + (sbyte)m.ReadU8(rec + 0xA);
            if (key <= lo || key >= hi) { Dropped++; continue; }
            int mode = (int)((m.ReadU32(rec + 4) >> 21) & 3) | modeBits;
            RetainedScene.AddBlendFace(key + KeyOffset, f.Corner, f.Corners, mode & 3, -1024, -1024, 1023, 1023);
            Faces++;
        }
    }
}
