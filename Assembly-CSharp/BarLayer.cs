using System;
using System.Collections.Generic;
using UnityEngine;

public class BarLayer : GraphLayer
{
	public int bar_count
	{
		get
		{
			return this.bars.Count;
		}
	}

	public void NewBar(int[] values, float x_position, string ID = "")
	{
		GameObject gameObject = Util.KInstantiateUI(this.prefab_bar, this.bar_container, true);
		if (ID == "")
		{
			ID = this.bars.Count.ToString();
		}
		gameObject.name = string.Format("bar_{0}", ID);
		GraphedBar component = gameObject.GetComponent<GraphedBar>();
		component.SetFormat(this.bar_formats[this.bars.Count % this.bar_formats.Length]);
		int[] array = new int[values.Length];
		for (int i = 0; i < values.Length; i++)
		{
			array[i] = (int)(base.graph.rectTransform().rect.height * base.graph.GetRelativeSize(new Vector2(0f, (float)values[i])).y);
		}
		component.SetValues(array, base.graph.GetRelativePosition(new Vector2(x_position, 0f)).x);
		this.bars.Add(component);
	}

	public void ClearBars()
	{
		foreach (GraphedBar graphedBar in this.bars)
		{
			if (graphedBar != null && graphedBar.gameObject != null)
			{
				global::UnityEngine.Object.DestroyImmediate(graphedBar.gameObject);
			}
		}
		this.bars.Clear();
	}

	public GameObject bar_container;

	public GameObject prefab_bar;

	public GraphedBarFormatting[] bar_formats;

	private List<GraphedBar> bars = new List<GraphedBar>();
}
