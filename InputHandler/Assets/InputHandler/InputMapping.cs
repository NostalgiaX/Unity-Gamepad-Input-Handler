using System.Collections.Generic;

public abstract class InputMapping
{
    public Dictionary<GamepadButton, string> ButtonBindingLookupTable = new Dictionary<GamepadButton, string>();
    public Dictionary<GamepadAxis, GamepadAxisInfo> AxisBindingLookupTable = new Dictionary<GamepadAxis, GamepadAxisInfo>();
    //protected int InputNumber = 1;
    public int OriginalIndex;
    public bool IsDisconnected = false;
    public bool OverridesAxisReading = false;

    public abstract void MapBindings(int deviceNumber);

    public abstract List<string> GetControllerAliasses();

    public virtual float OverrideAxisReading(GamepadAxis axis)
    {
        return 0.0f;
    }
}