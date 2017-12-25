using System;
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.IO;

namespace ResoAV
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region other
        public delegate void StringArgDelegate(string s);
        //private Timer _timer1 = new Timer(), _timer2 = new Timer();
        //private Thread _ttimer1, _ttimer2;
        public ObservableCollection<clCompetitor> competitor { set; get; }

        public MainWindow(ObservableCollection<clCompetitor> arg)
        {
			competitor = arg;
            InitializeComponent();

            cmbClass.ItemsSource = Enum.GetNames(typeof(clCompetitor.carClass));
			cmbClass.SelectedIndex = 0;
 
            grReg.Visibility = System.Windows.Visibility.Visible;

			Properties.Settings ps = Properties.Settings.Default;
			this.Top = ps.Top;
			this.Left = ps.Left;
        }

		public void Deactivate()
		{
			grReg.IsEnabled = false;
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			Properties.Settings ps = Properties.Settings.Default;
			ps.Top = this.Top;
			ps.Left = this.Left;
			ps.Save();
		}

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void main_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //tbInp.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
        }

        //add
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int num;
            if (string.IsNullOrEmpty(edNum.Text))
                num = competitor.Select(i => i.Number).OrderByDescending(i => i).FirstOrDefault() + 1;
            else if(!Int32.TryParse(edNum.Text, out num))
			{
				//throw new Exception("Стартовый номер указан не верно. Введённое значение - не число");
                MessageBox.Show("Стартовый номер указан не верно. Введённое значение - не число", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}
            if (competitor.Any(i => i.Number == num))
            {
                MessageBox.Show("Участник с таким номером уже существует", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var c = new clCompetitor 
            { 
                Number = num,//Int32.Parse(edNum.Text),
                Name = edName.Text,
                Model = edCar.Text,
                Class = (clCompetitor.carClass)(Enum.Parse(typeof(clCompetitor.carClass), cmbClass.SelectedItem.ToString(), true)),
            };
            competitor.Add(c);//String.Format("{0}  {1}  {2}", edNum.Text, edName.Text, edClass.Text));
            edNum.Text = "";
            edName.Text = "";
            edCar.Text = "";
        }

        //go to race
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
			//competitor = new ObservableCollection<clCompetitor>(competitor.OrderBy(i => i.Class));
			//var w = new Table();
			//w.Owner = this;
			//w.Show();
			Close();
        }
        #endregion

		private void edNum_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			if(!this.IsLoaded)
				return;
			int i;
			if(!Int32.TryParse(edNum.Text, out i))
				brNum.BorderBrush = Brushes.Red;
			else
				brNum.BorderBrush = Brushes.Green;
		}


	}

}
