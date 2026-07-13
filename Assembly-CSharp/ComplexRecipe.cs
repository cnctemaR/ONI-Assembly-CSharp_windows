using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class ComplexRecipe : IHasDlcRestrictions
{
	public void SetDLCRestrictions(string[] required, string[] forbidden)
	{
		this.requiredDlcIds = required;
		this.forbiddenDlcIds = forbidden;
	}

	public string[] GetRequiredDlcIds()
	{
		return this.requiredDlcIds;
	}

	public string[] GetForbiddenDlcIds()
	{
		return this.forbiddenDlcIds;
	}

	public bool ProductHasFacade { get; set; }

	public bool RequiresAllIngredientsDiscovered { get; set; }

	public Tag FirstResult
	{
		get
		{
			return this.results[0].material;
		}
	}

	private static GameObject CreateFabricationVisualizer(string anim, string nameRoot = null)
	{
		GameObject gameObject = new GameObject();
		if (nameRoot != null)
		{
			gameObject.name = nameRoot + "Visualizer";
		}
		gameObject.SetActive(false);
		gameObject.transform.SetLocalPosition(Vector3.zero);
		KBatchedAnimController kbatchedAnimController = gameObject.AddComponent<KBatchedAnimController>();
		kbatchedAnimController.AnimFiles = new KAnimFile[] { Assets.GetAnim(anim) };
		kbatchedAnimController.initialAnim = "fabricating";
		kbatchedAnimController.isMovable = true;
		KBatchedAnimTracker kbatchedAnimTracker = gameObject.AddComponent<KBatchedAnimTracker>();
		kbatchedAnimTracker.symbol = new HashedString("meter_ration");
		kbatchedAnimTracker.offset = Vector3.zero;
		global::UnityEngine.Object.DontDestroyOnLoad(gameObject);
		return gameObject;
	}

	public ComplexRecipe(string id, ComplexRecipe.RecipeElement[] ingredients, ComplexRecipe.RecipeElement[] results)
	{
		this.id = id;
		this.ingredients = ingredients;
		this.results = results;
		this.recipeCategoryID = ComplexRecipeManager.MakeRecipeCategoryID(id, "Default", results[0].material.ToString());
		if (!ComplexRecipeManager.Get().IsPostProcessing)
		{
			ComplexRecipeManager.Get().preProcessRecipes.Add(this);
		}
	}

	public ComplexRecipe(string id, ComplexRecipe.RecipeElement[] ingredients, ComplexRecipe.RecipeElement[] results, int consumedHEP, int producedHEP)
		: this(id, ingredients, results)
	{
		this.consumedHEP = consumedHEP;
		this.producedHEP = producedHEP;
	}

	public ComplexRecipe(string id, ComplexRecipe.RecipeElement[] ingredients, ComplexRecipe.RecipeElement[] results, int consumedHEP)
		: this(id, ingredients, results, consumedHEP, 0)
	{
	}

	public ComplexRecipe(string id, ComplexRecipe.RecipeElement[] ingredients, ComplexRecipe.RecipeElement[] results, string[] requiredDlcIds)
		: this(id, ingredients, results, requiredDlcIds, null)
	{
	}

	public ComplexRecipe(string id, ComplexRecipe.RecipeElement[] ingredients, ComplexRecipe.RecipeElement[] results, string[] requiredDlcIds, string[] forbiddenDlcIds)
		: this(id, ingredients, results)
	{
		this.requiredDlcIds = requiredDlcIds;
		this.forbiddenDlcIds = forbiddenDlcIds;
	}

	public ComplexRecipe(string id, ComplexRecipe.RecipeElement[] ingredients, ComplexRecipe.RecipeElement[] results, int consumedHEP, int producedHEP, string[] requiredDlcIds)
		: this(id, ingredients, results, consumedHEP, producedHEP, requiredDlcIds, null)
	{
	}

	public ComplexRecipe(string id, ComplexRecipe.RecipeElement[] ingredients, ComplexRecipe.RecipeElement[] results, int consumedHEP, int producedHEP, string[] requiredDlcIds, string[] forbiddenDlcIds)
		: this(id, ingredients, results, consumedHEP, producedHEP)
	{
		this.requiredDlcIds = requiredDlcIds;
		this.forbiddenDlcIds = forbiddenDlcIds;
	}

	public void SetFabricationAnim(string anim)
	{
		this.FabricationVisualizer = ComplexRecipe.CreateFabricationVisualizer(anim, this.id);
	}

	public float TotalResultUnits()
	{
		float num = 0f;
		foreach (ComplexRecipe.RecipeElement recipeElement in this.results)
		{
			num += recipeElement.amount;
		}
		return num;
	}

	public bool RequiresTechUnlock()
	{
		return !string.IsNullOrEmpty(this.requiredTech);
	}

	public bool IsRequiredTechUnlocked()
	{
		return string.IsNullOrEmpty(this.requiredTech) || Db.Get().Techs.Get(this.requiredTech).IsComplete();
	}

	public Sprite GetUIIcon()
	{
		Sprite sprite = null;
		Tag tag = ((this.nameDisplay == ComplexRecipe.RecipeNameDisplay.Ingredient) ? this.ingredients[0].material : this.results[0].material);
		if (this.nameDisplay == ComplexRecipe.RecipeNameDisplay.Custom && !string.IsNullOrEmpty(this.customSpritePrefabID))
		{
			tag = this.customSpritePrefabID;
		}
		KBatchedAnimController component = Assets.GetPrefab(tag).GetComponent<KBatchedAnimController>();
		if (component != null)
		{
			sprite = Def.GetUISpriteFromMultiObjectAnim(component.AnimFiles[0], "ui", false, "");
		}
		return sprite;
	}

	public Color GetUIColor()
	{
		return Color.white;
	}

	public string GetUIName(bool includeAmounts)
	{
		string text = (this.results[0].facadeID.IsNullOrWhiteSpace() ? this.results[0].material.ProperName() : this.results[0].facadeID.ProperName());
		switch (this.nameDisplay)
		{
		case ComplexRecipe.RecipeNameDisplay.Result:
			if (includeAmounts)
			{
				return string.Format(UI.UISIDESCREENS.REFINERYSIDESCREEN.RECIPE_SIMPLE_INCLUDE_AMOUNTS, text, this.results[0].amount);
			}
			return text;
		case ComplexRecipe.RecipeNameDisplay.IngredientToResult:
			if (includeAmounts)
			{
				return string.Format(UI.UISIDESCREENS.REFINERYSIDESCREEN.RECIPE_FROM_TO_INCLUDE_AMOUNTS, new object[]
				{
					this.ingredients[0].material.ProperName(),
					text,
					this.ingredients[0].amount,
					this.results[0].amount
				});
			}
			return string.Format(UI.UISIDESCREENS.REFINERYSIDESCREEN.RECIPE_FROM_TO, this.ingredients[0].material.ProperName(), text);
		case ComplexRecipe.RecipeNameDisplay.ResultWithIngredient:
			if (includeAmounts)
			{
				return string.Format(UI.UISIDESCREENS.REFINERYSIDESCREEN.RECIPE_WITH_INCLUDE_AMOUNTS, new object[]
				{
					this.ingredients[0].material.ProperName(),
					text,
					this.ingredients[0].amount,
					this.results[0].amount
				});
			}
			return string.Format(UI.UISIDESCREENS.REFINERYSIDESCREEN.RECIPE_WITH, this.ingredients[0].material.ProperName(), text);
		case ComplexRecipe.RecipeNameDisplay.Composite:
			if (includeAmounts)
			{
				return string.Format(UI.UISIDESCREENS.REFINERYSIDESCREEN.RECIPE_FROM_TO_COMPOSITE_INCLUDE_AMOUNTS, new object[]
				{
					this.ingredients[0].material.ProperName(),
					text,
					this.results[1].material.ProperName(),
					this.ingredients[0].amount,
					this.results[0].amount,
					this.results[1].amount
				});
			}
			return string.Format(UI.UISIDESCREENS.REFINERYSIDESCREEN.RECIPE_FROM_TO_COMPOSITE, this.ingredients[0].material.ProperName(), text, this.results[1].material.ProperName());
		case ComplexRecipe.RecipeNameDisplay.HEP:
			if (includeAmounts)
			{
				return string.Format(UI.UISIDESCREENS.REFINERYSIDESCREEN.RECIPE_FROM_TO_HEP_INCLUDE_AMOUNTS, new object[]
				{
					this.ingredients[0].material.ProperName(),
					this.results[1].material.ProperName(),
					this.ingredients[0].amount,
					this.producedHEP,
					this.results[1].amount
				});
			}
			return string.Format(UI.UISIDESCREENS.REFINERYSIDESCREEN.RECIPE_FROM_TO_HEP, this.ingredients[0].material.ProperName(), text);
		case ComplexRecipe.RecipeNameDisplay.Custom:
			return this.customName;
		}
		if (includeAmounts)
		{
			return string.Format(UI.UISIDESCREENS.REFINERYSIDESCREEN.RECIPE_SIMPLE_INCLUDE_AMOUNTS, this.ingredients[0].material.ProperName(), this.ingredients[0].amount);
		}
		return this.ingredients[0].material.ProperName();
	}

	public string id;

	public string recipeCategoryID;

	public ComplexRecipe.RecipeElement[] ingredients;

	public ComplexRecipe.RecipeElement[] results;

	public float time;

	public GameObject FabricationVisualizer;

	public int consumedHEP;

	public int producedHEP;

	private string[] requiredDlcIds;

	private string[] forbiddenDlcIds;

	public ComplexRecipe.RecipeNameDisplay nameDisplay;

	public string customName;

	public string customSpritePrefabID;

	public string description;

	public Func<string> runTimeDescription;

	public List<Tag> fabricators;

	public int sortOrder;

	public string requiredTech;

	public enum RecipeNameDisplay
	{
		Ingredient,
		Result,
		IngredientToResult,
		ResultWithIngredient,
		Composite,
		HEP,
		Custom
	}

	public class RecipeElement
	{
		public RecipeElement(Tag[] materialOptions, float amount)
		{
			this.material = null;
			this.possibleMaterials = materialOptions;
			this.amount = amount;
			this.temperatureOperation = ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature;
		}

		public RecipeElement(Tag[] materialOptions, float[] amounts)
		{
			this.material = null;
			this.possibleMaterials = materialOptions;
			this.possibleMaterialAmounts = amounts;
			this.temperatureOperation = ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature;
		}

		public RecipeElement(Tag[] materialOptions, float amount, ComplexRecipe.RecipeElement.TemperatureOperation temperatureOperation, string facadeID, bool storeElement = false, bool inheritElement = false)
		{
			this.material = null;
			this.possibleMaterials = materialOptions;
			this.amount = amount;
			this.temperatureOperation = temperatureOperation;
			this.storeElement = storeElement;
			this.facadeID = facadeID;
			this.inheritElement = inheritElement;
		}

		public RecipeElement(Tag[] materialOptions, float[] amounts, ComplexRecipe.RecipeElement.TemperatureOperation temperatureOperation, string facadeID, bool storeElement = false, bool inheritElement = false, bool doNotConsume = false)
		{
			this.material = null;
			this.possibleMaterials = materialOptions;
			this.possibleMaterialAmounts = amounts;
			this.amount = this.amount;
			this.temperatureOperation = temperatureOperation;
			this.storeElement = storeElement;
			this.facadeID = facadeID;
			this.inheritElement = inheritElement;
			this.doNotConsume = doNotConsume;
		}

		public RecipeElement(Tag material, float amount, bool inheritElement)
		{
			this.material = material;
			this.possibleMaterials = new Tag[] { material };
			this.amount = amount;
			this.temperatureOperation = ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature;
			this.inheritElement = inheritElement;
		}

		public RecipeElement(Tag material, float amount)
		{
			this.material = material;
			this.possibleMaterials = new Tag[] { material };
			this.amount = amount;
			this.temperatureOperation = ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature;
		}

		public RecipeElement(Tag material, float amount, ComplexRecipe.RecipeElement.TemperatureOperation temperatureOperation, bool storeElement = false)
		{
			this.material = material;
			this.possibleMaterials = new Tag[] { material };
			this.amount = amount;
			this.temperatureOperation = temperatureOperation;
			this.storeElement = storeElement;
		}

		public RecipeElement(Tag material, float amount, ComplexRecipe.RecipeElement.TemperatureOperation temperatureOperation, string facadeID, bool storeElement = false)
		{
			this.material = material;
			this.possibleMaterials = new Tag[] { material };
			this.amount = amount;
			this.temperatureOperation = temperatureOperation;
			this.storeElement = storeElement;
			this.facadeID = facadeID;
		}

		public RecipeElement(EdiblesManager.FoodInfo foodInfo, float amount, bool DoNotConsume = false)
		{
			this.material = foodInfo.Id;
			this.possibleMaterials = new Tag[] { this.material };
			this.amount = amount;
			this.doNotConsume = DoNotConsume;
		}

		public float amount { get; set; }

		public Tag material;

		public Tag[] possibleMaterials;

		public float[] possibleMaterialAmounts;

		public ComplexRecipe.RecipeElement.TemperatureOperation temperatureOperation;

		public bool storeElement;

		public bool inheritElement;

		public string facadeID;

		public bool doNotConsume;

		public struct IngredientDataSet
		{
			public IngredientDataSet(Tag[] substitutionOptions, float[] amounts)
			{
				this.substitutionOptions = substitutionOptions;
				this.amounts = amounts;
			}

			public Tag[] substitutionOptions;

			public float[] amounts;
		}

		public enum TemperatureOperation
		{
			AverageTemperature,
			Heated,
			Melted,
			Dehydrated
		}
	}
}
