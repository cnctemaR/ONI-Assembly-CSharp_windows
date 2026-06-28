using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class ConduitConsumer : KMonoBehaviour, ISaveLoadableJson
{
	public bool IsConnected
	{
		get
		{
			GameObject gameObject = Grid.Objects[this.utilityCell, (this.conduitType != ConduitType.Gas) ? 12 : 10];
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
			this.satisfied = value;
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
		int mask = GameScenePartitioner.Instance.objectLayerMasks[(this.conduitType != ConduitType.Gas) ? 12 : 10].mask;
		this.partitionerEntry = GameScenePartitioner.Instance.Add("ConduitConsumer.OnSpawn", base.gameObject, this.utilityCell, mask, new Action<object>(this.OnConduitConnectionChanged));
		this.GetConduitManager().AddConduitUpdater(new Action<float>(this.ConduitUpdate), 0);
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
		this.Trigger(-2094018600, this.IsConnected);
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
				if (this.operational.IsOperational)
				{
					float num = ((this.capacityElement == SimHashes.Void) ? this.storage.MassStored() : this.storage.GetMassAvailable(this.capacityElement));
					float num2 = Mathf.Min(this.storage.RemainingCapacity(), this.capacityKG - num);
					float num3 = this.ConsumptionRate * dt;
					num3 = Mathf.Min(num3, num2);
					float num4 = ((num3 <= 0f) ? 0f : conduit_mgr.RemoveElement(this.utilityCell, num3));
					if (contents.element == this.capacityElement || contents.element == SimHashes.Vacuum || this.capacityElement == SimHashes.Void)
					{
						if (num4 > 0f)
						{
							Element element = ElementLoader.FindElementByHash(contents.element);
							ConduitType conduitType = this.conduitType;
							if (conduitType != ConduitType.Gas)
							{
								if (conduitType == ConduitType.Liquid)
								{
									if (element.IsLiquid)
									{
										this.storage.AddLiquid(contents.element, num4, contents.temperature, true);
									}
									else
									{
										Debug.LogWarning("Liquid conduit consumer consuming non liquid: " + element.id.ToString());
									}
								}
							}
							else if (element.IsGas)
							{
								this.storage.AddGasChunk(contents.element, num4, contents.temperature, true);
							}
							else
							{
								Debug.LogWarning("Gas conduit consumer consuming non gas: " + element.id.ToString());
							}
						}
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
	public SimHashes capacityElement = SimHashes.Void;

	[SerializeField]
	public float capacityKG = float.PositiveInfinity;

	[NonSerialized]
	public bool isConsuming = true;

	[MyCmpReq]
	private Operational operational;

	[MyCmpReq]
	private Building building;

	[MyCmpGet]
	private Upgradable upgradable;

	[MyCmpGet]
	private Storage storage;

	private int utilityCell = -1;

	public float consumptionRate = float.PositiveInfinity;

	public static readonly Operational.Flag elementRequirementFlag = new Operational.Flag("elementRequired", Operational.Flag.Type.Requirement);

	private GameScenePartitionerEntry partitionerEntry;

	private bool satisfied;
}
