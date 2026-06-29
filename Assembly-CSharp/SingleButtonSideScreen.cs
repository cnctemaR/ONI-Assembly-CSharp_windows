using System;
using UnityEngine;

public class SingleButtonSideScreen : SideScreenContent
{
	protected override void OnPrefabInit()
	{
		this.button.onClick += this.OnButonClick;
	}

	private void OnButonClick()
	{
		if (this.target != null)
		{
			this.target.OnSidescreenButtonPressed();
			this.Refresh();
		}
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<ISidescreenButtonControl>() != null;
	}

	public override void SetTarget(GameObject new_target)
	{
		if (new_target == null)
		{
			Output.LogError(new object[] { "Invalid gameObject received" });
			return;
		}
		this.target = new_target.GetComponent<ISidescreenButtonControl>();
		if (this.target == null)
		{
			Output.LogError(new object[]
			{
				"The gameObject received does not contain a",
				typeof(ISidescreenButtonControl).ToString()
			});
			return;
		}
		this.Refresh();
	}

	private void Refresh()
	{
		this.titleKey = this.target.SidescreenTitleKey;
		this.statusText.text = this.target.SidescreenStatusMessage;
		this.buttonLabel.text = this.target.SidescreenButtonText;
	}

	private ISidescreenButtonControl target;

	public LocText statusText;

	public KButton button;

	public LocText buttonLabel;
}
