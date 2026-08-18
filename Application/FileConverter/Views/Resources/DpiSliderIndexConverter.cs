// <copyright file="DpiSliderIndexConverter.cs" company="AAllard">License: http://www.gnu.org/licenses/gpl.html GPL version 3.</copyright>

namespace FileConverter.ValueConverters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// Maps the discrete DPI choices to equally spaced slider positions.
    /// </summary>
    public class DpiSliderIndexConverter : IValueConverter
    {
        private static readonly double[] DpiValues = { 72, 96, 120, 150, 200, 240, 300, 400, 600 };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue &&
                double.TryParse(stringValue, NumberStyles.Float, CultureInfo.InvariantCulture, out double dpi))
            {
                int index = 0;
                double smallestDifference = Math.Abs(DpiValues[0] - dpi);

                for (int i = 1; i < DpiValues.Length; i++)
                {
                    double difference = Math.Abs(DpiValues[i] - dpi);
                    if (difference < smallestDifference)
                    {
                        smallestDifference = difference;
                        index = i;
                    }
                }

                return (double)index;
            }

            if (value is double indexValue)
            {
                int index = (int)Math.Round(indexValue);
                index = Math.Max(0, Math.Min(DpiValues.Length - 1, index));
                return DpiValues[index];
            }

            if (value is int intIndex)
            {
                int index = Math.Max(0, Math.Min(DpiValues.Length - 1, intIndex));
                return DpiValues[index];
            }

            return 6d;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int index;

            if (value is double doubleValue)
            {
                index = (int)Math.Round(doubleValue);
            }
            else if (value is int intValue)
            {
                index = intValue;
            }
            else
            {
                index = 6;
            }

            index = Math.Max(0, Math.Min(DpiValues.Length - 1, index));
            return DpiValues[index].ToString(CultureInfo.InvariantCulture);
        }
    }
}
