using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace Satsuma.IO
{
	public sealed class SimpleGraphFormat
	{
		public SimpleGraphFormat()
		{
			this.Extensions = new List<Dictionary<Arc, string>>();
		}

		public IGraph Graph { get; set; }

		public IList<Dictionary<Arc, string>> Extensions { get; private set; }

		public int StartIndex { get; set; }

		public Node[] Load(TextReader reader, Directedness directedness)
		{
			if (this.Graph == null)
			{
				this.Graph = new CustomGraph();
			}
			IBuildableGraph buildableGraph = (IBuildableGraph)this.Graph;
			buildableGraph.Clear();
			Regex regex = new Regex("\\s+");
			string[] array = regex.Split(reader.ReadLine());
			int num = int.Parse(array[0], CultureInfo.InvariantCulture);
			int num2 = int.Parse(array[1], CultureInfo.InvariantCulture);
			Node[] array2 = new Node[num];
			for (int i = 0; i < num; i++)
			{
				array2[i] = buildableGraph.AddNode();
			}
			this.Extensions.Clear();
			for (int j = 0; j < num2; j++)
			{
				array = regex.Split(reader.ReadLine());
				int num3 = (int)(long.Parse(array[0], CultureInfo.InvariantCulture) - (long)this.StartIndex);
				int num4 = (int)(long.Parse(array[1], CultureInfo.InvariantCulture) - (long)this.StartIndex);
				Arc arc = buildableGraph.AddArc(array2[num3], array2[num4], directedness);
				int num5 = array.Length - 2;
				for (int k = 0; k < num5 - this.Extensions.Count; k++)
				{
					this.Extensions.Add(new Dictionary<Arc, string>());
				}
				for (int l = 0; l < num5; l++)
				{
					this.Extensions[l][arc] = array[2 + l];
				}
			}
			return array2;
		}

		public Node[] Load(string filename, Directedness directedness)
		{
			Node[] array;
			using (StreamReader streamReader = new StreamReader(filename))
			{
				array = this.Load(streamReader, directedness);
			}
			return array;
		}

		public void Save(TextWriter writer)
		{
			Regex regex = new Regex("\\s");
			writer.WriteLine(this.Graph.NodeCount() + " " + this.Graph.ArcCount(ArcFilter.All));
			Dictionary<Node, long> dictionary = new Dictionary<Node, long>();
			long num = (long)this.StartIndex;
			foreach (Arc arc in this.Graph.Arcs(ArcFilter.All))
			{
				Node node = this.Graph.U(arc);
				long num2;
				if (!dictionary.TryGetValue(node, out num2))
				{
					Dictionary<Node, long> dictionary2 = dictionary;
					Node node2 = node;
					long num3 = num;
					num = num3 + 1L;
					num2 = num3;
					dictionary2[node2] = num3;
				}
				Node node3 = this.Graph.V(arc);
				long num4;
				if (!dictionary.TryGetValue(node3, out num4))
				{
					Dictionary<Node, long> dictionary3 = dictionary;
					Node node4 = node3;
					long num5 = num;
					num = num5 + 1L;
					num4 = num5;
					dictionary3[node4] = num5;
				}
				writer.Write(num2 + " " + num4);
				foreach (Dictionary<Arc, string> dictionary4 in this.Extensions)
				{
					string text;
					dictionary4.TryGetValue(arc, out text);
					if (string.IsNullOrEmpty(text) || regex.IsMatch(text))
					{
						throw new ArgumentException("Extension value is empty or contains whitespaces.");
					}
					writer.Write(' ' + dictionary4[arc]);
				}
				writer.WriteLine();
			}
		}

		public void Save(string filename)
		{
			using (StreamWriter streamWriter = new StreamWriter(filename))
			{
				this.Save(streamWriter);
			}
		}
	}
}
