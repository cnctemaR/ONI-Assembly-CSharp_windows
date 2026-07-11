using System;

namespace UnityEngine.Networking
{
	[Obsolete("The high level API classes are deprecated and will be removed in the future.")]
	[Serializable]
	public struct NetworkSceneId : IEquatable<NetworkSceneId>
	{
		public NetworkSceneId(uint value)
		{
			this.m_Value = value;
		}

		public bool IsEmpty()
		{
			return this.m_Value == 0U;
		}

		public override int GetHashCode()
		{
			return (int)this.m_Value;
		}

		public override bool Equals(object obj)
		{
			return obj is NetworkSceneId && this.Equals((NetworkSceneId)obj);
		}

		public bool Equals(NetworkSceneId other)
		{
			return this == other;
		}

		public static bool operator ==(NetworkSceneId c1, NetworkSceneId c2)
		{
			return c1.m_Value == c2.m_Value;
		}

		public static bool operator !=(NetworkSceneId c1, NetworkSceneId c2)
		{
			return c1.m_Value != c2.m_Value;
		}

		public override string ToString()
		{
			return this.m_Value.ToString();
		}

		public uint Value
		{
			get
			{
				return this.m_Value;
			}
		}

		[SerializeField]
		private uint m_Value;
	}
}
