using System;
using System.Collections.Generic;
using UnityEngine;

public class LineLayer : GraphLayer
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	public void NewLine(Vector2[] points, string ID = "")
	{
		GameObject gameObject = Util.KInstantiateUI(this.prefab_line, this.line_container, true);
		if (ID == "")
		{
			ID = this.lines.Count.ToString();
		}
		gameObject.name = string.Format("line_{0}", ID);
		GraphedLine component = gameObject.GetComponent<GraphedLine>();
		component.SetPoints(points);
		component.line_renderer.color = this.line_formatting[this.lines.Count % this.line_formatting.Length].color;
		component.line_renderer.LineThickness = (float)this.line_formatting[this.lines.Count % this.line_formatting.Length].thickness;
		this.lines.Add(component);
	}

	public void ClearLines()
	{
		foreach (GraphedLine graphedLine in this.lines)
		{
			if (graphedLine != null && graphedLine.gameObject != null)
			{
				global::UnityEngine.Object.DestroyImmediate(graphedLine.gameObject);
			}
		}
		this.lines.Clear();
	}

	[Header("Lines")]
	public LineLayer.LineFormat[] line_formatting;

	public GameObject prefab_line;

	public GameObject line_container;

	private List<GraphedLine> lines = new List<GraphedLine>();

	[Serializable]
	public struct LineFormat
	{
		public Color color;

		public int thickness;
	}
}
