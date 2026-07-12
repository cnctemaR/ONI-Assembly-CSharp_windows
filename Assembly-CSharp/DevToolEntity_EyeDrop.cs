using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ImGuiNET;
using UnityEngine;
using UnityEngine.EventSystems;

public class DevToolEntity_EyeDrop : DevTool
{
	public DevToolEntity_EyeDrop(Action<DevToolEntityTarget> onSelectionMadeFn, Func<DevToolEntityTarget, Option<string>> getErrorForCandidateTargetFn = null)
	{
		this.onSelectionMadeFn = onSelectionMadeFn;
		this.getErrorForCandidateTargetFn = getErrorForCandidateTargetFn;
	}

	protected override void RenderTo(DevPanel panel)
	{
		if (this.requestingNavBack)
		{
			this.requestingNavBack = false;
			panel.NavGoBack();
			return;
		}
		if (ImGuiEx.BeginHelpMarker())
		{
			ImGui.TextWrapped("This will do a raycast check against:");
			ImGui.Bullet();
			ImGui.SameLine();
			ImGui.TextWrapped("world gameobjects that have a KCollider2D component");
			ImGui.Bullet();
			ImGui.SameLine();
			ImGui.TextWrapped("ui gameobjects with a Graphic component that also have `raycastTarget` set to true");
			ImGui.Bullet();
			ImGui.SameLine();
			ImGui.TextWrapped("world sim cells");
			ImGui.TextWrapped("This means that some gameobjects that can be seen will not show up here.");
			ImGuiEx.EndHelpMarker();
		}
		ImGui.Separator();
		DevToolEntity_EyeDrop.ImGuiInput_SampleScreenPosition(ref this.sampleAtScreenPosition);
		using (ListPool<DevToolEntityTarget, DevToolEntity_EyeDrop>.PooledList pooledList = PoolsFor<DevToolEntity_EyeDrop>.AllocateList<DevToolEntityTarget>())
		{
			Option<string> option = DevToolEntity_EyeDrop.CollectUIGameObjectHitsTo(pooledList, this.sampleAtScreenPosition);
			Option<string> option2 = DevToolEntity_EyeDrop.CollectWorldGameObjectHitsTo(pooledList, this.sampleAtScreenPosition);
			ValueTuple<Option<DevToolEntityTarget.ForSimCell>, Option<string>> simCellAt = DevToolEntity_EyeDrop.GetSimCellAt(this.sampleAtScreenPosition);
			Option<DevToolEntityTarget.ForSimCell> item = simCellAt.Item1;
			Option<string> item2 = simCellAt.Item2;
			if (item.IsSome())
			{
				pooledList.Add(item.Unwrap());
			}
			if (ImGui.TreeNode("Debug Info"))
			{
				DevToolEntity_EyeDrop.<RenderTo>g__DrawBullet|5_0("[UI GameObjects]", option);
				DevToolEntity_EyeDrop.<RenderTo>g__DrawBullet|5_0("[World GameObjects]", option2);
				DevToolEntity_EyeDrop.<RenderTo>g__DrawBullet|5_0("[Sim Cell]", item2);
				ImGui.TreePop();
			}
			ImGui.Separator();
			foreach (DevToolEntityTarget devToolEntityTarget in pooledList)
			{
				Option<string> option3 = ((this.getErrorForCandidateTargetFn == null) ? Option.None : this.getErrorForCandidateTargetFn(devToolEntityTarget));
				Option<ValueTuple<Vector2, Vector2>> screenRect = devToolEntityTarget.GetScreenRect();
				bool flag = ImGuiEx.Button("Pick target \"" + devToolEntityTarget.GetDebugName() + "\"", option3.IsNone());
				bool flag2 = ImGui.IsItemHovered();
				if (flag2)
				{
					ImGui.BeginTooltip();
					if (option3.IsSome())
					{
						ImGui.Text("Error:");
						ImGui.Text(option3.Unwrap());
						if (screenRect.IsSome())
						{
							ImGui.Separator();
							ImGui.Separator();
						}
					}
					if (screenRect.IsNone())
					{
						ImGui.Text("Error: Couldn't get screen rect to display.");
					}
					ImGui.EndTooltip();
				}
				if (flag)
				{
					this.onSelectionMadeFn(devToolEntityTarget);
					this.requestingNavBack = true;
				}
				if (screenRect.IsSome())
				{
					DevToolEntity.DrawBoundingBox(screenRect.Unwrap(), devToolEntityTarget.GetDebugName(), flag2);
				}
			}
		}
	}

	public static Option<string> CollectUIGameObjectHitsTo(IList<DevToolEntityTarget> targets, Vector3 screenPosition)
	{
		using (ListPool<RaycastResult, DevToolEntity_EyeDrop>.PooledList pooledList = PoolsFor<DevToolEntity_EyeDrop>.AllocateList<RaycastResult>())
		{
			global::UnityEngine.EventSystems.EventSystem current = global::UnityEngine.EventSystems.EventSystem.current;
			if (current.IsNullOrDestroyed())
			{
				return "No EventSystem found.";
			}
			current.RaycastAll(new PointerEventData(current)
			{
				position = screenPosition
			}, pooledList);
			foreach (RaycastResult raycastResult in pooledList)
			{
				if (!(raycastResult.gameObject.name == "ImGui Consume Input"))
				{
					targets.Add(new DevToolEntityTarget.ForUIGameObject(raycastResult.gameObject));
				}
			}
		}
		return Option.None;
	}

	public static Option<string> CollectWorldGameObjectHitsTo(IList<DevToolEntityTarget> targets, Vector3 screenPosition)
	{
		Camera main = Camera.main;
		if (main.IsNullOrDestroyed())
		{
			return "No Main Camera found.";
		}
		ValueTuple<Option<DevToolEntityTarget.ForSimCell>, Option<string>> simCellAt = DevToolEntity_EyeDrop.GetSimCellAt(screenPosition);
		Option<DevToolEntityTarget.ForSimCell> item = simCellAt.Item1;
		Option<string> item2 = simCellAt.Item2;
		if (item2.IsSome())
		{
			return item2;
		}
		if (item.IsNone())
		{
			return "Couldn't find sim cell";
		}
		DevToolEntityTarget.ForSimCell forSimCell = item.Unwrap();
		Vector2 vector = main.ScreenToWorldPoint(screenPosition);
		using (ListPool<InterfaceTool.Intersection, DevToolEntity_EyeDrop>.PooledList pooledList = PoolsFor<DevToolEntity_EyeDrop>.AllocateList<InterfaceTool.Intersection>())
		{
			using (ListPool<ScenePartitionerEntry, DevToolEntity_EyeDrop>.PooledList pooledList2 = PoolsFor<DevToolEntity_EyeDrop>.AllocateList<ScenePartitionerEntry>())
			{
				int num;
				int num2;
				Grid.CellToXY(forSimCell.cellIndex, out num, out num2);
				Game.Instance.statusItemRenderer.GetIntersections(vector, pooledList);
				GameScenePartitioner.Instance.GatherEntries(num, num2, 1, 1, GameScenePartitioner.Instance.collisionLayer, pooledList2);
				foreach (ScenePartitionerEntry scenePartitionerEntry in pooledList2)
				{
					KCollider2D kcollider2D = scenePartitionerEntry.obj as KCollider2D;
					if (!kcollider2D.IsNullOrDestroyed() && kcollider2D.Intersects(vector) && !(kcollider2D.gameObject.name == "WorldSelectionCollider"))
					{
						targets.Add(new DevToolEntityTarget.ForWorldGameObject(kcollider2D.gameObject));
					}
				}
			}
		}
		return Option.None;
	}

	[return: TupleElementNames(new string[] { "target", "error" })]
	public static ValueTuple<Option<DevToolEntityTarget.ForSimCell>, Option<string>> GetSimCellAt(Vector3 screenPosition)
	{
		if (Game.Instance == null)
		{
			return new ValueTuple<Option<DevToolEntityTarget.ForSimCell>, Option<string>>(Option.None, "No Game instance found.");
		}
		Camera main = Camera.main;
		if (main.IsNullOrDestroyed())
		{
			return new ValueTuple<Option<DevToolEntityTarget.ForSimCell>, Option<string>>(Option.None, "No Main Camera found.");
		}
		Ray ray = main.ScreenPointToRay(screenPosition);
		float num;
		if (!new Plane(new Vector3(0f, 0f, -1f), new Vector3(0f, 0f, 1f)).Raycast(ray, out num))
		{
			return new ValueTuple<Option<DevToolEntityTarget.ForSimCell>, Option<string>>(Option.None, "Ray from camera did not hit game plane.");
		}
		int num2 = Grid.PosToCell(ray.GetPoint(num));
		if (num2 < 0 || Grid.CellCount <= num2)
		{
			return new ValueTuple<Option<DevToolEntityTarget.ForSimCell>, Option<string>>(Option.None, string.Format("Found cell index {0} is out of range {1}..{2}", num2, num2, Grid.CellCount));
		}
		if (!Grid.IsValidCell(num2))
		{
			return new ValueTuple<Option<DevToolEntityTarget.ForSimCell>, Option<string>>(Option.None, string.Format("Cell index {0} is invalid", num2));
		}
		return new ValueTuple<Option<DevToolEntityTarget.ForSimCell>, Option<string>>(new DevToolEntityTarget.ForSimCell(num2), Option.None);
	}

	public static void ImGuiInput_SampleScreenPosition(ref Vector2 unityScreenPosition)
	{
		float num = 4f;
		float num2 = 12f;
		float num3 = 4f;
		float num4 = 6f;
		float num5 = 2f;
		float num6 = num + num2 + num3;
		float num7 = num + num2 + num3 + num5 + num4;
		float num8 = num + 4f;
		Vector2 vector = Vector2.one * num * 2f;
		Vector2 vector2 = Vector2.one * num6 * 2f;
		Vector2 vector3 = Vector2.one * (num6 + num4) * 2f;
		Vector2 vector4 = Vector2.one * num7 * 2f;
		ImGuiWindowFlags imGuiWindowFlags = ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.HorizontalScrollbar;
		Vector2 mousePos = ImGui.GetMousePos();
		Vector2 vector5 = DevToolEntity_EyeDrop.posSampler_rectBasePos;
		if (DevToolEntity_EyeDrop.posSampler_dragStartPos.IsSome())
		{
			vector5 += mousePos - DevToolEntity_EyeDrop.posSampler_dragStartPos.Unwrap();
		}
		ImGui.SetNextWindowPos(vector5 - vector4 / 2f);
		ImGui.SetNextWindowSizeConstraints(Vector2.one, Vector2.one * -1f);
		ImGui.SetNextWindowSize(vector4);
		ImGui.PushStyleVar(ImGuiStyleVar.WindowMinSize, Vector2.zero);
		ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.zero);
		if (ImGui.Begin("###ID_EyeDropper", imGuiWindowFlags))
		{
			bool flag = ImGui.IsWindowHovered();
			bool flag2 = ImGui.IsWindowHovered() && ImGui.IsMouseDown(ImGuiMouseButton.Left);
			Color color;
			if (flag2)
			{
				color = Util.ColorFromHex("C5153B");
			}
			else if (flag)
			{
				color = Util.ColorFromHex("F498AC");
			}
			else
			{
				color = Util.ColorFromHex("EC4F71");
			}
			if (flag2 && DevToolEntity_EyeDrop.posSampler_dragStartPos.IsNone())
			{
				DevToolEntity_EyeDrop.posSampler_dragStartPos = mousePos;
			}
			if (ImGui.IsMouseReleased(ImGuiMouseButton.Left) && DevToolEntity_EyeDrop.posSampler_dragStartPos.IsSome())
			{
				DevToolEntity_EyeDrop.posSampler_rectBasePos += mousePos - DevToolEntity_EyeDrop.posSampler_dragStartPos.Unwrap();
				DevToolEntity_EyeDrop.posSampler_dragStartPos = Option.None;
			}
			ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
			Vector2 vector6 = ImGui.GetCursorScreenPos() + Vector2.one * num5;
			Vector2 vector7 = Vector2.one * num4;
			Vector2 vector8 = (vector2 - vector) / 2f + vector7;
			unityScreenPosition = new Vector2(vector6.x + vector8.x + num, -(vector6.y + vector8.y + num) + (float)Screen.height);
			windowDrawList.AddRectFilled(vector6, vector6 + vector3, ImGui.GetColorU32(new Vector4(0f, 0f, 0f, 0.7f)), num8);
			windowDrawList.AddRectFilled(vector6 + vector8, vector6 + vector8 + vector, ImGui.GetColorU32(color), num8);
			windowDrawList.AddRect(vector6 + vector7, vector6 + vector7 + vector2, ImGui.GetColorU32(color), num8, ImDrawFlags.None, num3);
			ImGui.End();
		}
		ImGui.PopStyleVar(2);
	}

	[CompilerGenerated]
	internal static void <RenderTo>g__DrawBullet|5_0(string groupName, Option<string> error)
	{
		ImGui.Bullet();
		ImGui.Text(groupName);
		ImGui.SameLine();
		if (error.IsSome())
		{
			ImGui.Text("[ERROR]");
			ImGui.SameLine();
			ImGui.Text(error.Unwrap());
			return;
		}
		ImGui.Text("No errors.");
	}

	private Vector2 sampleAtScreenPosition;

	private Action<DevToolEntityTarget> onSelectionMadeFn;

	private Func<DevToolEntityTarget, Option<string>> getErrorForCandidateTargetFn;

	private bool requestingNavBack;

	private static Vector2 posSampler_rectBasePos = new Vector2(200f, 200f);

	private static Option<Vector2> posSampler_dragStartPos = Option.None;
}
