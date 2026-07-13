using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace UnityEngine.Accessibility
{
	public class AccessibilityNode
	{
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<AccessibilityNode, bool> focusChanged;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Func<bool> invoked;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action incremented;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action decremented;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Func<AccessibilityScrollDirection, bool> scrolled;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Func<bool> dismissed;

		public IReadOnlyList<AccessibilityNode> children
		{
			get
			{
				return this.childList;
			}
		}

		public AccessibilityNode parent { get; private set; }

		public string label
		{
			get
			{
				return this.m_Label;
			}
			set
			{
				bool flag = string.Equals(this.m_Label, value);
				if (!flag)
				{
					this.m_Label = value;
					bool flag2 = this.IsInActiveHierarchy();
					if (flag2)
					{
						AccessibilityNodeManager.SetLabel(this.id, value);
					}
				}
			}
		}

		public string value
		{
			get
			{
				return this.m_Value;
			}
			set
			{
				bool flag = string.Equals(this.m_Value, value);
				if (!flag)
				{
					this.m_Value = value;
					bool flag2 = this.IsInActiveHierarchy();
					if (flag2)
					{
						AccessibilityNodeManager.SetValue(this.id, value);
					}
				}
			}
		}

		public string hint
		{
			get
			{
				return this.m_Hint;
			}
			set
			{
				bool flag = string.Equals(this.m_Hint, value);
				if (!flag)
				{
					this.m_Hint = value;
					bool flag2 = this.IsInActiveHierarchy();
					if (flag2)
					{
						AccessibilityNodeManager.SetHint(this.id, value);
					}
				}
			}
		}

		public Rect frame
		{
			get
			{
				Rect rect;
				if (!(this.m_Frame == default(Rect)))
				{
					rect = this.m_Frame;
				}
				else
				{
					Func<Rect> frameGetter = this.frameGetter;
					rect = (this.m_Frame = ((frameGetter != null) ? frameGetter() : Rect.zero));
				}
				return rect;
			}
			set
			{
				this.m_Frame = value;
				bool flag = this.IsInActiveHierarchy();
				if (flag)
				{
					AccessibilityNodeManager.SetFrame(this.id, value);
				}
			}
		}

		public Func<Rect> frameGetter
		{
			get
			{
				return this.m_FrameGetter;
			}
			set
			{
				this.m_FrameGetter = value;
				bool flag = this.IsInActiveHierarchy();
				if (flag)
				{
					AccessibilityNodeManager.SetFrame(this.id, this.frame);
				}
			}
		}

		public int id { get; private set; }

		public AccessibilityRole role
		{
			get
			{
				return this.m_Role;
			}
			set
			{
				bool flag = this.m_Role == value;
				if (!flag)
				{
					this.m_Role = value;
					bool flag2 = this.IsInActiveHierarchy();
					if (flag2)
					{
						AccessibilityNodeManager.SetRole(this.id, value);
					}
				}
			}
		}

		public AccessibilityState state
		{
			get
			{
				return this.m_State;
			}
			set
			{
				bool flag = this.m_State == value;
				if (!flag)
				{
					this.m_State = value;
					bool flag2 = this.IsInActiveHierarchy();
					if (flag2)
					{
						AccessibilityNodeManager.SetState(this.id, value);
					}
				}
			}
		}

		public bool isActive
		{
			get
			{
				return this.m_IsActive;
			}
			set
			{
				bool flag = this.m_IsActive == value;
				if (!flag)
				{
					this.m_IsActive = value;
					bool flag2 = this.IsInActiveHierarchy();
					if (flag2)
					{
						AccessibilityNodeManager.SetIsActive(this.id, value);
					}
				}
			}
		}

		public bool isFocused
		{
			get
			{
				return this.IsInActiveHierarchy() && AccessibilityNodeManager.GetIsFocused(this.id);
			}
		}

		public bool allowsDirectInteraction
		{
			get
			{
				return this.m_AllowsDirectInteraction;
			}
			set
			{
				bool flag = this.m_AllowsDirectInteraction == value;
				if (!flag)
				{
					this.m_AllowsDirectInteraction = value;
					bool flag2 = this.IsInActiveHierarchy();
					if (flag2)
					{
						AccessibilityNodeManager.SetAllowsDirectInteraction(this.id, value);
					}
				}
			}
		}

		internal AccessibilityNode(int nodeId, AccessibilityHierarchy hierarchy)
		{
			this.id = nodeId;
			this.m_Hierarchy = hierarchy;
			bool flag = !this.IsInActiveHierarchy();
			if (!flag)
			{
				AccessibilityNodeData accessibilityNodeData = new AccessibilityNodeData
				{
					nodeId = nodeId
				};
				this.CreateNativeNodeWithData(ref accessibilityNodeData);
			}
		}

		private void CreateNativeNodeWithData(ref AccessibilityNodeData nodeData)
		{
			bool isSupportedPlatform = AccessibilityManager.isSupportedPlatform;
			if (isSupportedPlatform)
			{
				while (!AccessibilityNodeManager.CreateNativeNodeWithData(nodeData))
				{
					Debug.LogWarning(string.Format("{0}: Node ID '{1}' is already ", "CreateNativeNodeWithData", nodeData.nodeId) + "used. Trying to create a node with an incremented node ID.");
					bool flag = nodeData.nodeId == int.MaxValue;
					if (flag)
					{
						nodeData.nodeId = 0;
					}
					else
					{
						int nodeId = nodeData.nodeId;
						nodeData.nodeId = nodeId + 1;
					}
				}
			}
			this.id = nodeData.nodeId;
		}

		internal void GetNodeData(ref AccessibilityNodeData nodeData)
		{
			int[] array = new int[this.children.Count];
			for (int i = 0; i < this.children.Count; i++)
			{
				array[i] = this.children[i].id;
			}
			nodeData.childIds = array;
			nodeData.label = this.label;
			nodeData.value = this.value;
			nodeData.hint = this.hint;
			nodeData.frame = this.frame;
			nodeData.nodeId = this.id;
			AccessibilityNode parent = this.parent;
			nodeData.parentId = ((parent != null) ? parent.id : (-1));
			nodeData.role = this.role;
			nodeData.state = this.state;
			nodeData.isActive = this.isActive;
			nodeData.allowsDirectInteraction = this.allowsDirectInteraction;
			nodeData.implementsInvoked = this.invoked != null;
			nodeData.implementsScrolled = this.scrolled != null;
			nodeData.implementsDismissed = this.dismissed != null;
		}

		internal void AllocateNative()
		{
			bool flag = !this.IsInActiveHierarchy();
			if (!flag)
			{
				AccessibilityNodeData accessibilityNodeData = new AccessibilityNodeData();
				accessibilityNodeData.label = this.label;
				accessibilityNodeData.value = this.value;
				accessibilityNodeData.hint = this.hint;
				accessibilityNodeData.frame = this.frame;
				accessibilityNodeData.nodeId = this.id;
				AccessibilityNode parent = this.parent;
				accessibilityNodeData.parentId = ((parent != null) ? parent.id : (-1));
				accessibilityNodeData.role = this.role;
				accessibilityNodeData.state = this.state;
				accessibilityNodeData.isActive = this.isActive;
				accessibilityNodeData.allowsDirectInteraction = this.allowsDirectInteraction;
				accessibilityNodeData.implementsInvoked = this.invoked != null;
				accessibilityNodeData.implementsScrolled = this.scrolled != null;
				accessibilityNodeData.implementsDismissed = this.dismissed != null;
				AccessibilityNodeData accessibilityNodeData2 = accessibilityNodeData;
				this.CreateNativeNodeWithData(ref accessibilityNodeData2);
				foreach (AccessibilityNode accessibilityNode in this.children)
				{
					accessibilityNode.AllocateNative();
				}
			}
		}

		internal void FreeNative(bool freeChildren)
		{
			if (freeChildren)
			{
				foreach (AccessibilityNode accessibilityNode in this.children)
				{
					accessibilityNode.FreeNative(true);
				}
			}
			bool flag = this.IsInActiveHierarchy();
			if (flag)
			{
				AccessibilityNodeManager.DestroyNativeNode(this.id);
			}
		}

		internal void Destroy(bool destroyChildren)
		{
			this.FreeNative(destroyChildren);
			AccessibilityNode parent = this.parent;
			if (parent != null)
			{
				parent.childList.Remove(this);
			}
			if (destroyChildren)
			{
				for (int i = this.childList.Count - 1; i >= 0; i--)
				{
					this.childList[i].Destroy(true);
				}
			}
			else
			{
				foreach (AccessibilityNode accessibilityNode in this.childList)
				{
					accessibilityNode.SetParent(this.parent, -1);
					AccessibilityNode parent2 = this.parent;
					if (parent2 != null)
					{
						parent2.childList.Add(accessibilityNode);
					}
				}
			}
			this.childList.Clear();
			this.m_Hierarchy = null;
		}

		private bool IsInActiveHierarchy()
		{
			return this.m_Hierarchy != null && AssistiveSupport.activeHierarchy == this.m_Hierarchy;
		}

		internal void SetParent(AccessibilityNode nodeParent, int index = -1)
		{
			this.parent = nodeParent;
			bool flag = this.IsInActiveHierarchy();
			if (flag)
			{
				int num = ((nodeParent != null) ? nodeParent.id : (-1));
				AccessibilityNodeManager.SetParent(this.id, num, index);
			}
		}

		public override int GetHashCode()
		{
			return this.id;
		}

		public override string ToString()
		{
			return string.Format("AccessibilityNode(ID: {0}, Label: \"{1}\")", this.id, this.label);
		}

		internal void NotifyFocusChanged(bool isNodeFocused)
		{
			AccessibilityManager.QueueNotification(new AccessibilityManager.NotificationContext
			{
				notification = (isNodeFocused ? AccessibilityManager.Notification.ElementFocused : AccessibilityManager.Notification.ElementUnfocused),
				focusedNode = this
			});
		}

		internal void InvokeFocusChanged(bool isNodeFocused)
		{
			Action<AccessibilityNode, bool> action = this.focusChanged;
			if (action != null)
			{
				action(this, isNodeFocused);
			}
		}

		internal bool InvokeNodeInvoked()
		{
			Func<bool> func = this.invoked;
			return func != null && func();
		}

		internal bool InvokeIncremented()
		{
			bool flag = this.incremented == null;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				this.incremented();
				flag2 = true;
			}
			return flag2;
		}

		internal bool InvokeDecremented()
		{
			bool flag = this.decremented == null;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				Action action = this.decremented;
				if (action != null)
				{
					action();
				}
				flag2 = true;
			}
			return flag2;
		}

		internal bool InvokeScrolled(AccessibilityScrollDirection direction)
		{
			Func<AccessibilityScrollDirection, bool> func = this.scrolled;
			return func != null && func(direction);
		}

		internal bool InvokeDismissed()
		{
			Func<bool> func = this.dismissed;
			return func != null && func();
		}

		[Obsolete("AccessibilityNode.selected has been renamed to AccessibilityNode.invoked to avoid confusion with AccessibilityState.Selected. (UnityUpgradable) -> invoked", false)]
		public event Func<bool> selected
		{
			[ExcludeFromCodeCoverage]
			add
			{
				this.invoked += value;
			}
			[ExcludeFromCodeCoverage]
			remove
			{
				this.invoked -= value;
			}
		}

		private AccessibilityHierarchy m_Hierarchy;

		internal List<AccessibilityNode> childList = new List<AccessibilityNode>();

		private string m_Label;

		private string m_Value;

		private string m_Hint;

		private Rect m_Frame;

		private Func<Rect> m_FrameGetter;

		private AccessibilityRole m_Role;

		private AccessibilityState m_State;

		private bool m_IsActive = true;

		private bool m_AllowsDirectInteraction;
	}
}
