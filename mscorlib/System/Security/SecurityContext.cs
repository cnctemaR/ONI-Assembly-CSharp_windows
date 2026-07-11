using System;
using System.Security.Permissions;
using System.Security.Principal;
using System.Threading;

namespace System.Security
{
	public sealed class SecurityContext
	{
		internal SecurityContext()
		{
		}

		internal SecurityContext(SecurityContext sc)
		{
			this._capture = true;
			this._winid = sc._winid;
			if (sc._stack != null)
			{
				this._stack = sc._stack.CreateCopy();
			}
		}

		public SecurityContext CreateCopy()
		{
			if (!this._capture)
			{
				throw new InvalidOperationException();
			}
			return new SecurityContext(this);
		}

		public static SecurityContext Capture()
		{
			SecurityContext securityContext = Thread.CurrentThread.ExecutionContext.SecurityContext;
			if (securityContext.FlowSuppressed)
			{
				return null;
			}
			return new SecurityContext
			{
				_capture = true,
				_winid = WindowsIdentity.GetCurrentToken(),
				_stack = CompressedStack.Capture()
			};
		}

		internal bool FlowSuppressed
		{
			get
			{
				return this._suppressFlow;
			}
			set
			{
				this._suppressFlow = value;
			}
		}

		internal bool WindowsIdentityFlowSuppressed
		{
			get
			{
				return this._suppressFlowWindowsIdentity;
			}
			set
			{
				this._suppressFlowWindowsIdentity = value;
			}
		}

		internal CompressedStack CompressedStack
		{
			get
			{
				return this._stack;
			}
			set
			{
				this._stack = value;
			}
		}

		internal IntPtr IdentityToken
		{
			get
			{
				return this._winid;
			}
			set
			{
				this._winid = value;
			}
		}

		public static bool IsFlowSuppressed()
		{
			return Thread.CurrentThread.ExecutionContext.SecurityContext.FlowSuppressed;
		}

		public static bool IsWindowsIdentityFlowSuppressed()
		{
			return Thread.CurrentThread.ExecutionContext.SecurityContext.WindowsIdentityFlowSuppressed;
		}

		public static void RestoreFlow()
		{
			SecurityContext securityContext = Thread.CurrentThread.ExecutionContext.SecurityContext;
			if (!securityContext.FlowSuppressed && !securityContext.WindowsIdentityFlowSuppressed)
			{
				throw new InvalidOperationException();
			}
			securityContext.FlowSuppressed = false;
			securityContext.WindowsIdentityFlowSuppressed = false;
		}

		[PermissionSet(SecurityAction.Assert, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"ControlPrincipal\"/>\n</PermissionSet>\n")]
		[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"Infrastructure\"/>\n</PermissionSet>\n")]
		public static void Run(SecurityContext securityContext, ContextCallback callback, object state)
		{
			if (securityContext == null)
			{
				throw new InvalidOperationException(Locale.GetText("Null SecurityContext"));
			}
			SecurityContext securityContext2 = Thread.CurrentThread.ExecutionContext.SecurityContext;
			IPrincipal currentPrincipal = Thread.CurrentPrincipal;
			try
			{
				if (securityContext2.IdentityToken != IntPtr.Zero)
				{
					Thread.CurrentPrincipal = new WindowsPrincipal(new WindowsIdentity(securityContext2.IdentityToken));
				}
				if (securityContext.CompressedStack != null)
				{
					CompressedStack.Run(securityContext.CompressedStack, callback, state);
				}
				else
				{
					callback(state);
				}
			}
			finally
			{
				if (currentPrincipal != null && securityContext2.IdentityToken != IntPtr.Zero)
				{
					Thread.CurrentPrincipal = currentPrincipal;
				}
			}
		}

		[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"Infrastructure\"/>\n</PermissionSet>\n")]
		public static AsyncFlowControl SuppressFlow()
		{
			Thread currentThread = Thread.CurrentThread;
			currentThread.ExecutionContext.SecurityContext.FlowSuppressed = true;
			currentThread.ExecutionContext.SecurityContext.WindowsIdentityFlowSuppressed = true;
			return new AsyncFlowControl(currentThread, AsyncFlowControlType.Security);
		}

		public static AsyncFlowControl SuppressFlowWindowsIdentity()
		{
			Thread currentThread = Thread.CurrentThread;
			currentThread.ExecutionContext.SecurityContext.WindowsIdentityFlowSuppressed = true;
			return new AsyncFlowControl(currentThread, AsyncFlowControlType.Security);
		}

		private bool _capture;

		private IntPtr _winid;

		private CompressedStack _stack;

		private bool _suppressFlowWindowsIdentity;

		private bool _suppressFlow;
	}
}
