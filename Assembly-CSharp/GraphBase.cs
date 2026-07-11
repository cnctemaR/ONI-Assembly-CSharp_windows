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
		Vector2[] array = new Vector2[num * (int)(this.axis_x.range / this.axis_x.guide_frequency)];
		for (int i = 0; i < array.Length; i += num)
		{
			Vector2 vector = new Vector2((float)i * (this.axis_x.guide_frequency / (float)num), this.axis_y.min_value);
			array[i] = this.GetRelativePosition(vector);
			Vector2 vector2 = new Vector2((float)i * (this.axis_x.guide_frequency / (float)num), this.axis_y.max_value);
			array[i + 1] = this.GetRelativePosition(vector2);
			GameObject gameObject2 = Util.KInstantiateUI(this.prefab_guide_vertical_label, gameObject, true);
			gameObject2.GetComponent<LocText>().alignment = TextAlignmentOptions.Bottom;
			gameObject2.GetComponent<LocText>().text = ((int)this.axis_x.guide_frequency * (i / num)).ToString();
			gameObject2.rectTransform().SetLocalPosition(new Vector2((float)i * (base.gameObject.rectTransform().rect.width / (float)array.Length), 4f) - base.gameObject.rectTransform().rect.size / 2f);
		}
		gameObject.GetComponent<UILineRenderer>().Points = array;
		this.guides.Add(gameObject);
		GameObject gameObject3 = Util.KInstantiateUI(this.prefab_guide_x, this.guides_x, true);
		gameObject3.name = "guides_horizontal";
		Vector2[] array2 = new Vector2[num * (int)(this.axis_y.range / this.axis_y.guide_frequency)];
		for (int j = 0; j < array2.Length; j += num)
		{
			Vector2 vector3 = new Vector2(this.axis_x.min_value, (float)j * (this.axis_y.guide_frequency / (float)num));
			array2[j] = this.GetRelativePosition(vector3);
			Vector2 vector4 = new Vector2(this.axis_x.max_value, (float)j * (this.axis_y.guide_frequency / (float)num));
			array2[j + 1] = this.GetRelativePosition(vector4);
			GameObject gameObject4 = Util.KInstantiateUI(this.prefab_guide_horizontal_label, gameObject3, true);
			gameObject4.GetComponent<LocText>().alignment = TextAlignmentOptions.MidlineLeft;
			gameObject4.GetComponent<LocText>().text = ((int)this.axis_y.guide_frequency * (j / num)).ToString();
			gameObject4.rectTransform().SetLocalPosition(new Vector2(8f, (float)j * (base.gameObject.rectTransform().rect.height / (float)array2.Length)) - base.gameObject.rectTransform().rect.size / 2f);
		}
		gameObject3.GetComponent<UILineRenderer>().Points = array2;
		this.guides.Add(gameObject3);
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

	protected List<GameObject> guides = new List<GameObject>();
}
