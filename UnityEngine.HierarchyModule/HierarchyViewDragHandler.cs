using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Scripting.LifecycleManagement;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;

namespace Unity.Hierarchy
{
	internal class HierarchyViewDragHandler
	{
		private Hierarchy Hierarchy
		{
			get
			{
				return this.m_HierarchyView.Source;
			}
		}

		private HierarchyFlattened HierarchyFlattened
		{
			get
			{
				return this.m_HierarchyView.Flattened;
			}
		}

		private HierarchyViewModel HierarchyViewModel
		{
			get
			{
				return this.m_HierarchyView.ViewModel;
			}
		}

		private BaseVerticalCollectionView TargetView
		{
			get
			{
				return this.m_MultiColumnListView;
			}
		}

		private ScrollView TargetScrollView
		{
			get
			{
				return this.TargetView.Q<ScrollView>(null, null);
			}
		}

		public HierarchyViewDragHandler(HierarchyView hierarchyView)
		{
			this.m_HierarchyView = hierarchyView;
			this.m_MultiColumnListView = this.m_HierarchyView.ListView;
			this.m_AutoExpansionData = new HierarchyViewDragHandler.AutoExpansionData();
			this.m_MultiColumnListView.canStartDrag += this.CanStartDrag;
			this.m_MultiColumnListView.setupDragAndDrop += this.SetupDragAndDrop;
			this.m_MultiColumnListView.dragAndDropUpdate += this.DragAndDropUpdate;
			this.m_MultiColumnListView.handleDrop += this.HandleDrop;
			this.m_MultiColumnListView.RegisterCallback<PointerLeaveEvent>(new EventCallback<PointerLeaveEvent>(this.OnPointerLeave), TrickleDown.NoTrickleDown);
			this.m_MultiColumnListView.RegisterCallback<PointerDownEvent>(new EventCallback<PointerDownEvent>(this.OnPointerDown), TrickleDown.TrickleDown);
			this.m_MultiColumnListView.RegisterCallback<PointerMoveEvent>(new EventCallback<PointerMoveEvent>(this.OnPointerMove), TrickleDown.TrickleDown);
			this.m_HierarchyView.RegisterCallback<PointerUpEvent>(new EventCallback<PointerUpEvent>(this.OnPointerUp), TrickleDown.TrickleDown);
		}

		private void OnPointerLeave(PointerLeaveEvent evt)
		{
			this.ClearDragAndDropUI();
		}

		private void OnPointerDown(PointerDownEvent evt)
		{
			this.m_CurrentEventModifiers = evt.modifiers;
		}

		private void OnPointerMove(PointerMoveEvent evt)
		{
			this.m_CurrentEventModifiers = evt.modifiers;
		}

		private void OnPointerUp(PointerUpEvent evt)
		{
			this.m_CurrentEventModifiers = evt.modifiers;
		}

		private bool IsSearchActive()
		{
			return this.m_HierarchyView.Filtering;
		}

		private bool CanStartDrag(CanStartDragArgs args)
		{
			bool flag = this.IsSearchActive();
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				RentSpanUnmanaged<HierarchyNode> rentSpanUnmanaged = new RentSpanUnmanaged<HierarchyNode>(this.HierarchyViewModel.HasAllFlagsCount(HierarchyNodeFlags.Selected), false);
				try
				{
					this.HierarchyViewModel.GetNodesWithAllFlags(HierarchyNodeFlags.Selected, in rentSpanUnmanaged);
					foreach (HierarchyNodeTypeHandler hierarchyNodeTypeHandler in this.Hierarchy.EnumerateNodeTypeHandlers())
					{
						IHierarchyEditorNodeTypeHandler hierarchyEditorNodeTypeHandler = hierarchyNodeTypeHandler as IHierarchyEditorNodeTypeHandler;
						bool flag3 = hierarchyEditorNodeTypeHandler != null && !hierarchyEditorNodeTypeHandler.CanStartDrag(this.m_HierarchyView, in rentSpanUnmanaged);
						if (flag3)
						{
							return false;
						}
					}
					flag2 = true;
				}
				finally
				{
					rentSpanUnmanaged.Dispose();
				}
			}
			return flag2;
		}

		private StartDragArgs SetupDragAndDrop(SetupDragAndDropArgs args)
		{
			int num = this.HierarchyViewModel.HasAllFlagsCount(HierarchyNodeFlags.Selected);
			RentSpanUnmanaged<HierarchyNode> rentSpanUnmanaged = new RentSpanUnmanaged<HierarchyNode>(num, false);
			this.HierarchyViewModel.GetNodesWithAllFlags(HierarchyNodeFlags.Selected, in rentSpanUnmanaged);
			List<EntityId> list = new List<EntityId>();
			List<string> list2;
			StartDragArgs startDragArgs2;
			using (CollectionPool<List<string>, string>.Get(out list2))
			{
				Dictionary<string, object> dictionary;
				using (CollectionPool<Dictionary<string, object>, KeyValuePair<string, object>>.Get(out dictionary))
				{
					HierarchyViewDragAndDropSetupData hierarchyViewDragAndDropSetupData = new HierarchyViewDragAndDropSetupData(in rentSpanUnmanaged, list, list2, this.m_HierarchyView, dictionary);
					foreach (HierarchyNodeTypeHandler hierarchyNodeTypeHandler in this.Hierarchy.EnumerateNodeTypeHandlers())
					{
						IHierarchyEditorNodeTypeHandler hierarchyEditorNodeTypeHandler = hierarchyNodeTypeHandler as IHierarchyEditorNodeTypeHandler;
						bool flag = hierarchyEditorNodeTypeHandler != null;
						if (flag)
						{
							hierarchyEditorNodeTypeHandler.OnStartDrag(in hierarchyViewDragAndDropSetupData);
						}
					}
					StartDragArgs startDragArgs = new StartDragArgs(args.startDragArgs.title, args.startDragArgs.visualMode);
					startDragArgs.SetEntityIds(list);
					startDragArgs.SetPaths(list2.ToArray());
					foreach (KeyValuePair<string, object> keyValuePair in dictionary)
					{
						startDragArgs.SetGenericData(keyValuePair.Key, keyValuePair.Value);
					}
					startDragArgs2 = startDragArgs;
				}
			}
			return startDragArgs2;
		}

		private DragVisualMode DragAndDropUpdate(HandleDragAndDropArgs args)
		{
			HierarchyViewDragHandler.HierarchyViewDragAndDropTargets visualMode = this.GetVisualMode(in args);
			bool flag = visualMode.dragVisualMode == DragVisualMode.Rejected;
			if (flag)
			{
				this.ClearDragAndDropUI();
			}
			else
			{
				this.HandleAutoExpansion(visualMode, args.position);
				this.ApplyDragAndDropUI(visualMode);
			}
			return visualMode.dragVisualMode;
		}

		private unsafe HierarchyViewDragHandler.HierarchyViewDragAndDropTargets GetVisualMode(in HandleDragAndDropArgs args)
		{
			bool flag = args.insertAtIndex < 0;
			HierarchyViewDragHandler.HierarchyViewDragAndDropTargets hierarchyViewDragAndDropTargets;
			if (flag)
			{
				hierarchyViewDragAndDropTargets = HierarchyViewDragHandler.HierarchyViewDragAndDropTargets.Rejected;
			}
			else
			{
				HierarchyViewDragHandler.HierarchyViewDragAndDropTargets hierarchyViewDragAndDropTargets2 = this.GetDragAndDropTargets(in args);
				HierarchyNode hierarchyNode = ((hierarchyViewDragAndDropTargets2.parentIndex == -1) ? (*this.Hierarchy.Root) : (*this.HierarchyViewModel[hierarchyViewDragAndDropTargets2.parentIndex]));
				hierarchyViewDragAndDropTargets2 = this.HandleNodeHandlersDrop(hierarchyViewDragAndDropTargets2, args.dragAndDropData, in hierarchyNode, false);
				bool flag2 = hierarchyViewDragAndDropTargets2.dragVisualMode > DragVisualMode.None;
				if (flag2)
				{
					hierarchyViewDragAndDropTargets = hierarchyViewDragAndDropTargets2;
				}
				else
				{
					hierarchyViewDragAndDropTargets = this.HandleDefaultCanDrop(in args, hierarchyViewDragAndDropTargets2, in hierarchyNode);
				}
			}
			return hierarchyViewDragAndDropTargets;
		}

		private unsafe HierarchyViewDragHandler.HierarchyViewDragAndDropTargets HandleDefaultCanDrop(in HandleDragAndDropArgs args, HierarchyViewDragHandler.HierarchyViewDragAndDropTargets dragAndDropTargets, in HierarchyNode parentNode)
		{
			bool flag = !this.DragSourceIsCurrentListView(in args);
			HierarchyViewDragHandler.HierarchyViewDragAndDropTargets hierarchyViewDragAndDropTargets;
			if (flag)
			{
				hierarchyViewDragAndDropTargets = HierarchyViewDragHandler.HierarchyViewDragAndDropTargets.Rejected;
			}
			else
			{
				RentSpanUnmanaged<HierarchyNode> rentSpanUnmanaged = new RentSpanUnmanaged<HierarchyNode>(this.HierarchyViewModel.HasAllFlagsCount(HierarchyNodeFlags.Selected), false);
				try
				{
					this.HierarchyViewModel.GetNodesWithAllFlags(HierarchyNodeFlags.Selected, in rentSpanUnmanaged);
					IHierarchyEditorNodeTypeHandler hierarchyEditorNodeTypeHandler = (((in parentNode) == this.Hierarchy.Root) ? null : (this.Hierarchy.GetNodeTypeHandler(in parentNode) as IHierarchyEditorNodeTypeHandler));
					for (int i = 0; i < rentSpanUnmanaged.Span.Length; i++)
					{
						HierarchyNode hierarchyNode = *rentSpanUnmanaged.Span[i];
						bool flag2 = this.IsDescendant(in parentNode, in hierarchyNode);
						if (flag2)
						{
							return HierarchyViewDragHandler.HierarchyViewDragAndDropTargets.Rejected;
						}
						IHierarchyEditorNodeTypeHandler hierarchyEditorNodeTypeHandler2 = this.Hierarchy.GetNodeTypeHandler(in hierarchyNode) as IHierarchyEditorNodeTypeHandler;
						bool flag3 = hierarchyEditorNodeTypeHandler != null && !hierarchyEditorNodeTypeHandler.AcceptChild(this.m_HierarchyView, in hierarchyNode);
						if (flag3)
						{
							return HierarchyViewDragHandler.HierarchyViewDragAndDropTargets.Rejected;
						}
						bool flag4 = hierarchyEditorNodeTypeHandler2 != null && !hierarchyEditorNodeTypeHandler2.AcceptParent(this.m_HierarchyView, in parentNode);
						if (flag4)
						{
							return HierarchyViewDragHandler.HierarchyViewDragAndDropTargets.Rejected;
						}
					}
					dragAndDropTargets.dragVisualMode = DragVisualMode.Move;
					hierarchyViewDragAndDropTargets = dragAndDropTargets;
				}
				finally
				{
					rentSpanUnmanaged.Dispose();
				}
			}
			return hierarchyViewDragAndDropTargets;
		}

		private bool IsDescendant(in HierarchyNode possibleDescendant, in HierarchyNode target)
		{
			bool flag = this.Hierarchy.GetDepth(in possibleDescendant) <= this.Hierarchy.GetDepth(in target);
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				HierarchyNode hierarchyNode = this.Hierarchy.GetParent(in possibleDescendant);
				while ((in hierarchyNode) != HierarchyNode.Null)
				{
					bool flag3 = (in hierarchyNode) == (in target);
					if (flag3)
					{
						return true;
					}
					hierarchyNode = this.Hierarchy.GetParent(in hierarchyNode);
				}
				flag2 = false;
			}
			return flag2;
		}

		private unsafe HierarchyViewDragHandler.HierarchyViewDragAndDropTargets HandleNodeHandlersDrop(HierarchyViewDragHandler.HierarchyViewDragAndDropTargets dragAndDropTargets, DragAndDropData dragAndDropData, in HierarchyNode parentNode, bool perform)
		{
			HierarchyNode hierarchyNode = ((dragAndDropTargets.targetIndex == -1 || dragAndDropTargets.targetIndex >= this.HierarchyViewModel.Count) ? (*HierarchyNode.Null) : (*this.HierarchyViewModel[dragAndDropTargets.targetIndex]));
			HierarchyViewDragAndDropHandlingData hierarchyViewDragAndDropHandlingData = new HierarchyViewDragAndDropHandlingData(in parentNode, in hierarchyNode, dragAndDropTargets.insertAtIndex, dragAndDropTargets.dropPosition, dragAndDropData, this.m_HierarchyView, this.m_CurrentEventModifiers);
			foreach (HierarchyNodeTypeHandler hierarchyNodeTypeHandler in this.Hierarchy.EnumerateNodeTypeHandlers())
			{
				IHierarchyEditorNodeTypeHandler hierarchyEditorNodeTypeHandler = hierarchyNodeTypeHandler as IHierarchyEditorNodeTypeHandler;
				bool flag = hierarchyEditorNodeTypeHandler != null;
				if (flag)
				{
					DragVisualMode dragVisualMode = (perform ? hierarchyEditorNodeTypeHandler.OnDrop(in hierarchyViewDragAndDropHandlingData) : hierarchyEditorNodeTypeHandler.CanDrop(in hierarchyViewDragAndDropHandlingData));
					bool flag2 = dragVisualMode > DragVisualMode.None;
					if (flag2)
					{
						dragAndDropTargets.dragVisualMode = dragVisualMode;
						return dragAndDropTargets;
					}
				}
			}
			dragAndDropTargets.dragVisualMode = DragVisualMode.None;
			return dragAndDropTargets;
		}

		private unsafe DragVisualMode HandleDrop(HandleDragAndDropArgs args)
		{
			this.ClearDragAndDropUI();
			bool flag = args.insertAtIndex < 0;
			DragVisualMode dragVisualMode;
			if (flag)
			{
				dragVisualMode = DragVisualMode.Rejected;
			}
			else
			{
				HierarchyViewDragHandler.HierarchyViewDragAndDropTargets hierarchyViewDragAndDropTargets = this.GetDragAndDropTargets(in args);
				HierarchyNode hierarchyNode = ((hierarchyViewDragAndDropTargets.parentIndex == -1) ? (*this.Hierarchy.Root) : (*this.HierarchyViewModel[hierarchyViewDragAndDropTargets.parentIndex]));
				int version = this.HierarchyViewModel.Version;
				hierarchyViewDragAndDropTargets = this.HandleNodeHandlersDrop(hierarchyViewDragAndDropTargets, args.dragAndDropData, in hierarchyNode, true);
				bool flag2 = this.HierarchyViewModel.Version != version;
				if (flag2)
				{
					dragVisualMode = DragVisualMode.Rejected;
				}
				else
				{
					DragVisualMode dragVisualMode2 = hierarchyViewDragAndDropTargets.dragVisualMode;
					bool flag3 = dragVisualMode2 == DragVisualMode.None;
					if (flag3)
					{
						dragVisualMode2 = this.HandleDefaultDrop(in args, hierarchyViewDragAndDropTargets, in hierarchyNode);
					}
					bool flag4 = (in hierarchyNode) != this.Hierarchy.Root;
					if (flag4)
					{
						this.HierarchyViewModel.SetFlags(in hierarchyNode, HierarchyNodeFlags.Expanded);
					}
					this.ClearAutoExpansionData(false);
					bool flag5 = dragVisualMode2 != DragVisualMode.Rejected && dragVisualMode2 > DragVisualMode.None;
					if (flag5)
					{
						this.m_HierarchyView.EnqueuePostUpdateAction(delegate
						{
							foreach (ref HierarchyNode ptr in this.m_HierarchyView.ViewModel.EnumerateNodesWithAllFlags(HierarchyNodeFlags.Selected))
							{
								bool flag6 = (in ptr) == HierarchyNode.Null || (in ptr) == this.m_HierarchyView.Source.Root;
								if (!flag6)
								{
									this.m_HierarchyView.Frame(in ptr);
									break;
								}
							}
						});
					}
					dragVisualMode = dragVisualMode2;
				}
			}
			return dragVisualMode;
		}

		private unsafe DragVisualMode HandleDefaultDrop(in HandleDragAndDropArgs args, HierarchyViewDragHandler.HierarchyViewDragAndDropTargets dragAndDropTargets, in HierarchyNode parentNode)
		{
			bool flag = !this.DragSourceIsCurrentListView(in args);
			DragVisualMode dragVisualMode;
			if (flag)
			{
				dragVisualMode = DragVisualMode.Rejected;
			}
			else
			{
				using (RentSpanUnmanaged<HierarchyNode> rentSpanUnmanaged = new RentSpanUnmanaged<HierarchyNode>(this.HierarchyViewModel.HasAllFlagsCount(HierarchyNodeFlags.Selected), false))
				{
					Span<HierarchyNode> span = rentSpanUnmanaged.Span;
					this.HierarchyViewModel.GetNodesWithAllFlags(HierarchyNodeFlags.Selected, span);
					HierarchyNode[] children = this.Hierarchy.GetChildren(in parentNode);
					int num = dragAndDropTargets.childIndex;
					for (int i = 0; i < span.Length; i++)
					{
						bool flag2 = span[i] == (in parentNode);
						if (flag2)
						{
							for (int j = i; j < span.Length - 1; j++)
							{
								*span[j] = *span[j + 1];
							}
							span = span.Slice(0, span.Length - 1);
							break;
						}
					}
					List<HierarchyNode> list = new List<HierarchyNode>(span.Length);
					List<int> list2 = new List<int>(span.Length);
					int num2 = 0;
					for (int k = 0; k < span.Length; k++)
					{
						HierarchyNode parent = this.Hierarchy.GetParent(span[k]);
						bool flag3 = (in parent) == (in parentNode) || list.Contains(parent);
						if (!flag3)
						{
							list.Add(parent);
							int childrenCount = this.Hierarchy.GetChildrenCount(in parent);
							list2.Add(childrenCount);
							num2 = Math.Max(num2, childrenCount);
						}
					}
					bool flag4 = list.Count > 0;
					int num4;
					if (flag4)
					{
						HierarchyNode[] array = new HierarchyNode[num2];
						for (int l = 0; l < list.Count; l++)
						{
							HierarchyNode hierarchyNode = list[l];
							this.Hierarchy.GetChildren(in hierarchyNode, array);
							int num3 = list2[l];
							num4 = 0;
							for (int m = 0; m < num3; m++)
							{
								HierarchyNode hierarchyNode2 = array[m];
								bool flag5 = this.HierarchyViewModel.HasAllFlags(in hierarchyNode2, HierarchyNodeFlags.Selected);
								if (!flag5)
								{
									this.Hierarchy.SetSortIndex(in hierarchyNode2, num4++);
								}
							}
						}
					}
					for (int n = 0; n < span.Length; n++)
					{
						this.Hierarchy.SetParent(span[n], in parentNode);
					}
					bool flag6 = num == -1;
					if (flag6)
					{
						num = children.Length;
					}
					num4 = 0;
					int num5 = 0;
					while (num5 < num && num5 < children.Length)
					{
						HierarchyNode hierarchyNode3 = children[num5];
						bool flag7 = this.HierarchyViewModel.HasAllFlags(in hierarchyNode3, HierarchyNodeFlags.Selected);
						if (!flag7)
						{
							this.Hierarchy.SetSortIndex(in hierarchyNode3, num4++);
						}
						num5++;
					}
					for (int num6 = 0; num6 < span.Length; num6++)
					{
						this.Hierarchy.SetSortIndex(span[num6], num4++);
					}
					for (int num7 = num; num7 < children.Length; num7++)
					{
						HierarchyNode hierarchyNode4 = children[num7];
						bool flag8 = this.HierarchyViewModel.HasAllFlags(in hierarchyNode4, HierarchyNodeFlags.Selected);
						if (!flag8)
						{
							this.Hierarchy.SetSortIndex(in hierarchyNode4, num4++);
						}
					}
					this.Hierarchy.SortChildren(in parentNode);
					dragVisualMode = DragVisualMode.Move;
				}
			}
			return dragVisualMode;
		}

		private HierarchyViewDragHandler.HierarchyViewDragAndDropTargets GetDragAndDropTargets(in HandleDragAndDropArgs args)
		{
			bool flag = this.DragSourceIsCurrentListView(in args);
			ReadOnlySpan<int> readOnlySpan;
			if (flag)
			{
				int num = this.HierarchyViewModel.HasAllFlagsCount(HierarchyNodeFlags.Selected);
				RentSpanUnmanaged<int> rentSpanUnmanaged = new RentSpanUnmanaged<int>(num, false);
				try
				{
					this.HierarchyViewModel.GetIndicesWithAllFlags(HierarchyNodeFlags.Selected, in rentSpanUnmanaged);
					readOnlySpan = in rentSpanUnmanaged;
					return this.HandleTreePosition(in args, in readOnlySpan);
				}
				finally
				{
					rentSpanUnmanaged.Dispose();
				}
			}
			readOnlySpan = Array.Empty<int>();
			return this.HandleTreePosition(in args, in readOnlySpan);
		}

		private bool DragSourceIsCurrentListView(in HandleDragAndDropArgs args)
		{
			return args.dragAndDropData.source == this.TargetView;
		}

		private HierarchyViewDragHandler.HierarchyViewDragAndDropTargets HandleTreePosition(in HandleDragAndDropArgs dnDArgs, in ReadOnlySpan<int> draggedIndices)
		{
			this.m_LeftIndentation = -1f;
			this.m_SiblingBottom = -1f;
			bool flag = dnDArgs.insertAtIndex < 0;
			HierarchyViewDragHandler.HierarchyViewDragAndDropTargets hierarchyViewDragAndDropTargets;
			if (flag)
			{
				hierarchyViewDragAndDropTargets = new HierarchyViewDragHandler.HierarchyViewDragAndDropTargets(dnDArgs.insertAtIndex, -1, -1, -1, DragAndDropPosition.OutsideItems);
			}
			else
			{
				bool flag2 = dnDArgs.dropPosition == DragAndDropPosition.OverItem;
				if (flag2)
				{
					hierarchyViewDragAndDropTargets = new HierarchyViewDragHandler.HierarchyViewDragAndDropTargets(dnDArgs.insertAtIndex, dnDArgs.insertAtIndex, dnDArgs.insertAtIndex, -1, DragAndDropPosition.OverItem);
				}
				else
				{
					bool flag3 = dnDArgs.insertAtIndex <= 0;
					if (flag3)
					{
						hierarchyViewDragAndDropTargets = new HierarchyViewDragHandler.HierarchyViewDragAndDropTargets(dnDArgs.insertAtIndex, 0, -1, 0, DragAndDropPosition.BetweenItems);
					}
					else
					{
						int indexFromWorldPosition = this.m_HierarchyView.GetIndexFromWorldPosition(dnDArgs.position, 4f);
						bool flag4 = indexFromWorldPosition >= this.m_HierarchyView.ViewModel.Count;
						if (flag4)
						{
							hierarchyViewDragAndDropTargets = new HierarchyViewDragHandler.HierarchyViewDragAndDropTargets(dnDArgs.insertAtIndex, 0, -1, -1, DragAndDropPosition.OutsideItems);
						}
						else
						{
							Vector2 position = dnDArgs.position;
							hierarchyViewDragAndDropTargets = this.HandleSiblingInsertionAtAvailableDepthsAndChangeTargetIfNeeded(in dnDArgs, in position, in draggedIndices);
						}
					}
				}
			}
			return hierarchyViewDragAndDropTargets;
		}

		private unsafe HierarchyViewDragHandler.HierarchyViewDragAndDropTargets HandleSiblingInsertionAtAvailableDepthsAndChangeTargetIfNeeded(in HandleDragAndDropArgs dnDArgs, in Vector2 pointerPosition, in ReadOnlySpan<int> draggedIndices)
		{
			int insertAtIndex = dnDArgs.insertAtIndex;
			int num;
			int num2;
			this.GetPreviousAndNextIndexesIgnoringDraggedItems(dnDArgs.insertAtIndex, out num, out num2, in draggedIndices);
			HierarchyViewDragHandler.HierarchyViewDragAndDropTargets hierarchyViewDragAndDropTargets = new HierarchyViewDragHandler.HierarchyViewDragAndDropTargets(dnDArgs.insertAtIndex, dnDArgs.insertAtIndex, -1, -1, DragAndDropPosition.BetweenItems);
			bool flag = num == -1;
			HierarchyViewDragHandler.HierarchyViewDragAndDropTargets hierarchyViewDragAndDropTargets2;
			if (flag)
			{
				hierarchyViewDragAndDropTargets2 = hierarchyViewDragAndDropTargets;
			}
			else
			{
				HierarchyNode hierarchyNode = *this.HierarchyViewModel[num];
				HierarchyNode hierarchyNode2 = ((num2 == -1) ? (*HierarchyNode.Null) : (*this.HierarchyViewModel[num2]));
				bool flag2 = this.HierarchyFlattened.GetChildrenCount(in hierarchyNode) > 0 && this.HierarchyViewModel.HasAllFlags(in hierarchyNode, HierarchyNodeFlags.Expanded);
				int depth = this.HierarchyFlattened.GetDepth(in hierarchyNode);
				int num3 = ((num2 == -1) ? 0 : this.HierarchyFlattened.GetDepth(in hierarchyNode2));
				int num4 = num3;
				int num5 = depth + (flag2 ? 1 : 0);
				int num6 = num;
				hierarchyViewDragAndDropTargets.targetIndex = num;
				HierarchyNode hierarchyNode3 = hierarchyNode;
				int i = depth;
				float num7 = 0f;
				float num8 = 14f;
				VisualElement visualElement = null;
				bool flag3 = depth > 0;
				if (flag3)
				{
					visualElement = this.TargetView.GetRootElementForIndex(num);
				}
				else
				{
					HierarchyNode hierarchyNode4 = ((dnDArgs.insertAtIndex == -1 || dnDArgs.insertAtIndex >= this.HierarchyViewModel.Count) ? (*HierarchyNode.Null) : (*this.HierarchyViewModel[dnDArgs.insertAtIndex]));
					int num9 = (((in hierarchyNode4) == HierarchyNode.Null) ? 0 : this.HierarchyFlattened.GetDepth(in hierarchyNode4));
					bool flag4 = num9 > 0;
					if (flag4)
					{
						visualElement = this.TargetView.GetRootElementForIndex(dnDArgs.insertAtIndex);
					}
				}
				HierarchyViewItem hierarchyViewItem = ((visualElement != null) ? visualElement.Q<HierarchyViewItem>(null, null) : null);
				bool flag5 = hierarchyViewItem != null;
				if (flag5)
				{
					num7 = hierarchyViewItem.Toggle.layout.width;
					num8 = ((depth > 0) ? ((hierarchyViewItem.LeftContainer.style.translate.value.x.value + 4f) / (float)depth) : num8);
				}
				VisualElement nameColumn = this.GetNameColumn();
				bool flag6 = false;
				Vector2 vector = Vector2.zero;
				bool flag7 = nameColumn != null;
				if (flag7)
				{
					vector = nameColumn.WorldToLocal(pointerPosition);
					flag6 = vector.x >= 0f && vector.x < nameColumn.layout.width;
				}
				bool flag8 = num5 <= num4;
				if (flag8)
				{
					this.m_LeftIndentation = num7 + num8 * (float)num4;
					bool flag9 = flag2;
					if (flag9)
					{
						hierarchyViewDragAndDropTargets.parentIndex = num;
						hierarchyViewDragAndDropTargets.childIndex = 0;
					}
					else
					{
						HierarchyNode parent = this.HierarchyFlattened.GetParent(in hierarchyNode);
						hierarchyViewDragAndDropTargets.parentIndex = this.GetNodeIndex(in parent);
						hierarchyViewDragAndDropTargets.childIndex = ((num2 == -1) ? this.HierarchyFlattened.GetChildrenCount(in parent) : this.GetChildIndex(in hierarchyNode2));
					}
					hierarchyViewDragAndDropTargets2 = hierarchyViewDragAndDropTargets;
				}
				else
				{
					int num10 = (flag6 ? Mathf.FloorToInt((vector.x - num7) / num8) : num5);
					bool flag10 = num10 >= num5;
					if (flag10)
					{
						this.m_LeftIndentation = num7 + num8 * (float)num5;
						bool flag11 = flag2;
						if (flag11)
						{
							hierarchyViewDragAndDropTargets.parentIndex = num;
							hierarchyViewDragAndDropTargets.childIndex = 0;
						}
						else
						{
							HierarchyNode parent2 = this.HierarchyFlattened.GetParent(in hierarchyNode);
							hierarchyViewDragAndDropTargets.parentIndex = this.GetNodeIndex(in parent2);
							hierarchyViewDragAndDropTargets.childIndex = this.GetChildIndex(in hierarchyNode) + 1;
						}
						hierarchyViewDragAndDropTargets2 = hierarchyViewDragAndDropTargets;
					}
					else
					{
						while (i > num4)
						{
							bool flag12 = i == num10;
							if (flag12)
							{
								break;
							}
							hierarchyNode3 = this.HierarchyFlattened.GetParent(in hierarchyNode3);
							num6 = this.GetNodeIndex(in hierarchyNode3);
							i--;
						}
						bool flag13 = num6 != insertAtIndex;
						bool flag14 = flag13;
						if (flag14)
						{
							VisualElement rootElementForIndex = this.TargetView.GetRootElementForIndex(num6);
							bool flag15 = rootElementForIndex != null;
							if (flag15)
							{
								VisualElement contentViewport = this.TargetScrollView.contentViewport;
								Rect rect = contentViewport.WorldToLocal(rootElementForIndex.worldBound);
								bool flag16 = contentViewport.localBound.yMin < rect.yMax && rect.yMax < contentViewport.localBound.yMax;
								if (flag16)
								{
									this.m_SiblingBottom = rect.yMax;
								}
							}
						}
						HierarchyNode parent3 = this.HierarchyFlattened.GetParent(in hierarchyNode3);
						hierarchyViewDragAndDropTargets.parentIndex = this.GetNodeIndex(in parent3);
						hierarchyViewDragAndDropTargets.targetIndex = num6;
						hierarchyViewDragAndDropTargets.childIndex = this.GetChildIndex(in hierarchyNode3) + 1;
						this.m_LeftIndentation = num7 + num8 * (float)i;
						hierarchyViewDragAndDropTargets2 = hierarchyViewDragAndDropTargets;
					}
				}
			}
			return hierarchyViewDragAndDropTargets2;
		}

		private void GetPreviousAndNextIndexesIgnoringDraggedItems(int insertAtIndex, out int previousNodeIndex, out int nextNodeIndex, in ReadOnlySpan<int> draggedIndices)
		{
			previousNodeIndex = (nextNodeIndex = -1);
			int i = insertAtIndex - 1;
			int j = insertAtIndex;
			while (i >= 0)
			{
				bool flag = !(in draggedIndices).Contains(i);
				if (flag)
				{
					previousNodeIndex = i;
					break;
				}
				i--;
			}
			int count = this.HierarchyViewModel.Count;
			while (j < count)
			{
				bool flag2 = !(in draggedIndices).Contains(j);
				if (flag2)
				{
					nextNodeIndex = j;
					break;
				}
				j++;
			}
		}

		private int GetChildIndex(in HierarchyNode childNode)
		{
			return this.HierarchyFlattened.GetChildIndex(in childNode);
		}

		private int GetNodeIndex(in HierarchyNode node)
		{
			return this.HierarchyViewModel.IndexOf(in node);
		}

		private void ApplyDragAndDropUI(HierarchyViewDragHandler.HierarchyViewDragAndDropTargets dragTargets)
		{
			bool flag = this.m_LastDragPosition.Equals(dragTargets);
			if (!flag)
			{
				ScrollView targetScrollView = this.TargetScrollView;
				bool flag2 = this.m_DragHoverBar == null;
				if (flag2)
				{
					this.m_DragHoverBar = new VisualElement
					{
						name = "HierarchyHoverBar"
					};
					this.m_DragHoverBar.AddToClassList(BaseVerticalCollectionView.dragHoverBarUssClassName);
					this.m_DragHoverBar.AddToClassList("hierarchy__container__drag-hover-bar");
					this.m_DragHoverBar.style.width = this.TargetView.localBound.width;
					this.m_DragHoverBar.style.visibility = Visibility.Hidden;
					this.m_DragHoverBar.pickingMode = PickingMode.Ignore;
					this.TargetView.RegisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.<ApplyDragAndDropUI>g__GeometryChangedCallback|57_0), TrickleDown.NoTrickleDown);
					targetScrollView.contentViewport.Add(this.m_DragHoverBar);
				}
				bool flag3 = this.m_DragHoverItemMarker == null;
				if (flag3)
				{
					this.m_DragHoverItemMarker = new VisualElement
					{
						name = "HierarchyHoverItemMarker"
					};
					this.m_DragHoverItemMarker.AddToClassList(BaseVerticalCollectionView.dragHoverMarkerUssClassName);
					this.m_DragHoverItemMarker.style.visibility = Visibility.Hidden;
					this.m_DragHoverItemMarker.pickingMode = PickingMode.Ignore;
					this.m_DragHoverBar.Add(this.m_DragHoverItemMarker);
					this.m_DragHoverSiblingMarker = new VisualElement
					{
						name = "HierarchyHoverSiblingMarker"
					};
					this.m_DragHoverSiblingMarker.AddToClassList(BaseVerticalCollectionView.dragHoverMarkerUssClassName);
					this.m_DragHoverSiblingMarker.style.visibility = Visibility.Hidden;
					this.m_DragHoverSiblingMarker.pickingMode = PickingMode.Ignore;
					targetScrollView.contentViewport.Add(this.m_DragHoverSiblingMarker);
				}
				this.ClearDragAndDropUI();
				this.m_LastDragPosition = dragTargets;
				switch (dragTargets.dropPosition)
				{
				case DragAndDropPosition.OverItem:
					break;
				case DragAndDropPosition.BetweenItems:
				{
					bool flag4 = dragTargets.insertAtIndex == 0;
					if (flag4)
					{
						this.PlaceHoverBarAt(0f, -1f, -1f);
					}
					else
					{
						VisualElement rootElementForIndex = this.TargetView.GetRootElementForIndex(dragTargets.insertAtIndex - 1);
						VisualElement rootElementForIndex2 = this.TargetView.GetRootElementForIndex(dragTargets.insertAtIndex);
						this.PlaceHoverBarAtElement(rootElementForIndex ?? rootElementForIndex2);
					}
					break;
				}
				case DragAndDropPosition.OutsideItems:
				{
					VisualElement rootElementForIndex3 = this.TargetView.GetRootElementForIndex(this.TargetView.itemsSource.Count - 1);
					bool flag5 = rootElementForIndex3 != null;
					if (flag5)
					{
						this.PlaceHoverBarAtElement(rootElementForIndex3);
					}
					else
					{
						this.PlaceHoverBarAt(0f, -1f, -1f);
					}
					break;
				}
				default:
					throw new ArgumentOutOfRangeException("dropPosition", dragTargets.dropPosition, "Unsupported dropPosition value");
				}
			}
		}

		private void ClearDragAndDropUI()
		{
			this.m_LastDragPosition = default(HierarchyViewDragHandler.HierarchyViewDragAndDropTargets);
			bool flag = this.m_DragHoverBar != null;
			if (flag)
			{
				this.m_DragHoverBar.style.visibility = Visibility.Hidden;
			}
			bool flag2 = this.m_DragHoverItemMarker != null;
			if (flag2)
			{
				this.m_DragHoverItemMarker.style.visibility = Visibility.Hidden;
			}
			bool flag3 = this.m_DragHoverSiblingMarker != null;
			if (flag3)
			{
				this.m_DragHoverSiblingMarker.style.visibility = Visibility.Hidden;
			}
		}

		private void ClearDragAndDrop()
		{
			this.m_CurrentEventModifiers = EventModifiers.None;
			this.ClearDragAndDropUI();
			this.ClearAutoExpansionData(true);
		}

		private void ClearAutoExpansionData(bool restoreState = true)
		{
			bool flag;
			if (restoreState)
			{
				HierarchyViewDragHandler.AutoExpansionData autoExpansionData = this.m_AutoExpansionData;
				flag = ((autoExpansionData != null) ? autoExpansionData.expandedNodesBeforeDrag : null) != null;
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				this.RestoreExpanded(this.m_AutoExpansionData.expandedNodesBeforeDrag);
			}
			this.m_AutoExpansionData = new HierarchyViewDragHandler.AutoExpansionData();
			IVisualElementScheduledItem expandItemScheduledItem = this.m_ExpandItemScheduledItem;
			if (expandItemScheduledItem != null)
			{
				expandItemScheduledItem.Pause();
			}
		}

		private void RestoreExpanded(ReadOnlySpan<HierarchyNode> expandedNodes)
		{
			using (new HierarchyViewModelFlagsChangeScope(this.HierarchyViewModel))
			{
				this.HierarchyViewModel.ClearFlags(HierarchyNodeFlags.Expanded);
				this.HierarchyViewModel.SetFlags(expandedNodes, HierarchyNodeFlags.Expanded);
			}
		}

		private float GetHoverBarTopPosition(VisualElement item)
		{
			VisualElement contentViewport = this.TargetScrollView.contentViewport;
			return Mathf.Min(contentViewport.WorldToLocal(item.worldBound).yMax, contentViewport.localBound.yMax - 2f);
		}

		private void PlaceHoverBarAtElement(VisualElement item)
		{
			this.PlaceHoverBarAt(this.GetHoverBarTopPosition(item), this.m_LeftIndentation, this.m_SiblingBottom);
		}

		private void PlaceHoverBarAt(float top, float indentationPadding = -1f, float siblingBottom = -1f)
		{
			this.m_DragHoverBar.style.top = top;
			this.m_DragHoverBar.style.visibility = Visibility.Visible;
			Rect nameColumnLayout = this.GetNameColumnLayout();
			float xMin = nameColumnLayout.xMin;
			float num = this.TargetView.localBound.width;
			bool flag = nameColumnLayout.width > 0f;
			if (flag)
			{
				num = nameColumnLayout.width;
			}
			else
			{
				indentationPadding = -1f;
			}
			bool flag2 = this.m_DragHoverItemMarker != null;
			if (flag2)
			{
				this.m_DragHoverItemMarker.style.visibility = Visibility.Visible;
			}
			bool flag3 = indentationPadding >= 0f;
			if (flag3)
			{
				this.m_DragHoverBar.style.marginLeft = xMin + indentationPadding;
				this.m_DragHoverBar.style.width = num - indentationPadding;
				bool flag4 = siblingBottom > 0f && this.m_DragHoverSiblingMarker != null;
				if (flag4)
				{
					this.m_DragHoverSiblingMarker.style.top = siblingBottom;
					this.m_DragHoverSiblingMarker.style.visibility = Visibility.Visible;
					this.m_DragHoverSiblingMarker.style.marginLeft = xMin + indentationPadding;
				}
			}
			else
			{
				this.m_DragHoverBar.style.marginLeft = xMin;
				this.m_DragHoverBar.style.width = num;
			}
		}

		private VisualElement GetNameColumn()
		{
			return this.TargetView.Q("HierarchyViewColumn Name", null);
		}

		private Rect GetNameColumnLayout()
		{
			VisualElement nameColumn = this.GetNameColumn();
			bool flag = nameColumn == null;
			Rect rect;
			if (flag)
			{
				rect = Rect.zero;
			}
			else
			{
				rect = nameColumn.layout;
			}
			return rect;
		}

		private void HandleAutoExpansion(HierarchyViewDragHandler.HierarchyViewDragAndDropTargets dropTargets, Vector2 pointerPosition)
		{
			bool flag = dropTargets.dropPosition > DragAndDropPosition.OverItem;
			if (!flag)
			{
				int parentIndex = dropTargets.parentIndex;
				VisualElement rootElementForIndex = this.m_MultiColumnListView.GetRootElementForIndex(parentIndex);
				bool flag2 = rootElementForIndex == null;
				if (!flag2)
				{
					this.HandleAutoExpansion(rootElementForIndex, parentIndex, pointerPosition);
				}
			}
		}

		private void HandleAutoExpansion(VisualElement item, int itemIndex, Vector2 pointerPosition)
		{
			Rect worldBound = item.worldBound;
			Rect rect = new Rect(worldBound.x, worldBound.y + 4f, worldBound.width, worldBound.height - 8f);
			bool flag = rect.Contains(pointerPosition);
			Vector2 vector = this.m_AutoExpansionData.expandItemBeginPosition - pointerPosition;
			bool flag2 = itemIndex != this.m_AutoExpansionData.lastItemIndex || !flag || vector.sqrMagnitude >= 100f;
			if (flag2)
			{
				this.m_AutoExpansionData.lastItemIndex = itemIndex;
				this.m_AutoExpansionData.expandItemBeginTimerMs = 0f;
				this.m_AutoExpansionData.expandItemBeginPosition = pointerPosition;
				this.DelayExpandItem();
			}
		}

		private void DelayExpandItem()
		{
			bool flag = this.m_ExpandItemScheduledItem == null;
			if (flag)
			{
				this.m_ExpandItemScheduledItem = this.m_MultiColumnListView.schedule.Execute(new Action<TimerState>(this.ExpandItem)).Every(10L);
			}
			else
			{
				this.m_ExpandItemScheduledItem.Pause();
				this.m_ExpandItemScheduledItem.Resume();
			}
		}

		internal unsafe void ExpandItem(TimerState state)
		{
			this.m_AutoExpansionData.expandItemBeginTimerMs = (float)state.deltaTime + this.m_AutoExpansionData.expandItemBeginTimerMs;
			bool flag = this.m_AutoExpansionData.expandItemBeginTimerMs > 700f;
			int lastItemIndex = this.m_AutoExpansionData.lastItemIndex;
			bool flag2 = flag && lastItemIndex >= 0 && lastItemIndex < this.HierarchyViewModel.Count;
			if (flag2)
			{
				HierarchyNode hierarchyNode = *this.HierarchyViewModel[lastItemIndex];
				bool flag3 = this.HierarchyViewModel.GetChildrenCount(in hierarchyNode) > 0;
				bool flag4 = this.HierarchyViewModel.HasAllFlags(in hierarchyNode, HierarchyNodeFlags.Expanded);
				bool flag5 = !flag3 || flag4;
				if (!flag5)
				{
					HierarchyNode[] nodesWithAllFlags = this.HierarchyViewModel.GetNodesWithAllFlags(HierarchyNodeFlags.Expanded);
					HierarchyViewDragHandler.AutoExpansionData autoExpansionData = this.m_AutoExpansionData;
					if (autoExpansionData.expandedNodesBeforeDrag == null)
					{
						autoExpansionData.expandedNodesBeforeDrag = nodesWithAllFlags;
					}
					this.m_AutoExpansionData.expandItemBeginTimerMs = 0f;
					this.m_AutoExpansionData.lastItemIndex = -1;
					this.HierarchyViewModel.SetFlags(in hierarchyNode, HierarchyNodeFlags.Expanded);
				}
			}
		}

		[CompilerGenerated]
		private void <ApplyDragAndDropUI>g__GeometryChangedCallback|57_0(GeometryChangedEvent e)
		{
			this.m_DragHoverBar.style.width = this.TargetView.localBound.width;
		}

		internal const string DragHoverBarStyleName = "hierarchy__container__drag-hover-bar";

		internal const string DragHoverBarItemName = "HierarchyHoverBar";

		internal const string DragHoverItemMarkerItemName = "HierarchyHoverItemMarker";

		internal const string DragHoverSiblingMarkerItemName = "HierarchyHoverSiblingMarker";

		private readonly HierarchyView m_HierarchyView;

		private readonly MultiColumnListView m_MultiColumnListView;

		private HierarchyViewDragHandler.HierarchyViewDragAndDropTargets m_LastDragPosition;

		private HierarchyViewDragHandler.AutoExpansionData m_AutoExpansionData;

		private IVisualElementScheduledItem m_ExpandItemScheduledItem;

		private VisualElement m_DragHoverBar;

		private VisualElement m_DragHoverItemMarker;

		private VisualElement m_DragHoverSiblingMarker;

		private EventModifiers m_CurrentEventModifiers;

		private float m_LeftIndentation = -1f;

		private float m_SiblingBottom = -1f;

		private const int k_DragHoverBarHeight = 2;

		private const int k_InvalidIndex = -1;

		private const long k_ExpandUpdateIntervalMs = 10L;

		private const float k_DropExpandTimeoutMs = 700f;

		private const float k_DropDeltaPosition = 100f;

		private const float k_HalfDropBetweenHeight = 4f;

		private const float k_DefaultIndentWidth = 14f;

		private const float k_DragHoverBarPositionOffset = 4f;

		private struct HierarchyViewDragAndDropTargets : IEquatable<HierarchyViewDragHandler.HierarchyViewDragAndDropTargets>
		{
			public HierarchyViewDragAndDropTargets(int insertAtIndex, int targetIndex, int parentIndex, int childIndex, DragAndDropPosition dropPosition)
			{
				this.insertAtIndex = insertAtIndex;
				this.targetIndex = targetIndex;
				this.parentIndex = parentIndex;
				this.childIndex = childIndex;
				this.dropPosition = dropPosition;
				this.dragVisualMode = DragVisualMode.Move;
			}

			public bool Equals(HierarchyViewDragHandler.HierarchyViewDragAndDropTargets other)
			{
				return this.parentIndex == other.parentIndex && this.childIndex == other.childIndex && this.dropPosition == other.dropPosition && this.targetIndex == other.targetIndex;
			}

			public override bool Equals(object obj)
			{
				bool flag;
				if (obj is HierarchyViewDragHandler.HierarchyViewDragAndDropTargets)
				{
					HierarchyViewDragHandler.HierarchyViewDragAndDropTargets hierarchyViewDragAndDropTargets = (HierarchyViewDragHandler.HierarchyViewDragAndDropTargets)obj;
					flag = this.Equals(hierarchyViewDragAndDropTargets);
				}
				else
				{
					flag = false;
				}
				return flag;
			}

			public override int GetHashCode()
			{
				return HashCode.Combine<int, int, int>(this.parentIndex, this.childIndex, (int)this.dropPosition);
			}

			public int insertAtIndex;

			public int targetIndex;

			public int parentIndex;

			public int childIndex;

			public DragAndDropPosition dropPosition;

			public DragVisualMode dragVisualMode;

			[NoAutoStaticsCleanup]
			public static readonly HierarchyViewDragHandler.HierarchyViewDragAndDropTargets Rejected = new HierarchyViewDragHandler.HierarchyViewDragAndDropTargets(-1, -1, -1, -1, DragAndDropPosition.OverItem)
			{
				dragVisualMode = DragVisualMode.Rejected
			};
		}

		private class AutoExpansionData
		{
			public HierarchyNode[] expandedNodesBeforeDrag;

			public int lastItemIndex = -1;

			public float expandItemBeginTimerMs;

			public Vector2 expandItemBeginPosition;
		}
	}
}
