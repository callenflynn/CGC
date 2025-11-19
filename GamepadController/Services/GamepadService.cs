using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SharpDX.DirectInput;

namespace GamepadController.Services
{
    public class GamepadService
    {
        private DirectInput? _directInput;
        private Dictionary<Guid, Joystick?> _joysticks = new();
        private List<GamepadDevice> _devices = new();
        private bool _isMonitoring = false;

        public event Action<GamepadDevice>? DeviceConnected;
        public event Action<Guid>? DeviceDisconnected;
        public event Action<GamepadDevice>? InputUpdated;

        public GamepadService()
        {
            try
            {
                _directInput = new DirectInput();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to initialize DirectInput: {ex.Message}");
            }
        }

        public List<GamepadDevice> GetConnectedDevices()
        {
            var devices = new List<GamepadDevice>();

            if (_directInput == null) return devices;

            try
            {
                var deviceInstances = _directInput.GetDevices();

                foreach (var deviceInstance in deviceInstances)
                {
                    try
                    {
                        // Filter to only show devices that appear in joy.cpl
                        // Exclude keyboards, mice, and system devices
                        if (deviceInstance.Type == DeviceType.Keyboard || 
                            deviceInstance.Type == DeviceType.Mouse ||
                            deviceInstance.Type == DeviceType.Device ||
                            deviceInstance.ProductName.Contains("USB Root Hub") ||
                            deviceInstance.ProductName.Contains("USB Composite Device") ||
                            deviceInstance.ProductName.Contains("Generic USB Hub"))
                        {
                            continue;
                        }

                        var device = new GamepadDevice
                        {
                            InstanceGuid = deviceInstance.InstanceGuid,
                            ProductName = deviceInstance.ProductName,
                            InstanceName = deviceInstance.InstanceName,
                            DeviceType = deviceInstance.Type,
                            IsConnected = true
                        };

                        devices.Add(device);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error processing device {deviceInstance.ProductName}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error enumerating devices: {ex.Message}");
            }

            return devices;
        }

        public bool AcquireDevice(GamepadDevice device)
        {
            if (_directInput == null) return false;

            try
            {
                if (_joysticks.ContainsKey(device.InstanceGuid) && _joysticks[device.InstanceGuid] != null)
                    return true;

                var joystick = new Joystick(_directInput, device.InstanceGuid);
                joystick.Acquire();

                _joysticks[device.InstanceGuid] = joystick;

                // Get device capabilities
                var objects = joystick.GetObjects();
                device.ButtonCount = objects.Count(o => o.ObjectType == ObjectGuid.Button);
                device.AxisCount = objects.Count(o => o.ObjectType == ObjectGuid.XAxis ||
                                                      o.ObjectType == ObjectGuid.YAxis ||
                                                      o.ObjectType == ObjectGuid.ZAxis ||
                                                      o.ObjectType == ObjectGuid.Slider);

                device.HasVibration = false; // Will be true for devices with force feedback support
                device.HasForceEffects = false;
                device.HasForceEffects = device.HasVibration;

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to acquire device {device.ProductName}: {ex.Message}");
                return false;
            }
        }

        public void ReleaseDevice(Guid deviceGuid)
        {
            try
            {
                if (_joysticks.TryGetValue(deviceGuid, out var joystick))
                {
                    joystick?.Unacquire();
                    joystick?.Dispose();
                    _joysticks.Remove(deviceGuid);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error releasing device: {ex.Message}");
            }
        }

        public GamepadDevice? GetDeviceState(GamepadDevice device)
        {
            if (!_joysticks.TryGetValue(device.InstanceGuid, out var joystick) || joystick == null)
                return null;

            try
            {
                joystick.Poll();
                var state = joystick.GetCurrentState();
                device.CurrentState = state;

                // Parse buttons
                device.ButtonStates.Clear();
                var buttons = state.Buttons;
                for (int i = 0; i < buttons.Length; i++)
                {
                    device.ButtonStates[$"Button{i}"] = buttons[i];
                }

                // Parse axes
                device.AxisValues.Clear();
                device.AxisValues["X"] = Normalize(state.X);
                device.AxisValues["Y"] = Normalize(state.Y);
                device.AxisValues["Z"] = Normalize(state.Z);
                device.AxisValues["RX"] = Normalize(state.RotationX);
                device.AxisValues["RY"] = Normalize(state.RotationY);
                device.AxisValues["RZ"] = Normalize(state.RotationZ);

                var sliders = state.Sliders;
                for (int i = 0; i < sliders.Length; i++)
                {
                    device.AxisValues[$"Slider{i}"] = Normalize(sliders[i]);
                }

                // Parse POV/Hat switches
                device.HatSwitchStates.Clear();
                var povs = state.PointOfViewControllers;
                foreach (var pov in povs)
                {
                    device.HatSwitchStates.Add(pov);
                }

                return device;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting device state: {ex.Message}");
                return null;
            }
        }

        public void StartMonitoring()
        {
            if (_isMonitoring) return;

            _isMonitoring = true;
            Task.Run(() => MonitoringLoop());
        }

        public void StopMonitoring()
        {
            _isMonitoring = false;
        }

        private async Task MonitoringLoop()
        {
            var lastDeviceCount = 0;

            while (_isMonitoring)
            {
                try
                {
                    var currentDevices = GetConnectedDevices();

                    // Check for new devices
                    if (currentDevices.Count != lastDeviceCount)
                    {
                        var previousGuids = _devices.Select(d => d.InstanceGuid).ToHashSet();
                        var currentGuids = currentDevices.Select(d => d.InstanceGuid).ToHashSet();

                        foreach (var newDevice in currentDevices.Where(d => !previousGuids.Contains(d.InstanceGuid)))
                        {
                            AcquireDevice(newDevice);
                            DeviceConnected?.Invoke(newDevice);
                        }

                        foreach (var removedGuid in previousGuids.Where(g => !currentGuids.Contains(g)))
                        {
                            ReleaseDevice(removedGuid);
                            DeviceDisconnected?.Invoke(removedGuid);
                        }

                        _devices = currentDevices;
                        lastDeviceCount = currentDevices.Count;
                    }

                    // Update device states
                    foreach (var device in _devices)
                    {
                        var updatedDevice = GetDeviceState(device);
                        if (updatedDevice != null)
                        {
                            InputUpdated?.Invoke(updatedDevice);
                        }
                    }

                    await Task.Delay(16); // ~60 Hz update rate
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error in monitoring loop: {ex.Message}");
                    await Task.Delay(100);
                }
            }
        }

        private float Normalize(int value)
        {
            // Normalize axis values from 0-65535 to 0-1
            return Math.Clamp(value / 65535f, 0f, 1f);
        }

        public void OpenJoyCPL()
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "joy.cpl",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to open joy.cpl: {ex.Message}");
            }
        }

        public void Dispose()
        {
            StopMonitoring();

            foreach (var joystick in _joysticks.Values.Where(j => j != null))
            {
                try
                {
                    joystick?.Unacquire();
                    joystick?.Dispose();
                }
                catch { }
            }

            _joysticks.Clear();
            _directInput?.Dispose();
        }
    }
}
