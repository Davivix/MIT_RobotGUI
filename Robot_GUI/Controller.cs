using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using HidLibrary;
using System.Linq.Expressions;

namespace Robot_GUI
{
    public class Controller
    {
        //private const int Gamepad_VendorID = 0x2563; // gembird gamepad
        private const int Gamepad_VendorID = 0x054C; // ps dualshock gamepad
        private HidDevice controller;
        public System.Timers.Timer Reconnect_Timer;

        private TextBox debugger_window;

        public bool[] LeftCross { get; private set; } = new bool[4];
        public bool[] RightButtons { get; private set; } = new bool[4];

        public Controller(ref TextBox debugger_window)
        {
            this.debugger_window = debugger_window;

            Reconnect_Timer = new System.Timers.Timer();
            Reconnect_Timer.Elapsed += Try_Connect_Controller;
            Reconnect_Timer.Interval = 500;
            Reconnect_Timer.Start();
        }

        private void Try_Connect_Controller(object sender, ElapsedEventArgs e)
        {
            if (controller != null)
                return;

            controller = HidDevices.Enumerate(Gamepad_VendorID).FirstOrDefault();
            if (controller != null)
            {
                controller.OpenDevice();
                controller.Removed += Controller_Disconnect;
                controller.ReadReport(Controller_Report);
            }
        }

        private void Controller_Disconnect()
        {
            controller.CloseDevice();
            controller = null;
        }

        private void Controller_Report(HidReport report)
        {
            Update_Controller_State(report);

            if (controller != null)
                controller.ReadReport(Controller_Report);
        }

        private void Update_Controller_State(HidReport report)
        {
            // in order - TOP - RIGHT - BOTTOM - LEFT

            //RightButtons = new[]
            //{
            //report.Data[11] == 0xff,
            //report.Data[12] == 0xff,
            //report.Data[13] == 0xff,
            //report.Data[14] == 0xff
            //};

            //LeftCross = new[]
            //{
            //report.Data[9] == 0xff,
            //report.Data[7] == 0xff,
            //report.Data[10] == 0xff,
            //report.Data[8] == 0xff
            //};
            //byte[] d = report.Data;

            //debugger_window.Invoke((MethodInvoker)delegate
            //{
            //    Debugger_Robot.OverWrite($"Report length: {d.Length}  First 16 bytes: {BitConverter.ToString(d.Take(Math.Min(16, d.Length)).ToArray())}", ref debugger_window);
            //});


            byte b = report.Data[4]; // You confirmed all buttons live here

            // --- Right buttons (face) ---
            // bits 7-4 = Triangle, Circle, Cross, Square
            RightButtons = new[]
            {
                 (b & 0b10000000) != 0, // Triangle (top)
                 (b & 0b01000000) != 0, // Circle   (right)
                 (b & 0b00100000) != 0, // Cross    (bottom)
                 (b & 0b00010000) != 0  // Square   (left)
            };

            // --- D-Pad (LeftCross) ---
            // lower nibble (bits 0–3): 0=Up,2=Right,4=Down,6=Left,8=Neutral
            int dpad = b & 0x0F;
            LeftCross = new[]
            {
                dpad == 0, // Up
                dpad == 2, // Right
                dpad == 4, // Down
                dpad == 6  // Left
            };
        }
        public bool[] Return_Button_Data()
        {
            bool[] inputs = new bool[5]; // index 0-4 -> Turning_dir, Engine_turn_base, Engine_main_arm, Grabber, Arm_grabber
            bool clockwise = false;
            bool counter_clockwise = true;

            // button mapping
            if (!(LeftCross[3] && LeftCross[1])) // skip if both left and right buttons are pressed
            {
                if (LeftCross[3]) // if left button is pressed
                {
                    inputs[1] = LeftCross[3];
                    inputs[0] = clockwise;
                }
                else if (LeftCross[1]) // if right button is pressed
                {
                    inputs[1] = LeftCross[1];
                    inputs[0] = counter_clockwise;
                }
            }

            if (!(LeftCross[2] && LeftCross[0]))
            {
                if (LeftCross[2]) // if bottom button is pressed
                {
                    inputs[2] = LeftCross[2];
                    inputs[0] = clockwise;
                }
                else if (LeftCross[0]) // if top button is pressed
                {
                    inputs[2] = LeftCross[0];
                    inputs[0] = counter_clockwise;
                }
            }

            if (!(RightButtons[3] && RightButtons[1]))
            {
                if (RightButtons[3]) // if left button is pressed
                {
                    inputs[3] = RightButtons[3];
                    inputs[0] = clockwise;
                }
                else if (RightButtons[1]) // if right button is pressed
                {
                    inputs[3] = RightButtons[1];
                    inputs[0] = counter_clockwise;
                }
            }

            if (!(RightButtons[2] && RightButtons[0]))
            {
                if (RightButtons[2]) // if bottom button is pressed
                {
                    inputs[4] = RightButtons[2];
                    inputs[0] = counter_clockwise;
                }
                else if (RightButtons[0]) // if top button is pressed
                {
                    inputs[4] = RightButtons[0];
                    inputs[0] = clockwise;
                }
            }

            return inputs;
        }

        public void Dispose()
        {
            Reconnect_Timer.Dispose();
            Reconnect_Timer = null;
        }
    }
}
