using System;
using UnityEngine;

public class ProgressBarSideScreen : SideScreenContent, IRender1000ms
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	public override int GetSideScreenSortOrder()
	{
		return -10;
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<IProgressBarSideScreen>() != null;
	}

	public override void SetTarget(GameObject target)
	{
		base.SetTarget(target);
		this.targetObject = target.GetComponent<IProgressBarSideScreen>();
		this.RefreshBar();
	}

	private void RefreshBar()
	{
		this.progressBar.SetMaxValue(this.targetObject.GetProgressBarMaxValue());
		this.progressBar.SetFillPercentage(this.targetObject.GetProgressBarFillPercentage());
		this.progressBar.label.SetText(this.targetObject.GetProgressBarLabel());
		this.label.SetText(this.targetObject.GetProgressBarTitleLabel());
		this.progressBar.GetComponentInChildren<ToolTip>().SetSimpleTooltip(this.targetObject.GetProgressBarTooltip());
	}

	public void Render1000ms(float dt)
	{
		this.RefreshBar();
	}

	public LocText label;

	public GenericUIProgressBar progressBar;

	public IProgressBarSideScreen targetObject;
}
