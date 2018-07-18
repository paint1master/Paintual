/**********************************************************

Part of the Synergy code created by Ashish Kaila (https://www.codeproject.com/Articles/140209/Building-a-Docking-Window-Management-Solution-in-W),
code which is licensed under The Code Project Open License (CPOL).
Details of the license can be found in the accompanying file : cpol_license.htm

**********************************************************/

//
// Copyright(C) MixModes Inc. 2011
// 

using System;
using System.Globalization;
using System.Windows.Data;

namespace Cuisine.Converters
{
    /// <summary>
    /// Enumeration value matcher
    /// </summary>
    public class EnumValueMatcherConverter : IValueConverter
    {
        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">Value to match</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <remarks>Usage:
        /// IsChecked="{Binding Path=EditingMode, 
        ///                     Mode=OneWay,
        ///                     Converter={StaticResource EnumValueMatcherConverter}, 
        ///                     ConverterParameter={x:Static InkCanvasEditingMode.InkAndGesture}}"/>
        /// </remarks>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value == null) ||
                (parameter == null) ||
                (value.GetType() != parameter.GetType()))
            {
                return false;
            }

            Type valueType = value.GetType();

            if ((!Enum.IsDefined(valueType, value)) ||
                (!Enum.IsDefined(valueType, parameter)))
            {
                return false;
            }

            return Enum.GetName(valueType, value) == Enum.GetName(valueType, parameter);
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
