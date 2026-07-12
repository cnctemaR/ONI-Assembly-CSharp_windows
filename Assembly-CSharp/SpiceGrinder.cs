using System;
using System.Collections.Generic;
using Database;
using Klei;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

public class SpiceGrinder : GameStateMachine<SpiceGrinder, SpiceGrinder.StatesInstance, IStateMachineTarget, SpiceGrinder.Def>
{
	public static void InitializeSpices()
	{
		Spices spices = Db.Get().Spices;
		SpiceGrinder.SettingOptions = new Dictionary<Tag, SpiceGrinder.Option>();
		for (int i = 0; i < spices.Count; i++)
		{
			Spice spice = spices[i];
			if (DlcManager.IsDlcListValidForCurrentContent(spice.DlcIds))
			{
				SpiceGrinder.SettingOptions.Add(spice.Id, new SpiceGrinder.Option(spice));
			}
		}
	}

	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.inoperational;
		this.root.Enter(new StateMachine<SpiceGrinder, SpiceGrinder.StatesInstance, IStateMachineTarget, SpiceGrinder.Def>.State.Callback(this.OnEnterRoot)).EventHandler(GameHashes.OnStorageChange, new GameStateMachine<SpiceGrinder, SpiceGrinder.StatesInstance, IStateMachineTarget, SpiceGrinder.Def>.GameEvent.Callback(this.OnStorageChanged));
		this.inoperational.EventTransition(GameHashes.OperationalChanged, this.ready, new StateMachine<SpiceGrinder, SpiceGrinder.StatesInstance, IStateMachineTarget, SpiceGrinder.Def>.Transition.ConditionCallback(this.IsOperational)).Enter(delegate(SpiceGrinder.StatesInstance smi)
		{
			smi.Play((smi.SelectedOption != null) ? "off" : "default", KAnim.PlayMode.Once);
			if (smi.SelectedOption == null)
			{
				smi.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.NoSpiceSelected, null);
			}
		}).Exit(delegate(SpiceGrinder.StatesInstance smi)
		{
			smi.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.NoSpiceSelected, false);
		});
		this.operational.EventTransition(GameHashes.OperationalChanged, this.inoperational, GameStateMachine<SpiceGrinder, SpiceGrinder.StatesInstance, IStateMachineTarget, SpiceGrinder.Def>.Not(new StateMachine<SpiceGrinder, SpiceGrinder.StatesInstance, IStateMachineTarget, SpiceGrinder.Def>.Transition.ConditionCallback(this.IsOperational))).ParamTransition<bool>(this.isReady, this.ready, GameStateMachine<SpiceGrinder, SpiceGrinder.StatesInstance, IStateMachineTarget, SpiceGrinder.Def>.IsTrue).Update(delegate(SpiceGrinder.StatesInstance smi, float dt)
		{
			if (smi.CurrentFood != null && !smi.HasOpenFetches)
			{
				bool flag = smi.CanSpice(smi.CurrentFood.Calories);
				this.isReady.Set(flag, smi, false);
			}
		}, UpdateRate.SIM_1000ms, false)
			.PlayAnim("on");
		this.ready.EventTransition(GameHashes.OperationalChanged, this.inoperational, GameStateMachine<SpiceGrinder, SpiceGrinder.StatesInstance, IStateMachineTarget, SpiceGrinder.Def>.Not(new StateMachine<SpiceGrinder, SpiceGrinder.StatesInstance, IStateMachineTarget, SpiceGrinder.Def>.Transition.ConditionCallback(this.IsOperational))).ParamTransition<bool>(this.isReady, this.operational, GameStateMachine<SpiceGrinder, SpiceGrinder.StatesInstance, IStateMachineTarget, SpiceGrinder.Def>.IsFalse).ToggleRecurringChore(new Func<SpiceGrinder.StatesInstance, Chore>(this.CreateChore), null);
	}

	private void OnEnterRoot(SpiceGrinder.StatesInstance smi)
	{
		smi.Initialize();
	}

	private bool IsOperational(SpiceGrinder.StatesInstance smi)
	{
		return smi.IsOperational;
	}

	private void OnStorageChanged(SpiceGrinder.StatesInstance smi, object data)
	{
		smi.UpdateMeter();
		smi.UpdateFoodSymbol();
		if (smi.SelectedOption == null)
		{
			return;
		}
		bool flag = smi.AvailableFood > 0f && smi.CanSpice(smi.CurrentFood.Calories);
		smi.sm.isReady.Set(flag, smi, false);
	}

	private Chore CreateChore(SpiceGrinder.StatesInstance smi)
	{
		return new WorkChore<SpiceGrinderWorkable>(Db.Get().ChoreTypes.Cook, smi.workable, null, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
	}

	public static Dictionary<Tag, SpiceGrinder.Option> SettingOptions = null;

	public static Operational.Flag spiceSet = new Operational.Flag("spiceSet", Operational.Flag.Type.Functional);

	public GameStateMachine<SpiceGrinder, SpiceGrinder.StatesInstance, IStateMachineTarget, SpiceGrinder.Def>.State inoperational;

	public GameStateMachine<SpiceGrinder, SpiceGrinder.StatesInstance, IStateMachineTarget, SpiceGrinder.Def>.State operational;

	public GameStateMachine<SpiceGrinder, SpiceGrinder.StatesInstance, IStateMachineTarget, SpiceGrinder.Def>.State ready;

	public StateMachine<SpiceGrinder, SpiceGrinder.StatesInstance, IStateMachineTarget, SpiceGrinder.Def>.BoolParameter isReady;

	public class Option : IConfigurableConsumerOption
	{
		public Effect StatBonus
		{
			get
			{
				if (this.statBonus == null)
				{
					return null;
				}
				if (string.IsNullOrEmpty(this.spiceDescription))
				{
					this.CreateDescription();
					this.GetName();
				}
				this.statBonus.Name = this.name;
				this.statBonus.description = this.spiceDescription;
				return this.statBonus;
			}
		}

		public Option(Spice spice)
		{
			this.Id = new Tag(spice.Id);
			this.Spice = spice;
			if (spice.StatBonus != null)
			{
				this.statBonus = new Effect(spice.Id, this.GetName(), this.spiceDescription, 600f, true, false, false, null, -1f, 0f, null, "");
				this.statBonus.Add(spice.StatBonus);
				Db.Get().effects.Add(this.statBonus);
			}
		}

		public Tag GetID()
		{
			return this.Spice.Id;
		}

		public string GetName()
		{
			if (string.IsNullOrEmpty(this.name))
			{
				string text = "STRINGS.ITEMS.SPICES." + this.Spice.Id.ToUpper() + ".NAME";
				StringEntry stringEntry;
				Strings.TryGet(text, out stringEntry);
				this.name = "MISSING " + text;
				if (stringEntry != null)
				{
					this.name = stringEntry;
				}
			}
			return this.name;
		}

		public string GetDetailedDescription()
		{
			if (string.IsNullOrEmpty(this.fullDescription))
			{
				this.CreateDescription();
			}
			return this.fullDescription;
		}

		public string GetDescription()
		{
			if (!string.IsNullOrEmpty(this.spiceDescription))
			{
				return this.spiceDescription;
			}
			string text = "STRINGS.ITEMS.SPICES." + this.Spice.Id.ToUpper() + ".DESC";
			StringEntry stringEntry;
			Strings.TryGet(text, out stringEntry);
			this.spiceDescription = "MISSING " + text;
			if (stringEntry != null)
			{
				this.spiceDescription = stringEntry.String;
			}
			return this.spiceDescription;
		}

		private void CreateDescription()
		{
			string text = "STRINGS.ITEMS.SPICES." + this.Spice.Id.ToUpper() + ".DESC";
			StringEntry stringEntry;
			Strings.TryGet(text, out stringEntry);
			this.spiceDescription = "MISSING " + text;
			if (stringEntry != null)
			{
				this.spiceDescription = stringEntry.String;
			}
			this.ingredientDescriptions = string.Format("\n\n<b>{0}</b>", BUILDINGS.PREFABS.SPICEGRINDER.INGREDIENTHEADER);
			for (int i = 0; i < this.Spice.Ingredients.Length; i++)
			{
				Spice.Ingredient ingredient = this.Spice.Ingredients[i];
				GameObject prefab = Assets.GetPrefab((ingredient.IngredientSet != null && ingredient.IngredientSet.Length != 0) ? ingredient.IngredientSet[0] : null);
				this.ingredientDescriptions += string.Format("\n{0}{1} {2}{3}", new object[]
				{
					"    • ",
					prefab.GetProperName(),
					ingredient.AmountKG,
					GameUtil.GetUnitTypeMassOrUnit(prefab)
				});
			}
			this.fullDescription = this.spiceDescription + this.ingredientDescriptions;
		}

		public Sprite GetIcon()
		{
			return Assets.GetSprite(this.Spice.Image);
		}

		public IConfigurableConsumerIngredient[] GetIngredients()
		{
			return this.Spice.Ingredients;
		}

		public readonly Tag Id;

		public readonly Spice Spice;

		private string name;

		private string fullDescription;

		private string spiceDescription;

		private string ingredientDescriptions;

		private Effect statBonus;
	}

	public class Def : StateMachine.BaseDef
	{
	}

	public class StatesInstance : GameStateMachine<SpiceGrinder, SpiceGrinder.StatesInstance, IStateMachineTarget, SpiceGrinder.Def>.GameInstance
	{
		public bool IsOperational
		{
			get
			{
				return this.operational != null && this.operational.IsOperational;
			}
		}

		public float AvailableFood
		{
			get
			{
				if (!(this.foodStorage == null))
				{
					return this.foodStorage.MassStored();
				}
				return 0f;
			}
		}

		public SpiceGrinder.Option SelectedOption
		{
			get
			{
				if (!(this.currentSpice.Id == Tag.Invalid))
				{
					return SpiceGrinder.SettingOptions[this.currentSpice.Id];
				}
				return null;
			}
		}

		public Edible CurrentFood
		{
			get
			{
				GameObject gameObject = this.foodStorage.FindFirst(GameTags.Edible);
				this.currentFood = ((gameObject != null) ? gameObject.GetComponent<Edible>() : null);
				return this.currentFood;
			}
		}

		public bool HasOpenFetches
		{
			get
			{
				return Array.Exists<FetchChore>(this.SpiceFetches, (FetchChore fetch) => fetch != null);
			}
		}

		public StatesInstance(IStateMachineTarget master, SpiceGrinder.Def def)
			: base(master, def)
		{
			this.workable.Grinder = this;
			Storage[] components = base.gameObject.GetComponents<Storage>();
			this.foodStorage = components[0];
			this.seedStorage = components[1];
			this.operational = base.GetComponent<Operational>();
			this.kbac = base.GetComponent<KBatchedAnimController>();
			this.foodStorageFilter = new FilteredStorage(base.GetComponent<KPrefabID>(), this.foodFilter, null, false, Db.Get().ChoreTypes.CookFetch);
			this.foodStorageFilter.SetHasMeter(false);
			this.meter = new MeterController(this.kbac, "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[] { "meter_frame", "meter_level" });
			this.SetupFoodSymbol();
			this.UpdateFoodSymbol();
			base.Subscribe(-905833192, new Action<object>(this.OnCopySettings));
			Prioritizable.AddRef(base.gameObject);
		}

		protected override void OnCleanUp()
		{
			base.OnCleanUp();
			Prioritizable.RemoveRef(base.gameObject);
		}

		public void Initialize()
		{
			SpiceGrinder.Option option;
			SpiceGrinder.SettingOptions.TryGetValue(new Tag(this.spiceHash), out option);
			this.OnOptionSelected(option);
			base.sm.OnStorageChanged(this, null);
			this.UpdateMeter();
		}

		private void OnCopySettings(object data)
		{
			SpiceGrinderWorkable component = ((GameObject)data).GetComponent<SpiceGrinderWorkable>();
			if (component != null)
			{
				this.currentSpice = component.Grinder.currentSpice;
				SpiceGrinder.Option option;
				SpiceGrinder.SettingOptions.TryGetValue(new Tag(component.Grinder.spiceHash), out option);
				this.OnOptionSelected(option);
			}
		}

		public void SetupFoodSymbol()
		{
			GameObject gameObject = new GameObject();
			gameObject.name = "foodSymbol";
			gameObject.SetActive(false);
			bool flag;
			Vector3 vector = this.kbac.GetSymbolTransform(SpiceGrinder.StatesInstance.HASH_FOOD, out flag).GetColumn(3);
			vector.z = Grid.GetLayerZ(Grid.SceneLayer.BuildingUse);
			gameObject.transform.SetPosition(vector);
			this.foodKBAC = gameObject.AddComponent<KBatchedAnimController>();
			this.foodKBAC.AnimFiles = new KAnimFile[] { Assets.GetAnim("mushbar_kanim") };
			this.foodKBAC.initialAnim = "object";
			this.kbac.SetSymbolVisiblity(SpiceGrinder.StatesInstance.HASH_FOOD, false);
		}

		public void UpdateFoodSymbol()
		{
			bool flag = this.AvailableFood > 0f && this.CurrentFood != null;
			this.foodKBAC.gameObject.SetActive(flag);
			if (flag)
			{
				this.foodKBAC.SwapAnims(this.CurrentFood.GetComponent<KBatchedAnimController>().AnimFiles);
				this.foodKBAC.Play("object", KAnim.PlayMode.Loop, 1f, 0f);
			}
		}

		public void UpdateMeter()
		{
			this.meter.SetPositionPercent(this.seedStorage.MassStored() / this.seedStorage.capacityKg);
		}

		public void SpiceFood()
		{
			float num = this.CurrentFood.Calories / 1000f;
			this.CurrentFood.SpiceEdible(this.currentSpice, SpiceGrinderConfig.SpicedStatus);
			this.foodStorage.Drop(this.CurrentFood.gameObject, true);
			this.currentFood = null;
			this.UpdateFoodSymbol();
			foreach (Spice.Ingredient ingredient in SpiceGrinder.SettingOptions[this.currentSpice.Id].Spice.Ingredients)
			{
				float num2 = num * ingredient.AmountKG / 1000f;
				int num3 = ingredient.IngredientSet.Length - 1;
				while (num2 > 0f && num3 >= 0)
				{
					Tag tag = ingredient.IngredientSet[num3];
					float num4;
					SimUtil.DiseaseInfo diseaseInfo;
					float num5;
					this.seedStorage.ConsumeAndGetDisease(tag, num2, out num4, out diseaseInfo, out num5);
					num2 -= num4;
					num3--;
				}
			}
			base.sm.isReady.Set(false, this, false);
		}

		public bool CanSpice(float kcalToSpice)
		{
			bool flag = true;
			float num = kcalToSpice / 1000f;
			Spice.Ingredient[] ingredients = SpiceGrinder.SettingOptions[this.currentSpice.Id].Spice.Ingredients;
			Dictionary<Tag, float> dictionary = new Dictionary<Tag, float>();
			for (int i = 0; i < ingredients.Length; i++)
			{
				Spice.Ingredient ingredient = ingredients[i];
				float num2 = 0f;
				int num3 = 0;
				while (ingredient.IngredientSet != null && num3 < ingredient.IngredientSet.Length)
				{
					num2 += this.seedStorage.GetMassAvailable(ingredient.IngredientSet[num3]);
					num3++;
				}
				float num4 = num * ingredient.AmountKG / 1000f;
				flag &= num4 <= num2;
				if (num4 > num2)
				{
					dictionary.Add(ingredient.IngredientSet[0], num4 - num2);
					if (this.SpiceFetches != null && this.SpiceFetches[i] == null)
					{
						this.SpiceFetches[i] = this.CreateFetchChore(ingredient.IngredientSet, ingredient.AmountKG * 10f);
					}
				}
			}
			this.UpdateSpiceIngredientStatus(flag, dictionary);
			return flag;
		}

		private FetchChore CreateFetchChore(Tag[] ingredientIngredientSet, float amount)
		{
			return this.CreateFetchChore(new HashSet<Tag>(ingredientIngredientSet), amount);
		}

		private FetchChore CreateFetchChore(HashSet<Tag> ingredients, float amount)
		{
			return new FetchChore(Db.Get().ChoreTypes.CookFetch, this.seedStorage, Mathf.Clamp(amount, 1f, this.seedStorage.Capacity() - this.seedStorage.MassStored()), ingredients, FetchChore.MatchCriteria.MatchID, Tag.Invalid, null, null, true, new Action<Chore>(this.ClearFetchChore), null, null, Operational.State.Operational, 0);
		}

		private void ClearFetchChore(Chore obj)
		{
			FetchChore fetchChore = obj as FetchChore;
			if (fetchChore == null || !fetchChore.isComplete || this.SpiceFetches == null)
			{
				return;
			}
			int i = this.SpiceFetches.Length - 1;
			while (i >= 0)
			{
				if (this.SpiceFetches[i] == fetchChore)
				{
					float num = fetchChore.originalAmount - fetchChore.amount;
					if (num > 0f)
					{
						this.SpiceFetches[i] = this.CreateFetchChore(fetchChore.tags, num);
						return;
					}
					this.SpiceFetches[i] = null;
					return;
				}
				else
				{
					i--;
				}
			}
		}

		private void UpdateSpiceIngredientStatus(bool can_spice, Dictionary<Tag, float> missing_spices)
		{
			KSelectable component = base.GetComponent<KSelectable>();
			if (can_spice)
			{
				this.missingResourceStatusItem = component.RemoveStatusItem(this.missingResourceStatusItem, false);
				return;
			}
			if (this.missingResourceStatusItem != Guid.Empty)
			{
				this.missingResourceStatusItem = component.ReplaceStatusItem(this.missingResourceStatusItem, Db.Get().BuildingStatusItems.MaterialsUnavailable, missing_spices);
				return;
			}
			this.missingResourceStatusItem = component.AddStatusItem(Db.Get().BuildingStatusItems.MaterialsUnavailable, missing_spices);
		}

		public void OnOptionSelected(SpiceGrinder.Option spiceOption)
		{
			base.smi.GetComponent<Operational>().SetFlag(SpiceGrinder.spiceSet, spiceOption != null);
			if (spiceOption == null)
			{
				this.kbac.Play("default", KAnim.PlayMode.Once, 1f, 0f);
				this.kbac.SetSymbolTint("stripe_anim2", Color.white);
			}
			else
			{
				this.kbac.Play(this.IsOperational ? "on" : "off", KAnim.PlayMode.Once, 1f, 0f);
			}
			if (this.SpiceFetches != null)
			{
				for (int i = 0; i < this.SpiceFetches.Length; i++)
				{
					if (this.SpiceFetches[i] != null)
					{
						this.SpiceFetches[i].Cancel("SpiceChanged");
					}
				}
				this.SpiceFetches = null;
			}
			if (this.currentSpice.Id != Tag.Invalid)
			{
				this.seedStorage.DropAll(false, false, default(Vector3), true, null);
				this.UpdateMeter();
				base.sm.isReady.Set(false, this, false);
			}
			if (this.missingResourceStatusItem != Guid.Empty)
			{
				this.missingResourceStatusItem = base.GetComponent<KSelectable>().RemoveStatusItem(this.missingResourceStatusItem, false);
			}
			if (spiceOption != null)
			{
				this.currentSpice = new SpiceInstance
				{
					Id = spiceOption.Id,
					TotalKG = spiceOption.Spice.TotalKG
				};
				this.SetSpiceSymbolColours(spiceOption.Spice);
				this.spiceHash = this.currentSpice.Id.GetHash();
				this.seedStorage.capacityKg = this.currentSpice.TotalKG * 10f;
				Spice.Ingredient[] ingredients = spiceOption.Spice.Ingredients;
				this.SpiceFetches = new FetchChore[ingredients.Length];
				Dictionary<Tag, float> dictionary = new Dictionary<Tag, float>();
				for (int j = 0; j < ingredients.Length; j++)
				{
					Spice.Ingredient ingredient = ingredients[j];
					float num = ((this.CurrentFood != null) ? (this.CurrentFood.Calories * ingredient.AmountKG / 1000000f) : 0f);
					if (this.seedStorage.GetMassAvailable(ingredient.IngredientSet[0]) < num)
					{
						this.SpiceFetches[j] = this.CreateFetchChore(ingredient.IngredientSet, ingredient.AmountKG * 10f);
					}
					if (this.CurrentFood != null)
					{
						dictionary.Add(ingredient.IngredientSet[0], num);
					}
				}
				if (this.CurrentFood != null)
				{
					this.UpdateSpiceIngredientStatus(false, dictionary);
				}
				this.foodFilter[0] = this.currentSpice.Id;
				this.foodStorageFilter.FilterChanged();
			}
		}

		private void SetSpiceSymbolColours(Spice spice)
		{
			this.kbac.SetSymbolTint("stripe_anim2", spice.PrimaryColor);
			this.kbac.SetSymbolTint("stripe_anim1", spice.SecondaryColor);
			this.kbac.SetSymbolTint("grinder", spice.PrimaryColor);
		}

		private static string HASH_FOOD = "food";

		private KBatchedAnimController kbac;

		private KBatchedAnimController foodKBAC;

		[MyCmpReq]
		public SpiceGrinderWorkable workable;

		[Serialize]
		private int spiceHash;

		private SpiceInstance currentSpice;

		private Edible currentFood;

		private Storage seedStorage;

		private Storage foodStorage;

		private MeterController meter;

		private Tag[] foodFilter = new Tag[1];

		private FilteredStorage foodStorageFilter;

		private Operational operational;

		private Guid missingResourceStatusItem = Guid.Empty;

		private FetchChore[] SpiceFetches;
	}
}
