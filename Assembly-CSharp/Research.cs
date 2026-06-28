using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class Research : KMonoBehaviour, ISaveLoadable
{
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
		}
		else
		{
			this.SetActiveResearch(null, false);
		}
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
		Research.<CancelResearch>c__AnonStoreyDD <CancelResearch>c__AnonStoreyDD = new Research.<CancelResearch>c__AnonStoreyDD();
		<CancelResearch>c__AnonStoreyDD.tech = tech;
		<CancelResearch>c__AnonStoreyDD.ti = this.queuedTech.Find((TechInstance qt) => qt.tech == <CancelResearch>c__AnonStoreyDD.tech);
		if (<CancelResearch>c__AnonStoreyDD.ti == null)
		{
			return;
		}
		if (<CancelResearch>c__AnonStoreyDD.ti == this.queuedTech[this.queuedTech.Count - 1] && clickedEntry)
		{
			this.SetActiveResearch(null, false);
		}
		int i;
		for (i = <CancelResearch>c__AnonStoreyDD.ti.tech.unlockedTech.Count - 1; i >= 0; i--)
		{
			if (this.queuedTech.Find((TechInstance qt) => qt.tech == <CancelResearch>c__AnonStoreyDD.ti.tech.unlockedTech[i]) != null)
			{
				this.CancelResearch(<CancelResearch>c__AnonStoreyDD.ti.tech.unlockedTech[i], false);
			}
		}
		this.queuedTech.Remove(<CancelResearch>c__AnonStoreyDD.ti);
		if (clickedEntry)
		{
			this.Trigger(-1914338957, this.queuedTech);
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
				this.queuedTech = this.queuedTech.OrderBy<TechInstance, int>((TechInstance tc) => tc.tech.tier).ToList<TechInstance>();
				this.activeResearch = this.queuedTech[0];
			}
		}
		else
		{
			this.queuedTech.Clear();
		}
		this.Trigger(-1914338957, this.queuedTech);
		this.CheckBuyResearch();
	}

	public void AddResearchPoints(string researchTypeID, float points)
	{
		if (!this.UseGlobalPointInventory && this.activeResearch == null)
		{
			Debug.LogWarning("No active research to add research points to. Global research inventory is disabled.", null);
			return;
		}
		ResearchPointInventory researchPointInventory = ((!this.UseGlobalPointInventory) ? this.activeResearch.progressInventory : this.globalPointInventory);
		researchPointInventory.AddResearchPoints(researchTypeID, points);
		this.CheckBuyResearch();
		this.Trigger(-125623018, null);
	}

	private void CheckBuyResearch()
	{
		if (this.activeResearch != null)
		{
			ResearchPointInventory researchPointInventory = ((!this.UseGlobalPointInventory) ? this.activeResearch.progressInventory : this.globalPointInventory);
			bool flag = this.activeResearch.tech.CanAfford(researchPointInventory);
			if (flag)
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
			this.saveData.activeResearchId = string.Empty;
		}
		if (this.queuedTech != null && this.queuedTech.Count > 0)
		{
			this.saveData.targetResearchId = this.queuedTech[this.queuedTech.Count - 1].tech.Id;
		}
		else
		{
			this.saveData.targetResearchId = string.Empty;
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
					TechInstance orAdd = this.GetOrAdd(tech);
					orAdd.Load(saveData);
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

	public static Research Instance;

	[MyCmpAdd]
	private Notifier notifier;

	private List<TechInstance> techs = new List<TechInstance>();

	private List<TechInstance> queuedTech = new List<TechInstance>();

	private TechInstance activeResearch;

	public ResearchTypes researchTypes;

	public bool UseGlobalPointInventory;

	[Serialize]
	public ResearchPointInventory globalPointInventory;

	[Serialize]
	private Research.SaveData saveData = default(Research.SaveData);

	private struct SaveData
	{
		public string activeResearchId;

		public string targetResearchId;

		public TechInstance.SaveData[] techs;
	}
}
