using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum GamepadButton
{
    ActionSouth = 0,
    ActionEast = 1,
    ActionWest = 2,
    ActionNorth = 3,
    LeftBumper = 4,
    RightBumper = 5,
    BackSelect = 6,
    Start = 7,
    LeftStickButton = 8,
    RightStickButton = 9
}

public enum GamepadAxis
{
    LeftHorizontal = -2,
    LeftVertical = -1,
    RightHorizontal = 4,
    RightVertical = 5,
    DPADHorizontal = 6,
    DPADVertical = 7,
    LeftTrigger = 9,
    RightTrigger = 10
}

public enum PositiveNegativeAxis
{
    Indifferent = 0,
    Negative = -1,
    Positive = 1
}

public struct GamepadAxisInfo
{
    public string AxisName;
    public bool Inverted;
    public float Minimum;
    public float Maximum;
    public float DeadZoneOffset;
    public float UnpressedValue;
}

public class AxisState
{
    public InputMapping BelongingMapping;
    public GamepadAxis Axis;
    public bool PressedLastFrame;
    public bool PressedInCurrentFrame;
    public PositiveNegativeAxis DoesAxisMatter;
}

public class InputHandler : MonoBehaviour
{
    public List<InputMapping> PlayerMappings = new List<InputMapping>();
    public Dictionary<string, Type> NameToInputMappingLookupTable = new Dictionary<string, Type>();
    private List<AxisState> AxisToButtonStates = new List<AxisState>();
    public bool HasUpdatedFrame = false;
    public float CheckForNewControllerTimer = 5.0f;
    public static InputHandler Instance;
    public event Action<int> OnNewControllerConnected;
    public event Action<int> OnControllerDisconnected;

    private void Awake()
    {
        // if the singleton hasn't been initialized yet
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;//Avoid doing anything else
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private List<Type> SupportedInputMappings = new List<Type>()
    {
        typeof(PS4Mapping), typeof(Xbox360Mapping), typeof(XboxOneMapping), typeof(SwitchProControllerMapping)
    };

    private void Start()
    {
        KeyboardMapping StandardKeyboardMapping = new KeyboardMapping();
        StandardKeyboardMapping.MapBindings(-1);
        StandardKeyboardMapping.OriginalIndex = -1;
        this.PlayerMappings.Add(StandardKeyboardMapping);
        this.FillNameToInputMappingLookupTable();

        var devices = Input.GetJoystickNames();

        for (int i = 0; i < devices.Length; i++)
        {
            if (!string.IsNullOrEmpty(devices[i]))
            {
                Type typeofInput;
                if (this.NameToInputMappingLookupTable.TryGetValue(devices[i], out typeofInput))
                {
                    InputMapping instance = (InputMapping)Activator.CreateInstance(typeofInput);
                    instance.OriginalIndex = i;

                    instance.MapBindings(i + 1);
                    this.PlayerMappings.Add(instance);
                }
                else
                {
                    Debug.Log("Unknown controller device, pray it works as Xbox input");
                    Xbox360Mapping mapping = new Xbox360Mapping();
                    mapping.OriginalIndex = i;
                    mapping.MapBindings(i + 1);
                    this.PlayerMappings.Add(mapping);
                }
            }
        }

        this.StartCoroutine(this.CheckForNewControllersCoroutine());
        this.StartCoroutine(this.CheckForControllerDC());
    }

    public IEnumerator CheckForNewControllersCoroutine()
    {
        while (true)
        {
            var devices = Input.GetJoystickNames();

            for (int i = 0; i < devices.Length; i++)
            {
                bool alreadyExists = false;
                //Check that the old mappings aren't made again due to a shift in the devices.
                for (int x = 1; x < this.PlayerMappings.Count; x++)
                {
                    if (this.PlayerMappings[x].OriginalIndex == i)
                    {
                        alreadyExists = true;
                    }
                }
                if (alreadyExists) continue;

                if (!string.IsNullOrEmpty(devices[i]))
                {
                    Type typeofInput;
                    if (this.NameToInputMappingLookupTable.TryGetValue(devices[i], out typeofInput))
                    {
                        InputMapping instance = (InputMapping)Activator.CreateInstance(typeofInput);
                        instance.OriginalIndex = i;
                        instance.MapBindings(i + 1);
                        this.PlayerMappings.Add(instance);
                    }
                    else
                    {
                        Debug.Log("Unknown controller device, pray it works as Xbox input");
                        Xbox360Mapping mapping = new Xbox360Mapping();
                        mapping.OriginalIndex = i;
                        mapping.MapBindings(i + 1);
                        this.PlayerMappings.Add(mapping);
                    }
                    if (OnNewControllerConnected != null)
                    {
                        OnNewControllerConnected.Invoke(this.PlayerMappings.Count - 1);
                    }
                }
            }
            yield return new WaitForSeconds(this.CheckForNewControllerTimer);
        }
    }

    public IEnumerator CheckForControllerDC()
    {
        while (true)
        {
            var devices = Input.GetJoystickNames();

            //This assumes that everything that is detected by Unity, stays seen by GetJoystickNames, either by name or as empty. (Also skip 1 because of keyboard)
            for (int i = 1; i < this.PlayerMappings.Count; i++)
            {
                if (devices.Length >= this.PlayerMappings[i].OriginalIndex)
                {
                    if (string.IsNullOrEmpty(devices[this.PlayerMappings[i].OriginalIndex]))
                    {
                        if (!this.PlayerMappings[i].IsDisconnected)
                        {
                            this.PlayerMappings[i].IsDisconnected = true;
                            if (OnControllerDisconnected != null)
                            {
                                OnControllerDisconnected.Invoke(i);
                            }
                        }
                    }
                    else
                    {
                        this.PlayerMappings[i].IsDisconnected = false;

                    }
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }


    private void Update()
    {
        if (!this.HasUpdatedFrame)
        {
            this.UpdateStates();
            this.HasUpdatedFrame = true;
        }

    }

    public void UpdateStates()
    {
        foreach (var state in this.AxisToButtonStates)
        {
            float value = this.GetAxisValue(state.Axis, state.BelongingMapping);

            if (state.DoesAxisMatter == PositiveNegativeAxis.Indifferent)
                state.PressedInCurrentFrame = value > 0.01f;
            else
            {
                state.PressedInCurrentFrame = state.DoesAxisMatter == PositiveNegativeAxis.Negative ? value < -0.1f : value > 0.1f;
            }
        }
        this.HasUpdatedFrame = true;
    }

    private void LateUpdate()
    {
        foreach (var state in this.AxisToButtonStates)
        {
            state.PressedLastFrame = state.PressedInCurrentFrame;
        }
        this.HasUpdatedFrame = false;
    }

    #region Regular Buttons

    public bool GetButtonDown(GamepadButton button, int playerNumber)
    {
        return Input.GetKeyDown(this.PlayerMappings[playerNumber].ButtonBindingLookupTable[button]);
    }

    public bool GetButton(GamepadButton button, int playerNumber)
    {
        return Input.GetKey(this.PlayerMappings[playerNumber].ButtonBindingLookupTable[button]);
    }

    public bool GetButtonUp(GamepadButton button, int playerNumber)
    {
        return Input.GetKeyUp(this.PlayerMappings[playerNumber].ButtonBindingLookupTable[button]);
    }

    #endregion Regular Buttons

    /// <summary>
    /// Mimics an axis to act like a button. This is useful for triggers on controllers like PS4 or Xbox, where you want the triggers to be a button, and not an analog value
    /// </summary>
    /// <param name="axis"></param>
    /// <param name="playerNumber"></param>
    /// <returns></returns>
    public bool GetAxisAsButtonDown(GamepadAxis axis, int playerNumber)
    {
        var mapping = this.AxisToButtonStates.Where(state => state.BelongingMapping == this.PlayerMappings[playerNumber] && state.Axis == axis && state.DoesAxisMatter == PositiveNegativeAxis.Indifferent).ToList();
        if (mapping.Count != 0)
        {
            return mapping[0].PressedInCurrentFrame && !mapping[0].PressedLastFrame;
        }
        else
        {
            //Add to list (Basically "subscribe" to it)
            var newAxisState = new AxisState() { BelongingMapping = this.PlayerMappings[playerNumber], Axis = axis };
            if (this.PlayerMappings[playerNumber].OverridesAxisReading)
            {
                float value = this.PlayerMappings[playerNumber].OverrideAxisReading(axis);
                newAxisState.PressedInCurrentFrame = value != 0;
            }
            else
            {
                GamepadAxisInfo info = this.PlayerMappings[playerNumber].AxisBindingLookupTable[axis];
                float value = this.GetAxisValue(axis, playerNumber);

                int multiplier = value >= info.UnpressedValue ? 1 : -1;
                if (multiplier < 0)
                {
                    newAxisState.PressedInCurrentFrame = value < info.UnpressedValue + (multiplier * info.DeadZoneOffset);
                }
                else
                {
                    newAxisState.PressedInCurrentFrame = value > info.UnpressedValue + (multiplier * info.DeadZoneOffset);
                }
            }
            this.AxisToButtonStates.Add(newAxisState);
            return newAxisState.PressedInCurrentFrame;
        }
    }

    /// <summary>
    /// Mostly used for DPAD on controllers, since you want to access each axis, as a button, but internally, it is seen as one axis.
    /// </summary>
    /// <param name="axis"></param>
    /// <param name="playerNumber"></param>
    /// <param name="whichDirection"></param>
    /// <returns></returns>
    public bool GetAxisAsButtonDown(GamepadAxis axis, int playerNumber, PositiveNegativeAxis whichDirection)
    {
        var mapping = this.AxisToButtonStates.Where(state => state.BelongingMapping == this.PlayerMappings[playerNumber] && state.Axis == axis && state.DoesAxisMatter == whichDirection).ToList();
        if (mapping.Count != 0)
        {
            return mapping[0].PressedInCurrentFrame && !mapping[0].PressedLastFrame;
        }
        else
        {
            //Add to list (Basically "subscribe" to it)
            var newAxisState = new AxisState() { BelongingMapping = this.PlayerMappings[playerNumber], Axis = axis, DoesAxisMatter = whichDirection };
            //newAxisState.PressedInCurrentFrame = this.PlayerMappings[playerNumber].OverridesAxisReading ? this.PlayerMappings[playerNumber].OverrideAxisReading(axis) > 0.01f : Input.GetAxisRaw(this.PlayerMappings[playerNumber].AxisBindingLookupTable[axis]) > 0.01f;
            if (this.PlayerMappings[playerNumber].OverridesAxisReading)
            {
                float value = this.PlayerMappings[playerNumber].OverrideAxisReading(axis);
                newAxisState.PressedInCurrentFrame = value != 0;
            }
            else
            {
                GamepadAxisInfo info = this.PlayerMappings[playerNumber].AxisBindingLookupTable[axis];
                float value = this.GetAxisValue(axis, playerNumber);

                int multiplier = whichDirection == PositiveNegativeAxis.Negative ? -1 : 1;
                if (multiplier < 0)
                {
                    newAxisState.PressedInCurrentFrame = value < info.UnpressedValue + (multiplier * info.DeadZoneOffset);
                }
                else
                {
                    newAxisState.PressedInCurrentFrame = value > info.UnpressedValue + (multiplier * info.DeadZoneOffset);
                }
            }
            this.AxisToButtonStates.Add(newAxisState);
            return newAxisState.PressedInCurrentFrame;
        }
    }

    public bool GetAxisAsButton(GamepadAxis axis, int playerNumber)
    {
        float axisVal = this.GetAxisValue(axis, playerNumber);
        return Mathf.Abs(axisVal) > 0.01f;
    }

    public bool GetAxisAsButtonUp(GamepadAxis axis, int playerNumber)
    {
        var mapping = this.AxisToButtonStates.Where(state => state.BelongingMapping == this.PlayerMappings[playerNumber] && state.Axis == axis && state.DoesAxisMatter == PositiveNegativeAxis.Indifferent).ToList();
        if (mapping.Count != 0)
        {
            return !mapping[0].PressedInCurrentFrame && mapping[0].PressedLastFrame;
        }
        else
        {
            //Add to list (Basically "subscribe" to it
            var newAxisState = new AxisState() { BelongingMapping = this.PlayerMappings[playerNumber], Axis = axis };

            if (this.PlayerMappings[playerNumber].OverridesAxisReading)
            {
                float value = this.PlayerMappings[playerNumber].OverrideAxisReading(axis);
                newAxisState.PressedInCurrentFrame = value != 0;
            }
            else
            {
                GamepadAxisInfo info = this.PlayerMappings[playerNumber].AxisBindingLookupTable[axis];
                float value = Input.GetAxisRaw(info.AxisName);

                int multiplier = value >= info.UnpressedValue ? 1 : -1;
                if (multiplier < 0)
                {
                    newAxisState.PressedInCurrentFrame = value < info.UnpressedValue + (multiplier * info.DeadZoneOffset);
                }
                else
                {
                    newAxisState.PressedInCurrentFrame = value > info.UnpressedValue + (multiplier * info.DeadZoneOffset);
                }
            }
            //newAxisState.PressedInCurrentFrame = value > info.UnpressedValue + (multiplier * info.DeadZoneOffset);
            this.AxisToButtonStates.Add(newAxisState);
            return !newAxisState.PressedInCurrentFrame && newAxisState.PressedLastFrame;
        }
    }

    public bool GetAxisAsButtonUp(GamepadAxis axis, int playerNumber, PositiveNegativeAxis whichDirection)
    {
        var mapping = this.AxisToButtonStates.Where(state => state.BelongingMapping == this.PlayerMappings[playerNumber] && state.Axis == axis && state.DoesAxisMatter == whichDirection).ToList();
        if (mapping.Count != 0)
        {
            return !mapping[0].PressedInCurrentFrame && mapping[0].PressedLastFrame;
        }
        else
        {
            //Add to list (Basically "subscribe" to it
            var newAxisState = new AxisState() { BelongingMapping = this.PlayerMappings[playerNumber], Axis = axis, DoesAxisMatter = whichDirection };

            if (this.PlayerMappings[playerNumber].OverridesAxisReading)
            {
                float value = this.PlayerMappings[playerNumber].OverrideAxisReading(axis);
                newAxisState.PressedInCurrentFrame = value != 0;
            }
            else
            {
                GamepadAxisInfo info = this.PlayerMappings[playerNumber].AxisBindingLookupTable[axis];
                float value = Input.GetAxisRaw(info.AxisName);

                int multiplier = whichDirection == PositiveNegativeAxis.Negative ? -1 : 1;
                if (multiplier < 0)
                {
                    newAxisState.PressedInCurrentFrame = value < info.UnpressedValue + (multiplier * info.DeadZoneOffset);
                }
                else
                {
                    newAxisState.PressedInCurrentFrame = value > info.UnpressedValue + (multiplier * info.DeadZoneOffset);
                }
            }
            //newAxisState.PressedInCurrentFrame = value > info.UnpressedValue + (multiplier * info.DeadZoneOffset);
            this.AxisToButtonStates.Add(newAxisState);
            return !newAxisState.PressedInCurrentFrame && newAxisState.PressedLastFrame;
        }
    }

    public float GetAxisValue(GamepadAxis axis, int playerNumber)
    {
        if (this.PlayerMappings[playerNumber].OverridesAxisReading)
        {
            return this.PlayerMappings[playerNumber].OverrideAxisReading(axis);
        }

        float input = Input.GetAxisRaw(this.PlayerMappings[playerNumber].AxisBindingLookupTable[axis].AxisName);
        //int multiplier = input > this.PlayerMappings[playerNumber].AxisBindingLookupTable[axis].UnpressedValue ? 1 : -1;
        //Make sure that we don't get false positives, due to the deadzone there is on buttons
        //bool valueBiggerThanDeadzone = input > this.PlayerMappings[playerNumber].AxisBindingLookupTable[axis].UnpressedValue + (multiplier * this.PlayerMappings[playerNumber].AxisBindingLookupTable[axis].DeadZoneOffset);

        if (this.IsValueInDeadzone(input, this.PlayerMappings[playerNumber].AxisBindingLookupTable[axis]))
        {
            return 0.0f;
        }
        return input * (this.PlayerMappings[playerNumber].AxisBindingLookupTable[axis].Inverted ? -1 : 1);
    }

    public float GetAxisValue(GamepadAxis axis, InputMapping mapping)
    {
        if (mapping.OverridesAxisReading)
        {
            return mapping.OverrideAxisReading(axis);
        }
        float input = Input.GetAxisRaw(mapping.AxisBindingLookupTable[axis].AxisName);
        //bool valueBiggerThanDeadzone = Math.Abs(input) > mapping.AxisBindingLookupTable[axis].UnpressedValue + mapping.AxisBindingLookupTable[axis].DeadZoneOffset;
        if (this.IsValueInDeadzone(input, mapping.AxisBindingLookupTable[axis]))
        {
            return 0.0f;
        }
        return input * (mapping.AxisBindingLookupTable[axis].Inverted ? -1 : 1);
    }

    public Vector2 GetCombinedAxis(GamepadAxis AxisX, GamepadAxis AxisY, int playerNumber, float deadZone = 0.0f)
    {
        Vector2 VectorToReturn = new Vector2();
        if (this.PlayerMappings[playerNumber].OverridesAxisReading)
        {
            VectorToReturn.x = this.PlayerMappings[playerNumber].OverrideAxisReading(AxisX);
            VectorToReturn.y = this.PlayerMappings[playerNumber].OverrideAxisReading(AxisY);
            return VectorToReturn;
        }

        VectorToReturn.x = Input.GetAxisRaw(this.PlayerMappings[playerNumber].AxisBindingLookupTable[AxisX].AxisName) * (this.PlayerMappings[playerNumber].AxisBindingLookupTable[AxisX].Inverted ? -1 : 1);
        VectorToReturn.y = Input.GetAxisRaw(this.PlayerMappings[playerNumber].AxisBindingLookupTable[AxisY].AxisName) * (this.PlayerMappings[playerNumber].AxisBindingLookupTable[AxisY].Inverted ? -1 : 1);
       
        if (VectorToReturn.magnitude <= deadZone)
        {
            return Vector2.zero;
        }

        //if (this.IsValueInDeadzone(VectorToReturn.x, this.PlayerMappings[playerNumber].AxisBindingLookupTable[AxisX]))
        //{
        //    return 0.0f;
        //}
        return VectorToReturn;
    }

    private bool IsValueInDeadzone(float value, GamepadAxisInfo info)
    {
        int multiplier = value >= info.UnpressedValue ? 1 : -1;
        //Make sure that we don't get false positives, due to the deadzone there is on buttons
        if (multiplier < 0)
        {
            return value > info.UnpressedValue + (multiplier * info.DeadZoneOffset);
        }
        return value < info.UnpressedValue + (multiplier * info.DeadZoneOffset);
    }

    /// <summary>
    /// For each supported input mapping, get their aliasses and add to the dictionary for later use.
    /// </summary>
    private void FillNameToInputMappingLookupTable()
    {
        foreach (var typeofMapping in this.SupportedInputMappings)
        {
            InputMapping instance = (InputMapping)Activator.CreateInstance(typeofMapping);
            foreach (var alias in instance.GetControllerAliasses())
            {
                this.NameToInputMappingLookupTable[alias] = typeofMapping;
            }
        }
    }
}