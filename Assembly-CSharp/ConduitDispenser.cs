using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class ConduitDispenser : KMonoBehaviour, ISaveLoadable
{
	public ConduitType TypeOfConduit
	{
		get
		{
			return this.conduitType;
		}
	}

	public ConduitFlow.ConduitContents ConduitContents
	{
		get
		{
			return this.GetConduitManager().GetContents(this.utilityCell);
		}
	}

	public void SetConduitData(ConduitType type)
	{
		this.conduitType = type;
	}

	public ConduitFlow GetConduitManager()
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

	private void OnConduitConnectionChanged(object data)
	{
		base.Trigger(-2094018600, this.IsConnected);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.utilityCell = base.GetComponent<Building>().GetUtilityOutputCell();
		ScenePartitionerLayer scenePartitionerLayer = GameScenePartitioner.Instance.objectLayers[(this.conduitType != ConduitType.Gas) ? 16 : 12];
		this.partitionerEntry = GameScenePartitioner.Instance.Add("ConduitConsumer.OnSpawn", base.gameObject, this.utilityCell, scenePartitionerLayer, new Action<object>(this.OnConduitConnectionChanged));
		this.GetConduitManager().AddConduitUpdater(new Action<float>(this.ConduitUpdate), ConduitFlowPriority.Last);
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

	private void ConduitUpdate(float dt)
	{
		this.operational.SetFlag(ConduitDispenser.outputConduitFlag, this.IsConnected);
		if (this.operational.IsOperational || this.alwaysDispense)
		{
			PrimaryElement primaryElement = this.FindSuitableElement();
			if (primaryElement != null)
			{
				primaryElement.KeepZeroMassObject = true;
				ConduitFlow conduitManager = this.GetConduitManager();
				float num = conduitManager.AddElement(this.utilityCell, primaryElement.ElementID, primaryElement.Mass, primaryElement.Temperature, primaryElement.DiseaseIdx, primaryElement.DiseaseCount);
				if (num > 0f)
				{
					float num2 = num / primaryElement.Mass;
					int num3 = (int)(num2 * (float)primaryElement.DiseaseCount);
					primaryElement.ModifyDiseaseCount(-num3, "ConduitDispenser.ConduitUpdate");
					primaryElement.Mass -= num;
					base.Trigger(-1697596308, primaryElement.gameObject);
				}
			}
		}
	}

	private PrimaryElement FindSuitableElement()
	{
		List<GameObject> items = this.storage.items;
		int count = items.Count;
		for (int i = 0; i < count; i++)
		{
			int num = (i + this.elementOutputOffset) % count;
			PrimaryElement component = items[num].GetComponent<PrimaryElement>();
			if (component != null && component.Mass > 0f && ((this.conduitType != ConduitType.Liquid) ? component.Element.IsGas : component.Element.IsLiquid) && (this.elementFilter == null || this.elementFilter.Length == 0 || (!this.invertElementFilter && this.IsFilteredElement(component.ElementID)) || (this.invertElementFilter && !this.IsFilteredElement(component.ElementID))))
			{
				this.elementOutputOffset = (this.elementOutputOffset + 1) % count;
				return component;
			}
		}
		return null;
	}

	private bool IsFilteredElement(SimHashes element)
	{
		return Array.IndexOf<SimHashes>(this.elementFilter, element) >= 0;
	}

	public bool IsConnected
	{
		get
		{
			GameObject gameObject = Grid.Objects[this.utilityCell, (this.conduitType != ConduitType.Gas) ? 16 : 12];
			return gameObject != null && gameObject.GetComponent<BuildingComplete>() != null;
		}
	}

	[SerializeField]
	public ConduitType conduitType;

	[SerializeField]
	public SimHashes[] elementFilter;

	[SerializeField]
	public bool invertElementFilter;

	[SerializeField]
	public bool alwaysDispense;

	private static Operational.Flag outputConduitFlag = new Operational.Flag("output_conduit", Operational.Flag.Type.Functional);

	[MyCmpReq]
	private Operational operational;

	[MyCmpReq]
	public Storage storage;

	private GameScenePartitionerEntry partitionerEntry;

	private int utilityCell = -1;

	private int elementOutputOffset;
}
