using System;
using STRINGS;
using TMPro;
using UnityEngine;

public class BuildingGroupScreen : KScreen
{
	public static bool SearchIsEmpty
	{
		get
		{
			return BuildingGroupScreen.Instance == null || BuildingGroupScreen.Instance.inputField.text.IsNullOrWhiteSpace();
		}
	}

	public static bool IsEditing
	{
		get
		{
			return !(BuildingGroupScreen.Instance == null) && BuildingGroupScreen.Instance.isEditing;
		}
	}

	protected override void OnPrefabInit()
	{
		BuildingGroupScreen.Instance = this;
		base.OnPrefabInit();
		base.ConsumeMouseScroll = true;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		KInputTextField kinputTextField = this.inputField;
		kinputTextField.onFocus = (global::System.Action)Delegate.Combine(kinputTextField.onFocus, new global::System.Action(delegate
		{
			base.isEditing = true;
			UISounds.PlaySound(UISounds.Sound.ClickHUD);
			this.ConfigurePlanScreenForSearch();
		}));
		this.inputField.onEndEdit.AddListener(delegate(string value)
		{
			base.isEditing = false;
		});
		this.inputField.OnValueChangesPaused = delegate
		{
			PlanScreen.Instance.RefreshCategoryPanelTitle();
			PlanScreen.Instance.RefreshSearch();
		};
		this.inputField.placeholder.GetComponent<TextMeshProUGUI>().text = UI.BUILDMENU.SEARCH_TEXT_PLACEHOLDER;
		this.clearButton.onClick += this.ClearSearch;
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		base.ConsumeMouseScroll = true;
	}

	public void ClearSearch()
	{
		this.inputField.text = "";
		this.inputField.ForceChangeValueRefresh();
	}

	private void ConfigurePlanScreenForSearch()
	{
		PlanScreen.Instance.SoftCloseRecipe();
		PlanScreen.Instance.ClearSelection();
		PlanScreen.Instance.ForceRefreshAllBuildingToggles();
		PlanScreen.Instance.ConfigurePanelSize(null);
	}

	public static BuildingGroupScreen Instance;

	public KInputTextField inputField;

	[SerializeField]
	public KButton clearButton;
}
