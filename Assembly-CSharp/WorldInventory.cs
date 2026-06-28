using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class WorldInventory : KMonoBehaviour, ISaveLoadableJson
{
	public event Action<Tag> OnDiscover;

	public static WorldInventory Instance { get; private set; }

	protected override void OnPrefabInit()
	{
		WorldInventory.Instance = this;
		Components.LiquidSources.Register(new Action<LiquidSource>(this.OnAddLiquidSource), new Action<LiquidSource>(this.OnRemoveLiquidSource));
		base.Subscribe(Game.Instance.gameObject, -1588644844, new EventSystem.EventHandler(this.OnAddedFetchable));
		base.Subscribe(Game.Instance.gameObject, -1491270284, new EventSystem.EventHandler(this.OnRemovedFetchable));
	}

	protected override void OnSpawn()
	{
		this.Prober = MinionGroupProber.Get();
	}

	public void OnAddLiquidSource(LiquidSource source)
	{
		this.PendingLiquidAdds.Add(source);
	}

	public void OnRemoveLiquidSource(LiquidSource source)
	{
		if (this.PendingLiquidAdds.Contains(source))
		{
			this.PendingLiquidAdds.Remove(source);
		}
		List<LiquidSource> list;
		if (this.LiquidSources.TryGetValue(source.GetElementTag(), out list))
		{
			list.Remove(source);
		}
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

	public void Discover(Tag tag)
	{
		if (this.Discovered.Add(tag) && this.OnDiscover != null)
		{
			this.OnDiscover(tag);
		}
	}

	public bool IsDiscovered(Tag tag)
	{
		return this.Discovered.Contains(tag);
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

	public Dictionary<Tag, Tag> GetDiscoveredResourceTags()
	{
		return this.discoveredResourceTags;
	}

	public List<Tag> GetDiscoveredResourcesFromTag(Tag tag)
	{
		List<Tag> list = new List<Tag>();
		foreach (KeyValuePair<Tag, Tag> keyValuePair in this.discoveredResourceTags)
		{
			if (keyValuePair.Value == tag)
			{
				list.Add(keyValuePair.Key);
			}
		}
		return list;
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
				Element element = ElementLoader.GetElement(key);
				if (element != null && !this.discoveredResourceTags.ContainsKey(key))
				{
					DebugUtil.SoftAssert(element.GetMaterialCategoryTag().IsValid, element.name + " was found by worldinventory but doesn't have a category! Add it to the element definition.");
					this.discoveredResourceTags[key] = element.GetMaterialCategoryTag();
				}
				for (int i = 0; i < value.Count; i++)
				{
					Pickupable pickupable = value[i];
					if (pickupable != null && (pickupable.storage == null || pickupable.storage.allowItemRemoval))
					{
						num2 += pickupable.TotalAmount;
					}
				}
				this.accessibleAmounts[keyValuePair.Key] = num2;
				this.accessibleUpdateIndex = (this.accessibleUpdateIndex + 1) % this.Inventory.Count;
				break;
			}
			num++;
		}
		for (int j = this.PendingLiquidAdds.Count - 1; j >= 0; j--)
		{
			Tag elementTag = this.PendingLiquidAdds[j].GetElementTag();
			List<LiquidSource> list;
			if (!this.LiquidSources.TryGetValue(elementTag, out list))
			{
				this.Discover(elementTag);
				list = new List<LiquidSource>();
				this.LiquidSources[elementTag] = list;
			}
			list.Add(this.PendingLiquidAdds[j]);
			this.PendingLiquidAdds.RemoveAt(j);
		}
		foreach (KeyValuePair<Tag, List<LiquidSource>> keyValuePair2 in this.LiquidSources)
		{
			List<LiquidSource> value2 = keyValuePair2.Value;
			for (int k = value2.Count - 1; k >= 0; k--)
			{
				if (value2[k] == null)
				{
					value2.RemoveAt(k);
				}
				else if (!this.discoveredResourceTags.ContainsKey(value2[k].GetElementTag()))
				{
					Element element2 = ElementLoader.FindElementByHash(value2[k].GetElementID());
					DebugUtil.SoftAssert(element2.GetMaterialCategoryTag().IsValid, element2.name + " was found by worldinventory but doesn't have a category! Add it to the element definition.");
					this.discoveredResourceTags[element2.tag] = element2.GetMaterialCategoryTag();
				}
			}
		}
		this.firstUpdate = false;
	}

	protected override void OnCleanUp()
	{
		Components.LiquidSources.Unregister(new Action<LiquidSource>(this.OnAddLiquidSource), new Action<LiquidSource>(this.OnRemoveLiquidSource));
	}

	private void OnAddedFetchable(object data)
	{
		GameObject gameObject = (GameObject)data;
		Pickupable component = gameObject.GetComponent<Pickupable>();
		KPrefabID component2 = component.GetComponent<KPrefabID>();
		for (int i = 0; i < component2.Tags.Length; i++)
		{
			Tag tag = component2.Tags[i];
			List<Pickupable> list;
			if (!this.Inventory.TryGetValue(tag, out list))
			{
				this.Discover(tag);
				list = new List<Pickupable>();
				this.Inventory[tag] = list;
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

	[Serialize]
	private HashSet<Tag> Discovered = new HashSet<Tag>();

	[Serialize]
	private Dictionary<Tag, Tag> discoveredResourceTags = new Dictionary<Tag, Tag>();

	private Dictionary<Tag, List<LiquidSource>> LiquidSources = new Dictionary<Tag, List<LiquidSource>>();

	private List<LiquidSource> PendingLiquidAdds = new List<LiquidSource>();

	private Dictionary<Tag, List<Pickupable>> Inventory = new Dictionary<Tag, List<Pickupable>>();

	private MinionGroupProber Prober;

	private Dictionary<Tag, float> accessibleAmounts = new Dictionary<Tag, float>();

	private int accessibleUpdateIndex;

	private bool firstUpdate = true;
}
