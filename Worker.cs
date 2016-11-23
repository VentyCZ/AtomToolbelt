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
        private Dictionary<string, object> accomplishments = new Dictionary<string, object>();

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

        public void Set(string name, object value)
        {
            accomplishments[name] = value;
        }

        public object Get(string name, object defaultValue = null)
        {
            return (accomplishments.ContainsKey(name) ? accomplishments[name] : defaultValue);
        }

        public WorkItem AddItem(string name, Action<Worker> work)
        {
            var item = new WorkItem(name, work);
            workItems.Enqueue(item);

            if (workItems.Count == 1)
            {
                //We got an item, lets work
                startSignal.Set();
            }

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
                    Work();
                } while (true);
            })).Start();
        }

        private void Work()
        {
            if (workItems.Count == 0)
            {
                //Wait for signal
                //... Just to be sure!
                while (workItems.Count == 0)
                {
                    startSignal.WaitOne();
                }
            }

            while (workItems.Count > 0)
            {
                //Gets the item from queue, does its work & repeats
                var item = workItems.Dequeue();

                //Inform about the work
                WorkItemStarted?.Invoke(this, item);

                //Start the work
                item.Work.Invoke(this);

                //Inform about job being done
                WorkItemCompleted?.Invoke(this, item);
            }

            //Inform about DONE!
            WorkCompleted?.Invoke(this);
        }

        public class WorkItem
        {
            public string Name { get; private set; }
            public Action<Worker> Work { get; private set; }

            public WorkItem(string name, Action<Worker> work)
            {
                Name = name;
                Work = work;
            }
        }
    }
}
