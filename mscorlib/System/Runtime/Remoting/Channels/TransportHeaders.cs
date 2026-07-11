using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Security;

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
			[SecurityCritical]
			get
			{
				return this.hash_table[key];
			}
			[SecurityCritical]
			set
			{
				this.hash_table[key] = value;
			}
		}

		[SecurityCritical]
		public IEnumerator GetEnumerator()
		{
			return this.hash_table.GetEnumerator();
		}

		private Hashtable hash_table;
	}
}
