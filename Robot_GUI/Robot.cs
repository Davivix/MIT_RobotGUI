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

        public const int MaxFrequency = 60;

        public byte Input_value = 0;
        private TextBox debugger_window;

        public bool Recording_Movement = false;
        public bool Record_Initial_Position = false;

        public bool Resetting_Position = false;

        public enum Input_BitPos
        {
            Turning_direction = 0,
            Engine_turn_base = 1,
            Engine_main_arm = 2,
            Engine_grabber = 3,
            Engine_grab_arm = 4,
            Clock = 5,
        }

        public enum Output_BitPost
        {
            Engine_turn_base = 0,
            Engine_main_arm = 1,
            Engine_grabber = 2,
            Engine_grab_arm = 3,
        }

        public readonly Input_BitPos[] Bits_Motors =
        {
            Input_BitPos.Engine_turn_base,
            Input_BitPos.Engine_main_arm,
            Input_BitPos.Engine_grabber,
            Input_BitPos.Engine_grab_arm
        };

        public byte Previous_Input_Value = 0xFF;

        public int Step_Count = 0;

        public Robot(string device_description, string profile, ref TextBox debugger_window)
        {
            this.debugger_window = debugger_window;
            //DeviceInformation ioDevice = new DeviceInformation();
            //ioDevice.Description = device_description;
            //ioDevice.DeviceMode = AccessMode.ModeWrite;

            //IO_Output = new InstantDoCtrl();
            //IO_Input = new InstantDiCtrl();

            //IO_Output.SelectedDevice = ioDevice;
            //IO_Input.SelectedDevice = ioDevice;

            //IO_Output.LoadProfile(profile);
            //IO_Input.LoadProfile(profile);
        }

        public static byte Get_Next_Input_Value(bool[] inputs, bool clock_signal)
        {
            byte value = 0;
            for (int i = 0; i < inputs.Length; i++)
                value |= (byte)(Convert.ToByte(inputs[i]) << i);

            if (clock_signal)
                value ^= 1 << (byte)Input_BitPos.Clock;

            return value;
        }

        public void Write_Input(byte value)
        {
            //IO_Output.Write(0, value);
        }

        public byte Read_Output()
        {
            //IO_Input.Read(0, out byte value);
            return 0xFF;
        }

        public async Task Reset_Default_Position(int clock_interval, CancellationToken token)
        {
            foreach (Output_BitPost bitpos in Enum.GetValues(typeof(Output_BitPost)))
            {
                int max_steps = 500;
                int steps = 0;

                string name = Enum.GetName(typeof(Output_BitPost), bitpos);
                int input_value = (int)Enum.Parse(typeof(Input_BitPos), name);

                byte motor_bit = (byte)~(1 << input_value);

                while ((Read_Output() & (1 << (int)bitpos)) != 0)
                {
                    token.ThrowIfCancellationRequested();

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

                        if (max_steps > 1000)
                        {
                            Debugger_Robot.Log($"max steps reached, breaking, next motor", ref debugger_window);
                            break;

                        }
                    }

                }
                Debugger_Robot.Log($"reset {name} to default position", ref debugger_window);
            }
        }

        public async Task Execute_Learned_Movement(byte[] input_states, int[] step_counts, int interval, CancellationToken token)
        {
            for (int i = 0; i < input_states.Length; i++)
            {
                byte state = input_states[i];
                int step_count = step_counts[i];

                for (int step = 0; step < step_count; step++)
                {
                    token.ThrowIfCancellationRequested();

                    await Step(interval, state);
                }
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

        public bool Is_Any_Motor_Active(byte input_value, Input_BitPos[] bits_to_check)
        {
            byte mask = 0;
            foreach (Input_BitPos bitpos in bits_to_check)
                mask |= (byte)(1 << (byte)bitpos);                     // sestavení masky pro kontrolu požadovaných bitů

            // protože 'input_value' už je v negativní logice, znamenalo by to, že pokud jsou všechny motory neaktivní, tak po aplikování masky by ve výsledné hodnotě byla nějaká kombinace jedniček
            // pro zjednodušení to tedy bitově znegujeme, aby vycházela čistá nula, když jsou všechny motory neaktivní
            input_value = (byte)~input_value;
            if ((input_value & mask) == 0)
                return false;

            return true;
        }

        public bool Input_State_Changed(byte current_value, byte previous_value, Input_BitPos[] bits_to_check)
        {
            byte mask = 0;
            foreach (Input_BitPos bitpos in bits_to_check)
                mask |= (byte)(1 << (byte)bitpos);                     // sestavení masky pro kontrolu požadovaných bitů

            return (current_value & mask) != (previous_value & mask);  // vrací 'true', pokud se skupina bitů v aktuální a předchozí hodnotě neshodují
        }

        public bool Input_State_Changed(byte current_value, byte previous_value, Input_BitPos bit_to_check)
        {
            byte mask = (byte)(1 << (byte)bit_to_check);               // sestavení masky pro kontrolu požadovaného bitu

            return (current_value & mask) != (previous_value & mask);  // vrací 'true', pokud se bit v aktuální a předchozí hodnotě neshoduje
        }
    }
}
