using System;
using System.Collections.Generic;
using System.Diagnostics;
using ImGuiNET;

public class Logger<EntryType> : Logger
{
	public IEnumerator<EntryType> GetEnumerator()
	{
		if (this.entries == null)
		{
			this.entries = new List<EntryType>();
		}
		return this.entries.GetEnumerator();
	}

	public override int Count
	{
		get
		{
			if (this.entries == null)
			{
				return 0;
			}
			return this.entries.Count;
		}
	}

	public void SetMaxEntries(int new_max)
	{
	}

	public Logger(string name, int new_max = 35)
		: base(name)
	{
		this.SetMaxEntries(new_max);
	}

	[Conditional("UNITY_EDITOR")]
	public void Log(EntryType entry)
	{
	}

	public override void DebugDevTool()
	{
		bool flag = base.enableConsoleLogging;
		if (ImGui.Checkbox("Console Logging:", ref flag))
		{
			base.enableConsoleLogging = flag;
		}
		flag = base.breakOnLog;
		if (ImGui.Checkbox("Break On Log:", ref flag))
		{
			base.breakOnLog = flag;
		}
		ImGui.Text(this.name + " Log:");
		if (ImGui.Button("Clear"))
		{
			this.entries.Clear();
		}
		if (this.entries != null)
		{
			ImGui.Indent();
			foreach (EntryType entryType in this.entries)
			{
				ImGui.Text(entryType.ToString());
			}
			ImGui.Unindent();
		}
	}

	private List<EntryType> entries;

	public Action<EntryType> OnLog;
}
