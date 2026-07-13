using System;
using UnityEngine.Bindings;

namespace UnityEngine.TextCore.Text
{
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule", "UnityEngine.IMGUIModule" })]
	[NativeHeader("Modules/TextCoreTextEngine/Native/TextInfo.h")]
	internal struct NativeTextInfo
	{
		public Span<ATGMeshInfo> meshInfos
		{
			get
			{
				bool flag = this.m_MeshInfosPtr == IntPtr.Zero || this.meshInfoCount <= 0;
				Span<ATGMeshInfo> span;
				if (flag)
				{
					span = default(Span<ATGMeshInfo>);
				}
				else
				{
					span = new Span<ATGMeshInfo>(this.m_MeshInfosPtr.ToPointer(), this.meshInfoCount);
				}
				return span;
			}
		}

		private IntPtr m_MeshInfosPtr;

		public int meshInfoCount;

		public int totalWidth;

		public int totalHeight;

		public bool isElided;
	}
}
