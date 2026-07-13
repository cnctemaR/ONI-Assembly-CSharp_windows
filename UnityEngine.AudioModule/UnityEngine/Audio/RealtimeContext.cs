using System;
using Unity.Audio;

namespace UnityEngine.Audio
{
	public struct RealtimeContext : ProcessorInstance.IContext
	{
		public readonly ulong dspTime
		{
			get
			{
				return this.m_DSPClock;
			}
		}

		public readonly bool isCreated
		{
			get
			{
				return this.Access.IsCreated;
			}
		}

		ProcessorInstance.AvailableData ProcessorInstance.IContext.GetAvailableData(Handle handle)
		{
			return new ProcessorInstance.AvailableData(ScriptableProcessorBindings.GetAvailableDataForRealtime(in this.Access, in handle));
		}

		unsafe bool ProcessorInstance.IContext.SendData(Handle handle, void* data, int size, int align, long typehash)
		{
			ScriptableProcessorBindings.ReturnDataFromProcessor(in this.Access, in handle, data, size, align, typehash);
			return true;
		}

		public unsafe readonly GeneratorInstance.Result Process(GeneratorInstance generatorInstance, ChannelBuffer buffer, GeneratorInstance.Arguments args)
		{
			ScriptableProcessorBindings.ValidateCanProcess(in generatorInstance.m_ProcessorInstance.Handle, in this);
			fixed (float* pinnableReference = buffer.Buffer.GetPinnableReference())
			{
				float* ptr = pinnableReference;
				fixed (RealtimeContext* ptr2 = &this)
				{
					RealtimeContext* ptr3 = ptr2;
					IGeneratorProcessorExtensions.ProcessArguments processArguments = new IGeneratorProcessorExtensions.ProcessArguments
					{
						AudioBuffer = ptr,
						Context = ptr3,
						FrameCount = buffer.frameCount
					};
					generatorInstance.m_ProcessorInstance.Header->InvokeProcessor(ProcessorFunction.Process, (void*)(&processArguments));
					return processArguments.Result;
				}
			}
		}

		internal RealtimeAccess Access;

		internal ulong m_DSPClock;
	}
}
