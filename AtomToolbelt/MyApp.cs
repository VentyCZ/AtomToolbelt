using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

namespace AtomToolbelt
{
    public static class MyApp
    {
        public static string Name { get { return assemblyInfo.Name; } }
        public static Version Version { get { return assemblyInfo.Version; } }
        public static string SmallVersion { get { return Version.smallVersion(); } }

        public static MainWindow MainWindow { get { return mainWindow; } }
        public static Components.HeaderBar Header { get { return mainWindow.Header; } }

        private static AssemblyName assemblyInfo = Assembly.GetExecutingAssembly().GetName();
        private static MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
        private static Dispatcher UI_Dispatcher = Application.Current.Dispatcher;

        public static void ExecuteOnUI(Action deleg)
        {
            if (deleg != null)
            {
                if (UI_Dispatcher.CheckAccess())
                {
                    deleg();
                }
                else
                {
                    /*
                     * NOTE: Stop everything before disposing the elements!!
                     * If not, the application wil keep running in the backgound...
                     * It's here just to save the app from crashing...                     
                     */
                    if (!UI_Dispatcher.HasShutdownStarted)
                    {
                        UI_Dispatcher.Invoke(deleg);
                    }
                }
            }
        }

        public static string PrepPath(string path)
        {
            return path.Replace('/', Path.DirectorySeparatorChar);
        }

        public static class Constants
        {
            public static class Pages
            {
                public const string Menu = "MENU";
                public const string FixReg = "FIX_REGISTRY";
                public const string Sync = "SYNC";
                public const string About = "ABOUT";
            }
        }
    }
}