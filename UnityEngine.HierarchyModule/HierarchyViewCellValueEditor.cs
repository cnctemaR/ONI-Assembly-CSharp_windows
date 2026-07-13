using System;
using UnityEngine.Bindings;
using UnityEngine.UIElements;

namespace Unity.Hierarchy
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIToolkitAuthoringModule" })]
	internal sealed class HierarchyViewCellValueEditor<TModel, TEditor, TValue> where TEditor : VisualElement, INotifyValueChanged<TValue>, new()
	{
		public TModel Model { get; private set; }

		public HierarchyViewCell Cell { get; private set; }

		public HierarchyViewCellValueEditor(Func<HierarchyViewCellValueEditor<TModel, TEditor, TValue>, TValue> getModelValue, Action<HierarchyViewCellValueEditor<TModel, TEditor, TValue>, TValue> setModelValue, Func<HierarchyViewCellValueEditor<TModel, TEditor, TValue>, TValue, bool> isDefaultValue, Action<HierarchyViewCellValueEditor<TModel, TEditor, TValue>, TValue> onSetEditorValue = null)
		{
			this.m_GetModelValue = getModelValue;
			this.m_SetModelValue = setModelValue;
			this.m_IsDefaultValue = isDefaultValue;
			this.m_OnSetEditorValue = onSetEditorValue;
		}

		public void Bind(TModel model, HierarchyViewCell cell, TEditor editor)
		{
			this.Model = model;
			this.Cell = cell;
			this.Cell.userData = this;
			this.Element = editor;
			this.Element.visible = true;
			this.Element.RegisterCallback<ChangeEvent<TValue>>(new EventCallback<ChangeEvent<TValue>>(this.SetModelValue), TrickleDown.NoTrickleDown);
			this.SyncEditorValueWithoutNotify();
		}

		public void Unbind()
		{
			this.Cell.userData = null;
			this.Cell = null;
			this.Element.visible = false;
			this.Element.UnregisterCallback<ChangeEvent<TValue>>(new EventCallback<ChangeEvent<TValue>>(this.SetModelValue), TrickleDown.NoTrickleDown);
			this.Element = default(TEditor);
		}

		public TValue GetModelValue()
		{
			return this.m_GetModelValue(this);
		}

		public void SetModelValue(TValue value)
		{
			bool flag = this.Cell == null;
			if (!flag)
			{
				TValue modelValue = this.GetModelValue();
				bool flag2 = !modelValue.Equals(value);
				if (flag2)
				{
					this.m_SetModelValue(this, value);
				}
				this.Cell.IsDefaultValue = this.IsModelDefaultValue();
			}
		}

		public TValue GetEditorValue()
		{
			return this.Element.value;
		}

		public void SetModelValue(ChangeEvent<TValue> evt)
		{
			this.SetModelValue(evt.newValue);
		}

		public void SetEditorValueWithoutNotify(TValue value)
		{
			bool flag = !value.Equals(this.Element.value);
			if (flag)
			{
				this.Element.SetValueWithoutNotify(value);
			}
			Action<HierarchyViewCellValueEditor<TModel, TEditor, TValue>, TValue> onSetEditorValue = this.m_OnSetEditorValue;
			if (onSetEditorValue != null)
			{
				onSetEditorValue(this, value);
			}
			this.Cell.IsDefaultValue = this.IsModelDefaultValue();
		}

		public void SyncEditorValueWithoutNotify()
		{
			this.SetEditorValueWithoutNotify(this.GetModelValue());
		}

		public bool IsModelDefaultValue()
		{
			return this.m_IsDefaultValue(this, this.GetModelValue());
		}

		private readonly Func<HierarchyViewCellValueEditor<TModel, TEditor, TValue>, TValue> m_GetModelValue;

		private readonly Action<HierarchyViewCellValueEditor<TModel, TEditor, TValue>, TValue> m_SetModelValue;

		private readonly Func<HierarchyViewCellValueEditor<TModel, TEditor, TValue>, TValue, bool> m_IsDefaultValue;

		private readonly Action<HierarchyViewCellValueEditor<TModel, TEditor, TValue>, TValue> m_OnSetEditorValue;

		public TEditor Element;
	}
}
