using SpawnPoint.UI;

namespace AtomToolbelt.Pages
{
    class Page_About : PageCollection.Page
    {
        Views.About view;

        public Page_About(string name, Views.About content) : base(name, content, false)
        {
            view = content;

            view.About_Version.Content = MyApp.SmallVersion;
            view.About_ProgramName.Content = MyApp.Name;
        }

        protected override void OnActivation()
        {
            MyApp.Header.ToggleBackButton(true);
        }
    }
}
