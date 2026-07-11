using System;
using UnityEngine;

[SkipSaveFileSerialization]
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
			bool flag3 = this.visualizeRequirements && isConnected && !isPowered && (this.button == null || this.button.IsEnabled);
			this.needPowerStatusGuid = this.selectable.ToggleStatusItem(Db.Get().BuildingStatusItems.NeedPower, this.needPowerStatusGuid, flag3, this);
			this.noWireStatusGuid = this.selectable.ToggleStatusItem(Db.Get().BuildingStatusItems.NoWireConnected, this.noWireStatusGuid, !isConnected, this);
			flag2 = flag != this.RequirementsMet && base.GetComponent<Light2D>() != null;
		}
		if (this.requireConduit && this.visualizeRequirements)
		{
			bool flag4 = !this.conduitConsumer.enabled || this.conduitConsumer.IsConnected;
			bool flag5 = !this.conduitConsumer.enabled || this.conduitConsumer.IsSatisfied;
			if (this.previouslyConnected != flag4)
			{
				this.previouslyConnected = flag4;
				StatusItem statusItem = null;
				ConduitType typeOfConduit = this.conduitConsumer.TypeOfConduit;
				if (typeOfConduit != ConduitType.Liquid)
				{
					if (typeOfConduit == ConduitType.Gas)
					{
						statusItem = Db.Get().BuildingStatusItems.NeedGasIn;
					}
				}
				else
				{
					statusItem = Db.Get().BuildingStatusItems.NeedLiquidIn;
				}
				if (statusItem != null)
				{
					this.selectable.ToggleStatusItem(statusItem, !flag4, new Tuple<ConduitType, Tag>(this.conduitConsumer.TypeOfConduit, this.conduitConsumer.capacityTag));
				}
				this.operational.SetFlag(RequireInputs.inputConnectedFlag, flag4);
			}
			flag = flag && flag4;
			if (this.previouslySatisfied != flag5)
			{
				this.previouslySatisfied = flag5;
				StatusItem statusItem2 = null;
				ConduitType typeOfConduit2 = this.conduitConsumer.TypeOfConduit;
				if (typeOfConduit2 != ConduitType.Liquid)
				{
					if (typeOfConduit2 == ConduitType.Gas)
					{
						statusItem2 = Db.Get().BuildingStatusItems.GasPipeEmpty;
					}
				}
				else
				{
					statusItem2 = Db.Get().BuildingStatusItems.LiquidPipeEmpty;
				}
				if (this.requireConduitHasMass)
				{
					if (statusItem2 != null)
					{
						this.selectable.ToggleStatusItem(statusItem2, !flag5, this);
					}
					this.operational.SetFlag(RequireInputs.pipesHaveMass, flag5);
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

	[SerializeField]
	private bool requirePower = true;

	[SerializeField]
	private bool requireConduit;

	public bool requireConduitHasMass = true;

	public bool visualizeRequirements = true;

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
}
