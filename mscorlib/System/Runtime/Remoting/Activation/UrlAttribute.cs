using System;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;
using System.Security;

namespace System.Runtime.Remoting.Activation
{
	[ComVisible(true)]
	[Serializable]
	public sealed class UrlAttribute : ContextAttribute
	{
		public UrlAttribute(string callsiteURL)
			: base(callsiteURL)
		{
			this.url = callsiteURL;
		}

		public string UrlValue
		{
			get
			{
				return this.url;
			}
		}

		public override bool Equals(object o)
		{
			return o is UrlAttribute && ((UrlAttribute)o).UrlValue == this.url;
		}

		public override int GetHashCode()
		{
			return this.url.GetHashCode();
		}

		[ComVisible(true)]
		[SecurityCritical]
		public override void GetPropertiesForNewContext(IConstructionCallMessage ctorMsg)
		{
		}

		[SecurityCritical]
		[ComVisible(true)]
		public override bool IsContextOK(Context ctx, IConstructionCallMessage msg)
		{
			return true;
		}

		private string url;
	}
}
