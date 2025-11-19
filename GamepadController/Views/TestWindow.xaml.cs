using System;
using System.Windows;
using GamepadController.ViewModels;

namespace GamepadController.Views
{
    public partial class TestWindow : Window
    {
        public TestWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
