using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class PlantablePlot : SingleEntityReceptacle, ISaveLoadable, IGameObjectEffectDescriptor, IEffectDescriptor
{
	public KPrefabID plant
	{
		get
		{
			return this.plantRef.Get();
		}
		set
		{
			this.plantRef.Set(value);
		}
	}

	public bool ValidPlant
	{
		get
		{
			return this.plantPreview == null || this.plantPreview.Valid;
		}
	}

	public bool AcceptsFertilizer
	{
		get
		{
			return this.accepts_fertilizer;
		}
	}

	public bool AcceptsIrrigation
	{
		get
		{
			return this.accepts_irrigation;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.cropRef = new Ref<Growing>();
		this.plantRef = new Ref<KPrefabID>();
		this.destroyEntityOnDeposit = true;
		this.Subscribe(-905833192, new Action<object>(this.OnCopySettings));
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		PlantablePlot component = gameObject.GetComponent<PlantablePlot>();
		if (component != null)
		{
			if (base.occupyingObject == null && this.requestedEntityTag != component.requestedEntityTag)
			{
				base.CancelActiveRequest();
				base.CreateOrder(component.requestedEntityTag);
			}
			if (base.occupyingObject != null)
			{
				Prioritizable component2 = base.GetComponent<Prioritizable>();
				if (component2 != null)
				{
					Prioritizable component3 = base.occupyingObject.GetComponent<Prioritizable>();
					if (component3 != null)
					{
						component3.SetMasterPriority(component2.GetMasterPriority());
					}
				}
			}
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.cropRef.Get() != null && this.plantRef.Get() == null)
		{
			this.plantRef.Set(this.cropRef.Get<KPrefabID>());
		}
		if (this.plant != null)
		{
			this.RegisterWithPlant(this.plant.gameObject);
		}
		this.autoReplaceEntity = true;
		if (base.Occupant != null && base.Occupant.GetComponent<Uprootable>().IsMarkedForUproot)
		{
			this.autoReplaceEntity = false;
		}
		Components.PlantablePlots.Add(this);
	}

	public void SetFertilizationFlags(bool fertilizer, bool irrigation)
	{
		this.accepts_fertilizer = fertilizer;
		this.accepts_irrigation = irrigation;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if (this.plantPreview != null)
		{
			global::UnityEngine.Object.Destroy(this.plantPreview.gameObject);
		}
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
			global::Debug.LogError("Planted seed " + depositedEntity.gameObject.name + " is missing PlantableSeed component", null);
			return null;
		}
		Vector3 vector = Grid.CellToPosCBC(Grid.PosToCell(this), Grid.SceneLayer.BuildingBack);
		GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab(component.PlantID), vector, Grid.SceneLayer.BuildingBack, SceneOrganizer.Instance.GetFolder(Folder.Entities), null, 0);
		gameObject.SetActive(true);
		KPrefabID component2 = gameObject.GetComponent<KPrefabID>();
		this.plantRef.Set(component2);
		Crop component3 = gameObject.GetComponent<Crop>();
		if (component3 != null)
		{
			PlantableSeed component4 = component.GetComponent<PlantableSeed>();
			component3.SetTimesHarvested(component4.timesHarvested);
		}
		this.RegisterWithPlant(gameObject);
		UprootedMonitor component5 = gameObject.GetComponent<UprootedMonitor>();
		if (component5)
		{
			component5.canBeUprooted = false;
		}
		this.autoReplaceEntity = true;
		Prioritizable component6 = base.GetComponent<Prioritizable>();
		if (component6 != null)
		{
			Prioritizable component7 = gameObject.GetComponent<Prioritizable>();
			if (component7 != null)
			{
				component7.SetMasterPriority(component6.GetMasterPriority());
			}
		}
		return gameObject;
	}

	private void RegisterWithPlant(GameObject plant)
	{
		plant.Trigger(1309017699, this.storage);
	}

	protected override void SubscribeToOccupant()
	{
		base.SubscribeToOccupant();
		if (base.occupyingObject != null)
		{
			base.Subscribe(base.occupyingObject, -216549700, new Action<object>(this.OnOccupantUprooted));
		}
	}

	protected override void UnsubscribeFromOccupant()
	{
		base.UnsubscribeFromOccupant();
		if (base.occupyingObject != null)
		{
			base.Unsubscribe(base.occupyingObject, -216549700, new Action<object>(this.OnOccupantUprooted));
		}
	}

	private void OnOccupantUprooted(object data)
	{
		this.autoReplaceEntity = false;
		this.requestedEntityTag = Tag.Invalid;
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

	public override void SetPreview(Tag entityTag, bool solid = false)
	{
		PlantableSeed plantableSeed = null;
		if (entityTag.IsValid)
		{
			GameObject prefab = Assets.GetPrefab(entityTag);
			if (prefab == null)
			{
				Output.LogWarningWithObj(base.gameObject, new object[] { "Planter tried previewing a tag with no asset! If this was the 'Empty' tag, ignore it, that will go away in new save games. Otherwise... Eh? Tag was: ", entityTag });
				return;
			}
			plantableSeed = prefab.GetComponent<PlantableSeed>();
		}
		if (this.plantPreview != null)
		{
			KPrefabID component = this.plantPreview.GetComponent<KPrefabID>();
			if (plantableSeed != null && component != null && component.PrefabTag == plantableSeed.PreviewID)
			{
				return;
			}
			this.plantPreview.gameObject.Unsubscribe(-1820564715, new Action<object>(this.OnValidChanged));
			global::UnityEngine.Object.Destroy(this.plantPreview.gameObject);
		}
		if (plantableSeed != null)
		{
			GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab(plantableSeed.PreviewID), Grid.SceneLayer.Front, Folder.BuildingPreviews, null, 0);
			this.plantPreview = gameObject.GetComponent<PlantPreview>();
			gameObject.transform.position = Vector3.zero;
			gameObject.transform.SetParent(base.gameObject.transform, false);
			gameObject.transform.localPosition = Vector3.zero;
			if (this.rotatable != null)
			{
				if (plantableSeed.direction == SingleEntityReceptacle.ReceptacleDirection.Top)
				{
					gameObject.transform.localPosition = this.occupyingObjectRelativePosition;
				}
				else if (plantableSeed.direction == SingleEntityReceptacle.ReceptacleDirection.Side)
				{
					gameObject.transform.localPosition = Rotatable.GetRotatedOffset(this.occupyingObjectRelativePosition, Orientation.R90);
				}
				else
				{
					gameObject.transform.localPosition = Rotatable.GetRotatedOffset(this.occupyingObjectRelativePosition, Orientation.R180);
				}
			}
			else
			{
				gameObject.transform.localPosition = this.occupyingObjectRelativePosition;
			}
			gameObject.SetActive(true);
			gameObject.Subscribe(-1820564715, new Action<object>(this.OnValidChanged));
			if (solid)
			{
				this.plantPreview.SetSolid();
			}
		}
	}

	private void OnValidChanged(object obj)
	{
		this.Trigger(-1820564715, obj);
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		return this.GetDescriptors(def.BuildingComplete);
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (this.accepts_fertilizer)
		{
			Descriptor descriptor = default(Descriptor);
			descriptor.SetupDescriptor(UI.BUILDINGEFFECTS.ALLOWS_FERTILIZER, UI.BUILDINGEFFECTS.TOOLTIPS.ALLOWS_FERTILIZER, Descriptor.DescriptorType.Effect);
			list.Add(descriptor);
		}
		if (this.accepts_irrigation)
		{
			Descriptor descriptor2 = default(Descriptor);
			descriptor2.SetupDescriptor(UI.BUILDINGEFFECTS.ALLOWS_IRRIGATION, UI.BUILDINGEFFECTS.TOOLTIPS.ALLOWS_IRRIGATION, Descriptor.DescriptorType.Effect);
			list.Add(descriptor2);
		}
		return list;
	}

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	[Serialize]
	private Ref<Growing> cropRef;

	[Serialize]
	private Ref<KPrefabID> plantRef;

	private PlantPreview plantPreview;

	[SerializeField]
	private bool accepts_fertilizer;

	[SerializeField]
	private bool accepts_irrigation;
}
