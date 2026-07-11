using System;
using System.Reflection;

namespace Mono
{
	internal struct RuntimeGenericParamInfoHandle
	{
		internal unsafe RuntimeGenericParamInfoHandle(RuntimeStructs.GenericParamInfo* value)
		{
			this.value = value;
		}

		internal unsafe RuntimeGenericParamInfoHandle(IntPtr ptr)
		{
			this.value = (RuntimeStructs.GenericParamInfo*)(void*)ptr;
		}

		internal Type[] Constraints
		{
			get
			{
				return this.GetConstraints();
			}
		}

		internal unsafe GenericParameterAttributes Attributes
		{
			get
			{
				return (GenericParameterAttributes)this.value->flags;
			}
		}

		private unsafe Type[] GetConstraints()
		{
			int constraintsCount = this.GetConstraintsCount();
			Type[] array = new Type[constraintsCount];
			for (int i = 0; i < constraintsCount; i++)
			{
				RuntimeClassHandle runtimeClassHandle = new RuntimeClassHandle(*(IntPtr*)(this.value->constraints + (IntPtr)i * (IntPtr)sizeof(RuntimeStructs.MonoClass*) / (IntPtr)sizeof(RuntimeStructs.MonoClass*)));
				array[i] = Type.GetTypeFromHandle(runtimeClassHandle.GetTypeHandle());
			}
			return array;
		}

		private unsafe int GetConstraintsCount()
		{
			int num = 0;
			RuntimeStructs.MonoClass** ptr = this.value->constraints;
			while (ptr != null && *(IntPtr*)ptr != (IntPtr)((UIntPtr)0))
			{
				ptr += sizeof(RuntimeStructs.MonoClass*) / sizeof(RuntimeStructs.MonoClass*);
				num++;
			}
			return num;
		}

		private unsafe RuntimeStructs.GenericParamInfo* value;
	}
}
