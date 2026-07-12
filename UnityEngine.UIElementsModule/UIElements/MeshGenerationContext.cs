using System;
using Unity.Profiling;
using UnityEngine.TextCore.Text;

namespace UnityEngine.UIElements
{
	public class MeshGenerationContext
	{
		public VisualElement visualElement
		{
			get
			{
				return this.painter.visualElement;
			}
		}

		public Painter2D painter2D
		{
			get
			{
				bool flag = this.m_Painter2D == null;
				if (flag)
				{
					this.m_Painter2D = new Painter2D(this);
				}
				return this.m_Painter2D;
			}
		}

		internal bool hasPainter2D
		{
			get
			{
				return this.m_Painter2D != null;
			}
		}

		internal MeshGenerationContext(IStylePainter painter)
		{
			this.painter = painter;
		}

		public MeshWriteData Allocate(int vertexCount, int indexCount, Texture texture = null)
		{
			MeshWriteData meshWriteData;
			using (MeshGenerationContext.s_AllocateMarker.Auto())
			{
				meshWriteData = this.painter.DrawMesh(vertexCount, indexCount, texture, null, MeshGenerationContext.MeshFlags.None);
			}
			return meshWriteData;
		}

		internal MeshWriteData Allocate(int vertexCount, int indexCount, Texture texture, Material material, MeshGenerationContext.MeshFlags flags)
		{
			MeshWriteData meshWriteData;
			using (MeshGenerationContext.s_AllocateMarker.Auto())
			{
				meshWriteData = this.painter.DrawMesh(vertexCount, indexCount, texture, material, flags);
			}
			return meshWriteData;
		}

		public void DrawVectorImage(VectorImage vectorImage, Vector2 offset, Angle rotationAngle, Vector2 scale)
		{
			using (MeshGenerationContext.s_DrawVectorImageMarker.Auto())
			{
				this.painter.DrawVectorImage(vectorImage, offset, rotationAngle, scale);
			}
		}

		public void DrawText(string text, Vector2 pos, float fontSize, Color color, FontAsset font = null)
		{
			bool flag = font == null;
			if (flag)
			{
				font = TextUtilities.GetFontAsset(this.visualElement);
			}
			this.painter.DrawText(text, pos, fontSize, color, font);
		}

		private Painter2D m_Painter2D;

		private static readonly ProfilerMarker s_AllocateMarker = new ProfilerMarker("UIR.MeshGenerationContext.Allocate");

		private static readonly ProfilerMarker s_DrawVectorImageMarker = new ProfilerMarker("UIR.MeshGenerationContext.DrawVectorImage");

		internal IStylePainter painter;

		[Flags]
		internal enum MeshFlags
		{
			None = 0,
			UVisDisplacement = 1,
			SkipDynamicAtlas = 2
		}
	}
}
