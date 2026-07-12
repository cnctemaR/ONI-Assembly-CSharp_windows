using System;
using System.Runtime.CompilerServices;
using ImGuiNET;
using STRINGS;
using UnityEngine;

public class DevToolEntity : DevTool
{
	protected override void RenderTo(DevPanel panel)
	{
		if (ImGui.BeginMenuBar())
		{
			if (ImGui.MenuItem("New Window"))
			{
				DevToolUtil.Open(new DevToolEntity());
			}
			ImGui.EndMenuBar();
		}
		ImGui.Text(this.currentTargetOpt.IsNone() ? "Pick target:" : "Change target:");
		if (ImGui.Button("Eyedrop"))
		{
			panel.PushDevTool(new DevToolEntity_EyeDrop(delegate(DevToolEntityTarget result)
			{
				this.currentTargetOpt = result;
			}, null));
		}
		ImGui.SameLine();
		if (ImGui.Button("Search GameObjects (NOT implemented)"))
		{
			panel.PushDevTool(new DevToolEntity_SearchGameObjects(delegate(DevToolEntityTarget result)
			{
				this.currentTargetOpt = result;
			}));
		}
		if (this.GetInGameSelectedEntity().IsSome())
		{
			ImGui.SameLine();
			if (ImGui.Button("\"" + this.GetInGameSelectedEntity().Unwrap().name + "\""))
			{
				this.currentTargetOpt = new DevToolEntityTarget.ForWorldGameObject(this.GetInGameSelectedEntity().Unwrap());
			}
		}
		ImGui.Separator();
		ImGui.Separator();
		ImGui.Spacing();
		if (this.currentTargetOpt.IsNone())
		{
			ImGui.Text("<nothing selected>");
			this.Name = "Entity";
		}
		else
		{
			ImGui.Checkbox("Draw Bounding Box", ref this.shouldDrawBoundingBox);
			ImGuiEx.SimpleField("Entity Name", this.currentTargetOpt.Unwrap().ToString());
			this.Name = "Entity: " + this.currentTargetOpt.Unwrap().ToString();
		}
		ImGui.Spacing();
		if (this.currentTargetOpt.IsNone())
		{
			return;
		}
		DevToolEntityTarget devToolEntityTarget = this.currentTargetOpt.Unwrap();
		DevToolEntityTarget.ForUIGameObject forUIGameObject = devToolEntityTarget as DevToolEntityTarget.ForUIGameObject;
		Option<GameObject> option;
		if (forUIGameObject != null)
		{
			option = forUIGameObject.gameObject;
		}
		else
		{
			DevToolEntityTarget.ForWorldGameObject forWorldGameObject = devToolEntityTarget as DevToolEntityTarget.ForWorldGameObject;
			if (forWorldGameObject != null)
			{
				option = forWorldGameObject.gameObject;
			}
			else
			{
				option = Option.None;
			}
		}
		if (option.IsSome() && ImGui.Button(string.Format("DevTool Inspect###ID_Inspect_{0}", option.Unwrap().GetInstanceID())))
		{
			DevToolSceneInspector.Inspect(option.Unwrap());
		}
		ImGui.Separator();
		if (ImGuiEx.Button("Debug Status Items", DevToolStatusItems.GetErrorForCandidateTarget(devToolEntityTarget).UnwrapOrDefault()))
		{
			panel.PushDevTool(new DevToolStatusItems((DevToolEntityTarget.ForWorldGameObject)devToolEntityTarget));
		}
		if (ImGuiEx.Button("Debug Cavity", DevToolCavity.GetErrorForCandidateTarget(devToolEntityTarget).UnwrapOrDefault()))
		{
			panel.PushDevTool(new DevToolCavity((DevToolEntityTarget.ForSimCell)devToolEntityTarget));
		}
		if (ImGuiEx.Button("Debug GoTo", DevToolEntity_DebugGoTo.GetErrorForCandidateTarget(devToolEntityTarget).UnwrapOrDefault()))
		{
			panel.PushDevTool(new DevToolEntity_DebugGoTo((DevToolEntityTarget.ForWorldGameObject)devToolEntityTarget));
		}
		if (ImGuiEx.Button("Debug RanchStation", DevToolEntity_RanchStation.GetErrorForCandidateTarget(devToolEntityTarget).UnwrapOrDefault()))
		{
			panel.PushDevTool(new DevToolEntity_RanchStation((DevToolEntityTarget.ForWorldGameObject)devToolEntityTarget));
		}
		if (this.shouldDrawBoundingBox)
		{
			Option<ValueTuple<Vector2, Vector2>> screenRect = devToolEntityTarget.GetScreenRect();
			if (screenRect.IsSome())
			{
				DevToolEntity.DrawBoundingBox(screenRect.Unwrap(), devToolEntityTarget.GetDebugName(), ImGui.IsWindowFocused());
			}
		}
	}

	public Option<GameObject> GetInGameSelectedEntity()
	{
		if (SelectTool.Instance == null)
		{
			return Option.None;
		}
		KSelectable selected = SelectTool.Instance.selected;
		if (selected.IsNullOrDestroyed())
		{
			return Option.None;
		}
		return selected.gameObject;
	}

	public static string GetNameFor(GameObject gameObject)
	{
		if (gameObject.IsNullOrDestroyed())
		{
			return "<null or destroyed GameObject>";
		}
		return string.Concat(new string[]
		{
			"\"",
			UI.StripLinkFormatting(gameObject.name),
			"\" [0x",
			gameObject.GetInstanceID().ToString("X"),
			"]"
		});
	}

	public static void DrawBoundingBox([TupleElementNames(new string[] { "cornerA", "cornerB" })] ValueTuple<Vector2, Vector2> screenRect, string name, bool isFocused)
	{
		if (isFocused)
		{
			DevToolEntity.DrawScreenRect(screenRect, name, new Color(1f, 0f, 0f, 1f), new Color(1f, 0f, 0f, 0.3f));
			return;
		}
		DevToolEntity.DrawScreenRect(screenRect, Option.None, new Color(0.9f, 0f, 0f, 0.6f), default(Option<Color>));
	}

	public static void DrawScreenRect([TupleElementNames(new string[] { "cornerA", "cornerB" })] ValueTuple<Vector2, Vector2> screenRect, Option<string> text = default(Option<string>), Option<Color> outlineColor = default(Option<Color>), Option<Color> fillColor = default(Option<Color>))
	{
		Vector2 vector = Vector2.Min(screenRect.Item1, screenRect.Item2);
		Vector2 vector2 = Vector2.Max(screenRect.Item1, screenRect.Item2);
		ImGui.GetBackgroundDrawList().AddRect(vector, vector2, ImGui.GetColorU32(outlineColor.UnwrapOr(Color.red, null)), 0f, ImDrawFlags.None, 4f);
		ImGui.GetBackgroundDrawList().AddRectFilled(vector, vector2, ImGui.GetColorU32(fillColor.UnwrapOr(Color.clear, null)));
		if (text.IsSome())
		{
			ImGui.GetBackgroundDrawList().AddText(ImGui.GetFont(), 30f, new Vector2(vector2.x, vector.y) + new Vector2(15f, 0f), ImGui.GetColorU32(Color.white), text.Unwrap());
		}
	}

	private Option<DevToolEntityTarget> currentTargetOpt;

	private bool shouldDrawBoundingBox = true;
}
