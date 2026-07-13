using System;
using System.Runtime.CompilerServices;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;

namespace UnityEngine.Audio
{
	public struct RootOutputInstance : IEquatable<RootOutputInstance>
	{
		public static implicit operator ProcessorInstance(in RootOutputInstance root)
		{
			return root.m_ProcessorInstance;
		}

		public bool Equals(RootOutputInstance other)
		{
			return this.m_ProcessorInstance.Equals(other.m_ProcessorInstance);
		}

		public override bool Equals(object obj)
		{
			bool flag = obj == null;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3;
				if (obj is RootOutputInstance)
				{
					RootOutputInstance rootOutputInstance = (RootOutputInstance)obj;
					flag3 = this.Equals(rootOutputInstance);
				}
				else
				{
					flag3 = false;
				}
				flag2 = flag3;
			}
			return flag2;
		}

		public static bool operator ==(RootOutputInstance a, RootOutputInstance b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(RootOutputInstance a, RootOutputInstance b)
		{
			return !a.Equals(b);
		}

		public override int GetHashCode()
		{
			return this.m_ProcessorInstance.GetHashCode();
		}

		internal unsafe RootOutputInstance(ProcessorHeader* header)
		{
			this.m_ProcessorInstance = new ProcessorInstance(header->DualThreadHandle, header);
		}

		internal readonly ProcessorInstance m_ProcessorInstance;

		[Obsolete("IProcessor has been deprecated. Use IRealtime instead. (UnityUpgradable) -> RootOutputInstance/IRealtime", true)]
		public interface IProcessor
		{
		}

		[JobProducerType(typeof(IRootOutputControlExtensions.JobStruct<, >))]
		public interface IControl<[global::System.Runtime.CompilerServices.IsUnmanaged] TRealtime> : ProcessorInstance.IControl<TRealtime> where TRealtime : struct, ValueType, ProcessorInstance.IRealtime
		{
			JobHandle Configure(ControlContext context, ref TRealtime realtime, in AudioFormat format);
		}

		[JobProducerType(typeof(IRootOutputProcessorExtensions.JobStruct<>))]
		public interface IRealtime : ProcessorInstance.IRealtime
		{
			JobHandle EarlyProcessing(in RealtimeContext context, ProcessorInstance.Pipe pipe);

			void Process(in RealtimeContext context, ProcessorInstance.Pipe pipe, JobHandle input);

			void EndProcessing(in RealtimeContext context, ProcessorInstance.Pipe pipe, ChannelBuffer output);

			void RemovedFromProcessing();
		}
	}
}
