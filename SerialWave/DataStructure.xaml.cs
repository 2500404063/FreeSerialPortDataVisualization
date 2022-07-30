using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace SerialWave
{
    /// <summary>
    /// DataStructure.xaml 的交互逻辑
    /// </summary>
    public partial class DataStructure : Window
    {
        public DataStructure()
        {
            InitializeComponent();
            if (GlobalConfig._OldEditedStructure != "")
            {
                this.textbox_structure.Text = GlobalConfig._OldEditedStructure;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GlobalConfig.WaveItems.Clear();
                GlobalConfig.Channels = 0;
                string raw = this.textbox_structure.Text;
                GlobalConfig._OldEditedStructure = raw;
                string[] rules = raw.Split(System.Environment.NewLine);
                int id = 0;
                foreach (string rule in rules)
                {
                    if (Regex.Match(rule, @"^\s*$").Success)
                    {
                        GlobalConfig.Channels++;
                        id = 0;
                    }
                    else
                    {
                        Regex regex = new(@"\S+?(?=\s)");
                        MatchCollection collection = regex.Matches(rule + " ");
                        WaveItem item = new()
                        {
                            Channel = GlobalConfig.Channels,
                            Id = id,
                            Type = collection[0].Value,
                            Label = collection[1].Value,
                            Color = collection[2].Value,
                        };
                        GlobalConfig.WaveItems.Add(item);
                        id++;
                    }
                }
                GlobalConfig.Channels = GlobalConfig.Channels + 1;
                Close();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }
    }
}
