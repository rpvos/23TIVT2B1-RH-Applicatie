namespace DoctorServer
{
    partial class DoctorForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.BikeListBox = new System.Windows.Forms.ListBox();
            this.ExitButton = new System.Windows.Forms.Button();
            this.Speed = new System.Windows.Forms.Label();
            this.SpeedValue = new System.Windows.Forms.Label();
            this.Heartrate = new System.Windows.Forms.Label();
            this.HeartrateValue = new System.Windows.Forms.Label();
            this.ElapsedTime = new System.Windows.Forms.Label();
            this.ElapsedTimeValue = new System.Windows.Forms.Label();
            this.DistanceTraveled = new System.Windows.Forms.Label();
            this.DistanceTraveledValue = new System.Windows.Forms.Label();
            this.Resistance = new System.Windows.Forms.Label();
            this.ResistanceValue = new System.Windows.Forms.Label();
            this.accumalatedPower = new System.Windows.Forms.Label();
            this.APLabel = new System.Windows.Forms.Label();
            this.plusResistance = new System.Windows.Forms.Button();
            this.minResistance = new System.Windows.Forms.Button();
            this.resistanceTextbox = new System.Windows.Forms.TextBox();
            this.PrivateChat = new System.Windows.Forms.ListBox();
            this.GlobalChat = new System.Windows.Forms.ListBox();
            this.PrivateChatBox = new System.Windows.Forms.TextBox();
            this.GlobalChatBox = new System.Windows.Forms.TextBox();
            this.privSendButton = new System.Windows.Forms.Button();
            this.globalSendButton = new System.Windows.Forms.Button();
            this.SessionButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.EmergencyStopButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // BikeListBox
            // 
            this.BikeListBox.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.BikeListBox.FormattingEnabled = true;
            this.BikeListBox.ItemHeight = 25;
            this.BikeListBox.Location = new System.Drawing.Point(2, 3);
            this.BikeListBox.Name = "BikeListBox";
            this.BikeListBox.Size = new System.Drawing.Size(100, 654);
            this.BikeListBox.TabIndex = 0;
            this.BikeListBox.SelectedIndexChanged += new System.EventHandler(this.BikeListBox_SelectedIndexChanged);
            // 
            // ExitButton
            // 
            this.ExitButton.BackColor = System.Drawing.Color.LightGray;
            this.ExitButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ExitButton.Location = new System.Drawing.Point(1252, 621);
            this.ExitButton.Name = "ExitButton";
            this.ExitButton.Size = new System.Drawing.Size(122, 40);
            this.ExitButton.TabIndex = 8;
            this.ExitButton.Text = "EXIT";
            this.ExitButton.UseVisualStyleBackColor = false;
            // 
            // Speed
            // 
            this.Speed.AutoSize = true;
            this.Speed.BackColor = System.Drawing.Color.White;
            this.Speed.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Speed.Location = new System.Drawing.Point(396, 15);
            this.Speed.Name = "Speed";
            this.Speed.Size = new System.Drawing.Size(177, 55);
            this.Speed.TabIndex = 0;
            this.Speed.Text = "Speed:";
            // 
            // SpeedValue
            // 
            this.SpeedValue.AutoSize = true;
            this.SpeedValue.BackColor = System.Drawing.Color.White;
            this.SpeedValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.SpeedValue.Location = new System.Drawing.Point(579, 15);
            this.SpeedValue.Name = "SpeedValue";
            this.SpeedValue.Size = new System.Drawing.Size(0, 55);
            this.SpeedValue.TabIndex = 1;
            // 
            // Heartrate
            // 
            this.Heartrate.AutoSize = true;
            this.Heartrate.BackColor = System.Drawing.Color.White;
            this.Heartrate.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Heartrate.Location = new System.Drawing.Point(335, 82);
            this.Heartrate.Name = "Heartrate";
            this.Heartrate.Size = new System.Drawing.Size(238, 55);
            this.Heartrate.TabIndex = 2;
            this.Heartrate.Text = "Heartrate:";
            // 
            // HeartrateValue
            // 
            this.HeartrateValue.AutoSize = true;
            this.HeartrateValue.BackColor = System.Drawing.Color.White;
            this.HeartrateValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.HeartrateValue.Location = new System.Drawing.Point(579, 82);
            this.HeartrateValue.Name = "HeartrateValue";
            this.HeartrateValue.Size = new System.Drawing.Size(0, 55);
            this.HeartrateValue.TabIndex = 3;
            // 
            // ElapsedTime
            // 
            this.ElapsedTime.AutoSize = true;
            this.ElapsedTime.BackColor = System.Drawing.Color.White;
            this.ElapsedTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ElapsedTime.Location = new System.Drawing.Point(243, 151);
            this.ElapsedTime.Name = "ElapsedTime";
            this.ElapsedTime.Size = new System.Drawing.Size(330, 55);
            this.ElapsedTime.TabIndex = 4;
            this.ElapsedTime.Text = "Elapsed Time:";
            // 
            // ElapsedTimeValue
            // 
            this.ElapsedTimeValue.AutoSize = true;
            this.ElapsedTimeValue.BackColor = System.Drawing.Color.White;
            this.ElapsedTimeValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ElapsedTimeValue.Location = new System.Drawing.Point(579, 151);
            this.ElapsedTimeValue.Name = "ElapsedTimeValue";
            this.ElapsedTimeValue.Size = new System.Drawing.Size(0, 55);
            this.ElapsedTimeValue.TabIndex = 5;
            // 
            // DistanceTraveled
            // 
            this.DistanceTraveled.AutoSize = true;
            this.DistanceTraveled.BackColor = System.Drawing.Color.White;
            this.DistanceTraveled.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.DistanceTraveled.Location = new System.Drawing.Point(149, 219);
            this.DistanceTraveled.Name = "DistanceTraveled";
            this.DistanceTraveled.Size = new System.Drawing.Size(424, 55);
            this.DistanceTraveled.TabIndex = 6;
            this.DistanceTraveled.Text = "Distance Traveled:";
            // 
            // DistanceTraveledValue
            // 
            this.DistanceTraveledValue.AutoSize = true;
            this.DistanceTraveledValue.BackColor = System.Drawing.Color.White;
            this.DistanceTraveledValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.DistanceTraveledValue.Location = new System.Drawing.Point(579, 219);
            this.DistanceTraveledValue.Name = "DistanceTraveledValue";
            this.DistanceTraveledValue.Size = new System.Drawing.Size(0, 55);
            this.DistanceTraveledValue.TabIndex = 7;
            // 
            // Resistance
            // 
            this.Resistance.AutoSize = true;
            this.Resistance.BackColor = System.Drawing.Color.White;
            this.Resistance.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Resistance.Location = new System.Drawing.Point(242, 287);
            this.Resistance.Name = "Resistance";
            this.Resistance.Size = new System.Drawing.Size(331, 55);
            this.Resistance.TabIndex = 9;
            this.Resistance.Text = "Resistance %:";
            // 
            // ResistanceValue
            // 
            this.ResistanceValue.AutoSize = true;
            this.ResistanceValue.BackColor = System.Drawing.Color.White;
            this.ResistanceValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ResistanceValue.Location = new System.Drawing.Point(579, 289);
            this.ResistanceValue.Name = "ResistanceValue";
            this.ResistanceValue.Size = new System.Drawing.Size(0, 55);
            this.ResistanceValue.TabIndex = 10;
            // 
            // accumalatedPower
            // 
            this.accumalatedPower.AutoSize = true;
            this.accumalatedPower.BackColor = System.Drawing.Color.White;
            this.accumalatedPower.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.accumalatedPower.Location = new System.Drawing.Point(108, 353);
            this.accumalatedPower.Name = "accumalatedPower";
            this.accumalatedPower.Size = new System.Drawing.Size(465, 55);
            this.accumalatedPower.TabIndex = 11;
            this.accumalatedPower.Text = "Accumalated Power:";
            // 
            // APLabel
            // 
            this.APLabel.AutoSize = true;
            this.APLabel.BackColor = System.Drawing.Color.White;
            this.APLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.APLabel.Location = new System.Drawing.Point(579, 353);
            this.APLabel.Name = "APLabel";
            this.APLabel.Size = new System.Drawing.Size(0, 55);
            this.APLabel.TabIndex = 12;
            // 
            // plusResistance
            // 
            this.plusResistance.BackColor = System.Drawing.Color.White;
            this.plusResistance.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.plusResistance.Location = new System.Drawing.Point(718, 293);
            this.plusResistance.Name = "plusResistance";
            this.plusResistance.Size = new System.Drawing.Size(50, 43);
            this.plusResistance.TabIndex = 13;
            this.plusResistance.Text = "+";
            this.plusResistance.UseVisualStyleBackColor = false;
            this.plusResistance.Click += new System.EventHandler(this.plusResistance_Click_1);
            // 
            // minResistance
            // 
            this.minResistance.BackColor = System.Drawing.Color.White;
            this.minResistance.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.minResistance.Location = new System.Drawing.Point(579, 293);
            this.minResistance.Name = "minResistance";
            this.minResistance.Size = new System.Drawing.Size(50, 43);
            this.minResistance.TabIndex = 14;
            this.minResistance.Text = "-";
            this.minResistance.UseVisualStyleBackColor = false;
            this.minResistance.Click += new System.EventHandler(this.minResistance_Click_1);
            // 
            // resistanceTextbox
            // 
            this.resistanceTextbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.resistanceTextbox.Location = new System.Drawing.Point(635, 293);
            this.resistanceTextbox.Name = "resistanceTextbox";
            this.resistanceTextbox.ReadOnly = true;
            this.resistanceTextbox.Size = new System.Drawing.Size(77, 44);
            this.resistanceTextbox.TabIndex = 15;
            this.resistanceTextbox.Text = "0";
            // 
            // PrivateChat
            // 
            this.PrivateChat.FormattingEnabled = true;
            this.PrivateChat.ItemHeight = 15;
            this.PrivateChat.Location = new System.Drawing.Point(865, 15);
            this.PrivateChat.Name = "PrivateChat";
            this.PrivateChat.Size = new System.Drawing.Size(285, 274);
            this.PrivateChat.TabIndex = 16;
            // 
            // GlobalChat
            // 
            this.GlobalChat.FormattingEnabled = true;
            this.GlobalChat.ItemHeight = 15;
            this.GlobalChat.Location = new System.Drawing.Point(1166, 15);
            this.GlobalChat.Name = "GlobalChat";
            this.GlobalChat.Size = new System.Drawing.Size(279, 274);
            this.GlobalChat.TabIndex = 16;
            // 
            // PrivateChatBox
            // 
            this.PrivateChatBox.Location = new System.Drawing.Point(865, 295);
            this.PrivateChatBox.Name = "PrivateChatBox";
            this.PrivateChatBox.Size = new System.Drawing.Size(225, 23);
            this.PrivateChatBox.TabIndex = 17;
            this.PrivateChatBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PrivKeyDown);
            // 
            // GlobalChatBox
            // 
            this.GlobalChatBox.Location = new System.Drawing.Point(1167, 296);
            this.GlobalChatBox.Name = "GlobalChatBox";
            this.GlobalChatBox.Size = new System.Drawing.Size(226, 23);
            this.GlobalChatBox.TabIndex = 17;
            this.GlobalChatBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GlobalKeyDown);
            // 
            // privSendButton
            // 
            this.privSendButton.Location = new System.Drawing.Point(1097, 295);
            this.privSendButton.Name = "privSendButton";
            this.privSendButton.Size = new System.Drawing.Size(53, 24);
            this.privSendButton.TabIndex = 18;
            this.privSendButton.Text = ">";
            this.privSendButton.UseVisualStyleBackColor = true;
            this.privSendButton.Click += new System.EventHandler(this.privSendButton_Click);
            // 
            // globalSendButton
            // 
            this.globalSendButton.Location = new System.Drawing.Point(1400, 296);
            this.globalSendButton.Name = "globalSendButton";
            this.globalSendButton.Size = new System.Drawing.Size(45, 23);
            this.globalSendButton.TabIndex = 19;
            this.globalSendButton.Text = ">";
            this.globalSendButton.UseVisualStyleBackColor = true;
            this.globalSendButton.Click += new System.EventHandler(this.globalSendButton_Click);
            // 
            // SessionButton
            // 
            this.SessionButton.Font = new System.Drawing.Font("Showcard Gothic", 18F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point);
            this.SessionButton.Location = new System.Drawing.Point(263, 430);
            this.SessionButton.Name = "SessionButton";
            this.SessionButton.Size = new System.Drawing.Size(215, 52);
            this.SessionButton.TabIndex = 20;
            this.SessionButton.Text = "Start session";
            this.SessionButton.UseVisualStyleBackColor = true;
            this.SessionButton.Click += new System.EventHandler(this.startSessionButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(943, 321);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(122, 30);
            this.label1.TabIndex = 22;
            this.label1.Text = "Private chat";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(1252, 322);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(118, 30);
            this.label2.TabIndex = 23;
            this.label2.Text = "Global chat";
            // 
            // EmergencyStopButton
            // 
            this.EmergencyStopButton.BackColor = System.Drawing.Color.Red;
            this.EmergencyStopButton.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.EmergencyStopButton.ForeColor = System.Drawing.Color.Black;
            this.EmergencyStopButton.Location = new System.Drawing.Point(614, 561);
            this.EmergencyStopButton.Name = "EmergencyStopButton";
            this.EmergencyStopButton.Size = new System.Drawing.Size(215, 55);
            this.EmergencyStopButton.TabIndex = 24;
            this.EmergencyStopButton.Text = "Emergency stop";
            this.EmergencyStopButton.UseVisualStyleBackColor = false;
            // 
            // DoctorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1513, 618);
            this.Controls.Add(this.EmergencyStopButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.SessionButton);
            this.Controls.Add(this.globalSendButton);
            this.Controls.Add(this.privSendButton);
            this.Controls.Add(this.GlobalChatBox);
            this.Controls.Add(this.PrivateChatBox);
            this.Controls.Add(this.GlobalChat);
            this.Controls.Add(this.PrivateChat);
            this.Controls.Add(this.resistanceTextbox);
            this.Controls.Add(this.minResistance);
            this.Controls.Add(this.plusResistance);
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
            this.Controls.Add(this.BikeListBox);
            this.Name = "DoctorForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox BikeListBox;
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
        private System.Windows.Forms.Button plusResistance;
        private System.Windows.Forms.Button minResistance;
        private System.Windows.Forms.TextBox resistanceTextbox;
        private System.Windows.Forms.ListBox PrivateChat;
        private System.Windows.Forms.ListBox GlobalChat;
        private System.Windows.Forms.TextBox PrivateChatBox;
        private System.Windows.Forms.TextBox GlobalChatBox;
        private System.Windows.Forms.Button privSendButton;
        private System.Windows.Forms.Button globalSendButton;
        private System.Windows.Forms.Button SessionButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button EmergencyStopButton;
    }
}

