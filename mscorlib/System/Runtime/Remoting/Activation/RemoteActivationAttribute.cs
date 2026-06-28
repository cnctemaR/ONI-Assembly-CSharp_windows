using System;
using System.Collections;
using System.Runtime.Remoting.Contexts;

namespace System.Runtime.Remoting.Activation
{
	internal class RemoteActivationAttribute : Attribute, IContextAttribute
	{
		public RemoteActivationAttribute()
		{
		}

		public RemoteActivationAttribute(IList contextProperties)
		{
			this._contextProperties = contextProperties;
		}

		public bool IsContextOK(Context ctx, IConstructionCallMessage ctor)
		{
			return false;
		}

		public void GetPropertiesForNewContext(IConstructionCallMessage ctor)
		{
			if (this._contextProperties != null)
			{
				foreach (object obj in this._contextProperties)
				{
					ctor.ContextProperties.Add(obj);
				}
			}
		}

		private IList _contextProperties;
	}
}
