using System;
using ImGuiNET;

public abstract class DevTool
{
	public DevTool()
	{
	}

	public void DoImGui()
	{
		if (ImGui.Begin(this.Name, ref this.Enabled, this.drawFlags))
		{
			if (!this.RequiresGameRunning || Game.Instance != null)
			{
				this.Render();
			}
			else
			{
				ImGui.Text("Game not loaded");
			}
		}
		ImGui.End();
	}

	public void Hide()
	{
		this.Enabled = false;
	}

	protected abstract void Render();

	public bool Enabled;

	public string Name;

	public bool RequiresGameRunning;

	public ImGuiWindowFlags drawFlags;
}
