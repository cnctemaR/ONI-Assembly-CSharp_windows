using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Properties;
using UnityEngine.Internal;

namespace UnityEngine.UIElements
{
	[UxmlObject]
	public class Columns : ICollection<Column>, IEnumerable<Column>, IEnumerable, INotifyBindablePropertyChanged
	{
		internal IList<Column> columns
		{
			get
			{
				return this.m_Columns;
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;

		[CreateProperty]
		public string primaryColumnName
		{
			get
			{
				return this.m_PrimaryColumnName;
			}
			set
			{
				bool flag = this.m_PrimaryColumnName == value;
				if (!flag)
				{
					this.m_PrimaryColumnName = value;
					this.NotifyChange(ColumnsDataType.PrimaryColumn);
					this.NotifyPropertyChanged(in Columns.primaryColumnNameProperty);
				}
			}
		}

		[CreateProperty]
		public bool reorderable
		{
			get
			{
				return this.m_Reorderable;
			}
			set
			{
				bool flag = this.m_Reorderable == value;
				if (!flag)
				{
					this.m_Reorderable = value;
					this.NotifyChange(ColumnsDataType.Reorderable);
					this.NotifyPropertyChanged(in Columns.reorderableProperty);
				}
			}
		}

		[CreateProperty]
		public bool resizable
		{
			get
			{
				return this.m_Resizable;
			}
			set
			{
				bool flag = this.m_Resizable == value;
				if (!flag)
				{
					this.m_Resizable = value;
					this.NotifyChange(ColumnsDataType.Resizable);
					this.NotifyPropertyChanged(in Columns.resizableProperty);
				}
			}
		}

		[CreateProperty]
		public bool resizePreview
		{
			get
			{
				return this.m_ResizePreview;
			}
			set
			{
				bool flag = this.m_ResizePreview == value;
				if (!flag)
				{
					this.m_ResizePreview = value;
					this.NotifyChange(ColumnsDataType.ResizePreview);
					this.NotifyPropertyChanged(in Columns.resizePreviewProperty);
				}
			}
		}

		internal IEnumerable<Column> displayList
		{
			get
			{
				this.InitOrderColumns();
				return this.m_DisplayColumns;
			}
		}

		internal IEnumerable<Column> visibleList
		{
			get
			{
				this.UpdateVisibleColumns();
				return this.m_VisibleColumns;
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal event Action<ColumnsDataType> changed;

		[CreateProperty]
		public Columns.StretchMode stretchMode
		{
			get
			{
				return this.m_StretchMode;
			}
			set
			{
				bool flag = this.m_StretchMode == value;
				if (!flag)
				{
					this.m_StretchMode = value;
					this.NotifyChange(ColumnsDataType.StretchMode);
					this.NotifyPropertyChanged(in Columns.stretchModeProperty);
				}
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal event Action<Column, int> columnAdded;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal event Action<Column> columnRemoved;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal event Action<Column, ColumnDataType> columnChanged;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal event Action<Column> columnResized;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal event Action<Column, int, int> columnReordered;

		public bool IsPrimary(Column column)
		{
			return this.primaryColumnName == column.name || (string.IsNullOrEmpty(this.primaryColumnName) && column.visibleIndex == 0);
		}

		public IEnumerator<Column> GetEnumerator()
		{
			return this.m_Columns.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public void Add(Column item)
		{
			this.Insert(this.m_Columns.Count, item);
		}

		public void Clear()
		{
			while (this.m_Columns.Count > 0)
			{
				this.Remove(this.m_Columns[this.m_Columns.Count - 1]);
			}
		}

		public bool Contains(Column item)
		{
			return this.m_Columns.Contains(item);
		}

		public bool Contains(string name)
		{
			foreach (Column column in this.m_Columns)
			{
				bool flag = column.name == name;
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		public void CopyTo(Column[] array, int arrayIndex)
		{
			this.m_Columns.CopyTo(array, arrayIndex);
		}

		public bool Remove(Column column)
		{
			bool flag = column == null;
			if (flag)
			{
				throw new ArgumentException("Cannot remove null column");
			}
			bool flag2 = this.m_Columns.Remove(column);
			bool flag3;
			if (flag2)
			{
				List<Column> displayColumns = this.m_DisplayColumns;
				if (displayColumns != null)
				{
					displayColumns.Remove(column);
				}
				List<Column> visibleColumns = this.m_VisibleColumns;
				if (visibleColumns != null)
				{
					visibleColumns.Remove(column);
				}
				column.collection = null;
				column.propertyChanged -= this.OnColumnsPropertyChanged;
				column.changed -= this.OnColumnChanged;
				column.resized -= this.OnColumnResized;
				Action<Column> action = this.columnRemoved;
				if (action != null)
				{
					action(column);
				}
				flag3 = true;
			}
			else
			{
				flag3 = false;
			}
			return flag3;
		}

		private void OnColumnsPropertyChanged(object sender, BindablePropertyChangedEventArgs args)
		{
			Column column = (Column)sender;
			int num = this.m_Columns.IndexOf(column);
			bool flag = num > 0;
			if (flag)
			{
				string text = string.Format("columns[{0}].{1}", num, args.propertyName);
				BindingId bindingId = text;
				this.NotifyPropertyChanged(in bindingId);
			}
		}

		private void OnColumnChanged(Column column, ColumnDataType type)
		{
			bool flag = type == ColumnDataType.Visibility;
			if (flag)
			{
				this.DirtyVisibleColumns();
			}
			Action<Column, ColumnDataType> action = this.columnChanged;
			if (action != null)
			{
				action(column, type);
			}
		}

		private void OnColumnResized(Column column)
		{
			Action<Column> action = this.columnResized;
			if (action != null)
			{
				action(column);
			}
		}

		public int Count
		{
			get
			{
				return this.m_Columns.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return this.m_Columns.IsReadOnly;
			}
		}

		public int IndexOf(Column column)
		{
			return this.m_Columns.IndexOf(column);
		}

		public void Insert(int index, Column column)
		{
			bool flag = column == null;
			if (flag)
			{
				throw new ArgumentException("Cannot insert null column");
			}
			bool flag2 = column.collection == this;
			if (flag2)
			{
				throw new ArgumentException("Already contains this column");
			}
			bool flag3 = column.collection != null;
			if (flag3)
			{
				column.collection.Remove(column);
			}
			this.m_Columns.Insert(index, column);
			bool flag4 = this.m_DisplayColumns != null;
			if (flag4)
			{
				this.m_DisplayColumns.Insert(index, column);
				this.DirtyVisibleColumns();
			}
			column.collection = this;
			column.propertyChanged += this.OnColumnsPropertyChanged;
			column.changed += this.OnColumnChanged;
			column.resized += this.OnColumnResized;
			Action<Column, int> action = this.columnAdded;
			if (action != null)
			{
				action(column, index);
			}
		}

		public void RemoveAt(int index)
		{
			this.Remove(this.m_Columns[index]);
		}

		public Column this[int index]
		{
			get
			{
				return this.m_Columns[index];
			}
		}

		public Column this[string name]
		{
			get
			{
				foreach (Column column in this.m_Columns)
				{
					bool flag = column.name == name;
					if (flag)
					{
						return column;
					}
				}
				return null;
			}
		}

		public void ReorderDisplay(int from, int to)
		{
			this.InitOrderColumns();
			Column column = this.m_DisplayColumns[from];
			this.m_DisplayColumns.RemoveAt(from);
			this.m_DisplayColumns.Insert(to, column);
			this.DirtyVisibleColumns();
			Action<Column, int, int> action = this.columnReordered;
			if (action != null)
			{
				action(column, from, to);
			}
		}

		private void InitOrderColumns()
		{
			bool flag = this.m_DisplayColumns == null;
			if (flag)
			{
				this.m_DisplayColumns = new List<Column>(this);
			}
		}

		private void DirtyVisibleColumns()
		{
			this.m_VisibleColumnsDirty = true;
			bool flag = this.m_VisibleColumns != null;
			if (flag)
			{
				this.m_VisibleColumns.Clear();
			}
		}

		private void UpdateVisibleColumns()
		{
			bool flag = !this.m_VisibleColumnsDirty;
			if (!flag)
			{
				this.InitOrderColumns();
				bool flag2 = this.m_VisibleColumns == null;
				if (flag2)
				{
					this.m_VisibleColumns = new List<Column>(this.m_Columns.Count);
				}
				this.m_VisibleColumns.AddRange(this.m_DisplayColumns.FindAll((Column c) => c.visible));
				this.m_VisibleColumnsDirty = false;
			}
		}

		private void NotifyChange(ColumnsDataType type)
		{
			Action<ColumnsDataType> action = this.changed;
			if (action != null)
			{
				action(type);
			}
		}

		private void NotifyPropertyChanged(in BindingId property)
		{
			EventHandler<BindablePropertyChangedEventArgs> eventHandler = this.propertyChanged;
			if (eventHandler != null)
			{
				eventHandler(this, new BindablePropertyChangedEventArgs(in property));
			}
		}

		private static readonly BindingId primaryColumnNameProperty = "primaryColumnName";

		private static readonly BindingId reorderableProperty = "reorderable";

		private static readonly BindingId resizableProperty = "resizable";

		private static readonly BindingId resizePreviewProperty = "resizePreview";

		private static readonly BindingId stretchModeProperty = "stretchMode";

		private IList<Column> m_Columns = new List<Column>();

		private List<Column> m_DisplayColumns;

		private List<Column> m_VisibleColumns;

		private bool m_VisibleColumnsDirty = true;

		private Columns.StretchMode m_StretchMode = Columns.StretchMode.GrowAndFill;

		private bool m_Reorderable = true;

		private bool m_Resizable = true;

		private bool m_ResizePreview;

		private string m_PrimaryColumnName;

		public enum StretchMode
		{
			Grow,
			GrowAndFill
		}

		[ExcludeFromDocs]
		[Serializable]
		public class UxmlSerializedData : UnityEngine.UIElements.UxmlSerializedData
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				UxmlDescriptionCache.RegisterType(typeof(Columns.UxmlSerializedData), new UxmlAttributeNames[]
				{
					new UxmlAttributeNames("primaryColumnName", "primary-column-name", null, Array.Empty<string>()),
					new UxmlAttributeNames("stretchMode", "stretch-mode", null, Array.Empty<string>()),
					new UxmlAttributeNames("reorderable", "reorderable", null, Array.Empty<string>()),
					new UxmlAttributeNames("resizable", "resizable", null, Array.Empty<string>()),
					new UxmlAttributeNames("resizePreview", "resize-preview", null, Array.Empty<string>()),
					new UxmlAttributeNames("columns", "columns", null, Array.Empty<string>())
				}, false);
			}

			public override object CreateInstance()
			{
				return new Columns();
			}

			public override void Deserialize(object obj)
			{
				Columns columns = (Columns)obj;
				bool flag = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.primaryColumnName_UxmlAttributeFlags);
				if (flag)
				{
					columns.primaryColumnName = this.primaryColumnName;
				}
				bool flag2 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.stretchMode_UxmlAttributeFlags);
				if (flag2)
				{
					columns.stretchMode = this.stretchMode;
				}
				bool flag3 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.reorderable_UxmlAttributeFlags);
				if (flag3)
				{
					columns.reorderable = this.reorderable;
				}
				bool flag4 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.resizable_UxmlAttributeFlags);
				if (flag4)
				{
					columns.resizable = this.resizable;
				}
				bool flag5 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.resizePreview_UxmlAttributeFlags);
				if (flag5)
				{
					columns.resizePreview = this.resizePreview;
				}
				bool flag6 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.columns_UxmlAttributeFlags) && this.columns != null;
				if (flag6)
				{
					foreach (Column.UxmlSerializedData uxmlSerializedData in this.columns)
					{
						Column column = (Column)uxmlSerializedData.CreateInstance();
						uxmlSerializedData.Deserialize(column);
						columns.Add(column);
					}
				}
			}

			[SerializeField]
			private string primaryColumnName;

			[UxmlIgnore]
			[HideInInspector]
			[SerializeField]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags primaryColumnName_UxmlAttributeFlags;

			[SerializeField]
			private Columns.StretchMode stretchMode;

			[SerializeField]
			[HideInInspector]
			[UxmlIgnore]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags stretchMode_UxmlAttributeFlags;

			[SerializeField]
			private bool reorderable;

			[HideInInspector]
			[SerializeField]
			[UxmlIgnore]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags reorderable_UxmlAttributeFlags;

			[SerializeField]
			private bool resizable;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags resizable_UxmlAttributeFlags;

			[SerializeField]
			private bool resizePreview;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags resizePreview_UxmlAttributeFlags;

			[UxmlObjectReference]
			[SerializeReference]
			private List<Column.UxmlSerializedData> columns;

			[HideInInspector]
			[SerializeField]
			[UxmlIgnore]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags columns_UxmlAttributeFlags;
		}

		[Obsolete("UxmlObjectFactory<T> is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		internal class UxmlObjectFactory<T> : UxmlObjectFactory<T, Columns.UxmlObjectTraits<T>> where T : Columns, new()
		{
		}

		[Obsolete("UxmlObjectFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		internal class UxmlObjectFactory : Columns.UxmlObjectFactory<Columns>
		{
		}

		[Obsolete("UxmlObjectTraits<T> is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		internal class UxmlObjectTraits<T> : UnityEngine.UIElements.UxmlObjectTraits<T> where T : Columns
		{
			public override void Init(ref T obj, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ref obj, bag, cc);
				obj.primaryColumnName = this.m_PrimaryColumnName.GetValueFromBag(bag, cc);
				obj.stretchMode = this.m_StretchMode.GetValueFromBag(bag, cc);
				obj.reorderable = this.m_Reorderable.GetValueFromBag(bag, cc);
				obj.resizable = this.m_Resizable.GetValueFromBag(bag, cc);
				obj.resizePreview = this.m_ResizePreview.GetValueFromBag(bag, cc);
				List<Column> valueFromBag = this.m_Columns.GetValueFromBag(bag, cc);
				bool flag = valueFromBag != null;
				if (flag)
				{
					foreach (Column column in valueFromBag)
					{
						obj.Add(column);
					}
				}
			}

			private readonly UxmlStringAttributeDescription m_PrimaryColumnName = new UxmlStringAttributeDescription
			{
				name = "primary-column-name"
			};

			private readonly UxmlEnumAttributeDescription<Columns.StretchMode> m_StretchMode = new UxmlEnumAttributeDescription<Columns.StretchMode>
			{
				name = "stretch-mode",
				defaultValue = Columns.StretchMode.GrowAndFill
			};

			private readonly UxmlBoolAttributeDescription m_Reorderable = new UxmlBoolAttributeDescription
			{
				name = "reorderable",
				defaultValue = true
			};

			private readonly UxmlBoolAttributeDescription m_Resizable = new UxmlBoolAttributeDescription
			{
				name = "resizable",
				defaultValue = true
			};

			private readonly UxmlBoolAttributeDescription m_ResizePreview = new UxmlBoolAttributeDescription
			{
				name = "resize-preview"
			};

			private readonly UxmlObjectListAttributeDescription<Column> m_Columns = new UxmlObjectListAttributeDescription<Column>();
		}
	}
}
