using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[NativeHeader("ModuleOverrides/com.unity.ui/Core/Native/Renderer/UIRMeshBuilder.bindings.h")]
	internal static class MeshBuilderNative
	{
		public static MeshWriteDataInterface MakeBorder(MeshBuilderNative.NativeBorderParams borderParams, float posZ)
		{
			MeshWriteDataInterface meshWriteDataInterface;
			MeshBuilderNative.MakeBorder_Injected(ref borderParams, posZ, out meshWriteDataInterface);
			return meshWriteDataInterface;
		}

		public static MeshWriteDataInterface MakeSolidRect(MeshBuilderNative.NativeRectParams rectParams, float posZ)
		{
			MeshWriteDataInterface meshWriteDataInterface;
			MeshBuilderNative.MakeSolidRect_Injected(ref rectParams, posZ, out meshWriteDataInterface);
			return meshWriteDataInterface;
		}

		public static MeshWriteDataInterface MakeTexturedRect(MeshBuilderNative.NativeRectParams rectParams, float posZ)
		{
			MeshWriteDataInterface meshWriteDataInterface;
			MeshBuilderNative.MakeTexturedRect_Injected(ref rectParams, posZ, out meshWriteDataInterface);
			return meshWriteDataInterface;
		}

		public static MeshWriteDataInterface MakeVectorGraphicsStretchBackground(Vertex[] svgVertices, ushort[] svgIndices, float svgWidth, float svgHeight, Rect targetRect, Rect sourceUV, ScaleMode scaleMode, Color tint, MeshBuilderNative.NativeColorPage colorPage, int settingIndexOffset, ref int finalVertexCount, ref int finalIndexCount)
		{
			MeshWriteDataInterface meshWriteDataInterface;
			MeshBuilderNative.MakeVectorGraphicsStretchBackground_Injected(svgVertices, svgIndices, svgWidth, svgHeight, ref targetRect, ref sourceUV, scaleMode, ref tint, ref colorPage, settingIndexOffset, ref finalVertexCount, ref finalIndexCount, out meshWriteDataInterface);
			return meshWriteDataInterface;
		}

		public static MeshWriteDataInterface MakeVectorGraphics9SliceBackground(Vertex[] svgVertices, ushort[] svgIndices, float svgWidth, float svgHeight, Rect targetRect, Vector4 sliceLTRB, Color tint, MeshBuilderNative.NativeColorPage colorPage, int settingIndexOffset)
		{
			MeshWriteDataInterface meshWriteDataInterface;
			MeshBuilderNative.MakeVectorGraphics9SliceBackground_Injected(svgVertices, svgIndices, svgWidth, svgHeight, ref targetRect, ref sliceLTRB, ref tint, ref colorPage, settingIndexOffset, out meshWriteDataInterface);
			return meshWriteDataInterface;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void MakeBorder_Injected(ref MeshBuilderNative.NativeBorderParams borderParams, float posZ, out MeshWriteDataInterface ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void MakeSolidRect_Injected(ref MeshBuilderNative.NativeRectParams rectParams, float posZ, out MeshWriteDataInterface ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void MakeTexturedRect_Injected(ref MeshBuilderNative.NativeRectParams rectParams, float posZ, out MeshWriteDataInterface ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void MakeVectorGraphicsStretchBackground_Injected(Vertex[] svgVertices, ushort[] svgIndices, float svgWidth, float svgHeight, ref Rect targetRect, ref Rect sourceUV, ScaleMode scaleMode, ref Color tint, ref MeshBuilderNative.NativeColorPage colorPage, int settingIndexOffset, ref int finalVertexCount, ref int finalIndexCount, out MeshWriteDataInterface ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void MakeVectorGraphics9SliceBackground_Injected(Vertex[] svgVertices, ushort[] svgIndices, float svgWidth, float svgHeight, ref Rect targetRect, ref Vector4 sliceLTRB, ref Color tint, ref MeshBuilderNative.NativeColorPage colorPage, int settingIndexOffset, out MeshWriteDataInterface ret);

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

			public Rect uvRegion;

			public Color color;

			public ScaleMode scaleMode;

			public Vector2 topLeftRadius;

			public Vector2 topRightRadius;

			public Vector2 bottomRightRadius;

			public Vector2 bottomLeftRadius;

			public Rect backgroundRepeatRect;

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
		}
	}
}
