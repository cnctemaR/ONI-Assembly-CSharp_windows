using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Multiplayer.Internal
{
	[StaticAccessor("GetMultiplayerManager()", StaticAccessorType.Dot)]
	[NativeHeader("Modules/Multiplayer/MultiplayerManager.h")]
	internal static class MultiplayerManager
	{
		public static extern MultiplayerRoleFlags activeMultiplayerRoleMask
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static MultiplayerRoleFlags GetMultiplayerRoleMaskForGameObject(GameObject gameObject)
		{
			return MultiplayerManager.GetMultiplayerRoleMaskForGameObject_Injected(Object.MarshalledUnityObject.Marshal<GameObject>(gameObject));
		}

		public static MultiplayerRoleFlags GetMultiplayerRoleMaskForComponent(Component component)
		{
			return MultiplayerManager.GetMultiplayerRoleMaskForComponent_Injected(Object.MarshalledUnityObject.Marshal<Component>(component));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern MultiplayerRoleFlags GetMultiplayerRoleMaskForGameObject_Injected(IntPtr gameObject);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern MultiplayerRoleFlags GetMultiplayerRoleMaskForComponent_Injected(IntPtr component);
	}
}
