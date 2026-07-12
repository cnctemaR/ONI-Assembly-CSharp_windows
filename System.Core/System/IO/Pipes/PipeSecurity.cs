using System;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Win32.SafeHandles;

namespace System.IO.Pipes
{
	public class PipeSecurity : NativeObjectSecurity
	{
		public PipeSecurity()
			: base(false, ResourceType.KernelObject)
		{
		}

		internal PipeSecurity(SafePipeHandle safeHandle, AccessControlSections includeSections)
			: base(false, ResourceType.KernelObject, safeHandle, includeSections)
		{
		}

		public void AddAccessRule(PipeAccessRule rule)
		{
			if (rule == null)
			{
				throw new ArgumentNullException("rule");
			}
			base.AddAccessRule(rule);
		}

		public void SetAccessRule(PipeAccessRule rule)
		{
			if (rule == null)
			{
				throw new ArgumentNullException("rule");
			}
			base.SetAccessRule(rule);
		}

		public void ResetAccessRule(PipeAccessRule rule)
		{
			if (rule == null)
			{
				throw new ArgumentNullException("rule");
			}
			base.ResetAccessRule(rule);
		}

		public bool RemoveAccessRule(PipeAccessRule rule)
		{
			if (rule == null)
			{
				throw new ArgumentNullException("rule");
			}
			AuthorizationRuleCollection accessRules = base.GetAccessRules(true, true, rule.IdentityReference.GetType());
			for (int i = 0; i < accessRules.Count; i++)
			{
				PipeAccessRule pipeAccessRule = accessRules[i] as PipeAccessRule;
				if (pipeAccessRule != null && pipeAccessRule.PipeAccessRights == rule.PipeAccessRights && pipeAccessRule.IdentityReference == rule.IdentityReference && pipeAccessRule.AccessControlType == rule.AccessControlType)
				{
					return base.RemoveAccessRule(rule);
				}
			}
			if (rule.PipeAccessRights != PipeAccessRights.FullControl)
			{
				return base.RemoveAccessRule(new PipeAccessRule(rule.IdentityReference, PipeAccessRule.AccessMaskFromRights(rule.PipeAccessRights, AccessControlType.Deny), false, rule.AccessControlType));
			}
			return base.RemoveAccessRule(rule);
		}

		public void RemoveAccessRuleSpecific(PipeAccessRule rule)
		{
			if (rule == null)
			{
				throw new ArgumentNullException("rule");
			}
			AuthorizationRuleCollection accessRules = base.GetAccessRules(true, true, rule.IdentityReference.GetType());
			for (int i = 0; i < accessRules.Count; i++)
			{
				PipeAccessRule pipeAccessRule = accessRules[i] as PipeAccessRule;
				if (pipeAccessRule != null && pipeAccessRule.PipeAccessRights == rule.PipeAccessRights && pipeAccessRule.IdentityReference == rule.IdentityReference && pipeAccessRule.AccessControlType == rule.AccessControlType)
				{
					base.RemoveAccessRuleSpecific(rule);
					return;
				}
			}
			if (rule.PipeAccessRights != PipeAccessRights.FullControl)
			{
				base.RemoveAccessRuleSpecific(new PipeAccessRule(rule.IdentityReference, PipeAccessRule.AccessMaskFromRights(rule.PipeAccessRights, AccessControlType.Deny), false, rule.AccessControlType));
				return;
			}
			base.RemoveAccessRuleSpecific(rule);
		}

		public void AddAuditRule(PipeAuditRule rule)
		{
			base.AddAuditRule(rule);
		}

		public void SetAuditRule(PipeAuditRule rule)
		{
			base.SetAuditRule(rule);
		}

		public bool RemoveAuditRule(PipeAuditRule rule)
		{
			return base.RemoveAuditRule(rule);
		}

		public void RemoveAuditRuleAll(PipeAuditRule rule)
		{
			base.RemoveAuditRuleAll(rule);
		}

		public void RemoveAuditRuleSpecific(PipeAuditRule rule)
		{
			base.RemoveAuditRuleSpecific(rule);
		}

		public override AccessRule AccessRuleFactory(IdentityReference identityReference, int accessMask, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AccessControlType type)
		{
			if (inheritanceFlags != InheritanceFlags.None)
			{
				throw new ArgumentException("This flag may not be set on a pipe.", "inheritanceFlags");
			}
			if (propagationFlags != PropagationFlags.None)
			{
				throw new ArgumentException("This flag may not be set on a pipe.", "propagationFlags");
			}
			return new PipeAccessRule(identityReference, accessMask, isInherited, type);
		}

		public sealed override AuditRule AuditRuleFactory(IdentityReference identityReference, int accessMask, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AuditFlags flags)
		{
			if (inheritanceFlags != InheritanceFlags.None)
			{
				throw new ArgumentException("This flag may not be set on a pipe.", "inheritanceFlags");
			}
			if (propagationFlags != PropagationFlags.None)
			{
				throw new ArgumentException("This flag may not be set on a pipe.", "propagationFlags");
			}
			return new PipeAuditRule(identityReference, accessMask, isInherited, flags);
		}

		private AccessControlSections GetAccessControlSectionsFromChanges()
		{
			AccessControlSections accessControlSections = AccessControlSections.None;
			if (base.AccessRulesModified)
			{
				accessControlSections = AccessControlSections.Access;
			}
			if (base.AuditRulesModified)
			{
				accessControlSections |= AccessControlSections.Audit;
			}
			if (base.OwnerModified)
			{
				accessControlSections |= AccessControlSections.Owner;
			}
			if (base.GroupModified)
			{
				accessControlSections |= AccessControlSections.Group;
			}
			return accessControlSections;
		}

		protected internal void Persist(SafeHandle handle)
		{
			base.WriteLock();
			try
			{
				AccessControlSections accessControlSectionsFromChanges = this.GetAccessControlSectionsFromChanges();
				base.Persist(handle, accessControlSectionsFromChanges);
				base.OwnerModified = (base.GroupModified = (base.AuditRulesModified = (base.AccessRulesModified = false)));
			}
			finally
			{
				base.WriteUnlock();
			}
		}

		protected internal void Persist(string name)
		{
			base.WriteLock();
			try
			{
				AccessControlSections accessControlSectionsFromChanges = this.GetAccessControlSectionsFromChanges();
				base.Persist(name, accessControlSectionsFromChanges);
				base.OwnerModified = (base.GroupModified = (base.AuditRulesModified = (base.AccessRulesModified = false)));
			}
			finally
			{
				base.WriteUnlock();
			}
		}

		public override Type AccessRightType
		{
			get
			{
				return typeof(PipeAccessRights);
			}
		}

		public override Type AccessRuleType
		{
			get
			{
				return typeof(PipeAccessRule);
			}
		}

		public override Type AuditRuleType
		{
			get
			{
				return typeof(PipeAuditRule);
			}
		}
	}
}
