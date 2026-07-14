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
		set
		{
			this.requirePower = value;
		}
	}

	public bool RequiresInputConduit
	{
		get
		{
			return this.requireConduit;
		}
		set
		{
			this.requireConduit = value;
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
		this.CheckRequirements();
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
		Operational component = base.GetComponent<Operational>();
		bool flag = component != null;
		this.previouslyConnectedOpFlag = flag && component.GetFlag(RequireInputs.inputConnectedFlag);
		this.previouslySatisfiedOpFlag = flag && component.GetFlag(RequireInputs.pipesHaveMass);
	}

	public void Sim200ms(float dt)
	{
		this.CheckRequirements();
	}

	private void CheckRequirements()
	{
		bool flag = true;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		if (this.requirePower)
		{
			bool isConnected = this.energy.IsConnected;
			bool isPowered = this.energy.IsPowered;
			flag &= isPowered && isConnected;
			flag3 = this.VisualizeRequirement(RequireInputs.Requirements.NeedPower) && isConnected && !isPowered && (this.button == null || this.button.IsEnabled);
			flag4 = this.VisualizeRequirement(RequireInputs.Requirements.NoWire) && !isConnected;
		}
		bool flag5 = flag2 | (flag != this.requirementsMet && base.GetComponent<Light2D>() != null);
		this.needPowerStatusGuid = this.selectable.ToggleStatusItem(Db.Get().BuildingStatusItems.NeedPower, this.needPowerStatusGuid, flag3, this);
		this.noWireStatusGuid = this.selectable.ToggleStatusItem(Db.Get().BuildingStatusItems.NoWireConnected, this.noWireStatusGuid, flag4, this);
		bool flag6 = this.conduitConsumer != null && this.conduitConsumer.conduitType == ConduitType.Liquid;
		bool flag7 = flag6 && this.conduitConsumer.IsConnected;
		bool flag8 = flag6 && this.conduitConsumer.IsSatisfied;
		bool flag9 = flag6 && this.conduitConsumer.enabled && this.requireConduitHasMass && this.requireConduit && this.VisualizeRequirement(RequireInputs.Requirements.ConduitEmpty);
		bool flag10 = flag6 && this.conduitConsumer.enabled && this.requireConduit && this.VisualizeRequirement(RequireInputs.Requirements.ConduitConnected);
		flag &= !flag9 || flag8;
		flag &= !flag10 || flag7;
		this.liquidConduitEmptyStatusGuid = this.selectable.ToggleStatusItem(Db.Get().BuildingStatusItems.LiquidPipeEmpty, this.liquidConduitEmptyStatusGuid, this.VisualizeRequirement(RequireInputs.Requirements.ConduitEmpty) && flag9 && !flag8, this);
		this.noLiquidConduitStatusGuid = this.selectable.ToggleStatusItem(Db.Get().BuildingStatusItems.NeedLiquidIn, this.noLiquidConduitStatusGuid, this.VisualizeRequirement(RequireInputs.Requirements.ConduitConnected) && flag10 && !flag7, this.conduitConsumer);
		if (flag6)
		{
			bool flag11 = !flag10 || flag7;
			bool flag12 = !flag9 || flag8;
			if (flag11 != this.previouslyConnectedOpFlag)
			{
				this.operational.SetFlag(RequireInputs.inputConnectedFlag, flag11);
				this.previouslyConnectedOpFlag = flag11;
			}
			if (flag12 != this.previouslySatisfiedOpFlag)
			{
				this.operational.SetFlag(RequireInputs.pipesHaveMass, flag12);
				this.previouslySatisfiedOpFlag = flag12;
			}
		}
		bool flag13 = this.conduitConsumer != null && this.conduitConsumer.conduitType == ConduitType.Gas;
		bool flag14 = flag13 && this.conduitConsumer.IsConnected;
		bool flag15 = flag13 && this.conduitConsumer.IsSatisfied;
		bool flag16 = flag13 && this.conduitConsumer.enabled && this.requireConduitHasMass && this.requireConduit && this.VisualizeRequirement(RequireInputs.Requirements.ConduitEmpty);
		bool flag17 = flag13 && this.conduitConsumer.enabled && this.requireConduit && this.VisualizeRequirement(RequireInputs.Requirements.ConduitConnected);
		flag &= !flag16 || flag15;
		flag &= !flag17 || flag14;
		this.gasConduitEmptyStatusGuid = this.selectable.ToggleStatusItem(Db.Get().BuildingStatusItems.GasPipeEmpty, this.gasConduitEmptyStatusGuid, this.VisualizeRequirement(RequireInputs.Requirements.ConduitEmpty) && flag16 && !flag15, this);
		this.noGasConduitStatusGuid = this.selectable.ToggleStatusItem(Db.Get().BuildingStatusItems.NeedGasIn, this.noGasConduitStatusGuid, this.VisualizeRequirement(RequireInputs.Requirements.ConduitConnected) && flag17 && !flag14, this.conduitConsumer);
		if (flag13)
		{
			bool flag18 = !flag17 || flag14;
			bool flag19 = !flag16 || flag15;
			if (flag18 != this.previouslyConnectedOpFlag)
			{
				this.operational.SetFlag(RequireInputs.inputConnectedFlag, flag18);
				this.previouslyConnectedOpFlag = flag18;
			}
			if (flag19 != this.previouslySatisfiedOpFlag)
			{
				this.operational.SetFlag(RequireInputs.pipesHaveMass, flag19);
				this.previouslySatisfiedOpFlag = flag19;
			}
		}
		this.requirementsMet = flag;
		if (flag5)
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

	private Guid noWireStatusGuid = Guid.Empty;

	private Guid needPowerStatusGuid = Guid.Empty;

	private Guid liquidConduitEmptyStatusGuid = Guid.Empty;

	private Guid gasConduitEmptyStatusGuid = Guid.Empty;

	private Guid noLiquidConduitStatusGuid = Guid.Empty;

	private Guid noGasConduitStatusGuid = Guid.Empty;

	private bool requirementsMet;

	private BuildingEnabledButton button;

	private IEnergyConsumer energy;

	public ConduitConsumer conduitConsumer;

	[MyCmpReq]
	private KSelectable selectable;

	[MyCmpGet]
	private Operational operational;

	private bool previouslyConnectedOpFlag = true;

	private bool previouslySatisfiedOpFlag = true;

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
