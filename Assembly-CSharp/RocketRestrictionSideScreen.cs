using System;
using UnityEngine;

public class RocketRestrictionSideScreen : SideScreenContent
{
	protected override void OnSpawn()
	{
		this.unrestrictedButton.onClick += this.ClickNone;
		this.spaceRestrictedButton.onClick += this.ClickSpace;
	}

	public override int GetSideScreenSortOrder()
	{
		return 0;
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetSMI<RocketControlStation.StatesInstance>() != null;
	}

	public override void SetTarget(GameObject new_target)
	{
		if (this.controlStation != null || this.controlStationLogicSubHandle != -1)
		{
			this.ClearTarget();
		}
		this.controlStation = new_target.GetComponent<RocketControlStation>();
		this.controlStationLogicSubHandle = this.controlStation.Subscribe(1861523068, new Action<object>(this.UpdateButtonStates));
		this.UpdateButtonStates(null);
	}

	public override void ClearTarget()
	{
		if (this.controlStationLogicSubHandle != -1 && this.controlStation != null)
		{
			this.controlStation.Unsubscribe(this.controlStationLogicSubHandle);
			this.controlStationLogicSubHandle = -1;
		}
		this.controlStation = null;
	}

	private void UpdateButtonStates(object data = null)
	{
		bool flag = this.controlStation.IsLogicInputConnected();
		if (!flag)
		{
			this.unrestrictedButton.isOn = !this.controlStation.RestrictWhenGrounded;
			this.spaceRestrictedButton.isOn = this.controlStation.RestrictWhenGrounded;
		}
		this.unrestrictedButton.gameObject.SetActive(!flag);
		this.spaceRestrictedButton.gameObject.SetActive(!flag);
		this.automationControlled.gameObject.SetActive(flag);
	}

	private void ClickNone()
	{
		this.controlStation.RestrictWhenGrounded = false;
		this.UpdateButtonStates(null);
	}

	private void ClickSpace()
	{
		this.controlStation.RestrictWhenGrounded = true;
		this.UpdateButtonStates(null);
	}

	private RocketControlStation controlStation;

	[Header("Buttons")]
	public KToggle unrestrictedButton;

	public KToggle spaceRestrictedButton;

	public GameObject automationControlled;

	private int controlStationLogicSubHandle = -1;
}
