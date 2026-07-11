using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>Implementation of a linear focus ring. Elements are sorted according to their focusIndex.</para>
	/// </summary>
	public class VisualElementFocusRing : IFocusRing
	{
		public VisualElementFocusRing(VisualElement root, VisualElementFocusRing.DefaultFocusOrder dfo = VisualElementFocusRing.DefaultFocusOrder.ChildOrder)
		{
			this.defaultFocusOrder = dfo;
			this.root = root;
			this.m_FocusRing = new List<VisualElementFocusRing.FocusRingRecord>();
		}

		/// <summary>
		///   <para>The focus order for elements having 0 has a focusIndex.</para>
		/// </summary>
		public VisualElementFocusRing.DefaultFocusOrder defaultFocusOrder { get; set; }

		private int FocusRingAutoIndexSort(VisualElementFocusRing.FocusRingRecord a, VisualElementFocusRing.FocusRingRecord b)
		{
			int num;
			switch (this.defaultFocusOrder)
			{
			default:
				num = Comparer<int>.Default.Compare(a.m_AutoIndex, b.m_AutoIndex);
				break;
			case VisualElementFocusRing.DefaultFocusOrder.PositionXY:
			{
				VisualElement visualElement = a.m_Focusable as VisualElement;
				VisualElement visualElement2 = b.m_Focusable as VisualElement;
				if (visualElement != null && visualElement2 != null)
				{
					if (visualElement.layout.position.x < visualElement2.layout.position.x)
					{
						num = -1;
						break;
					}
					if (visualElement.layout.position.x > visualElement2.layout.position.x)
					{
						num = 1;
						break;
					}
					if (visualElement.layout.position.y < visualElement2.layout.position.y)
					{
						num = -1;
						break;
					}
					if (visualElement.layout.position.y > visualElement2.layout.position.y)
					{
						num = 1;
						break;
					}
				}
				num = Comparer<int>.Default.Compare(a.m_AutoIndex, b.m_AutoIndex);
				break;
			}
			case VisualElementFocusRing.DefaultFocusOrder.PositionYX:
			{
				VisualElement visualElement3 = a.m_Focusable as VisualElement;
				VisualElement visualElement4 = b.m_Focusable as VisualElement;
				if (visualElement3 != null && visualElement4 != null)
				{
					if (visualElement3.layout.position.y < visualElement4.layout.position.y)
					{
						num = -1;
						break;
					}
					if (visualElement3.layout.position.y > visualElement4.layout.position.y)
					{
						num = 1;
						break;
					}
					if (visualElement3.layout.position.x < visualElement4.layout.position.x)
					{
						num = -1;
						break;
					}
					if (visualElement3.layout.position.x > visualElement4.layout.position.x)
					{
						num = 1;
						break;
					}
				}
				num = Comparer<int>.Default.Compare(a.m_AutoIndex, b.m_AutoIndex);
				break;
			}
			}
			return num;
		}

		private int FocusRingSort(VisualElementFocusRing.FocusRingRecord a, VisualElementFocusRing.FocusRingRecord b)
		{
			int num;
			if (a.m_Focusable.focusIndex == 0 && b.m_Focusable.focusIndex == 0)
			{
				num = this.FocusRingAutoIndexSort(a, b);
			}
			else if (a.m_Focusable.focusIndex == 0)
			{
				num = 1;
			}
			else if (b.m_Focusable.focusIndex == 0)
			{
				num = -1;
			}
			else
			{
				int num2 = Comparer<int>.Default.Compare(a.m_Focusable.focusIndex, b.m_Focusable.focusIndex);
				if (num2 == 0)
				{
					num2 = this.FocusRingAutoIndexSort(a, b);
				}
				num = num2;
			}
			return num;
		}

		private void DoUpdate()
		{
			this.m_FocusRing.Clear();
			if (this.root != null)
			{
				int num = 0;
				this.BuildRingRecursive(this.root, ref num);
				this.m_FocusRing.Sort(new Comparison<VisualElementFocusRing.FocusRingRecord>(this.FocusRingSort));
			}
		}

		private void BuildRingRecursive(VisualElement vc, ref int focusIndex)
		{
			for (int i = 0; i < vc.shadow.childCount; i++)
			{
				VisualElement visualElement = vc.shadow[i];
				if (visualElement.canGrabFocus)
				{
					this.m_FocusRing.Add(new VisualElementFocusRing.FocusRingRecord
					{
						m_AutoIndex = focusIndex++,
						m_Focusable = visualElement
					});
				}
				this.BuildRingRecursive(visualElement, ref focusIndex);
			}
		}

		private int GetFocusableInternalIndex(Focusable f)
		{
			if (f != null)
			{
				for (int i = 0; i < this.m_FocusRing.Count; i++)
				{
					if (f == this.m_FocusRing[i].m_Focusable)
					{
						return i;
					}
				}
			}
			return -1;
		}

		/// <summary>
		///   <para>Get the direction of the focus change for the given event. For example, when the Tab key is pressed, focus should be given to the element to the right in the focus ring.</para>
		/// </summary>
		/// <param name="currentFocusable"></param>
		/// <param name="e"></param>
		public FocusChangeDirection GetFocusChangeDirection(Focusable currentFocusable, EventBase e)
		{
			FocusChangeDirection focusChangeDirection;
			if (currentFocusable is IMGUIContainer && e.imguiEvent != null)
			{
				focusChangeDirection = FocusChangeDirection.none;
			}
			else
			{
				if (e.GetEventTypeId() == EventBase<KeyDownEvent>.TypeId())
				{
					KeyDownEvent keyDownEvent = e as KeyDownEvent;
					EventModifiers modifiers = keyDownEvent.modifiers;
					if (keyDownEvent.character == '\t')
					{
						if (currentFocusable == null)
						{
							return FocusChangeDirection.none;
						}
						if ((modifiers & EventModifiers.Shift) == EventModifiers.None)
						{
							return VisualElementFocusChangeDirection.right;
						}
						return VisualElementFocusChangeDirection.left;
					}
				}
				focusChangeDirection = FocusChangeDirection.none;
			}
			return focusChangeDirection;
		}

		/// <summary>
		///   <para>Get the next element in the given direction.</para>
		/// </summary>
		/// <param name="currentFocusable"></param>
		/// <param name="direction"></param>
		public Focusable GetNextFocusable(Focusable currentFocusable, FocusChangeDirection direction)
		{
			Focusable focusable;
			if (direction == FocusChangeDirection.none || direction == FocusChangeDirection.unspecified)
			{
				focusable = currentFocusable;
			}
			else
			{
				this.DoUpdate();
				if (this.m_FocusRing.Count == 0)
				{
					focusable = null;
				}
				else
				{
					int num = 0;
					if (direction == VisualElementFocusChangeDirection.right)
					{
						num = this.GetFocusableInternalIndex(currentFocusable) + 1;
						if (num == this.m_FocusRing.Count)
						{
							num = 0;
						}
					}
					else if (direction == VisualElementFocusChangeDirection.left)
					{
						num = this.GetFocusableInternalIndex(currentFocusable) - 1;
						if (num == -1)
						{
							num = this.m_FocusRing.Count - 1;
						}
					}
					focusable = this.m_FocusRing[num].m_Focusable;
				}
			}
			return focusable;
		}

		private VisualElement root;

		private List<VisualElementFocusRing.FocusRingRecord> m_FocusRing;

		/// <summary>
		///   <para>Ordering of elements in the focus ring.</para>
		/// </summary>
		public enum DefaultFocusOrder
		{
			/// <summary>
			///   <para>Order elements using a depth-first pre-order traversal of the element tree.</para>
			/// </summary>
			ChildOrder,
			/// <summary>
			///   <para>Order elements according to their position, first by X, then by Y.</para>
			/// </summary>
			PositionXY,
			/// <summary>
			///   <para>Order elements according to their position, first by Y, then by X.</para>
			/// </summary>
			PositionYX
		}

		private struct FocusRingRecord
		{
			public int m_AutoIndex;

			public Focusable m_Focusable;
		}
	}
}
