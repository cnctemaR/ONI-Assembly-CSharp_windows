using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Accessibility
{
	public class AccessibilityHierarchy
	{
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private event Action<AccessibilityHierarchy> m_Changed;

		internal event Action<AccessibilityHierarchy> changed
		{
			[VisibleToOtherModules(new string[] { "UnityEditor.AccessibilityModule" })]
			add
			{
				this.m_Changed += value;
			}
			[VisibleToOtherModules(new string[] { "UnityEditor.AccessibilityModule" })]
			remove
			{
				this.m_Changed -= value;
			}
		}

		public IReadOnlyList<AccessibilityNode> rootNodes
		{
			get
			{
				return this.m_RootNodes;
			}
		}

		public AccessibilityHierarchy()
		{
			this.m_FirstLowestCommonAncestorChain = new Stack<AccessibilityNode>();
			this.m_SecondLowestCommonAncestorChain = new Stack<AccessibilityNode>();
			this.m_Nodes = new Dictionary<int, AccessibilityNode>();
			this.m_RootNodes = new List<AccessibilityNode>();
		}

		public bool ContainsNode(AccessibilityNode node)
		{
			return node != null && this.m_Nodes.ContainsKey(node.id) && this.m_Nodes[node.id] == node;
		}

		public bool TryGetNode(int id, out AccessibilityNode node)
		{
			return this.m_Nodes.TryGetValue(id, out node);
		}

		public bool TryGetNodeAt(float horizontalPosition, float verticalPosition, out AccessibilityNode node)
		{
			Vector2 vector = new Vector2(horizontalPosition, verticalPosition);
			node = AccessibilityHierarchy.<TryGetNodeAt>g__FindNodeContainingPoint|16_0(this.m_RootNodes, vector);
			return node != null;
		}

		public AccessibilityNode AddNode(string label = null, AccessibilityNode parent = null)
		{
			bool flag = parent != null && !this.ContainsNode(parent);
			AccessibilityNode accessibilityNode;
			if (flag)
			{
				Debug.LogError(string.Format("{0}: Attempting to add an AccessibilityNode under {1}, which is ", "AddNode", parent) + "not part of this hierarchy.");
				accessibilityNode = null;
			}
			else
			{
				accessibilityNode = this.CreateNodeAndSetParent(-1, label, parent);
			}
			return accessibilityNode;
		}

		public AccessibilityNode InsertNode(int childIndex, string label = null, AccessibilityNode parent = null)
		{
			bool flag = parent != null && !this.ContainsNode(parent);
			AccessibilityNode accessibilityNode;
			if (flag)
			{
				Debug.LogError(string.Format("{0}: Attempting to insert an AccessibilityNode under {1}, ", "InsertNode", parent) + "which is not part of this hierarchy.");
				accessibilityNode = null;
			}
			else
			{
				accessibilityNode = this.CreateNodeAndSetParent(childIndex, label, parent);
			}
			return accessibilityNode;
		}

		public bool MoveNode(AccessibilityNode node, AccessibilityNode newParent, int newChildIndex = -1)
		{
			bool flag = node == null;
			bool flag2;
			if (flag)
			{
				Debug.LogError("MoveNode: No node provided to move.");
				flag2 = false;
			}
			else
			{
				bool flag3 = !this.ContainsNode(node);
				if (flag3)
				{
					Debug.LogError(string.Format("{0}: Attempting to move {1}, which is not part of this hierarchy.", "MoveNode", node));
					flag2 = false;
				}
				else
				{
					bool flag4 = newParent != null && !this.ContainsNode(newParent);
					if (flag4)
					{
						Debug.LogError(string.Format("{0}: Attempting to move {1} under {2}, which is not part ", "MoveNode", node, newParent) + "of this hierarchy.");
						flag2 = false;
					}
					else
					{
						bool flag5 = node == newParent;
						if (flag5)
						{
							Debug.LogError(string.Format("{0}: Attempting to move {1} under itself.", "MoveNode", node));
							flag2 = false;
						}
						else
						{
							bool flag6 = node.parent == newParent;
							if (flag6)
							{
								List<AccessibilityNode> list = ((newParent == null) ? this.m_RootNodes : newParent.childList);
								int num = list.IndexOf(node);
								bool flag7 = num == newChildIndex;
								if (flag7)
								{
									return false;
								}
								bool flag8 = (newChildIndex < 0 || newChildIndex >= list.Count) && num == list.Count - 1;
								if (flag8)
								{
									return false;
								}
							}
							bool flag9 = this.CheckForLoopsAndSetParent(node, newParent, newChildIndex);
							if (flag9)
							{
								this.NotifyHierarchyChanged();
								flag2 = true;
							}
							else
							{
								flag2 = false;
							}
						}
					}
				}
			}
			return flag2;
		}

		public void RemoveNode(AccessibilityNode node, bool removeChildren = true)
		{
			bool flag = node == null;
			if (flag)
			{
				Debug.LogError("RemoveNode: No node provided to remove.");
			}
			else
			{
				bool flag2 = !this.ContainsNode(node);
				if (flag2)
				{
					Debug.LogError(string.Format("{0}: Attempting to remove {1}, which is not part of this ", "RemoveNode", node) + "hierarchy.");
				}
				else
				{
					if (removeChildren)
					{
						this.<RemoveNode>g__RemoveFromNodes|20_0(node);
					}
					else
					{
						this.m_Nodes.Remove(node.id);
					}
					bool flag3 = this.m_RootNodes.Contains(node);
					if (flag3)
					{
						this.m_RootNodes.Remove(node);
						bool flag4 = !removeChildren;
						if (flag4)
						{
							this.m_RootNodes.AddRange(node.children);
						}
					}
					node.Destroy(removeChildren);
					this.NotifyHierarchyChanged();
				}
			}
		}

		public void Clear()
		{
			for (int i = this.m_RootNodes.Count - 1; i >= 0; i--)
			{
				this.RemoveNode(this.m_RootNodes[i], true);
			}
		}

		public void RefreshNodeFrames()
		{
			foreach (AccessibilityNode accessibilityNode in this.m_Nodes.Values)
			{
				AccessibilityNode accessibilityNode2 = accessibilityNode;
				Func<Rect> frameGetter = accessibilityNode.frameGetter;
				accessibilityNode2.frame = ((frameGetter != null) ? frameGetter() : Rect.zero);
			}
			bool flag = AssistiveSupport.activeHierarchy == this;
			if (flag)
			{
				AssistiveSupport.notificationDispatcher.SendLayoutChanged(null);
			}
		}

		public AccessibilityNode GetLowestCommonAncestor(AccessibilityNode firstNode, AccessibilityNode secondNode)
		{
			bool flag = firstNode == null || secondNode == null;
			AccessibilityNode accessibilityNode;
			if (flag)
			{
				accessibilityNode = null;
			}
			else
			{
				bool flag2 = firstNode.parent == null || secondNode.parent == null;
				if (flag2)
				{
					accessibilityNode = null;
				}
				else
				{
					bool flag3 = !this.ContainsNode(firstNode) || !this.ContainsNode(secondNode);
					if (flag3)
					{
						Debug.LogError("GetLowestCommonAncestor: Attempting to find the lowest common ancestor of " + string.Format("{0} and {1}, which are not in the same hierarchy.", firstNode, secondNode));
						accessibilityNode = null;
					}
					else
					{
						this.m_FirstLowestCommonAncestorChain.Clear();
						this.m_SecondLowestCommonAncestorChain.Clear();
						this.<GetLowestCommonAncestor>g__BuildNodeIdStack|23_0(firstNode, ref this.m_FirstLowestCommonAncestorChain);
						this.<GetLowestCommonAncestor>g__BuildNodeIdStack|23_0(secondNode, ref this.m_SecondLowestCommonAncestorChain);
						AccessibilityNode accessibilityNode2 = null;
						for (int i = Mathf.Min(this.m_FirstLowestCommonAncestorChain.Count, this.m_SecondLowestCommonAncestorChain.Count); i > 0; i--)
						{
							AccessibilityNode accessibilityNode3 = this.m_FirstLowestCommonAncestorChain.Pop();
							AccessibilityNode accessibilityNode4 = this.m_SecondLowestCommonAncestorChain.Pop();
							bool flag4 = accessibilityNode3 != accessibilityNode4;
							if (flag4)
							{
								break;
							}
							accessibilityNode2 = accessibilityNode3;
						}
						accessibilityNode = accessibilityNode2;
					}
				}
			}
			return accessibilityNode;
		}

		private AccessibilityNode CreateNode()
		{
			AccessibilityNode accessibilityNode = new AccessibilityNode(AccessibilityHierarchy.nextUniqueNodeId, this);
			bool flag = accessibilityNode.id == int.MaxValue;
			if (flag)
			{
				AccessibilityHierarchy.nextUniqueNodeId = 0;
			}
			else
			{
				AccessibilityHierarchy.nextUniqueNodeId = accessibilityNode.id + 1;
			}
			return accessibilityNode;
		}

		private AccessibilityNode CreateNodeAndSetParent(int childIndex, string label, AccessibilityNode parent)
		{
			AccessibilityNode accessibilityNode = this.CreateNode();
			this.m_Nodes[accessibilityNode.id] = accessibilityNode;
			bool flag = label != null;
			if (flag)
			{
				accessibilityNode.label = label;
			}
			this.SetParent(accessibilityNode, parent, null, (parent == null) ? this.m_RootNodes : parent.childList, childIndex);
			this.NotifyHierarchyChanged();
			return accessibilityNode;
		}

		private bool CheckForLoopsAndSetParent(AccessibilityNode node, AccessibilityNode parent, int index)
		{
			bool flag = parent == null;
			bool flag2;
			if (flag)
			{
				AccessibilityNode accessibilityNode = null;
				AccessibilityNode parent2 = node.parent;
				this.SetParent(node, accessibilityNode, ((parent2 != null) ? parent2.childList : null) ?? this.m_RootNodes, this.m_RootNodes, index);
				flag2 = true;
			}
			else
			{
				bool flag3 = node.parent == parent;
				if (flag3)
				{
					this.SetParent(node, parent, parent.childList, parent.childList, index);
					flag2 = true;
				}
				else
				{
					bool flag4 = node.parent == null && parent.parent == null;
					if (flag4)
					{
						this.SetParent(node, parent, this.m_RootNodes, parent.childList, index);
						flag2 = true;
					}
					else
					{
						for (AccessibilityNode accessibilityNode2 = parent.parent; accessibilityNode2 != null; accessibilityNode2 = accessibilityNode2.parent)
						{
							bool flag5 = accessibilityNode2 == node;
							if (flag5)
							{
								Debug.LogError(string.Format("{0}: Attempting to move {1} under {2}, which would ", "MoveNode", node, parent) + "create a loop in the hierarchy.");
								return false;
							}
						}
						AccessibilityNode parent3 = node.parent;
						this.SetParent(node, parent, ((parent3 != null) ? parent3.childList : null) ?? this.m_RootNodes, parent.childList, index);
						flag2 = true;
					}
				}
			}
			return flag2;
		}

		private void SetParent(AccessibilityNode node, AccessibilityNode parent, IList<AccessibilityNode> previousParentChildren, IList<AccessibilityNode> newParentChildren, int index)
		{
			if (previousParentChildren != null)
			{
				previousParentChildren.Remove(node);
			}
			node.SetParent(parent, index);
			bool flag = index < 0 || index >= newParentChildren.Count;
			if (flag)
			{
				newParentChildren.Add(node);
			}
			else
			{
				newParentChildren.Insert(index, node);
			}
		}

		private void NotifyHierarchyChanged()
		{
			Action<AccessibilityHierarchy> changed = this.m_Changed;
			if (changed != null)
			{
				changed(this);
			}
		}

		internal void AllocateNative()
		{
			foreach (AccessibilityNode accessibilityNode in this.m_RootNodes)
			{
				accessibilityNode.AllocateNative();
			}
		}

		internal void FreeNative()
		{
			foreach (AccessibilityNode accessibilityNode in this.m_RootNodes)
			{
				accessibilityNode.FreeNative(true);
			}
		}

		[CompilerGenerated]
		internal static AccessibilityNode <TryGetNodeAt>g__FindNodeContainingPoint|16_0(IList<AccessibilityNode> nodes, Vector2 pos)
		{
			int i = nodes.Count - 1;
			while (i >= 0)
			{
				AccessibilityNode accessibilityNode = nodes[i];
				AccessibilityNode accessibilityNode2 = AccessibilityHierarchy.<TryGetNodeAt>g__FindNodeContainingPoint|16_0(accessibilityNode.childList, pos);
				bool flag = accessibilityNode2 != null;
				AccessibilityNode accessibilityNode3;
				if (flag)
				{
					accessibilityNode3 = accessibilityNode2;
				}
				else
				{
					bool flag2 = accessibilityNode.isActive && accessibilityNode.frame.Contains(pos);
					if (!flag2)
					{
						i--;
						continue;
					}
					accessibilityNode3 = accessibilityNode;
				}
				return accessibilityNode3;
			}
			return null;
		}

		[CompilerGenerated]
		private void <RemoveNode>g__RemoveFromNodes|20_0(AccessibilityNode child)
		{
			this.m_Nodes.Remove(child.id);
			foreach (AccessibilityNode accessibilityNode in child.children)
			{
				this.<RemoveNode>g__RemoveFromNodes|20_0(accessibilityNode);
			}
		}

		[CompilerGenerated]
		private void <GetLowestCommonAncestor>g__BuildNodeIdStack|23_0(AccessibilityNode node, ref Stack<AccessibilityNode> nodeStack)
		{
			while (node != null)
			{
				nodeStack.Push(node);
				node = this.m_Nodes[node.id].parent;
			}
		}

		private readonly IDictionary<int, AccessibilityNode> m_Nodes;

		private List<AccessibilityNode> m_RootNodes;

		private Stack<AccessibilityNode> m_FirstLowestCommonAncestorChain;

		private Stack<AccessibilityNode> m_SecondLowestCommonAncestorChain;

		internal static int nextUniqueNodeId;
	}
}
