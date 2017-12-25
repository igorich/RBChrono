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
using System.Windows.Shapes;
using System.Reflection;
using System.Diagnostics;

namespace ResoAV
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();
            var ver = Assembly.GetExecutingAssembly().GetName().Version;
            lbVersion.Text = "ResoAV v." + ver;
        }

        private void TextBlockMail_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if(sender is TextBlock)
                Process.Start(String.Format("mailto:{0}?subject=ResoAV", (sender as TextBlock).Text));
        }
        private void TextBlockURL_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if(sender is TextBlock)
                Process.Start((sender as TextBlock).Text);
        }
    }
}
