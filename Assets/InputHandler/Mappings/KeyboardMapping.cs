using System.Collections.Generic;
using UnityEngine;

internal struct SpecificKeyValue
{
    public string VirtualButton;
    public float NegativePositiveMultiplier;
}

public class KeyboardMapping : InputMapping
{
    private Dictionary<GamepadAxis, SpecificKeyValue[]> axisOverrides = new Dictionary<GamepadAxis, SpecificKeyValue[]>();

    public KeyboardMapping()
    {
        this.OverridesAxisReading = true;
    }

    public override List<string> GetControllerAliasses()
    {
        return new List<string>();
    }

    public override void MapBindings(int deviceNumber)
    {

        //Buttons
        this.ButtonBindingLookupTable[GamepadButton.RightBumper] = "mouse 0";
        this.ButtonBindingLookupTable[GamepadButton.LeftBumper] = "space";
        this.ButtonBindingLookupTable[GamepadButton.RightStickButton] = "left ctrl";
        this.ButtonBindingLookupTable[GamepadButton.ActionSouth] = "space";
        this.ButtonBindingLookupTable[GamepadButton.ActionWest] = "return";


        //Axis (On Keyboard this doesn't really exist, so I'll have to "fake" it)
        this.axisOverrides[GamepadAxis.LeftHorizontal] = new SpecificKeyValue[] { new SpecificKeyValue() { VirtualButton = "a", NegativePositiveMultiplier = -1 }, new SpecificKeyValue() { VirtualButton = "d", NegativePositiveMultiplier = 1 } };
        this.axisOverrides[GamepadAxis.LeftVertical] = new SpecificKeyValue[] { new SpecificKeyValue() { VirtualButton = "s", NegativePositiveMultiplier = -1 }, new SpecificKeyValue() { VirtualButton = "w", NegativePositiveMultiplier = 1 } };
        this.axisOverrides[GamepadAxis.LeftTrigger] = new SpecificKeyValue[] { new SpecificKeyValue() { VirtualButton = "left shift", NegativePositiveMultiplier = 1 } };
        this.axisOverrides[GamepadAxis.RightTrigger] = new SpecificKeyValue[] { new SpecificKeyValue() { VirtualButton = "mouse 1", NegativePositiveMultiplier = 1 } };

        this.axisOverrides[GamepadAxis.DPADHorizontal] = new SpecificKeyValue[] { new SpecificKeyValue() { VirtualButton = "a", NegativePositiveMultiplier = -1 }, new SpecificKeyValue() { VirtualButton = "d", NegativePositiveMultiplier = 1 } };
        this.axisOverrides[GamepadAxis.DPADVertical] = new SpecificKeyValue[] { new SpecificKeyValue() { VirtualButton = "s", NegativePositiveMultiplier = -1 }, new SpecificKeyValue() { VirtualButton = "w", NegativePositiveMultiplier = 1 } };

        //TODO somehow do the mouse, good luck.
    }

    public override float OverrideAxisReading(GamepadAxis axis)
    {
        if (axis == GamepadAxis.RightHorizontal)
        {
            RaycastHit mouseHit;
            RaycastHit centerHit;
            Ray rayMouse = Camera.main.ScreenPointToRay(Input.mousePosition);
            Ray rayCenter = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

            if (Physics.Raycast(rayMouse, out mouseHit, Mathf.Infinity, 1 << 8) && Physics.Raycast(rayCenter, out centerHit, Mathf.Infinity, 1 << 8))
            {
                return mouseHit.point.x - centerHit.point.x;
            }
            return 0.0f;
        }
        else if (axis == GamepadAxis.RightVertical)
        {
            RaycastHit mouseHit;
            RaycastHit centerHit;
            Ray rayMouse = Camera.main.ScreenPointToRay(Input.mousePosition);
            Ray rayCenter = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

            if (Physics.Raycast(rayMouse, out mouseHit, Mathf.Infinity, 1 << 8) && Physics.Raycast(rayCenter, out centerHit, Mathf.Infinity, 1 << 8))
            {
                return mouseHit.point.z - centerHit.point.z;
            }
            return 0.0f;
        }
        else
        {
            float floatToReturn = 0.0f;
            foreach (var key in this.axisOverrides[axis])
            {
                floatToReturn += Input.GetKey(key.VirtualButton) ? 1.0f * key.NegativePositiveMultiplier : 0.0f;
            }
            return floatToReturn;
        }
    }
}