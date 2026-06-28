using System;
using System.Runtime.InteropServices;

namespace System.Security.Policy
{
	[ComVisible(true)]
	[Serializable]
	public sealed class PermissionRequestEvidence : IBuiltInEvidence
	{
		public PermissionRequestEvidence(PermissionSet request, PermissionSet optional, PermissionSet denied)
		{
			if (request != null)
			{
				this.requested = new PermissionSet(request);
			}
			if (optional != null)
			{
				this.optional = new PermissionSet(optional);
			}
			if (denied != null)
			{
				this.denied = new PermissionSet(denied);
			}
		}

		int IBuiltInEvidence.GetRequiredSize(bool verbose)
		{
			int num = ((!verbose) ? 1 : 3);
			if (this.requested != null)
			{
				int num2 = this.requested.ToXml().ToString().Length + ((!verbose) ? 0 : 5);
				num += num2;
			}
			if (this.optional != null)
			{
				int num3 = this.optional.ToXml().ToString().Length + ((!verbose) ? 0 : 5);
				num += num3;
			}
			if (this.denied != null)
			{
				int num4 = this.denied.ToXml().ToString().Length + ((!verbose) ? 0 : 5);
				num += num4;
			}
			return num;
		}

		[MonoTODO("IBuiltInEvidence")]
		int IBuiltInEvidence.InitFromBuffer(char[] buffer, int position)
		{
			return 0;
		}

		[MonoTODO("IBuiltInEvidence")]
		int IBuiltInEvidence.OutputToBuffer(char[] buffer, int position, bool verbose)
		{
			return 0;
		}

		public PermissionSet DeniedPermissions
		{
			get
			{
				return this.denied;
			}
		}

		public PermissionSet OptionalPermissions
		{
			get
			{
				return this.optional;
			}
		}

		public PermissionSet RequestedPermissions
		{
			get
			{
				return this.requested;
			}
		}

		public PermissionRequestEvidence Copy()
		{
			return new PermissionRequestEvidence(this.requested, this.optional, this.denied);
		}

		public override string ToString()
		{
			SecurityElement securityElement = new SecurityElement("System.Security.Policy.PermissionRequestEvidence");
			securityElement.AddAttribute("version", "1");
			if (this.requested != null)
			{
				SecurityElement securityElement2 = new SecurityElement("Request");
				securityElement2.AddChild(this.requested.ToXml());
				securityElement.AddChild(securityElement2);
			}
			if (this.optional != null)
			{
				SecurityElement securityElement3 = new SecurityElement("Optional");
				securityElement3.AddChild(this.optional.ToXml());
				securityElement.AddChild(securityElement3);
			}
			if (this.denied != null)
			{
				SecurityElement securityElement4 = new SecurityElement("Denied");
				securityElement4.AddChild(this.denied.ToXml());
				securityElement.AddChild(securityElement4);
			}
			return securityElement.ToString();
		}

		private PermissionSet requested;

		private PermissionSet optional;

		private PermissionSet denied;
	}
}
