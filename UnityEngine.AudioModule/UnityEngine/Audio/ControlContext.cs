using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Audio;
using Unity.Collections.LowLevel.Unsafe;
using Unity.IntegerTime;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Audio
{
	[RequiredByNativeCode]
	[NativeHeader("Modules/Audio/Public/ScriptableProcessors/ScriptBindings/ScriptableProcessor.bindings.h")]
	public struct ControlContext : ProcessorInstance.IContext
	{
		internal unsafe readonly ControlHeader* Header
		{
			get
			{
				return this.m_Header;
			}
		}

		public static ControlContext builtIn
		{
			get
			{
				return new ControlContext(ControlContext.InternalGetBuiltInControlHeader());
			}
		}

		internal unsafe ControlContext(void* headerThatShouldBeOfResourceType)
		{
			this.m_Handle = ((ControlHeader*)headerThatShouldBeOfResourceType)->Handle;
			this.m_Handle.CheckValidOrThrow();
			this.m_Header = (ControlHeader*)headerThatShouldBeOfResourceType;
		}

		public unsafe readonly GeneratorInstance AllocateGenerator<[global::System.Runtime.CompilerServices.IsUnmanaged] TRealtime, [global::System.Runtime.CompilerServices.IsUnmanaged] TControl>(in TRealtime realtimeState, in TControl controlState, AudioFormat? nestedFormat = null, in ProcessorInstance.CreationParameters creationParameters = null) where TRealtime : struct, ValueType, GeneratorInstance.IRealtime where TControl : struct, ValueType, GeneratorInstance.IControl<TRealtime>
		{
			this.m_Handle.CheckValidOrThrow();
			IGeneratorControlExtensions.JobStruct<TControl, TRealtime>.ControlStorage* ptr = ProcessorExtensions.CAllocChunk<IGeneratorControlExtensions.JobStruct<TControl, TRealtime>.ControlStorage>();
			GeneratorInstance.GeneratorHeader* ptr2 = &ptr->HeaderAndProcessor.Header;
			ptr2->Processor.ProcessorReflectionData = IGeneratorProcessorExtensions.GetReflectionData<TRealtime>();
			ptr2->Processor.ControlReflectionData = IGeneratorControlExtensions.GetReflectionData<TControl, TRealtime>();
			ref GeneratorInstance.GeneratorHeader ptr3 = ref *ptr2;
			TRealtime trealtime = realtimeState;
			ptr3.Configuration.IsRealtime = trealtime.isRealtime;
			ref GeneratorInstance.GeneratorHeader ptr4 = ref *ptr2;
			trealtime = realtimeState;
			ptr4.Configuration.IsFinite = trealtime.isFinite;
			trealtime = realtimeState;
			DiscreteTime? length = trealtime.length;
			DiscreteTime valueOrDefault;
			bool flag;
			if (length != null)
			{
				valueOrDefault = length.GetValueOrDefault();
				flag = true;
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			if (flag2)
			{
				ptr2->Configuration.ReportedLength = valueOrDefault;
				ptr2->Configuration.HasKnownLength = true;
			}
			ptr->HeaderAndProcessor.UserProcessor = realtimeState;
			ptr->UserControl = controlState;
			AudioConfiguration audioConfiguration = nestedFormat.GetValueOrDefault().audioConfiguration;
			ScriptableGeneratorBindings.InitializeGeneratorHandle(ptr2, this.m_Header, (nestedFormat != null) ? (&audioConfiguration) : null, creationParameters.BuildInitializationFlags());
			return new GeneratorInstance(ptr2);
		}

		public unsafe readonly RootOutputInstance AllocateRootOutput<[global::System.Runtime.CompilerServices.IsUnmanaged] TRealtime, [global::System.Runtime.CompilerServices.IsUnmanaged] TControl>(in TRealtime realtimeState, in TControl controlState, in ProcessorInstance.CreationParameters creationParameters = null) where TRealtime : struct, ValueType, RootOutputInstance.IRealtime where TControl : struct, ValueType, RootOutputInstance.IControl<TRealtime>
		{
			this.m_Handle.CheckValidOrThrow();
			IRootOutputControlExtensions.JobStruct<TControl, TRealtime>.ControlStorage* ptr = ProcessorExtensions.CAllocChunk<IRootOutputControlExtensions.JobStruct<TControl, TRealtime>.ControlStorage>();
			ProcessorHeader* ptr2 = &ptr->HeaderAndProcessor.Header;
			ptr2->ProcessorReflectionData = IRootOutputProcessorExtensions.GetReflectionData<TRealtime>();
			ptr2->ControlReflectionData = IRootOutputControlExtensions.GetReflectionData<TControl, TRealtime>();
			ptr->HeaderAndProcessor.UserProcessor = realtimeState;
			ptr->UserControl = controlState;
			IRootOutputProcessorExtensions.InitializeRootOutputHandle(ptr2, this.m_Header, creationParameters.BuildInitializationFlags());
			return new RootOutputInstance(ptr2);
		}

		public unsafe readonly bool IsGenerator<[global::System.Runtime.CompilerServices.IsUnmanaged] TRealtime, [global::System.Runtime.CompilerServices.IsUnmanaged] TControl>(ProcessorInstance processorInstance) where TRealtime : struct, ValueType, GeneratorInstance.IRealtime where TControl : struct, ValueType, GeneratorInstance.IControl<TRealtime>
		{
			this.m_Handle.CheckValidOrThrow();
			processorInstance.Handle.CheckValidOrThrow();
			return processorInstance.Header->ControlReflectionData == IGeneratorControlExtensions.GetReflectionData<TControl, TRealtime>();
		}

		public unsafe readonly bool IsRootOutput<[global::System.Runtime.CompilerServices.IsUnmanaged] TRealtime, [global::System.Runtime.CompilerServices.IsUnmanaged] TControl>(ProcessorInstance processorInstance) where TRealtime : struct, ValueType, RootOutputInstance.IRealtime where TControl : struct, ValueType, RootOutputInstance.IControl<TRealtime>
		{
			this.m_Handle.CheckValidOrThrow();
			processorInstance.Handle.CheckValidOrThrow();
			return processorInstance.Header->ControlReflectionData == IRootOutputControlExtensions.GetReflectionData<TControl, TRealtime>();
		}

		public bool Exists(ProcessorInstance processorInstance)
		{
			this.m_Handle.CheckValidOrThrow();
			return ScriptableProcessorBindings.CheckProcessorExists(processorInstance.Handle, this.m_Header);
		}

		public unsafe ProcessorInstance.Response SendMessage<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(ProcessorInstance processorInstance, ref T message) where T : struct, ValueType
		{
			this.m_Handle.CheckValidOrThrow();
			processorInstance.Handle.CheckValidOrThrow();
			fixed (T* ptr = &message)
			{
				T* ptr2 = ptr;
				ProcessorInstance.Message message2 = new ProcessorInstance.Message
				{
					TypeHash = BurstRuntime.GetHashCode64<T>(),
					Data = (void*)ptr2,
					ManagedHandle = 0
				};
				return ScriptableProcessorBindings.SendMessageToProcessor(processorInstance.Header, this.Header, &message2);
			}
		}

		internal unsafe ProcessorInstance.Response SendManagedMessage<T>(ProcessorInstance processorInstance, T message) where T : class
		{
			this.m_Handle.CheckValidOrThrow();
			processorInstance.Handle.CheckValidOrThrow();
			object obj = null;
			bool flag = this.Header->ManagedTransport != IntPtr.Zero;
			GCHandle gchandle;
			if (flag)
			{
				gchandle = GCHandle.FromIntPtr(this.Header->ManagedTransport);
				obj = gchandle.Target;
				gchandle.Target = message;
			}
			else
			{
				gchandle = GCHandle.Alloc(message, GCHandleType.Normal);
				this.Header->ManagedTransport = GCHandle.ToIntPtr(gchandle);
			}
			ProcessorInstance.Message message2 = new ProcessorInstance.Message
			{
				TypeHash = BurstRuntime.GetHashCode64<T>(),
				Data = null,
				ManagedHandle = GCHandle.ToIntPtr(gchandle)
			};
			ProcessorInstance.Response response = ScriptableProcessorBindings.SendMessageToProcessor(processorInstance.Header, this.Header, &message2);
			gchandle.Target = obj;
			return response;
		}

		public void Destroy(GeneratorInstance generatorInstance)
		{
			this.DestroyProcessor(generatorInstance.m_ProcessorInstance);
		}

		public void Destroy(RootOutputInstance rootOutputInstance)
		{
			this.DestroyProcessor(rootOutputInstance.m_ProcessorInstance);
		}

		public unsafe GeneratorInstance.Configuration GetConfiguration(GeneratorInstance generatorInstance)
		{
			this.m_Handle.CheckValidOrThrow();
			generatorInstance.m_ProcessorInstance.Handle.CheckValidOrThrow();
			return ((GeneratorInstance.GeneratorHeader*)generatorInstance.m_ProcessorInstance.Header)->Configuration;
		}

		public unsafe void Configure(GeneratorInstance generatorInstance, in AudioFormat format)
		{
			Handle handle = generatorInstance.m_ProcessorInstance.Handle;
			ControlHeader* header = this.Header;
			AudioConfiguration audioConfiguration = format.audioConfiguration;
			ScriptableProcessorBindings.PerformRecursiveConfigure(handle, header, in audioConfiguration);
		}

		public void Update(GeneratorInstance generatorInstance)
		{
			ScriptableProcessorBindings.PerformRecursiveUpdate(generatorInstance.m_ProcessorInstance.Handle, this.Header);
		}

		public unsafe static void WaitForBuiltInQueueFlush()
		{
			ControlContext.InternalWaitForQueueFlush((void*)ControlContext.builtIn.m_Header);
		}

		public unsafe static ControlContext.Manual CreateManualControlContext(in AudioFormat format)
		{
			ControlContext controlContext = new ControlContext(ControlContext.InternalCreateControlContext());
			controlContext.m_Handle.CheckValidOrThrow();
			ControlContext.InternalSetConfigurationManualControlContext((void*)controlContext.m_Header, format.audioConfiguration);
			return new ControlContext.Manual(in controlContext);
		}

		unsafe ProcessorInstance.AvailableData ProcessorInstance.IContext.GetAvailableData(Handle handle)
		{
			this.m_Handle.CheckValidOrThrow();
			bool flag = !handle.Valid;
			if (flag)
			{
				throw new InvalidOperationException("Invalid handle provided to GetAvailableData");
			}
			ProcessorInstance.AvailableData.Element* availableDataForControl = ScriptableProcessorBindings.GetAvailableDataForControl(this.m_Header, in handle);
			return new ProcessorInstance.AvailableData(availableDataForControl);
		}

		unsafe bool ProcessorInstance.IContext.SendData(Handle handle, void* data, int size, int align, long typehash)
		{
			return ScriptableProcessorBindings.AddDataToProcessorHandle(this.m_Header, in handle, data, size, align, typehash);
		}

		internal void DestroyProcessor(ProcessorInstance processorInstance)
		{
			this.m_Handle.CheckValidOrThrow();
			bool flag = processorInstance.Handle.Equals(default(Handle));
			if (flag)
			{
				throw new InvalidOperationException("Default / zero-initialized value of processor being destroyed");
			}
			ScriptableProcessorBindings.QueueProcessorDispose(processorInstance.Header, this.m_Header);
		}

		[RequiredByNativeCode(GenerateProxy = true)]
		internal static void CleanupHeader(ref ControlHeader header)
		{
			bool flag = header.ManagedTransport != IntPtr.Zero;
			if (flag)
			{
				GCHandle gchandle = GCHandle.FromIntPtr(header.ManagedTransport);
				bool isAllocated = gchandle.IsAllocated;
				if (isAllocated)
				{
					gchandle.Free();
				}
			}
		}

		[NativeMethod(Name = "audio::GetBuiltInControlHeader", IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe static extern void* InternalGetBuiltInControlHeader();

		[NativeMethod(Name = "audio::WaitForQueueFlush", IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void InternalWaitForQueueFlush(void* header);

		[NativeMethod(Name = "audio::CreateControlContext", IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void* InternalCreateControlContext();

		[NativeMethod(Name = "audio::DestroyControlContext", IsFreeFunction = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void InternalDestroyControlContext(void* header);

		[NativeMethod(Name = "audio::BeginMixManualControlContext ", IsFreeFunction = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern bool InternalBeginManualMixFromControlContext(void* header, ulong dspTick, void* resultContext);

		[NativeMethod(Name = "audio::EndMixManualControlContext", IsFreeFunction = true, ThrowsException = true)]
		private unsafe static void InternalEndMixManualControlContext(void* header, Span<float> data)
		{
			Span<float> span = data;
			fixed (float* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				ControlContext.InternalEndMixManualControlContext_Injected(header, ref managedSpanWrapper);
			}
		}

		[NativeMethod(Name = "audio::UpdateManualControlContext", IsFreeFunction = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void InternalUpdateManualControlContext(void* header);

		[NativeMethod(Name = "audio::SetConfigurationManualControlContext", IsFreeFunction = true, ThrowsException = true)]
		private unsafe static void InternalSetConfigurationManualControlContext(void* header, AudioConfiguration config)
		{
			ControlContext.InternalSetConfigurationManualControlContext_Injected(header, ref config);
		}

		[Obsolete("ControlContext.GetAvailableData has been deprecated. Use ControlContext.SendMessage instead.", true)]
		public ProcessorInstance.AvailableData GetAvailableData(ProcessorInstance processorInstance)
		{
			throw new NotImplementedException();
		}

		[Obsolete("ControlContext.SendData has been deprecated. Use ControlContext.SendMessage instead.", true)]
		public void SendData<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(ProcessorInstance processorInstance, in T data) where T : struct, ValueType
		{
			throw new NotImplementedException();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void InternalEndMixManualControlContext_Injected(void* header, ref ManagedSpanWrapper data);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void InternalSetConfigurationManualControlContext_Injected(void* header, [In] ref AudioConfiguration config);

		private unsafe ControlHeader* m_Header;

		internal Handle m_Handle;

		public struct Manual : IDisposable
		{
			public ControlContext context
			{
				get
				{
					return this.m_Context;
				}
			}

			public unsafe RealtimeContext? BeginMix(ulong dspTick)
			{
				RealtimeContext realtimeContext;
				bool flag = ControlContext.InternalBeginManualMixFromControlContext((void*)this.m_Context.m_Header, dspTick, (void*)(&realtimeContext));
				RealtimeContext? realtimeContext2;
				if (flag)
				{
					realtimeContext2 = new RealtimeContext?(realtimeContext);
				}
				else
				{
					realtimeContext2 = null;
				}
				return realtimeContext2;
			}

			public unsafe void EndMix(ChannelBuffer result)
			{
				ControlContext.InternalEndMixManualControlContext((void*)this.m_Context.m_Header, result.Buffer);
			}

			public unsafe void Update()
			{
				ControlContext.InternalUpdateManualControlContext((void*)this.m_Context.m_Header);
			}

			public unsafe void Dispose()
			{
				this.m_Context.m_Handle.CheckValidOrThrow();
				ControlContext.InternalDestroyControlContext((void*)this.m_Context.m_Header);
			}

			internal Manual(in ControlContext context)
			{
				this.m_Context = context;
			}

			private ControlContext m_Context;
		}

		[Obsolete("ControlContext.ProcessorUpdateSetting has been deprecated. Use ProcessorInstance.UpdateSetting instead. (UnityUpgradable) -> ProcessorInstance/UpdateSetting", true)]
		public struct ProcessorUpdateSetting
		{
		}

		[Obsolete("ControlContext.ProcessorCreationParameters has been deprecated. Use ProcessorInstance.CreationParameters instead. (UnityUpgradable) -> ProcessorInstance/CreationParameters", true)]
		public struct ProcessorCreationParameters
		{
		}
	}
}
