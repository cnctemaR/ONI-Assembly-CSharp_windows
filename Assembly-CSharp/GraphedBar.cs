using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class GraphedBar : KMonoBehaviour
{
	public void SetFormat(GraphedBarFormatting format)
	{
		this.format = format;
	}

	public void SetValues(int[] values, float x_position)
	{
		this.ClearValues();
		base.gameObject.rectTransform().anchorMin = new Vector2(x_position, 0f);
		base.gameObject.rectTransform().anchorMax = new Vector2(x_position, 1f);
		base.gameObject.rectTransform().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (float)this.format.width);
		for (int i = 0; i < values.Length; i++)
		{
			GameObject gameObject = Util.KInstantiateUI(this.prefab_segment, this.segments_container, true);
			LayoutElement component = gameObject.GetComponent<LayoutElement>();
			component.preferredHeight = (float)values[i];
			component.minWidth = (float)this.format.width;
			Image component2 = gameObject.GetComponent<Image>();
			component2.color = this.format.colors[i % this.format.colors.Length];
			this.segments.Add(gameObject);
		}
	}

	public void ClearValues()
	{
		foreach (GameObject gameObject in this.segments)
		{
			global::UnityEngine.Object.DestroyImmediate(gameObject);
		}
		this.segments.Clear();
	}

	public GameObject segments_container;

	public GameObject prefab_segment;

	private List<GameObject> segments = new List<GameObject>();

	private GraphedBarFormatting format;
}
