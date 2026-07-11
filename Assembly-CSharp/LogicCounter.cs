using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class LogicCounter : Switch, ISaveLoadable, ISim200ms
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe<LogicCounter>(-905833192, LogicCounter.OnCopySettingsDelegate);
	}

	private void OnCopySettings(object data)
	{
		LogicCounter component = ((GameObject)data).GetComponent<LogicCounter>();
		if (component != null)
		{
			this.maxCount = component.maxCount;
			this.resetCountAtMax = component.resetCountAtMax;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.OnToggle += this.OnSwitchToggled;
		this.increment = true;
		base.Subscribe<LogicCounter>(-801688580, LogicCounter.OnLogicValueChangedDelegate);
		this.UpdateLogicCircuit();
		this.UpdateVisualState(true);
		this.wasOn = this.switchedOn;
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		this.meter = new MeterController(component, "meter_target", component.FlipY ? "meter_dn" : "meter_up", Meter.Offset.UserSpecified, Grid.SceneLayer.LogicGatesFront, Vector3.zero, null);
		this.UpdateMeter();
	}

	private void OnSwitchToggled(bool toggled_on)
	{
		this.UpdateLogicCircuit();
		this.UpdateVisualState(false);
	}

	public void UpdateLogicCircuit()
	{
		base.GetComponent<LogicPorts>().SendSignal(LogicCounter.OUTPUT_PORT_ID, this.switchedOn ? 1 : 0);
	}

	public void UpdateMeter()
	{
		this.meter.SetPositionPercent((float)this.currentCount / 10f);
	}

	public void UpdateVisualState(bool force = false)
	{
		if (this.wasOn != this.switchedOn || force)
		{
			int num = (this.switchedOn ? 4 : 0) + (this.wasResetting ? 2 : 0) + (this.wasIncrementing ? 1 : 0);
			this.wasOn = this.switchedOn;
			base.GetComponent<KBatchedAnimController>().Play("on_" + num.ToString(), KAnim.PlayMode.Once, 1f, 0f);
		}
	}

	public void OnLogicValueChanged(object data)
	{
		LogicValueChanged logicValueChanged = (LogicValueChanged)data;
		if (logicValueChanged.portID == LogicCounter.INPUT_PORT_ID)
		{
			int newValue = logicValueChanged.newValue;
			if (LogicCircuitNetwork.IsBitActive(0, newValue))
			{
				if (!this.wasIncrementing)
				{
					this.wasIncrementing = true;
					if (!this.wasResetting)
					{
						if (this.currentCount == this.maxCount)
						{
							this.currentCount = 0;
						}
						if (this.increment)
						{
							this.currentCount++;
							if (this.currentCount == 10)
							{
								this.currentCount = 0;
							}
						}
						else
						{
							this.currentCount--;
							if (this.currentCount == -1)
							{
								this.currentCount = 9;
							}
						}
						this.UpdateMeter();
						this.SetCounterState();
						if (this.currentCount == this.maxCount && this.resetCountAtMax)
						{
							this.resetRequested = true;
						}
					}
				}
			}
			else
			{
				this.wasIncrementing = false;
			}
		}
		else
		{
			if (!(logicValueChanged.portID == LogicCounter.RESET_PORT_ID))
			{
				return;
			}
			int newValue2 = logicValueChanged.newValue;
			if (LogicCircuitNetwork.IsBitActive(0, newValue2))
			{
				if (!this.wasResetting)
				{
					this.ResetCounter();
					this.wasResetting = true;
				}
			}
			else
			{
				this.wasResetting = false;
			}
		}
		this.UpdateVisualState(true);
		this.UpdateLogicCircuit();
	}

	protected override void UpdateSwitchStatus()
	{
		StatusItem statusItem = (this.switchedOn ? Db.Get().BuildingStatusItems.LogicSensorStatusActive : Db.Get().BuildingStatusItems.LogicSensorStatusInactive);
		base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, statusItem, null);
	}

	public void ResetCounter()
	{
		this.resetRequested = false;
		this.currentCount = 0;
		this.SetCounterState();
		this.UpdateVisualState(true);
		this.UpdateMeter();
		this.UpdateLogicCircuit();
	}

	public void Sim200ms(float dt)
	{
		if (this.resetRequested)
		{
			this.ResetCounter();
		}
	}

	public void SetCounterState()
	{
		this.SetState(this.currentCount == this.maxCount);
	}

	[SerializeField]
	[Serialize]
	public int maxCount;

	[SerializeField]
	[Serialize]
	public int currentCount;

	[SerializeField]
	[Serialize]
	public bool resetCountAtMax;

	[SerializeField]
	[Serialize]
	public bool increment = true;

	private bool wasOn;

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private static readonly EventSystem.IntraObjectHandler<LogicCounter> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<LogicCounter>(delegate(LogicCounter component, object data)
	{
		component.OnCopySettings(data);
	});

	private static readonly EventSystem.IntraObjectHandler<LogicCounter> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<LogicCounter>(delegate(LogicCounter component, object data)
	{
		component.OnLogicValueChanged(data);
	});

	public static readonly HashedString INPUT_PORT_ID = new HashedString("LogicCounterInput");

	public static readonly HashedString RESET_PORT_ID = new HashedString("LogicCounterReset");

	public static readonly HashedString OUTPUT_PORT_ID = new HashedString("LogicCounterOutput");

	private bool resetRequested;

	[Serialize]
	private bool wasResetting;

	[Serialize]
	private bool wasIncrementing;

	private MeterController meter;
}
