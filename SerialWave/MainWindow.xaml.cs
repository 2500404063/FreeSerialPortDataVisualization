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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;
using System.Timers;

namespace SerialWave
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly SerialPort serialPort = new();

        delegate void FuncVoidNoParam();
        delegate void DataRecvUpdateHandler(byte[] data, int len, string[] val_str);
        delegate void DataListUpdateHandler(int channel, int id, object val);

        /*********************************************************
         * Serial Port
         *********************************************************/
        Timer timer_send = new();
        private void Timer_send_Elapsed(object? sender, ElapsedEventArgs e)
        {
            this.textbox_data_send.Dispatcher.Invoke(new FuncVoidNoParam(SendData));
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        public bool OpenSerialPort()
        {
            //Serial Port Open
            serialPort.BaudRate = GlobalConfig.BaudRate;
            serialPort.DataBits = 8;
            if (GlobalConfig.SerialName is not null)
            {
                serialPort.PortName = GlobalConfig.SerialName;
            }
            else
            {
                MessageBox.Show("Please Config Serial Port first!");
                return false;
            }
            serialPort.StopBits = StopBits.One;
            serialPort.Parity = Parity.None;
            serialPort.Open();
            serialPort.DataReceived += OnRecv;
            return serialPort.IsOpen;
        }

        public void CloseSerialPort()
        {
            serialPort.Close();
        }

        private void OnRecv(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                SerialPort obj = (SerialPort)sender;
                byte[] buffer = new byte[4096];
                int len = obj.BytesToRead;
                if (len > buffer.Length)
                {
                    len = 4096;
                }
                obj.Read(buffer, 0, len);
                //Decode Data
                int cur_pos = 0;
                List<string> val_str = new();
                for (int i = 0; i < GlobalConfig.WaveItems.Count; i++)
                {
                    int c = GlobalConfig.WaveItems[i].Channel;
                    int id = GlobalConfig.WaveItems[i].Id;
                    string label = GlobalConfig.WaveItems[i].Label;
                    switch (GlobalConfig.WaveItems[i].Type)
                    {
                        case "char":
                            byte char_val = buffer[cur_pos];
                            cur_pos += 1;
                            this.wave_tab.Dispatcher.Invoke(new DataListUpdateHandler(DataListUpdate), c, id, char_val);
                            val_str.Add(label + ": " + (((char_val >= 0x80 ? (int)char_val - 256 : (int)char_val))).ToString());
                            break;
                        case "uchar":
                            byte uchar_val = buffer[cur_pos];
                            cur_pos += 1;
                            this.wave_tab.Dispatcher.Invoke(new DataListUpdateHandler(DataListUpdate), c, id, uchar_val);
                            val_str.Add(label + ": " + ((int)uchar_val).ToString());
                            break;
                        case "short":
                            short short_val = 0;
                            if (GlobalConfig.LittleEnd)
                                short_val = BitConverter.ToInt16(buffer, cur_pos);
                            else
                                short_val = BitConverter.ToInt16(buffer.Skip(cur_pos).Take(2).Reverse().ToArray(), 0);
                            cur_pos += 2;
                            this.wave_tab.Dispatcher.Invoke(new DataListUpdateHandler(DataListUpdate), c, id, short_val);
                            val_str.Add(label + ": " + short_val.ToString());
                            break;
                        case "ushort":
                            ushort ushort_val = 0;
                            if (GlobalConfig.LittleEnd)
                                ushort_val = BitConverter.ToUInt16(buffer, cur_pos);
                            else
                                ushort_val = BitConverter.ToUInt16(buffer.Skip(cur_pos).Take(2).Reverse().ToArray(), 0);
                            cur_pos += 2;
                            this.wave_tab.Dispatcher.Invoke(new DataListUpdateHandler(DataListUpdate), c, id, ushort_val);
                            val_str.Add(label + ": " + ushort_val.ToString());
                            break;
                        case "int":
                            int int_val = 0;
                            if (GlobalConfig.LittleEnd)
                                int_val = BitConverter.ToInt32(buffer, cur_pos);
                            else
                                int_val = BitConverter.ToInt32(buffer.Skip(cur_pos).Take(4).Reverse().ToArray(), 0);
                            cur_pos += 4;
                            this.wave_tab.Dispatcher.Invoke(new DataListUpdateHandler(DataListUpdate), c, id, int_val);
                            val_str.Add(label + ": " + int_val.ToString());
                            break;
                        case "uint":
                            uint uint_val = 0;
                            if (GlobalConfig.LittleEnd)
                                uint_val = BitConverter.ToUInt32(buffer, cur_pos);
                            else
                                uint_val = BitConverter.ToUInt32(buffer.Skip(cur_pos).Take(4).Reverse().ToArray(), 0);
                            cur_pos += 4;
                            this.wave_tab.Dispatcher.Invoke(new DataListUpdateHandler(DataListUpdate), c, id, uint_val);
                            val_str.Add(label + ": " + uint_val.ToString());
                            break;
                        case "float":
                            float float_val = 0;
                            if (GlobalConfig.LittleEnd)
                                float_val = BitConverter.ToSingle(buffer, cur_pos);
                            else
                                float_val = BitConverter.ToSingle(buffer.Skip(cur_pos).Take(4).Reverse().ToArray(), 0);
                            cur_pos += 4;
                            this.wave_tab.Dispatcher.Invoke(new DataListUpdateHandler(DataListUpdate), c, id, float_val);
                            val_str.Add(label + ": " + float_val.ToString());
                            break;
                        case "double":
                            double double_val = 0;
                            if (GlobalConfig.LittleEnd)
                                double_val = BitConverter.ToDouble(buffer, cur_pos);
                            else
                                double_val = BitConverter.ToDouble(buffer.Skip(cur_pos).Take(8).Reverse().ToArray(), 0);
                            cur_pos += 8;
                            this.wave_tab.Dispatcher.Invoke(new DataListUpdateHandler(DataListUpdate), c, id, double_val);
                            val_str.Add(label + ": " + double_val.ToString());
                            break;
                        default:
                            MessageBox.Show("Unknow Type");
                            break;
                    }
                }
                this.textbox_data_recv.Dispatcher.Invoke(new DataRecvUpdateHandler(DataRecvUpdate), buffer, len, val_str.ToArray());
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        void DataListUpdate(int channel, int id, object val)
        {
            Coordinate coordinate = (Coordinate)(((TabItem)this.wave_tab.Items[1 + channel]).Content);
            coordinate.AddData(id, val);
            coordinate.ReDraw();
        }

        void DataRecvUpdate(byte[] data, int len, string[] val_str)
        {
            this.textbox_recv_count.Text = (int.Parse(this.textbox_recv_count.Text) + len).ToString();
            if (GlobalConfig.DecodeEnable && GlobalConfig.StructFormat)
            {
                this.textbox_data_recv.Text = "";
                foreach (string str in val_str)
                {
                    this.textbox_data_recv.Text += str + System.Environment.NewLine;
                }
            }
            else
            {
                if (GlobalConfig.ReadHex)
                {
                    string hex_data = Convert.ToHexString(data, 0, len);
                    for (int i = 0; i < hex_data.Length; i = i + 2)
                    {
                        this.textbox_data_recv.Text += hex_data[i];
                        this.textbox_data_recv.Text += hex_data[i + 1];
                        this.textbox_data_recv.Text += ' ';
                    }
                }
                else
                {
                    this.textbox_data_recv.Text += Encoding.UTF8.GetString(data, 0, len);
                }
            }
        }

        private void SendData()
        {
            if (this.checkbox_hex_send.IsChecked is not null)
            {
                //Hex
                if ((bool)this.checkbox_hex_send.IsChecked)
                {
                    try
                    {
                        string[] data = this.textbox_data_send.Text.Split(' ');
                        byte[] bytes = new byte[data.Length];
                        for (int i = 0; i < data.Length; i++)
                        {
                            if (data[i].Length == 2)
                            {
                                bytes[i] = Convert.ToByte(data[i], 16);
                            }
                            else
                            {
                                MessageBox.Show("Wrong Format to be sent. Format is AA BB CC");
                            }
                        }
                        serialPort.Write(bytes, 0, bytes.Length);
                        this.textbox_sent_count.Text = (int.Parse(this.textbox_sent_count.Text) + data.Length).ToString();
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show(err.Message);
                    }
                }
                //Text
                else
                {
                    byte[] data = Encoding.UTF8.GetBytes(this.textbox_data_send.Text);
                    serialPort.Write(data, 0, data.Length);
                    this.textbox_sent_count.Text = (int.Parse(this.textbox_sent_count.Text) + data.Length).ToString();
                }
            }
            else
            {
                MessageBox.Show("OMG? How can CheckBox be null?");
            }
        }

        /*********************************************************************************
         * Drawing
         *********************************************************************************/

        /**
         * Events
         */

        private void SerialOpen_Click(object sender, RoutedEventArgs e)
        {
            bool isOpen = OpenSerialPort();
            if (isOpen)
            {
                MessageBox.Show("Opened");
            }
        }

        private void SerialClose_Click(object sender, RoutedEventArgs e)
        {
            CloseSerialPort();
        }

        private void SerialSetting_Click(object sender, RoutedEventArgs e)
        {
            Setting setting = new();
            setting.ShowDialog();
        }

        private void CheckBox_ReadHex_Click(object sender, RoutedEventArgs e)
        {
            GlobalConfig.ReadHex = !GlobalConfig.ReadHex;
        }

        private void Button_Send_Click(object sender, RoutedEventArgs e)
        {
            SendData();
        }

        private void Button_KeepSend_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button button = (Button)sender;
                if ((string)button.Tag == "0")
                {
                    timer_send.Interval = int.Parse(this.send_interval.Text);
                    timer_send.Elapsed += Timer_send_Elapsed; ;
                    timer_send.Start();
                    button.Content = "Stop Sending";
                    button.Tag = "1";
                }
                else
                {
                    timer_send.Close();
                    button.Content = "Keep Sending";
                    button.Tag = "0";
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private void Label_Sent_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.textbox_sent_count.Text = "0";
        }

        private void Label_Recv_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.textbox_recv_count.Text = "0";
        }

        private void Sturct_Config_Click(object sender, RoutedEventArgs e)
        {
            DataStructure dataStructure = new();
            dataStructure.ShowDialog();
            int count = wave_tab.Items.Count - 1;
            for (int index = 0; index < count; index++)
            {
                wave_tab.Items.RemoveAt(1);
            }
            for (int i = 0; i < GlobalConfig.Channels; i++)
            {
                Coordinate coordinate = new Coordinate();
                for (int j = 0; j < GlobalConfig.WaveItems.Count; j++)
                {
                    if (GlobalConfig.WaveItems[j].Channel == i)
                    {
                        coordinate.AddFunction(
                            GlobalConfig.WaveItems[j].Label,
                            GlobalConfig.WaveItems[j].Type,
                            GlobalConfig.WaveItems[j].Color);
                        coordinate.ChangeBackgroundColor(GlobalConfig.BackgroundColor);
                    }
                }
                TabItem tabItem1 = new TabItem()
                {
                    Header = "Wave" + "(" + i.ToString() + ")",
                    Height = 24,
                    Width = 64,
                    Content = coordinate,
                };
                wave_tab.Items.Add(tabItem1);
            }
        }

        private void CheckBox_LittleEND_Click(object sender, RoutedEventArgs e)
        {
            GlobalConfig.LittleEnd = !GlobalConfig.LittleEnd;
        }

        private void CheckBox_Struct_Click(object sender, RoutedEventArgs e)
        {
            GlobalConfig.StructFormat = !GlobalConfig.StructFormat;
        }

        private void CheckBox_Decode_Click(object sender, RoutedEventArgs e)
        {
            GlobalConfig.DecodeEnable = !GlobalConfig.DecodeEnable;
        }

        private void MenuItem_Black_Click(object sender, RoutedEventArgs e)
        {
            GlobalConfig.BackgroundColor = "black";
        }

        private void MenuItem_White_Click(object sender, RoutedEventArgs e)
        {
            GlobalConfig.BackgroundColor = "white";
        }

        private void MenuItem_Gray_Click(object sender, RoutedEventArgs e)
        {
            GlobalConfig.BackgroundColor = "gray";
        }
    }
}
