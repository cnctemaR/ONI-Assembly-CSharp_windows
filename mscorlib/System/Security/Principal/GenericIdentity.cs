using System;
using System.Runtime.InteropServices;

namespace System.Security.Principal
{
	[ComVisible(true)]
	[Serializable]
	public class GenericIdentity : IIdentity
	{
		public GenericIdentity(string name, string type)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			this.m_name = name;
			this.m_type = type;
		}

		public GenericIdentity(string name)
			: this(name, string.Empty)
		{
		}

		public virtual string AuthenticationType
		{
			get
			{
				return this.m_type;
			}
		}

		public virtual string Name
		{
			get
			{
				return this.m_name;
			}
		}

		public virtual bool IsAuthenticated
		{
			get
			{
				return this.m_name.Length > 0;
			}
		}

		private string m_name;

		private string m_type;
	}
}
