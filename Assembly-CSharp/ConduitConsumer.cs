using System;
using STRINGS;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/ConduitConsumer")]
public class ConduitConsumer : KMonoBehaviour, IConduitConsumer
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
			return Grid.Objects[this.utilityCell, (this.conduitType == ConduitType.Gas) ? 12 : 16] != null && this.m_buildingComplete != null;
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

	protected virtual int GetInputCell(ConduitType inputConduitType)
	{
		if (this.useSecondaryInput)
		{
			ISecondaryInput[] components = base.GetComponents<ISecondaryInput>();
			foreach (ISecondaryInput secondaryInput in components)
			{
				if (secondaryInput.HasSecondaryConduitType(inputConduitType))
				{
					return Grid.OffsetCell(this.building.NaturalBuildingCell(), secondaryInput.GetSecondaryConduitOffset(inputConduitType));
				}
			}
			global::Debug.LogWarning("No secondaryInput of type was found");
			return Grid.OffsetCell(this.building.NaturalBuildingCell(), components[0].GetSecondaryConduitOffset(inputConduitType));
		}
		return this.building.GetUtilityInputCell();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		GameScheduler.Instance.Schedule("PlumbingTutorial", 2f, delegate(object obj)
		{
			Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Plumbing, true);
		}, null, null);
		ConduitFlow conduitManager = this.GetConduitManager();
		this.utilityCell = this.GetInputCell(conduitManager.conduitType);
		ScenePartitionerLayer scenePartitionerLayer = GameScenePartitioner.Instance.objectLayers[(this.conduitType == ConduitType.Gas) ? 12 : 16];
		this.partitionerEntry = GameScenePartitioner.Instance.Add("ConduitConsumer.OnSpawn", base.gameObject, this.utilityCell, scenePartitionerLayer, new Action<object>(this.OnConduitConnectionChanged));
		this.GetConduitManager().AddConduitUpdater(new Action<float>(this.ConduitUpdate), ConduitFlowPriority.Default);
		this.OnConduitConnectionChanged(null);
	}

	protected override void OnCleanUp()
	{
		this.GetConduitManager().RemoveConduitUpdater(new Action<float>(this.ConduitUpdate));
		GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
		base.OnCleanUp();
	}

	private void OnConduitConnectionChanged(object data)
	{
		base.Trigger(-2094018600, this.IsConnected);
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
		if (this.building.Def.CanMove)
		{
			this.utilityCell = this.GetInputCell(conduit_mgr.conduitType);
		}
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
			base.Trigger(-794517298, new BuildingHP.DamageSourceInfo
			{
				damage = 1,
				source = BUILDINGS.DAMAGESOURCES.BAD_INPUT_ELEMENT,
				popString = UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.WRONG_ELEMENT
			});
		}
		if (flag || this.wrongElementResult == ConduitConsumer.WrongElementResult.Store || contents.element == SimHashes.Vacuum || this.capacityTag == GameTags.Any)
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
			if (this.wrongElementResult == ConduitConsumer.WrongElementResult.Dump)
			{
				int num4 = (int)((float)contents.diseaseCount * (num2 / contents.mass));
				SimMessages.AddRemoveSubstance(Grid.PosToCell(base.transform.GetPosition()), contents.element, CellEventLogger.Instance.ConduitConsumerWrongElement, num2, contents.temperature, contents.diseaseIdx, num4, true, -1);
			}
		}
	}

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
	public bool useSecondaryInput;

	[SerializeField]
	public bool isOn = true;

	[NonSerialized]
	public bool isConsuming = true;

	[NonSerialized]
	public bool consumedLastTick = true;

	[MyCmpReq]
	public Operational operational;

	[MyCmpReq]
	protected Building building;

	public Operational.State OperatingRequirement;

	public ISecondaryInput targetSecondaryInput;

	[MyCmpGet]
	public Storage storage;

	[MyCmpGet]
	private BuildingComplete m_buildingComplete;

	private int utilityCell = -1;

	public float consumptionRate = float.PositiveInfinity;

	public SimHashes lastConsumedElement = SimHashes.Vacuum;

	private HandleVector<int>.Handle partitionerEntry;

	private bool satisfied;

	public ConduitConsumer.WrongElementResult wrongElementResult;

	public enum WrongElementResult
	{
		Destroy,
		Dump,
		Store
	}
}
