using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	public readonly struct ContactPairPoint
	{
		public Vector3 position
		{
			get
			{
				return this.m_Position;
			}
		}

		public float separation
		{
			get
			{
				return this.m_Separation;
			}
		}

		public Vector3 normal
		{
			get
			{
				return this.m_Normal;
			}
		}

		public Vector3 impulse
		{
			get
			{
				return this.m_Impulse;
			}
		}

		[Obsolete("Please use ContactPairPoint.position instead. (UnityUpgradable) -> position", false)]
		public Vector3 Position
		{
			get
			{
				return this.position;
			}
		}

		[Obsolete("Please use ContactPairPoint.separation instead. (UnityUpgradable) -> separation", false)]
		public float Separation
		{
			get
			{
				return this.separation;
			}
		}

		[Obsolete("Please use ContactPairPoint.normal instead. (UnityUpgradable) -> normal", false)]
		public Vector3 Normal
		{
			get
			{
				return this.normal;
			}
		}

		[Obsolete("Please use ContactPairPoint.impulse instead. (UnityUpgradable) -> impulse", false)]
		public Vector3 Impulse
		{
			get
			{
				return this.impulse;
			}
		}

		internal readonly Vector3 m_Position;

		internal readonly float m_Separation;

		internal readonly Vector3 m_Normal;

		internal readonly uint m_InternalFaceIndex0;

		internal readonly Vector3 m_Impulse;

		internal readonly uint m_InternalFaceIndex1;
	}
}
