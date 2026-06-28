using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildMenuCategoriesScreen : KIconToggleMenu
{
	public override float GetSortKey()
	{
		return 6f;
	}

	public BuildMenu.Category Category
	{
		get
		{
			return this.category;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.onSelect += this.OnClickCategory;
	}

	public void Configure(BuildMenu.Category category, int depth, object data, Dictionary<BuildMenu.Category, List<BuildingDef>> categorized_building_map, Dictionary<BuildMenu.Category, List<BuildMenu.Category>> categorized_category_map, BuildMenuBuildingsScreen buildings_screen)
	{
		this.category = category;
		this.categorizedBuildingMap = categorized_building_map;
		this.categorizedCategoryMap = categorized_category_map;
		this.buildingsScreen = buildings_screen;
		List<KIconToggleMenu.ToggleInfo> list = new List<KIconToggleMenu.ToggleInfo>();
		if (data.GetType() == typeof(string[]))
		{
			this.buildingNames = (string[])data;
		}
		else if (data.GetType() == typeof(BuildMenu.DisplayInfo[]))
		{
			this.subcategories = new List<BuildMenu.Category>();
			BuildMenu.DisplayInfo[] array = (BuildMenu.DisplayInfo[])data;
			foreach (BuildMenu.DisplayInfo displayInfo in array)
			{
				string iconName = displayInfo.iconName;
				string text = displayInfo.category.ToString().ToUpper();
				KIconToggleMenu.ToggleInfo toggleInfo = new KIconToggleMenu.ToggleInfo(Strings.Get("STRINGS.UI.NEWBUILDCATEGORIES." + text + ".NAME"), iconName, new BuildMenuCategoriesScreen.UserData
				{
					category = displayInfo.category,
					depth = depth,
					requirementsState = PlanScreen.RequirementsState.Tech
				}, displayInfo.hotkey, Strings.Get("STRINGS.UI.NEWBUILDCATEGORIES." + text + ".TOOLTIP"), string.Empty);
				list.Add(toggleInfo);
				this.subcategories.Add(displayInfo.category);
			}
			base.Setup(list);
			this.toggles.ForEach(delegate(KToggle to)
			{
				ImageToggleState[] components = to.GetComponents<ImageToggleState>();
				foreach (ImageToggleState imageToggleState in components)
				{
					if (imageToggleState.TargetImage.sprite != null && imageToggleState.TargetImage.name == "FG" && !imageToggleState.useSprites)
					{
						imageToggleState.SetSprites(Assets.GetSprite(imageToggleState.TargetImage.sprite.name + "_disabled"), imageToggleState.TargetImage.sprite, imageToggleState.TargetImage.sprite, Assets.GetSprite(imageToggleState.TargetImage.sprite.name + "_disabled"));
					}
				}
				to.GetComponent<KToggle>().soundPlayer.Enabled = false;
			});
		}
		this.UpdateBuildableStates(true);
	}

	private void OnClickCategory(KIconToggleMenu.ToggleInfo toggle_info)
	{
		BuildMenuCategoriesScreen.UserData userData = (BuildMenuCategoriesScreen.UserData)toggle_info.userData;
		PlanScreen.RequirementsState requirementsState = userData.requirementsState;
		if (requirementsState != PlanScreen.RequirementsState.Complete && requirementsState != PlanScreen.RequirementsState.Materials)
		{
			this.selectedCategory = BuildMenu.Category.INVALID;
			this.ClearSelection();
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Negative", false));
		}
		else if (this.selectedCategory != userData.category)
		{
			this.selectedCategory = userData.category;
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click", false));
		}
		else
		{
			this.selectedCategory = BuildMenu.Category.INVALID;
			this.ClearSelection();
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Deselect", false));
		}
		toggle_info.toggle.GetComponent<PlanCategoryNotifications>().ToggleAttention(false);
		if (this.onCategoryClicked != null)
		{
			this.onCategoryClicked(this.selectedCategory, userData.depth);
		}
	}

	private void UpdateButtonStates()
	{
		if (this.toggleInfo != null && this.toggleInfo.Count > 0)
		{
			foreach (KIconToggleMenu.ToggleInfo toggleInfo in this.toggleInfo)
			{
				BuildMenu.Category category = ((BuildMenuCategoriesScreen.UserData)toggleInfo.userData).category;
				PlanScreen.RequirementsState categoryRequirements = this.GetCategoryRequirements(category);
				bool flag = categoryRequirements == PlanScreen.RequirementsState.Tech;
				toggleInfo.toggle.gameObject.SetActive(!flag);
				if (categoryRequirements != PlanScreen.RequirementsState.Complete)
				{
					if (categoryRequirements == PlanScreen.RequirementsState.Materials)
					{
						toggleInfo.toggle.fgImage.SetAlpha((!flag) ? 1f : 0.2509804f);
						ImageToggleState.State state = ((this.selectedCategory == BuildMenu.Category.INVALID || category != this.selectedCategory) ? ImageToggleState.State.Disabled : ImageToggleState.State.DisabledActive);
						this.SetImageToggleState(toggleInfo.toggle.gameObject, state);
					}
				}
				else
				{
					ImageToggleState.State state2 = ((this.selectedCategory != BuildMenu.Category.INVALID && category == this.selectedCategory) ? ImageToggleState.State.Active : ImageToggleState.State.Inactive);
					this.SetImageToggleState(toggleInfo.toggle.gameObject, state2);
				}
				GameObject gameObject = toggleInfo.toggle.fgImage.transform.Find("ResearchIcon").gameObject;
				gameObject.gameObject.SetActive(flag);
			}
		}
	}

	private void SetImageToggleState(GameObject target, ImageToggleState.State state)
	{
		ImageToggleState[] components = target.GetComponents<ImageToggleState>();
		foreach (ImageToggleState imageToggleState in components)
		{
			imageToggleState.SetState(state);
		}
	}

	private PlanScreen.RequirementsState GetCategoryRequirements(BuildMenu.Category category)
	{
		bool flag = true;
		bool flag2 = true;
		List<BuildingDef> list;
		List<BuildMenu.Category> list2;
		if (this.categorizedBuildingMap.TryGetValue(category, out list))
		{
			if (list.Count > 0)
			{
				foreach (BuildingDef buildingDef in list)
				{
					if (buildingDef.ShowInBuildMenu && !buildingDef.Deprecated)
					{
						PlanScreen.RequirementsState requirementsState = BuildMenu.Instance.BuildableState(buildingDef);
						flag = flag && requirementsState == PlanScreen.RequirementsState.Tech;
						flag2 = flag2 && (requirementsState == PlanScreen.RequirementsState.Materials || requirementsState == PlanScreen.RequirementsState.Tech);
					}
				}
			}
		}
		else if (this.categorizedCategoryMap.TryGetValue(category, out list2))
		{
			foreach (BuildMenu.Category category2 in list2)
			{
				PlanScreen.RequirementsState categoryRequirements = this.GetCategoryRequirements(category2);
				flag = flag && categoryRequirements == PlanScreen.RequirementsState.Tech;
				flag2 = flag2 && (categoryRequirements == PlanScreen.RequirementsState.Materials || categoryRequirements == PlanScreen.RequirementsState.Tech);
			}
		}
		PlanScreen.RequirementsState requirementsState2;
		if (flag)
		{
			requirementsState2 = PlanScreen.RequirementsState.Tech;
		}
		else if (flag2)
		{
			requirementsState2 = PlanScreen.RequirementsState.Materials;
		}
		else
		{
			requirementsState2 = PlanScreen.RequirementsState.Complete;
		}
		if (DebugHandler.InstantBuildMode)
		{
			requirementsState2 = PlanScreen.RequirementsState.Complete;
		}
		return requirementsState2;
	}

	public void UpdateNotifications(ICollection<BuildMenu.Category> updated_categories)
	{
		if (this.toggleInfo == null)
		{
			return;
		}
		this.UpdateBuildableStates(false);
		foreach (KIconToggleMenu.ToggleInfo toggleInfo in this.toggleInfo)
		{
			BuildMenu.Category category = ((BuildMenuCategoriesScreen.UserData)toggleInfo.userData).category;
			if (updated_categories.Contains(category))
			{
				toggleInfo.toggle.gameObject.GetComponent<PlanCategoryNotifications>().ToggleAttention(true);
			}
		}
	}

	public override void Close()
	{
		base.Close();
		this.selectedCategory = BuildMenu.Category.INVALID;
		this.SetHasFocus(false);
		if (this.buildingNames != null)
		{
			this.buildingsScreen.Close();
		}
	}

	[ContextMenu("ForceUpdateBuildableStates")]
	private void ForceUpdateBuildableStates()
	{
		this.UpdateBuildableStates(true);
	}

	public void UpdateBuildableStates(bool skip_flourish)
	{
		if (this.subcategories != null && this.subcategories.Count > 0)
		{
			this.UpdateButtonStates();
			foreach (KIconToggleMenu.ToggleInfo toggleInfo in this.toggleInfo)
			{
				BuildMenuCategoriesScreen.UserData userData = (BuildMenuCategoriesScreen.UserData)toggleInfo.userData;
				BuildMenu.Category category = userData.category;
				PlanScreen.RequirementsState categoryRequirements = this.GetCategoryRequirements(category);
				if (userData.requirementsState != categoryRequirements)
				{
					userData.requirementsState = categoryRequirements;
					toggleInfo.userData = userData;
					if (!skip_flourish)
					{
						toggleInfo.toggle.ActivateFlourish(false);
						string text = "NotificationPing";
						Animator component = toggleInfo.toggle.GetComponent<Animator>();
						if (!component.GetCurrentAnimatorStateInfo(0).IsTag(text))
						{
							toggleInfo.toggle.gameObject.GetComponent<Animator>().Play(text);
							BuildMenu.Instance.PlayNewBuildingSounds();
						}
					}
				}
			}
		}
		else
		{
			this.buildingsScreen.UpdateBuildableStates();
		}
	}

	protected override void OnShow(bool show)
	{
		if (this.buildingNames != null)
		{
			if (show)
			{
				this.buildingsScreen.Configure(this.category, this.buildingNames);
				this.buildingsScreen.Show(true);
			}
			else
			{
				this.buildingsScreen.Close();
			}
		}
		base.OnShow(show);
	}

	public override void ClearSelection()
	{
		this.selectedCategory = BuildMenu.Category.INVALID;
		base.ClearSelection();
		foreach (KToggle ktoggle in this.toggles)
		{
			ktoggle.isOn = false;
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (this.modalKeyInputBehaviour)
		{
			if (this.HasFocus)
			{
				if (e.TryConsume(global::Action.Escape))
				{
					Game.Instance.Trigger(288942073, null);
				}
				else
				{
					base.OnKeyDown(e);
					if (!e.Consumed)
					{
						global::Action action = e.GetAction();
						if (action >= global::Action.BUILD_MENU_START_INTERCEPT)
						{
							e.TryConsume(action);
						}
					}
				}
			}
		}
		else
		{
			base.OnKeyDown(e);
			if (e.Consumed)
			{
				this.UpdateButtonStates();
			}
		}
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		if (this.modalKeyInputBehaviour)
		{
			if (this.HasFocus)
			{
				if (e.TryConsume(global::Action.Escape))
				{
					Game.Instance.Trigger(288942073, null);
				}
				else
				{
					base.OnKeyUp(e);
					if (!e.Consumed)
					{
						global::Action action = e.GetAction();
						if (action >= global::Action.BUILD_MENU_START_INTERCEPT)
						{
							e.TryConsume(action);
						}
					}
				}
			}
		}
		else
		{
			base.OnKeyUp(e);
		}
	}

	public override void SetHasFocus(bool has_focus)
	{
		base.SetHasFocus(has_focus);
		if (this.focusIndicator != null)
		{
			this.focusIndicator.color = ((!has_focus) ? this.unfocusedColour : this.focusedColour);
		}
	}

	public Action<BuildMenu.Category, int> onCategoryClicked;

	[SerializeField]
	public bool modalKeyInputBehaviour;

	[SerializeField]
	private Image focusIndicator;

	[SerializeField]
	private Color32 focusedColour;

	[SerializeField]
	private Color32 unfocusedColour;

	private IList<BuildMenu.Category> subcategories;

	private Dictionary<BuildMenu.Category, List<BuildingDef>> categorizedBuildingMap;

	private Dictionary<BuildMenu.Category, List<BuildMenu.Category>> categorizedCategoryMap;

	private BuildMenuBuildingsScreen buildingsScreen;

	private BuildMenu.Category category;

	private IList<string> buildingNames;

	private BuildMenu.Category selectedCategory = BuildMenu.Category.INVALID;

	private struct UserData
	{
		public BuildMenu.Category category;

		public int depth;

		public PlanScreen.RequirementsState requirementsState;
	}
}
