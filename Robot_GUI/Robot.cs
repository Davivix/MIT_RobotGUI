using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Automation.BDaq;

namespace Robot_GUI
{
    internal class Robot
    {
        public InstantDoCtrl IO_Output;
        public InstantDiCtrl IO_Input;

        public const int MaxFrequency = 450;

        public byte Input_value = 0;
        private TextBox debugger_window;

        public Robot(string device_description, string profile, ref TextBox debugger_window)
        {
            this.debugger_window = debugger_window;
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
            return value
        }

        public async Task Reset_Default_Position(int clock_interval)
        {
            foreach (Output_BitPost bitpos in Enum.GetValues(typeof(Output_BitPost)))
            {
                int max_steps = 300;
                int steps = 0;

                string name = Enum.GetName(typeof(Output_BitPost), bitpos);
                int input_value = (int)Enum.Parse(typeof(Input_BitPos), name);

                byte motor_bit = (byte)~(1 << input_value);

                while ((Read_Output() & (1 << (int)bitpos)) != 0)
                {
                    await Step(clock_interval, motor_bit);

                    steps++;

                    Debugger_Robot.Log($"step: {steps}, max: {max_steps}", ref debugger_window);

                    if (steps > max_steps)
                    {
                        Debugger_Robot.Log($"switching direction", ref debugger_window);
                        // switch direction
                        motor_bit ^= (1 << (int)Input_BitPos.Turning_direction);

                        steps = 0;
                        max_steps *= 2;

                        if (max_steps > 600)
                        {
                            Debugger_Robot.Log($"max steps reached, breaking, next motor", ref debugger_window);
                            break;

                        }
                    }

                }
                Debugger_Robot.Log($"reset {name} to default position", ref debugger_window);
            }
        }

        private async Task Step(int interval, byte write_value)
        {
            Write_Input(write_value);
            Debugger_Robot.Log($"wrote: {write_value}, waiting {interval}", ref debugger_window);
            await Task.Delay(interval);


            write_value ^= 1 << (byte)Input_BitPos.Clock;
            Write_Input(write_value);
            Debugger_Robot.Log($"wrote: {write_value}, waiting {interval}", ref debugger_window);
            await Task.Delay(interval);
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
