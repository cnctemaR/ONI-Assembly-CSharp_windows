using System;
using System.Collections.Generic;
using System.Linq;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class DiscoveredResources : KMonoBehaviour, ISaveLoadable, ISim4000ms
{
	public static void DestroyInstance()
	{
		DiscoveredResources.Instance = null;
	}

	public event Action<Tag, Tag> OnDiscover;

	public void Discover(Tag tag, Tag categoryTag)
	{
		bool flag = this.Discovered.Add(tag);
		this.DiscoverCategory(categoryTag, tag);
		if (flag)
		{
			if (this.OnDiscover != null)
			{
				this.OnDiscover(categoryTag, tag);
			}
			if (!this.newDiscoveries.ContainsKey(tag))
			{
				this.newDiscoveries.Add(tag, (float)GameClock.Instance.GetCycle() + GameClock.Instance.GetCurrentCycleAsPercentage());
			}
		}
	}

	public void Discover(Tag tag)
	{
		this.Discover(tag, DiscoveredResources.GetCategoryForEntity(Assets.GetPrefab(tag).GetComponent<KPrefabID>()));
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		DiscoveredResources.Instance = this;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.FilterDisabledContent();
	}

	private void FilterDisabledContent()
	{
		HashSet<Tag> hashSet = new HashSet<Tag>();
		foreach (Tag tag in this.Discovered)
		{
			Element element = ElementLoader.GetElement(tag);
			if (element != null && element.disabled)
			{
				hashSet.Add(tag);
			}
			else
			{
				GameObject gameObject = Assets.TryGetPrefab(tag);
				if (gameObject != null && gameObject.HasTag(GameTags.DeprecatedContent))
				{
					hashSet.Add(tag);
				}
				else if (gameObject == null)
				{
					hashSet.Add(tag);
				}
			}
		}
		foreach (Tag tag2 in hashSet)
		{
			this.Discovered.Remove(tag2);
		}
		foreach (KeyValuePair<Tag, HashSet<Tag>> keyValuePair in this.DiscoveredCategories)
		{
			foreach (Tag tag3 in hashSet)
			{
				if (keyValuePair.Value.Contains(tag3))
				{
					keyValuePair.Value.Remove(tag3);
				}
			}
		}
		foreach (string text in new List<string> { "Pacu", "PacuCleaner", "PacuTropical", "PacuBaby", "PacuCleanerBaby", "PacuTropicalBaby" })
		{
			if (this.DiscoveredCategories.ContainsKey(text))
			{
				List<Tag> list = this.DiscoveredCategories[text].ToList<Tag>();
				SolidConsumerMonitor.Def def = Assets.GetPrefab(text).GetDef<SolidConsumerMonitor.Def>();
				foreach (Tag tag4 in list)
				{
					if (def.diet.GetDietInfo(tag4) == null)
					{
						this.DiscoveredCategories[text].Remove(tag4);
					}
				}
			}
		}
		if (this.DiscoveredCategories.ContainsKey(GameTags.IndustrialIngredient))
		{
			foreach (string text2 in new List<string> { "CrabShell", "CrabWoodShell" })
			{
				if (this.DiscoveredCategories[GameTags.IndustrialIngredient].Contains(text2))
				{
					this.DiscoveredCategories[GameTags.IndustrialIngredient].Remove(text2);
					this.DiscoverCategory(GameTags.Organics, text2);
				}
			}
		}
		if (this.DiscoveredCategories.ContainsKey(GameTags.IndustrialIngredient))
		{
			foreach (string text3 in new List<string> { "OrbitalResearchDatabank", "ResearchDatabank" })
			{
				if (this.DiscoveredCategories[GameTags.IndustrialIngredient].Contains(text3))
				{
					this.DiscoveredCategories[GameTags.IndustrialIngredient].Remove(text3);
					this.DiscoverCategory(GameTags.TechComponents, text3);
				}
			}
		}
	}

	public bool CheckAllDiscoveredAreNew()
	{
		foreach (Tag tag in this.Discovered)
		{
			if (!this.newDiscoveries.ContainsKey(tag))
			{
				return false;
			}
		}
		return true;
	}

	private void DiscoverCategory(Tag category_tag, Tag item_tag)
	{
		HashSet<Tag> hashSet;
		if (!this.DiscoveredCategories.TryGetValue(category_tag, out hashSet))
		{
			hashSet = new HashSet<Tag>();
			this.DiscoveredCategories[category_tag] = hashSet;
		}
		hashSet.Add(item_tag);
	}

	public HashSet<Tag> GetDiscovered()
	{
		return this.Discovered;
	}

	public bool IsDiscovered(Tag tag)
	{
		return this.Discovered.Contains(tag) || this.DiscoveredCategories.ContainsKey(tag);
	}

	public bool AnyDiscovered(ICollection<Tag> tags)
	{
		foreach (Tag tag in tags)
		{
			if (this.IsDiscovered(tag))
			{
				return true;
			}
		}
		return false;
	}

	public bool TryGetDiscoveredResourcesFromTag(Tag tag, out HashSet<Tag> resources)
	{
		return this.DiscoveredCategories.TryGetValue(tag, out resources);
	}

	public HashSet<Tag> GetDiscoveredResourcesFromTag(Tag tag)
	{
		HashSet<Tag> hashSet;
		if (this.DiscoveredCategories.TryGetValue(tag, out hashSet))
		{
			return hashSet;
		}
		return new HashSet<Tag>();
	}

	public Dictionary<Tag, HashSet<Tag>> GetDiscoveredResourcesFromTagSet(TagSet tagSet)
	{
		Dictionary<Tag, HashSet<Tag>> dictionary = new Dictionary<Tag, HashSet<Tag>>();
		foreach (Tag tag in tagSet)
		{
			HashSet<Tag> hashSet;
			if (this.DiscoveredCategories.TryGetValue(tag, out hashSet))
			{
				dictionary[tag] = hashSet;
			}
		}
		return dictionary;
	}

	public static Tag GetCategoryForTags(HashSet<Tag> tags)
	{
		Tag tag = Tag.Invalid;
		foreach (Tag tag2 in tags)
		{
			if (GameTags.AllCategories.Contains(tag2) || GameTags.IgnoredMaterialCategories.Contains(tag2))
			{
				tag = tag2;
				break;
			}
		}
		return tag;
	}

	public static Tag GetCategoryForEntity(KPrefabID entity)
	{
		ElementChunk component = entity.GetComponent<ElementChunk>();
		if (component != null)
		{
			return component.GetComponent<PrimaryElement>().Element.materialCategory;
		}
		return DiscoveredResources.GetCategoryForTags(entity.Tags);
	}

	public void Sim4000ms(float dt)
	{
		float num = GameClock.Instance.GetTimeInCycles() + GameClock.Instance.GetCurrentCycleAsPercentage();
		List<Tag> list = new List<Tag>();
		foreach (KeyValuePair<Tag, float> keyValuePair in this.newDiscoveries)
		{
			if (num - keyValuePair.Value > 3f)
			{
				list.Add(keyValuePair.Key);
			}
		}
		foreach (Tag tag in list)
		{
			this.newDiscoveries.Remove(tag);
		}
	}

	public static DiscoveredResources Instance;

	[Serialize]
	private HashSet<Tag> Discovered = new HashSet<Tag>();

	[Serialize]
	private Dictionary<Tag, HashSet<Tag>> DiscoveredCategories = new Dictionary<Tag, HashSet<Tag>>();

	[Serialize]
	public Dictionary<Tag, float> newDiscoveries = new Dictionary<Tag, float>();
}
