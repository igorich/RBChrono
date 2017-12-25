using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Collections.ObjectModel;

namespace ResoAV
{
    /// <summary>
    /// Interaction logic for TimerControl.xaml
    /// </summary>
    public partial class TimerControl : UserControl
    {
        private Timer _timer = new Timer();
        private Thread _ttimer;
        private static BitmapImage _bmpCloseButton, _bmpCloseButton_Over, _bmpRefreshButton, _bmpRefreshButton_Over;
        private static BitmapImage _bmpRed, _bmpYellow, _bmpGreen;
        private ObservableCollection<TimeSpan> _signals = new ObservableCollection<TimeSpan>();

        public TimerControl()
        {
            InitializeComponent();
            _timer.Init(UpdateText1);
            _ttimer = new Thread(_timer.Start);

            #region Images
            Uri imgPath = null;
            if (_bmpCloseButton == null)
            {
                imgPath = new Uri(@"pack://application:,,,/ResoAV;component/Images/fileclose.png", UriKind.Absolute);
                _bmpCloseButton = new BitmapImage(imgPath);
            }
            if (_bmpCloseButton_Over == null)
            {
                imgPath = new Uri(@"pack://application:,,,/ResoAV;component/Images/fileclose_over.png", UriKind.Absolute);
                _bmpCloseButton_Over = new BitmapImage(imgPath);
            }

            imgClose.Source = _bmpCloseButton;

            if (_bmpRed == null)
            {
                imgPath = new Uri(@"pack://application:,,,/ResoAV;component/Images/led_red.png", UriKind.Absolute);
                _bmpRed = new BitmapImage(imgPath);
            }
            if (_bmpGreen == null)
            {
                imgPath = new Uri(@"pack://application:,,,/ResoAV;component/Images/led_green.png", UriKind.Absolute);
                _bmpGreen = new BitmapImage(imgPath);
            }
            if (_bmpYellow == null)
            {
                imgPath = new Uri(@"pack://application:,,,/ResoAV;component/Images/led_yellow.png", UriKind.Absolute);
                _bmpYellow = new BitmapImage(imgPath);
            }
            imgState.Source = _bmpGreen;
            if (_bmpRefreshButton == null)
            {
                imgPath = new Uri(@"pack://application:,,,/ResoAV;component/Images/refresh.png", UriKind.Absolute);
                _bmpRefreshButton = new BitmapImage(imgPath);
            }
            if (_bmpRefreshButton_Over == null)
            {
                imgPath = new Uri(@"pack://application:,,,/ResoAV;component/Images/refresh_over.png", UriKind.Absolute);
                _bmpRefreshButton_Over = new BitmapImage(imgPath);
            }
            imgRefresh.Source = _bmpRefreshButton;
            #endregion

            lvSignals.ItemsSource = _signals;
            if (!RaceSettings.RS.IsHardwareStart)
            {
                lvSignals.Visibility = System.Windows.Visibility.Collapsed;
                this.Height -= lvSignals.Height;
            }

            SetState(ThisStates.Ready);
        }

		private Table.ResultCallbackDelegate _resultCallback;
		private clCompetitor _man;
        public int ID { get { return _man.Number; } }
		public void Init(Table.ResultCallbackDelegate callback, clCompetitor man)
        {
			lbHeader.Text = man.Number + " " + man.Name + " " + man.Class;
			_resultCallback = callback;
			_man = man;
            HardwareCommunicator.ReceivedData += new HardwareCommunicator.ComEventHandler(HardwareCommunicator_ReceivedData);
        }

        void HardwareCommunicator_ReceivedData(byte type, object args)
        {
            if (btStop.Visibility == Visibility.Visible)// && btStop.IsEnabled == true)
            {
                //btStop_Click(this, null);
                Dispatcher.Invoke((Action)(() => 
                {
                    var tmp = GetResult();
                    _signals.Add(tmp);
                    lvSignals.ScrollIntoView(tmp);
                }), null);
            }
        }

        private void btStart_Click(object sender, RoutedEventArgs e)
        {
			SetState(ThisStates.Countdown);
			//hardware start
            if (RaceSettings.RS.IsHardwareStart)
                HardwareCommunicator.RunCountdown(ProcessHardwareStart);
            else
                DoStart(0);
        }

		private delegate void HardwareStartDelegate(int st);

		public void ProcessHardwareStart(int st)
		{
			HardwareStartDelegate updateDelegate = new HardwareStartDelegate(DoStart);
			Dispatcher.Invoke(updateDelegate, System.Windows.Threading.DispatcherPriority.Background, new object[] { st });
		}

		public void DoStart(int state)
		{
			try
			{
				var p = Parent;
				var table = ((p as StackPanel).Parent as Grid).Parent as ResoAV.Table;
				table.ProcessHardwareStartGUIThread(state);
			}
			catch(NullReferenceException ex)
			{
				System.Diagnostics.Trace.WriteLine("DoStart error: " + ex.Message);
				System.Diagnostics.Trace.WriteLine("Возможно не найден родительский объект класса ResoAV.Table");
			}

			if(state == 0)
			{
				if(!_ttimer.IsAlive)
				{
					_ttimer = new Thread(_timer.Start);
					_ttimer.Start();
				}
				SetState(ThisStates.Counting);
			}
		}

        private void btStop_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            _ttimer.Abort();

            Dispatcher.Invoke((Action)(() =>
            {
                if (Tag is TextBlock)
                    _resultCallback(_man, GetResult(), 0, (Tag as TextBlock));

                SetState(ThisStates.Finish);
            }), null);
        }


        //thread
        public delegate void StringArgDelegate(string s);
        private delegate void UpdateTimerContentDelegate2(string s);
        public void UpdateText1(string s)
        {
            UpdateTimerContentDelegate2 updateDelegate = new UpdateTimerContentDelegate2(UpdateTimerContent1);
            Dispatcher.Invoke(updateDelegate, System.Windows.Threading.DispatcherPriority.Background, s);
        }

        private void UpdateTimerContent1(string s)
        {
            tbTwo.Text = s;
        }

        public TimeSpan GetResult()
        {
            TimeSpan result;
            try
            {
                result = (TimeSpan)lvSignals.SelectedItem;
            }
            catch
            {
                result = _timer.GetTime;
            }
            return result;
        }

        private void imgClose_MouseEnter(object sender, MouseEventArgs e)
        {
            //if(!_ttimer.IsAlive)
            imgClose.Source = _bmpCloseButton_Over;
        }

        private void imgClose_MouseLeave(object sender, MouseEventArgs e)
        {
            imgClose.Source = _bmpCloseButton;
        }

        private void imgClose_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!_ttimer.IsAlive)
            {
                var parent = this.Parent as StackPanel;
                parent.Children.Remove(this);
                HardwareCommunicator.ReceivedData -= HardwareCommunicator_ReceivedData;
            }
        }

        public void DoAction()
        {
            if (btStart.Visibility == System.Windows.Visibility.Visible)
                btStart_Click(null, null);
            else if (btStop.Visibility == System.Windows.Visibility.Visible)
                btStop_Click(null, null);
        }

        private void imgRefresh_MouseLeave(object sender, MouseEventArgs e)
        {
            imgRefresh.Source = _bmpRefreshButton;
        }

        private void imgRefresh_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SetState(ThisStates.Ready);

            UpdateTimerContent1("00:00.000");

            if (_ttimer.IsAlive)
                _ttimer.Abort();
        }

        private void imgRefresh_MouseEnter(object sender, MouseEventArgs e)
        {
            imgRefresh.Source = _bmpRefreshButton_Over;
        }

        void SetState(ThisStates st)
        {
            //if (!RaceSettings.RS.IsHardwareStart)
            //    return;
            switch (st)
            {
                case ThisStates.Ready:
                    imgState.Source = _bmpGreen;
                    btStart.Visibility = System.Windows.Visibility.Visible;
                    btStart.IsEnabled = true;
                    btStop.Visibility = System.Windows.Visibility.Collapsed;
                    imgRefresh.IsEnabled = false;
                    imgClose.IsEnabled = true;
                    break;
				case ThisStates.Countdown:
					imgState.Source = _bmpYellow;
					btStart.Visibility = System.Windows.Visibility.Collapsed;
					btStop.Visibility = System.Windows.Visibility.Visible;
					btStop.IsEnabled = false;
					imgRefresh.IsEnabled = false;
					imgClose.IsEnabled = false;
					break;
				case ThisStates.Counting:
                    imgState.Source = _bmpYellow;
                    btStart.Visibility = System.Windows.Visibility.Collapsed;
                    btStop.Visibility = System.Windows.Visibility.Visible;
                    btStop.IsEnabled = true;
                    imgRefresh.IsEnabled = true;
                    imgClose.IsEnabled = false;
                    break;
                case ThisStates.Finish:
                    imgState.Source = _bmpRed;
                    btStart.Visibility = System.Windows.Visibility.Collapsed;
                    btStop.Visibility = System.Windows.Visibility.Visible;
                    btStop.IsEnabled = false;
                    imgRefresh.IsEnabled = true;
                    imgClose.IsEnabled = true;
                    break;
            }
        }

        private enum ThisStates
        {
            Ready,
			Countdown,
            Counting,
            Finish,
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_ttimer.IsAlive)
                _ttimer.Abort();

        }
    }
}
