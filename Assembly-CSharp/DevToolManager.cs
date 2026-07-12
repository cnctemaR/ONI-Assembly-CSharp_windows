using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using ImGuiNET;
using UnityEngine;

public class DevToolManager
{
	public bool Show
	{
		get
		{
			return this.showImGui;
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
		this.RegisterDevTool(new DevToolStatusItems(), "Debuggers/StatusItems");
		this.RegisterDevTool(new DevToolUI(), "Debuggers/UI");
		this.RegisterDevTool(new DevToolUnlockedIds(), "Debuggers/UnlockedIds List");
		this.RegisterDevTool(new DevToolStringsTable(), "Debuggers/StringsTable");
		this.RegisterDevTool(new DevToolChoreDebugger(), "Debuggers/Chore");
		this.RegisterDevTool(new DevToolBatchedAnimDebug(), "Debuggers/Batched Anim");
		this.RegisterDevTool(new DevTool_StoryTraits_Reveal(), "Debuggers/Story Traits Reveal");
		this.RegisterDevTool(new DevTool_StoryTrait_CritterManipulator(), "Debuggers/Story Trait - Critter Manipulator");
		this.RegisterDevTool(new DevToolSceneBrowser(), "Scene/Browser");
		this.RegisterDevTool(new DevToolSceneInspector(), "Scene/Inspector");
		this.RegisterDevTool(this.warning, "Help/" + this.warning.Name);
		this.RegisterDevTool(new DevToolCommandPalette(), "Help/Command Palette");
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
		tool.FullPath = location;
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

	public IReadOnlyList<DevTool> GetDevTools()
	{
		return this.tools;
	}

	public void UpdateShouldShowTools()
	{
		if (!DebugHandler.enabled)
		{
			this.showImGui = false;
			return;
		}
		bool flag = Input.GetKeyDown(KeyCode.BackQuote) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl));
		if (!this.toggleKeyWasDown && flag)
		{
			this.showImGui = !this.showImGui;
		}
		this.toggleKeyWasDown = flag;
	}

	public void UpdateTools()
	{
		if (!DebugHandler.enabled)
		{
			return;
		}
		if (this.showImGui)
		{
			this.DrawMenu();
			if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.Space))
			{
				DevToolCommandPalette.Init();
			}
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
					ImGui.Checkbox("ImGui.GetIO().WantCaptureMouse", ImGui.GetIO().WantCaptureMouse);
					ImGui.Checkbox("ImGui.GetIO().WantCaptureKeyboard", ImGui.GetIO().WantCaptureKeyboard);
				}
				ImGui.End();
			}
			if (this.showImguiDemo)
			{
				ImGui.ShowDemoWindow(ref this.showImguiDemo);
			}
		}
		this.UpdateConsumingGameInputs();
	}

	private void DrawMenu()
	{
		this.menuFontSize.InitializeIfNeeded();
		if (ImGui.BeginMainMenuBar())
		{
			if (!this.UserAcceptedWarning)
			{
				ImGui.Checkbox(this.warning.Name, ref this.warning.Enabled);
			}
			else
			{
				this.DrawMenuNodes(this.rootMenuNode);
				this.menuFontSize.DrawMenu();
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

	private unsafe void UpdateConsumingGameInputs()
	{
		this.doesImGuiWantInput = false;
		if (this.showImGui)
		{
			this.doesImGuiWantInput = *ImGui.GetIO().WantCaptureMouse || *ImGui.GetIO().WantCaptureKeyboard;
			if (!this.prevDoesImGuiWantInput && this.doesImGuiWantInput)
			{
				DevToolManager.<UpdateConsumingGameInputs>g__OnInputEnterImGui|27_0();
			}
			if (this.prevDoesImGuiWantInput && !this.doesImGuiWantInput)
			{
				DevToolManager.<UpdateConsumingGameInputs>g__OnInputExitImGui|27_1();
			}
		}
		if (this.prevShowImGui && this.prevDoesImGuiWantInput && !this.showImGui)
		{
			DevToolManager.<UpdateConsumingGameInputs>g__OnInputExitImGui|27_1();
		}
		this.prevShowImGui = this.showImGui;
		this.prevDoesImGuiWantInput = this.doesImGuiWantInput;
		KInputManager.devToolFocus = this.showImGui && this.doesImGuiWantInput;
	}

	[CompilerGenerated]
	internal static void <UpdateConsumingGameInputs>g__OnInputEnterImGui|27_0()
	{
		UnityMouseCatcherUI.SetEnabled(true);
		GameInputManager inputManager = Global.Instance.GetInputManager();
		for (int i = 0; i < inputManager.GetControllerCount(); i++)
		{
			inputManager.GetController(i).HandleCancelInput();
		}
	}

	[CompilerGenerated]
	internal static void <UpdateConsumingGameInputs>g__OnInputExitImGui|27_1()
	{
		UnityMouseCatcherUI.SetEnabled(false);
	}

	public const string SHOW_DEVTOOLS = "ShowDevtools";

	public static DevToolManager Instance;

	private bool toggleKeyWasDown;

	private bool showImGui;

	private bool prevShowImGui;

	private bool doesImGuiWantInput;

	private bool prevDoesImGuiWantInput;

	private bool showImguiState;

	private bool showImguiDemo;

	public bool UserAcceptedWarning;

	private DevToolWarning warning = new DevToolWarning();

	private List<DevTool> tools = new List<DevTool>();

	private DevToolManager.DevToolMenuNode rootMenuNode;

	private DevToolMenuFontSize menuFontSize = new DevToolMenuFontSize();

	public class DevToolMenuNode
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
