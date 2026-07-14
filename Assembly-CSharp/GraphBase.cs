using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI.Extensions;

[AddComponentMenu("KMonoBehaviour/scripts/GraphBase")]
public class GraphBase : KMonoBehaviour
{
	public Vector2 GetRelativePosition(Vector2 absolute_point)
	{
		Vector2 zero = Vector2.zero;
		float num = Mathf.Max(1f, this.axis_x.max_value - this.axis_x.min_value);
		float num2 = absolute_point.x - this.axis_x.min_value;
		zero.x = num2 / num;
		float num3 = Mathf.Max(1f, this.axis_y.max_value - this.axis_y.min_value);
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
		this.ClearVerticalGuides();
		this.ClearHorizontalGuides();
	}

	public void ClearHorizontalGuides()
	{
		foreach (GameObject gameObject in this.horizontalGuides)
		{
			if (gameObject != null)
			{
				global::UnityEngine.Object.DestroyImmediate(gameObject);
			}
		}
		this.horizontalGuides.Clear();
	}

	public void ClearVerticalGuides()
	{
		foreach (GameObject gameObject in this.verticalGuides)
		{
			if (gameObject != null)
			{
				global::UnityEngine.Object.DestroyImmediate(gameObject);
			}
		}
		this.verticalGuides.Clear();
	}

	public void RefreshGuides()
	{
		this.ClearGuides();
		this.RefreshHorizontalGuides();
		this.RefreshVerticalGuides();
	}

	public void RefreshHorizontalGuides()
	{
		if (this.prefab_guide_x != null)
		{
			GameObject gameObject = Util.KInstantiateUI(this.prefab_guide_x, this.guides_x, true);
			gameObject.name = "guides_horizontal";
			int num = ((this.axis_y.guide_frequency > 0f) ? ((int)(this.axis_y.range / this.axis_y.guide_frequency)) : 0);
			Vector2[] array = new Vector2[2 * num];
			for (int i = 0; i < array.Length; i += 2)
			{
				Vector2 vector = new Vector2(this.axis_x.min_value, (float)i * (this.axis_y.guide_frequency / 2f));
				array[i] = this.GetRelativePosition(vector);
				Vector2 vector2 = new Vector2(this.axis_x.max_value, (float)i * (this.axis_y.guide_frequency / 2f));
				array[i + 1] = this.GetRelativePosition(vector2);
				if (this.prefab_guide_horizontal_label != null)
				{
					GameObject gameObject2 = Util.KInstantiateUI(this.prefab_guide_horizontal_label, gameObject, true);
					gameObject2.GetComponent<LocText>().alignment = TextAlignmentOptions.MidlineLeft;
					gameObject2.GetComponent<LocText>().text = ((int)this.axis_y.guide_frequency * (i / 2)).ToString();
					gameObject2.rectTransform().SetLocalPosition(new Vector2(8f, (float)i * (base.gameObject.rectTransform().rect.height / (float)array.Length)) - base.gameObject.rectTransform().rect.size / 2f);
				}
			}
			gameObject.GetComponent<UILineRenderer>().Points = array;
			this.horizontalGuides.Add(gameObject);
		}
	}

	public void RefreshVerticalGuides()
	{
		if (this.prefab_guide_y != null)
		{
			GameObject gameObject = Util.KInstantiateUI(this.prefab_guide_y, this.guides_y, true);
			gameObject.name = "guides_vertical";
			int num = ((this.axis_x.guide_frequency > 0f) ? ((int)(this.axis_x.range / this.axis_x.guide_frequency)) : 0);
			Vector2[] array = new Vector2[2 * num];
			for (int i = 0; i < array.Length; i += 2)
			{
				Vector2 vector = new Vector2((float)i * (this.axis_x.guide_frequency / 2f), this.axis_y.min_value);
				array[i] = this.GetRelativePosition(vector);
				Vector2 vector2 = new Vector2((float)i * (this.axis_x.guide_frequency / 2f), this.axis_y.max_value);
				array[i + 1] = this.GetRelativePosition(vector2);
				if (this.prefab_guide_vertical_label != null)
				{
					GameObject gameObject2 = Util.KInstantiateUI(this.prefab_guide_vertical_label, gameObject, true);
					gameObject2.GetComponent<LocText>().alignment = TextAlignmentOptions.Bottom;
					gameObject2.GetComponent<LocText>().text = ((int)this.axis_x.guide_frequency * (i / 2)).ToString();
					gameObject2.rectTransform().SetLocalPosition(new Vector2((float)i * (base.gameObject.rectTransform().rect.width / (float)array.Length), 4f) - base.gameObject.rectTransform().rect.size / 2f);
				}
			}
			gameObject.GetComponent<UILineRenderer>().Points = array;
			this.verticalGuides.Add(gameObject);
		}
	}

	[Header("Axis")]
	public GraphAxis axis_x;

	public GraphAxis axis_y;

	[Header("References")]
	public GameObject prefab_guide_x;

	public GameObject prefab_guide_y;

	public GameObject prefab_guide_horizontal_label;

	public GameObject prefab_guide_vertical_label;

	public GameObject guides_x;

	public GameObject guides_y;

	public LocText label_title;

	public LocText label_x;

	public LocText label_y;

	public string graphName;

	protected List<GameObject> horizontalGuides = new List<GameObject>();

	protected List<GameObject> verticalGuides = new List<GameObject>();

	private const int points_per_guide_line = 2;
}
