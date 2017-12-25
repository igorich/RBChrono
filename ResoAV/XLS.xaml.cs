using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.IO;

namespace ResoAV
{
    /// <summary>
    /// Interaction logic for XLS.xaml
    /// </summary>
    public partial class XLS : Window
    {
        #region HTMLstrings
		private string html_body = "<HTML><HEAD><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"/><TITLE>Сводная таблица</TITLE></HEAD><BODY><DIV><H3 ALIGN=\"center\">{0}</H1><DIV><TABLE BORDER>{1}{2}</TABLE></DIV></BODY></HTML>";
		//private string page_header = "<H3 ALIGN=\"center\">{0}</H1>";
        private string html_header = "<TR><TH COLSPAN=6>{0}</TH></TR>";
		private string html_row = "<TR><TD><B>{0}</B></TD><TD><B>{1}</B></TD><TD>{2}</TD>{3}</TR>";
        private string html_table_header = String.Format("<TR><TD>{0}</TD><TD>{1}</TD><TD>{2}</TD><TD COLSPAN=6>{3}</TD></TR>",
            "Номер",
            "Имя",
            "Машина",
            "Результаты кругов и зачётный результат");
        #endregion
		private string _html;

        public XLS()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

		public void GeneratePageAndShow(List<KeyValuePair<string, IEnumerable<clCompetitor>>> arg)
		{
			var sb_all = new StringBuilder();
			foreach(var raceClass in arg)
			{
				sb_all.Append(String.Format(html_header, raceClass.Key));
                sb_all.Append(html_table_header);
                foreach (var man in raceClass.Value)
                {
                    var sb = new StringBuilder();
                    TimeSpan best_ts = man.AttemptCollection[0].Time;
                    for (int i = 1; i < man.AttemptCollection.Length; ++i)
                    {
                        if (man.AttemptCollection[i].Time < best_ts)
                            best_ts = man.AttemptCollection[i].Time;
                    }

                    foreach (var att in man.AttemptCollection)
                    {
                        if (att.Time == best_ts)
                            sb.Append(String.Format("<TD bgcolor=\"#B6FF00\">{0}</TD>", att.Time.ToResoAvView()));
                        else
                            sb.Append(String.Format("<TD>{0}</TD>", att.Time.ToResoAvView()));

                    }
                    sb.Append(String.Format("<TD bgcolor=\"#4169E1\">{0}</TD>", man.Result.ToResoAvView()));
                    sb_all.Append(String.Format(html_row, man.Number, man.Name, man.Model, sb.ToString()));
                }
			}

			_html = string.Format(html_body, RaceSettings.RS.RaceHeader, sb_all.ToString(), "");
			view.NavigateToString(_html);
		}

		internal void SaveHTML(string p)
		{
			int pos = p.LastIndexOf('.');
			p = p.Substring(0, pos) + ".html";
			using(StreamWriter outfile = new StreamWriter(p, false, Encoding.Unicode))
			{
				outfile.Write(_html);
			}
		}
	}
}
