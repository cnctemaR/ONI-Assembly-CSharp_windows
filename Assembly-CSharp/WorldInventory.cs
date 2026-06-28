using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class WorldInventory : KMonoBehaviour, ISaveLoadable
{
	public static WorldInventory Instance { get; private set; }

	[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<Tag> OnDiscover;

	protected override void OnPrefabInit()
	{
		WorldInventory.Instance = this;
		base.Subscribe(Game.Instance.gameObject, -1588644844, new Action<object>(this.OnAddedFetchable));
		base.Subscribe(Game.Instance.gameObject, -1491270284, new Action<object>(this.OnRemovedFetchable));
	}

	protected override void OnSpawn()
	{
		this.Prober = MinionGroupProber.Get();
		base.StartCoroutine(this.InitialRefresh());
	}

	private IEnumerator InitialRefresh()
	{
		for (int i = 0; i < 1; i++)
		{
			yield return null;
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
		float num = this.GetTotalAmount(tag);
		num -= MaterialNeeds.Instance.GetAmount(tag);
		return Mathf.Max(num, 0f);
	}

	public void Discover(Tag tag, Tag categoryTag)
	{
		bool flag = this.Discovered.Add(tag);
		this.DiscoverCategory(categoryTag, tag);
		if (flag && this.OnDiscover != null)
		{
			this.OnDiscover(tag);
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
					if (pickupable != null && (pickupable.storage == null || pickupable.storage.allowItemRemoval || pickupable.storage.countAsAccessible))
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
			Tag tag2 = Tag.Invalid;
			for (int i = 0; i < component2.Tags.Length; i++)
			{
				if (GameTags.AllCategories.Contains(component2.Tags[i]))
				{
					tag2 = component2.Tags[i];
					break;
				}
			}
			if (!tag2.IsValid)
			{
				DebugUtil.SoftAssert(false, component.name + " was found by worldinventory but doesn't have a category! Add it to the element definition.");
			}
			this.Discover(tag, tag2);
		}
		for (int j = 0; j < component2.Tags.Length; j++)
		{
			Tag tag3 = component2.Tags[j];
			List<Pickupable> list;
			if (!this.Inventory.TryGetValue(tag3, out list))
			{
				list = new List<Pickupable>();
				this.Inventory[tag3] = list;
			}
			list.Add(component);
		}
	}

	private void OnRemovedFetchable(object data)
	{
		GameObject gameObject = (GameObject)data;
		Pickupable component = gameObject.GetComponent<Pickupable>();
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

	private int accessibleUpdateIndex;

	private bool firstUpdate = true;
}
