using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Edible : Workable, ISaveLoadableJson, IGameObjectEffectDescriptor
{
	private Edible()
	{
		this.showProgressBar = false;
		base.SetOffsetTable(OffsetGroups.InvertedStandardTable);
	}

	public int DescriptionOrder { get; set; }

	public float rations
	{
		get
		{
			return base.GetComponent<PrimaryElement>().Units * (float)this.foodInfo.Rations;
		}
		set
		{
			base.GetComponent<PrimaryElement>().Units = value / (float)this.foodInfo.Rations;
		}
	}

	public EdiblesManager.FoodInfo FoodInfo
	{
		get
		{
			return this.foodInfo;
		}
		set
		{
			this.foodInfo = value;
			this.FoodID = this.foodInfo.Id;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_break") };
		if (this.foodInfo == null)
		{
			if (this.FoodID == null)
			{
				Output.LogError(new object[] { "No food FoodID" });
			}
			this.foodInfo = EdiblesManager.instance.GetFoodInfo(this.FoodID);
		}
		base.GetComponent<KSelectable>().SetName(this.foodInfo.Name);
		base.GetComponent<KPrefabID>().AddTag(GameTags.Edible);
		this.Subscribe(748399584, new EventSystem.EventHandler(this.OnCraft));
		this.Subscribe(1272413801, new EventSystem.EventHandler(this.OnCraft));
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Eating;
		Components.Edibles.Add(this);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().MiscStatusItems.Edible, this);
	}

	private void OnCraft(object data)
	{
		RationTracker.Get().RegisterRationsProduced((int)this.rations);
	}

	public float GetFeedingTime()
	{
		return this.rations * Edible.secondsPerRation;
	}

	protected override void OnStartWork(Worker worker)
	{
		base.SetWorkTime(this.GetFeedingTime());
		worker.GetAttributes().Add("Eating", this.caloriesModifier);
		this.StartConsuming();
	}

	protected override void OnStopWork(Worker worker)
	{
		worker.GetAttributes().Remove(this.caloriesModifier);
		this.StopConsuming(worker.gameObject);
	}

	private void StartConsuming()
	{
		this.consumptionStartTime = Time.time;
	}

	private void StopConsuming(GameObject target)
	{
		float num = Time.time - this.consumptionStartTime;
		float num2 = Mathf.Clamp01(num / this.GetFeedingTime());
		this.rationsConsumed = this.rations * num2;
		this.rations -= this.rationsConsumed;
		target.Trigger(1121894420, this);
		this.Trigger(-10536414, target);
		this.rationsConsumed = float.NaN;
		this.consumptionStartTime = float.NaN;
		if (this.rations <= 0f)
		{
			base.gameObject.DeleteObject();
		}
	}

	public override string[] GetWorkAnims(Worker worker)
	{
		return new string[] { "eat_pre", "eat_loop" };
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.Edibles.Remove(this);
	}

	public Edible.Quality GetQuality()
	{
		return this.foodInfo.Quality;
	}

	public List<Descriptor> GetRequirementDescriptions(GameObject go)
	{
		return null;
	}

	public List<string> GetEffectDescriptions(GameObject go)
	{
		List<string> list = new List<string>();
		list.Add(string.Format(UI.GAMEOBJECTEFFECTS.CALORIES, GameUtil.GetFormattedCalories((float)this.foodInfo.Rations * 100000f, GameUtil.TimeSlice.None, true)));
		if (this.consumptionEffectsString.Count > 0)
		{
			for (int i = 0; i < this.consumptionEffectsString.Count; i++)
			{
				list.Add(this.consumptionEffectsString[i]);
			}
		}
		return list;
	}

	public string FoodID;

	private EdiblesManager.FoodInfo foodInfo;

	private static float secondsPerRation = 3f;

	private static float caloriesPerRation = 100000f;

	private float consumptionStartTime = float.NaN;

	public float rationsConsumed = float.NaN;

	private AttributeModifier caloriesModifier = new AttributeModifier("CaloriesDelta", Edible.caloriesPerRation / Edible.secondsPerRation, DUPLICANTS.MODIFIERS.EATINGCALORIES.NAME, false);

	public List<string> consumptionEffectsString = new List<string>();

	public enum Quality
	{
		Poor,
		Average,
		Good,
		Great
	}
}
