using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ImGuiNET;
using UnityEngine;

public class DevToolStatusItems : DevTool
{
	public DevToolStatusItems()
	{
		ValueTuple<string, Action<StatusItemGroup.Entry>>[] array = new ValueTuple<string, Action<StatusItemGroup.Entry>>[4];
		array[0] = new ValueTuple<string, Action<StatusItemGroup.Entry>>("Text", delegate(StatusItemGroup.Entry entry)
		{
			ImGui.Text(entry.GetName());
		});
		array[1] = new ValueTuple<string, Action<StatusItemGroup.Entry>>("Id Name", delegate(StatusItemGroup.Entry entry)
		{
			ImGui.Text(entry.item.Id);
		});
		array[2] = new ValueTuple<string, Action<StatusItemGroup.Entry>>("Notification Type", delegate(StatusItemGroup.Entry entry)
		{
			ImGui.Text(entry.item.notificationType.ToString());
		});
		array[3] = new ValueTuple<string, Action<StatusItemGroup.Entry>>("Category", delegate(StatusItemGroup.Entry entry)
		{
			StatusItemCategory category = entry.category;
			ImGui.Text(((category != null) ? category.Name : null) ?? "<no category>");
		});
		this.columns = array;
		base..ctor();
		this.RequiresGameRunning = true;
	}

	protected override void Render()
	{
		KSelectable selected = SelectTool.Instance.selected;
		if (selected == null || !selected)
		{
			ImGui.Text("no object is selected");
			return;
		}
		StatusItemGroup statusItemGroup = selected.GetStatusItemGroup();
		if (statusItemGroup == null)
		{
			ImGui.Text("object doesn't have a StatusItemGroup");
			return;
		}
		DevToolStatusItems.DrawTable<StatusItemGroup.Entry>("status_items", this.columns, statusItemGroup.GetEnumerator());
	}

	private static void DrawTable<T>(string string_id, [TupleElementNames(new string[] { "header", "draw" })] ValueTuple<string, Action<T>>[] columns, IEnumerator<T> data)
	{
		ImGuiTableFlags imGuiTableFlags = ImGuiTableFlags.Resizable | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInnerH | ImGuiTableFlags.BordersOuterH | ImGuiTableFlags.BordersInnerV | ImGuiTableFlags.BordersOuterV | ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.ScrollX | ImGuiTableFlags.ScrollY;
		if (ImGui.BeginTable(string_id, columns.Length, imGuiTableFlags))
		{
			ValueTuple<string, Action<T>>[] array = columns;
			for (int i = 0; i < array.Length; i++)
			{
				ImGui.TableSetupColumn(array[i].Item1);
			}
			ImGui.TableHeadersRow();
			try
			{
				while (data.MoveNext())
				{
					T t = data.Current;
					ImGui.TableNextRow();
					foreach (ValueTuple<string, Action<T>> valueTuple in columns)
					{
						ImGui.TableNextColumn();
						try
						{
							valueTuple.Item2(t);
						}
						catch (Exception ex)
						{
							ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1f, 0f, 0f, 1f));
							ImGui.Text("<ERROR: " + ex.Message + ">");
							if (ImGui.IsItemHovered())
							{
								ImGui.BeginTooltip();
								ImGui.SetTooltip(ex.ToString());
								ImGui.EndTooltip();
							}
							ImGui.PopStyleColor(1);
						}
					}
				}
			}
			finally
			{
				if (data != null)
				{
					data.Dispose();
				}
			}
			ImGui.EndTable();
		}
	}

	[TupleElementNames(new string[] { "header", "draw" })]
	private ValueTuple<string, Action<StatusItemGroup.Entry>>[] columns;
}
