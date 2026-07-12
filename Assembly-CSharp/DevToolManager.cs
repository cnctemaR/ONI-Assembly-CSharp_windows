using System;
using System.Collections.Generic;
using System.IO;
using ImGuiNET;
using UnityEngine;

public class DevToolManager
{
	public bool Show
	{
		get
		{
			return this.showImgui;
		}
	}

	public DevToolManager()
	{
		DevToolManager.Instance = this;
		this.rootMenuNode = new DevToolManager.DevToolMenuNode("root");
		this.RegisterDevTool(new DevToolSimDebug(), "Debuggers/Sim Debug");
		this.RegisterDevTool(new DevToolStateMachineDebug(), "Debuggers/State Machine");
		this.RegisterDevTool(new DevToolSaveGameInfo(), "Debuggers/Save Game Info");
		this.RegisterDevTool(new DevToolPrintingPodDebug(), "Debuggers/Printing Pod Debug");
		this.RegisterDevTool(new DevToolBigBaseMutations(), "Debuggers/Big Base Mutation Utilities");
		this.RegisterDevTool(new DevToolNavGrid(), "Debuggers/Nav Grid");
		this.RegisterDevTool(new DevToolResearchDebugger(), "Debuggers/Research");
		this.RegisterDevTool(new DevToolSceneBrowser(), "Scene/Browser");
		this.RegisterDevTool(new DevToolSceneInspector(), "Scene/Inspector");
		this.RegisterDevTool(new DevToolInputDebugger(), "IMGUI/Input Debugger");
		this.RegisterDevTool(this.warning, "Help/" + this.warning.Name);
	}

	public void Init()
	{
		this.UserAcceptedWarning = KPlayerPrefs.GetInt("ShowDevtools", 0) == 1;
	}

	private void RegisterDevTool(DevTool tool, string location)
	{
		string fileName = Path.GetFileName(location);
		string directoryName = Path.GetDirectoryName(location);
		tool.Name = fileName;
		this.tools.Add(tool);
		DevToolManager.DevToolMenuNode devToolMenuNode = this.AddOrGetDevToolNode(directoryName);
		if (devToolMenuNode.Nodes == null)
		{
			devToolMenuNode.Nodes = new List<DevToolManager.DevToolMenuNode>();
		}
		DebugUtil.Assert(devToolMenuNode.Tool == null, "DevToolMenuNode cannot contain both a tool and list of nodes");
		devToolMenuNode.Nodes.Add(new DevToolManager.DevToolMenuNode(fileName, tool));
	}

	private DevToolManager.DevToolMenuNode AddOrGetDevToolNode(string path)
	{
		string[] array = path.Split(new char[] { '/' });
		DevToolManager.DevToolMenuNode devToolMenuNode = this.rootMenuNode;
		string[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			string split = array2[i];
			if (devToolMenuNode.Nodes == null)
			{
				devToolMenuNode.Nodes = new List<DevToolManager.DevToolMenuNode>();
			}
			DevToolManager.DevToolMenuNode devToolMenuNode2 = devToolMenuNode.Nodes.Find((DevToolManager.DevToolMenuNode x) => x.Name == split);
			if (devToolMenuNode2 == null)
			{
				devToolMenuNode2 = new DevToolManager.DevToolMenuNode(split);
				devToolMenuNode.Nodes.Add(devToolMenuNode2);
			}
			devToolMenuNode = devToolMenuNode2;
		}
		return devToolMenuNode;
	}

	public T GetDevTool<T>() where T : DevTool
	{
		return (T)((object)this.tools.Find((DevTool x) => x is T));
	}

	public void UpdateShouldShowTools()
	{
		if (!DebugHandler.enabled)
		{
			this.showImgui = false;
			return;
		}
		bool flag = Input.GetKeyDown(KeyCode.BackQuote) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl));
		if (!this.toggleKeyWasDown && flag)
		{
			this.showImgui = !this.showImgui;
		}
		this.toggleKeyWasDown = flag;
	}

	public void UpdateTools()
	{
		if (!DebugHandler.enabled)
		{
			return;
		}
		if (this.showImgui)
		{
			this.DrawMenu();
			foreach (DevTool devTool in this.tools)
			{
				if (devTool.Enabled)
				{
					devTool.DoImGui();
				}
			}
			if (this.showImguiState)
			{
				if (ImGui.Begin("ImGui state", ref this.showImguiState))
				{
					ImGui.Checkbox("Game has focus", ref this.gameFocus);
					ImGui.Checkbox("anyWindowFocus", ref this.anyWindowFocus);
					ImGui.Checkbox("anyWindowHover", ref this.anyWindowHover);
				}
				ImGui.End();
			}
			if (this.showImguiDemo)
			{
				ImGui.ShowDemoWindow(ref this.showImguiDemo);
			}
		}
		if (this.showImgui)
		{
			this.anyWindowFocus = ImGui.IsWindowFocused(ImGuiFocusedFlags.AnyWindow);
			this.anyWindowHover = ImGui.IsWindowHovered(ImGuiHoveredFlags.AnyWindow) || ImGui.IsPopupOpen("", ImGuiPopupFlags.AnyPopup);
			this.gameFocus = !this.anyWindowFocus;
			if (this.anyWindowHover && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2)))
			{
				this.gameFocus = false;
			}
			if (!this.anyWindowHover && (Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.Mouse2)))
			{
				ImGui.SetWindowFocus(null);
				this.gameFocus = true;
			}
		}
		else
		{
			this.gameFocus = true;
		}
		if (KInputManager.devToolFocus == this.gameFocus)
		{
			if (!this.gameFocus)
			{
				GameInputManager inputManager = Global.Instance.GetInputManager();
				for (int i = 0; i < inputManager.GetControllerCount(); i++)
				{
					inputManager.GetController(i).HandleCancelInput();
				}
			}
			KInputManager.devToolFocus = !this.gameFocus;
		}
	}

	private void DrawMenu()
	{
		if (ImGui.BeginMainMenuBar())
		{
			if (!this.UserAcceptedWarning)
			{
				ImGui.Checkbox(this.warning.Name, ref this.warning.Enabled);
			}
			else
			{
				this.DrawMenuNodes(this.rootMenuNode);
				if (ImGui.BeginMenu("IMGUI"))
				{
					ImGui.Checkbox("ImGui state", ref this.showImguiState);
					ImGui.Checkbox("ImGui Demo", ref this.showImguiDemo);
					ImGui.EndMenu();
				}
			}
			ImGui.EndMainMenuBar();
		}
	}

	private void DrawMenuNodes(DevToolManager.DevToolMenuNode node)
	{
		if (node.IsTool)
		{
			ImGui.Checkbox(node.Name, ref node.Tool.Enabled);
			return;
		}
		if (node.Name == "root")
		{
			using (List<DevToolManager.DevToolMenuNode>.Enumerator enumerator = node.Nodes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					DevToolManager.DevToolMenuNode devToolMenuNode = enumerator.Current;
					this.DrawMenuNodes(devToolMenuNode);
				}
				return;
			}
		}
		if (ImGui.BeginMenu(node.Name))
		{
			foreach (DevToolManager.DevToolMenuNode devToolMenuNode2 in node.Nodes)
			{
				this.DrawMenuNodes(devToolMenuNode2);
			}
			ImGui.EndMenu();
		}
	}

	public const string SHOW_DEVTOOLS = "ShowDevtools";

	public static DevToolManager Instance;

	private bool gameFocus;

	private bool anyWindowFocus;

	private bool anyWindowHover;

	private bool toggleKeyWasDown;

	private bool showImgui;

	private bool showImguiState;

	private bool showImguiDemo;

	public bool UserAcceptedWarning;

	private DevToolWarning warning = new DevToolWarning();

	private List<DevTool> tools = new List<DevTool>();

	private DevToolManager.DevToolMenuNode rootMenuNode;

	private class DevToolMenuNode
	{
		public DevToolMenuNode(string name)
		{
			this.Name = name;
		}

		public DevToolMenuNode(string name, DevTool tool)
		{
			this.Name = name;
			this.Tool = tool;
		}

		public bool IsTool
		{
			get
			{
				return this.Tool != null;
			}
		}

		public string Name;

		public DevTool Tool;

		public List<DevToolManager.DevToolMenuNode> Nodes;
	}
}
