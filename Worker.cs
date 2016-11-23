using System;
using System.Collections.Generic;
using System.Threading;

namespace SpawnPoint.Threading
{
    class Worker
    {
        private Thread wok;
        private AutoResetEvent startSignal = new AutoResetEvent(false);
        private Queue<WorkItem> workItems = new Queue<WorkItem>();

        public event WorkItemStartedEventArgs WorkItemStarted;
        public delegate void WorkItemStartedEventArgs(Worker sender, WorkItem item);

        public event WorkItemCompletedEventArgs WorkItemCompleted;
        public delegate void WorkItemCompletedEventArgs(Worker sender, WorkItem item);

        public event WorkCompleteEventArgs WorkCompleted;
        public delegate void WorkCompleteEventArgs(Worker sender);

        public void Start()
        {
            re_start();
        }

        public WorkItem AddItem(string name, Action work)
        {
            var item = new WorkItem(name, work);
            workItems.Enqueue(item);
            startSignal.Set();
            return item;
        }

        public void Exit()
        {
            if (wok != null)
            {
                wok.Abort();
                wok = null;
            }
        }

        private void re_start()
        {
            if (wok != null)
            {
                wok.Abort();
                wok = null;
            }

            (wok = new Thread(() =>
            {
                do
                {
                    Console.WriteLine("DOing");
                    Work();
                    Console.WriteLine("RESET");
                } while (true);
            })).Start();
        }

        private void Work()
        {
            if (workItems.Count == 0)
            {
                //Wait for signal
                Console.WriteLine("Waiting for signal (no work items)...");
                startSignal.WaitOne();
                Console.WriteLine("Got the signal! Getting to work!");
            }

            while (workItems.Count > 0)
            {
                //Gets the item from queue, does its work & repeats
                var item = workItems.Dequeue();

                //Inform about the work
                Console.WriteLine("Working on '{0}'...", item.Name);
                WorkItemStarted?.Invoke(this, item);

                //Start the work
                item.Work.Invoke();

                //Inform about job being done
                Console.WriteLine("Done working on '{0}'!", item.Name);
                WorkItemCompleted?.Invoke(this, item);
            }

            //Inform about DONE!
            WorkCompleted?.Invoke(this);
        }

        public class WorkItem
        {
            public string Name { get; private set; }
            public Action Work { get; private set; }

            public WorkItem(string name, Action work)
            {
                Name = name;
                Work = work;
            }
        }
    }
}
