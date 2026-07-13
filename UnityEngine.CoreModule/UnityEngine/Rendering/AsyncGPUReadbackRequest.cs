using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering
{
	[NativeHeader("Runtime/Graphics/Texture.h")]
	[NativeHeader("Runtime/Shaders/ComputeShader.h")]
	[NativeHeader("Runtime/Graphics/AsyncGPUReadbackManaged.h")]
	[UsedByNativeCode]
	public struct AsyncGPUReadbackRequest
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void Update();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void WaitForCompletion();

		public unsafe NativeArray<T> GetData<T>(int layer = 0) where T : struct
		{
			bool flag = !this.done || this.hasError;
			if (flag)
			{
				throw new InvalidOperationException("Cannot access the data as it is not available");
			}
			bool flag2 = layer < 0 || layer >= this.layerCount;
			if (flag2)
			{
				throw new ArgumentException(string.Format("Layer index is out of range {0} / {1}", layer, this.layerCount));
			}
			int num = UnsafeUtility.SizeOf<T>();
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>((void*)this.GetDataRaw(layer), this.layerDataSize / num, Allocator.None);
		}

		public bool done
		{
			get
			{
				return this.IsDone();
			}
		}

		public bool hasError
		{
			get
			{
				return this.HasError();
			}
		}

		public int layerCount
		{
			get
			{
				return this.GetLayerCount();
			}
		}

		public int layerDataSize
		{
			get
			{
				return this.GetLayerDataSize();
			}
		}

		public int width
		{
			get
			{
				return this.GetWidth();
			}
		}

		public int height
		{
			get
			{
				return this.GetHeight();
			}
		}

		public int depth
		{
			get
			{
				return this.GetDepth();
			}
		}

		public bool forcePlayerLoopUpdate
		{
			get
			{
				return this.GetForcePlayerLoopUpdate();
			}
			set
			{
				this.SetForcePlayerLoopUpdate(value);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool IsDone();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool HasError();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int GetLayerCount();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int GetLayerDataSize();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int GetWidth();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int GetHeight();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int GetDepth();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool GetForcePlayerLoopUpdate();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetForcePlayerLoopUpdate(bool b);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetScriptingCallback(Action<AsyncGPUReadbackRequest> callback);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern IntPtr GetDataRaw(int layer);

		[RequiredByNativeCode]
		private static void InvokeCallback(Action<AsyncGPUReadbackRequest> callback, AsyncGPUReadbackRequest obj)
		{
			callback(obj);
		}

		internal IntPtr m_Ptr;

		internal int m_Version;
	}
}
