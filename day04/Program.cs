using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace day04
{
	internal class Program
	{
		private static ManualResetEvent GetExitEvent()
		{
			var exitEvent = new ManualResetEvent(false);

			Console.CancelKeyPress += (sender, e) =>
			{
				e.Cancel = true;
				exitEvent.Set();
			};

			return exitEvent;
		}

		private static void Main(string[] args)
		{
			ManualResetEvent ctrlC = GetExitEvent();
			string[] input = File.ReadAllLines("input.txt");
			IEnumerable<(DateTime logDate, bool wakes, bool falls, int? guard)> logEntries = GetOrderedLogEntries(input);
			var analysis = new Dictionary<int, Dictionary<int, int>>();
			var guardNo = 0;
			int? start = null;
			int? end = null;

			foreach ((DateTime logDate, bool wakes, bool falls, int? guard) in logEntries)
			{
				if (guard.HasValue)
				{
					guardNo = guard.Value;
				}

				if (falls)
				{
					start = logDate.Minute;
				}

				if (wakes)
				{
					end = logDate.Minute;
				}

				if (start.HasValue && end.HasValue)
				{
					if (!analysis.TryGetValue(guardNo, out Dictionary<int, int> data))
					{
						data = new Dictionary<int, int>();
						Initialize(data);
						analysis.Add(guardNo, data);
					}

					for (int i = start.Value; i <= end.Value; i++)
					{
						data[i]++;
					}

					start = null;
					end = null;
				}
			}

			var foundGuard = 0;
			var minutesAtSleep = 0;
			var mostSleptMinute = 0;

			foreach (KeyValuePair<int, Dictionary<int, int>> keyValuePair in analysis)
			{
				int minutesAtSleepToAnalyze = keyValuePair.Value.Sum(x => x.Value);

				if (minutesAtSleepToAnalyze > minutesAtSleep)
				{
					foundGuard = keyValuePair.Key;
					minutesAtSleep = minutesAtSleepToAnalyze;
					mostSleptMinute = keyValuePair.Value.First(x => x.Value == keyValuePair.Value.Max(y => y.Value)).Key;
				}
			}

			Console.WriteLine($"Day 4: The product of the id of the guard and the most slept minute is \"{foundGuard * mostSleptMinute}\".");

			var highestMinuteValue = 0;
			var highestMinute = 0;

			foreach (KeyValuePair<int, Dictionary<int, int>> keyValuePair in analysis)
			{
				int highestMinuteToAnalyze = keyValuePair.Value.Max(x => x.Value);

				if (highestMinuteToAnalyze > highestMinuteValue)
				{
					foundGuard = keyValuePair.Key;
					highestMinuteValue = highestMinuteToAnalyze;
					highestMinute = keyValuePair.Value.First(x => x.Value == keyValuePair.Value.Max(y => y.Value)).Key;
				}
			}

			Console.WriteLine($"Part Two: The product of the id of the guard and the highest slept minute is \"{foundGuard * highestMinute}\".");
			ctrlC.WaitOne();
		}

		private static void Initialize(Dictionary<int, int> data)
		{
			for (var i = 0; i <= 59; i++)
			{
				data.Add(i, 0);
			}
		}

		private static IEnumerable<(DateTime logDate, bool wakes, bool falls, int? guard)> GetOrderedLogEntries(string[] input) =>
			input.Select(Parse).OrderBy(x => x.logDate);

		private static (DateTime logDate, bool wakes, bool falls, int? guard) Parse(string logEntry)
		{
			Match match = Regex.Match(logEntry, @"^\[(\d+)-(\d+)-(\d+) (\d+)\:(\d+)\] (falls|wakes|Guard \#(\d+) begins)");

			int year = int.Parse(match.Groups[1].ToString());
			int month = int.Parse(match.Groups[2].ToString());
			int day = int.Parse(match.Groups[3].ToString());
			int hour = int.Parse(match.Groups[4].ToString());
			int min = int.Parse(match.Groups[5].ToString());
			bool falls = match.Groups[6].ToString().Equals("falls");
			bool wakes = match.Groups[6].ToString().Equals("wakes");
			int? guard = null;

			if (match.Groups[6].ToString().StartsWith("Guard"))
			{
				guard = int.Parse(match.Groups[7].ToString());
			}

			return (new DateTime(year, month, day, hour, min, 0, 0), wakes, falls, guard);
		}
	}
}
