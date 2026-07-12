using System;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/RequireInputs")]
public class RequireInputs : KMonoBehaviour, ISim200ms
{
	public bool RequiresPower
	{
		get
		{
			return this.requirePower;
		}
	}

	public bool RequiresInputConduit
	{
		get
		{
			return this.requireConduit;
		}
	}

	public void SetRequirements(bool power, bool conduit)
	{
		this.requirePower = power;
		this.requireConduit = conduit;
	}

	public bool RequirementsMet
	{
		get
		{
			return this.requirementsMet;
		}
	}

	protected override void OnPrefabInit()
	{
		this.Bind();
	}

	protected override void OnSpawn()
	{
		this.CheckRequirements(true);
		this.Bind();
	}

	[ContextMenu("Bind")]
	private void Bind()
	{
		if (this.requirePower)
		{
			this.energy = base.GetComponent<IEnergyConsumer>();
			this.button = base.GetComponent<BuildingEnabledButton>();
		}
		if (this.requireConduit && !this.conduitConsumer)
		{
			this.conduitConsumer = base.GetComponent<ConduitConsumer>();
		}
	}

	public void Sim200ms(float dt)
	{
		this.CheckRequirements(false);
	}

	private void CheckRequirements(bool forceEvent)
	{
		bool flag = true;
		bool flag2 = false;
		if (this.requirePower)
		{
			bool isConnected = this.energy.IsConnected;
			bool isPowered = this.energy.IsPowered;
			flag = flag && isPowered && isConnected;
			bool flag3 = this.VisualizeRequirement(RequireInputs.Requirements.NeedPower) && isConnected && !isPowered && (this.button == null || this.button.IsEnabled);
			bool flag4 = this.VisualizeRequirement(RequireInputs.Requirements.NoWire) && !isConnected;
			this.needPowerStatusGuid = this.selectable.ToggleStatusItem(Db.Get().BuildingStatusItems.NeedPower, this.needPowerStatusGuid, flag3, this);
			this.noWireStatusGuid = this.selectable.ToggleStatusItem(Db.Get().BuildingStatusItems.NoWireConnected, this.noWireStatusGuid, flag4, this);
			flag2 = flag != this.RequirementsMet && base.GetComponent<Light2D>() != null;
		}
		if (this.requireConduit)
		{
			bool flag5 = !this.conduitConsumer.enabled || this.conduitConsumer.IsConnected;
			bool flag6 = !this.conduitConsumer.enabled || this.conduitConsumer.IsSatisfied;
			if (this.VisualizeRequirement(RequireInputs.Requirements.ConduitConnected) && this.previouslyConnected != flag5)
			{
				this.previouslyConnected = flag5;
				StatusItem statusItem = null;
				ConduitType conduitType = this.conduitConsumer.TypeOfConduit;
				if (conduitType != ConduitType.Gas)
				{
					if (conduitType == ConduitType.Liquid)
					{
						statusItem = Db.Get().BuildingStatusItems.NeedLiquidIn;
					}
				}
				else
				{
					statusItem = Db.Get().BuildingStatusItems.NeedGasIn;
				}
				if (statusItem != null)
				{
					this.selectable.ToggleStatusItem(statusItem, !flag5, new global::Tuple<ConduitType, Tag>(this.conduitConsumer.TypeOfConduit, this.conduitConsumer.capacityTag));
				}
				this.operational.SetFlag(RequireInputs.inputConnectedFlag, flag5);
			}
			flag = flag && flag5;
			if (this.VisualizeRequirement(RequireInputs.Requirements.ConduitEmpty) && this.previouslySatisfied != flag6)
			{
				this.previouslySatisfied = flag6;
				StatusItem statusItem2 = null;
				ConduitType conduitType = this.conduitConsumer.TypeOfConduit;
				if (conduitType != ConduitType.Gas)
				{
					if (conduitType == ConduitType.Liquid)
					{
						statusItem2 = Db.Get().BuildingStatusItems.LiquidPipeEmpty;
					}
				}
				else
				{
					statusItem2 = Db.Get().BuildingStatusItems.GasPipeEmpty;
				}
				if (this.requireConduitHasMass)
				{
					if (statusItem2 != null)
					{
						this.selectable.ToggleStatusItem(statusItem2, !flag6, this);
					}
					this.operational.SetFlag(RequireInputs.pipesHaveMass, flag6);
				}
			}
		}
		this.requirementsMet = flag;
		if (flag2)
		{
			Room roomOfGameObject = Game.Instance.roomProber.GetRoomOfGameObject(base.gameObject);
			if (roomOfGameObject != null)
			{
				Game.Instance.roomProber.UpdateRoom(roomOfGameObject.cavity);
			}
		}
	}

	public bool VisualizeRequirement(RequireInputs.Requirements r)
	{
		return (this.visualizeRequirements & r) == r;
	}

	[SerializeField]
	private bool requirePower = true;

	[SerializeField]
	private bool requireConduit;

	public bool requireConduitHasMass = true;

	public RequireInputs.Requirements visualizeRequirements = RequireInputs.Requirements.All;

	private static readonly Operational.Flag inputConnectedFlag = new Operational.Flag("inputConnected", Operational.Flag.Type.Requirement);

	private static readonly Operational.Flag pipesHaveMass = new Operational.Flag("pipesHaveMass", Operational.Flag.Type.Requirement);

	private Guid noWireStatusGuid;

	private Guid needPowerStatusGuid;

	private bool requirementsMet;

	private BuildingEnabledButton button;

	private IEnergyConsumer energy;

	public ConduitConsumer conduitConsumer;

	[MyCmpReq]
	private KSelectable selectable;

	[MyCmpGet]
	private Operational operational;

	private bool previouslyConnected = true;

	private bool previouslySatisfied = true;

	[Flags]
	public enum Requirements
	{
		None = 0,
		NoWire = 1,
		NeedPower = 2,
		ConduitConnected = 4,
		ConduitEmpty = 8,
		AllPower = 3,
		AllConduit = 12,
		All = 15
	}
}
