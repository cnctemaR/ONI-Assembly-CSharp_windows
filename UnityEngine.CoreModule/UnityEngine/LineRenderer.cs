using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Runtime/Graphics/LineRenderer.h")]
	[NativeHeader("Runtime/Graphics/GraphicsScriptBindings.h")]
	public sealed class LineRenderer : Renderer
	{
		[Obsolete("Use startWidth, endWidth or widthCurve instead.", false)]
		public void SetWidth(float start, float end)
		{
			this.startWidth = start;
			this.endWidth = end;
		}

		[Obsolete("Use startColor, endColor or colorGradient instead.", false)]
		public void SetColors(Color start, Color end)
		{
			this.startColor = start;
			this.endColor = end;
		}

		[Obsolete("Use positionCount instead.", false)]
		public void SetVertexCount(int count)
		{
			this.positionCount = count;
		}

		[Obsolete("Use positionCount instead (UnityUpgradable) -> positionCount", false)]
		public int numPositions
		{
			get
			{
				return this.positionCount;
			}
			set
			{
				this.positionCount = value;
			}
		}

		public float startWidth
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LineRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return LineRenderer.get_startWidth_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LineRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LineRenderer.set_startWidth_Injected(intPtr, value);
			}
		}

		public float endWidth
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LineRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return LineRenderer.get_endWidth_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LineRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LineRenderer.set_endWidth_Injected(intPtr, value);
			}
		}

		public float widthMultiplier
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LineRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return LineRenderer.get_widthMultiplier_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LineRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LineRenderer.set_widthMultiplier_Injected(intPtr, value);
			}
		}

		public int numCornerVertices
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LineRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return LineRenderer.get_numCornerVertices_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LineRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LineRenderer.set_numCornerVertices_Injected(intPtr, value);
			}
		}

		public int numCapVertices
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LineRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return LineRenderer.get_numCapVertices_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LineRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LineRenderer.set_numCapVertices_Injected(intPtr, value);
			}
		}

		public bool useWorldSpace
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LineRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return LineRenderer.get_useWorldSpace_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LineRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LineRenderer.set_useWorldSpace_Injected(intPtr, value);
			}
		}

		public bool loop
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LineRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return LineRenderer.get_loop_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LineRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LineRenderer.set_loop_Injected(intPtr, value);
			}
		}

		public Color startColor
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LineRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Color color;
				LineRenderer.get_startColor_Injected(intPtr, out color);
				return color;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LineRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LineRenderer.set_startColor_Injected(intPtr, ref value);
			}
		}

		public Color endColor
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LineRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Color color;
				LineRenderer.get_endColor_Injected(intPtr, out color);
				return color;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LineRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LineRenderer.set_endColor_Injected(intPtr, ref value);
			}
		}

		[NativeProperty("PositionsCount")]
		public int positionCount
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LineRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return LineRenderer.get_positionCount_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LineRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LineRenderer.set_positionCount_Injected(intPtr, value);
			}
		}

		public void SetPosition(int index, Vector3 position)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LineRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			LineRenderer.SetPosition_Injected(intPtr, index, ref position);
		}

		public Vector3 GetPosition(int index)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LineRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Vector3 vector;
			LineRenderer.GetPosition_Injected(intPtr, index, out vector);
			return vector;
		}

		public Vector2 textureScale
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LineRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector2 vector;
				LineRenderer.get_textureScale_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LineRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LineRenderer.set_textureScale_Injected(intPtr, ref value);
			}
		}

		public float shadowBias
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LineRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return LineRenderer.get_shadowBias_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LineRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LineRenderer.set_shadowBias_Injected(intPtr, value);
			}
		}

		public bool generateLightingData
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LineRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return LineRenderer.get_generateLightingData_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LineRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LineRenderer.set_generateLightingData_Injected(intPtr, value);
			}
		}

		public bool applyActiveColorSpace
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LineRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return LineRenderer.get_applyActiveColorSpace_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LineRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LineRenderer.set_applyActiveColorSpace_Injected(intPtr, value);
			}
		}

		public LineTextureMode textureMode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LineRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return LineRenderer.get_textureMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LineRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LineRenderer.set_textureMode_Injected(intPtr, value);
			}
		}

		public LineAlignment alignment
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LineRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return LineRenderer.get_alignment_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LineRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LineRenderer.set_alignment_Injected(intPtr, value);
			}
		}

		public SpriteMaskInteraction maskInteraction
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LineRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return LineRenderer.get_maskInteraction_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LineRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LineRenderer.set_maskInteraction_Injected(intPtr, value);
			}
		}

		public void Simplify(float tolerance)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LineRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			LineRenderer.Simplify_Injected(intPtr, tolerance);
		}

		public void BakeMesh(Mesh mesh, bool useTransform = false)
		{
			this.BakeMesh(mesh, Camera.main, useTransform);
		}

		public void BakeMesh([NotNull] Mesh mesh, [NotNull] Camera camera, bool useTransform = false)
		{
			if (mesh == null)
			{
				ThrowHelper.ThrowArgumentNullException(mesh, "mesh");
			}
			if (camera == null)
			{
				ThrowHelper.ThrowArgumentNullException(camera, "camera");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LineRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(mesh);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(mesh, "mesh");
			}
			IntPtr intPtr3 = Object.MarshalledUnityObject.MarshalNotNull<Camera>(camera);
			if (intPtr3 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(camera, "camera");
			}
			LineRenderer.BakeMesh_Injected(intPtr, intPtr2, intPtr3, useTransform);
		}

		public AnimationCurve widthCurve
		{
			get
			{
				return this.GetWidthCurveCopy();
			}
			set
			{
				this.SetWidthCurve(value);
			}
		}

		public Gradient colorGradient
		{
			get
			{
				return this.GetColorGradientCopy();
			}
			set
			{
				this.SetColorGradient(value);
			}
		}

		private AnimationCurve GetWidthCurveCopy()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LineRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr widthCurveCopy_Injected = LineRenderer.GetWidthCurveCopy_Injected(intPtr);
			return (widthCurveCopy_Injected == 0) ? null : AnimationCurve.BindingsMarshaller.ConvertToManaged(widthCurveCopy_Injected);
		}

		private void SetWidthCurve([NotNull] AnimationCurve curve)
		{
			if (curve == null)
			{
				ThrowHelper.ThrowArgumentNullException(curve, "curve");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LineRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = AnimationCurve.BindingsMarshaller.ConvertToNative(curve);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(curve, "curve");
			}
			LineRenderer.SetWidthCurve_Injected(intPtr, intPtr2);
		}

		private Gradient GetColorGradientCopy()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LineRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr colorGradientCopy_Injected = LineRenderer.GetColorGradientCopy_Injected(intPtr);
			return (colorGradientCopy_Injected == 0) ? null : Gradient.BindingsMarshaller.ConvertToManaged(colorGradientCopy_Injected);
		}

		private void SetColorGradient([NotNull] Gradient curve)
		{
			if (curve == null)
			{
				ThrowHelper.ThrowArgumentNullException(curve, "curve");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LineRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Gradient.BindingsMarshaller.ConvertToNative(curve);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(curve, "curve");
			}
			LineRenderer.SetColorGradient_Injected(intPtr, intPtr2);
		}

		[FreeFunction(Name = "LineRendererScripting::GetPositions", HasExplicitThis = true)]
		public unsafe int GetPositions([NotNull] [Out] Vector3[] positions)
		{
			if (positions == null)
			{
				ThrowHelper.ThrowArgumentNullException(positions, "positions");
			}
			int positions_Injected;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LineRenderer>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				fixed (Vector3[] array = positions)
				{
					BlittableArrayWrapper blittableArrayWrapper;
					if (array.Length != 0)
					{
						blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
					}
					positions_Injected = LineRenderer.GetPositions_Injected(intPtr, out blittableArrayWrapper);
				}
			}
			finally
			{
				Vector3[] array;
				BlittableArrayWrapper blittableArrayWrapper;
				blittableArrayWrapper.Unmarshal<Vector3>(ref array);
			}
			return positions_Injected;
		}

		[FreeFunction(Name = "LineRendererScripting::SetPositions", HasExplicitThis = true)]
		public unsafe void SetPositions([NotNull] Vector3[] positions)
		{
			if (positions == null)
			{
				ThrowHelper.ThrowArgumentNullException(positions, "positions");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LineRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<Vector3> span = new Span<Vector3>(positions);
			fixed (Vector3* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				LineRenderer.SetPositions_Injected(intPtr, ref managedSpanWrapper);
			}
		}

		public void SetPositions(NativeArray<Vector3> positions)
		{
			this.SetPositionsWithNativeContainer((IntPtr)positions.GetUnsafeReadOnlyPtr<Vector3>(), positions.Length);
		}

		public void SetPositions(NativeSlice<Vector3> positions)
		{
			this.SetPositionsWithNativeContainer((IntPtr)positions.GetUnsafeReadOnlyPtr<Vector3>(), positions.Length);
		}

		public int GetPositions([Out] NativeArray<Vector3> positions)
		{
			return this.GetPositionsWithNativeContainer((IntPtr)positions.GetUnsafePtr<Vector3>(), positions.Length);
		}

		public int GetPositions([Out] NativeSlice<Vector3> positions)
		{
			return this.GetPositionsWithNativeContainer((IntPtr)positions.GetUnsafePtr<Vector3>(), positions.Length);
		}

		[FreeFunction(Name = "LineRendererScripting::SetPositionsWithNativeContainer", HasExplicitThis = true)]
		private void SetPositionsWithNativeContainer(IntPtr positions, int count)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LineRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			LineRenderer.SetPositionsWithNativeContainer_Injected(intPtr, positions, count);
		}

		[FreeFunction(Name = "LineRendererScripting::GetPositionsWithNativeContainer", HasExplicitThis = true)]
		private int GetPositionsWithNativeContainer(IntPtr positions, int length)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LineRenderer>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return LineRenderer.GetPositionsWithNativeContainer_Injected(intPtr, positions, length);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_startWidth_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_startWidth_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_endWidth_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_endWidth_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_widthMultiplier_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_widthMultiplier_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_numCornerVertices_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_numCornerVertices_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_numCapVertices_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_numCapVertices_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_useWorldSpace_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_useWorldSpace_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_loop_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_loop_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_startColor_Injected(IntPtr _unity_self, out Color ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_startColor_Injected(IntPtr _unity_self, [In] ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_endColor_Injected(IntPtr _unity_self, out Color ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_endColor_Injected(IntPtr _unity_self, [In] ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_positionCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_positionCount_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetPosition_Injected(IntPtr _unity_self, int index, [In] ref Vector3 position);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetPosition_Injected(IntPtr _unity_self, int index, out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_textureScale_Injected(IntPtr _unity_self, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_textureScale_Injected(IntPtr _unity_self, [In] ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_shadowBias_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_shadowBias_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_generateLightingData_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_generateLightingData_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_applyActiveColorSpace_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_applyActiveColorSpace_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern LineTextureMode get_textureMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_textureMode_Injected(IntPtr _unity_self, LineTextureMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern LineAlignment get_alignment_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_alignment_Injected(IntPtr _unity_self, LineAlignment value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern SpriteMaskInteraction get_maskInteraction_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_maskInteraction_Injected(IntPtr _unity_self, SpriteMaskInteraction value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Simplify_Injected(IntPtr _unity_self, float tolerance);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void BakeMesh_Injected(IntPtr _unity_self, IntPtr mesh, IntPtr camera, bool useTransform);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetWidthCurveCopy_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetWidthCurve_Injected(IntPtr _unity_self, IntPtr curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetColorGradientCopy_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetColorGradient_Injected(IntPtr _unity_self, IntPtr curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetPositions_Injected(IntPtr _unity_self, out BlittableArrayWrapper positions);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetPositions_Injected(IntPtr _unity_self, ref ManagedSpanWrapper positions);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetPositionsWithNativeContainer_Injected(IntPtr _unity_self, IntPtr positions, int count);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetPositionsWithNativeContainer_Injected(IntPtr _unity_self, IntPtr positions, int length);
	}
}
