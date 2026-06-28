using System;
using UnityEngine;

[SkipSaveFileSerialization]
public class RequireInputs : KMonoBehaviour
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
		if (this.requireConduit)
		{
			this.conduitConsumer = base.GetComponent<ConduitConsumer>();
		}
	}

	private void SimUpdate(float dt)
	{
		this.CheckRequirements(false);
	}

	private void CheckRequirements(bool forceEvent)
	{
		bool flag = true;
		if (this.requirePower)
		{
			bool isConnected = this.energy.IsConnected;
			bool isPowered = this.energy.IsPowered;
			if (isConnected)
			{
				if (isConnected != this.wasConnected)
				{
					this.wireConnectedStatusItem = this.selectable.RemoveStatusItem(this.wireConnectedStatusItem, false);
				}
				if (this.visualizeRequirements)
				{
					bool flag2 = !isPowered && (this.button == null || this.button.IsEnabled);
					this.selectable.ToggleStatusItem(Db.Get().BuildingStatusItems.NeedPower, flag2, this);
				}
				flag = flag && isPowered;
			}
			else
			{
				if (isConnected != this.wasConnected)
				{
					this.selectable.RemoveStatusItem(Db.Get().BuildingStatusItems.NeedPower, false);
				}
				if (this.wireConnectedStatusItem == Guid.Empty)
				{
					this.wireConnectedStatusItem = this.selectable.AddStatusItem(Db.Get().BuildingStatusItems.NoWireConnected, null);
				}
				flag = flag && isConnected;
			}
			this.wasConnected = isConnected;
		}
		if (this.requireConduit && this.visualizeRequirements)
		{
			bool flag3 = !this.conduitConsumer.enabled || this.conduitConsumer.IsConnected;
			bool flag4 = !this.conduitConsumer.enabled || this.conduitConsumer.IsSatisfied;
			if (this.previouslyConnected != flag3)
			{
				this.previouslyConnected = flag3;
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
					this.selectable.ToggleStatusItem(statusItem, !flag3, this);
				}
				this.operational.SetFlag(RequireInputs.inputConnectedFlag, flag3);
			}
			flag = flag && flag3;
			if (this.previouslySatisfied != flag4)
			{
				this.previouslySatisfied = flag4;
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
						this.selectable.ToggleStatusItem(statusItem2, !flag4, this);
					}
					this.operational.SetFlag(RequireInputs.pipesHaveMass, flag4);
				}
			}
		}
		this.requirementsMet = flag;
	}

	[SerializeField]
	private bool requirePower = true;

	[SerializeField]
	private bool requireConduit = false;

	private bool wasConnected = false;

	public bool requireConduitHasMass = true;

	[NonSerialized]
	public bool visualizeRequirements = true;

	private static Operational.Flag inputConnectedFlag = new Operational.Flag("inputConnected", Operational.Flag.Type.Requirement);

	private static Operational.Flag pipesHaveMass = new Operational.Flag("pipesHaveMass", Operational.Flag.Type.Requirement);

	private Guid wireConnectedStatusItem;

	private bool requirementsMet = false;

	private BuildingEnabledButton button;

	private IEnergyConsumer energy;

	private ConduitConsumer conduitConsumer;

	[MyCmpReq]
	private KSelectable selectable;

	[MyCmpGet]
	private Operational operational;

	private bool previouslyConnected = true;

	private bool previouslySatisfied = true;
}
