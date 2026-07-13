using System;
using STRINGS;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/SpawnableConduitConsumer")]
public class EntityConduitConsumer : KMonoBehaviour, IConduitConsumer
{
	public Storage Storage
	{
		get
		{
			return this.storage;
		}
	}

	public ConduitType ConduitType
	{
		get
		{
			return this.conduitType;
		}
	}

	public bool IsConnected
	{
		get
		{
			return Grid.Objects[this.utilityCell, (this.conduitType == ConduitType.Gas) ? 12 : 16] != null;
		}
	}

	public bool CanConsume
	{
		get
		{
			bool flag = false;
			if (this.IsConnected)
			{
				flag = this.GetConduitManager().GetContents(this.utilityCell).mass > 0f;
			}
			return flag;
		}
	}

	public float stored_mass
	{
		get
		{
			if (this.storage == null)
			{
				return 0f;
			}
			if (!(this.capacityTag != GameTags.Any))
			{
				return this.storage.MassStored();
			}
			return this.storage.GetMassAvailable(this.capacityTag);
		}
	}

	public float space_remaining_kg
	{
		get
		{
			float num = this.capacityKG - this.stored_mass;
			if (!(this.storage == null))
			{
				return Mathf.Min(this.storage.RemainingCapacity(), num);
			}
			return num;
		}
	}

	public void SetConduitData(ConduitType type)
	{
		this.conduitType = type;
	}

	public ConduitType TypeOfConduit
	{
		get
		{
			return this.conduitType;
		}
	}

	public bool IsAlmostEmpty
	{
		get
		{
			return !this.ignoreMinMassCheck && this.MassAvailable < this.ConsumptionRate * 30f;
		}
	}

	public bool IsEmpty
	{
		get
		{
			return !this.ignoreMinMassCheck && (this.MassAvailable == 0f || this.MassAvailable < this.ConsumptionRate);
		}
	}

	public float ConsumptionRate
	{
		get
		{
			return this.consumptionRate;
		}
	}

	public bool IsSatisfied
	{
		get
		{
			return this.satisfied || !this.isConsuming;
		}
		set
		{
			this.satisfied = value || this.forceAlwaysSatisfied;
		}
	}

	private ConduitFlow GetConduitManager()
	{
		ConduitType conduitType = this.conduitType;
		if (conduitType == ConduitType.Gas)
		{
			return Game.Instance.gasConduitFlow;
		}
		if (conduitType != ConduitType.Liquid)
		{
			return null;
		}
		return Game.Instance.liquidConduitFlow;
	}

	public float MassAvailable
	{
		get
		{
			ConduitFlow conduitManager = this.GetConduitManager();
			int inputCell = this.GetInputCell(conduitManager.conduitType);
			return conduitManager.GetContents(inputCell).mass;
		}
	}

	private int GetInputCell(ConduitType inputConduitType)
	{
		return this.occupyArea.GetOffsetCellWithRotation(this.offset);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		ConduitFlow conduitManager = this.GetConduitManager();
		this.utilityCell = this.GetInputCell(conduitManager.conduitType);
		ScenePartitionerLayer scenePartitionerLayer = GameScenePartitioner.Instance.objectLayers[(this.conduitType == ConduitType.Gas) ? 12 : 16];
		this.partitionerEntry = GameScenePartitioner.Instance.Add("ConduitConsumer.OnSpawn", base.gameObject, this.utilityCell, scenePartitionerLayer, new Action<object>(this.OnConduitConnectionChanged));
		this.GetConduitManager().AddConduitUpdater(new Action<float>(this.ConduitUpdate), ConduitFlowPriority.Default);
		this.endpoint = new FlowUtilityNetwork.NetworkItem(conduitManager.conduitType, Endpoint.Sink, this.utilityCell, base.gameObject);
		if (conduitManager.conduitType == ConduitType.Solid)
		{
			Game.Instance.solidConduitSystem.AddToNetworks(this.utilityCell, this.endpoint, true);
		}
		else
		{
			Conduit.GetNetworkManager(conduitManager.conduitType).AddToNetworks(this.utilityCell, this.endpoint, true);
		}
		EntityCellVisualizer.Ports ports = EntityCellVisualizer.Ports.LiquidIn;
		if (conduitManager.conduitType == ConduitType.Solid)
		{
			ports = EntityCellVisualizer.Ports.SolidIn;
		}
		else if (conduitManager.conduitType == ConduitType.Gas)
		{
			ports = EntityCellVisualizer.Ports.GasIn;
		}
		this.cellVisualizer.AddPort(ports, this.offset);
		this.OnConduitConnectionChanged(null);
	}

	protected override void OnCleanUp()
	{
		if (this.endpoint.ConduitType == ConduitType.Solid)
		{
			Game.Instance.solidConduitSystem.RemoveFromNetworks(this.endpoint.Cell, this.endpoint, true);
		}
		else
		{
			Conduit.GetNetworkManager(this.endpoint.ConduitType).RemoveFromNetworks(this.endpoint.Cell, this.endpoint, true);
		}
		this.GetConduitManager().RemoveConduitUpdater(new Action<float>(this.ConduitUpdate));
		GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
		base.OnCleanUp();
	}

	private void OnConduitConnectionChanged(object _)
	{
		base.Trigger(-2094018600, BoxedBools.Box(this.IsConnected));
	}

	public void SetOnState(bool onState)
	{
		this.isOn = onState;
	}

	private void ConduitUpdate(float dt)
	{
		if (this.isConsuming && this.isOn)
		{
			ConduitFlow conduitManager = this.GetConduitManager();
			this.Consume(dt, conduitManager);
		}
	}

	private void Consume(float dt, ConduitFlow conduit_mgr)
	{
		this.IsSatisfied = false;
		this.consumedLastTick = false;
		this.utilityCell = this.GetInputCell(conduit_mgr.conduitType);
		if (!this.IsConnected)
		{
			return;
		}
		ConduitFlow.ConduitContents contents = conduit_mgr.GetContents(this.utilityCell);
		if (contents.mass <= 0f)
		{
			return;
		}
		this.IsSatisfied = true;
		if (!this.alwaysConsume && !this.operational.MeetsRequirements(this.OperatingRequirement))
		{
			return;
		}
		float num = this.ConsumptionRate * dt;
		num = Mathf.Min(num, this.space_remaining_kg);
		Element element = ElementLoader.FindElementByHash(contents.element);
		if (contents.element != this.lastConsumedElement)
		{
			DiscoveredResources.Instance.Discover(element.tag, element.materialCategory);
		}
		float num2 = 0f;
		if (num > 0f)
		{
			ConduitFlow.ConduitContents conduitContents = conduit_mgr.RemoveElement(this.utilityCell, num);
			num2 = conduitContents.mass;
			this.lastConsumedElement = conduitContents.element;
		}
		bool flag = element.HasTag(this.capacityTag);
		if (num2 > 0f && this.capacityTag != GameTags.Any && !flag)
		{
			base.BoxingTrigger<BuildingHP.DamageSourceInfo>(-794517298, new BuildingHP.DamageSourceInfo
			{
				damage = 1,
				source = BUILDINGS.DAMAGESOURCES.BAD_INPUT_ELEMENT,
				popString = UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.WRONG_ELEMENT
			});
		}
		if (flag || this.wrongElementResult == EntityConduitConsumer.WrongElementResult.Store || contents.element == SimHashes.Vacuum || this.capacityTag == GameTags.Any)
		{
			if (num2 > 0f)
			{
				this.consumedLastTick = true;
				int num3 = (int)((float)contents.diseaseCount * (num2 / contents.mass));
				Element element2 = ElementLoader.FindElementByHash(contents.element);
				ConduitType conduitType = this.conduitType;
				if (conduitType != ConduitType.Gas)
				{
					if (conduitType == ConduitType.Liquid)
					{
						if (element2.IsLiquid)
						{
							this.storage.AddLiquid(contents.element, num2, contents.temperature, contents.diseaseIdx, num3, this.keepZeroMassObject, false);
							return;
						}
						global::Debug.LogWarning("Liquid conduit consumer consuming non liquid: " + element2.id.ToString());
						return;
					}
				}
				else
				{
					if (element2.IsGas)
					{
						this.storage.AddGasChunk(contents.element, num2, contents.temperature, contents.diseaseIdx, num3, this.keepZeroMassObject, false);
						return;
					}
					global::Debug.LogWarning("Gas conduit consumer consuming non gas: " + element2.id.ToString());
					return;
				}
			}
		}
		else if (num2 > 0f)
		{
			this.consumedLastTick = true;
			if (this.wrongElementResult == EntityConduitConsumer.WrongElementResult.Dump)
			{
				int num4 = (int)((float)contents.diseaseCount * (num2 / contents.mass));
				SimMessages.AddRemoveSubstance(Grid.PosToCell(base.transform.GetPosition()), contents.element, CellEventLogger.Instance.ConduitConsumerWrongElement, num2, contents.temperature, contents.diseaseIdx, num4, true, -1);
			}
		}
	}

	private FlowUtilityNetwork.NetworkItem endpoint;

	[SerializeField]
	public ConduitType conduitType;

	[SerializeField]
	public bool ignoreMinMassCheck;

	[SerializeField]
	public Tag capacityTag = GameTags.Any;

	[SerializeField]
	public float capacityKG = float.PositiveInfinity;

	[SerializeField]
	public bool forceAlwaysSatisfied;

	[SerializeField]
	public bool alwaysConsume;

	[SerializeField]
	public bool keepZeroMassObject = true;

	[SerializeField]
	public bool isOn = true;

	[NonSerialized]
	public bool isConsuming = true;

	[NonSerialized]
	public bool consumedLastTick = true;

	[MyCmpReq]
	public Operational operational;

	[MyCmpReq]
	private OccupyArea occupyArea;

	[MyCmpReq]
	private EntityCellVisualizer cellVisualizer;

	public Operational.State OperatingRequirement;

	[MyCmpGet]
	public Storage storage;

	public CellOffset offset;

	private int utilityCell = -1;

	public float consumptionRate = float.PositiveInfinity;

	public SimHashes lastConsumedElement = SimHashes.Vacuum;

	private HandleVector<int>.Handle partitionerEntry;

	private bool satisfied;

	public EntityConduitConsumer.WrongElementResult wrongElementResult;

	public enum WrongElementResult
	{
		Destroy,
		Dump,
		Store
	}
}
