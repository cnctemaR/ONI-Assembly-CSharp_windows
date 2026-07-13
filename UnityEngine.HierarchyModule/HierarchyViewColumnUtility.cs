using System;
using System.Collections.Generic;
using UnityEngine.Bindings;
using UnityEngine.UIElements;

namespace Unity.Hierarchy
{
	[VisibleToOtherModules(new string[] { "UnityEditor.HierarchyModule", "UnityEditor.UIToolkitAuthoringModule" })]
	internal class HierarchyViewColumnUtility
	{
		public static HierarchyViewCell GetCellFromTarget(VisualElement target)
		{
			return (HierarchyViewCell)target.parent;
		}

		public static HierarchyViewCellValueEditor<TModel, TEditor, TValue> BindCellToValueEditor<TModel, TEditor, TValue>(TModel model, HierarchyViewCell cell, HierarchyViewColumnContextPool<HierarchyViewCellValueEditor<TModel, TEditor, TValue>> pool, params string[] classes) where TEditor : VisualElement, INotifyValueChanged<TValue>, new()
		{
			TEditor orCreateEditor = HierarchyViewColumnUtility.GetOrCreateEditor<TEditor>(cell, classes);
			HierarchyViewCellValueEditor<TModel, TEditor, TValue> hierarchyViewCellValueEditor = pool.Get(cell.View.GetHashCode());
			hierarchyViewCellValueEditor.Bind(model, cell, orCreateEditor);
			return hierarchyViewCellValueEditor;
		}

		public static void UnbindCellFromValueEditor<TModel, TEditor, TValue>(HierarchyViewCell cell, HierarchyViewColumnContextPool<HierarchyViewCellValueEditor<TModel, TEditor, TValue>> pool) where TEditor : VisualElement, INotifyValueChanged<TValue>, new()
		{
			HierarchyViewCellValueEditor<TModel, TEditor, TValue> hierarchyViewCellValueEditor = cell.userData as HierarchyViewCellValueEditor<TModel, TEditor, TValue>;
			bool flag = hierarchyViewCellValueEditor != null;
			if (flag)
			{
				pool.Release(cell.View.GetHashCode(), hierarchyViewCellValueEditor);
				hierarchyViewCellValueEditor.Unbind();
			}
		}

		public static TEditor GetOrCreateEditor<TEditor>(HierarchyViewCell cell, params string[] classes) where TEditor : VisualElement, new()
		{
			TEditor teditor = cell.Q<TEditor>(null, null);
			bool flag = teditor == null;
			if (flag)
			{
				teditor = new TEditor();
				HierarchyViewColumnUtility.AddToClassList(teditor, classes);
				cell.Add(teditor);
			}
			return teditor;
		}

		internal static VisualElement AddToClassList(VisualElement element, params string[] classes)
		{
			foreach (string text in classes)
			{
				element.AddToClassList(text);
			}
			return element;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIToolkitAuthoringModule" })]
		internal static HierarchyViewCellValueEditor<TModel, TEditor, TValue> CreateCellValueEditor<TModel, TEditor, TValue>(TModel model, HierarchyViewCell cell, Func<HierarchyViewCellValueEditor<TModel, TEditor, TValue>, TValue> getModelValue, Action<HierarchyViewCellValueEditor<TModel, TEditor, TValue>, TValue> setModelValue, Func<HierarchyViewCellValueEditor<TModel, TEditor, TValue>, TValue, bool> isDefaultValue, params string[] classes) where TEditor : VisualElement, INotifyValueChanged<TValue>, new()
		{
			TEditor orCreateEditor = HierarchyViewColumnUtility.GetOrCreateEditor<TEditor>(cell, classes);
			HierarchyViewCellValueEditor<TModel, TEditor, TValue> hierarchyViewCellValueEditor = new HierarchyViewCellValueEditor<TModel, TEditor, TValue>(getModelValue, setModelValue, isDefaultValue, null);
			hierarchyViewCellValueEditor.Bind(model, cell, orCreateEditor);
			return hierarchyViewCellValueEditor;
		}

		internal static int GetVisibleIndex(HierarchyViewState viewState, Column c)
		{
			string columnId = HierarchyViewColumnUtility.GetColumnId(c);
			foreach (HierarchyViewColumnState hierarchyViewColumnState in viewState.Columns)
			{
				bool flag = hierarchyViewColumnState.ColumnId == columnId;
				if (flag)
				{
					return hierarchyViewColumnState.Index;
				}
			}
			return HierarchyViewColumnUtility.GetColumnDefaultPriority(c);
		}

		internal static string GetColumnId(Column col)
		{
			HierarchyViewColumn hierarchyViewColumn = col as HierarchyViewColumn;
			bool flag = hierarchyViewColumn != null;
			string text;
			if (flag)
			{
				text = hierarchyViewColumn.Descriptor.Id;
			}
			else
			{
				bool flag2 = col is HierarchyViewItemColumn;
				if (flag2)
				{
					text = "HierarchyViewColumn Name";
				}
				else
				{
					text = null;
				}
			}
			return text;
		}

		internal static int GetColumnDefaultPriority(Column col)
		{
			HierarchyViewColumn hierarchyViewColumn = col as HierarchyViewColumn;
			bool flag = hierarchyViewColumn != null;
			int num;
			if (flag)
			{
				num = hierarchyViewColumn.Descriptor.DefaultPriority;
			}
			else
			{
				bool flag2 = col is HierarchyViewItemColumn;
				if (flag2)
				{
					num = 0;
				}
				else
				{
					num = 1000;
				}
			}
			return num;
		}

		internal static Column GetColumnWithId(IEnumerable<Column> columns, string id)
		{
			foreach (Column column in columns)
			{
				bool flag = HierarchyViewColumnUtility.GetColumnId(column) == id;
				if (flag)
				{
					return column;
				}
			}
			return null;
		}

		public const string k_ToggleIcon = "toggle-icon";

		public const string k_CellPropField = "cell-prop-field";
	}
}
