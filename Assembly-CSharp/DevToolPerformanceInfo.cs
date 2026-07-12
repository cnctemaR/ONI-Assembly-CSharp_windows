using System;
using System.Collections.Generic;
using ImGuiNET;
using UnityEngine;

public class DevToolPerformanceInfo : DevTool
{
	protected override void RenderTo(DevPanel panel)
	{
		if (Game.Instance == null)
		{
			ImGui.Text("No game loaded");
			return;
		}
		if (this.performanceMonitor == null)
		{
			this.performanceMonitor = Global.Instance.GetComponent<PerformanceMonitor>();
		}
		float fps = this.performanceMonitor.FPS;
		float num = 1000f / fps;
		ImGui.Text(string.Format("{0:0.00} ms ({1:0.00} fps)", num, fps));
		ImGui.NewLine();
		ImGui.Separator();
		if (ImGui.CollapsingHeader("Brains") && Game.BrainScheduler.debugGetBrainGroups() != null)
		{
			List<BrainScheduler.BrainGroup> list = Game.BrainScheduler.debugGetBrainGroups();
			for (int i = 0; i < list.Count; i++)
			{
				BrainScheduler.BrainGroup brainGroup = list[i];
				ImGui.Text(brainGroup.tag.ToString());
				ImGui.Indent();
				ImGui.Text("Brain count: " + brainGroup.BrainCount.ToString());
				ImGui.Text("probeSize: " + brainGroup.probeSize.ToString());
				ImGui.Text("probeCount: " + brainGroup.probeCount.ToString());
				ImGui.PushID(i);
				ImGui.Checkbox("Freeze AdjustLoad", ref brainGroup.debugFreezeLoadAdustment);
				ImGui.PopID();
				ImGui.SameLine();
				if (ImGui.Button("Reset probe size/count"))
				{
					brainGroup.ResetLoad();
				}
				ImGui.Text("Max priority brain count seen: " + brainGroup.debugMaxPriorityBrainCountSeen.ToString());
				ImGui.SameLine();
				if (ImGui.Button("Reset"))
				{
					brainGroup.debugMaxPriorityBrainCountSeen = 0;
				}
				ImGui.Unindent();
			}
		}
		if (ImGui.CollapsingHeader("Camera Culling"))
		{
			if (CameraController.Instance == null)
			{
				ImGui.Text("No camera instance");
				return;
			}
			GridVisibleArea visibleArea = CameraController.Instance.VisibleArea;
			ImGui.Checkbox("Freeze visible area", ref visibleArea.debugFreezeVisibleArea);
			ImGui.Checkbox("Freeze visible area extended", ref visibleArea.debugFreezeVisibleAreasExtended);
			Vector2I vector2I = visibleArea.CurrentArea.Min;
			Vector2I vector2I2 = visibleArea.CurrentArea.Max;
			Option<ValueTuple<Vector2, Vector2>> option = new DevToolEntityTarget.ForSimCell(Grid.XYToCell(vector2I.x, vector2I.y)).GetScreenRect();
			Option<ValueTuple<Vector2, Vector2>> option2 = new DevToolEntityTarget.ForSimCell(Grid.XYToCell(vector2I2.x, vector2I2.y)).GetScreenRect();
			if (option.IsSome() && option2.IsSome())
			{
				DevToolEntity.DrawScreenRect(new ValueTuple<Vector2, Vector2>(Vector2.Min(option.Unwrap().Item1, Vector2.Min(option.Unwrap().Item2, Vector2.Min(option2.Unwrap().Item1, option2.Unwrap().Item2))), Vector2.Max(option.Unwrap().Item1, Vector2.Max(option.Unwrap().Item2, Vector2.Max(option2.Unwrap().Item1, option2.Unwrap().Item2)))), "", new Option<Color>(Color.red), default(Option<Color>), default(Option<DevToolUtil.TextAlignment>));
			}
			vector2I = visibleArea.CurrentAreaExtended.Min;
			vector2I2 = visibleArea.CurrentAreaExtended.Max;
			option = new DevToolEntityTarget.ForSimCell(Grid.XYToCell(vector2I.x, vector2I.y)).GetScreenRect();
			option2 = new DevToolEntityTarget.ForSimCell(Grid.XYToCell(vector2I2.x, vector2I2.y)).GetScreenRect();
			if (option.IsSome() && option2.IsSome())
			{
				DevToolEntity.DrawScreenRect(new ValueTuple<Vector2, Vector2>(Vector2.Min(option.Unwrap().Item1, Vector2.Min(option.Unwrap().Item2, Vector2.Min(option2.Unwrap().Item1, option2.Unwrap().Item2))), Vector2.Max(option.Unwrap().Item1, Vector2.Max(option.Unwrap().Item2, Vector2.Max(option2.Unwrap().Item1, option2.Unwrap().Item2)))), "", new Option<Color>(Color.cyan), default(Option<Color>), default(Option<DevToolUtil.TextAlignment>));
			}
		}
	}

	private PerformanceMonitor performanceMonitor;
}
