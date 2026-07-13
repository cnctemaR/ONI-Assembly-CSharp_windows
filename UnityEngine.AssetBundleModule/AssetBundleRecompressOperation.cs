using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Modules/AssetBundle/Public/AssetBundleRecompressOperation.h")]
	[RequiredByNativeCode]
	[StructLayout(LayoutKind.Sequential)]
	public class AssetBundleRecompressOperation : AsyncOperation
	{
		public string humanReadableResult
		{
			[NativeMethod("GetResultStr")]
			get
			{
				string stringAndDispose;
				try
				{
					IntPtr intPtr = AssetBundleRecompressOperation.BindingsMarshaller.ConvertToNative(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					ManagedSpanWrapper managedSpanWrapper;
					AssetBundleRecompressOperation.get_humanReadableResult_Injected(intPtr, out managedSpanWrapper);
				}
				finally
				{
					ManagedSpanWrapper managedSpanWrapper;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
				}
				return stringAndDispose;
			}
		}

		public string inputPath
		{
			[NativeMethod("GetInputPath")]
			get
			{
				string stringAndDispose;
				try
				{
					IntPtr intPtr = AssetBundleRecompressOperation.BindingsMarshaller.ConvertToNative(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					ManagedSpanWrapper managedSpanWrapper;
					AssetBundleRecompressOperation.get_inputPath_Injected(intPtr, out managedSpanWrapper);
				}
				finally
				{
					ManagedSpanWrapper managedSpanWrapper;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
				}
				return stringAndDispose;
			}
		}

		public string outputPath
		{
			[NativeMethod("GetOutputPath")]
			get
			{
				string stringAndDispose;
				try
				{
					IntPtr intPtr = AssetBundleRecompressOperation.BindingsMarshaller.ConvertToNative(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					ManagedSpanWrapper managedSpanWrapper;
					AssetBundleRecompressOperation.get_outputPath_Injected(intPtr, out managedSpanWrapper);
				}
				finally
				{
					ManagedSpanWrapper managedSpanWrapper;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
				}
				return stringAndDispose;
			}
		}

		public AssetBundleLoadResult result
		{
			[NativeMethod("GetResult")]
			get
			{
				IntPtr intPtr = AssetBundleRecompressOperation.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AssetBundleRecompressOperation.get_result_Injected(intPtr);
			}
		}

		public bool success
		{
			[NativeMethod("GetSuccess")]
			get
			{
				IntPtr intPtr = AssetBundleRecompressOperation.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AssetBundleRecompressOperation.get_success_Injected(intPtr);
			}
		}

		public AssetBundleRecompressOperation()
		{
		}

		private AssetBundleRecompressOperation(IntPtr ptr)
			: base(ptr)
		{
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_humanReadableResult_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_inputPath_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_outputPath_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AssetBundleLoadResult get_result_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_success_Injected(IntPtr _unity_self);

		internal new static class BindingsMarshaller
		{
			public static AssetBundleRecompressOperation ConvertToManaged(IntPtr ptr)
			{
				return new AssetBundleRecompressOperation(ptr);
			}

			public static IntPtr ConvertToNative(AssetBundleRecompressOperation op)
			{
				return op.m_Ptr;
			}
		}
	}
}
