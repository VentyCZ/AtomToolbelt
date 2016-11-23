using System;
using SpawnPoint.UI;
using SpawnPoint.Threading;

namespace AtomToolbelt.Pages
{
    class Page_FixRegistry : PageCollection.Page
    {
        Views.FixRegistry view;

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
                w.WorkItemCompleted += W_WorkItemCompleted;
                w.WorkItemStarted += WorkBlockStart;
                w.WorkCompleted += AllDone;

                w.AddItem("Work1", () =>
                {
                    Console.WriteLine("Work1 reporting!");
                });

                w.AddItem("Work2", () =>
                {
                    Console.WriteLine("Work2 reporting!");
                });
                w.Start();
            }
        }

        protected override void OnDeactivation()
        {
            w.Exit();
        }

        private void W_WorkItemCompleted(Worker sender, Worker.WorkItem item)
        {
            view.SetStatus(string.Format("DONE! - {0}", item.Name));
        }

        private void WorkBlockStart(Worker sender, Worker.WorkItem item)
        {
            view.SetStatus(string.Format("START! - {0}", item.Name));
        }

        private void AllDone(Worker sender)
        {
            isWorking = false;
            Console.WriteLine("We're done!");
            view.SetStatus("All done!");

            MyApp.Header.ToggleBackButton(true);
        }
    }
}
