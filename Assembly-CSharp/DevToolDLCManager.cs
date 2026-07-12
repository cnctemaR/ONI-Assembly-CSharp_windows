using System;
using ImGuiNET;

public class DevToolDLCManager : DevTool
{
	protected override void RenderTo(DevPanel panel)
	{
		string name = DistributionPlatform.Inst.Name;
		if (!DistributionPlatform.Initialized)
		{
			ImGui.Text("Failed to initialize " + name);
			return;
		}
		ImGui.Text("Active content letters: " + DlcManager.GetActiveContentLetters());
		ImGui.Separator();
		foreach (string text in DlcManager.RELEASED_VERSIONS)
		{
			if (!text.IsNullOrWhiteSpace())
			{
				ImGui.Text(text);
				ImGui.SameLine();
				bool flag = DlcManager.IsContentSubscribed(text);
				if (ImGui.Checkbox("Enabled ", ref flag))
				{
					DlcManager.ToggleDLC(text);
				}
				ImGui.SameLine();
				bool flag2 = DistributionPlatform.Inst.IsDLCSubscribed(text);
				if (ImGui.Checkbox("Subscribed ", ref flag2))
				{
					DistributionPlatform.Inst.ToggleDLCSubscription(text);
				}
			}
		}
	}
}
