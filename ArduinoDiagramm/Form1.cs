﻿using System;
using System.Collections.Generic;
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
        List<string> list = new List<string>(); 
        public Form1()
        {
            InitializeComponent();
            
            string[] portsname = SerialPort.GetPortNames();
            textBox1.Text = "30";
            chart1.Series.Clear();
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

                        string[] tempar = temp.Split(':');

                        if (tempar.Count() < 2) return;
                        if (tempar[0] != "" && tempar[0] != null && tempar[1] != "" && tempar[1] != null)
                        {
                            if (tempar[0][0] != '*' || tempar[0][tempar[0].Length - 1] != '*' || tempar[0].Length < 3) return;
                            if (!list.Contains(tempar[0]))
                            {
                                list.Add(tempar[0]);
                                Series series = new Series();
                                series.Name = tempar[0];
                                series.ChartType = SeriesChartType.FastLine;
                                series.XValueType = ChartValueType.Time;
                                series.BorderWidth = 3;
                                chart1.Invoke((ThreadStart)delegate { chart1.Series.Add(series); });
                            }
                            chart1.Invoke((ThreadStart)delegate { LineChart(tempar[1], list.IndexOf(tempar[0])); });
                        }

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

        public void LineChart(string x, int name)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(
                new Action<Form1>((sender) =>
                {
                    richTextBox1.AppendText(x);
                    richTextBox1.ScrollToCaret();
                    try
                    {
                        chart1.Series[name].Points.AddXY(DateTime.Now, Convert.ToInt32(x));
                    }
                    catch (FormatException data)
                    {
                        statusMessage(data.Message);
                    }
                    int max = 0;
                    foreach (Series ser in chart1.Series)
                    {
                        if (max < ser.Points.Count())
                            max = ser.Points.Count();
                    }
                    chart1.ChartAreas[0].AxisX.ScaleView.Zoom(max, GetSizeView(), DateTimeIntervalType.Seconds);
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
            if (portsname.Length == 0)
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

        private void chart1_Click(object sender, EventArgs e)
        {

        }
    }
}
