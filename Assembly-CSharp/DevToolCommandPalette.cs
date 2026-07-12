using System;
using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using UnityEngine;

public class DevToolCommandPalette : DevTool
{
	public DevToolCommandPalette()
	{
		base.OnShow += delegate
		{
			this.m_filter = "";
			this.m_should_focus_search = true;
			this.m_selected_index = 0;
			if (this.m_all_commands == null)
			{
				this.m_all_commands = DevToolCommandPaletteUtil.GenerateDefaultCommandPalette();
			}
		};
		base.OnHide += delegate
		{
			this.m_all_commands = null;
		};
	}

	public static void Init()
	{
		DevToolCommandPalette.InitWithCommands(DevToolCommandPaletteUtil.GenerateDefaultCommandPalette());
	}

	public static void InitWithCommands(IList<DevToolCommandPalette.Command> commands)
	{
		DevToolCommandPalette devTool = DevToolManager.Instance.GetDevTool<DevToolCommandPalette>();
		float num = 800f;
		float num2 = 400f;
		Rect rect = new Rect(0f, 0f, (float)Screen.width, (float)Screen.height);
		Rect our_window_rect = new Rect
		{
			x = rect.x + rect.width / 2f - num / 2f,
			y = rect.y + rect.height / 2f - num2 / 2f,
			width = num,
			height = num2
		};
		devTool.m_all_commands = commands;
		devTool.OnNextPreDraw += delegate
		{
			ImGui.SetNextWindowPos(our_window_rect.position);
			ImGui.SetNextWindowSize(our_window_rect.size);
		};
		devTool.Show();
	}

	protected override void Render()
	{
		if (this.m_all_commands == null)
		{
			ImGui.Text("No commands list given");
			return;
		}
		if (this.m_all_commands.Count == 0)
		{
			ImGui.Text("Given command list is empty, no results to show.");
			return;
		}
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			base.Hide();
			return;
		}
		if (!ImGui.IsWindowFocused(ImGuiFocusedFlags.ChildWindows))
		{
			base.Hide();
			return;
		}
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			this.m_selected_index--;
		}
		if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			this.m_selected_index++;
		}
		if (this.m_cached_commands.Count > 0)
		{
			while (this.m_selected_index < 0)
			{
				this.m_selected_index += this.m_cached_commands.Count;
			}
			this.m_selected_index %= this.m_cached_commands.Count;
		}
		else
		{
			this.m_selected_index = 0;
		}
		if ((Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.KeypadEnter)) && this.m_cached_commands.Count > 0)
		{
			this.SelectCommand(this.m_cached_commands[this.m_selected_index]);
			return;
		}
		if (this.m_should_focus_search)
		{
			ImGui.SetKeyboardFocusHere();
		}
		if (ImGui.InputText("Filter", ref this.m_filter, 30U) || this.m_should_focus_search)
		{
			this.UpdateCachedCommands();
		}
		this.m_should_focus_search = false;
		ImGui.Separator();
		string text = "Up arrow & down arrow to navigate. Enter to select.";
		if (this.m_cached_commands.Count > 0 && DevToolCommandPalette.ShouldUseFilter(this.m_filter))
		{
			text += string.Format(" Found {0} Results", this.m_cached_commands.Count);
		}
		ImGui.Text(text);
		ImGui.Separator();
		if (this.m_cached_commands.Count <= 0)
		{
			ImGui.Text("Couldn't find anything that matches \"" + this.m_filter + "\", maybe it hasn't been added yet?");
			return;
		}
		for (int i = 0; i < this.m_cached_commands.Count; i++)
		{
			DevToolCommandPalette.Command command = this.m_cached_commands[i];
			bool flag = i == this.m_selected_index;
			ImGui.PushID(i);
			bool flag2;
			if (flag)
			{
				flag2 = ImGui.Selectable("> " + command.display_name);
			}
			else
			{
				flag2 = ImGui.Selectable("  " + command.display_name);
			}
			ImGui.PopID();
			if (flag2)
			{
				this.SelectCommand(command);
				return;
			}
		}
	}

	private void SelectCommand(DevToolCommandPalette.Command command)
	{
		command.Internal_Select();
		base.Hide();
	}

	private void UpdateCachedCommands()
	{
		if (DevToolCommandPalette.ShouldUseFilter(this.m_filter))
		{
			this.m_cached_commands.Clear();
			this.m_selected_index = 0;
			using (IEnumerator<DevToolCommandPalette.Command> enumerator = this.m_all_commands.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					DevToolCommandPalette.Command command = enumerator.Current;
					if (command.Contains(this.m_filter))
					{
						this.m_cached_commands.Add(command);
					}
				}
				return;
			}
		}
		if (this.m_cached_commands.Count != this.m_all_commands.Count)
		{
			this.m_cached_commands.Clear();
			this.m_selected_index = 0;
			this.m_cached_commands.AddRange(this.m_all_commands);
		}
	}

	private static bool ShouldUseFilter(string filter)
	{
		return !string.IsNullOrWhiteSpace(filter);
	}

	private int m_selected_index;

	private List<DevToolCommandPalette.Command> m_cached_commands = new List<DevToolCommandPalette.Command>();

	private IList<DevToolCommandPalette.Command> m_all_commands;

	private IList<DevToolCommandPalette.Command> m_commands_to_use_on_open;

	private string m_filter;

	private bool m_should_focus_search;

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

		public bool Contains(string filter)
		{
			filter = filter.Trim();
			string text = filter.ToLowerInvariant();
			string[] array = text.Split(new char[] { ' ' });
			string[] array2 = this.tags;
			for (int i = 0; i < array2.Length; i++)
			{
				string tag = array2[i];
				if (DevToolCommandPalette.Command.DoesTagContainFilter(tag, text))
				{
					return true;
				}
				if (array.Select<string, bool>((string f) => DevToolCommandPalette.Command.DoesTagContainFilter(tag, f)).All<bool>((bool result) => result))
				{
					return true;
				}
			}
			return false;
		}

		private static bool DoesTagContainFilter(string tag, string filter)
		{
			return !string.IsNullOrWhiteSpace(filter) && tag.Contains(filter);
		}

		public void Internal_Select()
		{
			try
			{
				this.m_on_select();
			}
			catch (Exception ex)
			{
				global::UnityEngine.Debug.LogException(ex);
			}
		}

		public string display_name;

		public string[] tags;

		private global::System.Action m_on_select;
	}
}
