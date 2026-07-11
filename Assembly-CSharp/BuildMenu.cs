using System;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.UI;

public class BuildMenu : KScreen
{
	public override float GetSortKey()
	{
		return 6f;
	}

	public static BuildMenu Instance { get; private set; }

	public static void DestroyInstance()
	{
		BuildMenu.Instance = null;
	}

	public static bool UseHotkeyBuildMenu()
	{
		int @int = KPlayerPrefs.GetInt("ENABLE_HOTKEY_BUILD_MENU");
		return @int != 0;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.ConsumeMouseScroll = true;
		this.initTime = KTime.Instance.UnscaledGameTime;
		bool flag = BuildMenu.UseHotkeyBuildMenu();
		if (flag)
		{
			BuildMenu.Instance = this;
			this.productInfoScreen = global::Util.KInstantiateUI<ProductInfoScreen>(this.productInfoScreenPrefab, base.gameObject, true);
			this.productInfoScreen.rectTransform().pivot = new Vector2(0f, 0f);
			this.productInfoScreen.onElementsFullySelected = new global::System.Action(this.OnRecipeElementsFullySelected);
			this.productInfoScreen.Show(false);
			this.buildingsScreen = global::Util.KInstantiateUI<BuildMenuBuildingsScreen>(this.buildingsMenuPrefab.gameObject, base.gameObject, true);
			BuildMenuBuildingsScreen buildMenuBuildingsScreen = this.buildingsScreen;
			buildMenuBuildingsScreen.onBuildingSelected = (Action<BuildingDef>)Delegate.Combine(buildMenuBuildingsScreen.onBuildingSelected, new Action<BuildingDef>(this.OnBuildingSelected));
			this.buildingsScreen.Show(false);
			Game.Instance.Subscribe(288942073, new Action<object>(this.OnUIClear));
			Game.Instance.Subscribe(-1190690038, new Action<object>(this.OnBuildToolDeactivated));
			this.Initialize();
			this.rectTransform().anchoredPosition = Vector2.zero;
		}
		else
		{
			base.gameObject.SetActive(flag);
		}
	}

	private void Initialize()
	{
		foreach (KeyValuePair<BuildMenu.Category, BuildMenuCategoriesScreen> keyValuePair in this.submenus)
		{
			BuildMenuCategoriesScreen value = keyValuePair.Value;
			value.Close();
			global::UnityEngine.Object.DestroyImmediate(value.gameObject);
		}
		this.submenuStack.Clear();
		this.tagCategoryMap = new Dictionary<Tag, BuildMenu.Category>();
		this.tagOrderMap = new Dictionary<Tag, int>();
		this.categorizedBuildingMap = new Dictionary<BuildMenu.Category, List<BuildingDef>>();
		this.categorizedCategoryMap = new Dictionary<BuildMenu.Category, List<BuildMenu.Category>>();
		int num = 0;
		BuildMenu.DisplayInfo orderedBuildings = BuildMenu.OrderedBuildings;
		this.PopulateCategorizedMaps(orderedBuildings.category, 0, orderedBuildings.data, this.tagCategoryMap, this.tagOrderMap, ref num, this.categorizedBuildingMap, this.categorizedCategoryMap);
		BuildMenuCategoriesScreen buildMenuCategoriesScreen = this.submenus[BuildMenu.Category.ROOT];
		buildMenuCategoriesScreen.Show(true);
		buildMenuCategoriesScreen.modalKeyInputBehaviour = false;
		foreach (KeyValuePair<BuildMenu.Category, BuildMenuCategoriesScreen> keyValuePair2 in this.submenus)
		{
			BuildMenu.Category key = keyValuePair2.Key;
			if (key != BuildMenu.Category.ROOT)
			{
				List<BuildMenu.Category> list;
				if (this.categorizedCategoryMap.TryGetValue(key, out list))
				{
					BuildMenuCategoriesScreen value2 = keyValuePair2.Value;
					Image component = value2.GetComponent<Image>();
					if (component != null)
					{
						component.enabled = list.Count > 0;
					}
				}
			}
		}
		this.PositionMenus();
	}

	[ContextMenu("PositionMenus")]
	private void PositionMenus()
	{
		foreach (KeyValuePair<BuildMenu.Category, BuildMenuCategoriesScreen> keyValuePair in this.submenus)
		{
			BuildMenu.Category key = keyValuePair.Key;
			BuildMenuCategoriesScreen value = keyValuePair.Value;
			LayoutGroup component = value.GetComponent<LayoutGroup>();
			Vector2 vector;
			BuildMenu.PadInfo padInfo;
			if (key == BuildMenu.Category.ROOT)
			{
				vector = this.rootMenuOffset;
				padInfo = this.rootMenuPadding;
				Image component2 = value.GetComponent<Image>();
				component2.enabled = false;
			}
			else
			{
				vector = this.nestedMenuOffset;
				padInfo = this.nestedMenuPadding;
			}
			value.rectTransform().anchoredPosition = vector;
			component.padding.left = padInfo.left;
			component.padding.right = padInfo.right;
			component.padding.top = padInfo.top;
			component.padding.bottom = padInfo.bottom;
		}
		this.buildingsScreen.rectTransform().anchoredPosition = this.buildingsMenuOffset;
	}

	public void Refresh()
	{
		foreach (KeyValuePair<BuildMenu.Category, BuildMenuCategoriesScreen> keyValuePair in this.submenus)
		{
			BuildMenuCategoriesScreen value = keyValuePair.Value;
			value.UpdateBuildableStates(true);
		}
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		Game.Instance.Subscribe(-107300940, new Action<object>(this.OnResearchComplete));
	}

	protected override void OnCmpDisable()
	{
		Game.Instance.Unsubscribe(-107300940, new Action<object>(this.OnResearchComplete));
		base.OnCmpDisable();
	}

	private BuildMenuCategoriesScreen CreateCategorySubMenu(BuildMenu.Category category, int depth, object data, Dictionary<BuildMenu.Category, List<BuildingDef>> categorized_building_map, Dictionary<BuildMenu.Category, List<BuildMenu.Category>> categorized_category_map, Dictionary<Tag, BuildMenu.Category> tag_category_map, BuildMenuBuildingsScreen buildings_screen)
	{
		BuildMenuCategoriesScreen buildMenuCategoriesScreen = global::Util.KInstantiateUI<BuildMenuCategoriesScreen>(this.categoriesMenuPrefab.gameObject, base.gameObject, true);
		buildMenuCategoriesScreen.Show(false);
		buildMenuCategoriesScreen.Configure(category, depth, data, this.categorizedBuildingMap, this.categorizedCategoryMap, this.buildingsScreen);
		BuildMenuCategoriesScreen buildMenuCategoriesScreen2 = buildMenuCategoriesScreen;
		buildMenuCategoriesScreen2.onCategoryClicked = (Action<BuildMenu.Category, int>)Delegate.Combine(buildMenuCategoriesScreen2.onCategoryClicked, new Action<BuildMenu.Category, int>(this.OnCategoryClicked));
		buildMenuCategoriesScreen.name = "BuildMenu_" + category.ToString();
		return buildMenuCategoriesScreen;
	}

	private void PopulateCategorizedMaps(BuildMenu.Category category, int depth, object data, Dictionary<Tag, BuildMenu.Category> category_map, Dictionary<Tag, int> order_map, ref int building_index, Dictionary<BuildMenu.Category, List<BuildingDef>> categorized_building_map, Dictionary<BuildMenu.Category, List<BuildMenu.Category>> categorized_category_map)
	{
		Type type = data.GetType();
		if (type == typeof(BuildMenu.DisplayInfo))
		{
			BuildMenu.DisplayInfo displayInfo = (BuildMenu.DisplayInfo)data;
			List<BuildMenu.Category> list;
			if (!categorized_category_map.TryGetValue(category, out list))
			{
				list = new List<BuildMenu.Category>();
				categorized_category_map[category] = list;
			}
			list.Add(displayInfo.category);
			this.PopulateCategorizedMaps(displayInfo.category, depth + 1, displayInfo.data, category_map, order_map, ref building_index, categorized_building_map, categorized_category_map);
		}
		else if (type == typeof(BuildMenu.DisplayInfo[]))
		{
			BuildMenu.DisplayInfo[] array = (BuildMenu.DisplayInfo[])data;
			List<BuildMenu.Category> list2;
			if (!categorized_category_map.TryGetValue(category, out list2))
			{
				list2 = new List<BuildMenu.Category>();
				categorized_category_map[category] = list2;
			}
			foreach (BuildMenu.DisplayInfo displayInfo2 in array)
			{
				list2.Add(displayInfo2.category);
				this.PopulateCategorizedMaps(displayInfo2.category, depth + 1, displayInfo2.data, category_map, order_map, ref building_index, categorized_building_map, categorized_category_map);
			}
		}
		else
		{
			BuildMenu.BuildingInfo[] array3 = (BuildMenu.BuildingInfo[])data;
			foreach (BuildMenu.BuildingInfo buildingInfo in array3)
			{
				Tag tag = new Tag(buildingInfo.id);
				category_map[tag] = category;
				order_map[tag] = building_index;
				building_index++;
				List<BuildingDef> list3;
				if (!categorized_building_map.TryGetValue(category, out list3))
				{
					list3 = new List<BuildingDef>();
					categorized_building_map[category] = list3;
				}
				BuildingDef buildingDef = Assets.GetBuildingDef(buildingInfo.id);
				buildingDef.HotKey = buildingInfo.hotkey;
				list3.Add(buildingDef);
			}
		}
		this.submenus[category] = this.CreateCategorySubMenu(category, depth, data, this.categorizedBuildingMap, this.categorizedCategoryMap, this.tagCategoryMap, this.buildingsScreen);
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.Consumed)
		{
			return;
		}
		if (!this.mouseOver || !this.ConsumeMouseScroll || e.TryConsume(global::Action.ZoomIn) || e.TryConsume(global::Action.ZoomOut))
		{
		}
		if (!e.Consumed && this.selectedCategory != BuildMenu.Category.INVALID && e.TryConsume(global::Action.Escape))
		{
			this.OnUIClear(null);
		}
		else if (!e.Consumed)
		{
			base.OnKeyDown(e);
		}
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		if (this.selectedCategory != BuildMenu.Category.INVALID && PlayerController.Instance.ConsumeIfNotDragging(e, global::Action.MouseRight))
		{
			this.OnUIClear(null);
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Deselect", false));
		}
		if (!e.Consumed)
		{
			base.OnKeyUp(e);
		}
	}

	private void OnUIClear(object data)
	{
		SelectTool.Instance.Activate();
		PlayerController.Instance.ActivateTool(SelectTool.Instance);
		SelectTool.Instance.Select(null, true);
		this.productInfoScreen.materialSelectionPanel.PriorityScreen.ResetPriority();
		this.CloseMenus();
	}

	private void OnBuildToolDeactivated(object data)
	{
		this.CloseMenus();
		this.productInfoScreen.materialSelectionPanel.PriorityScreen.ResetPriority();
	}

	private void CloseMenus()
	{
		this.productInfoScreen.Close();
		while (this.submenuStack.Count > 0)
		{
			KIconToggleMenu kiconToggleMenu = this.submenuStack.Pop();
			kiconToggleMenu.Close();
			this.productInfoScreen.Close();
		}
		this.selectedCategory = BuildMenu.Category.INVALID;
		this.submenus[BuildMenu.Category.ROOT].ClearSelection();
	}

	public override void ScreenUpdate(bool topLevel)
	{
		base.ScreenUpdate(topLevel);
		if (this.timeSinceNotificationPing < 8f)
		{
			this.timeSinceNotificationPing += Time.unscaledDeltaTime;
		}
		if (this.timeSinceNotificationPing >= 0.5f)
		{
			this.notificationPingCount = 0;
		}
	}

	public void PlayNewBuildingSounds()
	{
		if (KTime.Instance.UnscaledGameTime - this.initTime > 1.5f)
		{
			if (BuildMenu.Instance.timeSinceNotificationPing >= 8f)
			{
				string sound = GlobalAssets.GetSound("NewBuildable_Embellishment", false);
				if (sound != null)
				{
					EventInstance eventInstance = SoundEvent.BeginOneShot(sound, SoundListenerController.Instance.transform.GetPosition());
					SoundEvent.EndOneShot(eventInstance);
				}
			}
			string sound2 = GlobalAssets.GetSound("NewBuildable", false);
			if (sound2 != null)
			{
				EventInstance eventInstance2 = SoundEvent.BeginOneShot(sound2, SoundListenerController.Instance.transform.GetPosition());
				eventInstance2.setParameterValue("playCount", (float)BuildMenu.Instance.notificationPingCount);
				SoundEvent.EndOneShot(eventInstance2);
			}
		}
		this.timeSinceNotificationPing = 0f;
		this.notificationPingCount++;
	}

	public PlanScreen.RequirementsState BuildableState(BuildingDef def)
	{
		PlanScreen.RequirementsState requirementsState = PlanScreen.RequirementsState.Complete;
		if (!DebugHandler.InstantBuildMode && !Game.Instance.SandboxModeActive)
		{
			if (!Db.Get().TechItems.IsTechItemComplete(def.PrefabID))
			{
				requirementsState = PlanScreen.RequirementsState.Tech;
			}
			else if (!ProductInfoScreen.MaterialsMet(def.CraftRecipe))
			{
				requirementsState = PlanScreen.RequirementsState.Materials;
			}
		}
		return requirementsState;
	}

	private void CloseProductInfoScreen()
	{
		this.productInfoScreen.ClearProduct(true);
		this.productInfoScreen.Show(false);
	}

	private void Update()
	{
		this.elapsedTime += Time.unscaledDeltaTime;
		if (this.elapsedTime <= this.updateInterval)
		{
			return;
		}
		this.elapsedTime = 0f;
		if (this.productInfoScreen.gameObject.activeSelf)
		{
			this.productInfoScreen.materialSelectionPanel.UpdateResourceToggleValues();
		}
		foreach (KIconToggleMenu kiconToggleMenu in this.submenuStack)
		{
			if (kiconToggleMenu is BuildMenuCategoriesScreen)
			{
				(kiconToggleMenu as BuildMenuCategoriesScreen).UpdateBuildableStates(false);
			}
		}
		this.submenus[BuildMenu.Category.ROOT].UpdateBuildableStates(false);
	}

	private void OnRecipeElementsFullySelected()
	{
		if (this.selectedBuilding == null)
		{
			global::Debug.Log("No def!", null);
		}
		if (this.selectedBuilding.isKAnimTile && this.selectedBuilding.isUtility)
		{
			IList<Element> getSelectedElementAsList = this.productInfoScreen.materialSelectionPanel.GetSelectedElementAsList;
			bool flag = this.selectedBuilding.BuildingComplete.GetComponent<Wire>() != null;
			BaseUtilityBuildTool baseUtilityBuildTool = ((!flag) ? UtilityBuildTool.Instance : WireBuildTool.Instance);
			baseUtilityBuildTool.Activate(this.selectedBuilding, getSelectedElementAsList);
		}
		else
		{
			BuildTool.Instance.Activate(this.selectedBuilding, this.productInfoScreen.materialSelectionPanel.GetSelectedElementAsList, null);
		}
	}

	private void OnBuildingSelected(BuildingDef def)
	{
		if (this.selecting)
		{
			return;
		}
		this.selecting = true;
		this.selectedBuilding = def;
		this.buildingsScreen.SetHasFocus(false);
		foreach (KeyValuePair<BuildMenu.Category, BuildMenuCategoriesScreen> keyValuePair in this.submenus)
		{
			BuildMenuCategoriesScreen value = keyValuePair.Value;
			value.SetHasFocus(false);
		}
		ToolMenu.Instance.ClearSelection();
		if (def != null)
		{
			Vector2 anchoredPosition = this.productInfoScreen.rectTransform().anchoredPosition;
			RectTransform rectTransform = this.buildingsScreen.rectTransform();
			anchoredPosition.y = rectTransform.anchoredPosition.y;
			anchoredPosition.x = rectTransform.anchoredPosition.x + rectTransform.sizeDelta.x + 10f;
			this.productInfoScreen.rectTransform().anchoredPosition = anchoredPosition;
			this.productInfoScreen.ClearProduct(false);
			this.productInfoScreen.Show(true);
			this.productInfoScreen.ConfigureScreen(def);
		}
		else
		{
			this.productInfoScreen.Close();
		}
		this.selecting = false;
	}

	private void OnCategoryClicked(BuildMenu.Category new_category, int depth)
	{
		while (this.submenuStack.Count > depth)
		{
			KIconToggleMenu kiconToggleMenu = this.submenuStack.Pop();
			kiconToggleMenu.ClearSelection();
			kiconToggleMenu.Close();
		}
		this.productInfoScreen.Close();
		if (new_category != this.selectedCategory && new_category != BuildMenu.Category.INVALID)
		{
			foreach (KIconToggleMenu kiconToggleMenu2 in this.submenuStack)
			{
				if (kiconToggleMenu2 is BuildMenuCategoriesScreen)
				{
					BuildMenuCategoriesScreen buildMenuCategoriesScreen = kiconToggleMenu2 as BuildMenuCategoriesScreen;
					buildMenuCategoriesScreen.SetHasFocus(false);
				}
			}
			this.selectedCategory = new_category;
			BuildMenuCategoriesScreen buildMenuCategoriesScreen2;
			this.submenus.TryGetValue(new_category, out buildMenuCategoriesScreen2);
			if (buildMenuCategoriesScreen2 != null)
			{
				buildMenuCategoriesScreen2.Show(true);
				buildMenuCategoriesScreen2.SetHasFocus(true);
				this.submenuStack.Push(buildMenuCategoriesScreen2);
			}
		}
		else
		{
			this.selectedCategory = BuildMenu.Category.INVALID;
		}
		foreach (KIconToggleMenu kiconToggleMenu3 in this.submenuStack)
		{
			if (kiconToggleMenu3 is BuildMenuCategoriesScreen)
			{
				(kiconToggleMenu3 as BuildMenuCategoriesScreen).UpdateBuildableStates(true);
			}
		}
		this.submenus[BuildMenu.Category.ROOT].UpdateBuildableStates(true);
	}

	public void RefreshProductInfoScreen(BuildingDef def)
	{
		if (this.productInfoScreen.currentDef == def)
		{
			this.productInfoScreen.ClearProduct(false);
			this.productInfoScreen.Show(true);
			this.productInfoScreen.ConfigureScreen(def);
		}
	}

	private BuildMenu.Category GetParentCategory(BuildMenu.Category desired_category)
	{
		foreach (KeyValuePair<BuildMenu.Category, List<BuildMenu.Category>> keyValuePair in this.categorizedCategoryMap)
		{
			foreach (BuildMenu.Category category in keyValuePair.Value)
			{
				if (category == desired_category)
				{
					return keyValuePair.Key;
				}
			}
		}
		return BuildMenu.Category.INVALID;
	}

	private void AddParentCategories(BuildMenu.Category child_category, ICollection<BuildMenu.Category> categories)
	{
		for (;;)
		{
			BuildMenu.Category parentCategory = this.GetParentCategory(child_category);
			if (parentCategory == BuildMenu.Category.INVALID)
			{
				break;
			}
			categories.Add(parentCategory);
			child_category = parentCategory;
		}
	}

	private void OnResearchComplete(object data)
	{
		HashSet<BuildMenu.Category> hashSet = new HashSet<BuildMenu.Category>();
		Tech tech = (Tech)data;
		foreach (TechItem techItem in tech.unlockedItems)
		{
			BuildingDef buildingDef = Assets.GetBuildingDef(techItem.Id);
			if (buildingDef == null)
			{
				Output.LogWarning(new object[] { string.Format("Tech '{0}' unlocked building '{1}' but no such building exists", tech.Name, techItem.Id) });
			}
			else
			{
				BuildMenu.Category category = this.tagCategoryMap[buildingDef.Tag];
				hashSet.Add(category);
				this.AddParentCategories(category, hashSet);
			}
		}
		this.UpdateNotifications(hashSet, BuildMenu.OrderedBuildings);
	}

	private void UpdateNotifications(ICollection<BuildMenu.Category> updated_categories, object data)
	{
		foreach (KeyValuePair<BuildMenu.Category, BuildMenuCategoriesScreen> keyValuePair in this.submenus)
		{
			BuildMenuCategoriesScreen value = keyValuePair.Value;
			value.UpdateNotifications(updated_categories);
		}
	}

	public PrioritySetting GetBuildingPriority()
	{
		return this.productInfoScreen.materialSelectionPanel.PriorityScreen.GetLastSelectedPriority();
	}

	public const string ENABLE_HOTKEY_BUILD_MENU_KEY = "ENABLE_HOTKEY_BUILD_MENU";

	[SerializeField]
	private BuildMenuCategoriesScreen categoriesMenuPrefab;

	[SerializeField]
	private BuildMenuBuildingsScreen buildingsMenuPrefab;

	[SerializeField]
	private GameObject productInfoScreenPrefab;

	private ProductInfoScreen productInfoScreen;

	private BuildMenuBuildingsScreen buildingsScreen;

	private BuildingDef selectedBuilding;

	private BuildMenu.Category selectedCategory = BuildMenu.Category.INVALID;

	private Dictionary<BuildMenu.Category, BuildMenuCategoriesScreen> submenus = new Dictionary<BuildMenu.Category, BuildMenuCategoriesScreen>();

	private Stack<KIconToggleMenu> submenuStack = new Stack<KIconToggleMenu>();

	private bool selecting;

	[SerializeField]
	private Vector2 rootMenuOffset = Vector2.zero;

	[SerializeField]
	private BuildMenu.PadInfo rootMenuPadding = default(BuildMenu.PadInfo);

	[SerializeField]
	private Vector2 nestedMenuOffset = Vector2.zero;

	[SerializeField]
	private BuildMenu.PadInfo nestedMenuPadding = default(BuildMenu.PadInfo);

	[SerializeField]
	private Vector2 buildingsMenuOffset = Vector2.zero;

	public static readonly BuildMenu.DisplayInfo OrderedBuildings = new BuildMenu.DisplayInfo(BuildMenu.Category.ROOT, "icon_category_base", global::Action.NumActions, KKeyCode.None, new BuildMenu.DisplayInfo[]
	{
		new BuildMenu.DisplayInfo(BuildMenu.Category.Base, "icon_category_base", global::Action.Plan1, KKeyCode.None, new BuildMenu.DisplayInfo[]
		{
			new BuildMenu.DisplayInfo(BuildMenu.Category.Tiles, "icon_category_base", global::Action.BuildCategoryTiles, KKeyCode.T, new BuildMenu.BuildingInfo[]
			{
				new BuildMenu.BuildingInfo("Tile", global::Action.BuildMenuKeyT),
				new BuildMenu.BuildingInfo("GasPermeableMembrane", global::Action.BuildMenuKeyA),
				new BuildMenu.BuildingInfo("MeshTile", global::Action.BuildMenuKeyE),
				new BuildMenu.BuildingInfo("InsulationTile", global::Action.BuildMenuKeyD),
				new BuildMenu.BuildingInfo("PlasticTile", global::Action.BuildMenuKeyC),
				new BuildMenu.BuildingInfo("MetalTile", global::Action.BuildMenuKeyX),
				new BuildMenu.BuildingInfo("GlassTile", global::Action.BuildMenuKeyG),
				new BuildMenu.BuildingInfo("BunkerTile", global::Action.BuildMenuKeyB)
			}),
			new BuildMenu.DisplayInfo(BuildMenu.Category.Ladders, "icon_category_base", global::Action.BuildCategoryLadders, KKeyCode.A, new BuildMenu.BuildingInfo[]
			{
				new BuildMenu.BuildingInfo("Ladder", global::Action.BuildMenuKeyA),
				new BuildMenu.BuildingInfo("LadderFast", global::Action.BuildMenuKeyC),
				new BuildMenu.BuildingInfo("FirePole", global::Action.BuildMenuKeyF)
			}),
			new BuildMenu.DisplayInfo(BuildMenu.Category.Doors, "icon_category_base", global::Action.BuildCategoryDoors, KKeyCode.D, new BuildMenu.BuildingInfo[]
			{
				new BuildMenu.BuildingInfo("Door", global::Action.BuildMenuKeyD),
				new BuildMenu.BuildingInfo("ManualPressureDoor", global::Action.BuildMenuKeyA),
				new BuildMenu.BuildingInfo("PressureDoor", global::Action.BuildMenuKeyE),
				new BuildMenu.BuildingInfo("BunkerDoor", global::Action.BuildMenuKeyB)
			}),
			new BuildMenu.DisplayInfo(BuildMenu.Category.Storage, "icon_category_base", global::Action.BuildCategoryStorage, KKeyCode.S, new BuildMenu.BuildingInfo[]
			{
				new BuildMenu.BuildingInfo("StorageLocker", global::Action.BuildMenuKeyS),
				new BuildMenu.BuildingInfo("RationBox", global::Action.BuildMenuKeyR),
				new BuildMenu.BuildingInfo("Refrigerator", global::Action.BuildMenuKeyF),
				new BuildMenu.BuildingInfo("StorageLockerSmart", global::Action.BuildMenuKeyA)
			}),
			new BuildMenu.DisplayInfo(BuildMenu.Category.Research, "icon_category_misc", global::Action.BuildCategoryResearch, KKeyCode.R, new BuildMenu.BuildingInfo[]
			{
				new BuildMenu.BuildingInfo("ResearchCenter", global::Action.BuildMenuKeyR),
				new BuildMenu.BuildingInfo("AdvancedResearchCenter", global::Action.BuildMenuKeyS)
			})
		}),
		new BuildMenu.DisplayInfo(BuildMenu.Category.FoodAndAgriculture, "icon_category_food", global::Action.Plan2, KKeyCode.None, new BuildMenu.DisplayInfo[]
		{
			new BuildMenu.DisplayInfo(BuildMenu.Category.Farming, "icon_category_food", global::Action.BuildCategoryFarming, KKeyCode.F, new BuildMenu.BuildingInfo[]
			{
				new BuildMenu.BuildingInfo("PlanterBox", global::Action.BuildMenuKeyB),
				new BuildMenu.BuildingInfo("FarmTile", global::Action.BuildMenuKeyF),
				new BuildMenu.BuildingInfo("HydroponicFarm", global::Action.BuildMenuKeyD),
				new BuildMenu.BuildingInfo("Compost", global::Action.BuildMenuKeyC),
				new BuildMenu.BuildingInfo("FertilizerMaker", global::Action.BuildMenuKeyR)
			}),
			new BuildMenu.DisplayInfo(BuildMenu.Category.Cooking, "icon_category_food", global::Action.BuildCategoryCooking, KKeyCode.C, new BuildMenu.BuildingInfo[]
			{
				new BuildMenu.BuildingInfo("MicrobeMusher", global::Action.BuildMenuKeyC),
				new BuildMenu.BuildingInfo("CookingStation", global::Action.BuildMenuKeyG),
				new BuildMenu.BuildingInfo("EggCracker", global::Action.BuildMenuKeyE)
			}),
			new BuildMenu.DisplayInfo(BuildMenu.Category.Ranching, "icon_category_food", global::Action.BuildCategoryRanching, KKeyCode.R, new BuildMenu.BuildingInfo[]
			{
				new BuildMenu.BuildingInfo("CreatureDeliveryPoint", global::Action.BuildMenuKeyD),
				new BuildMenu.BuildingInfo("FishDeliveryPoint", global::Action.BuildMenuKeyG),
				new BuildMenu.BuildingInfo("CreatureFeeder", global::Action.BuildMenuKeyF),
				new BuildMenu.BuildingInfo("FishFeeder", global::Action.BuildMenuKeyE),
				new BuildMenu.BuildingInfo("RanchStation", global::Action.BuildMenuKeyR),
				new BuildMenu.BuildingInfo("ShearingStation", global::Action.BuildMenuKeyS),
				new BuildMenu.BuildingInfo("EggIncubator", global::Action.BuildMenuKeyI),
				new BuildMenu.BuildingInfo("CreatureTrap", global::Action.BuildMenuKeyT),
				new BuildMenu.BuildingInfo("FishTrap", global::Action.BuildMenuKeyA),
				new BuildMenu.BuildingInfo("AirborneCreatureLure", global::Action.BuildMenuKeyL)
			})
		}),
		new BuildMenu.DisplayInfo(BuildMenu.Category.HealthAndHappiness, "icon_category_medical", global::Action.Plan3, KKeyCode.None, new BuildMenu.DisplayInfo[]
		{
			new BuildMenu.DisplayInfo(BuildMenu.Category.Medical, "icon_category_medical", global::Action.BuildCategoryMedical, KKeyCode.C, new BuildMenu.BuildingInfo[]
			{
				new BuildMenu.BuildingInfo("Apothecary", global::Action.BuildMenuKeyA),
				new BuildMenu.BuildingInfo("MedicalCot", global::Action.BuildMenuKeyB),
				new BuildMenu.BuildingInfo("MedicalBed", global::Action.BuildMenuKeyC),
				new BuildMenu.BuildingInfo("MassageTable", global::Action.BuildMenuKeyT),
				new BuildMenu.BuildingInfo("Grave", global::Action.BuildMenuKeyR)
			}),
			new BuildMenu.DisplayInfo(BuildMenu.Category.Hygiene, "icon_category_medical", global::Action.BuildCategoryHygiene, KKeyCode.E, new BuildMenu.BuildingInfo[]
			{
				new BuildMenu.BuildingInfo("Outhouse", global::Action.BuildMenuKeyT),
				new BuildMenu.BuildingInfo("FlushToilet", global::Action.BuildMenuKeyV),
				new BuildMenu.BuildingInfo(ShowerConfig.ID, global::Action.BuildMenuKeyS),
				new BuildMenu.BuildingInfo("WashBasin", global::Action.BuildMenuKeyB),
				new BuildMenu.BuildingInfo("WashSink", global::Action.BuildMenuKeyW),
				new BuildMenu.BuildingInfo("HandSanitizer", global::Action.BuildMenuKeyA)
			}),
			new BuildMenu.DisplayInfo(BuildMenu.Category.Furniture, "icon_category_furniture", global::Action.BuildCategoryFurniture, KKeyCode.F, new BuildMenu.BuildingInfo[]
			{
				new BuildMenu.BuildingInfo(BedConfig.ID, global::Action.BuildMenuKeyC),
				new BuildMenu.BuildingInfo(LuxuryBedConfig.ID, global::Action.BuildMenuKeyX),
				new BuildMenu.BuildingInfo("DiningTable", global::Action.BuildMenuKeyD),
				new BuildMenu.BuildingInfo("FloorLamp", global::Action.BuildMenuKeyF),
				new BuildMenu.BuildingInfo("CeilingLight", global::Action.BuildMenuKeyT)
			}),
			new BuildMenu.DisplayInfo(BuildMenu.Category.Decor, "icon_category_furniture", global::Action.BuildCategoryDecor, KKeyCode.D, new BuildMenu.BuildingInfo[]
			{
				new BuildMenu.BuildingInfo("FlowerVase", global::Action.BuildMenuKeyF),
				new BuildMenu.BuildingInfo("Canvas", global::Action.BuildMenuKeyC),
				new BuildMenu.BuildingInfo("Sculpture", global::Action.BuildMenuKeyS),
				new BuildMenu.BuildingInfo("IceSculpture", global::Action.BuildMenuKeyE)
			}),
			new BuildMenu.DisplayInfo(BuildMenu.Category.Recreation, "icon_category_medical", global::Action.BuildCategoryRecreation, KKeyCode.R, new BuildMenu.BuildingInfo[]
			{
				new BuildMenu.BuildingInfo("WaterCooler", global::Action.BuildMenuKeyC),
				new BuildMenu.BuildingInfo("ArcadeMachine", global::Action.BuildMenuKeyA),
				new BuildMenu.BuildingInfo("Phonobox", global::Action.BuildMenuKeyP),
				new BuildMenu.BuildingInfo("EspressoMachine", global::Action.BuildMenuKeyE)
			})
		}),
		new BuildMenu.DisplayInfo(BuildMenu.Category.Infrastructure, "icon_category_utilities", global::Action.Plan4, KKeyCode.None, new BuildMenu.DisplayInfo[]
		{
			new BuildMenu.DisplayInfo(BuildMenu.Category.Wires, "icon_category_electrical", global::Action.BuildCategoryWires, KKeyCode.W, new BuildMenu.BuildingInfo[]
			{
				new BuildMenu.BuildingInfo("Wire", global::Action.BuildMenuKeyW),
				new BuildMenu.BuildingInfo("WireBridge", global::Action.BuildMenuKeyB),
				new BuildMenu.BuildingInfo("HighWattageWire", global::Action.BuildMenuKeyT),
				new BuildMenu.BuildingInfo("WireBridgeHighWattage", global::Action.BuildMenuKeyG),
				new BuildMenu.BuildingInfo("WireRefined", global::Action.BuildMenuKeyR),
				new BuildMenu.BuildingInfo("WireRefinedBridge", global::Action.BuildMenuKeyQ),
				new BuildMenu.BuildingInfo("WireRefinedHighWattage", global::Action.BuildMenuKeyE),
				new BuildMenu.BuildingInfo("WireRefinedBridgeHighWattage", global::Action.BuildMenuKeyA)
			}),
			new BuildMenu.DisplayInfo(BuildMenu.Category.Generators, "icon_category_electrical", global::Action.BuildCategoryGenerators, KKeyCode.G, new BuildMenu.BuildingInfo[]
			{
				new BuildMenu.BuildingInfo("ManualGenerator", global::Action.BuildMenuKeyG),
				new BuildMenu.BuildingInfo("Generator", global::Action.BuildMenuKeyC),
				new BuildMenu.BuildingInfo("HydrogenGenerator", global::Action.BuildMenuKeyD),
				new BuildMenu.BuildingInfo("MethaneGenerator", global::Action.BuildMenuKeyA),
				new BuildMenu.BuildingInfo("PetroleumGenerator", global::Action.BuildMenuKeyR),
				new BuildMenu.BuildingInfo("SteamTurbine", global::Action.BuildMenuKeyT),
				new BuildMenu.BuildingInfo("SolarPanel", global::Action.BuildMenuKeyS)
			}),
			new BuildMenu.DisplayInfo(BuildMenu.Category.PowerControl, "icon_category_electrical", global::Action.BuildCategoryPowerControl, KKeyCode.R, new BuildMenu.BuildingInfo[]
			{
				new BuildMenu.BuildingInfo("Battery", global::Action.BuildMenuKeyB),
				new BuildMenu.BuildingInfo("BatteryMedium", global::Action.BuildMenuKeyE),
				new BuildMenu.BuildingInfo("BatterySmart", global::Action.BuildMenuKeyS),
				new BuildMenu.BuildingInfo("PowerTransformerSmall", global::Action.BuildMenuKeyT),
				new BuildMenu.BuildingInfo("PowerTransformer", global::Action.BuildMenuKeyR),
				new BuildMenu.BuildingInfo(SwitchConfig.ID, global::Action.BuildMenuKeyC),
				new BuildMenu.BuildingInfo(TemperatureControlledSwitchConfig.ID, global::Action.BuildMenuKeyA),
				new BuildMenu.BuildingInfo(PressureSwitchLiquidConfig.ID, global::Action.BuildMenuKeyQ),
				new BuildMenu.BuildingInfo(PressureSwitchGasConfig.ID, global::Action.BuildMenuKeyG),
				new BuildMenu.BuildingInfo(LogicPowerRelayConfig.ID, global::Action.BuildMenuKeyX)
			}),
			new BuildMenu.DisplayInfo(BuildMenu.Category.Pipes, "icon_category_plumbing", global::Action.BuildCategoryPipes, KKeyCode.E, new BuildMenu.BuildingInfo[]
			{
				new BuildMenu.BuildingInfo("LiquidConduit", global::Action.BuildMenuKeyQ),
				new BuildMenu.BuildingInfo("LiquidConduitBridge", global::Action.BuildMenuKeyB),
				new BuildMenu.BuildingInfo("InsulatedLiquidConduit", global::Action.BuildMenuKeyW),
				new BuildMenu.BuildingInfo("LiquidConduitRadiant", global::Action.BuildMenuKeyE),
				new BuildMenu.BuildingInfo("GasConduit", global::Action.BuildMenuKeyG),
				new BuildMenu.BuildingInfo("GasConduitBridge", global::Action.BuildMenuKeyF),
				new BuildMenu.BuildingInfo("InsulatedGasConduit", global::Action.BuildMenuKeyD),
				new BuildMenu.BuildingInfo("GasConduitRadiant", global::Action.BuildMenuKeyR)
			}),
			new BuildMenu.DisplayInfo(BuildMenu.Category.PlumbingStructures, "icon_category_plumbing", global::Action.BuildCategoryPlumbingStructures, KKeyCode.B, new BuildMenu.BuildingInfo[]
			{
				new BuildMenu.BuildingInfo("LiquidPumpingStation", global::Action.BuildMenuKeyD),
				new BuildMenu.BuildingInfo("BottleEmptier", global::Action.BuildMenuKeyB),
				new BuildMenu.BuildingInfo("LiquidPump", global::Action.BuildMenuKeyQ),
				new BuildMenu.BuildingInfo("LiquidMiniPump", global::Action.BuildMenuKeyX),
				new BuildMenu.BuildingInfo("LiquidValve", global::Action.BuildMenuKeyA),
				new BuildMenu.BuildingInfo("LiquidLogicValve", global::Action.BuildMenuKeyL),
				new BuildMenu.BuildingInfo("LiquidVent", global::Action.BuildMenuKeyV),
				new BuildMenu.BuildingInfo("LiquidFilter", global::Action.BuildMenuKeyF),
				new BuildMenu.BuildingInfo("LiquidConduitPreferentialFlow", global::Action.BuildMenuKeyW),
				new BuildMenu.BuildingInfo("LiquidConduitOverflow", global::Action.BuildMenuKeyR)
			}),
			new BuildMenu.DisplayInfo(BuildMenu.Category.VentilationStructures, "icon_category_ventilation", global::Action.BuildCategoryVentilationStructures, KKeyCode.V, new BuildMenu.BuildingInfo[]
			{
				new BuildMenu.BuildingInfo("GasPump", global::Action.BuildMenuKeyQ),
				new BuildMenu.BuildingInfo("GasMiniPump", global::Action.BuildMenuKeyX),
				new BuildMenu.BuildingInfo("GasValve", global::Action.BuildMenuKeyA),
				new BuildMenu.BuildingInfo("GasLogicValve", global::Action.BuildMenuKeyC),
				new BuildMenu.BuildingInfo("GasVent", global::Action.BuildMenuKeyV),
				new BuildMenu.BuildingInfo("GasVentHighPressure", global::Action.BuildMenuKeyE),
				new BuildMenu.BuildingInfo("GasFilter", global::Action.BuildMenuKeyF),
				new BuildMenu.BuildingInfo("GasConduitPreferentialFlow", global::Action.BuildMenuKeyW),
				new BuildMenu.BuildingInfo("GasConduitOverflow", global::Action.BuildMenuKeyR)
			})
		}),
		new BuildMenu.DisplayInfo(BuildMenu.Category.Industrial, "icon_category_refinery", global::Action.Plan5, KKeyCode.None, new BuildMenu.DisplayInfo[]
		{
			new BuildMenu.DisplayInfo(BuildMenu.Category.Oxygen, "icon_category_oxygen", global::Action.BuildCategoryOxygen, KKeyCode.X, new BuildMenu.BuildingInfo[]
			{
				new BuildMenu.BuildingInfo("MineralDeoxidizer", global::Action.BuildMenuKeyX),
				new BuildMenu.BuildingInfo("AlgaeHabitat", global::Action.BuildMenuKeyA),
				new BuildMenu.BuildingInfo("AirFilter", global::Action.BuildMenuKeyD),
				new BuildMenu.BuildingInfo("CO2Scrubber", global::Action.BuildMenuKeyC),
				new BuildMenu.BuildingInfo("Electrolyzer", global::Action.BuildMenuKeyE)
			}),
			new BuildMenu.DisplayInfo(BuildMenu.Category.Utilities, "icon_category_utilities", global::Action.BuildCategoryUtilities, KKeyCode.T, new BuildMenu.BuildingInfo[]
			{
				new BuildMenu.BuildingInfo("SpaceHeater", global::Action.BuildMenuKeyS),
				new BuildMenu.BuildingInfo("LiquidHeater", global::Action.BuildMenuKeyT),
				new BuildMenu.BuildingInfo("LiquidCooledFan", global::Action.BuildMenuKeyQ),
				new BuildMenu.BuildingInfo("AirConditioner", global::Action.BuildMenuKeyR),
				new BuildMenu.BuildingInfo("LiquidConditioner", global::Action.BuildMenuKeyA),
				new BuildMenu.BuildingInfo("OreScrubber", global::Action.BuildMenuKeyC),
				new BuildMenu.BuildingInfo("ThermalBlock", global::Action.BuildMenuKeyF),
				new BuildMenu.BuildingInfo("ExteriorWall", global::Action.BuildMenuKeyW)
			}),
			new BuildMenu.DisplayInfo(BuildMenu.Category.Refining, "icon_category_refinery", global::Action.BuildCategoryRefining, KKeyCode.R, new BuildMenu.BuildingInfo[]
			{
				new BuildMenu.BuildingInfo("WaterPurifier", global::Action.BuildMenuKeyW),
				new BuildMenu.BuildingInfo("AlgaeDistillery", global::Action.BuildMenuKeyA),
				new BuildMenu.BuildingInfo("RockCrusher", global::Action.BuildMenuKeyG),
				new BuildMenu.BuildingInfo("Kiln", global::Action.BuildMenuKeyZ),
				new BuildMenu.BuildingInfo("OilWellCap", global::Action.BuildMenuKeyC),
				new BuildMenu.BuildingInfo("OilRefinery", global::Action.BuildMenuKeyR),
				new BuildMenu.BuildingInfo("Polymerizer", global::Action.BuildMenuKeyE),
				new BuildMenu.BuildingInfo("MetalRefinery", global::Action.BuildMenuKeyT),
				new BuildMenu.BuildingInfo("GlassForge", global::Action.BuildMenuKeyF)
			}),
			new BuildMenu.DisplayInfo(BuildMenu.Category.Equipment, "icon_category_misc", global::Action.BuildCategoryEquipment, KKeyCode.S, new BuildMenu.BuildingInfo[]
			{
				new BuildMenu.BuildingInfo("RoleStation", global::Action.BuildMenuKeyB),
				new BuildMenu.BuildingInfo("FarmStation", global::Action.BuildMenuKeyF),
				new BuildMenu.BuildingInfo("PowerControlStation", global::Action.BuildMenuKeyC),
				new BuildMenu.BuildingInfo("ClothingFabricator", global::Action.BuildMenuKeyT),
				new BuildMenu.BuildingInfo("SuitFabricator", global::Action.BuildMenuKeyX),
				new BuildMenu.BuildingInfo("SuitMarker", global::Action.BuildMenuKeyE),
				new BuildMenu.BuildingInfo("SuitLocker", global::Action.BuildMenuKeyD)
			})
		}),
		new BuildMenu.DisplayInfo(BuildMenu.Category.Logistics, "icon_category_ventilation", global::Action.Plan6, KKeyCode.None, new BuildMenu.DisplayInfo[]
		{
			new BuildMenu.DisplayInfo(BuildMenu.Category.TravelTubes, "icon_category_ventilation", global::Action.BuildCategoryTravelTubes, KKeyCode.T, new BuildMenu.BuildingInfo[]
			{
				new BuildMenu.BuildingInfo("TravelTube", global::Action.BuildMenuKeyT),
				new BuildMenu.BuildingInfo("TravelTubeEntrance", global::Action.BuildMenuKeyE),
				new BuildMenu.BuildingInfo("TravelTubeWallBridge", global::Action.BuildMenuKeyB)
			}),
			new BuildMenu.DisplayInfo(BuildMenu.Category.Conveyance, "icon_category_ventilation", global::Action.BuildCategoryConveyance, KKeyCode.C, new BuildMenu.BuildingInfo[]
			{
				new BuildMenu.BuildingInfo("SolidTransferArm", global::Action.BuildMenuKeyA),
				new BuildMenu.BuildingInfo("SolidConduit", global::Action.BuildMenuKeyC),
				new BuildMenu.BuildingInfo("SolidConduitInbox", global::Action.BuildMenuKeyI),
				new BuildMenu.BuildingInfo("SolidConduitOutbox", global::Action.BuildMenuKeyO),
				new BuildMenu.BuildingInfo("SolidConduitBridge", global::Action.BuildMenuKeyB)
			}),
			new BuildMenu.DisplayInfo(BuildMenu.Category.LogicWiring, "icon_category_automation", global::Action.BuildCategoryLogicWiring, KKeyCode.W, new BuildMenu.BuildingInfo[]
			{
				new BuildMenu.BuildingInfo("LogicWire", global::Action.BuildMenuKeyW),
				new BuildMenu.BuildingInfo("LogicWireBridge", global::Action.BuildMenuKeyB)
			}),
			new BuildMenu.DisplayInfo(BuildMenu.Category.LogicGates, "icon_category_automation", global::Action.BuildCategoryLogicGates, KKeyCode.G, new BuildMenu.BuildingInfo[]
			{
				new BuildMenu.BuildingInfo("LogicGateAND", global::Action.BuildMenuKeyA),
				new BuildMenu.BuildingInfo("LogicGateOR", global::Action.BuildMenuKeyR),
				new BuildMenu.BuildingInfo("LogicGateXOR", global::Action.BuildMenuKeyX),
				new BuildMenu.BuildingInfo("LogicGateNOT", global::Action.BuildMenuKeyT),
				new BuildMenu.BuildingInfo("LogicGateBUFFER", global::Action.BuildMenuKeyB),
				new BuildMenu.BuildingInfo("LogicGateFILTER", global::Action.BuildMenuKeyF),
				new BuildMenu.BuildingInfo(LogicMemoryConfig.ID, global::Action.BuildMenuKeyV)
			}),
			new BuildMenu.DisplayInfo(BuildMenu.Category.LogicSwitches, "icon_category_automation", global::Action.BuildCategoryLogicSwitches, KKeyCode.S, new BuildMenu.BuildingInfo[]
			{
				new BuildMenu.BuildingInfo(LogicSwitchConfig.ID, global::Action.BuildMenuKeyS),
				new BuildMenu.BuildingInfo(LogicPressureSensorGasConfig.ID, global::Action.BuildMenuKeyA),
				new BuildMenu.BuildingInfo(LogicPressureSensorLiquidConfig.ID, global::Action.BuildMenuKeyQ),
				new BuildMenu.BuildingInfo(LogicTemperatureSensorConfig.ID, global::Action.BuildMenuKeyT),
				new BuildMenu.BuildingInfo(LogicTimeOfDaySensorConfig.ID, global::Action.BuildMenuKeyD),
				new BuildMenu.BuildingInfo(LogicDiseaseSensorConfig.ID, global::Action.BuildMenuKeyG),
				new BuildMenu.BuildingInfo(LogicElementSensorGasConfig.ID, global::Action.BuildMenuKeyE),
				new BuildMenu.BuildingInfo("FloorSwitch", global::Action.BuildMenuKeyW),
				new BuildMenu.BuildingInfo("Checkpoint", global::Action.BuildMenuKeyC),
				new BuildMenu.BuildingInfo(CometDetectorConfig.ID, global::Action.BuildMenuKeyR)
			}),
			new BuildMenu.DisplayInfo(BuildMenu.Category.ConduitSensors, "icon_category_automation", global::Action.BuildCategoryLogicConduits, KKeyCode.X, new BuildMenu.BuildingInfo[]
			{
				new BuildMenu.BuildingInfo(LiquidConduitTemperatureSensorConfig.ID, global::Action.BuildMenuKeyT),
				new BuildMenu.BuildingInfo(LiquidConduitDiseaseSensorConfig.ID, global::Action.BuildMenuKeyG),
				new BuildMenu.BuildingInfo(LiquidConduitElementSensorConfig.ID, global::Action.BuildMenuKeyE),
				new BuildMenu.BuildingInfo(GasConduitTemperatureSensorConfig.ID, global::Action.BuildMenuKeyR),
				new BuildMenu.BuildingInfo(GasConduitDiseaseSensorConfig.ID, global::Action.BuildMenuKeyF),
				new BuildMenu.BuildingInfo(GasConduitElementSensorConfig.ID, global::Action.BuildMenuKeyS)
			})
		})
	});

	private Dictionary<BuildMenu.Category, List<BuildingDef>> categorizedBuildingMap;

	private Dictionary<BuildMenu.Category, List<BuildMenu.Category>> categorizedCategoryMap;

	private Dictionary<Tag, BuildMenu.Category> tagCategoryMap;

	private Dictionary<Tag, int> tagOrderMap;

	private const float NotificationPingExpire = 0.5f;

	private const float SpecialNotificationEmbellishDelay = 8f;

	private float timeSinceNotificationPing;

	private int notificationPingCount;

	private float initTime;

	private float updateInterval = 1f;

	private float elapsedTime;

	[Serializable]
	private struct PadInfo
	{
		public int left;

		public int right;

		public int top;

		public int bottom;
	}

	public enum Category
	{
		INVALID = -1,
		ROOT,
		Base,
		Tiles,
		Ladders,
		Doors,
		Storage,
		Infrastructure,
		Wires,
		PowerControl,
		Generators,
		Pipes,
		PlumbingStructures,
		VentilationStructures,
		Logistics,
		TravelTubes,
		Conveyance,
		LogicWiring,
		LogicGates,
		LogicSwitches,
		ConduitSensors,
		FoodAndAgriculture,
		Farming,
		Ranching,
		Cooking,
		HealthAndHappiness,
		Research,
		Medical,
		Hygiene,
		Furniture,
		Decor,
		Recreation,
		Industrial,
		Oxygen,
		Utilities,
		Refining,
		Equipment
	}

	public struct BuildingInfo
	{
		public BuildingInfo(string id, global::Action hotkey)
		{
			this.id = id;
			this.hotkey = hotkey;
		}

		public string id;

		public global::Action hotkey;
	}

	public struct DisplayInfo
	{
		public DisplayInfo(BuildMenu.Category category, string icon_name, global::Action hotkey, KKeyCode key_code, object data)
		{
			this.category = category;
			this.iconName = icon_name;
			this.hotkey = hotkey;
			this.keyCode = key_code;
			this.data = data;
		}

		public BuildMenu.Category category;

		public string iconName;

		public global::Action hotkey;

		public KKeyCode keyCode;

		public object data;
	}
}
