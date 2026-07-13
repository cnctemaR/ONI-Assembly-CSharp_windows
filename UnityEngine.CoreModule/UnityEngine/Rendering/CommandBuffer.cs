using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Profiling;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Internal;
using UnityEngine.Profiling;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering
{
	[NativeHeader("Runtime/Shaders/RayTracing/RayTracingShader.h")]
	[UsedByNativeCode]
	[NativeHeader("Runtime/Shaders/ComputeShader.h")]
	[NativeType("Runtime/Graphics/CommandBuffer/RenderingCommandBuffer.h")]
	[NativeHeader("Runtime/Export/Graphics/RenderingCommandBuffer.bindings.h")]
	public class CommandBuffer : IDisposable
	{
		public void ConvertTexture(RenderTargetIdentifier src, RenderTargetIdentifier dst)
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.ConvertTexture_Internal(src, 0, dst, 0);
		}

		public void ConvertTexture(RenderTargetIdentifier src, int srcElement, RenderTargetIdentifier dst, int dstElement)
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.ConvertTexture_Internal(src, srcElement, dst, dstElement);
		}

		[NativeMethod("AddWaitAllAsyncReadbackRequests")]
		public void WaitAllAsyncReadbackRequests()
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.WaitAllAsyncReadbackRequests_Injected(intPtr);
		}

		public void RequestAsyncReadback(ComputeBuffer src, Action<AsyncGPUReadbackRequest> callback)
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.Internal_RequestAsyncReadback_1(src, callback, null);
		}

		public void RequestAsyncReadback(GraphicsBuffer src, Action<AsyncGPUReadbackRequest> callback)
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.Internal_RequestAsyncReadback_8(src, callback, null);
		}

		public void RequestAsyncReadback(ComputeBuffer src, int size, int offset, Action<AsyncGPUReadbackRequest> callback)
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.Internal_RequestAsyncReadback_2(src, size, offset, callback, null);
		}

		public void RequestAsyncReadback(GraphicsBuffer src, int size, int offset, Action<AsyncGPUReadbackRequest> callback)
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.Internal_RequestAsyncReadback_9(src, size, offset, callback, null);
		}

		public void RequestAsyncReadback(Texture src, Action<AsyncGPUReadbackRequest> callback)
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.Internal_RequestAsyncReadback_3(src, callback, null);
		}

		public void RequestAsyncReadback(Texture src, int mipIndex, Action<AsyncGPUReadbackRequest> callback)
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.Internal_RequestAsyncReadback_4(src, mipIndex, callback, null);
		}

		public void RequestAsyncReadback(Texture src, int mipIndex, TextureFormat dstFormat, Action<AsyncGPUReadbackRequest> callback)
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.Internal_RequestAsyncReadback_5(src, mipIndex, GraphicsFormatUtility.GetGraphicsFormat(dstFormat, QualitySettings.activeColorSpace == ColorSpace.Linear), callback, null);
		}

		public void RequestAsyncReadback(Texture src, int mipIndex, GraphicsFormat dstFormat, Action<AsyncGPUReadbackRequest> callback)
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.Internal_RequestAsyncReadback_5(src, mipIndex, dstFormat, callback, null);
		}

		public void RequestAsyncReadback(Texture src, int mipIndex, int x, int width, int y, int height, int z, int depth, Action<AsyncGPUReadbackRequest> callback)
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.Internal_RequestAsyncReadback_6(src, mipIndex, x, width, y, height, z, depth, callback, null);
		}

		public void RequestAsyncReadback(Texture src, int mipIndex, int x, int width, int y, int height, int z, int depth, TextureFormat dstFormat, Action<AsyncGPUReadbackRequest> callback)
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.Internal_RequestAsyncReadback_7(src, mipIndex, x, width, y, height, z, depth, GraphicsFormatUtility.GetGraphicsFormat(dstFormat, QualitySettings.activeColorSpace == ColorSpace.Linear), callback, null);
		}

		public void RequestAsyncReadback(Texture src, int mipIndex, int x, int width, int y, int height, int z, int depth, GraphicsFormat dstFormat, Action<AsyncGPUReadbackRequest> callback)
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.Internal_RequestAsyncReadback_7(src, mipIndex, x, width, y, height, z, depth, dstFormat, callback, null);
		}

		public unsafe void RequestAsyncReadbackIntoNativeArray<T>(ref NativeArray<T> output, ComputeBuffer src, Action<AsyncGPUReadbackRequest> callback) where T : struct
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			AsyncRequestNativeArrayData asyncRequestNativeArrayData = AsyncRequestNativeArrayData.CreateAndCheckAccess<T>(output);
			this.Internal_RequestAsyncReadback_1(src, callback, &asyncRequestNativeArrayData);
		}

		public unsafe void RequestAsyncReadbackIntoNativeArray<T>(ref NativeArray<T> output, ComputeBuffer src, int size, int offset, Action<AsyncGPUReadbackRequest> callback) where T : struct
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			AsyncRequestNativeArrayData asyncRequestNativeArrayData = AsyncRequestNativeArrayData.CreateAndCheckAccess<T>(output);
			this.Internal_RequestAsyncReadback_2(src, size, offset, callback, &asyncRequestNativeArrayData);
		}

		public unsafe void RequestAsyncReadbackIntoNativeArray<T>(ref NativeArray<T> output, GraphicsBuffer src, Action<AsyncGPUReadbackRequest> callback) where T : struct
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			AsyncRequestNativeArrayData asyncRequestNativeArrayData = AsyncRequestNativeArrayData.CreateAndCheckAccess<T>(output);
			this.Internal_RequestAsyncReadback_8(src, callback, &asyncRequestNativeArrayData);
		}

		public unsafe void RequestAsyncReadbackIntoNativeArray<T>(ref NativeArray<T> output, GraphicsBuffer src, int size, int offset, Action<AsyncGPUReadbackRequest> callback) where T : struct
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			AsyncRequestNativeArrayData asyncRequestNativeArrayData = AsyncRequestNativeArrayData.CreateAndCheckAccess<T>(output);
			this.Internal_RequestAsyncReadback_9(src, size, offset, callback, &asyncRequestNativeArrayData);
		}

		public unsafe void RequestAsyncReadbackIntoNativeArray<T>(ref NativeArray<T> output, Texture src, Action<AsyncGPUReadbackRequest> callback) where T : struct
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			AsyncRequestNativeArrayData asyncRequestNativeArrayData = AsyncRequestNativeArrayData.CreateAndCheckAccess<T>(output);
			this.Internal_RequestAsyncReadback_3(src, callback, &asyncRequestNativeArrayData);
		}

		public unsafe void RequestAsyncReadbackIntoNativeArray<T>(ref NativeArray<T> output, Texture src, int mipIndex, Action<AsyncGPUReadbackRequest> callback) where T : struct
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			AsyncRequestNativeArrayData asyncRequestNativeArrayData = AsyncRequestNativeArrayData.CreateAndCheckAccess<T>(output);
			this.Internal_RequestAsyncReadback_4(src, mipIndex, callback, &asyncRequestNativeArrayData);
		}

		public unsafe void RequestAsyncReadbackIntoNativeArray<T>(ref NativeArray<T> output, Texture src, int mipIndex, TextureFormat dstFormat, Action<AsyncGPUReadbackRequest> callback) where T : struct
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			AsyncRequestNativeArrayData asyncRequestNativeArrayData = AsyncRequestNativeArrayData.CreateAndCheckAccess<T>(output);
			this.Internal_RequestAsyncReadback_5(src, mipIndex, GraphicsFormatUtility.GetGraphicsFormat(dstFormat, QualitySettings.activeColorSpace == ColorSpace.Linear), callback, &asyncRequestNativeArrayData);
		}

		public unsafe void RequestAsyncReadbackIntoNativeArray<T>(ref NativeArray<T> output, Texture src, int mipIndex, GraphicsFormat dstFormat, Action<AsyncGPUReadbackRequest> callback) where T : struct
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			AsyncRequestNativeArrayData asyncRequestNativeArrayData = AsyncRequestNativeArrayData.CreateAndCheckAccess<T>(output);
			this.Internal_RequestAsyncReadback_5(src, mipIndex, dstFormat, callback, &asyncRequestNativeArrayData);
		}

		public unsafe void RequestAsyncReadbackIntoNativeArray<T>(ref NativeArray<T> output, Texture src, int mipIndex, int x, int width, int y, int height, int z, int depth, Action<AsyncGPUReadbackRequest> callback) where T : struct
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			AsyncRequestNativeArrayData asyncRequestNativeArrayData = AsyncRequestNativeArrayData.CreateAndCheckAccess<T>(output);
			this.Internal_RequestAsyncReadback_6(src, mipIndex, x, width, y, height, z, depth, callback, &asyncRequestNativeArrayData);
		}

		public unsafe void RequestAsyncReadbackIntoNativeArray<T>(ref NativeArray<T> output, Texture src, int mipIndex, int x, int width, int y, int height, int z, int depth, TextureFormat dstFormat, Action<AsyncGPUReadbackRequest> callback) where T : struct
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			AsyncRequestNativeArrayData asyncRequestNativeArrayData = AsyncRequestNativeArrayData.CreateAndCheckAccess<T>(output);
			this.Internal_RequestAsyncReadback_7(src, mipIndex, x, width, y, height, z, depth, GraphicsFormatUtility.GetGraphicsFormat(dstFormat, QualitySettings.activeColorSpace == ColorSpace.Linear), callback, &asyncRequestNativeArrayData);
		}

		public unsafe void RequestAsyncReadbackIntoNativeArray<T>(ref NativeArray<T> output, Texture src, int mipIndex, int x, int width, int y, int height, int z, int depth, GraphicsFormat dstFormat, Action<AsyncGPUReadbackRequest> callback) where T : struct
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			AsyncRequestNativeArrayData asyncRequestNativeArrayData = AsyncRequestNativeArrayData.CreateAndCheckAccess<T>(output);
			this.Internal_RequestAsyncReadback_7(src, mipIndex, x, width, y, height, z, depth, dstFormat, callback, &asyncRequestNativeArrayData);
		}

		public unsafe void RequestAsyncReadbackIntoNativeSlice<T>(ref NativeSlice<T> output, ComputeBuffer src, Action<AsyncGPUReadbackRequest> callback) where T : struct
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			AsyncRequestNativeArrayData asyncRequestNativeArrayData = AsyncRequestNativeArrayData.CreateAndCheckAccess<T>(output);
			this.Internal_RequestAsyncReadback_1(src, callback, &asyncRequestNativeArrayData);
		}

		public unsafe void RequestAsyncReadbackIntoNativeSlice<T>(ref NativeSlice<T> output, ComputeBuffer src, int size, int offset, Action<AsyncGPUReadbackRequest> callback) where T : struct
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			AsyncRequestNativeArrayData asyncRequestNativeArrayData = AsyncRequestNativeArrayData.CreateAndCheckAccess<T>(output);
			this.Internal_RequestAsyncReadback_2(src, size, offset, callback, &asyncRequestNativeArrayData);
		}

		public unsafe void RequestAsyncReadbackIntoNativeSlice<T>(ref NativeSlice<T> output, GraphicsBuffer src, Action<AsyncGPUReadbackRequest> callback) where T : struct
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			AsyncRequestNativeArrayData asyncRequestNativeArrayData = AsyncRequestNativeArrayData.CreateAndCheckAccess<T>(output);
			this.Internal_RequestAsyncReadback_8(src, callback, &asyncRequestNativeArrayData);
		}

		public unsafe void RequestAsyncReadbackIntoNativeSlice<T>(ref NativeSlice<T> output, GraphicsBuffer src, int size, int offset, Action<AsyncGPUReadbackRequest> callback) where T : struct
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			AsyncRequestNativeArrayData asyncRequestNativeArrayData = AsyncRequestNativeArrayData.CreateAndCheckAccess<T>(output);
			this.Internal_RequestAsyncReadback_9(src, size, offset, callback, &asyncRequestNativeArrayData);
		}

		public unsafe void RequestAsyncReadbackIntoNativeSlice<T>(ref NativeSlice<T> output, Texture src, Action<AsyncGPUReadbackRequest> callback) where T : struct
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			AsyncRequestNativeArrayData asyncRequestNativeArrayData = AsyncRequestNativeArrayData.CreateAndCheckAccess<T>(output);
			this.Internal_RequestAsyncReadback_3(src, callback, &asyncRequestNativeArrayData);
		}

		public unsafe void RequestAsyncReadbackIntoNativeSlice<T>(ref NativeSlice<T> output, Texture src, int mipIndex, Action<AsyncGPUReadbackRequest> callback) where T : struct
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			AsyncRequestNativeArrayData asyncRequestNativeArrayData = AsyncRequestNativeArrayData.CreateAndCheckAccess<T>(output);
			this.Internal_RequestAsyncReadback_4(src, mipIndex, callback, &asyncRequestNativeArrayData);
		}

		public unsafe void RequestAsyncReadbackIntoNativeSlice<T>(ref NativeSlice<T> output, Texture src, int mipIndex, TextureFormat dstFormat, Action<AsyncGPUReadbackRequest> callback) where T : struct
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			AsyncRequestNativeArrayData asyncRequestNativeArrayData = AsyncRequestNativeArrayData.CreateAndCheckAccess<T>(output);
			this.Internal_RequestAsyncReadback_5(src, mipIndex, GraphicsFormatUtility.GetGraphicsFormat(dstFormat, QualitySettings.activeColorSpace == ColorSpace.Linear), callback, &asyncRequestNativeArrayData);
		}

		public unsafe void RequestAsyncReadbackIntoNativeSlice<T>(ref NativeSlice<T> output, Texture src, int mipIndex, GraphicsFormat dstFormat, Action<AsyncGPUReadbackRequest> callback) where T : struct
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			AsyncRequestNativeArrayData asyncRequestNativeArrayData = AsyncRequestNativeArrayData.CreateAndCheckAccess<T>(output);
			this.Internal_RequestAsyncReadback_5(src, mipIndex, dstFormat, callback, &asyncRequestNativeArrayData);
		}

		public unsafe void RequestAsyncReadbackIntoNativeSlice<T>(ref NativeSlice<T> output, Texture src, int mipIndex, int x, int width, int y, int height, int z, int depth, Action<AsyncGPUReadbackRequest> callback) where T : struct
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			AsyncRequestNativeArrayData asyncRequestNativeArrayData = AsyncRequestNativeArrayData.CreateAndCheckAccess<T>(output);
			this.Internal_RequestAsyncReadback_6(src, mipIndex, x, width, y, height, z, depth, callback, &asyncRequestNativeArrayData);
		}

		public unsafe void RequestAsyncReadbackIntoNativeSlice<T>(ref NativeSlice<T> output, Texture src, int mipIndex, int x, int width, int y, int height, int z, int depth, TextureFormat dstFormat, Action<AsyncGPUReadbackRequest> callback) where T : struct
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			AsyncRequestNativeArrayData asyncRequestNativeArrayData = AsyncRequestNativeArrayData.CreateAndCheckAccess<T>(output);
			this.Internal_RequestAsyncReadback_7(src, mipIndex, x, width, y, height, z, depth, GraphicsFormatUtility.GetGraphicsFormat(dstFormat, QualitySettings.activeColorSpace == ColorSpace.Linear), callback, &asyncRequestNativeArrayData);
		}

		public unsafe void RequestAsyncReadbackIntoNativeSlice<T>(ref NativeSlice<T> output, Texture src, int mipIndex, int x, int width, int y, int height, int z, int depth, GraphicsFormat dstFormat, Action<AsyncGPUReadbackRequest> callback) where T : struct
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			AsyncRequestNativeArrayData asyncRequestNativeArrayData = AsyncRequestNativeArrayData.CreateAndCheckAccess<T>(output);
			this.Internal_RequestAsyncReadback_7(src, mipIndex, x, width, y, height, z, depth, dstFormat, callback, &asyncRequestNativeArrayData);
		}

		[NativeMethod("AddRequestAsyncReadback")]
		private unsafe void Internal_RequestAsyncReadback_1([NotNull] ComputeBuffer src, [NotNull] Action<AsyncGPUReadbackRequest> callback, AsyncRequestNativeArrayData* nativeArrayData = null)
		{
			if (src == null)
			{
				ThrowHelper.ThrowArgumentNullException(src, "src");
			}
			if (callback == null)
			{
				ThrowHelper.ThrowArgumentNullException(callback, "callback");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = ComputeBuffer.BindingsMarshaller.ConvertToNative(src);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(src, "src");
			}
			CommandBuffer.Internal_RequestAsyncReadback_1_Injected(intPtr, intPtr2, callback, nativeArrayData);
		}

		[NativeMethod("AddRequestAsyncReadback")]
		private unsafe void Internal_RequestAsyncReadback_2([NotNull] ComputeBuffer src, int size, int offset, [NotNull] Action<AsyncGPUReadbackRequest> callback, AsyncRequestNativeArrayData* nativeArrayData = null)
		{
			if (src == null)
			{
				ThrowHelper.ThrowArgumentNullException(src, "src");
			}
			if (callback == null)
			{
				ThrowHelper.ThrowArgumentNullException(callback, "callback");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = ComputeBuffer.BindingsMarshaller.ConvertToNative(src);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(src, "src");
			}
			CommandBuffer.Internal_RequestAsyncReadback_2_Injected(intPtr, intPtr2, size, offset, callback, nativeArrayData);
		}

		[NativeMethod("AddRequestAsyncReadback")]
		private unsafe void Internal_RequestAsyncReadback_3([NotNull] Texture src, [NotNull] Action<AsyncGPUReadbackRequest> callback, AsyncRequestNativeArrayData* nativeArrayData = null)
		{
			if (src == null)
			{
				ThrowHelper.ThrowArgumentNullException(src, "src");
			}
			if (callback == null)
			{
				ThrowHelper.ThrowArgumentNullException(callback, "callback");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Texture>(src);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(src, "src");
			}
			CommandBuffer.Internal_RequestAsyncReadback_3_Injected(intPtr, intPtr2, callback, nativeArrayData);
		}

		[NativeMethod("AddRequestAsyncReadback")]
		private unsafe void Internal_RequestAsyncReadback_4([NotNull] Texture src, int mipIndex, [NotNull] Action<AsyncGPUReadbackRequest> callback, AsyncRequestNativeArrayData* nativeArrayData = null)
		{
			if (src == null)
			{
				ThrowHelper.ThrowArgumentNullException(src, "src");
			}
			if (callback == null)
			{
				ThrowHelper.ThrowArgumentNullException(callback, "callback");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Texture>(src);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(src, "src");
			}
			CommandBuffer.Internal_RequestAsyncReadback_4_Injected(intPtr, intPtr2, mipIndex, callback, nativeArrayData);
		}

		[NativeMethod("AddRequestAsyncReadback")]
		private unsafe void Internal_RequestAsyncReadback_5([NotNull] Texture src, int mipIndex, GraphicsFormat dstFormat, [NotNull] Action<AsyncGPUReadbackRequest> callback, AsyncRequestNativeArrayData* nativeArrayData = null)
		{
			if (src == null)
			{
				ThrowHelper.ThrowArgumentNullException(src, "src");
			}
			if (callback == null)
			{
				ThrowHelper.ThrowArgumentNullException(callback, "callback");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Texture>(src);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(src, "src");
			}
			CommandBuffer.Internal_RequestAsyncReadback_5_Injected(intPtr, intPtr2, mipIndex, dstFormat, callback, nativeArrayData);
		}

		[NativeMethod("AddRequestAsyncReadback")]
		private unsafe void Internal_RequestAsyncReadback_6([NotNull] Texture src, int mipIndex, int x, int width, int y, int height, int z, int depth, [NotNull] Action<AsyncGPUReadbackRequest> callback, AsyncRequestNativeArrayData* nativeArrayData = null)
		{
			if (src == null)
			{
				ThrowHelper.ThrowArgumentNullException(src, "src");
			}
			if (callback == null)
			{
				ThrowHelper.ThrowArgumentNullException(callback, "callback");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Texture>(src);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(src, "src");
			}
			CommandBuffer.Internal_RequestAsyncReadback_6_Injected(intPtr, intPtr2, mipIndex, x, width, y, height, z, depth, callback, nativeArrayData);
		}

		[NativeMethod("AddRequestAsyncReadback")]
		private unsafe void Internal_RequestAsyncReadback_7([NotNull] Texture src, int mipIndex, int x, int width, int y, int height, int z, int depth, GraphicsFormat dstFormat, [NotNull] Action<AsyncGPUReadbackRequest> callback, AsyncRequestNativeArrayData* nativeArrayData = null)
		{
			if (src == null)
			{
				ThrowHelper.ThrowArgumentNullException(src, "src");
			}
			if (callback == null)
			{
				ThrowHelper.ThrowArgumentNullException(callback, "callback");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Texture>(src);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(src, "src");
			}
			CommandBuffer.Internal_RequestAsyncReadback_7_Injected(intPtr, intPtr2, mipIndex, x, width, y, height, z, depth, dstFormat, callback, nativeArrayData);
		}

		[NativeMethod("AddRequestAsyncReadback")]
		private unsafe void Internal_RequestAsyncReadback_8([NotNull] GraphicsBuffer src, [NotNull] Action<AsyncGPUReadbackRequest> callback, AsyncRequestNativeArrayData* nativeArrayData = null)
		{
			if (src == null)
			{
				ThrowHelper.ThrowArgumentNullException(src, "src");
			}
			if (callback == null)
			{
				ThrowHelper.ThrowArgumentNullException(callback, "callback");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = GraphicsBuffer.BindingsMarshaller.ConvertToNative(src);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(src, "src");
			}
			CommandBuffer.Internal_RequestAsyncReadback_8_Injected(intPtr, intPtr2, callback, nativeArrayData);
		}

		[NativeMethod("AddRequestAsyncReadback")]
		private unsafe void Internal_RequestAsyncReadback_9([NotNull] GraphicsBuffer src, int size, int offset, [NotNull] Action<AsyncGPUReadbackRequest> callback, AsyncRequestNativeArrayData* nativeArrayData = null)
		{
			if (src == null)
			{
				ThrowHelper.ThrowArgumentNullException(src, "src");
			}
			if (callback == null)
			{
				ThrowHelper.ThrowArgumentNullException(callback, "callback");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = GraphicsBuffer.BindingsMarshaller.ConvertToNative(src);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(src, "src");
			}
			CommandBuffer.Internal_RequestAsyncReadback_9_Injected(intPtr, intPtr2, size, offset, callback, nativeArrayData);
		}

		[NativeMethod("AddSetInvertCulling")]
		public void SetInvertCulling(bool invertCulling)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.SetInvertCulling_Injected(intPtr, invertCulling);
		}

		private void ConvertTexture_Internal(RenderTargetIdentifier src, int srcElement, RenderTargetIdentifier dst, int dstElement)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.ConvertTexture_Internal_Injected(intPtr, ref src, srcElement, ref dst, dstElement);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::Internal_SetSinglePassStereo", HasExplicitThis = true)]
		private void Internal_SetSinglePassStereo(SinglePassStereoMode mode)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.Internal_SetSinglePassStereo_Injected(intPtr, mode);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::InitBuffer")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr InitBuffer();

		[FreeFunction("RenderingCommandBuffer_Bindings::CreateGPUFence_Internal", HasExplicitThis = true)]
		private IntPtr CreateGPUFence_Internal(GraphicsFenceType fenceType, SynchronisationStageFlags stage)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return CommandBuffer.CreateGPUFence_Internal_Injected(intPtr, fenceType, stage);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::WaitOnGPUFence_Internal", HasExplicitThis = true)]
		private void WaitOnGPUFence_Internal(IntPtr fencePtr, SynchronisationStageFlags stage)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.WaitOnGPUFence_Internal_Injected(intPtr, fencePtr, stage);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::ReleaseBuffer", HasExplicitThis = true, IsThreadSafe = true)]
		private void ReleaseBuffer()
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.ReleaseBuffer_Injected(intPtr);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetComputeFloatParam", HasExplicitThis = true)]
		public void SetComputeFloatParam([NotNull] ComputeShader computeShader, int nameID, float val)
		{
			if (computeShader == null)
			{
				ThrowHelper.ThrowArgumentNullException(computeShader, "computeShader");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(computeShader);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(computeShader, "computeShader");
			}
			CommandBuffer.SetComputeFloatParam_Injected(intPtr, intPtr2, nameID, val);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetComputeIntParam", HasExplicitThis = true)]
		public void SetComputeIntParam([NotNull] ComputeShader computeShader, int nameID, int val)
		{
			if (computeShader == null)
			{
				ThrowHelper.ThrowArgumentNullException(computeShader, "computeShader");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(computeShader);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(computeShader, "computeShader");
			}
			CommandBuffer.SetComputeIntParam_Injected(intPtr, intPtr2, nameID, val);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetComputeVectorParam", HasExplicitThis = true)]
		public void SetComputeVectorParam([NotNull] ComputeShader computeShader, int nameID, Vector4 val)
		{
			if (computeShader == null)
			{
				ThrowHelper.ThrowArgumentNullException(computeShader, "computeShader");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(computeShader);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(computeShader, "computeShader");
			}
			CommandBuffer.SetComputeVectorParam_Injected(intPtr, intPtr2, nameID, ref val);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetComputeVectorArrayParam", HasExplicitThis = true)]
		public unsafe void SetComputeVectorArrayParam([NotNull] ComputeShader computeShader, int nameID, Vector4[] values)
		{
			if (computeShader == null)
			{
				ThrowHelper.ThrowArgumentNullException(computeShader, "computeShader");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(computeShader);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(computeShader, "computeShader");
			}
			Span<Vector4> span = new Span<Vector4>(values);
			fixed (Vector4* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				CommandBuffer.SetComputeVectorArrayParam_Injected(intPtr, intPtr2, nameID, ref managedSpanWrapper);
			}
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetComputeMatrixParam", HasExplicitThis = true)]
		public void SetComputeMatrixParam([NotNull] ComputeShader computeShader, int nameID, Matrix4x4 val)
		{
			if (computeShader == null)
			{
				ThrowHelper.ThrowArgumentNullException(computeShader, "computeShader");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(computeShader);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(computeShader, "computeShader");
			}
			CommandBuffer.SetComputeMatrixParam_Injected(intPtr, intPtr2, nameID, ref val);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetComputeMatrixArrayParam", HasExplicitThis = true)]
		public unsafe void SetComputeMatrixArrayParam([NotNull] ComputeShader computeShader, int nameID, Matrix4x4[] values)
		{
			if (computeShader == null)
			{
				ThrowHelper.ThrowArgumentNullException(computeShader, "computeShader");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(computeShader);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(computeShader, "computeShader");
			}
			Span<Matrix4x4> span = new Span<Matrix4x4>(values);
			fixed (Matrix4x4* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				CommandBuffer.SetComputeMatrixArrayParam_Injected(intPtr, intPtr2, nameID, ref managedSpanWrapper);
			}
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::Internal_SetComputeFloats", HasExplicitThis = true)]
		private unsafe void Internal_SetComputeFloats([NotNull] ComputeShader computeShader, int nameID, float[] values)
		{
			if (computeShader == null)
			{
				ThrowHelper.ThrowArgumentNullException(computeShader, "computeShader");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(computeShader);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(computeShader, "computeShader");
			}
			Span<float> span = new Span<float>(values);
			fixed (float* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				CommandBuffer.Internal_SetComputeFloats_Injected(intPtr, intPtr2, nameID, ref managedSpanWrapper);
			}
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::Internal_SetComputeInts", HasExplicitThis = true)]
		private unsafe void Internal_SetComputeInts([NotNull] ComputeShader computeShader, int nameID, int[] values)
		{
			if (computeShader == null)
			{
				ThrowHelper.ThrowArgumentNullException(computeShader, "computeShader");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(computeShader);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(computeShader, "computeShader");
			}
			Span<int> span = new Span<int>(values);
			fixed (int* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				CommandBuffer.Internal_SetComputeInts_Injected(intPtr, intPtr2, nameID, ref managedSpanWrapper);
			}
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::Internal_SetComputeTextureParam", HasExplicitThis = true)]
		private void Internal_SetComputeTextureParam([NotNull] ComputeShader computeShader, int kernelIndex, int nameID, ref RenderTargetIdentifier rt, int mipLevel, RenderTextureSubElement element)
		{
			if (computeShader == null)
			{
				ThrowHelper.ThrowArgumentNullException(computeShader, "computeShader");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(computeShader);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(computeShader, "computeShader");
			}
			CommandBuffer.Internal_SetComputeTextureParam_Injected(intPtr, intPtr2, kernelIndex, nameID, ref rt, mipLevel, element);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetComputeBufferParam", HasExplicitThis = true)]
		private void Internal_SetComputeBufferParam([NotNull] ComputeShader computeShader, int kernelIndex, int nameID, ComputeBuffer buffer)
		{
			if (computeShader == null)
			{
				ThrowHelper.ThrowArgumentNullException(computeShader, "computeShader");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(computeShader);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(computeShader, "computeShader");
			}
			CommandBuffer.Internal_SetComputeBufferParam_Injected(intPtr, intPtr2, kernelIndex, nameID, (buffer == null) ? ((IntPtr)0) : ComputeBuffer.BindingsMarshaller.ConvertToNative(buffer));
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetComputeBufferParam", HasExplicitThis = true)]
		private void Internal_SetComputeGraphicsBufferHandleParam([NotNull] ComputeShader computeShader, int kernelIndex, int nameID, GraphicsBufferHandle bufferHandle)
		{
			if (computeShader == null)
			{
				ThrowHelper.ThrowArgumentNullException(computeShader, "computeShader");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(computeShader);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(computeShader, "computeShader");
			}
			CommandBuffer.Internal_SetComputeGraphicsBufferHandleParam_Injected(intPtr, intPtr2, kernelIndex, nameID, ref bufferHandle);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetComputeBufferParam", HasExplicitThis = true)]
		private void Internal_SetComputeGraphicsBufferParam([NotNull] ComputeShader computeShader, int kernelIndex, int nameID, GraphicsBuffer buffer)
		{
			if (computeShader == null)
			{
				ThrowHelper.ThrowArgumentNullException(computeShader, "computeShader");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(computeShader);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(computeShader, "computeShader");
			}
			CommandBuffer.Internal_SetComputeGraphicsBufferParam_Injected(intPtr, intPtr2, kernelIndex, nameID, (buffer == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(buffer));
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetComputeConstantBufferParam", HasExplicitThis = true)]
		private void Internal_SetComputeConstantComputeBufferParam([NotNull] ComputeShader computeShader, int nameID, ComputeBuffer buffer, int offset, int size)
		{
			if (computeShader == null)
			{
				ThrowHelper.ThrowArgumentNullException(computeShader, "computeShader");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(computeShader);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(computeShader, "computeShader");
			}
			CommandBuffer.Internal_SetComputeConstantComputeBufferParam_Injected(intPtr, intPtr2, nameID, (buffer == null) ? ((IntPtr)0) : ComputeBuffer.BindingsMarshaller.ConvertToNative(buffer), offset, size);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetComputeConstantBufferParam", HasExplicitThis = true)]
		private void Internal_SetComputeConstantGraphicsBufferParam([NotNull] ComputeShader computeShader, int nameID, GraphicsBuffer buffer, int offset, int size)
		{
			if (computeShader == null)
			{
				ThrowHelper.ThrowArgumentNullException(computeShader, "computeShader");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(computeShader);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(computeShader, "computeShader");
			}
			CommandBuffer.Internal_SetComputeConstantGraphicsBufferParam_Injected(intPtr, intPtr2, nameID, (buffer == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(buffer), offset, size);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetComputeParamsFromMaterial", HasExplicitThis = true)]
		private void Internal_SetComputeParamsFromMaterial([NotNull] ComputeShader computeShader, int kernelIndex, Material material)
		{
			if (computeShader == null)
			{
				ThrowHelper.ThrowArgumentNullException(computeShader, "computeShader");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(computeShader);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(computeShader, "computeShader");
			}
			CommandBuffer.Internal_SetComputeParamsFromMaterial_Injected(intPtr, intPtr2, kernelIndex, Object.MarshalledUnityObject.Marshal<Material>(material));
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::Internal_DispatchCompute", HasExplicitThis = true, ThrowsException = true)]
		private void Internal_DispatchCompute([NotNull] ComputeShader computeShader, int kernelIndex, int threadGroupsX, int threadGroupsY, int threadGroupsZ)
		{
			if (computeShader == null)
			{
				ThrowHelper.ThrowArgumentNullException(computeShader, "computeShader");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(computeShader);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(computeShader, "computeShader");
			}
			CommandBuffer.Internal_DispatchCompute_Injected(intPtr, intPtr2, kernelIndex, threadGroupsX, threadGroupsY, threadGroupsZ);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::Internal_DispatchComputeIndirect", HasExplicitThis = true, ThrowsException = true)]
		private void Internal_DispatchComputeIndirect([NotNull] ComputeShader computeShader, int kernelIndex, ComputeBuffer indirectBuffer, uint argsOffset)
		{
			if (computeShader == null)
			{
				ThrowHelper.ThrowArgumentNullException(computeShader, "computeShader");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(computeShader);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(computeShader, "computeShader");
			}
			CommandBuffer.Internal_DispatchComputeIndirect_Injected(intPtr, intPtr2, kernelIndex, (indirectBuffer == null) ? ((IntPtr)0) : ComputeBuffer.BindingsMarshaller.ConvertToNative(indirectBuffer), argsOffset);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::Internal_DispatchComputeIndirect", HasExplicitThis = true, ThrowsException = true)]
		private void Internal_DispatchComputeIndirectGraphicsBuffer([NotNull] ComputeShader computeShader, int kernelIndex, GraphicsBuffer indirectBuffer, uint argsOffset)
		{
			if (computeShader == null)
			{
				ThrowHelper.ThrowArgumentNullException(computeShader, "computeShader");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(computeShader);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(computeShader, "computeShader");
			}
			CommandBuffer.Internal_DispatchComputeIndirectGraphicsBuffer_Injected(intPtr, intPtr2, kernelIndex, (indirectBuffer == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(indirectBuffer), argsOffset);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::Internal_SetRayTracingBufferParam", HasExplicitThis = true)]
		private void Internal_SetRayTracingComputeBufferParam([NotNull] RayTracingShader rayTracingShader, int nameID, ComputeBuffer buffer)
		{
			if (rayTracingShader == null)
			{
				ThrowHelper.ThrowArgumentNullException(rayTracingShader, "rayTracingShader");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<RayTracingShader>(rayTracingShader);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(rayTracingShader, "rayTracingShader");
			}
			CommandBuffer.Internal_SetRayTracingComputeBufferParam_Injected(intPtr, intPtr2, nameID, (buffer == null) ? ((IntPtr)0) : ComputeBuffer.BindingsMarshaller.ConvertToNative(buffer));
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::Internal_SetRayTracingBufferParam", HasExplicitThis = true)]
		private void Internal_SetRayTracingGraphicsBufferParam([NotNull] RayTracingShader rayTracingShader, int nameID, GraphicsBuffer buffer)
		{
			if (rayTracingShader == null)
			{
				ThrowHelper.ThrowArgumentNullException(rayTracingShader, "rayTracingShader");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<RayTracingShader>(rayTracingShader);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(rayTracingShader, "rayTracingShader");
			}
			CommandBuffer.Internal_SetRayTracingGraphicsBufferParam_Injected(intPtr, intPtr2, nameID, (buffer == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(buffer));
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::Internal_SetRayTracingBufferParam", HasExplicitThis = true)]
		private void Internal_SetRayTracingGraphicsBufferHandleParam([NotNull] RayTracingShader rayTracingShader, int nameID, GraphicsBufferHandle bufferHandle)
		{
			if (rayTracingShader == null)
			{
				ThrowHelper.ThrowArgumentNullException(rayTracingShader, "rayTracingShader");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<RayTracingShader>(rayTracingShader);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(rayTracingShader, "rayTracingShader");
			}
			CommandBuffer.Internal_SetRayTracingGraphicsBufferHandleParam_Injected(intPtr, intPtr2, nameID, ref bufferHandle);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::Internal_SetRayTracingConstantBufferParam", HasExplicitThis = true)]
		private void Internal_SetRayTracingConstantComputeBufferParam([NotNull] RayTracingShader rayTracingShader, int nameID, ComputeBuffer buffer, int offset, int size)
		{
			if (rayTracingShader == null)
			{
				ThrowHelper.ThrowArgumentNullException(rayTracingShader, "rayTracingShader");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<RayTracingShader>(rayTracingShader);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(rayTracingShader, "rayTracingShader");
			}
			CommandBuffer.Internal_SetRayTracingConstantComputeBufferParam_Injected(intPtr, intPtr2, nameID, (buffer == null) ? ((IntPtr)0) : ComputeBuffer.BindingsMarshaller.ConvertToNative(buffer), offset, size);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::Internal_SetRayTracingConstantBufferParam", HasExplicitThis = true)]
		private void Internal_SetRayTracingConstantGraphicsBufferParam([NotNull] RayTracingShader rayTracingShader, int nameID, GraphicsBuffer buffer, int offset, int size)
		{
			if (rayTracingShader == null)
			{
				ThrowHelper.ThrowArgumentNullException(rayTracingShader, "rayTracingShader");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<RayTracingShader>(rayTracingShader);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(rayTracingShader, "rayTracingShader");
			}
			CommandBuffer.Internal_SetRayTracingConstantGraphicsBufferParam_Injected(intPtr, intPtr2, nameID, (buffer == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(buffer), offset, size);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::Internal_SetRayTracingTextureParam", HasExplicitThis = true)]
		private void Internal_SetRayTracingTextureParam([NotNull] RayTracingShader rayTracingShader, int nameID, ref RenderTargetIdentifier rt)
		{
			if (rayTracingShader == null)
			{
				ThrowHelper.ThrowArgumentNullException(rayTracingShader, "rayTracingShader");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<RayTracingShader>(rayTracingShader);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(rayTracingShader, "rayTracingShader");
			}
			CommandBuffer.Internal_SetRayTracingTextureParam_Injected(intPtr, intPtr2, nameID, ref rt);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::Internal_SetRayTracingFloatParam", HasExplicitThis = true)]
		private void Internal_SetRayTracingFloatParam([NotNull] RayTracingShader rayTracingShader, int nameID, float val)
		{
			if (rayTracingShader == null)
			{
				ThrowHelper.ThrowArgumentNullException(rayTracingShader, "rayTracingShader");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<RayTracingShader>(rayTracingShader);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(rayTracingShader, "rayTracingShader");
			}
			CommandBuffer.Internal_SetRayTracingFloatParam_Injected(intPtr, intPtr2, nameID, val);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::Internal_SetRayTracingIntParam", HasExplicitThis = true)]
		private void Internal_SetRayTracingIntParam([NotNull] RayTracingShader rayTracingShader, int nameID, int val)
		{
			if (rayTracingShader == null)
			{
				ThrowHelper.ThrowArgumentNullException(rayTracingShader, "rayTracingShader");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<RayTracingShader>(rayTracingShader);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(rayTracingShader, "rayTracingShader");
			}
			CommandBuffer.Internal_SetRayTracingIntParam_Injected(intPtr, intPtr2, nameID, val);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::Internal_SetRayTracingVectorParam", HasExplicitThis = true)]
		private void Internal_SetRayTracingVectorParam([NotNull] RayTracingShader rayTracingShader, int nameID, Vector4 val)
		{
			if (rayTracingShader == null)
			{
				ThrowHelper.ThrowArgumentNullException(rayTracingShader, "rayTracingShader");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<RayTracingShader>(rayTracingShader);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(rayTracingShader, "rayTracingShader");
			}
			CommandBuffer.Internal_SetRayTracingVectorParam_Injected(intPtr, intPtr2, nameID, ref val);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::Internal_SetRayTracingVectorArrayParam", HasExplicitThis = true)]
		private unsafe void Internal_SetRayTracingVectorArrayParam([NotNull] RayTracingShader rayTracingShader, int nameID, Vector4[] values)
		{
			if (rayTracingShader == null)
			{
				ThrowHelper.ThrowArgumentNullException(rayTracingShader, "rayTracingShader");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<RayTracingShader>(rayTracingShader);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(rayTracingShader, "rayTracingShader");
			}
			Span<Vector4> span = new Span<Vector4>(values);
			fixed (Vector4* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				CommandBuffer.Internal_SetRayTracingVectorArrayParam_Injected(intPtr, intPtr2, nameID, ref managedSpanWrapper);
			}
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::Internal_SetRayTracingMatrixParam", HasExplicitThis = true)]
		private void Internal_SetRayTracingMatrixParam([NotNull] RayTracingShader rayTracingShader, int nameID, Matrix4x4 val)
		{
			if (rayTracingShader == null)
			{
				ThrowHelper.ThrowArgumentNullException(rayTracingShader, "rayTracingShader");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<RayTracingShader>(rayTracingShader);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(rayTracingShader, "rayTracingShader");
			}
			CommandBuffer.Internal_SetRayTracingMatrixParam_Injected(intPtr, intPtr2, nameID, ref val);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::Internal_SetRayTracingMatrixArrayParam", HasExplicitThis = true)]
		private unsafe void Internal_SetRayTracingMatrixArrayParam([NotNull] RayTracingShader rayTracingShader, int nameID, Matrix4x4[] values)
		{
			if (rayTracingShader == null)
			{
				ThrowHelper.ThrowArgumentNullException(rayTracingShader, "rayTracingShader");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<RayTracingShader>(rayTracingShader);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(rayTracingShader, "rayTracingShader");
			}
			Span<Matrix4x4> span = new Span<Matrix4x4>(values);
			fixed (Matrix4x4* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				CommandBuffer.Internal_SetRayTracingMatrixArrayParam_Injected(intPtr, intPtr2, nameID, ref managedSpanWrapper);
			}
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::Internal_SetRayTracingFloats", HasExplicitThis = true)]
		private unsafe void Internal_SetRayTracingFloats([NotNull] RayTracingShader rayTracingShader, int nameID, float[] values)
		{
			if (rayTracingShader == null)
			{
				ThrowHelper.ThrowArgumentNullException(rayTracingShader, "rayTracingShader");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<RayTracingShader>(rayTracingShader);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(rayTracingShader, "rayTracingShader");
			}
			Span<float> span = new Span<float>(values);
			fixed (float* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				CommandBuffer.Internal_SetRayTracingFloats_Injected(intPtr, intPtr2, nameID, ref managedSpanWrapper);
			}
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::Internal_SetRayTracingInts", HasExplicitThis = true)]
		private unsafe void Internal_SetRayTracingInts([NotNull] RayTracingShader rayTracingShader, int nameID, int[] values)
		{
			if (rayTracingShader == null)
			{
				ThrowHelper.ThrowArgumentNullException(rayTracingShader, "rayTracingShader");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<RayTracingShader>(rayTracingShader);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(rayTracingShader, "rayTracingShader");
			}
			Span<int> span = new Span<int>(values);
			fixed (int* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				CommandBuffer.Internal_SetRayTracingInts_Injected(intPtr, intPtr2, nameID, ref managedSpanWrapper);
			}
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::Internal_BuildRayTracingAccelerationStructure", HasExplicitThis = true)]
		private void Internal_BuildRayTracingAccelerationStructure([NotNull] RayTracingAccelerationStructure accelerationStructure, RayTracingAccelerationStructure.BuildSettings buildSettings)
		{
			if (accelerationStructure == null)
			{
				ThrowHelper.ThrowArgumentNullException(accelerationStructure, "accelerationStructure");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = RayTracingAccelerationStructure.BindingsMarshaller.ConvertToNative(accelerationStructure);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(accelerationStructure, "accelerationStructure");
			}
			CommandBuffer.Internal_BuildRayTracingAccelerationStructure_Injected(intPtr, intPtr2, ref buildSettings);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::Internal_SetRayTracingAccelerationStructure", HasExplicitThis = true)]
		private void Internal_SetRayTracingAccelerationStructure([NotNull] RayTracingShader rayTracingShader, int nameID, [NotNull] RayTracingAccelerationStructure accelerationStructure)
		{
			if (rayTracingShader == null)
			{
				ThrowHelper.ThrowArgumentNullException(rayTracingShader, "rayTracingShader");
			}
			if (accelerationStructure == null)
			{
				ThrowHelper.ThrowArgumentNullException(accelerationStructure, "accelerationStructure");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<RayTracingShader>(rayTracingShader);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(rayTracingShader, "rayTracingShader");
			}
			IntPtr intPtr3 = RayTracingAccelerationStructure.BindingsMarshaller.ConvertToNative(accelerationStructure);
			if (intPtr3 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(accelerationStructure, "accelerationStructure");
			}
			CommandBuffer.Internal_SetRayTracingAccelerationStructure_Injected(intPtr, intPtr2, nameID, intPtr3);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::Internal_SetComputeRayTracingAccelerationStructure", HasExplicitThis = true)]
		private void Internal_SetComputeRayTracingAccelerationStructure([NotNull] ComputeShader computeShader, int kernelIndex, int nameID, [NotNull] RayTracingAccelerationStructure accelerationStructure)
		{
			if (computeShader == null)
			{
				ThrowHelper.ThrowArgumentNullException(computeShader, "computeShader");
			}
			if (accelerationStructure == null)
			{
				ThrowHelper.ThrowArgumentNullException(accelerationStructure, "accelerationStructure");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<ComputeShader>(computeShader);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(computeShader, "computeShader");
			}
			IntPtr intPtr3 = RayTracingAccelerationStructure.BindingsMarshaller.ConvertToNative(accelerationStructure);
			if (intPtr3 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(accelerationStructure, "accelerationStructure");
			}
			CommandBuffer.Internal_SetComputeRayTracingAccelerationStructure_Injected(intPtr, intPtr2, kernelIndex, nameID, intPtr3);
		}

		[NativeMethod("AddSetRayTracingShaderPass")]
		public unsafe void SetRayTracingShaderPass([NotNull] RayTracingShader rayTracingShader, string passName)
		{
			if (rayTracingShader == null)
			{
				ThrowHelper.ThrowArgumentNullException(rayTracingShader, "rayTracingShader");
			}
			try
			{
				IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<RayTracingShader>(rayTracingShader);
				if (intPtr2 == 0)
				{
					ThrowHelper.ThrowArgumentNullException(rayTracingShader, "rayTracingShader");
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(passName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = passName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				CommandBuffer.SetRayTracingShaderPass_Injected(intPtr, intPtr2, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::Internal_DispatchRays", HasExplicitThis = true, ThrowsException = true)]
		private unsafe void Internal_DispatchRays([NotNull] RayTracingShader rayTracingShader, string rayGenShaderName, uint width, uint height, uint depth, Camera camera = null)
		{
			if (rayTracingShader == null)
			{
				ThrowHelper.ThrowArgumentNullException(rayTracingShader, "rayTracingShader");
			}
			try
			{
				IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<RayTracingShader>(rayTracingShader);
				if (intPtr2 == 0)
				{
					ThrowHelper.ThrowArgumentNullException(rayTracingShader, "rayTracingShader");
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(rayGenShaderName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = rayGenShaderName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				CommandBuffer.Internal_DispatchRays_Injected(intPtr, intPtr2, ref managedSpanWrapper, width, height, depth, Object.MarshalledUnityObject.Marshal<Camera>(camera));
			}
			finally
			{
				char* ptr = null;
			}
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::Internal_DispatchRaysIndirect", HasExplicitThis = true, ThrowsException = true)]
		private unsafe void Internal_DispatchRaysIndirect([NotNull] RayTracingShader rayTracingShader, string rayGenShaderName, [NotNull] GraphicsBuffer argsBuffer, uint argsOffset = 0U, Camera camera = null)
		{
			if (rayTracingShader == null)
			{
				ThrowHelper.ThrowArgumentNullException(rayTracingShader, "rayTracingShader");
			}
			if (argsBuffer == null)
			{
				ThrowHelper.ThrowArgumentNullException(argsBuffer, "argsBuffer");
			}
			try
			{
				IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<RayTracingShader>(rayTracingShader);
				if (intPtr2 == 0)
				{
					ThrowHelper.ThrowArgumentNullException(rayTracingShader, "rayTracingShader");
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(rayGenShaderName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = rayGenShaderName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				IntPtr intPtr3 = GraphicsBuffer.BindingsMarshaller.ConvertToNative(argsBuffer);
				if (intPtr3 == 0)
				{
					ThrowHelper.ThrowArgumentNullException(argsBuffer, "argsBuffer");
				}
				CommandBuffer.Internal_DispatchRaysIndirect_Injected(intPtr, intPtr2, ref managedSpanWrapper, intPtr3, argsOffset, Object.MarshalledUnityObject.Marshal<Camera>(camera));
			}
			finally
			{
				char* ptr = null;
			}
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::Internal_BuildMachineLearningOperator", HasExplicitThis = true)]
		private void Internal_BuildMachineLearningOperator(IntPtr machineLearningOperator)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.Internal_BuildMachineLearningOperator_Injected(intPtr, machineLearningOperator);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::Internal_SetMachineLearningOperatorTensors", HasExplicitThis = true)]
		private unsafe void Internal_SetMachineLearningOperatorTensors(IntPtr machineLearningOperator, ReadOnlySpan<IntPtr> inputs, ReadOnlySpan<IntPtr> outputs)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ReadOnlySpan<IntPtr> readOnlySpan = inputs;
			fixed (IntPtr* ptr = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
				ReadOnlySpan<IntPtr> readOnlySpan2 = outputs;
				fixed (IntPtr* pinnableReference = readOnlySpan2.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper2 = new ManagedSpanWrapper((void*)pinnableReference, readOnlySpan2.Length);
					CommandBuffer.Internal_SetMachineLearningOperatorTensors_Injected(intPtr, machineLearningOperator, ref managedSpanWrapper, ref managedSpanWrapper2);
					ptr = null;
				}
			}
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::Internal_DispatchMachineLearningOperator", HasExplicitThis = true)]
		private void Internal_DispatchMachineLearningOperator(IntPtr machineLearningOperator)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.Internal_DispatchMachineLearningOperator_Injected(intPtr, machineLearningOperator);
		}

		[NativeMethod("AddGenerateMips")]
		private void Internal_GenerateMips(RenderTargetIdentifier rt)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.Internal_GenerateMips_Injected(intPtr, ref rt);
		}

		[NativeMethod("AddResolveAntiAliasedSurface")]
		private void Internal_ResolveAntiAliasedSurface(RenderTexture rt, RenderTexture target)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.Internal_ResolveAntiAliasedSurface_Injected(intPtr, Object.MarshalledUnityObject.Marshal<RenderTexture>(rt), Object.MarshalledUnityObject.Marshal<RenderTexture>(target));
		}

		[NativeMethod("AddCopyCounterValue")]
		private void CopyCounterValueCC(ComputeBuffer src, ComputeBuffer dst, uint dstOffsetBytes)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.CopyCounterValueCC_Injected(intPtr, (src == null) ? ((IntPtr)0) : ComputeBuffer.BindingsMarshaller.ConvertToNative(src), (dst == null) ? ((IntPtr)0) : ComputeBuffer.BindingsMarshaller.ConvertToNative(dst), dstOffsetBytes);
		}

		[NativeMethod("AddCopyCounterValue")]
		private void CopyCounterValueGC(GraphicsBuffer src, ComputeBuffer dst, uint dstOffsetBytes)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.CopyCounterValueGC_Injected(intPtr, (src == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(src), (dst == null) ? ((IntPtr)0) : ComputeBuffer.BindingsMarshaller.ConvertToNative(dst), dstOffsetBytes);
		}

		[NativeMethod("AddCopyCounterValue")]
		private void CopyCounterValueCG(ComputeBuffer src, GraphicsBuffer dst, uint dstOffsetBytes)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.CopyCounterValueCG_Injected(intPtr, (src == null) ? ((IntPtr)0) : ComputeBuffer.BindingsMarshaller.ConvertToNative(src), (dst == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(dst), dstOffsetBytes);
		}

		[NativeMethod("AddCopyCounterValue")]
		private void CopyCounterValueGG(GraphicsBuffer src, GraphicsBuffer dst, uint dstOffsetBytes)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.CopyCounterValueGG_Injected(intPtr, (src == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(src), (dst == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(dst), dstOffsetBytes);
		}

		public unsafe string name
		{
			get
			{
				string stringAndDispose;
				try
				{
					IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					ManagedSpanWrapper managedSpanWrapper;
					CommandBuffer.get_name_Injected(intPtr, out managedSpanWrapper);
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
					IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
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
					CommandBuffer.set_name_Injected(intPtr, ref managedSpanWrapper);
				}
				finally
				{
					char* ptr = null;
				}
			}
		}

		public int sizeInBytes
		{
			[NativeMethod("GetBufferSize")]
			get
			{
				IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return CommandBuffer.get_sizeInBytes_Injected(intPtr);
			}
		}

		[NativeMethod("ClearCommands")]
		public void Clear()
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.Clear_Injected(intPtr);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::Internal_DrawMesh", HasExplicitThis = true)]
		private void Internal_DrawMesh([NotNull] Mesh mesh, Matrix4x4 matrix, Material material, int submeshIndex, int shaderPass, MaterialPropertyBlock properties)
		{
			if (mesh == null)
			{
				ThrowHelper.ThrowArgumentNullException(mesh, "mesh");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Mesh>(mesh);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(mesh, "mesh");
			}
			CommandBuffer.Internal_DrawMesh_Injected(intPtr, intPtr2, ref matrix, Object.MarshalledUnityObject.Marshal<Material>(material), submeshIndex, shaderPass, (properties == null) ? ((IntPtr)0) : MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(properties));
		}

		[NativeMethod("AddDrawMultipleMeshes")]
		private unsafe void Internal_DrawMultipleMeshes(Matrix4x4[] matrices, Mesh[] meshes, int[] subsetIndices, int count, Material material, int shaderPass, MaterialPropertyBlock properties)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<Matrix4x4> span = new Span<Matrix4x4>(matrices);
			fixed (Matrix4x4* ptr = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, span.Length);
				Span<int> span2 = new Span<int>(subsetIndices);
				fixed (int* pinnableReference = span2.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper2 = new ManagedSpanWrapper((void*)pinnableReference, span2.Length);
					CommandBuffer.Internal_DrawMultipleMeshes_Injected(intPtr, ref managedSpanWrapper, meshes, ref managedSpanWrapper2, count, Object.MarshalledUnityObject.Marshal<Material>(material), shaderPass, (properties == null) ? ((IntPtr)0) : MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(properties));
					ptr = null;
				}
			}
		}

		[NativeMethod("AddDrawRenderer")]
		private void Internal_DrawRenderer([NotNull] Renderer renderer, Material material, int submeshIndex, int shaderPass)
		{
			if (renderer == null)
			{
				ThrowHelper.ThrowArgumentNullException(renderer, "renderer");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Renderer>(renderer);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(renderer, "renderer");
			}
			CommandBuffer.Internal_DrawRenderer_Injected(intPtr, intPtr2, Object.MarshalledUnityObject.Marshal<Material>(material), submeshIndex, shaderPass);
		}

		[NativeMethod("AddDrawRendererList")]
		private void Internal_DrawRendererList(RendererList rendererList)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.Internal_DrawRendererList_Injected(intPtr, ref rendererList);
		}

		private void Internal_DrawRenderer(Renderer renderer, Material material, int submeshIndex)
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.Internal_DrawRenderer(renderer, material, submeshIndex, -1);
		}

		private void Internal_DrawRenderer(Renderer renderer, Material material)
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.Internal_DrawRenderer(renderer, material, 0);
		}

		[NativeMethod("AddDrawProcedural")]
		private void Internal_DrawProcedural(Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, int vertexCount, int instanceCount, MaterialPropertyBlock properties)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.Internal_DrawProcedural_Injected(intPtr, ref matrix, Object.MarshalledUnityObject.Marshal<Material>(material), shaderPass, topology, vertexCount, instanceCount, (properties == null) ? ((IntPtr)0) : MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(properties));
		}

		[NativeMethod("AddDrawProceduralIndexed")]
		private void Internal_DrawProceduralIndexed(GraphicsBuffer indexBuffer, Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, int indexCount, int instanceCount, MaterialPropertyBlock properties)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.Internal_DrawProceduralIndexed_Injected(intPtr, (indexBuffer == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(indexBuffer), ref matrix, Object.MarshalledUnityObject.Marshal<Material>(material), shaderPass, topology, indexCount, instanceCount, (properties == null) ? ((IntPtr)0) : MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(properties));
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::Internal_DrawProceduralIndirect", HasExplicitThis = true)]
		private void Internal_DrawProceduralIndirect(Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.Internal_DrawProceduralIndirect_Injected(intPtr, ref matrix, Object.MarshalledUnityObject.Marshal<Material>(material), shaderPass, topology, (bufferWithArgs == null) ? ((IntPtr)0) : ComputeBuffer.BindingsMarshaller.ConvertToNative(bufferWithArgs), argsOffset, (properties == null) ? ((IntPtr)0) : MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(properties));
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::Internal_DrawProceduralIndexedIndirect", HasExplicitThis = true)]
		private void Internal_DrawProceduralIndexedIndirect(GraphicsBuffer indexBuffer, Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.Internal_DrawProceduralIndexedIndirect_Injected(intPtr, (indexBuffer == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(indexBuffer), ref matrix, Object.MarshalledUnityObject.Marshal<Material>(material), shaderPass, topology, (bufferWithArgs == null) ? ((IntPtr)0) : ComputeBuffer.BindingsMarshaller.ConvertToNative(bufferWithArgs), argsOffset, (properties == null) ? ((IntPtr)0) : MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(properties));
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::Internal_DrawProceduralIndirect", HasExplicitThis = true)]
		private void Internal_DrawProceduralIndirectGraphicsBuffer(Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, GraphicsBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.Internal_DrawProceduralIndirectGraphicsBuffer_Injected(intPtr, ref matrix, Object.MarshalledUnityObject.Marshal<Material>(material), shaderPass, topology, (bufferWithArgs == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(bufferWithArgs), argsOffset, (properties == null) ? ((IntPtr)0) : MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(properties));
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::Internal_DrawProceduralIndexedIndirect", HasExplicitThis = true)]
		private void Internal_DrawProceduralIndexedIndirectGraphicsBuffer(GraphicsBuffer indexBuffer, Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, GraphicsBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.Internal_DrawProceduralIndexedIndirectGraphicsBuffer_Injected(intPtr, (indexBuffer == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(indexBuffer), ref matrix, Object.MarshalledUnityObject.Marshal<Material>(material), shaderPass, topology, (bufferWithArgs == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(bufferWithArgs), argsOffset, (properties == null) ? ((IntPtr)0) : MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(properties));
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::Internal_DrawMeshInstanced", HasExplicitThis = true)]
		private unsafe void Internal_DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, int shaderPass, Matrix4x4[] matrices, int count, MaterialPropertyBlock properties)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.Marshal<Mesh>(mesh);
			IntPtr intPtr3 = Object.MarshalledUnityObject.Marshal<Material>(material);
			Span<Matrix4x4> span = new Span<Matrix4x4>(matrices);
			fixed (Matrix4x4* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				CommandBuffer.Internal_DrawMeshInstanced_Injected(intPtr, intPtr2, submeshIndex, intPtr3, shaderPass, ref managedSpanWrapper, count, (properties == null) ? ((IntPtr)0) : MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(properties));
			}
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::Internal_DrawMeshInstancedProcedural", HasExplicitThis = true)]
		private void Internal_DrawMeshInstancedProcedural(Mesh mesh, int submeshIndex, Material material, int shaderPass, int count, MaterialPropertyBlock properties)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.Internal_DrawMeshInstancedProcedural_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Mesh>(mesh), submeshIndex, Object.MarshalledUnityObject.Marshal<Material>(material), shaderPass, count, (properties == null) ? ((IntPtr)0) : MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(properties));
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::Internal_DrawMeshInstancedIndirect", HasExplicitThis = true)]
		private void Internal_DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, int shaderPass, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.Internal_DrawMeshInstancedIndirect_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Mesh>(mesh), submeshIndex, Object.MarshalledUnityObject.Marshal<Material>(material), shaderPass, (bufferWithArgs == null) ? ((IntPtr)0) : ComputeBuffer.BindingsMarshaller.ConvertToNative(bufferWithArgs), argsOffset, (properties == null) ? ((IntPtr)0) : MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(properties));
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::Internal_DrawMeshInstancedIndirect", HasExplicitThis = true)]
		private void Internal_DrawMeshInstancedIndirectGraphicsBuffer(Mesh mesh, int submeshIndex, Material material, int shaderPass, GraphicsBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.Internal_DrawMeshInstancedIndirectGraphicsBuffer_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Mesh>(mesh), submeshIndex, Object.MarshalledUnityObject.Marshal<Material>(material), shaderPass, (bufferWithArgs == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(bufferWithArgs), argsOffset, (properties == null) ? ((IntPtr)0) : MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(properties));
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::Internal_DrawOcclusionMesh", HasExplicitThis = true)]
		private void Internal_DrawOcclusionMesh(RectInt normalizedCamViewport)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.Internal_DrawOcclusionMesh_Injected(intPtr, ref normalizedCamViewport);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetRandomWriteTarget_Texture", HasExplicitThis = true, ThrowsException = true)]
		private void SetRandomWriteTarget_Texture(int index, ref RenderTargetIdentifier rt)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.SetRandomWriteTarget_Texture_Injected(intPtr, index, ref rt);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetRandomWriteTarget_Buffer", HasExplicitThis = true, ThrowsException = true)]
		private void SetRandomWriteTarget_Buffer(int index, ComputeBuffer uav, bool preserveCounterValue)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.SetRandomWriteTarget_Buffer_Injected(intPtr, index, (uav == null) ? ((IntPtr)0) : ComputeBuffer.BindingsMarshaller.ConvertToNative(uav), preserveCounterValue);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetRandomWriteTarget_Buffer", HasExplicitThis = true, ThrowsException = true)]
		private void SetRandomWriteTarget_GraphicsBuffer(int index, GraphicsBuffer uav, bool preserveCounterValue)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.SetRandomWriteTarget_GraphicsBuffer_Injected(intPtr, index, (uav == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(uav), preserveCounterValue);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::ClearRandomWriteTargets", HasExplicitThis = true, ThrowsException = true)]
		public void ClearRandomWriteTargets()
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.ClearRandomWriteTargets_Injected(intPtr);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetViewport", HasExplicitThis = true, ThrowsException = true)]
		public void SetViewport(Rect pixelRect)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.SetViewport_Injected(intPtr, ref pixelRect);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::EnableScissorRect", HasExplicitThis = true, ThrowsException = true)]
		public void EnableScissorRect(Rect scissor)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.EnableScissorRect_Injected(intPtr, ref scissor);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::DisableScissorRect", HasExplicitThis = true, ThrowsException = true)]
		public void DisableScissorRect()
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.DisableScissorRect_Injected(intPtr);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::CopyTexture_Internal", HasExplicitThis = true)]
		private void CopyTexture_Internal(ref RenderTargetIdentifier src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, ref RenderTargetIdentifier dst, int dstElement, int dstMip, int dstX, int dstY, int mode)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.CopyTexture_Internal_Injected(intPtr, ref src, srcElement, srcMip, srcX, srcY, srcWidth, srcHeight, ref dst, dstElement, dstMip, dstX, dstY, mode);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::Blit_Texture", HasExplicitThis = true)]
		private void Blit_Texture(Texture source, ref RenderTargetIdentifier dest, Material mat, int pass, Vector2 scale, Vector2 offset, int sourceDepthSlice, int destDepthSlice)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.Blit_Texture_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Texture>(source), ref dest, Object.MarshalledUnityObject.Marshal<Material>(mat), pass, ref scale, ref offset, sourceDepthSlice, destDepthSlice);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::Blit_Identifier", HasExplicitThis = true)]
		private void Blit_Identifier(ref RenderTargetIdentifier source, ref RenderTargetIdentifier dest, Material mat, int pass, Vector2 scale, Vector2 offset, int sourceDepthSlice, int destDepthSlice)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.Blit_Identifier_Injected(intPtr, ref source, ref dest, Object.MarshalledUnityObject.Marshal<Material>(mat), pass, ref scale, ref offset, sourceDepthSlice, destDepthSlice);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::GetTemporaryRT", HasExplicitThis = true)]
		private void GetTemporaryRT(int nameID, int width, int height, FilterMode filter, GraphicsFormat colorFormat, GraphicsFormat depthStencilFormat, int antiAliasing, bool enableRandomWrite, RenderTextureMemoryless memorylessMode, bool useDynamicScale, ShadowSamplingMode shadowSamplingMode)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.GetTemporaryRT_Injected(intPtr, nameID, width, height, filter, colorFormat, depthStencilFormat, antiAliasing, enableRandomWrite, memorylessMode, useDynamicScale, shadowSamplingMode);
		}

		public void GetTemporaryRT(int nameID, int width, int height, int depthBuffer, FilterMode filter, GraphicsFormat format, int antiAliasing, bool enableRandomWrite, RenderTextureMemoryless memorylessMode, bool useDynamicScale)
		{
			GraphicsFormat depthStencilFormatLegacy = RenderTexture.GetDepthStencilFormatLegacy(depthBuffer, format);
			this.GetTemporaryRT(nameID, width, height, filter, format, depthStencilFormatLegacy, antiAliasing, enableRandomWrite, memorylessMode, useDynamicScale, ShadowSamplingMode.None);
		}

		public void GetTemporaryRT(int nameID, int width, int height, int depthBuffer, FilterMode filter, GraphicsFormat format, int antiAliasing, bool enableRandomWrite, RenderTextureMemoryless memorylessMode)
		{
			this.GetTemporaryRT(nameID, width, height, depthBuffer, filter, format, antiAliasing, enableRandomWrite, memorylessMode, false);
		}

		public void GetTemporaryRT(int nameID, int width, int height, int depthBuffer, FilterMode filter, GraphicsFormat format, int antiAliasing, bool enableRandomWrite)
		{
			this.GetTemporaryRT(nameID, width, height, depthBuffer, filter, format, antiAliasing, enableRandomWrite, RenderTextureMemoryless.None);
		}

		public void GetTemporaryRT(int nameID, int width, int height, int depthBuffer, FilterMode filter, GraphicsFormat format, int antiAliasing)
		{
			this.GetTemporaryRT(nameID, width, height, depthBuffer, filter, format, antiAliasing, false, RenderTextureMemoryless.None);
		}

		public void GetTemporaryRT(int nameID, int width, int height, int depthBuffer, FilterMode filter, GraphicsFormat format)
		{
			this.GetTemporaryRT(nameID, width, height, depthBuffer, filter, format, 1);
		}

		public void GetTemporaryRT(int nameID, int width, int height, int depthBuffer, FilterMode filter, RenderTextureFormat format, RenderTextureReadWrite readWrite, int antiAliasing, bool enableRandomWrite, RenderTextureMemoryless memorylessMode, bool useDynamicScale)
		{
			ShadowSamplingMode shadowSamplingModeForFormat = RenderTexture.GetShadowSamplingModeForFormat(format);
			GraphicsFormat graphicsFormat = GraphicsFormatUtility.GetGraphicsFormat(format, readWrite);
			GraphicsFormat depthStencilFormatLegacy = RenderTexture.GetDepthStencilFormatLegacy(depthBuffer, format, false);
			this.GetTemporaryRT(nameID, width, height, filter, graphicsFormat, depthStencilFormatLegacy, antiAliasing, enableRandomWrite, memorylessMode, useDynamicScale, shadowSamplingModeForFormat);
		}

		public void GetTemporaryRT(int nameID, int width, int height, int depthBuffer, FilterMode filter, RenderTextureFormat format, RenderTextureReadWrite readWrite, int antiAliasing, bool enableRandomWrite, RenderTextureMemoryless memorylessMode)
		{
			this.GetTemporaryRT(nameID, width, height, depthBuffer, filter, format, readWrite, antiAliasing, enableRandomWrite, memorylessMode, false);
		}

		public void GetTemporaryRT(int nameID, int width, int height, int depthBuffer, FilterMode filter, RenderTextureFormat format, RenderTextureReadWrite readWrite, int antiAliasing, bool enableRandomWrite)
		{
			this.GetTemporaryRT(nameID, width, height, depthBuffer, filter, format, readWrite, antiAliasing, enableRandomWrite, RenderTextureMemoryless.None);
		}

		public void GetTemporaryRT(int nameID, int width, int height, int depthBuffer, FilterMode filter, RenderTextureFormat format, RenderTextureReadWrite readWrite, int antiAliasing)
		{
			this.GetTemporaryRT(nameID, width, height, depthBuffer, filter, format, readWrite, antiAliasing, false);
		}

		public void GetTemporaryRT(int nameID, int width, int height, int depthBuffer, FilterMode filter, RenderTextureFormat format, RenderTextureReadWrite readWrite)
		{
			this.GetTemporaryRT(nameID, width, height, depthBuffer, filter, format, readWrite, 1);
		}

		public void GetTemporaryRT(int nameID, int width, int height, int depthBuffer, FilterMode filter, RenderTextureFormat format)
		{
			this.GetTemporaryRT(nameID, width, height, depthBuffer, filter, format, RenderTextureReadWrite.Default);
		}

		public void GetTemporaryRT(int nameID, int width, int height, int depthBuffer, FilterMode filter)
		{
			this.GetTemporaryRT(nameID, width, height, depthBuffer, filter, RenderTextureFormat.Default);
		}

		public void GetTemporaryRT(int nameID, int width, int height, int depthBuffer)
		{
			this.GetTemporaryRT(nameID, width, height, depthBuffer, FilterMode.Point);
		}

		public void GetTemporaryRT(int nameID, int width, int height)
		{
			this.GetTemporaryRT(nameID, width, height, 0);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::GetTemporaryRTWithDescriptor", HasExplicitThis = true)]
		private void GetTemporaryRTWithDescriptor(int nameID, RenderTextureDescriptor desc, FilterMode filter)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.GetTemporaryRTWithDescriptor_Injected(intPtr, nameID, ref desc, filter);
		}

		public void GetTemporaryRT(int nameID, RenderTextureDescriptor desc, FilterMode filter)
		{
			this.GetTemporaryRTWithDescriptor(nameID, desc, filter);
		}

		public void GetTemporaryRT(int nameID, RenderTextureDescriptor desc)
		{
			this.GetTemporaryRT(nameID, desc, FilterMode.Point);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::GetTemporaryRTArray", HasExplicitThis = true)]
		private void GetTemporaryRTArray(int nameID, int width, int height, int slices, FilterMode filter, GraphicsFormat colorFormat, GraphicsFormat depthStencilFormat, int antiAliasing, bool enableRandomWrite, bool useDynamicScale, ShadowSamplingMode shadowSamplingMode)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.GetTemporaryRTArray_Injected(intPtr, nameID, width, height, slices, filter, colorFormat, depthStencilFormat, antiAliasing, enableRandomWrite, useDynamicScale, shadowSamplingMode);
		}

		public void GetTemporaryRTArray(int nameID, int width, int height, int slices, int depthBuffer, FilterMode filter, GraphicsFormat format, int antiAliasing, bool enableRandomWrite, bool useDynamicScale)
		{
			GraphicsFormat depthStencilFormatLegacy = RenderTexture.GetDepthStencilFormatLegacy(depthBuffer, format);
			this.GetTemporaryRTArray(nameID, width, height, slices, filter, format, depthStencilFormatLegacy, antiAliasing, enableRandomWrite, useDynamicScale, ShadowSamplingMode.None);
		}

		public void GetTemporaryRTArray(int nameID, int width, int height, int slices, int depthBuffer, FilterMode filter, GraphicsFormat format, int antiAliasing, bool enableRandomWrite)
		{
			this.GetTemporaryRTArray(nameID, width, height, slices, depthBuffer, filter, format, antiAliasing, enableRandomWrite, false);
		}

		public void GetTemporaryRTArray(int nameID, int width, int height, int slices, int depthBuffer, FilterMode filter, GraphicsFormat format, int antiAliasing)
		{
			this.GetTemporaryRTArray(nameID, width, height, slices, depthBuffer, filter, format, antiAliasing, false);
		}

		public void GetTemporaryRTArray(int nameID, int width, int height, int slices, int depthBuffer, FilterMode filter, GraphicsFormat format)
		{
			this.GetTemporaryRTArray(nameID, width, height, slices, depthBuffer, filter, format, 1);
		}

		public void GetTemporaryRTArray(int nameID, int width, int height, int slices, int depthBuffer, FilterMode filter, RenderTextureFormat format, RenderTextureReadWrite readWrite, int antiAliasing, bool enableRandomWrite)
		{
			ShadowSamplingMode shadowSamplingModeForFormat = RenderTexture.GetShadowSamplingModeForFormat(format);
			GraphicsFormat graphicsFormat = GraphicsFormatUtility.GetGraphicsFormat(format, readWrite);
			GraphicsFormat depthStencilFormatLegacy = RenderTexture.GetDepthStencilFormatLegacy(depthBuffer, format, false);
			this.GetTemporaryRTArray(nameID, width, height, slices, filter, graphicsFormat, depthStencilFormatLegacy, antiAliasing, enableRandomWrite, false, shadowSamplingModeForFormat);
		}

		public void GetTemporaryRTArray(int nameID, int width, int height, int slices, int depthBuffer, FilterMode filter, RenderTextureFormat format, RenderTextureReadWrite readWrite, int antiAliasing)
		{
			this.GetTemporaryRTArray(nameID, width, height, slices, depthBuffer, filter, format, readWrite, antiAliasing, false);
		}

		public void GetTemporaryRTArray(int nameID, int width, int height, int slices, int depthBuffer, FilterMode filter, RenderTextureFormat format, RenderTextureReadWrite readWrite)
		{
			this.GetTemporaryRTArray(nameID, width, height, slices, depthBuffer, filter, format, readWrite, 1);
		}

		public void GetTemporaryRTArray(int nameID, int width, int height, int slices, int depthBuffer, FilterMode filter, RenderTextureFormat format)
		{
			this.GetTemporaryRTArray(nameID, width, height, slices, depthBuffer, filter, format, RenderTextureReadWrite.Default);
		}

		public void GetTemporaryRTArray(int nameID, int width, int height, int slices, int depthBuffer, FilterMode filter)
		{
			this.GetTemporaryRTArray(nameID, width, height, slices, depthBuffer, filter, RenderTextureFormat.Default);
		}

		public void GetTemporaryRTArray(int nameID, int width, int height, int slices, int depthBuffer)
		{
			this.GetTemporaryRTArray(nameID, width, height, slices, depthBuffer, FilterMode.Point);
		}

		public void GetTemporaryRTArray(int nameID, int width, int height, int slices)
		{
			this.GetTemporaryRTArray(nameID, width, height, slices, 0);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::ReleaseTemporaryRT", HasExplicitThis = true)]
		public void ReleaseTemporaryRT(int nameID)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.ReleaseTemporaryRT_Injected(intPtr, nameID);
		}

		public void ClearRenderTarget(bool clearDepth, bool clearColor, Color backgroundColor)
		{
			this.ClearRenderTarget(clearDepth, clearColor, backgroundColor, 1f, 0U);
		}

		public void ClearRenderTarget(bool clearDepth, bool clearColor, Color backgroundColor, float depth)
		{
			this.ClearRenderTarget(clearDepth, clearColor, backgroundColor, depth, 0U);
		}

		public void ClearRenderTarget(bool clearDepth, bool clearColor, Color backgroundColor, float depth = 1f, uint stencil = 0U)
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.ClearRenderTargetSingle_Internal((clearColor ? RTClearFlags.Color : RTClearFlags.None) | (clearDepth ? RTClearFlags.DepthStencil : RTClearFlags.None), backgroundColor, depth, stencil);
		}

		public void ClearRenderTarget(RTClearFlags clearFlags, Color backgroundColor, float depth = 1f, uint stencil = 0U)
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.ClearRenderTargetSingle_Internal(clearFlags, backgroundColor, depth, stencil);
		}

		public void ClearRenderTarget(RTClearFlags clearFlags, Color[] backgroundColors, float depth = 1f, uint stencil = 0U)
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			bool flag = backgroundColors.Length < 1;
			if (flag)
			{
				throw new ArgumentException(string.Format("The number of clear colors must be at least 1, but is {0}", backgroundColors.Length));
			}
			bool flag2 = backgroundColors.Length > SystemInfo.supportedRenderTargetCount;
			if (flag2)
			{
				throw new ArgumentException(string.Format("The number of clear colors ({0}) exceeds the maximum supported number of render targets ({1})", backgroundColors.Length, SystemInfo.supportedRenderTargetCount));
			}
			this.ClearRenderTargetMulti_Internal(clearFlags, backgroundColors, depth, stencil);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetGlobalFloat", HasExplicitThis = true)]
		public void SetGlobalFloat(int nameID, float value)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.SetGlobalFloat_Injected(intPtr, nameID, value);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetGlobalInt", HasExplicitThis = true)]
		public void SetGlobalInt(int nameID, int value)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.SetGlobalInt_Injected(intPtr, nameID, value);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetGlobalInteger", HasExplicitThis = true)]
		public void SetGlobalInteger(int nameID, int value)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.SetGlobalInteger_Injected(intPtr, nameID, value);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetGlobalVector", HasExplicitThis = true)]
		public void SetGlobalVector(int nameID, Vector4 value)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.SetGlobalVector_Injected(intPtr, nameID, ref value);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetGlobalColor", HasExplicitThis = true)]
		public void SetGlobalColor(int nameID, Color value)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.SetGlobalColor_Injected(intPtr, nameID, ref value);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetGlobalMatrix", HasExplicitThis = true)]
		public void SetGlobalMatrix(int nameID, Matrix4x4 value)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.SetGlobalMatrix_Injected(intPtr, nameID, ref value);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::EnableShaderKeyword", HasExplicitThis = true)]
		public unsafe void EnableShaderKeyword(string keyword)
		{
			try
			{
				IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(keyword, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = keyword.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				CommandBuffer.EnableShaderKeyword_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::EnableShaderKeyword", HasExplicitThis = true)]
		private void EnableGlobalKeyword(GlobalKeyword keyword)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.EnableGlobalKeyword_Injected(intPtr, ref keyword);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::EnableMaterialKeyword", HasExplicitThis = true)]
		private void EnableMaterialKeyword(Material material, LocalKeyword keyword)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.EnableMaterialKeyword_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Material>(material), ref keyword);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::EnableComputeKeyword", HasExplicitThis = true)]
		private void EnableComputeKeyword(ComputeShader computeShader, LocalKeyword keyword)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.EnableComputeKeyword_Injected(intPtr, Object.MarshalledUnityObject.Marshal<ComputeShader>(computeShader), ref keyword);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::EnableRayTracingKeyword", HasExplicitThis = true)]
		private void EnableRayTracingKeyword(RayTracingShader rayTracingShader, LocalKeyword keyword)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.EnableRayTracingKeyword_Injected(intPtr, Object.MarshalledUnityObject.Marshal<RayTracingShader>(rayTracingShader), ref keyword);
		}

		public void EnableKeyword(in GlobalKeyword keyword)
		{
			this.EnableGlobalKeyword(keyword);
		}

		public void EnableKeyword(Material material, in LocalKeyword keyword)
		{
			this.EnableMaterialKeyword(material, keyword);
		}

		public void EnableKeyword(ComputeShader computeShader, in LocalKeyword keyword)
		{
			this.EnableComputeKeyword(computeShader, keyword);
		}

		public void EnableKeyword(RayTracingShader rayTracingShader, in LocalKeyword keyword)
		{
			this.EnableRayTracingKeyword(rayTracingShader, keyword);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::DisableShaderKeyword", HasExplicitThis = true)]
		public unsafe void DisableShaderKeyword(string keyword)
		{
			try
			{
				IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(keyword, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = keyword.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				CommandBuffer.DisableShaderKeyword_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::DisableShaderKeyword", HasExplicitThis = true)]
		private void DisableGlobalKeyword(GlobalKeyword keyword)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.DisableGlobalKeyword_Injected(intPtr, ref keyword);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::DisableMaterialKeyword", HasExplicitThis = true)]
		private void DisableMaterialKeyword(Material material, LocalKeyword keyword)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.DisableMaterialKeyword_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Material>(material), ref keyword);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::DisableComputeKeyword", HasExplicitThis = true)]
		private void DisableComputeKeyword(ComputeShader computeShader, LocalKeyword keyword)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.DisableComputeKeyword_Injected(intPtr, Object.MarshalledUnityObject.Marshal<ComputeShader>(computeShader), ref keyword);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::DisableRayTracingKeyword", HasExplicitThis = true)]
		private void DisableRayTracingKeyword(RayTracingShader rayTracingShader, LocalKeyword keyword)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.DisableRayTracingKeyword_Injected(intPtr, Object.MarshalledUnityObject.Marshal<RayTracingShader>(rayTracingShader), ref keyword);
		}

		public void DisableKeyword(in GlobalKeyword keyword)
		{
			this.DisableGlobalKeyword(keyword);
		}

		public void DisableKeyword(Material material, in LocalKeyword keyword)
		{
			this.DisableMaterialKeyword(material, keyword);
		}

		public void DisableKeyword(ComputeShader computeShader, in LocalKeyword keyword)
		{
			this.DisableComputeKeyword(computeShader, keyword);
		}

		public void DisableKeyword(RayTracingShader rayTracingShader, in LocalKeyword keyword)
		{
			this.DisableRayTracingKeyword(rayTracingShader, keyword);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetShaderKeyword", HasExplicitThis = true)]
		private void SetGlobalKeyword(GlobalKeyword keyword, bool value)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.SetGlobalKeyword_Injected(intPtr, ref keyword, value);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetMaterialKeyword", HasExplicitThis = true)]
		private void SetMaterialKeyword(Material material, LocalKeyword keyword, bool value)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.SetMaterialKeyword_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Material>(material), ref keyword, value);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetComputeKeyword", HasExplicitThis = true)]
		private void SetComputeKeyword(ComputeShader computeShader, LocalKeyword keyword, bool value)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.SetComputeKeyword_Injected(intPtr, Object.MarshalledUnityObject.Marshal<ComputeShader>(computeShader), ref keyword, value);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetRayTracingKeyword", HasExplicitThis = true)]
		private void SetRayTracingKeyword(RayTracingShader rayTracingShader, LocalKeyword keyword, bool value)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.SetRayTracingKeyword_Injected(intPtr, Object.MarshalledUnityObject.Marshal<RayTracingShader>(rayTracingShader), ref keyword, value);
		}

		public void SetKeyword(in GlobalKeyword keyword, bool value)
		{
			this.SetGlobalKeyword(keyword, value);
		}

		public void SetKeyword(Material material, in LocalKeyword keyword, bool value)
		{
			this.SetMaterialKeyword(material, keyword, value);
		}

		public void SetKeyword(ComputeShader computeShader, in LocalKeyword keyword, bool value)
		{
			this.SetComputeKeyword(computeShader, keyword, value);
		}

		public void SetKeyword(RayTracingShader rayTracingShader, in LocalKeyword keyword, bool value)
		{
			this.SetRayTracingKeyword(rayTracingShader, keyword, value);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetViewMatrix", HasExplicitThis = true, ThrowsException = true)]
		public void SetViewMatrix(Matrix4x4 view)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.SetViewMatrix_Injected(intPtr, ref view);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetProjectionMatrix", HasExplicitThis = true, ThrowsException = true)]
		public void SetProjectionMatrix(Matrix4x4 proj)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.SetProjectionMatrix_Injected(intPtr, ref proj);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetViewProjectionMatrices", HasExplicitThis = true, ThrowsException = true)]
		public void SetViewProjectionMatrices(Matrix4x4 view, Matrix4x4 proj)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.SetViewProjectionMatrices_Injected(intPtr, ref view, ref proj);
		}

		[NativeMethod("AddSetGlobalDepthBias")]
		public void SetGlobalDepthBias(float bias, float slopeBias)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.SetGlobalDepthBias_Injected(intPtr, bias, slopeBias);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetExecutionFlags", HasExplicitThis = true, ThrowsException = true)]
		public void SetExecutionFlags(CommandBufferExecutionFlags flags)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.SetExecutionFlags_Injected(intPtr, flags);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::ValidateAgainstExecutionFlags", HasExplicitThis = true, ThrowsException = true)]
		private bool ValidateAgainstExecutionFlags(CommandBufferExecutionFlags requiredFlags, CommandBufferExecutionFlags invalidFlags)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return CommandBuffer.ValidateAgainstExecutionFlags_Injected(intPtr, requiredFlags, invalidFlags);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetGlobalFloatArrayListImpl", HasExplicitThis = true)]
		private void SetGlobalFloatArrayListImpl(int nameID, object values)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.SetGlobalFloatArrayListImpl_Injected(intPtr, nameID, values);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetGlobalVectorArrayListImpl", HasExplicitThis = true)]
		private void SetGlobalVectorArrayListImpl(int nameID, object values)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.SetGlobalVectorArrayListImpl_Injected(intPtr, nameID, values);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetGlobalMatrixArrayListImpl", HasExplicitThis = true)]
		private void SetGlobalMatrixArrayListImpl(int nameID, object values)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.SetGlobalMatrixArrayListImpl_Injected(intPtr, nameID, values);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetGlobalFloatArray", HasExplicitThis = true, ThrowsException = true)]
		public unsafe void SetGlobalFloatArray(int nameID, [NotNull] float[] values)
		{
			if (values == null)
			{
				ThrowHelper.ThrowArgumentNullException(values, "values");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<float> span = new Span<float>(values);
			fixed (float* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				CommandBuffer.SetGlobalFloatArray_Injected(intPtr, nameID, ref managedSpanWrapper);
			}
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetGlobalVectorArray", HasExplicitThis = true, ThrowsException = true)]
		public unsafe void SetGlobalVectorArray(int nameID, [NotNull] Vector4[] values)
		{
			if (values == null)
			{
				ThrowHelper.ThrowArgumentNullException(values, "values");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<Vector4> span = new Span<Vector4>(values);
			fixed (Vector4* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				CommandBuffer.SetGlobalVectorArray_Injected(intPtr, nameID, ref managedSpanWrapper);
			}
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetGlobalMatrixArray", HasExplicitThis = true, ThrowsException = true)]
		public unsafe void SetGlobalMatrixArray(int nameID, [NotNull] Matrix4x4[] values)
		{
			if (values == null)
			{
				ThrowHelper.ThrowArgumentNullException(values, "values");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<Matrix4x4> span = new Span<Matrix4x4>(values);
			fixed (Matrix4x4* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				CommandBuffer.SetGlobalMatrixArray_Injected(intPtr, nameID, ref managedSpanWrapper);
			}
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetLateLatchProjectionMatrices", HasExplicitThis = true, ThrowsException = true)]
		public unsafe void SetLateLatchProjectionMatrices([NotNull] Matrix4x4[] projectionMat)
		{
			if (projectionMat == null)
			{
				ThrowHelper.ThrowArgumentNullException(projectionMat, "projectionMat");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<Matrix4x4> span = new Span<Matrix4x4>(projectionMat);
			fixed (Matrix4x4* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				CommandBuffer.SetLateLatchProjectionMatrices_Injected(intPtr, ref managedSpanWrapper);
			}
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::MarkLateLatchMatrixShaderPropertyID", HasExplicitThis = true)]
		public void MarkLateLatchMatrixShaderPropertyID(CameraLateLatchMatrixType matrixPropertyType, int shaderPropertyID)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.MarkLateLatchMatrixShaderPropertyID_Injected(intPtr, matrixPropertyType, shaderPropertyID);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::UnmarkLateLatchMatrix", HasExplicitThis = true)]
		public void UnmarkLateLatchMatrix(CameraLateLatchMatrixType matrixPropertyType)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.UnmarkLateLatchMatrix_Injected(intPtr, matrixPropertyType);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetGlobalTexture_Impl", HasExplicitThis = true)]
		private void SetGlobalTexture_Impl(int nameID, ref RenderTargetIdentifier rt, RenderTextureSubElement element)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.SetGlobalTexture_Impl_Injected(intPtr, nameID, ref rt, element);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetGlobalBuffer", HasExplicitThis = true)]
		private void SetGlobalBufferInternal(int nameID, ComputeBuffer value)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.SetGlobalBufferInternal_Injected(intPtr, nameID, (value == null) ? ((IntPtr)0) : ComputeBuffer.BindingsMarshaller.ConvertToNative(value));
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetGlobalBuffer", HasExplicitThis = true)]
		private void SetGlobalGraphicsBufferInternal(int nameID, GraphicsBuffer value)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.SetGlobalGraphicsBufferInternal_Injected(intPtr, nameID, (value == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(value));
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetGlobalRayTracingAccelerationStructure", HasExplicitThis = true)]
		private void SetGlobalRayTracingAccelerationStructureInternal(RayTracingAccelerationStructure accelerationStructure, int nameID)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.SetGlobalRayTracingAccelerationStructureInternal_Injected(intPtr, (accelerationStructure == null) ? ((IntPtr)0) : RayTracingAccelerationStructure.BindingsMarshaller.ConvertToNative(accelerationStructure), nameID);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetShadowSamplingMode_Impl", HasExplicitThis = true)]
		private void SetShadowSamplingMode_Impl(ref RenderTargetIdentifier shadowmap, ShadowSamplingMode mode)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.SetShadowSamplingMode_Impl_Injected(intPtr, ref shadowmap, mode);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::IssuePluginEventInternal", HasExplicitThis = true)]
		private void IssuePluginEventInternal(IntPtr callback, int eventID)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.IssuePluginEventInternal_Injected(intPtr, callback, eventID);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::BeginSample", HasExplicitThis = true)]
		public unsafe void BeginSample(string name)
		{
			try
			{
				IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				CommandBuffer.BeginSample_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::EndSample", HasExplicitThis = true)]
		public unsafe void EndSample(string name)
		{
			try
			{
				IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				CommandBuffer.EndSample_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		public void BeginSample(CustomSampler sampler)
		{
			this.BeginSample_CustomSampler(sampler);
		}

		public void EndSample(CustomSampler sampler)
		{
			this.EndSample_CustomSampler(sampler);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::BeginSample_CustomSampler", HasExplicitThis = true)]
		private void BeginSample_CustomSampler([NotNull] CustomSampler sampler)
		{
			if (sampler == null)
			{
				ThrowHelper.ThrowArgumentNullException(sampler, "sampler");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = CustomSampler.BindingsMarshaller.ConvertToNative(sampler);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(sampler, "sampler");
			}
			CommandBuffer.BeginSample_CustomSampler_Injected(intPtr, intPtr2);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::EndSample_CustomSampler", HasExplicitThis = true)]
		private void EndSample_CustomSampler([NotNull] CustomSampler sampler)
		{
			if (sampler == null)
			{
				ThrowHelper.ThrowArgumentNullException(sampler, "sampler");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = CustomSampler.BindingsMarshaller.ConvertToNative(sampler);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(sampler, "sampler");
			}
			CommandBuffer.EndSample_CustomSampler_Injected(intPtr, intPtr2);
		}

		[Conditional("ENABLE_PROFILER")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void BeginSample(ProfilerMarker marker)
		{
			this.BeginSample_ProfilerMarker(marker.Handle);
		}

		[Conditional("ENABLE_PROFILER")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EndSample(ProfilerMarker marker)
		{
			this.EndSample_ProfilerMarker(marker.Handle);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::BeginSample_ProfilerMarker", HasExplicitThis = true, ThrowsException = true)]
		private void BeginSample_ProfilerMarker(IntPtr markerHandle)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.BeginSample_ProfilerMarker_Injected(intPtr, markerHandle);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::EndSample_ProfilerMarker", HasExplicitThis = true, ThrowsException = true)]
		private void EndSample_ProfilerMarker(IntPtr markerHandle)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.EndSample_ProfilerMarker_Injected(intPtr, markerHandle);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::IssuePluginEventAndDataInternal", HasExplicitThis = true)]
		private void IssuePluginEventAndDataInternal(IntPtr callback, int eventID, IntPtr data)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.IssuePluginEventAndDataInternal_Injected(intPtr, callback, eventID, data);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::IssuePluginEventAndDataWithFlagsInternal", HasExplicitThis = true)]
		private void IssuePluginEventAndDataInternalWithFlags(IntPtr callback, int eventID, CustomMarkerCallbackFlags flags, IntPtr data)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.IssuePluginEventAndDataInternalWithFlags_Injected(intPtr, callback, eventID, flags, data);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::IssuePluginCustomBlitInternal", HasExplicitThis = true)]
		private void IssuePluginCustomBlitInternal(IntPtr callback, uint command, ref RenderTargetIdentifier source, ref RenderTargetIdentifier dest, uint commandParam, uint commandFlags)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.IssuePluginCustomBlitInternal_Injected(intPtr, callback, command, ref source, ref dest, commandParam, commandFlags);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::IssuePluginCustomTextureUpdateInternal", HasExplicitThis = true)]
		private void IssuePluginCustomTextureUpdateInternal(IntPtr callback, Texture targetTexture, uint userData, bool useNewUnityRenderingExtTextureUpdateParamsV2)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.IssuePluginCustomTextureUpdateInternal_Injected(intPtr, callback, Object.MarshalledUnityObject.Marshal<Texture>(targetTexture), userData, useNewUnityRenderingExtTextureUpdateParamsV2);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetGlobalConstantBuffer", HasExplicitThis = true)]
		private void SetGlobalConstantBufferInternal(ComputeBuffer buffer, int nameID, int offset, int size)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.SetGlobalConstantBufferInternal_Injected(intPtr, (buffer == null) ? ((IntPtr)0) : ComputeBuffer.BindingsMarshaller.ConvertToNative(buffer), nameID, offset, size);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetGlobalConstantBuffer", HasExplicitThis = true)]
		private void SetGlobalConstantGraphicsBufferInternal(GraphicsBuffer buffer, int nameID, int offset, int size)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.SetGlobalConstantGraphicsBufferInternal_Injected(intPtr, (buffer == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(buffer), nameID, offset, size);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::IncrementUpdateCount", HasExplicitThis = true)]
		public void IncrementUpdateCount(RenderTargetIdentifier dest)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.IncrementUpdateCount_Injected(intPtr, ref dest);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetInstanceMultiplier", HasExplicitThis = true)]
		public void SetInstanceMultiplier(uint multiplier)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.SetInstanceMultiplier_Injected(intPtr, multiplier);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetFoveatedRenderingMode", HasExplicitThis = true)]
		public void SetFoveatedRenderingMode(FoveatedRenderingMode foveatedRenderingMode)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.SetFoveatedRenderingMode_Injected(intPtr, foveatedRenderingMode);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetWireframe", HasExplicitThis = true)]
		public void SetWireframe(bool enable)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.SetWireframe_Injected(intPtr, enable);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::ConfigureFoveatedRendering", HasExplicitThis = true)]
		public void ConfigureFoveatedRendering(IntPtr platformData)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.ConfigureFoveatedRendering_Injected(intPtr, platformData);
		}

		private static void CheckThrowOnSetRenderTarget()
		{
			bool throwOnSetRenderTarget = CommandBuffer.ThrowOnSetRenderTarget;
			if (throwOnSetRenderTarget)
			{
				throw new Exception("Setrendertarget is not allowed in this context");
			}
		}

		public void SetRenderTarget(RenderTargetIdentifier rt)
		{
			CommandBuffer.CheckThrowOnSetRenderTarget();
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.SetRenderTargetSingle_Internal(rt, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store);
		}

		public void SetRenderTarget(RenderTargetIdentifier rt, RenderBufferLoadAction loadAction, RenderBufferStoreAction storeAction)
		{
			CommandBuffer.CheckThrowOnSetRenderTarget();
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			bool flag = loadAction == RenderBufferLoadAction.Clear;
			if (flag)
			{
				throw new ArgumentException("RenderBufferLoadAction.Clear is not supported");
			}
			this.SetRenderTargetSingle_Internal(rt, loadAction, storeAction, loadAction, storeAction);
		}

		public void SetRenderTarget(RenderTargetIdentifier rt, RenderBufferLoadAction colorLoadAction, RenderBufferStoreAction colorStoreAction, RenderBufferLoadAction depthLoadAction, RenderBufferStoreAction depthStoreAction)
		{
			CommandBuffer.CheckThrowOnSetRenderTarget();
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			bool flag = colorLoadAction == RenderBufferLoadAction.Clear || depthLoadAction == RenderBufferLoadAction.Clear;
			if (flag)
			{
				throw new ArgumentException("RenderBufferLoadAction.Clear is not supported");
			}
			this.SetRenderTargetSingle_Internal(rt, colorLoadAction, colorStoreAction, depthLoadAction, depthStoreAction);
		}

		public void SetRenderTarget(RenderTargetIdentifier rt, int mipLevel)
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			bool flag = mipLevel < 0;
			if (flag)
			{
				throw new ArgumentException(string.Format("Invalid value for mipLevel ({0})", mipLevel));
			}
			this.SetRenderTargetSingle_Internal(new RenderTargetIdentifier(rt, mipLevel, CubemapFace.Unknown, 0), RenderBufferLoadAction.Load, RenderBufferStoreAction.Store, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store);
		}

		public void SetRenderTarget(RenderTargetIdentifier rt, int mipLevel, CubemapFace cubemapFace)
		{
			CommandBuffer.CheckThrowOnSetRenderTarget();
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			bool flag = mipLevel < 0;
			if (flag)
			{
				throw new ArgumentException(string.Format("Invalid value for mipLevel ({0})", mipLevel));
			}
			this.SetRenderTargetSingle_Internal(new RenderTargetIdentifier(rt, mipLevel, cubemapFace, 0), RenderBufferLoadAction.Load, RenderBufferStoreAction.Store, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store);
		}

		public void SetRenderTarget(RenderTargetIdentifier rt, int mipLevel, CubemapFace cubemapFace, int depthSlice)
		{
			CommandBuffer.CheckThrowOnSetRenderTarget();
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			bool flag = depthSlice < -1;
			if (flag)
			{
				throw new ArgumentException(string.Format("Invalid value for depthSlice ({0})", depthSlice));
			}
			bool flag2 = mipLevel < 0;
			if (flag2)
			{
				throw new ArgumentException(string.Format("Invalid value for mipLevel ({0})", mipLevel));
			}
			this.SetRenderTargetSingle_Internal(new RenderTargetIdentifier(rt, mipLevel, cubemapFace, depthSlice), RenderBufferLoadAction.Load, RenderBufferStoreAction.Store, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store);
		}

		public void SetRenderTarget(RenderTargetIdentifier color, RenderTargetIdentifier depth)
		{
			CommandBuffer.CheckThrowOnSetRenderTarget();
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.SetRenderTargetColorDepth_Internal(color, depth, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store, RenderTargetFlags.None);
		}

		public void SetRenderTarget(RenderTargetIdentifier color, RenderTargetIdentifier depth, int mipLevel)
		{
			CommandBuffer.CheckThrowOnSetRenderTarget();
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			bool flag = mipLevel < 0;
			if (flag)
			{
				throw new ArgumentException(string.Format("Invalid value for mipLevel ({0})", mipLevel));
			}
			this.SetRenderTargetColorDepth_Internal(new RenderTargetIdentifier(color, mipLevel, CubemapFace.Unknown, 0), depth, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store, RenderTargetFlags.None);
		}

		public void SetRenderTarget(RenderTargetIdentifier color, RenderTargetIdentifier depth, int mipLevel, CubemapFace cubemapFace)
		{
			CommandBuffer.CheckThrowOnSetRenderTarget();
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			bool flag = mipLevel < 0;
			if (flag)
			{
				throw new ArgumentException(string.Format("Invalid value for mipLevel ({0})", mipLevel));
			}
			this.SetRenderTargetColorDepth_Internal(new RenderTargetIdentifier(color, mipLevel, cubemapFace, 0), depth, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store, RenderTargetFlags.None);
		}

		public void SetRenderTarget(RenderTargetIdentifier color, RenderTargetIdentifier depth, int mipLevel, CubemapFace cubemapFace, int depthSlice)
		{
			CommandBuffer.CheckThrowOnSetRenderTarget();
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			bool flag = depthSlice < -1;
			if (flag)
			{
				throw new ArgumentException(string.Format("Invalid value for depthSlice ({0})", depthSlice));
			}
			bool flag2 = mipLevel < 0;
			if (flag2)
			{
				throw new ArgumentException(string.Format("Invalid value for mipLevel ({0})", mipLevel));
			}
			this.SetRenderTargetColorDepth_Internal(new RenderTargetIdentifier(color, mipLevel, cubemapFace, depthSlice), depth, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store, RenderTargetFlags.None);
		}

		public void SetRenderTarget(RenderTargetIdentifier color, RenderBufferLoadAction colorLoadAction, RenderBufferStoreAction colorStoreAction, RenderTargetIdentifier depth, RenderBufferLoadAction depthLoadAction, RenderBufferStoreAction depthStoreAction)
		{
			CommandBuffer.CheckThrowOnSetRenderTarget();
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			bool flag = colorLoadAction == RenderBufferLoadAction.Clear || depthLoadAction == RenderBufferLoadAction.Clear;
			if (flag)
			{
				throw new ArgumentException("RenderBufferLoadAction.Clear is not supported");
			}
			this.SetRenderTargetColorDepth_Internal(color, depth, colorLoadAction, colorStoreAction, depthLoadAction, depthStoreAction, RenderTargetFlags.None);
		}

		public void SetRenderTarget(RenderTargetIdentifier[] colors, RenderTargetIdentifier depth)
		{
			CommandBuffer.CheckThrowOnSetRenderTarget();
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			bool flag = colors.Length < 1;
			if (flag)
			{
				throw new ArgumentException(string.Format("colors.Length must be at least 1, but was {0}", colors.Length));
			}
			bool flag2 = colors.Length > SystemInfo.supportedRenderTargetCount;
			if (flag2)
			{
				throw new ArgumentException(string.Format("colors.Length is {0} and exceeds the maximum number of supported render targets ({1})", colors.Length, SystemInfo.supportedRenderTargetCount));
			}
			this.SetRenderTargetMulti_Internal(colors, depth, null, null, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store, RenderTargetFlags.None);
		}

		public void SetRenderTarget(RenderTargetIdentifier[] colors, RenderTargetIdentifier depth, int mipLevel, CubemapFace cubemapFace, int depthSlice)
		{
			CommandBuffer.CheckThrowOnSetRenderTarget();
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			bool flag = colors.Length < 1;
			if (flag)
			{
				throw new ArgumentException(string.Format("colors.Length must be at least 1, but was {0}", colors.Length));
			}
			bool flag2 = colors.Length > SystemInfo.supportedRenderTargetCount;
			if (flag2)
			{
				throw new ArgumentException(string.Format("colors.Length is {0} and exceeds the maximum number of supported render targets ({1})", colors.Length, SystemInfo.supportedRenderTargetCount));
			}
			this.SetRenderTargetMultiSubtarget(colors, depth, null, null, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store, mipLevel, cubemapFace, depthSlice);
		}

		public void SetRenderTarget(RenderTargetBinding binding, int mipLevel, CubemapFace cubemapFace, int depthSlice)
		{
			CommandBuffer.CheckThrowOnSetRenderTarget();
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			bool flag = binding.colorRenderTargets.Length < 1;
			if (flag)
			{
				throw new ArgumentException(string.Format("The number of color render targets must be at least 1, but was {0}", binding.colorRenderTargets.Length));
			}
			bool flag2 = binding.colorRenderTargets.Length > SystemInfo.supportedRenderTargetCount;
			if (flag2)
			{
				throw new ArgumentException(string.Format("The number of color render targets ({0}) and exceeds the maximum supported number of render targets ({1})", binding.colorRenderTargets.Length, SystemInfo.supportedRenderTargetCount));
			}
			bool flag3 = binding.colorLoadActions.Length != binding.colorRenderTargets.Length;
			if (flag3)
			{
				throw new ArgumentException(string.Format("The number of color load actions provided ({0}) does not match the number of color render targets ({1})", binding.colorLoadActions.Length, binding.colorRenderTargets.Length));
			}
			bool flag4 = binding.colorStoreActions.Length != binding.colorRenderTargets.Length;
			if (flag4)
			{
				throw new ArgumentException(string.Format("The number of color store actions provided ({0}) does not match the number of color render targets ({1})", binding.colorLoadActions.Length, binding.colorRenderTargets.Length));
			}
			bool flag5 = binding.depthLoadAction == RenderBufferLoadAction.Clear || Array.IndexOf<RenderBufferLoadAction>(binding.colorLoadActions, RenderBufferLoadAction.Clear) > -1;
			if (flag5)
			{
				throw new ArgumentException("RenderBufferLoadAction.Clear is not supported");
			}
			bool flag6 = binding.colorRenderTargets.Length == 1;
			if (flag6)
			{
				this.SetRenderTargetColorDepthSubtarget(binding.colorRenderTargets[0], binding.depthRenderTarget, binding.colorLoadActions[0], binding.colorStoreActions[0], binding.depthLoadAction, binding.depthStoreAction, mipLevel, cubemapFace, depthSlice);
			}
			else
			{
				this.SetRenderTargetMultiSubtarget(binding.colorRenderTargets, binding.depthRenderTarget, binding.colorLoadActions, binding.colorStoreActions, binding.depthLoadAction, binding.depthStoreAction, mipLevel, cubemapFace, depthSlice);
			}
		}

		public void SetRenderTarget(RenderTargetBinding binding)
		{
			CommandBuffer.CheckThrowOnSetRenderTarget();
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			bool flag = binding.colorRenderTargets.Length < 1;
			if (flag)
			{
				throw new ArgumentException(string.Format("The number of color render targets must be at least 1, but was {0}", binding.colorRenderTargets.Length));
			}
			bool flag2 = binding.colorRenderTargets.Length > SystemInfo.supportedRenderTargetCount;
			if (flag2)
			{
				throw new ArgumentException(string.Format("The number of color render targets ({0}) and exceeds the maximum supported number of render targets ({1})", binding.colorRenderTargets.Length, SystemInfo.supportedRenderTargetCount));
			}
			bool flag3 = binding.colorLoadActions.Length != binding.colorRenderTargets.Length;
			if (flag3)
			{
				throw new ArgumentException(string.Format("The number of color load actions provided ({0}) does not match the number of color render targets ({1})", binding.colorLoadActions.Length, binding.colorRenderTargets.Length));
			}
			bool flag4 = binding.colorStoreActions.Length != binding.colorRenderTargets.Length;
			if (flag4)
			{
				throw new ArgumentException(string.Format("The number of color store actions provided ({0}) does not match the number of color render targets ({1})", binding.colorLoadActions.Length, binding.colorRenderTargets.Length));
			}
			bool flag5 = binding.depthLoadAction == RenderBufferLoadAction.Clear || Array.IndexOf<RenderBufferLoadAction>(binding.colorLoadActions, RenderBufferLoadAction.Clear) > -1;
			if (flag5)
			{
				throw new ArgumentException("RenderBufferLoadAction.Clear is not supported");
			}
			bool flag6 = binding.colorRenderTargets.Length == 1;
			if (flag6)
			{
				this.SetRenderTargetColorDepth_Internal(binding.colorRenderTargets[0], binding.depthRenderTarget, binding.colorLoadActions[0], binding.colorStoreActions[0], binding.depthLoadAction, binding.depthStoreAction, binding.flags);
			}
			else
			{
				this.SetRenderTargetMulti_Internal(binding.colorRenderTargets, binding.depthRenderTarget, binding.colorLoadActions, binding.colorStoreActions, binding.depthLoadAction, binding.depthStoreAction, binding.flags);
			}
		}

		private void ClearRenderTargetSingle_Internal(RTClearFlags clearFlags, Color color, float depth, uint stencil)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.ClearRenderTargetSingle_Internal_Injected(intPtr, clearFlags, ref color, depth, stencil);
		}

		private unsafe void ClearRenderTargetMulti_Internal(RTClearFlags clearFlags, Color[] colors, float depth, uint stencil)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<Color> span = new Span<Color>(colors);
			fixed (Color* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				CommandBuffer.ClearRenderTargetMulti_Internal_Injected(intPtr, clearFlags, ref managedSpanWrapper, depth, stencil);
			}
		}

		private void SetRenderTargetSingle_Internal(RenderTargetIdentifier rt, RenderBufferLoadAction colorLoadAction, RenderBufferStoreAction colorStoreAction, RenderBufferLoadAction depthLoadAction, RenderBufferStoreAction depthStoreAction)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.SetRenderTargetSingle_Internal_Injected(intPtr, ref rt, colorLoadAction, colorStoreAction, depthLoadAction, depthStoreAction);
		}

		private void SetRenderTargetColorDepth_Internal(RenderTargetIdentifier color, RenderTargetIdentifier depth, RenderBufferLoadAction colorLoadAction, RenderBufferStoreAction colorStoreAction, RenderBufferLoadAction depthLoadAction, RenderBufferStoreAction depthStoreAction, RenderTargetFlags flags)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.SetRenderTargetColorDepth_Internal_Injected(intPtr, ref color, ref depth, colorLoadAction, colorStoreAction, depthLoadAction, depthStoreAction, flags);
		}

		private unsafe void SetRenderTargetMulti_Internal(RenderTargetIdentifier[] colors, RenderTargetIdentifier depth, RenderBufferLoadAction[] colorLoadActions, RenderBufferStoreAction[] colorStoreActions, RenderBufferLoadAction depthLoadAction, RenderBufferStoreAction depthStoreAction, RenderTargetFlags flags)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<RenderTargetIdentifier> span = new Span<RenderTargetIdentifier>(colors);
			fixed (RenderTargetIdentifier* ptr = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, span.Length);
				Span<RenderBufferLoadAction> span2 = new Span<RenderBufferLoadAction>(colorLoadActions);
				fixed (RenderBufferLoadAction* ptr2 = span2.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, span2.Length);
					Span<RenderBufferStoreAction> span3 = new Span<RenderBufferStoreAction>(colorStoreActions);
					fixed (RenderBufferStoreAction* pinnableReference = span3.GetPinnableReference())
					{
						ManagedSpanWrapper managedSpanWrapper3 = new ManagedSpanWrapper((void*)pinnableReference, span3.Length);
						CommandBuffer.SetRenderTargetMulti_Internal_Injected(intPtr, ref managedSpanWrapper, ref depth, ref managedSpanWrapper2, ref managedSpanWrapper3, depthLoadAction, depthStoreAction, flags);
						ptr = null;
						ptr2 = null;
					}
				}
			}
		}

		private void SetRenderTargetColorDepthSubtarget(RenderTargetIdentifier color, RenderTargetIdentifier depth, RenderBufferLoadAction colorLoadAction, RenderBufferStoreAction colorStoreAction, RenderBufferLoadAction depthLoadAction, RenderBufferStoreAction depthStoreAction, int mipLevel, CubemapFace cubemapFace, int depthSlice)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.SetRenderTargetColorDepthSubtarget_Injected(intPtr, ref color, ref depth, colorLoadAction, colorStoreAction, depthLoadAction, depthStoreAction, mipLevel, cubemapFace, depthSlice);
		}

		private unsafe void SetRenderTargetMultiSubtarget(RenderTargetIdentifier[] colors, RenderTargetIdentifier depth, RenderBufferLoadAction[] colorLoadActions, RenderBufferStoreAction[] colorStoreActions, RenderBufferLoadAction depthLoadAction, RenderBufferStoreAction depthStoreAction, int mipLevel, CubemapFace cubemapFace, int depthSlice)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<RenderTargetIdentifier> span = new Span<RenderTargetIdentifier>(colors);
			fixed (RenderTargetIdentifier* ptr = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, span.Length);
				Span<RenderBufferLoadAction> span2 = new Span<RenderBufferLoadAction>(colorLoadActions);
				fixed (RenderBufferLoadAction* ptr2 = span2.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, span2.Length);
					Span<RenderBufferStoreAction> span3 = new Span<RenderBufferStoreAction>(colorStoreActions);
					fixed (RenderBufferStoreAction* pinnableReference = span3.GetPinnableReference())
					{
						ManagedSpanWrapper managedSpanWrapper3 = new ManagedSpanWrapper((void*)pinnableReference, span3.Length);
						CommandBuffer.SetRenderTargetMultiSubtarget_Injected(intPtr, ref managedSpanWrapper, ref depth, ref managedSpanWrapper2, ref managedSpanWrapper3, depthLoadAction, depthStoreAction, mipLevel, cubemapFace, depthSlice);
						ptr = null;
						ptr2 = null;
					}
				}
			}
		}

		[NativeMethod("ProcessVTFeedback")]
		private void Internal_ProcessVTFeedback(RenderTargetIdentifier rt, IntPtr resolver, int slice, int x, int width, int y, int height, int mip)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.Internal_ProcessVTFeedback_Injected(intPtr, ref rt, resolver, slice, x, width, y, height, mip);
		}

		[SecuritySafeCritical]
		public void SetBufferData(ComputeBuffer buffer, Array data)
		{
			bool flag = data == null;
			if (flag)
			{
				throw new ArgumentNullException("data");
			}
			bool flag2 = !UnsafeUtility.IsArrayBlittable(data);
			if (flag2)
			{
				throw new ArgumentException(string.Format("Array passed to RenderingCommandBuffer.SetBufferData(array) must be blittable.\n{0}", UnsafeUtility.GetReasonForArrayNonBlittable(data)));
			}
			this.InternalSetComputeBufferData(buffer, data, 0, 0, data.Length, UnsafeUtility.SizeOf(data.GetType().GetElementType()));
		}

		[SecuritySafeCritical]
		public void SetBufferData<T>(ComputeBuffer buffer, List<T> data) where T : struct
		{
			bool flag = data == null;
			if (flag)
			{
				throw new ArgumentNullException("data");
			}
			bool flag2 = !UnsafeUtility.IsGenericListBlittable<T>();
			if (flag2)
			{
				throw new ArgumentException(string.Format("List<{0}> passed to RenderingCommandBuffer.SetBufferData(List<>) must be blittable.\n{1}", typeof(T), UnsafeUtility.GetReasonForGenericListNonBlittable<T>()));
			}
			this.InternalSetComputeBufferData(buffer, NoAllocHelpers.ExtractArrayFromList<T>(data), 0, 0, NoAllocHelpers.SafeLength<T>(data), Marshal.SizeOf(typeof(T)));
		}

		[SecuritySafeCritical]
		public void SetBufferData<T>(ComputeBuffer buffer, NativeArray<T> data) where T : struct
		{
			this.InternalSetComputeBufferNativeData(buffer, (IntPtr)data.GetUnsafeReadOnlyPtr<T>(), 0, 0, data.Length, UnsafeUtility.SizeOf<T>());
		}

		[SecuritySafeCritical]
		public void SetBufferData(ComputeBuffer buffer, Array data, int managedBufferStartIndex, int graphicsBufferStartIndex, int count)
		{
			bool flag = data == null;
			if (flag)
			{
				throw new ArgumentNullException("data");
			}
			bool flag2 = !UnsafeUtility.IsArrayBlittable(data);
			if (flag2)
			{
				throw new ArgumentException(string.Format("Array passed to RenderingCommandBuffer.SetBufferData(array) must be blittable.\n{0}", UnsafeUtility.GetReasonForArrayNonBlittable(data)));
			}
			bool flag3 = managedBufferStartIndex < 0 || graphicsBufferStartIndex < 0 || count < 0 || managedBufferStartIndex + count > data.Length;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException(string.Format("Bad indices/count arguments (managedBufferStartIndex:{0} graphicsBufferStartIndex:{1} count:{2})", managedBufferStartIndex, graphicsBufferStartIndex, count));
			}
			this.InternalSetComputeBufferData(buffer, data, managedBufferStartIndex, graphicsBufferStartIndex, count, Marshal.SizeOf(data.GetType().GetElementType()));
		}

		[SecuritySafeCritical]
		public void SetBufferData<T>(ComputeBuffer buffer, List<T> data, int managedBufferStartIndex, int graphicsBufferStartIndex, int count) where T : struct
		{
			bool flag = data == null;
			if (flag)
			{
				throw new ArgumentNullException("data");
			}
			bool flag2 = !UnsafeUtility.IsGenericListBlittable<T>();
			if (flag2)
			{
				throw new ArgumentException(string.Format("List<{0}> passed to RenderingCommandBuffer.SetBufferData(List<>) must be blittable.\n{1}", typeof(T), UnsafeUtility.GetReasonForGenericListNonBlittable<T>()));
			}
			bool flag3 = managedBufferStartIndex < 0 || graphicsBufferStartIndex < 0 || count < 0 || managedBufferStartIndex + count > data.Count;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException(string.Format("Bad indices/count arguments (managedBufferStartIndex:{0} graphicsBufferStartIndex:{1} count:{2})", managedBufferStartIndex, graphicsBufferStartIndex, count));
			}
			this.InternalSetComputeBufferData(buffer, NoAllocHelpers.ExtractArrayFromList<T>(data), managedBufferStartIndex, graphicsBufferStartIndex, count, Marshal.SizeOf(typeof(T)));
		}

		[SecuritySafeCritical]
		public void SetBufferData<T>(ComputeBuffer buffer, NativeArray<T> data, int nativeBufferStartIndex, int graphicsBufferStartIndex, int count) where T : struct
		{
			bool flag = nativeBufferStartIndex < 0 || graphicsBufferStartIndex < 0 || count < 0 || nativeBufferStartIndex + count > data.Length;
			if (flag)
			{
				throw new ArgumentOutOfRangeException(string.Format("Bad indices/count arguments (nativeBufferStartIndex:{0} graphicsBufferStartIndex:{1} count:{2})", nativeBufferStartIndex, graphicsBufferStartIndex, count));
			}
			this.InternalSetComputeBufferNativeData(buffer, (IntPtr)data.GetUnsafeReadOnlyPtr<T>(), nativeBufferStartIndex, graphicsBufferStartIndex, count, UnsafeUtility.SizeOf<T>());
		}

		public void SetBufferCounterValue(ComputeBuffer buffer, uint counterValue)
		{
			this.InternalSetComputeBufferCounterValue(buffer, counterValue);
		}

		[FreeFunction(Name = "RenderingCommandBuffer_Bindings::InternalSetGraphicsBufferNativeData", HasExplicitThis = true, ThrowsException = true)]
		private void InternalSetComputeBufferNativeData([NotNull] ComputeBuffer buffer, IntPtr data, int nativeBufferStartIndex, int graphicsBufferStartIndex, int count, int elemSize)
		{
			if (buffer == null)
			{
				ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = ComputeBuffer.BindingsMarshaller.ConvertToNative(buffer);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
			}
			CommandBuffer.InternalSetComputeBufferNativeData_Injected(intPtr, intPtr2, data, nativeBufferStartIndex, graphicsBufferStartIndex, count, elemSize);
		}

		[FreeFunction(Name = "RenderingCommandBuffer_Bindings::InternalSetGraphicsBufferData", HasExplicitThis = true, ThrowsException = true)]
		private void InternalSetComputeBufferData([NotNull] ComputeBuffer buffer, Array data, int managedBufferStartIndex, int graphicsBufferStartIndex, int count, int elemSize)
		{
			if (buffer == null)
			{
				ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = ComputeBuffer.BindingsMarshaller.ConvertToNative(buffer);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
			}
			CommandBuffer.InternalSetComputeBufferData_Injected(intPtr, intPtr2, data, managedBufferStartIndex, graphicsBufferStartIndex, count, elemSize);
		}

		[FreeFunction(Name = "RenderingCommandBuffer_Bindings::InternalSetGraphicsBufferCounterValue", HasExplicitThis = true)]
		private void InternalSetComputeBufferCounterValue([NotNull] ComputeBuffer buffer, uint counterValue)
		{
			if (buffer == null)
			{
				ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = ComputeBuffer.BindingsMarshaller.ConvertToNative(buffer);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
			}
			CommandBuffer.InternalSetComputeBufferCounterValue_Injected(intPtr, intPtr2, counterValue);
		}

		[SecuritySafeCritical]
		public void SetBufferData(GraphicsBuffer buffer, Array data)
		{
			bool flag = data == null;
			if (flag)
			{
				throw new ArgumentNullException("data");
			}
			bool flag2 = !UnsafeUtility.IsArrayBlittable(data);
			if (flag2)
			{
				throw new ArgumentException(string.Format("Array passed to RenderingCommandBuffer.SetBufferData(array) must be blittable.\n{0}", UnsafeUtility.GetReasonForArrayNonBlittable(data)));
			}
			this.InternalSetGraphicsBufferData(buffer, data, 0, 0, data.Length, UnsafeUtility.SizeOf(data.GetType().GetElementType()));
		}

		[SecuritySafeCritical]
		public void SetBufferData<T>(GraphicsBuffer buffer, List<T> data) where T : struct
		{
			bool flag = data == null;
			if (flag)
			{
				throw new ArgumentNullException("data");
			}
			bool flag2 = !UnsafeUtility.IsGenericListBlittable<T>();
			if (flag2)
			{
				throw new ArgumentException(string.Format("List<{0}> passed to RenderingCommandBuffer.SetBufferData(List<>) must be blittable.\n{1}", typeof(T), UnsafeUtility.GetReasonForGenericListNonBlittable<T>()));
			}
			this.InternalSetGraphicsBufferData(buffer, NoAllocHelpers.ExtractArrayFromList<T>(data), 0, 0, NoAllocHelpers.SafeLength<T>(data), Marshal.SizeOf(typeof(T)));
		}

		[SecuritySafeCritical]
		public void SetBufferData<T>(GraphicsBuffer buffer, NativeArray<T> data) where T : struct
		{
			this.InternalSetGraphicsBufferNativeData(buffer, (IntPtr)data.GetUnsafeReadOnlyPtr<T>(), 0, 0, data.Length, UnsafeUtility.SizeOf<T>());
		}

		[SecuritySafeCritical]
		public void SetBufferData(GraphicsBuffer buffer, Array data, int managedBufferStartIndex, int graphicsBufferStartIndex, int count)
		{
			bool flag = data == null;
			if (flag)
			{
				throw new ArgumentNullException("data");
			}
			bool flag2 = !UnsafeUtility.IsArrayBlittable(data);
			if (flag2)
			{
				throw new ArgumentException(string.Format("Array passed to RenderingCommandBuffer.SetBufferData(array) must be blittable.\n{0}", UnsafeUtility.GetReasonForArrayNonBlittable(data)));
			}
			bool flag3 = managedBufferStartIndex < 0 || graphicsBufferStartIndex < 0 || count < 0 || managedBufferStartIndex + count > data.Length;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException(string.Format("Bad indices/count arguments (managedBufferStartIndex:{0} graphicsBufferStartIndex:{1} count:{2})", managedBufferStartIndex, graphicsBufferStartIndex, count));
			}
			this.InternalSetGraphicsBufferData(buffer, data, managedBufferStartIndex, graphicsBufferStartIndex, count, Marshal.SizeOf(data.GetType().GetElementType()));
		}

		[SecuritySafeCritical]
		public void SetBufferData<T>(GraphicsBuffer buffer, List<T> data, int managedBufferStartIndex, int graphicsBufferStartIndex, int count) where T : struct
		{
			bool flag = data == null;
			if (flag)
			{
				throw new ArgumentNullException("data");
			}
			bool flag2 = !UnsafeUtility.IsGenericListBlittable<T>();
			if (flag2)
			{
				throw new ArgumentException(string.Format("List<{0}> passed to RenderingCommandBuffer.SetBufferData(List<>) must be blittable.\n{1}", typeof(T), UnsafeUtility.GetReasonForGenericListNonBlittable<T>()));
			}
			bool flag3 = managedBufferStartIndex < 0 || graphicsBufferStartIndex < 0 || count < 0 || managedBufferStartIndex + count > data.Count;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException(string.Format("Bad indices/count arguments (managedBufferStartIndex:{0} graphicsBufferStartIndex:{1} count:{2})", managedBufferStartIndex, graphicsBufferStartIndex, count));
			}
			this.InternalSetGraphicsBufferData(buffer, NoAllocHelpers.ExtractArrayFromList<T>(data), managedBufferStartIndex, graphicsBufferStartIndex, count, Marshal.SizeOf(typeof(T)));
		}

		[SecuritySafeCritical]
		public void SetBufferData<T>(GraphicsBuffer buffer, NativeArray<T> data, int nativeBufferStartIndex, int graphicsBufferStartIndex, int count) where T : struct
		{
			bool flag = nativeBufferStartIndex < 0 || graphicsBufferStartIndex < 0 || count < 0 || nativeBufferStartIndex + count > data.Length;
			if (flag)
			{
				throw new ArgumentOutOfRangeException(string.Format("Bad indices/count arguments (nativeBufferStartIndex:{0} graphicsBufferStartIndex:{1} count:{2})", nativeBufferStartIndex, graphicsBufferStartIndex, count));
			}
			this.InternalSetGraphicsBufferNativeData(buffer, (IntPtr)data.GetUnsafeReadOnlyPtr<T>(), nativeBufferStartIndex, graphicsBufferStartIndex, count, UnsafeUtility.SizeOf<T>());
		}

		public void SetBufferCounterValue(GraphicsBuffer buffer, uint counterValue)
		{
			this.InternalSetGraphicsBufferCounterValue(buffer, counterValue);
		}

		[FreeFunction(Name = "RenderingCommandBuffer_Bindings::InternalSetGraphicsBufferNativeData", HasExplicitThis = true, ThrowsException = true)]
		private void InternalSetGraphicsBufferNativeData([NotNull] GraphicsBuffer buffer, IntPtr data, int nativeBufferStartIndex, int graphicsBufferStartIndex, int count, int elemSize)
		{
			if (buffer == null)
			{
				ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = GraphicsBuffer.BindingsMarshaller.ConvertToNative(buffer);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
			}
			CommandBuffer.InternalSetGraphicsBufferNativeData_Injected(intPtr, intPtr2, data, nativeBufferStartIndex, graphicsBufferStartIndex, count, elemSize);
		}

		[FreeFunction(Name = "RenderingCommandBuffer_Bindings::InternalSetGraphicsBufferData", HasExplicitThis = true, ThrowsException = true)]
		[SecurityCritical]
		private void InternalSetGraphicsBufferData([NotNull] GraphicsBuffer buffer, Array data, int managedBufferStartIndex, int graphicsBufferStartIndex, int count, int elemSize)
		{
			if (buffer == null)
			{
				ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = GraphicsBuffer.BindingsMarshaller.ConvertToNative(buffer);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
			}
			CommandBuffer.InternalSetGraphicsBufferData_Injected(intPtr, intPtr2, data, managedBufferStartIndex, graphicsBufferStartIndex, count, elemSize);
		}

		[FreeFunction(Name = "RenderingCommandBuffer_Bindings::InternalSetGraphicsBufferCounterValue", HasExplicitThis = true)]
		private void InternalSetGraphicsBufferCounterValue([NotNull] GraphicsBuffer buffer, uint counterValue)
		{
			if (buffer == null)
			{
				ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = GraphicsBuffer.BindingsMarshaller.ConvertToNative(buffer);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(buffer, "buffer");
			}
			CommandBuffer.InternalSetGraphicsBufferCounterValue_Injected(intPtr, intPtr2, counterValue);
		}

		[FreeFunction(Name = "RenderingCommandBuffer_Bindings::CopyBuffer", HasExplicitThis = true, ThrowsException = true)]
		private void CopyBufferImpl([NotNull] GraphicsBuffer source, [NotNull] GraphicsBuffer dest)
		{
			if (source == null)
			{
				ThrowHelper.ThrowArgumentNullException(source, "source");
			}
			if (dest == null)
			{
				ThrowHelper.ThrowArgumentNullException(dest, "dest");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = GraphicsBuffer.BindingsMarshaller.ConvertToNative(source);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(source, "source");
			}
			IntPtr intPtr3 = GraphicsBuffer.BindingsMarshaller.ConvertToNative(dest);
			if (intPtr3 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(dest, "dest");
			}
			CommandBuffer.CopyBufferImpl_Injected(intPtr, intPtr2, intPtr3);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::BeginRenderPass", HasExplicitThis = true)]
		private unsafe void BeginRenderPass_Internal(int width, int height, int volumeDepth, int samples, ReadOnlySpan<AttachmentDescriptor> attachments, int depthAttachmentIndex, int shadingRateImageAttachmentIndex, ReadOnlySpan<SubPassDescriptor> subPasses, ReadOnlySpan<byte> debugNameUtf8)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ReadOnlySpan<AttachmentDescriptor> readOnlySpan = attachments;
			fixed (AttachmentDescriptor* ptr = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
				ReadOnlySpan<SubPassDescriptor> readOnlySpan2 = subPasses;
				fixed (SubPassDescriptor* ptr2 = readOnlySpan2.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					ReadOnlySpan<byte> readOnlySpan3 = debugNameUtf8;
					fixed (byte* pinnableReference = readOnlySpan3.GetPinnableReference())
					{
						ManagedSpanWrapper managedSpanWrapper3 = new ManagedSpanWrapper((void*)pinnableReference, readOnlySpan3.Length);
						CommandBuffer.BeginRenderPass_Internal_Injected(intPtr, width, height, volumeDepth, samples, ref managedSpanWrapper, depthAttachmentIndex, shadingRateImageAttachmentIndex, ref managedSpanWrapper2, ref managedSpanWrapper3);
						ptr = null;
						ptr2 = null;
					}
				}
			}
		}

		public void BeginRenderPass(int width, int height, int volumeDepth, int samples, NativeArray<AttachmentDescriptor> attachments, int depthAttachmentIndex, NativeArray<SubPassDescriptor> subPasses, ReadOnlySpan<byte> debugNameUtf8)
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.BeginRenderPass_Internal(width, height, volumeDepth, samples, in attachments, depthAttachmentIndex, -1, in subPasses, debugNameUtf8);
		}

		public void BeginRenderPass(int width, int height, int samples, NativeArray<AttachmentDescriptor> attachments, int depthAttachmentIndex, NativeArray<SubPassDescriptor> subPasses, ReadOnlySpan<byte> debugNameUtf8)
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.BeginRenderPass_Internal(width, height, 1, samples, in attachments, depthAttachmentIndex, -1, in subPasses, debugNameUtf8);
		}

		public void BeginRenderPass(int width, int height, int volumeDepth, int samples, NativeArray<AttachmentDescriptor> attachments, int depthAttachmentIndex, NativeArray<SubPassDescriptor> subPasses)
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.BeginRenderPass_Internal(width, height, volumeDepth, samples, in attachments, depthAttachmentIndex, -1, in subPasses, default(ReadOnlySpan<byte>));
		}

		public void BeginRenderPass(int width, int height, int samples, NativeArray<AttachmentDescriptor> attachments, int depthAttachmentIndex, NativeArray<SubPassDescriptor> subPasses)
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.BeginRenderPass_Internal(width, height, 1, samples, in attachments, depthAttachmentIndex, -1, in subPasses, default(ReadOnlySpan<byte>));
		}

		public void BeginRenderPass(int width, int height, int samples, NativeArray<AttachmentDescriptor> attachments, int depthAttachmentIndex, int shadingRateImageAttachmentIndex, NativeArray<SubPassDescriptor> subPasses)
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.BeginRenderPass_Internal(width, height, 1, samples, in attachments, depthAttachmentIndex, shadingRateImageAttachmentIndex, in subPasses, default(ReadOnlySpan<byte>));
		}

		public void BeginRenderPass(int width, int height, int volumeDepth, int samples, NativeArray<AttachmentDescriptor> attachments, int depthAttachmentIndex, int shadingRateImageAttachmentIndex, NativeArray<SubPassDescriptor> subPasses)
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.BeginRenderPass_Internal(width, height, volumeDepth, samples, in attachments, depthAttachmentIndex, shadingRateImageAttachmentIndex, in subPasses, default(ReadOnlySpan<byte>));
		}

		public void BeginRenderPass(int width, int height, int samples, NativeArray<AttachmentDescriptor> attachments, int depthAttachmentIndex, int shadingRateImageAttachmentIndex, NativeArray<SubPassDescriptor> subPasses, ReadOnlySpan<byte> debugNameUtf8)
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.BeginRenderPass_Internal(width, height, 1, samples, in attachments, depthAttachmentIndex, shadingRateImageAttachmentIndex, in subPasses, debugNameUtf8);
		}

		public void BeginRenderPass(int width, int height, int volumeDepth, int samples, NativeArray<AttachmentDescriptor> attachments, int depthAttachmentIndex, int shadingRateImageAttachmentIndex, NativeArray<SubPassDescriptor> subPasses, ReadOnlySpan<byte> debugNameUtf8)
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.BeginRenderPass_Internal(width, height, volumeDepth, samples, in attachments, depthAttachmentIndex, shadingRateImageAttachmentIndex, in subPasses, debugNameUtf8);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::NextSubPass", HasExplicitThis = true)]
		private void NextSubPass_Internal()
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.NextSubPass_Internal_Injected(intPtr);
		}

		public void NextSubPass()
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.NextSubPass_Internal();
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::EndRenderPass", HasExplicitThis = true)]
		private void EndRenderPass_Internal()
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.EndRenderPass_Internal_Injected(intPtr);
		}

		public void EndRenderPass()
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.EndRenderPass_Internal();
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetupCameraProperties", HasExplicitThis = true)]
		private void SetupCameraProperties_Internal([NotNull] Camera camera)
		{
			if (camera == null)
			{
				ThrowHelper.ThrowArgumentNullException(camera, "camera");
			}
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Camera>(camera);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(camera, "camera");
			}
			CommandBuffer.SetupCameraProperties_Internal_Injected(intPtr, intPtr2);
		}

		public void SetupCameraProperties(Camera camera)
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.SetupCameraProperties_Internal(camera);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::InvokeOnRenderObjectCallbacks", HasExplicitThis = true)]
		private void InvokeOnRenderObjectCallbacks_Internal()
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.InvokeOnRenderObjectCallbacks_Internal_Injected(intPtr);
		}

		public void InvokeOnRenderObjectCallbacks()
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.InvokeOnRenderObjectCallbacks_Internal();
		}

		public void SetShadingRateFragmentSize(ShadingRateFragmentSize shadingRateFragmentSize)
		{
			this.SetShadingRateFragmentSize_Impl(shadingRateFragmentSize);
		}

		public void SetShadingRateCombiner(ShadingRateCombinerStage stage, ShadingRateCombiner combiner)
		{
			this.SetShadingRateCombiner_Impl(stage, combiner);
		}

		public void SetShadingRateImage(in RenderTargetIdentifier shadingRateImage)
		{
			this.SetShadingRateImage_Impl(in shadingRateImage);
		}

		public void ResetShadingRate()
		{
			this.ResetShadingRate_Impl();
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetShadingRateFragmentSize_Impl", HasExplicitThis = true)]
		private void SetShadingRateFragmentSize_Impl(ShadingRateFragmentSize shadingRateFragmentSize)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.SetShadingRateFragmentSize_Impl_Injected(intPtr, shadingRateFragmentSize);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetShadingRateCombiner_Impl", HasExplicitThis = true)]
		private void SetShadingRateCombiner_Impl(ShadingRateCombinerStage stage, ShadingRateCombiner combiner)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.SetShadingRateCombiner_Impl_Injected(intPtr, stage, combiner);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::SetShadingRateImage_Impl", HasExplicitThis = true)]
		private void SetShadingRateImage_Impl(in RenderTargetIdentifier shadingRateImage)
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.SetShadingRateImage_Impl_Injected(intPtr, in shadingRateImage);
		}

		[FreeFunction("RenderingCommandBuffer_Bindings::ResetShadingRate_Impl", HasExplicitThis = true)]
		private void ResetShadingRate_Impl()
		{
			IntPtr intPtr = CommandBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			CommandBuffer.ResetShadingRate_Impl_Injected(intPtr);
		}

		private CommandBuffer(IntPtr ptr)
		{
			this.m_Ptr = ptr;
		}

		~CommandBuffer()
		{
			this.Dispose(false);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			this.ReleaseBuffer();
			this.m_Ptr = IntPtr.Zero;
		}

		public CommandBuffer()
		{
			this.m_Ptr = CommandBuffer.InitBuffer();
		}

		public void Release()
		{
			this.Dispose();
		}

		public GraphicsFence CreateAsyncGraphicsFence()
		{
			return this.CreateGraphicsFence(GraphicsFenceType.AsyncQueueSynchronisation, SynchronisationStageFlags.PixelProcessing);
		}

		public GraphicsFence CreateAsyncGraphicsFence(SynchronisationStage stage)
		{
			return this.CreateGraphicsFence(GraphicsFenceType.AsyncQueueSynchronisation, GraphicsFence.TranslateSynchronizationStageToFlags(stage));
		}

		public GraphicsFence CreateGraphicsFence(GraphicsFenceType fenceType, SynchronisationStageFlags stage)
		{
			GraphicsFence graphicsFence = default(GraphicsFence);
			graphicsFence.m_FenceType = fenceType;
			graphicsFence.m_Ptr = this.CreateGPUFence_Internal(fenceType, stage);
			graphicsFence.InitPostAllocation();
			graphicsFence.Validate();
			return graphicsFence;
		}

		public void WaitOnAsyncGraphicsFence(GraphicsFence fence)
		{
			this.WaitOnAsyncGraphicsFence(fence, SynchronisationStage.VertexProcessing);
		}

		public void WaitOnAsyncGraphicsFence(GraphicsFence fence, SynchronisationStage stage)
		{
			this.WaitOnAsyncGraphicsFence(fence, GraphicsFence.TranslateSynchronizationStageToFlags(stage));
		}

		public void WaitOnAsyncGraphicsFence(GraphicsFence fence, SynchronisationStageFlags stage)
		{
			bool flag = fence.m_FenceType > GraphicsFenceType.AsyncQueueSynchronisation;
			if (flag)
			{
				throw new ArgumentException("Attempting to call WaitOnAsyncGPUFence on a fence that is not of GraphicsFenceType.AsyncQueueSynchronization");
			}
			fence.Validate();
			bool flag2 = fence.IsFencePending();
			if (flag2)
			{
				this.WaitOnGPUFence_Internal(fence.m_Ptr, stage);
			}
		}

		public void SetComputeFloatParam(ComputeShader computeShader, string name, float val)
		{
			this.SetComputeFloatParam(computeShader, Shader.PropertyToID(name), val);
		}

		public void SetComputeIntParam(ComputeShader computeShader, string name, int val)
		{
			this.SetComputeIntParam(computeShader, Shader.PropertyToID(name), val);
		}

		public void SetComputeVectorParam(ComputeShader computeShader, string name, Vector4 val)
		{
			this.SetComputeVectorParam(computeShader, Shader.PropertyToID(name), val);
		}

		public void SetComputeVectorArrayParam(ComputeShader computeShader, string name, Vector4[] values)
		{
			this.SetComputeVectorArrayParam(computeShader, Shader.PropertyToID(name), values);
		}

		public void SetComputeMatrixParam(ComputeShader computeShader, string name, Matrix4x4 val)
		{
			this.SetComputeMatrixParam(computeShader, Shader.PropertyToID(name), val);
		}

		public void SetComputeMatrixArrayParam(ComputeShader computeShader, string name, Matrix4x4[] values)
		{
			this.SetComputeMatrixArrayParam(computeShader, Shader.PropertyToID(name), values);
		}

		public void SetComputeFloatParams(ComputeShader computeShader, string name, params float[] values)
		{
			this.Internal_SetComputeFloats(computeShader, Shader.PropertyToID(name), values);
		}

		public void SetComputeFloatParams(ComputeShader computeShader, int nameID, params float[] values)
		{
			this.Internal_SetComputeFloats(computeShader, nameID, values);
		}

		public void SetComputeIntParams(ComputeShader computeShader, string name, params int[] values)
		{
			this.Internal_SetComputeInts(computeShader, Shader.PropertyToID(name), values);
		}

		public void SetComputeIntParams(ComputeShader computeShader, int nameID, params int[] values)
		{
			this.Internal_SetComputeInts(computeShader, nameID, values);
		}

		public void SetComputeTextureParam(ComputeShader computeShader, int kernelIndex, string name, RenderTargetIdentifier rt)
		{
			this.Internal_SetComputeTextureParam(computeShader, kernelIndex, Shader.PropertyToID(name), ref rt, 0, RenderTextureSubElement.Default);
		}

		public void SetComputeTextureParam(ComputeShader computeShader, int kernelIndex, int nameID, RenderTargetIdentifier rt)
		{
			this.Internal_SetComputeTextureParam(computeShader, kernelIndex, nameID, ref rt, 0, RenderTextureSubElement.Default);
		}

		public void SetComputeTextureParam(ComputeShader computeShader, int kernelIndex, string name, RenderTargetIdentifier rt, int mipLevel)
		{
			this.Internal_SetComputeTextureParam(computeShader, kernelIndex, Shader.PropertyToID(name), ref rt, mipLevel, RenderTextureSubElement.Default);
		}

		public void SetComputeTextureParam(ComputeShader computeShader, int kernelIndex, int nameID, RenderTargetIdentifier rt, int mipLevel)
		{
			this.Internal_SetComputeTextureParam(computeShader, kernelIndex, nameID, ref rt, mipLevel, RenderTextureSubElement.Default);
		}

		public void SetComputeTextureParam(ComputeShader computeShader, int kernelIndex, string name, RenderTargetIdentifier rt, int mipLevel, RenderTextureSubElement element)
		{
			this.Internal_SetComputeTextureParam(computeShader, kernelIndex, Shader.PropertyToID(name), ref rt, mipLevel, element);
		}

		public void SetComputeTextureParam(ComputeShader computeShader, int kernelIndex, int nameID, RenderTargetIdentifier rt, int mipLevel, RenderTextureSubElement element)
		{
			this.Internal_SetComputeTextureParam(computeShader, kernelIndex, nameID, ref rt, mipLevel, element);
		}

		public void SetComputeBufferParam(ComputeShader computeShader, int kernelIndex, int nameID, ComputeBuffer buffer)
		{
			this.Internal_SetComputeBufferParam(computeShader, kernelIndex, nameID, buffer);
		}

		public void SetComputeBufferParam(ComputeShader computeShader, int kernelIndex, string name, ComputeBuffer buffer)
		{
			this.Internal_SetComputeBufferParam(computeShader, kernelIndex, Shader.PropertyToID(name), buffer);
		}

		public void SetComputeBufferParam(ComputeShader computeShader, int kernelIndex, int nameID, GraphicsBufferHandle bufferHandle)
		{
			this.Internal_SetComputeGraphicsBufferHandleParam(computeShader, kernelIndex, nameID, bufferHandle);
		}

		public void SetComputeBufferParam(ComputeShader computeShader, int kernelIndex, string name, GraphicsBufferHandle bufferHandle)
		{
			this.Internal_SetComputeGraphicsBufferHandleParam(computeShader, kernelIndex, Shader.PropertyToID(name), bufferHandle);
		}

		public void SetComputeBufferParam(ComputeShader computeShader, int kernelIndex, int nameID, GraphicsBuffer buffer)
		{
			this.Internal_SetComputeGraphicsBufferParam(computeShader, kernelIndex, nameID, buffer);
		}

		public void SetComputeBufferParam(ComputeShader computeShader, int kernelIndex, string name, GraphicsBuffer buffer)
		{
			this.Internal_SetComputeGraphicsBufferParam(computeShader, kernelIndex, Shader.PropertyToID(name), buffer);
		}

		public void SetComputeConstantBufferParam(ComputeShader computeShader, int nameID, ComputeBuffer buffer, int offset, int size)
		{
			this.Internal_SetComputeConstantComputeBufferParam(computeShader, nameID, buffer, offset, size);
		}

		public void SetComputeConstantBufferParam(ComputeShader computeShader, string name, ComputeBuffer buffer, int offset, int size)
		{
			this.Internal_SetComputeConstantComputeBufferParam(computeShader, Shader.PropertyToID(name), buffer, offset, size);
		}

		public void SetComputeConstantBufferParam(ComputeShader computeShader, int nameID, GraphicsBuffer buffer, int offset, int size)
		{
			this.Internal_SetComputeConstantGraphicsBufferParam(computeShader, nameID, buffer, offset, size);
		}

		public void SetComputeConstantBufferParam(ComputeShader computeShader, string name, GraphicsBuffer buffer, int offset, int size)
		{
			this.Internal_SetComputeConstantGraphicsBufferParam(computeShader, Shader.PropertyToID(name), buffer, offset, size);
		}

		public void SetComputeParamsFromMaterial(ComputeShader computeShader, int kernelIndex, Material material)
		{
			this.Internal_SetComputeParamsFromMaterial(computeShader, kernelIndex, material);
		}

		public void DispatchCompute(ComputeShader computeShader, int kernelIndex, int threadGroupsX, int threadGroupsY, int threadGroupsZ)
		{
			this.Internal_DispatchCompute(computeShader, kernelIndex, threadGroupsX, threadGroupsY, threadGroupsZ);
		}

		public void DispatchCompute(ComputeShader computeShader, int kernelIndex, ComputeBuffer indirectBuffer, uint argsOffset)
		{
			bool flag = SystemInfo.graphicsDeviceType == GraphicsDeviceType.Metal && !SystemInfo.supportsIndirectArgumentsBuffer;
			if (flag)
			{
				throw new InvalidOperationException("Indirect argument buffers are not supported.");
			}
			this.Internal_DispatchComputeIndirect(computeShader, kernelIndex, indirectBuffer, argsOffset);
		}

		public void DispatchCompute(ComputeShader computeShader, int kernelIndex, GraphicsBuffer indirectBuffer, uint argsOffset)
		{
			bool flag = SystemInfo.graphicsDeviceType == GraphicsDeviceType.Metal && !SystemInfo.supportsIndirectArgumentsBuffer;
			if (flag)
			{
				throw new InvalidOperationException("Indirect argument buffers are not supported.");
			}
			this.Internal_DispatchComputeIndirectGraphicsBuffer(computeShader, kernelIndex, indirectBuffer, argsOffset);
		}

		public void BuildRayTracingAccelerationStructure(RayTracingAccelerationStructure accelerationStructure)
		{
			RayTracingAccelerationStructure.BuildSettings buildSettings = new RayTracingAccelerationStructure.BuildSettings
			{
				buildFlags = RayTracingAccelerationStructureBuildFlags.PreferFastTrace,
				relativeOrigin = Vector3.zero
			};
			this.Internal_BuildRayTracingAccelerationStructure(accelerationStructure, buildSettings);
		}

		public void BuildRayTracingAccelerationStructure(RayTracingAccelerationStructure accelerationStructure, Vector3 relativeOrigin)
		{
			RayTracingAccelerationStructure.BuildSettings buildSettings = new RayTracingAccelerationStructure.BuildSettings
			{
				buildFlags = RayTracingAccelerationStructureBuildFlags.PreferFastTrace,
				relativeOrigin = relativeOrigin
			};
			this.Internal_BuildRayTracingAccelerationStructure(accelerationStructure, buildSettings);
		}

		public void BuildRayTracingAccelerationStructure(RayTracingAccelerationStructure accelerationStructure, RayTracingAccelerationStructure.BuildSettings buildSettings)
		{
			this.Internal_BuildRayTracingAccelerationStructure(accelerationStructure, buildSettings);
		}

		public void SetRayTracingAccelerationStructure(RayTracingShader rayTracingShader, string name, RayTracingAccelerationStructure rayTracingAccelerationStructure)
		{
			this.Internal_SetRayTracingAccelerationStructure(rayTracingShader, Shader.PropertyToID(name), rayTracingAccelerationStructure);
		}

		public void SetRayTracingAccelerationStructure(RayTracingShader rayTracingShader, int nameID, RayTracingAccelerationStructure rayTracingAccelerationStructure)
		{
			this.Internal_SetRayTracingAccelerationStructure(rayTracingShader, nameID, rayTracingAccelerationStructure);
		}

		public void SetRayTracingAccelerationStructure(ComputeShader computeShader, int kernelIndex, string name, RayTracingAccelerationStructure rayTracingAccelerationStructure)
		{
			this.Internal_SetComputeRayTracingAccelerationStructure(computeShader, kernelIndex, Shader.PropertyToID(name), rayTracingAccelerationStructure);
		}

		public void SetRayTracingAccelerationStructure(ComputeShader computeShader, int kernelIndex, int nameID, RayTracingAccelerationStructure rayTracingAccelerationStructure)
		{
			this.Internal_SetComputeRayTracingAccelerationStructure(computeShader, kernelIndex, nameID, rayTracingAccelerationStructure);
		}

		public void SetRayTracingBufferParam(RayTracingShader rayTracingShader, string name, ComputeBuffer buffer)
		{
			this.Internal_SetRayTracingComputeBufferParam(rayTracingShader, Shader.PropertyToID(name), buffer);
		}

		public void SetRayTracingBufferParam(RayTracingShader rayTracingShader, int nameID, ComputeBuffer buffer)
		{
			this.Internal_SetRayTracingComputeBufferParam(rayTracingShader, nameID, buffer);
		}

		public void SetRayTracingBufferParam(RayTracingShader rayTracingShader, string name, GraphicsBuffer buffer)
		{
			this.Internal_SetRayTracingGraphicsBufferParam(rayTracingShader, Shader.PropertyToID(name), buffer);
		}

		public void SetRayTracingBufferParam(RayTracingShader rayTracingShader, int nameID, GraphicsBuffer buffer)
		{
			this.Internal_SetRayTracingGraphicsBufferParam(rayTracingShader, nameID, buffer);
		}

		public void SetRayTracingBufferParam(RayTracingShader rayTracingShader, string name, GraphicsBufferHandle bufferHandle)
		{
			this.Internal_SetRayTracingGraphicsBufferHandleParam(rayTracingShader, Shader.PropertyToID(name), bufferHandle);
		}

		public void SetRayTracingBufferParam(RayTracingShader rayTracingShader, int nameID, GraphicsBufferHandle bufferHandle)
		{
			this.Internal_SetRayTracingGraphicsBufferHandleParam(rayTracingShader, nameID, bufferHandle);
		}

		public void SetRayTracingConstantBufferParam(RayTracingShader rayTracingShader, int nameID, ComputeBuffer buffer, int offset, int size)
		{
			this.Internal_SetRayTracingConstantComputeBufferParam(rayTracingShader, nameID, buffer, offset, size);
		}

		public void SetRayTracingConstantBufferParam(RayTracingShader rayTracingShader, string name, ComputeBuffer buffer, int offset, int size)
		{
			this.Internal_SetRayTracingConstantComputeBufferParam(rayTracingShader, Shader.PropertyToID(name), buffer, offset, size);
		}

		public void SetRayTracingConstantBufferParam(RayTracingShader rayTracingShader, int nameID, GraphicsBuffer buffer, int offset, int size)
		{
			this.Internal_SetRayTracingConstantGraphicsBufferParam(rayTracingShader, nameID, buffer, offset, size);
		}

		public void SetRayTracingConstantBufferParam(RayTracingShader rayTracingShader, string name, GraphicsBuffer buffer, int offset, int size)
		{
			this.Internal_SetRayTracingConstantGraphicsBufferParam(rayTracingShader, Shader.PropertyToID(name), buffer, offset, size);
		}

		public void SetRayTracingTextureParam(RayTracingShader rayTracingShader, string name, RenderTargetIdentifier rt)
		{
			this.Internal_SetRayTracingTextureParam(rayTracingShader, Shader.PropertyToID(name), ref rt);
		}

		public void SetRayTracingTextureParam(RayTracingShader rayTracingShader, int nameID, RenderTargetIdentifier rt)
		{
			this.Internal_SetRayTracingTextureParam(rayTracingShader, nameID, ref rt);
		}

		public void SetRayTracingFloatParam(RayTracingShader rayTracingShader, string name, float val)
		{
			this.Internal_SetRayTracingFloatParam(rayTracingShader, Shader.PropertyToID(name), val);
		}

		public void SetRayTracingFloatParam(RayTracingShader rayTracingShader, int nameID, float val)
		{
			this.Internal_SetRayTracingFloatParam(rayTracingShader, nameID, val);
		}

		public void SetRayTracingFloatParams(RayTracingShader rayTracingShader, string name, params float[] values)
		{
			this.Internal_SetRayTracingFloats(rayTracingShader, Shader.PropertyToID(name), values);
		}

		public void SetRayTracingFloatParams(RayTracingShader rayTracingShader, int nameID, params float[] values)
		{
			this.Internal_SetRayTracingFloats(rayTracingShader, nameID, values);
		}

		public void SetRayTracingIntParam(RayTracingShader rayTracingShader, string name, int val)
		{
			this.Internal_SetRayTracingIntParam(rayTracingShader, Shader.PropertyToID(name), val);
		}

		public void SetRayTracingIntParam(RayTracingShader rayTracingShader, int nameID, int val)
		{
			this.Internal_SetRayTracingIntParam(rayTracingShader, nameID, val);
		}

		public void SetRayTracingIntParams(RayTracingShader rayTracingShader, string name, params int[] values)
		{
			this.Internal_SetRayTracingInts(rayTracingShader, Shader.PropertyToID(name), values);
		}

		public void SetRayTracingIntParams(RayTracingShader rayTracingShader, int nameID, params int[] values)
		{
			this.Internal_SetRayTracingInts(rayTracingShader, nameID, values);
		}

		public void SetRayTracingVectorParam(RayTracingShader rayTracingShader, string name, Vector4 val)
		{
			this.Internal_SetRayTracingVectorParam(rayTracingShader, Shader.PropertyToID(name), val);
		}

		public void SetRayTracingVectorParam(RayTracingShader rayTracingShader, int nameID, Vector4 val)
		{
			this.Internal_SetRayTracingVectorParam(rayTracingShader, nameID, val);
		}

		public void SetRayTracingVectorArrayParam(RayTracingShader rayTracingShader, string name, params Vector4[] values)
		{
			this.Internal_SetRayTracingVectorArrayParam(rayTracingShader, Shader.PropertyToID(name), values);
		}

		public void SetRayTracingVectorArrayParam(RayTracingShader rayTracingShader, int nameID, params Vector4[] values)
		{
			this.Internal_SetRayTracingVectorArrayParam(rayTracingShader, nameID, values);
		}

		public void SetRayTracingMatrixParam(RayTracingShader rayTracingShader, string name, Matrix4x4 val)
		{
			this.Internal_SetRayTracingMatrixParam(rayTracingShader, Shader.PropertyToID(name), val);
		}

		public void SetRayTracingMatrixParam(RayTracingShader rayTracingShader, int nameID, Matrix4x4 val)
		{
			this.Internal_SetRayTracingMatrixParam(rayTracingShader, nameID, val);
		}

		public void SetRayTracingMatrixArrayParam(RayTracingShader rayTracingShader, string name, params Matrix4x4[] values)
		{
			this.Internal_SetRayTracingMatrixArrayParam(rayTracingShader, Shader.PropertyToID(name), values);
		}

		public void SetRayTracingMatrixArrayParam(RayTracingShader rayTracingShader, int nameID, params Matrix4x4[] values)
		{
			this.Internal_SetRayTracingMatrixArrayParam(rayTracingShader, nameID, values);
		}

		public void DispatchRays(RayTracingShader rayTracingShader, string rayGenName, uint width, uint height, uint depth, Camera camera = null)
		{
			this.Internal_DispatchRays(rayTracingShader, rayGenName, width, height, depth, camera);
		}

		public void DispatchRays(RayTracingShader rayTracingShader, string rayGenName, GraphicsBuffer argsBuffer, uint argsOffset, Camera camera = null)
		{
			this.Internal_DispatchRaysIndirect(rayTracingShader, rayGenName, argsBuffer, argsOffset, camera);
		}

		internal void SetMachineLearningOperatorTensors(MachineLearningOperator op, ReadOnlySpan<IntPtr> inputs, ReadOnlySpan<IntPtr> outputs)
		{
			this.Internal_SetMachineLearningOperatorTensors(op.m_Ptr, inputs, outputs);
		}

		internal void DispatchMachineLearningOperator(MachineLearningOperator op)
		{
			this.Internal_DispatchMachineLearningOperator(op.m_Ptr);
		}

		public void GenerateMips(RenderTargetIdentifier rt)
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.Internal_GenerateMips(rt);
		}

		public void GenerateMips(RenderTexture rt)
		{
			bool flag = rt == null;
			if (flag)
			{
				throw new ArgumentNullException("rt");
			}
			this.GenerateMips(new RenderTargetIdentifier(rt));
		}

		public void ResolveAntiAliasedSurface(RenderTexture rt, RenderTexture target = null)
		{
			bool flag = rt == null;
			if (flag)
			{
				throw new ArgumentNullException("rt");
			}
			this.Internal_ResolveAntiAliasedSurface(rt, target);
		}

		public void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, [DefaultValue("0")] int submeshIndex, [DefaultValue("-1")] int shaderPass, [DefaultValue("null")] MaterialPropertyBlock properties)
		{
			bool flag = mesh == null;
			if (flag)
			{
				throw new ArgumentNullException("mesh");
			}
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			bool flag2 = submeshIndex < 0 || submeshIndex >= mesh.subMeshCount;
			if (flag2)
			{
				submeshIndex = Mathf.Clamp(submeshIndex, 0, mesh.subMeshCount - 1);
				Debug.LogWarning(string.Format("submeshIndex out of range. Clampped to {0}.", submeshIndex));
			}
			bool flag3 = material == null;
			if (flag3)
			{
				throw new ArgumentNullException("material");
			}
			this.Internal_DrawMesh(mesh, matrix, material, submeshIndex, shaderPass, properties);
		}

		[ExcludeFromDocs]
		public void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int submeshIndex, int shaderPass)
		{
			this.DrawMesh(mesh, matrix, material, submeshIndex, shaderPass, null);
		}

		[ExcludeFromDocs]
		public void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int submeshIndex)
		{
			this.DrawMesh(mesh, matrix, material, submeshIndex, -1);
		}

		[ExcludeFromDocs]
		public void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material)
		{
			this.DrawMesh(mesh, matrix, material, 0);
		}

		[ExcludeFromDocs]
		public void DrawMultipleMeshes(Matrix4x4[] matrices, Mesh[] meshes, int[] subsetIndices, int count, Material material, int shaderPass, [DefaultValue("null")] MaterialPropertyBlock properties)
		{
			bool flag = matrices.Length != meshes.Length || matrices.Length != subsetIndices.Length;
			if (flag)
			{
				throw new InvalidOperationException("matrices, meshes, subsetIndices must be of same length and must be valid");
			}
			bool flag2 = count < 1;
			if (flag2)
			{
				throw new InvalidOperationException("count must be atleast 1");
			}
			this.Internal_DrawMultipleMeshes(matrices, meshes, subsetIndices, count, material, shaderPass, properties);
		}

		public void DrawRenderer(Renderer renderer, Material material, [DefaultValue("0")] int submeshIndex, [DefaultValue("-1")] int shaderPass)
		{
			bool flag = renderer == null;
			if (flag)
			{
				throw new ArgumentNullException("renderer");
			}
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			bool flag2 = submeshIndex < 0;
			if (flag2)
			{
				submeshIndex = Mathf.Max(submeshIndex, 0);
				Debug.LogWarning(string.Format("submeshIndex out of range. Clampped to {0}.", submeshIndex));
			}
			bool flag3 = material == null;
			if (flag3)
			{
				throw new ArgumentNullException("material");
			}
			this.Internal_DrawRenderer(renderer, material, submeshIndex, shaderPass);
		}

		[ExcludeFromDocs]
		public void DrawRenderer(Renderer renderer, Material material, int submeshIndex)
		{
			this.DrawRenderer(renderer, material, submeshIndex, -1);
		}

		[ExcludeFromDocs]
		public void DrawRenderer(Renderer renderer, Material material)
		{
			this.DrawRenderer(renderer, material, 0);
		}

		public void DrawRendererList(RendererList rendererList)
		{
			this.Internal_DrawRendererList(rendererList);
		}

		public void DrawProcedural(Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, int vertexCount, [DefaultValue("1")] int instanceCount, [DefaultValue("null")] MaterialPropertyBlock properties)
		{
			bool flag = material == null;
			if (flag)
			{
				throw new ArgumentNullException("material");
			}
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.Internal_DrawProcedural(matrix, material, shaderPass, topology, vertexCount, instanceCount, properties);
		}

		[ExcludeFromDocs]
		public void DrawProcedural(Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, int vertexCount, int instanceCount)
		{
			this.DrawProcedural(matrix, material, shaderPass, topology, vertexCount, instanceCount, null);
		}

		[ExcludeFromDocs]
		public void DrawProcedural(Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, int vertexCount)
		{
			this.DrawProcedural(matrix, material, shaderPass, topology, vertexCount, 1);
		}

		public void DrawProcedural(GraphicsBuffer indexBuffer, Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, int indexCount, int instanceCount, MaterialPropertyBlock properties)
		{
			bool flag = indexBuffer == null;
			if (flag)
			{
				throw new ArgumentNullException("indexBuffer");
			}
			bool flag2 = material == null;
			if (flag2)
			{
				throw new ArgumentNullException("material");
			}
			this.Internal_DrawProceduralIndexed(indexBuffer, matrix, material, shaderPass, topology, indexCount, instanceCount, properties);
		}

		public void DrawProcedural(GraphicsBuffer indexBuffer, Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, int indexCount, int instanceCount)
		{
			this.DrawProcedural(indexBuffer, matrix, material, shaderPass, topology, indexCount, instanceCount, null);
		}

		public void DrawProcedural(GraphicsBuffer indexBuffer, Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, int indexCount)
		{
			this.DrawProcedural(indexBuffer, matrix, material, shaderPass, topology, indexCount, 1);
		}

		public void DrawProceduralIndirect(Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties)
		{
			bool flag = !SystemInfo.supportsIndirectArgumentsBuffer;
			if (flag)
			{
				throw new InvalidOperationException("Indirect argument buffers are not supported.");
			}
			bool flag2 = material == null;
			if (flag2)
			{
				throw new ArgumentNullException("material");
			}
			bool flag3 = bufferWithArgs == null;
			if (flag3)
			{
				throw new ArgumentNullException("bufferWithArgs");
			}
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.Internal_DrawProceduralIndirect(matrix, material, shaderPass, topology, bufferWithArgs, argsOffset, properties);
		}

		public void DrawProceduralIndirect(Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, ComputeBuffer bufferWithArgs, int argsOffset)
		{
			this.DrawProceduralIndirect(matrix, material, shaderPass, topology, bufferWithArgs, argsOffset, null);
		}

		public void DrawProceduralIndirect(Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, ComputeBuffer bufferWithArgs)
		{
			this.DrawProceduralIndirect(matrix, material, shaderPass, topology, bufferWithArgs, 0);
		}

		public void DrawProceduralIndirect(GraphicsBuffer indexBuffer, Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties)
		{
			bool flag = !SystemInfo.supportsIndirectArgumentsBuffer;
			if (flag)
			{
				throw new InvalidOperationException("Indirect argument buffers are not supported.");
			}
			bool flag2 = indexBuffer == null;
			if (flag2)
			{
				throw new ArgumentNullException("indexBuffer");
			}
			bool flag3 = material == null;
			if (flag3)
			{
				throw new ArgumentNullException("material");
			}
			bool flag4 = bufferWithArgs == null;
			if (flag4)
			{
				throw new ArgumentNullException("bufferWithArgs");
			}
			this.Internal_DrawProceduralIndexedIndirect(indexBuffer, matrix, material, shaderPass, topology, bufferWithArgs, argsOffset, properties);
		}

		public void DrawProceduralIndirect(GraphicsBuffer indexBuffer, Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, ComputeBuffer bufferWithArgs, int argsOffset)
		{
			this.DrawProceduralIndirect(indexBuffer, matrix, material, shaderPass, topology, bufferWithArgs, argsOffset, null);
		}

		public void DrawProceduralIndirect(GraphicsBuffer indexBuffer, Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, ComputeBuffer bufferWithArgs)
		{
			this.DrawProceduralIndirect(indexBuffer, matrix, material, shaderPass, topology, bufferWithArgs, 0);
		}

		public void DrawProceduralIndirect(Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, GraphicsBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties)
		{
			bool flag = !SystemInfo.supportsIndirectArgumentsBuffer;
			if (flag)
			{
				throw new InvalidOperationException("Indirect argument buffers are not supported.");
			}
			bool flag2 = material == null;
			if (flag2)
			{
				throw new ArgumentNullException("material");
			}
			bool flag3 = bufferWithArgs == null;
			if (flag3)
			{
				throw new ArgumentNullException("bufferWithArgs");
			}
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.Internal_DrawProceduralIndirectGraphicsBuffer(matrix, material, shaderPass, topology, bufferWithArgs, argsOffset, properties);
		}

		public void DrawProceduralIndirect(Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, GraphicsBuffer bufferWithArgs, int argsOffset)
		{
			this.DrawProceduralIndirect(matrix, material, shaderPass, topology, bufferWithArgs, argsOffset, null);
		}

		public void DrawProceduralIndirect(Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, GraphicsBuffer bufferWithArgs)
		{
			this.DrawProceduralIndirect(matrix, material, shaderPass, topology, bufferWithArgs, 0);
		}

		public void DrawProceduralIndirect(GraphicsBuffer indexBuffer, Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, GraphicsBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties)
		{
			bool flag = !SystemInfo.supportsIndirectArgumentsBuffer;
			if (flag)
			{
				throw new InvalidOperationException("Indirect argument buffers are not supported.");
			}
			bool flag2 = indexBuffer == null;
			if (flag2)
			{
				throw new ArgumentNullException("indexBuffer");
			}
			bool flag3 = material == null;
			if (flag3)
			{
				throw new ArgumentNullException("material");
			}
			bool flag4 = bufferWithArgs == null;
			if (flag4)
			{
				throw new ArgumentNullException("bufferWithArgs");
			}
			this.Internal_DrawProceduralIndexedIndirectGraphicsBuffer(indexBuffer, matrix, material, shaderPass, topology, bufferWithArgs, argsOffset, properties);
		}

		public void DrawProceduralIndirect(GraphicsBuffer indexBuffer, Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, GraphicsBuffer bufferWithArgs, int argsOffset)
		{
			this.DrawProceduralIndirect(indexBuffer, matrix, material, shaderPass, topology, bufferWithArgs, argsOffset, null);
		}

		public void DrawProceduralIndirect(GraphicsBuffer indexBuffer, Matrix4x4 matrix, Material material, int shaderPass, MeshTopology topology, GraphicsBuffer bufferWithArgs)
		{
			this.DrawProceduralIndirect(indexBuffer, matrix, material, shaderPass, topology, bufferWithArgs, 0);
		}

		public void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, int shaderPass, Matrix4x4[] matrices, int count, MaterialPropertyBlock properties)
		{
			bool flag = !SystemInfo.supportsInstancing;
			if (flag)
			{
				throw new InvalidOperationException("DrawMeshInstanced is not supported.");
			}
			bool flag2 = mesh == null;
			if (flag2)
			{
				throw new ArgumentNullException("mesh");
			}
			bool flag3 = submeshIndex < 0 || submeshIndex >= mesh.subMeshCount;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException("submeshIndex", "submeshIndex out of range.");
			}
			bool flag4 = material == null;
			if (flag4)
			{
				throw new ArgumentNullException("material");
			}
			bool flag5 = !material.enableInstancing;
			if (flag5)
			{
				throw new InvalidOperationException("Material needs to enable instancing for use with DrawMeshInstanced.");
			}
			bool flag6 = matrices == null;
			if (flag6)
			{
				throw new ArgumentNullException("matrices");
			}
			bool flag7 = count < 0 || count > Mathf.Min(Graphics.kMaxDrawMeshInstanceCount, matrices.Length);
			if (flag7)
			{
				throw new ArgumentOutOfRangeException("count", string.Format("Count must be in the range of 0 to {0}.", Mathf.Min(Graphics.kMaxDrawMeshInstanceCount, matrices.Length)));
			}
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			bool flag8 = count > 0;
			if (flag8)
			{
				this.Internal_DrawMeshInstanced(mesh, submeshIndex, material, shaderPass, matrices, count, properties);
			}
		}

		public void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, int shaderPass, Matrix4x4[] matrices, int count)
		{
			this.DrawMeshInstanced(mesh, submeshIndex, material, shaderPass, matrices, count, null);
		}

		public void DrawMeshInstanced(Mesh mesh, int submeshIndex, Material material, int shaderPass, Matrix4x4[] matrices)
		{
			this.DrawMeshInstanced(mesh, submeshIndex, material, shaderPass, matrices, matrices.Length);
		}

		public void DrawMeshInstancedProcedural(Mesh mesh, int submeshIndex, Material material, int shaderPass, int count, MaterialPropertyBlock properties = null)
		{
			bool flag = !SystemInfo.supportsInstancing;
			if (flag)
			{
				throw new InvalidOperationException("DrawMeshInstancedProcedural is not supported.");
			}
			bool flag2 = mesh == null;
			if (flag2)
			{
				throw new ArgumentNullException("mesh");
			}
			bool flag3 = submeshIndex < 0 || submeshIndex >= mesh.subMeshCount;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException("submeshIndex", "submeshIndex out of range.");
			}
			bool flag4 = material == null;
			if (flag4)
			{
				throw new ArgumentNullException("material");
			}
			bool flag5 = count <= 0;
			if (flag5)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			bool flag6 = count > 0;
			if (flag6)
			{
				this.Internal_DrawMeshInstancedProcedural(mesh, submeshIndex, material, shaderPass, count, properties);
			}
		}

		public void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, int shaderPass, ComputeBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties)
		{
			bool flag = !SystemInfo.supportsInstancing;
			if (flag)
			{
				throw new InvalidOperationException("Instancing is not supported.");
			}
			bool flag2 = !SystemInfo.supportsIndirectArgumentsBuffer;
			if (flag2)
			{
				throw new InvalidOperationException("Indirect argument buffers are not supported.");
			}
			bool flag3 = mesh == null;
			if (flag3)
			{
				throw new ArgumentNullException("mesh");
			}
			bool flag4 = submeshIndex < 0 || submeshIndex >= mesh.subMeshCount;
			if (flag4)
			{
				throw new ArgumentOutOfRangeException("submeshIndex", "submeshIndex out of range.");
			}
			bool flag5 = material == null;
			if (flag5)
			{
				throw new ArgumentNullException("material");
			}
			bool flag6 = bufferWithArgs == null;
			if (flag6)
			{
				throw new ArgumentNullException("bufferWithArgs");
			}
			this.Internal_DrawMeshInstancedIndirect(mesh, submeshIndex, material, shaderPass, bufferWithArgs, argsOffset, properties);
		}

		public void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, int shaderPass, ComputeBuffer bufferWithArgs, int argsOffset)
		{
			this.DrawMeshInstancedIndirect(mesh, submeshIndex, material, shaderPass, bufferWithArgs, argsOffset, null);
		}

		public void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, int shaderPass, ComputeBuffer bufferWithArgs)
		{
			this.DrawMeshInstancedIndirect(mesh, submeshIndex, material, shaderPass, bufferWithArgs, 0, null);
		}

		public void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, int shaderPass, GraphicsBuffer bufferWithArgs, int argsOffset, MaterialPropertyBlock properties)
		{
			bool flag = !SystemInfo.supportsInstancing;
			if (flag)
			{
				throw new InvalidOperationException("Instancing is not supported.");
			}
			bool flag2 = !SystemInfo.supportsIndirectArgumentsBuffer;
			if (flag2)
			{
				throw new InvalidOperationException("Indirect argument buffers are not supported.");
			}
			bool flag3 = mesh == null;
			if (flag3)
			{
				throw new ArgumentNullException("mesh");
			}
			bool flag4 = submeshIndex < 0 || submeshIndex >= mesh.subMeshCount;
			if (flag4)
			{
				throw new ArgumentOutOfRangeException("submeshIndex", "submeshIndex out of range.");
			}
			bool flag5 = material == null;
			if (flag5)
			{
				throw new ArgumentNullException("material");
			}
			bool flag6 = bufferWithArgs == null;
			if (flag6)
			{
				throw new ArgumentNullException("bufferWithArgs");
			}
			this.Internal_DrawMeshInstancedIndirectGraphicsBuffer(mesh, submeshIndex, material, shaderPass, bufferWithArgs, argsOffset, properties);
		}

		public void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, int shaderPass, GraphicsBuffer bufferWithArgs, int argsOffset)
		{
			this.DrawMeshInstancedIndirect(mesh, submeshIndex, material, shaderPass, bufferWithArgs, argsOffset, null);
		}

		public void DrawMeshInstancedIndirect(Mesh mesh, int submeshIndex, Material material, int shaderPass, GraphicsBuffer bufferWithArgs)
		{
			this.DrawMeshInstancedIndirect(mesh, submeshIndex, material, shaderPass, bufferWithArgs, 0, null);
		}

		public void DrawOcclusionMesh(RectInt normalizedCamViewport)
		{
			this.Internal_DrawOcclusionMesh(normalizedCamViewport);
		}

		public void SetRandomWriteTarget(int index, RenderTargetIdentifier rt)
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.SetRandomWriteTarget_Texture(index, ref rt);
		}

		public void SetRandomWriteTarget(int index, ComputeBuffer buffer, bool preserveCounterValue)
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.SetRandomWriteTarget_Buffer(index, buffer, preserveCounterValue);
		}

		public void SetRandomWriteTarget(int index, ComputeBuffer buffer)
		{
			this.SetRandomWriteTarget(index, buffer, false);
		}

		public void SetRandomWriteTarget(int index, GraphicsBuffer buffer, bool preserveCounterValue)
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.SetRandomWriteTarget_GraphicsBuffer(index, buffer, preserveCounterValue);
		}

		public void SetRandomWriteTarget(int index, GraphicsBuffer buffer)
		{
			this.SetRandomWriteTarget(index, buffer, false);
		}

		public void CopyCounterValue(ComputeBuffer src, ComputeBuffer dst, uint dstOffsetBytes)
		{
			this.CopyCounterValueCC(src, dst, dstOffsetBytes);
		}

		public void CopyCounterValue(GraphicsBuffer src, ComputeBuffer dst, uint dstOffsetBytes)
		{
			this.CopyCounterValueGC(src, dst, dstOffsetBytes);
		}

		public void CopyCounterValue(ComputeBuffer src, GraphicsBuffer dst, uint dstOffsetBytes)
		{
			this.CopyCounterValueCG(src, dst, dstOffsetBytes);
		}

		public void CopyCounterValue(GraphicsBuffer src, GraphicsBuffer dst, uint dstOffsetBytes)
		{
			this.CopyCounterValueGG(src, dst, dstOffsetBytes);
		}

		public void CopyTexture(RenderTargetIdentifier src, RenderTargetIdentifier dst)
		{
			this.CopyTexture_Internal(ref src, -1, -1, -1, -1, -1, -1, ref dst, -1, -1, -1, -1, 1);
		}

		public void CopyTexture(RenderTargetIdentifier src, int srcElement, RenderTargetIdentifier dst, int dstElement)
		{
			this.CopyTexture_Internal(ref src, srcElement, -1, -1, -1, -1, -1, ref dst, dstElement, -1, -1, -1, 2);
		}

		public void CopyTexture(RenderTargetIdentifier src, int srcElement, int srcMip, RenderTargetIdentifier dst, int dstElement, int dstMip)
		{
			this.CopyTexture_Internal(ref src, srcElement, srcMip, -1, -1, -1, -1, ref dst, dstElement, dstMip, -1, -1, 3);
		}

		public void CopyTexture(RenderTargetIdentifier src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, RenderTargetIdentifier dst, int dstElement, int dstMip, int dstX, int dstY)
		{
			this.CopyTexture_Internal(ref src, srcElement, srcMip, srcX, srcY, srcWidth, srcHeight, ref dst, dstElement, dstMip, dstX, dstY, 4);
		}

		public void Blit(Texture source, RenderTargetIdentifier dest)
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.Blit_Texture(source, ref dest, null, -1, new Vector2(1f, 1f), new Vector2(0f, 0f), Texture2DArray.allSlices, 0);
		}

		public void Blit(Texture source, RenderTargetIdentifier dest, Vector2 scale, Vector2 offset)
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.Blit_Texture(source, ref dest, null, -1, scale, offset, Texture2DArray.allSlices, 0);
		}

		public void Blit(Texture source, RenderTargetIdentifier dest, Material mat)
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.Blit_Texture(source, ref dest, mat, -1, new Vector2(1f, 1f), new Vector2(0f, 0f), Texture2DArray.allSlices, 0);
		}

		public void Blit(Texture source, RenderTargetIdentifier dest, Material mat, int pass)
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.Blit_Texture(source, ref dest, mat, pass, new Vector2(1f, 1f), new Vector2(0f, 0f), Texture2DArray.allSlices, 0);
		}

		public void Blit(RenderTargetIdentifier source, RenderTargetIdentifier dest)
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.Blit_Identifier(ref source, ref dest, null, -1, new Vector2(1f, 1f), new Vector2(0f, 0f), Texture2DArray.allSlices, 0);
		}

		public void Blit(RenderTargetIdentifier source, RenderTargetIdentifier dest, Vector2 scale, Vector2 offset)
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.Blit_Identifier(ref source, ref dest, null, -1, scale, offset, Texture2DArray.allSlices, 0);
		}

		public void Blit(RenderTargetIdentifier source, RenderTargetIdentifier dest, Material mat)
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.Blit_Identifier(ref source, ref dest, mat, -1, new Vector2(1f, 1f), new Vector2(0f, 0f), Texture2DArray.allSlices, 0);
		}

		public void Blit(RenderTargetIdentifier source, RenderTargetIdentifier dest, Material mat, int pass)
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.Blit_Identifier(ref source, ref dest, mat, pass, new Vector2(1f, 1f), new Vector2(0f, 0f), Texture2DArray.allSlices, 0);
		}

		public void Blit(RenderTargetIdentifier source, RenderTargetIdentifier dest, int sourceDepthSlice, int destDepthSlice)
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.Blit_Identifier(ref source, ref dest, null, -1, new Vector2(1f, 1f), new Vector2(0f, 0f), sourceDepthSlice, destDepthSlice);
		}

		public void Blit(RenderTargetIdentifier source, RenderTargetIdentifier dest, Vector2 scale, Vector2 offset, int sourceDepthSlice, int destDepthSlice)
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.Blit_Identifier(ref source, ref dest, null, -1, scale, offset, sourceDepthSlice, destDepthSlice);
		}

		public void Blit(RenderTargetIdentifier source, RenderTargetIdentifier dest, Material mat, int pass, int destDepthSlice)
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.Blit_Identifier(ref source, ref dest, mat, pass, new Vector2(1f, 1f), new Vector2(0f, 0f), Texture2DArray.allSlices, destDepthSlice);
		}

		public void SetGlobalFloat(string name, float value)
		{
			this.SetGlobalFloat(Shader.PropertyToID(name), value);
		}

		public void SetGlobalInt(string name, int value)
		{
			this.SetGlobalInt(Shader.PropertyToID(name), value);
		}

		public void SetGlobalInteger(string name, int value)
		{
			this.SetGlobalInteger(Shader.PropertyToID(name), value);
		}

		public void SetGlobalVector(string name, Vector4 value)
		{
			this.SetGlobalVector(Shader.PropertyToID(name), value);
		}

		public void SetGlobalColor(string name, Color value)
		{
			this.SetGlobalColor(Shader.PropertyToID(name), value);
		}

		public void SetGlobalMatrix(string name, Matrix4x4 value)
		{
			this.SetGlobalMatrix(Shader.PropertyToID(name), value);
		}

		public void SetGlobalFloatArray(string propertyName, List<float> values)
		{
			this.SetGlobalFloatArray(Shader.PropertyToID(propertyName), values);
		}

		public void SetGlobalFloatArray(int nameID, List<float> values)
		{
			bool flag = values == null;
			if (flag)
			{
				throw new ArgumentNullException("values");
			}
			bool flag2 = values.Count == 0;
			if (flag2)
			{
				throw new ArgumentException("Zero-sized array is not allowed.");
			}
			this.SetGlobalFloatArrayListImpl(nameID, values);
		}

		public void SetGlobalFloatArray(string propertyName, float[] values)
		{
			this.SetGlobalFloatArray(Shader.PropertyToID(propertyName), values);
		}

		public void SetGlobalVectorArray(string propertyName, List<Vector4> values)
		{
			this.SetGlobalVectorArray(Shader.PropertyToID(propertyName), values);
		}

		public void SetGlobalVectorArray(int nameID, List<Vector4> values)
		{
			bool flag = values == null;
			if (flag)
			{
				throw new ArgumentNullException("values");
			}
			bool flag2 = values.Count == 0;
			if (flag2)
			{
				throw new ArgumentException("Zero-sized array is not allowed.");
			}
			this.SetGlobalVectorArrayListImpl(nameID, values);
		}

		public void SetGlobalVectorArray(string propertyName, Vector4[] values)
		{
			this.SetGlobalVectorArray(Shader.PropertyToID(propertyName), values);
		}

		public void SetGlobalMatrixArray(string propertyName, List<Matrix4x4> values)
		{
			this.SetGlobalMatrixArray(Shader.PropertyToID(propertyName), values);
		}

		public void SetGlobalMatrixArray(int nameID, List<Matrix4x4> values)
		{
			bool flag = values == null;
			if (flag)
			{
				throw new ArgumentNullException("values");
			}
			bool flag2 = values.Count == 0;
			if (flag2)
			{
				throw new ArgumentException("Zero-sized array is not allowed.");
			}
			this.SetGlobalMatrixArrayListImpl(nameID, values);
		}

		public void SetGlobalMatrixArray(string propertyName, Matrix4x4[] values)
		{
			this.SetGlobalMatrixArray(Shader.PropertyToID(propertyName), values);
		}

		public void SetGlobalTexture(string name, RenderTargetIdentifier value)
		{
			this.SetGlobalTexture(Shader.PropertyToID(name), value, RenderTextureSubElement.Default);
		}

		public void SetGlobalTexture(int nameID, RenderTargetIdentifier value)
		{
			this.SetGlobalTexture_Impl(nameID, ref value, RenderTextureSubElement.Default);
		}

		public void SetGlobalTexture(string name, RenderTargetIdentifier value, RenderTextureSubElement element)
		{
			this.SetGlobalTexture(Shader.PropertyToID(name), value, element);
		}

		public void SetGlobalTexture(int nameID, RenderTargetIdentifier value, RenderTextureSubElement element)
		{
			this.SetGlobalTexture_Impl(nameID, ref value, element);
		}

		public void SetGlobalBuffer(string name, ComputeBuffer value)
		{
			this.SetGlobalBufferInternal(Shader.PropertyToID(name), value);
		}

		public void SetGlobalBuffer(int nameID, ComputeBuffer value)
		{
			this.SetGlobalBufferInternal(nameID, value);
		}

		public void SetGlobalBuffer(string name, GraphicsBuffer value)
		{
			this.SetGlobalGraphicsBufferInternal(Shader.PropertyToID(name), value);
		}

		public void SetGlobalBuffer(int nameID, GraphicsBuffer value)
		{
			this.SetGlobalGraphicsBufferInternal(nameID, value);
		}

		public void SetGlobalConstantBuffer(ComputeBuffer buffer, int nameID, int offset, int size)
		{
			this.SetGlobalConstantBufferInternal(buffer, nameID, offset, size);
		}

		public void SetGlobalConstantBuffer(ComputeBuffer buffer, string name, int offset, int size)
		{
			this.SetGlobalConstantBufferInternal(buffer, Shader.PropertyToID(name), offset, size);
		}

		public void SetGlobalConstantBuffer(GraphicsBuffer buffer, int nameID, int offset, int size)
		{
			this.SetGlobalConstantGraphicsBufferInternal(buffer, nameID, offset, size);
		}

		public void SetGlobalConstantBuffer(GraphicsBuffer buffer, string name, int offset, int size)
		{
			this.SetGlobalConstantGraphicsBufferInternal(buffer, Shader.PropertyToID(name), offset, size);
		}

		public void SetGlobalRayTracingAccelerationStructure(string name, RayTracingAccelerationStructure accelerationStructure)
		{
			this.SetGlobalRayTracingAccelerationStructureInternal(accelerationStructure, Shader.PropertyToID(name));
		}

		public void SetGlobalRayTracingAccelerationStructure(int nameID, RayTracingAccelerationStructure accelerationStructure)
		{
			this.SetGlobalRayTracingAccelerationStructureInternal(accelerationStructure, nameID);
		}

		public void SetShadowSamplingMode(RenderTargetIdentifier shadowmap, ShadowSamplingMode mode)
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.SetShadowSamplingMode_Impl(ref shadowmap, mode);
		}

		public void SetSinglePassStereo(SinglePassStereoMode mode)
		{
			this.Internal_SetSinglePassStereo(mode);
		}

		public void IssuePluginEvent(IntPtr callback, int eventID)
		{
			bool flag = callback == IntPtr.Zero;
			if (flag)
			{
				throw new ArgumentException("Null callback specified.");
			}
			this.IssuePluginEventInternal(callback, eventID);
		}

		public void IssuePluginEventAndData(IntPtr callback, int eventID, IntPtr data)
		{
			bool flag = callback == IntPtr.Zero;
			if (flag)
			{
				throw new ArgumentException("Null callback specified.");
			}
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.IssuePluginEventAndDataInternal(callback, eventID, data);
		}

		public void IssuePluginEventAndDataWithFlags(IntPtr callback, int eventID, CustomMarkerCallbackFlags flags, IntPtr data)
		{
			bool flag = callback == IntPtr.Zero;
			if (flag)
			{
				throw new ArgumentException("Null callback specified.");
			}
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.IssuePluginEventAndDataInternalWithFlags(callback, eventID, flags, data);
		}

		public void IssuePluginCustomBlit(IntPtr callback, uint command, RenderTargetIdentifier source, RenderTargetIdentifier dest, uint commandParam, uint commandFlags)
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.IssuePluginCustomBlitInternal(callback, command, ref source, ref dest, commandParam, commandFlags);
		}

		[Obsolete("Use IssuePluginCustomTextureUpdateV2 to register TextureUpdate callbacks instead. Callbacks will be passed event IDs kUnityRenderingExtEventUpdateTextureBeginV2 or kUnityRenderingExtEventUpdateTextureEndV2, and data parameter of type UnityRenderingExtTextureUpdateParamsV2.", false)]
		public void IssuePluginCustomTextureUpdate(IntPtr callback, Texture targetTexture, uint userData)
		{
			this.IssuePluginCustomTextureUpdateInternal(callback, targetTexture, userData, false);
		}

		[Obsolete("Use IssuePluginCustomTextureUpdateV2 to register TextureUpdate callbacks instead. Callbacks will be passed event IDs kUnityRenderingExtEventUpdateTextureBeginV2 or kUnityRenderingExtEventUpdateTextureEndV2, and data parameter of type UnityRenderingExtTextureUpdateParamsV2.", false)]
		public void IssuePluginCustomTextureUpdateV1(IntPtr callback, Texture targetTexture, uint userData)
		{
			this.IssuePluginCustomTextureUpdateInternal(callback, targetTexture, userData, false);
		}

		public void IssuePluginCustomTextureUpdateV2(IntPtr callback, Texture targetTexture, uint userData)
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.IssuePluginCustomTextureUpdateInternal(callback, targetTexture, userData, true);
		}

		public void ProcessVTFeedback(RenderTargetIdentifier rt, IntPtr resolver, int slice, int x, int width, int y, int height, int mip)
		{
			this.ValidateAgainstExecutionFlags(CommandBufferExecutionFlags.None, CommandBufferExecutionFlags.AsyncCompute);
			this.Internal_ProcessVTFeedback(rt, resolver, slice, x, width, y, height, mip);
		}

		public void CopyBuffer(GraphicsBuffer source, GraphicsBuffer dest)
		{
			Graphics.ValidateCopyBuffer(source, dest);
			this.CopyBufferImpl(source, dest);
		}

		[Obsolete("CommandBuffer.CreateGPUFence has been deprecated. Use CreateGraphicsFence instead (UnityUpgradable) -> CreateAsyncGraphicsFence(*)", false)]
		public GPUFence CreateGPUFence(SynchronisationStage stage)
		{
			return default(GPUFence);
		}

		[Obsolete("CommandBuffer.CreateGPUFence has been deprecated. Use CreateGraphicsFence instead (UnityUpgradable) -> CreateAsyncGraphicsFence()", false)]
		public GPUFence CreateGPUFence()
		{
			return default(GPUFence);
		}

		[Obsolete("CommandBuffer.WaitOnGPUFence has been deprecated. Use WaitOnGraphicsFence instead (UnityUpgradable) -> WaitOnAsyncGraphicsFence(*)", false)]
		public void WaitOnGPUFence(GPUFence fence, SynchronisationStage stage)
		{
		}

		[Obsolete("CommandBuffer.WaitOnGPUFence has been deprecated. Use WaitOnGraphicsFence instead (UnityUpgradable) -> WaitOnAsyncGraphicsFence(*)", false)]
		public void WaitOnGPUFence(GPUFence fence)
		{
		}

		[Obsolete("CommandBuffer.SetComputeBufferData has been deprecated. Use SetBufferData instead (UnityUpgradable) -> SetBufferData(*)", true)]
		public void SetComputeBufferData(ComputeBuffer buffer, Array data)
		{
		}

		[Obsolete("CommandBuffer.SetComputeBufferData has been deprecated. Use SetBufferData instead (UnityUpgradable) -> SetBufferData<T>(*)", true)]
		public void SetComputeBufferData<T>(ComputeBuffer buffer, List<T> data) where T : struct
		{
		}

		[Obsolete("CommandBuffer.SetComputeBufferData has been deprecated. Use SetBufferData instead (UnityUpgradable) -> SetBufferData<T>(*)", true)]
		public void SetComputeBufferData<T>(ComputeBuffer buffer, NativeArray<T> data) where T : struct
		{
		}

		[Obsolete("CommandBuffer.SetComputeBufferData has been deprecated. Use SetBufferData instead (UnityUpgradable) -> SetBufferData(*)", true)]
		public void SetComputeBufferData(ComputeBuffer buffer, Array data, int managedBufferStartIndex, int graphicsBufferStartIndex, int count)
		{
		}

		[Obsolete("CommandBuffer.SetComputeBufferData has been deprecated. Use SetBufferData instead (UnityUpgradable) -> SetBufferData<T>(*)", true)]
		public void SetComputeBufferData<T>(ComputeBuffer buffer, List<T> data, int managedBufferStartIndex, int graphicsBufferStartIndex, int count) where T : struct
		{
		}

		[Obsolete("CommandBuffer.SetComputeBufferData has been deprecated. Use SetBufferData instead (UnityUpgradable) -> SetBufferData<T>(*)", true)]
		public void SetComputeBufferData<T>(ComputeBuffer buffer, NativeArray<T> data, int nativeBufferStartIndex, int graphicsBufferStartIndex, int count) where T : struct
		{
		}

		[Obsolete("CommandBuffer.SetComputeBufferCounterValue has been deprecated. Use SetBufferCounterValue instead (UnityUpgradable) -> SetBufferCounterValue(*)", true)]
		public void SetComputeBufferCounterValue(ComputeBuffer buffer, uint counterValue)
		{
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void WaitAllAsyncReadbackRequests_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void Internal_RequestAsyncReadback_1_Injected(IntPtr _unity_self, IntPtr src, Action<AsyncGPUReadbackRequest> callback, AsyncRequestNativeArrayData* nativeArrayData);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void Internal_RequestAsyncReadback_2_Injected(IntPtr _unity_self, IntPtr src, int size, int offset, Action<AsyncGPUReadbackRequest> callback, AsyncRequestNativeArrayData* nativeArrayData);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void Internal_RequestAsyncReadback_3_Injected(IntPtr _unity_self, IntPtr src, Action<AsyncGPUReadbackRequest> callback, AsyncRequestNativeArrayData* nativeArrayData);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void Internal_RequestAsyncReadback_4_Injected(IntPtr _unity_self, IntPtr src, int mipIndex, Action<AsyncGPUReadbackRequest> callback, AsyncRequestNativeArrayData* nativeArrayData);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void Internal_RequestAsyncReadback_5_Injected(IntPtr _unity_self, IntPtr src, int mipIndex, GraphicsFormat dstFormat, Action<AsyncGPUReadbackRequest> callback, AsyncRequestNativeArrayData* nativeArrayData);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void Internal_RequestAsyncReadback_6_Injected(IntPtr _unity_self, IntPtr src, int mipIndex, int x, int width, int y, int height, int z, int depth, Action<AsyncGPUReadbackRequest> callback, AsyncRequestNativeArrayData* nativeArrayData);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void Internal_RequestAsyncReadback_7_Injected(IntPtr _unity_self, IntPtr src, int mipIndex, int x, int width, int y, int height, int z, int depth, GraphicsFormat dstFormat, Action<AsyncGPUReadbackRequest> callback, AsyncRequestNativeArrayData* nativeArrayData);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void Internal_RequestAsyncReadback_8_Injected(IntPtr _unity_self, IntPtr src, Action<AsyncGPUReadbackRequest> callback, AsyncRequestNativeArrayData* nativeArrayData);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void Internal_RequestAsyncReadback_9_Injected(IntPtr _unity_self, IntPtr src, int size, int offset, Action<AsyncGPUReadbackRequest> callback, AsyncRequestNativeArrayData* nativeArrayData);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetInvertCulling_Injected(IntPtr _unity_self, bool invertCulling);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ConvertTexture_Internal_Injected(IntPtr _unity_self, [In] ref RenderTargetIdentifier src, int srcElement, [In] ref RenderTargetIdentifier dst, int dstElement);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetSinglePassStereo_Injected(IntPtr _unity_self, SinglePassStereoMode mode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr CreateGPUFence_Internal_Injected(IntPtr _unity_self, GraphicsFenceType fenceType, SynchronisationStageFlags stage);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void WaitOnGPUFence_Internal_Injected(IntPtr _unity_self, IntPtr fencePtr, SynchronisationStageFlags stage);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReleaseBuffer_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetComputeFloatParam_Injected(IntPtr _unity_self, IntPtr computeShader, int nameID, float val);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetComputeIntParam_Injected(IntPtr _unity_self, IntPtr computeShader, int nameID, int val);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetComputeVectorParam_Injected(IntPtr _unity_self, IntPtr computeShader, int nameID, [In] ref Vector4 val);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetComputeVectorArrayParam_Injected(IntPtr _unity_self, IntPtr computeShader, int nameID, ref ManagedSpanWrapper values);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetComputeMatrixParam_Injected(IntPtr _unity_self, IntPtr computeShader, int nameID, [In] ref Matrix4x4 val);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetComputeMatrixArrayParam_Injected(IntPtr _unity_self, IntPtr computeShader, int nameID, ref ManagedSpanWrapper values);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetComputeFloats_Injected(IntPtr _unity_self, IntPtr computeShader, int nameID, ref ManagedSpanWrapper values);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetComputeInts_Injected(IntPtr _unity_self, IntPtr computeShader, int nameID, ref ManagedSpanWrapper values);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetComputeTextureParam_Injected(IntPtr _unity_self, IntPtr computeShader, int kernelIndex, int nameID, ref RenderTargetIdentifier rt, int mipLevel, RenderTextureSubElement element);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetComputeBufferParam_Injected(IntPtr _unity_self, IntPtr computeShader, int kernelIndex, int nameID, IntPtr buffer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetComputeGraphicsBufferHandleParam_Injected(IntPtr _unity_self, IntPtr computeShader, int kernelIndex, int nameID, [In] ref GraphicsBufferHandle bufferHandle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetComputeGraphicsBufferParam_Injected(IntPtr _unity_self, IntPtr computeShader, int kernelIndex, int nameID, IntPtr buffer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetComputeConstantComputeBufferParam_Injected(IntPtr _unity_self, IntPtr computeShader, int nameID, IntPtr buffer, int offset, int size);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetComputeConstantGraphicsBufferParam_Injected(IntPtr _unity_self, IntPtr computeShader, int nameID, IntPtr buffer, int offset, int size);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetComputeParamsFromMaterial_Injected(IntPtr _unity_self, IntPtr computeShader, int kernelIndex, IntPtr material);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_DispatchCompute_Injected(IntPtr _unity_self, IntPtr computeShader, int kernelIndex, int threadGroupsX, int threadGroupsY, int threadGroupsZ);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_DispatchComputeIndirect_Injected(IntPtr _unity_self, IntPtr computeShader, int kernelIndex, IntPtr indirectBuffer, uint argsOffset);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_DispatchComputeIndirectGraphicsBuffer_Injected(IntPtr _unity_self, IntPtr computeShader, int kernelIndex, IntPtr indirectBuffer, uint argsOffset);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetRayTracingComputeBufferParam_Injected(IntPtr _unity_self, IntPtr rayTracingShader, int nameID, IntPtr buffer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetRayTracingGraphicsBufferParam_Injected(IntPtr _unity_self, IntPtr rayTracingShader, int nameID, IntPtr buffer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetRayTracingGraphicsBufferHandleParam_Injected(IntPtr _unity_self, IntPtr rayTracingShader, int nameID, [In] ref GraphicsBufferHandle bufferHandle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetRayTracingConstantComputeBufferParam_Injected(IntPtr _unity_self, IntPtr rayTracingShader, int nameID, IntPtr buffer, int offset, int size);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetRayTracingConstantGraphicsBufferParam_Injected(IntPtr _unity_self, IntPtr rayTracingShader, int nameID, IntPtr buffer, int offset, int size);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetRayTracingTextureParam_Injected(IntPtr _unity_self, IntPtr rayTracingShader, int nameID, ref RenderTargetIdentifier rt);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetRayTracingFloatParam_Injected(IntPtr _unity_self, IntPtr rayTracingShader, int nameID, float val);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetRayTracingIntParam_Injected(IntPtr _unity_self, IntPtr rayTracingShader, int nameID, int val);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetRayTracingVectorParam_Injected(IntPtr _unity_self, IntPtr rayTracingShader, int nameID, [In] ref Vector4 val);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetRayTracingVectorArrayParam_Injected(IntPtr _unity_self, IntPtr rayTracingShader, int nameID, ref ManagedSpanWrapper values);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetRayTracingMatrixParam_Injected(IntPtr _unity_self, IntPtr rayTracingShader, int nameID, [In] ref Matrix4x4 val);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetRayTracingMatrixArrayParam_Injected(IntPtr _unity_self, IntPtr rayTracingShader, int nameID, ref ManagedSpanWrapper values);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetRayTracingFloats_Injected(IntPtr _unity_self, IntPtr rayTracingShader, int nameID, ref ManagedSpanWrapper values);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetRayTracingInts_Injected(IntPtr _unity_self, IntPtr rayTracingShader, int nameID, ref ManagedSpanWrapper values);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_BuildRayTracingAccelerationStructure_Injected(IntPtr _unity_self, IntPtr accelerationStructure, [In] ref RayTracingAccelerationStructure.BuildSettings buildSettings);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetRayTracingAccelerationStructure_Injected(IntPtr _unity_self, IntPtr rayTracingShader, int nameID, IntPtr accelerationStructure);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetComputeRayTracingAccelerationStructure_Injected(IntPtr _unity_self, IntPtr computeShader, int kernelIndex, int nameID, IntPtr accelerationStructure);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetRayTracingShaderPass_Injected(IntPtr _unity_self, IntPtr rayTracingShader, ref ManagedSpanWrapper passName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_DispatchRays_Injected(IntPtr _unity_self, IntPtr rayTracingShader, ref ManagedSpanWrapper rayGenShaderName, uint width, uint height, uint depth, IntPtr camera);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_DispatchRaysIndirect_Injected(IntPtr _unity_self, IntPtr rayTracingShader, ref ManagedSpanWrapper rayGenShaderName, IntPtr argsBuffer, uint argsOffset, IntPtr camera);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_BuildMachineLearningOperator_Injected(IntPtr _unity_self, IntPtr machineLearningOperator);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetMachineLearningOperatorTensors_Injected(IntPtr _unity_self, IntPtr machineLearningOperator, ref ManagedSpanWrapper inputs, ref ManagedSpanWrapper outputs);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_DispatchMachineLearningOperator_Injected(IntPtr _unity_self, IntPtr machineLearningOperator);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_GenerateMips_Injected(IntPtr _unity_self, [In] ref RenderTargetIdentifier rt);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_ResolveAntiAliasedSurface_Injected(IntPtr _unity_self, IntPtr rt, IntPtr target);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CopyCounterValueCC_Injected(IntPtr _unity_self, IntPtr src, IntPtr dst, uint dstOffsetBytes);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CopyCounterValueGC_Injected(IntPtr _unity_self, IntPtr src, IntPtr dst, uint dstOffsetBytes);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CopyCounterValueCG_Injected(IntPtr _unity_self, IntPtr src, IntPtr dst, uint dstOffsetBytes);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CopyCounterValueGG_Injected(IntPtr _unity_self, IntPtr src, IntPtr dst, uint dstOffsetBytes);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_name_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_name_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_sizeInBytes_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Clear_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_DrawMesh_Injected(IntPtr _unity_self, IntPtr mesh, [In] ref Matrix4x4 matrix, IntPtr material, int submeshIndex, int shaderPass, IntPtr properties);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_DrawMultipleMeshes_Injected(IntPtr _unity_self, ref ManagedSpanWrapper matrices, Mesh[] meshes, ref ManagedSpanWrapper subsetIndices, int count, IntPtr material, int shaderPass, IntPtr properties);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_DrawRenderer_Injected(IntPtr _unity_self, IntPtr renderer, IntPtr material, int submeshIndex, int shaderPass);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_DrawRendererList_Injected(IntPtr _unity_self, [In] ref RendererList rendererList);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_DrawProcedural_Injected(IntPtr _unity_self, [In] ref Matrix4x4 matrix, IntPtr material, int shaderPass, MeshTopology topology, int vertexCount, int instanceCount, IntPtr properties);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_DrawProceduralIndexed_Injected(IntPtr _unity_self, IntPtr indexBuffer, [In] ref Matrix4x4 matrix, IntPtr material, int shaderPass, MeshTopology topology, int indexCount, int instanceCount, IntPtr properties);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_DrawProceduralIndirect_Injected(IntPtr _unity_self, [In] ref Matrix4x4 matrix, IntPtr material, int shaderPass, MeshTopology topology, IntPtr bufferWithArgs, int argsOffset, IntPtr properties);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_DrawProceduralIndexedIndirect_Injected(IntPtr _unity_self, IntPtr indexBuffer, [In] ref Matrix4x4 matrix, IntPtr material, int shaderPass, MeshTopology topology, IntPtr bufferWithArgs, int argsOffset, IntPtr properties);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_DrawProceduralIndirectGraphicsBuffer_Injected(IntPtr _unity_self, [In] ref Matrix4x4 matrix, IntPtr material, int shaderPass, MeshTopology topology, IntPtr bufferWithArgs, int argsOffset, IntPtr properties);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_DrawProceduralIndexedIndirectGraphicsBuffer_Injected(IntPtr _unity_self, IntPtr indexBuffer, [In] ref Matrix4x4 matrix, IntPtr material, int shaderPass, MeshTopology topology, IntPtr bufferWithArgs, int argsOffset, IntPtr properties);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_DrawMeshInstanced_Injected(IntPtr _unity_self, IntPtr mesh, int submeshIndex, IntPtr material, int shaderPass, ref ManagedSpanWrapper matrices, int count, IntPtr properties);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_DrawMeshInstancedProcedural_Injected(IntPtr _unity_self, IntPtr mesh, int submeshIndex, IntPtr material, int shaderPass, int count, IntPtr properties);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_DrawMeshInstancedIndirect_Injected(IntPtr _unity_self, IntPtr mesh, int submeshIndex, IntPtr material, int shaderPass, IntPtr bufferWithArgs, int argsOffset, IntPtr properties);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_DrawMeshInstancedIndirectGraphicsBuffer_Injected(IntPtr _unity_self, IntPtr mesh, int submeshIndex, IntPtr material, int shaderPass, IntPtr bufferWithArgs, int argsOffset, IntPtr properties);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_DrawOcclusionMesh_Injected(IntPtr _unity_self, [In] ref RectInt normalizedCamViewport);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetRandomWriteTarget_Texture_Injected(IntPtr _unity_self, int index, ref RenderTargetIdentifier rt);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetRandomWriteTarget_Buffer_Injected(IntPtr _unity_self, int index, IntPtr uav, bool preserveCounterValue);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetRandomWriteTarget_GraphicsBuffer_Injected(IntPtr _unity_self, int index, IntPtr uav, bool preserveCounterValue);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ClearRandomWriteTargets_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetViewport_Injected(IntPtr _unity_self, [In] ref Rect pixelRect);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EnableScissorRect_Injected(IntPtr _unity_self, [In] ref Rect scissor);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DisableScissorRect_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CopyTexture_Internal_Injected(IntPtr _unity_self, ref RenderTargetIdentifier src, int srcElement, int srcMip, int srcX, int srcY, int srcWidth, int srcHeight, ref RenderTargetIdentifier dst, int dstElement, int dstMip, int dstX, int dstY, int mode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Blit_Texture_Injected(IntPtr _unity_self, IntPtr source, ref RenderTargetIdentifier dest, IntPtr mat, int pass, [In] ref Vector2 scale, [In] ref Vector2 offset, int sourceDepthSlice, int destDepthSlice);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Blit_Identifier_Injected(IntPtr _unity_self, ref RenderTargetIdentifier source, ref RenderTargetIdentifier dest, IntPtr mat, int pass, [In] ref Vector2 scale, [In] ref Vector2 offset, int sourceDepthSlice, int destDepthSlice);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetTemporaryRT_Injected(IntPtr _unity_self, int nameID, int width, int height, FilterMode filter, GraphicsFormat colorFormat, GraphicsFormat depthStencilFormat, int antiAliasing, bool enableRandomWrite, RenderTextureMemoryless memorylessMode, bool useDynamicScale, ShadowSamplingMode shadowSamplingMode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetTemporaryRTWithDescriptor_Injected(IntPtr _unity_self, int nameID, [In] ref RenderTextureDescriptor desc, FilterMode filter);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetTemporaryRTArray_Injected(IntPtr _unity_self, int nameID, int width, int height, int slices, FilterMode filter, GraphicsFormat colorFormat, GraphicsFormat depthStencilFormat, int antiAliasing, bool enableRandomWrite, bool useDynamicScale, ShadowSamplingMode shadowSamplingMode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReleaseTemporaryRT_Injected(IntPtr _unity_self, int nameID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalFloat_Injected(IntPtr _unity_self, int nameID, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalInt_Injected(IntPtr _unity_self, int nameID, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalInteger_Injected(IntPtr _unity_self, int nameID, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalVector_Injected(IntPtr _unity_self, int nameID, [In] ref Vector4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalColor_Injected(IntPtr _unity_self, int nameID, [In] ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalMatrix_Injected(IntPtr _unity_self, int nameID, [In] ref Matrix4x4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EnableShaderKeyword_Injected(IntPtr _unity_self, ref ManagedSpanWrapper keyword);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EnableGlobalKeyword_Injected(IntPtr _unity_self, [In] ref GlobalKeyword keyword);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EnableMaterialKeyword_Injected(IntPtr _unity_self, IntPtr material, [In] ref LocalKeyword keyword);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EnableComputeKeyword_Injected(IntPtr _unity_self, IntPtr computeShader, [In] ref LocalKeyword keyword);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EnableRayTracingKeyword_Injected(IntPtr _unity_self, IntPtr rayTracingShader, [In] ref LocalKeyword keyword);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DisableShaderKeyword_Injected(IntPtr _unity_self, ref ManagedSpanWrapper keyword);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DisableGlobalKeyword_Injected(IntPtr _unity_self, [In] ref GlobalKeyword keyword);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DisableMaterialKeyword_Injected(IntPtr _unity_self, IntPtr material, [In] ref LocalKeyword keyword);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DisableComputeKeyword_Injected(IntPtr _unity_self, IntPtr computeShader, [In] ref LocalKeyword keyword);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DisableRayTracingKeyword_Injected(IntPtr _unity_self, IntPtr rayTracingShader, [In] ref LocalKeyword keyword);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalKeyword_Injected(IntPtr _unity_self, [In] ref GlobalKeyword keyword, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetMaterialKeyword_Injected(IntPtr _unity_self, IntPtr material, [In] ref LocalKeyword keyword, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetComputeKeyword_Injected(IntPtr _unity_self, IntPtr computeShader, [In] ref LocalKeyword keyword, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetRayTracingKeyword_Injected(IntPtr _unity_self, IntPtr rayTracingShader, [In] ref LocalKeyword keyword, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetViewMatrix_Injected(IntPtr _unity_self, [In] ref Matrix4x4 view);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetProjectionMatrix_Injected(IntPtr _unity_self, [In] ref Matrix4x4 proj);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetViewProjectionMatrices_Injected(IntPtr _unity_self, [In] ref Matrix4x4 view, [In] ref Matrix4x4 proj);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalDepthBias_Injected(IntPtr _unity_self, float bias, float slopeBias);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetExecutionFlags_Injected(IntPtr _unity_self, CommandBufferExecutionFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ValidateAgainstExecutionFlags_Injected(IntPtr _unity_self, CommandBufferExecutionFlags requiredFlags, CommandBufferExecutionFlags invalidFlags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalFloatArrayListImpl_Injected(IntPtr _unity_self, int nameID, object values);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalVectorArrayListImpl_Injected(IntPtr _unity_self, int nameID, object values);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalMatrixArrayListImpl_Injected(IntPtr _unity_self, int nameID, object values);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalFloatArray_Injected(IntPtr _unity_self, int nameID, ref ManagedSpanWrapper values);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalVectorArray_Injected(IntPtr _unity_self, int nameID, ref ManagedSpanWrapper values);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalMatrixArray_Injected(IntPtr _unity_self, int nameID, ref ManagedSpanWrapper values);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetLateLatchProjectionMatrices_Injected(IntPtr _unity_self, ref ManagedSpanWrapper projectionMat);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void MarkLateLatchMatrixShaderPropertyID_Injected(IntPtr _unity_self, CameraLateLatchMatrixType matrixPropertyType, int shaderPropertyID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UnmarkLateLatchMatrix_Injected(IntPtr _unity_self, CameraLateLatchMatrixType matrixPropertyType);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalTexture_Impl_Injected(IntPtr _unity_self, int nameID, ref RenderTargetIdentifier rt, RenderTextureSubElement element);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalBufferInternal_Injected(IntPtr _unity_self, int nameID, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalGraphicsBufferInternal_Injected(IntPtr _unity_self, int nameID, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalRayTracingAccelerationStructureInternal_Injected(IntPtr _unity_self, IntPtr accelerationStructure, int nameID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetShadowSamplingMode_Impl_Injected(IntPtr _unity_self, ref RenderTargetIdentifier shadowmap, ShadowSamplingMode mode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void IssuePluginEventInternal_Injected(IntPtr _unity_self, IntPtr callback, int eventID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void BeginSample_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EndSample_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void BeginSample_CustomSampler_Injected(IntPtr _unity_self, IntPtr sampler);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EndSample_CustomSampler_Injected(IntPtr _unity_self, IntPtr sampler);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void BeginSample_ProfilerMarker_Injected(IntPtr _unity_self, IntPtr markerHandle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EndSample_ProfilerMarker_Injected(IntPtr _unity_self, IntPtr markerHandle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void IssuePluginEventAndDataInternal_Injected(IntPtr _unity_self, IntPtr callback, int eventID, IntPtr data);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void IssuePluginEventAndDataInternalWithFlags_Injected(IntPtr _unity_self, IntPtr callback, int eventID, CustomMarkerCallbackFlags flags, IntPtr data);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void IssuePluginCustomBlitInternal_Injected(IntPtr _unity_self, IntPtr callback, uint command, ref RenderTargetIdentifier source, ref RenderTargetIdentifier dest, uint commandParam, uint commandFlags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void IssuePluginCustomTextureUpdateInternal_Injected(IntPtr _unity_self, IntPtr callback, IntPtr targetTexture, uint userData, bool useNewUnityRenderingExtTextureUpdateParamsV2);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalConstantBufferInternal_Injected(IntPtr _unity_self, IntPtr buffer, int nameID, int offset, int size);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGlobalConstantGraphicsBufferInternal_Injected(IntPtr _unity_self, IntPtr buffer, int nameID, int offset, int size);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void IncrementUpdateCount_Injected(IntPtr _unity_self, [In] ref RenderTargetIdentifier dest);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetInstanceMultiplier_Injected(IntPtr _unity_self, uint multiplier);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetFoveatedRenderingMode_Injected(IntPtr _unity_self, FoveatedRenderingMode foveatedRenderingMode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetWireframe_Injected(IntPtr _unity_self, bool enable);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ConfigureFoveatedRendering_Injected(IntPtr _unity_self, IntPtr platformData);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ClearRenderTargetSingle_Internal_Injected(IntPtr _unity_self, RTClearFlags clearFlags, [In] ref Color color, float depth, uint stencil);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ClearRenderTargetMulti_Internal_Injected(IntPtr _unity_self, RTClearFlags clearFlags, ref ManagedSpanWrapper colors, float depth, uint stencil);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetRenderTargetSingle_Internal_Injected(IntPtr _unity_self, [In] ref RenderTargetIdentifier rt, RenderBufferLoadAction colorLoadAction, RenderBufferStoreAction colorStoreAction, RenderBufferLoadAction depthLoadAction, RenderBufferStoreAction depthStoreAction);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetRenderTargetColorDepth_Internal_Injected(IntPtr _unity_self, [In] ref RenderTargetIdentifier color, [In] ref RenderTargetIdentifier depth, RenderBufferLoadAction colorLoadAction, RenderBufferStoreAction colorStoreAction, RenderBufferLoadAction depthLoadAction, RenderBufferStoreAction depthStoreAction, RenderTargetFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetRenderTargetMulti_Internal_Injected(IntPtr _unity_self, ref ManagedSpanWrapper colors, [In] ref RenderTargetIdentifier depth, ref ManagedSpanWrapper colorLoadActions, ref ManagedSpanWrapper colorStoreActions, RenderBufferLoadAction depthLoadAction, RenderBufferStoreAction depthStoreAction, RenderTargetFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetRenderTargetColorDepthSubtarget_Injected(IntPtr _unity_self, [In] ref RenderTargetIdentifier color, [In] ref RenderTargetIdentifier depth, RenderBufferLoadAction colorLoadAction, RenderBufferStoreAction colorStoreAction, RenderBufferLoadAction depthLoadAction, RenderBufferStoreAction depthStoreAction, int mipLevel, CubemapFace cubemapFace, int depthSlice);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetRenderTargetMultiSubtarget_Injected(IntPtr _unity_self, ref ManagedSpanWrapper colors, [In] ref RenderTargetIdentifier depth, ref ManagedSpanWrapper colorLoadActions, ref ManagedSpanWrapper colorStoreActions, RenderBufferLoadAction depthLoadAction, RenderBufferStoreAction depthStoreAction, int mipLevel, CubemapFace cubemapFace, int depthSlice);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_ProcessVTFeedback_Injected(IntPtr _unity_self, [In] ref RenderTargetIdentifier rt, IntPtr resolver, int slice, int x, int width, int y, int height, int mip);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetComputeBufferNativeData_Injected(IntPtr _unity_self, IntPtr buffer, IntPtr data, int nativeBufferStartIndex, int graphicsBufferStartIndex, int count, int elemSize);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetComputeBufferData_Injected(IntPtr _unity_self, IntPtr buffer, Array data, int managedBufferStartIndex, int graphicsBufferStartIndex, int count, int elemSize);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetComputeBufferCounterValue_Injected(IntPtr _unity_self, IntPtr buffer, uint counterValue);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetGraphicsBufferNativeData_Injected(IntPtr _unity_self, IntPtr buffer, IntPtr data, int nativeBufferStartIndex, int graphicsBufferStartIndex, int count, int elemSize);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetGraphicsBufferData_Injected(IntPtr _unity_self, IntPtr buffer, Array data, int managedBufferStartIndex, int graphicsBufferStartIndex, int count, int elemSize);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetGraphicsBufferCounterValue_Injected(IntPtr _unity_self, IntPtr buffer, uint counterValue);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CopyBufferImpl_Injected(IntPtr _unity_self, IntPtr source, IntPtr dest);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void BeginRenderPass_Internal_Injected(IntPtr _unity_self, int width, int height, int volumeDepth, int samples, ref ManagedSpanWrapper attachments, int depthAttachmentIndex, int shadingRateImageAttachmentIndex, ref ManagedSpanWrapper subPasses, ref ManagedSpanWrapper debugNameUtf8);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void NextSubPass_Internal_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EndRenderPass_Internal_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetupCameraProperties_Internal_Injected(IntPtr _unity_self, IntPtr camera);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InvokeOnRenderObjectCallbacks_Internal_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetShadingRateFragmentSize_Impl_Injected(IntPtr _unity_self, ShadingRateFragmentSize shadingRateFragmentSize);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetShadingRateCombiner_Impl_Injected(IntPtr _unity_self, ShadingRateCombinerStage stage, ShadingRateCombiner combiner);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetShadingRateImage_Impl_Injected(IntPtr _unity_self, in RenderTargetIdentifier shadingRateImage);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ResetShadingRate_Impl_Injected(IntPtr _unity_self);

		public static bool ThrowOnSetRenderTarget;

		internal IntPtr m_Ptr;

		internal static class BindingsMarshaller
		{
			public static CommandBuffer ConvertToManaged(IntPtr ptr)
			{
				return new CommandBuffer(ptr);
			}

			public static IntPtr ConvertToNative(CommandBuffer commandBuffer)
			{
				return commandBuffer.m_Ptr;
			}
		}
	}
}
