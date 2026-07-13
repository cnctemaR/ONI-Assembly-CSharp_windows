using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KSerialization;
using UnityEngine;

public class StarmapHexCellInventory : KMonoBehaviour, ISaveLoadable
{
	public static void ClearStatics()
	{
		StarmapHexCellInventory.AllInventories.Clear();
	}

	public int ItemCount
	{
		get
		{
			if (this.Items != null)
			{
				return this.Items.Count;
			}
			return 0;
		}
	}

	public float TotalMass
	{
		get
		{
			return this.ReadTotalMass();
		}
	}

	public bool RegisterInventory(AxialI location)
	{
		StarmapHexCellInventory starmapHexCellInventory = null;
		if (!StarmapHexCellInventory.AllInventories.TryGetValue(location, out starmapHexCellInventory) || starmapHexCellInventory == this)
		{
			StarmapHexCellInventory.AllInventories[location] = this;
			return true;
		}
		return false;
	}

	public void TransferAllItemsFromExternalInventory(StarmapHexCellInventory externalInventory)
	{
		bool flag = false;
		foreach (StarmapHexCellInventory.SerializedItem serializedItem in externalInventory.Items)
		{
			bool flag2 = this.TransferItemFromGroup(serializedItem.ID, serializedItem.Mass, serializedItem.StateMask) != null;
			flag = flag || flag2;
		}
		if (flag)
		{
			base.gameObject.Trigger(-1697596308, null);
		}
		externalInventory.DeleteAll();
	}

	private StarmapHexCellInventory.SerializedItem TransferItemFromGroup(Tag itemID, float mass, Element.State state)
	{
		return this.AddItem(itemID, mass, state, false);
	}

	public StarmapHexCellInventory.SerializedItem AddItem(Element element, float mass)
	{
		return this.AddItem(element.id.CreateTag(), mass, element.state);
	}

	public StarmapHexCellInventory.SerializedItem AddItem(Tag itemID, float mass, Element.State state)
	{
		return this.AddItem(itemID, mass, state, true);
	}

	private StarmapHexCellInventory.SerializedItem AddItem(Tag itemID, float mass, Element.State state, bool triggerStorageChangeCb)
	{
		StarmapHexCellInventory.SerializedItem serializedItem = this.FindItem(itemID);
		if (serializedItem == null)
		{
			serializedItem = new StarmapHexCellInventory.SerializedItem(itemID, 0f, state);
			this.Items.Add(serializedItem);
		}
		serializedItem.Mass += mass;
		if (triggerStorageChangeCb)
		{
			base.gameObject.Trigger(-1697596308, null);
		}
		return serializedItem;
	}

	public PrimaryElement ExtractAndSpawnItem(Tag ID)
	{
		return this.ExtractAndSpawnItemMass(ID, float.MaxValue);
	}

	public PrimaryElement ExtractAndSpawnItemMass(Tag ID, float mass)
	{
		GameObject gameObject = null;
		PrimaryElement primaryElement = null;
		StarmapHexCellInventory.SerializedItem serializedItem = this.FindItem(ID);
		if (serializedItem != null)
		{
			float num = Mathf.Min(mass, serializedItem.Mass);
			Element element = ElementLoader.GetElement(ID);
			Vector3 position = base.transform.GetPosition();
			if (num <= 0f)
			{
				global::Debug.LogWarning("StarmapHexCellInventory.ExtractAndSpawn() found an invalid mass to extract from item ID(" + ID.ToString() + "). If the stored item had zero mass, it will be removed now");
				if (serializedItem.Mass <= 0f)
				{
					this.DeleteItem(serializedItem);
					return null;
				}
			}
			if (element != null)
			{
				if (element.IsGas)
				{
					gameObject = GasSourceManager.Instance.CreateChunk(element, num, element.defaultValues.temperature, byte.MaxValue, 0, position).gameObject;
				}
				else if (element.IsLiquid)
				{
					gameObject = LiquidSourceManager.Instance.CreateChunk(element, num, element.defaultValues.temperature, byte.MaxValue, 0, position).gameObject;
				}
				else if (element.IsSolid)
				{
					gameObject = element.substance.SpawnResource(position, num, element.defaultValues.temperature, byte.MaxValue, 0, true, false, true);
					gameObject.GetComponent<Pickupable>().prevent_absorb_until_stored = true;
					element.substance.ActivateSubstanceGameObject(gameObject, byte.MaxValue, 0);
				}
				primaryElement = gameObject.GetComponent<PrimaryElement>();
				primaryElement.KeepZeroMassObject = false;
			}
			else
			{
				GameObject prefab = Assets.GetPrefab(serializedItem.ID);
				if (!(prefab != null))
				{
					global::Debug.LogWarning("StarmapHexCellInventory.ExtractAndSpawn() found an invalid item ID(" + ID.ToString() + ") stored. Removing from list.");
					this.DeleteItem(serializedItem);
					return null;
				}
				gameObject = Util.KInstantiate(prefab, base.transform.gameObject, null);
				gameObject.transform.SetLocalPosition(position);
				primaryElement = gameObject.GetComponent<PrimaryElement>();
				primaryElement.Units = num;
				gameObject.SetActive(true);
			}
			if (primaryElement != null)
			{
				this.DeleteItemMass(serializedItem, num);
			}
		}
		return primaryElement;
	}

	public float ExtractAndStoreItemMass(Tag ID, float mass, Storage storage)
	{
		StarmapHexCellInventory.SerializedItem serializedItem = this.FindItem(ID);
		if (serializedItem == null)
		{
			return 0f;
		}
		float num = Mathf.Min(mass, serializedItem.Mass);
		if (num <= 0f)
		{
			global::Debug.LogWarning("StarmapHexCellInventory.ExtractAndSpawn() found an invalid mass to extract from item ID(" + ID.ToString() + "). If the stored item had zero mass, it will be removed now");
			if (serializedItem.Mass <= 0f)
			{
				this.DeleteItem(serializedItem);
				return 0f;
			}
		}
		Element element = ElementLoader.GetElement(ID);
		if (element != null)
		{
			storage.AddElement(element.id, num, element.defaultValues.temperature, byte.MaxValue, 0, false, true);
			this.DeleteItemMass(serializedItem, num);
			return num;
		}
		GameObject prefab = Assets.GetPrefab(serializedItem.ID);
		if (prefab != null)
		{
			GameObject gameObject = Util.KInstantiate(prefab, base.transform.gameObject, null);
			gameObject.transform.SetLocalPosition(base.transform.GetPosition());
			gameObject.GetComponent<PrimaryElement>().Units = num;
			gameObject.SetActive(true);
			storage.Store(gameObject, true, false, true, false);
			this.DeleteItemMass(serializedItem, num);
			return num;
		}
		global::Debug.LogWarning("StarmapHexCellInventory.ExtractAndSpawn() found an invalid item ID(" + ID.ToString() + ") stored. Removing from list.");
		this.DeleteItem(serializedItem);
		return 0f;
	}

	private void DeleteAll()
	{
		this.Items.Clear();
		base.gameObject.Trigger(-1697596308, null);
	}

	private void DeleteItem(StarmapHexCellInventory.SerializedItem item)
	{
		this.DeleteItemMass(item, item.Mass);
	}

	private void DeleteItemMass(StarmapHexCellInventory.SerializedItem item, float massToDelete)
	{
		if (item != null)
		{
			item.Mass -= massToDelete;
			if (item.Mass <= 0f)
			{
				this.Items.Remove(item);
			}
			base.gameObject.Trigger(-1697596308, null);
		}
	}

	private void RefreshStatusItems(object data = null)
	{
		base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().MiscStatusItems.ClusterMapHarvestableResource, this.Items);
	}

	private StarmapHexCellInventory.SerializedItem FindItem(Tag id)
	{
		if (this.Items != null)
		{
			return this.Items.Find((StarmapHexCellInventory.SerializedItem i) => i.ID == id);
		}
		return null;
	}

	private float ReadTotalMass()
	{
		if (this.Items == null || this.Items.Count == 0)
		{
			return 0f;
		}
		float num = 0f;
		foreach (StarmapHexCellInventory.SerializedItem serializedItem in this.Items)
		{
			num += serializedItem.Mass;
		}
		return num;
	}

	[OnDeserialized]
	internal void OnDeserializedMethod()
	{
		if (this.Items != null)
		{
			this.Items.RemoveAll((StarmapHexCellInventory.SerializedItem x) => Assets.TryGetPrefab(x.ID) == null);
			foreach (StarmapHexCellInventory.SerializedItem serializedItem in this.Items)
			{
				serializedItem.RecalculateState();
			}
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.Subscribe(-1697596308, new Action<object>(this.RefreshStatusItems));
		this.RefreshStatusItems(null);
	}

	public static Dictionary<AxialI, StarmapHexCellInventory> AllInventories = new Dictionary<AxialI, StarmapHexCellInventory>();

	[Serialize]
	public List<StarmapHexCellInventory.SerializedItem> Items = new List<StarmapHexCellInventory.SerializedItem>();

	[SerializationConfig(MemberSerialization.OptIn)]
	public class SerializedItem
	{
		public Element.State StateMask
		{
			get
			{
				return this.stateMask;
			}
		}

		public bool IsSolid
		{
			get
			{
				return (this.stateMask & Element.State.Solid) > Element.State.Vacuum;
			}
		}

		public bool IsLiquid
		{
			get
			{
				return (this.stateMask & Element.State.Liquid) > Element.State.Vacuum;
			}
		}

		public bool IsGas
		{
			get
			{
				return (this.stateMask & Element.State.Gas) > Element.State.Vacuum;
			}
		}

		public bool IsEntity
		{
			get
			{
				return this.stateMask == Element.State.Vacuum;
			}
		}

		public Element.State ItemMatterState
		{
			get
			{
				return this.stateMask;
			}
		}

		public SerializedItem(Tag id, float mass)
			: this(id, mass, Element.State.Vacuum)
		{
		}

		public SerializedItem(Tag id, float mass, Element.State state)
		{
			this.ID = id;
			this.Mass = mass;
			this.stateMask = state;
		}

		public void RecalculateState()
		{
			Element element = ElementLoader.GetElement(this.ID);
			if (element == null)
			{
				this.stateMask = Element.State.Vacuum;
				return;
			}
			this.stateMask = element.state;
		}

		[Serialize]
		public Tag ID;

		[Serialize]
		public float Mass;

		private Element.State stateMask;
	}
}
