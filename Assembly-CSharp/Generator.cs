using System;
using System.Diagnostics;
using KSerialization;
using UnityEngine;

[DebuggerDisplay("{name}")]
[SerializationConfig(MemberSerialization.OptIn)]
public class Generator : KMonoBehaviour, ISaveLoadable, IEnergyProducer
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
			return Grid.Objects[this.PowerCell, 20] != null;
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
		Components.Generators.Add(this);
		this.Subscribe(-592767678, new Action<object>(this.OnOperationalChanged));
		this.capacity = Generator.CalculateCapacity(this.building.Def, null);
		this.PowerCell = this.building.GetPowerOutputCell();
		this.CheckConnectionStatus();
		this.OnOperationalChanged(null);
	}

	protected virtual void SimUpdate(float dt)
	{
		this.CheckConnectionStatus();
	}

	private void SetStatusItem(StatusItem status_item)
	{
		if (status_item != this.currentStatusItem && this.currentStatusItem != null)
		{
			this.statusItemID = this.selectable.RemoveStatusItem(this.statusItemID, false);
		}
		if (status_item != null && this.statusItemID == Guid.Empty)
		{
			this.statusItemID = this.selectable.AddStatusItem(status_item, this);
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
				this.operational.SetFlag(Generator.generatorConnectedFlag, true);
			}
			else
			{
				this.SetStatusItem(Db.Get().BuildingStatusItems.NoWireConnected);
				this.operational.SetFlag(Generator.generatorConnectedFlag, false);
			}
		}
		else
		{
			this.SetStatusItem(null);
			this.operational.SetFlag(Generator.generatorConnectedFlag, true);
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
		Components.Generators.Remove(this);
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

	public void ResetJoules()
	{
		this.joulesAvailable = 0f;
	}

	public virtual void ApplyDeltaJoules(float joulesDelta, bool canOverPower = false)
	{
		this.joulesAvailable = Mathf.Clamp(this.joulesAvailable + joulesDelta, 0f, (!canOverPower) ? this.Capacity : float.MaxValue);
	}

	public void GenerateJoules(float joulesAvailable, bool canOverPower = false)
	{
		this.joulesAvailable = Mathf.Clamp(joulesAvailable, 0f, (!canOverPower) ? this.Capacity : float.MaxValue);
		ReportManager.Instance.ReportValue(ReportManager.ReportType.EnergyCreated, this.joulesAvailable, this.GetProperName(), null);
	}

	private void OnOperationalChanged(object data)
	{
		if (this.operational.IsOperational)
		{
			Game.Instance.circuitManager.Connect(this);
		}
		else
		{
			Game.Instance.circuitManager.Disconnect(this);
		}
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

	public static readonly Operational.Flag generatorConnectedFlag = new Operational.Flag("GeneratorConnected", Operational.Flag.Type.Requirement);

	private float capacity;

	private StatusItem currentStatusItem;

	private Guid statusItemID;
}
