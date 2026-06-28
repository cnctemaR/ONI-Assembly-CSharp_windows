using System;
using System.Collections.Generic;
using System.IO;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Storage : Workable, ISaveLoadableDetailJson, IEffectDescriptor
{
	protected Storage()
	{
		base.SetOffsetTable(OffsetGroups.InvertedStandardTable);
		this.showProgressBar = false;
	}

	public event global::System.Action OnStorageIncreased;

	public int DescriptionOrder { get; set; }

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
		this.Subscribe(1623392196, new EventSystem.EventHandler(this.OnDeath));
		this.Subscribe(1502190696, new EventSystem.EventHandler(this.OnQueueDestroyObject));
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Storing;
		this.faceTargetWhenWorking = false;
		this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_equip") };
		if (this.choreType == null)
		{
			this.choreType = Db.Get().ChoreTypes.Fetch;
		}
	}

	protected override void OnSpawn()
	{
		base.SetWorkTime(0.25f);
		foreach (Storage.StoredItemModifierInfo storedItemModifierInfo in this.defaultStoredItemModifers)
		{
			foreach (GameObject gameObject in this.items)
			{
				storedItemModifierInfo.initialize(gameObject, true);
			}
		}
	}

	public override string[] GetWorkAnims(Worker worker)
	{
		return Storage.WorkAnims;
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
			if (!component.WholeUnitsOnly)
			{
				text = string.Format(locString, GameUtil.GetFormattedMass(component.TotalAmount, GameUtil.TimeSlice.None, true, "F1"), go.GetComponent<KSelectable>().GetName());
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
			if (gameObject2 != null && component != null && gameObject2.GetComponent<Pickupable>().TryAbsorb(go))
			{
				this.Trigger(-1697596308, go);
				this.ApplyStoredItemModifiers(go);
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
				this.ApplyStoredItemModifiers(go);
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

	public void AddGasChunk(SimHashes element, float mass, float temperature, bool keep_zero_mass)
	{
		if (mass <= 0f)
		{
			return;
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
				this.ApplyStoredItemModifiers(go);
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
			this.ApplyStoredItemModifiers(go);
		}
	}

	public List<GameObject> Find(Tag tag)
	{
		List<GameObject> list = new List<GameObject>();
		foreach (GameObject gameObject in this.items)
		{
			if (!(gameObject == null))
			{
				if (gameObject.HasTag(tag))
				{
					list.Add(gameObject);
				}
			}
		}
		return list;
	}

	public List<Tag> GetAllTagsInStorage()
	{
		List<Tag> list = new List<Tag>();
		foreach (GameObject gameObject in this.items)
		{
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
		foreach (GameObject gameObject in this.items)
		{
			if (gameObject.HasTags(tags))
			{
				list.Add(gameObject);
			}
		}
		return list;
	}

	public GameObject Find(int ID)
	{
		foreach (GameObject gameObject in this.items)
		{
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
		List<Pickupable> list = new List<Pickupable>();
		foreach (GameObject gameObject in this.items)
		{
			if (!(gameObject == null))
			{
				if (gameObject.HasTag(tag))
				{
					Pickupable component = gameObject.GetComponent<Pickupable>();
					float num = Math.Min(component.TotalAmount, amount);
					component.TotalAmount -= num;
					if (component.TotalAmount <= 0f)
					{
						list.Add(component);
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
		foreach (Pickupable pickupable in list)
		{
			global::UnityEngine.Object.DestroyImmediate(pickupable.gameObject);
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
			Debug.LogError("Storage for [" + base.gameObject.name + "] is being destroyed but it still contains items!", base.gameObject);
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
		this.ApplyStoredItemModifiers(go);
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

	public List<Descriptor> GetRequirementDescriptions(BuildingDef def)
	{
		return null;
	}

	public List<Descriptor> GetEffectDescriptions(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (this.showDescriptor)
		{
			Descriptor descriptor = default(Descriptor);
			descriptor.SetupDescriptor(string.Format(UI.LISTENTRYSTRINGNOLINEBREAK, string.Format(UI.BUILDINGEFFECTS.STORAGECAPACITY, GameUtil.GetFormattedMass(this.Capacity(), GameUtil.TimeSlice.None, true, "F1"))), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.STORAGECAPACITY, GameUtil.GetFormattedMass(this.Capacity(), GameUtil.TimeSlice.None, true, "F1")));
			list.Add(descriptor);
		}
		return list;
	}

	public void DisableDefaultItemModifer(Storage.StoredItemModifier modifier)
	{
		this.defaultStoredItemModifers = new List<Storage.StoredItemModifierInfo>(this.defaultStoredItemModifers);
		this.defaultStoredItemModifers.RemoveAll((Storage.StoredItemModifierInfo x) => x.modifier == modifier);
	}

	private static void MakeContainedItemTemperatureInsulated(IList<GameObject> items, GameObject go)
	{
		if (go == null)
		{
			return;
		}
		bool flag = items.Contains(go);
		Storage.MakeItemTemperatureInsulated(go, flag);
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

	private static void MakeContainedItemInvisible(IList<GameObject> items, GameObject go)
	{
		if (go == null)
		{
			return;
		}
		bool flag = items.Contains(go);
		Storage.MakeItemInvisible(go, flag);
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

	private void ApplyStoredItemModifiers(GameObject go)
	{
		foreach (Storage.StoredItemModifierInfo storedItemModifierInfo in this.defaultStoredItemModifers)
		{
			storedItemModifierInfo.callback(this.items, go);
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
				Output.LogError(new object[] { "Tried to deserialize", tag, "into storage but failed" });
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

	public float capacityKg = 20000f;

	public bool disableOnStore = true;

	public bool showInUI = true;

	public bool showDescriptor;

	public bool allowSublimation = true;

	public List<Tag> storageFilters;

	public ChoreType choreType;

	public Storage.FXPrefix fxPrefix;

	public List<GameObject> items = new List<GameObject>();

	private static readonly string[] WorkAnims = new string[] { "working_pre", "working_loop" };

	[MyCmpAdd]
	protected UserMenu userMenu;

	[MyCmpGet]
	public Prioritizable prioritizable;

	public bool dropOnLoad;

	protected float maxKGPerItem = float.MaxValue;

	private bool endOfLife;

	[Serialize]
	private bool onlyFetchMarkedItems;

	private static readonly List<Storage.StoredItemModifierInfo> DefaultStoredItemModifiers = new List<Storage.StoredItemModifierInfo>
	{
		new Storage.StoredItemModifierInfo(Storage.StoredItemModifier.Insulate, new Action<GameObject, bool>(Storage.MakeItemTemperatureInsulated), new Action<IList<GameObject>, GameObject>(Storage.MakeContainedItemTemperatureInsulated)),
		new Storage.StoredItemModifierInfo(Storage.StoredItemModifier.Hide, new Action<GameObject, bool>(Storage.MakeItemInvisible), new Action<IList<GameObject>, GameObject>(Storage.MakeContainedItemInvisible))
	};

	private List<Storage.StoredItemModifierInfo> defaultStoredItemModifers = Storage.DefaultStoredItemModifiers;

	public enum StoredItemModifier
	{
		Insulate,
		Hide
	}

	public enum FXPrefix
	{
		Delivered,
		PickedUp
	}

	private struct StoredItemModifierInfo
	{
		public StoredItemModifierInfo(Storage.StoredItemModifier modifier, Action<GameObject, bool> initialize, Action<IList<GameObject>, GameObject> callback)
		{
			this.modifier = modifier;
			this.initialize = initialize;
			this.callback = callback;
		}

		public Storage.StoredItemModifier modifier;

		public Action<GameObject, bool> initialize;

		public Action<IList<GameObject>, GameObject> callback;
	}
}
