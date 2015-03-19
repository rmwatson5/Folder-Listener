namespace File_Listener
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Security.Permissions;
	using System.Management.Instrumentation;
	using System.Management;

	public class Program
	{
		public const string Path = @"C:\Music";
		public static void Main(string[] args)
		{
			Run();
			Console.ReadLine();
		}

		[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
		public static void Run()
		{
			// Create a new FileSystemWatcher and set its properties.
			var watcher = new FileSystemWatcher
				                            {
												Path = Path,
					                            NotifyFilter =
						                            NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName
						                            | NotifyFilters.DirectoryName,
					                            Filter = "*.*"
				                            };
			/* Watch for changes in LastAccess and LastWrite times, and
			   the renaming of files or directories. */
			// Only watch text files.

			// Add event handlers.
			watcher.Changed += OnChanged;
			watcher.Created += OnChanged;
			watcher.Deleted += OnChanged;
			watcher.Renamed += OnRenamed;

			// Begin watching.
			watcher.EnableRaisingEvents = true;

			// Wait for the user to quit the program.
			Console.WriteLine("Press \'q\' to quit the sample.");
			while (Console.Read() != 'q');
		}

		// Define the event handlers. 
		private static void OnChanged(object source, FileSystemEventArgs e)
		{
			// Specify what is done when a file is changed, created, or deleted.
			Console.WriteLine("File: " + e.FullPath + " " + e.ChangeType);
			var dir = new DirectoryInfo(Path);
			var lastModified = dir.GetAccessControl();
			var modifiedBy = lastModified
				.GetOwner(typeof(System.Security.Principal.NTAccount));
			Console.WriteLine("{0} was modified by {1}", Path, modifiedBy);
		}

		private static void OnRenamed(object source, RenamedEventArgs e)
		{
			// Specify what is done when a file is renamed.
			Console.WriteLine("File: {0} renamed to {1}", e.OldFullPath, e.FullPath);
		}

		private static void WriteUser()
		{
			ManagementScope mgtScope = new ManagementScope("\\\\ComputerName\\root\\cimv2");
			// you could also replace the username in the select with * to query all objects
			ObjectQuery objQuery = new ObjectQuery("SELECT username FROM Win32_ComputerSystem");

			ManagementObjectSearcher srcSearcher = new ManagementObjectSearcher(mgtScope, objQuery);

			ManagementObjectCollection colCollection = srcSearcher.Get();

			foreach (ManagementObject curObjCurObject in colCollection)
			{

				Console.WriteLine(curObjCurObject["username"].ToString());
			}

			//if you want ot get the name of the machine that changed it once it gets into that  Event change the query to look like this. I just tested this locally and it does work 

			ManagementObjectSearcher mosQuery = new ManagementObjectSearcher("SELECT * FROM Win32_Process WHERE ProcessId = " + Process.GetCurrentProcess().Id.ToString());
			ManagementObjectCollection queryCollection1 = mosQuery.Get();
			foreach (ManagementObject manObject in queryCollection1)
			{
				Console.WriteLine("Name : " + manObject["name"].ToString());
				Console.WriteLine("Version : " + manObject["version"].ToString());
				Console.WriteLine("Manufacturer : " + manObject["Manufacturer"].ToString());
				Console.WriteLine("Computer Name : " + manObject["csname"].ToString());
				Console.WriteLine("Windows Directory : " + manObject["WindowsDirectory"].ToString());
			}  
		}
	}
}
