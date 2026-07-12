using System;

namespace UnityEngine
{
	public struct PenData
	{
		public Vector2 position;

		public Vector2 tilt;

		public PenStatus penStatus;

		public float twist;

		public float pressure;

		public PenEventType contactType;

		public Vector2 deltaPos;
	}
}
