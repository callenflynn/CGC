using System;
using System.Windows;
using GamepadController.ViewModels;

namespace GamepadController.Views
{
    public partial class DevicePropertiesWindow : Window
    {
        public DevicePropertiesWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            
            // Update dead zone display when sliders change
            LeftDeadZoneSlider.ValueChanged += (s, e) => 
                LeftDeadZoneValue.Text = $"{(int)LeftDeadZoneSlider.Value}%";
            
            RightDeadZoneSlider.ValueChanged += (s, e) => 
                RightDeadZoneValue.Text = $"{(int)RightDeadZoneSlider.Value}%";
        }

        private void CalibrateButton_Click(object sender, RoutedEventArgs e)
        {
            // Open joy.cpl for calibration
            var viewModel = DataContext as MainViewModel;
            viewModel?.OpenJoyCPL();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
