using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Channels
{
	[MonoTODO("Serialization format not compatible with .NET")]
	[ComVisible(true)]
	[Serializable]
	public class TransportHeaders : ITransportHeaders
	{
		public TransportHeaders()
		{
			this.hash_table = new Hashtable(CaseInsensitiveHashCodeProvider.DefaultInvariant, CaseInsensitiveComparer.DefaultInvariant);
		}

		public object this[object key]
		{
			get
			{
				return this.hash_table[key];
			}
			set
			{
				this.hash_table[key] = value;
			}
		}

		public IEnumerator GetEnumerator()
		{
			return this.hash_table.GetEnumerator();
		}

		private Hashtable hash_table;
	}
}
