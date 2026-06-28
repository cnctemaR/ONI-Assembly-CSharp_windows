using System;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.UI;

public class BuildMenu : KScreen
{
	public override float GetSortKey()
	{
		return 2f;
	}

	public static BuildMenu Instance { get; private set; }

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.ConsumeMouseScroll = true;
		this.initTime = KTime.Instance.UnscaledGameTime;
		base.gameObject.SetActive(false);
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
			string[] array3 = (string[])data;
			foreach (string text in array3)
			{
				Tag tag = new Tag(text);
				category_map[tag] = category;
				order_map[tag] = building_index;
				building_index++;
				List<BuildingDef> list3;
				if (!categorized_building_map.TryGetValue(category, out list3))
				{
					list3 = new List<BuildingDef>();
					categorized_building_map[category] = list3;
				}
				list3.Add(Assets.GetBuildingDef(text));
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
		this.CloseMenus();
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
					EventInstance eventInstance = SoundEvent.BeginOneShot(sound, SoundListenerController.Instance.transform.position);
					SoundEvent.EndOneShot(eventInstance);
				}
			}
			string sound2 = GlobalAssets.GetSound("NewBuildable", false);
			if (sound2 != null)
			{
				EventInstance eventInstance2 = SoundEvent.BeginOneShot(sound2, SoundListenerController.Instance.transform.position);
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
		if (!DebugHandler.InstantBuildMode)
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

	private static readonly BuildMenu.DisplayInfo OrderedBuildings = new BuildMenu.DisplayInfo(BuildMenu.Category.INVALID, "none", global::Action.NumActions, null);

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
		Ladders,
		Tiles,
		Doors,
		TravelTubes,
		Storage,
		Research,
		Infrastructure,
		Generators,
		Wires,
		PowerControl,
		PlumbingStructures,
		Pipes,
		VentilationStructures,
		Tubes,
		LogicWiring,
		LogicGates,
		LogicSwitches,
		FoodAndAgriculture,
		Cooking,
		Farming,
		Ranching,
		HealthAndHappiness,
		Furniture,
		Decor,
		Hygiene,
		Medical,
		Recreation,
		Industrial,
		Oxygen,
		Utilities,
		Refining,
		Equipment
	}

	public struct DisplayInfo
	{
		public DisplayInfo(BuildMenu.Category category, string icon_name, global::Action hotkey, object data)
		{
			this.category = category;
			this.iconName = icon_name;
			this.hotkey = hotkey;
			this.data = data;
		}

		public BuildMenu.Category category;

		public string iconName;

		public global::Action hotkey;

		public object data;
	}
}
