using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.IO;
using System.Globalization;
using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace ResoAV
{
	/// <summary>
	/// Interaction logic for Table.xaml
	/// самый главный пиздец огромный класс
	/// не дай б-г придётся рефакторить
	/// </summary>
	public partial class Table : Window, INotifyPropertyChanged
	{
		public ObservableCollection<clCompetitor> competitor { set; get; }
        public ContextMenu leftButtonMenu { set; get; }
		private TextBlock HwCountdown;
        //private XLS _w;

		public Table()
		{
            InterceptKeys.Main1();
            InterceptKeys.ResoavHookKey += new EventHandler<ResoavEventArgs>(InterceptKeys_ResoavHookKey);
			if(!InterceptKeys.KeysUnderLook.ContainsKey(System.Windows.Forms.Keys.D1))
			{
				InterceptKeys.KeysUnderLook.Add(System.Windows.Forms.Keys.D1, false);
				InterceptKeys.KeysUnderLook.Add(System.Windows.Forms.Keys.D2, false);
				InterceptKeys.KeysUnderLook.Add(System.Windows.Forms.Keys.D3, false);
				InterceptKeys.KeysUnderLook.Add(System.Windows.Forms.Keys.D4, false);
				InterceptKeys.KeysUnderLook.Add(System.Windows.Forms.Keys.D5, false);
				InterceptKeys.KeysUnderLook.Add(System.Windows.Forms.Keys.D6, false);
				InterceptKeys.KeysUnderLook.Add(System.Windows.Forms.Keys.D7, false);
				InterceptKeys.KeysUnderLook.Add(System.Windows.Forms.Keys.D8, false);
				InterceptKeys.KeysUnderLook.Add(System.Windows.Forms.Keys.D9, false);
			}

			competitor = new ObservableCollection<clCompetitor>();
#if DEBUG
            competitor.Add(new clCompetitor() { Number = 25, Name = "Иван Петров", Model = "ваз-2101", Class = clCompetitor.carClass.TUNING });
			competitor.Add(new clCompetitor() { Number = 28, Name = "Филип Рот", Model = "ваз-2102", Class = clCompetitor.carClass.STANDART });
			competitor.Add(new clCompetitor() { Number = 41, Name = "Барт Меткалф", Model = "ваз-21099", Class = clCompetitor.carClass.SPORT });
			competitor.Add(new clCompetitor() { Number = 43, Name = "Степанов Борис", Model = "ваз-2104", Class = clCompetitor.carClass.RB_4x4 });
			//competitor.Add(new clCompetitor() { Number = 44, Name = "Гленн Клоуз", Model = "ваз-2105", Class = clCompetitor.carClass.RB1600 });
			//competitor.Add(new clCompetitor() { Number = 5, Name = "Валерий  Леонтьев", Model = "ваз-2106", Class = clCompetitor.carClass.ABSOLUTE });
			//competitor.Add(new clCompetitor() { Number = 50, Name = "Надежда Заседателева", Model = "ваз-2108", Class = clCompetitor.carClass.RB2000 });
			//competitor.Add(new clCompetitor() { Number = 19, Name = "Брюс Уиллис", Model = "ваз-2108", Class = clCompetitor.carClass.RB4000 });
			//competitor.Add(new clCompetitor() { Number = 125, Name = "Егор Гайдар", Model = "ваз-2112", Class = clCompetitor.carClass.RB1600 });
#endif

            leftButtonMenu = new ContextMenu();
            var subMenu = new MenuItem();
            subMenu.Header = "Отправить в гонку";
            subMenu.Click += new RoutedEventHandler(menuToRace_Click);
            leftButtonMenu.Items.Add(subMenu);
            subMenu = new MenuItem();
            subMenu.Header = "Присудить сход";
            subMenu.Click += new RoutedEventHandler(menuShod_Click);
            leftButtonMenu.Items.Add(subMenu);
            subMenu = new MenuItem();
            subMenu.Header = "Не стартовал";
            subMenu.Click += new RoutedEventHandler(menuNS_Click);
            leftButtonMenu.Items.Add(subMenu);
            subMenu = new MenuItem();
            subMenu.Header = "Штраф +" + RaceSettings.RS.Penality + " сек";
            subMenu.Click += new RoutedEventHandler(menuPenality_Click);
            leftButtonMenu.Items.Add(subMenu);
            
			InitializeComponent();

            var temp = new clCompetitor();
            for (int i = 1; i <= temp.AttemptCollection.Length; ++i)
            {
                var v = new ColumnDefinition();
                v.Width = new GridLength(82);
                grUnderMain.ColumnDefinitions.Add(v);
                var t = new TextBlock();
                t.SetValue(Grid.ColumnProperty, 3 + i);
                t.Text = "Попытка " + i;
                t.FontWeight = FontWeights.Bold;
                t.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                t.TextAlignment = TextAlignment.Center;
                t.Background = new SolidColorBrush(Color.FromRgb
                    (System.Drawing.Color.LightGray.R, 
                    System.Drawing.Color.LightGray.G, 
                    System.Drawing.Color.LightGray.B));
                grUnderMain.Children.Add(t);
            }
            DataContext = this;
			lbRacingTitle.Text = RaceSettings.RS.RaceHeader;
            //HardwareCommunicator.ReInitialize();
		}

        void InterceptKeys_ResoavHookKey(object sender, ResoavEventArgs e)
        {
            //Keys.D1 = 49
            //throw new NotImplementedException();
            Console.WriteLine("Activate timer number: " + e.Key);
            if (spTimersPanel.Children.Count > (int)e.Key - 49)
            {
                var ch = (TimerControl)spTimersPanel.Children[(int)e.Key - 49];
                ch.DoAction();
            }

        }

        void menuNS_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mnu = sender as MenuItem;
            TextBlock targetCell = null;
            if (mnu != null && ((ContextMenu)mnu.Parent).PlacementTarget is TextBlock)
            {
                targetCell = ((ContextMenu)mnu.Parent).PlacementTarget as TextBlock;
				var man = (((targetCell.Parent as Border).Parent as Grid).DataContext as clCompetitor);
				
				int column = (int)targetCell.Parent.GetValue(Grid.ColumnProperty);
				if(column >= 0)
					man.AttemptModifyNS(column, true); 

				targetCell.Background = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                targetCell.Padding = new Thickness(5, 5, 15, 0);
            }
        }

        void menuShod_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mnu = sender as MenuItem;
            TextBlock targetCell = null;
            if (mnu != null && ((ContextMenu)mnu.Parent).PlacementTarget is TextBlock)
            {
                targetCell = ((ContextMenu)mnu.Parent).PlacementTarget as TextBlock;
				var man = (((targetCell.Parent as Border).Parent as Grid).DataContext as clCompetitor);
				
				int column = (int)targetCell.Parent.GetValue(Grid.ColumnProperty);
				if(column >= 0)
					man.AttemptModifyShod(column, true); 

                targetCell.Background = new SolidColorBrush(Color.FromRgb(0, 255, 0));
            }

        }

        void menuPenality_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mnu = sender as MenuItem;
            TextBlock targetCell = null;
            if (mnu != null && ((ContextMenu)mnu.Parent).PlacementTarget is TextBlock)
            {
                targetCell = ((ContextMenu)mnu.Parent).PlacementTarget as TextBlock;
				var man = (((targetCell.Parent as Border).Parent as Grid).DataContext as clCompetitor);
				
                int column = (int)targetCell.Parent.GetValue(Grid.ColumnProperty);
				if(column >= 0)
					man.AttemptModify(column, 1);
            }
        }

        void menuToRace_Click(object sender, RoutedEventArgs e)
		{
			MenuItem mnu = sender as MenuItem;
			TextBlock targetCell = null;
			if (mnu != null && ((ContextMenu)mnu.Parent).PlacementTarget is TextBlock)
			{
				targetCell = ((ContextMenu)mnu.Parent).PlacementTarget as TextBlock;
				var man = (((targetCell.Parent as Border).Parent as Grid).DataContext as clCompetitor);
				int column = (int)targetCell.Parent.GetValue(Grid.ColumnProperty);
				if(man != null && man.AllowForRace(column))
				{
                    //check uniq
                    foreach (var ch in spTimersPanel.Children)
                    {
                        if (ch is TimerControl)
                        {
                            if ((ch as TimerControl).ID == man.Number)
                                return;
                        }
                    }
                    //
					var tcNew = new TimerControl();
					tcNew.Init(ResultCallback, man);
					tcNew.Tag = targetCell;
					spTimersPanel.Children.Add(tcNew);
					((Border)targetCell.Parent).Background = new SolidColorBrush(Color.FromRgb(255, 255, 0));
				}
			}
		}

		public delegate void ResultCallbackDelegate(clCompetitor man, TimeSpan result, int penality, TextBlock targetCell);
		public void ResultCallback(clCompetitor man, TimeSpan result, int penality, TextBlock targetCell)
		{
			int column = (int)(targetCell.Parent as Border).GetValue(Grid.ColumnProperty);
			//int row = (int)(targetCell.Parent as Border).GetValue(Grid.RowProperty);
			if(column >= 0)
				man.AttemptModify(column, result);

			(targetCell.Parent as Border).Background = new SolidColorBrush(Color.FromRgb(0, 255, 0));
		}

		private void btGenerateResults_Click(object sender, RoutedEventArgs e)
		{
			//TODO: убрать это безобразие
			var listSTD = (from i in competitor
                           where i.Class == clCompetitor.carClass.STANDART
						   orderby i.Result
						   select i);
			var list4x4 = (from i in competitor
			               where i.Class == clCompetitor.carClass.RB_4x4
			               orderby i.Result
			               select i);
			var listSport = (from i in competitor
						   where i.Class == clCompetitor.carClass.SPORT
						   orderby i.Result
						   select i);
            var listABS = (from i in competitor
                           where i.Class == clCompetitor.carClass.TUNING
                           orderby i.Result
                           select i);
            string mydocpath = RaceSettings.RS.SavingPath;//Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			StringBuilder sb = new StringBuilder();

			string filename = string.Format("{0}\\ResoAv_results_{1}_{2}_{3}.csv", mydocpath, DateTime.Now.ToShortDateString(), DateTime.Now.Hour, DateTime.Now.Minute);
			using(StreamWriter outfile = new StreamWriter(filename, false, Encoding.Default))
			{
				outfile.WriteLine(String.Format("\"{0}\";\"{1}\";\"{2}\";\"{3}\";\"{4}\";\"{5}\";\"{6}\";\"{7}\"",
					"Позиция в классе",
					"Стартовый номер",
					"Класс",
					"Имя",
					"Автомобиль",
					"Пенализация",
					"Зачётный результат",
					"Результаты попыток"));
                PrintClassToGrid(listSTD, outfile);
                PrintClassToGrid(list4x4, outfile);
                PrintClassToGrid(listSport, outfile);
                outfile.WriteLine("");//отделить класс Абсолют от остального
                PrintClassToGrid(listABS, outfile);
			}

            //if(_w == null)
            var _w = new XLS();
			var q = new List<KeyValuePair<string, IEnumerable<clCompetitor>>>();

            q.Add(new KeyValuePair<string, IEnumerable<clCompetitor>>(clCompetitor.carClass.STANDART.ToString(), listSTD));
			q.Add(new KeyValuePair<string, IEnumerable<clCompetitor>>(clCompetitor.carClass.SPORT.ToString(), listSport));
            q.Add(new KeyValuePair<string, IEnumerable<clCompetitor>>(clCompetitor.carClass.RB_4x4.ToString(), list4x4));
            q.Add(new KeyValuePair<string, IEnumerable<clCompetitor>>(clCompetitor.carClass.TUNING.ToString(), listABS));
            _w.GeneratePageAndShow(q);
			_w.SaveHTML(filename);
            _w.Show();
		}

        private void PrintClassToGrid(IEnumerable<clCompetitor> list, StreamWriter file)
        {
            int internal_position = 1;
            foreach (var item in list)
            {
                file.Write(String.Format("\"{0}\";\"{1}\";\"{2}\";\"{3}\";\"{4}\";",
                    internal_position,
                    item.Number,
                    item.Class,
                    item.Name,
                    item.Model));
                file.Write(String.Format("\"{0}\";\"{1}\";", item.TotalPenality + "  sec",
                    item.Result.ToResoAvView()));
                foreach(var i in item.AttemptCollection)
                    file.Write(String.Format("\"{0}\";", i.Time.ToResoAvView()));
                file.WriteLine("");
                internal_position++;
            }
        }

        
		#region Члены INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if(handler != null) 
                handler(this, new PropertyChangedEventArgs(propertyName));
		} 
		#endregion

        private void Window_Closing(object sender, CancelEventArgs e)
        {			
			var r = MessageBox.Show("Вы действительно хотите закрыть программу?", "Выход", MessageBoxButton.OKCancel,MessageBoxImage.Question );
			if(r == MessageBoxResult.Cancel)
			{
				e.Cancel = true;
				return;
			}
			else
			{
				InterceptKeys.UnhookWindowsHookEx1();
				var w = this.Owner;
				if(w != null)
					(w as MainWindow).Deactivate();
			}

            HardwareCommunicator.Close();
        }

		private void TimeTextBlock_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			if(sender != null && sender is TextBlock)
			{
				(sender as TextBlock).Tag = (sender as TextBlock).Background;
				(sender as TextBlock).Background = new SolidColorBrush(Color.FromRgb(System.Drawing.Color.LightGray.R, System.Drawing.Color.LightGray.G, System.Drawing.Color.LightGray.B));
			}
		}

		private void TimeTextBlock_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{
			if(sender != null && sender is TextBlock)
			{
				(sender as TextBlock).Background = ((sender as TextBlock).Tag as SolidColorBrush);
			}
		}

		private void btGoToRegistration_Click(object sender, RoutedEventArgs e)
		{
			if(DateTime.Now.Hour >= RaceSettings.RS.RaceStartTime.Hour &&
				DateTime.Now.Minute > RaceSettings.RS.RaceStartTime.Minute)
			{
				MessageBox.Show("Время регистрации истекло", "ResoAV", MessageBoxButton.OK, MessageBoxImage.Stop);
				//return;
			}
			//if()//проверка времени, если больше чем в настройках - отказ
			var w = new MainWindow(competitor);
			w.ShowDialog();
		}

		#region Hardware start
		/*private delegate void HardwareStartDelegate(int st);

		public void ProcessHardwareStart(int st)
		{
			HardwareStartDelegate updateDelegate = new HardwareStartDelegate(ProcessHardwareStartGUIThread);
			Dispatcher.Invoke(updateDelegate, System.Windows.Threading.DispatcherPriority.Background, new object[] { st });
		}*/
		/// <summary>
		/// must run in GUI thread
		/// </summary>
		public void ProcessHardwareStartGUIThread(int state)
		{
            if (!RaceSettings.RS.IsHardwareStart)
                return;

			if(HwCountdown == null)
			{
				HwCountdown = new TextBlock { Background = new SolidColorBrush(Colors.Transparent) };
				HwCountdown.Foreground = new SolidColorBrush(Colors.Red);
				HwCountdown.SetValue(Grid.RowProperty, 2);
				HwCountdown.SetValue(Grid.ColumnProperty, 2);
				HwCountdown.VerticalAlignment = System.Windows.VerticalAlignment.Center;
				HwCountdown.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
				HwCountdown.FontSize = 400;
				HwCountdown.FontWeight = FontWeights.Bold;
				grGlobal.Children.Add(HwCountdown);
			}
			if(state > 0)
			{
				HwCountdown.Text = state.ToString();
			}
			else if(state == 0)
				HwCountdown.Text = "!";
			HwCountdown.Visibility = (state >= 0 ? Visibility.Visible : Visibility.Collapsed);
		}
		#endregion


		#region Menu
		private void MenuItem_Reset_Click(object sender, RoutedEventArgs e)
		{
			//var mbx = new MessageBox();

			competitor.Clear();
		}

		private void MenuItem_Options_Click(object sender, RoutedEventArgs e)
		{
			var setup = new RaceSettings();
			setup.ShowDialog();
			lbRacingTitle.Text = RaceSettings.RS.RaceHeader;	
		}

        private void MenuItem_About_Click(object sender, RoutedEventArgs e)
		{
            var w = new About();
            w.ShowDialog();
        }

        #region save file
		private void MenuItem_Save_Click(object sender, RoutedEventArgs e)
		{
			using(var dlg = new System.Windows.Forms.SaveFileDialog())
			{
				dlg.InitialDirectory = ".";
				dlg.Filter = "ResoAV files (*.rbc)|*.rbc|All files(*.*)|*.*";
				dlg.FilterIndex = 1;
				dlg.RestoreDirectory = true;
				if(dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					SaveCompetitorsAndSettingsToFile(dlg.FileName);
				}
			}
		}

		private void SaveSettingsToFile(TextWriter file)
		{
			file.WriteLine(String.Format("service_information;{0}; {1}", "Attempts", RaceSettings.RS.Attempts));
			file.WriteLine(String.Format("service_information;{0}; {1}", "Penality", RaceSettings.RS.Penality));
			file.WriteLine(String.Format("service_information;{0}; {1}", "PenalityShod", RaceSettings.RS.PenalityShod));
			file.WriteLine(String.Format("service_information;{0}; {1}", "RaceHeader", RaceSettings.RS.RaceHeader));
			file.WriteLine(String.Format("service_information;{0}; {1}", "RaceStartTime", RaceSettings.RS.RaceStartTime));
		}

		private void SaveCompetitorsAndSettingsToFile(string filepath)
		{
			using(TextWriter file = new StreamWriter(filepath))
			{
				SaveSettingsToFile(file);
				foreach(var cmp in competitor)
				{
					try
					{
						file.WriteLine(String.Format("{0};{1};{2};{3};", cmp.Number, cmp.Name, cmp.Model, cmp.Class));
					}
					catch(Exception ex)
					{
						System.Console.WriteLine(ex.Message + ", competitor num:" + cmp.Number);
					}
				}
			}
		}
		#endregion

		#region open file
		private void MenuItem_Open_Click(object sender, RoutedEventArgs e)
		{
			using(var dlg = new System.Windows.Forms.OpenFileDialog())
			{
				dlg.InitialDirectory = ".";
				dlg.Filter = "ResoAV files (*.rbc)|*.rbc|All files(*.*)|*.*";
				dlg.FilterIndex = 1;
				dlg.RestoreDirectory = true;
				if(dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
					LoadCompetitorsFromFile(dlg.FileName);
			}
		}

		private void LoadCompetitorsFromFile(string filepath)
		{
			using(TextReader file = new StreamReader(filepath))
			{
				string line = file.ReadLine();
				while(!string.IsNullOrEmpty(line))
				{
					var columns = line.Split(';');
					if(columns.Count() > 0 && columns[0] == "service_information")
					{
						SetSettings(columns);
					}
					else
					{
						try
						{
							var comp = new clCompetitor()
							{
								//Number = columns[0],
								Name = columns[1],
								Model = columns[2],
								//Class = columns[3];
							};
							comp.Number = Int32.Parse(columns[0]);
                            if (!competitor.Any(i => i.Number == comp.Number))
                            {
                                comp.Class = (clCompetitor.carClass)(Enum.Parse(typeof(clCompetitor.carClass), columns[3], true));
                                competitor.Add(comp);
                            }
						}
						catch(Exception ex)
						{
							System.Console.WriteLine(ex.Message);
						}
					}
					line = file.ReadLine();
				}

			}
            lbRacingTitle.Text = RaceSettings.RS.RaceHeader;
		}

		private void SetSettings(string[] columns)
		{
			try
			{
				if(columns.Count() < 3)
					return;
				switch(columns[1])
				{
					case "Attempts":
						RaceSettings.RS.Attempts = Int32.Parse(columns[2]);
						break;
					case "Penality":
						RaceSettings.RS.Penality = Int32.Parse(columns[2]);
						break;
					case "PenalityShod":
						RaceSettings.RS.PenalityShod = Int32.Parse(columns[2]);
						break;
					case "RaceHeader":
						RaceSettings.RS.RaceHeader = columns[2];
						break;
					case "RaceStartTime":
						RaceSettings.RS.RaceStartTime = DateTime.Parse(columns[2]);
						break;
				}
			}
			catch(Exception ex)
			{
				System.Console.WriteLine(ex.Message);
			}
		}
		#endregion

		#endregion

	}

}
