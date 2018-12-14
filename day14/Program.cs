using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace day14
{
	internal class Program
	{
		private static int receipeIndex;

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

		private static int GetNextReceipeIndex()
		{
			receipeIndex++;

			return receipeIndex;
		}

		private static void Main(string[] args)
		{
			ManualResetEvent ctrlC = GetExitEvent();
			var recipes = new LinkedList<Recipe>();
			var elves = new[] { new Elf(), new Elf() };
			var sequence = "030121";
			int numberOfRecipes = int.Parse(sequence);

			elves[0].CurrentRecipe = recipes.AddFirst(new Recipe(GetNextReceipeIndex(), 3));
			elves[1].CurrentRecipe = recipes.AddLast(new Recipe(GetNextReceipeIndex(), 7));

			var consoleSpinner = new ConsoleSpiner();

			for (var i = 0; i < numberOfRecipes + 10; i++)
			{
				if (i % 1000 == 0)
				{
					consoleSpinner.Spin();
				}

				GenerateRecipes(recipes, elves);
			}

			consoleSpinner.Done();

			LinkedListNode<Recipe> next = recipes.First;

			for (var i = 0; i < numberOfRecipes; i++)
			{
				next = next.Next;
			}

			var sb = new StringBuilder();

			for (var j = 0; j < 10; j++)
			{
				sb.Append(next.Value.Score);
				next = next.Next;
			}

			Console.WriteLine($"Day 14: The score of the next then recipes after the first \"{numberOfRecipes}\" is \"{sb}\"");

			next = recipes.First;
			var predicate = new int[sequence.Length];
			var k = 0;

			while (true)
			{
				k++;

				if (k % 100000 == 0)
				{
					consoleSpinner.Spin();
				}

				Array.Copy(predicate, 1, predicate, 0, predicate.Length - 1);
				predicate[predicate.Length - 1] = next.Value.Score;

				if (string.Join("", predicate) == sequence)
				{
					break;
				}

				if (next.Next == null)
				{
					GenerateRecipes(recipes, elves);
				}

				next = next.Next;
			}

			consoleSpinner.Done();
			Console.WriteLine($"Part Two: \"{next.Previous.Value.Index + 1 - sequence.Length}\" recipies appear on the scoreboard to the left.");

			ctrlC.WaitOne();
		}

		private static void GenerateRecipes(LinkedList<Recipe> recipes, Elf[] elves)
		{
			foreach (char c in elves.Sum(x => x.CurrentRecipe.Value.Score).ToString())
			{
				recipes.AddLast(new Recipe(GetNextReceipeIndex(), int.Parse($"{c}")));
			}

			foreach (Elf elf in elves)
			{
				int steps = 1 + elf.CurrentRecipe.Value.Score;
				LinkedListNode<Recipe> next = elf.CurrentRecipe;

				for (var j = 0; j < steps; j++)
				{
					next = next.Next ?? recipes.First;
				}

				elf.CurrentRecipe = next;
			}
		}

		private class Elf
		{
			public LinkedListNode<Recipe> CurrentRecipe { get; set; }
		}

		private class Recipe
		{
			public Recipe(int index, int score)
			{
				Index = index;
				Score = score;
			}

			public int Score { get; }

			public int Index { get; }
		}

		private class ConsoleSpiner
		{
			private int counter;

			public void Spin()
			{
				counter++;

				switch (counter % 4)
				{
					case 0:
						Console.Write("/");

						break;
					case 1:
						Console.Write("-");

						break;
					case 2:
						Console.Write("\\");

						break;
					case 3:
						Console.Write("|");

						break;
				}

				Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
			}

			public void Done()
			{
				Console.Write(" ");
				Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
			}
		}
	}
}
