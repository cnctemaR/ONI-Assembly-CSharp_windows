using System;
using UnityEngine.Bindings;

namespace UnityEngine.Rendering
{
	[NativeHeader("Runtime/Graphics/MachineLearning/MachineLearningOperatorAttributes.h")]
	[NativeHeader("Runtime/Graphics/MachineLearning/MachineLearningOperator.h")]
	[NativeHeader("Runtime/Graphics/MachineLearning/MachineLearningContext.h")]
	public enum MachineLearningOperatorType : uint
	{
		None,
		Identity,
		Gemm,
		Conv,
		ReLU,
		ReduceMax,
		ReduceMean,
		ReduceMin,
		ReduceProd,
		ReduceSum,
		ReduceSumSquare,
		ReduceL1,
		ReduceL2,
		ReduceLogSum,
		ReduceLogSumExp
	}
}
