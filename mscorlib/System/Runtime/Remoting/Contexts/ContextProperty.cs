using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Contexts
{
	[ComVisible(true)]
	public class ContextProperty
	{
		private ContextProperty(string name, object prop)
		{
			this.name = name;
			this.prop = prop;
		}

		public virtual string Name
		{
			get
			{
				return this.name;
			}
		}

		public virtual object Property
		{
			get
			{
				return this.prop;
			}
		}

		private string name;

		private object prop;
	}
}
