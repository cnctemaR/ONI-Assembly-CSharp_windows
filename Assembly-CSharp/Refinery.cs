using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Klei;
using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Refinery : KMonoBehaviour, IEffectDescriptor, IHasBuildQueue, ISim200ms
{
	public RefineryWorkable GetWorkable
	{
		get
		{
			if (this.workable != null)
			{
				return this.workable;
			}
			this.workable = base.GetComponent<RefineryWorkable>();
			return this.workable;
		}
	}

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

	public bool WaitingForWorker
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
			return !this.duplicantOperated || this.workable.worker != null;
		}
	}

	public Refinery.MachineOrder CurrentMachineOrder
	{
		get
		{
			return (this.machineOrders.Count <= 0) ? null : this.machineOrders[0];
		}
	}

	public List<Refinery.MachineOrder> GetMachineOrders
	{
		get
		{
			return this.machineOrders;
		}
	}

	[OnSerializing]
	internal void OnSerializingMethod()
	{
		this.savedOrders = new List<Refinery.OrderSaveData>();
		for (int i = 0; i < this.userOrders.Count; i++)
		{
			Refinery.UserOrder userOrder = this.userOrders[i];
			this.savedOrders.Add(new Refinery.OrderSaveData(userOrder.recipe.id, userOrder.infinite));
		}
	}

	[OnDeserializing]
	internal void OnDeserializingMethod()
	{
		this.savedOrders = new List<Refinery.OrderSaveData>();
	}

	public string GetConversationTopic()
	{
		if (this.machineOrders.Count > 0)
		{
			Refinery.UserOrder parentOrder = this.machineOrders[0].parentOrder;
			ComplexRecipe recipe = parentOrder.recipe;
			return recipe.results[0].material.Name;
		}
		return null;
	}

	public ComplexRecipe[] GetRecipes()
	{
		Tag tag = base.GetComponent<KPrefabID>().PrefabID();
		List<ComplexRecipe> recipes = ComplexRecipeManager.Get().recipes;
		List<ComplexRecipe> list = new List<ComplexRecipe>();
		foreach (ComplexRecipe complexRecipe in recipes)
		{
			foreach (Tag tag2 in complexRecipe.fabricators)
			{
				if (tag2 == tag)
				{
					list.Add(complexRecipe);
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
			ComplexRecipeManager complexRecipeManager = ComplexRecipeManager.Get();
			ComplexRecipe complexRecipe = complexRecipeManager.GetRecipe(orderSaveData.id);
			if (complexRecipe == null)
			{
				complexRecipe = complexRecipeManager.GetObsoleteRecipe(orderSaveData.id);
			}
			if (complexRecipe != null)
			{
				Refinery.UserOrder userOrder = new Refinery.UserOrder(complexRecipe, orderSaveData.infinite, this);
				if (this.OnCreateOrder != null)
				{
					this.OnCreateOrder(userOrder);
				}
				this.userOrders.Add(userOrder);
				if (flag && this.duplicantOperated)
				{
					this.workable.SetWorkTime(complexRecipe.time);
					flag = false;
				}
			}
		}
		this.savedOrders = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.choreType = Db.Get().ChoreTypes.Fabricate;
		this.choreTags = new Tag[] { GameTags.ChoreTypes.Fabricating };
		base.Subscribe(-1957399615, new Action<object>(this.OnDroppedAll));
		base.Subscribe(-592767678, new Action<object>(this.OnOperationalChanged));
		if (this.duplicantOperated)
		{
			this.GetWorkable.WorkerStatusItem = Db.Get().DuplicantStatusItems.Processing;
			this.GetWorkable.AttributeConvertor = Db.Get().AttributeConverters.MachinerySpeed;
			this.GetWorkable.AttributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		}
		Components.Refineries.Add(this);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.duplicantOperated)
		{
			this.workable = base.GetComponent<RefineryWorkable>();
		}
		base.Subscribe(-235298596, new Action<object>(this.OnBuildingUpgraded));
		this.ReloadSavedQueue();
		this.buildStorage.Transfer(this.inStorage, true, true);
		this.UpdateOrderQueue(true);
	}

	private void Cancel(Refinery.UserOrder order)
	{
		this.isCancellingOrder = true;
		for (int i = this.machineOrders.Count - 1; i >= 0; i--)
		{
			Refinery.MachineOrder machineOrder = this.machineOrders[i];
			if (machineOrder.parentOrder == order)
			{
				machineOrder.Cancel();
				this.machineOrders.RemoveAt(i);
				if (machineOrder.chore != null || machineOrder.underway)
				{
					this.buildStorage.Transfer(this.inStorage, true, true);
				}
			}
		}
		if (this.OnOrderCancelledOrComplete != null)
		{
			this.OnOrderCancelledOrComplete(order);
		}
		this.isCancellingOrder = false;
	}

	protected override void OnCleanUp()
	{
		foreach (Refinery.UserOrder userOrder in this.userOrders)
		{
			this.Cancel(userOrder);
		}
		Components.Refineries.Remove(this);
		base.OnCleanUp();
	}

	protected virtual void ProduceSolidProducts()
	{
	}

	protected virtual List<GameObject> CompleteOrder(Refinery.UserOrder completed_order)
	{
		List<GameObject> list = new List<GameObject>();
		foreach (ComplexRecipe.RecipeElement recipeElement in completed_order.recipe.results)
		{
			GameObject gameObject = this.buildStorage.FindFirst(recipeElement.material);
			if (gameObject != null)
			{
				Edible component = gameObject.GetComponent<Edible>();
				if (component)
				{
					ReportManager.Instance.ReportValue(ReportManager.ReportType.CaloriesCreated, -component.Calories, StringFormatter.Replace(UI.ENDOFDAYREPORT.NOTES.CRAFTED_USED, "{0}", component.GetProperName()), UI.ENDOFDAYREPORT.NOTES.CRAFTED_CONTEXT);
				}
			}
			SimUtil.DiseaseInfo diseaseInfo;
			diseaseInfo.count = 0;
			diseaseInfo.idx = 0;
			float num = 0f;
			float num2 = 0f;
			foreach (ComplexRecipe.RecipeElement recipeElement2 in completed_order.recipe.ingredients)
			{
				num2 += recipeElement2.amount;
			}
			foreach (ComplexRecipe.RecipeElement recipeElement3 in completed_order.recipe.ingredients)
			{
				float num3 = recipeElement3.amount / num2;
				SimUtil.DiseaseInfo diseaseInfo2;
				float num4;
				this.buildStorage.ConsumeAndGetDisease(recipeElement3.material, recipeElement3.amount, out diseaseInfo2, out num4);
				if (diseaseInfo2.count > diseaseInfo.count)
				{
					diseaseInfo = diseaseInfo2;
				}
				num += num4 * num3;
			}
			Refinery.ResultState resultState = this.resultState;
			if (resultState != Refinery.ResultState.Normal && resultState != Refinery.ResultState.Hot)
			{
				if (resultState == Refinery.ResultState.Melted)
				{
					if (this.storeProduced)
					{
						float num5 = ElementLoader.GetElement(recipeElement.material).lowTemp + (ElementLoader.GetElement(recipeElement.material).highTemp - ElementLoader.GetElement(recipeElement.material).lowTemp) / 2f;
						this.outStorage.AddLiquid(ElementLoader.GetElementID(recipeElement.material), recipeElement.amount, num5, 0, 0, false, true);
					}
				}
			}
			else
			{
				GameObject prefab = Assets.GetPrefab(recipeElement.material);
				GameObject gameObject2 = GameUtil.KInstantiate(prefab, Grid.SceneLayer.Ore, null, 0);
				int num6 = Grid.PosToCell(this);
				gameObject2.transform.SetPosition(Grid.CellToPosCCC(num6, Grid.SceneLayer.Ore) + this.outputOffset);
				PrimaryElement component2 = gameObject2.GetComponent<PrimaryElement>();
				component2.Units = recipeElement.amount;
				component2.Temperature = num;
				gameObject2.SetActive(true);
				float num7 = recipeElement.amount / completed_order.recipe.TotalResultUnits();
				component2.AddDisease(diseaseInfo.idx, Mathf.RoundToInt((float)diseaseInfo.count * num7), "Refinery.CompleteOrder");
				gameObject2.GetComponent<KMonoBehaviour>().Trigger(748399584, null);
				list.Add(gameObject2);
				if (this.storeProduced)
				{
					this.outStorage.Store(gameObject2, false, false, true, false);
				}
			}
			if (list.Count > 0)
			{
				SymbolOverrideController component3 = base.GetComponent<SymbolOverrideController>();
				if (component3 != null)
				{
					KBatchedAnimController component4 = list[0].GetComponent<KBatchedAnimController>();
					KAnim.Build build = component4.AnimFiles[0].GetData().build;
					KAnim.Build.Symbol symbol = build.GetSymbol(build.name);
					component3.TryRemoveSymbolOverride("output_tracker", 0);
					component3.AddSymbolOverride("output_tracker", symbol, 0);
				}
			}
			if (!completed_order.infinite && this.OnOrderCancelledOrComplete != null)
			{
				this.OnOrderCancelledOrComplete(completed_order);
			}
		}
		return list;
	}

	public void CreateOrder(ComplexRecipe recipe, bool isInfinite, string soundPath)
	{
		if (DebugHandler.InstantBuildMode)
		{
			Refinery.UserOrder userOrder = new Refinery.UserOrder(recipe, false, this);
			if (this.OnCreateOrder != null)
			{
				this.OnCreateOrder(userOrder);
			}
			this.CompleteOrder(userOrder);
		}
		else if (this.userOrders.Count < 6)
		{
			KFMOD.PlayOneShot(soundPath);
			Refinery.UserOrder userOrder2 = new Refinery.UserOrder(recipe, isInfinite, this);
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
				ComplexRecipe recipe = machineOrder2.parentOrder.recipe;
				bool flag = true;
				foreach (ComplexRecipe.RecipeElement recipeElement in recipe.ingredients)
				{
					if (this.inStorage.GetUnitsAvailable(recipeElement.material) < recipeElement.amount)
					{
						flag = false;
					}
				}
				if (flag)
				{
					machineOrder2.underway = true;
					foreach (ComplexRecipe.RecipeElement recipeElement2 in recipe.ingredients)
					{
						this.inStorage.Transfer(this.buildStorage, recipeElement2.material, recipeElement2.amount, false, true);
					}
					if (this.duplicantOperated)
					{
						this.workable.CreateOrder(machineOrder2, this.choreType, this.choreTags);
					}
					this.OnBuildQueued(machineOrder2);
				}
			}
			Dictionary<Tag, float> dictionary = new Dictionary<Tag, float>();
			for (int k = 0; k < this.machineOrders.Count; k++)
			{
				Refinery.MachineOrder machineOrder3 = this.machineOrders[k];
				if (machineOrder3.chore == null)
				{
					Refinery.UserOrder parentOrder = machineOrder3.parentOrder;
					ComplexRecipe recipe2 = parentOrder.recipe;
					foreach (ComplexRecipe.RecipeElement recipeElement3 in recipe2.ingredients)
					{
						dictionary[recipeElement3.material] = this.inStorage.GetUnitsAvailable(recipeElement3.material);
					}
				}
			}
			for (int m = 0; m < this.machineOrders.Count; m++)
			{
				Refinery.MachineOrder machineOrder4 = this.machineOrders[m];
				if (machineOrder4.chore == null)
				{
					Refinery.UserOrder parentOrder2 = machineOrder4.parentOrder;
					ComplexRecipe recipe3 = parentOrder2.recipe;
					List<KeyValuePair<Tag, float>> list = new List<KeyValuePair<Tag, float>>();
					foreach (ComplexRecipe.RecipeElement recipeElement4 in recipe3.ingredients)
					{
						float num2;
						if (dictionary[recipeElement4.material] < recipeElement4.amount)
						{
							num2 = recipeElement4.amount - dictionary[recipeElement4.material];
							dictionary[recipeElement4.material] = 0f;
						}
						else
						{
							Dictionary<Tag, float> dictionary2;
							Tag material;
							(dictionary2 = dictionary)[material = recipeElement4.material] = dictionary2[material] - recipeElement4.amount;
							num2 = 0f;
						}
						if (num2 > 0f)
						{
							list.Add(new KeyValuePair<Tag, float>(recipeElement4.material, num2));
						}
					}
					int num3 = -m;
					if (machineOrder4.fetchList == null && list.Count > 0)
					{
						machineOrder4.fetchList = new FetchList2(this.inStorage, Db.Get().ChoreTypes.MachineFetch, this.choreTags);
						machineOrder4.fetchList.ShowStatusItem = false;
						machineOrder4.fetchList.SetPriorityMod(num3);
						foreach (KeyValuePair<Tag, float> keyValuePair in list)
						{
							FetchList2 fetchList = machineOrder4.fetchList;
							Tag key = keyValuePair.Key;
							float value = keyValuePair.Value;
							fetchList.Add(key, null, null, value, FetchOrder2.OperationalRequirement.None);
						}
						machineOrder4.fetchList.Submit(new global::System.Action(this.OnFetchComplete), false);
					}
					else if (machineOrder4.fetchList != null)
					{
						machineOrder4.fetchList.SetPriorityMod(num3);
					}
				}
			}
			try
			{
				if (machineOrder2.chore == null && machineOrder2.fetchList != null)
				{
					machineOrder2.fetchList.ShowStatusItem = true;
				}
			}
			catch
			{
				global::Debug.Log("!", null);
			}
		}
		base.Trigger(1721324763, this);
	}

	private void StartWork()
	{
		base.GetComponent<Operational>().SetActive(true, false);
		this.ShowProgressBar(true);
	}

	public void ShowProgressBar(bool show)
	{
		if (show)
		{
			this.progressBar = ProgressBar.CreateProgressBar(base.GetComponent<Building>(), () => this.orderProgress);
		}
		else if (this.progressBar != null)
		{
			this.progressBar.gameObject.DeleteObject();
			this.progressBar = null;
		}
	}

	private void StopWork()
	{
		base.GetComponent<Operational>().SetActive(false, false);
		this.ShowProgressBar(false);
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
		if (idx == 0 && this.duplicantOperated)
		{
			this.workable.OnCancelOrder();
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
		foreach (ComplexRecipe.RecipeElement recipeElement in order.recipe.ingredients)
		{
			if (storage.GetAmountAvailable(recipeElement.material) < recipeElement.amount)
			{
				return false;
			}
		}
		return true;
	}

	private bool CanFabricate(Refinery.UserOrder order)
	{
		return this.CanFabricate(order, this.inStorage);
	}

	public virtual List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		ComplexRecipe[] recipes = this.GetRecipes();
		if (recipes.Length > 0)
		{
			Descriptor descriptor = default(Descriptor);
			descriptor.SetupDescriptor(UI.BUILDINGEFFECTS.PROCESSES, UI.BUILDINGEFFECTS.TOOLTIPS.PROCESSES, Descriptor.DescriptorType.Effect);
			list.Add(descriptor);
		}
		foreach (ComplexRecipe complexRecipe in this.GetRecipes())
		{
			string text = string.Empty;
			string text2 = string.Empty;
			foreach (ComplexRecipe.RecipeElement recipeElement in complexRecipe.ingredients)
			{
				text = text + "• " + string.Format(UI.BUILDINGEFFECTS.PROCESSEDITEM, string.Empty, recipeElement.material.ProperName());
				text2 += string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.PROCESSEDITEM, string.Join(", ", complexRecipe.results.Select<ComplexRecipe.RecipeElement, string>((ComplexRecipe.RecipeElement r) => r.material.ProperName()).ToArray<string>()));
			}
			Descriptor descriptor2 = new Descriptor(text, text2, Descriptor.DescriptorType.Effect, false);
			descriptor2.IncreaseIndent();
			list.Add(descriptor2);
		}
		return list;
	}

	public void OnCompleteWork()
	{
		if (this.isCancellingOrder)
		{
			return;
		}
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
		this.buildStorage.Transfer(this.inStorage, false, false);
		this.UpdateOrderQueue(false);
		this.ShowProgressBar(false);
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
	}

	protected virtual void OnBuildQueued(Refinery.MachineOrder order)
	{
	}

	public virtual List<Descriptor> AdditionalEffectsForRecipe(ComplexRecipe recipe)
	{
		return new List<Descriptor>();
	}

	public void Sim200ms(float dt)
	{
		if (this.duplicantOperated)
		{
			return;
		}
		if (this.machineOrders.Count > 0 && (this.machineOrders[0].fetchList == null || this.machineOrders[0].fetchList.IsComplete || this.machineOrders[0].underway))
		{
			if (!this.operational.IsActive)
			{
				this.machineOrders[0].underway = true;
				this.StartWork();
			}
			this.orderProgress += dt / this.machineOrders[0].parentOrder.recipe.time;
			if (this.orderProgress >= 1f)
			{
				this.machineOrders[0].underway = false;
				if (this.machineOrders.Count == 1)
				{
					this.StopWork();
				}
				this.OnCompleteWork();
				this.orderProgress = 0f;
			}
		}
		else if (!this.operational.IsActive && this.machineOrders.Count > 0)
		{
			if (this.buildStorage.MassStored() > 0f)
			{
				this.buildStorage.Transfer(this.inStorage, false, false);
			}
			this.UpdateOrderQueue(true);
		}
	}

	private ProgressBar progressBar;

	protected RefineryWorkable workable;

	public bool duplicantOperated = true;

	private bool isCancellingOrder;

	public bool labelByResult = true;

	public RefinerySideScreen.StyleSetting sideScreenStyle;

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

	[SerializeField]
	public bool storeProduced;

	[Serialize]
	private List<Refinery.OrderSaveData> savedOrders;

	protected ChoreType choreType;

	protected Tag[] choreTags;

	private float orderProgress;

	[Serializable]
	public class UserOrder : IBuildQueueOrder
	{
		public UserOrder(ComplexRecipe recipe, bool infinite = false, Refinery refinery = null)
		{
			this.recipe = recipe;
			this.infinite = infinite;
			this.refinery = refinery;
		}

		public Tag Result
		{
			get
			{
				return this.recipe.results[0].material;
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
			foreach (ComplexRecipe.RecipeElement recipeElement in this.recipe.ingredients)
			{
				float amount = WorldInventory.Instance.GetAmount(recipeElement.material);
				dictionary[recipeElement.material] = recipeElement.amount - amount;
			}
			return dictionary;
		}

		public Dictionary<Tag, float> GetMaterialRequirements()
		{
			this.materialRequirements.Clear();
			foreach (ComplexRecipe.RecipeElement recipeElement in this.recipe.ingredients)
			{
				this.materialRequirements.Add(recipeElement.material, recipeElement.amount);
			}
			return this.materialRequirements;
		}

		public ComplexRecipe recipe;

		public bool infinite;

		private Refinery refinery;

		private Dictionary<Tag, float> materialRequirements = new Dictionary<Tag, float>();
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

		public bool underway;
	}

	[Serializable]
	public struct OrderSaveData
	{
		public OrderSaveData(string id, bool infinite)
		{
			this.id = id;
			this.infinite = infinite;
		}

		public string id;

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
