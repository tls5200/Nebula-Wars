﻿// written by: Thomas Stewart, Michael Quinn
// tested by: Michael Quinn
// debugged by: Thomas Stewart, Shane Barry, Diane Gregory

using UnityEngine;
using System.Collections;

/// <summary>
/// Key are used to represent a particular input from the user
/// Unifies KeyCodes and Axises
/// </summary>
public class Key
{
    public const string MOUSE_WHEEL_NAME = "MouseWheel";
    public const int MOUSE_WHEEL_UP = 1022;
    public const int MOUSE_WHELL_DOWN = 1023; 
    public const int START_AXIS = 1024;
    public const int NUM_AXIS = 20;
    public const int NUM_CONTROLLERS = 4;

    private int value;

    /// <summary>
    /// Creates a Key that represents the given KeyCode
    /// </summary>
    /// <param name="key">KeyCode to represent this Key as</param>
    public Key(KeyCode key)
    {
        value = (int)key;
    }

    /// <summary>
    /// Creates a Key that represents the Axis described by the given values
    /// </summary>
    /// <param name="controller">Which controller the axis is from</param>
    /// <param name="axis">Which axis from the controller</param>
    /// <param name="positive">If this represents the positive or negative part of the axis</param>
    public Key(int controller, int axis, bool positive)
    {
        if (controller < NUM_CONTROLLERS && axis < NUM_AXIS)
        {
            controller--;
            axis--;

            if (positive)
                value = START_AXIS + controller * NUM_AXIS + axis * 2;
            else
                value = START_AXIS + controller * NUM_AXIS + axis * 2 + 1;
        }
        else
        {
            throw new System.Exception("There is no " + axis.ToString() + " axis on controller " + controller.ToString());
        }
    }

    /// <summary>
    /// Creates a Key that represents the mouse wheel's axis
    /// </summary>
    /// <param name="mouseWheelUp">If this represents the wheel up or wheel down</param>
    public Key(bool mouseWheelUp)
    {
        if (mouseWheelUp)
            value = MOUSE_WHEEL_UP;
        else
            value = MOUSE_WHELL_DOWN;
    }

    /// <summary>
    /// Creates a Key that represents the input corrosponding to the given value
    /// Uses the same values as KeyCode but extends past them to represent axises
    /// </summary>
    /// <param name="key"></param>
    public Key(int key)
    {
        foreach (KeyCode item in System.Enum.GetValues(typeof(KeyCode)))
        {
            if ((int)item == key)
            {
                value = (int)item;
                return;
            }
        }

        if (key >= START_AXIS && key < START_AXIS + NUM_AXIS * NUM_CONTROLLERS)
        {
            value = key;
        }
        else
        {
            throw new System.Exception(key.ToString() + " is not a valid Key value!");
        }
    }

    /// <summary>
    /// Creates a Key that represents the same input as the given Key
    /// </summary>
    /// <param name="key">Key to copy</param>
    public Key(Key key)
    {
        value = key.value;
    }

    /// <summary>
    /// Returns the int value used to keep track of which input it represents
    /// </summary>
    /// <returns>The value used to keep track of the input</returns>
    public int GetValue()
    {
        return value;
    }

    /// <summary>
    /// Changes the value used to keep track of which input it represents
    /// </summary>
    /// <param name="value">Value to change to</param>
    public void ChangeValue(int value)
    {
        this.value = value;
    }

    /// <summary>
    /// Changes this Key to represent the same input as the given Key
    /// </summary>
    /// <param name="key">Key to copy</param>
    public void ChangeValue(Key copy)
    {
        value = copy.GetValue();
    }

    /// <summary>
    /// Return wether the Key is activated or not. 
    /// If this represents a KeyCode, its activated value will be returned.
    /// If this represents an axis, true will be returned if the value is above the KeyActivationThreshold.
    /// </summary>
    /// <returns>If this is activated or not.</returns>
    public bool IsPressed()
    {
        try
        {
            if (value == MOUSE_WHEEL_UP)
            {
                float axisValue = Input.GetAxis(MOUSE_WHEEL_NAME);

                if (axisValue > 0)
                    return true;
                else
                    return false;
            }
            else if (value == MOUSE_WHELL_DOWN)
            {
                float axisValue = Input.GetAxis(MOUSE_WHEEL_NAME);

                if (axisValue < 0)
                    return true;
                else
                    return false;
            }
            else if (value >= START_AXIS)
            {
                string name = ToString();

                if (name[0] == '+')
                {
                    return (Input.GetAxis(name.Substring(1)) > Options.Get().keyActivationThreshold);
                }
                else if (name[0] == '-')
                {
                    return (Input.GetAxis(name.Substring(1)) < -Options.Get().keyActivationThreshold);
                }
                else
                {
                    throw new System.Exception();
                }
            }
            else
            {
                return Input.GetKey((KeyCode)value);
            }
        }
        catch
        {
            throw new System.Exception(value.ToString() + " is not a valid Key value!");
        }
    }

    /// <summary>
    /// Return the float value of this Key.
    /// If this represents a KeyCode, it will return 1 if it is activated, 0 if not. 
    /// If this represents an axis, the axis's value will be returned if it is greater than the keyDeadZone
    /// </summary>
    /// <returns>If this is activated or not.</returns>
    public float GetAxis()
    {
        try
        {
            if (value == MOUSE_WHEEL_UP)
            {
                float axisValue = Input.GetAxis(MOUSE_WHEEL_NAME);

                if (axisValue > 0)
                    return axisValue;
                else
                    return 0;
            }
            else if (value == MOUSE_WHELL_DOWN)
            {
                float axisValue = Input.GetAxis(MOUSE_WHEEL_NAME);

                if (axisValue < 0)
                    return -axisValue;
                else
                    return 0;
            }
            else if (value >= START_AXIS)
            {
                string name = ToString();
                float axisValue = Input.GetAxis(name.Substring(1));

                if (name[0] == '+')
                {
                    if (axisValue > 0 && axisValue > Options.Get().keyDeadZone)
                    {
                        axisValue = (axisValue - Options.Get().keyDeadZone) / (1 - Options.Get().keyDeadZone);
                        return axisValue;
                    }
                    else
                    {
                        return 0f;
                    }
                }
                else if (name[0] == '-')
                {
                    if (axisValue < 0 && axisValue < -Options.Get().keyDeadZone)
                    {
                        axisValue = -axisValue;
                        axisValue = (axisValue - Options.Get().keyDeadZone) / (1 - Options.Get().keyDeadZone);
                        return axisValue;
                    }
                    else
                    {
                        return 0f;
                    }
                }
                else
                {
                    throw new System.Exception();
                }
            }
            else if (Input.GetKey((KeyCode)value))
            {
                return 1.0f;
            }
            else
            {
                return 0f;
            }
        }
        catch
        {
            throw new System.Exception(value.ToString() + " is not a valid Key value!");
        }
    }

    /// <summary>
    /// Return a string representing this Key's represented input in 5 charaters or less
    /// </summary>
    /// <returns>5 charater or less string representing this Key</returns>
    public string ToShortString()
    {
        switch (value)
        {
            case MOUSE_WHEEL_UP:
                return "WhlUp";
            case MOUSE_WHELL_DOWN:
                return "WhlDn";
            case (int)KeyCode.A:
                return "A";
            case (int)KeyCode.Alpha0:
                return "0";
            case (int)KeyCode.Alpha1:
                return "1";
            case (int)KeyCode.Alpha2:
                return "2";
            case (int)KeyCode.Alpha3:
                return "3";
            case (int)KeyCode.Alpha4:
                return "4";
            case (int)KeyCode.Alpha5:
                return "5";
            case (int)KeyCode.Alpha6:
                return "6";
            case (int)KeyCode.Alpha7:
                return "7";
            case (int)KeyCode.Alpha8:
                return "8";
            case (int)KeyCode.Alpha9:
                return "9";
            case (int)KeyCode.AltGr:
                return "AltGr";
            case (int)KeyCode.Ampersand:
                return "&";
            case (int)KeyCode.Asterisk:
                return "*";
            case (int)KeyCode.At:
                return "@";
            case (int)KeyCode.B:
                return "B";
            case (int)KeyCode.BackQuote:
                return "BkQt";
            case (int)KeyCode.Backslash:
                return "\\";
            case (int)KeyCode.Backspace:
                return "BkSp";
            case (int)KeyCode.Break:
                return "Brk";
            case (int)KeyCode.C:
                return "C";
            case (int)KeyCode.CapsLock:
                return "Caps";
            case (int)KeyCode.Caret:
                return "^";
            case (int)KeyCode.Clear:
                return "Clr";
            case (int)KeyCode.Colon:
                return ":";
            case (int)KeyCode.Comma:
                return ",";
            case (int)KeyCode.D:
                return "D";
            case (int)KeyCode.Delete:
                return "Del";
            case (int)KeyCode.Dollar:
                return "$";
            case (int)KeyCode.DoubleQuote:
                return "\"";
            case (int)KeyCode.DownArrow:
                return "Down";
            case (int)KeyCode.E:
                return "E";
            case (int)KeyCode.End:
                return "End";
            case (int)KeyCode.Equals:
                return "=";
            case (int)KeyCode.Escape:
                return "Esc";
            case (int)KeyCode.Exclaim:
                return "!";
            case (int)KeyCode.F:
                return "F";
            case (int)KeyCode.F1:
                return "F1";
            case (int)KeyCode.F2:
                return "F2";
            case (int)KeyCode.F3:
                return "F3";
            case (int)KeyCode.F4:
                return "F4";
            case (int)KeyCode.F5:
                return "F5";
            case (int)KeyCode.F6:
                return "F6";
            case (int)KeyCode.F7:
                return "F7";
            case (int)KeyCode.F8:
                return "F8";
            case (int)KeyCode.F9:
                return "F9";
            case (int)KeyCode.F10:
                return "F10";
            case (int)KeyCode.F11:
                return "F11";
            case (int)KeyCode.F12:
                return "F12";
            case (int)KeyCode.F13:
                return "F13";
            case (int)KeyCode.F14:
                return "F14";
            case (int)KeyCode.F15:
                return "F15";
            case (int)KeyCode.G:
                return "G";
            case (int)KeyCode.Greater:
                return ">";
            case (int)KeyCode.H:
                return "H";
            case (int)KeyCode.Hash:
                return "Hash";
            case (int)KeyCode.Help:
                return "Help";
            case (int)KeyCode.Home:
                return "Home";
            case (int)KeyCode.I:
                return "I";
            case (int)KeyCode.Insert:
                return "Ins";
            case (int)KeyCode.J:
                return "J";
            case (int)KeyCode.K:
                return "K";
            case (int)KeyCode.Keypad0:
                return "N 0";
            case (int)KeyCode.Keypad1:
                return "N 1";
            case (int)KeyCode.Keypad2:
                return "N 2";
            case (int)KeyCode.Keypad3:
                return "N 3";
            case (int)KeyCode.Keypad4:
                return "N 4";
            case (int)KeyCode.Keypad5:
                return "N 5";
            case (int)KeyCode.Keypad6:
                return "N 6";
            case (int)KeyCode.Keypad7:
                return "N 7";
            case (int)KeyCode.Keypad8:
                return "N 8";
            case (int)KeyCode.Keypad9:
                return "N 9";
            case (int)KeyCode.KeypadDivide:
                return "N /";
            case (int)KeyCode.KeypadEnter:
                return "NEntr";
            case (int)KeyCode.KeypadEquals:
                return "N =";
            case (int)KeyCode.KeypadMinus:
                return "N -";
            case (int)KeyCode.KeypadMultiply:
                return "N *";
            case (int)KeyCode.KeypadPeriod:
                return "N .";
            case (int)KeyCode.KeypadPlus:
                return "N +";
            case (int)KeyCode.L:
                return "L";
            case (int)KeyCode.LeftAlt:
                return "LAlt";
            //case (int)KeyCode.LeftApple:
            //return "LApl";
            case (int)KeyCode.LeftArrow:
                return "Left";
            case (int)KeyCode.LeftBracket:
                return "[";
            case (int)KeyCode.LeftCommand:
                return "LCmd";
            case (int)KeyCode.LeftControl:
                return "LCtrl";
            case (int)KeyCode.LeftParen:
                return "LPrn";
            case (int)KeyCode.LeftShift:
                return "LSft";
            case (int)KeyCode.LeftWindows:
                return "LWin";
            case (int)KeyCode.Less:
                return "<";
            case (int)KeyCode.M:
                return "M";
            case (int)KeyCode.Menu:
                return "Menu";
            case (int)KeyCode.Minus:
                return "-";
            case (int)KeyCode.Mouse0:
                return "LClk";
            case (int)KeyCode.Mouse1:
                return "RClk";
            case (int)KeyCode.Mouse2:
                return "MClk";
            case (int)KeyCode.Mouse3:
                return "FWrd";
            case (int)KeyCode.Mouse4:
                return "BWrd";
            case (int)KeyCode.Mouse5:
                return "Mou5";
            case (int)KeyCode.Mouse6:
                return "Mou6";
            case (int)KeyCode.N:
                return "N";
            case (int)KeyCode.None:
                return "None";
            case (int)KeyCode.Numlock:
                return "Num";
            case (int)KeyCode.O:
                return "O";
            case (int)KeyCode.P:
                return "P";
            case (int)KeyCode.PageDown:
                return "PgDn";
            case (int)KeyCode.PageUp:
                return "PgUp";
            case (int)KeyCode.Pause:
                return "Pause";
            case (int)KeyCode.Period:
                return ".";
            case (int)KeyCode.Plus:
                return "+";
            case (int)KeyCode.Print:
                return "Prt";
            case (int)KeyCode.Q:
                return "Q";
            case (int)KeyCode.Question:
                return "?";
            case (int)KeyCode.Quote:
                return "'";
            case (int)KeyCode.R:
                return "R";
            case (int)KeyCode.Return:
                return "Rtn";
            case (int)KeyCode.RightAlt:
                return "RAlt";
            //case (int)KeyCode.RightApple:
            //return "RApl";
            case (int)KeyCode.RightArrow:
                return "Right";
            case (int)KeyCode.RightBracket:
                return "]";
            case (int)KeyCode.RightCommand:
                return "RCmd";
            case (int)KeyCode.RightControl:
                return "RCtrl";
            case (int)KeyCode.RightParen:
                return "RPrn";
            case (int)KeyCode.RightShift:
                return "RSft";
            case (int)KeyCode.RightWindows:
                return "RWin";
            case (int)KeyCode.S:
                return "S";
            case (int)KeyCode.ScrollLock:
                return "Scrl";
            case (int)KeyCode.Semicolon:
                return ";";
            case (int)KeyCode.Slash:
                return "/";
            case (int)KeyCode.Space:
                return "Space";
            case (int)KeyCode.SysReq:
                return "SyRq";
            case (int)KeyCode.T:
                return "T";
            case (int)KeyCode.Tab:
                return "Tab";
            case (int)KeyCode.U:
                return "U";
            case (int)KeyCode.Underscore:
                return "_";
            case (int)KeyCode.UpArrow:
                return "Up";
            case (int)KeyCode.V:
                return "V";
            case (int)KeyCode.W:
                return "W";
            case (int)KeyCode.X:
                return "X";
            case (int)KeyCode.Y:
                return "Y";
            case (int)KeyCode.Z:
                return "Z";
            default:
                break;
        }

        //if this Key represents a controller button
        if (value >= 330 && value < 510)
        {
            int temp = value - 330;
            int controller = temp / 20;
            int button = temp % 20;

            if (Options.Get().keyXboxNames)
            {
                switch (button)
                {
                    case 0:
                        return "ABtn";
                    case 1:
                        return "BBtn";
                    case 2:
                        return "XBtn";
                    case 3:
                        return "YBtn";
                    case 4:
                        return "LBmp";
                    case 5:
                        return "RBmp";
                    case 6:
                        return "Back";
                    case 7:
                        return "Strt";
                    case 8:
                        return "LStk";
                    case 9:
                        return "RStk";
                    default:
                        return "Btn" + button;
                }
            }
            else
            {
                return "Btn" + button;
            }
        }

        //if this Key represents an axis
        if (value >= START_AXIS)
        {
            int tempValue = value - START_AXIS;

            int controller = tempValue / NUM_AXIS + 1;
            int axis = tempValue % NUM_AXIS;
            bool positive = (tempValue % 2) == 0;
            axis = axis / 2 + 1;

            if (Options.Get().keyXboxNames)
            {
                if (positive)
                {
                    switch (axis)
                    {
                        case 1:
                            return "+XAxs";
                        case 2:
                            return "+YAxs";
                        case 3:
                            return "RTrig";
                        case 4:
                            return "+ZAxs";
                        case 5:
                            return "+WAxs";
                        case 6:
                            return "DUp";
                        case 7:
                            return "DRght";
                        case 9:
                            return "LTrig";
                        case 10:
                            return "RTrig";
                        default:
                            return "+Axs" + axis;
                    }
                }
                else
                {
                    switch (axis)
                    {
                        case 1:
                            return "-XAxs";
                        case 2:
                            return "-YAxs";
                        case 3:
                            return "LTrig";
                        case 4:
                            return "-ZAxs";
                        case 5:
                            return "-WAxs";
                        case 6:
                            return "DDown";
                        case 7:
                            return "DLeft";
                        case 9:
                            return "LTrig";
                        case 10:
                            return "RTrig";
                        default:
                            return "-Axs" + axis;
                    }
                }
            }
            else
            {
                if (positive)
                {
                    return "+Axs" + axis;
                }
                else
                {
                    return "-Axs" + axis; 
                }
            }
        }

        return "?" + value;
    }

    /// <summary>
    /// Return a string representing this Key's input
    /// </summary>
    /// <returns>String representing this Key's input</returns>
    public override string ToString()
    {
        try
        {
            if (value == MOUSE_WHEEL_UP)
            {
                return "Wheel Up";
            }
            else if (value == MOUSE_WHELL_DOWN)
            {
                return "Wheel Down";
            }
            //Key represents a controller's axis
            else if (value >= START_AXIS)
            {
                int tempValue = value - START_AXIS;

                int controller = tempValue / NUM_AXIS + 1;
                int axis = tempValue % NUM_AXIS;
                bool positive = (tempValue % 2) == 0;
                axis = axis / 2 + 1;

                return (GetAxisString(controller, axis, positive));
            }
            //Key represents a controller's button
            else if (value >= 330 && value < 510)
            {
                int temp = value - 330;
                int controller = temp / 20;
                int button = temp % 20;

                if (Options.Get().keyXboxNames)
                {
                    switch (button)
                    {
                        case 0:
                            return "Con" + controller + " A-Btn";
                        case 1:
                            return "Con" + controller + " B-Btn";
                        case 2:
                            return "Con" + controller + " X-Btn";
                        case 3:
                            return "Con" + controller + " Y-Btn";
                        case 4:
                            return "Con" + controller + " L-Bmp";
                        case 5:
                            return "Con" + controller + " R-Bmp";
                        case 6:
                            return "Con" + controller + " Back";
                        case 7:
                            return "Con" + controller + " Start";
                        case 8:
                            return "Con" + controller + " L-Stk";
                        case 9:
                            return "Con" + controller + " R-Stk";
                        default:
                            return "Con" + controller + " Btn" + button;
                    }
                }
                else
                {
                    return "Con" + controller + " Btn" + button;
                }
            }
            else
            {
                return ((KeyCode)value).ToString();
            }
        }
        catch
        {
            throw new System.Exception(value.ToString() + " is not a valid Key value!");
        }
    }

    /// <summary>
    /// Returns the string that represents the axis described by the given values
    /// </summary>
    /// <param name="controller">Which controller the axis is from</param>
    /// <param name="axis">Which axis from the controller</param>
    /// <returns>String representing the axis</returns>
    public static string GetAxisString(int controller, int axis)
    {
        return ("Con" + controller.ToString() + " Axis" + axis.ToString());
    }

    /// <summary>
    /// Returns the string that represents the axis described by the given values
    /// </summary>
    /// <param name="controller">Which controller the axis is from</param>
    /// <param name="axis">Which axis from the controller</param>
    /// <param name="positive">If this represents the positive or negative part of the axis</param>
    /// <returns>String representing the axis</returns>
    public static string GetAxisString(int controller, int axis, bool positive)
    {
        if (positive)
        {
            return ("+" + GetAxisString(controller, axis));
        }
        else
        {
            return ("-" + GetAxisString(controller, axis));
        }
    }

    /// <summary>
    /// Returns one of the inputs that is currently activated. If none are, it will return null.
    /// </summary>
    /// <returns>The activated input or null</returns>
    public static Key ActivatedKey()
    {
        foreach (KeyCode item in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKey(item))
            {
                return new Key(item);
            }
        }

        float axisValue;

        for (int i = 1; i <= NUM_CONTROLLERS; i++)
        {
            for (int j = 1; j <= NUM_AXIS / 2; j++)
            {
                axisValue = Input.GetAxis(GetAxisString(i, j));
                if (System.Math.Abs(axisValue) > Options.Get().keyDeadZone)
                {
                    if (axisValue > 0)
                    {
                        return new Key(i, j, true);
                    }
                    else
                    {
                        return new Key(i, j, false);
                    }
                }
            }
        }

        axisValue = Input.GetAxis(MOUSE_WHEEL_NAME);

        if (axisValue > 0)
            return new Key(true);
        else if (axisValue < 0)
            return new Key(false);

        return null;
    }
}
