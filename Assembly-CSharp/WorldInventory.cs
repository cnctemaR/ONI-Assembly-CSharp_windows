using System;
using System.Collections;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class WorldInventory : KMonoBehaviour, ISaveLoadable
{
	public static WorldInventory Instance { get; private set; }

	public event Action<Tag, Tag> OnDiscover;

	protected override void OnPrefabInit()
	{
		WorldInventory.Instance = this;
		base.Subscribe(Game.Instance.gameObject, -1588644844, new Action<object>(this.OnAddedFetchable));
		base.Subscribe(Game.Instance.gameObject, -1491270284, new Action<object>(this.OnRemovedFetchable));
		base.Subscribe<WorldInventory>(631075836, WorldInventory.OnNewDayDelegate);
	}

	private void GenerateInventoryReport(object data)
	{
		int num = 0;
		int num2 = 0;
		foreach (object obj in Components.Brains)
		{
			CreatureBrain creatureBrain = obj as CreatureBrain;
			if (creatureBrain != null)
			{
				if (creatureBrain.HasTag(GameTags.Creatures.Wild))
				{
					num++;
					ReportManager.Instance.ReportValue(ReportManager.ReportType.WildCritters, 1f, creatureBrain.GetProperName(), creatureBrain.GetProperName());
				}
				else
				{
					num2++;
					ReportManager.Instance.ReportValue(ReportManager.ReportType.DomesticatedCritters, 1f, creatureBrain.GetProperName(), creatureBrain.GetProperName());
				}
			}
		}
		foreach (Spacecraft spacecraft in SpacecraftManager.instance.GetSpacecraft())
		{
			if (spacecraft.state != Spacecraft.MissionState.Grounded && spacecraft.state != Spacecraft.MissionState.Destroyed)
			{
				ReportManager.Instance.ReportValue(ReportManager.ReportType.RocketsInFlight, 1f, spacecraft.rocketName, null);
			}
		}
	}

	protected override void OnSpawn()
	{
		this.Prober = MinionGroupProber.Get();
		base.StartCoroutine(this.InitialRefresh());
	}

	private IEnumerator InitialRefresh()
	{
		int num;
		for (int i = 0; i < 1; i = num)
		{
			yield return null;
			num = i + 1;
		}
		for (int j = 0; j < Components.Pickupables.Count; j++)
		{
			Pickupable pickupable = Components.Pickupables[j];
			if (pickupable != null)
			{
				ReachabilityMonitor.Instance smi = pickupable.GetSMI<ReachabilityMonitor.Instance>();
				if (smi != null)
				{
					smi.UpdateReachability();
				}
			}
		}
		yield break;
	}

	public bool IsReachable(Pickupable pickupable)
	{
		return this.Prober.IsReachable(pickupable);
	}

	public float GetTotalAmount(Tag tag)
	{
		float num = 0f;
		this.accessibleAmounts.TryGetValue(tag, out num);
		return num;
	}

	public List<Pickupable> GetPickupables(Tag tag)
	{
		List<Pickupable> list = null;
		this.Inventory.TryGetValue(tag, out list);
		return list;
	}

	public List<Tag> GetPickupableTagsFromCategoryTag(Tag t)
	{
		List<Tag> list = new List<Tag>();
		List<Pickupable> pickupables = this.GetPickupables(t);
		if (pickupables != null && pickupables.Count > 0)
		{
			foreach (Pickupable pickupable in pickupables)
			{
				list.AddRange(pickupable.KPrefabID.Tags);
			}
		}
		return list;
	}

	public float GetAmount(Tag tag)
	{
		return Mathf.Max(this.GetTotalAmount(tag) - MaterialNeeds.Instance.GetAmount(tag), 0f);
	}

	public void Discover(Tag tag, Tag categoryTag)
	{
		bool flag = this.Discovered.Add(tag);
		this.DiscoverCategory(categoryTag, tag);
		if (flag && this.OnDiscover != null)
		{
			this.OnDiscover(categoryTag, tag);
		}
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

	public bool Contains(Recipe.Ingredient[] ingredients)
	{
		bool flag = true;
		foreach (Recipe.Ingredient ingredient in ingredients)
		{
			if (this.GetAmount(ingredient.tag) < ingredient.amount)
			{
				flag = false;
				break;
			}
		}
		return flag;
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

	private void Update()
	{
		int num = 0;
		foreach (KeyValuePair<Tag, List<Pickupable>> keyValuePair in this.Inventory)
		{
			if (num == this.accessibleUpdateIndex || this.firstUpdate)
			{
				Tag key = keyValuePair.Key;
				List<Pickupable> value = keyValuePair.Value;
				float num2 = 0f;
				for (int i = 0; i < value.Count; i++)
				{
					Pickupable pickupable = value[i];
					if (pickupable != null && !pickupable.HasTag(GameTags.StoredPrivate))
					{
						num2 += pickupable.TotalAmount;
					}
				}
				this.accessibleAmounts[key] = num2;
				this.accessibleUpdateIndex = (this.accessibleUpdateIndex + 1) % this.Inventory.Count;
				break;
			}
			num++;
		}
		this.firstUpdate = false;
	}

	protected override void OnLoadLevel()
	{
		base.OnLoadLevel();
		WorldInventory.Instance = null;
	}

	public static Tag GetCategoryForTags(HashSet<Tag> tags)
	{
		Tag tag = Tag.Invalid;
		foreach (Tag tag2 in tags)
		{
			if (GameTags.AllCategories.Contains(tag2))
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
		return WorldInventory.GetCategoryForTags(entity.Tags);
	}

	private void OnAddedFetchable(object data)
	{
		GameObject gameObject = (GameObject)data;
		if (gameObject.GetComponent<Health>() != null)
		{
			return;
		}
		Pickupable component = gameObject.GetComponent<Pickupable>();
		KPrefabID component2 = component.GetComponent<KPrefabID>();
		Tag tag = component2.PrefabID();
		if (!this.Inventory.ContainsKey(tag))
		{
			Tag categoryForEntity = WorldInventory.GetCategoryForEntity(component2);
			DebugUtil.DevAssertArgs(categoryForEntity.IsValid, new object[] { component.name, "was found by worldinventory but doesn't have a category! Add it to the element definition." });
			this.Discover(tag, categoryForEntity);
		}
		foreach (Tag tag2 in component2.Tags)
		{
			List<Pickupable> list;
			if (!this.Inventory.TryGetValue(tag2, out list))
			{
				list = new List<Pickupable>();
				this.Inventory[tag2] = list;
			}
			list.Add(component);
		}
	}

	private void OnRemovedFetchable(object data)
	{
		Pickupable component = ((GameObject)data).GetComponent<Pickupable>();
		foreach (Tag tag in component.GetComponent<KPrefabID>().Tags)
		{
			List<Pickupable> list;
			if (this.Inventory.TryGetValue(tag, out list))
			{
				list.Remove(component);
			}
		}
	}

	public Dictionary<Tag, float> GetAccessibleAmounts()
	{
		return this.accessibleAmounts;
	}

	[Serialize]
	private HashSet<Tag> Discovered = new HashSet<Tag>();

	[Serialize]
	private Dictionary<Tag, HashSet<Tag>> DiscoveredCategories = new Dictionary<Tag, HashSet<Tag>>();

	private Dictionary<Tag, List<Pickupable>> Inventory = new Dictionary<Tag, List<Pickupable>>();

	private MinionGroupProber Prober;

	private Dictionary<Tag, float> accessibleAmounts = new Dictionary<Tag, float>();

	private static readonly EventSystem.IntraObjectHandler<WorldInventory> OnNewDayDelegate = new EventSystem.IntraObjectHandler<WorldInventory>(delegate(WorldInventory component, object data)
	{
		component.GenerateInventoryReport(data);
	});

	private int accessibleUpdateIndex;

	private bool firstUpdate = true;
}
