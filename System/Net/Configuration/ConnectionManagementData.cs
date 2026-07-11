using System;
using System.Collections;

namespace System.Net.Configuration
{
	internal class ConnectionManagementData
	{
		public ConnectionManagementData(object parent)
		{
			this.data = new Hashtable(CaseInsensitiveHashCodeProvider.DefaultInvariant, CaseInsensitiveComparer.DefaultInvariant);
			if (parent != null && parent is ConnectionManagementData)
			{
				ConnectionManagementData connectionManagementData = (ConnectionManagementData)parent;
				foreach (object obj in connectionManagementData.data.Keys)
				{
					string text = (string)obj;
					this.data[text] = connectionManagementData.data[text];
				}
			}
		}

		public void Add(string address, string nconns)
		{
			if (nconns == null || nconns == "")
			{
				nconns = "2";
			}
			this.data[address] = uint.Parse(nconns);
		}

		public void Add(string address, int nconns)
		{
			this.data[address] = (uint)nconns;
		}

		public void Remove(string address)
		{
			this.data.Remove(address);
		}

		public void Clear()
		{
			this.data.Clear();
		}

		public uint GetMaxConnections(string hostOrIP)
		{
			object obj = this.data[hostOrIP];
			if (obj == null)
			{
				obj = this.data["*"];
			}
			if (obj == null)
			{
				return 2U;
			}
			return (uint)obj;
		}

		public Hashtable Data
		{
			get
			{
				return this.data;
			}
		}

		private Hashtable data;

		private const int defaultMaxConnections = 2;
	}
}
