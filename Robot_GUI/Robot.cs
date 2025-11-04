using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Automation.BDaq;

namespace Robot_GUI
{
    internal class Robot
    {

        // private
        private InstantDoCtrl IO_Output;
        private InstantDiCtrl IO_Input;

        // public
        public const int MaxFrequency = 126;
        public int Step_Count = 0;

        // flagy
        public bool Recording_Movement = false;
        public bool Record_Initial_Position = false;
        public bool Resetting_Position = false;

        // Definuje fyzické propojení výstupu IO-karty ke vstupu robota, bitové pořadí jednotlivých signálů.
        public enum Input_BitPos
        {
            Turning_direction = 0,
            Engine_turn_base = 1,
            Engine_main_arm = 2,
            Engine_grabber = 3,
            Engine_grab_arm = 4,
            Clock = 5,
        }

        // Definuje fyzické propojení výstupu robota ke vstupu IO-karty, bitové pořadí jednotlivých signálů.
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

        public readonly Input_BitPos[] Bits_Excluding_Clock =
        {
            Input_BitPos.Turning_direction,
            Input_BitPos.Engine_turn_base,
            Input_BitPos.Engine_main_arm,
            Input_BitPos.Engine_grabber,
            Input_BitPos.Engine_grab_arm
        };

        private RichTextBox Debugger_Window;
        public byte Previous_Input_Value = 0xFF;

        public Robot(string device_description, string profile, ref RichTextBox debugger_window)
        {
            Debugger_Window = debugger_window;
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

        public static byte Get_Next_Input_Value(bool[] inputs, bool clock_signal, Input_BitPos[] bits_to_mask)
        {
            byte value = 0;
            for (int i = 0; i < inputs.Length; i++)
                value |= (byte)(Convert.ToByte(inputs[i]) << (byte)bits_to_mask[i]);

            if (clock_signal)
                value ^= 1 << (byte)Input_BitPos.Clock;

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

        // Vratí robota do původní pozice
        // Motor je v původní pozici, jestli se na vstupu IOkarty nachází log. 0
        // Postupně pro každý motor provede 500 kroků do jednoho směru, pokud se nedostane do původní pozice, provede 1000 kroků v opačném směru
        public async Task Reset_Default_Position(int clock_interval, CancellationToken token)
        {
            foreach (Output_BitPost bitpos in Enum.GetValues(typeof(Output_BitPost)))
            {
                int max_steps = 20;
                int steps = 0;

                string name = Enum.GetName(typeof(Output_BitPost), bitpos);
                int input_value = (int)Enum.Parse(typeof(Input_BitPos), name);

                bool turning_base = false;
                if (name == "Engine_turn_base")
                    turning_base = true;

                byte motor_bit = Get_Reset_Write_Value(name, input_value);
                Debugger_Robot.Log($"Resetting: {name}", ref Debugger_Window, Color.Green);

                while ((Read_Output() & (1 << (int)bitpos)) != 0)
                {
                    token.ThrowIfCancellationRequested();

                    await Step(clock_interval, motor_bit, token);

                    steps++;

                    if (steps > max_steps)
                    {
                        if (turning_base)
                        {
                            // Změna směru
                            motor_bit ^= (1 << (int)Input_BitPos.Turning_direction);

                            steps = 0;
                            max_steps *= 2;

                            if (max_steps > 40)
                                break;
                        }
                        else
                            break;
                    }

                }
            }
        }

        private byte Get_Reset_Write_Value(string name, int input_value)
        {
            byte motor_bit = (byte)~(1 << input_value);

            switch (name)
            {
                case "Engine_main_arm":
                    motor_bit ^= 1 << (int)Input_BitPos.Turning_direction; // nastavíme do nuly, směr nahoru
                    break;
                case "Engine_grabber":
                    motor_bit ^= 1 << (int)Input_BitPos.Turning_direction; // nastavíme do nuly, otevřít chapadlo
                    break;
            }
            // Pro rameno s chapadlem je potřebná hodnota směru otáčení log. 1, tedy směr nahoru, jelikož je log. 1 nastavená defaultně, nepotřebujeme podmínku

            return motor_bit;
        }

        // Provede načtený pohyb ze souboru, kde pro každý stav 'input_states[n]' vykoná 'step_counts[n]' kroků
        public async Task Execute_Learned_Movement(byte[] input_states, int[] step_counts, int step_interval, CancellationToken token)
        {
            for (int i = 0; i < input_states.Length; i++)
            {
                byte state = input_states[i];
                int step_count = step_counts[i];

                for (int step = 0; step < step_count; step++)
                {
                    token.ThrowIfCancellationRequested();

                    await Step(step_interval, state,token);
                }
            }
        }

        // Provede jeden krok robota
        private async Task Step(int interval, byte write_value, CancellationToken token)
        {
            Write_Input(write_value);
            await Precision.DelayAsync(interval, token);

            write_value ^= 1 << (byte)Input_BitPos.Clock;
            Write_Input(write_value);
            await Precision.DelayAsync(interval, token);
        }

        // Kontroluje, jestli je alespoň jeden motor aktivní. Pokud ano, vrací 'true'
        public static bool Is_Any_Motor_Active(byte input_value, Input_BitPos[] bits_to_check)
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

        // Porovnává skupinu bitů mezi dvěma hodnotami
        public static bool Input_State_Changed(byte current_value, byte previous_value, Input_BitPos[] bits_to_check)
        {
            byte mask = 0;
            foreach (Input_BitPos bitpos in bits_to_check)
                mask |= (byte)(1 << (byte)bitpos);                     // sestavení masky pro kontrolu požadovaných bitů

            return (current_value & mask) != (previous_value & mask);  // vrací 'true', pokud se skupina bitů v aktuální a předchozí hodnotě neshodují
        }

        // Porovnává jeden bit mezi dvěma hodnotami
        public static bool Input_State_Changed(byte current_value, byte previous_value, Input_BitPos bit_to_check)
        {
            byte mask = (byte)(1 << (byte)bit_to_check);               // sestavení masky pro kontrolu požadovaného bitu

            return (current_value & mask) != (previous_value & mask);  // vrací 'true', pokud se bit v aktuální a předchozí hodnotě neshoduje
        }
    }
}
