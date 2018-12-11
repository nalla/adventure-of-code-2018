using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace day01
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
			int[] frequencyChanges = input.Select(int.Parse).ToArray();

			var frequency = 0;
			var knownFrequencies = new HashSet<int> { 0 };


			foreach (int change in frequencyChanges)
			{
				frequency += change;
				knownFrequencies.Add(frequency);
			}

			Console.WriteLine($"Day 1: The resulting frequency after all of the changes in frequency is \"{frequency}\".");

			var loop = true;

			while (loop)
			{
				foreach (int change in frequencyChanges)
				{
					frequency += change;

					if (knownFrequencies.Contains(frequency))
					{
						Console.WriteLine($"Part Two: The first frequency reached twice is \"{frequency}\".");
						loop = false;

						break;
					}
				}
			}

			ctrlC.WaitOne();
		}
	}
}
