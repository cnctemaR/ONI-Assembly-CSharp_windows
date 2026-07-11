using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Runtime/BaseClasses/TagManager.h")]
	[NativeClass("BitField", "struct BitField;")]
	[NativeHeader("Runtime/BaseClasses/BitField.h")]
	[RequiredByNativeCode(Optional = true, GenerateProxy = true)]
	public struct LayerMask
	{
		public static implicit operator int(LayerMask mask)
		{
			return mask.m_Mask;
		}

		public static implicit operator LayerMask(int intVal)
		{
			LayerMask layerMask;
			layerMask.m_Mask = intVal;
			return layerMask;
		}

		public int value
		{
			get
			{
				return this.m_Mask;
			}
			set
			{
				this.m_Mask = value;
			}
		}

		[NativeMethod("LayerToString")]
		[StaticAccessor("GetTagManager()", StaticAccessorType.Dot)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string LayerToName(int layer);

		[StaticAccessor("GetTagManager()", StaticAccessorType.Dot)]
		[NativeMethod("StringToLayer")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int NameToLayer(string layerName);

		public static int GetMask(params string[] layerNames)
		{
			if (layerNames == null)
			{
				throw new ArgumentNullException("layerNames");
			}
			int num = 0;
			foreach (string text in layerNames)
			{
				int num2 = LayerMask.NameToLayer(text);
				if (num2 != -1)
				{
					num |= 1 << num2;
				}
			}
			return num;
		}

		[NativeName("m_Bits")]
		private int m_Mask;
	}
}
