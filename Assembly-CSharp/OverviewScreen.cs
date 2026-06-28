using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class OverviewScreen : KTabMenu
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.ScreenInstantiator.Instantiate();
		KScreen[] componentsInChildren = this.ScreenInstantiator.GetComponentsInChildren<KScreen>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			this.TabScreens.Add(componentsInChildren[i]);
		}
		foreach (KScreen kscreen in this.TabScreens)
		{
			base.AddTab(kscreen.displayName, kscreen);
			kscreen.gameObject.SetActive(false);
		}
	}

	public override void ActivateTab(int tabIdx)
	{
		if (tabIdx != 0)
		{
			if (tabIdx == 1)
			{
				this.titleBar.SetTitle(UI.VITALS);
			}
		}
		else
		{
			this.titleBar.SetTitle(UI.JOBS);
		}
		base.ActivateTab(tabIdx);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	protected override void OnActivate()
	{
		base.OnActivate();
	}

	protected override void OnDeactivate()
	{
		foreach (KScreen kscreen in this.TabScreens)
		{
			kscreen.Deactivate();
			global::UnityEngine.Object.Destroy(kscreen);
		}
	}

	private List<KScreen> TabScreens = new List<KScreen>();

	public InstantiateUIPrefabChild ScreenInstantiator;

	public TitleBar titleBar;
}
