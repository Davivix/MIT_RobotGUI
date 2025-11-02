using System;
using System.IO;

namespace Robot_GUI
{
    internal static class FileManager
    {
        private static readonly object fileLock = new object();

        public static void SaveFile(string path, string content)
        {
            lock (fileLock)
            {
                File.WriteAllText(path, content);
            }
        }

        public static void AppendLine(string path, string line)
        {
            lock (fileLock)
            {
                File.AppendAllText(path, line + Environment.NewLine);
            }
        }

        public static (byte[] input_states, int[] step_counts) ReadFile(string path)
        {
            string[] lines = File.ReadAllLines(path);

            byte[] input_states = new byte[lines.Length/2];
            int[] step_counts = new int[lines.Length/2];

            for (int i = 0; i < lines.Length; i++)
            {
                if (i % 2 ==0)
                    input_states[i/2] = byte.Parse(lines[i]);
                else
                    step_counts[i/2] = int.Parse(lines[i]);
            }

            return (input_states, step_counts);
        }
    }
}
