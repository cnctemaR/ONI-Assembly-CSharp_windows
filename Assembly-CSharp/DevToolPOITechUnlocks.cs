using System;
using ImGuiNET;

public class DevToolPOITechUnlocks : DevTool
{
	protected override void RenderTo(DevPanel panel)
	{
		if (Research.Instance == null)
		{
			return;
		}
		foreach (TechItem techItem in Db.Get().TechItems.resources)
		{
			if (techItem.isPOIUnlock)
			{
				ImGui.Text(techItem.Id);
				ImGui.SameLine();
				bool flag = techItem.IsComplete();
				if (ImGui.Checkbox("Unlocked ", ref flag))
				{
					techItem.POIUnlocked();
				}
			}
		}
	}
}
