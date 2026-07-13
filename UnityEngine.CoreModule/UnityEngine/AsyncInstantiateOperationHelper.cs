using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequiredByNativeCode]
	internal class AsyncInstantiateOperationHelper
	{
		[RequiredByNativeCode]
		public static Object[] CreateAsyncInstantiateOperationResultArray(AsyncInstantiateOperation op, int size)
		{
			return op.CreateResultArray(size);
		}
	}
}
