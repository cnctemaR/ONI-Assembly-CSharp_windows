using System;
using System.Collections.Generic;
using System.Diagnostics;
using Klei.AI;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[DebuggerDisplay("{name}")]
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
			return this.capacity;
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
			bool flag = false;
			GameObject gameObject = Grid.Objects[this.PowerCell, 26];
			if (gameObject != null && gameObject.GetComponent<BuildingComplete>() != null)
			{
				flag = true;
			}
			return flag;
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
			return Mathf.Max(1f + this.generatorOutputAttribute.GetTotalValue() / 100f, 0.1f);
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Attributes attributes = base.gameObject.GetAttributes();
		this.generatorOutputAttribute = attributes.Add(Db.Get().Attributes.GeneratorOutput);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Components.Generators.Add(this);
		base.Subscribe<Generator>(-592767678, Generator.OnOperationalChangedDelegate);
		this.capacity = Generator.CalculateCapacity(this.building.Def, null);
		this.PowerCell = this.building.GetPowerOutputCell();
		this.CheckConnectionStatus();
		this.OnOperationalChanged(null);
		Game.Instance.energySim.AddGenerator(this);
	}

	public virtual void EnergySim200ms(float dt)
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

	protected override void OnCleanUp()
	{
		Game.Instance.energySim.RemoveGenerator(this);
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
		global::Debug.Assert(base.GetComponent<Battery>() == null);
		this.joulesAvailable = Mathf.Clamp(joulesAvailable, 0f, (!canOverPower) ? this.Capacity : float.MaxValue);
		ReportManager.Instance.ReportValue(ReportManager.ReportType.EnergyCreated, this.joulesAvailable, this.GetProperName(), null);
		if (!Game.Instance.savedInfo.powerCreatedbyGeneratorType.ContainsKey(this.PrefabID()))
		{
			Game.Instance.savedInfo.powerCreatedbyGeneratorType.Add(this.PrefabID(), 0f);
		}
		Dictionary<Tag, float> powerCreatedbyGeneratorType;
		Tag tag;
		(powerCreatedbyGeneratorType = Game.Instance.savedInfo.powerCreatedbyGeneratorType)[tag = this.PrefabID()] = powerCreatedbyGeneratorType[tag] + this.joulesAvailable;
	}

	public void AssignJoulesAvailable(float joulesAvailable)
	{
		global::Debug.Assert(base.GetComponent<PowerTransformer>() != null);
		this.joulesAvailable = joulesAvailable;
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

	public virtual void ConsumeEnergy(float joules)
	{
		this.joulesAvailable = Mathf.Max(0f, this.JoulesAvailable - joules);
	}

	protected const int SimUpdateSortKey = 1001;

	[MyCmpReq]
	protected Building building;

	[MyCmpReq]
	protected Operational operational;

	[MyCmpReq]
	protected KSelectable selectable;

	[Serialize]
	private float joulesAvailable;

	[SerializeField]
	public int powerDistributionOrder;

	public static readonly Operational.Flag generatorConnectedFlag = new Operational.Flag("GeneratorConnected", Operational.Flag.Type.Requirement);

	protected static readonly Operational.Flag wireConnectedFlag = new Operational.Flag("generatorWireConnected", Operational.Flag.Type.Requirement);

	private float capacity;

	private StatusItem currentStatusItem;

	private Guid statusItemID;

	private AttributeInstance generatorOutputAttribute;

	private static readonly EventSystem.IntraObjectHandler<Generator> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<Generator>(delegate(Generator component, object data)
	{
		component.OnOperationalChanged(data);
	});
}
