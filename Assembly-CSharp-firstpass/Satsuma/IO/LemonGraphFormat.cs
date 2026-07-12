using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Satsuma.IO
{
	public sealed class LemonGraphFormat
	{
		public IGraph Graph { get; set; }

		public Dictionary<string, Dictionary<Node, string>> NodeMaps { get; private set; }

		public Dictionary<string, Dictionary<Arc, string>> ArcMaps { get; private set; }

		public Dictionary<string, string> Attributes { get; private set; }

		public LemonGraphFormat()
		{
			this.NodeMaps = new Dictionary<string, Dictionary<Node, string>>();
			this.ArcMaps = new Dictionary<string, Dictionary<Arc, string>>();
			this.Attributes = new Dictionary<string, string>();
		}

		private static string Escape(string s)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int i = 0;
			while (i < s.Length)
			{
				char c = s[i];
				switch (c)
				{
				case '\t':
					stringBuilder.Append("\\t");
					break;
				case '\n':
					stringBuilder.Append("\\n");
					break;
				case '\v':
				case '\f':
					goto IL_0086;
				case '\r':
					stringBuilder.Append("\\r");
					break;
				default:
					if (c != '"')
					{
						if (c != '\\')
						{
							goto IL_0086;
						}
						stringBuilder.Append("\\\\");
					}
					else
					{
						stringBuilder.Append("\\\"");
					}
					break;
				}
				IL_008E:
				i++;
				continue;
				IL_0086:
				stringBuilder.Append(c);
				goto IL_008E;
			}
			return stringBuilder.ToString();
		}

		private static string Unescape(string s)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			foreach (char c in s)
			{
				if (flag)
				{
					if (c != 'n')
					{
						if (c != 'r')
						{
							if (c != 't')
							{
								stringBuilder.Append(c);
							}
							else
							{
								stringBuilder.Append('\t');
							}
						}
						else
						{
							stringBuilder.Append('\r');
						}
					}
					else
					{
						stringBuilder.Append('\n');
					}
					flag = false;
				}
				else
				{
					flag = c == '\\';
					if (!flag)
					{
						stringBuilder.Append(c);
					}
				}
			}
			return stringBuilder.ToString();
		}

		public void Load(TextReader reader, Directedness? directedness)
		{
			if (this.Graph == null)
			{
				this.Graph = new CustomGraph();
			}
			IBuildableGraph buildableGraph = (IBuildableGraph)this.Graph;
			buildableGraph.Clear();
			this.NodeMaps.Clear();
			Dictionary<string, Node> dictionary = new Dictionary<string, Node>();
			this.ArcMaps.Clear();
			this.Attributes.Clear();
			Regex regex = new Regex("\\s*(?:(\"(?:\\\"|.)*\")|(\\S+))\\s*", RegexOptions.None);
			string text = "";
			Directedness directedness2 = Directedness.Directed;
			bool flag = false;
			List<string> list = null;
			int num = -1;
			for (;;)
			{
				string text2 = reader.ReadLine();
				if (text2 == null)
				{
					break;
				}
				text2 = text2.Trim();
				if (!(text2 == "") && text2[0] != '#')
				{
					List<string> list2 = regex.Matches(text2).Cast<Match>().Select<Match, string>(delegate(Match m)
					{
						string text6 = m.Value;
						if (text6 == "")
						{
							return text6;
						}
						if (text6[0] == '"' && text6[text6.Length - 1] == '"')
						{
							text6 = LemonGraphFormat.Unescape(text6.Substring(1, text6.Length - 2));
						}
						return text6;
					})
						.ToList<string>();
					string text3 = list2.First<string>();
					if (text2[0] == '@')
					{
						text = text3.Substring(1);
						directedness2 = directedness ?? ((text == "arcs") ? Directedness.Directed : Directedness.Undirected);
						flag = true;
					}
					else
					{
						if (text != null)
						{
							if (!(text == "nodes") && !(text == "red_nodes") && !(text == "blue_nodes"))
							{
								if (!(text == "arcs") && !(text == "edges"))
								{
									if (text == "attributes")
									{
										this.Attributes[list2[0]] = list2[1];
									}
								}
								else
								{
									if (flag)
									{
										list = list2;
										using (List<string>.Enumerator enumerator = list.GetEnumerator())
										{
											while (enumerator.MoveNext())
											{
												string text4 = enumerator.Current;
												if (!this.ArcMaps.ContainsKey(text4))
												{
													this.ArcMaps[text4] = new Dictionary<Arc, string>();
												}
											}
											goto IL_0323;
										}
									}
									Node node = dictionary[list2[0]];
									Node node2 = dictionary[list2[1]];
									Arc arc = buildableGraph.AddArc(node, node2, directedness2);
									for (int i = 2; i < list2.Count; i++)
									{
										this.ArcMaps[list[i - 2]][arc] = list2[i];
									}
								}
							}
							else if (flag)
							{
								list = list2;
								for (int j = 0; j < list.Count; j++)
								{
									string text5 = list[j];
									if (text5 == "label")
									{
										num = j;
									}
									if (!this.NodeMaps.ContainsKey(text5))
									{
										this.NodeMaps[text5] = new Dictionary<Node, string>();
									}
								}
							}
							else
							{
								Node node3 = buildableGraph.AddNode();
								for (int k = 0; k < list2.Count; k++)
								{
									this.NodeMaps[list[k]][node3] = list2[k];
									if (k == num)
									{
										dictionary[list2[k]] = node3;
									}
								}
							}
						}
						IL_0323:
						flag = false;
					}
				}
			}
		}

		public void Load(string filename, Directedness? directedness)
		{
			using (StreamReader streamReader = new StreamReader(filename))
			{
				this.Load(streamReader, directedness);
			}
		}

		public void Save(TextWriter writer, IEnumerable<string> comment = null)
		{
			if (comment != null)
			{
				foreach (string text in comment)
				{
					writer.WriteLine("# " + text);
				}
			}
			writer.WriteLine("@nodes");
			writer.Write("label");
			foreach (KeyValuePair<string, Dictionary<Node, string>> keyValuePair in this.NodeMaps)
			{
				if (keyValuePair.Key != "label")
				{
					writer.Write(" " + keyValuePair.Key);
				}
			}
			writer.WriteLine();
			foreach (Node node in this.Graph.Nodes())
			{
				writer.Write(node.Id);
				foreach (KeyValuePair<string, Dictionary<Node, string>> keyValuePair2 in this.NodeMaps)
				{
					if (keyValuePair2.Key != "label")
					{
						string text2;
						if (!keyValuePair2.Value.TryGetValue(node, out text2))
						{
							text2 = "";
						}
						writer.Write(" \"" + LemonGraphFormat.Escape(text2) + "\"");
					}
				}
				writer.WriteLine();
			}
			writer.WriteLine();
			for (int i = 0; i < 2; i++)
			{
				IEnumerable<Arc> enumerable = ((i == 0) ? (from arc in this.Graph.Arcs(ArcFilter.All)
					where !this.Graph.IsEdge(arc)
					select arc) : this.Graph.Arcs(ArcFilter.Edge));
				writer.WriteLine((i == 0) ? "@arcs" : "@edges");
				if (this.ArcMaps.Count == 0)
				{
					writer.WriteLine('-');
				}
				else
				{
					foreach (KeyValuePair<string, Dictionary<Arc, string>> keyValuePair3 in this.ArcMaps)
					{
						writer.Write(keyValuePair3.Key + " ");
					}
					writer.WriteLine();
				}
				foreach (Arc arc2 in enumerable)
				{
					writer.Write(this.Graph.U(arc2).Id + 32L + this.Graph.V(arc2).Id);
					foreach (KeyValuePair<string, Dictionary<Arc, string>> keyValuePair4 in this.ArcMaps)
					{
						string text3;
						if (!keyValuePair4.Value.TryGetValue(arc2, out text3))
						{
							text3 = "";
						}
						writer.Write(" \"" + LemonGraphFormat.Escape(text3) + "\"");
					}
					writer.WriteLine();
				}
				writer.WriteLine();
			}
			if (this.Attributes.Count > 0)
			{
				writer.WriteLine("@attributes");
				foreach (KeyValuePair<string, string> keyValuePair5 in this.Attributes)
				{
					writer.WriteLine(string.Concat(new string[]
					{
						"\"",
						LemonGraphFormat.Escape(keyValuePair5.Key),
						"\" \"",
						LemonGraphFormat.Escape(keyValuePair5.Value),
						"\""
					}));
				}
				writer.WriteLine();
			}
		}

		public void Save(string filename, IEnumerable<string> comment = null)
		{
			using (StreamWriter streamWriter = new StreamWriter(filename))
			{
				this.Save(streamWriter, comment);
			}
		}
	}
}
