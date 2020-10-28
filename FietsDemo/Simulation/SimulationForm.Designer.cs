namespace FietsDemo
{
    partial class SimulationForm
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
            this.SpeedLabel = new System.Windows.Forms.Label();
            this.SpeedTextBox = new System.Windows.Forms.TextBox();
            this.plusSpeed = new System.Windows.Forms.Button();
            this.minSpeed = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.HeartrateLabel = new System.Windows.Forms.Label();
            this.HeartrateTextBox = new System.Windows.Forms.TextBox();
            this.plusHeartrate = new System.Windows.Forms.Button();
            this.minHeartrate = new System.Windows.Forms.Button();
            this.ResistanceLabel = new System.Windows.Forms.Label();
            this.ResistanceValue = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // SpeedLabel
            // 
            this.SpeedLabel.AutoSize = true;
            this.SpeedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SpeedLabel.Location = new System.Drawing.Point(64, 74);
            this.SpeedLabel.Name = "SpeedLabel";
            this.SpeedLabel.Size = new System.Drawing.Size(161, 31);
            this.SpeedLabel.TabIndex = 0;
            this.SpeedLabel.Text = "Speed (m/s)";
            // 
            // SpeedTextBox
            // 
            this.SpeedTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SpeedTextBox.Location = new System.Drawing.Point(102, 114);
            this.SpeedTextBox.Name = "SpeedTextBox";
            this.SpeedTextBox.ReadOnly = true;
            this.SpeedTextBox.Size = new System.Drawing.Size(83, 38);
            this.SpeedTextBox.TabIndex = 1;
            this.SpeedTextBox.Text = "0";
            // 
            // plusSpeed
            // 
            this.plusSpeed.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.plusSpeed.Location = new System.Drawing.Point(191, 118);
            this.plusSpeed.Name = "plusSpeed";
            this.plusSpeed.Size = new System.Drawing.Size(30, 29);
            this.plusSpeed.TabIndex = 2;
            this.plusSpeed.Text = "+";
            this.plusSpeed.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.plusSpeed.UseVisualStyleBackColor = true;
            this.plusSpeed.Click += new System.EventHandler(this.plusSpeed_click);
            // 
            // minSpeed
            // 
            this.minSpeed.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.minSpeed.Location = new System.Drawing.Point(66, 118);
            this.minSpeed.Name = "minSpeed";
            this.minSpeed.Size = new System.Drawing.Size(30, 29);
            this.minSpeed.TabIndex = 3;
            this.minSpeed.Text = "-";
            this.minSpeed.UseVisualStyleBackColor = true;
            this.minSpeed.Click += new System.EventHandler(this.minusSpeed_click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(63, 102);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 13);
            this.label2.TabIndex = 4;
            // 
            // HeartrateLabel
            // 
            this.HeartrateLabel.AutoSize = true;
            this.HeartrateLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HeartrateLabel.Location = new System.Drawing.Point(282, 74);
            this.HeartrateLabel.Name = "HeartrateLabel";
            this.HeartrateLabel.Size = new System.Drawing.Size(198, 31);
            this.HeartrateLabel.TabIndex = 5;
            this.HeartrateLabel.Text = "Heartrate(bpm)";
            // 
            // HeartrateTextBox
            // 
            this.HeartrateTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HeartrateTextBox.Location = new System.Drawing.Point(338, 114);
            this.HeartrateTextBox.Name = "HeartrateTextBox";
            this.HeartrateTextBox.ReadOnly = true;
            this.HeartrateTextBox.Size = new System.Drawing.Size(83, 38);
            this.HeartrateTextBox.TabIndex = 6;
            this.HeartrateTextBox.Text = "50";
            // 
            // plusHeartrate
            // 
            this.plusHeartrate.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.plusHeartrate.Location = new System.Drawing.Point(427, 120);
            this.plusHeartrate.Name = "plusHeartrate";
            this.plusHeartrate.Size = new System.Drawing.Size(30, 29);
            this.plusHeartrate.TabIndex = 7;
            this.plusHeartrate.Text = "+";
            this.plusHeartrate.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.plusHeartrate.UseVisualStyleBackColor = true;
            this.plusHeartrate.Click += new System.EventHandler(this.plusHeartrate_Click);
            // 
            // minHeartrate
            // 
            this.minHeartrate.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.minHeartrate.Location = new System.Drawing.Point(302, 120);
            this.minHeartrate.Name = "minHeartrate";
            this.minHeartrate.Size = new System.Drawing.Size(30, 29);
            this.minHeartrate.TabIndex = 8;
            this.minHeartrate.Text = "-";
            this.minHeartrate.UseVisualStyleBackColor = true;
            this.minHeartrate.Click += new System.EventHandler(this.minHeartrate_Click);
            // 
            // ResistanceLabel
            // 
            this.ResistanceLabel.AutoSize = true;
            this.ResistanceLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ResistanceLabel.Location = new System.Drawing.Point(96, 186);
            this.ResistanceLabel.Name = "ResistanceLabel";
            this.ResistanceLabel.Size = new System.Drawing.Size(210, 33);
            this.ResistanceLabel.TabIndex = 9;
            this.ResistanceLabel.Text = "Resistance %: ";
            // 
            // ResistanceValue
            // 
            this.ResistanceValue.AutoSize = true;
            this.ResistanceValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ResistanceValue.Location = new System.Drawing.Point(299, 186);
            this.ResistanceValue.Name = "ResistanceValue";
            this.ResistanceValue.Size = new System.Drawing.Size(0, 33);
            this.ResistanceValue.TabIndex = 10;
            // 
            // SimulationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(544, 257);
            this.Controls.Add(this.ResistanceValue);
            this.Controls.Add(this.ResistanceLabel);
            this.Controls.Add(this.minHeartrate);
            this.Controls.Add(this.plusHeartrate);
            this.Controls.Add(this.HeartrateTextBox);
            this.Controls.Add(this.HeartrateLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.minSpeed);
            this.Controls.Add(this.plusSpeed);
            this.Controls.Add(this.SpeedTextBox);
            this.Controls.Add(this.SpeedLabel);
            this.Name = "SimulationForm";
            this.Text = "SimulationForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label SpeedLabel;
        private System.Windows.Forms.TextBox SpeedTextBox;
        private System.Windows.Forms.Button plusSpeed;
        private System.Windows.Forms.Button minSpeed;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label HeartrateLabel;
        private System.Windows.Forms.TextBox HeartrateTextBox;
        private System.Windows.Forms.Button plusHeartrate;
        private System.Windows.Forms.Button minHeartrate;
        private System.Windows.Forms.Label ResistanceLabel;
        private System.Windows.Forms.Label ResistanceValue;
    }
}