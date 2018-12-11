using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace day03
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
			Claim[] claims = Parse(input).ToArray();
			int maxWidth = claims.Max(x => x.X + x.Width) + 1;
			int maxHeight = claims.Max(x => x.Y + x.Height) + 1;
			var fabric = new int [maxWidth, maxHeight];

			foreach (Claim claim in claims)
			{
				for (int i = claim.X; i < claim.X + claim.Width; i++)
				{
					for (int j = claim.Y; j < claim.Y + claim.Height; j++)
					{
						fabric[i, j]++;
					}
				}
			}

			var numberOfSquareInches = 0;

			for (var i = 0; i < maxWidth; i++)
			{
				for (var j = 0; j < maxHeight; j++)
				{
					if (fabric[i, j] >= 2)
					{
						numberOfSquareInches++;
					}
				}
			}

			Console.WriteLine($"Day 3: The number of square inces of fabric that are within two or more claims is \"{numberOfSquareInches}\"");

			foreach (Claim claim in claims)
			{
				var overlapes = false;

				for (int i = claim.X; i < claim.X + claim.Width; i++)
				{
					for (int j = claim.Y; j < claim.Y + claim.Height; j++)
					{
						if (fabric[i, j] > 1)
						{
							overlapes = true;
						}
					}
				}

				if (!overlapes)
				{
					Console.WriteLine($"Part Two: The id of the only claim that is not overlapping is \"{claim.Id}\"");

					break;
				}
			}

			ctrlC.WaitOne();
		}

		private static IEnumerable<Claim> Parse(string[] input)
		{
			foreach (string claim in input)
			{
				var regex = new Regex(@"^#(\d+) @ (\d+),(\d+): (\d+)x(\d+)$");
				Match match = regex.Match(claim);

				int id = int.Parse(match.Groups[1].ToString());
				int x = int.Parse(match.Groups[2].ToString());
				int y = int.Parse(match.Groups[3].ToString());
				int width = int.Parse(match.Groups[4].ToString());
				int height = int.Parse(match.Groups[5].ToString());

				yield return new Claim(id, x, y, width, height);
			}
		}

		private class Claim
		{
			public Claim(int id, int x, int y, int width, int height)
			{
				Id = id;
				X = x;
				Y = y;
				Width = width;
				Height = height;
			}

			public int Height { get; }

			public int Width { get; }

			public int Y { get; }

			public int X { get; }

			public int Id { get; }
		}
	}
}
