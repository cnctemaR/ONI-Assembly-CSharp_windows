using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlanBuildingToggle : KToggle
{
	public void Config(BuildingDef def)
	{
		this.def = def;
		this.techItem = Db.Get().TechItems.TryGet(def.PrefabID);
		this.gameSubscriptions.Add(Game.Instance.Subscribe(-107300940, new Action<object>(this.CheckResearch)));
		this.gameSubscriptions.Add(Game.Instance.Subscribe(-1948169901, new Action<object>(this.CheckResearch)));
		this.gameSubscriptions.Add(Game.Instance.Subscribe(1557339983, new Action<object>(this.CheckResearch)));
		this.sprite = def.GetUISprite("ui", false);
		base.onClick += delegate
		{
			PlanScreen.Instance.OnSelectBuilding(this.gameObject, def, null);
		};
		this.CheckResearch(null);
		this.Refresh();
	}

	protected override void OnDestroy()
	{
		if (Game.Instance != null)
		{
			foreach (int num in this.gameSubscriptions)
			{
				Game.Instance.Unsubscribe(num);
			}
		}
		this.gameSubscriptions.Clear();
		base.OnDestroy();
	}

	private void CheckResearch(object data = null)
	{
		this.researchComplete = PlanScreen.TechRequirementsMet(this.techItem);
	}

	public bool Refresh()
	{
		bool flag = this.researchComplete || DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive;
		bool flag2 = false;
		if (base.gameObject.activeSelf != flag)
		{
			base.gameObject.SetActive(flag);
			flag2 = true;
		}
		if (!base.gameObject.activeInHierarchy)
		{
			return flag2;
		}
		if (this.bgImage == null)
		{
			return flag2;
		}
		this.PositionTooltip();
		this.RefreshLabel();
		this.RefreshDisplay();
		return flag2;
	}

	private void RefreshLabel()
	{
		if (this.text != null)
		{
			this.text.fontSize = (float)(ScreenResolutionMonitor.UsingGamepadUIMode() ? PlanScreen.fontSizeBigMode : PlanScreen.fontSizeStandardMode);
			this.text.text = this.def.Name;
		}
	}

	private void RefreshDisplay()
	{
		PlanScreen.RequirementsState buildableState = PlanScreen.Instance.GetBuildableState(this.def);
		bool flag = buildableState == PlanScreen.RequirementsState.Complete || DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive;
		bool flag2 = base.gameObject == PlanScreen.Instance.SelectedBuildingGameObject;
		ImageToggleState.State state = ((buildableState == PlanScreen.RequirementsState.Complete) ? ImageToggleState.State.Inactive : ImageToggleState.State.Disabled);
		if (flag2 && flag)
		{
			state = ImageToggleState.State.Active;
		}
		else if (!flag2 && flag)
		{
			state = ImageToggleState.State.Inactive;
		}
		else if (flag2 && !flag)
		{
			state = ImageToggleState.State.DisabledActive;
		}
		else if (!flag2 && !flag)
		{
			state = ImageToggleState.State.Disabled;
		}
		this.imageToggleState.SetState(state);
		this.RefreshBuildingButtonIconAndColors(flag);
		this.RefreshFG(buildableState);
	}

	private void PositionTooltip()
	{
		this.tooltip.overrideParentObject = PlanScreen.Instance.buildingGroupsRoot;
		this.tooltip.tooltipPivot = Vector2.zero;
		this.tooltip.parentPositionAnchor = new Vector2(1f, 0f);
		this.tooltip.tooltipPositionOffset = (PlanScreen.Instance.ProductInfoScreen.gameObject.activeSelf ? new Vector2(16f + PlanScreen.Instance.ProductInfoScreen.rectTransform().sizeDelta.x, 0f) : new Vector2(-40f, 0f));
		this.tooltip.ClearMultiStringTooltip();
		string name = this.def.Name;
		string effect = this.def.Effect;
		this.tooltip.AddMultiStringTooltip(name, PlanScreen.Instance.buildingToolTipSettings.BuildButtonName);
		this.tooltip.AddMultiStringTooltip(effect, PlanScreen.Instance.buildingToolTipSettings.BuildButtonDescription);
	}

	private void RefreshBuildingButtonIconAndColors(bool buttonAvailable)
	{
		if (this.sprite == null)
		{
			this.sprite = PlanScreen.Instance.defaultBuildingIconSprite;
		}
		this.buildingIcon.sprite = this.sprite;
		this.buildingIcon.SetNativeSize();
		float num = (ScreenResolutionMonitor.UsingGamepadUIMode() ? 3.25f : 4f);
		this.buildingIcon.rectTransform().sizeDelta /= num;
		Material material = (buttonAvailable ? PlanScreen.Instance.defaultUIMaterial : PlanScreen.Instance.desaturatedUIMaterial);
		if (this.buildingIcon.material != material)
		{
			this.buildingIcon.material = material;
			if (!buttonAvailable)
			{
				if (this.researchComplete)
				{
					this.buildingIcon.color = new Color(1f, 1f, 1f, 0.6f);
					return;
				}
				this.buildingIcon.color = new Color(1f, 1f, 1f, 0.15f);
				return;
			}
			else
			{
				this.buildingIcon.color = Color.white;
			}
		}
	}

	private void RefreshFG(PlanScreen.RequirementsState requirementsState)
	{
		if (requirementsState == PlanScreen.RequirementsState.Tech)
		{
			this.fgImage.sprite = PlanScreen.Instance.Overlay_NeedTech;
			this.fgImage.gameObject.SetActive(true);
		}
		else
		{
			this.fgImage.gameObject.SetActive(false);
		}
		string tooltipForRequirementsState = PlanScreen.GetTooltipForRequirementsState(this.def, requirementsState);
		if (tooltipForRequirementsState != null)
		{
			this.tooltip.AddMultiStringTooltip("\n", PlanScreen.Instance.buildingToolTipSettings.ResearchRequirement);
			this.tooltip.AddMultiStringTooltip(tooltipForRequirementsState, PlanScreen.Instance.buildingToolTipSettings.ResearchRequirement);
		}
	}

	private BuildingDef def;

	private HashedString buildingCategory;

	private TechItem techItem;

	private List<int> gameSubscriptions = new List<int>();

	private bool researchComplete;

	private Sprite sprite;

	[SerializeField]
	private ToolTip tooltip;

	[SerializeField]
	private LocText text;

	[SerializeField]
	private ImageToggleState imageToggleState;

	[SerializeField]
	private Image buildingIcon;

	[SerializeField]
	private Image fgIcon;
}
