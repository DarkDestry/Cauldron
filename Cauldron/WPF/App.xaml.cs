using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;

namespace Cauldron
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
        private void Application_Startup(object sender, StartupEventArgs e)
		{
			//Hook to Daemon here?
			Editor editor = new	Editor();
			if (e.Args.Length == 1)
				MessageBox.Show("Now opening file: \n\n" + e.Args[0]);
			editor.Show();
		}

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
#if RELEASE
			string messageBoxText = "An unhandled exception just occurred: " + e.Exception.Message;

			messageBoxText += "\n\nStack Trace:\n";
			messageBoxText += e.Exception.StackTrace;

			MessageBox.Show(messageBoxText, "Exception", MessageBoxButton.OK, MessageBoxImage.Warning);
			e.Handled = true;
#endif
        }

    }
    }
