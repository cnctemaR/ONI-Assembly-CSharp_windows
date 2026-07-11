using System;
using System.Security;
using System.Security.Permissions;

namespace System.Configuration
{
	[Serializable]
	public sealed class ConfigurationPermission : CodeAccessPermission, IUnrestrictedPermission
	{
		public ConfigurationPermission(PermissionState state)
		{
			this.unrestricted = state == PermissionState.Unrestricted;
		}

		public override IPermission Copy()
		{
			return new ConfigurationPermission((!this.unrestricted) ? PermissionState.None : PermissionState.Unrestricted);
		}

		public override void FromXml(SecurityElement securityElement)
		{
			if (securityElement == null)
			{
				throw new ArgumentNullException("securityElement");
			}
			if (securityElement.Tag != "IPermission")
			{
				throw new ArgumentException("securityElement");
			}
			string text = securityElement.Attribute("Unrestricted");
			if (text != null)
			{
				this.unrestricted = string.Compare(text, "true", StringComparison.InvariantCultureIgnoreCase) == 0;
			}
		}

		public override IPermission Intersect(IPermission target)
		{
			if (target == null)
			{
				return null;
			}
			ConfigurationPermission configurationPermission = target as ConfigurationPermission;
			if (configurationPermission == null)
			{
				throw new ArgumentException("target");
			}
			return new ConfigurationPermission((!this.unrestricted || !configurationPermission.IsUnrestricted()) ? PermissionState.None : PermissionState.Unrestricted);
		}

		public override IPermission Union(IPermission target)
		{
			if (target == null)
			{
				return this.Copy();
			}
			ConfigurationPermission configurationPermission = target as ConfigurationPermission;
			if (configurationPermission == null)
			{
				throw new ArgumentException("target");
			}
			return new ConfigurationPermission((!this.unrestricted && !configurationPermission.IsUnrestricted()) ? PermissionState.None : PermissionState.Unrestricted);
		}

		public override bool IsSubsetOf(IPermission target)
		{
			if (target == null)
			{
				return !this.unrestricted;
			}
			ConfigurationPermission configurationPermission = target as ConfigurationPermission;
			if (configurationPermission == null)
			{
				throw new ArgumentException("target");
			}
			return !this.unrestricted || configurationPermission.IsUnrestricted();
		}

		public bool IsUnrestricted()
		{
			return this.unrestricted;
		}

		public override SecurityElement ToXml()
		{
			SecurityElement securityElement = new SecurityElement("IPermission");
			securityElement.AddAttribute("class", base.GetType().AssemblyQualifiedName);
			securityElement.AddAttribute("version", "1");
			if (this.unrestricted)
			{
				securityElement.AddAttribute("Unrestricted", "true");
			}
			return securityElement;
		}

		private bool unrestricted;
	}
}
