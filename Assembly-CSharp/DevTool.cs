using System;
using ImGuiNET;

public abstract class DevTool
{
	public event global::System.Action OnUninit;

	public DevTool()
	{
		this.Name = DevToolUtil.GenerateDevToolName(this);
	}

	public void DoImGui(DevPanel panel)
	{
		if (this.RequiresGameRunning && Game.Instance == null)
		{
			ImGui.Text("Game not loaded");
			return;
		}
		this.RenderTo(panel);
	}

	public void ClosePanel()
	{
		this.isRequestingToClosePanel = true;
	}

	protected abstract void RenderTo(DevPanel panel);

	public void Internal_Uninit()
	{
		if (this.OnUninit != null)
		{
			this.OnUninit();
		}
	}

	public string Name;

	public bool RequiresGameRunning;

	public bool isRequestingToClosePanel;

	public ImGuiWindowFlags drawFlags;
}
