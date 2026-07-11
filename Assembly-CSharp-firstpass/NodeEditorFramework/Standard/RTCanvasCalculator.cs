using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NodeEditorFramework.Standard
{
	public class RTCanvasCalculator : MonoBehaviour
	{
		public NodeCanvas canvas { get; private set; }

		private void Start()
		{
			this.LoadCanvas(this.canvasPath);
		}

		public void AssureCanvas()
		{
			if (this.canvas == null)
			{
				this.LoadCanvas(this.canvasPath);
				if (this.canvas == null)
				{
					throw new UnityException("No canvas specified to calculate on " + base.name + "!");
				}
			}
		}

		public void LoadCanvas(string path)
		{
			this.canvasPath = path;
			if (!string.IsNullOrEmpty(this.canvasPath))
			{
				this.canvas = NodeEditorSaveManager.LoadNodeCanvas(this.canvasPath, true);
				this.CalculateCanvas();
				return;
			}
			this.canvas = null;
		}

		public void CalculateCanvas()
		{
			this.AssureCanvas();
			NodeEditor.RecalculateAll(this.canvas);
			this.DebugOutputResults();
		}

		private void DebugOutputResults()
		{
			this.AssureCanvas();
			foreach (Node node in this.getOutputNodes())
			{
				string text = "(OUT) " + node.name + ": ";
				if (node.Outputs.Count == 0)
				{
					using (List<NodeInput>.Enumerator enumerator2 = node.Inputs.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							NodeInput nodeInput = enumerator2.Current;
							text = string.Concat(new string[]
							{
								text,
								nodeInput.typeID,
								" ",
								nodeInput.IsValueNull ? "NULL" : nodeInput.GetValue().ToString(),
								"; "
							});
						}
						goto IL_0138;
					}
					goto IL_00BE;
				}
				goto IL_00BE;
				IL_0138:
				global::Debug.Log(text);
				continue;
				IL_00BE:
				foreach (NodeOutput nodeOutput in node.Outputs)
				{
					text = string.Concat(new string[]
					{
						text,
						nodeOutput.typeID,
						" ",
						nodeOutput.IsValueNull ? "NULL" : nodeOutput.GetValue().ToString(),
						"; "
					});
				}
				goto IL_0138;
			}
		}

		public List<Node> getInputNodes()
		{
			this.AssureCanvas();
			return this.canvas.nodes.Where<Node>(delegate(Node node)
			{
				if (node.Inputs.Count != 0 || node.Outputs.Count == 0)
				{
					return node.Inputs.TrueForAll((NodeInput input) => input.connection == null);
				}
				return true;
			}).ToList<Node>();
		}

		public List<Node> getOutputNodes()
		{
			this.AssureCanvas();
			return this.canvas.nodes.Where<Node>(delegate(Node node)
			{
				if (node.Outputs.Count != 0 || node.Inputs.Count == 0)
				{
					return node.Outputs.TrueForAll((NodeOutput output) => output.connections.Count == 0);
				}
				return true;
			}).ToList<Node>();
		}

		public string canvasPath;
	}
}
