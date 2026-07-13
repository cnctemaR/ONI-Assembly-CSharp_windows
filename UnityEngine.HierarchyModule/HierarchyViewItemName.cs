using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Bindings;
using UnityEngine.UIElements;

namespace Unity.Hierarchy
{
	[UxmlElement]
	[VisibleToOtherModules(new string[] { "UnityEditor.HierarchyModule" })]
	internal class HierarchyViewItemName : VisualElement
	{
		public string Text
		{
			get
			{
				return this.Label.text;
			}
			set
			{
				this.Label.text = value;
			}
		}

		internal bool IsRenaming { get; set; }

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action OnBeginRename;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<string, bool> OnEndRename;

		public Label Label { get; } = new Label();

		private TextField TextField { get; } = new TextField();

		public HierarchyViewItemName()
		{
			base.AddToClassList("hierarchy-item__name");
			this.focusable = true;
			base.delegatesFocus = false;
			this.m_PrewarmControl = false;
			base.Add(this.Label);
			base.Add(this.TextField);
			this.TextField.selectAllOnFocus = true;
			this.TextField.selectAllOnMouseUp = false;
			this.TextField.style.display = DisplayStyle.None;
			this.TextField.RegisterCallback<MouseUpEvent>(new EventCallback<MouseUpEvent>(this.OnMouseUpEvent), TrickleDown.NoTrickleDown);
			this.TextField.RegisterCallback<KeyDownEvent>(new EventCallback<KeyDownEvent>(this.OnInterceptKeyDownEvent), TrickleDown.TrickleDown);
			this.TextField.RegisterCallback<KeyDownEvent>(new EventCallback<KeyDownEvent>(this.OnKeyDownEvent), TrickleDown.NoTrickleDown);
			this.TextField.RegisterCallback<BlurEvent>(new EventCallback<BlurEvent>(this.OnBlurEvent), TrickleDown.NoTrickleDown);
		}

		public void BeginRename()
		{
			bool isRenaming = this.IsRenaming;
			if (!isRenaming)
			{
				this.IsRenaming = true;
				base.delegatesFocus = true;
				this.m_PrewarmControl = true;
				this.Label.style.display = DisplayStyle.None;
				this.TextField.style.display = DisplayStyle.Flex;
				this.TextField.value = this.Text;
				this.TextField.Q<TextElement>(null, null).Focus();
				Action onBeginRename = this.OnBeginRename;
				if (onBeginRename != null)
				{
					onBeginRename();
				}
			}
		}

		public void CancelRename()
		{
			bool isRenaming = this.IsRenaming;
			if (isRenaming)
			{
				this.EndRename(true);
			}
		}

		private void EndRename(bool canceled = false)
		{
			this.IsRenaming = false;
			base.delegatesFocus = false;
			this.m_PrewarmControl = false;
			this.TextField.style.display = DisplayStyle.None;
			this.Label.style.display = DisplayStyle.Flex;
			bool flag = !canceled && !string.IsNullOrEmpty(this.TextField.value);
			if (flag)
			{
				this.Label.text = this.TextField.value;
			}
			Action<string, bool> onEndRename = this.OnEndRename;
			if (onEndRename != null)
			{
				onEndRename(this.Text, canceled);
			}
		}

		private void OnMouseUpEvent(MouseUpEvent evt)
		{
			bool flag = !this.IsRenaming;
			if (!flag)
			{
				this.TextField.Q<TextElement>(null, null).Focus();
				evt.StopPropagation();
			}
		}

		private void OnInterceptKeyDownEvent(KeyDownEvent evt)
		{
			bool flag = !this.m_PrewarmControl;
			if (!flag)
			{
				bool flag2 = evt.keyCode == KeyCode.None;
				if (flag2)
				{
					evt.StopPropagation();
				}
				else
				{
					this.m_PrewarmControl = false;
				}
			}
		}

		private void OnKeyDownEvent(KeyDownEvent evt)
		{
			bool flag = this.IsRenaming && evt.keyCode == KeyCode.Escape;
			if (flag)
			{
				this.EndRename(true);
			}
			evt.StopPropagation();
		}

		private void OnBlurEvent(BlurEvent evt)
		{
			bool flag = !this.IsRenaming;
			if (!flag)
			{
				this.EndRename(false);
			}
		}

		internal const string k_StyleName = "hierarchy-item__name";

		private bool m_PrewarmControl;
	}
}
