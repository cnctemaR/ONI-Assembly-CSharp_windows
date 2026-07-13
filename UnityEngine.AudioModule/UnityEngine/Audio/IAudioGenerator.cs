using System;
using UnityEngine.Scripting;

namespace UnityEngine.Audio
{
	[UsedByNativeCode]
	public interface IAudioGenerator : GeneratorInstance.ICapabilities
	{
		GeneratorInstance CreateInstance(ControlContext context, AudioFormat? nestedFormat, ProcessorInstance.CreationParameters creationParameters);

		[Serializable]
		public struct Serializable
		{
			public IAudioGenerator definition
			{
				get
				{
					return this.Reference as IAudioGenerator;
				}
				set
				{
					this.Reference = (Object)value;
				}
			}

			public T Get<T>() where T : Object, IAudioGenerator
			{
				return this.Reference as T;
			}

			public void Set<T>(T value) where T : Object, IAudioGenerator
			{
				this.Reference = value;
			}

			public Serializable(IAudioGenerator audioGenerator)
			{
				this.Reference = (Object)audioGenerator;
			}

			[SerializeField]
			internal Object Reference;
		}
	}
}
