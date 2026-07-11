using System;
using System.Collections;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class BuildMenuBuildingsScreen : KIconToggleMenu
{
	public override float GetSortKey()
	{
		return 8f;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.UpdateBuildableStates();
		Game.Instance.Subscribe(-107300940, new Action<object>(this.OnResearchComplete));
		base.onSelect += this.OnClickBuilding;
		Game.Instance.Subscribe(-1190690038, new Action<object>(this.OnBuildToolDeactivated));
	}

	public void Configure(BuildMenu.Category category, IList<BuildMenu.BuildingInfo> building_infos)
	{
		this.ClearButtons();
		this.SetHasFocus(true);
		List<KIconToggleMenu.ToggleInfo> list = new List<KIconToggleMenu.ToggleInfo>();
		string text = category.ToString().ToUpper();
		this.titleLabel.text = Strings.Get("STRINGS.UI.NEWBUILDCATEGORIES." + text + ".BUILDMENUTITLE");
		foreach (BuildMenu.BuildingInfo buildingInfo in building_infos)
		{
			BuildingDef def = Assets.GetBuildingDef(buildingInfo.id);
			if (def.ShowInBuildMenu && !def.Deprecated)
			{
				KIconToggleMenu.ToggleInfo toggleInfo = new KIconToggleMenu.ToggleInfo(def.Name, new BuildMenuBuildingsScreen.UserData(def, PlanScreen.RequirementsState.Tech), def.HotKey, () => def.GetUISprite("ui", false));
				list.Add(toggleInfo);
			}
		}
		base.Setup(list);
		for (int i = 0; i < this.toggleInfo.Count; i++)
		{
			this.RefreshToggle(this.toggleInfo[i]);
		}
		int num = 0;
		IEnumerator enumerator2 = this.gridSizer.transform.GetEnumerator();
		try
		{
			while (enumerator2.MoveNext())
			{
				object obj = enumerator2.Current;
				Transform transform = (Transform)obj;
				if (transform.gameObject.activeSelf)
				{
					num++;
				}
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = enumerator2 as IDisposable) != null)
			{
				disposable.Dispose();
			}
		}
		this.gridSizer.constraintCount = Mathf.Min(num, 3);
		int num2 = Mathf.Min(num, this.gridSizer.constraintCount);
		int num3 = (num + this.gridSizer.constraintCount - 1) / this.gridSizer.constraintCount;
		int num4 = num2 - 1;
		int num5 = num3 - 1;
		Vector2 vector = new Vector2((float)num2 * this.gridSizer.cellSize.x + (float)num4 * this.gridSizer.spacing.x + (float)this.gridSizer.padding.left + (float)this.gridSizer.padding.right, (float)num3 * this.gridSizer.cellSize.y + (float)num5 * this.gridSizer.spacing.y + (float)this.gridSizer.padding.top + (float)this.gridSizer.padding.bottom);
		this.contentSizeLayout.minWidth = vector.x;
		this.contentSizeLayout.minHeight = vector.y;
	}

	private void ConfigureToolTip(ToolTip tooltip, BuildingDef def)
	{
		tooltip.ClearMultiStringTooltip();
		tooltip.AddMultiStringTooltip(def.Name, this.buildingToolTipSettings.BuildButtonName);
		tooltip.AddMultiStringTooltip(def.Effect, this.buildingToolTipSettings.BuildButtonDescription);
	}

	public void CloseRecipe(bool playSound = false)
	{
		if (playSound)
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Deselect", false));
		}
		ToolMenu.Instance.ClearSelection();
		this.DeactivateBuildTools();
		if (PlayerController.Instance.ActiveTool == PrebuildTool.Instance)
		{
			SelectTool.Instance.Activate();
		}
		this.selectedBuilding = null;
		this.onBuildingSelected(this.selectedBuilding);
	}

	private void RefreshToggle(KIconToggleMenu.ToggleInfo info)
	{
		if (info == null || info.toggle == null)
		{
			return;
		}
		BuildMenuBuildingsScreen.UserData userData = info.userData as BuildMenuBuildingsScreen.UserData;
		BuildingDef def = userData.def;
		TechItem techItem = Db.Get().TechItems.TryGet(def.PrefabID);
		bool flag = DebugHandler.InstantBuildMode || techItem == null || techItem.IsComplete();
		bool flag2 = flag || techItem == null || techItem.parentTech.ArePrerequisitesComplete();
		KToggle toggle = info.toggle;
		if (toggle.gameObject.activeSelf != flag2)
		{
			toggle.gameObject.SetActive(flag2);
		}
		if (toggle.bgImage == null)
		{
			return;
		}
		Image image = toggle.bgImage.GetComponentsInChildren<Image>()[1];
		Sprite uisprite = def.GetUISprite("ui", false);
		image.sprite = uisprite;
		image.SetNativeSize();
		image.rectTransform().sizeDelta /= 4f;
		ToolTip component = toggle.gameObject.GetComponent<ToolTip>();
		component.ClearMultiStringTooltip();
		string text = def.Name;
		string effect = def.Effect;
		if (def.HotKey != global::Action.NumActions)
		{
			text = text + " " + GameUtil.GetHotkeyString(def.HotKey);
		}
		component.AddMultiStringTooltip(text, this.buildingToolTipSettings.BuildButtonName);
		component.AddMultiStringTooltip(effect, this.buildingToolTipSettings.BuildButtonDescription);
		LocText componentInChildren = toggle.GetComponentInChildren<LocText>();
		if (componentInChildren != null)
		{
			componentInChildren.text = def.Name;
		}
		PlanScreen.RequirementsState requirementsState = BuildMenu.Instance.BuildableState(def);
		int num = ((requirementsState != PlanScreen.RequirementsState.Complete) ? 0 : 1);
		ImageToggleState.State state;
		if (def == this.selectedBuilding && (requirementsState == PlanScreen.RequirementsState.Complete || DebugHandler.InstantBuildMode))
		{
			state = ImageToggleState.State.Active;
		}
		else
		{
			state = ((requirementsState != PlanScreen.RequirementsState.Complete && !DebugHandler.InstantBuildMode) ? ImageToggleState.State.Disabled : ImageToggleState.State.Inactive);
		}
		if (def == this.selectedBuilding && state == ImageToggleState.State.Disabled)
		{
			state = ImageToggleState.State.DisabledActive;
		}
		else if (state == ImageToggleState.State.Disabled)
		{
			state = ImageToggleState.State.Disabled;
		}
		toggle.GetComponent<ImageToggleState>().SetState(state);
		Material material;
		Color color;
		if (requirementsState == PlanScreen.RequirementsState.Complete || DebugHandler.InstantBuildMode)
		{
			material = this.defaultUIMaterial;
			color = Color.white;
		}
		else
		{
			material = this.desaturatedUIMaterial;
			Color color2;
			if (flag)
			{
				color2 = new Color(1f, 1f, 1f, 0.6f);
			}
			else
			{
				Color color3 = new Color(1f, 1f, 1f, 0.15f);
				image.color = color3;
				color2 = color3;
			}
			color = color2;
		}
		if (image.material != material)
		{
			image.material = material;
			image.color = color;
		}
		Image fgImage = toggle.gameObject.GetComponent<KToggle>().fgImage;
		fgImage.gameObject.SetActive(false);
		if (!flag)
		{
			fgImage.sprite = this.Overlay_NeedTech;
			fgImage.gameObject.SetActive(true);
			string text2 = string.Format(UI.PRODUCTINFO_REQUIRESRESEARCHDESC, techItem.parentTech.Name);
			component.AddMultiStringTooltip("\n", this.buildingToolTipSettings.ResearchRequirement);
			component.AddMultiStringTooltip(text2, this.buildingToolTipSettings.ResearchRequirement);
		}
		else if (requirementsState != PlanScreen.RequirementsState.Complete)
		{
			fgImage.gameObject.SetActive(false);
			component.AddMultiStringTooltip("\n", this.buildingToolTipSettings.ResearchRequirement);
			string text3 = UI.PRODUCTINFO_MISSINGRESOURCES_HOVER;
			component.AddMultiStringTooltip(text3, this.buildingToolTipSettings.ResearchRequirement);
			foreach (Recipe.Ingredient ingredient in def.CraftRecipe.Ingredients)
			{
				string text4 = string.Format("{0}{1}: {2}", "• ", ingredient.tag.ProperName(), GameUtil.GetFormattedMass(ingredient.amount, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				component.AddMultiStringTooltip(text4, this.buildingToolTipSettings.ResearchRequirement);
			}
			component.AddMultiStringTooltip(string.Empty, this.buildingToolTipSettings.ResearchRequirement);
		}
	}

	public void ClearUI()
	{
		base.Show(false);
		this.ClearButtons();
	}

	private void ClearButtons()
	{
		foreach (KToggle ktoggle in this.toggles)
		{
			ktoggle.gameObject.SetActive(false);
			ktoggle.gameObject.transform.SetParent(null);
			global::UnityEngine.Object.DestroyImmediate(ktoggle.gameObject);
		}
		if (this.toggles != null)
		{
			this.toggles.Clear();
		}
		if (this.toggleInfo != null)
		{
			this.toggleInfo.Clear();
		}
	}

	private void OnClickBuilding(KIconToggleMenu.ToggleInfo toggle_info)
	{
		BuildMenuBuildingsScreen.UserData userData = toggle_info.userData as BuildMenuBuildingsScreen.UserData;
		this.OnSelectBuilding(userData.def);
	}

	private void OnSelectBuilding(BuildingDef def)
	{
		PlanScreen.RequirementsState requirementsState = BuildMenu.Instance.BuildableState(def);
		if (requirementsState != PlanScreen.RequirementsState.Complete && requirementsState != PlanScreen.RequirementsState.Materials)
		{
			this.selectedBuilding = null;
			this.ClearSelection();
			this.CloseRecipe(true);
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Negative", false));
		}
		else if (def != this.selectedBuilding)
		{
			this.selectedBuilding = def;
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click", false));
		}
		else
		{
			this.selectedBuilding = null;
			this.ClearSelection();
			this.CloseRecipe(true);
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Deselect", false));
		}
		this.onBuildingSelected(this.selectedBuilding);
	}

	public void UpdateBuildableStates()
	{
		if (this.toggleInfo == null || this.toggleInfo.Count <= 0)
		{
			return;
		}
		BuildingDef buildingDef = null;
		foreach (KIconToggleMenu.ToggleInfo toggleInfo in this.toggleInfo)
		{
			this.RefreshToggle(toggleInfo);
			BuildMenuBuildingsScreen.UserData userData = toggleInfo.userData as BuildMenuBuildingsScreen.UserData;
			BuildingDef def = userData.def;
			if (!def.Deprecated)
			{
				PlanScreen.RequirementsState requirementsState = BuildMenu.Instance.BuildableState(def);
				if (requirementsState != userData.requirementsState)
				{
					if (def == BuildMenu.Instance.SelectedBuildingDef)
					{
						buildingDef = def;
					}
					this.RefreshToggle(toggleInfo);
					userData.requirementsState = requirementsState;
				}
			}
		}
		if (buildingDef != null)
		{
			BuildMenu.Instance.RefreshProductInfoScreen(buildingDef);
		}
	}

	private void OnResearchComplete(object data)
	{
		this.UpdateBuildableStates();
	}

	private void DeactivateBuildTools()
	{
		InterfaceTool activeTool = PlayerController.Instance.ActiveTool;
		if (activeTool != null)
		{
			Type type = activeTool.GetType();
			if (type == typeof(BuildTool) || typeof(BaseUtilityBuildTool).IsAssignableFrom(type) || typeof(PrebuildTool).IsAssignableFrom(type))
			{
				activeTool.DeactivateTool(null);
			}
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (!this.mouseOver || !this.ConsumeMouseScroll || e.TryConsume(global::Action.ZoomIn) || e.TryConsume(global::Action.ZoomOut))
		{
		}
		if (!this.HasFocus)
		{
			return;
		}
		if (e.TryConsume(global::Action.Escape))
		{
			Game.Instance.Trigger(288942073, null);
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Close", false));
			return;
		}
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

	public override void OnKeyUp(KButtonEvent e)
	{
		if (!this.HasFocus)
		{
			return;
		}
		if (this.selectedBuilding != null && PlayerController.Instance.ConsumeIfNotDragging(e, global::Action.MouseRight))
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Close", false));
			Game.Instance.Trigger(288942073, null);
			return;
		}
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

	public override void Close()
	{
		ToolMenu.Instance.ClearSelection();
		this.DeactivateBuildTools();
		if (PlayerController.Instance.ActiveTool == PrebuildTool.Instance)
		{
			SelectTool.Instance.Activate();
		}
		this.selectedBuilding = null;
		this.ClearButtons();
		base.gameObject.SetActive(false);
	}

	public override void SetHasFocus(bool has_focus)
	{
		base.SetHasFocus(has_focus);
		if (this.focusIndicator != null)
		{
			this.focusIndicator.color = ((!has_focus) ? this.unfocusedColour : this.focusedColour);
		}
	}

	private void OnBuildToolDeactivated(object data)
	{
		this.CloseRecipe(false);
	}

	[SerializeField]
	private Image focusIndicator;

	[SerializeField]
	private Color32 focusedColour;

	[SerializeField]
	private Color32 unfocusedColour;

	public Action<BuildingDef> onBuildingSelected;

	[SerializeField]
	private LocText titleLabel;

	[SerializeField]
	private BuildMenuBuildingsScreen.BuildingToolTipSettings buildingToolTipSettings;

	[SerializeField]
	private LayoutElement contentSizeLayout;

	[SerializeField]
	private GridLayoutGroup gridSizer;

	[SerializeField]
	private Sprite Overlay_NeedTech;

	[SerializeField]
	private Material defaultUIMaterial;

	[SerializeField]
	private Material desaturatedUIMaterial;

	private BuildingDef selectedBuilding;

	[Serializable]
	public struct BuildingToolTipSettings
	{
		public TextStyleSetting BuildButtonName;

		public TextStyleSetting BuildButtonDescription;

		public TextStyleSetting MaterialRequirement;

		public TextStyleSetting ResearchRequirement;
	}

	[Serializable]
	public struct BuildingNameTextSetting
	{
		public TextStyleSetting ActiveSelected;

		public TextStyleSetting ActiveDeselected;

		public TextStyleSetting InactiveSelected;

		public TextStyleSetting InactiveDeselected;
	}

	private class UserData
	{
		public UserData(BuildingDef def, PlanScreen.RequirementsState state)
		{
			this.def = def;
			this.requirementsState = state;
		}

		public BuildingDef def;

		public PlanScreen.RequirementsState requirementsState;
	}
}
