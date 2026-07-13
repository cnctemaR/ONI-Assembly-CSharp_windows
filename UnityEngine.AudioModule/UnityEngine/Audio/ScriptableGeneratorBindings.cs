using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Audio
{
	[NativeHeader("Modules/Audio/Public/ScriptableProcessors/ScriptBindings/ScriptableProcessor.bindings.h")]
	internal static class ScriptableGeneratorBindings
	{
		[RequiredByNativeCode(GenerateProxy = true)]
		internal unsafe static void InstantiateGeneratorFromObject(Object generatorObjectDefinition, ref ControlHeader control, out GeneratorInstance runtimeHandle)
		{
			IAudioGenerator audioGenerator = generatorObjectDefinition as IAudioGenerator;
			bool flag = audioGenerator != null;
			if (flag)
			{
				fixed (ControlHeader* ptr = &control)
				{
					ControlHeader* ptr2 = ptr;
					ControlContext controlContext = new ControlContext((void*)ptr2);
					runtimeHandle = audioGenerator.CreateInstance(controlContext, null, default(ProcessorInstance.CreationParameters));
					bool flag2 = controlContext.Exists(in runtimeHandle);
					if (flag2)
					{
						GeneratorInstance.Configuration configuration = controlContext.GetConfiguration(runtimeHandle);
						bool flag3 = audioGenerator.isFinite != configuration.IsFinite;
						if (flag3)
						{
							Debug.LogError(string.Format("Generator {0} has inconsistent isFinite declaration: {1} vs {2}", generatorObjectDefinition, audioGenerator.isFinite, configuration.IsFinite));
						}
						bool flag4 = audioGenerator.isRealtime != configuration.isRealtime;
						if (flag4)
						{
							Debug.LogError(string.Format("Generator {0} has inconsistent isRealtime declaration: {1} vs {2}", generatorObjectDefinition, audioGenerator.isRealtime, configuration.isRealtime));
						}
						bool flag5 = audioGenerator.length != configuration.length;
						if (flag5)
						{
							Debug.LogError(string.Format("Generator {0} has inconsistent length declaration: {1} vs {2}", generatorObjectDefinition, audioGenerator.length, configuration.length));
						}
					}
				}
			}
			else
			{
				runtimeHandle = default(GeneratorInstance);
				Debug.LogError(string.Format("Trying to play object {0}, but it doesn't implement {1}", generatorObjectDefinition, "IAudioGenerator"));
			}
		}

		internal unsafe static void InitializeGeneratorHandle(GeneratorInstance.GeneratorHeader* header, ControlHeader* control, AudioConfiguration* nestedConfiguration, ProcessorInstance.InitializationFlags flags)
		{
			ScriptableGeneratorBindings.InternalInitializeGeneratorHandle((void*)header, (void*)control, nestedConfiguration, flags);
		}

		[NativeMethod(Name = "audio::InitializeGeneratorHandle", IsFreeFunction = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void InternalInitializeGeneratorHandle(void* header, void* control, AudioConfiguration* nestedConfiguration, ProcessorInstance.InitializationFlags flags);
	}
}
