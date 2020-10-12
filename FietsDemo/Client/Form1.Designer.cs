namespace FietsDemo
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.Speed = new System.Windows.Forms.Label();
            this.SpeedValue = new System.Windows.Forms.Label();
            this.Heartrate = new System.Windows.Forms.Label();
            this.HeartrateValue = new System.Windows.Forms.Label();
            this.ElapsedTime = new System.Windows.Forms.Label();
            this.ElapsedTimeValue = new System.Windows.Forms.Label();
            this.DistanceTraveled = new System.Windows.Forms.Label();
            this.DistanceTraveledValue = new System.Windows.Forms.Label();
            this.ExitButton = new System.Windows.Forms.Button();
            this.Resistance = new System.Windows.Forms.Label();
            this.ResistanceValue = new System.Windows.Forms.Label();
            this.accumalatedPower = new System.Windows.Forms.Label();
            this.APLabel = new System.Windows.Forms.Label();
            this.ESPSimulatorButton = new System.Windows.Forms.Button();
            this.SoftwareSimulatorButton = new System.Windows.Forms.Button();
            this.ResistanceLabel = new System.Windows.Forms.Label();
            this.doctorChat = new System.Windows.Forms.ListBox();
            this.chatTextBox = new System.Windows.Forms.TextBox();
            this.sendDocButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Speed
            // 
            this.Speed.AutoSize = true;
            this.Speed.BackColor = System.Drawing.Color.White;
            this.Speed.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Speed.Location = new System.Drawing.Point(300, 83);
            this.Speed.Name = "Speed";
            this.Speed.Size = new System.Drawing.Size(177, 55);
            this.Speed.TabIndex = 0;
            this.Speed.Text = "Speed:";
            this.Speed.Click += new System.EventHandler(this.Speed_Click);
            // 
            // SpeedValue
            // 
            this.SpeedValue.AutoSize = true;
            this.SpeedValue.BackColor = System.Drawing.Color.White;
            this.SpeedValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SpeedValue.Location = new System.Drawing.Point(483, 83);
            this.SpeedValue.Name = "SpeedValue";
            this.SpeedValue.Size = new System.Drawing.Size(0, 55);
            this.SpeedValue.TabIndex = 1;
            // 
            // Heartrate
            // 
            this.Heartrate.AutoSize = true;
            this.Heartrate.BackColor = System.Drawing.Color.White;
            this.Heartrate.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Heartrate.Location = new System.Drawing.Point(239, 150);
            this.Heartrate.Name = "Heartrate";
            this.Heartrate.Size = new System.Drawing.Size(238, 55);
            this.Heartrate.TabIndex = 2;
            this.Heartrate.Text = "Heartrate:";
            // 
            // HeartrateValue
            // 
            this.HeartrateValue.AutoSize = true;
            this.HeartrateValue.BackColor = System.Drawing.Color.White;
            this.HeartrateValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HeartrateValue.Location = new System.Drawing.Point(483, 150);
            this.HeartrateValue.Name = "HeartrateValue";
            this.HeartrateValue.Size = new System.Drawing.Size(0, 55);
            this.HeartrateValue.TabIndex = 3;
            // 
            // ElapsedTime
            // 
            this.ElapsedTime.AutoSize = true;
            this.ElapsedTime.BackColor = System.Drawing.Color.White;
            this.ElapsedTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ElapsedTime.Location = new System.Drawing.Point(147, 219);
            this.ElapsedTime.Name = "ElapsedTime";
            this.ElapsedTime.Size = new System.Drawing.Size(330, 55);
            this.ElapsedTime.TabIndex = 4;
            this.ElapsedTime.Text = "Elapsed Time:";
            // 
            // ElapsedTimeValue
            // 
            this.ElapsedTimeValue.AutoSize = true;
            this.ElapsedTimeValue.BackColor = System.Drawing.Color.White;
            this.ElapsedTimeValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ElapsedTimeValue.Location = new System.Drawing.Point(483, 219);
            this.ElapsedTimeValue.Name = "ElapsedTimeValue";
            this.ElapsedTimeValue.Size = new System.Drawing.Size(0, 55);
            this.ElapsedTimeValue.TabIndex = 5;
            // 
            // DistanceTraveled
            // 
            this.DistanceTraveled.AutoSize = true;
            this.DistanceTraveled.BackColor = System.Drawing.Color.White;
            this.DistanceTraveled.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DistanceTraveled.Location = new System.Drawing.Point(53, 287);
            this.DistanceTraveled.Name = "DistanceTraveled";
            this.DistanceTraveled.Size = new System.Drawing.Size(424, 55);
            this.DistanceTraveled.TabIndex = 6;
            this.DistanceTraveled.Text = "Distance Traveled:";
            // 
            // DistanceTraveledValue
            // 
            this.DistanceTraveledValue.AutoSize = true;
            this.DistanceTraveledValue.BackColor = System.Drawing.Color.White;
            this.DistanceTraveledValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DistanceTraveledValue.Location = new System.Drawing.Point(483, 287);
            this.DistanceTraveledValue.Name = "DistanceTraveledValue";
            this.DistanceTraveledValue.Size = new System.Drawing.Size(0, 55);
            this.DistanceTraveledValue.TabIndex = 7;
            // 
            // ExitButton
            // 
            this.ExitButton.BackColor = System.Drawing.Color.LightGray;
            this.ExitButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ExitButton.Location = new System.Drawing.Point(1252, 621);
            this.ExitButton.Name = "ExitButton";
            this.ExitButton.Size = new System.Drawing.Size(122, 40);
            this.ExitButton.TabIndex = 8;
            this.ExitButton.Text = "EXIT";
            this.ExitButton.UseVisualStyleBackColor = false;
            this.ExitButton.Click += new System.EventHandler(this.exitButton_Click);
            // 
            // Resistance
            // 
            this.Resistance.AutoSize = true;
            this.Resistance.BackColor = System.Drawing.Color.White;
            this.Resistance.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Resistance.Location = new System.Drawing.Point(146, 355);
            this.Resistance.Name = "Resistance";
            this.Resistance.Size = new System.Drawing.Size(331, 55);
            this.Resistance.TabIndex = 9;
            this.Resistance.Text = "Resistance %:";
            // 
            // ResistanceValue
            // 
            this.ResistanceValue.AutoSize = true;
            this.ResistanceValue.BackColor = System.Drawing.Color.White;
            this.ResistanceValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ResistanceValue.Location = new System.Drawing.Point(483, 357);
            this.ResistanceValue.Name = "ResistanceValue";
            this.ResistanceValue.Size = new System.Drawing.Size(0, 55);
            this.ResistanceValue.TabIndex = 10;
            // 
            // accumalatedPower
            // 
            this.accumalatedPower.AutoSize = true;
            this.accumalatedPower.BackColor = System.Drawing.Color.White;
            this.accumalatedPower.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.accumalatedPower.Location = new System.Drawing.Point(12, 421);
            this.accumalatedPower.Name = "accumalatedPower";
            this.accumalatedPower.Size = new System.Drawing.Size(465, 55);
            this.accumalatedPower.TabIndex = 11;
            this.accumalatedPower.Text = "Accumalated Power:";
            // 
            // APLabel
            // 
            this.APLabel.AutoSize = true;
            this.APLabel.BackColor = System.Drawing.Color.White;
            this.APLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.APLabel.Location = new System.Drawing.Point(483, 421);
            this.APLabel.Name = "APLabel";
            this.APLabel.Size = new System.Drawing.Size(0, 55);
            this.APLabel.TabIndex = 12;
            // 
            // ESPSimulatorButton
            // 
            this.ESPSimulatorButton.BackColor = System.Drawing.Color.LightBlue;
            this.ESPSimulatorButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ESPSimulatorButton.Location = new System.Drawing.Point(1188, 83);
            this.ESPSimulatorButton.Name = "ESPSimulatorButton";
            this.ESPSimulatorButton.Size = new System.Drawing.Size(203, 50);
            this.ESPSimulatorButton.TabIndex = 16;
            this.ESPSimulatorButton.Text = "ESP Simulator";
            this.ESPSimulatorButton.UseVisualStyleBackColor = false;
            this.ESPSimulatorButton.Click += new System.EventHandler(this.ESPSimulatorButton_Click);
            // 
            // SoftwareSimulatorButton
            // 
            this.SoftwareSimulatorButton.BackColor = System.Drawing.Color.White;
            this.SoftwareSimulatorButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SoftwareSimulatorButton.Location = new System.Drawing.Point(1133, 27);
            this.SoftwareSimulatorButton.Name = "SoftwareSimulatorButton";
            this.SoftwareSimulatorButton.Size = new System.Drawing.Size(258, 50);
            this.SoftwareSimulatorButton.TabIndex = 17;
            this.SoftwareSimulatorButton.Text = "Software Simulator";
            this.SoftwareSimulatorButton.UseVisualStyleBackColor = false;
            this.SoftwareSimulatorButton.Click += new System.EventHandler(this.SoftwareSimulatorButton_Click);
            // 
            // ResistanceLabel
            // 
            this.ResistanceLabel.AutoSize = true;
            this.ResistanceLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ResistanceLabel.Location = new System.Drawing.Point(490, 357);
            this.ResistanceLabel.Name = "ResistanceLabel";
            this.ResistanceLabel.Size = new System.Drawing.Size(0, 55);
            this.ResistanceLabel.TabIndex = 18;
            // 
            // doctorChat
            // 
            this.doctorChat.FormattingEnabled = true;
            this.doctorChat.Location = new System.Drawing.Point(1133, 166);
            this.doctorChat.Name = "doctorChat";
            this.doctorChat.Size = new System.Drawing.Size(247, 277);
            this.doctorChat.TabIndex = 19;
            // 
            // chatTextBox
            // 
            this.chatTextBox.Location = new System.Drawing.Point(1133, 450);
            this.chatTextBox.Name = "chatTextBox";
            this.chatTextBox.Size = new System.Drawing.Size(204, 20);
            this.chatTextBox.TabIndex = 20;
            // 
            // sendDocButton
            // 
            this.sendDocButton.Location = new System.Drawing.Point(1343, 450);
            this.sendDocButton.Name = "sendDocButton";
            this.sendDocButton.Size = new System.Drawing.Size(37, 20);
            this.sendDocButton.TabIndex = 21;
            this.sendDocButton.Text = ">";
            this.sendDocButton.UseVisualStyleBackColor = true;
            this.sendDocButton.Click += new System.EventHandler(this.sendDocButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(1403, 667);
            this.Controls.Add(this.sendDocButton);
            this.Controls.Add(this.chatTextBox);
            this.Controls.Add(this.doctorChat);
            this.Controls.Add(this.ResistanceLabel);
            this.Controls.Add(this.SoftwareSimulatorButton);
            this.Controls.Add(this.ESPSimulatorButton);
            this.Controls.Add(this.APLabel);
            this.Controls.Add(this.accumalatedPower);
            this.Controls.Add(this.ResistanceValue);
            this.Controls.Add(this.Resistance);
            this.Controls.Add(this.ExitButton);
            this.Controls.Add(this.DistanceTraveledValue);
            this.Controls.Add(this.DistanceTraveled);
            this.Controls.Add(this.ElapsedTimeValue);
            this.Controls.Add(this.ElapsedTime);
            this.Controls.Add(this.HeartrateValue);
            this.Controls.Add(this.Heartrate);
            this.Controls.Add(this.SpeedValue);
            this.Controls.Add(this.Speed);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Speed;
        private System.Windows.Forms.Label SpeedValue;
        private System.Windows.Forms.Label Heartrate;
        private System.Windows.Forms.Label HeartrateValue;
        private System.Windows.Forms.Label ElapsedTime;
        private System.Windows.Forms.Label ElapsedTimeValue;
        private System.Windows.Forms.Label DistanceTraveled;
        private System.Windows.Forms.Label DistanceTraveledValue;
        private System.Windows.Forms.Button ExitButton;
        private System.Windows.Forms.Label Resistance;
        private System.Windows.Forms.Label ResistanceValue;
        private System.Windows.Forms.Label accumalatedPower;
        private System.Windows.Forms.Label APLabel;
        private System.Windows.Forms.Button ESPSimulatorButton;
        private System.Windows.Forms.Button SoftwareSimulatorButton;
        private System.Windows.Forms.Label ResistanceLabel;
        private System.Windows.Forms.ListBox doctorChat;
        private System.Windows.Forms.TextBox chatTextBox;
        private System.Windows.Forms.Button sendDocButton;
    }
}