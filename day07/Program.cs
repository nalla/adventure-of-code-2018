using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace day07
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
			IEnumerable<(char dependency, char step)> input = Parse(lines).ToArray();
			(string sequence, _) = Work(1, GetSteps(input));

			Console.WriteLine($"Day 7: The steps should be completed in the order \"{sequence}\".");

			(_, int seconds) = Work(5, GetSteps(input));

			Console.WriteLine($"Part Two: With 5 workers it will tage \"{seconds}\" to complete all steps.");
			ctrlC.WaitOne();
		}

		private static Dictionary<char, List<char>> GetSteps(IEnumerable<(char dependency, char step)> input)
		{
			var steps = new Dictionary<char, List<char>>();

			foreach ((char dependency, char step) in input)
			{
				if (!steps.TryGetValue(step, out List<char> dependencies))
				{
					dependencies = new List<char>();
					steps.Add(step, dependencies);
				}

				dependencies.Add(dependency);
			}

			return steps;
		}

		private static (string sequence, int seconds) Work(int numberOfWorkers, Dictionary<char, List<char>> steps)
		{
			var sequence = new StringBuilder();
			var seconds = 0;
			var workers = new List<Worker>();

			for (var i = 0; i < numberOfWorkers; i++)
			{
				workers.Add(new Worker());
			}

			while (steps.Any())
			{
				var nextStep = 'A';

				while (workers.Any(x => !x.Busy) && nextStep != default)
				{
					nextStep = GetNextChar(steps, workers);

					if (nextStep != default)
					{
						Worker worker = workers.FirstOrDefault(x => !x.Busy);

						worker?.Start(nextStep, step =>
						{
							sequence.Append(step);
							RemoveDependency(step, steps);

							if (steps.ContainsKey(step) && steps[step].Count == 0)
							{
								steps.Remove(step);
							}
						});
					}
				}

				foreach (Worker worker in workers)
				{
					worker.Work();
				}

				seconds++;
			}

			return (sequence.ToString(), seconds);
		}

		private static char GetNextChar(Dictionary<char, List<char>> steps, List<Worker> workers)
		{
			IOrderedEnumerable<char> allDependencies = steps.SelectMany(x => x.Value).Distinct().Where(x => workers.All(y => y.Step != x)).OrderBy(x => x);
			char a = allDependencies.FirstOrDefault(x => !steps.Keys.Contains(x));
			char b = steps.Where(x => workers.All(y => y.Step != x.Key)).FirstOrDefault(x => x.Value.Count == 0).Key;

			if (a == default)
			{
				return b;
			}

			if (b == default)
			{
				return a;
			}

			return a < b ? a : b;
		}


		private static void RemoveDependency(char c, Dictionary<char, List<char>> steps)
		{
			foreach (List<char> dependencies in steps.Values)
			{
				dependencies.Remove(c);
			}
		}

		private static IEnumerable<(char dependency, char step)> Parse(string[] input) => input.Select(Parse).OrderBy(x => x.step);

		private static (char dependency, char step) Parse(string line)
		{
			Match match = Regex.Match(line, @"^Step ([A-Z]) [\w\s]+ ([A-Z])");

			char dependency = char.Parse(match.Groups[1].ToString());
			char step = char.Parse(match.Groups[2].ToString());

			return (dependency, step);
		}

		private class Worker
		{
			private Action<char> finishCallback;
			private int ticks;

			public bool Busy { get; private set; }

			public char Step { get; private set; }

			public void Start(char step, Action<char> callback)
			{
				Step = step;
				finishCallback = callback;
				Busy = true;
				ticks = 60 + (step - 64);
			}

			public void Work()
			{
				if (Busy)
				{
					ticks--;

					if (ticks == 0)
					{
						finishCallback(Step);
						Busy = false;
						Step = default;
					}
				}
			}
		}
	}
}
