using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class GraphBase : KMonoBehaviour
{
	public Vector2 GetRelativePosition(Vector2 absolute_point)
	{
		Vector2 zero = Vector2.zero;
		float num = this.axis_x.max_value - this.axis_x.min_value;
		float num2 = absolute_point.x - this.axis_x.min_value;
		zero.x = num2 / num;
		float num3 = this.axis_y.max_value - this.axis_y.min_value;
		float num4 = absolute_point.y - this.axis_y.min_value;
		zero.y = num4 / num3;
		return zero;
	}

	public Vector2 GetRelativeSize(Vector2 absolute_size)
	{
		return this.GetRelativePosition(absolute_size);
	}

	public void ClearGuides()
	{
		foreach (GameObject gameObject in this.guides)
		{
			if (gameObject != null)
			{
				global::UnityEngine.Object.DestroyImmediate(gameObject);
			}
		}
		this.guides.Clear();
	}

	public void RefreshGuides()
	{
		this.ClearGuides();
		int num = 2;
		GameObject gameObject = Util.KInstantiateUI(this.prefab_guide_y, this.guides_y, true);
		gameObject.name = "guides_vertical";
		Vector2[] array = new Vector2[num * (int)(this.axis_x.range / (float)this.axis_x.guide_frequency)];
		for (int i = 0; i < array.Length; i += num)
		{
			array[i] = this.GetRelativePosition(new Vector2((float)(i * (this.axis_x.guide_frequency / num)), this.axis_y.min_value));
			array[i + 1] = this.GetRelativePosition(new Vector2((float)(i * (this.axis_x.guide_frequency / num)), this.axis_y.max_value));
		}
		gameObject.GetComponent<UILineRenderer>().Points = array;
		this.guides.Add(gameObject);
		GameObject gameObject2 = Util.KInstantiateUI(this.prefab_guide_x, this.guides_x, true);
		gameObject2.name = "guides_horizontal";
		Vector2[] array2 = new Vector2[num * (int)(this.axis_y.range / (float)this.axis_y.guide_frequency)];
		for (int j = 0; j < array2.Length; j += num)
		{
			array2[j] = this.GetRelativePosition(new Vector2(this.axis_x.min_value, (float)(j * (this.axis_y.guide_frequency / num))));
			array2[j + 1] = this.GetRelativePosition(new Vector2(this.axis_x.max_value, (float)(j * (this.axis_y.guide_frequency / num))));
		}
		gameObject2.GetComponent<UILineRenderer>().Points = array2;
		this.guides.Add(gameObject2);
	}

	[Header("Axis")]
	public GraphAxis axis_x;

	public GraphAxis axis_y;

	[Header("References")]
	public GameObject prefab_guide_x;

	public GameObject prefab_guide_y;

	public GameObject guides_x;

	public GameObject guides_y;

	protected List<GameObject> guides = new List<GameObject>();
}
