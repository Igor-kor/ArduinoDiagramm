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
                string temp = serialPort1.ReadLine();

                if (temp != "" && temp != null && Convert.ToInt32(temp) != 0)
                {                   
                    chart1.Invoke((ThreadStart)delegate { LineChart(Convert.ToInt32(temp)); });
                }
            }
        }

        private void chart1_Click(object sender, EventArgs e)
        {
            chart1.Series.First().Points.Add(10);
        }
        public void LineChart(int x)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(
                new Action<Form1>((sender) => { 
                    chart1.Series[0].Points.Add(x); 
                    if (chart1.Series[0].Points.Count > 100)
                    {
                        chart1.Series[0].Points.RemoveAt(0); ;
                    }
                }),
                new object[] { this });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            serialPort1.Open();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            serialPort1.PortName = comboBox1.SelectedItem.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            serialPort1.Close();
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
