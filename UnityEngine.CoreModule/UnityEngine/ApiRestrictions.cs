using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Runtime/Scripting/ApiRestrictions.h")]
	[UsedByNativeCode]
	[StaticAccessor("GetApiRestrictions()", StaticAccessorType.Arrow)]
	[ExtensionOfNativeClass]
	internal class ApiRestrictions
	{
		internal static void PushDisableApiInternal(ApiRestrictions.ContextRestrictions contextApi, Object context, ApiRestrictions.GlobalRestrictions globalApi)
		{
			ApiRestrictions.PushDisableApiInternal_Injected(contextApi, Object.MarshalledUnityObject.Marshal<Object>(context), globalApi);
		}

		internal static void PopDisableApiInternal(ApiRestrictions.ContextRestrictions contextApi, Object context, ApiRestrictions.GlobalRestrictions globalApi)
		{
			ApiRestrictions.PopDisableApiInternal_Injected(contextApi, Object.MarshalledUnityObject.Marshal<Object>(context), globalApi);
		}

		internal static bool TryApiInternal(ApiRestrictions.ContextRestrictions contextApi, Object context, ApiRestrictions.GlobalRestrictions globalApi, bool allowErrorLogging)
		{
			return ApiRestrictions.TryApiInternal_Injected(contextApi, Object.MarshalledUnityObject.Marshal<Object>(context), globalApi, allowErrorLogging);
		}

		internal static void PushDisableApi(ApiRestrictions.ContextRestrictions api, Object owner)
		{
			ApiRestrictions.PushDisableApiInternal(api, owner, ApiRestrictions.GlobalRestrictions.GLOBALCOUNT);
		}

		internal static void PushDisableApi(ApiRestrictions.GlobalRestrictions api)
		{
			ApiRestrictions.PushDisableApiInternal(ApiRestrictions.ContextRestrictions.CONTEXTCOUNT, null, api);
		}

		internal static void PopDisableApi(ApiRestrictions.ContextRestrictions api, Object context)
		{
			ApiRestrictions.PopDisableApiInternal(api, context, ApiRestrictions.GlobalRestrictions.GLOBALCOUNT);
		}

		internal static void PopDisableApi(ApiRestrictions.GlobalRestrictions api)
		{
			ApiRestrictions.PopDisableApiInternal(ApiRestrictions.ContextRestrictions.CONTEXTCOUNT, null, api);
		}

		internal static bool TryApi(ApiRestrictions.ContextRestrictions api, Object context, bool allowErrorLogging = true)
		{
			return ApiRestrictions.TryApiInternal(api, context, ApiRestrictions.GlobalRestrictions.GLOBALCOUNT, allowErrorLogging);
		}

		internal static bool TryApi(ApiRestrictions.GlobalRestrictions api, bool allowErrorLogging = true)
		{
			return ApiRestrictions.TryApiInternal(ApiRestrictions.ContextRestrictions.CONTEXTCOUNT, null, api, allowErrorLogging);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PushDisableApiInternal_Injected(ApiRestrictions.ContextRestrictions contextApi, IntPtr context, ApiRestrictions.GlobalRestrictions globalApi);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PopDisableApiInternal_Injected(ApiRestrictions.ContextRestrictions contextApi, IntPtr context, ApiRestrictions.GlobalRestrictions globalApi);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool TryApiInternal_Injected(ApiRestrictions.ContextRestrictions contextApi, IntPtr context, ApiRestrictions.GlobalRestrictions globalApi, bool allowErrorLogging);

		internal enum GlobalRestrictions
		{
			OBJECT_DESTROYIMMEDIATE,
			OBJECT_SENDMESSAGE,
			OBJECT_RENDERING,
			GLOBALCOUNT
		}

		internal enum ContextRestrictions
		{
			RENDERERSCENE_ADDREMOVE,
			OBJECT_ADDCOMPONENTTRANSFORM,
			CONTEXTCOUNT
		}
	}
}
