using System;
using UnityEngine;
using UnityEngine.UI.Extensions;

[Serializable]
public class GraphedLine : KMonoBehaviour
{
	public void SetPoints(Vector2[] points)
	{
		this.points = points;
		this.UpdatePoints();
	}

	private void UpdatePoints()
	{
		Vector2[] array = new Vector2[this.points.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = this.layer.graph.GetRelativePosition(this.points[i]);
		}
		this.line_renderer.Points = array;
	}

	public UILineRenderer line_renderer;

	public LineLayer layer;

	private Vector2[] points;
}
