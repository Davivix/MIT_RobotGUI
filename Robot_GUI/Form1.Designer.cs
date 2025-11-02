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
            this.label1 = new System.Windows.Forms.Label();
            this.frequency_textbox = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.Clock_Enable = new System.Windows.Forms.CheckBox();
            this.output = new System.Windows.Forms.Label();
            this.Reset_btn = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.Movement_Record_btn = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.Debugger_window = new System.Windows.Forms.RichTextBox();
            this.button4 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(593, 36);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Změnit frekvenci";
            // 
            // frequency_textbox
            // 
            this.frequency_textbox.Location = new System.Drawing.Point(595, 61);
            this.frequency_textbox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.frequency_textbox.Name = "frequency_textbox";
            this.frequency_textbox.Size = new System.Drawing.Size(100, 20);
            this.frequency_textbox.TabIndex = 7;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(713, 55);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(76, 23);
            this.button1.TabIndex = 8;
            this.button1.Text = "Změnit";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Change_Frequency_Click);
            // 
            // Clock_Enable
            // 
            this.Clock_Enable.AutoSize = true;
            this.Clock_Enable.Location = new System.Drawing.Point(599, 101);
            this.Clock_Enable.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Clock_Enable.Name = "Clock_Enable";
            this.Clock_Enable.Size = new System.Drawing.Size(98, 17);
            this.Clock_Enable.TabIndex = 9;
            this.Clock_Enable.Text = "Clock Enable";
            this.Clock_Enable.UseVisualStyleBackColor = true;
            this.Clock_Enable.Click += new System.EventHandler(this.Clock_Enable_Changed);
            // 
            // output
            // 
            this.output.Location = new System.Drawing.Point(479, 263);
            this.output.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.output.Name = "output";
            this.output.Size = new System.Drawing.Size(308, 75);
            this.output.TabIndex = 12;
            // 
            // Reset_btn
            // 
            this.Reset_btn.Location = new System.Drawing.Point(581, 140);
            this.Reset_btn.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Reset_btn.Name = "Reset_btn";
            this.Reset_btn.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Reset_btn.Size = new System.Drawing.Size(169, 23);
            this.Reset_btn.TabIndex = 13;
            this.Reset_btn.Text = "Reset do základní pozice";
            this.Reset_btn.UseVisualStyleBackColor = true;
            this.Reset_btn.Click += new System.EventHandler(this.Robot_Reset_Position_Click);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "txt";
            this.saveFileDialog1.Filter = "\"Text files (*.txt)|*.txt|All files (*.*)|*.*\"";
            // 
            // Movement_Record_btn
            // 
            this.Movement_Record_btn.Location = new System.Drawing.Point(581, 185);
            this.Movement_Record_btn.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Movement_Record_btn.Name = "Movement_Record_btn";
            this.Movement_Record_btn.Size = new System.Drawing.Size(169, 23);
            this.Movement_Record_btn.TabIndex = 14;
            this.Movement_Record_btn.Text = "Začít nahrávat pohyb";
            this.Movement_Record_btn.UseVisualStyleBackColor = true;
            this.Movement_Record_btn.Click += new System.EventHandler(this.Record_Movement_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "txt";
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "\"Text files (*.txt)|*.txt|All files (*.*)|*.*\"";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(713, 406);
            this.button2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(76, 23);
            this.button2.TabIndex = 16;
            this.button2.Text = "Manuál";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.Show_Manual_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(479, 40);
            this.button3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(92, 40);
            this.button3.TabIndex = 17;
            this.button3.Text = "STOP";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.Stop_Click);
            // 
            // Debugger_window
            // 
            this.Debugger_window.BackColor = System.Drawing.Color.Black;
            this.Debugger_window.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.Debugger_window.ForeColor = System.Drawing.Color.White;
            this.Debugger_window.Location = new System.Drawing.Point(12, 12);
            this.Debugger_window.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Debugger_window.Name = "Debugger_window";
            this.Debugger_window.ReadOnly = true;
            this.Debugger_window.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Horizontal;
            this.Debugger_window.ShortcutsEnabled = false;
            this.Debugger_window.Size = new System.Drawing.Size(444, 433);
            this.Debugger_window.TabIndex = 18;
            this.Debugger_window.TabStop = false;
            this.Debugger_window.Text = "";
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(581, 227);
            this.button4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button4.Name = "button4";
            this.button4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.button4.Size = new System.Drawing.Size(169, 23);
            this.button4.TabIndex = 19;
            this.button4.Text = "Načíst pohyb ze souboru";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.Load_File_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(799, 452);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.Debugger_window);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.Movement_Record_btn);
            this.Controls.Add(this.Reset_btn);
            this.Controls.Add(this.output);
            this.Controls.Add(this.Clock_Enable);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.frequency_textbox);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "Form1";
            this.Text = "RobotGUI";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.On_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox frequency_textbox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox Clock_Enable;
        private System.Windows.Forms.Label output;
        private System.Windows.Forms.Button Reset_btn;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button Movement_Record_btn;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.RichTextBox Debugger_window;
        private System.Windows.Forms.Button button4;
    }
}

