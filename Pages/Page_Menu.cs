using SpawnPoint.UI;
using System.Windows;
using System.Windows.Controls;
using consts = AtomToolbelt.MyApp.Constants;

namespace AtomToolbelt.Pages
{
    class Page_Menu : PageCollection.Page
    {
        Views.Menu view;

        public Page_Menu(string name, Views.Menu content) : base(name, content, true)
        {
            view = content;

            view.Menu_FixRegistry.Click += onMenuItemClick;
            view.Menu_SyncSettings.Click += onMenuItemClick;
            view.Menu_About.Click += onMenuItemClick;
        }

        private void onMenuItemClick(object sender, RoutedEventArgs e)
        {
            Button menuItem = (Button)sender;

            if (menuItem == view.Menu_FixRegistry)
            {
                MyApp.MainWindow.SetPage(consts.Pages.FixReg);
            }
            else if (menuItem == view.Menu_SyncSettings)
            {
                MyApp.MainWindow.SetPage(consts.Pages.Sync);
            }
            else if (menuItem == view.Menu_About)
            {
                MyApp.MainWindow.SetPage(consts.Pages.About);
            }
        }
    }
}
