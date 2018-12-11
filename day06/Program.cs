using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace day06
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
			string[] lines = File.ReadAllLines("input.txt");
			Point[] points = Parse(lines).ToArray();
			int maxX = points.Max(x => x.X);
			int maxY = points.Max(x => x.Y);
			var grid = new int[maxX + 1, maxY + 1];
			var regions = new Dictionary<int, int>();

			for (var x = 0; x <= maxX; x++)
			{
				for (var y = 0; y <= maxY; y++)
				{
					int bestDistance = maxX + maxY;
					int bestId = -1;

					// find distance to closest point
					foreach (Point p in points)
					{
						int dist = Math.Abs(x - p.X) + Math.Abs(y - p.Y);
						if (dist < bestDistance)
						{
							bestDistance = dist;
							bestId = p.Id;
						}
						else if (dist == bestDistance)
						{
							bestId = -1;
						}
					}

					grid[x, y] = bestId;

					regions.TryGetValue(bestId, out int total);
					total++;
					regions[bestId] = total;
				}
			}

			for (var x = 0; x <= maxX; x++)
			{
				int infinite = grid[x, 0];

				regions.Remove(infinite);
				infinite = grid[x, maxY];
				regions.Remove(infinite);
			}

			for (var y = 0; y <= maxY; y++)
			{
				int infinite = grid[0, y];

				regions.Remove(infinite);
				infinite = grid[maxX, y];
				regions.Remove(infinite);
			}

			Console.WriteLine($"Day 6: The size of the largest area that isn't infinitive is \"{regions.Values.Max()}\".");

			var lessThan = 0;

			for (var x = 0; x <= maxX; x++)
			{
				for (var y = 0; y <= maxY; y++)
				{
					var size = 0;

					foreach (Point p in points)
					{
						size += Math.Abs(x - p.X) + Math.Abs(y - p.Y);
					}

					if (size < 10000)
					{
						lessThan++;
					}
				}
			}

			Console.WriteLine($"Part Two: The size of the region containing all locations with total distance of less than 10000 is \"{lessThan}\".");

			ctrlC.WaitOne();
		}

		private static IEnumerable<Point> Parse(string[] lines)
		{
			var id = 0;

			foreach (string line in lines)
			{
				Match match = Regex.Match(line, @"(\d+), (\d+)");

				yield return new Point(int.Parse(match.Groups[1].ToString()), int.Parse(match.Groups[2].ToString()), id);

				id++;
			}
		}

		private static int ManhattanDistance(int x1, int x2, int y1, int y2) => Math.Abs(x1 - x2) + Math.Abs(y1 - y2);

		private class Point
		{
			public Point(int x, int y, int id)
			{
				Id = id;
				X = x;
				Y = y;
			}

			public int Id { get; }

			public int X { get; }

			public int Y { get; }
		}
	}
}
