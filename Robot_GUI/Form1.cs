using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Robot_GUI
{
    public partial class Form1 : Form
    {

        private Robot robot;
        private Controller controller;
        private Clock clock;
        private CancellationTokenSource Robot_cts;
        private string File_Path;

        public Form1()
        {
            InitializeComponent();

            robot = new Robot("PCIE-1730,BID#0", "PCIE-1730_profile.xml");
            controller = new Controller();
            clock = new Clock(Update_Robot_Inputs);


            FormClosing += Form1_FormClosing;
        }

        private void Update_Robot_Inputs()
        {
            // získání stavu vstupů uživatele
            bool[] user_inputs = new bool[5];
            user_inputs = controller.Return_Button_Data();

            // výpočet hodnoty k zápisu pomocí stavu vstupů uživatele
            byte next_input_value = Robot.Get_Next_Input_Value(user_inputs, clock.ClockSignal);
            next_input_value = (byte)~next_input_value; // negativní logika

            if (robot.Recording_Movement)
                Handle_Movement_Recording_Logic(next_input_value);

            // zápis na výstup IO karty
            robot.Write_Input(next_input_value);
            byte output_value = robot.Read_Output();

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
                FileManager.AppendLine(File_Path, $"{robot.Step_Count}");
                FileManager.AppendLine(File_Path, $"{next_input_value | (1 << (int)Robot.Input_BitPos.Clock)}");

                robot.Step_Count = 0;
                robot.Previous_Input_Value = next_input_value;
            }
        }

        private void Change_Frequency_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(frequency_textbox.Text, out double Hz))
            {
                MessageBox.Show("Hodnota není platná", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            clock.SetFrequency(Hz);

            Debugger_Robot.Log($"Frekvence byla změněna na {clock.Frequency}Hz, interval jednoho kroku: {clock.Interval_ms * 2}ms", ref Debugger_window, Color.White);
        }

        private void Clock_Enable_Changed(object sender, EventArgs e)
        {
            if (Clock_Enable.Checked)
            {
                clock.Start();
                Debugger_Robot.Log("Clock started", ref Debugger_window, Color.White);
            }
            else
            {
                clock.Stop();
                Debugger_Robot.Log("Clock stopped", ref Debugger_window, Color.White);
            }
        }

        private async void Load_File_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                (byte[] input_states, int[] step_counts) file_data = (null, null);
                try
                {
                    file_data = FileManager.ReadFile(openFileDialog1.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Data jsou v neplatném formátu\n" + ex.Message, "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                try
                {
                    Robot_cts = new CancellationTokenSource();
                    Debugger_Robot.Log("Data úspěšně načtena, provádím načtený pohyb", ref Debugger_window, Color.Lime);

                    await robot.Execute_Learned_Movement(file_data.input_states, file_data.step_counts, clock.Interval_ms, Robot_cts.Token);
                    Debugger_Robot.Log("Načtený pohyb úspěšně proveden", ref Debugger_window, Color.Lime);
                }
                catch (OperationCanceledException)
                {
                    Debugger_Robot.Log("Opakování načteného pohybu zrušeno", ref Debugger_window, Color.Red);
                }
                finally
                {
                    Robot_cts.Dispose();
                    Robot_cts = null;
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
                    Debugger_Robot.Log("Nahrávání pohybu spuštěno", ref Debugger_window, Color.Lime);
                }
            }
            else
            {
                robot.Recording_Movement = false;
                Movement_Record_btn.Text = "Začít nahrávat pohyb";
                Reset_btn.Enabled = true;

                FileManager.AppendLine(File_Path, $"{robot.Step_Count}");

                robot.Step_Count = 0;

                Debugger_Robot.Log("Nahrávání pohybu ukončeno", ref Debugger_window, Color.Red);
            }
        }

        private void Show_Manual_Click(object sender, EventArgs e)
        {
            Form manual_window = new Form
            {
                Text = "Ovládání robota",
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                Size = new Size(800, 450),
                TopMost = true
            };

            PictureBox manual_picture = new PictureBox
            {
                Dock = DockStyle.Fill,
                Image = Image.FromFile("Resources/gembird_robot_manual.jpg"),
                SizeMode = PictureBoxSizeMode.Zoom
            };

            manual_window.Controls.Add(manual_picture);
            manual_window.Show();
        }

        private async void Robot_Reset_Position_Click(object sender, EventArgs e)
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
                Debugger_Robot.Log("Robot se nastavuje do původní polohy", ref Debugger_window, Color.Lime);
                await robot.Reset_Default_Position(clock.Interval_ms, Robot_cts.Token);
                Debugger_Robot.Log("Robot byl úspěšně nastaven do původní polohy", ref Debugger_window, Color.Lime);
            }
            catch (OperationCanceledException)
            {
                Debugger_Robot.Log("Nastavování robota do původní polohy zrušeno", ref Debugger_window, Color.Red);
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
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Escape)
            {
                if (Robot_cts != null && !Robot_cts.IsCancellationRequested)
                {
                    Robot_cts.Cancel();
                    Debugger_Robot.Log("Interrupt key pressed.", ref Debugger_window, Color.Red);
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            clock.Dispose();
            controller.Dispose();
        }

        private void Stop_Click(object sender, EventArgs e)
        {
            if (Robot_cts != null && !Robot_cts.IsCancellationRequested)
            {
                Robot_cts.Cancel();
                Debugger_Robot.Log("Interrupt key pressed.", ref Debugger_window, Color.Red);
            }

            clock.Stop();
        }
    }

    public static class Debugger_Robot
    {
        public static void Log(string message, ref RichTextBox debugger_window, Color text_color)
        {
            debugger_window.SelectionColor = text_color;
            debugger_window.AppendText($">{message}\r\n");
        }

        public static void OverWrite(string message, ref RichTextBox debugger_window, Color text_color)
        {
            debugger_window.SelectionColor = text_color;
            debugger_window.Text = ($">{message}\r\n");
        }
    }

}
