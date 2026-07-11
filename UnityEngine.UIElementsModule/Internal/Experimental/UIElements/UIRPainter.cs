using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Experimental.UIElements;
using UnityEngine.UIR;

namespace UnityEngine.Internal.Experimental.UIElements
{
	internal class UIRPainter : IStylePainterInternal, IStylePainter
	{
		public void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.m_VertexGPUBuffer.Dispose();
				this.m_IndexGPUBuffer.Dispose();
				this.m_VertexData.Dispose();
				this.m_IndexData.Dispose();
				this.m_VertexUpdateRanges.Dispose();
				this.m_IndexUpdateRanges.Dispose();
				this.m_DrawRanges.Dispose();
			}
		}

		public void Draw()
		{
			Utility.DrawRanges<ushort, UIRPainter.Vertex>(this.m_IndexGPUBuffer, this.m_VertexGPUBuffer, this.m_DrawRanges.Slice<DrawBufferRange>(0, this.m_DrawRangeCount));
		}

		public float opacity
		{
			get
			{
				return 1f;
			}
			set
			{
			}
		}

		public void DrawRect(RectStylePainterParameters painterParams)
		{
			Rect rect = painterParams.rect;
			Color color = painterParams.color;
			Matrix4x4 worldTransform = this.currentElement.worldTransform;
			this.m_VertexData[this.m_VertexOffset] = new UIRPainter.Vertex
			{
				Position = worldTransform.MultiplyPoint(new Vector2(rect.x, rect.y)),
				Tint = color,
				UV = Vector2.zero,
				TransformID = 0f,
				Flags = 0f
			};
			this.m_VertexData[this.m_VertexOffset + 1] = new UIRPainter.Vertex
			{
				Position = worldTransform.MultiplyPoint(new Vector2(rect.x + rect.width, rect.y)),
				Tint = color,
				UV = Vector2.zero,
				TransformID = 0f,
				Flags = 0f
			};
			this.m_VertexData[this.m_VertexOffset + 2] = new UIRPainter.Vertex
			{
				Position = worldTransform.MultiplyPoint(new Vector2(rect.x, rect.y + rect.height)),
				Tint = color,
				UV = Vector2.zero,
				TransformID = 0f,
				Flags = 0f
			};
			this.m_VertexData[this.m_VertexOffset + 3] = new UIRPainter.Vertex
			{
				Position = worldTransform.MultiplyPoint(new Vector2(rect.x + rect.width, rect.y + rect.height)),
				Tint = color,
				UV = Vector2.zero,
				TransformID = 0f,
				Flags = 0f
			};
			int elementStride = this.m_VertexGPUBuffer.ElementStride;
			this.m_VertexUpdateRanges[this.m_VertexUpdateOffset] = new GfxUpdateBufferRange
			{
				source = new UIntPtr(this.m_VertexData.Slice<UIRPainter.Vertex>(this.m_VertexOffset, 4).GetUnsafeReadOnlyPtr<UIRPainter.Vertex>()),
				offsetFromWriteStart = 0U,
				size = (uint)(4 * elementStride)
			};
			this.m_VertexGPUBuffer.UpdateRanges(this.m_VertexUpdateRanges.Slice<GfxUpdateBufferRange>(this.m_VertexUpdateOffset, 1), this.m_VertexOffset * elementStride, (this.m_VertexOffset + 4) * elementStride);
			this.m_VertexUpdateOffset++;
			this.m_IndexData[this.m_IndexOffset] = (ushort)this.m_VertexOffset;
			this.m_IndexData[this.m_IndexOffset + 1] = (ushort)(this.m_VertexOffset + 1);
			this.m_IndexData[this.m_IndexOffset + 2] = (ushort)(this.m_VertexOffset + 2);
			this.m_IndexData[this.m_IndexOffset + 3] = (ushort)(this.m_VertexOffset + 2);
			this.m_IndexData[this.m_IndexOffset + 4] = (ushort)(this.m_VertexOffset + 1);
			this.m_IndexData[this.m_IndexOffset + 5] = (ushort)(this.m_VertexOffset + 3);
			int elementStride2 = this.m_IndexGPUBuffer.ElementStride;
			this.m_IndexUpdateRanges[this.m_IndexUpdateOffset] = new GfxUpdateBufferRange
			{
				source = new UIntPtr(this.m_IndexData.Slice<ushort>(this.m_IndexOffset, 6).GetUnsafeReadOnlyPtr<ushort>()),
				offsetFromWriteStart = 0U,
				size = (uint)(6 * elementStride2)
			};
			this.m_IndexGPUBuffer.UpdateRanges(this.m_IndexUpdateRanges.Slice<GfxUpdateBufferRange>(this.m_IndexUpdateOffset, 1), this.m_IndexOffset * elementStride2, (this.m_IndexOffset + 6) * elementStride2);
			this.m_IndexUpdateOffset++;
			DrawBufferRange drawBufferRange = this.m_DrawRanges[this.m_DrawRangeCount];
			drawBufferRange.firstIndex = this.m_IndexOffset;
			drawBufferRange.indexCount = 6;
			drawBufferRange.minIndexVal = this.m_VertexOffset;
			drawBufferRange.vertsReferenced = 4;
			this.m_DrawRanges[this.m_DrawRangeCount++] = drawBufferRange;
			this.m_VertexOffset += 4;
			this.m_IndexOffset += 6;
		}

		public void DrawMesh(MeshStylePainterParameters painterParameters)
		{
		}

		public void DrawText(TextStylePainterParameters painterParams)
		{
		}

		public void DrawTexture(TextureStylePainterParameters painterParams)
		{
		}

		public void DrawImmediate(Action callback)
		{
		}

		public void DrawBackground()
		{
			IStyle style = this.currentElement.style;
			if (style.backgroundColor != Color.clear)
			{
				RectStylePainterParameters @default = RectStylePainterParameters.GetDefault(this.currentElement);
				@default.border.SetWidth(0f);
				this.DrawRect(@default);
			}
		}

		public void DrawBorder()
		{
		}

		public void DrawText(string text)
		{
		}

		private const int kMaxVertices = 1024;

		private const int kMaxIndices = 4096;

		private const int kMaxRanges = 1024;

		private Utility.GPUBuffer<UIRPainter.Vertex> m_VertexGPUBuffer = new Utility.GPUBuffer<UIRPainter.Vertex>(1024, Utility.GPUBufferType.Vertex);

		private Utility.GPUBuffer<ushort> m_IndexGPUBuffer = new Utility.GPUBuffer<ushort>(4096, Utility.GPUBufferType.Index);

		private NativeArray<UIRPainter.Vertex> m_VertexData = new NativeArray<UIRPainter.Vertex>(1024, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

		private NativeArray<ushort> m_IndexData = new NativeArray<ushort>(4096, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

		private int m_VertexOffset = 0;

		private int m_IndexOffset = 0;

		private NativeArray<GfxUpdateBufferRange> m_VertexUpdateRanges = new NativeArray<GfxUpdateBufferRange>(1024, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

		private NativeArray<GfxUpdateBufferRange> m_IndexUpdateRanges = new NativeArray<GfxUpdateBufferRange>(1024, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

		private int m_VertexUpdateOffset = 0;

		private int m_IndexUpdateOffset = 0;

		private NativeArray<DrawBufferRange> m_DrawRanges = new NativeArray<DrawBufferRange>(1024, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

		private int m_DrawRangeCount = 0;

		internal VisualElement currentElement;

		private struct Vertex
		{
			public Vector3 Position;

			public Color32 Tint;

			public Vector2 UV;

			public float TransformID;

			public float Flags;
		}
	}
}
