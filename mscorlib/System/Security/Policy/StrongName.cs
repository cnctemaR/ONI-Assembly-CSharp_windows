using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Security.Policy
{
	[ComVisible(true)]
	[Serializable]
	public sealed class StrongName : EvidenceBase, IIdentityPermissionFactory, IBuiltInEvidence
	{
		public StrongName(StrongNamePublicKeyBlob blob, string name, Version version)
		{
			if (blob == null)
			{
				throw new ArgumentNullException("blob");
			}
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (version == null)
			{
				throw new ArgumentNullException("version");
			}
			if (name.Length == 0)
			{
				throw new ArgumentException(Locale.GetText("Empty"), "name");
			}
			this.publickey = blob;
			this.name = name;
			this.version = version;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public StrongNamePublicKeyBlob PublicKey
		{
			get
			{
				return this.publickey;
			}
		}

		public Version Version
		{
			get
			{
				return this.version;
			}
		}

		public object Copy()
		{
			return new StrongName(this.publickey, this.name, this.version);
		}

		public IPermission CreateIdentityPermission(Evidence evidence)
		{
			return new StrongNameIdentityPermission(this.publickey, this.name, this.version);
		}

		public override bool Equals(object o)
		{
			StrongName strongName = o as StrongName;
			return strongName != null && !(this.name != strongName.Name) && this.Version.Equals(strongName.Version) && this.PublicKey.Equals(strongName.PublicKey);
		}

		public override int GetHashCode()
		{
			return this.publickey.GetHashCode();
		}

		public override string ToString()
		{
			SecurityElement securityElement = new SecurityElement(typeof(StrongName).Name);
			securityElement.AddAttribute("version", "1");
			securityElement.AddAttribute("Key", this.publickey.ToString());
			securityElement.AddAttribute("Name", this.name);
			securityElement.AddAttribute("Version", this.version.ToString());
			return securityElement.ToString();
		}

		int IBuiltInEvidence.GetRequiredSize(bool verbose)
		{
			return (verbose ? 5 : 1) + this.name.Length;
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

		private StrongNamePublicKeyBlob publickey;

		private string name;

		private Version version;
	}
}
