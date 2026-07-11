using System;
using System.Collections;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Security.Policy
{
	[ComVisible(true)]
	[Serializable]
	public sealed class SiteMembershipCondition : IMembershipCondition, ISecurityEncodable, ISecurityPolicyEncodable, IConstantMembershipCondition
	{
		internal SiteMembershipCondition()
		{
		}

		public SiteMembershipCondition(string site)
		{
			this.Site = site;
		}

		public string Site
		{
			get
			{
				return this._site;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("site");
				}
				if (!global::System.Security.Policy.Site.IsValid(value))
				{
					throw new ArgumentException("invalid site");
				}
				this._site = value;
			}
		}

		public bool Check(Evidence evidence)
		{
			if (evidence == null)
			{
				return false;
			}
			IEnumerator hostEnumerator = evidence.GetHostEnumerator();
			while (hostEnumerator.MoveNext())
			{
				if (hostEnumerator.Current is Site)
				{
					string[] array = this._site.Split(new char[] { '.' });
					string[] array2 = (hostEnumerator.Current as Site).origin_site.Split(new char[] { '.' });
					int i = array.Length - 1;
					int num = array2.Length - 1;
					while (i >= 0)
					{
						if (i == 0)
						{
							return string.Compare(array[0], "*", true, CultureInfo.InvariantCulture) == 0;
						}
						if (string.Compare(array[i], array2[num], true, CultureInfo.InvariantCulture) != 0)
						{
							return false;
						}
						i--;
						num--;
					}
					return true;
				}
			}
			return false;
		}

		public IMembershipCondition Copy()
		{
			return new SiteMembershipCondition(this._site);
		}

		public override bool Equals(object o)
		{
			return o != null && o is SiteMembershipCondition && new Site((o as SiteMembershipCondition)._site).Equals(new Site(this._site));
		}

		public void FromXml(SecurityElement e)
		{
			this.FromXml(e, null);
		}

		public void FromXml(SecurityElement e, PolicyLevel level)
		{
			MembershipConditionHelper.CheckSecurityElement(e, "e", this.version, this.version);
			this._site = e.Attribute("Site");
		}

		public override int GetHashCode()
		{
			return this._site.GetHashCode();
		}

		public override string ToString()
		{
			return "Site - " + this._site;
		}

		public SecurityElement ToXml()
		{
			return this.ToXml(null);
		}

		public SecurityElement ToXml(PolicyLevel level)
		{
			SecurityElement securityElement = MembershipConditionHelper.Element(typeof(SiteMembershipCondition), this.version);
			securityElement.AddAttribute("Site", this._site);
			return securityElement;
		}

		private readonly int version = 1;

		private string _site;
	}
}
