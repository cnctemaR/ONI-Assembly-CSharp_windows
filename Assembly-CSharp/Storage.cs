using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Klei;
using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Storage : Workable, ISaveLoadableDetails, IEffectDescriptor
{
	protected Storage()
	{
		base.SetOffsetTable(OffsetGroups.InvertedStandardTable);
		this.showProgressBar = false;
	}

	public bool ShouldOnlyTransferFromLowerPriority
	{
		get
		{
			return this.onlyTransferFromLowerPriority || this.allowItemRemoval;
		}
	}

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

	public void SetDefaultStoredItemModifiers(List<Storage.StoredItemModifier> modifiers)
	{
		this.defaultStoredItemModifers = modifiers;
	}

	public override Workable.AnimInfo GetAnim(Worker worker)
	{
		if (this.useGunForDelivery && worker.usesMultiTool)
		{
			Workable.AnimInfo anim = base.GetAnim(worker);
			anim.smi = new MultitoolController.Instance(this, worker, "store", EffectPrefabs.Instance.PickupEffect);
			return anim;
		}
		return base.GetAnim(worker);
	}

	[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event global::System.Action OnStorageIncreased;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe(1623392196, new Action<object>(this.OnDeath));
		base.Subscribe(1502190696, new Action<object>(this.OnQueueDestroyObject));
		base.Subscribe(-905833192, new Action<object>(this.OnCopySettings));
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Storing;
		this.faceTargetWhenWorking = true;
		this.resetProgressOnStop = true;
		this.synchronizeAnims = false;
	}

	protected override void OnSpawn()
	{
		base.SetWorkTime(1.5f);
		foreach (GameObject gameObject in this.items)
		{
			this.ApplyStoredItemModifiers(gameObject, true, true);
		}
	}

	public GameObject Store(GameObject go, bool hide_popups = false, bool block_events = false, bool do_disease_transfer = true, bool is_deserializing = false)
	{
		if (go == null)
		{
			return null;
		}
		GameObject gameObject = go;
		Pickupable component = go.GetComponent<Pickupable>();
		if (!hide_popups && PopFXManager.Instance != null)
		{
			LocString locString;
			Transform transform;
			if (this.fxPrefix == Storage.FXPrefix.Delivered)
			{
				locString = UI.DELIVERED;
				transform = base.transform;
			}
			else
			{
				locString = UI.PICKEDUP;
				transform = go.transform;
			}
			string text;
			if (!Assets.IsTagCountable(go.PrefabID()))
			{
				text = string.Format(locString, GameUtil.GetFormattedMass(component.TotalAmount, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"), go.GetProperName());
			}
			else
			{
				text = string.Format(locString, (int)component.TotalAmount, go.GetProperName());
			}
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, text, transform, 1.5f, false);
		}
		go.transform.parent = base.transform;
		Vector3 vector = Grid.CellToPosCCC(Grid.PosToCell(this), Grid.SceneLayer.Move);
		vector.z = go.transform.GetPosition().z;
		go.transform.SetPosition(vector);
		if (!block_events && do_disease_transfer)
		{
			this.TransferDiseaseWithObject(go);
		}
		if (!is_deserializing)
		{
			foreach (GameObject gameObject2 in this.items)
			{
				if (gameObject2 != null && component != null && gameObject2.GetComponent<Pickupable>().TryAbsorb(component, hide_popups))
				{
					base.Trigger(-1697596308, go);
					base.Trigger(-778359855, null);
					this.ApplyStoredItemModifiers(go, true, false);
					if (this.OnStorageIncreased != null)
					{
						this.OnStorageIncreased();
					}
					gameObject = gameObject2;
					go = null;
					break;
				}
			}
		}
		if (go != null)
		{
			this.items.Add(go);
			if (!is_deserializing)
			{
				this.ApplyStoredItemModifiers(go, true, false);
			}
			if (!block_events)
			{
				EventSystem.Trigger(go, 856640610, this);
				base.Trigger(-1697596308, go);
				base.Trigger(-778359855, null);
				if (this.OnStorageIncreased != null)
				{
					this.OnStorageIncreased();
				}
			}
			if (this.temperatureAdjuster != null)
			{
				SimTemperatureTransfer component2 = go.GetComponent<SimTemperatureTransfer>();
				this.temperatureAdjuster.Register(component2);
			}
		}
		return gameObject;
	}

	public PrimaryElement AddLiquid(SimHashes element, float mass, float temperature, byte disease_idx, int disease_count, bool keep_zero_mass = false, bool do_disease_transfer = true)
	{
		if (mass <= 0f)
		{
			return null;
		}
		PrimaryElement primaryElement = this.FindPrimaryElement(element);
		if (primaryElement != null)
		{
			float finalTemperature = GameUtil.GetFinalTemperature(primaryElement.Temperature, primaryElement.Mass, temperature, mass);
			primaryElement.KeepZeroMassObject = keep_zero_mass;
			primaryElement.Mass += mass;
			primaryElement.Temperature = finalTemperature;
			primaryElement.AddDisease(disease_idx, disease_count, "Storage.AddLiquid");
			base.Trigger(-1697596308, primaryElement.gameObject);
		}
		else
		{
			SubstanceChunk substanceChunk = LiquidSourceManager.Instance.CreateChunk(element, mass, temperature, disease_idx, disease_count, base.transform.GetPosition());
			primaryElement = substanceChunk.GetComponent<PrimaryElement>();
			primaryElement.KeepZeroMassObject = keep_zero_mass;
			GameObject gameObject = substanceChunk.gameObject;
			bool flag = true;
			this.Store(gameObject, flag, false, do_disease_transfer, false);
		}
		return primaryElement;
	}

	public PrimaryElement AddGasChunk(SimHashes element, float mass, float temperature, byte disease_idx, int disease_count, bool keep_zero_mass, bool do_disease_transfer = true)
	{
		if (mass <= 0f)
		{
			return null;
		}
		PrimaryElement primaryElement = this.FindPrimaryElement(element);
		if (primaryElement != null)
		{
			float mass2 = primaryElement.Mass;
			float finalTemperature = GameUtil.GetFinalTemperature(primaryElement.Temperature, mass2, temperature, mass);
			primaryElement.KeepZeroMassObject = keep_zero_mass;
			primaryElement.SetMassTemperature(mass2 + mass, finalTemperature);
			primaryElement.AddDisease(disease_idx, disease_count, "Storage.AddGasChunk");
			base.Trigger(-1697596308, primaryElement.gameObject);
		}
		else
		{
			SubstanceChunk substanceChunk = GasSourceManager.Instance.CreateChunk(element, mass, temperature, disease_idx, disease_count, base.transform.GetPosition());
			primaryElement = substanceChunk.GetComponent<PrimaryElement>();
			primaryElement.KeepZeroMassObject = keep_zero_mass;
			GameObject gameObject = substanceChunk.gameObject;
			bool flag = true;
			this.Store(gameObject, flag, false, do_disease_transfer, false);
		}
		return primaryElement;
	}

	public void Transfer(Storage target, bool block_events = false, bool hide_popups = false)
	{
		while (this.items.Count > 0)
		{
			this.Transfer(this.items[0], target, block_events, hide_popups);
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
				dest_storage.Store(pickupable.gameObject, hide_popups, block_events, true, false);
				if (!block_events)
				{
					base.Trigger(-1697596308, component2.gameObject);
				}
			}
			else
			{
				this.Transfer(gameObject, dest_storage, block_events, hide_popups);
			}
			return amount;
		}
		return 0f;
	}

	public bool Transfer(GameObject go, Storage target, bool block_events = false, bool hide_popups = false)
	{
		this.items.RemoveAll((GameObject it) => it == null);
		int count = this.items.Count;
		for (int i = 0; i < count; i++)
		{
			if (this.items[i] == go)
			{
				this.items.RemoveAt(i);
				this.ApplyStoredItemModifiers(go, false, false);
				if (this.temperatureAdjuster != null)
				{
					SimTemperatureTransfer component = go.GetComponent<SimTemperatureTransfer>();
					this.temperatureAdjuster.Unregister(component);
				}
				target.Store(go, hide_popups, block_events, true, false);
				if (!block_events)
				{
					base.Trigger(-1697596308, go);
				}
				return true;
			}
		}
		return false;
	}

	public void DropAll(bool empty_containers = false)
	{
		while (this.items.Count > 0)
		{
			GameObject gameObject = this.items[0];
			this.TransferDiseaseWithObject(gameObject);
			this.items.RemoveAt(0);
			if (gameObject != null)
			{
				gameObject.Trigger(1228788923, this);
				bool flag = false;
				if (empty_containers)
				{
					Dumpable component = gameObject.GetComponent<Dumpable>();
					if (component != null && gameObject.GetComponent<PrimaryElement>().Element.IsGas)
					{
						component.Dump();
						flag = true;
					}
				}
				if (!flag)
				{
					Vector3 vector = Grid.CellToPosCCC(Grid.PosToCell(this), Grid.SceneLayer.Ore);
					gameObject.transform.SetPosition(vector);
					KBatchedAnimController component2 = gameObject.GetComponent<KBatchedAnimController>();
					if (component2)
					{
						component2.HackRefreshZOrder();
					}
					this.MakeWorldActive(gameObject);
				}
			}
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
					this.TransferDiseaseWithObject(go);
					go.Trigger(1228788923, this);
					this.MakeWorldActive(go);
					break;
				}
			}
		}
		return go;
	}

	public override void AwardExperience(float work_dt, MinionResume resume)
	{
		resume.AddExperienceIfRole("Hauler", work_dt * ROLES.ACTIVE_EXPERIENCE_VERY_SLOW);
		resume.AddExperienceIfRole(MaterialsManager.ID, work_dt * ROLES.ACTIVE_EXPERIENCE_VERY_SLOW);
	}

	private void TransferDiseaseWithObject(GameObject obj)
	{
		if (obj == null || !this.doDiseaseTransfer || this.primaryElement == null)
		{
			return;
		}
		PrimaryElement component = obj.GetComponent<PrimaryElement>();
		if (component == null)
		{
			return;
		}
		SimUtil.DiseaseInfo invalid = SimUtil.DiseaseInfo.Invalid;
		invalid.idx = component.DiseaseIdx;
		invalid.count = (int)((float)component.DiseaseCount * 0.05f);
		SimUtil.DiseaseInfo invalid2 = SimUtil.DiseaseInfo.Invalid;
		invalid2.idx = this.primaryElement.DiseaseIdx;
		invalid2.count = (int)((float)this.primaryElement.DiseaseCount * 0.05f);
		component.ModifyDiseaseCount(-invalid.count, "Storage.TransferDiseaseWithObject");
		this.primaryElement.ModifyDiseaseCount(-invalid2.count, "Storage.TransferDiseaseWithObject");
		if (invalid.count > 0)
		{
			this.primaryElement.AddDisease(invalid.idx, invalid.count, "Storage.TransferDiseaseWithObject");
		}
		if (invalid2.count > 0)
		{
			component.AddDisease(invalid2.idx, invalid2.count, "Storage.TransferDiseaseWithObject");
		}
	}

	private void MakeWorldActive(GameObject go)
	{
		GameObject folder = SceneOrganizer.Instance.GetFolder(Folder.Loot);
		if (folder != null)
		{
			go.transform.parent = folder.transform;
			base.Trigger(-1697596308, go);
			EventSystem.Trigger(go, 856640610, null);
			this.ApplyStoredItemModifiers(go, false, false);
			if (this.temperatureAdjuster != null)
			{
				SimTemperatureTransfer component = go.GetComponent<SimTemperatureTransfer>();
				this.temperatureAdjuster.Unregister(component);
			}
			if (go != null)
			{
				PrimaryElement component2 = go.GetComponent<PrimaryElement>();
				if (component2 != null && component2.KeepZeroMassObject)
				{
					component2.KeepZeroMassObject = false;
					if (component2.Mass <= 0f)
					{
						Util.KDestroyGameObject(go);
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

	public void ConsumeAllIgnoringDisease()
	{
		while (this.items.Count > 0)
		{
			this.ConsumeIgnoringDisease(this.items[0]);
		}
	}

	public void ConsumeAndGetDisease(Tag tag, float amount, out SimUtil.DiseaseInfo disease_info, out float aggregate_temperature)
	{
		disease_info = SimUtil.DiseaseInfo.Invalid;
		List<GameObject> list = null;
		aggregate_temperature = 0f;
		float num = 0f;
		bool flag = false;
		for (int i = 0; i < this.items.Count; i++)
		{
			GameObject gameObject = this.items[i];
			if (!(gameObject == null))
			{
				if (gameObject.HasTag(tag))
				{
					flag = true;
					PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
					float num2 = Math.Min(component.Units, amount);
					aggregate_temperature = SimUtil.CalculateFinalTemperature(num, aggregate_temperature, num2, component.Temperature);
					SimUtil.DiseaseInfo percentOfDisease = SimUtil.GetPercentOfDisease(component, num2 / component.Units);
					disease_info = SimUtil.CalculateFinalDiseaseInfo(disease_info, percentOfDisease);
					component.Units -= num2;
					component.ModifyDiseaseCount(-percentOfDisease.count, "Storage.ConsumeAndGetDisease");
					if (component.Units <= 0f && !component.KeepZeroMassObject)
					{
						if (list == null)
						{
							list = new List<GameObject>();
						}
						list.Add(gameObject);
					}
					amount -= num2;
					num += num2;
					base.Trigger(-1697596308, gameObject);
					if (amount <= 0f)
					{
						break;
					}
				}
			}
		}
		if (!flag)
		{
			global::Debug.LogWarning("TODO(YOG): Why are the ingredients not in storage?", null);
			aggregate_temperature = base.GetComponent<PrimaryElement>().Temperature;
		}
		if (list != null)
		{
			for (int j = 0; j < list.Count; j++)
			{
				this.items.Remove(list[j]);
				Util.KDestroyGameObject(list[j]);
			}
		}
	}

	public void ConsumeAndGetDisease(Recipe.Ingredient ingredient, out SimUtil.DiseaseInfo disease_info, out float temperature)
	{
		this.ConsumeAndGetDisease(ingredient.tag, ingredient.amount, out disease_info, out temperature);
	}

	public void ConsumeIgnoringDisease(Tag tag, float amount)
	{
		SimUtil.DiseaseInfo diseaseInfo;
		float num;
		this.ConsumeAndGetDisease(tag, amount, out diseaseInfo, out num);
	}

	public void ConsumeIgnoringDisease(GameObject item_go)
	{
		if (this.items.Contains(item_go))
		{
			PrimaryElement component = item_go.GetComponent<PrimaryElement>();
			if (component != null && component.KeepZeroMassObject)
			{
				component.Units = 0f;
				component.ModifyDiseaseCount(-component.DiseaseCount, "consume item");
				base.Trigger(-1697596308, item_go);
			}
			else
			{
				this.items.Remove(item_go);
				base.Trigger(-1697596308, item_go);
				item_go.DeleteObject();
			}
		}
	}

	public GameObject Drop(int ID)
	{
		return this.Drop(this.Find(ID));
	}

	private void OnDeath(object data)
	{
		this.DropAll(true);
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
		return (float)Mathf.RoundToInt(num * 1000f) / 1000f;
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
		foreach (GameObject gameObject in this.items)
		{
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
			base.Trigger(644822890, null);
		}
	}

	protected override void OnCleanUp()
	{
		if (this.items.Count != 0)
		{
			global::Debug.LogWarning("Storage for [" + base.gameObject.name + "] is being destroyed but it still contains items!", base.gameObject);
		}
	}

	private void OnQueueDestroyObject(object data)
	{
		this.endOfLife = true;
		this.DropAll(true);
		this.OnCleanUp();
	}

	public void Remove(GameObject go)
	{
		this.items.Remove(go);
		this.TransferDiseaseWithObject(go);
		base.Trigger(-1697596308, go);
		this.ApplyStoredItemModifiers(go, false, false);
	}

	public float GetAmountAvailable(Tag tag)
	{
		float num = 0f;
		for (int i = 0; i < this.items.Count; i++)
		{
			GameObject gameObject = this.items[i];
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
		for (int i = 0; i < this.items.Count; i++)
		{
			GameObject gameObject = this.items[i];
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
		for (int i = 0; i < this.items.Count; i++)
		{
			GameObject gameObject = this.items[i];
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
			descriptor.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.STORAGECAPACITY, GameUtil.GetFormattedMass(this.Capacity(), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.STORAGECAPACITY, GameUtil.GetFormattedMass(this.Capacity(), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), Descriptor.DescriptorType.Effect);
			list.Add(descriptor);
		}
		return list;
	}

	private static void MakeItemTemperatureInsulated(GameObject go, bool is_stored, bool is_initializing)
	{
		SimTemperatureTransfer component = go.GetComponent<SimTemperatureTransfer>();
		if (component == null)
		{
			return;
		}
		component.enabled = !is_stored;
	}

	private static void MakeItemInvisible(GameObject go, bool is_stored, bool is_initializing)
	{
		if (is_initializing)
		{
			return;
		}
		bool flag = !is_stored;
		KAnimControllerBase component = go.GetComponent<KAnimControllerBase>();
		if (component != null && component.enabled != flag)
		{
			component.enabled = flag;
		}
		KSelectable component2 = go.GetComponent<KSelectable>();
		if (component2 != null && component2.enabled != flag)
		{
			component2.enabled = flag;
		}
	}

	private static void MakeItemSealed(GameObject go, bool is_stored, bool is_initializing)
	{
		if (go != null)
		{
			if (is_stored)
			{
				go.GetComponent<KPrefabID>().AddTag(GameTags.Sealed);
			}
			else
			{
				go.GetComponent<KPrefabID>().RemoveTag(GameTags.Sealed);
			}
		}
	}

	private static void MakeItemPreserved(GameObject go, bool is_stored, bool is_initializing)
	{
		if (go != null)
		{
			if (is_stored)
			{
				go.GetComponent<KPrefabID>().AddTag(GameTags.Preserved);
			}
			else
			{
				go.GetComponent<KPrefabID>().RemoveTag(GameTags.Preserved);
			}
		}
	}

	private void ApplyStoredItemModifiers(GameObject go, bool is_stored, bool is_initializing)
	{
		List<Storage.StoredItemModifier> list = this.defaultStoredItemModifers;
		for (int i = 0; i < list.Count; i++)
		{
			Storage.StoredItemModifier storedItemModifier = list[i];
			for (int j = 0; j < Storage.StoredItemModifierHandlers.Count; j++)
			{
				Storage.StoredItemModifierInfo storedItemModifierInfo = Storage.StoredItemModifierHandlers[j];
				if (storedItemModifierInfo.modifier == storedItemModifier)
				{
					storedItemModifierInfo.toggleState(go, is_stored, is_initializing);
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
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		this.ClearItems();
		int num4 = reader.ReadInt32();
		this.items = new List<GameObject>(num4);
		for (int i = 0; i < num4; i++)
		{
			float realtimeSinceStartup2 = Time.realtimeSinceStartup;
			string text = reader.ReadKleiString();
			Tag tag = TagManager.Create(text, null);
			SaveLoadRoot saveLoadRoot = SaveLoadRoot.Load(tag, reader, true);
			num += Time.realtimeSinceStartup - realtimeSinceStartup2;
			if (saveLoadRoot != null)
			{
				float realtimeSinceStartup3 = Time.realtimeSinceStartup;
				GameObject gameObject = this.Store(saveLoadRoot.gameObject, true, true, false, true);
				num2 += Time.realtimeSinceStartup - realtimeSinceStartup3;
				if (gameObject != null)
				{
					float realtimeSinceStartup4 = Time.realtimeSinceStartup;
					gameObject.GetComponent<Pickupable>().OnStore(this);
					num3 += Time.realtimeSinceStartup - realtimeSinceStartup4;
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

	// Note: this type is marked as 'beforefieldinit'.
	static Storage()
	{
		List<Storage.StoredItemModifierInfo> list = new List<Storage.StoredItemModifierInfo>();
		list.Add(new Storage.StoredItemModifierInfo(Storage.StoredItemModifier.Hide, new Action<GameObject, bool, bool>(Storage.MakeItemInvisible)));
		list.Add(new Storage.StoredItemModifierInfo(Storage.StoredItemModifier.Insulate, new Action<GameObject, bool, bool>(Storage.MakeItemTemperatureInsulated)));
		list.Add(new Storage.StoredItemModifierInfo(Storage.StoredItemModifier.Seal, new Action<GameObject, bool, bool>(Storage.MakeItemSealed)));
		list.Add(new Storage.StoredItemModifierInfo(Storage.StoredItemModifier.Preserve, new Action<GameObject, bool, bool>(Storage.MakeItemPreserved)));
		Storage.StoredItemModifierHandlers = list;
		Storage.StandardSealedStorage = new List<Storage.StoredItemModifier>
		{
			Storage.StoredItemModifier.Hide,
			Storage.StoredItemModifier.Seal
		};
		Storage.StandardFabricatorStorage = new List<Storage.StoredItemModifier>
		{
			Storage.StoredItemModifier.Hide,
			Storage.StoredItemModifier.Preserve
		};
	}

	public bool allowItemRemoval;

	public bool onlyTransferFromLowerPriority;

	public bool allowSublimation = true;

	public float capacityKg = 20000f;

	public bool showInUI = true;

	public bool showDescriptor;

	public bool doDiseaseTransfer = true;

	public List<Tag> storageFilters;

	public bool useGunForDelivery = true;

	public int storageNetworkID = -1;

	public Storage.FXPrefix fxPrefix;

	public List<GameObject> items = new List<GameObject>();

	[MyCmpAdd]
	protected UserMenu userMenu;

	[MyCmpGet]
	public Prioritizable prioritizable;

	[MyCmpGet]
	public Automatable automatable;

	[MyCmpGet]
	protected PrimaryElement primaryElement;

	public bool dropOnLoad;

	protected float maxKGPerItem = float.MaxValue;

	private bool endOfLife;

	[Serialize]
	private bool onlyFetchMarkedItems;

	[NonSerialized]
	public SimulatedTemperatureAdjuster temperatureAdjuster;

	private static readonly List<Storage.StoredItemModifierInfo> StoredItemModifierHandlers;

	[SerializeField]
	private List<Storage.StoredItemModifier> defaultStoredItemModifers = new List<Storage.StoredItemModifier> { Storage.StoredItemModifier.Hide };

	public static readonly List<Storage.StoredItemModifier> StandardSealedStorage;

	public static readonly List<Storage.StoredItemModifier> StandardFabricatorStorage;

	public enum StoredItemModifier
	{
		Insulate,
		Hide,
		Seal,
		Preserve
	}

	public enum FXPrefix
	{
		Delivered,
		PickedUp
	}

	private struct StoredItemModifierInfo
	{
		public StoredItemModifierInfo(Storage.StoredItemModifier modifier, Action<GameObject, bool, bool> toggle_state)
		{
			this.modifier = modifier;
			this.toggleState = toggle_state;
		}

		public Storage.StoredItemModifier modifier;

		public Action<GameObject, bool, bool> toggleState;
	}
}
