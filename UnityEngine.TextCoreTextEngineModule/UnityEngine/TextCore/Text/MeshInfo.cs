using System;
using UnityEngine.Bindings;
using UnityEngine.TextCore.LowLevel;

namespace UnityEngine.TextCore.Text
{
	[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule", "UnityEngine.UIElementsModule" })]
	internal struct MeshInfo
	{
		public MeshInfo(int size, bool isIMGUI)
		{
			this = default(MeshInfo);
			this.applySDF = true;
			this.material = null;
			if (isIMGUI)
			{
				size = Mathf.Min(size, 16383);
			}
			int num = size * 4;
			int num2 = size * 6;
			this.vertexCount = 0;
			this.vertexBufferSize = num;
			this.vertexData = new TextCoreVertex[num];
			this.material = null;
			this.glyphRenderMode = GlyphRenderMode.DEFAULT;
		}

		internal void ResizeMeshInfo(int size, bool isIMGUI)
		{
			if (isIMGUI)
			{
				size = Mathf.Min(size, 16383);
			}
			int num = size * 4;
			int num2 = size * 6;
			this.vertexBufferSize = num;
			Array.Resize<TextCoreVertex>(ref this.vertexData, num);
		}

		internal void Clear(bool uploadChanges)
		{
			bool flag = this.vertexData == null;
			if (!flag)
			{
				Array.Clear(this.vertexData, 0, this.vertexData.Length);
				this.vertexBufferSize = this.vertexData.Length;
				this.vertexCount = 0;
			}
		}

		internal void ClearUnusedVertices()
		{
			int num = this.vertexData.Length - this.vertexCount;
			bool flag = num > 0;
			if (flag)
			{
				Array.Clear(this.vertexData, this.vertexCount, num);
			}
			this.vertexBufferSize = this.vertexData.Length;
		}

		public int vertexCount;

		public TextCoreVertex[] vertexData;

		public Material material;

		[Ignore]
		public int vertexBufferSize;

		[Ignore]
		public bool applySDF;

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal GlyphRenderMode glyphRenderMode;
	}
}
