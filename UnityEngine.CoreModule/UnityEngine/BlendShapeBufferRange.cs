using System;

namespace UnityEngine
{
	[Serializable]
	public struct BlendShapeBufferRange
	{
		public uint startIndex
		{
			get
			{
				return this.m_StartIndex;
			}
			internal set
			{
				this.m_StartIndex = value;
			}
		}

		public uint endIndex
		{
			get
			{
				return this.m_EndIndex;
			}
			internal set
			{
				this.m_EndIndex = value;
			}
		}

		[SerializeField]
		private uint m_StartIndex;

		[SerializeField]
		private uint m_EndIndex;
	}
}
