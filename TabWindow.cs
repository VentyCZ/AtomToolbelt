using AtomToolbelt.Components;
using SpawnPoint.UI;
using System;
using System.Windows;

namespace AtomToolbelt
{
    public class TabWindow : Window
    {
        private HeaderBar header = null;

        private readonly PageCollection tabs = new PageCollection();

        public TabWindow()
        {
            SourceInitialized += onSourceUpdate;
            SourceUpdated += onSourceUpdate;
            Closed += onWindowClose;
        }

        private void onWindowClose(object sender, EventArgs e)
        {
            tabs.Exit();
        }

        private void onSourceUpdate(object sender, EventArgs e)
        {
            this.Beautify();
        }

        protected void Connect(HeaderBar header)
        {
            this.header = header;

            header.Connect(this);
        }

        protected PageCollection.Page AddPage(string name, FrameworkElement contents, bool isDefault = false)
        {
            return AddPage(tabs.Create(name, contents, isDefault));
        }

        protected PageCollection.Page AddPage(PageCollection.Page tab)
        {
            return tabs.Add(tab);
        }

        public void SetPage(string valPage)
        {
            SetPage(tabs.Get(valPage));
        }

        protected void SetPage(PageCollection.Page pg)
        {
            if (pg == null)
                return;

            pg.Activate();

            if (header != null && pg.IsDefault)
                header.ToggleBackButton(false);

            Console.WriteLine(pg != null ? "Page Set: " + pg.Name : "Non-Existent Page Error");
        }

        public void GoBack()
        {
            tabs.DefaultPage?.Activate();
            header.ToggleBackButton(false);
        }
    }
}