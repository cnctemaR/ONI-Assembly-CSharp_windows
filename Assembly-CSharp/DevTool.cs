using System;
using ImGuiNET;

public abstract class DevTool
{
	public event global::System.Action OnInit;

	public event global::System.Action OnUpdate;

	public event global::System.Action OnUninit;

	public DevTool()
	{
		this.Name = DevToolUtil.GenerateDevToolName(this);
	}

	public void DoImGui(DevPanel panel)
	{
		if (this.RequiresGameRunning && Game.Instance == null)
		{
			ImGui.Text("Game must be loaded to use this devtool.");
			return;
		}
		this.RenderTo(panel);
	}

	public void ClosePanel()
	{
		this.isRequestingToClosePanel = true;
	}

	protected abstract void RenderTo(DevPanel panel);

	public void Internal_TryInit()
	{
		if (this.didInit)
		{
			return;
		}
		this.didInit = true;
		if (this.OnInit != null)
		{
			this.OnInit();
		}
	}

	public void Internal_Update()
	{
		if (this.OnUpdate != null)
		{
			this.OnUpdate();
		}
	}

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

	private bool didInit;
}
