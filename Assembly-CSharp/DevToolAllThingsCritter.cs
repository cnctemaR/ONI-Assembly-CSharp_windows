using System;
using ImGuiNET;
using UnityEngine;

public class DevToolAllThingsCritter : DevTool
{
	protected override void RenderTo(DevPanel panel)
	{
		if (SelectTool.Instance.selected != null || this.lockObject != null)
		{
			this.Contents();
			return;
		}
		ImGui.Text("No Critter Selected");
	}

	private void Contents()
	{
		ImGui.Spacing();
		if (Camera.main != null && SelectTool.Instance != null)
		{
			GameObject gameObject = null;
			ImGui.Checkbox("Lock", ref this.follow);
			if (this.follow)
			{
				if (this.lockObject == null && SelectTool.Instance.selected != null && SelectTool.Instance.selected.GetComponent<KPrefabID>() != null && SelectTool.Instance.selected.HasTag(GameTags.Creature))
				{
					this.lockObject = SelectTool.Instance.selected.gameObject;
				}
				gameObject = this.lockObject;
			}
			else if (SelectTool.Instance.selected != null)
			{
				if (SelectTool.Instance.selected.GetComponent<KPrefabID>() != null && SelectTool.Instance.selected.HasTag(GameTags.Creature))
				{
					gameObject = SelectTool.Instance.selected.gameObject;
				}
				this.lockObject = null;
			}
			if (gameObject != null)
			{
				ImGuiEx.SimpleField("Name", DevToolEntity.GetNameFor(gameObject));
				Vector3 position = gameObject.transform.GetPosition();
				string text = string.Format("X={0:F2}, Y={1:F2}, Z={2:F2}", position.x, position.y, position.z);
				ImGuiEx.SimpleField("Position", text);
				ImGuiEx.SimpleField("Cell", Grid.PosToCell(gameObject));
				this.NavigatorContents(position, gameObject);
				this.OccupyAreaContents(gameObject);
				this.ColliderContents(gameObject);
				this.CritterTemperatureMonitorContents(gameObject);
			}
		}
	}

	private void NavigatorContents(Vector3 pos, GameObject go)
	{
		Navigator component = go.GetComponent<Navigator>();
		if (component != null && ImGui.CollapsingHeader("Navigator", ImGuiTreeNodeFlags.DefaultOpen))
		{
			ImGui.Checkbox("Draw", ref this.drawNavDots);
			Vector2 positionFor = DevToolEntity.GetPositionFor(component.gameObject);
			string text = string.Format("X={0:F2}, Y={1:F2}", pos.x, pos.y);
			ImGui.TextColored(Color.green, "World: " + text);
			if (this.drawNavDots)
			{
				ImGui.GetBackgroundDrawList().AddCircleFilled(positionFor, 10f, ImGui.GetColorU32(Color.green));
			}
			Vector2 vector = component.GetComponent<KBatchedAnimController>().GetPivotSymbolPosition();
			Vector2 screenPosition = DevToolEntity.GetScreenPosition(vector);
			string text2 = string.Format("X={0:F2}, Y={1:F2}", vector.x, vector.y);
			ImGui.TextColored(Color.blue, "Pivot: " + text2);
			if (this.drawNavDots)
			{
				ImGui.GetBackgroundDrawList().AddCircleFilled(screenPosition, 10f, ImGui.GetColorU32(Color.blue));
			}
			TransitionDriver transitionDriver = component.transitionDriver;
			if (transitionDriver.GetTransition != null)
			{
				if (transitionDriver.GetTransition.navGridTransition.useXOffset)
				{
					Vector2 vector2 = go.GetComponent<KBoxCollider2D>().size / 2f;
					if (transitionDriver.GetTransition.x > 0)
					{
						pos.x += vector2.x;
					}
					else if (transitionDriver.GetTransition.x < 0)
					{
						pos.x -= vector2.x;
					}
					Vector2 screenPosition2 = DevToolEntity.GetScreenPosition(pos);
					string text3 = string.Format("X={0:F2}, Y={1:F2}", pos.x, pos.y);
					ImGui.TextColored(Color.magenta, "Nav Transition: " + text3);
					if (this.drawNavDots)
					{
						ImGui.GetBackgroundDrawList().AddCircleFilled(screenPosition2, 10f, ImGui.GetColorU32(Color.magenta));
					}
				}
				ImGuiEx.SimpleField("Transition", transitionDriver.GetTransition.navGridTransition.ToString());
			}
		}
	}

	private void OccupyAreaContents(GameObject go)
	{
		OccupyArea component = go.GetComponent<OccupyArea>();
		if (component != null && ImGui.CollapsingHeader("Occupy Area", ImGuiTreeNodeFlags.DefaultOpen))
		{
			Extents extents = component.GetExtents();
			ImGui.Checkbox("Draw Occupy Area", ref this.drawOccupyArea);
			if (this.drawOccupyArea)
			{
				Vector2 screenPosition = DevToolEntity.GetScreenPosition(Grid.CellToPos(Grid.OffsetCell(Grid.PosToCell(go), extents.width, extents.height)));
				Vector2 screenPosition2 = DevToolEntity.GetScreenPosition(Grid.CellToPos(Grid.XYToCell(extents.x, extents.y)));
				DevToolEntity.DrawScreenRect(new ValueTuple<Vector2, Vector2>(screenPosition2, screenPosition), go.name, Color.cyan, new Color(0f, 1f, 1f, 0.33f), default(Option<DevToolUtil.TextAlignment>));
			}
			ImGui.Text(string.Format("X={0:F2}, Y={1:F2}", extents.x, extents.y));
			ImGui.Text(string.Format("Width={0:F2}, Height={1:F2}", extents.width, extents.height));
		}
	}

	private void ColliderContents(GameObject go)
	{
		KCollider2D component = go.GetComponent<KCollider2D>();
		if (component != null && ImGui.CollapsingHeader("Collider", ImGuiTreeNodeFlags.DefaultOpen))
		{
			ImGui.Checkbox("Draw Collider", ref this.drawCollider);
			if (this.drawCollider)
			{
				Vector2 screenPosition = DevToolEntity.GetScreenPosition(component.bounds.min);
				Vector2 screenPosition2 = DevToolEntity.GetScreenPosition(component.bounds.max);
				DevToolEntity.DrawScreenRect(new ValueTuple<Vector2, Vector2>(screenPosition, screenPosition2), go.name, Color.green, new Color(0f, 1f, 0f, 0.33f), default(Option<DevToolUtil.TextAlignment>));
			}
			string text = string.Format("X={0:F2}, Y={1:F2}", component.offset.x, component.offset.y);
			ImGuiEx.SimpleField("Offset", text);
			ImGui.Text("Bounds");
			ImGuiEx.SimpleField("Offset", text);
			string text2 = string.Format("{0} Cell: {1}", component.bounds.min, Grid.PosToCell(component.bounds.min));
			ImGuiEx.SimpleField("Min", text2);
			string text3 = string.Format("{0} Cell: {1}", component.bounds.max, Grid.PosToCell(component.bounds.max));
			ImGuiEx.SimpleField("Max", text3);
			ImGuiEx.SimpleField("Center", component.bounds.center);
		}
	}

	private void CritterTemperatureMonitorContents(GameObject go)
	{
		CritterTemperatureMonitor.Instance smi = go.GetSMI<CritterTemperatureMonitor.Instance>();
		if (smi != null && ImGui.CollapsingHeader("Temperature Monitor", ImGuiTreeNodeFlags.DefaultOpen))
		{
			ImGuiEx.SimpleField("Current State", smi.GetCurrentState().name.Replace("root.", ""));
			ImGuiEx.SimpleField("External", smi.GetTemperatureExternal());
			ImGuiEx.SimpleField("Internal", smi.GetTemperatureInternal());
		}
	}

	private bool follow;

	private GameObject lockObject;

	private bool drawNavDots = true;

	private bool drawOccupyArea;

	private bool drawCollider;
}
