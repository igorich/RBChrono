using System;
using System.Windows.Threading;

namespace ResoAV
{
	public partial class Clock
	{
		DispatcherTimer timer;
		public Clock()
		{
			this.InitializeComponent();

			timer = new DispatcherTimer();
			timer.Interval = TimeSpan.FromSeconds(1.0);
			timer.Start();
			timer.Tick += new EventHandler(delegate(object s, EventArgs a)
			{
				tb.Text = String.Format("{0}:{1}:{2}",
					DateTime.Now.Hour.ToString("D2"),
					DateTime.Now.Minute.ToString("D2"),
					DateTime.Now.Second.ToString("D2"));
			});
		}

	}
}