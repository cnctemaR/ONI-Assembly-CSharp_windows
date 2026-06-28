using System;
using System.Collections.Generic;
using UnityEngine;

public class ResearchSideScreen : FabricatorSideScreen
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.selectResearchButton.onClick += delegate
		{
			ManagementMenu.Instance.ToggleResearch();
		};
		Research.Instance.Subscribe(-1914338957, new EventSystem.EventHandler(this.RefreshDisplayState));
		this.RefreshDisplayState(null);
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		this.RefreshDisplayState(null);
		this.target = SelectTool.Instance.selected.GetComponent<KMonoBehaviour>().gameObject;
		this.target.gameObject.Subscribe(-1852328367, new EventSystem.EventHandler(this.RefreshDisplayState));
		this.target.gameObject.Subscribe(-592767678, new EventSystem.EventHandler(this.RefreshDisplayState));
	}

	protected override void OnCmpDisable()
	{
		base.OnCmpDisable();
		if (this.target)
		{
			this.target.gameObject.Unsubscribe(-1852328367, new EventSystem.EventHandler(this.RefreshDisplayState));
			this.target.gameObject.Unsubscribe(187661686, new EventSystem.EventHandler(this.RefreshDisplayState));
			this.target = null;
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Research.Instance.Unsubscribe(-1914338957, new EventSystem.EventHandler(this.RefreshDisplayState));
		if (this.target)
		{
			this.target.gameObject.Unsubscribe(-1852328367, new EventSystem.EventHandler(this.RefreshDisplayState));
			this.target.gameObject.Unsubscribe(187661686, new EventSystem.EventHandler(this.RefreshDisplayState));
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
		Operational component2 = component.GetComponent<Operational>();
		DetailsScreen.Instance.MaskSideContent(false);
		if (component2.IsOperational)
		{
			DetailsScreen.Instance.RefreshTitle();
			this.content.SetActive(true);
			this.overrideContent.SetActive(false);
		}
		else
		{
			bool flag = true;
			foreach (KeyValuePair<Operational.Flag, bool> keyValuePair in component2.Flags)
			{
				if (!keyValuePair.Value && keyValuePair.Key != ResearchCenter.ResearchSelectedFlag)
				{
					flag = false;
				}
			}
			if (flag)
			{
				this.overrideContent.SetActive(true);
			}
			else
			{
				this.overrideContent.SetActive(false);
				DetailsScreen.Instance.MaskSideContent(true);
			}
		}
	}

	public KButton selectResearchButton;

	public GameObject content;

	public GameObject overrideContent;

	private GameObject target;
}
