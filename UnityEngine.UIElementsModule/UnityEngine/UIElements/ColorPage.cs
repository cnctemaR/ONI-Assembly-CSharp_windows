using System;
using UnityEngine.UIElements.UIR;

namespace UnityEngine.UIElements
{
	internal struct ColorPage
	{
		public static ColorPage Init(RenderTreeManager renderTreeManager, BMPAlloc alloc)
		{
			bool flag = alloc.IsValid();
			return new ColorPage
			{
				isValid = flag,
				pageAndID = (flag ? renderTreeManager.shaderInfoAllocator.ColorAllocToVertexData(alloc) : default(Color32))
			};
		}

		public MeshBuilderNative.NativeColorPage ToNativeColorPage()
		{
			return new MeshBuilderNative.NativeColorPage
			{
				isValid = (this.isValid ? 1 : 0),
				pageAndID = this.pageAndID
			};
		}

		public bool isValid;

		public Color32 pageAndID;
	}
}
