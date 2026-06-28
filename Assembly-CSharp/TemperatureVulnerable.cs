using System;
using UnityEngine;

public class TemperatureVulnerable : KMonoBehaviour
{
	public float InternalTemperature
	{
		get
		{
			return this.primaryElement.Temperature;
		}
	}

	public TemperatureVulnerable.TemperatureStates GetInternalState
	{
		get
		{
			return this.internalTemperatureState;
		}
	}

	public bool IsLethal
	{
		get
		{
			return this.internalTemperatureState == TemperatureVulnerable.TemperatureStates.LethalHot || this.internalTemperatureState == TemperatureVulnerable.TemperatureStates.LethalCold;
		}
	}

	public bool IsNormal
	{
		get
		{
			return this.internalTemperatureState == TemperatureVulnerable.TemperatureStates.Normal;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		float num = this.internalTemperatureWarning_Low + 0.5f * (this.internalTemperatureWarning_High - this.internalTemperatureWarning_Low);
		this.primaryElement.Temperature = num;
		this.handle = GameScheduler.Instance.SchedulePeriodic(base.name, 1f, new Action<object>(this.UpdateTemperature), null, null, 0f);
		this.UpdateTemperature(null);
	}

	protected override void OnCleanUp()
	{
		this.handle.Clear();
		base.OnCleanUp();
	}

	public void Configure(float tempWarningLow = 283f, float tempLethalLow = 273f, float tempWarningHigh = 294f, float tempLethalHigh = 315f, float _baseHeatTransferLerpRate = 0.3f)
	{
		this.internalTemperatureWarning_Low = tempWarningLow;
		this.internalTemperatureLethal_Low = tempLethalLow;
		this.internalTemperatureLethal_High = tempLethalHigh;
		this.internalTemperatureWarning_High = tempWarningHigh;
		this.baseHeatTransferLerpRate = _baseHeatTransferLerpRate;
	}

	public bool IsCellSafe(int cell)
	{
		float num = Grid.Temperature[cell];
		return num > this.internalTemperatureLethal_Low && num < this.internalTemperatureLethal_High;
	}

	public void UpdateTemperature(object data)
	{
		int num = Grid.PosToCell(base.gameObject);
		if (!Grid.IsValidCell(num))
		{
			return;
		}
		float num2 = Grid.Temperature[num];
		this.primaryElement.Temperature = Mathf.Lerp(this.primaryElement.Temperature, num2, this.baseHeatTransferLerpRate * Time.deltaTime * Grid.Cell[num].mass);
		KSelectable component = base.GetComponent<KSelectable>();
		if (Grid.Element[num].id == SimHashes.Vacuum)
		{
			component.RemoveStatusItem(Db.Get().CreatureStatusItems.EnvironmentTooCold);
			component.RemoveStatusItem(Db.Get().CreatureStatusItems.EnvironmentTooWarm);
		}
		else if (num2 >= this.internalTemperatureLethal_High)
		{
			component.RemoveStatusItem(Db.Get().CreatureStatusItems.EnvironmentTooCold);
			component.AddStatusItem(Db.Get().CreatureStatusItems.EnvironmentTooWarm, this);
		}
		else if (num2 <= this.internalTemperatureLethal_Low)
		{
			component.RemoveStatusItem(Db.Get().CreatureStatusItems.EnvironmentTooWarm);
			component.AddStatusItem(Db.Get().CreatureStatusItems.EnvironmentTooCold, this);
		}
		else
		{
			component.RemoveStatusItem(Db.Get().CreatureStatusItems.EnvironmentTooCold);
			component.RemoveStatusItem(Db.Get().CreatureStatusItems.EnvironmentTooWarm);
		}
		if (this.InternalTemperature <= this.internalTemperatureWarning_Low)
		{
			if (this.InternalTemperature <= this.internalTemperatureLethal_Low)
			{
				if (this.internalTemperatureState != TemperatureVulnerable.TemperatureStates.LethalCold)
				{
					this.SetTemperatureState(TemperatureVulnerable.TemperatureStates.LethalCold);
				}
			}
			else if (this.internalTemperatureState != TemperatureVulnerable.TemperatureStates.WarningCold)
			{
				this.SetTemperatureState(TemperatureVulnerable.TemperatureStates.WarningCold);
			}
		}
		else if (this.InternalTemperature >= this.internalTemperatureWarning_High)
		{
			if (this.InternalTemperature >= this.internalTemperatureLethal_High)
			{
				if (this.internalTemperatureState != TemperatureVulnerable.TemperatureStates.LethalHot)
				{
					this.SetTemperatureState(TemperatureVulnerable.TemperatureStates.LethalHot);
				}
			}
			else if (this.internalTemperatureState != TemperatureVulnerable.TemperatureStates.WarningHot)
			{
				this.SetTemperatureState(TemperatureVulnerable.TemperatureStates.WarningHot);
			}
		}
		else if (this.internalTemperatureState != TemperatureVulnerable.TemperatureStates.Normal)
		{
			this.SetTemperatureState(TemperatureVulnerable.TemperatureStates.Normal);
		}
	}

	private void SetTemperatureState(TemperatureVulnerable.TemperatureStates state)
	{
		KSelectable component = base.GetComponent<KSelectable>();
		if (this.internalTemperatureState != state)
		{
			this.internalTemperatureState = state;
			component.RemoveStatusItem(Db.Get().CreatureStatusItems.Hot);
			component.RemoveStatusItem(Db.Get().CreatureStatusItems.Cold);
			switch (this.internalTemperatureState)
			{
			case TemperatureVulnerable.TemperatureStates.LethalCold:
				this.Trigger(-1758196852, null);
				break;
			case TemperatureVulnerable.TemperatureStates.WarningCold:
				component.AddStatusItem(Db.Get().CreatureStatusItems.Cold, this);
				this.Trigger(-107174716, null);
				break;
			case TemperatureVulnerable.TemperatureStates.Normal:
				this.Trigger(115888613, null);
				break;
			case TemperatureVulnerable.TemperatureStates.WarningHot:
				component.AddStatusItem(Db.Get().CreatureStatusItems.Hot, this);
				this.Trigger(-1234705021, null);
				break;
			case TemperatureVulnerable.TemperatureStates.LethalHot:
				this.Trigger(-55477301, null);
				break;
			}
		}
	}

	[HideInInspector]
	public float internalTemperatureLethal_Low;

	[HideInInspector]
	public float internalTemperatureWarning_Low;

	[HideInInspector]
	public float internalTemperatureWarning_High;

	[HideInInspector]
	public float internalTemperatureLethal_High;

	[MyCmpReq]
	private PrimaryElement primaryElement;

	public float baseHeatTransferLerpRate;

	private TemperatureVulnerable.TemperatureStates internalTemperatureState = TemperatureVulnerable.TemperatureStates.Normal;

	private SchedulerHandle handle;

	public enum TemperatureStates
	{
		LethalCold,
		WarningCold,
		Normal,
		WarningHot,
		LethalHot
	}
}
