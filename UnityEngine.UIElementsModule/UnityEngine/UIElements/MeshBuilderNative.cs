using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[NativeHeader("Modules/UIElements/Core/Native/Renderer/UIRMeshBuilder.bindings.h")]
	internal static class MeshBuilderNative
	{
		[ThreadSafe]
		public static MeshWriteDataInterface MakeBorder(ref MeshBuilderNative.NativeBorderParams borderParams)
		{
			MeshWriteDataInterface meshWriteDataInterface;
			MeshBuilderNative.MakeBorder_Injected(ref borderParams, out meshWriteDataInterface);
			return meshWriteDataInterface;
		}

		[ThreadSafe]
		public static MeshWriteDataInterface MakeSolidRect(ref MeshBuilderNative.NativeRectParams rectParams)
		{
			MeshWriteDataInterface meshWriteDataInterface;
			MeshBuilderNative.MakeSolidRect_Injected(ref rectParams, out meshWriteDataInterface);
			return meshWriteDataInterface;
		}

		[ThreadSafe]
		public static MeshWriteDataInterface MakeTexturedRect(ref MeshBuilderNative.NativeRectParams rectParams)
		{
			MeshWriteDataInterface meshWriteDataInterface;
			MeshBuilderNative.MakeTexturedRect_Injected(ref rectParams, out meshWriteDataInterface);
			return meshWriteDataInterface;
		}

		[ThreadSafe]
		public unsafe static MeshWriteDataInterface MakeVectorGraphicsStretchBackground(Vertex[] svgVertices, ushort[] svgIndices, float svgWidth, float svgHeight, Rect targetRect, Rect sourceUV, ScaleMode scaleMode, Color tint, MeshBuilderNative.NativeColorPage colorPage)
		{
			Span<Vertex> span = new Span<Vertex>(svgVertices);
			fixed (Vertex* ptr = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, span.Length);
				Span<ushort> span2 = new Span<ushort>(svgIndices);
				MeshWriteDataInterface meshWriteDataInterface;
				fixed (ushort* pinnableReference = span2.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper2 = new ManagedSpanWrapper((void*)pinnableReference, span2.Length);
					MeshBuilderNative.MakeVectorGraphicsStretchBackground_Injected(ref managedSpanWrapper, ref managedSpanWrapper2, svgWidth, svgHeight, ref targetRect, ref sourceUV, scaleMode, ref tint, ref colorPage, out meshWriteDataInterface);
					ptr = null;
				}
				return meshWriteDataInterface;
			}
		}

		[ThreadSafe]
		public unsafe static MeshWriteDataInterface MakeVectorGraphics9SliceBackground(Vertex[] svgVertices, ushort[] svgIndices, float svgWidth, float svgHeight, Rect targetRect, Vector4 sliceLTRB, Color tint, MeshBuilderNative.NativeColorPage colorPage)
		{
			Span<Vertex> span = new Span<Vertex>(svgVertices);
			fixed (Vertex* ptr = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, span.Length);
				Span<ushort> span2 = new Span<ushort>(svgIndices);
				MeshWriteDataInterface meshWriteDataInterface;
				fixed (ushort* pinnableReference = span2.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper2 = new ManagedSpanWrapper((void*)pinnableReference, span2.Length);
					MeshBuilderNative.MakeVectorGraphics9SliceBackground_Injected(ref managedSpanWrapper, ref managedSpanWrapper2, svgWidth, svgHeight, ref targetRect, ref sliceLTRB, ref tint, ref colorPage, out meshWriteDataInterface);
					ptr = null;
				}
				return meshWriteDataInterface;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void MakeBorder_Injected(ref MeshBuilderNative.NativeBorderParams borderParams, out MeshWriteDataInterface ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void MakeSolidRect_Injected(ref MeshBuilderNative.NativeRectParams rectParams, out MeshWriteDataInterface ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void MakeTexturedRect_Injected(ref MeshBuilderNative.NativeRectParams rectParams, out MeshWriteDataInterface ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void MakeVectorGraphicsStretchBackground_Injected(ref ManagedSpanWrapper svgVertices, ref ManagedSpanWrapper svgIndices, float svgWidth, float svgHeight, [In] ref Rect targetRect, [In] ref Rect sourceUV, ScaleMode scaleMode, [In] ref Color tint, [In] ref MeshBuilderNative.NativeColorPage colorPage, out MeshWriteDataInterface ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void MakeVectorGraphics9SliceBackground_Injected(ref ManagedSpanWrapper svgVertices, ref ManagedSpanWrapper svgIndices, float svgWidth, float svgHeight, [In] ref Rect targetRect, [In] ref Vector4 sliceLTRB, [In] ref Color tint, [In] ref MeshBuilderNative.NativeColorPage colorPage, out MeshWriteDataInterface ret);

		public const float kEpsilon = 0.001f;

		public struct NativeColorPage
		{
			public int isValid;

			public Color32 pageAndID;
		}

		public struct NativeBorderParams
		{
			public Rect rect;

			public Color leftColor;

			public Color topColor;

			public Color rightColor;

			public Color bottomColor;

			public float leftWidth;

			public float topWidth;

			public float rightWidth;

			public float bottomWidth;

			public Vector2 topLeftRadius;

			public Vector2 topRightRadius;

			public Vector2 bottomRightRadius;

			public Vector2 bottomLeftRadius;

			internal MeshBuilderNative.NativeColorPage leftColorPage;

			internal MeshBuilderNative.NativeColorPage topColorPage;

			internal MeshBuilderNative.NativeColorPage rightColorPage;

			internal MeshBuilderNative.NativeColorPage bottomColorPage;
		}

		public struct NativeRectParams
		{
			public Rect rect;

			public Rect subRect;

			public Rect uv;

			public Color color;

			public ScaleMode scaleMode;

			public IntPtr backgroundRepeatInstanceList;

			public int backgroundRepeatInstanceListStartIndex;

			public int backgroundRepeatInstanceListEndIndex;

			public Vector2 topLeftRadius;

			public Vector2 topRightRadius;

			public Vector2 bottomRightRadius;

			public Vector2 bottomLeftRadius;

			public Rect backgroundRepeatRect;

			public IntPtr texture;

			public IntPtr sprite;

			public IntPtr vectorImage;

			public IntPtr spriteTexture;

			public IntPtr spriteVertices;

			public IntPtr spriteUVs;

			public IntPtr spriteTriangles;

			public Rect spriteGeomRect;

			public Vector2 contentSize;

			public Vector2 textureSize;

			public float texturePixelsPerPoint;

			public int leftSlice;

			public int topSlice;

			public int rightSlice;

			public int bottomSlice;

			public float sliceScale;

			public Vector4 rectInset;

			public MeshBuilderNative.NativeColorPage colorPage;

			public int meshFlags;
		}
	}
}
