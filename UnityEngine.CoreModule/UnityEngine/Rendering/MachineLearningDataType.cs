using System;
using UnityEngine.Bindings;

namespace UnityEngine.Rendering
{
	[NativeHeader("Runtime/Graphics/MachineLearning/MachineLearningTensor.h")]
	[NativeHeader("Runtime/Export/Graphics/MachineLearning.bindings.h")]
	public enum MachineLearningDataType
	{
		Unknown,
		Float32,
		Float16,
		UInt32,
		UInt16,
		UInt8,
		Int32,
		Int16,
		Int8,
		Float64,
		UInt64,
		Int64
	}
}
