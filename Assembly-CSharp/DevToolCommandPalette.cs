using System;
using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using UnityEngine;

public class DevToolCommandPalette : DevTool
{
	public DevToolCommandPalette()
		: this(null)
	{
	}

	public DevToolCommandPalette(List<DevToolCommandPalette.Command> commands = null)
	{
		this.drawFlags |= ImGuiWindowFlags.NoResize;
		this.drawFlags |= ImGuiWindowFlags.NoScrollbar;
		this.drawFlags |= ImGuiWindowFlags.NoScrollWithMouse;
		if (commands == null)
		{
			this.commands.allValues = DevToolCommandPaletteUtil.GenerateDefaultCommandPalette();
			return;
		}
		this.commands.allValues = commands;
	}

	public static void Init()
	{
		DevToolCommandPalette.InitWithCommands(DevToolCommandPaletteUtil.GenerateDefaultCommandPalette());
	}

	public static void InitWithCommands(List<DevToolCommandPalette.Command> commands)
	{
		DevToolManager.Instance.panels.AddPanelFor(new DevToolCommandPalette(commands));
	}

	protected override void RenderTo(DevPanel panel)
	{
		DevToolCommandPalette.Resize(panel);
		if (this.commands.allValues == null)
		{
			ImGui.Text("No commands list given");
			return;
		}
		if (this.commands.allValues.Count == 0)
		{
			ImGui.Text("Given command list is empty, no results to show.");
			return;
		}
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			panel.Close();
			return;
		}
		if (!ImGui.IsWindowFocused(ImGuiFocusedFlags.ChildWindows))
		{
			panel.Close();
			return;
		}
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			this.m_selected_index--;
			this.shouldScrollToSelectedCommandFlag = true;
		}
		if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			this.m_selected_index++;
			this.shouldScrollToSelectedCommandFlag = true;
		}
		if (this.commands.filteredValues.Count > 0)
		{
			while (this.m_selected_index < 0)
			{
				this.m_selected_index += this.commands.filteredValues.Count;
			}
			this.m_selected_index %= this.commands.filteredValues.Count;
		}
		else
		{
			this.m_selected_index = 0;
		}
		if ((Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.KeypadEnter)) && this.commands.filteredValues.Count > 0)
		{
			this.SelectCommand(this.commands.filteredValues[this.m_selected_index], panel);
			return;
		}
		if (this.m_should_focus_search)
		{
			ImGui.SetKeyboardFocusHere();
		}
		if (ImGui.InputText("Filter", ref this.commands.filter, 30U) || this.m_should_focus_search)
		{
			this.commands.Refilter();
		}
		this.m_should_focus_search = false;
		ImGui.Separator();
		string text = "Up arrow & down arrow to navigate. Enter to select. ";
		if (this.commands.filteredValues.Count > 0 && this.commands.didUseFilter)
		{
			text += string.Format("Found {0} Results", this.commands.filteredValues.Count);
		}
		ImGui.Text(text);
		ImGui.Separator();
		if (ImGui.BeginChild("ID_scroll_region"))
		{
			if (this.commands.filteredValues.Count <= 0)
			{
				ImGui.Text("Couldn't find anything that matches \"" + this.commands.filter + "\", maybe it hasn't been added yet?");
			}
			else
			{
				for (int i = 0; i < this.commands.filteredValues.Count; i++)
				{
					DevToolCommandPalette.Command command = this.commands.filteredValues[i];
					bool flag = i == this.m_selected_index;
					ImGui.PushID(i);
					bool flag2;
					if (flag)
					{
						flag2 = ImGui.Selectable("> " + command.display_name, flag);
					}
					else
					{
						flag2 = ImGui.Selectable("  " + command.display_name, flag);
					}
					ImGui.PopID();
					if (this.shouldScrollToSelectedCommandFlag && flag)
					{
						this.shouldScrollToSelectedCommandFlag = false;
						ImGui.SetScrollHereY(0.5f);
					}
					if (flag2)
					{
						this.SelectCommand(command, panel);
						ImGui.EndChild();
						return;
					}
				}
			}
		}
		ImGui.EndChild();
	}

	private void SelectCommand(DevToolCommandPalette.Command command, DevPanel panel)
	{
		command.Internal_Select();
		panel.Close();
	}

	private static void Resize(DevPanel devToolPanel)
	{
		float num = 800f;
		float num2 = 400f;
		Rect rect = new Rect(0f, 0f, (float)Screen.width, (float)Screen.height);
		Rect rect2 = new Rect
		{
			x = rect.x + rect.width / 2f - num / 2f,
			y = rect.y + rect.height / 2f - num2 / 2f,
			width = num,
			height = num2
		};
		devToolPanel.SetPosition(rect2.position, ImGuiCond.None);
		devToolPanel.SetSize(rect2.size, ImGuiCond.None);
	}

	private int m_selected_index;

	private StringSearchableList<DevToolCommandPalette.Command> commands = new StringSearchableList<DevToolCommandPalette.Command>(delegate(DevToolCommandPalette.Command command, in string filter)
	{
		return !StringSearchableListUtil.DoAnyTagsMatchFilter(command.tags, in filter);
	});

	private bool m_should_focus_search = true;

	private bool shouldScrollToSelectedCommandFlag;

	public class Command
	{
		public Command(string primary_tag, global::System.Action on_select)
			: this(new string[] { primary_tag }, on_select)
		{
		}

		public Command(string primary_tag, string tag_a, global::System.Action on_select)
			: this(new string[] { primary_tag, tag_a }, on_select)
		{
		}

		public Command(string primary_tag, string tag_a, string tag_b, global::System.Action on_select)
			: this(new string[] { primary_tag, tag_a, tag_b }, on_select)
		{
		}

		public Command(string primary_tag, string tag_a, string tag_b, string tag_c, global::System.Action on_select)
			: this(new string[] { primary_tag, tag_a, tag_b, tag_c }, on_select)
		{
		}

		public Command(string primary_tag, string tag_a, string tag_b, string tag_c, string tag_d, global::System.Action on_select)
			: this(new string[] { primary_tag, tag_a, tag_b, tag_c, tag_d }, on_select)
		{
		}

		public Command(string primary_tag, string tag_a, string tag_b, string tag_c, string tag_d, string tag_e, global::System.Action on_select)
			: this(new string[] { primary_tag, tag_a, tag_b, tag_c, tag_d, tag_e }, on_select)
		{
		}

		public Command(string primary_tag, string tag_a, string tag_b, string tag_c, string tag_d, string tag_e, string tag_f, global::System.Action on_select)
			: this(new string[] { primary_tag, tag_a, tag_b, tag_c, tag_d, tag_e, tag_f }, on_select)
		{
		}

		public Command(string primary_tag, string[] additional_tags, global::System.Action on_select)
			: this(new string[] { primary_tag }.Concat<string>(additional_tags).ToArray<string>(), on_select)
		{
		}

		public Command(string[] tags, global::System.Action on_select)
		{
			this.display_name = tags[0];
			this.tags = tags.Select<string, string>((string t) => t.ToLowerInvariant()).ToArray<string>();
			this.m_on_select = on_select;
		}

		public void Internal_Select()
		{
			this.m_on_select();
		}

		public string display_name;

		public string[] tags;

		private global::System.Action m_on_select;
	}
}
