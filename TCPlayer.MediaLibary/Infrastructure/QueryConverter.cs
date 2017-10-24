using AppLib.WPF.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace TCPlayer.MediaLibary.Infrastructure
{
    public class QueryConverter : ConverterBase<QueryConverter>, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            List<bool> evaluated = new List<bool>(values.Length);
            foreach (var value in values)
            {
                if (value == null)
                {
                    evaluated.Add(false);
                }
                else
                {

                    var t = value.GetType();
                    if (t == typeof(string))
                    {
                        evaluated.Add((value as string).Length > 0);
                    }
                    else if (t == typeof(bool))
                    {
                        evaluated.Add((bool)value);
                    }
                    else if (t == typeof(uint?))
                    {
                        var c = (uint?)value;
                        evaluated.Add(c.HasValue);
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            return evaluated.Contains(true);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
