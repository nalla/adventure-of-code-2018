using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace day02
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
			var numberOfTwoTimes = 0;
			var numberOfThreeTimes = 0;

			foreach (string boxid in input)
			{
				var foundTwoTimes = false;
				var foundThreeTimes = false;

				foreach (char c in boxid)
				{
					int count = boxid.Count(x => x == c);

					if (!foundTwoTimes && count == 2)
					{
						numberOfTwoTimes++;
						foundTwoTimes = true;
					}

					if (!foundThreeTimes && count == 3)
					{
						numberOfThreeTimes++;
						foundThreeTimes = true;
					}

					if (foundTwoTimes && foundThreeTimes)
					{
						break;
					}
				}
			}

			Console.WriteLine($"Day 2: The checksum of the box IDs is \"{numberOfTwoTimes * numberOfThreeTimes}\".");

			var loop = true;

			foreach (string boxid in input)
			{
				IEnumerable<string> partialInput = input.Where(x => x != boxid).ToArray();

				for (var i = 0; i < boxid.Length; i++)
				{
					int index = i;
					string candidate = boxid.Remove(i, 1);
					IEnumerable<string> compareData = partialInput.Select(x => x.Remove(index, 1));

					if (compareData.Contains(candidate))
					{
						Console.WriteLine($"Part Two: The letters that are comon between the two correct box ids are \"{candidate}\".");
						loop = false;

						break;
					}
				}

				if (!loop)
				{
					break;
				}
			}

			ctrlC.WaitOne();
		}
	}
}
