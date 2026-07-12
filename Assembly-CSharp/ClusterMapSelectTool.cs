using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClusterMapSelectTool : InterfaceTool
{
	public static void DestroyInstance()
	{
		ClusterMapSelectTool.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		ClusterMapSelectTool.Instance = this;
	}

	public void Activate()
	{
		PlayerController.Instance.ActivateTool(this);
		ToolMenu.Instance.PriorityScreen.ResetPriority();
		this.Select(null, false);
	}

	public KSelectable GetSelected()
	{
		return this.m_selected;
	}

	public override bool ShowHoverUI()
	{
		return ClusterMapScreen.Instance.HasCurrentHover();
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		base.OnDeactivateTool(new_tool);
		base.ClearHover();
		this.Select(null, false);
	}

	private void UpdateHoveredSelectables()
	{
		this.m_hoveredSelectables.Clear();
		if (ClusterMapScreen.Instance.HasCurrentHover())
		{
			AxialI currentHoverLocation = ClusterMapScreen.Instance.GetCurrentHoverLocation();
			List<KSelectable> list = (from entity in ClusterGrid.Instance.GetVisibleEntitiesAtCell(currentHoverLocation)
				select entity.GetComponent<KSelectable>() into selectable
				where selectable != null && selectable.IsSelectable
				select selectable).ToList<KSelectable>();
			this.m_hoveredSelectables.AddRange(list);
		}
	}

	public override void LateUpdate()
	{
		this.UpdateHoveredSelectables();
		KSelectable kselectable = ((this.m_hoveredSelectables.Count > 0) ? this.m_hoveredSelectables[0] : null);
		base.UpdateHoverElements(this.m_hoveredSelectables);
		if (!this.hasFocus)
		{
			base.ClearHover();
		}
		else if (kselectable != this.hover)
		{
			base.ClearHover();
			this.hover = kselectable;
			if (kselectable != null)
			{
				Game.Instance.Trigger(2095258329, kselectable.gameObject);
				kselectable.Hover(!this.playedSoundThisFrame);
				this.playedSoundThisFrame = true;
			}
		}
		this.playedSoundThisFrame = false;
	}

	public void SelectNextFrame(KSelectable new_selected, bool skipSound = false)
	{
		this.delayedNextSelection = new_selected;
		this.delayedSkipSound = skipSound;
		UIScheduler.Instance.ScheduleNextFrame("DelayedSelect", new Action<object>(this.DoSelectNextFrame), null, null);
	}

	private void DoSelectNextFrame(object data)
	{
		this.Select(this.delayedNextSelection, this.delayedSkipSound);
		this.delayedNextSelection = null;
	}

	public void Select(KSelectable new_selected, bool skipSound = false)
	{
		if (new_selected == this.m_selected)
		{
			return;
		}
		if (this.m_selected != null)
		{
			this.m_selected.Unselect();
		}
		GameObject gameObject = null;
		if (new_selected != null && new_selected.GetMyWorldId() == -1)
		{
			if (new_selected == this.hover)
			{
				base.ClearHover();
			}
			new_selected.Select();
			gameObject = new_selected.gameObject;
		}
		this.m_selected = ((gameObject == null) ? null : new_selected);
		Game.Instance.Trigger(-1503271301, gameObject);
	}

	private List<KSelectable> m_hoveredSelectables = new List<KSelectable>();

	private KSelectable m_selected;

	public static ClusterMapSelectTool Instance;

	private KSelectable delayedNextSelection;

	private bool delayedSkipSound;
}
