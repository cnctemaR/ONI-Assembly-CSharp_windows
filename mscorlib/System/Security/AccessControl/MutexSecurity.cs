using System;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;

namespace System.Security.AccessControl
{
	public sealed class MutexSecurity : NativeObjectSecurity
	{
		public MutexSecurity()
			: base(false, ResourceType.KernelObject)
		{
		}

		public MutexSecurity(string name, AccessControlSections includeSections)
			: base(false, ResourceType.KernelObject, name, includeSections, new NativeObjectSecurity.ExceptionFromErrorCode(MutexSecurity.MutexExceptionFromErrorCode), null)
		{
		}

		internal MutexSecurity(SafeHandle handle, AccessControlSections includeSections)
			: base(false, ResourceType.KernelObject, handle, includeSections, new NativeObjectSecurity.ExceptionFromErrorCode(MutexSecurity.MutexExceptionFromErrorCode), null)
		{
		}

		public override Type AccessRightType
		{
			get
			{
				return typeof(MutexRights);
			}
		}

		public override Type AccessRuleType
		{
			get
			{
				return typeof(MutexAccessRule);
			}
		}

		public override Type AuditRuleType
		{
			get
			{
				return typeof(MutexAuditRule);
			}
		}

		public override AccessRule AccessRuleFactory(IdentityReference identityReference, int accessMask, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AccessControlType type)
		{
			return new MutexAccessRule(identityReference, (MutexRights)accessMask, type);
		}

		public void AddAccessRule(MutexAccessRule rule)
		{
			base.AddAccessRule(rule);
		}

		public bool RemoveAccessRule(MutexAccessRule rule)
		{
			return base.RemoveAccessRule(rule);
		}

		public void RemoveAccessRuleAll(MutexAccessRule rule)
		{
			base.RemoveAccessRuleAll(rule);
		}

		public void RemoveAccessRuleSpecific(MutexAccessRule rule)
		{
			base.RemoveAccessRuleSpecific(rule);
		}

		public void ResetAccessRule(MutexAccessRule rule)
		{
			base.ResetAccessRule(rule);
		}

		public void SetAccessRule(MutexAccessRule rule)
		{
			base.SetAccessRule(rule);
		}

		public override AuditRule AuditRuleFactory(IdentityReference identityReference, int accessMask, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AuditFlags flags)
		{
			return new MutexAuditRule(identityReference, (MutexRights)accessMask, flags);
		}

		public void AddAuditRule(MutexAuditRule rule)
		{
			base.AddAuditRule(rule);
		}

		public bool RemoveAuditRule(MutexAuditRule rule)
		{
			return base.RemoveAuditRule(rule);
		}

		public void RemoveAuditRuleAll(MutexAuditRule rule)
		{
			base.RemoveAuditRuleAll(rule);
		}

		public void RemoveAuditRuleSpecific(MutexAuditRule rule)
		{
			base.RemoveAuditRuleSpecific(rule);
		}

		public void SetAuditRule(MutexAuditRule rule)
		{
			base.SetAuditRule(rule);
		}

		private static Exception MutexExceptionFromErrorCode(int errorCode, string name, SafeHandle handle, object context)
		{
			if (errorCode == 2)
			{
				return new WaitHandleCannotBeOpenedException();
			}
			return NativeObjectSecurity.DefaultExceptionFromErrorCode(errorCode, name, handle, context);
		}
	}
}
