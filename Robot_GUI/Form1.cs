using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
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

        CancellationTokenSource Robot_cts;

        private string File_Path;

        public Form1()
        {
            InitializeComponent();

            robot = new Robot("PCIE-1730,BID#0", "PCIE-1730_profile.xml", ref Debugger_window);
            controller = new Controller(ref Debugger_window);
            clock = new Clock(Update_Robot_Inputs);

            Manual_Inputs = new CheckBox[] { Turning_dir_check, Turn_base_check, Engine_main_arm_check, Grabber_check, Arm_grabber_check };

            Load += Form_Load;
            FormClosing += Form1_FormClosing;
        }

        private void Form_Load(object sender, EventArgs e)
        {
            output.Text = $"Aktuální hodnota na vstupu robota: {0}\r\nInterval: {clock.Interval_ms * 2}ms\r\nFrekvence: {clock.Frequency}Hz";
        }



        private void Update_Robot_Inputs()
        {
            // získání stavu vstupů uživatele
            bool[] user_inputs = new bool[5];
            if (Controller_Mode.Checked)
            {
                user_inputs = controller.Return_Button_Data();
            }
            else
                user_inputs = Manual_Inputs.Select(c => c.Checked).ToArray();


            // výpočet hodnoty k zápisu pomocí stavu vstupů uživatele
            byte next_input_value = Robot.Get_Next_Input_Value(user_inputs, clock.ClockSignal);
            next_input_value = (byte)~next_input_value; // negativní logika

            if (robot.Recording_Movement)
                Handle_Movement_Recording_Logic(next_input_value);

            // zápis na výstup IO karty
            //robot.Write_Input(next_input_value);
            //byte output_value = robot.Read_Output();

            // Update UI
            output.Invoke((MethodInvoker)delegate
            {
                output.Text = $"Aktuální hodnota na vstupu robota: {next_input_value}\r\nAktuální hodnota na výstupu robota: {0}\r\nInterval: {clock.Interval_ms * 2}ms\r\nFrekvence: {clock.Frequency}Hz";
            });
        }

        private void Handle_Movement_Recording_Logic(byte next_input_value)
        {
            bool active_motors = robot.Is_Any_Motor_Active(next_input_value, robot.Bits_Motors);

            if (robot.Record_Initial_Position)
            {
                if (active_motors)
                {
                    Debugger_window.Invoke((MethodInvoker)delegate
                    {
                        Debugger_Robot.Log($"Stav: {next_input_value | (1 << (int)Robot.Input_BitPos.Clock)}", ref Debugger_window);
                    });
                    FileManager.AppendLine(File_Path, $"{next_input_value | (1 << (int)Robot.Input_BitPos.Clock)}");

                    robot.Record_Initial_Position = false;
                    if (!clock.ClockSignal)
                        robot.Step_Count++;

                    robot.Previous_Input_Value = next_input_value;
                }

                return;
            }

            // Inkrementace počtu kroků robota pro zápis do souboru
            // Krok nastane právě tehdy, když alespoň 1 motor je aktivní (ze 4 bitů) a zárověň došlo k sestupné hraně hodin.
            if (active_motors)
            {
                if (!clock.ClockSignal)
                    robot.Step_Count++;
            }

            bool motor_state_changed = robot.Input_State_Changed(next_input_value, robot.Previous_Input_Value, robot.Bits_Motors);
            bool turning_direction_changed = robot.Input_State_Changed(next_input_value, robot.Previous_Input_Value, Robot.Input_BitPos.Turning_direction);


            if ((motor_state_changed || turning_direction_changed) && active_motors)
            {
                Debugger_window.Invoke((MethodInvoker)delegate
                {
                    Debugger_Robot.Log($"Kroky: {robot.Step_Count}", ref Debugger_window);
                });
                FileManager.AppendLine(File_Path, $"{robot.Step_Count}");

                Debugger_window.Invoke((MethodInvoker)delegate
                {
                    Debugger_Robot.Log($"Stav: {next_input_value | (1 << (int)Robot.Input_BitPos.Clock)}", ref Debugger_window);
                });
                FileManager.AppendLine(File_Path, $"{next_input_value | (1 << (int)Robot.Input_BitPos.Clock)}");
                robot.Step_Count = 0;

                robot.Previous_Input_Value = next_input_value;
            }
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

        private async void Load_File_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    (byte[] input_states, int[] step_counts) file_data = FileManager.ReadFile(openFileDialog1.FileName);

                    try
                    {
                        Robot_cts = new CancellationTokenSource();

                        await robot.Execute_Learned_Movement(file_data.input_states, file_data.step_counts, clock.Interval_ms, Robot_cts.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        Debugger_Robot.Log("Stopping operation.", ref Debugger_window);
                    }
                    finally
                    {
                        Robot_cts.Dispose();
                        Robot_cts = null;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Data jsou v neplatném formátu\n" + ex.Message, "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Record_Movement_Click(object sender, EventArgs e)
        {
            if (!robot.Recording_Movement)
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    robot.Recording_Movement = true;
                    robot.Record_Initial_Position = true;
                    Reset_btn.Enabled = false;

                    robot.Step_Count = 0;
                    Movement_Record_btn.Text = "Ukončit nahrávání pohybu";
                    File_Path = saveFileDialog1.FileName;
                    FileManager.SaveFile(File_Path, "");
                    Debugger_Robot.Log("Recording started", ref Debugger_window);
                }
            }
            else
            {
                robot.Recording_Movement = false;
                Movement_Record_btn.Text = "Začít nahrávat pohyb";
                Reset_btn.Enabled = true;

                FileManager.AppendLine(File_Path, $"{robot.Step_Count}");

                robot.Step_Count = 0;

                Debugger_Robot.Log("Recording stopped", ref Debugger_window);
            }
        }

        private void Show_Manual(object sender, EventArgs e)
        {
            var imgForm = new Form
            {
                Text = "Controller Layout",
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                Size = new Size(800, 450),
                TopMost = true
            };

            var picture = new PictureBox
            {
                Dock = DockStyle.Fill,
                Image = Image.FromFile("Resources/gembird_robot_manual.jpg"),
                SizeMode = PictureBoxSizeMode.Zoom
            };

            imgForm.Controls.Add(picture);
            imgForm.Show();
        }

        private async void Robot_Reset_Position_btn(object sender, EventArgs e)
        {
            if (robot.Resetting_Position)
            {
                if (Robot_cts != null && !Robot_cts.IsCancellationRequested)
                    Robot_cts.Cancel();

                return;
            }

            robot.Resetting_Position = true;
            Reset_btn.Text = "Zastavit operaci";

            clock.Stop();
            Set_Controls(false);

            Robot_cts = new CancellationTokenSource();

            try
            {
                await robot.Reset_Default_Position(clock.Interval_ms, Robot_cts.Token);
                Debugger_Robot.Log("Robot position reset completed successfully", ref Debugger_window);
            }
            catch (OperationCanceledException)
            {
                Debugger_Robot.Log("Stopping operation.", ref Debugger_window);
            }
            finally
            {
                Robot_cts.Dispose();
                Robot_cts = null;

                robot.Resetting_Position = false;
                Reset_btn.Text = "Reset do základní pozice";

                Set_Controls(true);
            }
        }

        private void Set_Controls(bool state)
        {
            Clock_Enable.Enabled = state;
            Movement_Record_btn.Enabled = state;

            if (!state)
                Clock_Enable.Checked = state;
        }

        private void On_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (Robot_cts != null && !Robot_cts.IsCancellationRequested)
                {
                    Robot_cts.Cancel();
                    Debugger_Robot.Log("Interrupt key pressed.", ref Debugger_window);
                }
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
