using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[NativeHeader("Modules/UIElements/Core/Native/Renderer/UIPainter2D.bindings.h")]
	internal static class UIPainter2D
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr Create(bool computeBBox = false);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Destroy(IntPtr handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Reset(IntPtr handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float GetLineWidth(IntPtr handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetLineWidth(IntPtr handle, float value);

		public static Color GetStrokeColor(IntPtr handle)
		{
			Color color;
			UIPainter2D.GetStrokeColor_Injected(handle, out color);
			return color;
		}

		public static void SetStrokeColor(IntPtr handle, Color value)
		{
			UIPainter2D.SetStrokeColor_Injected(handle, ref value);
		}

		[NativeName("GetStrokeGradientCopy")]
		public static Gradient GetStrokeGradient(IntPtr handle)
		{
			IntPtr strokeGradient_Injected = UIPainter2D.GetStrokeGradient_Injected(handle);
			return (strokeGradient_Injected == 0) ? null : Gradient.BindingsMarshaller.ConvertToManaged(strokeGradient_Injected);
		}

		public static void SetStrokeGradient(IntPtr handle, Gradient gradient)
		{
			UIPainter2D.SetStrokeGradient_Injected(handle, (gradient == null) ? ((IntPtr)0) : Gradient.BindingsMarshaller.ConvertToNative(gradient));
		}

		public static FillGradient GetFillGradient(IntPtr handle)
		{
			FillGradient fillGradient;
			UIPainter2D.GetFillGradient_Injected(handle, out fillGradient);
			return fillGradient;
		}

		public static void SetFillGradient(IntPtr handle, FillGradient gradient)
		{
			UIPainter2D.SetFillGradient_Injected(handle, ref gradient);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool HasFillGradient(IntPtr handle);

		public static void SetStrokeFillGradient(IntPtr handle, FillGradient gradient)
		{
			UIPainter2D.SetStrokeFillGradient_Injected(handle, ref gradient);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool HasStrokeFillGradient(IntPtr handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetHasFillTexture(IntPtr handle, bool hasFillTexture);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool HasFillTexture(IntPtr handle);

		internal static void SetFillTransform(IntPtr handle, Matrix4x4 fillTransform)
		{
			UIPainter2D.SetFillTransform_Injected(handle, ref fillTransform);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetOpacity(IntPtr handle, float opacity);

		public static Color GetFillColor(IntPtr handle)
		{
			Color color;
			UIPainter2D.GetFillColor_Injected(handle, out color);
			return color;
		}

		public static void SetFillColor(IntPtr handle, Color value)
		{
			UIPainter2D.SetFillColor_Injected(handle, ref value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern LineJoin GetLineJoin(IntPtr handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetLineJoin(IntPtr handle, LineJoin value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern LineCap GetLineCap(IntPtr handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetLineCap(IntPtr handle, LineCap value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float GetMiterLimit(IntPtr handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetMiterLimit(IntPtr handle, float value);

		public unsafe static void SetDashPattern(IntPtr handle, ReadOnlySpan<float> value)
		{
			ReadOnlySpan<float> readOnlySpan = value;
			fixed (float* pinnableReference = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, readOnlySpan.Length);
				UIPainter2D.SetDashPattern_Injected(handle, ref managedSpanWrapper);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetDashGapPattern(IntPtr handle, float dash, float gap);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float GetDashOffset(IntPtr handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetDashOffset(IntPtr handle, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void BeginPath(IntPtr handle);

		public static void MoveTo(IntPtr handle, Vector2 pos)
		{
			UIPainter2D.MoveTo_Injected(handle, ref pos);
		}

		public static void LineTo(IntPtr handle, Vector2 pos)
		{
			UIPainter2D.LineTo_Injected(handle, ref pos);
		}

		public static void ArcTo(IntPtr handle, Vector2 p1, Vector2 p2, float radius)
		{
			UIPainter2D.ArcTo_Injected(handle, ref p1, ref p2, radius);
		}

		public static void Arc(IntPtr handle, Vector2 center, float radius, float startAngleRads, float endAngleRads, ArcDirection direction)
		{
			UIPainter2D.Arc_Injected(handle, ref center, radius, startAngleRads, endAngleRads, direction);
		}

		public static void BezierCurveTo(IntPtr handle, Vector2 p1, Vector2 p2, Vector2 p3)
		{
			UIPainter2D.BezierCurveTo_Injected(handle, ref p1, ref p2, ref p3);
		}

		public static void QuadraticCurveTo(IntPtr handle, Vector2 p1, Vector2 p2)
		{
			UIPainter2D.QuadraticCurveTo_Injected(handle, ref p1, ref p2);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ClosePath(IntPtr handle);

		public static Rect GetBBox(IntPtr handle)
		{
			Rect rect;
			UIPainter2D.GetBBox_Injected(handle, out rect);
			return rect;
		}

		public static MeshWriteDataInterface Stroke(IntPtr handle, bool isDetached)
		{
			MeshWriteDataInterface meshWriteDataInterface;
			UIPainter2D.Stroke_Injected(handle, isDetached, out meshWriteDataInterface);
			return meshWriteDataInterface;
		}

		public static MeshWriteDataInterface Fill(IntPtr handle, FillRule fillRule)
		{
			MeshWriteDataInterface meshWriteDataInterface;
			UIPainter2D.Fill_Injected(handle, fillRule, out meshWriteDataInterface);
			return meshWriteDataInterface;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int TakeStrokeSnapshot(IntPtr handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int TakeFillSnapshot(IntPtr handle, FillRule fillRule);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ClearSnapshots(IntPtr handle);

		[ThreadSafe]
		public static MeshWriteDataInterface ExecuteSnapshotFromJob(IntPtr painterHandle, int i)
		{
			MeshWriteDataInterface meshWriteDataInterface;
			UIPainter2D.ExecuteSnapshotFromJob_Injected(painterHandle, i, out meshWriteDataInterface);
			return meshWriteDataInterface;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetStrokeColor_Injected(IntPtr handle, out Color ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetStrokeColor_Injected(IntPtr handle, [In] ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetStrokeGradient_Injected(IntPtr handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetStrokeGradient_Injected(IntPtr handle, IntPtr gradient);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetFillGradient_Injected(IntPtr handle, out FillGradient ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetFillGradient_Injected(IntPtr handle, [In] ref FillGradient gradient);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetStrokeFillGradient_Injected(IntPtr handle, [In] ref FillGradient gradient);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetFillTransform_Injected(IntPtr handle, [In] ref Matrix4x4 fillTransform);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetFillColor_Injected(IntPtr handle, out Color ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetFillColor_Injected(IntPtr handle, [In] ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetDashPattern_Injected(IntPtr handle, ref ManagedSpanWrapper value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void MoveTo_Injected(IntPtr handle, [In] ref Vector2 pos);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void LineTo_Injected(IntPtr handle, [In] ref Vector2 pos);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ArcTo_Injected(IntPtr handle, [In] ref Vector2 p1, [In] ref Vector2 p2, float radius);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Arc_Injected(IntPtr handle, [In] ref Vector2 center, float radius, float startAngleRads, float endAngleRads, ArcDirection direction);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void BezierCurveTo_Injected(IntPtr handle, [In] ref Vector2 p1, [In] ref Vector2 p2, [In] ref Vector2 p3);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void QuadraticCurveTo_Injected(IntPtr handle, [In] ref Vector2 p1, [In] ref Vector2 p2);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetBBox_Injected(IntPtr handle, out Rect ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Stroke_Injected(IntPtr handle, bool isDetached, out MeshWriteDataInterface ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Fill_Injected(IntPtr handle, FillRule fillRule, out MeshWriteDataInterface ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ExecuteSnapshotFromJob_Injected(IntPtr painterHandle, int i, out MeshWriteDataInterface ret);
	}
}
