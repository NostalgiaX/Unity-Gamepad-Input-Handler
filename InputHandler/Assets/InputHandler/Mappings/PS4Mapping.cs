using System.Collections.Generic;

public class PS4Mapping : InputMapping
{
    public override List<string> GetControllerAliasses()
    {
        return new List<string>() { "Wireless Controller" };
    }

    public override void MapBindings(int deviceNumber)
    {
        this.ButtonBindingLookupTable[GamepadButton.RightBumper] = "joystick " + deviceNumber + " button 5";
        this.ButtonBindingLookupTable[GamepadButton.LeftBumper] = "joystick " + deviceNumber + " button 4";
        this.ButtonBindingLookupTable[GamepadButton.RightStickButton] = "joystick " + deviceNumber + " button 11";

        this.ButtonBindingLookupTable[GamepadButton.ActionSouth] = "joystick " + deviceNumber + " button 1";
        this.ButtonBindingLookupTable[GamepadButton.ActionWest] = "joystick " + deviceNumber + " button 0";
        this.ButtonBindingLookupTable[GamepadButton.ActionEast] = "joystick " + deviceNumber + " button 2";
        this.ButtonBindingLookupTable[GamepadButton.ActionNorth] = "joystick " + deviceNumber + " button 3";
        this.ButtonBindingLookupTable[GamepadButton.Start] = "joystick " + deviceNumber + " button 9";
        this.ButtonBindingLookupTable[GamepadButton.BackSelect] = "joystick " + deviceNumber + " button 8";



        this.AxisBindingLookupTable[GamepadAxis.LeftHorizontal] = new GamepadAxisInfo() { AxisName = "joystick " + deviceNumber + " analog 0", Minimum = -1.0f, Maximum = 1.0f, DeadZoneOffset = 0.1f };
        this.AxisBindingLookupTable[GamepadAxis.LeftVertical] = new GamepadAxisInfo() { AxisName = "joystick " + deviceNumber + " analog 1", Minimum = -1.0f, Maximum = 1.0f, Inverted = true, DeadZoneOffset = 0.1f };
        this.AxisBindingLookupTable[GamepadAxis.RightHorizontal] = new GamepadAxisInfo() { AxisName = "joystick " + deviceNumber + " analog 2", Minimum = -1.0f, Maximum = 1.0f, DeadZoneOffset = 0.1f };
        this.AxisBindingLookupTable[GamepadAxis.RightVertical] = new GamepadAxisInfo() { AxisName = "joystick " + deviceNumber + " analog 5", Minimum = -1.0f, Maximum = 1.0f, Inverted = true, DeadZoneOffset = 0.1f };

        this.AxisBindingLookupTable[GamepadAxis.LeftTrigger] = new GamepadAxisInfo() { AxisName = "joystick " + deviceNumber + " analog 3", Minimum = -1.0f, Maximum = 1.0f, DeadZoneOffset = 0.3f, UnpressedValue = -1 };
        this.AxisBindingLookupTable[GamepadAxis.RightTrigger] = new GamepadAxisInfo() { AxisName = "joystick " + deviceNumber + " analog 4", Minimum = -1.0f, Maximum = 1.0f, DeadZoneOffset = 0.3f, UnpressedValue = -1 };

        this.AxisBindingLookupTable[GamepadAxis.DPADHorizontal] = new GamepadAxisInfo() { AxisName = "joystick " + deviceNumber + " analog 6", Minimum = -1.0f, Maximum = 1.0f, DeadZoneOffset = 0.3f, UnpressedValue = 0 };
        this.AxisBindingLookupTable[GamepadAxis.DPADVertical] = new GamepadAxisInfo() { AxisName = "joystick " + deviceNumber + " analog 7", Minimum = -1.0f, Maximum = 1.0f, DeadZoneOffset = 0.3f, UnpressedValue = 0 };
    }
}