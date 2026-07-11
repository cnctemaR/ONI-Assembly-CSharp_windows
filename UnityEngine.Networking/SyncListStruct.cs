using System;

namespace UnityEngine.Networking
{
	[Obsolete("The high level API classes are deprecated and will be removed in the future.")]
	public class SyncListStruct<T> : SyncList<T> where T : struct
	{
		public new void AddInternal(T item)
		{
			base.AddInternal(item);
		}

		protected override void SerializeItem(NetworkWriter writer, T item)
		{
		}

		protected override T DeserializeItem(NetworkReader reader)
		{
			return new T();
		}

		public T GetItem(int i)
		{
			return base[i];
		}

		public new ushort Count
		{
			get
			{
				return (ushort)base.Count;
			}
		}
	}
}
