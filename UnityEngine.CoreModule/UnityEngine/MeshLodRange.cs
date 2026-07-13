using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	[Serializable]
	public struct MeshLodRange
	{
		public uint indexStart
		{
			get
			{
				return this.m_IndexStart;
			}
			set
			{
				this.m_IndexStart = value;
			}
		}

		public uint indexCount
		{
			get
			{
				return this.m_IndexCount;
			}
			set
			{
				this.m_IndexCount = value;
			}
		}

		public MeshLodRange(uint indexStart, uint indexCount)
		{
			this.m_IndexStart = indexStart;
			this.m_IndexCount = indexCount;
		}

		public override string ToString()
		{
			return string.Format("MeshLodRange start:{0} count:{1})", this.m_IndexStart, this.m_IndexCount);
		}

		[SerializeField]
		private uint m_IndexStart;

		[SerializeField]
		private uint m_IndexCount;
	}
}
