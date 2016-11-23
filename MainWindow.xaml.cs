using System.Windows;
using AtomToolbelt.Pages;
using consts = AtomToolbelt.MyApp.Constants;

namespace AtomToolbelt
{
    public partial class MainWindow : TabWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += onLoad;
        }

        private void onLoad(object sender, RoutedEventArgs e)
        {
            //Window title - Taskbar only, displayed title is controlled by Header
            Title = MyApp.Name;

            //Basic page setup
            AddPage(new Page_Menu(consts.Pages.Menu, Page_Menu));
            AddPage(new Page_FixRegistry(consts.Pages.FixReg, Page_FixRegistry));
            AddPage(new Page_About(consts.Pages.About, Page_About));

            //Connect the header - Sets the displayed title, among other things
            Connect(Header);
        }
    }
}