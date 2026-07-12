using System;

namespace System.Runtime.Serialization
{
	[DataContract(Namespace = "http://schemas.microsoft.com/2003/10/Serialization/Arrays")]
	internal struct KeyValue<K, V>
	{
		internal KeyValue(K key, V value)
		{
			this.key = key;
			this.value = value;
		}

		[DataMember(IsRequired = true)]
		public K Key
		{
			get
			{
				return this.key;
			}
			set
			{
				this.key = value;
			}
		}

		[DataMember(IsRequired = true)]
		public V Value
		{
			get
			{
				return this.value;
			}
			set
			{
				this.value = value;
			}
		}

		private K key;

		private V value;
	}
}
