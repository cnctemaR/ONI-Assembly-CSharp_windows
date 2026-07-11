using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Crop")]
public class Crop : KMonoBehaviour, IGameObjectEffectDescriptor
{
	public string cropId
	{
		get
		{
			return this.cropVal.cropId;
		}
	}

	public Storage PlanterStorage
	{
		get
		{
			return this.planterStorage;
		}
		set
		{
			this.planterStorage = value;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Components.Crops.Add(this);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.Subscribe<Crop>(1272413801, Crop.OnHarvestDelegate);
		base.Subscribe<Crop>(-1736624145, Crop.OnSeedDroppedDelegate);
	}

	public void Configure(Crop.CropVal cropval)
	{
		this.cropVal = cropval;
	}

	public bool CanGrow()
	{
		return this.cropVal.renewable;
	}

	public void SpawnFruit(object callbackParam)
	{
		if (this == null)
		{
			return;
		}
		Crop.CropVal cropVal = this.cropVal;
		if (!string.IsNullOrEmpty(cropVal.cropId))
		{
			GameObject gameObject = Scenario.SpawnPrefab(Grid.PosToCell(base.gameObject), 0, 0, cropVal.cropId, Grid.SceneLayer.Ore);
			if (gameObject != null)
			{
				float num = 0.75f;
				gameObject.transform.SetPosition(gameObject.transform.GetPosition() + new Vector3(0f, num, 0f));
				gameObject.SetActive(true);
				PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
				component.Units = (float)cropVal.numProduced;
				component.Temperature = base.gameObject.GetComponent<PrimaryElement>().Temperature;
				Edible component2 = gameObject.GetComponent<Edible>();
				if (component2)
				{
					ReportManager.Instance.ReportValue(ReportManager.ReportType.CaloriesCreated, component2.Calories, StringFormatter.Replace(UI.ENDOFDAYREPORT.NOTES.HARVESTED, "{0}", component2.GetProperName()), UI.ENDOFDAYREPORT.NOTES.HARVESTED_CONTEXT);
				}
			}
			else
			{
				DebugUtil.LogErrorArgs(base.gameObject, new object[] { "tried to spawn an invalid crop prefab:", cropVal.cropId });
			}
			base.Trigger(-1072826864, null);
		}
	}

	protected override void OnCleanUp()
	{
		Components.Crops.Remove(this);
		base.OnCleanUp();
	}

	private void OnHarvest(object obj)
	{
	}

	public void OnSeedDropped(object data)
	{
	}

	public List<Descriptor> RequirementDescriptors(GameObject go)
	{
		return new List<Descriptor>();
	}

	public List<Descriptor> InformationDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		Tag tag = new Tag(this.cropVal.cropId);
		GameObject prefab = Assets.GetPrefab(tag);
		Edible component = prefab.GetComponent<Edible>();
		float num = 0f;
		string text = "";
		if (component != null)
		{
			num = component.FoodInfo.CaloriesPerUnit;
		}
		float num2 = num * (float)this.cropVal.numProduced;
		InfoDescription component2 = prefab.GetComponent<InfoDescription>();
		if (component2)
		{
			text = component2.description;
		}
		string text2;
		if (GameTags.DisplayAsCalories.Contains(tag))
		{
			text2 = GameUtil.GetFormattedCalories(num2, GameUtil.TimeSlice.None, true);
		}
		else if (GameTags.DisplayAsUnits.Contains(tag))
		{
			text2 = GameUtil.GetFormattedUnits((float)this.cropVal.numProduced, GameUtil.TimeSlice.None, false);
		}
		else
		{
			text2 = GameUtil.GetFormattedMass((float)this.cropVal.numProduced, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}");
		}
		LocString yield = UI.UISIDESCREENS.PLANTERSIDESCREEN.YIELD;
		Descriptor descriptor = new Descriptor(string.Format(yield, prefab.GetProperName(), text2), string.Format(UI.UISIDESCREENS.PLANTERSIDESCREEN.TOOLTIPS.YIELD, text, GameUtil.GetFormattedCalories(num, GameUtil.TimeSlice.None, true), GameUtil.GetFormattedCalories(num2, GameUtil.TimeSlice.None, true)), Descriptor.DescriptorType.Effect, false);
		list.Add(descriptor);
		return list;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		foreach (Descriptor descriptor in this.RequirementDescriptors(go))
		{
			list.Add(descriptor);
		}
		foreach (Descriptor descriptor2 in this.InformationDescriptors(go))
		{
			list.Add(descriptor2);
		}
		return list;
	}

	[MyCmpReq]
	private KSelectable selectable;

	public Crop.CropVal cropVal;

	public string domesticatedDesc = "";

	private Storage planterStorage;

	private static readonly EventSystem.IntraObjectHandler<Crop> OnHarvestDelegate = new EventSystem.IntraObjectHandler<Crop>(delegate(Crop component, object data)
	{
		component.OnHarvest(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Crop> OnSeedDroppedDelegate = new EventSystem.IntraObjectHandler<Crop>(delegate(Crop component, object data)
	{
		component.OnSeedDropped(data);
	});

	[Serializable]
	public struct CropVal
	{
		public CropVal(string crop_id, float crop_duration, int num_produced = 1, bool renewable = true)
		{
			this.cropId = crop_id;
			this.cropDuration = crop_duration;
			this.numProduced = num_produced;
			this.renewable = renewable;
		}

		public string cropId;

		public float cropDuration;

		public int numProduced;

		public bool renewable;
	}
}
