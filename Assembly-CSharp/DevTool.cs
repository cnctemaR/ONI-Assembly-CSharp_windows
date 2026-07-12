using System;
using ImGuiNET;

public abstract class DevTool
{
	public event global::System.Action OnNextPreDraw;

	public event global::System.Action OnShow;

	public event global::System.Action OnHide;

	public DevTool()
	{
	}

	public void DoImGui()
	{
		if (this.OnNextPreDraw != null)
		{
			this.OnNextPreDraw();
			this.OnNextPreDraw = null;
		}
		if (ImGui.Begin(this.Name, ref this.Enabled, this.drawFlags))
		{
			if (!this.RequiresGameRunning || Game.Instance != null)
			{
				if (!this.m_was_enabled && this.Enabled)
				{
					global::System.Action onShow = this.OnShow;
					if (onShow != null)
					{
						onShow();
					}
					this.m_was_enabled = this.Enabled;
				}
				this.Render();
				if (!this.Enabled && this.m_was_enabled)
				{
					global::System.Action onHide = this.OnHide;
					if (onHide != null)
					{
						onHide();
					}
				}
				this.m_was_enabled = this.Enabled;
			}
			else
			{
				ImGui.Text("Game not loaded");
			}
		}
		ImGui.End();
	}

	public void Show()
	{
		this.Enabled = true;
	}

	public void Hide()
	{
		this.Enabled = false;
	}

	protected abstract void Render();

	public bool Enabled;

	public string Name;

	public string FullPath;

	public bool RequiresGameRunning;

	public ImGuiWindowFlags drawFlags;

	private bool m_was_enabled;
}
