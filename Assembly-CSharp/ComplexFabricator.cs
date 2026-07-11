using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Klei;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class ComplexFabricator : KMonoBehaviour, ISim200ms, ISim1000ms
{
	public ComplexFabricatorWorkable Workable
	{
		get
		{
			return this.workable;
		}
	}

	public int CurrentOrderIdx
	{
		get
		{
			return this.nextOrderIdx;
		}
	}

	public ComplexRecipe CurrentWorkingOrder
	{
		get
		{
			if (!this.HasWorkingOrder)
			{
				return null;
			}
			return this.recipe_list[this.workingOrderIdx];
		}
	}

	public ComplexRecipe NextOrder
	{
		get
		{
			if (!this.nextOrderIsWorkable)
			{
				return null;
			}
			return this.recipe_list[this.nextOrderIdx];
		}
	}

	public float OrderProgress
	{
		get
		{
			return this.orderProgress;
		}
		set
		{
			this.orderProgress = value;
		}
	}

	public bool HasAnyOrder
	{
		get
		{
			return this.HasWorkingOrder || this.hasOpenOrders;
		}
	}

	public bool HasWorker
	{
		get
		{
			return !this.duplicantOperated || this.workable.worker != null;
		}
	}

	public bool WaitingForWorker
	{
		get
		{
			return this.HasWorkingOrder && !this.HasWorker;
		}
	}

	private bool HasWorkingOrder
	{
		get
		{
			return this.workingOrderIdx > -1;
		}
	}

	public List<FetchList2> DebugFetchLists
	{
		get
		{
			return this.fetchListList;
		}
	}

	[OnDeserialized]
	protected virtual void OnDeserializedMethod()
	{
		List<string> list = new List<string>();
		foreach (string text in this.recipeQueueCounts.Keys)
		{
			if (ComplexRecipeManager.Get().GetRecipe(text) == null)
			{
				list.Add(text);
			}
		}
		foreach (string text2 in list)
		{
			global::Debug.LogWarningFormat("{1} removing missing recipe from queue: {0}", new object[] { text2, base.name });
			this.recipeQueueCounts.Remove(text2);
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.GetRecipes();
		this.simRenderLoadBalance = true;
		this.choreType = Db.Get().ChoreTypes.Fabricate;
		base.Subscribe<ComplexFabricator>(-1957399615, ComplexFabricator.OnDroppedAllDelegate);
		base.Subscribe<ComplexFabricator>(-592767678, ComplexFabricator.OnOperationalChangedDelegate);
		base.Subscribe<ComplexFabricator>(-905833192, ComplexFabricator.OnCopySettingsDelegate);
		base.Subscribe<ComplexFabricator>(-1697596308, ComplexFabricator.OnStorageChangeDelegate);
		this.workable = base.GetComponent<ComplexFabricatorWorkable>();
		Components.ComplexFabricators.Add(this);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.InitRecipeQueueCount();
		foreach (string text in this.recipeQueueCounts.Keys)
		{
			if (this.recipeQueueCounts[text] == 100)
			{
				this.recipeQueueCounts[text] = ComplexFabricator.QUEUE_INFINITE;
			}
		}
		this.buildStorage.Transfer(this.inStorage, true, true);
		this.DropExcessIngredients(this.inStorage);
		int num = this.FindRecipeIndex(this.lastWorkingRecipe);
		if (num > -1)
		{
			this.nextOrderIdx = num;
		}
	}

	protected override void OnCleanUp()
	{
		this.CancelAllOpenOrders();
		this.CancelChore();
		Components.ComplexFabricators.Remove(this);
		base.OnCleanUp();
	}

	private void OnOperationalChanged(object data)
	{
		if ((bool)data)
		{
			this.queueDirty = true;
		}
		else
		{
			this.CancelAllOpenOrders();
		}
		this.UpdateChore();
	}

	public void Sim1000ms(float dt)
	{
		this.RefreshAndStartNextOrder();
		if (this.materialNeedCache.Count > 0 && this.fetchListList.Count == 0)
		{
			global::Debug.LogWarningFormat(base.gameObject, "{0} has material needs cached, but no open fetches. materialNeedCache={1}, fetchListList={2}", new object[]
			{
				base.gameObject,
				this.materialNeedCache.Count,
				this.fetchListList.Count
			});
			this.queueDirty = true;
		}
	}

	public void Sim200ms(float dt)
	{
		if (!this.operational.IsOperational)
		{
			return;
		}
		this.operational.SetActive(this.HasWorkingOrder && this.HasWorker, false);
		if (!this.duplicantOperated && this.HasWorkingOrder)
		{
			ComplexRecipe complexRecipe = this.recipe_list[this.workingOrderIdx];
			this.orderProgress += dt / complexRecipe.time;
			if (this.orderProgress >= 1f)
			{
				this.CompleteWorkingOrder();
			}
		}
	}

	private void RefreshAndStartNextOrder()
	{
		if (!this.operational.IsOperational)
		{
			return;
		}
		if (this.queueDirty)
		{
			this.RefreshQueue();
		}
		if (!this.HasWorkingOrder && this.nextOrderIsWorkable)
		{
			this.StartWorkingOrder(this.nextOrderIdx);
		}
	}

	public void SetQueueDirty()
	{
		this.queueDirty = true;
	}

	private void RefreshQueue()
	{
		this.queueDirty = false;
		this.ValidateWorkingOrder();
		this.ValidateNextOrder();
		this.UpdateOpenOrders();
		this.DropExcessIngredients(this.inStorage);
		base.Trigger(1721324763, this);
	}

	private void StartWorkingOrder(int index)
	{
		global::Debug.Assert(!this.HasWorkingOrder, "machineOrderIdx already set");
		this.workingOrderIdx = index;
		if (this.recipe_list[this.workingOrderIdx].id != this.lastWorkingRecipe)
		{
			this.orderProgress = 0f;
			this.lastWorkingRecipe = this.recipe_list[this.workingOrderIdx].id;
		}
		this.TransferCurrentRecipeIngredientsForBuild();
		global::Debug.Assert(this.openOrderCounts[this.workingOrderIdx] > 0, "openOrderCount invalid");
		List<int> list = this.openOrderCounts;
		int num = this.workingOrderIdx;
		int num2 = list[num];
		list[num] = num2 - 1;
		this.UpdateChore();
		this.AdvanceNextOrder();
	}

	private void CancelWorkingOrder()
	{
		global::Debug.Assert(this.HasWorkingOrder, "machineOrderIdx not set");
		this.buildStorage.Transfer(this.inStorage, true, true);
		this.workingOrderIdx = -1;
		this.orderProgress = 0f;
		this.UpdateChore();
	}

	public void CompleteWorkingOrder()
	{
		if (!this.HasWorkingOrder)
		{
			global::Debug.LogWarning("CompleteWorkingOrder called with no working order.", base.gameObject);
			return;
		}
		ComplexRecipe complexRecipe = this.recipe_list[this.workingOrderIdx];
		this.SpawnOrderProduct(complexRecipe);
		float num = this.buildStorage.MassStored();
		if (num != 0f)
		{
			global::Debug.LogWarningFormat(base.gameObject, "{0} build storage contains mass {1} after order completion.", new object[] { base.gameObject, num });
			this.buildStorage.Transfer(this.inStorage, true, true);
		}
		this.DecrementRecipeQueueCountInternal(complexRecipe, true);
		this.workingOrderIdx = -1;
		this.orderProgress = 0f;
		this.CancelChore();
		if (!this.cancelling)
		{
			this.RefreshAndStartNextOrder();
		}
	}

	private void ValidateWorkingOrder()
	{
		if (!this.HasWorkingOrder)
		{
			return;
		}
		ComplexRecipe complexRecipe = this.recipe_list[this.workingOrderIdx];
		if (!this.IsRecipeQueued(complexRecipe))
		{
			this.CancelWorkingOrder();
		}
	}

	private void UpdateChore()
	{
		if (!this.duplicantOperated)
		{
			return;
		}
		bool flag = this.operational.IsOperational && this.HasWorkingOrder;
		if (flag && this.chore == null)
		{
			this.CreateChore();
			return;
		}
		if (!flag && this.chore != null)
		{
			this.CancelChore();
		}
	}

	private void AdvanceNextOrder()
	{
		for (int i = 0; i < this.recipe_list.Length; i++)
		{
			this.nextOrderIdx = (this.nextOrderIdx + 1) % this.recipe_list.Length;
			ComplexRecipe complexRecipe = this.recipe_list[this.nextOrderIdx];
			this.nextOrderIsWorkable = this.GetRemainingQueueCount(complexRecipe) > 0 && this.HasIngredients(complexRecipe, this.inStorage);
			if (this.nextOrderIsWorkable)
			{
				break;
			}
		}
	}

	private void ValidateNextOrder()
	{
		ComplexRecipe complexRecipe = this.recipe_list[this.nextOrderIdx];
		this.nextOrderIsWorkable = this.GetRemainingQueueCount(complexRecipe) > 0 && this.HasIngredients(complexRecipe, this.inStorage);
		if (!this.nextOrderIsWorkable)
		{
			this.AdvanceNextOrder();
		}
	}

	private void CancelAllOpenOrders()
	{
		for (int i = 0; i < this.openOrderCounts.Count; i++)
		{
			this.openOrderCounts[i] = 0;
		}
		this.ClearMaterialNeeds();
		this.CancelFetches();
	}

	private void UpdateOpenOrders()
	{
		ComplexRecipe[] recipes = this.GetRecipes();
		if (recipes.Length != this.openOrderCounts.Count)
		{
			global::Debug.LogErrorFormat(base.gameObject, "Recipe count {0} doesn't match open order count {1}", new object[]
			{
				recipes.Length,
				this.openOrderCounts.Count
			});
		}
		bool flag = false;
		this.hasOpenOrders = false;
		for (int i = 0; i < recipes.Length; i++)
		{
			ComplexRecipe complexRecipe = recipes[i];
			int recipePrefetchCount = this.GetRecipePrefetchCount(complexRecipe);
			if (recipePrefetchCount > 0)
			{
				this.hasOpenOrders = true;
			}
			int num = this.openOrderCounts[i];
			if (num != recipePrefetchCount)
			{
				if (recipePrefetchCount < num)
				{
					flag = true;
				}
				this.openOrderCounts[i] = recipePrefetchCount;
			}
		}
		DictionaryPool<Tag, float, ComplexFabricator>.PooledDictionary pooledDictionary = DictionaryPool<Tag, float, ComplexFabricator>.Allocate();
		DictionaryPool<Tag, float, ComplexFabricator>.PooledDictionary pooledDictionary2 = DictionaryPool<Tag, float, ComplexFabricator>.Allocate();
		for (int j = 0; j < this.openOrderCounts.Count; j++)
		{
			if (this.openOrderCounts[j] > 0)
			{
				foreach (ComplexRecipe.RecipeElement recipeElement in this.recipe_list[j].ingredients)
				{
					pooledDictionary[recipeElement.material] = this.inStorage.GetAmountAvailable(recipeElement.material);
				}
			}
		}
		for (int l = 0; l < this.recipe_list.Length; l++)
		{
			int num2 = this.openOrderCounts[l];
			if (num2 > 0)
			{
				foreach (ComplexRecipe.RecipeElement recipeElement2 in this.recipe_list[l].ingredients)
				{
					float num3 = recipeElement2.amount * (float)num2;
					float num4 = num3 - pooledDictionary[recipeElement2.material];
					if (num4 > 0f)
					{
						float num5;
						pooledDictionary2.TryGetValue(recipeElement2.material, out num5);
						pooledDictionary2[recipeElement2.material] = num5 + num4;
						pooledDictionary[recipeElement2.material] = 0f;
					}
					else
					{
						DictionaryPool<Tag, float, ComplexFabricator>.PooledDictionary pooledDictionary3 = pooledDictionary;
						Tag material = recipeElement2.material;
						pooledDictionary3[material] -= num3;
					}
				}
			}
		}
		if (flag)
		{
			this.CancelFetches();
		}
		if (pooledDictionary2.Count > 0)
		{
			this.UpdateFetches(pooledDictionary2);
		}
		this.UpdateMaterialNeeds(pooledDictionary2);
		pooledDictionary2.Recycle();
		pooledDictionary.Recycle();
	}

	private void UpdateMaterialNeeds(Dictionary<Tag, float> missingAmounts)
	{
		this.ClearMaterialNeeds();
		foreach (KeyValuePair<Tag, float> keyValuePair in missingAmounts)
		{
			MaterialNeeds.Instance.UpdateNeed(keyValuePair.Key, keyValuePair.Value);
			this.materialNeedCache.Add(keyValuePair.Key, keyValuePair.Value);
		}
	}

	private void ClearMaterialNeeds()
	{
		foreach (KeyValuePair<Tag, float> keyValuePair in this.materialNeedCache)
		{
			MaterialNeeds.Instance.UpdateNeed(keyValuePair.Key, -keyValuePair.Value);
		}
		this.materialNeedCache.Clear();
	}

	private void OnFetchComplete()
	{
		for (int i = this.fetchListList.Count - 1; i >= 0; i--)
		{
			if (this.fetchListList[i].IsComplete)
			{
				this.fetchListList.RemoveAt(i);
				this.queueDirty = true;
			}
		}
	}

	private void OnStorageChange(object data)
	{
		this.queueDirty = true;
	}

	private void OnDroppedAll(object data)
	{
		if (this.HasWorkingOrder)
		{
			this.CancelWorkingOrder();
		}
		this.CancelAllOpenOrders();
		this.RefreshQueue();
	}

	private void DropExcessIngredients(Storage storage)
	{
		TagBits tagBits = default(TagBits);
		tagBits.Or(ref this.keepAdditionalTags);
		for (int i = 0; i < this.recipe_list.Length; i++)
		{
			ComplexRecipe complexRecipe = this.recipe_list[i];
			if (this.IsRecipeQueued(complexRecipe))
			{
				foreach (ComplexRecipe.RecipeElement recipeElement in complexRecipe.ingredients)
				{
					tagBits.SetTag(recipeElement.material);
				}
			}
		}
		for (int k = storage.items.Count - 1; k >= 0; k--)
		{
			GameObject gameObject = storage.items[k];
			if (!(gameObject == null))
			{
				PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
				if (!(component == null) && (!this.keepExcessLiquids || !component.Element.IsLiquid))
				{
					KPrefabID component2 = gameObject.GetComponent<KPrefabID>();
					if (component2 && !component2.HasAnyTags(ref tagBits))
					{
						storage.Drop(gameObject, true);
					}
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
		foreach (ComplexRecipe complexRecipe in this.recipe_list)
		{
			int num;
			if (!component.recipeQueueCounts.TryGetValue(complexRecipe.id, out num))
			{
				num = 0;
			}
			this.SetRecipeQueueCountInternal(complexRecipe, num);
		}
		this.RefreshQueue();
	}

	private int CompareRecipe(ComplexRecipe a, ComplexRecipe b)
	{
		if (a.sortOrder != b.sortOrder)
		{
			return a.sortOrder - b.sortOrder;
		}
		return StringComparer.InvariantCulture.Compare(a.id, b.id);
	}

	public ComplexRecipe[] GetRecipes()
	{
		if (this.recipe_list == null)
		{
			Tag prefabTag = base.GetComponent<KPrefabID>().PrefabTag;
			List<ComplexRecipe> recipes = ComplexRecipeManager.Get().recipes;
			List<ComplexRecipe> list = new List<ComplexRecipe>();
			foreach (ComplexRecipe complexRecipe in recipes)
			{
				using (List<Tag>.Enumerator enumerator2 = complexRecipe.fabricators.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (enumerator2.Current == prefabTag)
						{
							list.Add(complexRecipe);
						}
					}
				}
			}
			this.recipe_list = list.ToArray();
			Array.Sort<ComplexRecipe>(this.recipe_list, new Comparison<ComplexRecipe>(this.CompareRecipe));
		}
		return this.recipe_list;
	}

	private void InitRecipeQueueCount()
	{
		foreach (ComplexRecipe complexRecipe in this.GetRecipes())
		{
			bool flag = false;
			using (Dictionary<string, int>.KeyCollection.Enumerator enumerator = this.recipeQueueCounts.Keys.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current == complexRecipe.id)
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				this.recipeQueueCounts.Add(complexRecipe.id, 0);
			}
			this.openOrderCounts.Add(0);
		}
	}

	private int FindRecipeIndex(string id)
	{
		for (int i = 0; i < this.recipe_list.Length; i++)
		{
			if (this.recipe_list[i].id == id)
			{
				return i;
			}
		}
		return -1;
	}

	public int GetRecipeQueueCount(ComplexRecipe recipe)
	{
		return this.recipeQueueCounts[recipe.id];
	}

	public bool IsRecipeQueued(ComplexRecipe recipe)
	{
		int num = this.recipeQueueCounts[recipe.id];
		global::Debug.Assert(num >= 0 || num == ComplexFabricator.QUEUE_INFINITE);
		return num != 0;
	}

	public int GetRecipePrefetchCount(ComplexRecipe recipe)
	{
		int remainingQueueCount = this.GetRemainingQueueCount(recipe);
		global::Debug.Assert(remainingQueueCount >= 0);
		return Mathf.Min(2, remainingQueueCount);
	}

	private int GetRemainingQueueCount(ComplexRecipe recipe)
	{
		int num = this.recipeQueueCounts[recipe.id];
		global::Debug.Assert(num >= 0 || num == ComplexFabricator.QUEUE_INFINITE);
		if (num == ComplexFabricator.QUEUE_INFINITE)
		{
			return ComplexFabricator.MAX_QUEUE_SIZE;
		}
		if (num > 0)
		{
			if (this.IsCurrentRecipe(recipe))
			{
				num--;
			}
			return num;
		}
		return 0;
	}

	private bool IsCurrentRecipe(ComplexRecipe recipe)
	{
		return this.workingOrderIdx >= 0 && this.recipe_list[this.workingOrderIdx].id == recipe.id;
	}

	public void SetRecipeQueueCount(ComplexRecipe recipe, int count)
	{
		this.SetRecipeQueueCountInternal(recipe, count);
		this.RefreshQueue();
	}

	private void SetRecipeQueueCountInternal(ComplexRecipe recipe, int count)
	{
		this.recipeQueueCounts[recipe.id] = count;
	}

	public void IncrementRecipeQueueCount(ComplexRecipe recipe)
	{
		if (this.recipeQueueCounts[recipe.id] == ComplexFabricator.QUEUE_INFINITE)
		{
			this.recipeQueueCounts[recipe.id] = 0;
		}
		else if (this.recipeQueueCounts[recipe.id] >= ComplexFabricator.MAX_QUEUE_SIZE)
		{
			this.recipeQueueCounts[recipe.id] = ComplexFabricator.QUEUE_INFINITE;
		}
		else
		{
			Dictionary<string, int> dictionary = this.recipeQueueCounts;
			string id = recipe.id;
			int num = dictionary[id];
			dictionary[id] = num + 1;
		}
		this.RefreshQueue();
	}

	public void DecrementRecipeQueueCount(ComplexRecipe recipe, bool respectInfinite = true)
	{
		this.DecrementRecipeQueueCountInternal(recipe, respectInfinite);
		this.RefreshQueue();
	}

	private void DecrementRecipeQueueCountInternal(ComplexRecipe recipe, bool respectInfinite = true)
	{
		if (!respectInfinite || this.recipeQueueCounts[recipe.id] != ComplexFabricator.QUEUE_INFINITE)
		{
			if (this.recipeQueueCounts[recipe.id] == ComplexFabricator.QUEUE_INFINITE)
			{
				this.recipeQueueCounts[recipe.id] = ComplexFabricator.MAX_QUEUE_SIZE;
				return;
			}
			if (this.recipeQueueCounts[recipe.id] == 0)
			{
				this.recipeQueueCounts[recipe.id] = ComplexFabricator.QUEUE_INFINITE;
				return;
			}
			Dictionary<string, int> dictionary = this.recipeQueueCounts;
			string id = recipe.id;
			int num = dictionary[id];
			dictionary[id] = num - 1;
		}
	}

	private void CreateChore()
	{
		global::Debug.Assert(this.chore == null, "chore should be null");
		this.chore = this.workable.CreateWorkChore(this.choreType, this.orderProgress);
	}

	private void CancelChore()
	{
		if (this.cancelling)
		{
			return;
		}
		this.cancelling = true;
		if (this.chore != null)
		{
			this.chore.Cancel("order cancelled");
			this.chore = null;
		}
		this.cancelling = false;
	}

	private void UpdateFetches(DictionaryPool<Tag, float, ComplexFabricator>.PooledDictionary missingAmounts)
	{
		ChoreType byHash = Db.Get().ChoreTypes.GetByHash(this.fetchChoreTypeIdHash);
		foreach (KeyValuePair<Tag, float> keyValuePair in missingAmounts)
		{
			if (keyValuePair.Value >= PICKUPABLETUNING.MINIMUM_PICKABLE_AMOUNT && !this.HasPendingFetch(keyValuePair.Key))
			{
				FetchList2 fetchList = new FetchList2(this.inStorage, byHash);
				fetchList.Add(keyValuePair.Key, null, null, keyValuePair.Value, FetchOrder2.OperationalRequirement.None);
				fetchList.ShowStatusItem = false;
				fetchList.Submit(new global::System.Action(this.OnFetchComplete), false);
				this.fetchListList.Add(fetchList);
			}
		}
	}

	private bool HasPendingFetch(Tag tag)
	{
		foreach (FetchList2 fetchList in this.fetchListList)
		{
			float num;
			fetchList.MinimumAmount.TryGetValue(tag, out num);
			if (num > 0f)
			{
				return true;
			}
		}
		return false;
	}

	private void CancelFetches()
	{
		foreach (FetchList2 fetchList in this.fetchListList)
		{
			fetchList.Cancel("cancel all orders");
		}
		this.fetchListList.Clear();
	}

	protected virtual void TransferCurrentRecipeIngredientsForBuild()
	{
		ComplexRecipe.RecipeElement[] ingredients = this.recipe_list[this.workingOrderIdx].ingredients;
		int i = 0;
		while (i < ingredients.Length)
		{
			ComplexRecipe.RecipeElement recipeElement = ingredients[i];
			float num;
			for (;;)
			{
				num = recipeElement.amount - this.buildStorage.GetAmountAvailable(recipeElement.material);
				if (num <= 0f)
				{
					break;
				}
				if (this.inStorage.GetAmountAvailable(recipeElement.material) <= 0f)
				{
					goto Block_2;
				}
				this.inStorage.Transfer(this.buildStorage, recipeElement.material, num, false, true);
			}
			IL_009D:
			i++;
			continue;
			Block_2:
			global::Debug.LogWarningFormat("TransferCurrentRecipeIngredientsForBuild ran out of {0} but still needed {1} more.", new object[] { recipeElement.material, num });
			goto IL_009D;
		}
	}

	protected virtual bool HasIngredients(ComplexRecipe recipe, Storage storage)
	{
		foreach (ComplexRecipe.RecipeElement recipeElement in recipe.ingredients)
		{
			float amountAvailable = storage.GetAmountAvailable(recipeElement.material);
			if (recipeElement.amount - amountAvailable >= PICKUPABLETUNING.MINIMUM_PICKABLE_AMOUNT)
			{
				return false;
			}
		}
		return true;
	}

	protected virtual List<GameObject> SpawnOrderProduct(ComplexRecipe recipe)
	{
		List<GameObject> list = new List<GameObject>();
		SimUtil.DiseaseInfo diseaseInfo;
		diseaseInfo.count = 0;
		diseaseInfo.idx = 0;
		float num = 0f;
		float num2 = 0f;
		foreach (ComplexRecipe.RecipeElement recipeElement in recipe.ingredients)
		{
			num2 += recipeElement.amount;
		}
		foreach (ComplexRecipe.RecipeElement recipeElement2 in recipe.ingredients)
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
		foreach (ComplexRecipe.RecipeElement recipeElement3 in recipe.results)
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
			if (resultState > ComplexFabricator.ResultState.Heated)
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
				GameObject gameObject2 = GameUtil.KInstantiate(Assets.GetPrefab(recipeElement3.material), Grid.SceneLayer.Ore, null, 0);
				int num6 = Grid.PosToCell(this);
				gameObject2.transform.SetPosition(Grid.CellToPosCCC(num6, Grid.SceneLayer.Ore) + this.outputOffset);
				PrimaryElement component2 = gameObject2.GetComponent<PrimaryElement>();
				component2.Units = recipeElement3.amount;
				component2.Temperature = ((this.resultState == ComplexFabricator.ResultState.PassTemperature) ? num : this.heatedTemperature);
				gameObject2.SetActive(true);
				float num7 = recipeElement3.amount / recipe.TotalResultUnits();
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
					KAnim.Build build = list[0].GetComponent<KBatchedAnimController>().AnimFiles[0].GetData().build;
					KAnim.Build.Symbol symbol = build.GetSymbol(build.name);
					if (symbol != null)
					{
						component3.TryRemoveSymbolOverride("output_tracker", 0);
						component3.AddSymbolOverride("output_tracker", symbol, 0);
					}
					else
					{
						global::Debug.LogWarning(component3.name + " is missing symbol " + build.name);
					}
				}
			}
		}
		return list;
	}

	public virtual List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		ComplexRecipe[] recipes = this.GetRecipes();
		if (recipes.Length != 0)
		{
			Descriptor descriptor = default(Descriptor);
			descriptor.SetupDescriptor(UI.BUILDINGEFFECTS.PROCESSES, UI.BUILDINGEFFECTS.TOOLTIPS.PROCESSES, Descriptor.DescriptorType.Effect);
			list.Add(descriptor);
		}
		foreach (ComplexRecipe complexRecipe in recipes)
		{
			string text = "";
			string uiname = complexRecipe.GetUIName(false);
			foreach (ComplexRecipe.RecipeElement recipeElement in complexRecipe.ingredients)
			{
				text = text + "• " + string.Format(UI.BUILDINGEFFECTS.PROCESSEDITEM, recipeElement.material.ProperName(), recipeElement.amount) + "\n";
			}
			Descriptor descriptor2 = new Descriptor(uiname, string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.FABRICATOR_INGREDIENTS, text), Descriptor.DescriptorType.Effect, false);
			descriptor2.IncreaseIndent();
			list.Add(descriptor2);
		}
		return list;
	}

	public virtual List<Descriptor> AdditionalEffectsForRecipe(ComplexRecipe recipe)
	{
		return new List<Descriptor>();
	}

	public string GetConversationTopic()
	{
		if (this.HasWorkingOrder)
		{
			ComplexRecipe complexRecipe = this.recipe_list[this.workingOrderIdx];
			if (complexRecipe != null)
			{
				return complexRecipe.results[0].material.Name;
			}
		}
		return null;
	}

	private const int MaxPrefetchCount = 2;

	public bool duplicantOperated = true;

	protected ComplexFabricatorWorkable workable;

	[SerializeField]
	public HashedString fetchChoreTypeIdHash = Db.Get().ChoreTypes.FabricateFetch.IdHash;

	[SerializeField]
	public ComplexFabricator.ResultState resultState;

	[SerializeField]
	public float heatedTemperature;

	[SerializeField]
	public bool storeProduced;

	public ComplexFabricatorSideScreen.StyleSetting sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;

	public bool labelByResult = true;

	public Vector3 outputOffset = Vector3.zero;

	public ChoreType choreType;

	public bool keepExcessLiquids;

	public TagBits keepAdditionalTags;

	public static int MAX_QUEUE_SIZE = 99;

	public static int QUEUE_INFINITE = -1;

	[Serialize]
	private Dictionary<string, int> recipeQueueCounts = new Dictionary<string, int>();

	private int nextOrderIdx;

	private bool nextOrderIsWorkable;

	private int workingOrderIdx = -1;

	[Serialize]
	private string lastWorkingRecipe;

	[Serialize]
	private float orderProgress;

	private List<int> openOrderCounts = new List<int>();

	private bool queueDirty = true;

	private bool hasOpenOrders;

	private List<FetchList2> fetchListList = new List<FetchList2>();

	private Chore chore;

	private bool cancelling;

	private ComplexRecipe[] recipe_list;

	private Dictionary<Tag, float> materialNeedCache = new Dictionary<Tag, float>();

	[SerializeField]
	public Storage inStorage;

	[SerializeField]
	public Storage buildStorage;

	[SerializeField]
	public Storage outStorage;

	[MyCmpAdd]
	private LoopingSounds loopingSounds;

	[MyCmpReq]
	protected Operational operational;

	[MyCmpAdd]
	private ComplexFabricatorSM fabricatorSM;

	private ProgressBar progressBar;

	private static readonly EventSystem.IntraObjectHandler<ComplexFabricator> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<ComplexFabricator>(delegate(ComplexFabricator component, object data)
	{
		component.OnStorageChange(data);
	});

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

	public enum ResultState
	{
		PassTemperature,
		Heated,
		Melted
	}
}
