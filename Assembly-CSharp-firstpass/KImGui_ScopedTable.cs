using System;
using ImGuiNET;

public struct KImGui_ScopedTable : IDisposable
{
	public KImGui_ScopedTable(string label, int num_columns, ImGuiTableFlags flags = ImGuiTableFlags.None)
	{
		if (flags == ImGuiTableFlags.None)
		{
			this.do_pop = ImGui.BeginTable(label, num_columns);
			return;
		}
		this.do_pop = ImGui.BeginTable(label, num_columns, flags);
	}

	public void Dispose()
	{
		if (this.do_pop)
		{
			ImGui.EndTable();
		}
	}

	public static implicit operator bool(KImGui_ScopedTable n)
	{
		return n.do_pop;
	}

	public static bool operator ==(KImGui_ScopedTable node, bool value)
	{
		return node.do_pop == value;
	}

	public static bool operator !=(KImGui_ScopedTable node, bool value)
	{
		return node.do_pop != value;
	}

	public override bool Equals(object obj)
	{
		KImGui_ScopedTable kimGui_ScopedTable = (KImGui_ScopedTable)obj;
		return this == kimGui_ScopedTable;
	}

	public override int GetHashCode()
	{
		return this.do_pop.GetHashCode();
	}

	private bool do_pop;
}
