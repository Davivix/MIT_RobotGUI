using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Robot_GUI
{
    public partial class Form1 : Form
    {
        private Robot robot;
        private Controller controller;
        private Clock clock;

        public CheckBox[] Manual_Inputs;
        public bool Turning_dir, Turn_base, Engine_main_arm, Grabber, Arm_grabber;

        public Form1()
        {
            InitializeComponent();

            robot = new Robot("PCIE-1730,BID#0", "PCIE-1730_profile.xml", ref Debugger_window);
            controller = new Controller();
            clock = new Clock(Update_Robot_Inputs);

            Manual_Inputs = new CheckBox[] { Turning_dir_check, Turn_base_check, Engine_main_arm_check, Grabber_check, Arm_grabber_check };

            Load += Form_Load;
            FormClosing += Form1_FormClosing;
        }

        private void Form_Load(object sender, EventArgs e)
        {
            output.Text = $"Aktuální hodnota na vstupu robota: {0}\r\nInterval: {clock.Interval_ms * 2}ms\r\nFrekvence: {clock.Frequency}Hz";
        }

        private void Robot_Reset_Position(object sender, EventArgs e)
        {
            robot.Reset_Default_Position(clock.Interval_ms);
        }

        private void Update_Robot_Inputs()
        {
            // získání stavu vstupů uživatele
            bool[] user_inputs;
            if (Controller_Mode.Checked)
                user_inputs = controller.Return_Button_Data();
            else
                user_inputs = Manual_Inputs.Select(c => c.Checked).ToArray();

            // výpočet hodnoty k zápisu pomocí stavu vstupů uživatele
            byte input_value = Robot.Get_Next_Input_Value(user_inputs, clock.ClockSignal);
            input_value = (byte)~input_value;
            // zápis na výstup IO karty
            robot.Write_Input(input_value);
            byte output_value = robot.Read_Output();

            // Update UI
            output.Invoke((MethodInvoker)delegate
            {
                output.Text = $"Aktuální hodnota na vstupu robota: {robot.Input_value}Aktuální hodnota na výstupu robota: {0}\r\nInterval: {clock.Interval_ms * 2}ms\r\nFrekvence: {clock.Frequency}Hz";
            });
        }

        private void Change_Frequency(object sender, EventArgs e)
        {
            if (!double.TryParse(frequency_textbox.Text, out double Hz))
            {
                MessageBox.Show("Hodnota není platná", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            clock.SetFrequency(Hz);

            Debugger_Robot.Log($"Changed frequency to {clock.Frequency}Hz, interval: {clock.Interval_ms * 2}ms", ref Debugger_window);
        }

        private void Clock_Enable_Changed(object sender, EventArgs e)
        {
            if (Clock_Enable.Checked)
            {
                clock.Start();
                Debugger_Robot.Log("Clock started", ref Debugger_window);
            }
            else
            {
                clock.Stop();
                Debugger_Robot.Log("Clock stopped", ref Debugger_window);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            clock.Dispose();
            controller.Dispose();
        }

    }

    public static class Debugger_Robot
    {
        public static void Log(string message, ref TextBox debugger_window)
        {
            debugger_window.AppendText($">{message}\r\n");
        }

        public static void OverWrite(string message, ref TextBox debugger_window)
        {
            debugger_window.Text = ($">{message}\r\n");
        }
    }

}
