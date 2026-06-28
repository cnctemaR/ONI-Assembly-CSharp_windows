using System;
using STRINGS;

public class RegionToolHoverTextCard : HoverTextConfiguration
{
	protected override void ConfigureTitle(HoverTextScreen screen)
	{
		this.TitleLine = screen.NewLine("title line", 24);
		this.TitleText = screen.AddText(Game.Instance.RegionManager.selectedRegionPrefab.HoverStr, this.ToolTitleTextStyle, true);
	}

	public override void UpdateHoverElements(KSelectable[] hoverObjects)
	{
		if (RegionTool.Instance.Dragging && RegionInterfaceScreen.Instance != null && RegionInterfaceScreen.Instance.DragHighlightedRegions != null)
		{
			this.ActionName = ((RegionInterfaceScreen.Instance.DragHighlightedRegions.Count != 0) ? UI.TOOLS.REGIONCATEGORY.MERGETOOLTIP : UI.TOOLS.REGIONCATEGORY.CREATETOOLTIP);
		}
		else
		{
			this.ActionName = ((!(RegionInterfaceScreen.Instance.RegionUnderCursor() == null)) ? UI.TOOLS.REGIONCATEGORY.MERGETOOLTIP : UI.TOOLS.REGIONCATEGORY.CREATETOOLTIP);
		}
		if (this.ActionText != null)
		{
			this.ActionText.text = this.ActionName.ToUpper();
		}
		base.UpdateHoverElements(hoverObjects);
	}
}
