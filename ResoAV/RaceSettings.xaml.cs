using System;
using System.Windows;
using System.Collections.Generic;

namespace ResoAV
{
	/// <summary>
	/// Логика взаимодействия для сумрачного гения
	/// </summary>
	public partial class RaceSettings : Window
	{
		internal static Properties.Settings RS
		{
			get { return Properties.Settings.Default; }
		}

		public string RHeader { set; get; }
		public string PenalityShod {set; get; }
		public string Penality { set; get; }
		public string Attempts { set; get; }
		public string SavingPath { set; get; }
		public string ResultCalculationType { set; get; }
		public string RaceStartTime { set; get; }
        public string CommunicationPort { set; get; }
        public bool isHardwareStart { set; get; }
	
		public RaceSettings()
		{
			//load settings
			PenalityShod = RS.PenalityShod.ToString();//10 * 60;//seconds
			Penality = RS.Penality.ToString();//5;//seconds
			Attempts = RS.Attempts.ToString();//2;
			//ResultCalculationType = RS.ResultCalculationType;
			RaceStartTime = String.Format("{0:D2}:{1:D2}",
					RS.RaceStartTime.Hour,
					RS.RaceStartTime.Minute);

			RHeader = RS.RaceHeader;
			if(string.IsNullOrEmpty(RHeader))
				RHeader = "RB-Club.";

			SavingPath = RS.SavingPath;
			if(string.IsNullOrEmpty(SavingPath) ||
				!System.IO.Directory.Exists(SavingPath))
				SavingPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            isHardwareStart = RS.IsHardwareStart;

            CommunicationPort = RS.CommunicationPort;

			InitializeComponent();
		}

		private void main1_Loaded(object sender, RoutedEventArgs e)
        {
            var ports = System.IO.Ports.SerialPort.GetPortNames();
            if (ports != null && ports.Length > 0)
                cbComs.ItemsSource = ports;

			foreach(var i in cbCalcType.Items)
			{
				if((i as System.Windows.Controls.ComboBoxItem).Name == RS.ResultCalculationType)
					cbCalcType.SelectedItem = i;
			}
		}

		private void Apply_Click(object sender, RoutedEventArgs e)
		{
			//SaveSettings
			RS.PenalityShod = Int32.Parse(PenalityShod);
			RS.Penality = Int32.Parse(Penality);
			int tmp = Int32.Parse(Attempts);
			if(tmp != RS.Attempts)
				MessageBox.Show("Для применения новых настроек необходимо перезапустить программу",
					"Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
			RS.Attempts = tmp;
            if (RS.Attempts < 1)
                RS.Attempts = 1;
			RS.SavingPath = SavingPath;
			RS.RaceHeader = RHeader;
			RS.ResultCalculationType = ResultCalculationType;
			RS.RaceStartTime = DateTime.Parse(RaceStartTime);
            RS.IsHardwareStart = isHardwareStart;
            RS.CommunicationPort = CommunicationPort;
			RS.Save();

            HardwareCommunicator.ReInitialize();

			Close();
		}

		private void SelectPath_Click(object sender, RoutedEventArgs e)
		{
			using(var dlg = new System.Windows.Forms.FolderBrowserDialog())
			{
				if(dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					if(System.IO.Directory.Exists(dlg.SelectedPath))
					{
						SavingPath = dlg.SelectedPath;
						edSavingPath.Text = SavingPath;
					}
				}
			}
		}

		private void cbCalcType_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			if(cbCalcType.SelectedItem is System.Windows.Controls.ComboBoxItem)
				ResultCalculationType = (cbCalcType.SelectedItem as System.Windows.Controls.ComboBoxItem).Name;
		}

		//private void main1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		//{
		//    if(System.IO.Directory.Exists(SavingPath))
		//        RaceSettings1.Instance.SavingPath = SavingPath;
		//}
	}

    public class RaceSettings1 
    {
		public static TimeSpan CalcResult(clCompetitor clCompetitor)
		{
			var foo = GetCalcType(RaceSettings.RS.ResultCalculationType);
			return foo(clCompetitor);
		}
		/// <summary>
		/// Метод подсчёта результатов
		/// </summary>
		static public CalculationType GetCalcType(string name)
		{
			switch(name)
			{
				case "OneFastest":
					return OneFastest;
				case "OverallSum":
					return OverallSum;
				case "TwoFastest":
					return TwoFastest;
				default:
					return OneFastest;
			}
		}
		public delegate TimeSpan CalculationType(clCompetitor c);
		public static TimeSpan OneFastest(clCompetitor c)
		{
            TimeSpan min = GetSpanByAttempt(c.AttemptCollection[0]), tmp;
            foreach (var i in c.AttemptCollection)
            {
                tmp = GetSpanByAttempt(i);
                if (tmp < min)
                    min = tmp;
            }
            return min;
		}

		public static TimeSpan TwoFastest(clCompetitor c)
		{
            var list = new List<TimeSpan>();
            foreach (var i in c.AttemptCollection)
            {
                list.Add(GetSpanByAttempt(i));   
            }
            list.Sort();
            if (list.Count > 1)
                return list[0] + list[1];
            else
                return list[0];
		}

		public static TimeSpan OverallSum(clCompetitor c)
		{
            TimeSpan res = new TimeSpan();
            foreach (var i in c.AttemptCollection)
            {
                res = res.Add(GetSpanByAttempt(i));
            }
			return res;
		}

		private static TimeSpan GetSpanByAttempt(clCompetitor.AttemptResult att)
		{
			if(att.IsShod || att.IsNotStarted)
				return new TimeSpan(0, 0, RaceSettings.RS.PenalityShod);
			else
				return att.Time.Add(new TimeSpan(0, 0, att.PenaltyCount * RaceSettings.RS.Penality));
		}

		//

	}
}
