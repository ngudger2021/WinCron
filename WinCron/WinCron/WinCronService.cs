using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WinCron
{
    public partial class WinCronService : ServiceBase
    {
        System.Timers.Timer tmr = new System.Timers.Timer();
        System.Timers.Timer cnter = new System.Timers.Timer();
        public WinCronService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            RunCron();
        }

        protected override void OnStop()
        {
        }

		private void OnTimedEvent1(Object source, System.Timers.ElapsedEventArgs e)
        {
            if (DateTime.Now.Second == 0)
            {
                cnter.Stop();
                cnter.Enabled = false;
                tmr.Elapsed += OnTimedEvent;
                tmr.Interval = 60000;
                tmr.Enabled = true;
            }
        }

		private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            RunCron();
        }

		public void SystemCheck()
		{
			string CronPath = System.Environment.GetEnvironmentVariable("USERPROFILE") + "\\.crontab";
			string Crontab = System.Environment.GetEnvironmentVariable("USERPROFILE") + "\\.crontab\\crontab";
			if (!Directory.Exists(CronPath))
			{
				Directory.CreateDirectory(CronPath);
				DirectoryInfo di = new DirectoryInfo(CronPath);

				//See if directory has hidden flag, if not, make hidden
				if ((di.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
				{
					//Add Hidden flag    
					di.Attributes |= FileAttributes.Hidden;
				}
			}
			if (!File.Exists(Crontab))
			{
				using (StreamWriter sw = new StreamWriter(Crontab))
				{
					sw.WriteLine("# Edit this file to introduce tasks to be run by cron.");
					sw.WriteLine("#");
					sw.WriteLine("# Each task to run has to be defined through a single line");
					sw.WriteLine("# indicating with different fields when the task will be run");
					sw.WriteLine("# and what command to run for the task");
					sw.WriteLine("#");
					sw.WriteLine("# To define the time you can provide concrete values for");
					sw.WriteLine("# minute (m), hour (h), day of month (dom), month (mon),");
					sw.WriteLine("# and day of week (dow) or use '*' in these fields (for 'any').");
					sw.WriteLine("#");
					sw.WriteLine("# Notice that tasks will be started based on the cron's system");
					sw.WriteLine("# daemon's notion of time and timezones.");
					sw.WriteLine("#");
					sw.WriteLine("# Output of the crontab jobs (including errors) is sent through");
					sw.WriteLine("# email to the user the crontab file belongs to (unless redirected).");
					sw.WriteLine("#");
					sw.WriteLine("# For example, you can run a backup of all your user accounts");
					sw.WriteLine("# at 5 a.m every week with:");
					sw.WriteLine("# 0 5 * * 1 powershell -command \"Compress-Archive $env:USERPROFILE\\Desktop c:\\$(get-date -f dd-MM-yyyy_HH_mm_ss)_Backups.zip\"");
					sw.WriteLine("#");
					sw.WriteLine("# For more information see the manual pages of crontab(5) and cron(8)");
					sw.WriteLine("#");
					sw.WriteLine("# m h  dom mon dow   command");
				}
			}
		}

		private void RunCron()
		{
			//DateTime.Now.Dump();
			try
			{
				using (StreamReader sr = new StreamReader(@"d:\crontab"))
				{
					while (!sr.EndOfStream)
					{
						string line = sr.ReadLine();
						if (line.Substring(0, 1) != "#")
						{
							int minute = 0;
							int hour = 0;
							string dayOfMonth = "";
							int month = 0;
							string dayOfWeek = "";
							string[] Job = line.Split(' ');

							// MINUTES
							Match MatchMinute = Regex.Match(Job[0], @"^\*\/(\d|\d{2})$");
							if (Job[0] != "*" && !Job[0].Contains("/"))
							{
								minute = int.Parse(Job[0]);
							}
							else if (MatchMinute.Success)
							{
								int value = int.Parse(Job[0].Substring(Job[0].IndexOf('/') + 1));
								if (DateTime.Now.Minute % value == 0)
								{
									minute = DateTime.Now.Minute;
								}
							}
							else
							{
								minute = DateTime.Now.Minute;
							}

							// HOURS
							Match MatchHour = Regex.Match(Job[1], @"^\*\/(\d|\d{2})$");
							//if (Job[1].ToString().ContainsAny(""))
							//{

							//}
							if (Job[1] != "*" && !Job[1].Contains("/"))
							{
								hour = int.Parse(Job[1]);
							}
							else if (MatchHour.Success)
							{
								int value = int.Parse(Job[1].Substring(Job[1].IndexOf('/') + 1));
								if (DateTime.Now.Hour % value == 0)
								{
									hour = DateTime.Now.Hour;
								}
							}
							else
							{
								hour = DateTime.Now.Hour;
							}

							// DAY OF THE MONTH
							if (Job[2] != "*")
							{
								dayOfMonth = Job[2];
							}
							else
							{
								dayOfMonth = DateTime.Now.Day.ToString("ddd");
							}
							if (Job[3] != "*")
							{
								month = int.Parse(Job[3]);
							}
							else
							{
								month = DateTime.Now.Month;
							}
							if (Job[4] != "*")
							{
								dayOfWeek = Job[4];
							}
							else
							{
								dayOfWeek = dayOfMonth = DateTime.Now.Day.ToString("ddd");
							}
							if (DateTime.Now.Minute == minute && DateTime.Now.Hour == hour && DateTime.Now.Day.ToString("ddd") == dayOfMonth && DateTime.Now.Month == month && DateTime.Now.Day.ToString("ddd") == dayOfWeek)
							{
								var result = line.Split('"').Select((element, index) => index % 2 == 0  // If even index
																		   ? element.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)  // Split the item
																		: new string[] { element })  // Keep the entire item
													.SelectMany(element => element).ToList();

								ProcessStartInfo startInfo = new ProcessStartInfo();
								startInfo.FileName = @"CMD.EXE ";
								startInfo.WindowStyle = ProcessWindowStyle.Normal;
								startInfo.Arguments = @"/K " + result[5].ToString();
								//startInfo.WorkingDirectory = @"C:\Windows\System32\";
								Process.Start(startInfo);
							}
						}
					}
				}
				//DateTime.Now.Dump();
			}
			catch (Exception ex)
			{
				//ex.Message.Dump();
			}
		}

		public bool ContainsAny(string haystack, params string[] needles)
		{
			foreach (string needle in needles)
			{
				if (haystack.Contains(needle))
					return true;
			}

			return false;
		}
	}
}
