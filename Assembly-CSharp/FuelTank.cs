using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

public class FuelTank : KMonoBehaviour, IUserControlledCapacity, IFuelTank
{
	public IStorage Storage
	{
		get
		{
			return this.storage;
		}
	}

	public bool ConsumeFuelOnLand
	{
		get
		{
			return this.consumeFuelOnLand;
		}
	}

	public float UserMaxCapacity
	{
		get
		{
			return this.targetFillMass;
		}
		set
		{
			this.targetFillMass = value;
			this.storage.capacityKg = this.targetFillMass;
			ConduitConsumer component = base.GetComponent<ConduitConsumer>();
			if (component != null)
			{
				component.capacityKG = this.targetFillMass;
			}
			ManualDeliveryKG component2 = base.GetComponent<ManualDeliveryKG>();
			if (component2 != null)
			{
				component2.capacity = (component2.refillMass = this.targetFillMass);
			}
			base.Trigger(-945020481, this);
		}
	}

	public float MinCapacity
	{
		get
		{
			return 0f;
		}
	}

	public float MaxCapacity
	{
		get
		{
			return this.physicalFuelCapacity;
		}
	}

	public float AmountStored
	{
		get
		{
			return this.storage.MassStored();
		}
	}

	public bool WholeValues
	{
		get
		{
			return false;
		}
	}

	public LocString CapacityUnits
	{
		get
		{
			return GameUtil.GetCurrentMassUnit(false);
		}
	}

	public Tag FuelType
	{
		get
		{
			return this.fuelType;
		}
		set
		{
			this.fuelType = value;
			if (this.storage.storageFilters == null)
			{
				this.storage.storageFilters = new List<Tag>();
			}
			this.storage.storageFilters.Add(this.fuelType);
			ManualDeliveryKG component = base.GetComponent<ManualDeliveryKG>();
			if (component != null)
			{
				component.RequestedItemTag = this.fuelType;
			}
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe<FuelTank>(-905833192, FuelTank.OnCopySettingsDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.targetFillMass == -1f)
		{
			this.targetFillMass = this.physicalFuelCapacity;
		}
		base.GetComponent<KBatchedAnimController>().Play("grounded", KAnim.PlayMode.Loop, 1f, 0f);
		if (DlcManager.FeatureClusterSpaceEnabled())
		{
			base.GetComponent<RocketModule>().AddModuleCondition(ProcessCondition.ProcessConditionType.RocketStorage, new ConditionProperlyFueled(this));
		}
		base.Subscribe<FuelTank>(-887025858, FuelTank.OnRocketLandedDelegate);
		this.UserMaxCapacity = this.UserMaxCapacity;
		this.meter = new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[] { "meter_target", "meter_fill", "meter_frame", "meter_OL" });
		this.meter.gameObject.GetComponent<KBatchedAnimTracker>().matchParentOffset = true;
		this.OnStorageChange(null);
		base.Subscribe<FuelTank>(-1697596308, FuelTank.OnStorageChangedDelegate);
	}

	private void OnStorageChange(object data)
	{
		this.meter.SetPositionPercent(this.storage.MassStored() / this.storage.capacityKg);
	}

	private void OnRocketLanded(object data)
	{
		if (this.ConsumeFuelOnLand)
		{
			this.storage.ConsumeAllIgnoringDisease();
		}
	}

	private void OnCopySettings(object data)
	{
		FuelTank component = ((GameObject)data).GetComponent<FuelTank>();
		if (component != null)
		{
			this.UserMaxCapacity = component.UserMaxCapacity;
		}
	}

	public void DEBUG_FillTank()
	{
		if (!DlcManager.FeatureClusterSpaceEnabled())
		{
			RocketEngine rocketEngine = null;
			foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(base.GetComponent<AttachableBuilding>()))
			{
				rocketEngine = gameObject.GetComponent<RocketEngine>();
				if (rocketEngine != null && rocketEngine.mainEngine)
				{
					break;
				}
			}
			if (rocketEngine != null)
			{
				Element element = ElementLoader.GetElement(rocketEngine.fuelTag);
				if (element.IsLiquid)
				{
					this.storage.AddLiquid(element.id, this.targetFillMass - this.storage.MassStored(), element.defaultValues.temperature, 0, 0, false, true);
					return;
				}
				if (element.IsGas)
				{
					this.storage.AddGasChunk(element.id, this.targetFillMass - this.storage.MassStored(), element.defaultValues.temperature, 0, 0, false, true);
					return;
				}
				if (element.IsSolid)
				{
					this.storage.AddOre(element.id, this.targetFillMass - this.storage.MassStored(), element.defaultValues.temperature, 0, 0, false, true);
					return;
				}
			}
			else
			{
				global::Debug.LogWarning("Fuel tank couldn't find rocket engine");
			}
			return;
		}
		RocketEngineCluster rocketEngineCluster = null;
		foreach (GameObject gameObject2 in AttachableBuilding.GetAttachedNetwork(base.GetComponent<AttachableBuilding>()))
		{
			rocketEngineCluster = gameObject2.GetComponent<RocketEngineCluster>();
			if (rocketEngineCluster != null && rocketEngineCluster.mainEngine)
			{
				break;
			}
		}
		if (rocketEngineCluster != null)
		{
			Element element2 = ElementLoader.GetElement(rocketEngineCluster.fuelTag);
			if (element2.IsLiquid)
			{
				this.storage.AddLiquid(element2.id, this.targetFillMass - this.storage.MassStored(), element2.defaultValues.temperature, 0, 0, false, true);
			}
			else if (element2.IsGas)
			{
				this.storage.AddGasChunk(element2.id, this.targetFillMass - this.storage.MassStored(), element2.defaultValues.temperature, 0, 0, false, true);
			}
			else if (element2.IsSolid)
			{
				this.storage.AddOre(element2.id, this.targetFillMass - this.storage.MassStored(), element2.defaultValues.temperature, 0, 0, false, true);
			}
			rocketEngineCluster.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>().UpdateStatusItem();
			return;
		}
		global::Debug.LogWarning("Fuel tank couldn't find rocket engine");
	}

	public Storage storage;

	private MeterController meter;

	[Serialize]
	public float targetFillMass = -1f;

	[SerializeField]
	public float physicalFuelCapacity;

	public bool consumeFuelOnLand;

	[SerializeField]
	private Tag fuelType;

	private static readonly EventSystem.IntraObjectHandler<FuelTank> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<FuelTank>(delegate(FuelTank component, object data)
	{
		component.OnCopySettings(data);
	});

	private static readonly EventSystem.IntraObjectHandler<FuelTank> OnRocketLandedDelegate = new EventSystem.IntraObjectHandler<FuelTank>(delegate(FuelTank component, object data)
	{
		component.OnRocketLanded(data);
	});

	private static readonly EventSystem.IntraObjectHandler<FuelTank> OnStorageChangedDelegate = new EventSystem.IntraObjectHandler<FuelTank>(delegate(FuelTank component, object data)
	{
		component.OnStorageChange(data);
	});
}
