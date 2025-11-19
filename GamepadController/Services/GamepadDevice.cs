using System;
using System.Collections.Generic;
using SharpDX.DirectInput;

namespace GamepadController.Services
{
    public class GamepadDevice
    {
        public Guid InstanceGuid { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public DeviceType DeviceType { get; set; }
        public bool IsConnected { get; set; }
        public int VendorId { get; set; }
        public int ProductId { get; set; }
        public string InstanceName { get; set; } = string.Empty;
        public JoystickState? CurrentState { get; set; }
        public Dictionary<string, float> AxisValues { get; set; } = new();
        public Dictionary<string, bool> ButtonStates { get; set; } = new();
        public List<int> HatSwitchStates { get; set; } = new();
        public int ButtonCount { get; set; }
        public int AxisCount { get; set; }
        public bool HasVibration { get; set; }
        public bool HasForceEffects { get; set; }

    public string DeviceTypeDescription => DeviceType switch
    {
        DeviceType.Gamepad => "Gamepad",
        DeviceType.Joystick => "Joystick",
        DeviceType.Keyboard => "Keyboard",
        DeviceType.Mouse => "Mouse",
        _ => "Input Device"
    };        public override string ToString() => $"{ProductName} ({DeviceTypeDescription})";
    }
}
