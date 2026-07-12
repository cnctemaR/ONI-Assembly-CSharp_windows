using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine.Pool;

namespace UnityEngine.UIElements
{
	internal class ColumnLayout
	{
		public Columns columns
		{
			get
			{
				return this.m_Columns;
			}
		}

		public bool isDirty
		{
			get
			{
				return this.m_IsDirty;
			}
		}

		public float columnsWidth
		{
			get
			{
				bool columnsWidthDirty = this.m_ColumnsWidthDirty;
				if (columnsWidthDirty)
				{
					this.m_ColumnsWidth = 0f;
					foreach (Column column in this.m_Columns.visibleList)
					{
						this.m_ColumnsWidth += column.desiredWidth;
					}
					this.m_ColumnsWidthDirty = false;
				}
				return this.m_ColumnsWidth;
			}
		}

		public float layoutWidth
		{
			get
			{
				return this.m_LayoutWidth;
			}
		}

		public float minColumnsWidth
		{
			get
			{
				return this.m_MinColumnsWidth;
			}
		}

		public float maxColumnsWidth
		{
			get
			{
				return this.m_MaxColumnsWidth;
			}
		}

		public bool hasStretchableColumns
		{
			get
			{
				return this.m_StretchableColumns.Count > 0;
			}
		}

		public bool hasRelativeWidthColumns
		{
			get
			{
				return this.m_RelativeWidthColumns.Count > 0 || this.m_MixedWidthColumns.Count > 0;
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action layoutRequested;

		public ColumnLayout(Columns columns)
		{
			this.m_Columns = columns;
			for (int i = 0; i < columns.Count; i++)
			{
				this.OnColumnAdded(columns[i], i);
			}
			columns.columnAdded += this.OnColumnAdded;
			columns.columnRemoved += this.OnColumnRemoved;
			columns.columnReordered += this.OnColumnReordered;
		}

		public void Dirty()
		{
			bool isDirty = this.m_IsDirty;
			if (!isDirty)
			{
				this.m_IsDirty = true;
				this.ClearCache();
				Action action = this.layoutRequested;
				if (action != null)
				{
					action();
				}
			}
		}

		private void OnColumnAdded(Column column, int index)
		{
			column.changed += this.OnColumnChanged;
			column.resized += this.OnColumnResized;
			this.Dirty();
		}

		private void OnColumnRemoved(Column column)
		{
			column.changed -= this.OnColumnChanged;
			column.resized -= this.OnColumnResized;
			this.Dirty();
		}

		private void OnColumnReordered(Column column, int from, int to)
		{
			this.Dirty();
		}

		private bool RequiresLayoutUpdate(ColumnDataType type)
		{
			return type - ColumnDataType.Visibility <= 4 || type - ColumnDataType.HeaderTemplate <= 1;
		}

		private void OnColumnChanged(Column column, ColumnDataType type)
		{
			bool flag = this.m_DragResizing || !this.RequiresLayoutUpdate(type);
			if (!flag)
			{
				this.Dirty();
			}
		}

		private void OnColumnResized(Column column)
		{
			this.m_ColumnsWidthDirty = true;
		}

		private static bool IsClamped(float value, float min, float max)
		{
			return value >= min && value <= max;
		}

		public void DoLayout(float width)
		{
			this.m_LayoutWidth = width;
			bool isDirty = this.m_IsDirty;
			if (isDirty)
			{
				this.UpdateCache();
			}
			bool hasRelativeWidthColumns = this.hasRelativeWidthColumns;
			if (hasRelativeWidthColumns)
			{
				this.UpdateMinAndMaxColumnsWidth();
			}
			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			List<Column> list = new List<Column>();
			List<Column> list2 = new List<Column>();
			foreach (Column column in this.m_Columns)
			{
				bool flag = !column.visible;
				if (!flag)
				{
					float minWidth = column.GetMinWidth(this.m_LayoutWidth);
					float maxWidth = column.GetMaxWidth(this.m_LayoutWidth);
					float width2 = column.GetWidth(this.m_LayoutWidth);
					bool flag2 = float.IsNaN(column.desiredWidth);
					if (flag2)
					{
						bool flag3 = this.m_Columns.stretchMode == Columns.StretchMode.GrowAndFill && column.stretchable;
						if (flag3)
						{
							list.Add(column);
							continue;
						}
						column.desiredWidth = Mathf.Clamp(width2, minWidth, maxWidth);
					}
					else
					{
						bool flag4 = this.m_Columns.stretchMode == Columns.StretchMode.GrowAndFill && column.stretchable;
						if (flag4)
						{
							list2.Add(column);
							num3 += this.GetDesiredWidth(column);
						}
						bool flag5 = !ColumnLayout.IsClamped(column.desiredWidth, minWidth, maxWidth);
						if (flag5)
						{
							column.desiredWidth = Mathf.Clamp(width2, minWidth, maxWidth);
						}
						bool flag6 = this.columns.stretchMode == Columns.StretchMode.Grow && column.width.unit == LengthUnit.Percent;
						if (flag6)
						{
							column.desiredWidth = Mathf.Clamp(width2, minWidth, maxWidth);
						}
					}
					bool flag7 = !column.stretchable;
					if (flag7)
					{
						num2 += column.desiredWidth;
					}
					num += column.desiredWidth;
				}
			}
			bool flag8 = list.Count > 0;
			if (flag8)
			{
				float num4 = Math.Max(0f, width - num2);
				int num5 = this.m_StretchableColumns.Count;
				list.Sort((Column c1, Column c2) => c1.GetMaxWidth(this.m_LayoutWidth).CompareTo(c2.GetMaxWidth(this.m_LayoutWidth)));
				foreach (Column column2 in list)
				{
					float num6 = num4 / (float)num5;
					column2.desiredWidth = Mathf.Clamp(num6, column2.GetMinWidth(this.m_LayoutWidth), column2.GetMaxWidth(this.m_LayoutWidth));
					num4 = Math.Max(0f, num4 - column2.desiredWidth);
					num5--;
				}
				list2.Sort((Column c1, Column c2) => c1.GetMaxWidth(this.m_LayoutWidth).CompareTo(c2.GetMaxWidth(this.m_LayoutWidth)));
				foreach (Column column3 in list2)
				{
					float desiredWidth = this.GetDesiredWidth(column3);
					float num7 = desiredWidth / num3;
					float num8 = num4 * num7;
					column3.desiredWidth = Mathf.Clamp(num8, column3.GetMinWidth(this.m_LayoutWidth), column3.GetMaxWidth(this.m_LayoutWidth));
					num4 = Math.Max(0f, num4 - column3.desiredWidth);
					num3 -= desiredWidth;
					num5--;
				}
			}
			bool flag9 = this.hasStretchableColumns || (this.hasRelativeWidthColumns && this.m_Columns.stretchMode == Columns.StretchMode.GrowAndFill);
			if (flag9)
			{
				float num9 = 0f;
				bool flag10 = this.m_Columns.stretchMode == Columns.StretchMode.Grow;
				if (flag10)
				{
					bool flag11 = !float.IsNaN(this.m_PreviousWidth);
					if (flag11)
					{
						num9 = this.m_PreviousWidth - width;
					}
				}
				else
				{
					num9 = this.columnsWidth - Mathf.Clamp(width, this.minColumnsWidth, this.maxColumnsWidth);
				}
				bool flag12 = num9 != 0f;
				if (flag12)
				{
					List<Column> list3;
					using (CollectionPool<List<Column>, Column>.Get(out list3))
					{
						List<Column> list4;
						using (CollectionPool<List<Column>, Column>.Get(out list4))
						{
							List<Column> list5;
							using (CollectionPool<List<Column>, Column>.Get(out list5))
							{
								list3.AddRange(this.m_StretchableColumns);
								list4.AddRange(this.m_FixedColumns);
								list5.AddRange(this.m_RelativeWidthColumns);
								this.StretchResizeColumns(list3, list4, list5, ref num9, false, false);
							}
						}
					}
				}
			}
			this.m_PreviousWidth = width;
			this.m_IsDirty = false;
		}

		public void StretchResizeColumns(List<Column> stretchableColumns, List<Column> fixedColumns, List<Column> relativeWidthColumns, ref float delta, bool resizeToFit, bool dragResize)
		{
			bool flag = stretchableColumns.Count == 0 && relativeWidthColumns.Count == 0 && fixedColumns.Count == 0;
			if (!flag)
			{
				bool flag2 = delta > 0f;
				if (flag2)
				{
					this.DistributeOverflow(stretchableColumns, fixedColumns, relativeWidthColumns, ref delta, resizeToFit, dragResize);
				}
				else
				{
					this.DistributeExcess(stretchableColumns, fixedColumns, relativeWidthColumns, ref delta, resizeToFit, dragResize);
				}
			}
		}

		private void DistributeOverflow(List<Column> stretchableColumns, List<Column> fixedColumns, List<Column> relativeWidthColumns, ref float delta, bool resizeToFit, bool dragResize)
		{
			float num = Math.Abs(delta);
			bool flag = !resizeToFit && !dragResize;
			if (flag)
			{
				num = this.RecomputeToDesiredWidth(fixedColumns, num, true, true);
				num = this.RecomputeToDesiredWidth(relativeWidthColumns, num, true, true);
			}
			num = this.RecomputeToMinWidthProportionally(stretchableColumns, num, !resizeToFit && !dragResize);
			if (resizeToFit)
			{
				num = this.RecomputeToMinWidthProportionally(relativeWidthColumns, num, false);
				num = this.RecomputeToMinWidthProportionally(fixedColumns, num, false);
				num = this.RecomputeToMinWidth(relativeWidthColumns, num, false);
				num = this.RecomputeToMinWidth(fixedColumns, num, false);
			}
			else if (dragResize)
			{
				num = this.RecomputeToMinWidth(relativeWidthColumns, num, true);
				num = this.RecomputeToMinWidth(fixedColumns, num, true);
			}
			else
			{
				bool flag2 = num > 0f;
				if (flag2)
				{
					num = this.RecomputeToMinWidth(relativeWidthColumns, num, true);
					num = this.RecomputeToMinWidth(fixedColumns, num, true);
				}
			}
			delta = Math.Max(0f, delta - num);
		}

		private void DistributeExcess(List<Column> stretchableColumns, List<Column> fixedColumns, List<Column> relativeWidthColumns, ref float delta, bool resizeToFit, bool dragResize)
		{
			float num = Math.Abs(delta);
			bool flag = !resizeToFit && !dragResize;
			if (flag)
			{
				num = this.RecomputeToDesiredWidth(fixedColumns, num, true, false);
				num = this.RecomputeToDesiredWidth(relativeWidthColumns, num, true, false);
			}
			if (dragResize)
			{
				num = this.RecomputeToDesiredWidth(fixedColumns, num, true, false);
				num = this.RecomputeToDesiredWidth(relativeWidthColumns, num, true, false);
			}
			num = this.RecomputeToMaxWidthProportionally(stretchableColumns, num, !resizeToFit && !dragResize);
			if (resizeToFit)
			{
				num = this.RecomputeToMaxWidthProportionally(relativeWidthColumns, num, false);
				num = this.RecomputeToMaxWidthProportionally(fixedColumns, num, false);
				num = this.RecomputeToMaxWidth(relativeWidthColumns, num, false);
				num = this.RecomputeToMaxWidth(fixedColumns, num, false);
			}
			delta += num;
		}

		private float RecomputeToMaxWidthProportionally(List<Column> columns, float distributedDelta, bool setDesiredWidthOnly = false)
		{
			bool flag = distributedDelta > 0f;
			if (flag)
			{
				columns.Sort((Column c1, Column c2) => c1.GetMaxWidth(this.m_LayoutWidth).CompareTo(c2.GetMaxWidth(this.m_LayoutWidth)));
				float totalColumnWidth = 0f;
				columns.ForEach(delegate(Column c)
				{
					totalColumnWidth += this.GetDesiredWidth(c);
				});
				for (int i = 0; i < columns.Count; i++)
				{
					Column column = columns[i];
					float desiredWidth = this.GetDesiredWidth(column);
					float num = this.GetDesiredWidth(column) / totalColumnWidth;
					float num2 = distributedDelta * num;
					float num3 = 0f;
					float maxWidth = column.GetMaxWidth(this.m_LayoutWidth);
					bool flag2 = this.GetDesiredWidth(column) < maxWidth;
					if (flag2)
					{
						num3 = Math.Min(num2, maxWidth - this.GetDesiredWidth(column));
					}
					bool flag3 = num3 > 0f;
					if (flag3)
					{
						this.ResizeColumn(column, this.GetDesiredWidth(column) + num3, setDesiredWidthOnly);
					}
					totalColumnWidth -= desiredWidth;
					distributedDelta -= num3;
					bool flag4 = distributedDelta <= 0f;
					if (flag4)
					{
						break;
					}
				}
			}
			return distributedDelta;
		}

		private float RecomputeToMinWidthProportionally(List<Column> columns, float distributedDelta, bool setDesiredWidthOnly = false)
		{
			bool flag = distributedDelta > 0f;
			if (flag)
			{
				columns.Sort((Column c1, Column c2) => c2.GetMinWidth(this.m_LayoutWidth).CompareTo(c1.GetMinWidth(this.m_LayoutWidth)));
				float totalColumnsWidth = 0f;
				columns.ForEach(delegate(Column c)
				{
					totalColumnsWidth += this.GetDesiredWidth(c);
				});
				for (int i = 0; i < columns.Count; i++)
				{
					Column column = columns[i];
					float desiredWidth = this.GetDesiredWidth(column);
					float num = this.GetDesiredWidth(column) / totalColumnsWidth;
					float num2 = distributedDelta * num;
					float num3 = 0f;
					bool flag2 = this.GetDesiredWidth(column) > column.GetMinWidth(this.m_LayoutWidth);
					if (flag2)
					{
						num3 = Math.Min(num2, this.GetDesiredWidth(column) - column.GetMinWidth(this.m_LayoutWidth));
					}
					bool flag3 = num3 > 0f;
					if (flag3)
					{
						this.ResizeColumn(column, this.GetDesiredWidth(column) - num3, setDesiredWidthOnly);
					}
					totalColumnsWidth -= desiredWidth;
					distributedDelta -= num3;
					bool flag4 = distributedDelta <= 0f;
					if (flag4)
					{
						break;
					}
				}
			}
			return distributedDelta;
		}

		private float RecomputeToDesiredWidth(List<Column> columns, float distributedDelta, bool setDesiredWidthOnly, bool distributeOverflow)
		{
			if (distributeOverflow)
			{
				for (int i = columns.Count - 1; i >= 0; i--)
				{
					distributedDelta = this.RecomputeToDesiredWidth(columns[i], distributedDelta, setDesiredWidthOnly, true);
					bool flag = distributedDelta <= 0f;
					if (flag)
					{
						break;
					}
				}
			}
			else
			{
				for (int j = 0; j < columns.Count; j++)
				{
					distributedDelta = this.RecomputeToDesiredWidth(columns[j], distributedDelta, setDesiredWidthOnly, false);
					bool flag2 = distributedDelta <= 0f;
					if (flag2)
					{
						break;
					}
				}
			}
			return distributedDelta;
		}

		private float RecomputeToDesiredWidth(Column column, float distributedDelta, bool setDesiredWidthOnly, bool distributeOverflow)
		{
			float num = 0f;
			float num2 = Mathf.Clamp(column.GetWidth(this.m_LayoutWidth), column.GetMinWidth(this.m_LayoutWidth), column.GetMaxWidth(this.m_LayoutWidth));
			bool flag = this.GetDesiredWidth(column) > num2 && distributeOverflow;
			if (flag)
			{
				num = Math.Min(distributedDelta, Math.Abs(this.GetDesiredWidth(column) - num2));
			}
			bool flag2 = this.GetDesiredWidth(column) < num2 && !distributeOverflow;
			if (flag2)
			{
				num = Math.Min(distributedDelta, Math.Abs(num2 - this.GetDesiredWidth(column)));
			}
			float num3 = (distributeOverflow ? (this.GetDesiredWidth(column) - num) : (this.GetDesiredWidth(column) + num));
			bool flag3 = num > 0f;
			if (flag3)
			{
				this.ResizeColumn(column, num3, setDesiredWidthOnly);
			}
			distributedDelta -= num;
			return distributedDelta;
		}

		private float RecomputeToMinWidth(List<Column> columns, float distributedDelta, bool setDesiredWidthOnly = false)
		{
			bool flag = distributedDelta > 0f;
			if (flag)
			{
				for (int i = columns.Count - 1; i >= 0; i--)
				{
					Column column = columns[i];
					float num = 0f;
					bool flag2 = this.GetDesiredWidth(column) > column.GetMinWidth(this.m_LayoutWidth);
					if (flag2)
					{
						num = Math.Min(distributedDelta, this.GetDesiredWidth(column) - column.GetMinWidth(this.m_LayoutWidth));
					}
					bool flag3 = num > 0f;
					if (flag3)
					{
						this.ResizeColumn(column, this.GetDesiredWidth(column) - num, setDesiredWidthOnly);
					}
					distributedDelta -= num;
					bool flag4 = distributedDelta <= 0f;
					if (flag4)
					{
						break;
					}
				}
			}
			return distributedDelta;
		}

		private float RecomputeToMaxWidth(List<Column> columns, float distributedDelta, bool setDesiredWidthOnly = false)
		{
			bool flag = distributedDelta > 0f;
			if (flag)
			{
				for (int i = 0; i < columns.Count; i++)
				{
					Column column = columns[i];
					float num = 0f;
					bool flag2 = this.GetDesiredWidth(column) < column.GetMaxWidth(this.m_LayoutWidth);
					if (flag2)
					{
						num = Math.Min(distributedDelta, Math.Abs(column.GetMaxWidth(this.m_LayoutWidth) - this.GetDesiredWidth(column)));
					}
					bool flag3 = num > 0f;
					if (flag3)
					{
						this.ResizeColumn(column, this.GetDesiredWidth(column) + num, setDesiredWidthOnly);
					}
					distributedDelta -= num;
					bool flag4 = distributedDelta <= 0f;
					if (flag4)
					{
						break;
					}
				}
			}
			return distributedDelta;
		}

		public void ResizeToFit(float width)
		{
			float num = this.columnsWidth - Mathf.Clamp(width, this.minColumnsWidth, this.maxColumnsWidth);
			List<Column> list;
			using (CollectionPool<List<Column>, Column>.Get(out list))
			{
				List<Column> list2;
				using (CollectionPool<List<Column>, Column>.Get(out list2))
				{
					List<Column> list3;
					using (CollectionPool<List<Column>, Column>.Get(out list3))
					{
						list.AddRange(this.m_StretchableColumns);
						list2.AddRange(this.m_FixedColumns);
						list3.AddRange(this.m_RelativeWidthColumns);
						this.StretchResizeColumns(list, list2, list3, ref num, true, false);
						bool isDirty = this.m_IsDirty;
						if (isDirty)
						{
							this.UpdateCache();
						}
					}
				}
			}
		}

		private void ResizeColumn(Column column, float width, bool setDesiredWidthOnly = false)
		{
			Length length = new Length(width / this.layoutWidth * 100f, LengthUnit.Percent);
			bool dragResizeInPreviewMode = this.m_DragResizeInPreviewMode;
			if (dragResizeInPreviewMode)
			{
				this.m_PreviewDesiredWidths[column] = width;
			}
			else
			{
				bool flag = !setDesiredWidthOnly;
				if (flag)
				{
					column.width = ((column.width.unit == LengthUnit.Percent) ? length : width);
				}
				column.desiredWidth = width;
			}
		}

		internal void BeginDragResize(Column column, float pos, bool previewMode)
		{
			bool isDirty = this.m_IsDirty;
			if (isDirty)
			{
				throw new Exception("Cannot begin resizing columns because the layout needs to be updated");
			}
			this.m_DragResizeInPreviewMode = previewMode;
			this.m_DragResizing = true;
			int visibleIndex = column.visibleIndex;
			this.m_DragStartPos = pos;
			this.m_DragLastPos = pos;
			this.m_DragInitialColumnWidth = column.desiredWidth;
			this.m_DragStretchableColumns.Clear();
			this.m_DragFixedColumns.Clear();
			this.m_DragRelativeColumns.Clear();
			bool dragResizeInPreviewMode = this.m_DragResizeInPreviewMode;
			if (dragResizeInPreviewMode)
			{
				bool flag = this.m_PreviewDesiredWidths == null;
				if (flag)
				{
					this.m_PreviewDesiredWidths = new Dictionary<Column, float>();
				}
				this.m_PreviewDesiredWidths[column] = column.desiredWidth;
			}
			for (int i = visibleIndex + 1; i < this.m_Columns.visibleList.Count<Column>(); i++)
			{
				Column column2 = this.m_Columns.visibleList.ElementAt<Column>(i);
				bool flag2 = !column2.visible;
				if (!flag2)
				{
					bool stretchable = column2.stretchable;
					if (stretchable)
					{
						this.m_DragStretchableColumns.Add(column2);
					}
					else
					{
						bool flag3 = column2.width.unit == LengthUnit.Percent;
						if (flag3)
						{
							this.m_DragRelativeColumns.Add(column2);
						}
						else
						{
							this.m_DragFixedColumns.Add(column2);
						}
					}
					bool dragResizeInPreviewMode2 = this.m_DragResizeInPreviewMode;
					if (dragResizeInPreviewMode2)
					{
						this.m_PreviewDesiredWidths[column2] = column2.desiredWidth;
					}
				}
			}
		}

		public float GetDesiredPosition(Column column)
		{
			bool flag = !column.visible;
			float num;
			if (flag)
			{
				num = float.NaN;
			}
			else
			{
				float num2 = 0f;
				for (int i = 0; i < column.visibleIndex; i++)
				{
					Column column2 = this.m_Columns.visibleList.ElementAt<Column>(i);
					float desiredWidth = this.GetDesiredWidth(column2);
					bool flag2 = float.IsNaN(desiredWidth);
					if (!flag2)
					{
						num2 += desiredWidth;
					}
				}
				num = num2;
			}
			return num;
		}

		public float GetDesiredWidth(Column c)
		{
			bool flag = this.m_DragResizeInPreviewMode && this.m_PreviewDesiredWidths.ContainsKey(c);
			float num;
			if (flag)
			{
				num = this.m_PreviewDesiredWidths[c];
			}
			else
			{
				num = c.desiredWidth;
			}
			return num;
		}

		public void DragResize(Column column, float pos)
		{
			float minWidth = column.GetMinWidth(this.m_LayoutWidth);
			float maxWidth = column.GetMaxWidth(this.m_LayoutWidth);
			bool flag = this.m_Columns.stretchMode == Columns.StretchMode.GrowAndFill;
			if (flag)
			{
				float num = pos - this.m_DragLastPos;
				float num2 = Mathf.Clamp(this.GetDesiredWidth(column) + num, minWidth, maxWidth);
				num = num2 - this.GetDesiredWidth(column);
				bool flag2 = this.m_DragStretchableColumns.Count == 0 && num < 0f;
				if (flag2)
				{
					this.StretchResizeColumns(this.m_DragStretchableColumns, this.m_DragFixedColumns, this.m_DragRelativeColumns, ref num, false, true);
					num2 = Mathf.Clamp(this.GetDesiredWidth(column) + num2 - this.GetDesiredWidth(column), minWidth, maxWidth);
				}
				else
				{
					bool flag3 = num > 0f && this.columnsWidth + num < this.m_LayoutWidth;
					if (flag3)
					{
						float num3 = ((num < this.m_LayoutWidth - this.columnsWidth) ? 0f : (num - (this.m_LayoutWidth - this.columnsWidth)));
						this.StretchResizeColumns(this.m_DragStretchableColumns, this.m_DragFixedColumns, this.m_DragRelativeColumns, ref num3, false, true);
						num2 = Mathf.Clamp(this.GetDesiredWidth(column) + num - num3, minWidth, maxWidth);
					}
					else
					{
						this.StretchResizeColumns(this.m_DragStretchableColumns, this.m_DragFixedColumns, this.m_DragRelativeColumns, ref num, false, true);
						num2 = Mathf.Clamp(this.GetDesiredWidth(column) + num, minWidth, maxWidth);
					}
				}
				this.ResizeColumn(column, num2, false);
			}
			else
			{
				float num4 = pos - this.m_DragStartPos;
				float num5 = Math.Max(minWidth, Math.Min(maxWidth, this.m_DragInitialColumnWidth + num4));
				this.ResizeColumn(column, num5, false);
			}
			this.m_DragLastPos = pos;
		}

		internal void EndDragResize(Column column, bool cancelled)
		{
			bool dragResizeInPreviewMode = this.m_DragResizeInPreviewMode;
			if (dragResizeInPreviewMode)
			{
				this.m_DragResizeInPreviewMode = false;
				bool flag = !cancelled;
				if (flag)
				{
					foreach (KeyValuePair<Column, float> keyValuePair in this.m_PreviewDesiredWidths)
					{
						this.ResizeColumn(keyValuePair.Key, keyValuePair.Value, keyValuePair.Key != column);
					}
				}
				this.m_PreviewDesiredWidths.Clear();
			}
			this.m_DragResizing = false;
			this.m_DragStretchableColumns.Clear();
			this.m_DragFixedColumns.Clear();
			this.m_DragRelativeColumns.Clear();
		}

		private void UpdateCache()
		{
			this.ClearCache();
			foreach (Column column in this.m_Columns.visibleList)
			{
				bool flag = column.stretchable && this.columns.stretchMode == Columns.StretchMode.GrowAndFill;
				if (flag)
				{
					this.m_StretchableColumns.Add(column);
				}
				else
				{
					bool flag2 = column.width.unit == LengthUnit.Pixel;
					if (flag2)
					{
						this.m_FixedColumns.Add(column);
					}
				}
				bool flag3 = column.width.unit == LengthUnit.Percent;
				if (flag3)
				{
					this.m_RelativeWidthColumns.Add(column);
				}
				bool flag4 = column.width.unit == LengthUnit.Pixel && (column.minWidth.unit == LengthUnit.Percent || column.maxWidth.unit == LengthUnit.Percent);
				if (flag4)
				{
					this.m_MixedWidthColumns.Add(column);
				}
				this.m_MaxColumnsWidth += column.GetMaxWidth(this.m_LayoutWidth);
				this.m_MinColumnsWidth += column.GetMinWidth(this.m_LayoutWidth);
			}
		}

		private void UpdateMinAndMaxColumnsWidth()
		{
			this.m_MaxColumnsWidth = 0f;
			this.m_MinColumnsWidth = 0f;
			foreach (Column column in this.m_Columns.visibleList)
			{
				this.m_MaxColumnsWidth += column.GetMaxWidth(this.m_LayoutWidth);
				this.m_MinColumnsWidth += column.GetMinWidth(this.m_LayoutWidth);
			}
		}

		private void ClearCache()
		{
			this.m_StretchableColumns.Clear();
			this.m_RelativeWidthColumns.Clear();
			this.m_FixedColumns.Clear();
			this.m_MaxColumnsWidth = 0f;
			this.m_MinColumnsWidth = 0f;
			this.m_ColumnsWidthDirty = true;
		}

		private List<Column> m_StretchableColumns = new List<Column>();

		private List<Column> m_FixedColumns = new List<Column>();

		private List<Column> m_RelativeWidthColumns = new List<Column>();

		private List<Column> m_MixedWidthColumns = new List<Column>();

		private Columns m_Columns;

		private float m_ColumnsWidth = 0f;

		private bool m_ColumnsWidthDirty = true;

		private float m_MaxColumnsWidth = 0f;

		private float m_MinColumnsWidth = 0f;

		private bool m_IsDirty = false;

		private float m_PreviousWidth = float.NaN;

		private float m_LayoutWidth = float.NaN;

		private bool m_DragResizeInPreviewMode;

		private bool m_DragResizing = false;

		private float m_DragStartPos;

		private float m_DragLastPos;

		private float m_DragInitialColumnWidth;

		private List<Column> m_DragStretchableColumns = new List<Column>();

		private List<Column> m_DragRelativeColumns = new List<Column>();

		private List<Column> m_DragFixedColumns = new List<Column>();

		private Dictionary<Column, float> m_PreviewDesiredWidths;
	}
}
