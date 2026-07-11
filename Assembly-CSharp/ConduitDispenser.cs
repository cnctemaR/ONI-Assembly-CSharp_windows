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
		GameScheduler.Instance.Schedule("PlumbingTutorial", 2f, delegate(object obj)
		{
			Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Plumbing, true);
		}, null, null);
		this.utilityCell = base.GetComponent<Building>().GetUtilityOutputCell();
		ScenePartitionerLayer scenePartitionerLayer = GameScenePartitioner.Instance.objectLayers[(this.conduitType == ConduitType.Gas) ? 12 : 16];
		this.partitionerEntry = GameScenePartitioner.Instance.Add("ConduitConsumer.OnSpawn", base.gameObject, this.utilityCell, scenePartitionerLayer, new Action<object>(this.OnConduitConnectionChanged));
		this.GetConduitManager().AddConduitUpdater(new Action<float>(this.ConduitUpdate), ConduitFlowPriority.Dispense);
		this.OnConduitConnectionChanged(null);
	}

	protected override void OnCleanUp()
	{
		this.GetConduitManager().RemoveConduitUpdater(new Action<float>(this.ConduitUpdate));
		GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
		base.OnCleanUp();
	}

	public void SetOnState(bool onState)
	{
		this.isOn = onState;
	}

	private void ConduitUpdate(float dt)
	{
		this.operational.SetFlag(ConduitDispenser.outputConduitFlag, this.IsConnected);
		this.blocked = false;
		if (this.isOn)
		{
			this.Dispense(dt);
		}
	}

	private void Dispense(float dt)
	{
		if (this.operational.IsOperational || this.alwaysDispense)
		{
			PrimaryElement primaryElement = this.FindSuitableElement();
			if (primaryElement != null)
			{
				primaryElement.KeepZeroMassObject = true;
				float num = this.GetConduitManager().AddElement(this.utilityCell, primaryElement.ElementID, primaryElement.Mass, primaryElement.Temperature, primaryElement.DiseaseIdx, primaryElement.DiseaseCount);
				if (num > 0f)
				{
					int num2 = (int)(num / primaryElement.Mass * (float)primaryElement.DiseaseCount);
					primaryElement.ModifyDiseaseCount(-num2, "ConduitDispenser.ConduitUpdate");
					primaryElement.Mass -= num;
					base.Trigger(-1697596308, primaryElement.gameObject);
					return;
				}
				this.blocked = true;
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
			if (component != null && component.Mass > 0f && ((this.conduitType == ConduitType.Liquid) ? component.Element.IsLiquid : component.Element.IsGas) && (this.elementFilter == null || this.elementFilter.Length == 0 || (!this.invertElementFilter && this.IsFilteredElement(component.ElementID)) || (this.invertElementFilter && !this.IsFilteredElement(component.ElementID))))
			{
				this.elementOutputOffset = (this.elementOutputOffset + 1) % count;
				return component;
			}
		}
		return null;
	}

	private bool IsFilteredElement(SimHashes element)
	{
		for (int num = 0; num != this.elementFilter.Length; num++)
		{
			if (this.elementFilter[num] == element)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsConnected
	{
		get
		{
			GameObject gameObject = Grid.Objects[this.utilityCell, (this.conduitType == ConduitType.Gas) ? 12 : 16];
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

	[SerializeField]
	public bool isOn = true;

	[SerializeField]
	public bool blocked;

	private static readonly Operational.Flag outputConduitFlag = new Operational.Flag("output_conduit", Operational.Flag.Type.Functional);

	[MyCmpReq]
	private Operational operational;

	[MyCmpReq]
	public Storage storage;

	private HandleVector<int>.Handle partitionerEntry;

	private int utilityCell = -1;

	private int elementOutputOffset;
}
