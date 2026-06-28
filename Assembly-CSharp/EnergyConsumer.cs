using System;
using System.Collections.Generic;
using System.Diagnostics;
using FMOD.Studio;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[DebuggerDisplay("{name} {WattsUsed}W")]
public class EnergyConsumer : KMonoBehaviour, ISaveLoadable, IEnergyConsumer, IEffectDescriptor
{
	public int PowerSortOrder
	{
		get
		{
			return this.powerSortOrder;
		}
	}

	public int PowerCell { get; private set; }

	public bool HasWire
	{
		get
		{
			return Grid.Objects[this.PowerCell, 20] != null;
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

	public bool IsConnected
	{
		get
		{
			return this.CircuitID != ushort.MaxValue;
		}
	}

	public string Name
	{
		get
		{
			return this.selectable.GetName();
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
			this._BaseWattageRating = value;
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
		Components.EnergyConsumers.Add(this);
		Building component = base.GetComponent<Building>();
		this.PowerCell = component.GetPowerInputCell();
		Game.Instance.circuitManager.Connect(this);
	}

	protected override void OnCleanUp()
	{
		Game.Instance.circuitManager.Disconnect(this);
		Components.EnergyConsumers.Remove(this);
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
			break;
		case CircuitManager.ConnectionStatus.Unpowered:
			if (this.IsPowered && base.GetComponent<Battery>() == null)
			{
				this.IsPowered = false;
				this.circuitOverloadTime = 6f;
				this.PlayCircuitSound("overdraw");
			}
			break;
		case CircuitManager.ConnectionStatus.Powered:
			if (!this.IsPowered && this.circuitOverloadTime <= 0f)
			{
				this.IsPowered = true;
				this.PlayCircuitSound("powered");
			}
			break;
		}
	}

	private void PlayCircuitSound(string state)
	{
		string text = null;
		if (state == "powered")
		{
			text = Sounds.Instance.BuildingPowerOnMigrated;
		}
		else if (state == "overdraw")
		{
			text = Sounds.Instance.ElectricGridOverloadMigrated;
		}
		else
		{
			global::Debug.Log("Invalid state for sound in EnergyConsumer.", null);
		}
		float num;
		if (!this.lastTimeSoundPlayed.TryGetValue(state, out num))
		{
			num = 0f;
		}
		float num2 = (Time.time - num) / this.soundDecayTime;
		FMOD.Studio.EventInstance eventInstance = KFMOD.BeginOneShot(text, CameraController.Instance.GetVerticallyScaledPosition(this.transform.position));
		eventInstance.setParameterValue("timeSinceLast", num2);
		KFMOD.EndOneShot(eventInstance);
		this.lastTimeSoundPlayed[state] = Time.time;
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
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

	[MyCmpGet]
	private KSelectable selectable;

	[SerializeField]
	public int powerSortOrder;

	[Serialize]
	private float circuitOverloadTime;

	public static Operational.Flag PoweredFlag = new Operational.Flag("powered", Operational.Flag.Type.Requirement);

	private Dictionary<string, float> lastTimeSoundPlayed = new Dictionary<string, float>();

	private float soundDecayTime = 10f;

	private float _BaseWattageRating;
}
