using System;
using System.Collections.Generic;
using UnityEngine;

public class LineLayer : GraphLayer
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	public void NewLine(Tuple<float, float>[] points, string ID = "")
	{
		Vector2[] array = new Vector2[points.Length];
		for (int i = 0; i < points.Length; i++)
		{
			array[i] = new Vector2(points[i].first, points[i].second);
		}
		this.NewLine(array, ID, 128, LineLayer.DataScalingType.DropValues);
	}

	public void NewLine(Vector2[] points, string ID = "", int compressDataToPointCount = 128, LineLayer.DataScalingType compressType = LineLayer.DataScalingType.DropValues)
	{
		GameObject gameObject = Util.KInstantiateUI(this.prefab_line, this.line_container, true);
		if (ID == string.Empty)
		{
			ID = this.lines.Count.ToString();
		}
		gameObject.name = string.Format("line_{0}", ID);
		GraphedLine component = gameObject.GetComponent<GraphedLine>();
		if (points.Length > compressDataToPointCount)
		{
			Vector2[] array = new Vector2[compressDataToPointCount];
			if (compressType == LineLayer.DataScalingType.DropValues)
			{
				float num = (float)(points.Length - compressDataToPointCount + 1);
				float num2 = (float)points.Length / num;
				int num3 = 0;
				float num4 = 0f;
				for (int i = 0; i < points.Length; i++)
				{
					num4 += 1f;
					if (num4 >= num2)
					{
						num4 -= num2;
					}
					else
					{
						array[num3] = points[i];
						num3++;
					}
				}
			}
			else
			{
				int num5 = points.Length / compressDataToPointCount;
				for (int j = 0; j < compressDataToPointCount; j++)
				{
					if (j > 0)
					{
						float num6 = 0f;
						if (compressType != LineLayer.DataScalingType.Max)
						{
							if (compressType == LineLayer.DataScalingType.Average)
							{
								for (int k = 0; k < num5; k++)
								{
									num6 += points[j * num5 - k].y;
								}
								num6 /= (float)num5;
							}
						}
						else
						{
							for (int l = 0; l < num5; l++)
							{
								num6 = Mathf.Max(num6, points[j * num5 - l].y);
							}
						}
						array[j] = new Vector2(points[j * num5].x, num6);
					}
				}
			}
			points = array;
		}
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

	private void Update()
	{
		RectTransform component = base.gameObject.GetComponent<RectTransform>();
		if (!RectTransformUtility.RectangleContainsScreenPoint(component, Input.mousePosition))
		{
			for (int i = 0; i < this.lines.Count; i++)
			{
				this.lines[i].HidePointHighlight();
			}
			return;
		}
		Vector2 vector = Vector2.zero;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(base.gameObject.GetComponent<RectTransform>(), Input.mousePosition, null, out vector);
		vector += component.sizeDelta / 2f;
		for (int j = 0; j < this.lines.Count; j++)
		{
			if (this.lines[j].PointCount != 0)
			{
				Vector2 closestDataToPointOnXAxis = this.lines[j].GetClosestDataToPointOnXAxis(vector);
				this.lines[j].SetPointHighlight(closestDataToPointOnXAxis);
			}
		}
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

	public enum DataScalingType
	{
		Average,
		Max,
		DropValues
	}
}
