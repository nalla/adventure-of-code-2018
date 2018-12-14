using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace day09
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

			Play("Day 9", 418, 70769);
			Play("Part Two", 418, 70769 * 100);
			ctrlC.WaitOne();
		}

		private static void Play(string prefix, int numberOfPlayers, int numberOfMarbels)
		{
			var currentPlayer = 0;
			var score = new long[numberOfPlayers + 1];
			var gamefield = new LinkedList<int>();
			LinkedListNode<int> currentNode = gamefield.AddFirst(0);
			var consoleSpinner = new ConsoleSpiner();

			for (var i = 1; i <= numberOfMarbels; i++)
			{
				currentPlayer = Math.Max(1, ++currentPlayer % (numberOfPlayers + 1));

				if (i % 23 == 0)
				{
					score[currentPlayer] += i;
					var moveLeft = 7;

					while (moveLeft > 0)
					{
						moveLeft--;
						currentNode = currentNode.Previous ?? gamefield.Last;
					}

					score[currentPlayer] += currentNode.Value;

					LinkedListNode<int> next = currentNode.Next ?? gamefield.First;

					gamefield.Remove(currentNode);
					currentNode = next;
				}
				else
				{
					currentNode = gamefield.AddAfter(currentNode.Next ?? gamefield.First, i);
				}

				if (i % 1000 == 0)
				{
					consoleSpinner.Spin();
				}
			}

			consoleSpinner.Done();
			Console.WriteLine($"{prefix}: The winning elfs score is \"{score.Max()}\"");
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
