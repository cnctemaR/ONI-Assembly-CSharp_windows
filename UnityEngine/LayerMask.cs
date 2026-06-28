using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
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

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string LayerToName(int layer);

		[GeneratedByOldBindingsGenerator]
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

		private int m_Mask;
	}
}
