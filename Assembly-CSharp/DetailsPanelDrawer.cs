using System;
using System.Collections.Generic;
using UnityEngine;

public class DetailsPanelDrawer
{
	public DetailsPanelDrawer(GameObject label_prefab, GameObject parent)
	{
		this.parent = parent;
		this.labelPrefab = label_prefab;
		this.stringformatter = new UIStringFormatter();
		this.floatFormatter = new UIFloatFormatter();
	}

	public DetailsPanelDrawer NewLabel(string text)
	{
		DetailsPanelDrawer.Label label = default(DetailsPanelDrawer.Label);
		if (this.activeLabelCount >= this.labels.Count)
		{
			label.text = Util.KInstantiate(this.labelPrefab, this.parent, null).GetComponent<LocText>();
			label.tooltip = label.text.GetComponent<ToolTip>();
			label.text.transform.localScale = new Vector3(1f, 1f, 1f);
			this.labels.Add(label);
		}
		else
		{
			label = this.labels[this.activeLabelCount];
		}
		this.activeLabelCount++;
		label.text.text = text;
		label.tooltip.toolTip = "";
		label.tooltip.OnToolTip = null;
		label.text.gameObject.SetActive(true);
		return this;
	}

	public DetailsPanelDrawer Tooltip(string tooltip_text)
	{
		this.labels[this.activeLabelCount - 1].tooltip.toolTip = tooltip_text;
		return this;
	}

	public DetailsPanelDrawer Tooltip(Func<string> tooltip_cb)
	{
		this.labels[this.activeLabelCount - 1].tooltip.OnToolTip = tooltip_cb;
		return this;
	}

	public string Format(string format, float value)
	{
		return this.floatFormatter.Format(format, value);
	}

	public string Format(string format, string s0)
	{
		return this.stringformatter.Format(format, s0);
	}

	public string Format(string format, string s0, string s1)
	{
		return this.stringformatter.Format(format, s0, s1);
	}

	public DetailsPanelDrawer BeginDrawing()
	{
		this.activeLabelCount = 0;
		this.stringformatter.BeginDrawing();
		this.floatFormatter.BeginDrawing();
		return this;
	}

	public DetailsPanelDrawer EndDrawing()
	{
		this.floatFormatter.EndDrawing();
		this.stringformatter.EndDrawing();
		for (int i = this.activeLabelCount; i < this.labels.Count; i++)
		{
			if (this.labels[i].text.gameObject.activeSelf)
			{
				this.labels[i].text.gameObject.SetActive(false);
			}
		}
		return this;
	}

	private List<DetailsPanelDrawer.Label> labels = new List<DetailsPanelDrawer.Label>();

	private int activeLabelCount;

	private UIStringFormatter stringformatter;

	private UIFloatFormatter floatFormatter;

	private GameObject parent;

	private GameObject labelPrefab;

	private struct Label
	{
		public LocText text;

		public ToolTip tooltip;
	}
}
