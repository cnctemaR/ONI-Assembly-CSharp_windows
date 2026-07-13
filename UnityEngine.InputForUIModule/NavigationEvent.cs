using System;
using Unity.IntegerTime;
using UnityEngine.Bindings;

namespace UnityEngine.InputForUI
{
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal struct NavigationEvent : IEventProperties
	{
		public DiscreteTime timestamp { readonly get; set; }

		public EventSource eventSource { readonly get; set; }

		public uint playerId { readonly get; set; }

		public EventModifiers eventModifiers { readonly get; set; }

		public override string ToString()
		{
			return string.Format("Navigation {0}", this.type) + ((this.type == NavigationEvent.Type.Move) ? string.Format(" {0}", this.direction) : "") + ((this.eventSource != EventSource.Keyboard) ? string.Format(" {0}", this.eventSource) : "");
		}

		internal static NavigationEvent.Direction DetermineMoveDirection(Vector2 vec, float deadZone = 0.6f)
		{
			bool flag = vec.sqrMagnitude < deadZone * deadZone;
			NavigationEvent.Direction direction;
			if (flag)
			{
				direction = NavigationEvent.Direction.None;
			}
			else
			{
				bool flag2 = Mathf.Abs(vec.x) > Mathf.Abs(vec.y);
				if (flag2)
				{
					direction = ((vec.x > 0f) ? NavigationEvent.Direction.Right : NavigationEvent.Direction.Left);
				}
				else
				{
					direction = ((vec.y > 0f) ? NavigationEvent.Direction.Up : NavigationEvent.Direction.Down);
				}
			}
			return direction;
		}

		public NavigationEvent.Type type;

		public NavigationEvent.Direction direction;

		public bool shouldBeUsed;

		public enum Type
		{
			Move = 1,
			Submit,
			Cancel
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
