namespace St;

/// <summary>
/// ST_MOUSE_PROBE=1: what <see cref="MouseLook"/> asked of the turn and look routine
/// and what the base angles did, once a second. A driven tick lands when the angle
/// moved by exactly the step (pitch: or stopped at its limit, 0x2BC or 0xD44); an
/// undriven tick that still moved the angle is coasting, which nobody at the pad
/// should see after the hand stops. See "Mouse look" in docs/INPUT.md.
/// </summary>
static class MouseLookProbe
{
    static long _ticks, _yawDriven, _yawLanded, _pitchDriven, _pitchLanded, _pitchAtLimit, _coasting;
    static int _lastYaw, _lastPitch;
    static double _since = -1.0;

    public static void Tick(int yaw, int pitch, int yawMoved, int pitchMoved, int pitchNow)
    {
        _ticks++;
        if (yaw != 0) { _yawDriven++; if (yawMoved == yaw) _yawLanded++; }
        if (pitch != 0)
        {
            _pitchDriven++;
            bool atLimit = pitchNow is 0x2BC or 0xD44;
            if (pitchMoved == pitch) _pitchLanded++;
            else if (atLimit) _pitchAtLimit++;
        }
        if (yaw == 0 && pitch == 0 && (yawMoved != 0 || pitchMoved != 0)) _coasting++;
        _lastYaw = yawMoved;
        _lastPitch = pitchMoved;

        double now = Environment.TickCount64 / 1000.0;
        if (_since < 0.0) _since = now;
        if (now - _since < 1.0) return;
        Console.WriteLine($"[ST] mouse look: {_ticks} tick(s); yaw driven {_yawDriven}, landed {_yawLanded}; " +
                          $"pitch driven {_pitchDriven}, landed {_pitchLanded}, held at the limit {_pitchAtLimit} " +
                          $"(now 0x{pitchNow:X3}); undriven but moving {_coasting}; last step {_lastYaw},{_lastPitch}");
        Console.Out.Flush();
        _since = now;
        _ticks = _yawDriven = _yawLanded = _pitchDriven = _pitchLanded = _pitchAtLimit = _coasting = 0;
    }
}
