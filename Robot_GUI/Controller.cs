using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using HidLibrary;

namespace Robot_GUI
{
    public class Controller
    {
        private const int Gamepad_VendorID = 0x2563; // gembird gamepad
        //private const int Gamepad_VendorID = 0x054C; // ps gamepad
        private HidDevice controller;
        public Timer Reconnect_Timer;

        private bool[] leftCross = new bool[4];
        private bool[] rightButtons = new bool[4];

        public Controller()
        {
            Reconnect_Timer = new Timer();
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
            rightButtons = new[]
            {
            report.Data[11] == 0xff,
            report.Data[12] == 0xff,
            report.Data[13] == 0xff,
            report.Data[14] == 0xff
            };

            leftCross = new[]
            {
            report.Data[9] == 0xff,
            report.Data[7] == 0xff,
            report.Data[10] == 0xff,
            report.Data[8] == 0xff
            };
        }

        public bool[] Return_Button_Data()
        {
            // maps Turning_dir=btn_down, Turn_base=btn_left, Engine_main_arm=cross_down, Grabber=cross_left, Arm_grabber=cross_right
            return new[]
            {
            rightButtons[2],
            rightButtons[3],
            leftCross[2],
            leftCross[3],
            leftCross[1],
            };
        }

        public void Dispose()
        {
            Reconnect_Timer.Dispose();
            Reconnect_Timer = null;
        }
    }
}
