using System;
using System.Diagnostics;

namespace UnityEngine.UIElements
{
	internal class ReusableTreeViewItem : ReusableCollectionItem
	{
		public override VisualElement rootElement
		{
			get
			{
				return this.m_Container ?? base.bindableElement;
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<PointerUpEvent> onPointerUp;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<ChangeEvent<bool>> onToggleValueChanged;

		internal float indentWidth
		{
			get
			{
				return this.m_IndentWidth;
			}
		}

		public ReusableTreeViewItem()
		{
			this.m_PointerUpCallback = new EventCallback<PointerUpEvent>(this.OnPointerUp);
			this.m_ToggleValueChangedCallback = new EventCallback<ChangeEvent<bool>>(this.OnToggleValueChanged);
			this.m_ToggleGeometryChangedCallback = new EventCallback<GeometryChangedEvent>(this.OnToggleGeometryChanged);
		}

		public override void Init(VisualElement item)
		{
			base.Init(item);
			VisualElement visualElement = new VisualElement
			{
				name = BaseTreeView.itemUssClassName
			};
			visualElement.AddToClassList(BaseTreeView.itemUssClassName);
			this.InitExpandHierarchy(visualElement, item);
		}

		protected void InitExpandHierarchy(VisualElement root, VisualElement item)
		{
			this.m_Container = root;
			this.m_Container.style.flexDirection = FlexDirection.Row;
			this.m_IndentElement = new VisualElement
			{
				name = BaseTreeView.itemIndentUssClassName,
				style = 
				{
					flexDirection = FlexDirection.Row
				}
			};
			this.m_Container.hierarchy.Add(this.m_IndentElement);
			this.m_Toggle = new Toggle
			{
				name = BaseTreeView.itemToggleUssClassName,
				userData = this
			};
			this.m_Toggle.AddToClassList(Foldout.toggleUssClassName);
			this.m_Toggle.AddToClassList(BaseTreeView.itemToggleUssClassName);
			this.m_Toggle.visualInput.AddToClassList(Foldout.inputUssClassName);
			this.m_Checkmark = this.m_Toggle.visualInput.Q(null, Toggle.checkmarkUssClassName);
			this.m_Checkmark.AddToClassList(Foldout.checkmarkUssClassName);
			this.m_Container.hierarchy.Add(this.m_Toggle);
			this.m_BindableContainer = new VisualElement
			{
				name = BaseTreeView.itemContentContainerUssClassName,
				style = 
				{
					flexGrow = 1f
				}
			};
			this.m_BindableContainer.AddToClassList(BaseTreeView.itemContentContainerUssClassName);
			this.m_Container.Add(this.m_BindableContainer);
			this.m_BindableContainer.Add(item);
		}

		public override void PreAttachElement()
		{
			base.PreAttachElement();
			this.rootElement.AddToClassList(BaseTreeView.itemUssClassName);
			VisualElement container = this.m_Container;
			if (container != null)
			{
				container.RegisterCallback<PointerUpEvent>(this.m_PointerUpCallback, TrickleDown.NoTrickleDown);
			}
			Toggle toggle = this.m_Toggle;
			if (toggle != null)
			{
				toggle.visualInput.Q(null, Toggle.checkmarkUssClassName).RegisterCallback<GeometryChangedEvent>(this.m_ToggleGeometryChangedCallback, TrickleDown.NoTrickleDown);
			}
			Toggle toggle2 = this.m_Toggle;
			if (toggle2 != null)
			{
				toggle2.RegisterValueChangedCallback<bool>(this.m_ToggleValueChangedCallback);
			}
		}

		public override void DetachElement()
		{
			base.DetachElement();
			this.rootElement.RemoveFromClassList(BaseTreeView.itemUssClassName);
			VisualElement container = this.m_Container;
			if (container != null)
			{
				container.UnregisterCallback<PointerUpEvent>(this.m_PointerUpCallback, TrickleDown.NoTrickleDown);
			}
			Toggle toggle = this.m_Toggle;
			if (toggle != null)
			{
				toggle.visualInput.Q(null, Toggle.checkmarkUssClassName).UnregisterCallback<GeometryChangedEvent>(this.m_ToggleGeometryChangedCallback, TrickleDown.NoTrickleDown);
			}
			Toggle toggle2 = this.m_Toggle;
			if (toggle2 != null)
			{
				toggle2.UnregisterValueChangedCallback<bool>(this.m_ToggleValueChangedCallback);
			}
		}

		public void Indent(int depth)
		{
			bool flag = this.m_IndentElement == null;
			if (!flag)
			{
				this.m_Depth = depth;
				this.UpdateIndentLayout();
			}
		}

		public void SetExpandedWithoutNotify(bool expanded)
		{
			Toggle toggle = this.m_Toggle;
			if (toggle != null)
			{
				toggle.SetValueWithoutNotify(expanded);
			}
		}

		public void SetToggleVisibility(bool visible)
		{
			bool flag = this.m_Toggle != null;
			if (flag)
			{
				this.m_Toggle.visible = visible;
			}
		}

		private void OnToggleGeometryChanged(GeometryChangedEvent evt)
		{
			float num = this.m_Checkmark.resolvedStyle.width + this.m_Checkmark.resolvedStyle.marginLeft + this.m_Checkmark.resolvedStyle.marginRight;
			bool flag = Math.Abs(num - this.m_IndentWidth) < float.Epsilon;
			if (!flag)
			{
				this.m_IndentWidth = num;
				this.UpdateIndentLayout();
			}
		}

		private void UpdateIndentLayout()
		{
			this.m_IndentElement.style.width = this.m_IndentWidth * (float)this.m_Depth;
			this.m_IndentElement.EnableInClassList(BaseTreeView.itemIndentUssClassName, this.m_Depth > 0);
		}

		private void OnPointerUp(PointerUpEvent evt)
		{
			Action<PointerUpEvent> action = this.onPointerUp;
			if (action != null)
			{
				action(evt);
			}
		}

		private void OnToggleValueChanged(ChangeEvent<bool> evt)
		{
			Action<ChangeEvent<bool>> action = this.onToggleValueChanged;
			if (action != null)
			{
				action(evt);
			}
		}

		private Toggle m_Toggle;

		private VisualElement m_Container;

		private VisualElement m_IndentElement;

		private VisualElement m_BindableContainer;

		private VisualElement m_Checkmark;

		private int m_Depth;

		private float m_IndentWidth;

		private EventCallback<PointerUpEvent> m_PointerUpCallback;

		private EventCallback<ChangeEvent<bool>> m_ToggleValueChangedCallback;

		private EventCallback<GeometryChangedEvent> m_ToggleGeometryChangedCallback;
	}
}
