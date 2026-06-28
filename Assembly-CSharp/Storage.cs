using System;
using System.Collections.Generic;
using System.IO;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Storage : Workable, ISaveLoadableDetails, IEffectDescriptor
{
	protected Storage()
	{
		base.SetOffsetTable(OffsetGroups.InvertedStandardTable);
		this.showProgressBar = false;
	}

	public event global::System.Action OnStorageIncreased;

	public GameObject this[int idx]
	{
		get
		{
			return this.items[idx];
		}
	}

	public IEnumerator<GameObject> GetEnumerator()
	{
		return this.items.GetEnumerator();
	}

	public int Count
	{
		get
		{
			return this.items.Count;
		}
	}

	public override Workable.AnimInfo GetAnim(Worker worker)
	{
		Workable.AnimInfo anim = base.GetAnim(worker);
		anim.smi = new MultitoolController.Instance(this, worker, "build", EffectPrefabs.Instance.BuildEffect);
		return anim;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.Subscribe(1623392196, new Action<object>(this.OnDeath));
		this.Subscribe(1502190696, new Action<object>(this.OnQueueDestroyObject));
		this.Subscribe(-905833192, new Action<object>(this.OnCopySettings));
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Storing;
		this.faceTargetWhenWorking = false;
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_equip_kanim") };
		if (this.choreType == null)
		{
			this.choreType = Db.Get().ChoreTypes.Fetch;
		}
	}

	protected override void OnSpawn()
	{
		base.SetWorkTime(0.25f);
		foreach (GameObject gameObject in this.items)
		{
			this.ApplyStoredItemModifiers(gameObject, true);
		}
	}

	public GameObject Store(GameObject go, bool hide_popups = false, bool block_events = false)
	{
		GameObject gameObject = go;
		if (go == null)
		{
			return null;
		}
		Debug.Assert(!this.endOfLife, string.Concat(new string[]
		{
			"Storage [",
			base.gameObject.name,
			"] is being destroyed but an object [",
			go.name,
			"] is being added"
		}));
		Pickupable component = go.GetComponent<Pickupable>();
		if (!hide_popups && PopFXManager.Instance != null)
		{
			LocString locString;
			Transform transform;
			if (this.fxPrefix == Storage.FXPrefix.Delivered)
			{
				locString = UI.DELIVERED;
				transform = this.transform;
			}
			else
			{
				locString = UI.PICKEDUP;
				transform = go.transform;
			}
			string text;
			if (!component.CountableUnits)
			{
				text = string.Format(locString, GameUtil.GetFormattedMass(component.TotalAmount, GameUtil.TimeSlice.None, true, "{0:0.#}"), go.GetComponent<KSelectable>().GetName());
			}
			else
			{
				text = string.Format(locString, (int)component.TotalAmount, go.GetComponent<KSelectable>().GetName());
			}
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, text, transform, 1.5f, false);
		}
		go.transform.parent = this.transform;
		Vector3 vector = Grid.CellToPosCCC(Grid.PosToCell(this), Grid.SceneLayer.Move);
		vector.z = go.transform.position.z;
		go.transform.SetPosition(vector);
		foreach (GameObject gameObject2 in this.items)
		{
			if (gameObject2 != null && component != null && gameObject2.GetComponent<Pickupable>().TryAbsorb(component, hide_popups))
			{
				this.Trigger(-1697596308, go);
				this.ApplyStoredItemModifiers(go, true);
				if (this.OnStorageIncreased != null)
				{
					this.OnStorageIncreased();
				}
				gameObject = gameObject2;
				go = null;
				break;
			}
		}
		if (go != null)
		{
			if (this.disableOnStore)
			{
				go.GetComponent<KAnimControllerBase>().enabled = false;
			}
			this.items.Add(go);
			if (!block_events)
			{
				EventSystem.Trigger(go, 856640610, this);
				this.Trigger(-1697596308, go);
				this.ApplyStoredItemModifiers(go, true);
				if (this.OnStorageIncreased != null)
				{
					this.OnStorageIncreased();
				}
			}
		}
		return gameObject;
	}

	public PrimaryElement AddLiquid(SimHashes element, float mass, float temperature, bool keep_zero_mass = false)
	{
		if (mass <= 0f)
		{
			return null;
		}
		PrimaryElement primaryElement = this.FindPrimaryElement(element);
		if (primaryElement != null)
		{
			primaryElement.Temperature = GameUtil.GetFinalTemperature(primaryElement.Temperature, primaryElement.Mass, temperature, mass);
			primaryElement.KeepZeroMassObject = keep_zero_mass;
			primaryElement.Mass += mass;
			this.Trigger(-1697596308, primaryElement.gameObject);
		}
		else
		{
			SubstanceChunk substanceChunk = LiquidSourceManager.Instance.CreateChunk(element, mass, temperature, this.transform.position);
			primaryElement = substanceChunk.GetComponent<PrimaryElement>();
			primaryElement.KeepZeroMassObject = keep_zero_mass;
			this.Store(substanceChunk.gameObject, true, false);
		}
		return primaryElement;
	}

	public PrimaryElement AddGasChunk(SimHashes element, float mass, float temperature, bool keep_zero_mass)
	{
		if (mass <= 0f)
		{
			return null;
		}
		PrimaryElement primaryElement = this.FindPrimaryElement(element);
		if (primaryElement != null)
		{
			primaryElement.Temperature = GameUtil.GetFinalTemperature(primaryElement.Temperature, primaryElement.Mass, temperature, mass);
			primaryElement.KeepZeroMassObject = true;
			primaryElement.Mass += mass;
			this.Trigger(-1697596308, primaryElement.gameObject);
		}
		else
		{
			SubstanceChunk substanceChunk = GasSourceManager.Instance.CreateChunk(element, mass, temperature, this.transform.position);
			primaryElement = substanceChunk.GetComponent<PrimaryElement>();
			primaryElement.KeepZeroMassObject = keep_zero_mass;
			this.Store(substanceChunk.gameObject, true, false);
		}
		return primaryElement;
	}

	public void Transfer(Storage target, bool hide_popups = false)
	{
		while (this.items.Count > 0)
		{
			this.Transfer(this.items[0], target, hide_popups);
		}
	}

	public float Transfer(Storage dest_storage, Tag tag, float amount, bool block_events = false, bool hide_popups = false)
	{
		List<GameObject> list = this.Find(tag);
		if (list.Count > 0)
		{
			GameObject gameObject = list[0];
			PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
			if (amount < component.Units)
			{
				Pickupable component2 = gameObject.GetComponent<Pickupable>();
				Pickupable pickupable = component2.Take(amount);
				dest_storage.Store(pickupable.gameObject, hide_popups, block_events);
				this.Trigger(-1697596308, component2.gameObject);
			}
			else
			{
				this.Transfer(gameObject, dest_storage, hide_popups);
			}
			return amount;
		}
		return 0f;
	}

	public bool Transfer(GameObject go, Storage target, bool hide_popups = false)
	{
		this.items.RemoveAll((GameObject it) => it == null);
		int count = this.items.Count;
		for (int i = 0; i < count; i++)
		{
			if (this.items[i] == go)
			{
				this.items.RemoveAt(i);
				this.Trigger(-1697596308, go);
				this.ApplyStoredItemModifiers(go, false);
				target.Store(go, hide_popups, false);
				return true;
			}
		}
		return false;
	}

	public void DropAll()
	{
		while (this.items.Count > 0)
		{
			GameObject gameObject = this.items[0];
			this.items.RemoveAt(0);
			this.MakeWorldActive(gameObject);
		}
	}

	public List<GameObject> Drop(Tag t)
	{
		List<GameObject> list = this.Find(t);
		foreach (GameObject gameObject in list)
		{
			this.Drop(gameObject);
		}
		return list;
	}

	public GameObject Drop(GameObject go)
	{
		if (go != null)
		{
			int count = this.items.Count;
			for (int i = 0; i < count; i++)
			{
				if (go == this.items[i])
				{
					this.items[i] = this.items[count - 1];
					this.items.RemoveAt(count - 1);
					this.MakeWorldActive(go);
					break;
				}
			}
		}
		return go;
	}

	private void MakeWorldActive(GameObject go)
	{
		GameObject folder = SceneOrganizer.Instance.GetFolder(Folder.Loot);
		if (folder != null)
		{
			go.transform.parent = folder.transform;
			this.Trigger(-1697596308, go);
			EventSystem.Trigger(go, 856640610, null);
			this.ApplyStoredItemModifiers(go, false);
			if (go != null)
			{
				PrimaryElement component = go.GetComponent<PrimaryElement>();
				if (component != null && component.KeepZeroMassObject)
				{
					component.KeepZeroMassObject = false;
					if (component.Mass <= 0f)
					{
						global::UnityEngine.Object.Destroy(go);
					}
				}
			}
		}
	}

	public List<GameObject> Find(Tag tag, List<GameObject> result)
	{
		for (int i = 0; i < this.items.Count; i++)
		{
			GameObject gameObject = this.items[i];
			if (!(gameObject == null))
			{
				if (gameObject.HasTag(tag))
				{
					result.Add(gameObject);
				}
			}
		}
		return result;
	}

	public List<GameObject> Find(Tag tag)
	{
		return this.Find(tag, new List<GameObject>());
	}

	public GameObject FindFirst(Tag tag)
	{
		GameObject gameObject = null;
		for (int i = 0; i < this.items.Count; i++)
		{
			GameObject gameObject2 = this.items[i];
			if (!(gameObject2 == null))
			{
				if (gameObject2.HasTag(tag))
				{
					gameObject = gameObject2;
					break;
				}
			}
		}
		return gameObject;
	}

	public PrimaryElement FindFirstWithMass(Tag tag)
	{
		PrimaryElement primaryElement = null;
		for (int i = 0; i < this.items.Count; i++)
		{
			GameObject gameObject = this.items[i];
			if (!(gameObject == null))
			{
				if (gameObject.HasTag(tag))
				{
					PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
					if (component.Mass > 0f)
					{
						primaryElement = component;
						break;
					}
				}
			}
		}
		return primaryElement;
	}

	public List<Tag> GetAllTagsInStorage()
	{
		List<Tag> list = new List<Tag>();
		for (int i = 0; i < this.items.Count; i++)
		{
			GameObject gameObject = this.items[i];
			if (!list.Contains(gameObject.PrefabID()))
			{
				list.Add(gameObject.PrefabID());
			}
		}
		return list;
	}

	public List<GameObject> Find(IList<Tag> tags)
	{
		if (tags == null || tags.Count == 0)
		{
			return null;
		}
		List<GameObject> list = new List<GameObject>();
		for (int i = 0; i < this.items.Count; i++)
		{
			GameObject gameObject = this.items[i];
			if (gameObject.HasTags(tags))
			{
				list.Add(gameObject);
			}
		}
		return list;
	}

	public GameObject Find(int ID)
	{
		for (int i = 0; i < this.items.Count; i++)
		{
			GameObject gameObject = this.items[i];
			if (ID == gameObject.PrefabID().GetHashCode())
			{
				return gameObject;
			}
		}
		return null;
	}

	public void ConsumeAll()
	{
		while (this.items.Count > 0)
		{
			this.Consume(this.items[0].PrefabID());
		}
	}

	public void Consume(Tag tag, float amount)
	{
		List<Pickupable> list = null;
		for (int i = 0; i < this.items.Count; i++)
		{
			GameObject gameObject = this.items[i];
			if (!(gameObject == null))
			{
				if (gameObject.HasTag(tag))
				{
					Pickupable component = gameObject.GetComponent<Pickupable>();
					float num = Math.Min(component.TotalAmount, amount);
					component.TotalAmount -= num;
					if (component.TotalAmount <= 0f)
					{
						PrimaryElement component2 = component.GetComponent<PrimaryElement>();
						if (!component2.KeepZeroMassObject)
						{
							if (list == null)
							{
								list = new List<Pickupable>();
							}
							list.Add(component);
						}
					}
					amount -= num;
					this.Trigger(-1697596308, gameObject);
					if (amount <= 0f)
					{
						break;
					}
				}
			}
		}
		if (list != null)
		{
			for (int j = 0; j < list.Count; j++)
			{
				this.items.Remove(list[j].gameObject);
				global::UnityEngine.Object.Destroy(list[j].gameObject);
			}
		}
	}

	public void Consume(Recipe.Ingredient ingredient)
	{
		this.Consume(ingredient.tag, ingredient.amount);
	}

	public void Consume(Tag tag)
	{
		List<GameObject> list = this.Find(tag);
		foreach (GameObject gameObject in list)
		{
			this.items.Remove(gameObject);
			this.Trigger(-1697596308, gameObject);
			gameObject.DeleteObject();
		}
	}

	public void Consume(GameObject item_go)
	{
		if (this.items.Contains(item_go))
		{
			this.items.Remove(item_go);
			this.Trigger(-1697596308, item_go);
			item_go.DeleteObject();
		}
	}

	public GameObject Drop(int ID)
	{
		return this.Drop(this.Find(ID));
	}

	private void OnDeath(object data)
	{
		this.DropAll();
	}

	public bool IsFull()
	{
		return this.RemainingCapacity() <= 0f;
	}

	public bool IsEmpty()
	{
		return this.items.Count == 0;
	}

	public float Capacity()
	{
		return this.capacityKg;
	}

	public bool IsEndOfLife()
	{
		return this.endOfLife;
	}

	public float MassStored()
	{
		float num = 0f;
		for (int i = 0; i < this.items.Count; i++)
		{
			if (!(this.items[i] == null))
			{
				PrimaryElement component = this.items[i].GetComponent<PrimaryElement>();
				if (component != null)
				{
					num += component.Units * component.MassPerUnit;
				}
			}
		}
		return num;
	}

	public PrimaryElement GetAnyChunk()
	{
		PrimaryElement primaryElement = null;
		int count = this.items.Count;
		if (count > 0)
		{
			for (int i = 0; i < count; i++)
			{
				GameObject gameObject = this.items[i];
				if (!(gameObject == null))
				{
					primaryElement = gameObject.GetComponent<PrimaryElement>();
					if (primaryElement != null && primaryElement.Mass > 0f)
					{
						break;
					}
				}
			}
		}
		return primaryElement;
	}

	public bool Has(Tag tag)
	{
		bool flag = false;
		foreach (GameObject gameObject in this.items)
		{
			PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
			if (component.HasTag(tag) && component.Mass > 0f)
			{
				flag = true;
				break;
			}
		}
		return flag;
	}

	public List<PrimaryElement> FindPrimaryElements(Tag tag)
	{
		List<PrimaryElement> list = new List<PrimaryElement>();
		List<GameObject> list2 = this.Find(tag);
		foreach (GameObject gameObject in list2)
		{
			PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
			if (component != null && component.Mass > 0f)
			{
				list.Add(component);
			}
		}
		return list;
	}

	public PrimaryElement AddToPrimaryElement(SimHashes element, float additional_mass, float temperature)
	{
		PrimaryElement primaryElement = this.FindPrimaryElement(element);
		if (primaryElement != null)
		{
			float finalTemperature = GameUtil.GetFinalTemperature(primaryElement.Temperature, primaryElement.Mass, temperature, additional_mass);
			primaryElement.Mass += additional_mass;
			primaryElement.Temperature = finalTemperature;
		}
		return primaryElement;
	}

	public PrimaryElement FindPrimaryElement(SimHashes element)
	{
		PrimaryElement primaryElement = null;
		int count = this.items.Count;
		if (count > 0)
		{
			for (int i = 0; i < count; i++)
			{
				GameObject gameObject = this.items[i];
				if (!(gameObject == null))
				{
					PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
					if (component.ElementID == element)
					{
						primaryElement = component;
						break;
					}
				}
			}
		}
		return primaryElement;
	}

	public float RemainingCapacity()
	{
		return this.capacityKg - this.MassStored();
	}

	public bool GetOnlyFetchMarkedItems()
	{
		return this.onlyFetchMarkedItems;
	}

	public void SetOnlyFetchMarkedItems(bool is_set)
	{
		if (is_set != this.onlyFetchMarkedItems)
		{
			this.onlyFetchMarkedItems = is_set;
			this.onPriorityChanged.Signal();
		}
	}

	protected override void OnCleanUp()
	{
		if (this.items.Count != 0)
		{
			Debug.LogWarning("Storage for [" + base.gameObject.name + "] is being destroyed but it still contains items!", base.gameObject);
		}
	}

	private void OnQueueDestroyObject(object data)
	{
		this.endOfLife = true;
		this.DropAll();
		this.OnCleanUp();
	}

	public void Remove(GameObject go)
	{
		this.items.Remove(go);
		this.Trigger(-1697596308, go);
		this.ApplyStoredItemModifiers(go, false);
	}

	public float GetAmountAvailable(Tag tag)
	{
		float num = 0f;
		foreach (GameObject gameObject in this.items)
		{
			if (gameObject != null && gameObject.HasTag(tag))
			{
				num += gameObject.GetComponent<PrimaryElement>().Units;
			}
		}
		return num;
	}

	public float GetMassAvailable(Tag tag)
	{
		float num = 0f;
		foreach (GameObject gameObject in this.items)
		{
			if (gameObject != null && gameObject.HasTag(tag))
			{
				num += gameObject.GetComponent<PrimaryElement>().Mass;
			}
		}
		return num;
	}

	public float GetMassAvailable(SimHashes element)
	{
		float num = 0f;
		foreach (GameObject gameObject in this.items)
		{
			if (gameObject != null)
			{
				PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
				if (component.ElementID == element)
				{
					num += component.Mass;
				}
			}
		}
		return num;
	}

	public bool IsMaterialOnStorage(Tag tag, ref float amount)
	{
		foreach (GameObject gameObject in this.items)
		{
			if (gameObject != null)
			{
				Pickupable component = gameObject.GetComponent<Pickupable>();
				if (component != null)
				{
					KPrefabID component2 = component.GetComponent<KPrefabID>();
					if (component2.HasTag(tag))
					{
						amount = component.TotalAmount;
						return true;
					}
				}
			}
		}
		return false;
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (this.showDescriptor)
		{
			Descriptor descriptor = default(Descriptor);
			descriptor.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.STORAGECAPACITY, GameUtil.GetFormattedMass(this.Capacity(), GameUtil.TimeSlice.None, true, "{0:0.#}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.STORAGECAPACITY, GameUtil.GetFormattedMass(this.Capacity(), GameUtil.TimeSlice.None, true, "{0:0.#}")), Descriptor.DescriptorType.Effect);
			list.Add(descriptor);
		}
		return list;
	}

	private static void MakeItemTemperatureInsulated(GameObject go, bool insulate)
	{
		SimTemperatureTransfer component = go.GetComponent<SimTemperatureTransfer>();
		if (component == null)
		{
			return;
		}
		component.enabled = !insulate;
	}

	private static void MakeItemInvisible(GameObject go, bool invisible)
	{
		KAnimControllerBase component = go.GetComponent<KAnimControllerBase>();
		if (component == null)
		{
			return;
		}
		component.enabled = !invisible;
		KSelectable component2 = go.GetComponent<KSelectable>();
		if (component2 != null)
		{
			component2.enabled = !invisible;
		}
	}

	private static void MakeItemSealed(GameObject go, bool seal)
	{
		Sublimates component = go.GetComponent<Sublimates>();
		if (component == null)
		{
			return;
		}
		component.enabled = !seal;
	}

	private void ApplyStoredItemModifiers(GameObject go, bool stored)
	{
		List<Storage.StoredItemModifier> defaultStoredItemModifiers = this.defaultStoredItemModifers;
		if (defaultStoredItemModifiers == null)
		{
			defaultStoredItemModifiers = Storage.DefaultStoredItemModifiers;
		}
		foreach (Storage.StoredItemModifier storedItemModifier in defaultStoredItemModifiers)
		{
			foreach (Storage.StoredItemModifierInfo storedItemModifierInfo in Storage.StoredItemModifierHandlers)
			{
				if (storedItemModifier == storedItemModifierInfo.modifier)
				{
					storedItemModifierInfo.toggleState(go, stored);
					break;
				}
			}
		}
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		Storage component = gameObject.GetComponent<Storage>();
		if (component != null)
		{
			this.SetOnlyFetchMarkedItems(component.onlyFetchMarkedItems);
		}
	}

	private bool ShouldSaveItem(GameObject go)
	{
		bool flag = false;
		if (go != null && go.GetComponent<SaveLoadRoot>() != null)
		{
			PrimaryElement component = go.GetComponent<PrimaryElement>();
			if (component.Mass > 0f)
			{
				flag = true;
			}
		}
		return flag;
	}

	public void Serialize(BinaryWriter writer)
	{
		int num = 0;
		int count = this.items.Count;
		for (int i = 0; i < count; i++)
		{
			if (this.ShouldSaveItem(this.items[i]))
			{
				num++;
			}
		}
		writer.Write(num);
		if (num == 0)
		{
			return;
		}
		if (this.items != null && this.items.Count > 0)
		{
			for (int j = 0; j < this.items.Count; j++)
			{
				GameObject gameObject = this.items[j];
				if (this.ShouldSaveItem(gameObject))
				{
					SaveLoadRoot component = gameObject.GetComponent<SaveLoadRoot>();
					if (component != null)
					{
						string name = gameObject.GetComponent<KPrefabID>().GetSaveLoadTag().Name;
						writer.WriteKleiString(name);
						component.Save(writer);
					}
					else
					{
						Output.LogWithObj(gameObject, new object[] { "Tried to save obj in storage but obj has no SaveLoadRoot" });
					}
				}
			}
		}
	}

	public void Deserialize(IReader reader)
	{
		this.ClearItems();
		int num = reader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			string text = reader.ReadKleiString();
			Tag tag = TagManager.Create(text, null);
			SaveLoadRoot saveLoadRoot = SaveLoadRoot.Load(tag, reader);
			if (saveLoadRoot != null)
			{
				GameObject gameObject = this.Store(saveLoadRoot.gameObject, true, true);
				if (gameObject != null)
				{
					gameObject.GetComponent<Pickupable>().OnStore(this);
					if (this.dropOnLoad)
					{
						this.Drop(saveLoadRoot.gameObject);
					}
				}
			}
			else
			{
				Output.LogWarningWithObj(base.gameObject, new object[] { "Tried to deserialize " + tag.ToString() + " into storage but failed" });
			}
		}
	}

	private void ClearItems()
	{
		foreach (GameObject gameObject in this.items)
		{
			gameObject.DeleteObject();
		}
		this.items.Clear();
	}

	public bool allowItemRemoval;

	public bool countAsAccessible;

	public float capacityKg = 20000f;

	public bool disableOnStore = true;

	public bool showInUI = true;

	public bool showDescriptor;

	public bool allowSublimation = true;

	public List<Tag> storageFilters;

	public ChoreType choreType;

	public Storage.FXPrefix fxPrefix;

	public List<GameObject> items = new List<GameObject>();

	[MyCmpAdd]
	protected UserMenu userMenu;

	[MyCmpGet]
	public Prioritizable prioritizable;

	public bool dropOnLoad;

	protected float maxKGPerItem = float.MaxValue;

	private bool endOfLife;

	[Serialize]
	private bool onlyFetchMarkedItems;

	private static readonly List<Storage.StoredItemModifier> DefaultStoredItemModifiers = new List<Storage.StoredItemModifier> { Storage.StoredItemModifier.Hide };

	private static readonly List<Storage.StoredItemModifierInfo> StoredItemModifierHandlers = new List<Storage.StoredItemModifierInfo>
	{
		new Storage.StoredItemModifierInfo(Storage.StoredItemModifier.Insulate, new Action<GameObject, bool>(Storage.MakeItemTemperatureInsulated)),
		new Storage.StoredItemModifierInfo(Storage.StoredItemModifier.Hide, new Action<GameObject, bool>(Storage.MakeItemInvisible)),
		new Storage.StoredItemModifierInfo(Storage.StoredItemModifier.Seal, new Action<GameObject, bool>(Storage.MakeItemSealed))
	};

	[SerializeField]
	public List<Storage.StoredItemModifier> defaultStoredItemModifers;

	public enum StoredItemModifier
	{
		Insulate,
		Hide,
		Seal
	}

	public enum FXPrefix
	{
		Delivered,
		PickedUp
	}

	private struct StoredItemModifierInfo
	{
		public StoredItemModifierInfo(Storage.StoredItemModifier modifier, Action<GameObject, bool> toggle_state)
		{
			this.modifier = modifier;
			this.toggleState = toggle_state;
		}

		public Storage.StoredItemModifier modifier;

		public Action<GameObject, bool> toggleState;
	}
}
