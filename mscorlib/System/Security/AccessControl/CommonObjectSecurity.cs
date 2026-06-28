using System;
using System.Collections.Generic;

namespace System.Security.AccessControl
{
	[MonoTODO("required for NativeObjectSecurity - implementation is missing")]
	public abstract class CommonObjectSecurity : ObjectSecurity
	{
		protected CommonObjectSecurity(bool isContainer)
			: base(isContainer, false)
		{
		}

		public AuthorizationRuleCollection GetAccessRules(bool includeExplicit, bool includeInherited, Type targetType)
		{
			throw new NotImplementedException();
		}

		public AuthorizationRuleCollection GetAuditRules(bool includeExplicit, bool includeInherited, Type targetType)
		{
			throw new NotImplementedException();
		}

		protected void AddAccessRule(AccessRule rule)
		{
			this.access_rules.Add(rule);
			base.AccessRulesModified = true;
		}

		protected bool RemoveAccessRule(AccessRule rule)
		{
			throw new NotImplementedException();
		}

		protected void RemoveAccessRuleAll(AccessRule rule)
		{
			throw new NotImplementedException();
		}

		protected void RemoveAccessRuleSpecific(AccessRule rule)
		{
			throw new NotImplementedException();
		}

		protected void ResetAccessRule(AccessRule rule)
		{
			throw new NotImplementedException();
		}

		protected void SetAccessRule(AccessRule rule)
		{
			throw new NotImplementedException();
		}

		protected override bool ModifyAccess(AccessControlModification modification, AccessRule rule, out bool modified)
		{
			foreach (AccessRule accessRule in this.access_rules)
			{
				if (rule == accessRule)
				{
					switch (modification)
					{
					case AccessControlModification.Add:
						this.AddAccessRule(rule);
						break;
					case AccessControlModification.Set:
						this.SetAccessRule(rule);
						break;
					case AccessControlModification.Reset:
						this.ResetAccessRule(rule);
						break;
					case AccessControlModification.Remove:
						this.RemoveAccessRule(rule);
						break;
					case AccessControlModification.RemoveAll:
						this.RemoveAccessRuleAll(rule);
						break;
					case AccessControlModification.RemoveSpecific:
						this.RemoveAccessRuleSpecific(rule);
						break;
					}
					modified = true;
					return true;
				}
			}
			modified = false;
			return false;
		}

		protected void AddAuditRule(AuditRule rule)
		{
			this.audit_rules.Add(rule);
			base.AuditRulesModified = true;
		}

		protected bool RemoveAuditRule(AuditRule rule)
		{
			throw new NotImplementedException();
		}

		protected void RemoveAuditRuleAll(AuditRule rule)
		{
			throw new NotImplementedException();
		}

		protected void RemoveAuditRuleSpecific(AuditRule rule)
		{
			throw new NotImplementedException();
		}

		protected void SetAuditRule(AuditRule rule)
		{
			throw new NotImplementedException();
		}

		protected override bool ModifyAudit(AccessControlModification modification, AuditRule rule, out bool modified)
		{
			foreach (AuditRule auditRule in this.audit_rules)
			{
				if (rule == auditRule)
				{
					switch (modification)
					{
					case AccessControlModification.Add:
						this.AddAuditRule(rule);
						break;
					case AccessControlModification.Set:
						this.SetAuditRule(rule);
						break;
					case AccessControlModification.Remove:
						this.RemoveAuditRule(rule);
						break;
					case AccessControlModification.RemoveAll:
						this.RemoveAuditRuleAll(rule);
						break;
					case AccessControlModification.RemoveSpecific:
						this.RemoveAuditRuleSpecific(rule);
						break;
					}
					base.AuditRulesModified = true;
					modified = true;
					return true;
				}
			}
			modified = false;
			return false;
		}

		private List<AccessRule> access_rules = new List<AccessRule>();

		private List<AuditRule> audit_rules = new List<AuditRule>();
	}
}
