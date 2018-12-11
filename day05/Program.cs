using System;
using System.IO;
using System.Threading;

namespace day05
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
			string input = File.ReadAllText("input.txt").Trim();
			string polymer = RemoveUnits(input);

			Console.WriteLine($"Day 5: The number of units that remain after a full scan is \"{polymer.Length}\".");

			int lowestUnitCount = int.MaxValue;

			foreach (char type in "abcdefghijklmnopqrstuvwxyz")
			{
				string polymerCandidate = polymer.Replace($"{type}", "").Replace($"{char.ToUpper(type)}", "");
				string reducedPolymerCandidate = RemoveUnits(polymerCandidate);

				if (reducedPolymerCandidate.Length < lowestUnitCount)
				{
					lowestUnitCount = reducedPolymerCandidate.Length;
				}
			}

			Console.WriteLine($"Part Two: The length of the shortest polymer possible is \"{lowestUnitCount}\".");
			ctrlC.WaitOne();
		}

		private static string RemoveUnits(string input)
		{
			while (true)
			{
				int previousLength = input.Length;
				foreach (char type in "abcdefghijklmnopqrstuvwxyz")
				{
					input = input.Replace($"{type}{char.ToUpper(type)}", "").Replace($"{char.ToUpper(type)}{type}", "");
				}

				if (previousLength == input.Length)
				{
					break;
				}
			}

			return input;
		}
	}
}
