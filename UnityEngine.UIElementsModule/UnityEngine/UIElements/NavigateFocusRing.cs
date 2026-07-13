using System;

namespace UnityEngine.UIElements
{
	internal class NavigateFocusRing : IFocusRing
	{
		private FocusController focusController
		{
			get
			{
				return this.m_Root.focusController;
			}
		}

		public NavigateFocusRing(VisualElement root)
		{
			this.m_Root = root;
			this.m_Ring = new VisualElementFocusRing(root, VisualElementFocusRing.DefaultFocusOrder.ChildOrder);
		}

		public FocusChangeDirection GetFocusChangeDirection(Focusable currentFocusable, EventBase e)
		{
			bool flag = e.eventTypeId == EventBase<PointerDownEvent>.TypeId();
			if (flag)
			{
				Focusable focusable;
				bool focusableParentForPointerEvent = this.focusController.GetFocusableParentForPointerEvent(e.elementTarget, out focusable);
				if (focusableParentForPointerEvent)
				{
					return VisualElementFocusChangeTarget.GetPooled(focusable);
				}
			}
			bool flag2 = e.eventTypeId == EventBase<NavigationMoveEvent>.TypeId();
			if (flag2)
			{
				switch (((NavigationMoveEvent)e).direction)
				{
				case NavigationMoveEvent.Direction.Left:
					return NavigateFocusRing.Left;
				case NavigationMoveEvent.Direction.Up:
					return NavigateFocusRing.Up;
				case NavigationMoveEvent.Direction.Right:
					return NavigateFocusRing.Right;
				case NavigationMoveEvent.Direction.Down:
					return NavigateFocusRing.Down;
				case NavigationMoveEvent.Direction.Next:
					return NavigateFocusRing.Next;
				case NavigationMoveEvent.Direction.Previous:
					return NavigateFocusRing.Previous;
				}
			}
			return FocusChangeDirection.none;
		}

		public virtual Focusable GetNextFocusable(Focusable currentFocusable, FocusChangeDirection direction)
		{
			bool flag = direction == FocusChangeDirection.none || direction == FocusChangeDirection.unspecified;
			Focusable focusable;
			if (flag)
			{
				focusable = currentFocusable;
			}
			else
			{
				VisualElementFocusChangeTarget visualElementFocusChangeTarget = direction as VisualElementFocusChangeTarget;
				bool flag2 = visualElementFocusChangeTarget != null;
				if (flag2)
				{
					focusable = visualElementFocusChangeTarget.target;
				}
				else
				{
					VisualElement visualElement = this.m_Root;
					VisualElement root = this.m_Root;
					bool flag3;
					if (root == null)
					{
						flag3 = false;
					}
					else
					{
						BaseVisualElementPanel elementPanel = root.elementPanel;
						bool? flag4 = ((elementPanel != null) ? new bool?(elementPanel.isFlat) : null);
						bool flag5 = false;
						flag3 = (flag4.GetValueOrDefault() == flag5) & (flag4 != null);
					}
					bool flag6 = flag3;
					if (flag6)
					{
						UIDocument uidocument;
						bool flag7 = !this.IsWorldSpaceNavigationValid(currentFocusable, out uidocument);
						if (flag7)
						{
							return null;
						}
						bool flag8 = direction == NavigateFocusRing.Next || direction == NavigateFocusRing.Previous;
						if (flag8)
						{
							VisualElementFocusRing focusRing = uidocument.focusRing;
							return (focusRing != null) ? focusRing.GetNextFocusableInSequence(currentFocusable, direction) : null;
						}
						visualElement = uidocument.rootVisualElement;
					}
					bool flag9 = direction == NavigateFocusRing.Up || direction == NavigateFocusRing.Down || direction == NavigateFocusRing.Right || direction == NavigateFocusRing.Left;
					if (flag9)
					{
						focusable = this.GetNextFocusable2D(currentFocusable, (NavigateFocusRing.ChangeDirection)direction, visualElement);
					}
					else
					{
						focusable = this.m_Ring.GetNextFocusableInSequence(currentFocusable, direction);
					}
				}
			}
			return focusable;
		}

		private bool IsWorldSpaceNavigationValid(Focusable currentFocusable, out UIDocument document)
		{
			document = null;
			VisualElement visualElement = currentFocusable as VisualElement;
			bool flag = visualElement == null;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				document = UIDocument.FindRootUIDocument(visualElement);
				bool flag3 = document == null || document.rootVisualElement == null;
				flag2 = !flag3;
			}
			return flag2;
		}

		private Focusable GetNextFocusable2D(Focusable currentFocusable, NavigateFocusRing.ChangeDirection direction, VisualElement root)
		{
			VisualElement visualElement = currentFocusable as VisualElement;
			bool flag = visualElement == null;
			if (flag)
			{
				visualElement = root;
			}
			Rect boundingBox = root.boundingBox;
			Rect rect = new Rect(boundingBox.position - Vector2.one, boundingBox.size + Vector2.one * 2f);
			Rect rect2 = visualElement.ChangeCoordinatesTo(root, visualElement.rect);
			Rect rect3 = new Rect(rect2.position - Vector2.one, rect2.size + Vector2.one * 2f);
			bool flag2 = direction == NavigateFocusRing.Up;
			if (flag2)
			{
				rect3.yMin = rect.yMin;
			}
			else
			{
				bool flag3 = direction == NavigateFocusRing.Down;
				if (flag3)
				{
					rect3.yMax = rect.yMax;
				}
				else
				{
					bool flag4 = direction == NavigateFocusRing.Left;
					if (flag4)
					{
						rect3.xMin = rect.xMin;
					}
					else
					{
						bool flag5 = direction == NavigateFocusRing.Right;
						if (flag5)
						{
							rect3.xMax = rect.xMax;
						}
					}
				}
			}
			NavigateFocusRing.FocusableHierarchyTraversal focusableHierarchyTraversal = default(NavigateFocusRing.FocusableHierarchyTraversal);
			focusableHierarchyTraversal.root = root;
			focusableHierarchyTraversal.currentFocusable = visualElement;
			focusableHierarchyTraversal.direction = direction;
			focusableHierarchyTraversal.validRect = rect3;
			focusableHierarchyTraversal.firstPass = true;
			VisualElement visualElement2 = focusableHierarchyTraversal.GetBestOverall(root, null);
			bool flag6 = visualElement2 != null;
			Focusable focusable;
			if (flag6)
			{
				focusable = visualElement2;
			}
			else
			{
				rect3 = new Rect(rect2.position - Vector2.one, rect2.size + Vector2.one * 2f);
				bool flag7 = direction == NavigateFocusRing.Down;
				if (flag7)
				{
					rect3.yMin = rect.yMin;
				}
				else
				{
					bool flag8 = direction == NavigateFocusRing.Up;
					if (flag8)
					{
						rect3.yMax = rect.yMax;
					}
					else
					{
						bool flag9 = direction == NavigateFocusRing.Right;
						if (flag9)
						{
							rect3.xMin = rect.xMin;
						}
						else
						{
							bool flag10 = direction == NavigateFocusRing.Left;
							if (flag10)
							{
								rect3.xMax = rect.xMax;
							}
						}
					}
				}
				focusableHierarchyTraversal = default(NavigateFocusRing.FocusableHierarchyTraversal);
				focusableHierarchyTraversal.root = root;
				focusableHierarchyTraversal.currentFocusable = visualElement;
				focusableHierarchyTraversal.direction = direction;
				focusableHierarchyTraversal.validRect = rect3;
				focusableHierarchyTraversal.firstPass = false;
				visualElement2 = focusableHierarchyTraversal.GetBestOverall(root, null);
				bool flag11 = visualElement2 != null;
				if (flag11)
				{
					focusable = visualElement2;
				}
				else
				{
					focusable = currentFocusable;
				}
			}
			return focusable;
		}

		private static bool IsActive(VisualElement v)
		{
			return v.resolvedStyle.display != DisplayStyle.None && v.enabledInHierarchy;
		}

		private static bool IsNavigable(Focusable focusable)
		{
			return focusable.canGrabFocus && focusable.tabIndex >= 0 && !focusable.delegatesFocus && !focusable.excludeFromFocusRing;
		}

		public static readonly NavigateFocusRing.ChangeDirection Left = new NavigateFocusRing.ChangeDirection(1);

		public static readonly NavigateFocusRing.ChangeDirection Right = new NavigateFocusRing.ChangeDirection(2);

		public static readonly NavigateFocusRing.ChangeDirection Up = new NavigateFocusRing.ChangeDirection(3);

		public static readonly NavigateFocusRing.ChangeDirection Down = new NavigateFocusRing.ChangeDirection(4);

		public static readonly FocusChangeDirection Next = VisualElementFocusChangeDirection.right;

		public static readonly FocusChangeDirection Previous = VisualElementFocusChangeDirection.left;

		private readonly VisualElement m_Root;

		private readonly VisualElementFocusRing m_Ring;

		public class ChangeDirection : FocusChangeDirection
		{
			public ChangeDirection(int i)
				: base(i)
			{
			}
		}

		private struct FocusableHierarchyTraversal
		{
			private bool ValidateHierarchyTraversal(VisualElement v)
			{
				return NavigateFocusRing.IsActive(v) && v.ChangeCoordinatesTo(this.root, v.boundingBox).Overlaps(this.validRect);
			}

			private bool ValidateElement(VisualElement v)
			{
				return NavigateFocusRing.IsNavigable(v) && v.ChangeCoordinatesTo(this.root, v.rect).Overlaps(this.validRect);
			}

			private int Order(VisualElement a, VisualElement b)
			{
				Rect rect = a.ChangeCoordinatesTo(this.root, a.rect);
				Rect rect2 = b.ChangeCoordinatesTo(this.root, b.rect);
				int num = this.StrictOrder(rect, rect2);
				return (num != 0) ? num : this.TieBreaker(rect, rect2);
			}

			private int StrictOrder(VisualElement a, VisualElement b)
			{
				return this.StrictOrder(a.ChangeCoordinatesTo(this.root, a.rect), b.ChangeCoordinatesTo(this.root, b.rect));
			}

			private int StrictOrder(Rect ra, Rect rb)
			{
				float num = 0f;
				bool flag = this.direction == NavigateFocusRing.Up;
				if (flag)
				{
					num = rb.yMax - ra.yMax;
				}
				else
				{
					bool flag2 = this.direction == NavigateFocusRing.Down;
					if (flag2)
					{
						num = ra.yMin - rb.yMin;
					}
					else
					{
						bool flag3 = this.direction == NavigateFocusRing.Left;
						if (flag3)
						{
							num = rb.xMax - ra.xMax;
						}
						else
						{
							bool flag4 = this.direction == NavigateFocusRing.Right;
							if (flag4)
							{
								num = ra.xMin - rb.xMin;
							}
						}
					}
				}
				bool flag5 = !Mathf.Approximately(num, 0f);
				int num2;
				if (flag5)
				{
					num2 = ((num > 0f) ? 1 : (-1));
				}
				else
				{
					num2 = 0;
				}
				return num2;
			}

			private int TieBreaker(Rect ra, Rect rb)
			{
				Rect rect = this.currentFocusable.ChangeCoordinatesTo(this.root, this.currentFocusable.rect);
				float num = (ra.min - rect.min).sqrMagnitude - (rb.min - rect.min).sqrMagnitude;
				bool flag = !Mathf.Approximately(num, 0f);
				int num2;
				if (flag)
				{
					num2 = ((num > 0f) ? 1 : (-1));
				}
				else
				{
					num2 = 0;
				}
				return num2;
			}

			public VisualElement GetBestOverall(VisualElement candidate, VisualElement bestSoFar = null)
			{
				bool flag = !this.ValidateHierarchyTraversal(candidate);
				VisualElement visualElement;
				if (flag)
				{
					visualElement = bestSoFar;
				}
				else
				{
					bool flag2 = this.ValidateElement(candidate);
					if (flag2)
					{
						bool flag3 = (!this.firstPass || this.StrictOrder(candidate, this.currentFocusable) > 0) && (bestSoFar == null || this.Order(bestSoFar, candidate) > 0);
						if (flag3)
						{
							bestSoFar = candidate;
						}
						visualElement = bestSoFar;
					}
					else
					{
						int childCount = candidate.hierarchy.childCount;
						for (int i = 0; i < childCount; i++)
						{
							VisualElement visualElement2 = candidate.hierarchy[i];
							bestSoFar = this.GetBestOverall(visualElement2, bestSoFar);
						}
						visualElement = bestSoFar;
					}
				}
				return visualElement;
			}

			public VisualElement root;

			public VisualElement currentFocusable;

			public Rect validRect;

			public bool firstPass;

			public NavigateFocusRing.ChangeDirection direction;
		}
	}
}
