using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class PlantSubSpeciesCatalog : KMonoBehaviour
{
	public static void DestroyInstance()
	{
		PlantSubSpeciesCatalog.Instance = null;
	}

	public bool AnyNonOriginalDiscovered
	{
		get
		{
			foreach (KeyValuePair<Tag, List<PlantSubSpeciesCatalog.SubSpeciesInfo>> keyValuePair in this.discoveredSubspeciesBySpecies)
			{
				if (keyValuePair.Value.Find((PlantSubSpeciesCatalog.SubSpeciesInfo ss) => !ss.IsOriginal).IsValid)
				{
					return true;
				}
			}
			return false;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		PlantSubSpeciesCatalog.Instance = this;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.EnsureOriginalSubSpecies();
	}

	public List<Tag> GetAllDiscoveredSpecies()
	{
		return this.discoveredSubspeciesBySpecies.Keys.ToList<Tag>();
	}

	public List<PlantSubSpeciesCatalog.SubSpeciesInfo> GetAllSubSpeciesForSpecies(Tag speciesID)
	{
		List<PlantSubSpeciesCatalog.SubSpeciesInfo> list;
		if (this.discoveredSubspeciesBySpecies.TryGetValue(speciesID, out list))
		{
			return list;
		}
		return null;
	}

	public bool GetOriginalSubSpecies(Tag speciesID, out PlantSubSpeciesCatalog.SubSpeciesInfo subSpeciesInfo)
	{
		if (!this.discoveredSubspeciesBySpecies.ContainsKey(speciesID))
		{
			subSpeciesInfo = default(PlantSubSpeciesCatalog.SubSpeciesInfo);
			return false;
		}
		subSpeciesInfo = this.discoveredSubspeciesBySpecies[speciesID].Find((PlantSubSpeciesCatalog.SubSpeciesInfo i) => i.IsOriginal);
		return true;
	}

	public PlantSubSpeciesCatalog.SubSpeciesInfo GetSubSpecies(Tag speciesID, Tag subSpeciesID)
	{
		return this.discoveredSubspeciesBySpecies[speciesID].Find((PlantSubSpeciesCatalog.SubSpeciesInfo i) => i.ID == subSpeciesID);
	}

	public PlantSubSpeciesCatalog.SubSpeciesInfo FindSubSpecies(Tag subSpeciesID)
	{
		Predicate<PlantSubSpeciesCatalog.SubSpeciesInfo> <>9__0;
		foreach (KeyValuePair<Tag, List<PlantSubSpeciesCatalog.SubSpeciesInfo>> keyValuePair in this.discoveredSubspeciesBySpecies)
		{
			List<PlantSubSpeciesCatalog.SubSpeciesInfo> value = keyValuePair.Value;
			Predicate<PlantSubSpeciesCatalog.SubSpeciesInfo> predicate;
			if ((predicate = <>9__0) == null)
			{
				predicate = (<>9__0 = (PlantSubSpeciesCatalog.SubSpeciesInfo i) => i.ID == subSpeciesID);
			}
			PlantSubSpeciesCatalog.SubSpeciesInfo subSpeciesInfo = value.Find(predicate);
			if (subSpeciesInfo.ID.IsValid)
			{
				return subSpeciesInfo;
			}
		}
		return default(PlantSubSpeciesCatalog.SubSpeciesInfo);
	}

	public void DiscoverSubSpecies(PlantSubSpeciesCatalog.SubSpeciesInfo newSubSpeciesInfo, MutantPlant source)
	{
		if (!this.discoveredSubspeciesBySpecies[newSubSpeciesInfo.speciesID].Contains(newSubSpeciesInfo))
		{
			this.discoveredSubspeciesBySpecies[newSubSpeciesInfo.speciesID].Add(newSubSpeciesInfo);
			Notification notification = new Notification(MISC.NOTIFICATIONS.NEWMUTANTSEED.NAME, NotificationType.Good, new Func<List<Notification>, object, string>(this.NewSubspeciesTooltipCB), newSubSpeciesInfo, true, 0f, null, null, source.transform, true, false);
			base.gameObject.AddOrGet<Notifier>().Add(notification, "");
		}
	}

	private string NewSubspeciesTooltipCB(List<Notification> notifications, object data)
	{
		PlantSubSpeciesCatalog.SubSpeciesInfo subSpeciesInfo = (PlantSubSpeciesCatalog.SubSpeciesInfo)data;
		return MISC.NOTIFICATIONS.NEWMUTANTSEED.TOOLTIP.Replace("{Plant}", subSpeciesInfo.speciesID.ProperName());
	}

	public void IdentifySubSpecies(Tag subSpeciesID)
	{
		if (this.identifiedSubSpecies.Add(subSpeciesID))
		{
			this.FindSubSpecies(subSpeciesID);
			foreach (object obj in Components.MutantPlants)
			{
				MutantPlant mutantPlant = (MutantPlant)obj;
				if (mutantPlant.HasTag(subSpeciesID))
				{
					mutantPlant.UpdateNameAndTags();
				}
			}
			GeneticAnalysisCompleteMessage geneticAnalysisCompleteMessage = new GeneticAnalysisCompleteMessage(subSpeciesID);
			Messenger.Instance.QueueMessage(geneticAnalysisCompleteMessage);
		}
	}

	public bool IsSubSpeciesIdentified(Tag subSpeciesID)
	{
		return this.identifiedSubSpecies.Contains(subSpeciesID);
	}

	public List<PlantSubSpeciesCatalog.SubSpeciesInfo> GetAllUnidentifiedSubSpecies(Tag speciesID)
	{
		return this.discoveredSubspeciesBySpecies[speciesID].FindAll((PlantSubSpeciesCatalog.SubSpeciesInfo ss) => !this.IsSubSpeciesIdentified(ss.ID));
	}

	public bool IsValidPlantableSeed(Tag seedID, Tag subspeciesID)
	{
		if (!seedID.IsValid)
		{
			return false;
		}
		MutantPlant component = Assets.GetPrefab(seedID).GetComponent<MutantPlant>();
		if (component == null)
		{
			return !subspeciesID.IsValid;
		}
		List<PlantSubSpeciesCatalog.SubSpeciesInfo> allSubSpeciesForSpecies = PlantSubSpeciesCatalog.Instance.GetAllSubSpeciesForSpecies(component.SpeciesID);
		return allSubSpeciesForSpecies != null && allSubSpeciesForSpecies.FindIndex((PlantSubSpeciesCatalog.SubSpeciesInfo s) => s.ID == subspeciesID) != -1 && PlantSubSpeciesCatalog.Instance.IsSubSpeciesIdentified(subspeciesID);
	}

	private void EnsureOriginalSubSpecies()
	{
		foreach (GameObject gameObject in Assets.GetPrefabsWithComponent<MutantPlant>())
		{
			MutantPlant component = gameObject.GetComponent<MutantPlant>();
			Tag speciesID = component.SpeciesID;
			if (!this.discoveredSubspeciesBySpecies.ContainsKey(speciesID))
			{
				this.discoveredSubspeciesBySpecies[speciesID] = new List<PlantSubSpeciesCatalog.SubSpeciesInfo>();
				this.discoveredSubspeciesBySpecies[speciesID].Add(component.GetSubSpeciesInfo());
			}
			this.identifiedSubSpecies.Add(component.SubSpeciesID);
		}
	}

	public static PlantSubSpeciesCatalog Instance;

	[Serialize]
	private Dictionary<Tag, List<PlantSubSpeciesCatalog.SubSpeciesInfo>> discoveredSubspeciesBySpecies = new Dictionary<Tag, List<PlantSubSpeciesCatalog.SubSpeciesInfo>>();

	[Serialize]
	private HashSet<Tag> identifiedSubSpecies = new HashSet<Tag>();

	[Serializable]
	public struct SubSpeciesInfo : IEquatable<PlantSubSpeciesCatalog.SubSpeciesInfo>
	{
		public bool IsValid
		{
			get
			{
				return this.ID.IsValid;
			}
		}

		public bool IsOriginal
		{
			get
			{
				return this.mutationIDs == null || this.mutationIDs.Count == 0;
			}
		}

		public SubSpeciesInfo(Tag speciesID, List<string> mutationIDs)
		{
			this.speciesID = speciesID;
			this.mutationIDs = ((mutationIDs != null) ? new List<string>(mutationIDs) : new List<string>());
			this.ID = PlantSubSpeciesCatalog.SubSpeciesInfo.SubSpeciesIDFromMutations(speciesID, mutationIDs);
		}

		public static Tag SubSpeciesIDFromMutations(Tag speciesID, List<string> mutationIDs)
		{
			if (mutationIDs == null || mutationIDs.Count == 0)
			{
				Tag tag = speciesID;
				return tag.ToString() + "_Original";
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(speciesID);
			foreach (string text in mutationIDs)
			{
				stringBuilder.Append("_");
				stringBuilder.Append(text);
			}
			return stringBuilder.ToString().ToTag();
		}

		public string GetMutationsNames()
		{
			if (this.mutationIDs == null || this.mutationIDs.Count == 0)
			{
				return CREATURES.PLANT_MUTATIONS.NONE.NAME;
			}
			return string.Join(", ", Db.Get().PlantMutations.GetNamesForMutations(this.mutationIDs));
		}

		public string GetNameWithMutations(string properName, bool identified, bool cleanOriginal)
		{
			string text;
			if (this.mutationIDs == null || this.mutationIDs.Count == 0)
			{
				if (cleanOriginal)
				{
					text = properName;
				}
				else
				{
					text = CREATURES.PLANT_MUTATIONS.PLANT_NAME_FMT.Replace("{PlantName}", properName).Replace("{MutationList}", CREATURES.PLANT_MUTATIONS.NONE.NAME);
				}
			}
			else if (!identified)
			{
				text = CREATURES.PLANT_MUTATIONS.PLANT_NAME_FMT.Replace("{PlantName}", properName).Replace("{MutationList}", CREATURES.PLANT_MUTATIONS.UNIDENTIFIED);
			}
			else
			{
				text = CREATURES.PLANT_MUTATIONS.PLANT_NAME_FMT.Replace("{PlantName}", properName).Replace("{MutationList}", string.Join(", ", Db.Get().PlantMutations.GetNamesForMutations(this.mutationIDs)));
			}
			return text;
		}

		public static bool operator ==(PlantSubSpeciesCatalog.SubSpeciesInfo obj1, PlantSubSpeciesCatalog.SubSpeciesInfo obj2)
		{
			return obj1.Equals(obj2);
		}

		public static bool operator !=(PlantSubSpeciesCatalog.SubSpeciesInfo obj1, PlantSubSpeciesCatalog.SubSpeciesInfo obj2)
		{
			return !(obj1 == obj2);
		}

		public override bool Equals(object other)
		{
			return other is PlantSubSpeciesCatalog.SubSpeciesInfo && this == (PlantSubSpeciesCatalog.SubSpeciesInfo)other;
		}

		public bool Equals(PlantSubSpeciesCatalog.SubSpeciesInfo other)
		{
			return this.ID == other.ID;
		}

		public override int GetHashCode()
		{
			return this.ID.GetHashCode();
		}

		public string GetMutationsTooltip()
		{
			if (this.mutationIDs == null || this.mutationIDs.Count == 0)
			{
				return CREATURES.STATUSITEMS.ORIGINALPLANTMUTATION.TOOLTIP;
			}
			if (!PlantSubSpeciesCatalog.Instance.IsSubSpeciesIdentified(this.ID))
			{
				return CREATURES.STATUSITEMS.UNKNOWNMUTATION.TOOLTIP;
			}
			string text = this.mutationIDs[0];
			PlantMutation plantMutation = Db.Get().PlantMutations.Get(text);
			return CREATURES.STATUSITEMS.SPECIFICPLANTMUTATION.TOOLTIP.Replace("{MutationName}", plantMutation.Name) + "\n" + plantMutation.GetTooltip();
		}

		public Tag speciesID;

		public Tag ID;

		public List<string> mutationIDs;

		private const string ORIGINAL_SUFFIX = "_Original";
	}
}
