using System;
using ImGuiNET;

public struct KImGui_ScopedIndent : IDisposable
{
	public KImGui_ScopedIndent(float indent_amount = 0f)
	{
		this.indent_amount = indent_amount;
		ImGui.Indent(indent_amount);
	}

	public void Dispose()
	{
		ImGui.Unindent(this.indent_amount);
	}

	private float indent_amount;
}
