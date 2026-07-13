using System;
using System.Collections;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class RocketModuleSideScreen : SideScreenContent
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		RocketModuleSideScreen.instance = this;
	}

	protected override void OnForcedCleanUp()
	{
		RocketModuleSideScreen.instance = null;
		base.OnForcedCleanUp();
	}

	public override int GetSideScreenSortOrder()
	{
		return 104;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.addNewModuleButton.onClick += delegate
		{
			Vector2 vector = Vector2.zero;
			if (SelectModuleSideScreen.Instance != null)
			{
				vector = SelectModuleSideScreen.Instance.mainContents.GetComponent<KScrollRect>().content.rectTransform().anchoredPosition;
			}
			this.ClickAddNew(vector.y, null);
		};
		this.removeModuleButton.onClick += this.ClickRemove;
		this.moveModuleUpButton.onClick += this.ClickSwapUp;
		this.moveModuleDownButton.onClick += this.ClickSwapDown;
		this.changeModuleButton.onClick += delegate
		{
			Vector2 vector2 = Vector2.zero;
			if (SelectModuleSideScreen.Instance != null)
			{
				vector2 = SelectModuleSideScreen.Instance.mainContents.GetComponent<KScrollRect>().content.rectTransform().anchoredPosition;
			}
			this.ClickChangeModule(vector2.y);
		};
		this.moduleNameLabel.textStyleSetting = this.nameSetting;
		this.moduleDescriptionLabel.textStyleSetting = this.descriptionSetting;
		this.moduleNameLabel.ApplySettings();
		this.moduleDescriptionLabel.ApplySettings();
	}

	protected override void OnCmpDisable()
	{
		base.OnCmpDisable();
		DetailsScreen.Instance.ClearSecondarySideScreen();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<ReorderableBuilding>() != null;
	}

	public override void SetTarget(GameObject new_target)
	{
		if (new_target == null)
		{
			global::Debug.LogError("Invalid gameObject received");
			return;
		}
		this.reorderable = new_target.GetComponent<ReorderableBuilding>();
		this.moduleIcon.sprite = Def.GetUISprite(this.reorderable.gameObject, "ui", false).first;
		this.moduleNameLabel.SetText(this.reorderable.GetProperName());
		this.moduleDescriptionLabel.SetText(this.reorderable.GetComponent<Building>().Desc);
		this.UpdateButtonStates();
	}

	public void UpdateButtonStates()
	{
		this.changeModuleButton.isInteractable = this.reorderable.CanChangeModule();
		this.changeModuleButton.GetComponent<ToolTip>().SetSimpleTooltip(this.changeModuleButton.isInteractable ? UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONCHANGEMODULE.DESC.text : UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONCHANGEMODULE.INVALID.text);
		this.addNewModuleButton.isInteractable = true;
		this.addNewModuleButton.GetComponent<ToolTip>().SetSimpleTooltip(UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.ADDMODULE.DESC.text);
		Deconstructable component = this.reorderable.GetComponent<Deconstructable>();
		bool flag = component != null && component.IsMarkedForDeconstruction();
		this.removeModuleButton.isInteractable = component != null && this.reorderable.CanRemoveModule();
		this.removeModuleButton.GetComponent<ToolTip>().SetSimpleTooltip(this.removeModuleButton.isInteractable ? (flag ? UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONREMOVEMODULE.DESC_CANCEL.text : UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONREMOVEMODULE.DESC.text) : UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONREMOVEMODULE.INVALID.text);
		this.removeButtonLabel.SetText(flag ? UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONREMOVEMODULE.LABEL_CANCEL : UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONREMOVEMODULE.LABEL);
		this.moveModuleDownButton.isInteractable = this.reorderable.CanSwapDown(true);
		this.moveModuleDownButton.GetComponent<ToolTip>().SetSimpleTooltip(this.moveModuleDownButton.isInteractable ? UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONSWAPMODULEDOWN.DESC.text : UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONSWAPMODULEDOWN.INVALID.text);
		this.moveModuleUpButton.isInteractable = this.reorderable.CanSwapUp(true);
		this.moveModuleUpButton.GetComponent<ToolTip>().SetSimpleTooltip(this.moveModuleUpButton.isInteractable ? UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONSWAPMODULEUP.DESC.text : UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONSWAPMODULEUP.INVALID.text);
	}

	public void ClickAddNew(float scrollViewPosition, BuildingDef autoSelectDef = null)
	{
		SelectModuleSideScreen selectModuleSideScreen = (SelectModuleSideScreen)DetailsScreen.Instance.SetSecondarySideScreen(this.changeModuleSideScreen, UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.CHANGEMODULEPANEL);
		selectModuleSideScreen.addingNewModule = true;
		selectModuleSideScreen.SetTarget(this.reorderable.gameObject);
		if (autoSelectDef != null)
		{
			selectModuleSideScreen.SelectModule(autoSelectDef);
		}
		this.ScrollToTargetPoint(scrollViewPosition);
	}

	private void ScrollToTargetPoint(float scrollViewPosition)
	{
		if (SelectModuleSideScreen.Instance != null)
		{
			SelectModuleSideScreen.Instance.mainContents.GetComponent<KScrollRect>().content.anchoredPosition = new Vector2(0f, scrollViewPosition);
			if (base.gameObject.activeInHierarchy)
			{
				base.StartCoroutine(this.DelayedScrollToTargetPoint(scrollViewPosition));
			}
		}
	}

	private IEnumerator DelayedScrollToTargetPoint(float scrollViewPosition)
	{
		if (SelectModuleSideScreen.Instance != null)
		{
			yield return SequenceUtil.WaitForEndOfFrame;
			SelectModuleSideScreen.Instance.mainContents.GetComponent<KScrollRect>().content.anchoredPosition = new Vector2(0f, scrollViewPosition);
		}
		yield break;
	}

	private void ClickRemove()
	{
		Deconstructable component = this.reorderable.GetComponent<Deconstructable>();
		if (component == null)
		{
			return;
		}
		if (component.IsMarkedForDeconstruction())
		{
			component.CancelDeconstruction();
		}
		else
		{
			this.reorderable.Trigger(-790448070, null);
		}
		this.UpdateButtonStates();
		component.Trigger(1980521255, null);
	}

	private void ClickSwapUp()
	{
		this.reorderable.SwapWithAbove(true);
		this.UpdateButtonStates();
	}

	private void ClickSwapDown()
	{
		this.reorderable.SwapWithBelow(true);
		this.UpdateButtonStates();
	}

	private void ClickChangeModule(float scrollViewPosition)
	{
		SelectModuleSideScreen selectModuleSideScreen = (SelectModuleSideScreen)DetailsScreen.Instance.SetSecondarySideScreen(this.changeModuleSideScreen, UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.CHANGEMODULEPANEL);
		selectModuleSideScreen.addingNewModule = false;
		selectModuleSideScreen.SetTarget(this.reorderable.gameObject);
		this.ScrollToTargetPoint(scrollViewPosition);
	}

	public static RocketModuleSideScreen instance;

	private ReorderableBuilding reorderable;

	public KScreen changeModuleSideScreen;

	public Image moduleIcon;

	[Header("Buttons")]
	public KButton addNewModuleButton;

	public KButton removeModuleButton;

	public KButton changeModuleButton;

	public KButton moveModuleUpButton;

	public KButton moveModuleDownButton;

	[Header("Labels")]
	public LocText removeButtonLabel;

	public LocText moduleNameLabel;

	public LocText moduleDescriptionLabel;

	public TextStyleSetting nameSetting;

	public TextStyleSetting descriptionSetting;
}
