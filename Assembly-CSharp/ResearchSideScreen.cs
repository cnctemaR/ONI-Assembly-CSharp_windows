using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class ResearchSideScreen : SideScreenContent
{
	public ResearchSideScreen()
	{
		this.refreshDisplayStateDelegate = new Action<object>(this.RefreshDisplayState);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.selectResearchButton.onClick += delegate
		{
			ManagementMenu.Instance.ToggleResearch();
		};
		Research.Instance.Subscribe(-1914338957, this.refreshDisplayStateDelegate);
		Research.Instance.Subscribe(-125623018, this.refreshDisplayStateDelegate);
		this.RefreshDisplayState(null);
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		this.RefreshDisplayState(null);
		this.target = SelectTool.Instance.selected.GetComponent<KMonoBehaviour>().gameObject;
		this.target.gameObject.Subscribe(-1852328367, this.refreshDisplayStateDelegate);
		this.target.gameObject.Subscribe(-592767678, this.refreshDisplayStateDelegate);
	}

	protected override void OnCmpDisable()
	{
		base.OnCmpDisable();
		if (this.target)
		{
			this.target.gameObject.Unsubscribe(-1852328367, this.refreshDisplayStateDelegate);
			this.target.gameObject.Unsubscribe(187661686, this.refreshDisplayStateDelegate);
			this.target = null;
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Research.Instance.Unsubscribe(-1914338957, this.refreshDisplayStateDelegate);
		Research.Instance.Unsubscribe(-125623018, this.refreshDisplayStateDelegate);
		if (this.target)
		{
			this.target.gameObject.Unsubscribe(-1852328367, this.refreshDisplayStateDelegate);
			this.target.gameObject.Unsubscribe(187661686, this.refreshDisplayStateDelegate);
			this.target = null;
		}
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<ResearchCenter>() != null;
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
			return;
		}
		string text = "";
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
				text = string.Concat(new object[]
				{
					text,
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

	public KButton selectResearchButton;

	public Image researchButtonIcon;

	public GameObject content;

	private GameObject target;

	private Action<object> refreshDisplayStateDelegate;

	public LocText DescriptionText;
}
