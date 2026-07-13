using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Modules/UI/CanvasManager.h")]
	[NativeHeader("Modules/UI/UIStructs.h")]
	[RequireComponent(typeof(RectTransform))]
	[NativeHeader("Modules/UI/Canvas.h")]
	[NativeClass("UI::Canvas")]
	public sealed class Canvas : Behaviour
	{
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Canvas.WillRenderCanvases preWillRenderCanvases;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Canvas.WillRenderCanvases willRenderCanvases;

		public extern RenderMode renderMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool isRootCanvas
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public Rect pixelRect
		{
			get
			{
				Rect rect;
				this.get_pixelRect_Injected(out rect);
				return rect;
			}
		}

		public extern float scaleFactor
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float referencePixelsPerUnit
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool overridePixelPerfect
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool vertexColorAlwaysGammaSpace
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool pixelPerfect
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern float planeDistance
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int renderOrder
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool overrideSorting
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int sortingOrder
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int targetDisplay
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int sortingLayerID
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int cachedSortingLayerValue
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern AdditionalCanvasShaderChannels additionalShaderChannels
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern string sortingLayerName
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern Canvas rootCanvas
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public Vector2 renderingDisplaySize
		{
			get
			{
				Vector2 vector;
				this.get_renderingDisplaySize_Injected(out vector);
				return vector;
			}
		}

		public extern StandaloneRenderResize updateRectTransformForStandalone
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static Action<int> externBeginRenderOverlays { get; set; }

		internal static Action<int, int> externRenderOverlaysBefore { get; set; }

		internal static Action<int> externEndRenderOverlays { get; set; }

		[FreeFunction("UI::CanvasManager::SetExternalCanvasEnabled")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetExternalCanvasEnabled(bool enabled);

		[NativeProperty("Camera", false, TargetType.Function)]
		public extern Camera worldCamera
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[NativeProperty("SortingBucketNormalizedSize", false, TargetType.Function)]
		public extern float normalizedSortingGridSize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("Setting normalizedSize via a int is not supported. Please use normalizedSortingGridSize", false)]
		[NativeProperty("SortingBucketNormalizedSize", false, TargetType.Function)]
		public extern int sortingGridNormalizedSize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("Shared default material now used for text and general UI elements, call Canvas.GetDefaultCanvasMaterial()", false)]
		[FreeFunction("UI::GetDefaultUIMaterial")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Material GetDefaultCanvasTextMaterial();

		[FreeFunction("UI::GetDefaultUIMaterial")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Material GetDefaultCanvasMaterial();

		[FreeFunction("UI::GetETC1SupportedCanvasMaterial")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Material GetETC1SupportedCanvasMaterial();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void UpdateCanvasRectTransform(bool alignWithCamera);

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
		private extern void get_pixelRect_Injected(out Rect ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_renderingDisplaySize_Injected(out Vector2 ret);

		public delegate void WillRenderCanvases();
	}
}
