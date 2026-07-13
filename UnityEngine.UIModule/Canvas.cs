using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Modules/UI/UIStructs.h")]
	[NativeClass("UI::Canvas")]
	[RequireComponent(typeof(RectTransform))]
	[NativeHeader("Modules/UI/CanvasManager.h")]
	[NativeHeader("Modules/UI/Canvas.h")]
	public sealed class Canvas : Behaviour
	{
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Canvas.WillRenderCanvases preWillRenderCanvases;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Canvas.WillRenderCanvases willRenderCanvases;

		public RenderMode renderMode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Canvas>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Canvas.get_renderMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Canvas>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Canvas.set_renderMode_Injected(intPtr, value);
			}
		}

		public bool isRootCanvas
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Canvas>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Canvas.get_isRootCanvas_Injected(intPtr);
			}
		}

		public Rect pixelRect
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Canvas>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Rect rect;
				Canvas.get_pixelRect_Injected(intPtr, out rect);
				return rect;
			}
		}

		public float scaleFactor
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Canvas>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Canvas.get_scaleFactor_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Canvas>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Canvas.set_scaleFactor_Injected(intPtr, value);
			}
		}

		public float referencePixelsPerUnit
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Canvas>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Canvas.get_referencePixelsPerUnit_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Canvas>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Canvas.set_referencePixelsPerUnit_Injected(intPtr, value);
			}
		}

		public bool overridePixelPerfect
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Canvas>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Canvas.get_overridePixelPerfect_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Canvas>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Canvas.set_overridePixelPerfect_Injected(intPtr, value);
			}
		}

		public bool vertexColorAlwaysGammaSpace
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Canvas>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Canvas.get_vertexColorAlwaysGammaSpace_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Canvas>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Canvas.set_vertexColorAlwaysGammaSpace_Injected(intPtr, value);
			}
		}

		public bool pixelPerfect
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Canvas>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Canvas.get_pixelPerfect_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Canvas>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Canvas.set_pixelPerfect_Injected(intPtr, value);
			}
		}

		public float planeDistance
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Canvas>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Canvas.get_planeDistance_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Canvas>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Canvas.set_planeDistance_Injected(intPtr, value);
			}
		}

		public int renderOrder
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Canvas>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Canvas.get_renderOrder_Injected(intPtr);
			}
		}

		public bool overrideSorting
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Canvas>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Canvas.get_overrideSorting_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Canvas>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Canvas.set_overrideSorting_Injected(intPtr, value);
			}
		}

		public int sortingOrder
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Canvas>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Canvas.get_sortingOrder_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Canvas>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Canvas.set_sortingOrder_Injected(intPtr, value);
			}
		}

		public int targetDisplay
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Canvas>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Canvas.get_targetDisplay_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Canvas>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Canvas.set_targetDisplay_Injected(intPtr, value);
			}
		}

		public int sortingLayerID
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Canvas>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Canvas.get_sortingLayerID_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Canvas>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Canvas.set_sortingLayerID_Injected(intPtr, value);
			}
		}

		public int cachedSortingLayerValue
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Canvas>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Canvas.get_cachedSortingLayerValue_Injected(intPtr);
			}
		}

		public AdditionalCanvasShaderChannels additionalShaderChannels
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Canvas>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Canvas.get_additionalShaderChannels_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Canvas>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Canvas.set_additionalShaderChannels_Injected(intPtr, value);
			}
		}

		public unsafe string sortingLayerName
		{
			get
			{
				string stringAndDispose;
				try
				{
					IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Canvas>(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					ManagedSpanWrapper managedSpanWrapper;
					Canvas.get_sortingLayerName_Injected(intPtr, out managedSpanWrapper);
				}
				finally
				{
					ManagedSpanWrapper managedSpanWrapper;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
				}
				return stringAndDispose;
			}
			set
			{
				try
				{
					IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Canvas>(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					ManagedSpanWrapper managedSpanWrapper;
					if (!StringMarshaller.TryMarshalEmptyOrNullString(value, ref managedSpanWrapper))
					{
						ReadOnlySpan<char> readOnlySpan = value.AsSpan();
						fixed (char* ptr = readOnlySpan.GetPinnableReference())
						{
							managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
						}
					}
					Canvas.set_sortingLayerName_Injected(intPtr, ref managedSpanWrapper);
				}
				finally
				{
					char* ptr = null;
				}
			}
		}

		public Canvas rootCanvas
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Canvas>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Canvas>(Canvas.get_rootCanvas_Injected(intPtr));
			}
		}

		public Vector2 renderingDisplaySize
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Canvas>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector2 vector;
				Canvas.get_renderingDisplaySize_Injected(intPtr, out vector);
				return vector;
			}
		}

		public StandaloneRenderResize updateRectTransformForStandalone
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Canvas>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Canvas.get_updateRectTransformForStandalone_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Canvas>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Canvas.set_updateRectTransformForStandalone_Injected(intPtr, value);
			}
		}

		internal static Action<int> externBeginRenderOverlays
		{
			get; [VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
			set;
		}

		internal static Action<int, int> externRenderOverlaysBefore
		{
			get; [VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
			set;
		}

		internal static Action<int> externEndRenderOverlays
		{
			get; [VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
			set;
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		[FreeFunction("UI::CanvasManager::SetExternalCanvasEnabled")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetExternalCanvasEnabled(bool enabled);

		[NativeProperty("Camera", false, TargetType.Function)]
		public Camera worldCamera
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Canvas>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Camera>(Canvas.get_worldCamera_Injected(intPtr));
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Canvas>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Canvas.set_worldCamera_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Camera>(value));
			}
		}

		[NativeProperty("SortingBucketNormalizedSize", false, TargetType.Function)]
		public float normalizedSortingGridSize
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Canvas>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Canvas.get_normalizedSortingGridSize_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Canvas>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Canvas.set_normalizedSortingGridSize_Injected(intPtr, value);
			}
		}

		[NativeProperty("SortingBucketNormalizedSize", false, TargetType.Function)]
		[Obsolete("Setting normalizedSize via a int is not supported. Please use normalizedSortingGridSize", false)]
		public int sortingGridNormalizedSize
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Canvas>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Canvas.get_sortingGridNormalizedSize_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Canvas>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Canvas.set_sortingGridNormalizedSize_Injected(intPtr, value);
			}
		}

		[FreeFunction("UI::GetDefaultUIMaterial")]
		[Obsolete("Shared default material now used for text and general UI elements, call Canvas.GetDefaultCanvasMaterial()", false)]
		public static Material GetDefaultCanvasTextMaterial()
		{
			return Unmarshal.UnmarshalUnityObject<Material>(Canvas.GetDefaultCanvasTextMaterial_Injected());
		}

		[FreeFunction("UI::GetDefaultUIMaterial")]
		public static Material GetDefaultCanvasMaterial()
		{
			return Unmarshal.UnmarshalUnityObject<Material>(Canvas.GetDefaultCanvasMaterial_Injected());
		}

		[FreeFunction("UI::GetETC1SupportedCanvasMaterial")]
		public static Material GetETC1SupportedCanvasMaterial()
		{
			return Unmarshal.UnmarshalUnityObject<Material>(Canvas.GetETC1SupportedCanvasMaterial_Injected());
		}

		internal void UpdateCanvasRectTransform(bool alignWithCamera)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Canvas>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Canvas.UpdateCanvasRectTransform_Injected(intPtr, alignWithCamera);
		}

		public static void ForceUpdateCanvases()
		{
			Canvas.SendPreWillRenderCanvases();
			Canvas.SendWillRenderCanvases();
		}

		[RequiredByNativeCode]
		private static void SendPreWillRenderCanvases()
		{
			Canvas.WillRenderCanvases willRenderCanvases = Canvas.preWillRenderCanvases;
			if (willRenderCanvases != null)
			{
				willRenderCanvases();
			}
		}

		[RequiredByNativeCode]
		private static void SendWillRenderCanvases()
		{
			Canvas.WillRenderCanvases willRenderCanvases = Canvas.willRenderCanvases;
			if (willRenderCanvases != null)
			{
				willRenderCanvases();
			}
		}

		[RequiredByNativeCode]
		private static void BeginRenderExtraOverlays(int displayIndex)
		{
			Action<int> externBeginRenderOverlays = Canvas.externBeginRenderOverlays;
			if (externBeginRenderOverlays != null)
			{
				externBeginRenderOverlays(displayIndex);
			}
		}

		[RequiredByNativeCode]
		private static void RenderExtraOverlaysBefore(int displayIndex, int sortingOrder)
		{
			Action<int, int> externRenderOverlaysBefore = Canvas.externRenderOverlaysBefore;
			if (externRenderOverlaysBefore != null)
			{
				externRenderOverlaysBefore(displayIndex, sortingOrder);
			}
		}

		[RequiredByNativeCode]
		private static void EndRenderExtraOverlays(int displayIndex)
		{
			Action<int> externEndRenderOverlays = Canvas.externEndRenderOverlays;
			if (externEndRenderOverlays != null)
			{
				externEndRenderOverlays(displayIndex);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern RenderMode get_renderMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_renderMode_Injected(IntPtr _unity_self, RenderMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isRootCanvas_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_pixelRect_Injected(IntPtr _unity_self, out Rect ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_scaleFactor_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_scaleFactor_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_referencePixelsPerUnit_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_referencePixelsPerUnit_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_overridePixelPerfect_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_overridePixelPerfect_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_vertexColorAlwaysGammaSpace_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_vertexColorAlwaysGammaSpace_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_pixelPerfect_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_pixelPerfect_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_planeDistance_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_planeDistance_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_renderOrder_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_overrideSorting_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_overrideSorting_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_sortingOrder_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_sortingOrder_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_targetDisplay_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_targetDisplay_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_sortingLayerID_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_sortingLayerID_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_cachedSortingLayerValue_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AdditionalCanvasShaderChannels get_additionalShaderChannels_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_additionalShaderChannels_Injected(IntPtr _unity_self, AdditionalCanvasShaderChannels value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_sortingLayerName_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_sortingLayerName_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_rootCanvas_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_renderingDisplaySize_Injected(IntPtr _unity_self, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern StandaloneRenderResize get_updateRectTransformForStandalone_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_updateRectTransformForStandalone_Injected(IntPtr _unity_self, StandaloneRenderResize value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_worldCamera_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_worldCamera_Injected(IntPtr _unity_self, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_normalizedSortingGridSize_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_normalizedSortingGridSize_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_sortingGridNormalizedSize_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_sortingGridNormalizedSize_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetDefaultCanvasTextMaterial_Injected();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetDefaultCanvasMaterial_Injected();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetETC1SupportedCanvasMaterial_Injected();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UpdateCanvasRectTransform_Injected(IntPtr _unity_self, bool alignWithCamera);

		public delegate void WillRenderCanvases();
	}
}
