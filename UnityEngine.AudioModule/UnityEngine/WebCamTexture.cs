using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[NativeHeader("AudioScriptingClasses.h")]
	[NativeHeader("Runtime/Video/ScriptBindings/WebCamTexture.bindings.h")]
	[NativeHeader("Runtime/Video/BaseWebCamTexture.h")]
	public sealed class WebCamTexture : Texture
	{
		public static extern WebCamDevice[] devices
		{
			[NativeName("Internal_GetDevices")]
			[StaticAccessor("WebCamTextureBindings", StaticAccessorType.DoubleColon)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
			get;
		}

		public WebCamTexture(string deviceName, int requestedWidth, int requestedHeight, int requestedFPS)
		{
			WebCamTexture.Internal_CreateWebCamTexture(this, deviceName, requestedWidth, requestedHeight, requestedFPS);
		}

		public WebCamTexture(string deviceName, int requestedWidth, int requestedHeight)
		{
			WebCamTexture.Internal_CreateWebCamTexture(this, deviceName, requestedWidth, requestedHeight, 0);
		}

		public WebCamTexture(string deviceName)
		{
			WebCamTexture.Internal_CreateWebCamTexture(this, deviceName, 0, 0, 0);
		}

		public WebCamTexture(int requestedWidth, int requestedHeight, int requestedFPS)
		{
			WebCamTexture.Internal_CreateWebCamTexture(this, "", requestedWidth, requestedHeight, requestedFPS);
		}

		public WebCamTexture(int requestedWidth, int requestedHeight)
		{
			WebCamTexture.Internal_CreateWebCamTexture(this, "", requestedWidth, requestedHeight, 0);
		}

		public WebCamTexture()
		{
			WebCamTexture.Internal_CreateWebCamTexture(this, "", 0, 0, 0);
		}

		public void Play()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WebCamTexture>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			WebCamTexture.Play_Injected(intPtr);
		}

		public void Pause()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WebCamTexture>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			WebCamTexture.Pause_Injected(intPtr);
		}

		public void Stop()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WebCamTexture>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			WebCamTexture.Stop_Injected(intPtr);
		}

		public bool isPlaying
		{
			[NativeName("IsPlaying")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WebCamTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return WebCamTexture.get_isPlaying_Injected(intPtr);
			}
		}

		[NativeName("Device")]
		public unsafe string deviceName
		{
			get
			{
				string stringAndDispose;
				try
				{
					IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WebCamTexture>(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					ManagedSpanWrapper managedSpanWrapper;
					WebCamTexture.get_deviceName_Injected(intPtr, out managedSpanWrapper);
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
					IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WebCamTexture>(this);
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
					WebCamTexture.set_deviceName_Injected(intPtr, ref managedSpanWrapper);
				}
				finally
				{
					char* ptr = null;
				}
			}
		}

		public float requestedFPS
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WebCamTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return WebCamTexture.get_requestedFPS_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WebCamTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				WebCamTexture.set_requestedFPS_Injected(intPtr, value);
			}
		}

		public int requestedWidth
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WebCamTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return WebCamTexture.get_requestedWidth_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WebCamTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				WebCamTexture.set_requestedWidth_Injected(intPtr, value);
			}
		}

		public int requestedHeight
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WebCamTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return WebCamTexture.get_requestedHeight_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WebCamTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				WebCamTexture.set_requestedHeight_Injected(intPtr, value);
			}
		}

		public int videoRotationAngle
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WebCamTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return WebCamTexture.get_videoRotationAngle_Injected(intPtr);
			}
		}

		public bool videoVerticallyMirrored
		{
			[NativeName("IsVideoVerticallyMirrored")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WebCamTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return WebCamTexture.get_videoVerticallyMirrored_Injected(intPtr);
			}
		}

		public bool didUpdateThisFrame
		{
			[NativeName("DidUpdateThisFrame")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WebCamTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return WebCamTexture.get_didUpdateThisFrame_Injected(intPtr);
			}
		}

		[FreeFunction("WebCamTextureBindings::Internal_GetPixel", HasExplicitThis = true)]
		public Color GetPixel(int x, int y)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WebCamTexture>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Color color;
			WebCamTexture.GetPixel_Injected(intPtr, x, y, out color);
			return color;
		}

		public Color[] GetPixels()
		{
			return this.GetPixels(0, 0, this.width, this.height);
		}

		[FreeFunction("WebCamTextureBindings::Internal_GetPixels", HasExplicitThis = true, ThrowsException = true)]
		[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
		public Color[] GetPixels(int x, int y, int blockWidth, int blockHeight)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WebCamTexture>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return WebCamTexture.GetPixels_Injected(intPtr, x, y, blockWidth, blockHeight);
		}

		[ExcludeFromDocs]
		public Color32[] GetPixels32()
		{
			return this.GetPixels32(null);
		}

		[FreeFunction("WebCamTextureBindings::Internal_GetPixels32", HasExplicitThis = true, ThrowsException = true)]
		[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
		public Color32[] GetPixels32([UnityMarshalAs(NativeType.ScriptingObjectPtr)] [DefaultValue("null")] Color32[] colors)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WebCamTexture>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return WebCamTexture.GetPixels32_Injected(intPtr, colors);
		}

		public Vector2? autoFocusPoint
		{
			get
			{
				return (this.internalAutoFocusPoint.x < 0f) ? null : new Vector2?(this.internalAutoFocusPoint);
			}
			set
			{
				this.internalAutoFocusPoint = ((value == null) ? new Vector2(-1f, -1f) : value.Value);
			}
		}

		internal Vector2 internalAutoFocusPoint
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WebCamTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Vector2 vector;
				WebCamTexture.get_internalAutoFocusPoint_Injected(intPtr, out vector);
				return vector;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WebCamTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				WebCamTexture.set_internalAutoFocusPoint_Injected(intPtr, ref value);
			}
		}

		public bool isDepth
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<WebCamTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return WebCamTexture.get_isDepth_Injected(intPtr);
			}
		}

		[StaticAccessor("WebCamTextureBindings", StaticAccessorType.DoubleColon)]
		private unsafe static void Internal_CreateWebCamTexture([Writable] WebCamTexture self, string scriptingDevice, int requestedWidth, int requestedHeight, int maxFramerate)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(scriptingDevice, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = scriptingDevice.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				WebCamTexture.Internal_CreateWebCamTexture_Injected(self, ref managedSpanWrapper, requestedWidth, requestedHeight, maxFramerate);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Play_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Pause_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Stop_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isPlaying_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_deviceName_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_deviceName_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_requestedFPS_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_requestedFPS_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_requestedWidth_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_requestedWidth_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_requestedHeight_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_requestedHeight_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_videoRotationAngle_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_videoVerticallyMirrored_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_didUpdateThisFrame_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetPixel_Injected(IntPtr _unity_self, int x, int y, out Color ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Color[] GetPixels_Injected(IntPtr _unity_self, int x, int y, int blockWidth, int blockHeight);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Color32[] GetPixels32_Injected(IntPtr _unity_self, [DefaultValue("null")] Color32[] colors);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_internalAutoFocusPoint_Injected(IntPtr _unity_self, out Vector2 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_internalAutoFocusPoint_Injected(IntPtr _unity_self, [In] ref Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isDepth_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_CreateWebCamTexture_Injected([Writable] WebCamTexture self, ref ManagedSpanWrapper scriptingDevice, int requestedWidth, int requestedHeight, int maxFramerate);
	}
}
