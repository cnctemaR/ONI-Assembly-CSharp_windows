using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class HighEnergyParticleDirectionSideScreen : SideScreenContent
{
	public override string GetTitle()
	{
		return UI.UISIDESCREENS.HIGHENERGYPARTICLEDIRECTIONSIDESCREEN.TITLE;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		for (int i = 0; i < this.Buttons.Count; i++)
		{
			KButton button = this.Buttons[i];
			button.onClick += delegate
			{
				int num = this.Buttons.IndexOf(button);
				if (this.activeButton != null)
				{
					this.activeButton.isInteractable = true;
				}
				button.isInteractable = false;
				this.activeButton = button;
				if (this.target != null)
				{
					this.target.Direction = EightDirectionUtil.AngleToDirection(num * 45);
					Game.Instance.ForceOverlayUpdate(true);
					this.Refresh();
				}
			};
		}
	}

	public override int GetSideScreenSortOrder()
	{
		return 10;
	}

	public override bool IsValidForTarget(GameObject target)
	{
		HighEnergyParticleRedirector component = target.GetComponent<HighEnergyParticleRedirector>();
		bool flag = component != null;
		if (flag)
		{
			flag = flag && component.directionControllable;
		}
		bool flag2 = target.GetComponent<HighEnergyParticleSpawner>() != null || target.GetComponent<ManualHighEnergyParticleSpawner>() != null;
		return (flag || flag2) && target.GetComponent<IHighEnergyParticleDirection>() != null;
	}

	public override void SetTarget(GameObject new_target)
	{
		if (new_target == null)
		{
			global::Debug.LogError("Invalid gameObject received");
			return;
		}
		this.target = new_target.GetComponent<IHighEnergyParticleDirection>();
		if (this.target == null)
		{
			global::Debug.LogError("The gameObject received does not contain IHighEnergyParticleDirection component");
			return;
		}
		this.Refresh();
	}

	private void Refresh()
	{
		int directionIndex = EightDirectionUtil.GetDirectionIndex(this.target.Direction);
		if (directionIndex >= 0 && directionIndex < this.Buttons.Count)
		{
			this.Buttons[directionIndex].SignalClick(KKeyCode.Mouse0);
		}
		else
		{
			if (this.activeButton)
			{
				this.activeButton.isInteractable = true;
			}
			this.activeButton = null;
		}
		this.directionLabel.SetText(string.Format(UI.UISIDESCREENS.HIGHENERGYPARTICLEDIRECTIONSIDESCREEN.SELECTED_DIRECTION, this.directionStrings[directionIndex]));
	}

	private IHighEnergyParticleDirection target;

	public List<KButton> Buttons;

	private KButton activeButton;

	public LocText directionLabel;

	private string[] directionStrings = new string[]
	{
		UI.UISIDESCREENS.HIGHENERGYPARTICLEDIRECTIONSIDESCREEN.DIRECTION_N,
		UI.UISIDESCREENS.HIGHENERGYPARTICLEDIRECTIONSIDESCREEN.DIRECTION_NW,
		UI.UISIDESCREENS.HIGHENERGYPARTICLEDIRECTIONSIDESCREEN.DIRECTION_W,
		UI.UISIDESCREENS.HIGHENERGYPARTICLEDIRECTIONSIDESCREEN.DIRECTION_SW,
		UI.UISIDESCREENS.HIGHENERGYPARTICLEDIRECTIONSIDESCREEN.DIRECTION_S,
		UI.UISIDESCREENS.HIGHENERGYPARTICLEDIRECTIONSIDESCREEN.DIRECTION_SE,
		UI.UISIDESCREENS.HIGHENERGYPARTICLEDIRECTIONSIDESCREEN.DIRECTION_E,
		UI.UISIDESCREENS.HIGHENERGYPARTICLEDIRECTIONSIDESCREEN.DIRECTION_NE
	};
}
