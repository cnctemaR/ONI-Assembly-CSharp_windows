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
			}
			else
			{
				this.canvas = null;
			}
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
			List<Node> outputNodes = this.getOutputNodes();
			foreach (Node node in outputNodes)
			{
				string text = "(OUT) " + node.name + ": ";
				if (node.Outputs.Count == 0)
				{
					foreach (NodeInput nodeInput in node.Inputs)
					{
						string text2 = text;
						text = string.Concat(new string[]
						{
							text2,
							nodeInput.typeID,
							" ",
							(!nodeInput.IsValueNull) ? nodeInput.GetValue().ToString() : "NULL",
							"; "
						});
					}
				}
				else
				{
					foreach (NodeOutput nodeOutput in node.Outputs)
					{
						string text2 = text;
						text = string.Concat(new string[]
						{
							text2,
							nodeOutput.typeID,
							" ",
							(!nodeOutput.IsValueNull) ? nodeOutput.GetValue().ToString() : "NULL",
							"; "
						});
					}
				}
				Debug.Log(text);
			}
		}

		public List<Node> getInputNodes()
		{
			this.AssureCanvas();
			return this.canvas.nodes.Where<Node>(delegate(Node node)
			{
				bool flag;
				if (node.Inputs.Count != 0 || node.Outputs.Count == 0)
				{
					flag = node.Inputs.TrueForAll((NodeInput input) => input.connection == null);
				}
				else
				{
					flag = true;
				}
				return flag;
			}).ToList<Node>();
		}

		public List<Node> getOutputNodes()
		{
			this.AssureCanvas();
			return this.canvas.nodes.Where<Node>(delegate(Node node)
			{
				bool flag;
				if (node.Outputs.Count != 0 || node.Inputs.Count == 0)
				{
					flag = node.Outputs.TrueForAll((NodeOutput output) => output.connections.Count == 0);
				}
				else
				{
					flag = true;
				}
				return flag;
			}).ToList<Node>();
		}

		public string canvasPath;
	}
}
