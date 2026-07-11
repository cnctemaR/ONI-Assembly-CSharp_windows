using System;
using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	[ComVisible(true)]
	[Serializable]
	public sealed class SecurityPermissionAttribute : CodeAccessSecurityAttribute
	{
		public SecurityPermissionAttribute(SecurityAction action)
			: base(action)
		{
			this.m_Flags = SecurityPermissionFlag.NoFlags;
		}

		public bool Assertion
		{
			get
			{
				return (this.m_Flags & SecurityPermissionFlag.Assertion) != SecurityPermissionFlag.NoFlags;
			}
			set
			{
				if (value)
				{
					this.m_Flags |= SecurityPermissionFlag.Assertion;
				}
				else
				{
					this.m_Flags &= ~SecurityPermissionFlag.Assertion;
				}
			}
		}

		public bool BindingRedirects
		{
			get
			{
				return (this.m_Flags & SecurityPermissionFlag.BindingRedirects) != SecurityPermissionFlag.NoFlags;
			}
			set
			{
				if (value)
				{
					this.m_Flags |= SecurityPermissionFlag.BindingRedirects;
				}
				else
				{
					this.m_Flags &= ~SecurityPermissionFlag.BindingRedirects;
				}
			}
		}

		public bool ControlAppDomain
		{
			get
			{
				return (this.m_Flags & SecurityPermissionFlag.ControlAppDomain) != SecurityPermissionFlag.NoFlags;
			}
			set
			{
				if (value)
				{
					this.m_Flags |= SecurityPermissionFlag.ControlAppDomain;
				}
				else
				{
					this.m_Flags &= ~SecurityPermissionFlag.ControlAppDomain;
				}
			}
		}

		public bool ControlDomainPolicy
		{
			get
			{
				return (this.m_Flags & SecurityPermissionFlag.ControlDomainPolicy) != SecurityPermissionFlag.NoFlags;
			}
			set
			{
				if (value)
				{
					this.m_Flags |= SecurityPermissionFlag.ControlDomainPolicy;
				}
				else
				{
					this.m_Flags &= ~SecurityPermissionFlag.ControlDomainPolicy;
				}
			}
		}

		public bool ControlEvidence
		{
			get
			{
				return (this.m_Flags & SecurityPermissionFlag.ControlEvidence) != SecurityPermissionFlag.NoFlags;
			}
			set
			{
				if (value)
				{
					this.m_Flags |= SecurityPermissionFlag.ControlEvidence;
				}
				else
				{
					this.m_Flags &= ~SecurityPermissionFlag.ControlEvidence;
				}
			}
		}

		public bool ControlPolicy
		{
			get
			{
				return (this.m_Flags & SecurityPermissionFlag.ControlPolicy) != SecurityPermissionFlag.NoFlags;
			}
			set
			{
				if (value)
				{
					this.m_Flags |= SecurityPermissionFlag.ControlPolicy;
				}
				else
				{
					this.m_Flags &= ~SecurityPermissionFlag.ControlPolicy;
				}
			}
		}

		public bool ControlPrincipal
		{
			get
			{
				return (this.m_Flags & SecurityPermissionFlag.ControlPrincipal) != SecurityPermissionFlag.NoFlags;
			}
			set
			{
				if (value)
				{
					this.m_Flags |= SecurityPermissionFlag.ControlPrincipal;
				}
				else
				{
					this.m_Flags &= ~SecurityPermissionFlag.ControlPrincipal;
				}
			}
		}

		public bool ControlThread
		{
			get
			{
				return (this.m_Flags & SecurityPermissionFlag.ControlThread) != SecurityPermissionFlag.NoFlags;
			}
			set
			{
				if (value)
				{
					this.m_Flags |= SecurityPermissionFlag.ControlThread;
				}
				else
				{
					this.m_Flags &= ~SecurityPermissionFlag.ControlThread;
				}
			}
		}

		public bool Execution
		{
			get
			{
				return (this.m_Flags & SecurityPermissionFlag.Execution) != SecurityPermissionFlag.NoFlags;
			}
			set
			{
				if (value)
				{
					this.m_Flags |= SecurityPermissionFlag.Execution;
				}
				else
				{
					this.m_Flags &= ~SecurityPermissionFlag.Execution;
				}
			}
		}

		[ComVisible(true)]
		public bool Infrastructure
		{
			get
			{
				return (this.m_Flags & SecurityPermissionFlag.Infrastructure) != SecurityPermissionFlag.NoFlags;
			}
			set
			{
				if (value)
				{
					this.m_Flags |= SecurityPermissionFlag.Infrastructure;
				}
				else
				{
					this.m_Flags &= ~SecurityPermissionFlag.Infrastructure;
				}
			}
		}

		public bool RemotingConfiguration
		{
			get
			{
				return (this.m_Flags & SecurityPermissionFlag.RemotingConfiguration) != SecurityPermissionFlag.NoFlags;
			}
			set
			{
				if (value)
				{
					this.m_Flags |= SecurityPermissionFlag.RemotingConfiguration;
				}
				else
				{
					this.m_Flags &= ~SecurityPermissionFlag.RemotingConfiguration;
				}
			}
		}

		public bool SerializationFormatter
		{
			get
			{
				return (this.m_Flags & SecurityPermissionFlag.SerializationFormatter) != SecurityPermissionFlag.NoFlags;
			}
			set
			{
				if (value)
				{
					this.m_Flags |= SecurityPermissionFlag.SerializationFormatter;
				}
				else
				{
					this.m_Flags &= ~SecurityPermissionFlag.SerializationFormatter;
				}
			}
		}

		public bool SkipVerification
		{
			get
			{
				return (this.m_Flags & SecurityPermissionFlag.SkipVerification) != SecurityPermissionFlag.NoFlags;
			}
			set
			{
				if (value)
				{
					this.m_Flags |= SecurityPermissionFlag.SkipVerification;
				}
				else
				{
					this.m_Flags &= ~SecurityPermissionFlag.SkipVerification;
				}
			}
		}

		public bool UnmanagedCode
		{
			get
			{
				return (this.m_Flags & SecurityPermissionFlag.UnmanagedCode) != SecurityPermissionFlag.NoFlags;
			}
			set
			{
				if (value)
				{
					this.m_Flags |= SecurityPermissionFlag.UnmanagedCode;
				}
				else
				{
					this.m_Flags &= ~SecurityPermissionFlag.UnmanagedCode;
				}
			}
		}

		public override IPermission CreatePermission()
		{
			SecurityPermission securityPermission;
			if (base.Unrestricted)
			{
				securityPermission = new SecurityPermission(PermissionState.Unrestricted);
			}
			else
			{
				securityPermission = new SecurityPermission(this.m_Flags);
			}
			return securityPermission;
		}

		public SecurityPermissionFlag Flags
		{
			get
			{
				return this.m_Flags;
			}
			set
			{
				this.m_Flags = value;
			}
		}

		private SecurityPermissionFlag m_Flags;
	}
}
