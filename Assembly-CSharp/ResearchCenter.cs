using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class ResearchCenter : Workable, IEffectDescriptor
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Researching;
		this.attributeConverter = Db.Get().AttributeConverters.ResearchSpeed;
		ElementConverter elementConverter = this.elementConverter;
		elementConverter.onConvertMass = (Action<float>)Delegate.Combine(elementConverter.onConvertMass, new Action<float>(this.ConvertMassToResearchPoints));
		this.storage.choreType = Db.Get().ChoreTypes.ResearchFetch;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.Subscribe(-1503271301, new Action<object>(this.OnSelectObject));
		Research.Instance.Subscribe(-1914338957, new Action<object>(this.CheckValidResearchSelected));
		Research.Instance.Subscribe(-125623018, new Action<object>(this.CheckValidResearchSelected));
		this.Subscribe(187661686, new Action<object>(this.CheckValidResearchSelected));
		this.Subscribe(-1697596308, new Action<object>(this.CheckHasMaterial));
		this.CheckValidResearchSelected(null);
		Components.ResearchCenters.Add(this);
		this.CheckValidResearchSelected(null);
	}

	private void ConvertMassToResearchPoints(float mass_consumed)
	{
		this.remainder_mass_points += mass_consumed / this.mass_per_point - (float)Mathf.FloorToInt(mass_consumed / this.mass_per_point);
		int num = Mathf.FloorToInt(mass_consumed / this.mass_per_point);
		num += Mathf.FloorToInt(this.remainder_mass_points);
		this.remainder_mass_points -= (float)Mathf.FloorToInt(this.remainder_mass_points);
		if (num > 0)
		{
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Research, Strings.Get("STRINGS.RESEARCH.TYPES." + this.research_point_type_id.ToUpper() + ".NAME"), this.transform, 1.5f, false);
			for (int i = 0; i < num; i++)
			{
				Research.Instance.AddResearchPoints(this.research_point_type_id, 1f);
			}
		}
	}

	private void SimUpdate(float dt)
	{
		if (!this.operational.IsActive)
		{
			if (this.operational.IsOperational && this.chore == null && this.HasMaterial())
			{
				this.chore = new WorkChore<ResearchCenter>(Db.Get().ChoreTypes.Research, this, null, true, null, null, null, true, null, true, default(Tag), null, false, true, true, int.MaxValue);
				base.SetWorkTime(float.PositiveInfinity);
			}
		}
	}

	public override float GetPercentComplete()
	{
		if (Research.Instance.GetActiveResearch() == null)
		{
			return 0f;
		}
		return Research.Instance.GetActiveResearch().progressInventory.PointsByTypeID[this.research_point_type_id] / Research.Instance.GetActiveResearch().tech.costsByResearchTypeID[this.research_point_type_id];
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		this.operational.SetActive(true, false);
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		float num = Db.Get().AttributeConverters.ResearchSpeed.Lookup(worker).Evaluate();
		this.elementConverter.consumedElements[0].massConsumptionRate = Mathf.Max(0.2f, 1.16f + num * 1.16f);
		return base.OnWorkTick(worker, dt);
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
		base.ShowProgressBar(false);
		this.operational.SetActive(false, false);
	}

	private bool ResearchComponentCompleted()
	{
		TechInstance activeResearch = Research.Instance.GetActiveResearch();
		if (activeResearch != null)
		{
			float num = 0f;
			float num2 = 0f;
			activeResearch.progressInventory.PointsByTypeID.TryGetValue(this.research_point_type_id, out num);
			activeResearch.tech.costsByResearchTypeID.TryGetValue(this.research_point_type_id, out num2);
			if (num >= num2)
			{
				return true;
			}
		}
		return false;
	}

	private void CheckValidResearchSelected(object data)
	{
		bool flag = false;
		bool flag2 = false;
		TechInstance activeResearch = Research.Instance.GetActiveResearch();
		if (activeResearch != null)
		{
			flag = true;
			if (activeResearch.tech.costsByResearchTypeID.ContainsKey(this.research_point_type_id) && Research.Instance.Get(activeResearch.tech).progressInventory.PointsByTypeID[this.research_point_type_id] < activeResearch.tech.costsByResearchTypeID[this.research_point_type_id])
			{
				flag2 = true;
			}
		}
		if (this.operational.GetFlag(EnergyConsumer.PoweredFlag))
		{
			if (flag)
			{
				base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.NoResearchSelected, false);
				if (!flag2 && !this.ResearchComponentCompleted())
				{
					base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.NoResearchSelected, false);
					base.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.NoApplicableResearchSelected, null);
				}
				else
				{
					base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.NoApplicableResearchSelected, false);
				}
			}
			else
			{
				base.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.NoResearchSelected, null);
				base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.NoApplicableResearchSelected, false);
			}
		}
		else
		{
			base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.NoResearchSelected, false);
			base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.NoApplicableResearchSelected, false);
		}
		this.operational.SetFlag(ResearchCenter.ResearchSelectedFlag, flag && flag2);
		if ((!flag || !flag2) && base.worker)
		{
			base.StopWork(base.worker, true);
		}
	}

	private void ClearResearchScreen()
	{
		if (this.researchScreen != null)
		{
			this.researchScreen.Deactivate();
			this.researchScreen = null;
		}
	}

	private void OnSelectResearchClick()
	{
		DetailsScreen.Instance.Show(false);
		if (this.researchScreen == null)
		{
			ManagementMenu.Instance.ToggleResearch();
		}
		else
		{
			this.ClearResearchScreen();
		}
	}

	private void OnSelectObject(object data)
	{
		this.ClearResearchScreen();
	}

	private void CheckHasMaterial(object o = null)
	{
		if (!this.HasMaterial() && this.chore != null)
		{
			this.chore.Cancel("No material remaining");
			this.chore = null;
		}
	}

	private bool HasMaterial()
	{
		return this.storage.MassStored() > 0f;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Research.Instance.Unsubscribe(-1914338957, new Action<object>(this.CheckValidResearchSelected));
		Research.Instance.Unsubscribe(-125623018, new Action<object>(this.CheckValidResearchSelected));
		this.Unsubscribe(-1852328367, new Action<object>(this.CheckValidResearchSelected));
		Components.ResearchCenters.Remove(this);
		this.ClearResearchScreen();
	}

	public string GetStatusString()
	{
		string text = Strings.Get("STRINGS.RESEARCH.MESSAGING.NORESEARCHSELECTED");
		if (Research.Instance.GetActiveResearch() != null)
		{
			text = "<b>" + Research.Instance.GetActiveResearch().tech.Name + "</b>";
			int num = 0;
			foreach (KeyValuePair<string, float> keyValuePair in Research.Instance.GetActiveResearch().progressInventory.PointsByTypeID)
			{
				if (Research.Instance.GetActiveResearch().tech.costsByResearchTypeID[keyValuePair.Key] != 0f)
				{
					num++;
				}
			}
			foreach (KeyValuePair<string, float> keyValuePair2 in Research.Instance.GetActiveResearch().progressInventory.PointsByTypeID)
			{
				if (Research.Instance.GetActiveResearch().tech.costsByResearchTypeID[keyValuePair2.Key] != 0f)
				{
					bool flag = keyValuePair2.Key == this.research_point_type_id;
					if (flag)
					{
						text = text + "\n   - " + Research.Instance.researchTypes.GetResearchType(keyValuePair2.Key).name;
						string text2 = text;
						text = string.Concat(new object[]
						{
							text2,
							": ",
							keyValuePair2.Value,
							"/",
							Research.Instance.GetActiveResearch().tech.costsByResearchTypeID[keyValuePair2.Key]
						});
					}
				}
			}
			foreach (KeyValuePair<string, float> keyValuePair3 in Research.Instance.GetActiveResearch().progressInventory.PointsByTypeID)
			{
				if (Research.Instance.GetActiveResearch().tech.costsByResearchTypeID[keyValuePair3.Key] != 0f)
				{
					if (!(keyValuePair3.Key == this.research_point_type_id))
					{
						if (num > 1)
						{
							text = text + "\n   - " + string.Format(RESEARCH.MESSAGING.RESEARCHTYPEALSOREQUIRED, Research.Instance.researchTypes.GetResearchType(keyValuePair3.Key).name);
						}
						else
						{
							text = text + "\n   - " + string.Format(RESEARCH.MESSAGING.RESEARCHTYPEREQUIRED, Research.Instance.researchTypes.GetResearchType(keyValuePair3.Key).name);
						}
					}
				}
			}
		}
		return text;
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		Descriptor descriptor = default(Descriptor);
		descriptor.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.PRODUCES_RESEARCH_POINTS, Research.Instance.researchTypes.GetResearchType(this.research_point_type_id).name), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.PRODUCES_RESEARCH_POINTS, Research.Instance.researchTypes.GetResearchType(this.research_point_type_id).name), Descriptor.DescriptorType.Effect);
		list.Add(descriptor);
		return list;
	}

	private Chore chore;

	private ResearchScreen researchScreen;

	[MyCmpAdd]
	private UserMenu userMenu;

	[MyCmpAdd]
	private Notifier notifier;

	[MyCmpAdd]
	private Operational operational;

	[MyCmpAdd]
	private Storage storage;

	[MyCmpGet]
	private ElementConverter elementConverter;

	[SerializeField]
	public string research_point_type_id;

	[SerializeField]
	public float mass_per_point;

	[SerializeField]
	private float remainder_mass_points;

	public static Operational.Flag ResearchSelectedFlag = new Operational.Flag("researchSelected", Operational.Flag.Type.Requirement);
}
