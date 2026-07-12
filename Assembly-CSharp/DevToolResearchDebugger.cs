using System;
using System.Collections.Generic;
using ImGuiNET;
using UnityEngine;

public class DevToolResearchDebugger : DevTool
{
	public DevToolResearchDebugger()
	{
		this.RequiresGameRunning = true;
	}

	protected override void RenderTo(DevPanel panel)
	{
		TechInstance activeResearch = Research.Instance.GetActiveResearch();
		if (activeResearch == null)
		{
			ImGui.Text("No Active Research");
			return;
		}
		ImGui.Text("Active Research");
		ImGui.Text("ID: " + activeResearch.tech.Id);
		ImGui.Text("Name: " + Util.StripTextFormatting(activeResearch.tech.Name));
		ImGui.Separator();
		ImGui.Text("Active Research Inventory");
		foreach (KeyValuePair<string, float> keyValuePair in new Dictionary<string, float>(activeResearch.progressInventory.PointsByTypeID))
		{
			if (activeResearch.tech.RequiresResearchType(keyValuePair.Key))
			{
				float num = activeResearch.tech.costsByResearchTypeID[keyValuePair.Key];
				float num2 = keyValuePair.Value;
				if (ImGui.Button("Fill"))
				{
					num2 = num;
				}
				ImGui.SameLine();
				ImGui.SetNextItemWidth(100f);
				ImGui.InputFloat(keyValuePair.Key, ref num2, 1f, 10f);
				ImGui.SameLine();
				ImGui.Text(string.Format("of {0}", num));
				activeResearch.progressInventory.PointsByTypeID[keyValuePair.Key] = Mathf.Clamp(num2, 0f, num);
			}
		}
		ImGui.Separator();
		ImGui.Text("Global Points Inventory");
		foreach (KeyValuePair<string, float> keyValuePair2 in Research.Instance.globalPointInventory.PointsByTypeID)
		{
			ImGui.Text(keyValuePair2.Key + ": " + keyValuePair2.Value.ToString());
		}
	}
}
