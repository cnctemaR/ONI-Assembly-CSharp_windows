using System;
using System.Collections.Generic;
using ImGuiNET;

public class DevToolMenuNodeParent : IMenuNode
{
	public DevToolMenuNodeParent(string name)
	{
		this.name = name;
		this.children = new List<IMenuNode>();
	}

	public void AddChild(IMenuNode menuNode)
	{
		this.children.Add(menuNode);
	}

	public string GetName()
	{
		return this.name;
	}

	public void Draw()
	{
		if (ImGui.BeginMenu(this.name))
		{
			foreach (IMenuNode menuNode in this.children)
			{
				menuNode.Draw();
			}
			ImGui.EndMenu();
		}
	}

	public string name;

	public List<IMenuNode> children;
}
