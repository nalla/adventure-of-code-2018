using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace day08
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
			List<int> input = File.ReadAllText("input.txt").Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
			Node node = ReadNode(input);

			Console.WriteLine($"Day 8: The sum of all metadata entries is \"{Flatten(new[] { node }).SelectMany(x => x.MetadataEntries).Sum()}\".");
			Console.WriteLine($"Part Two: The root node value is \"{CalculateNodeValue(node)}\".");
			ctrlC.WaitOne();
		}

		private static int CalculateNodeValue(Node node)
		{
			if (node.ChildNodes.Length == 0)
			{
				return node.MetadataEntries.Sum();
			}

			var nodeValue = 0;

			foreach (int metadataEntry in node.MetadataEntries)
			{
				if (metadataEntry - 1 < node.ChildNodes.Length)
				{
					nodeValue += CalculateNodeValue(node.ChildNodes[metadataEntry - 1]);
				}
			}

			return nodeValue;
		}

		private static Node ReadNode(List<int> input)
		{
			int numberOfChildNodes = input.First();
			input.RemoveAt(0);
			int numberOfMetadataEntries = input.First();
			input.RemoveAt(0);
			var node = new Node
			{
				ChildNodes = new Node[numberOfChildNodes],
				MetadataEntries = new int[numberOfMetadataEntries]
			};

			for (var i = 0; i < numberOfChildNodes; i++)
			{
				node.ChildNodes[i] = ReadNode(input);
			}

			for (var i = 0; i < numberOfMetadataEntries; i++)
			{
				node.MetadataEntries[i] = input.First();
				input.RemoveAt(0);
			}

			return node;
		}

		private static IEnumerable<Node> Flatten(IEnumerable<Node> e) => e.SelectMany(c => Flatten(c.ChildNodes)).Concat(e);

		private class Node
		{
			public Node[] ChildNodes { get; set; }

			public int[] MetadataEntries { get; set; }
		}
	}
}
