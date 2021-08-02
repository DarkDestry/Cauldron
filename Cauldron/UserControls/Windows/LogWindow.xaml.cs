using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Cauldron.Core.Logging;

namespace Cauldron.UserControls.Windows
{
    /// <summary>
    /// Interaction logic for LogWindow.xaml
    /// </summary>
    public partial class LogWindow : UserControl
    {
        public bool FilterDebug
        {
            get => (Logger.LogLevelMask & LogLevel.Debug) != 0;
            set
            {
                if (value) Logger.LogLevelMask |= LogLevel.Debug;
                else Logger.LogLevelMask &= ~ LogLevel.Debug;
            }
        }
        
        public bool FilterInfo
        {
            get => (Logger.LogLevelMask & LogLevel.Info) != 0;
            set
            {
                if (value) Logger.LogLevelMask |= LogLevel.Info;
                else Logger.LogLevelMask &= ~ LogLevel.Info;
            }
        }
        
        public bool FilterWarning
        {
            get => (Logger.LogLevelMask & LogLevel.Warning) != 0;
            set
            {
                if (value) Logger.LogLevelMask |= LogLevel.Warning;
                else Logger.LogLevelMask &= ~ LogLevel.Warning;
            }
        }
        
        public bool FilterError
        {
            get => (Logger.LogLevelMask & LogLevel.Error) != 0;
            set
            {
                if (value) Logger.LogLevelMask |= LogLevel.Error;
                else Logger.LogLevelMask &= ~ LogLevel.Error;
            }
        }
        
        public bool FilterCritical
        {
            get => (Logger.LogLevelMask & LogLevel.Critical) != 0;
            set
            {
                if (value) Logger.LogLevelMask |= LogLevel.Critical;
                else Logger.LogLevelMask &= ~ LogLevel.Critical;
            }
        }

        public LogWindow()
        {
            InitializeComponent();
        }

        private void ListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ListView _ListView = sender as ListView;
            GridView _GridView = _ListView.View as GridView;
            var _ActualWidth = _ListView.ActualWidth - SystemParameters.VerticalScrollBarWidth;
            for (Int32 i = 2; i < _GridView.Columns.Count; i++)
            {
                _ActualWidth = _ActualWidth - _GridView.Columns[i].ActualWidth;
            }
            _GridView.Columns[0].Width = _ActualWidth;
        }
        

    }
}
