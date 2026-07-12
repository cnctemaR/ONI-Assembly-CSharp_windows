using System;

namespace Unity.Collections
{
	internal sealed class NativeArrayReadOnlyDebugView<T> where T : struct
	{
		public NativeArrayReadOnlyDebugView(NativeArray<T>.ReadOnly array)
		{
			this.m_Array = array;
		}

		public T[] Items
		{
			get
			{
				bool flag = !this.m_Array.IsCreated;
				T[] array;
				if (flag)
				{
					array = null;
				}
				else
				{
					array = this.m_Array.ToArray();
				}
				return array;
			}
		}

		private NativeArray<T>.ReadOnly m_Array;
	}
}
