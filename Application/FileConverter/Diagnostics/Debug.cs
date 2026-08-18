// <copyright file="Debug.cs" company="AAllard">License: http://www.gnu.org/licenses/gpl.html GPL version 3.</copyright>

namespace FileConverter.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Windows;

    public static class Debug
    {
        private static readonly string diagnosticsFolderPath;
        private static readonly Dictionary<int, DiagnosticsData> diagnosticsDataById = new Dictionary<int, DiagnosticsData>();
        private static int threadCount = 0;
        private static readonly int mainThreadId = 0;

        static Debug()
        {
            Debug.mainThreadId = Thread.CurrentThread.ManagedThreadId;            
            // File diagnostics are intentionally disabled in this build.
            // Keep main-thread console logging available, but do not create
            // Diagnostics-* directories or write diagnostic log files.
        }

        public static int FirstErrorCode
        {
            get;
            private set;
        }

        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

        public static DiagnosticsData[] Data => Debug.diagnosticsDataById.Values.ToArray();

        public static void Log(string message)
        {
            Debug.LogInternal(error: false, message, ConsoleColor.White);
        }

        public static void Assert(bool condition)
        {
            if (!condition)
            {
                LogError("Assertion failed");
            }
        }

        public static void Assert(bool condition, string message)
        {
            if (!condition)
            {
                LogError(message);
            }
        }

        public static void LogError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            Debug.LogInternal(error: true, $"Error: {message}", ConsoleColor.Red);
        }

        public static void LogError(int errorCode, string message)
        {
            if (Debug.FirstErrorCode == 0)
            {
                Debug.FirstErrorCode = errorCode;
            }

            Debug.LogError($"{message} (code 0x{errorCode:X})");
        }

        public static void Release()
        {
            Debug.Log("Diagnostics manager released correctly.");

            foreach (KeyValuePair<int, DiagnosticsData> kvp in Debug.diagnosticsDataById)
            {
                kvp.Value.Release();
            }

            Debug.diagnosticsDataById.Clear();
        }

        private static void LogInternal(bool error, string log, ConsoleColor color)
        {
            DiagnosticsData diagnosticsData;

            Thread currentThread = Thread.CurrentThread;
            int threadId = currentThread.ManagedThreadId;

            // Display main thread logs in standard output.
            if (threadId == Debug.mainThreadId)
            {
                Console.ForegroundColor = color;
                if (error)
                {
                    Console.Error.WriteLine(log);
                }
                else
                {
                    Console.WriteLine(log);
                }

                Console.ResetColor();
            }
            // File-backed diagnostics are disabled. Main-thread messages above
            // still go to the console; background-thread messages are discarded.
        }
    }
}
