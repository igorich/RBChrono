using System;
using System.Windows.Data;
using System.Windows.Media;
using System.Globalization;

namespace ResoAV
{

    public static class MyExtensions
    {
        public static string ToResoAvView(this TimeSpan ts)
        {
            string res = ts.Minutes.ToString("D2")
               + ":" + ts.Seconds.ToString("D2")
               + "." + ts.Milliseconds.ToString("D3");
            return res;
        }
    }
	[ValueConversion(typeof(string), typeof(clCompetitor.AttemptResult[]))]
	public class AttemptListConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string str = parameter.ToString();

			int param = Int32.Parse(str);
			var collection = (clCompetitor.AttemptResult[])value;
			//return ((clCompetitor.AttemptResult[])value)[(int)parameter].ToString();
			if(param > collection.Length)
				return "";
			else
				return collection[param].ToString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	[ValueConversion(typeof(Brush), typeof(clCompetitor.carClass))]
	public class ColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if(value is clCompetitor.carClass)
			{
				switch((clCompetitor.carClass)value)
				{
					case clCompetitor.carClass.TUNING:
						return new SolidColorBrush(Color.FromRgb(233, 184, 180));
					case clCompetitor.carClass.SPORT:
						return new SolidColorBrush(Color.FromRgb(226, 205, 91));
                    case clCompetitor.carClass.RB_4x4:
                        return new SolidColorBrush(Color.FromRgb(212, 226, 191));
					default://clCompetitor.carClass.ABSOLUTE
						return new SolidColorBrush(Color.FromRgb(198, 216, 236));
					/*case clCompetitor.carClass.RB2000:
                        return new SolidColorBrush(Color.FromRgb(233, 184, 180));
                    case clCompetitor.carClass.RB1600:
                        return new SolidColorBrush(Color.FromRgb(226, 205, 91));
                    default://clCompetitor.carClass.ABSOLUTE
                        return new SolidColorBrush(Color.FromRgb(198, 216, 236));*/
				}
			}
			return new SolidColorBrush(Color.FromRgb(255, 255, 255));
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

}
