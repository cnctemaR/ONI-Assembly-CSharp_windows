using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace System.Runtime.Serialization
{
	[ComVisible(true)]
	public sealed class SerializationInfoEnumerator : IEnumerator
	{
		internal SerializationInfoEnumerator(ArrayList list)
		{
			this.enumerator = list.GetEnumerator();
		}

		object IEnumerator.Current
		{
			get
			{
				return this.enumerator.Current;
			}
		}

		public SerializationEntry Current
		{
			get
			{
				return (SerializationEntry)this.enumerator.Current;
			}
		}

		public string Name
		{
			get
			{
				SerializationEntry serializationEntry = this.Current;
				return serializationEntry.Name;
			}
		}

		public Type ObjectType
		{
			get
			{
				SerializationEntry serializationEntry = this.Current;
				return serializationEntry.ObjectType;
			}
		}

		public object Value
		{
			get
			{
				SerializationEntry serializationEntry = this.Current;
				return serializationEntry.Value;
			}
		}

		public bool MoveNext()
		{
			return this.enumerator.MoveNext();
		}

		public void Reset()
		{
			this.enumerator.Reset();
		}

		private IEnumerator enumerator;
	}
}
