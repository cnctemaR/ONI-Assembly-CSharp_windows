using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Security.Policy
{
	[ComVisible(true)]
	[Serializable]
	public abstract class CodeGroup
	{
		protected CodeGroup(IMembershipCondition membershipCondition, PolicyStatement policy)
		{
			if (membershipCondition == null)
			{
				throw new ArgumentNullException("membershipCondition");
			}
			if (policy != null)
			{
				this.m_policy = policy.Copy();
			}
			this.m_membershipCondition = membershipCondition.Copy();
		}

		internal CodeGroup(SecurityElement e, PolicyLevel level)
		{
			this.FromXml(e, level);
		}

		public abstract CodeGroup Copy();

		public abstract string MergeLogic { get; }

		public abstract PolicyStatement Resolve(Evidence evidence);

		public abstract CodeGroup ResolveMatchingCodeGroups(Evidence evidence);

		public PolicyStatement PolicyStatement
		{
			get
			{
				return this.m_policy;
			}
			set
			{
				this.m_policy = value;
			}
		}

		public string Description
		{
			get
			{
				return this.m_description;
			}
			set
			{
				this.m_description = value;
			}
		}

		public IMembershipCondition MembershipCondition
		{
			get
			{
				return this.m_membershipCondition;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentException("value");
				}
				this.m_membershipCondition = value;
			}
		}

		public string Name
		{
			get
			{
				return this.m_name;
			}
			set
			{
				this.m_name = value;
			}
		}

		public IList Children
		{
			get
			{
				return this.m_children;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.m_children = new ArrayList(value);
			}
		}

		public virtual string AttributeString
		{
			get
			{
				if (this.m_policy != null)
				{
					return this.m_policy.AttributeString;
				}
				return null;
			}
		}

		public virtual string PermissionSetName
		{
			get
			{
				if (this.m_policy == null)
				{
					return null;
				}
				if (this.m_policy.PermissionSet is NamedPermissionSet)
				{
					return ((NamedPermissionSet)this.m_policy.PermissionSet).Name;
				}
				return null;
			}
		}

		public void AddChild(CodeGroup group)
		{
			if (group == null)
			{
				throw new ArgumentNullException("group");
			}
			this.m_children.Add(group.Copy());
		}

		public override bool Equals(object o)
		{
			CodeGroup codeGroup = o as CodeGroup;
			return codeGroup != null && this.Equals(codeGroup, false);
		}

		public bool Equals(CodeGroup cg, bool compareChildren)
		{
			if (cg.Name != this.Name)
			{
				return false;
			}
			if (cg.Description != this.Description)
			{
				return false;
			}
			if (!cg.MembershipCondition.Equals(this.m_membershipCondition))
			{
				return false;
			}
			if (compareChildren)
			{
				int count = cg.Children.Count;
				if (this.Children.Count != count)
				{
					return false;
				}
				for (int i = 0; i < count; i++)
				{
					if (!((CodeGroup)this.Children[i]).Equals((CodeGroup)cg.Children[i], false))
					{
						return false;
					}
				}
			}
			return true;
		}

		public void RemoveChild(CodeGroup group)
		{
			if (group != null)
			{
				this.m_children.Remove(group);
			}
		}

		public override int GetHashCode()
		{
			int num = this.m_membershipCondition.GetHashCode();
			if (this.m_policy != null)
			{
				num += this.m_policy.GetHashCode();
			}
			return num;
		}

		public void FromXml(SecurityElement e)
		{
			this.FromXml(e, null);
		}

		public void FromXml(SecurityElement e, PolicyLevel level)
		{
			if (e == null)
			{
				throw new ArgumentNullException("e");
			}
			string text = e.Attribute("PermissionSetName");
			PermissionSet permissionSet;
			if (text != null && level != null)
			{
				permissionSet = level.GetNamedPermissionSet(text);
			}
			else
			{
				SecurityElement securityElement = e.SearchForChildByTag("PermissionSet");
				if (securityElement != null)
				{
					permissionSet = (PermissionSet)Activator.CreateInstance(Type.GetType(securityElement.Attribute("class")), true);
					permissionSet.FromXml(securityElement);
				}
				else
				{
					permissionSet = new PermissionSet(new PermissionSet(PermissionState.None));
				}
			}
			this.m_policy = new PolicyStatement(permissionSet);
			this.m_children.Clear();
			if (e.Children != null && e.Children.Count > 0)
			{
				foreach (object obj in e.Children)
				{
					SecurityElement securityElement2 = (SecurityElement)obj;
					if (securityElement2.Tag == "CodeGroup")
					{
						this.AddChild(CodeGroup.CreateFromXml(securityElement2, level));
					}
				}
			}
			this.m_membershipCondition = null;
			SecurityElement securityElement3 = e.SearchForChildByTag("IMembershipCondition");
			if (securityElement3 != null)
			{
				string text2 = securityElement3.Attribute("class");
				Type type = Type.GetType(text2);
				if (type == null)
				{
					type = Type.GetType("System.Security.Policy." + text2);
				}
				this.m_membershipCondition = (IMembershipCondition)Activator.CreateInstance(type, true);
				this.m_membershipCondition.FromXml(securityElement3, level);
			}
			this.m_name = e.Attribute("Name");
			this.m_description = e.Attribute("Description");
			this.ParseXml(e, level);
		}

		protected virtual void ParseXml(SecurityElement e, PolicyLevel level)
		{
		}

		public SecurityElement ToXml()
		{
			return this.ToXml(null);
		}

		public SecurityElement ToXml(PolicyLevel level)
		{
			SecurityElement securityElement = new SecurityElement("CodeGroup");
			securityElement.AddAttribute("class", base.GetType().AssemblyQualifiedName);
			securityElement.AddAttribute("version", "1");
			if (this.Name != null)
			{
				securityElement.AddAttribute("Name", this.Name);
			}
			if (this.Description != null)
			{
				securityElement.AddAttribute("Description", this.Description);
			}
			if (this.MembershipCondition != null)
			{
				securityElement.AddChild(this.MembershipCondition.ToXml());
			}
			if (this.PolicyStatement != null && this.PolicyStatement.PermissionSet != null)
			{
				securityElement.AddChild(this.PolicyStatement.PermissionSet.ToXml());
			}
			foreach (object obj in this.Children)
			{
				CodeGroup codeGroup = (CodeGroup)obj;
				securityElement.AddChild(codeGroup.ToXml());
			}
			this.CreateXml(securityElement, level);
			return securityElement;
		}

		protected virtual void CreateXml(SecurityElement element, PolicyLevel level)
		{
		}

		internal static CodeGroup CreateFromXml(SecurityElement se, PolicyLevel level)
		{
			string text = se.Attribute("class");
			string text2 = text;
			int num = text2.IndexOf(",");
			if (num > 0)
			{
				text2 = text2.Substring(0, num);
			}
			num = text2.LastIndexOf(".");
			if (num > 0)
			{
				text2 = text2.Substring(num + 1);
			}
			if (text2 == "FileCodeGroup")
			{
				return new FileCodeGroup(se, level);
			}
			if (text2 == "FirstMatchCodeGroup")
			{
				return new FirstMatchCodeGroup(se, level);
			}
			if (text2 == "NetCodeGroup")
			{
				return new NetCodeGroup(se, level);
			}
			if (!(text2 == "UnionCodeGroup"))
			{
				CodeGroup codeGroup = (CodeGroup)Activator.CreateInstance(Type.GetType(text), true);
				codeGroup.FromXml(se, level);
				return codeGroup;
			}
			return new UnionCodeGroup(se, level);
		}

		private PolicyStatement m_policy;

		private IMembershipCondition m_membershipCondition;

		private string m_description;

		private string m_name;

		private ArrayList m_children = new ArrayList();
	}
}
