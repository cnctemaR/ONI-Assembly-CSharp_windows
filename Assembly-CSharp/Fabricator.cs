using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Fabricator : Workable, IEffectDescriptor, IHasBuildQueue
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
			return this.userOrders.ConvertAll<IBuildQueueOrder>((Fabricator.UserOrder o) => o);
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
			return base.worker != null;
		}
	}

	[OnSerializing]
	internal void OnSerializingMethod()
	{
		this.savedOrders = new List<Fabricator.OrderSaveData>();
		for (int i = 0; i < this.userOrders.Count; i++)
		{
			Fabricator.UserOrder userOrder = this.userOrders[i];
			string[] array = new string[userOrder.orderTags.Count];
			bool infinite = this.userOrders[i].infinite;
			for (int j = 0; j < array.Length; j++)
			{
				array[j] = userOrder.orderTags[j].Name;
			}
			this.savedOrders.Add(new Fabricator.OrderSaveData(userOrder.recipe.Result.Name, array, infinite));
		}
	}

	[OnDeserializing]
	internal void OnDeserializingMethod()
	{
		this.savedOrders = new List<Fabricator.OrderSaveData>();
	}

	public Recipe[] GetRecipes()
	{
		string name = base.GetComponent<KPrefabID>().PrefabID().Name;
		List<Recipe> recipes = RecipeManager.Get().recipes;
		List<Recipe> list = new List<Recipe>();
		foreach (Recipe recipe in recipes)
		{
			foreach (string text in recipe.fabricators)
			{
				if (text != null && text == name)
				{
					list.Add(recipe);
				}
			}
		}
		return list.ToArray();
	}

	private void ReloadSavedQueue()
	{
		this.userOrders.Clear();
		Recipe[] recipes = this.GetRecipes();
		this.buildStorage.Transfer(this.inStorage, true, true);
		if (this.savedOrders == null)
		{
			return;
		}
		bool flag = true;
		foreach (Fabricator.OrderSaveData orderSaveData in this.savedOrders)
		{
			string[] tagNames = orderSaveData.tagNames;
			if (tagNames != null)
			{
				string recipePrefab = orderSaveData.recipePrefab;
				bool flag2 = false;
				for (int i = 0; i < recipes.Length; i++)
				{
					if (recipes[i].Result.Name == recipePrefab)
					{
						List<Tag> list = new List<Tag>();
						for (int j = 0; j < tagNames.Length; j++)
						{
							list.Add(new Tag(tagNames[j]));
						}
						flag2 = true;
						this.userOrders.Add(new Fabricator.UserOrder(recipes[i], list, this.OnCreateOrder, orderSaveData.infinite));
						if (flag)
						{
							base.SetWorkTime(recipes[i].FabricationTime);
							flag = false;
						}
						break;
					}
				}
				if (!flag2)
				{
					Output.LogWarning(new object[] { "Order failed, missing recipe [", recipePrefab, "]" });
				}
			}
		}
		this.savedOrders = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.choreType = Db.Get().ChoreTypes.Fabricate;
		if (!this.fetchChoreTypeIdHash.IsValid)
		{
			this.fetchChoreTypeIdHash = Db.Get().ChoreTypes.Fetch.IdHash;
		}
		base.Subscribe(-1957399615, new Action<object>(this.OnDroppedAll));
		base.Subscribe(-592767678, new Action<object>(this.OnOperationalChanged));
		Components.Fabricators.Add(this);
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Fabricating;
		this.attributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
		this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
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

	private void Cancel(Fabricator.UserOrder order)
	{
		this.isCancellingOrder = true;
		for (int i = this.machineOrders.Count - 1; i >= 0; i--)
		{
			Fabricator.MachineOrder machineOrder = this.machineOrders[i];
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
		order.Cleanup(this.OnOrderCancelledOrComplete);
		this.isCancellingOrder = false;
	}

	public override void AwardExperience(float work_dt, MinionResume resume)
	{
		resume.AddExperienceIfRole(MachineTechnician.ID, work_dt * ROLES.ACTIVE_EXPERIENCE_QUICK);
		resume.AddExperienceIfRole("PowerTechnician", work_dt * ROLES.ACTIVE_EXPERIENCE_QUICK);
		resume.AddExperienceIfRole("MechatronicEngineer", work_dt * ROLES.ACTIVE_EXPERIENCE_QUICK);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		foreach (Fabricator.UserOrder userOrder in this.userOrders)
		{
			this.Cancel(userOrder);
		}
		Components.Fabricators.Remove(this);
	}

	protected virtual GameObject CompleteOrder(Fabricator.UserOrder completed_order)
	{
		GameObject gameObject = completed_order.recipe.Craft(this.buildStorage, completed_order.orderTags);
		gameObject.transform.SetLocalPosition(base.transform.GetLocalPosition());
		PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
		Fabricator.ResultState resultState = this.resultState;
		if (resultState != Fabricator.ResultState.Normal)
		{
			if (resultState != Fabricator.ResultState.Hot)
			{
				if (resultState == Fabricator.ResultState.Melted)
				{
					int outputCell = this.outputPoint.GetOutputCell();
					PrimaryElement component2 = gameObject.GetComponent<PrimaryElement>();
					SimMessages.AddRemoveSubstance(outputCell, component.Element.highTempTransition.id, CellEventLogger.Instance.FabricatorProduceMelted, component2.Mass, component.Element.highTempTransition.defaultValues.temperature + 10f, byte.MaxValue, 0, true, -1);
					component.gameObject.DeleteObject();
				}
			}
			else
			{
				int num = Grid.PosToCell(this);
				component.gameObject.transform.SetPosition(Grid.CellToPosCCC(num, Grid.SceneLayer.Ore));
				component.Temperature = component.Element.highTemp - 10f;
			}
		}
		else
		{
			int num2 = Grid.PosToCell(this);
			gameObject.transform.SetPosition(Grid.CellToPosCCC(num2, Grid.SceneLayer.Ore));
		}
		if (!completed_order.infinite)
		{
			completed_order.Cleanup(this.OnOrderCancelledOrComplete);
		}
		return gameObject;
	}

	public override float GetWorkTime()
	{
		if (this.machineOrders.Count > 0)
		{
			Fabricator.MachineOrder machineOrder = this.machineOrders[0];
			this.workTime = machineOrder.parentOrder.recipe.FabricationTime;
			return this.workTime;
		}
		return -1f;
	}

	public void CreateOrder(Recipe recipe, List<Tag> tags, bool isInfinite, string soundPath)
	{
		if (DebugHandler.InstantBuildMode)
		{
			Fabricator.UserOrder userOrder = new Fabricator.UserOrder(recipe, tags, this.OnCreateOrder, false);
			this.CompleteOrder(userOrder);
		}
		else if (this.userOrders.Count < 6)
		{
			KFMOD.PlayOneShot(soundPath);
			Fabricator.UserOrder userOrder2 = new Fabricator.UserOrder(recipe, tags, this.OnCreateOrder, isInfinite);
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
			Fabricator.UserOrder userOrder = this.userOrders[num];
			if (!this.AlreadyMachineQueued(userOrder) || userOrder.infinite)
			{
				Fabricator.MachineOrder machineOrder = new Fabricator.MachineOrder();
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
			Fabricator.MachineOrder machineOrder2 = this.machineOrders[0];
			if (machineOrder2.chore == null)
			{
				Recipe.Ingredient[] allIngredients = machineOrder2.parentOrder.recipe.GetAllIngredients(machineOrder2.parentOrder.orderTags);
				bool flag = true;
				foreach (Recipe.Ingredient ingredient in allIngredients)
				{
					if (this.inStorage.GetMassAvailable(ingredient.tag) < ingredient.amount)
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					machineOrder2.chore = new WorkChore<Fabricator>(this.choreType, this, null, null, true, null, null, null, true, null, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 0, false);
					if (this.workTimeRemaining <= 0f)
					{
						this.workTimeRemaining = this.GetWorkTime();
					}
					foreach (Recipe.Ingredient ingredient2 in allIngredients)
					{
						this.inStorage.Transfer(this.buildStorage, ingredient2.tag, ingredient2.amount, false, true);
					}
					this.OnBuildQueued(machineOrder2);
				}
			}
			Dictionary<Tag, float> dictionary = new Dictionary<Tag, float>();
			for (int k = 0; k < this.machineOrders.Count; k++)
			{
				Fabricator.MachineOrder machineOrder3 = this.machineOrders[k];
				if (machineOrder3.chore == null)
				{
					Fabricator.UserOrder parentOrder = machineOrder3.parentOrder;
					Recipe.Ingredient[] allIngredients2 = parentOrder.recipe.GetAllIngredients(parentOrder.orderTags);
					foreach (Recipe.Ingredient ingredient3 in allIngredients2)
					{
						dictionary[ingredient3.tag] = this.inStorage.GetMassAvailable(ingredient3.tag);
					}
				}
			}
			ChoreType byHash = Db.Get().ChoreTypes.GetByHash(this.fetchChoreTypeIdHash);
			for (int m = 0; m < this.machineOrders.Count; m++)
			{
				Fabricator.MachineOrder machineOrder4 = this.machineOrders[m];
				if (machineOrder4.chore == null)
				{
					Fabricator.UserOrder parentOrder2 = machineOrder4.parentOrder;
					Recipe.Ingredient[] allIngredients3 = parentOrder2.recipe.GetAllIngredients(parentOrder2.orderTags);
					bool flag2 = true;
					foreach (Recipe.Ingredient ingredient4 in allIngredients3)
					{
						if (dictionary[ingredient4.tag] < ingredient4.amount)
						{
							ingredient4.amount -= dictionary[ingredient4.tag];
							dictionary[ingredient4.tag] = 0f;
							flag2 = false;
						}
						else
						{
							Dictionary<Tag, float> dictionary2;
							Tag tag;
							(dictionary2 = dictionary)[tag = ingredient4.tag] = dictionary2[tag] - ingredient4.amount;
							ingredient4.amount = 0f;
						}
					}
					int num2 = -m;
					if (machineOrder4.fetchList == null && !flag2)
					{
						machineOrder4.fetchList = new FetchList2(this.inStorage, byHash, this.choreTags);
						machineOrder4.fetchList.ShowStatusItem = false;
						machineOrder4.fetchList.SetPriorityMod(num2);
						this.AddIngredientsToFetchList(allIngredients3, machineOrder4.fetchList);
						machineOrder4.fetchList.Submit(new global::System.Action(this.OnFetchComplete), false);
					}
					else if (machineOrder4.fetchList != null)
					{
						machineOrder4.fetchList.SetPriorityMod(num2);
					}
				}
			}
			if (machineOrder2.chore == null)
			{
				machineOrder2.fetchList.ShowStatusItem = true;
			}
		}
	}

	private void OnFetchComplete()
	{
		this.UpdateOrderQueue(false);
	}

	private bool AlreadyMachineQueued(Fabricator.UserOrder user_order)
	{
		bool flag = false;
		foreach (Fabricator.MachineOrder machineOrder in this.machineOrders)
		{
			if (machineOrder.parentOrder == user_order)
			{
				flag = true;
				break;
			}
		}
		return flag;
	}

	private void AddIngredientsToFetchList(Recipe.Ingredient[] ingredients, FetchList2 fetchList)
	{
		if (fetchList == null || ingredients == null || ingredients.Length == 0)
		{
			global::Debug.LogError("Invalid parameters received for the fetch list.", null);
			return;
		}
		foreach (Recipe.Ingredient ingredient in ingredients)
		{
			if (ingredient.amount > 0f)
			{
				Tag tag = ingredient.tag;
				float amount = ingredient.amount;
				fetchList.Add(tag, null, null, amount, FetchOrder2.OperationalRequirement.None);
			}
		}
	}

	public virtual void CancelOrder(int idx)
	{
		if (idx == 0)
		{
			this.workTimeRemaining = this.GetWorkTime();
		}
		if (idx < this.userOrders.Count)
		{
			Fabricator.UserOrder userOrder = this.userOrders[idx];
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

	private bool CanFabricate(Fabricator.UserOrder order, Storage storage)
	{
		Recipe.Ingredient[] allIngredients = order.recipe.GetAllIngredients(order.orderTags);
		foreach (Recipe.Ingredient ingredient in allIngredients)
		{
			float num = 0f;
			if (!storage.IsMaterialOnStorage(ingredient.tag, ref num))
			{
				return false;
			}
			if (num < ingredient.amount)
			{
				return false;
			}
		}
		return true;
	}

	private bool CanFabricate(Fabricator.UserOrder order)
	{
		return this.CanFabricate(order, this.inStorage);
	}

	public virtual List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		Recipe[] recipes = this.GetRecipes();
		if (recipes.Length > 0)
		{
			Descriptor descriptor = default(Descriptor);
			descriptor.SetupDescriptor(UI.BUILDINGEFFECTS.FABRICATES, UI.BUILDINGEFFECTS.TOOLTIPS.FABRICATES, Descriptor.DescriptorType.Effect);
			list.Add(descriptor);
		}
		foreach (Recipe recipe in this.GetRecipes())
		{
			GameObject prefab = Assets.GetPrefab(recipe.Result);
			string keywordStyle = GameUtil.GetKeywordStyle(prefab);
			Descriptor descriptor2 = default(Descriptor);
			descriptor2.IncreaseIndent();
			descriptor2.SetupDescriptor("• " + string.Format(UI.BUILDINGEFFECTS.FABRICATEDITEM, keywordStyle, recipe.Name), GameUtil.GetGameObjectEffectsTooltipString(prefab), Descriptor.DescriptorType.Effect);
			list.Add(descriptor2);
		}
		return list;
	}

	protected override void OnCompleteWork(Worker worker)
	{
		if (this.isCancellingOrder)
		{
			return;
		}
		base.OnCompleteWork(worker);
		if (this.machineOrders.Count <= 0)
		{
			global::Debug.LogWarning("Somehow we tried to complete an order when there was no orders to complete. Need more info on how to reproduce this for a proper fix.", null);
			return;
		}
		Fabricator.MachineOrder machineOrder = this.machineOrders[0];
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
			Fabricator.MachineOrder machineOrder = this.machineOrders[0];
			machineOrder.Cancel();
			if (this.machineOrders.Count > 0 && this.machineOrders[0] == machineOrder)
			{
				this.machineOrders.RemoveAt(0);
			}
		}
		base.ShowProgressBar(this.userOrders.Count > 0);
	}

	protected virtual void OnBuildQueued(Fabricator.MachineOrder order)
	{
	}

	private bool isCancellingOrder;

	public const int MAX_NUM_ORDERS = 6;

	public Action<Fabricator.UserOrder> OnCreateOrder;

	public Action<Fabricator.UserOrder> OnOrderCancelledOrComplete;

	[MyCmpAdd]
	private LoopingSounds loopingSounds;

	[MyCmpGet]
	private OutputPoint outputPoint;

	[MyCmpReq]
	protected Operational operational;

	[MyCmpAdd]
	private FabricatorSM fabricatorSM;

	[SerializeField]
	public Storage inStorage;

	[SerializeField]
	public Storage buildStorage;

	[SerializeField]
	public Storage outStorage;

	protected List<Fabricator.UserOrder> userOrders = new List<Fabricator.UserOrder>();

	protected List<Fabricator.MachineOrder> machineOrders = new List<Fabricator.MachineOrder>();

	private const int MaxPrefetchCount = 3;

	[SerializeField]
	public Fabricator.ResultState resultState;

	[Serialize]
	private List<Fabricator.OrderSaveData> savedOrders;

	public bool hideRecipesUndiscoveredIngredients;

	protected ChoreType choreType;

	[SerializeField]
	public HashedString fetchChoreTypeIdHash;

	[SerializeField]
	public Tag[] choreTags;

	[Serializable]
	public class UserOrder : IBuildQueueOrder
	{
		public UserOrder(Recipe recipe, List<Tag> orderTags, Action<Fabricator.UserOrder> on_create_order, bool infinite = false)
		{
			this.recipe = recipe;
			this.orderTags = orderTags;
			this.infinite = infinite;
			on_create_order(this);
		}

		public void Cleanup(Action<Fabricator.UserOrder> on_order_cancelled_or_complete)
		{
			on_order_cancelled_or_complete(this);
		}

		public Tag Result
		{
			get
			{
				return this.recipe.Result;
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
			for (int i = 0; i < this.recipe.Ingredients.Count; i++)
			{
				Recipe.Ingredient ingredient = this.recipe.Ingredients[i];
				float amount = ingredient.amount;
				float amount2 = WorldInventory.Instance.GetAmount(ingredient.tag);
				dictionary[ingredient.tag] = amount - amount2;
			}
			return dictionary;
		}

		public Recipe recipe;

		public List<Tag> orderTags;

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

		public Fabricator.UserOrder parentOrder;

		public FetchList2 fetchList;

		public Chore chore;
	}

	[Serializable]
	public struct OrderSaveData
	{
		public OrderSaveData(string recipePrefab, string[] tagNames, bool infinite)
		{
			this.recipePrefab = recipePrefab;
			this.tagNames = tagNames;
			this.infinite = infinite;
		}

		public string recipePrefab;

		public string[] tagNames;

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
