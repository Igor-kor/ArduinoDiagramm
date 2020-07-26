using System;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Threading;

namespace ArduinoDiagramm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            string[] portsname = SerialPort.GetPortNames();
            textBox1.Text = "30";
            foreach (string port in portsname)
            {
                comboBox1.Items.Add(port);
            }
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (e.EventType != SerialData.Chars) return;
            if (serialPort1.IsOpen)
            {
                try
                {
                    string temp = serialPort1.ReadLine();
                    if (temp != "" && temp != null)
                    {
                        chart1.Invoke((ThreadStart)delegate { LineChart(temp); });
                    }
                }
                catch (FormatException data)
                {
                    chart1.Invoke((ThreadStart)delegate { statusMessage(data.Message); });
                }
                catch (System.IO.IOException data)
                {
                    chart1.Invoke((ThreadStart)delegate { statusMessage(data.Message); });
                }
            }
        }

        private void statusMessage(string message)
        {
            label1.Text = message;
        }

        public void LineChart(string x)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(
                new Action<Form1>((sender) =>
                {
                    richTextBox1.Text += x;
                    try
                    {
                        chart1.Series[0].Points.AddXY(DateTime.Now, Convert.ToInt32(x));
                    }
                    catch (FormatException data)
                    {
                         statusMessage(data.Message);
                    }
                    chart1.ChartAreas[0].AxisX.ScaleView.Zoom(chart1.Series[0].Points.Count, GetSizeView(), DateTimeIntervalType.Seconds);
                }),
                new object[] { this });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.Open();
                statusMessage("Порт открыт");
            }
            catch (System.IO.IOException data)
            {
                statusMessage(data.Message);
            }
            catch (InvalidOperationException data)
            {
                statusMessage(data.Message);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            serialPort1.PortName = comboBox1.SelectedItem.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            serialPort1.Close();
            statusMessage("Порт закрыт");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            string[] portsname = SerialPort.GetPortNames();
            foreach (string port in portsname)
            {
                comboBox1.Items.Add(port);
            }
            if(portsname.Length == 0)
            {
                statusMessage("Доступные порты не найдены!");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            serialPort1.Close();
        }

        private double GetSizeView()
        {
            double size;
            try
            {
                size = Convert.ToDouble(textBox1.Text);
            }
            catch (FormatException)
            {
                size = 30;
            }
            if (size == 0) size = 30;
            return size;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {         
            chart1.ChartAreas[0].AxisX.ScaleView.Zoom(chart1.Series[0].Points.Count, GetSizeView(), DateTimeIntervalType.Seconds);
        }
    }
}
