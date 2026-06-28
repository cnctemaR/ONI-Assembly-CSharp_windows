using System;
using System.Collections.Generic;
using UnityEngine;

public class ToolHoverFields
{
	public ToolHoverFields(List<GameObject> MultiLabelDisplays, List<ShadowBar> ShadowBars)
	{
		this.MultiLabelDisplays = new List<GameObject>();
		for (int i = 0; i < MultiLabelDisplays.Count; i++)
		{
			this.MultiLabelDisplays.Add(MultiLabelDisplays[i]);
		}
		this.ShadowBars = new List<ShadowBar>();
		for (int j = 0; j < ShadowBars.Count; j++)
		{
			this.ShadowBars.Add(ShadowBars[j]);
		}
	}

	public List<GameObject> MultiLabelDisplays = new List<GameObject>();

	public List<ShadowBar> ShadowBars = new List<ShadowBar>();
}
