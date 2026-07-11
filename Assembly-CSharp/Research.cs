using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KSerialization;
using STRINGS;

[SerializationConfig(MemberSerialization.OptIn)]
public class Research : KMonoBehaviour, ISaveLoadable
{
	public static void DestroyInstance()
	{
		Research.Instance = null;
	}

	public bool IsBeingResearched(Tech tech)
	{
		return this.activeResearch != null && tech != null && this.activeResearch.tech == tech;
	}

	protected override void OnPrefabInit()
	{
		Research.Instance = this;
		this.researchTypes = new ResearchTypes();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.globalPointInventory == null)
		{
			this.globalPointInventory = new ResearchPointInventory();
		}
		base.Subscribe<Research>(-1523247426, Research.OnRolesUpdatedDelegate);
		Components.ResearchCenters.OnAdd += new Action<ResearchCenter>(this.CheckResearchBuildings);
		Components.ResearchCenters.OnRemove += new Action<ResearchCenter>(this.CheckResearchBuildings);
		foreach (KPrefabID kprefabID in Assets.Prefabs)
		{
			ResearchCenter component = kprefabID.GetComponent<ResearchCenter>();
			if (component != null)
			{
				this.researchCenterPrefabs.Add(component);
			}
		}
	}

	public ResearchType GetResearchType(string id)
	{
		return this.researchTypes.GetResearchType(id);
	}

	public TechInstance GetActiveResearch()
	{
		return this.activeResearch;
	}

	public TechInstance GetTargetResearch()
	{
		if (this.queuedTech != null && this.queuedTech.Count > 0)
		{
			return this.queuedTech[this.queuedTech.Count - 1];
		}
		return null;
	}

	public TechInstance Get(Tech tech)
	{
		foreach (TechInstance techInstance in this.techs)
		{
			if (techInstance.tech == tech)
			{
				return techInstance;
			}
		}
		return null;
	}

	public TechInstance GetOrAdd(Tech tech)
	{
		TechInstance techInstance = this.techs.Find((TechInstance tc) => tc.tech == tech);
		if (techInstance != null)
		{
			return techInstance;
		}
		TechInstance techInstance2 = new TechInstance(tech);
		this.techs.Add(techInstance2);
		return techInstance2;
	}

	public void GetNextTech()
	{
		if (this.queuedTech.Count > 0)
		{
			this.queuedTech.RemoveAt(0);
		}
		if (this.queuedTech.Count > 0)
		{
			this.SetActiveResearch(this.queuedTech[this.queuedTech.Count - 1].tech, false);
			return;
		}
		this.SetActiveResearch(null, false);
	}

	private void AddTechToQueue(Tech tech)
	{
		TechInstance orAdd = this.GetOrAdd(tech);
		if (!orAdd.IsComplete())
		{
			this.queuedTech.Add(orAdd);
		}
		orAdd.tech.requiredTech.ForEach(delegate(Tech _tech)
		{
			this.AddTechToQueue(_tech);
		});
	}

	public void CancelResearch(Tech tech, bool clickedEntry = true)
	{
		Research.<>c__DisplayClass25_0 CS$<>8__locals1 = new Research.<>c__DisplayClass25_0();
		CS$<>8__locals1.tech = tech;
		CS$<>8__locals1.ti = this.queuedTech.Find((TechInstance qt) => qt.tech == CS$<>8__locals1.tech);
		if (CS$<>8__locals1.ti == null)
		{
			return;
		}
		if (CS$<>8__locals1.ti == this.queuedTech[this.queuedTech.Count - 1] && clickedEntry)
		{
			this.SetActiveResearch(null, false);
		}
		int i;
		int j;
		for (i = CS$<>8__locals1.ti.tech.unlockedTech.Count - 1; i >= 0; i = j - 1)
		{
			if (this.queuedTech.Find((TechInstance qt) => qt.tech == CS$<>8__locals1.ti.tech.unlockedTech[i]) != null)
			{
				this.CancelResearch(CS$<>8__locals1.ti.tech.unlockedTech[i], false);
			}
			j = i;
		}
		this.queuedTech.Remove(CS$<>8__locals1.ti);
		if (clickedEntry)
		{
			base.Trigger(-1914338957, this.queuedTech);
		}
	}

	public void SetActiveResearch(Tech tech, bool clearQueue = false)
	{
		if (clearQueue)
		{
			this.queuedTech.Clear();
		}
		this.activeResearch = null;
		if (tech != null)
		{
			if (this.queuedTech.Count == 0)
			{
				this.AddTechToQueue(tech);
			}
			if (this.queuedTech.Count > 0)
			{
				this.queuedTech.Sort((TechInstance x, TechInstance y) => x.tech.tier.CompareTo(y.tech.tier));
				this.activeResearch = this.queuedTech[0];
			}
		}
		else
		{
			this.queuedTech.Clear();
		}
		base.Trigger(-1914338957, this.queuedTech);
		this.CheckBuyResearch();
		this.CheckResearchBuildings(null);
		if (this.activeResearch != null)
		{
			if (this.activeResearch.tech.costsByResearchTypeID.Count > 1)
			{
				if (!MinionResume.AnyMinionHasPerk(Db.Get().SkillPerks.AllowAdvancedResearch.Id))
				{
					this.notifier.Remove(this.NoResearcherRole);
					this.notifier.Add(this.NoResearcherRole, "");
				}
			}
			else
			{
				this.notifier.Remove(this.NoResearcherRole);
			}
			if (this.activeResearch.tech.costsByResearchTypeID.Count <= 2)
			{
				this.notifier.Remove(this.NoResearcherRole);
				return;
			}
			if (!MinionResume.AnyMinionHasPerk(Db.Get().SkillPerks.AllowInterstellarResearch.Id))
			{
				this.notifier.Remove(this.NoResearcherRole);
				this.notifier.Add(this.NoResearcherRole, "");
				return;
			}
		}
		else
		{
			this.notifier.Remove(this.NoResearcherRole);
		}
	}

	public void AddResearchPoints(string researchTypeID, float points)
	{
		if (!this.UseGlobalPointInventory && this.activeResearch == null)
		{
			Debug.LogWarning("No active research to add research points to. Global research inventory is disabled.");
			return;
		}
		(this.UseGlobalPointInventory ? this.globalPointInventory : this.activeResearch.progressInventory).AddResearchPoints(researchTypeID, points);
		this.CheckBuyResearch();
		base.Trigger(-125623018, null);
	}

	private void CheckBuyResearch()
	{
		if (this.activeResearch != null)
		{
			ResearchPointInventory researchPointInventory = (this.UseGlobalPointInventory ? this.globalPointInventory : this.activeResearch.progressInventory);
			if (this.activeResearch.tech.CanAfford(researchPointInventory))
			{
				foreach (KeyValuePair<string, float> keyValuePair in this.activeResearch.tech.costsByResearchTypeID)
				{
					researchPointInventory.RemoveResearchPoints(keyValuePair.Key, keyValuePair.Value);
				}
				this.activeResearch.Purchased();
				Game.Instance.Trigger(-107300940, this.activeResearch.tech);
				this.GetNextTech();
			}
		}
	}

	protected override void OnCleanUp()
	{
		Components.ResearchCenters.OnAdd -= new Action<ResearchCenter>(this.CheckResearchBuildings);
		Components.ResearchCenters.OnRemove -= new Action<ResearchCenter>(this.CheckResearchBuildings);
		base.OnCleanUp();
	}

	public void CompleteQueue()
	{
		while (this.queuedTech.Count > 0)
		{
			foreach (KeyValuePair<string, float> keyValuePair in this.activeResearch.tech.costsByResearchTypeID)
			{
				this.AddResearchPoints(keyValuePair.Key, keyValuePair.Value);
			}
		}
	}

	public List<TechInstance> GetResearchQueue()
	{
		return new List<TechInstance>(this.queuedTech);
	}

	[OnSerializing]
	internal void OnSerializing()
	{
		this.saveData = default(Research.SaveData);
		if (this.activeResearch != null)
		{
			this.saveData.activeResearchId = this.activeResearch.tech.Id;
		}
		else
		{
			this.saveData.activeResearchId = "";
		}
		if (this.queuedTech != null && this.queuedTech.Count > 0)
		{
			this.saveData.targetResearchId = this.queuedTech[this.queuedTech.Count - 1].tech.Id;
		}
		else
		{
			this.saveData.targetResearchId = "";
		}
		this.saveData.techs = new TechInstance.SaveData[this.techs.Count];
		for (int i = 0; i < this.techs.Count; i++)
		{
			this.saveData.techs[i] = this.techs[i].Save();
		}
	}

	[OnDeserialized]
	internal void OnDeserialized()
	{
		if (this.saveData.techs != null)
		{
			foreach (TechInstance.SaveData saveData in this.saveData.techs)
			{
				Tech tech = Db.Get().Techs.TryGet(saveData.techId);
				if (tech != null)
				{
					this.GetOrAdd(tech).Load(saveData);
				}
			}
		}
		foreach (TechInstance techInstance in this.techs)
		{
			if (this.saveData.targetResearchId == techInstance.tech.Id)
			{
				this.SetActiveResearch(techInstance.tech, false);
				break;
			}
		}
	}

	private void OnRolesUpdated(object data)
	{
		if (this.activeResearch == null || this.activeResearch.tech.costsByResearchTypeID.Count <= 1)
		{
			this.notifier.Remove(this.NoResearcherRole);
			return;
		}
		if (!MinionResume.AnyMinionHasPerk(Db.Get().SkillPerks.AllowAdvancedResearch.Id))
		{
			this.notifier.Add(this.NoResearcherRole, "");
			return;
		}
		this.notifier.Remove(this.NoResearcherRole);
	}

	public string GetMissingResearchBuildingName()
	{
		foreach (KeyValuePair<string, float> keyValuePair in this.activeResearch.tech.costsByResearchTypeID)
		{
			bool flag = true;
			if (keyValuePair.Value > 0f)
			{
				flag = false;
				using (List<ResearchCenter>.Enumerator enumerator2 = Components.ResearchCenters.Items.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (enumerator2.Current.research_point_type_id == keyValuePair.Key)
						{
							flag = true;
							break;
						}
					}
				}
			}
			if (!flag)
			{
				foreach (ResearchCenter researchCenter in this.researchCenterPrefabs)
				{
					if (researchCenter.research_point_type_id == keyValuePair.Key)
					{
						return researchCenter.GetProperName();
					}
				}
				return null;
			}
		}
		return null;
	}

	private void CheckResearchBuildings(object data)
	{
		if (this.activeResearch == null)
		{
			this.notifier.Remove(this.MissingResearchStation);
			return;
		}
		if (string.IsNullOrEmpty(this.GetMissingResearchBuildingName()))
		{
			this.notifier.Remove(this.MissingResearchStation);
			return;
		}
		this.notifier.Add(this.MissingResearchStation, "");
	}

	public static Research Instance;

	[MyCmpAdd]
	private Notifier notifier;

	private List<TechInstance> techs = new List<TechInstance>();

	private List<TechInstance> queuedTech = new List<TechInstance>();

	private TechInstance activeResearch;

	private Notification NoResearcherRole = new Notification(RESEARCH.MESSAGING.NO_RESEARCHER_SKILL, NotificationType.Bad, HashedString.Invalid, (List<Notification> list, object data) => RESEARCH.MESSAGING.NO_RESEARCHER_SKILL_TOOLTIP, null, false, 12f, null, null, null);

	private Notification MissingResearchStation = new Notification(RESEARCH.MESSAGING.MISSING_RESEARCH_STATION, NotificationType.Bad, HashedString.Invalid, (List<Notification> list, object data) => RESEARCH.MESSAGING.MISSING_RESEARCH_STATION_TOOLTIP.ToString().Replace("{0}", Research.Instance.GetMissingResearchBuildingName()), null, false, 11f, null, null, null);

	private List<ResearchCenter> researchCenterPrefabs = new List<ResearchCenter>();

	public ResearchTypes researchTypes;

	public bool UseGlobalPointInventory;

	[Serialize]
	public ResearchPointInventory globalPointInventory;

	[Serialize]
	private Research.SaveData saveData;

	private static readonly EventSystem.IntraObjectHandler<Research> OnRolesUpdatedDelegate = new EventSystem.IntraObjectHandler<Research>(delegate(Research component, object data)
	{
		component.OnRolesUpdated(data);
	});

	private struct SaveData
	{
		public string activeResearchId;

		public string targetResearchId;

		public TechInstance.SaveData[] techs;
	}
}
