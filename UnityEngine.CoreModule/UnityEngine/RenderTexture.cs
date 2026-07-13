using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Internal;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Runtime/Graphics/RenderTexture.h")]
	[NativeHeader("Runtime/Graphics/RenderBufferManager.h")]
	[NativeHeader("Runtime/Graphics/GraphicsScriptBindings.h")]
	[UsedByNativeCode]
	[NativeHeader("Runtime/Camera/Camera.h")]
	public class RenderTexture : Texture
	{
		public override int width
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return RenderTexture.get_width_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				RenderTexture.set_width_Injected(intPtr, value);
			}
		}

		public override int height
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return RenderTexture.get_height_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				RenderTexture.set_height_Injected(intPtr, value);
			}
		}

		public override TextureDimension dimension
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return RenderTexture.get_dimension_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				RenderTexture.set_dimension_Injected(intPtr, value);
			}
		}

		[NativeName("GetColorFormat")]
		private GraphicsFormat GetColorFormat(bool suppressWarnings)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return RenderTexture.GetColorFormat_Injected(intPtr, suppressWarnings);
		}

		[NativeName("SetColorFormat")]
		private void SetColorFormat(GraphicsFormat format)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			RenderTexture.SetColorFormat_Injected(intPtr, format);
		}

		public new GraphicsFormat graphicsFormat
		{
			get
			{
				return this.GetColorFormat(true);
			}
			set
			{
				this.SetColorFormat(value);
			}
		}

		[NativeProperty("MipMap")]
		public bool useMipMap
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return RenderTexture.get_useMipMap_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				RenderTexture.set_useMipMap_Injected(intPtr, value);
			}
		}

		[NativeProperty("SRGBReadWrite")]
		public bool sRGB
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return RenderTexture.get_sRGB_Injected(intPtr);
			}
		}

		[NativeProperty("VRUsage")]
		public VRTextureUsage vrUsage
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return RenderTexture.get_vrUsage_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				RenderTexture.set_vrUsage_Injected(intPtr, value);
			}
		}

		[NativeProperty("Memoryless")]
		public RenderTextureMemoryless memorylessMode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return RenderTexture.get_memorylessMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				RenderTexture.set_memorylessMode_Injected(intPtr, value);
			}
		}

		public RenderTextureFormat format
		{
			get
			{
				bool flag = this.graphicsFormat > GraphicsFormat.None;
				RenderTextureFormat renderTextureFormat;
				if (flag)
				{
					renderTextureFormat = GraphicsFormatUtility.GetRenderTextureFormat(this.graphicsFormat);
				}
				else
				{
					renderTextureFormat = ((this.GetDescriptor().shadowSamplingMode != ShadowSamplingMode.None) ? RenderTextureFormat.Shadowmap : RenderTextureFormat.Depth);
				}
				return renderTextureFormat;
			}
			set
			{
				bool flag = value == RenderTextureFormat.Depth || value == RenderTextureFormat.Shadowmap;
				if (flag)
				{
					bool flag2 = this.depthStencilFormat == GraphicsFormat.None;
					if (flag2)
					{
						RenderTexture.WarnAboutFallbackTo16BitsDepth(value);
						this.depthStencilFormat = GraphicsFormat.D16_UNorm;
					}
					bool flag3 = value == RenderTextureFormat.Shadowmap;
					if (flag3)
					{
						this.SetShadowSamplingMode(ShadowSamplingMode.CompareDepths);
					}
				}
				this.graphicsFormat = GraphicsFormatUtility.GetGraphicsFormat(value, this.sRGB);
			}
		}

		public GraphicsFormat stencilFormat
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return RenderTexture.get_stencilFormat_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				RenderTexture.set_stencilFormat_Injected(intPtr, value);
			}
		}

		public GraphicsFormat depthStencilFormat
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return RenderTexture.get_depthStencilFormat_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				RenderTexture.set_depthStencilFormat_Injected(intPtr, value);
			}
		}

		public bool autoGenerateMips
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return RenderTexture.get_autoGenerateMips_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				RenderTexture.set_autoGenerateMips_Injected(intPtr, value);
			}
		}

		public int volumeDepth
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return RenderTexture.get_volumeDepth_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				RenderTexture.set_volumeDepth_Injected(intPtr, value);
			}
		}

		public int antiAliasing
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return RenderTexture.get_antiAliasing_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				RenderTexture.set_antiAliasing_Injected(intPtr, value);
			}
		}

		public bool bindTextureMS
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return RenderTexture.get_bindTextureMS_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				RenderTexture.set_bindTextureMS_Injected(intPtr, value);
			}
		}

		public bool enableRandomWrite
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return RenderTexture.get_enableRandomWrite_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				RenderTexture.set_enableRandomWrite_Injected(intPtr, value);
			}
		}

		public bool useDynamicScale
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return RenderTexture.get_useDynamicScale_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				RenderTexture.set_useDynamicScale_Injected(intPtr, value);
			}
		}

		public bool useDynamicScaleExplicit
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return RenderTexture.get_useDynamicScaleExplicit_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				RenderTexture.set_useDynamicScaleExplicit_Injected(intPtr, value);
			}
		}

		public bool enableShadingRate
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return RenderTexture.get_enableShadingRate_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				RenderTexture.set_enableShadingRate_Injected(intPtr, value);
			}
		}

		public void ApplyDynamicScale()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			RenderTexture.ApplyDynamicScale_Injected(intPtr);
		}

		private bool GetIsPowerOfTwo()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return RenderTexture.GetIsPowerOfTwo_Injected(intPtr);
		}

		public bool isPowerOfTwo
		{
			get
			{
				return this.GetIsPowerOfTwo();
			}
			set
			{
			}
		}

		[FreeFunction("RenderTexture::GetActiveAsRenderTexture")]
		private static RenderTexture GetActive()
		{
			return Unmarshal.UnmarshalUnityObject<RenderTexture>(RenderTexture.GetActive_Injected());
		}

		[FreeFunction("RenderTextureScripting::SetActive")]
		private static void SetActive(RenderTexture rt)
		{
			RenderTexture.SetActive_Injected(Object.MarshalledUnityObject.Marshal<RenderTexture>(rt));
		}

		public static RenderTexture active
		{
			get
			{
				return RenderTexture.GetActive();
			}
			set
			{
				RenderTexture.SetActive(value);
			}
		}

		[FreeFunction(Name = "RenderTextureScripting::GetColorBuffer", HasExplicitThis = true)]
		private RenderBuffer GetColorBuffer()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			RenderBuffer renderBuffer;
			RenderTexture.GetColorBuffer_Injected(intPtr, out renderBuffer);
			return renderBuffer;
		}

		[FreeFunction(Name = "RenderTextureScripting::GetDepthBuffer", HasExplicitThis = true)]
		private RenderBuffer GetDepthBuffer()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			RenderBuffer renderBuffer;
			RenderTexture.GetDepthBuffer_Injected(intPtr, out renderBuffer);
			return renderBuffer;
		}

		private void SetMipMapCount(int count)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			RenderTexture.SetMipMapCount_Injected(intPtr, count);
		}

		internal void SetShadowSamplingMode(ShadowSamplingMode samplingMode)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			RenderTexture.SetShadowSamplingMode_Injected(intPtr, samplingMode);
		}

		public RenderBuffer colorBuffer
		{
			get
			{
				return this.GetColorBuffer();
			}
		}

		public RenderBuffer depthBuffer
		{
			get
			{
				return this.GetDepthBuffer();
			}
		}

		public IntPtr GetNativeDepthBufferPtr()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return RenderTexture.GetNativeDepthBufferPtr_Injected(intPtr);
		}

		public void DiscardContents(bool discardColor, bool discardDepth)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			RenderTexture.DiscardContents_Injected(intPtr, discardColor, discardDepth);
		}

		[Obsolete("This function has no effect.", false)]
		public void MarkRestoreExpected()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			RenderTexture.MarkRestoreExpected_Injected(intPtr);
		}

		public void DiscardContents()
		{
			this.DiscardContents(true, true);
		}

		[NativeName("ResolveAntiAliasedSurface")]
		private void ResolveAA()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			RenderTexture.ResolveAA_Injected(intPtr);
		}

		[NativeName("ResolveAntiAliasedSurface")]
		private void ResolveAATo(RenderTexture rt)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			RenderTexture.ResolveAATo_Injected(intPtr, Object.MarshalledUnityObject.Marshal<RenderTexture>(rt));
		}

		public void ResolveAntiAliasedSurface()
		{
			this.ResolveAA();
		}

		public void ResolveAntiAliasedSurface(RenderTexture target)
		{
			this.ResolveAATo(target);
		}

		[FreeFunction(Name = "RenderTextureScripting::SetGlobalShaderProperty", HasExplicitThis = true)]
		public unsafe void SetGlobalShaderProperty(string propertyName)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(propertyName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = propertyName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				RenderTexture.SetGlobalShaderProperty_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		public bool Create()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return RenderTexture.Create_Injected(intPtr);
		}

		public void Release()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			RenderTexture.Release_Injected(intPtr);
		}

		public bool IsCreated()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return RenderTexture.IsCreated_Injected(intPtr);
		}

		public void GenerateMips()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			RenderTexture.GenerateMips_Injected(intPtr);
		}

		[NativeThrows]
		public void ConvertToEquirect(RenderTexture equirect, Camera.MonoOrStereoscopicEye eye = Camera.MonoOrStereoscopicEye.Mono)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			RenderTexture.ConvertToEquirect_Injected(intPtr, Object.MarshalledUnityObject.Marshal<RenderTexture>(equirect), eye);
		}

		internal void SetSRGBReadWrite(bool srgb)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			RenderTexture.SetSRGBReadWrite_Injected(intPtr, srgb);
		}

		[FreeFunction("RenderTextureScripting::Create")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] RenderTexture rt);

		[FreeFunction("RenderTextureSupportsStencil")]
		public static bool SupportsStencil(RenderTexture rt)
		{
			return RenderTexture.SupportsStencil_Injected(Object.MarshalledUnityObject.Marshal<RenderTexture>(rt));
		}

		[NativeName("SetRenderTextureDescFromScript")]
		private void SetRenderTextureDescriptor(RenderTextureDescriptor desc)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			RenderTexture.SetRenderTextureDescriptor_Injected(intPtr, ref desc);
		}

		[NativeName("GetRenderTextureDesc")]
		private RenderTextureDescriptor GetDescriptor()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			RenderTextureDescriptor renderTextureDescriptor;
			RenderTexture.GetDescriptor_Injected(intPtr, out renderTextureDescriptor);
			return renderTextureDescriptor;
		}

		[FreeFunction("GetRenderBufferManager().GetTextures().GetTempBuffer")]
		private static RenderTexture GetTemporary_Internal(RenderTextureDescriptor desc)
		{
			return Unmarshal.UnmarshalUnityObject<RenderTexture>(RenderTexture.GetTemporary_Internal_Injected(ref desc));
		}

		[FreeFunction("GetRenderBufferManager().GetTextures().ReleaseTempBuffer")]
		public static void ReleaseTemporary(RenderTexture temp)
		{
			RenderTexture.ReleaseTemporary_Injected(Object.MarshalledUnityObject.Marshal<RenderTexture>(temp));
		}

		public int depth
		{
			[FreeFunction("RenderTextureScripting::GetDepth", HasExplicitThis = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return RenderTexture.get_depth_Injected(intPtr);
			}
			[FreeFunction("RenderTextureScripting::SetDepth", HasExplicitThis = true)]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<RenderTexture>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				RenderTexture.set_depth_Injected(intPtr, value);
			}
		}

		[RequiredByNativeCode]
		protected internal RenderTexture()
		{
		}

		public RenderTexture(RenderTextureDescriptor desc)
		{
			RenderTexture.ValidateRenderTextureDesc(ref desc);
			RenderTexture.Internal_Create(this);
			this.SetRenderTextureDescriptor(desc);
		}

		public RenderTexture(RenderTexture textureToCopy)
		{
			bool flag = textureToCopy == null;
			if (flag)
			{
				throw new ArgumentNullException("textureToCopy");
			}
			RenderTextureDescriptor descriptor = textureToCopy.descriptor;
			RenderTexture.ValidateRenderTextureDesc(ref descriptor);
			RenderTexture.Internal_Create(this);
			this.SetRenderTextureDescriptor(descriptor);
		}

		[ExcludeFromDocs]
		public RenderTexture(int width, int height, int depth, DefaultFormat format)
			: this(width, height, RenderTexture.GetDefaultColorFormat(format), RenderTexture.GetDefaultDepthStencilFormat(format, depth), Texture.GenerateAllMips)
		{
			bool flag = this != null;
			if (flag)
			{
				this.SetShadowSamplingMode(RenderTexture.GetShadowSamplingModeForFormat(format));
			}
		}

		[ExcludeFromDocs]
		public RenderTexture(int width, int height, int depth, GraphicsFormat format)
			: this(width, height, depth, format, Texture.GenerateAllMips)
		{
		}

		[ExcludeFromDocs]
		public RenderTexture(int width, int height, int depth, GraphicsFormat format, int mipCount)
		{
			bool flag = format != GraphicsFormat.None && !base.ValidateFormat(format, GraphicsFormatUsage.Render);
			if (!flag)
			{
				RenderTexture.Internal_Create(this);
				this.depthStencilFormat = RenderTexture.GetDepthStencilFormatLegacy(depth, format);
				this.width = width;
				this.height = height;
				this.graphicsFormat = format;
				this.SetMipMapCount(mipCount);
				this.SetSRGBReadWrite(GraphicsFormatUtility.IsSRGBFormat(format));
			}
		}

		[ExcludeFromDocs]
		public RenderTexture(int width, int height, GraphicsFormat colorFormat, GraphicsFormat depthStencilFormat, int mipCount)
		{
			bool flag = colorFormat != GraphicsFormat.None && !base.ValidateFormat(colorFormat, GraphicsFormatUsage.Render);
			if (!flag)
			{
				RenderTexture.Internal_Create(this);
				this.width = width;
				this.height = height;
				this.depthStencilFormat = depthStencilFormat;
				this.graphicsFormat = colorFormat;
				this.SetMipMapCount(mipCount);
				this.SetSRGBReadWrite(GraphicsFormatUtility.IsSRGBFormat(colorFormat));
			}
		}

		[ExcludeFromDocs]
		public RenderTexture(int width, int height, GraphicsFormat colorFormat, GraphicsFormat depthStencilFormat)
			: this(width, height, colorFormat, depthStencilFormat, Texture.GenerateAllMips)
		{
		}

		public RenderTexture(int width, int height, int depth, [DefaultValue("RenderTextureFormat.Default")] RenderTextureFormat format, [DefaultValue("RenderTextureReadWrite.Default")] RenderTextureReadWrite readWrite)
		{
			this.Initialize(width, height, depth, format, readWrite, Texture.GenerateAllMips);
		}

		[ExcludeFromDocs]
		public RenderTexture(int width, int height, int depth, RenderTextureFormat format)
			: this(width, height, depth, format, Texture.GenerateAllMips)
		{
		}

		[ExcludeFromDocs]
		public RenderTexture(int width, int height, int depth)
			: this(width, height, depth, RenderTextureFormat.Default)
		{
		}

		[ExcludeFromDocs]
		public RenderTexture(int width, int height, int depth, RenderTextureFormat format, int mipCount)
		{
			this.Initialize(width, height, depth, format, RenderTextureReadWrite.Default, mipCount);
		}

		private void Initialize(int width, int height, int depth, RenderTextureFormat format, RenderTextureReadWrite readWrite, int mipCount)
		{
			GraphicsFormat compatibleFormat = RenderTexture.GetCompatibleFormat(format, readWrite);
			GraphicsFormat depthStencilFormatLegacy = RenderTexture.GetDepthStencilFormatLegacy(depth, format, false);
			bool flag = compatibleFormat > GraphicsFormat.None;
			if (flag)
			{
				bool flag2 = !base.ValidateFormat(compatibleFormat, GraphicsFormatUsage.Render);
				if (flag2)
				{
					return;
				}
			}
			RenderTexture.Internal_Create(this);
			this.width = width;
			this.height = height;
			this.depthStencilFormat = depthStencilFormatLegacy;
			this.graphicsFormat = compatibleFormat;
			this.SetMipMapCount(mipCount);
			this.SetSRGBReadWrite(GraphicsFormatUtility.IsSRGBFormat(compatibleFormat));
			this.SetShadowSamplingMode(RenderTexture.GetShadowSamplingModeForFormat(format));
		}

		internal static GraphicsFormat GetDepthStencilFormatLegacy(int depthBits, GraphicsFormat colorFormat)
		{
			return RenderTexture.GetDepthStencilFormatLegacy(depthBits, false);
		}

		internal static GraphicsFormat GetDepthStencilFormatLegacy(int depthBits, RenderTextureFormat format, bool disableFallback = false)
		{
			bool flag = !disableFallback && (format == RenderTextureFormat.Depth || format == RenderTextureFormat.Shadowmap) && depthBits < 16;
			if (flag)
			{
				RenderTexture.WarnAboutFallbackTo16BitsDepth(format);
				depthBits = 16;
			}
			return RenderTexture.GetDepthStencilFormatLegacy(depthBits, format == RenderTextureFormat.Shadowmap);
		}

		internal static GraphicsFormat GetDepthStencilFormatLegacy(int depthBits, DefaultFormat format)
		{
			return RenderTexture.GetDepthStencilFormatLegacy(depthBits, format == DefaultFormat.Shadow);
		}

		internal static GraphicsFormat GetDepthStencilFormatLegacy(int depthBits, ShadowSamplingMode shadowSamplingMode)
		{
			return RenderTexture.GetDepthStencilFormatLegacy(depthBits, shadowSamplingMode != ShadowSamplingMode.None);
		}

		internal static GraphicsFormat GetDepthStencilFormatLegacy(int depthBits, bool requestedShadowMap)
		{
			GraphicsFormat graphicsFormat = (requestedShadowMap ? GraphicsFormatUtility.GetDepthStencilFormat(depthBits, 0) : GraphicsFormatUtility.GetDepthStencilFormat(depthBits));
			bool flag = depthBits > 16 && graphicsFormat == GraphicsFormat.None && requestedShadowMap;
			GraphicsFormat graphicsFormat2;
			if (flag)
			{
				Debug.LogWarning(string.Format("No compatible shadow map depth format with {0} or more depth bits has been found. Changing to a 16 bit depth buffer.", depthBits));
				graphicsFormat2 = GraphicsFormat.D16_UNorm;
			}
			else
			{
				graphicsFormat2 = graphicsFormat;
			}
			return graphicsFormat2;
		}

		public RenderTextureDescriptor descriptor
		{
			get
			{
				return this.GetDescriptor();
			}
			set
			{
				RenderTexture.ValidateRenderTextureDesc(ref value);
				this.SetRenderTextureDescriptor(value);
			}
		}

		private static void ValidateRenderTextureDesc(ref RenderTextureDescriptor desc)
		{
			bool flag = desc.graphicsFormat == GraphicsFormat.None && desc.depthStencilFormat == GraphicsFormat.None;
			if (flag)
			{
				RenderTexture.WarnAboutFallbackTo16BitsDepth(desc.colorFormat);
				desc.depthStencilFormat = GraphicsFormat.D16_UNorm;
			}
			bool flag2 = desc.graphicsFormat != GraphicsFormat.None && !SystemInfo.IsFormatSupported(desc.graphicsFormat, GraphicsFormatUsage.Render);
			if (flag2)
			{
				throw new ArgumentException("RenderTextureDesc graphicsFormat must be a supported GraphicsFormat. " + desc.graphicsFormat.ToString() + " is not supported on this platform.", "desc.graphicsFormat");
			}
			bool flag3 = desc.depthStencilFormat != GraphicsFormat.None && !GraphicsFormatUtility.IsDepthStencilFormat(desc.depthStencilFormat);
			if (flag3)
			{
				throw new ArgumentException("RenderTextureDesc depthStencilFormat must be a supported depth/stencil GraphicsFormat. " + desc.depthStencilFormat.ToString() + " is not supported on this platform.", "desc.depthStencilFormat");
			}
			bool flag4 = desc.width <= 0;
			if (flag4)
			{
				throw new ArgumentException("RenderTextureDesc width must be greater than zero.", "desc.width");
			}
			bool flag5 = desc.height <= 0;
			if (flag5)
			{
				throw new ArgumentException("RenderTextureDesc height must be greater than zero.", "desc.height");
			}
			bool flag6 = desc.volumeDepth <= 0;
			if (flag6)
			{
				throw new ArgumentException("RenderTextureDesc volumeDepth must be greater than zero.", "desc.volumeDepth");
			}
			bool flag7 = desc.msaaSamples != 1 && desc.msaaSamples != 2 && desc.msaaSamples != 4 && desc.msaaSamples != 8;
			if (flag7)
			{
				throw new ArgumentException("RenderTextureDesc msaaSamples must be 1, 2, 4, or 8.", "desc.msaaSamples");
			}
			bool flag8 = desc.dimension == TextureDimension.CubeArray && desc.volumeDepth % 6 != 0;
			if (flag8)
			{
				throw new ArgumentException("RenderTextureDesc volumeDepth must be a multiple of 6 when dimension is CubeArray", "desc.volumeDepth");
			}
			bool flag9 = GraphicsFormatUtility.IsDepthStencilFormat(desc.graphicsFormat);
			if (flag9)
			{
				throw new ArgumentException("RenderTextureDesc graphicsFormat must not be a depth/stencil format. " + desc.graphicsFormat.ToString() + " is not supported.", "desc.graphicsFormat");
			}
		}

		internal static GraphicsFormat GetDefaultColorFormat(DefaultFormat format)
		{
			GraphicsFormat graphicsFormat;
			if (format - DefaultFormat.DepthStencil > 1)
			{
				graphicsFormat = SystemInfo.GetGraphicsFormat(format);
			}
			else
			{
				graphicsFormat = GraphicsFormat.None;
			}
			return graphicsFormat;
		}

		internal static GraphicsFormat GetDefaultDepthStencilFormat(DefaultFormat format, int depth)
		{
			GraphicsFormat graphicsFormat;
			if (format - DefaultFormat.DepthStencil > 1)
			{
				graphicsFormat = RenderTexture.GetDepthStencilFormatLegacy(depth, format);
			}
			else
			{
				graphicsFormat = SystemInfo.GetGraphicsFormat(format);
			}
			return graphicsFormat;
		}

		internal static ShadowSamplingMode GetShadowSamplingModeForFormat(RenderTextureFormat format)
		{
			return (format == RenderTextureFormat.Shadowmap) ? ShadowSamplingMode.CompareDepths : ShadowSamplingMode.None;
		}

		internal static ShadowSamplingMode GetShadowSamplingModeForFormat(DefaultFormat format)
		{
			return (format == DefaultFormat.Shadow) ? ShadowSamplingMode.CompareDepths : ShadowSamplingMode.None;
		}

		internal static void WarnAboutFallbackTo16BitsDepth(RenderTextureFormat format)
		{
			Debug.LogWarning(string.Format("{0} RenderTexture requested without a depth buffer. Changing to a 16 bit depth buffer. To resolve this warning, please specify the desired number of depth bits when creating the render texture.", format));
		}

		internal static GraphicsFormat GetCompatibleFormat(RenderTextureFormat renderTextureFormat, RenderTextureReadWrite readWrite)
		{
			GraphicsFormat graphicsFormat = GraphicsFormatUtility.GetGraphicsFormat(renderTextureFormat, readWrite);
			GraphicsFormat compatibleFormat = SystemInfo.GetCompatibleFormat(graphicsFormat, GraphicsFormatUsage.Render);
			bool flag = graphicsFormat == compatibleFormat;
			GraphicsFormat graphicsFormat2;
			if (flag)
			{
				graphicsFormat2 = graphicsFormat;
			}
			else
			{
				Debug.LogWarning(string.Format("'{0}' is not supported. RenderTexture::GetTemporary fallbacks to {1} format on this platform. Use 'SystemInfo.IsFormatSupported' C# API to check format support.", graphicsFormat.ToString(), compatibleFormat.ToString()));
				graphicsFormat2 = compatibleFormat;
			}
			return graphicsFormat2;
		}

		public static RenderTexture GetTemporary(RenderTextureDescriptor desc)
		{
			RenderTexture.ValidateRenderTextureDesc(ref desc);
			desc.createdFromScript = true;
			return RenderTexture.GetTemporary_Internal(desc);
		}

		private static RenderTexture GetTemporaryImpl(int width, int height, GraphicsFormat depthStencilFormat, GraphicsFormat colorFormat, int antiAliasing = 1, RenderTextureMemoryless memorylessMode = RenderTextureMemoryless.None, VRTextureUsage vrUsage = VRTextureUsage.None, bool useDynamicScale = false, ShadowSamplingMode shadowSamplingMode = ShadowSamplingMode.None)
		{
			return RenderTexture.GetTemporary(new RenderTextureDescriptor(width, height, colorFormat, depthStencilFormat)
			{
				msaaSamples = antiAliasing,
				memoryless = memorylessMode,
				vrUsage = vrUsage,
				useDynamicScale = useDynamicScale,
				shadowSamplingMode = shadowSamplingMode
			});
		}

		[ExcludeFromDocs]
		public static RenderTexture GetTemporary(int width, int height, int depthBuffer, GraphicsFormat format, [DefaultValue("1")] int antiAliasing, [DefaultValue("RenderTextureMemoryless.None")] RenderTextureMemoryless memorylessMode, [DefaultValue("VRTextureUsage.None")] VRTextureUsage vrUsage, [DefaultValue("false")] bool useDynamicScale)
		{
			ShadowSamplingMode shadowSamplingMode = ShadowSamplingMode.None;
			return RenderTexture.GetTemporaryImpl(width, height, RenderTexture.GetDepthStencilFormatLegacy(depthBuffer, shadowSamplingMode), format, antiAliasing, memorylessMode, vrUsage, useDynamicScale, ShadowSamplingMode.None);
		}

		[ExcludeFromDocs]
		public static RenderTexture GetTemporary(int width, int height, int depthBuffer, GraphicsFormat format, int antiAliasing, RenderTextureMemoryless memorylessMode, VRTextureUsage vrUsage)
		{
			return RenderTexture.GetTemporary(width, height, depthBuffer, format, antiAliasing, memorylessMode, vrUsage, false);
		}

		[ExcludeFromDocs]
		public static RenderTexture GetTemporary(int width, int height, int depthBuffer, GraphicsFormat format, int antiAliasing, RenderTextureMemoryless memorylessMode)
		{
			return RenderTexture.GetTemporary(width, height, depthBuffer, format, antiAliasing, memorylessMode, VRTextureUsage.None);
		}

		[ExcludeFromDocs]
		public static RenderTexture GetTemporary(int width, int height, int depthBuffer, GraphicsFormat format, int antiAliasing)
		{
			return RenderTexture.GetTemporary(width, height, depthBuffer, format, antiAliasing, RenderTextureMemoryless.None);
		}

		[ExcludeFromDocs]
		public static RenderTexture GetTemporary(int width, int height, int depthBuffer, GraphicsFormat format)
		{
			return RenderTexture.GetTemporary(width, height, depthBuffer, format, 1);
		}

		public static RenderTexture GetTemporary(int width, int height, [DefaultValue("0")] int depthBuffer, [DefaultValue("RenderTextureFormat.Default")] RenderTextureFormat format, [DefaultValue("RenderTextureReadWrite.Default")] RenderTextureReadWrite readWrite, [DefaultValue("1")] int antiAliasing, [DefaultValue("RenderTextureMemoryless.None")] RenderTextureMemoryless memorylessMode, [DefaultValue("VRTextureUsage.None")] VRTextureUsage vrUsage, [DefaultValue("false")] bool useDynamicScale)
		{
			GraphicsFormat compatibleFormat = RenderTexture.GetCompatibleFormat(format, readWrite);
			GraphicsFormat depthStencilFormatLegacy = RenderTexture.GetDepthStencilFormatLegacy(depthBuffer, format, false);
			ShadowSamplingMode shadowSamplingModeForFormat = RenderTexture.GetShadowSamplingModeForFormat(format);
			return RenderTexture.GetTemporaryImpl(width, height, depthStencilFormatLegacy, compatibleFormat, antiAliasing, memorylessMode, vrUsage, useDynamicScale, shadowSamplingModeForFormat);
		}

		[ExcludeFromDocs]
		public static RenderTexture GetTemporary(int width, int height, int depthBuffer, RenderTextureFormat format, RenderTextureReadWrite readWrite, int antiAliasing, RenderTextureMemoryless memorylessMode, VRTextureUsage vrUsage)
		{
			return RenderTexture.GetTemporary(width, height, depthBuffer, format, readWrite, antiAliasing, memorylessMode, vrUsage, false);
		}

		[ExcludeFromDocs]
		public static RenderTexture GetTemporary(int width, int height, int depthBuffer, RenderTextureFormat format, RenderTextureReadWrite readWrite, int antiAliasing, RenderTextureMemoryless memorylessMode)
		{
			return RenderTexture.GetTemporary(width, height, depthBuffer, format, readWrite, antiAliasing, memorylessMode, VRTextureUsage.None);
		}

		[ExcludeFromDocs]
		public static RenderTexture GetTemporary(int width, int height, int depthBuffer, RenderTextureFormat format, RenderTextureReadWrite readWrite, int antiAliasing)
		{
			return RenderTexture.GetTemporary(width, height, depthBuffer, format, readWrite, antiAliasing, RenderTextureMemoryless.None);
		}

		[ExcludeFromDocs]
		public static RenderTexture GetTemporary(int width, int height, int depthBuffer, RenderTextureFormat format, RenderTextureReadWrite readWrite)
		{
			return RenderTexture.GetTemporary(width, height, depthBuffer, format, readWrite, 1);
		}

		[ExcludeFromDocs]
		public static RenderTexture GetTemporary(int width, int height, int depthBuffer, RenderTextureFormat format)
		{
			return RenderTexture.GetTemporary(width, height, depthBuffer, format, RenderTextureReadWrite.Default);
		}

		[ExcludeFromDocs]
		public static RenderTexture GetTemporary(int width, int height, int depthBuffer)
		{
			return RenderTexture.GetTemporary(width, height, depthBuffer, RenderTextureFormat.Default);
		}

		[ExcludeFromDocs]
		public static RenderTexture GetTemporary(int width, int height)
		{
			return RenderTexture.GetTemporary(width, height, 0);
		}

		[Obsolete("Use RenderTexture.dimension instead.", false)]
		public bool isCubemap
		{
			get
			{
				return this.dimension == TextureDimension.Cube;
			}
			set
			{
				this.dimension = (value ? TextureDimension.Cube : TextureDimension.Tex2D);
			}
		}

		[Obsolete("Use RenderTexture.dimension instead.", false)]
		public bool isVolume
		{
			get
			{
				return this.dimension == TextureDimension.Tex3D;
			}
			set
			{
				this.dimension = (value ? TextureDimension.Tex3D : TextureDimension.Tex2D);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("RenderTexture.enabled is always now, no need to use it.", false)]
		public static bool enabled
		{
			get
			{
				return true;
			}
			set
			{
			}
		}

		[Obsolete("GetTexelOffset always returns zero now, no point in using it.", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public Vector2 GetTexelOffset()
		{
			return Vector2.zero;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_width_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_width_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_height_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_height_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern TextureDimension get_dimension_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_dimension_Injected(IntPtr _unity_self, TextureDimension value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern GraphicsFormat GetColorFormat_Injected(IntPtr _unity_self, bool suppressWarnings);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetColorFormat_Injected(IntPtr _unity_self, GraphicsFormat format);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_useMipMap_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_useMipMap_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_sRGB_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern VRTextureUsage get_vrUsage_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_vrUsage_Injected(IntPtr _unity_self, VRTextureUsage value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern RenderTextureMemoryless get_memorylessMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_memorylessMode_Injected(IntPtr _unity_self, RenderTextureMemoryless value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern GraphicsFormat get_stencilFormat_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_stencilFormat_Injected(IntPtr _unity_self, GraphicsFormat value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern GraphicsFormat get_depthStencilFormat_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_depthStencilFormat_Injected(IntPtr _unity_self, GraphicsFormat value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_autoGenerateMips_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_autoGenerateMips_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_volumeDepth_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_volumeDepth_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_antiAliasing_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_antiAliasing_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_bindTextureMS_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_bindTextureMS_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_enableRandomWrite_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_enableRandomWrite_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_useDynamicScale_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_useDynamicScale_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_useDynamicScaleExplicit_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_useDynamicScaleExplicit_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_enableShadingRate_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_enableShadingRate_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ApplyDynamicScale_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetIsPowerOfTwo_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetActive_Injected();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetActive_Injected(IntPtr rt);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetColorBuffer_Injected(IntPtr _unity_self, out RenderBuffer ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetDepthBuffer_Injected(IntPtr _unity_self, out RenderBuffer ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetMipMapCount_Injected(IntPtr _unity_self, int count);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetShadowSamplingMode_Injected(IntPtr _unity_self, ShadowSamplingMode samplingMode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetNativeDepthBufferPtr_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DiscardContents_Injected(IntPtr _unity_self, bool discardColor, bool discardDepth);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void MarkRestoreExpected_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ResolveAA_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ResolveAATo_Injected(IntPtr _unity_self, IntPtr rt);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalShaderProperty_Injected(IntPtr _unity_self, ref ManagedSpanWrapper propertyName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Create_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Release_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsCreated_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GenerateMips_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ConvertToEquirect_Injected(IntPtr _unity_self, IntPtr equirect, Camera.MonoOrStereoscopicEye eye);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetSRGBReadWrite_Injected(IntPtr _unity_self, bool srgb);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SupportsStencil_Injected(IntPtr rt);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetRenderTextureDescriptor_Injected(IntPtr _unity_self, [In] ref RenderTextureDescriptor desc);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetDescriptor_Injected(IntPtr _unity_self, out RenderTextureDescriptor ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetTemporary_Internal_Injected([In] ref RenderTextureDescriptor desc);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReleaseTemporary_Injected(IntPtr temp);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_depth_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_depth_Injected(IntPtr _unity_self, int value);
	}
}
