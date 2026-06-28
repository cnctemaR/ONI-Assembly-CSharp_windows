using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace System.Runtime.Serialization
{
	[MonoTODO("Serialization format not compatible with.NET")]
	[ComVisible(true)]
	[Serializable]
	public class ObjectIDGenerator
	{
		public ObjectIDGenerator()
		{
			this.table = new Hashtable(ObjectIDGenerator.comparer, ObjectIDGenerator.comparer);
			this.current = 1L;
		}

		public virtual long GetId(object obj, out bool firstTime)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			object obj2 = this.table[obj];
			if (obj2 != null)
			{
				firstTime = false;
				return (long)obj2;
			}
			firstTime = true;
			this.table.Add(obj, this.current);
			long num;
			this.current = (num = this.current) + 1L;
			return num;
		}

		public virtual long HasId(object obj, out bool firstTime)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			object obj2 = this.table[obj];
			if (obj2 != null)
			{
				firstTime = false;
				return (long)obj2;
			}
			firstTime = true;
			return 0L;
		}

		internal long NextId
		{
			get
			{
				long num;
				this.current = (num = this.current) + 1L;
				return num;
			}
		}

		private Hashtable table;

		private long current;

		private static ObjectIDGenerator.InstanceComparer comparer = new ObjectIDGenerator.InstanceComparer();

		private class InstanceComparer : IComparer, IHashCodeProvider
		{
			int IComparer.Compare(object o1, object o2)
			{
				if (o1 is string)
				{
					return (!o1.Equals(o2)) ? 1 : 0;
				}
				return (o1 != o2) ? 1 : 0;
			}

			int IHashCodeProvider.GetHashCode(object o)
			{
				return object.InternalGetHashCode(o);
			}
		}
	}
}
