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
			GameObject gameObject = Grid.Objects[this.utilityCell, (this.conduitType != ConduitType.Gas) ? 16 : 12];
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
				ConduitFlow conduitManager = this.GetConduitManager();
				flag = conduitManager.GetContents(this.utilityCell).mass > 0f;
			}
			return flag;
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
			int utilityInputCell = this.building.GetUtilityInputCell();
			ConduitFlow conduitManager = this.GetConduitManager();
			return conduitManager.GetContents(utilityInputCell).mass;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.utilityCell = this.building.GetUtilityInputCell();
		ScenePartitionerLayer scenePartitionerLayer = GameScenePartitioner.Instance.objectLayers[(this.conduitType != ConduitType.Gas) ? 16 : 12];
		this.partitionerEntry = GameScenePartitioner.Instance.Add("ConduitConsumer.OnSpawn", base.gameObject, this.utilityCell, scenePartitionerLayer, new Action<object>(this.OnConduitConnectionChanged));
		this.GetConduitManager().AddConduitUpdater(new Action<float>(this.ConduitUpdate), ConduitFlow.Priority.Default);
		this.OnConduitConnectionChanged(null);
	}

	protected override void OnCleanUp()
	{
		this.GetConduitManager().RemoveConduitUpdater(new Action<float>(this.ConduitUpdate));
		if (this.partitionerEntry != null)
		{
			this.partitionerEntry.Release();
		}
		base.OnCleanUp();
	}

	private void OnConduitConnectionChanged(object data)
	{
		base.Trigger(-2094018600, this.IsConnected);
	}

	private void ConduitUpdate(float dt)
	{
		if (this.isConsuming)
		{
			ConduitFlow conduitManager = this.GetConduitManager();
			this.Consume(dt, conduitManager);
		}
	}

	private void Consume(float dt, ConduitFlow conduit_mgr)
	{
		if (this.IsConnected)
		{
			ConduitFlow.ConduitContents contents = conduit_mgr.GetContents(this.utilityCell);
			if (contents.mass > 0f)
			{
				this.IsSatisfied = true;
				if (this.alwaysConsume || this.operational.IsOperational)
				{
					float num = ((!(this.capacityTag != GameTags.Any)) ? this.storage.MassStored() : this.storage.GetMassAvailable(this.capacityTag));
					float num2 = Mathf.Min(this.storage.RemainingCapacity(), this.capacityKG - num);
					float num3 = this.ConsumptionRate * dt;
					num3 = Mathf.Min(num3, num2);
					float num4 = 0f;
					if (num3 > 0f)
					{
						num4 = conduit_mgr.RemoveElement(this.utilityCell, num3).mass;
					}
					Element element = ElementLoader.FindElementByHash(contents.element);
					bool flag = element.HasTag(this.capacityTag);
					if (num4 > 0f && this.capacityTag != GameTags.Any && !flag)
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
						if (num4 > 0f)
						{
							int num5 = (int)((float)contents.diseaseCount * (num4 / contents.mass));
							Element element2 = ElementLoader.FindElementByHash(contents.element);
							ConduitType conduitType = this.conduitType;
							if (conduitType != ConduitType.Liquid)
							{
								if (conduitType == ConduitType.Gas)
								{
									if (element2.IsGas)
									{
										this.storage.AddGasChunk(contents.element, num4, contents.temperature, contents.diseaseIdx, num5, true, false);
									}
									else
									{
										global::Debug.LogWarning("Gas conduit consumer consuming non gas: " + element2.id.ToString(), null);
									}
								}
							}
							else if (element2.IsLiquid)
							{
								this.storage.AddLiquid(contents.element, num4, contents.temperature, contents.diseaseIdx, num5, true, false);
							}
							else
							{
								global::Debug.LogWarning("Liquid conduit consumer consuming non liquid: " + element2.id.ToString(), null);
							}
						}
					}
					else if (num4 > 0f && this.wrongElementResult == ConduitConsumer.WrongElementResult.Dump)
					{
						int num6 = (int)((float)contents.diseaseCount * (num4 / contents.mass));
						int num7 = Grid.PosToCell(base.transform.position);
						SimMessages.AddRemoveSubstance(num7, contents.element, CellEventLogger.Instance.ConduitConsumerWrongElement, num4, contents.temperature, contents.diseaseIdx, num6, -1);
					}
				}
			}
			else
			{
				this.IsSatisfied = false;
			}
		}
		else
		{
			this.IsSatisfied = false;
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

	[NonSerialized]
	public bool isConsuming = true;

	[MyCmpReq]
	private Operational operational;

	[MyCmpReq]
	private Building building;

	[MyCmpGet]
	private Upgradable upgradable;

	[MyCmpGet]
	public Storage storage;

	private int utilityCell = -1;

	public float consumptionRate = float.PositiveInfinity;

	public static readonly Operational.Flag elementRequirementFlag = new Operational.Flag("elementRequired", Operational.Flag.Type.Requirement);

	private GameScenePartitionerEntry partitionerEntry;

	private bool satisfied;

	public ConduitConsumer.WrongElementResult wrongElementResult;

	public enum WrongElementResult
	{
		Destroy,
		Dump,
		Store
	}
}
