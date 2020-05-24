﻿using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using SDRSharp.Radio;

namespace SDRSharp
{
	public static class Program
	{
		[STAThread]
		static void Main()
		{
			Utils.Log("starting");

			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

			if (Environment.OSVersion.Platform == PlatformID.Win32Windows || Environment.OSVersion.Platform == PlatformID.Win32NT)
			{
				var process = Process.GetCurrentProcess();
				process.PriorityBoostEnabled = true;
				process.PriorityClass = ProcessPriorityClass.RealTime;
				Utils.TimeBeginPeriod(1);
			}

			DSPThreadPool.Initialize();

			Control.CheckForIllegalCrossThreadCalls = false;
			Application.EnableVisualStyles();
			Application.Run(new MainForm());

			if (Environment.OSVersion.Platform == PlatformID.Win32Windows || Environment.OSVersion.Platform == PlatformID.Win32NT)
			{
				Utils.TimeEndPeriod(1);
			}

			DSPThreadPool.Terminate();

			Application.Exit(); // ExtIO may have some thread still running preventing the program from terminating
		}

		static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			var exception = (Exception) e.ExceptionObject;
			File.WriteAllText("crash.txt", exception.Message + "\r\n" + exception.StackTrace);
			Utils.Log($"crash: {exception.Message} {exception.StackTrace}");
		}
	}
}
