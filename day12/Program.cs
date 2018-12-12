using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace day12
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
			Dictionary<string, bool> instructions = GetInstructions(lines.Skip(2));
			int count = Evolve(instructions, GetPots(lines.First()), 20);

			Console.WriteLine($"Day 8: After 20 generations the sum of the pots that has plants is \"{count}\"");

			// You need to visual inspect the difference that will be printed when you uncomment
			// the line in the evolve function. You will notice that from generation 98 on forward
			// the difference will stay equal.
			const int difference = 62;

			count = Evolve(instructions, GetPots(lines.First()), 100);
			Console.WriteLine($"Part Two: After 50000000000 generations the sum of the pots that has plants is \"{count + (50000000000 - 100) * difference}\".");
			ctrlC.WaitOne();
		}

		private static int Evolve(Dictionary<string, bool> instructions, LinkedList<Pot> pots, long generations)
		{
			var lastCount = 0;

			for (long i = 0; i < generations; i++)
			{
				foreach (Pot node in pots.ToArray().AsParallel())
				{
					LinkedListNode<Pot> c = pots.Find(node);
					LinkedListNode<Pot> l1 = c.Previous ?? pots.AddBefore(c, new Pot(node.Index - 1, false));
					LinkedListNode<Pot> l2 = l1.Previous ?? pots.AddBefore(l1, new Pot(l1.Value.Index - 1, false));
					LinkedListNode<Pot> r1 = c.Next ?? pots.AddAfter(c, new Pot(node.Index + 1, false));
					LinkedListNode<Pot> r2 = r1.Next ?? pots.AddAfter(r1, new Pot(r1.Value.Index + 1, false));
					string key = $"{l2.Value.Plant}{l1.Value.Plant}{c.Value.Plant}{r1.Value.Plant}{r2.Value.Plant}";

					c.Value.HasPlantInNextGeneration = instructions[key];
				}

				foreach (Pot node in pots)
				{
					node.Evolve();
				}

				int count = pots.Where(x => x.HasPlant).Sum(x => x.Index);

				//Console.WriteLine(count - lastCount);
				lastCount = pots.Where(x => x.HasPlant).Sum(x => x.Index);
			}

			return pots.Where(x => x.HasPlant).Sum(x => x.Index);
		}

		private static Dictionary<string, bool> GetInstructions(IEnumerable<string> lines)
		{
			var instructions = new Dictionary<string, bool>();

			foreach (string line in lines)
			{
				string key = line.Substring(0, 5);
				bool value = line.Last() == '#';

				instructions.Add(key, value);
			}

			return instructions;
		}

		private static LinkedList<Pot> GetPots(string line)
		{
			string input = line.Replace("initial state: ", "");
			var list = new LinkedList<Pot>();

			for (var i = 0; i < input.Length; i++)
			{
				list.AddLast(new Pot(i, input[i] == '#'));
			}

			list.AddBefore(list.First, new Pot(-1, false));
			list.AddBefore(list.First, new Pot(-2, false));

			return list;
		}

		private class Pot
		{
			public Pot(int index, bool hasPlant)
			{
				Index = index;
				HasPlant = hasPlant;
			}

			public bool HasPlant { get; private set; }

			public char Plant => HasPlant ? '#' : '.';

			public bool HasPlantInNextGeneration { private get; set; }

			public int Index { get; }

			public void Evolve() => HasPlant = HasPlantInNextGeneration;
		}
	}
}
