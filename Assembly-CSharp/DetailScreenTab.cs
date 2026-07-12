using System;
using UnityEngine;

public abstract class DetailScreenTab : TargetPanel
{
	public abstract override bool IsValidForTarget(GameObject target);

	protected override void OnSelectTarget(GameObject target)
	{
		base.OnSelectTarget(target);
	}

	protected CollapsibleDetailContentPanel CreateCollapsableSection(string title = null)
	{
		CollapsibleDetailContentPanel collapsibleDetailContentPanel = Util.KInstantiateUI<CollapsibleDetailContentPanel>(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		if (!string.IsNullOrEmpty(title))
		{
			collapsibleDetailContentPanel.SetTitle(title);
		}
		return collapsibleDetailContentPanel;
	}

	private void Update()
	{
		this.Refresh(false);
	}

	protected virtual void Refresh(bool force = false)
	{
	}
}
