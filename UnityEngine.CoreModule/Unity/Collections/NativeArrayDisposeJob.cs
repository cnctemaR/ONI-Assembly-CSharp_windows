using System;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Scripting;

namespace Unity.Collections
{
	[NativeClass(null)]
	internal struct NativeArrayDisposeJob : IJob
	{
		public void Execute()
		{
			this.Data.Dispose();
		}

		[RequiredByNativeCode]
		internal static void RegisterNativeArrayDisposeJobReflectionData()
		{
			IJobExtensions.EarlyJobInit<NativeArrayDisposeJob>();
		}

		internal NativeArrayDispose Data;
	}
}
