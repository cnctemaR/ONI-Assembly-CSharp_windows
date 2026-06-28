using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;

namespace System.Net.NetworkInformation
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	[Serializable]
	public sealed class NetworkInformationPermissionAttribute : CodeAccessSecurityAttribute
	{
		public NetworkInformationPermissionAttribute(SecurityAction action)
			: base(action)
		{
		}

		[global::System.MonoTODO("verify implementation")]
		public override IPermission CreatePermission()
		{
			NetworkInformationAccess networkInformationAccess = NetworkInformationAccess.None;
			string text = this.Access;
			if (text != null)
			{
				if (NetworkInformationPermissionAttribute.<>f__switch$map11 == null)
				{
					NetworkInformationPermissionAttribute.<>f__switch$map11 = new Dictionary<string, int>(2)
					{
						{ "Read", 0 },
						{ "Full", 1 }
					};
				}
				int num;
				if (NetworkInformationPermissionAttribute.<>f__switch$map11.TryGetValue(text, out num))
				{
					if (num != 0)
					{
						if (num == 1)
						{
							networkInformationAccess = NetworkInformationAccess.Read | NetworkInformationAccess.Ping;
						}
					}
					else
					{
						networkInformationAccess = NetworkInformationAccess.Read;
					}
				}
			}
			return new NetworkInformationPermission(networkInformationAccess);
		}

		public string Access
		{
			get
			{
				return this.access;
			}
			set
			{
				string text = this.access;
				if (text != null)
				{
					if (NetworkInformationPermissionAttribute.<>f__switch$map10 == null)
					{
						NetworkInformationPermissionAttribute.<>f__switch$map10 = new Dictionary<string, int>(3)
						{
							{ "Read", 0 },
							{ "Full", 0 },
							{ "None", 0 }
						};
					}
					int num;
					if (NetworkInformationPermissionAttribute.<>f__switch$map10.TryGetValue(text, out num))
					{
						if (num == 0)
						{
							this.access = value;
							return;
						}
					}
				}
				throw new ArgumentException("Only 'Read', 'Full' and 'None' are allowed");
			}
		}

		private string access;
	}
}
