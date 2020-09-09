﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FietsDemo
{
    public partial class MainForm : Form
    {

        public MainForm()
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        public void setSpeed(double speed)
        {
            SpeedValue.Invoke((MethodInvoker)(() =>
            {
                SpeedValue.Text = speed + " m/s";
            }));
        }

        public void setHeartrate(int heartrate)
        {
            HeartrateValue.Invoke((MethodInvoker)(() =>
            {
                HeartrateValue.Text = heartrate + " bpm";
            }));
        }
    }
}
