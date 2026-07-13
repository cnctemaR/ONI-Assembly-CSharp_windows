using System;
using System.Runtime.CompilerServices;
using Unity.IntegerTime;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Audio
{
	public struct GeneratorInstance : IEquatable<GeneratorInstance>
	{
		[Obsolete("GeneratorInstance.Configure has been deprecated. Use ControlContext.Configure instead.", true)]
		public void Configure(ControlContext context, in AudioFormat format)
		{
			throw new NotImplementedException();
		}

		[Obsolete("GeneratorInstance.Update has been deprecated. Use ControlContext.Update instead.", true)]
		public void Update(ControlContext context)
		{
			throw new NotImplementedException();
		}

		[Obsolete("GeneratorInstance.Process has been deprecated. Use RealtimeContext.Process instead.", true)]
		public GeneratorInstance.Result Process(RealtimeContext context, ChannelBuffer buffer, GeneratorInstance.Arguments args)
		{
			throw new NotImplementedException();
		}

		public static implicit operator ProcessorInstance(in GeneratorInstance generatorInstance)
		{
			return generatorInstance.m_ProcessorInstance;
		}

		public bool Equals(GeneratorInstance other)
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
				if (obj is GeneratorInstance)
				{
					GeneratorInstance generatorInstance = (GeneratorInstance)obj;
					flag3 = this.Equals(generatorInstance);
				}
				else
				{
					flag3 = false;
				}
				flag2 = flag3;
			}
			return flag2;
		}

		public static bool operator ==(GeneratorInstance a, GeneratorInstance b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(GeneratorInstance a, GeneratorInstance b)
		{
			return !a.Equals(b);
		}

		public override int GetHashCode()
		{
			return this.m_ProcessorInstance.GetHashCode();
		}

		internal unsafe GeneratorInstance(GeneratorInstance.GeneratorHeader* header)
		{
			this.m_ProcessorInstance = new ProcessorInstance(header->Processor.DualThreadHandle, &header->Processor);
		}

		internal readonly ProcessorInstance m_ProcessorInstance;

		[Obsolete("IProcessor has been deprecated. Use IRealtime instead. (UnityUpgradable) -> GeneratorInstance/IRealtime", true)]
		public interface IProcessor
		{
		}

		public interface ICapabilities
		{
			bool isFinite { get; }

			bool isRealtime { get; }

			DiscreteTime? length { get; }
		}

		public readonly struct Setup
		{
			public Setup(AudioSpeakerMode speakerMode, int sampleRate)
			{
				this.speakerMode = speakerMode;
				this.sampleRate = sampleRate;
			}

			public Setup(in AudioFormat fromFormat)
			{
				this = new GeneratorInstance.Setup(fromFormat.speakerMode, fromFormat.sampleRate);
			}

			public readonly AudioSpeakerMode speakerMode;

			public readonly int sampleRate;
		}

		public struct Properties
		{
			private byte m_Reserved;
		}

		public struct Configuration
		{
			public GeneratorInstance.Setup setup
			{
				get
				{
					return this.Setup;
				}
			}

			public GeneratorInstance.Properties properties
			{
				get
				{
					return this.Properties;
				}
			}

			public bool isFinite
			{
				get
				{
					return this.IsFinite;
				}
			}

			public bool isRealtime
			{
				get
				{
					return this.IsRealtime;
				}
			}

			public DiscreteTime? length
			{
				get
				{
					return this.HasKnownLength ? new DiscreteTime?(this.ReportedLength) : null;
				}
			}

			internal GeneratorInstance.Setup Setup;

			internal GeneratorInstance.Properties Properties;

			internal DiscreteTime ReportedLength;

			internal bool IsFinite;

			internal bool IsRealtime;

			internal bool HasKnownLength;
		}

		[Obsolete("Types with embedded references are not supported in this version of your compiler.", true)]
		public ref struct Result
		{
			public int processedFrames
			{
				get
				{
					return this.m_ProcessedFrames;
				}
			}

			public static implicit operator GeneratorInstance.Result(int processedFrames)
			{
				return new GeneratorInstance.Result
				{
					m_ProcessedFrames = processedFrames
				};
			}

			internal int m_ProcessedFrames;
		}

		public ref struct Arguments
		{
			internal float Speed;
		}

		[JobProducerType(typeof(IGeneratorControlExtensions.JobStruct<, >))]
		public interface IControl<[global::System.Runtime.CompilerServices.IsUnmanaged] TRealtime> : ProcessorInstance.IControl<TRealtime> where TRealtime : struct, ValueType, ProcessorInstance.IRealtime
		{
			void Configure(ControlContext context, ref TRealtime realtime, in AudioFormat format, out GeneratorInstance.Setup setup, ref GeneratorInstance.Properties properties);
		}

		[JobProducerType(typeof(IGeneratorProcessorExtensions.JobStruct<>))]
		public interface IRealtime : ProcessorInstance.IRealtime, GeneratorInstance.ICapabilities
		{
			GeneratorInstance.Result Process(in RealtimeContext context, ProcessorInstance.Pipe pipe, ChannelBuffer buffer, GeneratorInstance.Arguments args);
		}

		[NativeHeader("Modules/Audio/Public/ScriptableProcessors/ScriptBindings/GeneratorHandle.h")]
		[RequiredByNativeCode]
		internal struct GeneratorHeader
		{
			internal ProcessorHeader Processor;

			internal GeneratorInstance.Configuration Configuration;
		}
	}
}
