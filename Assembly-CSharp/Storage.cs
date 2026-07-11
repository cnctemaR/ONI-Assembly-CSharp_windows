using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using Klei;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Storage : Workable, ISaveLoadableDetails, IEffectDescriptor
{
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

	public PrioritySetting masterPriority
	{
		get
		{
			if (this.prioritizable)
			{
				return this.prioritizable.GetMasterPriority();
			}
			return Chore.DefaultPrioritySetting;
		}
	}

	public override Workable.AnimInfo GetAnim(Worker worker)
	{
		if (this.useGunForDelivery && worker.usesMultiTool)
		{
			Workable.AnimInfo anim = base.GetAnim(worker);
			anim.smi = new MultitoolController.Instance(this, worker, "store", Assets.GetPrefab(EffectConfigs.OreAbsorbId));
			return anim;
		}
		return base.GetAnim(worker);
	}

	public event global::System.Action OnStorageIncreased;

	protected Storage()
	{
		base.SetOffsetTable(OffsetGroups.InvertedStandardTable);
		this.showProgressBar = false;
		this.faceTargetWhenWorking = true;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe<Storage>(1623392196, Storage.OnDeathDelegate);
		base.Subscribe<Storage>(1502190696, Storage.OnQueueDestroyObjectDelegate);
		base.Subscribe<Storage>(-905833192, Storage.OnCopySettingsDelegate);
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Storing;
		this.resetProgressOnStop = true;
		this.synchronizeAnims = false;
		this.workingPstComplete = null;
		this.workingPstFailed = null;
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		if (!this.allowSettingOnlyFetchMarkedItems)
		{
			this.onlyFetchMarkedItems = false;
		}
		this.UpdateFetchCategory();
	}

	protected override void OnSpawn()
	{
		base.SetWorkTime(1.5f);
		foreach (GameObject gameObject in this.items)
		{
			this.ApplyStoredItemModifiers(gameObject, true, true);
			if (this.sendOnStoreOnSpawn)
			{
				gameObject.Trigger(856640610, this);
			}
		}
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		if (component != null)
		{
			component.SetSymbolVisiblity("sweep", this.onlyFetchMarkedItems);
		}
		Prioritizable component2 = base.GetComponent<Prioritizable>();
		if (component2 != null)
		{
			Prioritizable prioritizable = component2;
			prioritizable.onPriorityChanged = (Action<PrioritySetting>)Delegate.Combine(prioritizable.onPriorityChanged, new Action<PrioritySetting>(this.OnPriorityChanged));
		}
		this.UpdateFetchCategory();
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
				if (gameObject2 != null)
				{
					if (component != null && component.prevent_absorb_until_stored)
					{
						component.prevent_absorb_until_stored = false;
					}
					if (component != null && gameObject2.GetComponent<Pickupable>().TryAbsorb(component, hide_popups, true))
					{
						if (!block_events)
						{
							base.Trigger(-1697596308, go);
							base.Trigger(-778359855, null);
							if (this.OnStorageIncreased != null)
							{
								this.OnStorageIncreased();
							}
						}
						this.ApplyStoredItemModifiers(go, true, false);
						gameObject = gameObject2;
						go = null;
						break;
					}
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
				go.Trigger(856640610, this);
				base.Trigger(-1697596308, go);
				base.Trigger(-778359855, null);
				if (this.OnStorageIncreased != null)
				{
					this.OnStorageIncreased();
				}
			}
		}
		return gameObject;
	}

	public PrimaryElement AddOre(SimHashes element, float mass, float temperature, byte disease_idx, int disease_count, bool keep_zero_mass = false, bool do_disease_transfer = true)
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
			primaryElement.AddDisease(disease_idx, disease_count, "Storage.AddOre");
			base.Trigger(-1697596308, primaryElement.gameObject);
		}
		else
		{
			Element element2 = ElementLoader.FindElementByHash(element);
			GameObject gameObject = element2.substance.SpawnResource(base.transform.GetPosition(), mass, temperature, disease_idx, disease_count, true, false, true);
			gameObject.GetComponent<Pickupable>().prevent_absorb_until_stored = true;
			element2.substance.ActivateSubstanceGameObject(gameObject, disease_idx, disease_count);
			this.Store(gameObject, true, false, do_disease_transfer, false);
		}
		return primaryElement;
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
			this.Store(substanceChunk.gameObject, true, false, do_disease_transfer, false);
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
			this.Store(substanceChunk.gameObject, true, false, do_disease_transfer, false);
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
		GameObject gameObject = this.FindFirst(tag);
		if (gameObject != null)
		{
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
				amount = component.Units;
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

	public void DropAll(Vector3 position, bool vent_gas = false, bool dump_liquid = false, Vector3 offset = default(Vector3), bool do_disease_transfer = true)
	{
		while (this.items.Count > 0)
		{
			GameObject gameObject = this.items[0];
			if (do_disease_transfer)
			{
				this.TransferDiseaseWithObject(gameObject);
			}
			this.items.RemoveAt(0);
			if (gameObject != null)
			{
				bool flag = false;
				if (vent_gas || dump_liquid)
				{
					Dumpable component = gameObject.GetComponent<Dumpable>();
					if (component != null)
					{
						if (vent_gas && gameObject.GetComponent<PrimaryElement>().Element.IsGas)
						{
							component.Dump(position + offset);
							flag = true;
						}
						if (dump_liquid && gameObject.GetComponent<PrimaryElement>().Element.IsLiquid)
						{
							component.Dump(position + offset);
							flag = true;
						}
					}
				}
				if (!flag)
				{
					gameObject.transform.SetPosition(position + offset);
					KBatchedAnimController component2 = gameObject.GetComponent<KBatchedAnimController>();
					if (component2)
					{
						component2.SetSceneLayer(Grid.SceneLayer.Ore);
					}
					this.MakeWorldActive(gameObject);
				}
			}
		}
	}

	public void DropAll(bool vent_gas = false, bool dump_liquid = false, Vector3 offset = default(Vector3), bool do_disease_transfer = true)
	{
		while (this.items.Count > 0)
		{
			GameObject gameObject = this.items[0];
			if (do_disease_transfer)
			{
				this.TransferDiseaseWithObject(gameObject);
			}
			this.items.RemoveAt(0);
			if (gameObject != null)
			{
				bool flag = false;
				if (vent_gas || dump_liquid)
				{
					Dumpable component = gameObject.GetComponent<Dumpable>();
					if (component != null)
					{
						if (vent_gas && gameObject.GetComponent<PrimaryElement>().Element.IsGas)
						{
							component.Dump();
							flag = true;
						}
						if (dump_liquid && gameObject.GetComponent<PrimaryElement>().Element.IsLiquid)
						{
							component.Dump();
							flag = true;
						}
					}
				}
				if (!flag)
				{
					Vector3 vector = Grid.CellToPosCCC(Grid.PosToCell(this), Grid.SceneLayer.Ore) + offset;
					gameObject.transform.SetPosition(vector);
					KBatchedAnimController component2 = gameObject.GetComponent<KBatchedAnimController>();
					if (component2)
					{
						component2.SetSceneLayer(Grid.SceneLayer.Ore);
					}
					this.MakeWorldActive(gameObject);
				}
			}
		}
	}

	public List<GameObject> Drop(Tag t)
	{
		ListPool<GameObject, Storage>.PooledList pooledList = ListPool<GameObject, Storage>.Allocate();
		this.Find(t, pooledList);
		foreach (GameObject gameObject in pooledList)
		{
			this.Drop(gameObject, true);
		}
		pooledList.Recycle();
		return pooledList;
	}

	public GameObject Drop(GameObject go, bool do_disease_transfer = true)
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
					if (do_disease_transfer)
					{
						this.TransferDiseaseWithObject(go);
					}
					this.MakeWorldActive(go);
					break;
				}
			}
		}
		return go;
	}

	public void RenotifyAll()
	{
		this.items.RemoveAll((GameObject it) => it == null);
		foreach (GameObject gameObject in this.items)
		{
			gameObject.Trigger(856640610, this);
		}
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
		go.transform.parent = null;
		base.Trigger(-1697596308, go);
		go.Trigger(856640610, null);
		this.ApplyStoredItemModifiers(go, false, false);
		if (go != null)
		{
			PrimaryElement component = go.GetComponent<PrimaryElement>();
			if (component != null && component.KeepZeroMassObject)
			{
				component.KeepZeroMassObject = false;
				if (component.Mass <= 0f)
				{
					Util.KDestroyGameObject(go);
				}
			}
		}
	}

	public List<GameObject> Find(Tag tag, List<GameObject> result)
	{
		for (int i = 0; i < this.items.Count; i++)
		{
			GameObject gameObject = this.items[i];
			if (!(gameObject == null) && gameObject.HasTag(tag))
			{
				result.Add(gameObject);
			}
		}
		return result;
	}

	public GameObject FindFirst(Tag tag)
	{
		GameObject gameObject = null;
		for (int i = 0; i < this.items.Count; i++)
		{
			GameObject gameObject2 = this.items[i];
			if (!(gameObject2 == null) && gameObject2.HasTag(tag))
			{
				gameObject = gameObject2;
				break;
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
			if (!(gameObject == null) && gameObject.HasTag(tag))
			{
				PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
				if (component.Mass > 0f)
				{
					primaryElement = component;
					break;
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
		for (int i = this.items.Count - 1; i >= 0; i--)
		{
			this.ConsumeIgnoringDisease(this.items[i]);
		}
	}

	public void ConsumeAndGetDisease(Tag tag, float amount, out SimUtil.DiseaseInfo disease_info, out float aggregate_temperature)
	{
		DebugUtil.Assert(tag.IsValid);
		disease_info = SimUtil.DiseaseInfo.Invalid;
		aggregate_temperature = 0f;
		float num = 0f;
		bool flag = false;
		int num2 = 0;
		while (num2 < this.items.Count && amount > 0f)
		{
			GameObject gameObject = this.items[num2];
			if (!(gameObject == null) && gameObject.HasTag(tag))
			{
				PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
				if (component.Units > 0f)
				{
					flag = true;
					float num3 = Math.Min(component.Units, amount);
					global::Debug.Assert(num3 > 0f, "Delta amount was zero, which should be impossible.");
					aggregate_temperature = SimUtil.CalculateFinalTemperature(num, aggregate_temperature, num3, component.Temperature);
					SimUtil.DiseaseInfo percentOfDisease = SimUtil.GetPercentOfDisease(component, num3 / component.Units);
					disease_info = SimUtil.CalculateFinalDiseaseInfo(disease_info, percentOfDisease);
					component.Units -= num3;
					component.ModifyDiseaseCount(-percentOfDisease.count, "Storage.ConsumeAndGetDisease");
					amount -= num3;
					num += num3;
				}
				if (component.Units <= 0f && !component.KeepZeroMassObject)
				{
					if (this.deleted_objects == null)
					{
						this.deleted_objects = new List<GameObject>();
					}
					this.deleted_objects.Add(gameObject);
				}
				base.Trigger(-1697596308, gameObject);
			}
			num2++;
		}
		if (!flag)
		{
			aggregate_temperature = base.GetComponent<PrimaryElement>().Temperature;
		}
		if (this.deleted_objects != null)
		{
			for (int i = 0; i < this.deleted_objects.Count; i++)
			{
				this.items.Remove(this.deleted_objects[i]);
				Util.KDestroyGameObject(this.deleted_objects[i]);
			}
			this.deleted_objects.Clear();
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
				return;
			}
			this.items.Remove(item_go);
			base.Trigger(-1697596308, item_go);
			item_go.DeleteObject();
		}
	}

	public GameObject Drop(int ID)
	{
		return this.Drop(this.Find(ID), true);
	}

	private void OnDeath(object data)
	{
		this.DropAll(true, true, default(Vector3), true);
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

	public float UnitsStored()
	{
		float num = 0f;
		for (int i = 0; i < this.items.Count; i++)
		{
			if (!(this.items[i] == null))
			{
				PrimaryElement component = this.items[i].GetComponent<PrimaryElement>();
				if (component != null)
				{
					num += component.Units;
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
			this.UpdateFetchCategory();
			base.Trigger(644822890, null);
			base.GetComponent<KBatchedAnimController>().SetSymbolVisiblity("sweep", is_set);
		}
	}

	private void UpdateFetchCategory()
	{
		if (this.fetchCategory == Storage.FetchCategory.Building)
		{
			return;
		}
		this.fetchCategory = (this.onlyFetchMarkedItems ? Storage.FetchCategory.StorageSweepOnly : Storage.FetchCategory.GeneralStorage);
	}

	protected override void OnCleanUp()
	{
		if (this.items.Count != 0)
		{
			global::Debug.LogWarning("Storage for [" + base.gameObject.name + "] is being destroyed but it still contains items!", base.gameObject);
		}
		base.OnCleanUp();
	}

	private void OnQueueDestroyObject(object data)
	{
		this.endOfLife = true;
		this.DropAll(true, false, default(Vector3), true);
		this.OnCleanUp();
	}

	public void Remove(GameObject go, bool do_disease_transfer = true)
	{
		this.items.Remove(go);
		if (do_disease_transfer)
		{
			this.TransferDiseaseWithObject(go);
		}
		base.Trigger(-1697596308, go);
		this.ApplyStoredItemModifiers(go, false, false);
	}

	public bool ForceStore(Tag tag, float amount)
	{
		global::Debug.Assert(amount < PICKUPABLETUNING.MINIMUM_PICKABLE_AMOUNT);
		for (int i = 0; i < this.items.Count; i++)
		{
			GameObject gameObject = this.items[i];
			if (gameObject != null && gameObject.HasTag(tag))
			{
				gameObject.GetComponent<PrimaryElement>().Mass += amount;
				return true;
			}
		}
		return false;
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

	public float GetUnitsAvailable(Tag tag)
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
				if (component != null && component.GetComponent<KPrefabID>().HasTag(tag))
				{
					amount = component.TotalAmount;
					return true;
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

	public static void MakeItemTemperatureInsulated(GameObject go, bool is_stored, bool is_initializing)
	{
		SimTemperatureTransfer component = go.GetComponent<SimTemperatureTransfer>();
		if (component == null)
		{
			return;
		}
		component.enabled = !is_stored;
	}

	public static void MakeItemInvisible(GameObject go, bool is_stored, bool is_initializing)
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

	public static void MakeItemSealed(GameObject go, bool is_stored, bool is_initializing)
	{
		if (go != null)
		{
			if (is_stored)
			{
				go.GetComponent<KPrefabID>().AddTag(GameTags.Sealed, false);
				return;
			}
			go.GetComponent<KPrefabID>().RemoveTag(GameTags.Sealed);
		}
	}

	public static void MakeItemPreserved(GameObject go, bool is_stored, bool is_initializing)
	{
		if (go != null)
		{
			if (is_stored)
			{
				go.GetComponent<KPrefabID>().AddTag(GameTags.Preserved, false);
				return;
			}
			go.GetComponent<KPrefabID>().RemoveTag(GameTags.Preserved);
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
		Storage component = ((GameObject)data).GetComponent<Storage>();
		if (component != null)
		{
			this.SetOnlyFetchMarkedItems(component.onlyFetchMarkedItems);
		}
	}

	private void OnPriorityChanged(PrioritySetting priority)
	{
		foreach (GameObject gameObject in this.items)
		{
			gameObject.Trigger(-1626373771, this);
		}
	}

	private bool ShouldSaveItem(GameObject go)
	{
		bool flag = false;
		if (go != null && go.GetComponent<SaveLoadRoot>() != null && go.GetComponent<PrimaryElement>().Mass > 0f)
		{
			flag = true;
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
						global::Debug.Log("Tried to save obj in storage but obj has no SaveLoadRoot", gameObject);
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
			Tag tag = TagManager.Create(reader.ReadKleiString());
			SaveLoadRoot saveLoadRoot = SaveLoadRoot.Load(tag, reader);
			num += Time.realtimeSinceStartup - realtimeSinceStartup2;
			if (saveLoadRoot != null)
			{
				KBatchedAnimController component = saveLoadRoot.GetComponent<KBatchedAnimController>();
				if (component != null)
				{
					component.enabled = false;
				}
				saveLoadRoot.SetRegistered(false);
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
						this.Drop(saveLoadRoot.gameObject, true);
					}
				}
			}
			else
			{
				global::Debug.LogWarning("Tried to deserialize " + tag.ToString() + " into storage but failed", base.gameObject);
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

	public bool ignoreSourcePriority;

	public bool allowItemRemoval;

	public bool onlyTransferFromLowerPriority;

	public bool allowSublimation = true;

	public float capacityKg = 20000f;

	public bool showInUI = true;

	public bool showDescriptor;

	public bool allowUIItemRemoval;

	public bool doDiseaseTransfer = true;

	public List<Tag> storageFilters;

	public bool useGunForDelivery = true;

	public bool sendOnStoreOnSpawn;

	public Storage.FetchCategory fetchCategory;

	public int storageNetworkID = -1;

	public float storageFullMargin;

	public Storage.FXPrefix fxPrefix;

	public List<GameObject> items = new List<GameObject>();

	[MyCmpGet]
	public Prioritizable prioritizable;

	[MyCmpGet]
	public Automatable automatable;

	[MyCmpGet]
	protected PrimaryElement primaryElement;

	public bool dropOnLoad;

	protected float maxKGPerItem = float.MaxValue;

	private bool endOfLife;

	public bool allowSettingOnlyFetchMarkedItems = true;

	[Serialize]
	private bool onlyFetchMarkedItems;

	private static readonly List<Storage.StoredItemModifierInfo> StoredItemModifierHandlers = new List<Storage.StoredItemModifierInfo>
	{
		new Storage.StoredItemModifierInfo(Storage.StoredItemModifier.Hide, new Action<GameObject, bool, bool>(Storage.MakeItemInvisible)),
		new Storage.StoredItemModifierInfo(Storage.StoredItemModifier.Insulate, new Action<GameObject, bool, bool>(Storage.MakeItemTemperatureInsulated)),
		new Storage.StoredItemModifierInfo(Storage.StoredItemModifier.Seal, new Action<GameObject, bool, bool>(Storage.MakeItemSealed)),
		new Storage.StoredItemModifierInfo(Storage.StoredItemModifier.Preserve, new Action<GameObject, bool, bool>(Storage.MakeItemPreserved))
	};

	[SerializeField]
	private List<Storage.StoredItemModifier> defaultStoredItemModifers = new List<Storage.StoredItemModifier> { Storage.StoredItemModifier.Hide };

	public static readonly List<Storage.StoredItemModifier> StandardSealedStorage = new List<Storage.StoredItemModifier>
	{
		Storage.StoredItemModifier.Hide,
		Storage.StoredItemModifier.Seal
	};

	public static readonly List<Storage.StoredItemModifier> StandardFabricatorStorage = new List<Storage.StoredItemModifier>
	{
		Storage.StoredItemModifier.Hide,
		Storage.StoredItemModifier.Preserve
	};

	public static readonly List<Storage.StoredItemModifier> StandardInsulatedStorage = new List<Storage.StoredItemModifier>
	{
		Storage.StoredItemModifier.Hide,
		Storage.StoredItemModifier.Seal,
		Storage.StoredItemModifier.Insulate
	};

	private static readonly EventSystem.IntraObjectHandler<Storage> OnDeathDelegate = new EventSystem.IntraObjectHandler<Storage>(delegate(Storage component, object data)
	{
		component.OnDeath(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Storage> OnQueueDestroyObjectDelegate = new EventSystem.IntraObjectHandler<Storage>(delegate(Storage component, object data)
	{
		component.OnQueueDestroyObject(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Storage> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<Storage>(delegate(Storage component, object data)
	{
		component.OnCopySettings(data);
	});

	private List<GameObject> deleted_objects;

	public enum StoredItemModifier
	{
		Insulate,
		Hide,
		Seal,
		Preserve
	}

	public enum FetchCategory
	{
		Building,
		GeneralStorage,
		StorageSweepOnly
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
