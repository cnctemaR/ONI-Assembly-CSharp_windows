using System;
using System.Diagnostics;

namespace UnityEngine.Android
{
	public class PermissionCallbacks : AndroidJavaProxy
	{
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<string> PermissionGranted;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<string> PermissionDenied;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<string> PermissionDeniedAndDontAskAgain;

		public PermissionCallbacks()
			: base("com.unity3d.player.IPermissionRequestCallbacks")
		{
		}

		private void onPermissionGranted(string permissionName)
		{
			Action<string> permissionGranted = this.PermissionGranted;
			if (permissionGranted != null)
			{
				permissionGranted(permissionName);
			}
		}

		private void onPermissionDenied(string permissionName)
		{
			Action<string> permissionDenied = this.PermissionDenied;
			if (permissionDenied != null)
			{
				permissionDenied(permissionName);
			}
		}

		private void onPermissionDeniedAndDontAskAgain(string permissionName)
		{
			bool flag = this.PermissionDeniedAndDontAskAgain != null;
			if (flag)
			{
				this.PermissionDeniedAndDontAskAgain(permissionName);
			}
			else
			{
				Action<string> permissionDenied = this.PermissionDenied;
				if (permissionDenied != null)
				{
					permissionDenied(permissionName);
				}
			}
		}
	}
}
