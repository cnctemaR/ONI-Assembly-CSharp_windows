using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequiredByNativeCode(Optional = true, GenerateProxy = true)]
	[NativeHeader("Runtime/BaseClasses/TagManager.h")]
	[NativeHeader("Runtime/Graphics/RenderingLayerMask.h")]
	[NativeClass("RenderingLayerMask", "struct RenderingLayerMask;")]
	[Serializable]
	public struct RenderingLayerMask
	{
		public static RenderingLayerMask defaultRenderingLayerMask { get; } = new RenderingLayerMask
		{
			m_Bits = 1U
		};

		public static implicit operator uint(RenderingLayerMask mask)
		{
			return mask.m_Bits;
		}

		public static implicit operator RenderingLayerMask(uint intVal)
		{
			RenderingLayerMask renderingLayerMask;
			renderingLayerMask.m_Bits = intVal;
			return renderingLayerMask;
		}

		public static implicit operator int(RenderingLayerMask mask)
		{
			return (int)mask.m_Bits;
		}

		public static implicit operator RenderingLayerMask(int intVal)
		{
			RenderingLayerMask renderingLayerMask;
			renderingLayerMask.m_Bits = (uint)intVal;
			return renderingLayerMask;
		}

		public uint value
		{
			get
			{
				return this.m_Bits;
			}
			set
			{
				this.m_Bits = value;
			}
		}

		[NativeMethod("GetDefaultRenderingLayerValue")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint Internal_GetDefaultRenderingLayerValue();

		[NativeMethod("RenderingLayerToString")]
		[StaticAccessor("GetTagManager()", StaticAccessorType.Dot)]
		public static string RenderingLayerToName(int layer)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				RenderingLayerMask.RenderingLayerToName_Injected(layer, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		[StaticAccessor("GetTagManager()", StaticAccessorType.Dot)]
		[NativeMethod("StringToRenderingLayer")]
		public unsafe static int NameToRenderingLayer(string layerName)
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
				num = RenderingLayerMask.NameToRenderingLayer_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return num;
		}

		public static uint GetMask(params string[] renderingLayerNames)
		{
			bool flag = renderingLayerNames == null;
			if (flag)
			{
				throw new ArgumentNullException("renderingLayerNames");
			}
			uint num = 0U;
			for (int i = 0; i < renderingLayerNames.Length; i++)
			{
				int num2 = RenderingLayerMask.NameToRenderingLayer(renderingLayerNames[i]);
				bool flag2 = num2 != -1;
				if (flag2)
				{
					num |= 1U << num2;
				}
			}
			return num;
		}

		public unsafe static uint GetMask(ReadOnlySpan<string> renderingLayerNames)
		{
			bool flag = renderingLayerNames == null;
			if (flag)
			{
				throw new ArgumentNullException("renderingLayerNames");
			}
			uint num = 0U;
			ReadOnlySpan<string> readOnlySpan = renderingLayerNames;
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				string text = *readOnlySpan[i];
				int num2 = RenderingLayerMask.NameToRenderingLayer(text);
				bool flag2 = num2 != -1;
				if (flag2)
				{
					num |= 1U << num2;
				}
			}
			return num;
		}

		[StaticAccessor("GetTagManager()", StaticAccessorType.Dot)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetDefinedRenderingLayerCount();

		[StaticAccessor("GetTagManager()", StaticAccessorType.Dot)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetLastDefinedRenderingLayerIndex();

		[StaticAccessor("GetTagManager()", StaticAccessorType.Dot)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern uint GetDefinedRenderingLayersCombinedMaskValue();

		[StaticAccessor("GetTagManager()", StaticAccessorType.Dot)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] GetDefinedRenderingLayerNames();

		[StaticAccessor("GetTagManager()", StaticAccessorType.Dot)]
		public static int[] GetDefinedRenderingLayerValues()
		{
			int[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				RenderingLayerMask.GetDefinedRenderingLayerValues_Injected(out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				int[] array;
				blittableArrayWrapper.Unmarshal<int>(ref array);
				array2 = array;
			}
			return array2;
		}

		[StaticAccessor("GetTagManager()", StaticAccessorType.Dot)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetRenderingLayerCount();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RenderingLayerToName_Injected(int layer, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int NameToRenderingLayer_Injected(ref ManagedSpanWrapper layerName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetDefinedRenderingLayerValues_Injected(out BlittableArrayWrapper ret);

		[NativeName("m_Bits")]
		private uint m_Bits;

		internal const int maxRenderingLayerSize = 32;
	}
}
