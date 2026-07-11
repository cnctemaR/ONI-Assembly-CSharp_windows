using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

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
			if (o1 == null && o2 == null)
			{
				return true;
			}
			if (o1 == null || o2 == null)
			{
				return false;
			}
			RuntimeType runtimeType = (RuntimeType)o1.GetType();
			RuntimeType runtimeType2 = (RuntimeType)o2.GetType();
			if (runtimeType != runtimeType2)
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

		internal static int GetHashCodeOfPtr(IntPtr ptr)
		{
			int num = (int)ptr;
			int num2 = ValueType.Internal.hash_code_of_ptr_seed;
			if (num2 == 0)
			{
				num2 = num;
				Interlocked.CompareExchange(ref ValueType.Internal.hash_code_of_ptr_seed, num2, 0);
				num2 = ValueType.Internal.hash_code_of_ptr_seed;
			}
			return num - num2;
		}

		public override string ToString()
		{
			return base.GetType().FullName;
		}

		private static class Internal
		{
			public static int hash_code_of_ptr_seed;
		}
	}
}
