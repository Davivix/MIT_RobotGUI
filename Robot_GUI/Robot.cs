using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Automation.BDaq;

namespace Robot_GUI
{
    internal class Robot
    {
        public InstantDoCtrl IO_Output;
        public InstantDiCtrl IO_Input;

        public const int MaxFrequency = 450;

        public Robot(string device_description, string profile)
        {
            DeviceInformation ioDevice = new DeviceInformation();
            ioDevice.Description = device_description;
            ioDevice.DeviceMode = AccessMode.ModeWrite;

            IO_Output = new InstantDoCtrl();
            IO_Input = new InstantDiCtrl();

            IO_Output.SelectedDevice = ioDevice;
            IO_Input.SelectedDevice = ioDevice;

            IO_Output.LoadProfile(profile);
            IO_Input.LoadProfile(profile);
        }

        public static byte Get_Next_Input_Value(bool[] inputs, bool clock_signal)
        {
            byte value = 0;
            for (int i = 0; i < inputs.Length; i++)
                value |= (byte)(Convert.ToByte(inputs[i]) << i);

            if (clock_signal)
                value ^= (1 << (byte)Input_BitPos.Clock);

            return value;
        }

        public void Write_Input(byte value)
        {
            IO_Output.Write(0, value);
        }

        public byte Read_Output()
        {
            IO_Input.Read(0, out byte value);
            return value;
        }

        public string Reset_Default_Position()
        {
            string n = string.Empty;
            foreach (Output_BitPost bitpos in Enum.GetValues(typeof(Output_BitPost)))
            {
                int max_steps = 500; 

                while ((Read_Output() & (1 << (int)bitpos)) != 0)
                { 
                

                }
            }
            return n;
        }


        public enum Input_BitPos
        {
            Turning_direction = 0,
            Turn_base = 1,
            Engine_main_arm = 2,
            Engine_grabber = 3,
            Engine_grab_arm = 4,
            Clock = 5,
        }

        public enum Output_BitPost
        {
            Turn_base = 0,
            Engine_main_arm = 1,
            Engine_grabber = 2,
            Engine_grab_arm = 3,
        }
    }
}
