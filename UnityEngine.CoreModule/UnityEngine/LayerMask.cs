using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeClass("BitField", "struct BitField;")]
	[NativeHeader("Runtime/BaseClasses/TagManager.h")]
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

		[StaticAccessor("GetTagManager()", StaticAccessorType.Dot)]
		[NativeMethod("LayerToString")]
		public static string LayerToName(int layer)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				LayerMask.LayerToName_Injected(layer, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		[StaticAccessor("GetTagManager()", StaticAccessorType.Dot)]
		[NativeMethod("StringToLayer")]
		public unsafe static int NameToLayer(string layerName)
		{
			int num;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(layerName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = layerName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				num = LayerMask.NameToLayer_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return num;
		}

		public static int GetMask(params string[] layerNames)
		{
			bool flag = layerNames == null;
			if (flag)
			{
				throw new ArgumentNullException("layerNames");
			}
			int num = 0;
			foreach (string text in layerNames)
			{
				int num2 = LayerMask.NameToLayer(text);
				bool flag2 = num2 != -1;
				if (flag2)
				{
					num |= 1 << num2;
				}
			}
			return num;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void LayerToName_Injected(int layer, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int NameToLayer_Injected(ref ManagedSpanWrapper layerName);

		[NativeName("m_Bits")]
		private int m_Mask;
	}
}
