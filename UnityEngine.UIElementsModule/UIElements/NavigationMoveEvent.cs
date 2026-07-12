using System;

namespace UnityEngine.UIElements
{
	public class NavigationMoveEvent : NavigationEventBase<NavigationMoveEvent>
	{
		static NavigationMoveEvent()
		{
			EventBase<NavigationMoveEvent>.SetCreateFunction(() => new NavigationMoveEvent());
		}

		internal static NavigationMoveEvent.Direction DetermineMoveDirection(float x, float y, float deadZone = 0.6f)
		{
			bool flag = new Vector2(x, y).sqrMagnitude < deadZone * deadZone;
			NavigationMoveEvent.Direction direction;
			if (flag)
			{
				direction = NavigationMoveEvent.Direction.None;
			}
			else
			{
				bool flag2 = Mathf.Abs(x) > Mathf.Abs(y);
				if (flag2)
				{
					bool flag3 = x > 0f;
					if (flag3)
					{
						direction = NavigationMoveEvent.Direction.Right;
					}
					else
					{
						direction = NavigationMoveEvent.Direction.Left;
					}
				}
				else
				{
					bool flag4 = y > 0f;
					if (flag4)
					{
						direction = NavigationMoveEvent.Direction.Up;
					}
					else
					{
						direction = NavigationMoveEvent.Direction.Down;
					}
				}
			}
			return direction;
		}

		public NavigationMoveEvent.Direction direction { get; private set; }

		public Vector2 move { get; private set; }

		public static NavigationMoveEvent GetPooled(Vector2 moveVector, EventModifiers modifiers = EventModifiers.None)
		{
			NavigationMoveEvent pooled = NavigationEventBase<NavigationMoveEvent>.GetPooled(NavigationDeviceType.Unknown, modifiers);
			pooled.direction = NavigationMoveEvent.DetermineMoveDirection(moveVector.x, moveVector.y, 0.6f);
			pooled.move = moveVector;
			return pooled;
		}

		internal static NavigationMoveEvent GetPooled(Vector2 moveVector, NavigationDeviceType deviceType, EventModifiers modifiers = EventModifiers.None)
		{
			NavigationMoveEvent pooled = NavigationEventBase<NavigationMoveEvent>.GetPooled(deviceType, modifiers);
			pooled.direction = NavigationMoveEvent.DetermineMoveDirection(moveVector.x, moveVector.y, 0.6f);
			pooled.move = moveVector;
			return pooled;
		}

		public static NavigationMoveEvent GetPooled(NavigationMoveEvent.Direction direction, EventModifiers modifiers = EventModifiers.None)
		{
			NavigationMoveEvent pooled = NavigationEventBase<NavigationMoveEvent>.GetPooled(NavigationDeviceType.Unknown, modifiers);
			pooled.direction = direction;
			pooled.move = Vector2.zero;
			return pooled;
		}

		internal static NavigationMoveEvent GetPooled(NavigationMoveEvent.Direction direction, NavigationDeviceType deviceType, EventModifiers modifiers = EventModifiers.None)
		{
			NavigationMoveEvent pooled = NavigationEventBase<NavigationMoveEvent>.GetPooled(deviceType, modifiers);
			pooled.direction = direction;
			pooled.move = Vector2.zero;
			return pooled;
		}

		protected override void Init()
		{
			base.Init();
			this.LocalInit();
		}

		public NavigationMoveEvent()
		{
			this.LocalInit();
		}

		private void LocalInit()
		{
			this.direction = NavigationMoveEvent.Direction.None;
			this.move = Vector2.zero;
		}

		public enum Direction
		{
			None,
			Left,
			Up,
			Right,
			Down,
			Next,
			Previous
		}
	}
}
