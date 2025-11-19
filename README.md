# Gamepad Controller - Modern UI for Windows Game Device Management

A sleek, modern desktop application that provides an easy-to-use interface for managing game controllers, joysticks, racing rigs, and other input devices on Windows. It integrates with Windows' native `joy.cpl` while offering a contemporary UI with real-time input monitoring.

## Features

✨ **Modern Dark UI** - Clean, professional interface with dark theme
🎮 **Universal Device Support** - Works with:
- Game controllers (Xbox, PlayStation, etc.)
- Joysticks (Flight sticks, HOTAS systems)
- Racing wheels and sim rigs
- Any DirectInput-compatible device

📊 **Real-time Input Monitoring** - Live display of:
- Button states with visual feedback
- Analog stick and trigger positions
- D-Pad/Hat switch input
- Force feedback capability detection

⚙️ **joy.cpl Integration** - One-click access to Windows Game Controller settings for:
- Device calibration
- Detailed configuration
- Test mode
- Force feedback setup

## System Requirements

- Windows 7 or later (tested on Windows 10/11)
- .NET 8.0 Runtime
- DirectInput compatible device

## Installation

### Build from Source

**Prerequisites:**
- Visual Studio 2022 or later (or Visual Studio Code with C# extensions)
- .NET 8.0 SDK

**Steps:**
1. Clone the repository:
   ```bash
   git clone https://github.com/callenflynn/CGC.git
   cd CGC
   ```

2. Build the solution:
   ```bash
   dotnet build GamepadController.sln -c Release
   ```

3. Run the application:
   ```bash
   dotnet run --project GamepadController/GamepadController.csproj
   ```

### Or use the Visual Studio GUI:
1. Open `GamepadController.sln`
2. Build → Build Solution (Ctrl+Shift+B)
3. Debug → Start Debugging (F5)

## Usage

### Main Window
- **Device List** (Left Panel): Shows all connected input devices
- **Device Info** (Right Panel): Displays detailed information about selected device
- **Live Input Display**: Real-time visualization of button presses and axis positions

### Controls

| Button | Function |
|--------|----------|
| 🔄 Refresh | Scan for newly connected devices |
| ▶ Monitor | Start real-time input monitoring |
| ⏸ Stop | Stop monitoring (reduces CPU usage) |
| ⚙ joy.cpl | Opens Windows Game Controller settings |

### Workflow

1. **Connect your device** - Plug in your controller, joystick, or racing wheel
2. **Click Refresh** - The app will detect and list your device
3. **Select the device** - Click on it in the device list to view details
4. **Monitor input** - Click Monitor to see real-time button/axis activity
5. **Advanced setup** - Click joy.cpl for Windows' native configuration options

## Features Explained

### Device Information Panel
Shows:
- Device name and type (Gamepad, Joystick, etc.)
- Connection status
- Number of buttons and axes
- Force feedback capability
- Vendor and Product IDs

### Live Input Display
- **Axes Section**: Shows analog stick, trigger, and slider positions
- **Buttons Section**: Color-coded button states (green = pressed, gray = released)
- **Update Rate**: ~60 Hz for smooth response visualization

### joy.cpl Integration
One-click access to Windows' Game Controller settings where you can:
- Calibrate analog sticks
- Test button functionality
- Configure force feedback
- Update device drivers

## Project Structure

```
GamepadController/
├── App.xaml(.cs)              # Application entry point
├── Views/
│   └── MainWindow.xaml(.cs)   # Main UI window
├── ViewModels/
│   ├── MainViewModel.cs        # Main view model with commands
│   └── ViewModelBase.cs        # Base class for MVVM pattern
├── Services/
│   ├── GamepadService.cs       # DirectInput wrapper and device management
│   └── GamepadDevice.cs        # Device model
├── Converters/
│   └── ValueConverters.cs      # XAML value converters
└── GamepadController.csproj    # Project file

```

## Technical Details

### DirectInput Integration
The application uses SharpDX.DirectInput to interface with Windows' DirectInput API, providing:
- Automatic device detection and hot-plug support
- Axis normalization and state tracking
- Button and POV switch input handling
- Force feedback capability detection

### Threading Model
- UI operations run on the dispatcher thread
- Device monitoring runs on a background thread (~60 Hz)
- Thread-safe updates via Dispatcher.Invoke

### MVVM Pattern
- Clean separation between UI and business logic
- Reactive updates using INotifyPropertyChanged
- Command binding for button interactions

## Troubleshooting

### Device not showing up?
1. Click **Refresh** button
2. Verify device is properly connected
3. Check Windows Device Manager (devmgmt.msc)
4. Try the device in joy.cpl (⚙ button)
5. Update device drivers if necessary

### No input detected?
1. Ensure **Monitor** button is active (highlighted)
2. Check that device appears in device list
3. Try moving joystick or pressing buttons
4. Test in joy.cpl first to ensure device works

### Application crashes?
1. Ensure .NET 8.0 Runtime is installed
2. Try running as Administrator
3. Check Windows Event Viewer for error details
4. Update GPU drivers if using a compatible device

## Supported Devices

This application works with any DirectInput-compatible device, including:
- Xbox One/Series Controllers (via DirectInput)
- PlayStation 4/5 Controllers
- Generic USB gamepads
- Flight sticks (CH Products, Thrustmaster, etc.)
- Racing wheels (Logitech, Fanatec, Thrustmaster, etc.)
- HOTAS systems
- Vintage joysticks
- Custom DIY controllers

## Known Limitations

- Xbox controllers may work better through Xbox Accessories app, but DirectInput mode is supported
- Some modern devices may require specific drivers for full feature support
- Force feedback visualization is limited to capability detection (actual effect testing via joy.cpl)

## Future Enhancements

- [ ] Profile saving and loading
- [ ] Input remapping
- [ ] Macro recording
- [ ] Advanced force feedback testing
- [ ] Portable/standalone executable
- [ ] Multilingual support
- [ ] Controller-specific configuration presets

## Contributing

Contributions are welcome! Please feel free to:
- Report bugs
- Suggest features
- Submit pull requests
- Share device compatibility information

## License

MIT License - feel free to use, modify, and distribute.

## Credits

Built with:
- [SharpDX](http://sharpdx.org/) - DirectX wrapper for .NET
- [WPF](https://github.com/dotnet/wpf) - Windows Presentation Foundation

## Support

For issues and questions:
1. Check this README and Troubleshooting section
2. Visit the [GitHub Issues](https://github.com/callenflynn/CGC/issues)
3. Review Windows Event Viewer for system-level errors

---

**Enjoy your games! 🎮**
