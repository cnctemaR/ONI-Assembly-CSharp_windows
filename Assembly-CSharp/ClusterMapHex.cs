using System;
using System.Collections.Generic;
using System.Linq;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class ClusterMapHex : MultiToggle, ICanvasRaycastFilter
{
	public AxialI location { get; private set; }

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.rectTransform = base.GetComponent<RectTransform>();
		this.onClick = new global::System.Action(this.TrySelect);
		this.onDoubleClick = new Func<bool>(this.TryGoTo);
		this.onEnter = new global::System.Action(this.OnHover);
		this.onExit = new global::System.Action(this.OnUnhover);
	}

	public void SetLocation(AxialI location)
	{
		this.location = location;
	}

	public void SetRevealed(ClusterRevealLevel level)
	{
		this._revealLevel = level;
		switch (level)
		{
		case ClusterRevealLevel.Hidden:
			this.fogOfWar.gameObject.SetActive(true);
			this.peekedTile.gameObject.SetActive(false);
			return;
		case ClusterRevealLevel.Peeked:
			this.fogOfWar.gameObject.SetActive(false);
			this.peekedTile.gameObject.SetActive(true);
			return;
		case ClusterRevealLevel.Visible:
			this.fogOfWar.gameObject.SetActive(false);
			this.peekedTile.gameObject.SetActive(false);
			return;
		default:
			return;
		}
	}

	public void SetDestinationStatus(string fail_reason)
	{
		this.m_tooltip.ClearMultiStringTooltip();
		this.UpdateHoverColors(string.IsNullOrEmpty(fail_reason));
		if (!string.IsNullOrEmpty(fail_reason))
		{
			this.m_tooltip.AddMultiStringTooltip(fail_reason, this.invalidDestinationTooltipStyle);
		}
	}

	public void SetDestinationStatus(string fail_reason, int pathLength, int rocketRange, bool repeat)
	{
		this.m_tooltip.ClearMultiStringTooltip();
		if (pathLength > 0)
		{
			string text = (repeat ? UI.CLUSTERMAP.TOOLTIP_PATH_LENGTH_RETURN : UI.CLUSTERMAP.TOOLTIP_PATH_LENGTH);
			if (repeat)
			{
				pathLength *= 2;
			}
			text = string.Format(text, pathLength, GameUtil.GetFormattedRocketRange((float)rocketRange, GameUtil.TimeSlice.None, true));
			this.m_tooltip.AddMultiStringTooltip(text, this.informationTooltipStyle);
		}
		this.UpdateHoverColors(string.IsNullOrEmpty(fail_reason));
		if (!string.IsNullOrEmpty(fail_reason))
		{
			this.m_tooltip.AddMultiStringTooltip(fail_reason, this.invalidDestinationTooltipStyle);
		}
	}

	public void UpdateToggleState(ClusterMapHex.ToggleState state)
	{
		int num = -1;
		switch (state)
		{
		case ClusterMapHex.ToggleState.Unselected:
			num = 0;
			break;
		case ClusterMapHex.ToggleState.Selected:
			num = 1;
			break;
		case ClusterMapHex.ToggleState.OrbitHighlight:
			num = 2;
			break;
		}
		base.ChangeState(num);
	}

	private void TrySelect()
	{
		if (DebugHandler.InstantBuildMode)
		{
			SaveGame.Instance.GetSMI<ClusterFogOfWarManager.Instance>().RevealLocation(this.location, 0);
		}
		ClusterMapScreen.Instance.SelectHex(this);
	}

	private bool TryGoTo()
	{
		List<WorldContainer> list = (from entity in ClusterGrid.Instance.GetVisibleEntitiesAtCell(this.location)
			select entity.GetComponent<WorldContainer>() into x
			where x != null
			select x).ToList<WorldContainer>();
		if (list.Count == 1)
		{
			CameraController.Instance.ActiveWorldStarWipe(list[0].id, null);
			return true;
		}
		return false;
	}

	private void OnHover()
	{
		this.m_tooltip.ClearMultiStringTooltip();
		string text = "";
		switch (this._revealLevel)
		{
		case ClusterRevealLevel.Hidden:
			text = UI.CLUSTERMAP.TOOLTIP_HIDDEN_HEX;
			break;
		case ClusterRevealLevel.Peeked:
			text = ((ClusterGrid.Instance.GetEntitiesOnCell(this.location).Count > 0) ? UI.CLUSTERMAP.TOOLTIP_PEEKED_HEX_WITH_OBJECT : UI.CLUSTERMAP.TOOLTIP_HIDDEN_HEX);
			break;
		case ClusterRevealLevel.Visible:
			if (ClusterGrid.Instance.GetEntitiesOnCell(this.location).Count == 0)
			{
				text = UI.CLUSTERMAP.TOOLTIP_EMPTY_HEX;
			}
			break;
		}
		if (!text.IsNullOrWhiteSpace())
		{
			this.m_tooltip.AddMultiStringTooltip(text, this.informationTooltipStyle);
		}
		this.UpdateHoverColors(true);
		ClusterMapScreen.Instance.OnHoverHex(this);
	}

	private void OnUnhover()
	{
		ClusterMapScreen.Instance.OnUnhoverHex(this);
	}

	private void UpdateHoverColors(bool validDestination)
	{
		Color color = (validDestination ? this.hoverColorValid : this.hoverColorInvalid);
		for (int i = 0; i < this.states.Length; i++)
		{
			this.states[i].color_on_hover = color;
			for (int j = 0; j < this.states[i].additional_display_settings.Length; j++)
			{
				this.states[i].additional_display_settings[j].color_on_hover = color;
			}
		}
		base.RefreshHoverColor();
	}

	public bool IsRaycastLocationValid(Vector2 inputPoint, Camera eventCamera)
	{
		Vector2 vector = this.rectTransform.position;
		float num = Mathf.Abs(inputPoint.x - vector.x);
		float num2 = Mathf.Abs(inputPoint.y - vector.y);
		Vector2 vector2 = this.rectTransform.lossyScale;
		return num <= vector2.x && num2 <= vector2.y && vector2.y * vector2.x - vector2.y / 2f * num - vector2.x * num2 >= 0f;
	}

	private RectTransform rectTransform;

	public Color hoverColorValid;

	public Color hoverColorInvalid;

	public Image fogOfWar;

	public Image peekedTile;

	public TextStyleSetting invalidDestinationTooltipStyle;

	public TextStyleSetting informationTooltipStyle;

	[MyCmpGet]
	private ToolTip m_tooltip;

	private ClusterRevealLevel _revealLevel;

	public enum ToggleState
	{
		Unselected,
		Selected,
		OrbitHighlight
	}
}
