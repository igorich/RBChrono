using System;
using System.Threading;

namespace ResoAV
{
    class Timer //: Thread
    {
        private DateTime _start;
        private DateTime _stop;
        //private TimeSpan _span;
        public TimeSpan GetTime { get { return _stop - _start; } }
        private MainWindow.StringArgDelegate _callback;
        //private Window w;

        public void Start()
        {
            _start = DateTime.Now;
            while (true)
            {
                _stop = DateTime.Now;
                UpdateTimerContent();
                Thread.Sleep(50);
            }
        }

        public TimeSpan Stop()
        {
            return _stop - _start;
            //return _span;
        }

        private delegate void UpdateTimerContentDelegate();
        private void UpdateTimerContent()
        {
            _callback(GetTime.ToResoAvView());
        }

        public delegate void NoArgDelegate(string s);
        public void Init(MainWindow.StringArgDelegate arg)
        {
            _callback = arg;
        }
    }
}
