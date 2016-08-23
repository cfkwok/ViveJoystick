static class SteamVR_Controller_Extension
{
    const float threshold = 0.3f;

    /*
     *  You might expect that pressing one of the edges of the SteamVR controller touchpad could
     *  be detected with a call to device.GetPress( EVRButtonId.k_EButton_DPad_* ), but currently this always returns false.
     *  Not sure whether this is SteamVR's design intent, not yet implemented, or a bug.
     *  The expected behaviour can be achieved by detecting overall Touchpad press, with Touch-Axis comparison to an edge threshold.
     */
    public static bool GetDPadPress(this SteamVR_Controller.Device device, Valve.VR.EVRButtonId dPadButtonId)
    {
        if (device.GetPress(SteamVR_Controller.ButtonMask.Touchpad))  // Is any DPad button pressed?
        {
            var touchpadAxis = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);

            if (touchpadAxis.y > (1.0f - threshold)) { return dPadButtonId == Valve.VR.EVRButtonId.k_EButton_DPad_Up; }
            else if (touchpadAxis.y < threshold) { return dPadButtonId == Valve.VR.EVRButtonId.k_EButton_DPad_Down; }
            else if (touchpadAxis.x > (1.0f - threshold)) { return dPadButtonId == Valve.VR.EVRButtonId.k_EButton_DPad_Right; }
            else if (touchpadAxis.x < threshold) { return dPadButtonId == Valve.VR.EVRButtonId.k_EButton_DPad_Left; }
        }

        return false;
    }
}