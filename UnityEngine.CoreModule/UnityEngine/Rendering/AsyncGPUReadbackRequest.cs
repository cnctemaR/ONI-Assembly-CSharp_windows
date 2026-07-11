using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering
{
	/// <summary>
	///   <para>Represents an asynchronous request for a GPU resource.</para>
	/// </summary>
	[NativeHeader("Runtime/Shaders/ComputeShader.h")]
	[UsedByNativeCode]
	[NativeHeader("Runtime/Graphics/AsyncGPUReadbackManaged.h")]
	[NativeHeader("Runtime/Graphics/Texture.h")]
	public struct AsyncGPUReadbackRequest
	{
		/// <summary>
		///   <para>Triggers an update of the request.</para>
		/// </summary>
		public void Update()
		{
			AsyncGPUReadbackRequest.Update_Injected(ref this);
		}

		/// <summary>
		///   <para>Waits for completion of the request.</para>
		/// </summary>
		public void WaitForCompletion()
		{
			AsyncGPUReadbackRequest.WaitForCompletion_Injected(ref this);
		}

		public unsafe NativeArray<T> GetData<T>(int layer = 0) where T : struct
		{
			if (!this.done || this.hasError)
			{
				throw new InvalidOperationException("Cannot access the data as it is not available");
			}
			if (layer < 0 || layer >= this.layerCount)
			{
				throw new ArgumentException(string.Format("Layer index is out of range {0} / {1}", layer, this.layerCount));
			}
			int num = UnsafeUtility.SizeOf<T>();
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>((void*)this.GetDataRaw(layer), this.layerDataSize / num, Allocator.None);
		}

		/// <summary>
		///   <para>Checks whether the request has been processed.</para>
		/// </summary>
		public bool done
		{
			get
			{
				return this.IsDone();
			}
		}

		/// <summary>
		///   <para>This property is true if the request has encountered an error.</para>
		/// </summary>
		public bool hasError
		{
			get
			{
				return this.HasError();
			}
		}

		/// <summary>
		///   <para>Number of layers in the current request.</para>
		/// </summary>
		public int layerCount
		{
			get
			{
				return this.GetLayerCount();
			}
		}

		/// <summary>
		///   <para>The size in bytes of one layer of the readback data.</para>
		/// </summary>
		public int layerDataSize
		{
			get
			{
				return this.GetLayerDataSize();
			}
		}

		/// <summary>
		///   <para>The width of the requested GPU data.</para>
		/// </summary>
		public int width
		{
			get
			{
				return this.GetWidth();
			}
		}

		/// <summary>
		///   <para>When reading data from a ComputeBuffer, height is 1, otherwise, the property takes the value of the requested height from the texture.</para>
		/// </summary>
		public int height
		{
			get
			{
				return this.GetHeight();
			}
		}

		/// <summary>
		///   <para>When reading data from a ComputeBuffer, depth is 1, otherwise, the property takes the value of the requested depth from the texture.</para>
		/// </summary>
		public int depth
		{
			get
			{
				return this.GetDepth();
			}
		}

		private bool IsDone()
		{
			return AsyncGPUReadbackRequest.IsDone_Injected(ref this);
		}

		private bool HasError()
		{
			return AsyncGPUReadbackRequest.HasError_Injected(ref this);
		}

		private int GetLayerCount()
		{
			return AsyncGPUReadbackRequest.GetLayerCount_Injected(ref this);
		}

		private int GetLayerDataSize()
		{
			return AsyncGPUReadbackRequest.GetLayerDataSize_Injected(ref this);
		}

		private int GetWidth()
		{
			return AsyncGPUReadbackRequest.GetWidth_Injected(ref this);
		}

		private int GetHeight()
		{
			return AsyncGPUReadbackRequest.GetHeight_Injected(ref this);
		}

		private int GetDepth()
		{
			return AsyncGPUReadbackRequest.GetDepth_Injected(ref this);
		}

		internal void SetScriptingCallback(Action<AsyncGPUReadbackRequest> callback)
		{
			AsyncGPUReadbackRequest.SetScriptingCallback_Injected(ref this, callback);
		}

		private IntPtr GetDataRaw(int layer)
		{
			return AsyncGPUReadbackRequest.GetDataRaw_Injected(ref this, layer);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Update_Injected(ref AsyncGPUReadbackRequest _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void WaitForCompletion_Injected(ref AsyncGPUReadbackRequest _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsDone_Injected(ref AsyncGPUReadbackRequest _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasError_Injected(ref AsyncGPUReadbackRequest _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetLayerCount_Injected(ref AsyncGPUReadbackRequest _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetLayerDataSize_Injected(ref AsyncGPUReadbackRequest _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetWidth_Injected(ref AsyncGPUReadbackRequest _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetHeight_Injected(ref AsyncGPUReadbackRequest _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetDepth_Injected(ref AsyncGPUReadbackRequest _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetScriptingCallback_Injected(ref AsyncGPUReadbackRequest _unity_self, Action<AsyncGPUReadbackRequest> callback);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetDataRaw_Injected(ref AsyncGPUReadbackRequest _unity_self, int layer);

		internal IntPtr m_Ptr;

		internal int m_Version;
	}
}
