using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Channels
{
	[ComVisible(true)]
	public class SinkProviderData
	{
		public SinkProviderData(string name)
		{
			this.sinkName = name;
			this.children = new ArrayList();
			this.properties = new Hashtable();
		}

		public IList Children
		{
			get
			{
				return this.children;
			}
		}

		public string Name
		{
			get
			{
				return this.sinkName;
			}
		}

		public IDictionary Properties
		{
			get
			{
				return this.properties;
			}
		}

		private string sinkName;

		private ArrayList children;

		private Hashtable properties;
	}
}
