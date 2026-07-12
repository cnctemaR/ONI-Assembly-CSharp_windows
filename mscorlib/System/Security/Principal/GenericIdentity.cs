using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace System.Security.Principal
{
	[Serializable]
	public class GenericIdentity : ClaimsIdentity
	{
		public GenericIdentity(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			this.m_name = name;
			this.m_type = "";
			this.AddNameClaim();
		}

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
			this.AddNameClaim();
		}

		private GenericIdentity()
		{
		}

		protected GenericIdentity(GenericIdentity identity)
			: base(identity)
		{
			this.m_name = identity.m_name;
			this.m_type = identity.m_type;
		}

		public override ClaimsIdentity Clone()
		{
			return new GenericIdentity(this);
		}

		public override IEnumerable<Claim> Claims
		{
			get
			{
				return base.Claims;
			}
		}

		public override string Name
		{
			get
			{
				return this.m_name;
			}
		}

		public override string AuthenticationType
		{
			get
			{
				return this.m_type;
			}
		}

		public override bool IsAuthenticated
		{
			get
			{
				return !this.m_name.Equals("");
			}
		}

		private void AddNameClaim()
		{
			if (this.m_name != null)
			{
				base.AddClaim(new Claim(base.NameClaimType, this.m_name, "http://www.w3.org/2001/XMLSchema#string", "LOCAL AUTHORITY", "LOCAL AUTHORITY", this));
			}
		}

		private readonly string m_name;

		private readonly string m_type;
	}
}
