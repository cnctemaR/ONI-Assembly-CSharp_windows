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
			return Grid.Objects[this.PowerCell, 26] != null;
		}
	}

	public virtual bool IsPowered
	{
		get
		{
			return this.operational.GetFlag(EnergyConsumer.PoweredFlag);
		}
		private set
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
			return this._BaseWattageRating;
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
			return this.building.Def.EnergyConsumptionWhenActive;
		}
	}

	public float BaseWattsNeededWhenActive
	{
		get
		{
			return this.building.Def.EnergyConsumptionWhenActive;
		}
	}

	protected override void OnPrefabInit()
	{
		this.CircuitID = ushort.MaxValue;
		this.IsPowered = false;
		this.BaseWattageRating = this.building.Def.EnergyConsumptionWhenActive;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Components.EnergyConsumers.Add(this);
		Building component = base.GetComponent<Building>();
		this.PowerCell = component.GetPowerInputCell();
		Game.Instance.circuitManager.Connect(this);
		Game.Instance.energySim.AddEnergyConsumer(this);
	}

	protected override void OnCleanUp()
	{
		Game.Instance.energySim.RemoveEnergyConsumer(this);
		Game.Instance.circuitManager.Disconnect(this);
		Components.EnergyConsumers.Remove(this);
		base.OnCleanUp();
	}

	public virtual void EnergySim200ms(float dt)
	{
		this.CircuitID = Game.Instance.circuitManager.GetCircuitID(this.PowerCell);
		if (!this.IsConnected)
		{
			this.IsPowered = false;
		}
		this.circuitOverloadTime = Mathf.Max(0f, this.circuitOverloadTime - dt);
	}

	public virtual void SetConnectionStatus(CircuitManager.ConnectionStatus connection_status)
	{
		if (connection_status != CircuitManager.ConnectionStatus.NotConnected)
		{
			if (connection_status != CircuitManager.ConnectionStatus.Unpowered)
			{
				if (connection_status == CircuitManager.ConnectionStatus.Powered)
				{
					if (!this.IsPowered && this.circuitOverloadTime <= 0f)
					{
						this.IsPowered = true;
						this.PlayCircuitSound("powered");
					}
				}
			}
			else if (this.IsPowered && base.GetComponent<Battery>() == null)
			{
				this.IsPowered = false;
				this.circuitOverloadTime = 6f;
				this.PlayCircuitSound("overdraw");
			}
		}
		else
		{
			this.IsPowered = false;
		}
	}

	protected void PlayCircuitSound(string state)
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
			global::Debug.Log("Invalid state for sound in EnergyConsumer.");
		}
		if (!CameraController.Instance.IsAudibleSound(base.transform.GetPosition()))
		{
			return;
		}
		float num;
		if (!this.lastTimeSoundPlayed.TryGetValue(state, out num))
		{
			num = 0f;
		}
		float num2 = (Time.time - num) / this.soundDecayTime;
		FMOD.Studio.EventInstance eventInstance = KFMOD.BeginOneShot(text, CameraController.Instance.GetVerticallyScaledPosition(base.transform.GetPosition()));
		eventInstance.setParameterValue("timeSinceLast", num2);
		KFMOD.EndOneShot(eventInstance);
		this.lastTimeSoundPlayed[state] = Time.time;
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		return null;
	}

	[MyCmpReq]
	private Building building;

	[MyCmpGet]
	protected Operational operational;

	[MyCmpGet]
	private KSelectable selectable;

	[SerializeField]
	public int powerSortOrder;

	[Serialize]
	protected float circuitOverloadTime;

	public static readonly Operational.Flag PoweredFlag = new Operational.Flag("powered", Operational.Flag.Type.Requirement);

	private Dictionary<string, float> lastTimeSoundPlayed = new Dictionary<string, float>();

	private float soundDecayTime = 10f;

	private float _BaseWattageRating;
}
