using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace GamepadController.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo? culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? new SolidColorBrush(Color.FromRgb(0, 150, 0)) : new SolidColorBrush(Color.FromRgb(100, 100, 100));
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo? culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo? culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            }
            return System.Windows.Visibility.Collapsed;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo? culture)
        {
            throw new NotImplementedException();
        }
    }

    public class XAxisToPixelConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo? culture)
        {
            if (value is float floatValue && parameter is string paramStr && float.TryParse(paramStr, out float range))
            {
                // Convert 0-1 range to pixel offset (-range to +range)
                return (floatValue - 0.5f) * range * 2;
            }
            return 0;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo? culture)
        {
            throw new NotImplementedException();
        }
    }

    public class YAxisToPixelConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo? culture)
        {
            if (value is float floatValue && parameter is string paramStr && float.TryParse(paramStr, out float range))
            {
                // Convert 0-1 range to pixel offset (-range to +range), inverted for Y
                return (floatValue - 0.5f) * range * 2;
            }
            return 0;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo? culture)
        {
            throw new NotImplementedException();
        }
    }

    public class AxisToAngleConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo? culture)
        {
            if (value is float floatValue && parameter is string paramStr && float.TryParse(paramStr, out float maxAngle))
            {
                // Convert 0-1 range to angle
                return (floatValue - 0.5f) * maxAngle * 2;
            }
            return 0;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo? culture)
        {
            throw new NotImplementedException();
        }
    }

    public class HatSwitchConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo? culture)
        {
            if (value is List<int> hatValues && hatValues.Count > 0)
            {
                int hatValue = hatValues[0];
                return hatValue switch
                {
                    0 => "↑ North",
                    4500 => "↗ Northeast",
                    9000 => "→ East",
                    13500 => "↘ Southeast",
                    18000 => "↓ South",
                    22500 => "↙ Southwest",
                    27000 => "← West",
                    31500 => "↖ Northwest",
                    -1 => "⊗ Centered",
                    _ => $"∠{hatValue}°"
                };
            }
            return "⊗ Centered";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo? culture)
        {
            throw new NotImplementedException();
        }
    }
}
