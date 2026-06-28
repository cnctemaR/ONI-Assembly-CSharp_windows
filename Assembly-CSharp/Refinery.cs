using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Klei;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Refinery : Workable, IEffectDescriptor, IHasBuildQueue
{
	public int NumOrders
	{
		get
		{
			return this.userOrders.Count;
		}
	}

	public List<IBuildQueueOrder> Orders
	{
		get
		{
			return this.userOrders.ConvertAll<IBuildQueueOrder>((Refinery.UserOrder o) => o);
		}
	}

	public bool NeedsWorker
	{
		get
		{
			return this.machineOrders.Count > 0 && this.machineOrders[0].fetchList != null && this.machineOrders[0].fetchList.IsComplete;
		}
	}

	public bool HasWorker
	{
		get
		{
			return base.worker != null;
		}
	}

	[OnSerializing]
	internal void OnSerializingMethod()
	{
		this.savedOrders = new List<Refinery.OrderSaveData>();
		for (int i = 0; i < this.userOrders.Count; i++)
		{
			Refinery.UserOrder userOrder = this.userOrders[i];
			this.savedOrders.Add(new Refinery.OrderSaveData(userOrder.recipe.material.Name, userOrder.infinite));
		}
	}

	[OnDeserializing]
	internal void OnDeserializingMethod()
	{
		this.savedOrders = new List<Refinery.OrderSaveData>();
	}

	public RefinementRecipe[] GetRecipes()
	{
		Tag tag = base.GetComponent<KPrefabID>().PrefabID();
		List<RefinementRecipe> recipes = RefineryRecipeManager.Get().recipes;
		List<RefinementRecipe> list = new List<RefinementRecipe>();
		foreach (RefinementRecipe refinementRecipe in recipes)
		{
			foreach (Tag tag2 in refinementRecipe.fabricators)
			{
				if (tag2 == tag)
				{
					list.Add(refinementRecipe);
				}
			}
		}
		return list.ToArray();
	}

	private void ReloadSavedQueue()
	{
		this.userOrders.Clear();
		this.buildStorage.Transfer(this.inStorage, true, true);
		if (this.savedOrders == null)
		{
			return;
		}
		bool flag = true;
		foreach (Refinery.OrderSaveData orderSaveData in this.savedOrders)
		{
			RefinementRecipe recipe = RefineryRecipeManager.Get().GetRecipe(TagManager.Create(orderSaveData.material, null));
			Refinery.UserOrder userOrder = new Refinery.UserOrder(recipe, orderSaveData.infinite);
			if (this.OnCreateOrder != null)
			{
				this.OnCreateOrder(userOrder);
			}
			this.userOrders.Add(userOrder);
			if (flag)
			{
				base.SetWorkTime(recipe.time);
				flag = false;
			}
		}
		this.savedOrders = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.choreType = Db.Get().ChoreTypes.Fabricate;
		base.GetComponent<Storage>().choreType = Db.Get().ChoreTypes.FabricateFetch;
		base.Subscribe(-1957399615, new Action<object>(this.OnDroppedAll));
		base.Subscribe(-592767678, new Action<object>(this.OnOperationalChanged));
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Processing;
		this.attributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.Subscribe(-235298596, new Action<object>(this.OnBuildingUpgraded));
		this.ReloadSavedQueue();
		this.buildStorage.Transfer(this.inStorage, true, true);
		this.UpdateOrderQueue(true);
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		if (!this.operational.IsOperational)
		{
			return;
		}
		this.operational.SetActive(true, false);
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
		this.operational.SetActive(false, false);
	}

	private void Cancel(Refinery.UserOrder order)
	{
		for (int i = this.machineOrders.Count - 1; i >= 0; i--)
		{
			Refinery.MachineOrder machineOrder = this.machineOrders[i];
			if (machineOrder.parentOrder == order)
			{
				machineOrder.Cancel();
				this.machineOrders.RemoveAt(i);
				if (machineOrder.chore != null)
				{
					this.buildStorage.Transfer(this.inStorage, true, true);
				}
			}
		}
		if (this.OnOrderCancelledOrComplete != null)
		{
			this.OnOrderCancelledOrComplete(order);
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		foreach (Refinery.UserOrder userOrder in this.userOrders)
		{
			this.Cancel(userOrder);
		}
	}

	protected virtual List<GameObject> CompleteOrder(Refinery.UserOrder completed_order)
	{
		GameObject gameObject = this.buildStorage.FindFirst(completed_order.recipe.material);
		if (gameObject != null)
		{
			Edible component = gameObject.GetComponent<Edible>();
			if (component)
			{
				ReportManager.Instance.ReportValue(ReportManager.ReportType.CaloriesCreated, -component.Calories, string.Format(UI.ENDOFDAYREPORT.NOTES.CRAFTED_USED, component.GetProperName()), UI.ENDOFDAYREPORT.NOTES.CRAFTED_CONTEXT);
			}
		}
		SimUtil.DiseaseInfo diseaseInfo;
		float num;
		this.buildStorage.ConsumeAndGetDisease(completed_order.recipe.material, completed_order.recipe.amount, out diseaseInfo, out num);
		List<GameObject> list = new List<GameObject>();
		foreach (RefinementRecipe.Result result in completed_order.recipe.results)
		{
			GameObject prefab = Assets.GetPrefab(result.tag);
			GameObject gameObject2 = GameUtil.KInstantiate(prefab, Grid.SceneLayer.Ore, Folder.Ore, null, 0);
			gameObject2.transform.localPosition = base.transform.localPosition + this.outputOffset;
			PrimaryElement component2 = gameObject2.GetComponent<PrimaryElement>();
			component2.Units = result.amount;
			component2.Temperature = num;
			gameObject2.SetActive(true);
			float num2 = result.amount / completed_order.recipe.TotalResultMass();
			component2.AddDisease(diseaseInfo.idx, Mathf.RoundToInt((float)diseaseInfo.count * num2), "Refinery.CompleteOrder");
			gameObject2.GetComponent<KMonoBehaviour>().Trigger(748399584, null);
			list.Add(gameObject2);
		}
		KBatchedAnimController component3 = list[0].GetComponent<KBatchedAnimController>();
		HashedString batchTag = component3.AnimFiles[0].batchTag;
		KAnim.Build build = component3.AnimFiles[0].GetData().build;
		KAnim.Build.Symbol symbol = build.GetSymbol(new KAnimHashedString(build.name));
		KBatchedAnimController component4 = base.GetComponent<KBatchedAnimController>();
		component4.AddSymbolOverride(new KAnimHashedString("output_tracker"), batchTag, symbol, false);
		if (!completed_order.infinite && this.OnOrderCancelledOrComplete != null)
		{
			this.OnOrderCancelledOrComplete(completed_order);
		}
		return list;
	}

	public override float GetWorkTime()
	{
		if (this.machineOrders.Count > 0)
		{
			Refinery.MachineOrder machineOrder = this.machineOrders[0];
			this.workTime = machineOrder.parentOrder.recipe.time;
			return this.workTime;
		}
		return -1f;
	}

	public void CreateOrder(RefinementRecipe recipe, bool isInfinite, string soundPath)
	{
		if (DebugHandler.InstantBuildMode)
		{
			Refinery.UserOrder userOrder = new Refinery.UserOrder(recipe, false);
			if (this.OnCreateOrder != null)
			{
				this.OnCreateOrder(userOrder);
			}
			this.CompleteOrder(userOrder);
		}
		else if (this.userOrders.Count < 6)
		{
			KFMOD.PlayOneShot(soundPath);
			Refinery.UserOrder userOrder2 = new Refinery.UserOrder(recipe, isInfinite);
			if (this.OnCreateOrder != null)
			{
				this.OnCreateOrder(userOrder2);
			}
			this.userOrders.Add(userOrder2);
			this.UpdateOrderQueue(false);
		}
		else
		{
			UISounds.PlaySound(UISounds.Sound.Negative);
		}
	}

	private void UpdateOrderQueue(bool force_update = false)
	{
		if (!force_update && !this.operational.IsOperational)
		{
			return;
		}
		int num = 0;
		while (num < this.userOrders.Count && this.machineOrders.Count < 3)
		{
			Refinery.UserOrder userOrder = this.userOrders[num];
			if (!this.AlreadyMachineQueued(userOrder) || userOrder.infinite)
			{
				Refinery.MachineOrder machineOrder = new Refinery.MachineOrder();
				machineOrder.parentOrder = userOrder;
				this.machineOrders.Add(machineOrder);
			}
			if (!userOrder.infinite)
			{
				num++;
			}
		}
		if (this.machineOrders.Count > 0)
		{
			Refinery.MachineOrder machineOrder2 = this.machineOrders[0];
			if (machineOrder2.chore == null)
			{
				RefinementRecipe recipe = machineOrder2.parentOrder.recipe;
				bool flag = true;
				if (this.inStorage.GetMassAvailable(recipe.material) < recipe.amount)
				{
					flag = false;
				}
				if (flag)
				{
					machineOrder2.chore = new WorkChore<Refinery>(this.choreType, this, null, true, null, null, null, true, null, true, default(Tag), null, false, true, true, PriorityScreen.PriorityClass.basic, int.MaxValue);
					if (this.workTimeRemaining <= 0f)
					{
						this.workTimeRemaining = this.GetWorkTime();
					}
					this.inStorage.Transfer(this.buildStorage, recipe.material, recipe.amount, false, true);
					this.OnBuildQueued(machineOrder2);
				}
			}
			Dictionary<Tag, float> dictionary = new Dictionary<Tag, float>();
			for (int i = 0; i < this.machineOrders.Count; i++)
			{
				Refinery.MachineOrder machineOrder3 = this.machineOrders[i];
				if (machineOrder3.chore == null)
				{
					Refinery.UserOrder parentOrder = machineOrder3.parentOrder;
					RefinementRecipe recipe2 = parentOrder.recipe;
					dictionary[recipe2.material] = this.inStorage.GetMassAvailable(recipe2.material);
				}
			}
			for (int j = 0; j < this.machineOrders.Count; j++)
			{
				Refinery.MachineOrder machineOrder4 = this.machineOrders[j];
				if (machineOrder4.chore == null)
				{
					Refinery.UserOrder parentOrder2 = machineOrder4.parentOrder;
					RefinementRecipe recipe3 = parentOrder2.recipe;
					float num2;
					if (dictionary[recipe3.material] < recipe3.amount)
					{
						num2 = recipe3.amount - dictionary[recipe3.material];
						dictionary[recipe3.material] = 0f;
					}
					else
					{
						Dictionary<Tag, float> dictionary2;
						Tag material;
						(dictionary2 = dictionary)[material = recipe3.material] = dictionary2[material] - recipe3.amount;
						num2 = 0f;
					}
					int num3 = -j;
					if (machineOrder4.fetchList == null && num2 > 0f)
					{
						machineOrder4.fetchList = new FetchList2(this.inStorage);
						machineOrder4.fetchList.ShowStatusItem = false;
						machineOrder4.fetchList.SetPriorityMod(num3);
						machineOrder4.fetchList.Add(recipe3.material, null, num2, FetchOrder2.OperationalRequirement.None);
						machineOrder4.fetchList.Submit(new global::System.Action(this.OnFetchComplete), false);
					}
					else if (machineOrder4.fetchList != null)
					{
						machineOrder4.fetchList.SetPriorityMod(num3);
					}
				}
			}
			if (machineOrder2.chore == null)
			{
				machineOrder2.fetchList.ShowStatusItem = true;
			}
		}
		base.Trigger(1721324763, this);
	}

	private void OnFetchComplete()
	{
		this.UpdateOrderQueue(false);
	}

	private bool AlreadyMachineQueued(Refinery.UserOrder user_order)
	{
		bool flag = false;
		foreach (Refinery.MachineOrder machineOrder in this.machineOrders)
		{
			if (machineOrder.parentOrder == user_order)
			{
				flag = true;
				break;
			}
		}
		return flag;
	}

	public virtual void CancelOrder(int idx)
	{
		if (idx == 0)
		{
			this.workTimeRemaining = this.GetWorkTime();
		}
		if (idx < this.userOrders.Count)
		{
			Refinery.UserOrder userOrder = this.userOrders[idx];
			this.Cancel(userOrder);
			this.userOrders.RemoveAt(idx);
			this.operational.SetActive(false, false);
		}
		if (this.userOrders.Count == 0)
		{
			this.CancelAll();
		}
		this.UpdateOrderQueue(false);
	}

	private bool CanFabricate(Refinery.UserOrder order, Storage storage)
	{
		return storage.GetAmountAvailable(order.recipe.material) >= order.recipe.amount;
	}

	private bool CanFabricate(Refinery.UserOrder order)
	{
		return this.CanFabricate(order, this.inStorage);
	}

	public virtual List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		RefinementRecipe[] recipes = this.GetRecipes();
		if (recipes.Length > 0)
		{
			Descriptor descriptor = default(Descriptor);
			descriptor.SetupDescriptor(UI.BUILDINGEFFECTS.PROCESSES, UI.BUILDINGEFFECTS.TOOLTIPS.PROCESSES, Descriptor.DescriptorType.Effect);
			list.Add(descriptor);
		}
		foreach (RefinementRecipe refinementRecipe in this.GetRecipes())
		{
			string keywordStyle = GameUtil.GetKeywordStyle(refinementRecipe.material);
			Descriptor descriptor2 = new Descriptor("• " + string.Format(UI.BUILDINGEFFECTS.PROCESSEDITEM, keywordStyle, refinementRecipe.material.ProperName()), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.PROCESSEDITEM, string.Join(", ", refinementRecipe.results.Select<RefinementRecipe.Result, string>((RefinementRecipe.Result r) => r.tag.ProperName()).ToArray<string>())), Descriptor.DescriptorType.Effect, false);
			descriptor2.IncreaseIndent();
			list.Add(descriptor2);
		}
		return list;
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		if (this.machineOrders.Count <= 0)
		{
			global::Debug.LogWarning("Somehow we tried to complete an order when there was no orders to complete. Need more info on how to reproduce this for a proper fix.", null);
			return;
		}
		Refinery.MachineOrder machineOrder = this.machineOrders[0];
		machineOrder.Complete();
		if (!machineOrder.parentOrder.infinite)
		{
			this.userOrders.RemoveAt(0);
		}
		this.machineOrders.RemoveAt(0);
		this.operational.SetActive(false, false);
		this.CompleteOrder(machineOrder.parentOrder);
		this.buildStorage.Transfer(this.outStorage, true, true);
		this.UpdateOrderQueue(false);
	}

	private void OnDroppedAll(object data)
	{
		this.UpdateOrderQueue(false);
	}

	private void OnOperationalChanged(object data)
	{
		bool flag = (bool)data;
		if (flag)
		{
			this.UpdateOrderQueue(false);
		}
		else if (this.userOrders.Count > 0)
		{
			this.CancelAll();
		}
	}

	private void OnBuildingUpgraded(object data)
	{
		for (int i = this.userOrders.Count - 1; i >= 0; i--)
		{
			this.CancelOrder(i);
		}
		this.CancelAll();
	}

	private void CancelAll()
	{
		this.buildStorage.Transfer(this.inStorage, true, true);
		while (this.machineOrders.Count > 0)
		{
			Refinery.MachineOrder machineOrder = this.machineOrders[0];
			machineOrder.Cancel();
			if (this.machineOrders.Count > 0 && this.machineOrders[0] == machineOrder)
			{
				this.machineOrders.RemoveAt(0);
			}
		}
		base.ShowProgressBar(this.userOrders.Count > 0);
	}

	protected virtual void OnBuildQueued(Refinery.MachineOrder order)
	{
	}

	public virtual List<Descriptor> AdditionalEffectsForRecipe(RefinementRecipe recipe)
	{
		return new List<Descriptor>();
	}

	public Action<Refinery.UserOrder> OnCreateOrder;

	public Action<Refinery.UserOrder> OnOrderCancelledOrComplete;

	[MyCmpAdd]
	private LoopingSounds loopingSounds;

	[MyCmpGet]
	private OutputPoint outputPoint;

	[MyCmpReq]
	protected Operational operational;

	[MyCmpAdd]
	private RefinerySM refinerySM;

	[SerializeField]
	public Storage inStorage;

	[SerializeField]
	public Storage buildStorage;

	[SerializeField]
	public Storage outStorage;

	public Vector3 outputOffset = Vector3.zero;

	protected List<Refinery.UserOrder> userOrders = new List<Refinery.UserOrder>();

	protected List<Refinery.MachineOrder> machineOrders = new List<Refinery.MachineOrder>();

	private const int MaxPrefetchCount = 3;

	private MeterController outputVisualizer;

	[SerializeField]
	public Refinery.ResultState resultState;

	[Serialize]
	private List<Refinery.OrderSaveData> savedOrders;

	protected ChoreType choreType;

	[Serializable]
	public class UserOrder : IBuildQueueOrder
	{
		public UserOrder(RefinementRecipe recipe, bool infinite = false)
		{
			this.recipe = recipe;
			this.infinite = infinite;
		}

		public Tag Result
		{
			get
			{
				return this.recipe.material;
			}
		}

		public Sprite Icon
		{
			get
			{
				return this.recipe.GetUIIcon();
			}
		}

		public Color IconColor
		{
			get
			{
				return this.recipe.GetUIColor();
			}
		}

		public bool Infinite
		{
			get
			{
				return this.infinite;
			}
		}

		public Dictionary<Tag, float> CheckMaterialRequirements()
		{
			Dictionary<Tag, float> dictionary = new Dictionary<Tag, float>();
			float amount = WorldInventory.Instance.GetAmount(this.recipe.material);
			dictionary[this.recipe.material] = this.recipe.amount - amount;
			return dictionary;
		}

		public RefinementRecipe recipe;

		public bool infinite;
	}

	public class MachineOrder
	{
		public void Cancel()
		{
			if (this.chore != null)
			{
				this.chore.Cancel("Fabrication cancelled");
				this.chore = null;
			}
			if (this.fetchList != null)
			{
				this.fetchList.Cancel("Fabrication cancelled");
				this.fetchList = null;
			}
		}

		public void Complete()
		{
		}

		public Refinery.UserOrder parentOrder;

		public FetchList2 fetchList;

		public Chore chore;
	}

	[Serializable]
	public struct OrderSaveData
	{
		public OrderSaveData(string material, bool infinite)
		{
			this.material = material;
			this.infinite = infinite;
		}

		public string material;

		public bool infinite;
	}

	protected enum SubAnim
	{
		Queued,
		Full,
		On,
		Use
	}

	public enum ResultState
	{
		Normal,
		Hot,
		Melted
	}
}
