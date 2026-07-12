using System;
using ImGuiNET;

public class DevToolInputDebugger : DevTool
{
	protected override void Render()
	{
		GameInputManager inputManager = Global.Instance.GetInputManager();
		for (int i = 0; i < inputManager.GetControllerCount(); i++)
		{
			KInputController controller = inputManager.GetController(i);
			ImGui.Text("Controller " + i.ToString() + ": MouseRight=" + controller.IsActive(global::Action.MouseRight).ToString());
		}
	}
}
