using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using Cauldron.Core;

namespace Cauldron
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
        private void Application_Startup(object sender, StartupEventArgs e)
		{
            for (int i = 0; i < e.Args.Length; i++)
            {
                if (e.Args[i].StartsWith("-") || e.Args[i].StartsWith("/"))
                {
                    if (e.Args.Length - 1 > i && !(e.Args[i+1].StartsWith("-") || e.Args[i+1].StartsWith("/")))
                    {
                        CommandArguments.SetArgument(e.Args[i].Substring(1), e.Args[i + 1]);
                    }
                    else
                    {   
                        CommandArguments.SetArgument(e.Args[i].Substring(1));
                    }
                }
            }
			//Hook to Daemon here?
			Editor editor = new	Editor();
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
