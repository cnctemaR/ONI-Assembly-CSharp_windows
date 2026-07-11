using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Android
{
	[UsedByNativeCode]
	[NativeHeader("Runtime/Export/Android/AndroidPermissions.bindings.h")]
	public struct Permission
	{
		[StaticAccessor("PermissionsBindings", StaticAccessorType.DoubleColon)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool HasUserAuthorizedPermission(string permission);

		[StaticAccessor("PermissionsBindings", StaticAccessorType.DoubleColon)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void RequestUserPermission(string permission);

		public const string Camera = "android.permission.CAMERA";

		public const string Microphone = "android.permission.RECORD_AUDIO";

		public const string FineLocation = "android.permission.ACCESS_FINE_LOCATION";

		public const string CoarseLocation = "android.permission.ACCESS_COARSE_LOCATION";

		public const string ExternalStorageRead = "android.permission.READ_EXTERNAL_STORAGE";

		public const string ExternalStorageWrite = "android.permission.WRITE_EXTERNAL_STORAGE";
	}
}
