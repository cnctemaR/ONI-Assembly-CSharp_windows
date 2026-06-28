using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class ResearchSideScreen : SideScreenContent
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.selectResearchButton.onClick += delegate
		{
			ManagementMenu.Instance.ToggleResearch();
		};
		Research.Instance.Subscribe(-1914338957, new Action<object>(this.RefreshDisplayState));
		Research.Instance.Subscribe(-125623018, new Action<object>(this.RefreshDisplayState));
		this.RefreshDisplayState(null);
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		this.RefreshDisplayState(null);
		this.target = SelectTool.Instance.selected.GetComponent<KMonoBehaviour>().gameObject;
		this.target.gameObject.Subscribe(-1852328367, new Action<object>(this.RefreshDisplayState));
		this.target.gameObject.Subscribe(-592767678, new Action<object>(this.RefreshDisplayState));
	}

	protected override void OnCmpDisable()
	{
		base.OnCmpDisable();
		if (this.target)
		{
			this.target.gameObject.Unsubscribe(-1852328367, new Action<object>(this.RefreshDisplayState));
			this.target.gameObject.Unsubscribe(187661686, new Action<object>(this.RefreshDisplayState));
			this.target = null;
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Research.Instance.Unsubscribe(-1914338957, new Action<object>(this.RefreshDisplayState));
		Research.Instance.Unsubscribe(-125623018, new Action<object>(this.RefreshDisplayState));
		if (this.target)
		{
			this.target.gameObject.Unsubscribe(-1852328367, new Action<object>(this.RefreshDisplayState));
			this.target.gameObject.Unsubscribe(187661686, new Action<object>(this.RefreshDisplayState));
			this.target = null;
		}
	}

	private void RefreshDisplayState(object data = null)
	{
		if (SelectTool.Instance.selected == null)
		{
			return;
		}
		ResearchCenter component = SelectTool.Instance.selected.GetComponent<ResearchCenter>();
		if (component == null)
		{
			return;
		}
		this.researchButtonIcon.sprite = Research.Instance.researchTypes.GetResearchType(component.research_point_type_id).sprite;
		TechInstance activeResearch = Research.Instance.GetActiveResearch();
		if (activeResearch == null)
		{
			this.DescriptionText.text = "<b>" + UI.UISIDESCREENS.RESEARCHSIDESCREEN.NOSELECTEDRESEARCH + "</b>";
		}
		else
		{
			string text = string.Empty;
			if (!activeResearch.tech.costsByResearchTypeID.ContainsKey(component.research_point_type_id) || activeResearch.tech.costsByResearchTypeID[component.research_point_type_id] <= 0f)
			{
				text += "<color=#7f7f7f>";
			}
			text = text + "<b>" + activeResearch.tech.Name + "</b>";
			if (!activeResearch.tech.costsByResearchTypeID.ContainsKey(component.research_point_type_id) || activeResearch.tech.costsByResearchTypeID[component.research_point_type_id] <= 0f)
			{
				text += "</color>";
			}
			foreach (KeyValuePair<string, float> keyValuePair in activeResearch.tech.costsByResearchTypeID)
			{
				if (keyValuePair.Value != 0f)
				{
					bool flag = keyValuePair.Key == component.research_point_type_id;
					text += "\n   ";
					text += "<b>";
					if (!flag)
					{
						text += "<color=#7f7f7f>";
					}
					string text2 = text;
					text = string.Concat(new object[]
					{
						text2,
						"- ",
						Research.Instance.researchTypes.GetResearchType(keyValuePair.Key).name,
						": ",
						activeResearch.progressInventory.PointsByTypeID[keyValuePair.Key],
						"/",
						activeResearch.tech.costsByResearchTypeID[keyValuePair.Key]
					});
					if (!flag)
					{
						text += "</color>";
					}
					text += "</b>";
				}
			}
			this.DescriptionText.text = text;
		}
	}

	public KButton selectResearchButton;

	public Image researchButtonIcon;

	public GameObject content;

	private GameObject target;

	public LocText DescriptionText;
}
