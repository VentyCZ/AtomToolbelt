using System.Windows.Controls;

namespace AtomToolbelt.Views
{
    public partial class FixRegistry : UserControl
    {
        public FixRegistry()
        {
            InitializeComponent();
        }

        public void SetStatus(string statusText)
        {
            MyApp.ExecuteOnUI(delegate ()
            {
                Status.Text = statusText;
            });
        }
    }
}
