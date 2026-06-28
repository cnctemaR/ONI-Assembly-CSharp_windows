using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

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
		this.Subscribe(1272413801, new Action<object>(this.OnHarvest));
		this.Subscribe(-1736624145, new Action<object>(this.OnSeedDropped));
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
		Crop.CropVal cropVal = this.cropVal;
		if (!string.IsNullOrEmpty(cropVal.cropId))
		{
			GameObject gameObject = Scenario.SpawnPrefab(Grid.PosToCell(base.gameObject), 0, 0, cropVal.cropId, Grid.SceneLayer.Use, Folder.Entities);
			if (gameObject != null)
			{
				float num = 0.75f;
				gameObject.transform.SetPosition(gameObject.transform.position + new Vector3(0f, num, 0f));
				gameObject.SetActive(true);
				PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
				component.Units = (float)cropVal.numProduced;
				Edible component2 = gameObject.GetComponent<Edible>();
				if (component2)
				{
					ReportManager.Instance.ReportValue(ReportManager.ReportType.CaloriesCreated, component2.Calories, string.Format(UI.ENDOFDAYREPORT.NOTES.HARVESTED, component2.name));
				}
			}
			else
			{
				Output.LogErrorWithObj(base.gameObject, new object[] { "tried to spawn an invalid crop prefab:", cropVal.cropId });
			}
			this.Trigger(-1072826864, null);
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
		Edible edible = null;
		try
		{
			edible = prefab.GetComponent<Edible>();
		}
		catch
		{
			global::Debug.Log("!", null);
		}
		float num = 0f;
		string text = string.Empty;
		if (edible != null)
		{
			num = edible.FoodInfo.CaloriesPerUnit;
		}
		float num2 = num * (float)this.cropVal.numProduced;
		InfoDescription component = prefab.GetComponent<InfoDescription>();
		if (component)
		{
			text = component.description;
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
		Descriptor descriptor2 = new Descriptor(string.Format(UI.UISIDESCREENS.PLANTERSIDESCREEN.BONUS_SEEDS, GameUtil.GetFormattedPercent(33f, GameUtil.TimeSlice.None)), string.Format(UI.UISIDESCREENS.PLANTERSIDESCREEN.TOOLTIPS.BONUS_SEEDS, GameUtil.GetFormattedPercent(33f, GameUtil.TimeSlice.None)), Descriptor.DescriptorType.Effect, false);
		list.Add(descriptor2);
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

	public string domesticatedDesc = string.Empty;

	private Storage planterStorage;

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
