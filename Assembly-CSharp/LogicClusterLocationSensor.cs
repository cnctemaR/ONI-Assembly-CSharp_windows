using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class LogicClusterLocationSensor : Switch, ISaveLoadable, ISim200ms
{
	public bool ActiveInSpace
	{
		get
		{
			return this.activeInSpace;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe<LogicClusterLocationSensor>(-905833192, LogicClusterLocationSensor.OnCopySettingsDelegate);
	}

	private void OnCopySettings(object data)
	{
		LogicClusterLocationSensor component = ((GameObject)data).GetComponent<LogicClusterLocationSensor>();
		if (component != null)
		{
			this.activeLocations.Clear();
			for (int i = 0; i < component.activeLocations.Count; i++)
			{
				this.SetLocationEnabled(component.activeLocations[i], true);
			}
			this.activeInSpace = component.activeInSpace;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.OnToggle += this.OnSwitchToggled;
		this.UpdateLogicCircuit();
		this.UpdateVisualState(true);
		this.wasOn = this.switchedOn;
	}

	public void SetLocationEnabled(AxialI location, bool setting)
	{
		if (!setting)
		{
			this.activeLocations.Remove(location);
			return;
		}
		if (!this.activeLocations.Contains(location))
		{
			this.activeLocations.Add(location);
		}
	}

	public void SetSpaceEnabled(bool setting)
	{
		this.activeInSpace = setting;
	}

	public void Sim200ms(float dt)
	{
		bool flag = this.CheckCurrentLocationSelected();
		this.SetState(flag);
	}

	private bool CheckCurrentLocationSelected()
	{
		AxialI myWorldLocation = base.gameObject.GetMyWorldLocation();
		return this.activeLocations.Contains(myWorldLocation) || (this.activeInSpace && this.CheckInEmptySpace());
	}

	private bool CheckInEmptySpace()
	{
		bool flag = true;
		AxialI myWorldLocation = base.gameObject.GetMyWorldLocation();
		foreach (WorldContainer worldContainer in ClusterManager.Instance.WorldContainers)
		{
			if (!worldContainer.IsModuleInterior && worldContainer.GetMyWorldLocation() == myWorldLocation)
			{
				flag = false;
				break;
			}
		}
		return flag;
	}

	public bool CheckLocationSelected(AxialI location)
	{
		return this.activeLocations.Contains(location);
	}

	private void OnSwitchToggled(bool toggled_on)
	{
		this.UpdateLogicCircuit();
		this.UpdateVisualState(false);
	}

	private void UpdateLogicCircuit()
	{
		base.GetComponent<LogicPorts>().SendSignal(LogicSwitch.PORT_ID, this.switchedOn ? 1 : 0);
	}

	private void UpdateVisualState(bool force = false)
	{
		if (this.wasOn != this.switchedOn || force)
		{
			this.wasOn = this.switchedOn;
			KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
			AxialI myWorldLocation = base.gameObject.GetMyWorldLocation();
			bool flag = true;
			foreach (WorldContainer worldContainer in ClusterManager.Instance.WorldContainers)
			{
				if (!worldContainer.IsModuleInterior && worldContainer.GetMyWorldLocation() == myWorldLocation)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				component.Play(this.switchedOn ? "on_space_pre" : "on_space_pst", KAnim.PlayMode.Once, 1f, 0f);
				component.Queue(this.switchedOn ? "on_space" : "off_space", KAnim.PlayMode.Once, 1f, 0f);
				return;
			}
			component.Play(this.switchedOn ? "on_asteroid_pre" : "on_asteroid_pst", KAnim.PlayMode.Once, 1f, 0f);
			component.Queue(this.switchedOn ? "on_asteroid" : "off_asteroid", KAnim.PlayMode.Once, 1f, 0f);
		}
	}

	protected override void UpdateSwitchStatus()
	{
		StatusItem statusItem = (this.switchedOn ? Db.Get().BuildingStatusItems.LogicSensorStatusActive : Db.Get().BuildingStatusItems.LogicSensorStatusInactive);
		base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, statusItem, null);
	}

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	[Serialize]
	private List<AxialI> activeLocations = new List<AxialI>();

	[Serialize]
	private bool activeInSpace = true;

	private bool wasOn;

	private static readonly EventSystem.IntraObjectHandler<LogicClusterLocationSensor> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<LogicClusterLocationSensor>(delegate(LogicClusterLocationSensor component, object data)
	{
		component.OnCopySettings(data);
	});
}
