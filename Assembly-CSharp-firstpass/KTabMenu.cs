using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KTabMenu : KScreen
{
	public event KTabMenu.TabActivated onTabActivated;

	public int PreviousActiveTab
	{
		get
		{
			return this.previouslyActiveTab;
		}
	}

	public int AddTab(string tabName, KScreen contents)
	{
		int count = this.tabs.Count;
		this.header.Add(tabName, new KTabMenuHeader.OnClick(this.ActivateTab), this.tabs.Count);
		this.header.SetTabEnabled(count, true);
		this.tabs.Add(contents);
		return count;
	}

	public int AddTab(Sprite icon, string tabName, KScreen contents, string tooltip = "")
	{
		int count = this.tabs.Count;
		this.header.Add(icon, tabName, new KTabMenuHeader.OnClick(this.ActivateTab), this.tabs.Count, tooltip);
		this.header.SetTabEnabled(count, true);
		this.tabs.Add(contents);
		return count;
	}

	public virtual void ActivateTab(int tabIdx)
	{
		this.header.Activate(tabIdx, this.previouslyActiveTab);
		for (int i = 0; i < this.tabs.Count; i++)
		{
			this.tabs[i].gameObject.SetActive(i == tabIdx);
		}
		ScrollRect component = this.body.GetComponent<ScrollRect>();
		if (component != null && tabIdx < this.tabs.Count)
		{
			component.content = this.tabs[tabIdx].GetComponent<RectTransform>();
		}
		if (this.onTabActivated != null)
		{
			this.onTabActivated(tabIdx, this.previouslyActiveTab);
		}
		this.previouslyActiveTab = tabIdx;
	}

	protected override void OnDeactivate()
	{
		foreach (KScreen kscreen in this.tabs)
		{
			kscreen.Deactivate();
		}
		base.OnDeactivate();
	}

	public void SetTabEnabled(int tabIdx, bool enabled)
	{
		this.header.SetTabEnabled(tabIdx, enabled);
	}

	protected int CountTabs()
	{
		int num = 0;
		for (int i = 0; i < this.header.transform.childCount; i++)
		{
			if (this.header.transform.GetChild(i).gameObject.activeSelf)
			{
				num++;
			}
		}
		return num;
	}

	[SerializeField]
	protected KTabMenuHeader header;

	[SerializeField]
	protected RectTransform body;

	protected List<KScreen> tabs = new List<KScreen>();

	protected int previouslyActiveTab = -1;

	public delegate void TabActivated(int tabIdx, int previouslyActiveTabIdx);
}
