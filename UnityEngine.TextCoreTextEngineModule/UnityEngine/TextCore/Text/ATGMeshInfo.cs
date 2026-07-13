using System;
using UnityEngine.Bindings;

namespace UnityEngine.TextCore.Text
{
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	[NativeHeader("Modules/TextCoreTextEngine/Native/ATGMeshInfo.h")]
	internal struct ATGMeshInfo
	{
		public Span<NativeTextElementInfo> textElementInfos
		{
			get
			{
				bool flag = this.m_TextElementInfosPtr == IntPtr.Zero || this.m_TextElementCount == 0;
				Span<NativeTextElementInfo> span;
				if (flag)
				{
					span = default(Span<NativeTextElementInfo>);
				}
				else
				{
					span = new Span<NativeTextElementInfo>(this.m_TextElementInfosPtr.ToPointer(), this.m_TextElementCount);
				}
				return span;
			}
		}

		private IntPtr m_TextElementInfosPtr;

		private int m_TextElementCount;

		public int textAssetId;
	}
}
