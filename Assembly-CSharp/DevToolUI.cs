using System;
using System.Collections.Generic;
using ImGuiNET;
using UnityEngine;
using UnityEngine.EventSystems;

public class DevToolUI : DevTool
{
	protected override void RenderTo(DevPanel panel)
	{
		this.RepopulateRaycastHits();
		this.DrawPingObject();
		this.DrawRaycastHits();
	}

	private void DrawPingObject()
	{
		if (this.m_last_pinged_hit != null)
		{
			GameObject gameObject = this.m_last_pinged_hit.Value.gameObject;
			if (gameObject != null && gameObject)
			{
				ImGui.Text("Last Pinged: \"" + DevToolUI.GetQualifiedName(gameObject) + "\"");
				ImGui.SameLine();
				if (ImGui.Button("Inspect"))
				{
					DevToolSceneInspector.Inspect(gameObject);
				}
				ImGui.Spacing();
				ImGui.Spacing();
			}
			else
			{
				this.m_last_pinged_hit = null;
			}
		}
		ImGui.Text("Press \",\" to ping the top hovered ui object");
		ImGui.Spacing();
		ImGui.Spacing();
	}

	private void Internal_Ping(RaycastResult raycastResult)
	{
		GameObject gameObject = raycastResult.gameObject;
		this.m_last_pinged_hit = new RaycastResult?(raycastResult);
	}

	public static void PingHoveredObject()
	{
		using (ListPool<RaycastResult, DevToolUI>.PooledList pooledList = PoolsFor<DevToolUI>.AllocateList<RaycastResult>())
		{
			global::UnityEngine.EventSystems.EventSystem current = global::UnityEngine.EventSystems.EventSystem.current;
			if (!(current == null) && current)
			{
				current.RaycastAll(new PointerEventData(current)
				{
					position = Input.mousePosition
				}, pooledList);
				DevToolUI devToolUI = DevToolManager.Instance.panels.AddOrGetDevTool<DevToolUI>();
				if (pooledList.Count > 0)
				{
					devToolUI.Internal_Ping(pooledList[0]);
				}
			}
		}
	}

	private void DrawRaycastHits()
	{
		if (this.m_raycast_hits.Count <= 0)
		{
			ImGui.Text("Didn't hit any ui");
			return;
		}
		ImGui.Text("Raycast Hits:");
		ImGui.Indent();
		for (int i = 0; i < this.m_raycast_hits.Count; i++)
		{
			RaycastResult raycastResult = this.m_raycast_hits[i];
			ImGui.BulletText(string.Format("[{0}] {1}", i, DevToolUI.GetQualifiedName(raycastResult.gameObject)));
		}
		ImGui.Unindent();
	}

	private void RepopulateRaycastHits()
	{
		this.m_raycast_hits.Clear();
		global::UnityEngine.EventSystems.EventSystem current = global::UnityEngine.EventSystems.EventSystem.current;
		if (current == null || !current)
		{
			return;
		}
		current.RaycastAll(new PointerEventData(current)
		{
			position = Input.mousePosition
		}, this.m_raycast_hits);
	}

	private static string GetQualifiedName(GameObject game_object)
	{
		KScreen componentInParent = game_object.GetComponentInParent<KScreen>();
		if (componentInParent != null)
		{
			return componentInParent.gameObject.name + " :: " + game_object.name;
		}
		return game_object.name ?? "";
	}

	private List<RaycastResult> m_raycast_hits = new List<RaycastResult>();

	private RaycastResult? m_last_pinged_hit;
}
