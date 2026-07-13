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

		[Obsolete("Unreliable. Query ShouldShowRequestPermissionRationale and use PermissionDenied event.", false)]
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<string> PermissionDeniedAndDontAskAgain;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<string> PermissionRequestDismissed;

		public PermissionCallbacks()
			: base("com.unity3d.player.IPermissionRequestCallbacks")
		{
		}

		public override IntPtr Invoke(string methodName, IntPtr javaArgs)
		{
			IntPtr intPtr;
			if (!(methodName == "onPermissionResult"))
			{
				intPtr = base.Invoke(methodName, javaArgs);
			}
			else
			{
				this.onPermissionResult(javaArgs);
				intPtr = IntPtr.Zero;
			}
			return intPtr;
		}

		private void onPermissionResult(IntPtr javaArgs)
		{
			IntPtr objectArrayElement = AndroidJNISafe.GetObjectArrayElement(javaArgs, 0);
			int[] array = AndroidJNISafe.FromIntArray(AndroidJNISafe.GetObjectArrayElement(javaArgs, 1));
			int i = 0;
			while (i < array.Length)
			{
				string stringChars = AndroidJNISafe.GetStringChars(AndroidJNISafe.GetObjectArrayElement(objectArrayElement, i));
				switch (array[i])
				{
				case 0:
				{
					bool flag = this.PermissionRequestDismissed == null;
					if (flag)
					{
						goto IL_00A2;
					}
					this.PermissionRequestDismissed(stringChars);
					break;
				}
				case 1:
				{
					Action<string> permissionGranted = this.PermissionGranted;
					if (permissionGranted != null)
					{
						permissionGranted(stringChars);
					}
					break;
				}
				case 2:
					goto IL_00A2;
				case 3:
				{
					bool flag2 = this.PermissionDeniedAndDontAskAgain == null;
					if (flag2)
					{
						goto IL_00A2;
					}
					this.PermissionDeniedAndDontAskAgain(stringChars);
					break;
				}
				}
				IL_00B7:
				i++;
				continue;
				IL_00A2:
				Action<string> permissionDenied = this.PermissionDenied;
				if (permissionDenied != null)
				{
					permissionDenied(stringChars);
				}
				goto IL_00B7;
			}
		}

		private enum Result
		{
			Dismissed,
			Granted,
			Denied,
			DeniedDontAskAgain
		}
	}
}
