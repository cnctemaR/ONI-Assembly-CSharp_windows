using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.U2D
{
	[NativeHeader("Modules/SpriteShape/Public/SpriteShapeUtility.h")]
	[MovedFrom("UnityEngine.Experimental.U2D")]
	public class SpriteShapeUtility
	{
		[FreeFunction("SpriteShapeUtility::Generate")]
		[NativeThrows]
		public unsafe static int[] Generate(Mesh mesh, SpriteShapeParameters shapeParams, ShapeControlPoint[] points, SpriteShapeMetaData[] metaData, AngleRangeInfo[] angleRange, Sprite[] sprites, Sprite[] corners)
		{
			int[] array2;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.Marshal<Mesh>(mesh);
				Span<ShapeControlPoint> span = new Span<ShapeControlPoint>(points);
				fixed (ShapeControlPoint* ptr = span.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, span.Length);
					Span<SpriteShapeMetaData> span2 = new Span<SpriteShapeMetaData>(metaData);
					fixed (SpriteShapeMetaData* ptr2 = span2.GetPinnableReference())
					{
						ManagedSpanWrapper managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, span2.Length);
						BlittableArrayWrapper blittableArrayWrapper;
						SpriteShapeUtility.Generate_Injected(intPtr, ref shapeParams, ref managedSpanWrapper, ref managedSpanWrapper2, angleRange, sprites, corners, out blittableArrayWrapper);
					}
				}
			}
			finally
			{
				ShapeControlPoint* ptr = null;
				SpriteShapeMetaData* ptr2 = null;
				BlittableArrayWrapper blittableArrayWrapper;
				int[] array;
				blittableArrayWrapper.Unmarshal<int>(ref array);
				array2 = array;
			}
			return array2;
		}

		[FreeFunction("SpriteShapeUtility::GenerateSpriteShape")]
		[NativeThrows]
		public unsafe static void GenerateSpriteShape(SpriteShapeRenderer renderer, SpriteShapeParameters shapeParams, ShapeControlPoint[] points, SpriteShapeMetaData[] metaData, AngleRangeInfo[] angleRange, Sprite[] sprites, Sprite[] corners)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.Marshal<SpriteShapeRenderer>(renderer);
			Span<ShapeControlPoint> span = new Span<ShapeControlPoint>(points);
			fixed (ShapeControlPoint* ptr = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, span.Length);
				Span<SpriteShapeMetaData> span2 = new Span<SpriteShapeMetaData>(metaData);
				fixed (SpriteShapeMetaData* pinnableReference = span2.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper2 = new ManagedSpanWrapper((void*)pinnableReference, span2.Length);
					SpriteShapeUtility.GenerateSpriteShape_Injected(intPtr, ref shapeParams, ref managedSpanWrapper, ref managedSpanWrapper2, angleRange, sprites, corners);
					ptr = null;
				}
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Generate_Injected(IntPtr mesh, [In] ref SpriteShapeParameters shapeParams, ref ManagedSpanWrapper points, ref ManagedSpanWrapper metaData, AngleRangeInfo[] angleRange, Sprite[] sprites, Sprite[] corners, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GenerateSpriteShape_Injected(IntPtr renderer, [In] ref SpriteShapeParameters shapeParams, ref ManagedSpanWrapper points, ref ManagedSpanWrapper metaData, AngleRangeInfo[] angleRange, Sprite[] sprites, Sprite[] corners);
	}
}
