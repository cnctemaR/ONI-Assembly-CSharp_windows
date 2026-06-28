using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class PlantablePlot : SingleEntityReceptacle, ISaveLoadableJson, IEffectDescriptor
{
	public Crop crop
	{
		get
		{
			return this.cropRef.Get();
		}
		set
		{
			this.cropRef.Set(value);
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.cropRef = new Ref<Crop>();
		this.destroyEntityOnDeposit = true;
		base.stringKey_Place = "STRINGS.UI.PLACEINRECEPTACLE";
		base.stringKey_CancelPlace = "STRINGS.UI.CANCELPLACEINRECEPTACLE";
		base.stringKey_Remove = "STRINGS.UI.USERMENUACTIONS.UPROOT.NAME";
		base.stringKey_CancelRemove = "STRINGS.UI.CANCELREMOVALFROMRECEPTACLE";
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.crop != null)
		{
			this.RegisterWithPlant(this.crop.gameObject);
		}
		this.autoReplaceEntity = true;
		if (base.Occupant != null && base.Occupant.GetComponent<Uprootable>().IsMarkedForUproot)
		{
			this.autoReplaceEntity = false;
		}
		Components.PlantablePlots.Add(this);
	}

	public void SetPlantedModifier(AttributeModifier modifier)
	{
		this.plantedModifier = modifier;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if (base.occupyingObject)
		{
			base.occupyingObject.Trigger(-216549700, null);
		}
		Components.PlantablePlots.Remove(this);
	}

	public override GameObject SpawnOccupyingObject(GameObject depositedEntity)
	{
		PlantableSeed component = depositedEntity.GetComponent<PlantableSeed>();
		if (component == null)
		{
			Debug.LogError("Planted seed " + depositedEntity.gameObject.name + " is missing PlantableSeed component");
			return null;
		}
		Vector3 vector = Grid.CellToPosCBC(Grid.PosToCell(this), Grid.SceneLayer.BuildingBack);
		GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab(component.PlantID), vector, Grid.SceneLayer.BuildingBack, SceneOrganizer.Instance.GetFolder(Folder.Entities), null, 0);
		gameObject.SetActive(true);
		Crop component2 = gameObject.GetComponent<Crop>();
		if (component2)
		{
			this.crop = component2;
			Growing component3 = component2.GetComponent<Growing>();
			component3.OnReplant();
		}
		else if (this.cropRef != null)
		{
			this.cropRef.Set(null);
		}
		this.RegisterWithPlant(gameObject);
		UprootedMonitor component4 = gameObject.GetComponent<UprootedMonitor>();
		if (component4)
		{
			component4.canBeUprooted = false;
		}
		this.autoReplaceEntity = true;
		return gameObject;
	}

	private void RegisterWithPlant(GameObject plant)
	{
		if (this.plantedModifier != null)
		{
			Attributes attributes = plant.GetAttributes();
			if (attributes != null)
			{
				attributes.Add("PlanterEffect", this.plantedModifier);
			}
		}
		Crop component = plant.GetComponent<Crop>();
		if (component != null)
		{
			component.PlanterStorage = this.storage;
		}
		FertilizationMonitor.Instance smi = plant.GetSMI<FertilizationMonitor.Instance>();
		if (smi != null)
		{
			smi.SetStorage(this.storage);
		}
	}

	protected override void SubscribeToOccupant()
	{
		base.SubscribeToOccupant();
		if (base.occupyingObject != null)
		{
			base.Subscribe(base.occupyingObject, -216549700, new EventSystem.EventHandler(this.OnOccupantUprooted));
		}
	}

	protected override void UnsubscribeFromOccupant()
	{
		base.UnsubscribeFromOccupant();
		if (base.occupyingObject != null)
		{
			base.Unsubscribe(base.occupyingObject, -216549700, new EventSystem.EventHandler(this.OnOccupantUprooted));
		}
	}

	private void OnOccupantUprooted(object data)
	{
		this.autoReplaceEntity = false;
		this.requestedEntityTag = GameTags.Empty;
	}

	public override void OrderRemoveOccupant()
	{
		Uprootable component = base.Occupant.GetComponent<Uprootable>();
		if (component == null)
		{
			return;
		}
		component.MarkForUproot();
	}

	public int DescriptionOrder { get; set; }

	public List<Descriptor> GetRequirementDescriptions(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		Descriptor descriptor = default(Descriptor);
		descriptor.SetupDescriptor(string.Format(UI.LISTENTRYSTRINGNOLINEBREAK, UI.BUILDINGEFFECTS.REQUIRESSEED), UI.BUILDINGEFFECTS.TOOLTIPS.REQUIRESSEED);
		list.Add(descriptor);
		return list;
	}

	public List<Descriptor> GetEffectDescriptions(BuildingDef def)
	{
		return null;
	}

	[Serialize]
	private Ref<Crop> cropRef;

	private AttributeModifier plantedModifier;
}
