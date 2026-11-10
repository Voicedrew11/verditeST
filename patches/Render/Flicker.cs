using RecompOne.Runtime;

namespace St;

/// <summary>
/// The flickering light's colour (<c>0x8017BE8C &amp; 6</c>, maps 24 and 32): after
/// the fog, every fogged corner of the map and of the models is made from one channel
/// (<see cref="MapLight.Flicker"/>, <c>func_8001DAE0</c>/<c>8001DB20</c> and the models'
/// copies): with <c>&amp; 2</c> the blue held under 0x56 and the colour made
/// <c>(3b, 2b, b)</c>, warm; else with <c>&amp; 4</c> the red held and the colour made
/// <c>(r, 3r, 3r)</c>, cold. On the GPU, runtime 0124's colour rule. The random word
/// that sets the light's level each frame is <see cref="Word"/>.
/// </summary>
static class Flicker
{
    public const uint Word = 0x8017BE88;

    public static void Recolour(ref RetainedScene.ModelInstance i, int scene)
    {
        if ((scene & 2) != 0) { i.RecolourFrom = 3; i.RecolourCap = 0x55; (i.RecolourR, i.RecolourG, i.RecolourB) = (3, 2, 1); }
        else if ((scene & 4) != 0) { i.RecolourFrom = 1; i.RecolourCap = 0x55; (i.RecolourR, i.RecolourG, i.RecolourB) = (1, 3, 3); }
    }
}
