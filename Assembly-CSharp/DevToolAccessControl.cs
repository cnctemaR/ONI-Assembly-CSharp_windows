using System;
using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using UnityEngine;

public class DevToolAccessControl : DevTool
{
	public DevToolAccessControl()
	{
		DevToolAccessControl.Instance = this;
	}

	private bool Init()
	{
		if (Game.Instance == null)
		{
			return false;
		}
		if (!this.initialized)
		{
			this.initialized = true;
			foreach (Tag tag in (from e in Assets.GetPrefabsWithTag(GameTags.BaseMinion)
				select e.GetComponent<KPrefabID>().PrefabTag).ToList<Tag>())
			{
				this.minionsByType.Add(tag, new List<MinionAssignablesProxy>());
			}
			this.robotTypes = (from e in Assets.GetPrefabsWithTag(GameTags.Robots.Behaviours.HasDoorPermissions)
				select e.GetComponent<KPrefabID>().PrefabTag).ToList<Tag>();
		}
		return true;
	}

	protected override void RenderTo(DevPanel panel)
	{
		if (this.Init())
		{
			if (this.DoorSelected())
			{
				ImGui.Checkbox("Lock Selection", ref this.lockSelected);
				this.MinionContents();
				this.RobotContents();
			}
			this.GridRestrictionSerializerContents();
			return;
		}
		ImGui.Text("No Access Control selected");
	}

	private bool DoorSelected()
	{
		return SelectTool.Instance != null && SelectTool.Instance.selected != null && this.SetDoorAccessControl() != null;
	}

	private AccessControl SetDoorAccessControl()
	{
		AccessControl component = SelectTool.Instance.selected.GetComponent<AccessControl>();
		if (component != this.selectedAccessControl && !this.lockSelected)
		{
			this.selectedAccessControl = component;
		}
		return this.selectedAccessControl;
	}

	private void MinionContents()
	{
		foreach (MinionAssignablesProxy minionAssignablesProxy in Components.MinionAssignablesProxy.Items)
		{
			Tag minionModel = minionAssignablesProxy.GetMinionModel();
			if (!this.minionsByType[minionModel].Contains(minionAssignablesProxy))
			{
				this.minionsByType[minionModel].Add(minionAssignablesProxy);
			}
		}
		foreach (Tag tag in this.minionsByType.Keys)
		{
			ImGui.PushID(tag.Name);
			ImGui.Text(tag.Name);
			AccessControl.Permission setPermission = this.selectedAccessControl.GetSetPermission(Tag.Invalid.GetHashCode(), tag);
			bool flag = setPermission == AccessControl.Permission.GoLeft || setPermission == AccessControl.Permission.Both;
			bool flag2 = setPermission == AccessControl.Permission.GoRight || setPermission == AccessControl.Permission.Both;
			ImGui.SameLine();
			if (ImGui.Checkbox("Left", ref flag))
			{
				this.UpdateAccess(tag, flag, flag2);
			}
			ImGui.SameLine();
			if (ImGui.Checkbox("Right", ref flag2))
			{
				this.UpdateAccess(tag, flag, flag2);
			}
			ImGui.PopID();
			ImGui.Indent();
			ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.75f);
			foreach (MinionAssignablesProxy minionAssignablesProxy2 in this.minionsByType[tag])
			{
				ImGui.PushID(minionAssignablesProxy2.TargetInstanceID);
				ImGui.Text(minionAssignablesProxy2.target.GetProperName());
				AccessControl.Permission setPermission2 = this.selectedAccessControl.GetSetPermission(minionAssignablesProxy2);
				ImGui.SameLine();
				bool flag3 = setPermission2 == AccessControl.Permission.GoLeft || setPermission2 == AccessControl.Permission.Both;
				bool flag4 = setPermission2 == AccessControl.Permission.GoRight || setPermission2 == AccessControl.Permission.Both;
				if (ImGui.Checkbox("Left", ref flag3))
				{
					this.UpdateMinionAccess(minionAssignablesProxy2, flag3, flag4);
				}
				ImGui.SameLine();
				if (ImGui.Checkbox("Right", ref flag4))
				{
					this.UpdateMinionAccess(minionAssignablesProxy2, flag3, flag4);
				}
				ImGui.PopID();
			}
			ImGui.PopStyleVar();
			ImGui.Unindent();
		}
	}

	private void RobotContents()
	{
		ImGui.PushID(GameTags.Robot.Name);
		ImGui.Text(GameTags.Robot.Name);
		AccessControl.Permission setPermission = this.selectedAccessControl.GetSetPermission(Tag.Invalid.GetHashCode(), GameTags.Robot);
		bool flag = setPermission == AccessControl.Permission.GoLeft || setPermission == AccessControl.Permission.Both;
		bool flag2 = setPermission == AccessControl.Permission.GoRight || setPermission == AccessControl.Permission.Both;
		ImGui.SameLine();
		if (ImGui.Checkbox("Left", ref flag))
		{
			this.UpdateAccess(GameTags.Robot, flag, flag2);
		}
		ImGui.SameLine();
		if (ImGui.Checkbox("Right", ref flag2))
		{
			this.UpdateAccess(GameTags.Robot, flag, flag2);
		}
		ImGui.PopID();
		ImGui.Indent();
		ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.75f);
		foreach (Tag tag in this.robotTypes)
		{
			ImGui.PushID(tag.Name);
			ImGui.Text(tag.Name);
			AccessControl.Permission setPermission2 = this.selectedAccessControl.GetSetPermission(tag);
			bool flag3 = setPermission2 == AccessControl.Permission.GoLeft || setPermission2 == AccessControl.Permission.Both;
			bool flag4 = setPermission2 == AccessControl.Permission.GoRight || setPermission2 == AccessControl.Permission.Both;
			ImGui.SameLine();
			if (ImGui.Checkbox("Left", ref flag3))
			{
				this.UpdateAccess(tag, flag3, flag4);
			}
			ImGui.SameLine();
			if (ImGui.Checkbox("Right", ref flag4))
			{
				this.UpdateAccess(tag, flag3, flag4);
			}
			ImGui.PopID();
		}
		ImGui.PopStyleVar();
		ImGui.Unindent();
	}

	private void GridRestrictionSerializerContents()
	{
	}

	private void UpdateMinionAccess(MinionAssignablesProxy proxy, bool left, bool right)
	{
		AccessControl.Permission permission;
		if (left)
		{
			if (right)
			{
				permission = AccessControl.Permission.Both;
			}
			else
			{
				permission = AccessControl.Permission.GoLeft;
			}
		}
		else if (right)
		{
			permission = AccessControl.Permission.GoRight;
		}
		else
		{
			permission = AccessControl.Permission.Neither;
		}
		this.selectedAccessControl.SetPermission(proxy, permission);
	}

	private void UpdateAccess(Tag id, bool left, bool right)
	{
		AccessControl.Permission permission;
		if (left)
		{
			if (right)
			{
				permission = AccessControl.Permission.Both;
			}
			else
			{
				permission = AccessControl.Permission.GoLeft;
			}
		}
		else if (right)
		{
			permission = AccessControl.Permission.GoRight;
		}
		else
		{
			permission = AccessControl.Permission.Neither;
		}
		this.selectedAccessControl.SetPermission(id, permission);
	}

	public static DevToolAccessControl Instance;

	private bool initialized;

	private Dictionary<Tag, List<MinionAssignablesProxy>> minionsByType = new Dictionary<Tag, List<MinionAssignablesProxy>>();

	private AccessControl selectedAccessControl;

	private bool lockSelected;

	private List<Tag> robotTypes;

	private struct MinionPermissions
	{
		public bool Left;

		public bool Right;

		public MinionAssignablesProxy Proxy;
	}
}
