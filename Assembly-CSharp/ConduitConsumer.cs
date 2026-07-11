using System;
using STRINGS;
using UnityEngine;

[SkipSaveFileSerialization]
public class ConduitConsumer : KMonoBehaviour
{
	public bool IsConnected
	{
		get
		{
			GameObject gameObject = Grid.Objects[this.utilityCell, (this.conduitType == ConduitType.Gas) ? 12 : 16];
			return gameObject != null && gameObject.GetComponent<BuildingComplete>() != null;
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
			int inputCell = this.GetInputCell();
			return this.GetConduitManager().GetContents(inputCell).mass;
		}
	}

	private int GetInputCell()
	{
		if (this.useSecondaryInput)
		{
			ISecondaryInput component = base.GetComponent<ISecondaryInput>();
			return Grid.OffsetCell(this.building.NaturalBuildingCell(), component.GetSecondaryConduitOffset());
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
		this.utilityCell = this.GetInputCell();
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
		if (this.building.Def.CanMove)
		{
			this.utilityCell = this.GetInputCell();
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
		if (!this.alwaysConsume && !this.operational.IsOperational)
		{
			return;
		}
		float num = this.ConsumptionRate * dt;
		num = Mathf.Min(num, this.space_remaining_kg);
		float num2 = 0f;
		if (num > 0f)
		{
			ConduitFlow.ConduitContents conduitContents = conduit_mgr.RemoveElement(this.utilityCell, num);
			num2 = conduitContents.mass;
			this.lastConsumedElement = conduitContents.element;
		}
		bool flag = ElementLoader.FindElementByHash(contents.element).HasTag(this.capacityTag);
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
				int num3 = (int)((float)contents.diseaseCount * (num2 / contents.mass));
				Element element = ElementLoader.FindElementByHash(contents.element);
				ConduitType conduitType = this.conduitType;
				if (conduitType != ConduitType.Gas)
				{
					if (conduitType == ConduitType.Liquid)
					{
						if (element.IsLiquid)
						{
							this.storage.AddLiquid(contents.element, num2, contents.temperature, contents.diseaseIdx, num3, this.keepZeroMassObject, false);
							return;
						}
						global::Debug.LogWarning("Liquid conduit consumer consuming non liquid: " + element.id.ToString());
						return;
					}
				}
				else
				{
					if (element.IsGas)
					{
						this.storage.AddGasChunk(contents.element, num2, contents.temperature, contents.diseaseIdx, num3, this.keepZeroMassObject, false);
						return;
					}
					global::Debug.LogWarning("Gas conduit consumer consuming non gas: " + element.id.ToString());
					return;
				}
			}
		}
		else if (num2 > 0f && this.wrongElementResult == ConduitConsumer.WrongElementResult.Dump)
		{
			int num4 = (int)((float)contents.diseaseCount * (num2 / contents.mass));
			SimMessages.AddRemoveSubstance(Grid.PosToCell(base.transform.GetPosition()), contents.element, CellEventLogger.Instance.ConduitConsumerWrongElement, num2, contents.temperature, contents.diseaseIdx, num4, true, -1);
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

	[MyCmpReq]
	public Operational operational;

	[MyCmpReq]
	private Building building;

	[MyCmpGet]
	public Storage storage;

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
