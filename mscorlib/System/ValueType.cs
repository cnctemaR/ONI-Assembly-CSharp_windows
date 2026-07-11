using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public abstract class ValueType
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool InternalEquals(object o1, object o2, out object[] fields);

		internal static bool DefaultEquals(object o1, object o2)
		{
			if (o2 == null)
			{
				return false;
			}
			object[] array;
			bool flag = ValueType.InternalEquals(o1, o2, out array);
			if (array == null)
			{
				return flag;
			}
			for (int i = 0; i < array.Length; i += 2)
			{
				object obj = array[i];
				object obj2 = array[i + 1];
				if (obj == null)
				{
					if (obj2 != null)
					{
						return false;
					}
				}
				else if (!obj.Equals(obj2))
				{
					return false;
				}
			}
			return true;
		}

		public override bool Equals(object obj)
		{
			return ValueType.DefaultEquals(this, obj);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int InternalGetHashCode(object o, out object[] fields);

		public override int GetHashCode()
		{
			object[] array;
			int num = ValueType.InternalGetHashCode(this, out array);
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] != null)
					{
						num ^= array[i].GetHashCode();
					}
				}
			}
			return num;
		}

		public override string ToString()
		{
			return base.GetType().FullName;
		}
	}
}
