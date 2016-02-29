// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Globalization;
using Windows.UI.Xaml.Data;

namespace Bikes.View
{
    /// <summary>
    /// Converts bool? values to "Off" and "On" strings.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class OffOnConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType == null)
            {
                throw new ArgumentNullException("targetType");
            }
            if (targetType != typeof(object))
            {
                throw new ArgumentException("The type was unexpected.", "targetType");
            }
            if (value is bool? || value == null)
            {
                return (bool?)value == true ? "On" : "Off";
            }
            throw new ArgumentException("The type was unexpected.", "value");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}