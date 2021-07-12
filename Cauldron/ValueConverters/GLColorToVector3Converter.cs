using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using SharpGL.SceneGraph;

namespace Cauldron.ValueConverters
{
    class GLColorToVector3Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (CldVector3) (GLColor) value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (GLColor) (CldVector3) value;
        }
    }
}
