namespace Robot_GUI
{
    partial class Form1
    {
        /// <summary>
        /// Vyžaduje se proměnná návrháře.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Uvolněte všechny používané prostředky.
        /// </summary>
        /// <param name="disposing">hodnota true, když by se měl spravovaný prostředek odstranit; jinak false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Kód generovaný Návrhářem Windows Form

        /// <summary>
        /// Metoda vyžadovaná pro podporu Návrháře - neupravovat
        /// obsah této metody v editoru kódu.
        /// </summary>
        private void InitializeComponent()
        {
            this.Debugger_window = new System.Windows.Forms.TextBox();
            this.Turning_dir_check = new System.Windows.Forms.CheckBox();
            this.Turn_base_check = new System.Windows.Forms.CheckBox();
            this.Engine_main_arm_check = new System.Windows.Forms.CheckBox();
            this.Arm_grabber_check = new System.Windows.Forms.CheckBox();
            this.Grabber_check = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.frequency_textbox = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.Clock_Enable = new System.Windows.Forms.CheckBox();
            this.Controller_Mode = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.output = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Debugger_window
            // 
            this.Debugger_window.BackColor = System.Drawing.SystemColors.MenuText;
            this.Debugger_window.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.Debugger_window.ForeColor = System.Drawing.Color.Lime;
            this.Debugger_window.Location = new System.Drawing.Point(12, 207);
            this.Debugger_window.Multiline = true;
            this.Debugger_window.Name = "Debugger_window";
            this.Debugger_window.ReadOnly = true;
            this.Debugger_window.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.Debugger_window.Size = new System.Drawing.Size(354, 231);
            this.Debugger_window.TabIndex = 0;
            // 
            // Turning_dir_check
            // 
            this.Turning_dir_check.AutoSize = true;
            this.Turning_dir_check.Location = new System.Drawing.Point(21, 34);
            this.Turning_dir_check.Name = "Turning_dir_check";
            this.Turning_dir_check.Size = new System.Drawing.Size(90, 17);
            this.Turning_dir_check.TabIndex = 1;
            this.Turning_dir_check.Text = "Směr otáčení";
            this.Turning_dir_check.UseVisualStyleBackColor = true;
            // 
            // Turn_base_check
            // 
            this.Turn_base_check.AutoSize = true;
            this.Turn_base_check.Location = new System.Drawing.Point(21, 87);
            this.Turn_base_check.Name = "Turn_base_check";
            this.Turn_base_check.Size = new System.Drawing.Size(110, 17);
            this.Turn_base_check.TabIndex = 2;
            this.Turn_base_check.Text = "Otáčení základny";
            this.Turn_base_check.UseVisualStyleBackColor = true;
            // 
            // Engine_main_arm_check
            // 
            this.Engine_main_arm_check.AutoSize = true;
            this.Engine_main_arm_check.Location = new System.Drawing.Point(21, 134);
            this.Engine_main_arm_check.Name = "Engine_main_arm_check";
            this.Engine_main_arm_check.Size = new System.Drawing.Size(127, 17);
            this.Engine_main_arm_check.TabIndex = 3;
            this.Engine_main_arm_check.Text = "Motor: hlávní rameno";
            this.Engine_main_arm_check.UseVisualStyleBackColor = true;
            // 
            // Arm_grabber_check
            // 
            this.Arm_grabber_check.AutoSize = true;
            this.Arm_grabber_check.Location = new System.Drawing.Point(184, 87);
            this.Arm_grabber_check.Name = "Arm_grabber_check";
            this.Arm_grabber_check.Size = new System.Drawing.Size(141, 17);
            this.Arm_grabber_check.TabIndex = 4;
            this.Arm_grabber_check.Text = "Motor: rameno chapadlo";
            this.Arm_grabber_check.UseVisualStyleBackColor = true;
            // 
            // Grabber_check
            // 
            this.Grabber_check.AutoSize = true;
            this.Grabber_check.Location = new System.Drawing.Point(184, 134);
            this.Grabber_check.Name = "Grabber_check";
            this.Grabber_check.Size = new System.Drawing.Size(103, 17);
            this.Grabber_check.TabIndex = 5;
            this.Grabber_check.Text = "Motor: chapadlo";
            this.Grabber_check.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(593, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Změnit frekvenci";
            // 
            // frequency_textbox
            // 
            this.frequency_textbox.Location = new System.Drawing.Point(596, 59);
            this.frequency_textbox.Name = "frequency_textbox";
            this.frequency_textbox.Size = new System.Drawing.Size(100, 20);
            this.frequency_textbox.TabIndex = 7;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(713, 56);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 8;
            this.button1.Text = "Změnit";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Change_Frequency);
            // 
            // Clock_Enable
            // 
            this.Clock_Enable.AutoSize = true;
            this.Clock_Enable.Location = new System.Drawing.Point(596, 118);
            this.Clock_Enable.Name = "Clock_Enable";
            this.Clock_Enable.Size = new System.Drawing.Size(89, 17);
            this.Clock_Enable.TabIndex = 9;
            this.Clock_Enable.Text = "Clock Enable";
            this.Clock_Enable.UseVisualStyleBackColor = true;
            this.Clock_Enable.Click += new System.EventHandler(this.Clock_Enable_Changed);
            // 
            // Controller_Mode
            // 
            this.Controller_Mode.AutoSize = true;
            this.Controller_Mode.Location = new System.Drawing.Point(596, 217);
            this.Controller_Mode.Name = "Controller_Mode";
            this.Controller_Mode.Size = new System.Drawing.Size(55, 17);
            this.Controller_Mode.TabIndex = 10;
            this.Controller_Mode.Text = "Režim";
            this.Controller_Mode.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(593, 178);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 26);
            this.label2.TabIndex = 11;
            this.label2.Text = "Unchecked: Manuál\r\nChecked: Ovladač\r\n";
            // 
            // output
            // 
            this.output.Location = new System.Drawing.Point(372, 356);
            this.output.Name = "output";
            this.output.Size = new System.Drawing.Size(416, 73);
            this.output.TabIndex = 12;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.output);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.Controller_Mode);
            this.Controls.Add(this.Clock_Enable);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.frequency_textbox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Grabber_check);
            this.Controls.Add(this.Arm_grabber_check);
            this.Controls.Add(this.Engine_main_arm_check);
            this.Controls.Add(this.Turn_base_check);
            this.Controls.Add(this.Turning_dir_check);
            this.Controls.Add(this.Debugger_window);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.Text = "RobotGUI";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox Debugger_window;
        private System.Windows.Forms.CheckBox Turning_dir_check;
        private System.Windows.Forms.CheckBox Turn_base_check;
        private System.Windows.Forms.CheckBox Engine_main_arm_check;
        private System.Windows.Forms.CheckBox Arm_grabber_check;
        private System.Windows.Forms.CheckBox Grabber_check;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox frequency_textbox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox Clock_Enable;
        private System.Windows.Forms.CheckBox Controller_Mode;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label output;
    }
}

