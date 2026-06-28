using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class EnergyConsumer : KMonoBehaviour, ISaveLoadableJson, IEnergyConsumer, IEffectDescriptor
{
	public int PowerSortOrder
	{
		get
		{
			return this.powerSortOrder;
		}
	}

	public int DescriptionOrder { get; set; }

	public int PowerCell { get; private set; }

	public bool HasWire
	{
		get
		{
			return Grid.Objects[this.PowerCell, 6] != null;
		}
	}

	public bool IsPowered
	{
		get
		{
			return this.operational.GetFlag(EnergyConsumer.PoweredFlag);
		}
		set
		{
			this.operational.SetFlag(EnergyConsumer.PoweredFlag, value);
		}
	}

	public bool IsConnected { get; private set; }

	public string Name
	{
		get
		{
			return base.GetComponent<KSelectable>().GetName();
		}
	}

	public ushort CircuitID { get; private set; }

	public float BaseWattageRating
	{
		get
		{
			return this._BaseWattageRating * this.GetUpgradeEnergyConsumptionMultiplier();
		}
		set
		{
			if (value != this._BaseWattageRating)
			{
				this._BaseWattageRating = value;
			}
		}
	}

	public float WattsUsed
	{
		get
		{
			if (this.operational.IsActive)
			{
				return this.BaseWattageRating;
			}
			return 0f;
		}
	}

	public float WattsNeededWhenActive
	{
		get
		{
			float energyConsumptionWhenActive = this.building.Def.EnergyConsumptionWhenActive;
			return energyConsumptionWhenActive * this.GetUpgradeEnergyConsumptionMultiplier();
		}
	}

	public float BaseWattsNeededWhenActive
	{
		get
		{
			return this.building.Def.EnergyConsumptionWhenActive;
		}
	}

	public static float CalculateWattageRating(BuildingDef def, Element element)
	{
		return def.EnergyConsumptionWhenActive / BuildingDef.GetEnergyEfficiency(element, 0.0125f);
	}

	public static string GetWattageRatingString(BuildingDef def, Element element, bool isNegative)
	{
		return EnergyConsumer.CalculateWattageRating(def, element).ToString("0") + " W";
	}

	protected override void OnPrefabInit()
	{
		this.CircuitID = ushort.MaxValue;
		this.IsPowered = false;
		this.BaseWattageRating = this.building.Def.EnergyConsumptionWhenActive;
	}

	private float GetUpgradeEnergyConsumptionMultiplier()
	{
		if (this.upgradable != null)
		{
			return this.upgradable.GetEnergyConsumptionMultiplier();
		}
		return 1f;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Building component = base.GetComponent<Building>();
		this.PowerCell = component.GetPowerInputCell();
		Game.Instance.circuitManager.Connect(this);
	}

	protected override void OnCleanUp()
	{
		Game.Instance.circuitManager.Disconnect(this);
		base.OnCleanUp();
	}

	private void SimUpdate(float dt)
	{
		this.CircuitID = Game.Instance.circuitManager.GetCircuitID(this.PowerCell);
		if (!this.IsConnected)
		{
			this.IsPowered = false;
		}
		this.circuitOverloadTime = Mathf.Max(0f, this.circuitOverloadTime - dt);
	}

	public void SetConnectionStatus(CircuitManager.ConnectionStatus connection_status)
	{
		switch (connection_status)
		{
		case CircuitManager.ConnectionStatus.NotConnected:
			this.IsPowered = false;
			this.IsConnected = false;
			break;
		case CircuitManager.ConnectionStatus.Powered:
			if (!this.IsPowered && this.circuitOverloadTime <= 0f)
			{
				this.IsPowered = true;
				base.PlaySound3D(Sounds.Instance.BuildingPowerOnMigrated);
			}
			if (!this.IsConnected)
			{
				this.IsConnected = true;
			}
			break;
		case CircuitManager.ConnectionStatus.OverDraw:
		case CircuitManager.ConnectionStatus.Unpowered:
			if (this.IsPowered && base.GetComponent<Battery>() == null)
			{
				this.IsPowered = false;
				this.circuitOverloadTime = 6f;
				base.PlaySound3D(Sounds.Instance.ElectricGridOverloadMigrated);
			}
			if (!this.IsConnected)
			{
				this.IsConnected = true;
			}
			break;
		}
	}

	public List<Descriptor> GetRequirementDescriptions(BuildingDef def)
	{
		return null;
	}

	public List<Descriptor> GetEffectDescriptions(BuildingDef def)
	{
		return null;
	}

	private const int SimUpdateSortKey = 1001;

	[MyCmpReq]
	private Building building;

	[MyCmpGet]
	private Operational operational;

	[MyCmpGet]
	private Upgradable upgradable;

	[SerializeField]
	public int powerSortOrder;

	[Serialize]
	private float circuitOverloadTime;

	public static Operational.Flag PoweredFlag = new Operational.Flag("powered", Operational.Flag.Type.Requirement);

	private float _BaseWattageRating;
}
