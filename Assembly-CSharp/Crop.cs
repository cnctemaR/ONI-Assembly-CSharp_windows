using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
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
		if (Crop.lowYieldStatus == null)
		{
			Crop.lowYieldStatus = new StatusItem("lowYield", CREATURES.STATUSITEMS.LOW_YIELD.NAME, CREATURES.STATUSITEMS.LOW_YIELD.TOOLTIP, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			Crop.normalYieldStatus = new StatusItem("normalYield", CREATURES.STATUSITEMS.NORMAL_YIELD.NAME, CREATURES.STATUSITEMS.NORMAL_YIELD.TOOLTIP, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			Crop.normalYieldStatus.resolveStringCallback = delegate(string str, object obj)
			{
				if (str.Contains("{Effects}"))
				{
					Crop crop = (Crop)obj;
					string text = string.Empty;
					if (crop.medYieldEffects != null)
					{
						foreach (IYieldEffect yieldEffect in crop.medYieldEffects)
						{
							foreach (Descriptor descriptor in yieldEffect.GetDescription(crop.gameObject))
							{
								text += string.Format(CREATURES.STATUSITEMS.NORMAL_YIELD.LINE_ITEM, descriptor.text);
							}
						}
					}
					str = str.Replace("{Effects}", text);
					str = str.Replace("{Percent}", GameUtil.GetFormattedPercent(40f, GameUtil.TimeSlice.None));
				}
				return str;
			};
			Crop.highYieldStatus = new StatusItem("highYield", CREATURES.STATUSITEMS.HIGH_YIELD.NAME, CREATURES.STATUSITEMS.HIGH_YIELD.TOOLTIP, string.Empty, StatusItem.IconType.Info, NotificationType.Neutral, false, SimViewMode.None, SimViewMode.None);
			Crop.highYieldStatus.resolveStringCallback = delegate(string str, object obj)
			{
				if (str.Contains("{Effects}"))
				{
					Crop crop2 = (Crop)obj;
					string text2 = string.Empty;
					if (crop2.medYieldEffects != null)
					{
						foreach (IYieldEffect yieldEffect2 in crop2.medYieldEffects)
						{
							foreach (Descriptor descriptor2 in yieldEffect2.GetDescription(crop2.gameObject))
							{
								text2 += string.Format(CREATURES.STATUSITEMS.HIGH_YIELD.LINE_ITEM, descriptor2.text);
							}
						}
					}
					if (crop2.highYieldEffects != null)
					{
						foreach (IYieldEffect yieldEffect3 in crop2.highYieldEffects)
						{
							foreach (Descriptor descriptor3 in yieldEffect3.GetDescription(crop2.gameObject))
							{
								text2 += string.Format(CREATURES.STATUSITEMS.HIGH_YIELD.LINE_ITEM, descriptor3.text);
							}
						}
					}
					str = str.Replace("{Effects}", text2);
					str = str.Replace("{Percent}", GameUtil.GetFormattedPercent(80f, GameUtil.TimeSlice.None));
				}
				return str;
			};
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.Subscribe(-254803949, new Action<object>(this.OnGrow));
		this.Subscribe(1272413801, new Action<object>(this.OnHarvest));
		this.Subscribe(-1736624145, new Action<object>(this.OnSeedDropped));
		this.UpdateStatus();
	}

	public void Configure(Crop.CropVal cropval)
	{
		this.cropVal = cropval;
	}

	public void SetYieldModifiers(IYieldEffect[] medYieldEffects, IYieldEffect[] highYieldEffects)
	{
		this.medYieldEffects = medYieldEffects;
		this.highYieldEffects = highYieldEffects;
	}

	public bool CanGrow()
	{
		return this.cropVal.renewable || this.cropVal.harvests - this.timesHarvested > 0;
	}

	private void UpdateStatus()
	{
		if (this.harvestsRemainingHandle != Guid.Empty)
		{
			this.selectable.RemoveStatusItem(this.harvestsRemainingHandle, false);
		}
		this.harvestsRemainingHandle = this.selectable.AddStatusItem(Db.Get().CreatureStatusItems.HarvestsRemaining, this);
	}

	public int GetHarvestsRemaining()
	{
		return this.cropVal.harvests - this.timesHarvested;
	}

	public int GetTimesHarvested()
	{
		return this.timesHarvested;
	}

	public int GetTotalHarvests()
	{
		return this.cropVal.harvests;
	}

	public void SetTimesHarvested(int times)
	{
		this.timesHarvested = times;
	}

	public void SpawnFruit(object callbackParam)
	{
		List<IYieldEffect> list = new List<IYieldEffect>();
		if (this.medYieldEffects != null && (this.yield == Crop.Yield.Medium || this.yield == Crop.Yield.High))
		{
			list.AddRange(this.medYieldEffects);
		}
		if (this.highYieldEffects != null && this.yield == Crop.Yield.High)
		{
			list.AddRange(this.highYieldEffects);
		}
		Crop.CropVal cropVal = this.cropVal;
		if (list != null)
		{
			foreach (IYieldEffect yieldEffect in list)
			{
				cropVal = yieldEffect.ApplyToCropVal(base.gameObject, cropVal, this.cropVal);
			}
		}
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
				if (list != null)
				{
					foreach (IYieldEffect yieldEffect2 in list)
					{
						yieldEffect2.ApplyToCrop(base.gameObject, gameObject);
					}
				}
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
			this.Trigger(-1072826864, list);
		}
		if (!this.cropVal.renewable)
		{
			this.timesHarvested++;
			if (this.cropVal.harvests - this.timesHarvested <= 0)
			{
				this.Trigger(591871899, list);
			}
		}
		this.ClearGreatEffect();
	}

	private void OnGrow(object obj)
	{
		Amounts amounts = base.gameObject.GetAmounts();
		AmountInstance amountInstance = amounts.Get(Db.Get().Amounts.YieldBonus);
		if (this.highYieldEffects != null && amountInstance.value > amountInstance.GetMax() * 0.8f)
		{
			this.yield = Crop.Yield.High;
			this.SpawnGreatEffect();
		}
		else if (this.medYieldEffects != null && amountInstance.value > amountInstance.GetMax() * 0.4f)
		{
			this.yield = Crop.Yield.Medium;
		}
		else
		{
			this.yield = Crop.Yield.Low;
		}
		this.UpdateYieldStatus();
	}

	private void OnHarvest(object obj)
	{
		this.selectable.RemoveStatusItem(this.yieldStatusHandle, false);
		this.ClearGreatEffect();
	}

	private void UpdateYieldStatus()
	{
		if (this.yieldStatusHandle != Guid.Empty)
		{
			this.selectable.RemoveStatusItem(this.yieldStatusHandle, true);
		}
		switch (this.yield)
		{
		case Crop.Yield.Low:
			this.yieldStatusHandle = this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Yield, Crop.lowYieldStatus, this);
			break;
		case Crop.Yield.Medium:
			this.yieldStatusHandle = this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Yield, Crop.normalYieldStatus, this);
			break;
		case Crop.Yield.High:
			this.yieldStatusHandle = this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Yield, Crop.highYieldStatus, this);
			break;
		}
	}

	public void OnSeedDropped(object data)
	{
		if (data != null)
		{
			PlantableSeed plantableSeed = (PlantableSeed)data;
			plantableSeed.timesHarvested = this.timesHarvested;
		}
	}

	private void SpawnGreatEffect()
	{
		if (this.greatEffect == null)
		{
			OccupyArea component = base.GetComponent<OccupyArea>();
			Extents extents = component.GetExtents();
			KPrefabID component2 = base.GetComponent<KPrefabID>();
			Vector3 vector;
			if (component2.HasTag(GameTags.Hanging))
			{
				vector = new Vector3((float)(extents.x + extents.width / 2) + 0.5f, (float)extents.y + 0.5f);
			}
			else
			{
				vector = new Vector3((float)(extents.x + extents.width / 2) + 0.5f, (float)(extents.y + extents.height) - 0.5f);
			}
			this.greatEffect = GameUtil.KInstantiate(EffectPrefabs.Instance.HarvestGlow, vector, Grid.SceneLayer.FXFront, SceneOrganizer.Instance.GetFolder(Folder.FX), null, 0);
			this.greatEffect.AddOrGet<LoopingSounds>();
			KBatchedAnimController component3 = this.greatEffect.GetComponent<KBatchedAnimController>();
			component3.Play("idle_pre", KAnim.PlayMode.Once, 1f, 0f);
			component3.Queue("idle_loop", KAnim.PlayMode.Loop, 1f, 0f);
		}
	}

	private void ClearGreatEffect()
	{
		if (this.greatEffect != null)
		{
			KBatchedAnimController component = this.greatEffect.GetComponent<KBatchedAnimController>();
			component.Play("idle_pst", KAnim.PlayMode.Once, 1f, 0f);
			GameObject localRef = this.greatEffect;
			this.greatEffect.Subscribe(-1061186183, delegate(object obj)
			{
				Util.KDestroyGameObject(localRef);
			});
			this.greatEffect = null;
		}
	}

	public List<Descriptor> RequirementDescriptors(GameObject go)
	{
		return new List<Descriptor>
		{
			new Descriptor(string.Format(UI.UISIDESCREENS.PLANTERSIDESCREEN.NUMBEROFHARVESTS, this.cropVal.harvests), string.Format(UI.UISIDESCREENS.PLANTERSIDESCREEN.TOOLTIPS.NUMBEROFHARVESTS, this.cropVal.harvests), Descriptor.DescriptorType.Requirement, false)
		};
	}

	public List<Descriptor> HarvestDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		Tag tag = new Tag(this.cropVal.cropId);
		GameObject prefab = Assets.GetPrefab(tag);
		Crop.CropVal cropVal = this.cropVal;
		Crop.CropVal cropVal2 = this.cropVal;
		if (this.medYieldEffects != null)
		{
			foreach (IYieldEffect yieldEffect in this.medYieldEffects)
			{
				cropVal = yieldEffect.ApplyToCropVal(go, cropVal, this.cropVal);
				cropVal2 = yieldEffect.ApplyToCropVal(go, cropVal2, this.cropVal);
			}
		}
		if (this.highYieldEffects != null)
		{
			foreach (IYieldEffect yieldEffect2 in this.highYieldEffects)
			{
				cropVal2 = yieldEffect2.ApplyToCropVal(go, cropVal2, this.cropVal);
			}
		}
		Edible component = prefab.GetComponent<Edible>();
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		string text = string.Empty;
		if (component != null)
		{
			num = component.FoodInfo.CaloriesPerUnit;
			num2 = num * (float)this.cropVal.numProduced;
			num3 = num * (float)cropVal.numProduced;
			num4 = num * (float)cropVal2.numProduced;
		}
		InfoDescription component2 = prefab.GetComponent<InfoDescription>();
		if (component2)
		{
			text = component2.description;
		}
		string text2;
		string text3;
		string text4;
		if (GameTags.DisplayAsCalories.Contains(tag))
		{
			text2 = GameUtil.GetFormattedCalories(num2, GameUtil.TimeSlice.None, true);
			text3 = GameUtil.GetFormattedCalories(num3, GameUtil.TimeSlice.None, true);
			text4 = GameUtil.GetFormattedCalories(num4, GameUtil.TimeSlice.None, true);
		}
		else if (GameTags.DisplayAsUnits.Contains(tag))
		{
			text2 = GameUtil.GetFormattedUnits((float)this.cropVal.numProduced, GameUtil.TimeSlice.None, false);
			text3 = GameUtil.GetFormattedUnits((float)cropVal.numProduced, GameUtil.TimeSlice.None, false);
			text4 = GameUtil.GetFormattedUnits((float)cropVal2.numProduced, GameUtil.TimeSlice.None, false);
		}
		else
		{
			text2 = GameUtil.GetFormattedMass((float)this.cropVal.numProduced, GameUtil.TimeSlice.None, true, "{0:0.#}");
			text3 = GameUtil.GetFormattedMass((float)cropVal.numProduced, GameUtil.TimeSlice.None, true, "{0:0.#}");
			text4 = GameUtil.GetFormattedMass((float)cropVal2.numProduced, GameUtil.TimeSlice.None, true, "{0:0.#}");
		}
		LocString locString = UI.UISIDESCREENS.PLANTERSIDESCREEN.YIELD;
		Descriptor descriptor = new Descriptor(string.Format(locString, prefab.GetProperName(), text2), string.Format(UI.UISIDESCREENS.PLANTERSIDESCREEN.TOOLTIPS.YIELD, text, GameUtil.GetFormattedCalories(num, GameUtil.TimeSlice.None, true), GameUtil.GetFormattedCalories(num2, GameUtil.TimeSlice.None, true)), Descriptor.DescriptorType.HarvestLowYield, false);
		list.Add(descriptor);
		locString = UI.UISIDESCREENS.PLANTERSIDESCREEN.YIELD;
		Descriptor descriptor2 = new Descriptor(string.Format(locString, prefab.GetProperName(), text3), string.Format(UI.UISIDESCREENS.PLANTERSIDESCREEN.TOOLTIPS.YIELD, text, GameUtil.GetFormattedCalories(num, GameUtil.TimeSlice.None, true), GameUtil.GetFormattedCalories(num3, GameUtil.TimeSlice.None, true)), Descriptor.DescriptorType.HarvestMedYield, false);
		list.Add(descriptor2);
		locString = UI.UISIDESCREENS.PLANTERSIDESCREEN.YIELD;
		Descriptor descriptor3 = new Descriptor(string.Format(locString, prefab.GetProperName(), text4), string.Format(UI.UISIDESCREENS.PLANTERSIDESCREEN.TOOLTIPS.YIELD, text, GameUtil.GetFormattedCalories(num, GameUtil.TimeSlice.None, true), GameUtil.GetFormattedCalories(num4, GameUtil.TimeSlice.None, true)), Descriptor.DescriptorType.HarvestHighYield, false);
		list.Add(descriptor3);
		return list;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		foreach (Descriptor descriptor in this.RequirementDescriptors(go))
		{
			list.Add(descriptor);
		}
		foreach (Descriptor descriptor2 in this.HarvestDescriptors(go))
		{
			list.Add(descriptor2);
		}
		return list;
	}

	[MyCmpReq]
	private KSelectable selectable;

	public Crop.CropVal cropVal;

	[Serialize]
	private int timesHarvested;

	private Guid harvestsRemainingHandle = Guid.Empty;

	public string domesticatedDesc = string.Empty;

	private Crop.Yield yield;

	public static StatusItem lowYieldStatus;

	public static StatusItem normalYieldStatus;

	public static StatusItem highYieldStatus;

	public IYieldEffect[] medYieldEffects;

	public IYieldEffect[] highYieldEffects;

	private Storage planterStorage;

	private Guid yieldStatusHandle;

	private GameObject greatEffect;

	[Serializable]
	public struct CropVal
	{
		public CropVal(string crop_id, float crop_duration, float regrow_duration, int num_produced = 1, bool renewable = true, int harvests = 1)
		{
			this.cropId = crop_id;
			this.cropDuration = crop_duration;
			this.regrowDuration = regrow_duration;
			this.numProduced = num_produced;
			this.renewable = renewable;
			this.harvests = harvests;
		}

		public string cropId;

		public float cropDuration;

		public float regrowDuration;

		public int numProduced;

		public bool renewable;

		public int harvests;
	}

	public enum Yield
	{
		Low,
		Medium,
		High
	}
}
