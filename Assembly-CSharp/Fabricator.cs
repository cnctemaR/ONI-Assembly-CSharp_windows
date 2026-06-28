using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using KSerialization;
using STRINGS;
using UnityEngine;
using UnityEngine.Assertions;

[SerializationConfig(MemberSerialization.OptIn)]
public class Fabricator : BuildingWorkable, IEffectDescriptor
{
	public int DescriptionOrder { get; set; }

	public string FabricationMachine
	{
		get
		{
			return base.GetComponent<KPrefabID>().PrefabTag.Name;
		}
	}

	public ReadOnlyCollection<Fabricator.UserOrder> Orders
	{
		get
		{
			return this.userOrders.AsReadOnly();
		}
	}

	public ReadOnlyCollection<Fabricator.MachineOrder> MachineOrders
	{
		get
		{
			return this.machineOrders.AsReadOnly();
		}
	}

	public bool IsQueueFull
	{
		get
		{
			return this.userOrders.Count == 6;
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
			this.savedOrders.Add(new Fabricator.OrderSaveData(userOrder.recipe.Name, array, infinite));
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
		this.buildStorage.Transfer(this.inStorage, true);
		if (this.savedOrders == null)
		{
			return;
		}
		foreach (Fabricator.OrderSaveData orderSaveData in this.savedOrders)
		{
			string[] tagNames = orderSaveData.tagNames;
			if (tagNames != null)
			{
				string recipeName = orderSaveData.recipeName;
				bool flag = false;
				for (int i = 0; i < recipes.Length; i++)
				{
					if (recipes[i].Name == recipeName)
					{
						List<Tag> list = new List<Tag>();
						for (int j = 0; j < tagNames.Length; j++)
						{
							list.Add(new Tag(tagNames[j]));
						}
						flag = true;
						this.userOrders.Add(new Fabricator.UserOrder(recipes[i], list, orderSaveData.infinite));
						break;
					}
				}
				if (!flag)
				{
					Output.LogWarning(new object[] { "Order failed, missing recipe [", recipeName, "]" });
				}
			}
		}
		this.savedOrders = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.choreType = Db.Get().ChoreTypes.Fabricate;
		base.GetComponent<Storage>().choreType = Db.Get().ChoreTypes.FabricateFetch;
		this.Subscribe(-1957399615, new EventSystem.EventHandler(this.OnDroppedAll));
		this.Subscribe(-592767678, new EventSystem.EventHandler(this.OnOperationalChanged));
		Components.Fabricators.Add(this);
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Fabricating;
		this.attributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.Subscribe(-235298596, new EventSystem.EventHandler(this.OnBuildingUpgraded));
		this.ReloadSavedQueue();
		this.buildStorage.Transfer(this.inStorage, true);
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
		for (int i = this.machineOrders.Count - 1; i >= 0; i--)
		{
			Fabricator.MachineOrder machineOrder = this.machineOrders[i];
			if (machineOrder.parentOrder == order)
			{
				machineOrder.Cancel();
				this.machineOrders.RemoveAt(i);
				if (machineOrder.chore != null)
				{
					this.buildStorage.Transfer(this.inStorage, true);
				}
			}
		}
	}

	protected override void OnCleanUp()
	{
		foreach (Fabricator.UserOrder userOrder in this.userOrders)
		{
			this.Cancel(userOrder);
		}
		Components.Fabricators.Remove(this);
	}

	protected virtual void CompleteOrder(Fabricator.UserOrder completed_order)
	{
		int num = Math.Max(completed_order.recipe.NumProduced, 1);
		for (int i = 0; i < num; i++)
		{
			GameObject gameObject;
			PrimaryElement primaryElement;
			if (i == 0)
			{
				gameObject = completed_order.recipe.Craft(this.buildStorage, completed_order.orderTags);
				primaryElement = gameObject.GetComponent<PrimaryElement>();
			}
			else
			{
				gameObject = Util.KInstantiate(completed_order.recipe.Result, Vector3.zero, Quaternion.identity, SceneOrganizer.Instance.GetFolder(Folder.Entities), null, true, 0);
				primaryElement = gameObject.GetComponent<PrimaryElement>();
				if (primaryElement != null)
				{
					if (completed_order.orderTags != null && completed_order.orderTags.Count > 0)
					{
						Element element = ElementLoader.GetElement(completed_order.orderTags[0]);
						primaryElement.ElementID = element.id;
					}
					Assert.IsTrue(primaryElement.ElementID != (SimHashes)0);
				}
			}
			gameObject.transform.localPosition = this.transform.localPosition;
			switch (this.resultState)
			{
			case Fabricator.ResultState.Normal:
			{
				int num2 = Grid.PosToCell(this);
				gameObject.transform.SetPosition(Grid.CellToPosCCC(num2, Grid.SceneLayer.Use));
				break;
			}
			case Fabricator.ResultState.Hot:
			{
				int num3 = Grid.PosToCell(this);
				primaryElement.gameObject.transform.SetPosition(Grid.CellToPosCCC(num3, Grid.SceneLayer.Use));
				primaryElement.Temperature = primaryElement.Element.highTemp - 10f;
				break;
			}
			case Fabricator.ResultState.Melted:
			{
				int outputCell = this.outputPoint.GetOutputCell();
				PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
				SimMessages.AddRemoveSubstance(outputCell, primaryElement.Element.highTempTransition.id, CellEventLogger.Instance.FabricatorProduceMelted, component.Mass, primaryElement.Element.highTempTransition.defaultValues.temperature + 10f, -1);
				primaryElement.gameObject.DeleteObject();
				break;
			}
			}
		}
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

	public void CreateOrder(Recipe recipe, List<Tag> tags, bool isInfinite)
	{
		if (DebugHandler.InstantBuildMode)
		{
			Fabricator.UserOrder userOrder = new Fabricator.UserOrder(recipe, tags, false);
			this.CompleteOrder(userOrder);
		}
		else if (!this.IsQueueFull)
		{
			Fabricator.UserOrder userOrder2 = new Fabricator.UserOrder(recipe, tags, isInfinite);
			this.userOrders.Add(userOrder2);
			this.UpdateOrderQueue(false);
			this.OnCreateOrder.Signal(userOrder2);
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
					machineOrder2.chore = new WorkChore<Fabricator>(this.choreType, this, null, true, null, null, null, true, null, true, default(Tag), null, false, true);
					if (this.workTimeRemaining <= 0f)
					{
						this.workTimeRemaining = this.GetWorkTime();
					}
					foreach (Recipe.Ingredient ingredient2 in allIngredients)
					{
						this.inStorage.Transfer(this.buildStorage, ingredient2.tag, ingredient2.amount, false, true);
					}
					this.OnBuildQueued();
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
			for (int m = 0; m < this.machineOrders.Count; m++)
			{
				Fabricator.MachineOrder machineOrder4 = this.machineOrders[m];
				if (machineOrder4.chore == null)
				{
					Fabricator.UserOrder parentOrder2 = machineOrder4.parentOrder;
					Recipe.Ingredient[] allIngredients3 = parentOrder2.recipe.GetAllIngredients(parentOrder2.orderTags);
					bool flag2 = false;
					foreach (Recipe.Ingredient ingredient4 in allIngredients3)
					{
						if (dictionary[ingredient4.tag] < ingredient4.amount)
						{
							flag2 = false;
						}
						Dictionary<Tag, float> dictionary3;
						Dictionary<Tag, float> dictionary2 = (dictionary3 = dictionary);
						Tag tag2;
						Tag tag = (tag2 = ingredient4.tag);
						float num2 = dictionary3[tag2];
						dictionary2[tag] = num2 - ingredient4.amount;
					}
					if (machineOrder4.fetchList == null && !flag2)
					{
						machineOrder4.fetchList = new FetchList2(this.inStorage);
						machineOrder4.fetchList.ShowStatusItem = false;
						this.AddIngredientsToFetchList(allIngredients3, machineOrder4.fetchList);
						machineOrder4.fetchList.Submit(new global::System.Action(this.OnFetchComplete), false);
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
			Debug.LogError("Invalid parameters received for the fetch list.");
			return;
		}
		foreach (Recipe.Ingredient ingredient in ingredients)
		{
			fetchList.Add(ingredient.tag, ingredient.amount, false);
		}
	}

	public void CancelOrder(int idx)
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
		if (this.OnOrderCancelled != null)
		{
			this.OnOrderCancelled();
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

	public List<Descriptor> GetRequirementDescriptions(BuildingDef def)
	{
		return null;
	}

	public List<Descriptor> GetEffectDescriptions(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		Recipe[] recipes = this.GetRecipes();
		if (recipes.Length > 0)
		{
			Descriptor descriptor = default(Descriptor);
			descriptor.SetupDescriptor(string.Format(UI.LISTENTRYSTRINGNOLINEBREAK, UI.BUILDINGEFFECTS.FABRICATES), UI.BUILDINGEFFECTS.TOOLTIPS.FABRICATES);
			list.Add(descriptor);
		}
		foreach (Recipe recipe in this.GetRecipes())
		{
			string keywordStyle = GameUtil.GetKeywordStyle(recipe.Result);
			Descriptor descriptor2 = default(Descriptor);
			string text = UI.LISTENTRYTAB + UI.LISTENTRYTAB + "• ";
			descriptor2.SetupDescriptor(text + string.Format(UI.BUILDINGEFFECTS.FABRICATEDITEM, keywordStyle, recipe.Name), GameUtil.GetGameObjectEffectsTooltipString(recipe.Result));
			list.Add(descriptor2);
		}
		return list;
	}

	protected override void OnCompleteWork(Worker worker)
	{
		base.OnCompleteWork(worker);
		Fabricator.MachineOrder machineOrder = this.machineOrders[0];
		machineOrder.Complete();
		if (!machineOrder.parentOrder.infinite)
		{
			this.userOrders.RemoveAt(0);
		}
		this.machineOrders.RemoveAt(0);
		this.operational.SetActive(false, false);
		this.CompleteOrder(machineOrder.parentOrder);
		this.buildStorage.Transfer(this.outStorage, true);
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
		this.buildStorage.Transfer(this.inStorage, true);
		foreach (Fabricator.MachineOrder machineOrder in this.machineOrders)
		{
			machineOrder.Cancel();
		}
		this.machineOrders.Clear();
		base.ShowProgressBar(this.userOrders.Count > 0);
	}

	protected virtual void OnBuildQueued()
	{
	}

	public const int MaxNumOrders = 6;

	private const int MaxPrefetchCount = 3;

	public Action<Fabricator.UserOrder> OnCreateOrder;

	public global::System.Action OnOrderCancelled;

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

	private List<Fabricator.UserOrder> userOrders = new List<Fabricator.UserOrder>();

	private List<Fabricator.MachineOrder> machineOrders = new List<Fabricator.MachineOrder>();

	[SerializeField]
	public Fabricator.ResultState resultState;

	[Serialize]
	private List<Fabricator.OrderSaveData> savedOrders;

	protected ChoreType choreType;

	[Serializable]
	public class UserOrder
	{
		public UserOrder(Recipe recipe, List<Tag> orderTags, bool infinite = false)
		{
			this.recipe = recipe;
			this.orderTags = orderTags;
			this.infinite = infinite;
		}

		public Recipe recipe;

		public List<Tag> orderTags;

		public bool infinite;

		public Action<Fabricator.UserOrder> OnComplete;

		public Action<Fabricator.UserOrder> OnCancel;
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
			this.parentOrder.OnCancel.Signal(this.parentOrder);
		}

		public void Complete()
		{
			this.parentOrder.OnComplete.Signal(this.parentOrder);
		}

		public Fabricator.UserOrder parentOrder;

		public FetchList2 fetchList;

		public Chore chore;
	}

	[Serializable]
	public struct OrderSaveData
	{
		public OrderSaveData(string recipeName, string[] tagNames, bool infinite)
		{
			this.recipeName = recipeName;
			this.tagNames = tagNames;
			this.infinite = infinite;
		}

		public string recipeName;

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
