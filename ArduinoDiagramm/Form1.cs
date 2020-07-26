using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace ArduinoDiagramm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            string[] portsname = SerialPort.GetPortNames();
            foreach (string port in portsname)
            {
                comboBox1.Items.Add(port);
            }
        }
        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {

            if (e.EventType != System.IO.Ports.SerialData.Chars) return;
            if (serialPort1.IsOpen)
            {
                try
                {
                    string temp = serialPort1.ReadLine();
                    if (temp != "" && temp != null && Convert.ToInt32(temp) != 0)
                    {
                        chart1.Invoke((ThreadStart)delegate { LineChart(Convert.ToInt32(temp)); });
                    }
                }
                catch (FormatException data)
                {
                    chart1.Invoke((ThreadStart)delegate { statusMessage(data.Message);});
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

        private void chart1_Click(object sender, EventArgs e)
        {
            chart1.Series.First().Points.Add(10);
        }
        public void LineChart(int x)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(
                new Action<Form1>((sender) =>
                {

                    chart1.Series[0].Points.Add(x);
                    var i = chart1.Series[0].Points.Count > chart1.ChartAreas[0].AxisX.Interval ? chart1.Series[0].Points.Count : chart1.ChartAreas[0].AxisX.Interval;
                    chart1.ChartAreas[0].AxisX.ScaleView.Zoom(i - chart1.ChartAreas[0].AxisX.Interval, i);
                }),
                new object[] { this });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.Open();
                label1.Text = "Порт открыт";
            }
            catch (System.IO.IOException data)
            {
                label1.Text = data.Message;
            }catch(System.InvalidOperationException data)
            {
                label1.Text = data.Message;
            }

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            serialPort1.PortName = comboBox1.SelectedItem.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            serialPort1.Close();
            label1.Text = "Порт закрыт";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            string[] portsname = SerialPort.GetPortNames();
            foreach (string port in portsname)
            {
                comboBox1.Items.Add(port);
            }
        }

    }
}
