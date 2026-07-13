using System;
using UnityEngine;

public class LoreBearerSideScreen : SideScreenContent
{
	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<LoreBearer>() != null;
	}

	public override int GetSideScreenSortOrder()
	{
		return this.target.GetSideScreenSortOrder();
	}

	public override void SetTarget(GameObject new_target)
	{
		if (new_target == null)
		{
			global::Debug.LogError("Invalid gameObject received");
			return;
		}
		this.target = new_target.GetComponent<LoreBearer>();
		this.Refresh();
	}

	private void Refresh()
	{
		this.button.isInteractable = this.target.SidescreenButtonInteractable();
		this.button.ClearOnClick();
		this.button.onClick += this.target.OnSidescreenButtonPressed;
		this.button.onClick += this.Refresh;
		this.button.GetComponentInChildren<LocText>().SetText(this.target.SidescreenButtonText);
		this.button.GetComponent<ToolTip>().SetSimpleTooltip(this.target.SidescreenButtonTooltip);
	}

	public const int DefaultButtonMenuSideScreenSortOrder = 20;

	public KButton button;

	private LoreBearer target;
}
