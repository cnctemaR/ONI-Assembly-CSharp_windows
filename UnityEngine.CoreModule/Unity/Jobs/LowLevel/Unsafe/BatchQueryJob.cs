using System;
using Unity.Collections;

namespace Unity.Jobs.LowLevel.Unsafe
{
	public struct BatchQueryJob<CommandT, ResultT> where CommandT : struct where ResultT : struct
	{
		public BatchQueryJob(NativeArray<CommandT> commands, NativeArray<ResultT> results)
		{
			this.commands = commands;
			this.results = results;
		}

		[ReadOnly]
		internal NativeArray<CommandT> commands;

		internal NativeArray<ResultT> results;
	}
}
