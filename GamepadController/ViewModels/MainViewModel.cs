using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GamepadController.Services;
using GamepadController.Views;

namespace GamepadController.ViewModels
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Predicate<object?>? _canExecute;

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;
        public void Execute(object? parameter) => _execute(parameter);
    }

    public class MainViewModel : ViewModelBase
    {
        private readonly GamepadService _gamepadService;
        private ObservableCollection<GamepadDevice> _devices = new();
        private GamepadDevice? _selectedDevice;
        private bool _isMonitoring = false;

        public ObservableCollection<GamepadDevice> Devices
        {
            get => _devices;
            set => SetProperty(ref _devices, value);
        }

        public GamepadDevice? SelectedDevice
        {
            get => _selectedDevice;
            set
            {
                if (SetProperty(ref _selectedDevice, value))
                {
                    OnDeviceSelected(value);
                }
            }
        }

        public bool IsMonitoring
        {
            get => _isMonitoring;
            set => SetProperty(ref _isMonitoring, value);
        }

        public ICommand RefreshCommand { get; }
        public ICommand OpenJoyCPLCommand { get; }
        public ICommand StartMonitoringCommand { get; }
        public ICommand StopMonitoringCommand { get; }
        public ICommand TestCommand { get; }
        public ICommand PropertiesCommand { get; }

        public MainViewModel()
        {
            _gamepadService = new GamepadService();

            RefreshCommand = new RelayCommand(_ => RefreshDevices());
            OpenJoyCPLCommand = new RelayCommand(_ => OpenJoyCPL());
            StartMonitoringCommand = new RelayCommand(_ => StartMonitoring());
            StopMonitoringCommand = new RelayCommand(_ => StopMonitoring());
            TestCommand = new RelayCommand(_ => OpenTest(), _ => SelectedDevice != null);
            PropertiesCommand = new RelayCommand(_ => OpenProperties(), _ => SelectedDevice != null);

            _gamepadService.DeviceConnected += OnDeviceConnected;
            _gamepadService.DeviceDisconnected += OnDeviceDisconnected;
            _gamepadService.InputUpdated += OnInputUpdated;

            RefreshDevices();
        }

        public void RefreshDevices()
        {
            try
            {
                var devices = _gamepadService.GetConnectedDevices();
                
                // Get current device GUIDs
                var currentGuids = Devices.Select(d => d.InstanceGuid).ToHashSet();
                var newGuids = devices.Select(d => d.InstanceGuid).ToHashSet();

                // Remove devices that are no longer connected
                var devicesToRemove = Devices.Where(d => !newGuids.Contains(d.InstanceGuid)).ToList();
                foreach (var device in devicesToRemove)
                {
                    _gamepadService.ReleaseDevice(device.InstanceGuid);
                    Devices.Remove(device);
                }

                // Add new devices
                foreach (var device in devices)
                {
                    if (!currentGuids.Contains(device.InstanceGuid))
                    {
                        _gamepadService.AcquireDevice(device);
                        Devices.Add(device);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error refreshing devices: {ex.Message}");
            }
        }

        public void OpenJoyCPL()
        {
            _gamepadService.OpenJoyCPL();
        }

        public void OpenTest()
        {
            if (SelectedDevice == null) return;
            
            if (!IsMonitoring)
            {
                StartMonitoring();
            }
            var testWindow = new TestWindow(this);
            testWindow.Show();
        }

        public void OpenProperties()
        {
            if (SelectedDevice == null) return;
            
            var propertiesWindow = new DevicePropertiesWindow(this);
            propertiesWindow.Owner = Application.Current.MainWindow;
            propertiesWindow.ShowDialog();
        }

        public void StartMonitoring()
        {
            if (IsMonitoring) return;
            IsMonitoring = true;
            _gamepadService.StartMonitoring();
        }

        public void StopMonitoring()
        {
            IsMonitoring = false;
            _gamepadService.StopMonitoring();
        }

        private void OnDeviceConnected(GamepadDevice device)
        {
            Application.Current?.Dispatcher.Invoke(() =>
            {
                Devices.Add(device);
            });
        }

        private void OnDeviceDisconnected(Guid deviceGuid)
        {
            Application.Current?.Dispatcher.Invoke(() =>
            {
                var device = Devices.FirstOrDefault(d => d.InstanceGuid == deviceGuid);
                if (device != null)
                {
                    Devices.Remove(device);
                }
            });
        }

        private void OnInputUpdated(GamepadDevice device)
        {
            Application.Current?.Dispatcher.Invoke(() =>
            {
                if (SelectedDevice?.InstanceGuid == device.InstanceGuid)
                {
                    SelectedDevice = device;
                    OnPropertyChanged(nameof(SelectedDevice));
                }
            });
        }

        private void OnDeviceSelected(GamepadDevice? device)
        {
            // Logic when a device is selected
        }

        public void Cleanup()
        {
            StopMonitoring();
            _gamepadService.Dispose();
        }
    }
}
