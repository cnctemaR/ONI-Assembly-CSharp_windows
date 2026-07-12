using System;
using ImGuiNET;

public struct KImGui_ScopedTreeNode : IDisposable
{
	public KImGui_ScopedTreeNode(string label)
	{
		this.do_pop = ImGui.TreeNode(label);
	}

	public void Dispose()
	{
		if (this.do_pop)
		{
			ImGui.TreePop();
		}
	}

	public static implicit operator bool(KImGui_ScopedTreeNode n)
	{
		return n.do_pop;
	}

	public static bool operator ==(KImGui_ScopedTreeNode node, bool value)
	{
		return node.do_pop == value;
	}

	public static bool operator !=(KImGui_ScopedTreeNode node, bool value)
	{
		return node.do_pop != value;
	}

	public override bool Equals(object obj)
	{
		KImGui_ScopedTreeNode kimGui_ScopedTreeNode = (KImGui_ScopedTreeNode)obj;
		return this == kimGui_ScopedTreeNode;
	}

	public override int GetHashCode()
	{
		return this.do_pop.GetHashCode();
	}

	private bool do_pop;
}
