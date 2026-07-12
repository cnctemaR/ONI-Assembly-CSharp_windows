using System;
using System.Collections.Generic;
using System.Reflection;
using ImGuiNET;
using ImGuiObjectDrawer;

public readonly struct ImGuiObjectTableDrawer<T>
{
	public ImGuiObjectTableDrawer(string tableId, ImGuiTableFlags flags, List<ImGuiObjectTableDrawer<T>.Column> columns)
	{
		this.tableId = tableId;
		this.useTableId = tableId != null;
		this.flags = flags;
		this.columns = columns;
	}

	public void Draw(IEnumerable<T> data)
	{
		this.Draw(data.GetEnumerator());
	}

	public void Draw(IEnumerator<T> data)
	{
		if (this.useTableId)
		{
			ImGui.PushID(this.tableId);
		}
		if (ImGui.BeginTable("ID_table_contents", this.columns.Count, this.flags))
		{
			ImGui.TableSetupScrollFreeze(this.columns.Count, 1);
			foreach (ImGuiObjectTableDrawer<T>.Column column in this.columns)
			{
				ImGui.TableSetupColumn(column.name);
			}
			ImGui.TableNextRow(ImGuiTableRowFlags.Headers);
			for (int i = 0; i < this.columns.Count; i++)
			{
				ImGui.TableSetColumnIndex(i);
				ImGui.PushID(i);
				ImGuiObjectTableDrawer<T>.Column column2 = this.columns[i];
				ImGui.TableHeader(column2.name);
				if (column2.drawHeaderTooltip != null && ImGui.IsItemHovered())
				{
					ImGui.BeginTooltip();
					column2.drawHeaderTooltip();
					ImGui.EndTooltip();
				}
				ImGui.PopID();
			}
			int num = 0;
			try
			{
				while (data.MoveNext())
				{
					T t = data.Current;
					ImGui.TableNextRow();
					ImGui.PushID(string.Format("ID_row_{0}", num++));
					foreach (ref ImGuiObjectTableDrawer<T>.Column ptr in this.columns)
					{
						ImGui.TableNextColumn();
						ptr.draw(t);
					}
					ImGui.PopID();
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
		if (this.useTableId)
		{
			ImGui.PopID();
		}
	}

	public static ImGuiObjectTableDrawer<T>.Builder New()
	{
		return new ImGuiObjectTableDrawer<T>.Builder();
	}

	public readonly bool useTableId;

	public readonly string tableId;

	public readonly ImGuiTableFlags flags;

	public readonly List<ImGuiObjectTableDrawer<T>.Column> columns;

	public readonly struct Column
	{
		public Column(string name, Action<T> draw, Action drawHeaderTooltip = null)
		{
			this.name = name;
			this.draw = draw;
			this.drawHeaderTooltip = drawHeaderTooltip;
		}

		public readonly string name;

		public readonly Action<T> draw;

		public readonly Action drawHeaderTooltip;
	}

	public class Builder
	{
		public ImGuiObjectTableDrawer<T>.Builder Id(string id)
		{
			this.internal_id = id;
			return this;
		}

		public ImGuiObjectTableDrawer<T>.Builder Flags(ImGuiTableFlags flags)
		{
			this.internal_flags = flags;
			return this;
		}

		public ImGuiObjectTableDrawer<T>.Builder Column(string columnName, Func<T, string> getText)
		{
			this.internal_columns.Add(new ImGuiObjectTableDrawer<T>.Column(columnName, delegate(T e)
			{
				ImGui.TextUnformatted(getText(e));
			}, null));
			return this;
		}

		public ImGuiObjectTableDrawer<T>.Builder Column(string columnName, Action<T> draw)
		{
			this.internal_columns.Add(new ImGuiObjectTableDrawer<T>.Column(columnName, draw, null));
			return this;
		}

		public ImGuiObjectTableDrawer<T>.Builder Column(string columnName, Func<T, object> getObj)
		{
			this.internal_columns.Add(new ImGuiObjectTableDrawer<T>.Column(columnName, delegate(T e)
			{
				ImGui.TextUnformatted(getObj(e).ToString());
			}, null));
			return this;
		}

		public ImGuiObjectTableDrawer<T>.Builder TooltipForPrevColumn(string text)
		{
			Action <>9__1;
			return this.Internal_UpdateColumn(this.internal_columns.Count - 1, delegate(ImGuiObjectTableDrawer<T>.Column col)
			{
				string name = col.name;
				Action<T> draw = col.draw;
				Action action;
				if ((action = <>9__1) == null)
				{
					action = (<>9__1 = delegate
					{
						ImGui.TextUnformatted(text);
					});
				}
				return new ImGuiObjectTableDrawer<T>.Column(name, draw, action);
			});
		}

		public ImGuiObjectTableDrawer<T>.Builder TooltipForPrevColumn(Func<string> getText)
		{
			Action <>9__1;
			return this.Internal_UpdateColumn(this.internal_columns.Count - 1, delegate(ImGuiObjectTableDrawer<T>.Column col)
			{
				string name = col.name;
				Action<T> draw = col.draw;
				Action action;
				if ((action = <>9__1) == null)
				{
					action = (<>9__1 = delegate
					{
						ImGui.TextUnformatted(getText());
					});
				}
				return new ImGuiObjectTableDrawer<T>.Column(name, draw, action);
			});
		}

		public ImGuiObjectTableDrawer<T>.Builder TooltipForPrevColumn(Action drawTooltip)
		{
			return this.Internal_UpdateColumn(this.internal_columns.Count - 1, (ImGuiObjectTableDrawer<T>.Column col) => new ImGuiObjectTableDrawer<T>.Column(col.name, col.draw, drawTooltip));
		}

		public ImGuiObjectTableDrawer<T>.Builder TooltipForPrevColumn(Func<object> getObj)
		{
			Action <>9__1;
			return this.Internal_UpdateColumn(this.internal_columns.Count - 1, delegate(ImGuiObjectTableDrawer<T>.Column col)
			{
				string name = col.name;
				Action<T> draw = col.draw;
				Action action;
				if ((action = <>9__1) == null)
				{
					action = (<>9__1 = delegate
					{
						ImGui.TextUnformatted(getObj().ToString());
					});
				}
				return new ImGuiObjectTableDrawer<T>.Column(name, draw, action);
			});
		}

		public ImGuiObjectTableDrawer<T>.Builder Internal_UpdateColumn(int index, Func<ImGuiObjectTableDrawer<T>.Column, ImGuiObjectTableDrawer<T>.Column> update)
		{
			this.internal_columns[index] = update(this.internal_columns[index]);
			return this;
		}

		public ImGuiObjectTableDrawer<T>.Builder ColumnsFromType()
		{
			return this.ColumnsFromType<T>((T t) => t);
		}

		public ImGuiObjectTableDrawer<T>.Builder ColumnsFromType<U>(Func<T, U> getValue)
		{
			return this.ColumnsFromType(typeof(U), (T v) => getValue(v));
		}

		public ImGuiObjectTableDrawer<T>.Builder ColumnsFromType(Type objType, Func<T, object> getValue)
		{
			MemberInfo[] members = objType.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			for (int i = 0; i < members.Length; i++)
			{
				MemberInfo member = members[i];
				if (member.MemberType == MemberTypes.Property || member.MemberType == MemberTypes.Field)
				{
					this.internal_columns.Add(new ImGuiObjectTableDrawer<T>.Column(member.Name, delegate(T obj)
					{
						MemberDetails memberDetails;
						if (MemberDetails.TryMakeFor(getValue(obj), member, out memberDetails))
						{
							ImGui.TextUnformatted(memberDetails.value.ToString());
							return;
						}
						ImGui.TextUnformatted("<error getting value>");
					}, null));
				}
			}
			return this;
		}

		public ImGuiObjectTableDrawer<T> Build()
		{
			return new ImGuiObjectTableDrawer<T>(this.internal_id, this.internal_flags, this.internal_columns);
		}

		public string internal_id;

		public ImGuiTableFlags internal_flags = ImGuiTableFlags.Reorderable | ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInnerH | ImGuiTableFlags.BordersOuterH | ImGuiTableFlags.BordersInnerV | ImGuiTableFlags.BordersOuterV | ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.ScrollY;

		public List<ImGuiObjectTableDrawer<T>.Column> internal_columns = new List<ImGuiObjectTableDrawer<T>.Column>();
	}
}
