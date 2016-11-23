using System;
using SpawnPoint.UI;
using SpawnPoint.Threading;
using AtomToolbelt.Utils;

namespace AtomToolbelt.Pages
{
    class Page_FixRegistry : PageCollection.Page
    {
        Views.FixRegistry view;
        AtomRegistry atom = new AtomRegistry();

        bool isWorking = false;
        Worker w;

        public Page_FixRegistry(string name, Views.FixRegistry content) : base(name, content, false)
        {
            view = content;
        }

        protected override void OnActivation()
        {
            if (isWorking)
            {
                Console.WriteLine("WUT?");
            }
            else
            {
                //You cant go back
                MyApp.Header.ToggleBackButton(false);

                //Start
                isWorking = true;
                w = new Worker();
                w.WorkItemStarted += WorkBlockStart;
                w.WorkCompleted += AllDone;

                w.AddItem("Discovery", Work_Discorver);
                w.Start();
            }
        }

        private void Work_Discorver(Worker worker)
        {
            var result = atom.DiscoverAtomVersions();

            if (result == AtomRegistry.DiscoveryResult.Success)
            {
                worker.AddItem("Edit", Work_Edit);
            }
            else
            {
                view.SetStatus("Failure: " + result.ToString());
                w.Exit();
            }
        }

        private void Work_Edit(Worker worker)
        {
            view.SetStatus("Using version: " + atom.LatestVersion);
        }

        protected override void OnDeactivation()
        {
            w.Exit();
        }

        private void WorkBlockStart(Worker sender, Worker.WorkItem item)
        {
            view.SetStatus(string.Format("START! - {0}", item.Name));
        }

        private void AllDone(Worker sender)
        {
            isWorking = false;
            //view.SetStatus("All done!");

            MyApp.Header.ToggleBackButton(true);
        }
    }
}
