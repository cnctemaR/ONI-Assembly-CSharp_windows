using System;
using System.Collections.Generic;
using System.Linq;
using Klei;
using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class ComplexFabricator : KMonoBehaviour, ISim200ms
{
	public ComplexFabricatorWorkable Workable
	{
		get
		{
			return this.workable;
		}
	}

	public List<ComplexFabricator.UserOrder> GetUserOrders()
	{
		return this.userOrders;
	}

	public List<ComplexFabricator.MachineOrder> GetMachineOrders()
	{
		return this.machineOrders;
	}

	public int CurrentOrderIdx
	{
		get
		{
			return this.currentOrderIdx;
		}
	}

	public ComplexFabricator.MachineOrder CurrentMachineOrder
	{
		get
		{
			return (this.machineOrders.Count <= 0) ? null : this.machineOrders[0];
		}
	}

	public bool IsSearchHandleActive
	{
		get
		{
			return this.ingredientSearchHandle.IsValid;
		}
	}

	public int NumOrders
	{
		get
		{
			return this.userOrders.Count;
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

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.choreType = Db.Get().ChoreTypes.Fabricate;
		this.choreTags = new Tag[] { GameTags.ChoreTypes.Fabricating };
		base.Subscribe<ComplexFabricator>(-1957399615, ComplexFabricator.OnDroppedAllDelegate);
		base.Subscribe<ComplexFabricator>(-592767678, ComplexFabricator.OnOperationalChangedDelegate);
		this.workable = base.GetComponent<ComplexFabricatorWorkable>();
		if (this.duplicantOperated)
		{
			this.workable.WorkerStatusItem = Db.Get().DuplicantStatusItems.Processing;
			this.workable.AttributeConvertor = Db.Get().AttributeConverters.MachinerySpeed;
			this.workable.AttributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		}
		Components.ComplexFabricators.Add(this);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.duplicantOperated)
		{
			this.workable = base.GetComponent<ComplexFabricatorWorkable>();
		}
		this.InitRecipeQueueCount();
		foreach (string text in this.recipeQueueCounts.Keys)
		{
			if (this.recipeQueueCounts[text] == ComplexFabricator.MAX_QUEUE_SIZE + 1)
			{
				this.recipeQueueCounts[text] = ComplexFabricator.QUEUE_INFINITE;
			}
		}
		this.RefreshUserOrdersFromQueueCounts();
		this.buildStorage.Transfer(this.inStorage, true, true);
		this.UpdateMachineOrders(true);
		base.Subscribe<ComplexFabricator>(-905833192, ComplexFabricator.OnCopySettingsDelegate);
		base.Subscribe(-1697596308, new Action<object>(this.OnStorageChanged));
	}

	private void OnStorageChanged(object data = null)
	{
		if (this.machineOrders.Count > 0 && (this.machineOrders[0].chore != null || (!this.duplicantOperated && this.machineOrders[0].underway)))
		{
			if ((this.workable != null && this.workable.WorkTimeRemaining < 0f) || this.orderProgress >= this.machineOrders[0].parentOrder.recipe.time)
			{
				return;
			}
			foreach (ComplexRecipe.RecipeElement recipeElement in this.machineOrders[0].parentOrder.recipe.ingredients)
			{
				if (this.buildStorage.GetAmountAvailable(recipeElement.material) < recipeElement.amount)
				{
					this.CancelAllMachineOrders();
					this.UpdateMachineOrders(false);
					break;
				}
			}
		}
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		if (gameObject == null)
		{
			return;
		}
		ComplexFabricator component = gameObject.GetComponent<ComplexFabricator>();
		if (component == null)
		{
			return;
		}
		foreach (string text in this.recipeQueueCounts.Keys.ToArray<string>())
		{
			if (component.recipeQueueCounts.ContainsKey(text))
			{
				this.recipeQueueCounts[text] = component.recipeQueueCounts[text];
			}
			else
			{
				this.recipeQueueCounts[text] = 0;
			}
		}
		this.RefreshUserOrdersFromQueueCounts();
		this.UpdateMachineOrders(false);
	}

	protected override void OnCleanUp()
	{
		this.ingredientSearchHandle.ClearScheduler();
		foreach (ComplexFabricator.UserOrder userOrder in this.userOrders)
		{
			this.CancelMachineOrdersByUserOrder(userOrder);
		}
		Components.ComplexFabricators.Remove(this);
		base.OnCleanUp();
	}

	public ComplexRecipe[] GetRecipes()
	{
		if (this.possible_recipes_cache == null)
		{
			KPrefabID component = base.GetComponent<KPrefabID>();
			Tag prefabTag = component.PrefabTag;
			List<ComplexRecipe> recipes = ComplexRecipeManager.Get().recipes;
			List<ComplexRecipe> list = new List<ComplexRecipe>();
			foreach (ComplexRecipe complexRecipe in recipes)
			{
				foreach (Tag tag in complexRecipe.fabricators)
				{
					if (tag == prefabTag)
					{
						list.Add(complexRecipe);
					}
				}
			}
			this.possible_recipes_cache = list.ToArray();
		}
		return this.possible_recipes_cache;
	}

	private void InitRecipeQueueCount()
	{
		foreach (ComplexRecipe complexRecipe in this.GetRecipes())
		{
			bool flag = false;
			foreach (string text in this.recipeQueueCounts.Keys)
			{
				if (text == complexRecipe.id)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				this.recipeQueueCounts.Add(complexRecipe.id, 0);
			}
		}
	}

	private void RefreshUserOrdersFromQueueCounts()
	{
		ComplexFabricator.UserOrder prevCurrentOrder = null;
		if (this.userOrders != null && this.currentOrderIdx != -1 && this.currentOrderIdx < this.userOrders.Count)
		{
			prevCurrentOrder = this.userOrders[this.currentOrderIdx];
		}
		this.userOrders.Clear();
		int num = 0;
		foreach (KeyValuePair<string, int> keyValuePair in this.recipeQueueCounts)
		{
			if (keyValuePair.Value > 0 || keyValuePair.Value == ComplexFabricator.QUEUE_INFINITE)
			{
				num++;
				ComplexFabricator.UserOrder userOrder = new ComplexFabricator.UserOrder(ComplexRecipeManager.Get().GetRecipe(keyValuePair.Key), true);
				this.userOrders.Add(userOrder);
			}
		}
		if (prevCurrentOrder != null)
		{
			int num2 = this.userOrders.FindIndex((ComplexFabricator.UserOrder match) => match.recipe == prevCurrentOrder.recipe);
			if (num2 != -1)
			{
				this.userOrders = this.ShiftListLeft<ComplexFabricator.UserOrder>(this.userOrders, num2);
			}
		}
	}

	public List<T> ShiftListLeft<T>(List<T> list, int shiftBy)
	{
		if (list.Count <= shiftBy)
		{
			return list;
		}
		List<T> range = list.GetRange(shiftBy, list.Count - shiftBy);
		range.AddRange(list.GetRange(0, shiftBy));
		return range;
	}

	public int GetRecipeQueueCount(ComplexRecipe recipe)
	{
		return this.recipeQueueCounts[recipe.id];
	}

	public void SetRecipeQueueCount(ComplexRecipe recipe, int count)
	{
		bool flag = true;
		if (this.machineOrders.Count == 0)
		{
			flag = true;
		}
		ComplexFabricator.UserOrder userOrder = this.userOrders.Find((ComplexFabricator.UserOrder match) => match.recipe == recipe);
		if (userOrder != null && this.GetUserOrderIndex(userOrder) == this.currentOrderIdx && this.GetRecipeQueueCount(recipe) == ComplexFabricator.QUEUE_INFINITE)
		{
			this.CancelMachineOrdersByUserOrder(userOrder);
		}
		this.recipeQueueCounts[recipe.id] = count;
		this.RefreshUserOrdersFromQueueCounts();
		if (flag)
		{
			this.UpdateMachineOrders(false);
		}
	}

	public void IncrementRecipeQueueCount(ComplexRecipe recipe)
	{
		if (this.machineOrders.Count == 0)
		{
		}
		ComplexFabricator.UserOrder userOrder = this.userOrders.Find((ComplexFabricator.UserOrder match) => match.recipe == recipe);
		if (userOrder != null && this.GetUserOrderIndex(userOrder) == this.currentOrderIdx && this.GetRecipeQueueCount(recipe) == ComplexFabricator.QUEUE_INFINITE)
		{
			this.CancelMachineOrdersByUserOrder(userOrder);
		}
		bool flag;
		if (this.recipeQueueCounts[recipe.id] == ComplexFabricator.QUEUE_INFINITE)
		{
			this.recipeQueueCounts[recipe.id] = 0;
			flag = true;
		}
		else if (this.recipeQueueCounts[recipe.id] >= ComplexFabricator.MAX_QUEUE_SIZE)
		{
			this.recipeQueueCounts[recipe.id] = ComplexFabricator.QUEUE_INFINITE;
			flag = true;
		}
		else
		{
			Dictionary<string, int> dictionary;
			string id;
			(dictionary = this.recipeQueueCounts)[id = recipe.id] = dictionary[id] + 1;
			flag = true;
		}
		this.RefreshUserOrdersFromQueueCounts();
		if (flag)
		{
			this.UpdateMachineOrders(false);
		}
	}

	public void DecrementRecipeQueueCount(ComplexRecipe recipe, bool respectInfinite = true)
	{
		bool flag = false;
		ComplexFabricator.UserOrder userOrder = this.userOrders.Find((ComplexFabricator.UserOrder match) => match.recipe == recipe);
		if (userOrder != null && this.GetRecipeQueueCount(recipe) == 1)
		{
			if (this.GetUserOrderIndex(userOrder) == this.currentOrderIdx)
			{
				flag = true;
			}
			this.CancelMachineOrdersByUserOrder(userOrder);
		}
		if (!respectInfinite || this.recipeQueueCounts[recipe.id] != ComplexFabricator.QUEUE_INFINITE)
		{
			if (this.recipeQueueCounts[recipe.id] == ComplexFabricator.QUEUE_INFINITE)
			{
				this.recipeQueueCounts[recipe.id] = ComplexFabricator.MAX_QUEUE_SIZE;
				flag = true;
			}
			else if (this.recipeQueueCounts[recipe.id] == 0)
			{
				this.recipeQueueCounts[recipe.id] = ComplexFabricator.QUEUE_INFINITE;
				flag = true;
			}
			else
			{
				Dictionary<string, int> dictionary;
				string id;
				(dictionary = this.recipeQueueCounts)[id = recipe.id] = dictionary[id] - 1;
				flag = true;
			}
		}
		this.RefreshUserOrdersFromQueueCounts();
		if (flag)
		{
			this.UpdateMachineOrders(false);
		}
	}

	private int GetTotalQueuedCount()
	{
		int num = 0;
		foreach (KeyValuePair<string, int> keyValuePair in this.recipeQueueCounts)
		{
			if (keyValuePair.Value == ComplexFabricator.QUEUE_INFINITE)
			{
				num += ComplexFabricator.MAX_QUEUE_SIZE + 1;
			}
			else
			{
				num += keyValuePair.Value;
			}
		}
		return num;
	}

	private List<ComplexFabricator.UserOrder> GetNextMachineOrders()
	{
		List<ComplexFabricator.UserOrder> list = new List<ComplexFabricator.UserOrder>();
		bool flag = false;
		int num = 0;
		while (list.Count < Mathf.Min(3, this.GetTotalQueuedCount()))
		{
			int num2 = (this.currentOrderIdx + num) % this.userOrders.Count;
			ComplexFabricator.UserOrder userOrder = this.userOrders[num2];
			if (userOrder.CheckMaterialRequirements(WorldInventory.Instance, this.inStorage) || userOrder.CheckMaterialRequirements(WorldInventory.Instance, this.buildStorage))
			{
				list.Add(userOrder);
			}
			else
			{
				flag = true;
			}
			num++;
			if (num > this.userOrders.Count * 3)
			{
				break;
			}
		}
		if (flag)
		{
			this.ScheduleCheckWorldInventory();
		}
		return list;
	}

	private bool CheckIngredientsInStorage(ComplexRecipe recipe)
	{
		foreach (ComplexRecipe.RecipeElement recipeElement in recipe.ingredients)
		{
			if (this.inStorage.GetAmountAvailable(recipeElement.material) < recipeElement.amount)
			{
				return false;
			}
		}
		return true;
	}

	private void ClearInvalidMachineOrders(List<ComplexFabricator.UserOrder> nextMachineOrderSources)
	{
		int num = -1;
		for (int i = 0; i < this.machineOrders.Count; i++)
		{
			if (nextMachineOrderSources.Count <= i)
			{
				num = i;
				break;
			}
			if (this.machineOrders[i].parentOrder.recipe != nextMachineOrderSources[i].recipe)
			{
				num = i;
				break;
			}
		}
		if (num != -1)
		{
			for (int j = this.machineOrders.Count - 1; j >= num; j--)
			{
				DebugUtil.DevAssertWithStack(!this.willBeSadIfMachineOrdersChanges, new object[] { "machineOrders changed when it wasn't expected. sad." });
				if (j == 0 && this.machineOrders[0].chore != null)
				{
					this.buildStorage.Transfer(this.inStorage, true, true);
				}
				this.OnMachineOrderCancelledOrComplete(this.machineOrders[j]);
				this.machineOrders[j].Cancel();
				this.machineOrders.RemoveAt(j);
			}
		}
	}

	private void AddValidMachineOrders(List<ComplexFabricator.UserOrder> nextMachineOrderSources)
	{
		for (int i = this.machineOrders.Count; i < nextMachineOrderSources.Count; i++)
		{
			DebugUtil.DevAssertWithStack(!this.willBeSadIfMachineOrdersChanges, new object[] { "machineOrders changed when it wasn't expected. sad." });
			ComplexFabricator.MachineOrder machineOrder = new ComplexFabricator.MachineOrder();
			machineOrder.parentOrder = nextMachineOrderSources[i];
			this.machineOrders.Add(machineOrder);
			this.OnCreateMachineOrder(machineOrder);
		}
	}

	private void RefreshMachineOrderList()
	{
		List<ComplexFabricator.UserOrder> nextMachineOrders = this.GetNextMachineOrders();
		this.ClearInvalidMachineOrders(nextMachineOrders);
		this.AddValidMachineOrders(nextMachineOrders);
	}

	protected void UpdateMachineOrders(bool force_update = false)
	{
		if (!force_update && !this.operational.IsOperational)
		{
			return;
		}
		this.RefreshMachineOrderList();
		if (this.machineOrders.Count > 0)
		{
			if (!this.machineOrders[0].underway && this.machineOrders[0].parentOrder.CheckMaterialRequirements(WorldInventory.Instance, this.inStorage))
			{
				if (this.duplicantOperated)
				{
					this.machineOrders[0].underway = true;
				}
				else if (!this.operational.IsActive && !this.machineOrders[0].underway && this.HasIngredients(this.machineOrders[0], this.inStorage))
				{
					this.TransferCurrentRecipeIngredientsForBuild();
					this.machineOrders[0].underway = true;
				}
			}
			if (this.duplicantOperated && this.machineOrders[0].underway && this.machineOrders[0].chore == null && this.HasIngredients(this.machineOrders[0], this.inStorage))
			{
				this.TransferCurrentRecipeIngredientsForBuild();
				this.workable.CreateOrder(this.machineOrders[0], this.choreType, this.choreTags);
				this.workable.ResetWorkTime();
			}
			Dictionary<Tag, float> dictionary = new Dictionary<Tag, float>();
			for (int i = 0; i < this.machineOrders.Count; i++)
			{
				ComplexFabricator.MachineOrder machineOrder = this.machineOrders[i];
				if (machineOrder.chore == null)
				{
					ComplexFabricator.UserOrder parentOrder = machineOrder.parentOrder;
					ComplexRecipe.RecipeElement[] ingredients = parentOrder.recipe.ingredients;
					foreach (ComplexRecipe.RecipeElement recipeElement in ingredients)
					{
						dictionary[recipeElement.material] = this.inStorage.GetMassAvailable(recipeElement.material);
					}
				}
			}
			ChoreType byHash = Db.Get().ChoreTypes.GetByHash(this.fetchChoreTypeIdHash);
			Dictionary<Tag, float> dictionary2 = new Dictionary<Tag, float>();
			for (int k = 0; k < this.machineOrders.Count; k++)
			{
				ComplexFabricator.MachineOrder machineOrder2 = this.machineOrders[k];
				if (machineOrder2.chore == null && (!machineOrder2.underway || this.duplicantOperated))
				{
					ComplexFabricator.UserOrder parentOrder2 = machineOrder2.parentOrder;
					ComplexRecipe.RecipeElement[] ingredients2 = parentOrder2.recipe.ingredients;
					bool flag = true;
					foreach (ComplexRecipe.RecipeElement recipeElement2 in ingredients2)
					{
						if (dictionary[recipeElement2.material] < recipeElement2.amount)
						{
							if (dictionary2.ContainsKey(recipeElement2.material))
							{
								Dictionary<Tag, float> dictionary3;
								Tag material;
								(dictionary3 = dictionary2)[material = recipeElement2.material] = dictionary3[material] + (recipeElement2.amount - dictionary[recipeElement2.material]);
							}
							else
							{
								dictionary2.Add(recipeElement2.material, recipeElement2.amount - dictionary[recipeElement2.material]);
							}
							dictionary[recipeElement2.material] = 0f;
							flag = false;
						}
						else
						{
							Dictionary<Tag, float> dictionary3;
							Tag material2;
							(dictionary3 = dictionary)[material2 = recipeElement2.material] = dictionary3[material2] - recipeElement2.amount;
						}
					}
					int num = -k;
					if (machineOrder2.fetchList == null && !flag)
					{
						machineOrder2.fetchList = new FetchList2(this.inStorage, byHash, this.choreTags);
						machineOrder2.fetchList.ShowStatusItem = false;
						machineOrder2.fetchList.SetPriorityMod(num);
						ComplexRecipe.RecipeElement[] array3 = new ComplexRecipe.RecipeElement[dictionary2.Count];
						int num2 = 0;
						foreach (Tag tag in dictionary2.Keys.ToList<Tag>())
						{
							float num3 = 0f;
							foreach (ComplexRecipe.RecipeElement recipeElement3 in machineOrder2.parentOrder.recipe.ingredients)
							{
								if (recipeElement3.material == tag)
								{
									num3 = recipeElement3.amount;
									break;
								}
							}
							float num4 = Mathf.Min(num3, dictionary2[tag]);
							if (num4 != 0f)
							{
								array3[num2] = new ComplexRecipe.RecipeElement(tag, num4);
								Dictionary<Tag, float> dictionary3;
								Tag tag2;
								(dictionary3 = dictionary2)[tag2 = tag] = dictionary3[tag2] - num4;
							}
							num2++;
						}
						this.AddIngredientsToFetchList(array3, machineOrder2.fetchList);
						machineOrder2.fetchList.Submit(new global::System.Action(this.OnFetchComplete), false);
					}
					else if (machineOrder2.fetchList != null)
					{
						machineOrder2.fetchList.SetPriorityMod(num);
					}
				}
			}
		}
		base.Trigger(1721324763, this);
		if (this.machineOrders.Count > 0)
		{
			this.SetCurrentUserOrderByMachineOrder(this.machineOrders[0]);
		}
	}

	protected virtual void TransferCurrentRecipeIngredientsForBuild()
	{
		ComplexRecipe.RecipeElement[] ingredients = this.machineOrders[0].parentOrder.recipe.ingredients;
		foreach (ComplexRecipe.RecipeElement recipeElement in ingredients)
		{
			while (this.buildStorage.GetAmountAvailable(recipeElement.material) < recipeElement.amount)
			{
				this.inStorage.Transfer(this.buildStorage, recipeElement.material, recipeElement.amount, false, true);
				if (this.inStorage.GetAmountAvailable(recipeElement.material) <= 0f)
				{
					break;
				}
			}
		}
	}

	protected virtual bool HasIngredients(ComplexFabricator.MachineOrder order, Storage storage)
	{
		ComplexRecipe.RecipeElement[] ingredients = order.parentOrder.recipe.ingredients;
		bool flag = true;
		foreach (ComplexRecipe.RecipeElement recipeElement in ingredients)
		{
			float massAvailable = storage.GetMassAvailable(recipeElement.material);
			if (massAvailable < recipeElement.amount)
			{
				flag = false;
				break;
			}
		}
		return flag;
	}

	public void Sim200ms(float dt)
	{
		if (this.duplicantOperated)
		{
			return;
		}
		if (this.operational.IsOperational && !this.operational.IsActive && this.machineOrders.Count > 0 && this.machineOrders[0].underway)
		{
			this.StartWork();
		}
		if (this.operational.IsActive)
		{
			bool flag = this.machineOrders.Count > 0;
			if (flag)
			{
				this.orderProgress += dt / this.machineOrders[0].parentOrder.recipe.time;
				if (this.orderProgress >= 1f)
				{
					this.machineOrders[0].underway = false;
					this.SetOperationalInactive();
					this.OnCompleteMachineOrder();
					this.orderProgress = 0f;
				}
			}
		}
	}

	private void CancelMachineOrder(ComplexFabricator.MachineOrder order)
	{
		DebugUtil.DevAssertWithStack(!this.willBeSadIfMachineOrdersChanges, new object[] { "machineOrders changed when it wasn't expected. sad." });
		this.OnMachineOrderCancelledOrComplete(order);
		order.Cancel();
		this.machineOrders.Remove(order);
		if (!this.duplicantOperated && order.underway)
		{
			this.buildStorage.Transfer(this.inStorage, true, true);
			this.SetOperationalInactive();
			this.orderProgress = 0f;
		}
		else if (this.duplicantOperated && (order.chore != null || order.underway))
		{
			this.buildStorage.Transfer(this.inStorage, true, true);
			this.SetOperationalInactive();
			this.orderProgress = 0f;
		}
	}

	private void CancelMachineOrdersByUserOrder(ComplexFabricator.UserOrder order)
	{
		this.isCancellingOrder = true;
		for (int i = this.machineOrders.Count - 1; i >= 0; i--)
		{
			ComplexFabricator.MachineOrder machineOrder = this.machineOrders[i];
			if (machineOrder.parentOrder.recipe == order.recipe)
			{
				this.CancelMachineOrder(this.machineOrders[i]);
			}
		}
		if (this.OnUserOrderCancelledOrComplete != null)
		{
			this.OnUserOrderCancelledOrComplete(order);
		}
		this.isCancellingOrder = false;
	}

	private void CancelAllMachineOrders()
	{
		this.buildStorage.Transfer(this.inStorage, true, true);
		DebugUtil.DevAssertWithStack(this.machineOrders.Count == 0 || !this.willBeSadIfMachineOrdersChanges, new object[] { "machineOrders changed when it wasn't expected. sad." });
		while (this.machineOrders.Count > 0)
		{
			ComplexFabricator.MachineOrder machineOrder = this.machineOrders[0];
			machineOrder.Cancel();
			if (this.machineOrders.Count > 0 && this.machineOrders[0] == machineOrder)
			{
				this.OnMachineOrderCancelledOrComplete(this.machineOrders[0]);
				this.machineOrders.RemoveAt(0);
			}
		}
	}

	protected virtual List<GameObject> SpawnOrderProduct(ComplexFabricator.UserOrder completed_order)
	{
		List<GameObject> list = new List<GameObject>();
		SimUtil.DiseaseInfo diseaseInfo;
		diseaseInfo.count = 0;
		diseaseInfo.idx = 0;
		float num = 0f;
		float num2 = 0f;
		foreach (ComplexRecipe.RecipeElement recipeElement in completed_order.recipe.ingredients)
		{
			num2 += recipeElement.amount;
		}
		foreach (ComplexRecipe.RecipeElement recipeElement2 in completed_order.recipe.ingredients)
		{
			float num3 = recipeElement2.amount / num2;
			SimUtil.DiseaseInfo diseaseInfo2;
			float num4;
			this.buildStorage.ConsumeAndGetDisease(recipeElement2.material, recipeElement2.amount, out diseaseInfo2, out num4);
			if (diseaseInfo2.count > diseaseInfo.count)
			{
				diseaseInfo = diseaseInfo2;
			}
			num += num4 * num3;
		}
		foreach (ComplexRecipe.RecipeElement recipeElement3 in completed_order.recipe.results)
		{
			GameObject gameObject = this.buildStorage.FindFirst(recipeElement3.material);
			if (gameObject != null)
			{
				Edible component = gameObject.GetComponent<Edible>();
				if (component)
				{
					ReportManager.Instance.ReportValue(ReportManager.ReportType.CaloriesCreated, -component.Calories, StringFormatter.Replace(UI.ENDOFDAYREPORT.NOTES.CRAFTED_USED, "{0}", component.GetProperName()), UI.ENDOFDAYREPORT.NOTES.CRAFTED_CONTEXT);
				}
			}
			ComplexFabricator.ResultState resultState = this.resultState;
			if (resultState != ComplexFabricator.ResultState.Normal && resultState != ComplexFabricator.ResultState.Hot)
			{
				if (resultState == ComplexFabricator.ResultState.Melted)
				{
					if (this.storeProduced)
					{
						float num5 = ElementLoader.GetElement(recipeElement3.material).lowTemp + (ElementLoader.GetElement(recipeElement3.material).highTemp - ElementLoader.GetElement(recipeElement3.material).lowTemp) / 2f;
						this.outStorage.AddLiquid(ElementLoader.GetElementID(recipeElement3.material), recipeElement3.amount, num5, 0, 0, false, true);
					}
				}
			}
			else
			{
				GameObject prefab = Assets.GetPrefab(recipeElement3.material);
				GameObject gameObject2 = GameUtil.KInstantiate(prefab, Grid.SceneLayer.Ore, null, 0);
				int num6 = Grid.PosToCell(this);
				gameObject2.transform.SetPosition(Grid.CellToPosCCC(num6, Grid.SceneLayer.Ore) + this.outputOffset);
				PrimaryElement component2 = gameObject2.GetComponent<PrimaryElement>();
				component2.Units = recipeElement3.amount;
				component2.Temperature = num;
				gameObject2.SetActive(true);
				float num7 = recipeElement3.amount / completed_order.recipe.TotalResultUnits();
				component2.AddDisease(diseaseInfo.idx, Mathf.RoundToInt((float)diseaseInfo.count * num7), "ComplexFabricator.CompleteOrder");
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
					if (symbol != null)
					{
						component3.TryRemoveSymbolOverride("output_tracker", 0);
						component3.AddSymbolOverride("output_tracker", symbol, 0);
					}
					else
					{
						global::Debug.LogWarning(component3.name + " is missing symbol " + build.name, null);
					}
				}
			}
		}
		return list;
	}

	private void PollInventory(object data = null)
	{
		bool flag = false;
		for (int i = 0; i < Mathf.Min(3, this.GetTotalQueuedCount()); i++)
		{
			int num = (this.currentOrderIdx + i) % this.userOrders.Count;
			ComplexFabricator.UserOrder userOrder = this.userOrders[num];
			bool flag2 = false;
			foreach (ComplexFabricator.MachineOrder machineOrder in this.machineOrders)
			{
				if (machineOrder.parentOrder.recipe == userOrder.recipe)
				{
					flag2 = true;
					break;
				}
			}
			if (!flag2)
			{
				if (userOrder.CheckMaterialRequirements(WorldInventory.Instance, this.inStorage))
				{
					flag = true;
					break;
				}
			}
		}
		this.StopCheckWorldInventory();
		if (flag)
		{
			this.UpdateMachineOrders(false);
		}
		else
		{
			this.ScheduleCheckWorldInventory();
		}
	}

	private void ScheduleCheckWorldInventory()
	{
		if (!this.ingredientSearchHandle.IsValid)
		{
			this.ingredientSearchHandle = GameScheduler.Instance.Schedule("Idle ComplexFabricator look for ingredients", 4f, new Action<object>(this.PollInventory), null, null);
		}
	}

	private void StopCheckWorldInventory()
	{
		if (this.ingredientSearchHandle.IsValid)
		{
			this.ingredientSearchHandle.ClearScheduler();
		}
	}

	public void SetCurrentUserOrderByMachineOrder(ComplexFabricator.MachineOrder nextMachineOrder)
	{
		if (nextMachineOrder == null)
		{
			this.currentOrderIdx = 0;
		}
		else
		{
			this.currentOrderIdx = this.GetUserOrderIndex(nextMachineOrder.parentOrder);
		}
	}

	private void AddIngredientsToFetchList(ComplexRecipe.RecipeElement[] ingredients, FetchList2 fetchList)
	{
		if (fetchList == null || ingredients == null || ingredients.Length == 0)
		{
			global::Debug.LogError("Invalid parameters received for the fetch list.", null);
			return;
		}
		foreach (ComplexRecipe.RecipeElement recipeElement in ingredients)
		{
			if (recipeElement != null && recipeElement.amount > 0f)
			{
				Tag material = recipeElement.material;
				float amount = recipeElement.amount;
				fetchList.Add(material, null, null, amount, FetchOrder2.OperationalRequirement.None);
			}
		}
	}

	private void StartWork()
	{
		this.operational.SetActive(true, false);
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

	private void SetOperationalInactive()
	{
		this.operational.SetActive(false, false);
		this.ShowProgressBar(false);
	}

	private void OnFetchComplete()
	{
		this.UpdateMachineOrders(false);
	}

	private bool CanFabricate(ComplexFabricator.UserOrder order, Storage storage)
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
		foreach (ComplexRecipe complexRecipe in recipes)
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

	public void OnCompleteMachineOrder()
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
		this.willBeSadIfMachineOrdersChanges = true;
		this.SpawnOrderProduct(this.machineOrders[0].parentOrder);
		this.buildStorage.Transfer(this.outStorage, true, true);
		this.OnMachineOrderCancelledOrComplete(this.machineOrders[0]);
		int userOrderIndex = this.GetUserOrderIndex(this.machineOrders[0].parentOrder);
		this.machineOrders.RemoveAt(0);
		this.willBeSadIfMachineOrdersChanges = false;
		this.DecrementRecipeQueueCount(this.userOrders[userOrderIndex].recipe, true);
		this.SetCurrentUserOrderByMachineOrder((this.machineOrders.Count <= 0) ? null : this.machineOrders[0]);
		this.UpdateMachineOrders(false);
		this.ShowProgressBar(false);
	}

	private int GetUserOrderIndex(ComplexFabricator.UserOrder order)
	{
		for (int i = 0; i < this.userOrders.Count; i++)
		{
			if (this.userOrders[i].recipe == order.recipe)
			{
				return i;
			}
		}
		global::Debug.LogError("Could not find user order index", null);
		return -1;
	}

	private void OnDroppedAll(object data)
	{
	}

	private void OnOperationalChanged(object data)
	{
		bool flag = (bool)data;
		if (flag)
		{
			this.UpdateMachineOrders(false);
		}
	}

	public virtual List<Descriptor> AdditionalEffectsForRecipe(ComplexRecipe recipe)
	{
		return new List<Descriptor>();
	}

	public string GetConversationTopic()
	{
		if (this.machineOrders.Count > 0)
		{
			ComplexFabricator.UserOrder parentOrder = this.machineOrders[0].parentOrder;
			ComplexRecipe recipe = parentOrder.recipe;
			return recipe.results[0].material.Name;
		}
		return null;
	}

	public bool duplicantOperated = true;

	protected ComplexFabricatorWorkable workable;

	public Action<ComplexFabricator.UserOrder> OnUserOrderCancelledOrComplete;

	public Action<ComplexFabricator.MachineOrder> OnCreateMachineOrder;

	public Action<ComplexFabricator.MachineOrder> OnMachineOrderCancelledOrComplete;

	[SerializeField]
	public HashedString fetchChoreTypeIdHash = Db.Get().ChoreTypes.MachineFetch.IdHash;

	[SerializeField]
	public ComplexFabricator.ResultState resultState;

	[SerializeField]
	public bool storeProduced;

	public ComplexFabricatorSideScreen.StyleSetting sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;

	public bool labelByResult = true;

	public Vector3 outputOffset = Vector3.zero;

	private const int MaxPrefetchCount = 3;

	protected ChoreType choreType;

	protected Tag[] choreTags;

	public static int MAX_QUEUE_SIZE = 99;

	public static int QUEUE_INFINITE = -1;

	[Serialize]
	private Dictionary<string, int> recipeQueueCounts = new Dictionary<string, int>();

	[Serialize]
	public bool clearUserOrderOnComplete;

	protected List<ComplexFabricator.UserOrder> userOrders = new List<ComplexFabricator.UserOrder>();

	protected List<ComplexFabricator.MachineOrder> machineOrders = new List<ComplexFabricator.MachineOrder>();

	[Serialize]
	private int currentOrderIdx;

	private bool isCancellingOrder;

	private float orderProgress;

	private bool willBeSadIfMachineOrdersChanges;

	private ComplexRecipe[] possible_recipes_cache;

	private SchedulerHandle ingredientSearchHandle;

	[SerializeField]
	public Storage inStorage;

	[SerializeField]
	public Storage buildStorage;

	[SerializeField]
	public Storage outStorage;

	[MyCmpAdd]
	private LoopingSounds loopingSounds;

	[MyCmpGet]
	private OutputPoint outputPoint;

	[MyCmpReq]
	protected Operational operational;

	[MyCmpAdd]
	private ComplexFabricatorSM fabricatorSM;

	private MeterController outputVisualizer;

	private ProgressBar progressBar;

	private static readonly EventSystem.IntraObjectHandler<ComplexFabricator> OnDroppedAllDelegate = new EventSystem.IntraObjectHandler<ComplexFabricator>(delegate(ComplexFabricator component, object data)
	{
		component.OnDroppedAll(data);
	});

	private static readonly EventSystem.IntraObjectHandler<ComplexFabricator> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<ComplexFabricator>(delegate(ComplexFabricator component, object data)
	{
		component.OnOperationalChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<ComplexFabricator> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<ComplexFabricator>(delegate(ComplexFabricator component, object data)
	{
		component.OnCopySettings(data);
	});

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

	[Serializable]
	public class UserOrder
	{
		public UserOrder(ComplexRecipe recipe, bool infinite = false)
		{
			this.recipe = recipe;
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

		public bool CheckMaterialRequirements(WorldInventory worldInventory, Storage storage)
		{
			Dictionary<Tag, float> dictionary = new Dictionary<Tag, float>();
			foreach (ComplexRecipe.RecipeElement recipeElement in this.recipe.ingredients)
			{
				if (worldInventory.GetAmount(recipeElement.material) + storage.GetAmountAvailable(recipeElement.material) < recipeElement.amount)
				{
					return false;
				}
			}
			return true;
		}

		public ComplexRecipe recipe;
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

		public ComplexFabricator.UserOrder parentOrder;

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
}
