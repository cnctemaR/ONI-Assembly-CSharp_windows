using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering
{
	/// <summary>
	///   <para>Used to manage synchronisation between tasks on async compute queues and the graphics queue.</para>
	/// </summary>
	[NativeHeader("Runtime/Graphics/GPUFence.h")]
	[UsedByNativeCode]
	public struct GPUFence
	{
		/// <summary>
		///   <para>Has the GPUFence passed?
		///
		/// Allows for CPU determination of whether the GPU has passed the point in its processing represented by the GPUFence.</para>
		/// </summary>
		public bool passed
		{
			get
			{
				this.Validate();
				if (!SystemInfo.supportsGPUFence)
				{
					throw new NotSupportedException("Cannot determine if this GPUFence has passed as this platform has not implemented GPUFences.");
				}
				return !this.IsFencePending() || GPUFence.HasFencePassed_Internal(this.m_Ptr);
			}
		}

		[FreeFunction("GPUFenceInternals::HasFencePassed_Internal")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasFencePassed_Internal(IntPtr fencePtr);

		internal void InitPostAllocation()
		{
			if (this.m_Ptr == IntPtr.Zero)
			{
				if (SystemInfo.supportsGPUFence)
				{
					throw new NullReferenceException("The internal fence ptr is null, this should not be possible for fences that have been correctly constructed using Graphics.CreateGPUFence() or CommandBuffer.CreateGPUFence()");
				}
				this.m_Version = this.GetPlatformNotSupportedVersion();
			}
			else
			{
				this.m_Version = GPUFence.GetVersionNumber(this.m_Ptr);
			}
		}

		internal bool IsFencePending()
		{
			return !(this.m_Ptr == IntPtr.Zero) && this.m_Version == GPUFence.GetVersionNumber(this.m_Ptr);
		}

		internal void Validate()
		{
			if (this.m_Version == 0 || (SystemInfo.supportsGPUFence && this.m_Version == this.GetPlatformNotSupportedVersion()))
			{
				throw new InvalidOperationException("This GPUFence object has not been correctly constructed see Graphics.CreateGPUFence() or CommandBuffer.CreateGPUFence()");
			}
		}

		private int GetPlatformNotSupportedVersion()
		{
			return -1;
		}

		[NativeThrows]
		[FreeFunction("GPUFenceInternals::GetVersionNumber")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetVersionNumber(IntPtr fencePtr);

		internal IntPtr m_Ptr;

		internal int m_Version;
	}
}
