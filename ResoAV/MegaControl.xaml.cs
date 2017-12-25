using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ResoAV
{
	/// <summary>
	/// Логика взаимодействия для MegaControl.xaml
	/// </summary>
	public partial class MegaControl : UserControl
	{
		public MegaControl()
		{
			InitializeComponent();
		}

		#region DependencyProperty Item
		/// <summary>
		/// Registers a dependency property as backing store for the Item property
		/// </summary>
		public static readonly DependencyProperty ItemProperty =
			DependencyProperty.Register("Item", typeof(clCompetitor), typeof(MegaControl),
			new FrameworkPropertyMetadata(null,
				  FrameworkPropertyMetadataOptions.AffectsRender |
				  FrameworkPropertyMetadataOptions.AffectsParentMeasure));

		public clCompetitor Item
		{
			get { return (clCompetitor)GetValue(ItemProperty); }
			set { SetValue(ItemProperty, value); }
		}
		#endregion	

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

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			this.DataContext = Item as ResoAV.clCompetitor;
			this.Width = 82 * (Item as ResoAV.clCompetitor).AttemptCollection.Length;

			var temp = new clCompetitor();
			for(int i = 0; i < temp.AttemptCollection.Length; ++i)
			{
				var v = new ColumnDefinition();
				v.Width = new GridLength(82);
				grMain.ColumnDefinitions.Add(v);
				var b = new Border();
				b.SetValue(Grid.ColumnProperty, i);
				b.BorderThickness= new Thickness(1, 1, 0, 0);
				//b.BorderBrush="Black";
				var t = new TextBlock();
				//t.FontWeight = FontWeights.Bold;
				t.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
				t.TextAlignment = TextAlignment.Center;
				t.Padding = new Thickness(0, 5, 0, 0);
				t.MouseEnter += TimeTextBlock_MouseEnter;
				t.MouseLeave += TimeTextBlock_MouseLeave;
				t.SetBinding(TextBlock.TextProperty, new Binding("AttemptCollection") 
				{ 
					Converter = new AttemptListConverter() , 
					ConverterParameter = i,
				});
				t.ContextMenu = this.ContextMenu;

				b.Child = t;
				grMain.Children.Add(b);
			}
		}

	}
}
