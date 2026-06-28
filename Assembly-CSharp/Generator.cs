using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Generator : KMonoBehaviour, ISaveLoadableJson
{
	public int PowerDistributionOrder
	{
		get
		{
			return this.powerDistributionOrder;
		}
	}

	public virtual float Capacity
	{
		get
		{
			return this.capacity * this.GetUpgradeCapacityMultiplier();
		}
	}

	public virtual float BaseCapacity
	{
		get
		{
			return this.capacity;
		}
	}

	public virtual bool IsEmpty
	{
		get
		{
			return this.joulesAvailable <= 0f;
		}
	}

	public virtual float JoulesAvailable
	{
		get
		{
			return this.joulesAvailable;
		}
		set
		{
			this.joulesAvailable = value;
		}
	}

	public float WattageRating
	{
		get
		{
			return this.building.Def.GeneratorWattageRating * this.Efficiency;
		}
	}

	public float BaseWattageRating
	{
		get
		{
			return this.building.Def.GeneratorWattageRating;
		}
	}

	public float PercentFull
	{
		get
		{
			if (this.Capacity == 0f)
			{
				return 1f;
			}
			return this.joulesAvailable / this.Capacity;
		}
	}

	public bool HasWire
	{
		get
		{
			return Grid.Objects[this.PowerCell, 6] != null;
		}
	}

	public int PowerCell { get; private set; }

	public ushort CircuitID
	{
		get
		{
			return Game.Instance.circuitManager.GetCircuitID(this.PowerCell);
		}
	}

	private float Efficiency
	{
		get
		{
			return BuildingDef.GetEnergyEfficiency(null, this.GetUpgradeEfficiencyMultiplier());
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.capacity = Generator.CalculateCapacity(this.building.Def, null);
		this.PowerCell = this.building.GetPowerInputCell();
		Game.Instance.circuitManager.Connect(this);
		this.CheckConnectionStatus();
	}

	protected virtual void SimUpdate(float dt)
	{
		this.CheckConnectionStatus();
	}

	private void SetStatusItem(StatusItem status_item)
	{
		if (status_item != this.currentStatusItem && this.currentStatusItem != null)
		{
			this.selectable.RemoveStatusItem(this.currentStatusItem);
		}
		if (status_item != null)
		{
			this.selectable.AddStatusItem(status_item, this);
		}
		this.currentStatusItem = status_item;
	}

	private void CheckConnectionStatus()
	{
		if (this.CircuitID == 65535)
		{
			if (this.HasWire)
			{
				this.SetStatusItem(Db.Get().BuildingStatusItems.NoPowerConsumers);
			}
			else
			{
				this.SetStatusItem(Db.Get().BuildingStatusItems.NoWireConnected);
			}
		}
		else
		{
			this.SetStatusItem(null);
		}
	}

	private float GetUpgradeEfficiencyMultiplier()
	{
		if (this.upgradable != null)
		{
			return this.upgradable.GetEnergyGenerationMultiplier();
		}
		return 1f;
	}

	private float GetUpgradeCapacityMultiplier()
	{
		if (this.upgradable != null)
		{
			return this.upgradable.GetCapacityUpgradeMultiplier();
		}
		return 1f;
	}

	protected float GetUpgradeTemperatureMultiplier()
	{
		if (this.upgradable != null)
		{
			return this.upgradable.GetTemperatureUpgradeMultiplier();
		}
		return 1f;
	}

	protected override void OnCleanUp()
	{
		Game.Instance.circuitManager.Disconnect(this);
		base.OnCleanUp();
	}

	public static float CalculateCapacity(BuildingDef def, Element element)
	{
		if (element == null)
		{
			return def.GeneratorBaseCapacity;
		}
		return def.GeneratorBaseCapacity * (1f + ((!element.HasTag(GameTags.RefinedMetal)) ? 0f : 1f));
	}

	public void ApplyDeltaJoules(float joulesDelta, bool canOverPower = false)
	{
		this.joulesAvailable = Mathf.Clamp(this.joulesAvailable + joulesDelta, 0f, (!canOverPower) ? this.Capacity : float.MaxValue);
	}

	public void ApplyImmediateJoulesAvailable(float joulesAvailable, bool canOverPower = false)
	{
		Debug.Assert(base.GetComponent<Battery>() == null);
		this.joulesAvailable = Mathf.Clamp(joulesAvailable, 0f, (!canOverPower) ? this.Capacity : float.MaxValue);
		ReportManager.Instance.ReportValue(ReportManager.ReportType.EnergyCreated, this.joulesAvailable, null);
	}

	protected const int SimUpdateSortKey = 1001;

	[MyCmpReq]
	protected Building building;

	[MyCmpGet]
	protected Upgradable upgradable;

	[MyCmpReq]
	protected Operational operational;

	[MyCmpReq]
	protected KSelectable selectable;

	[Serialize]
	private float joulesAvailable;

	[SerializeField]
	public int powerDistributionOrder;

	private float capacity;

	private StatusItem currentStatusItem;
}
