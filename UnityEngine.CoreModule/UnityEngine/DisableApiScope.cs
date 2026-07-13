using System;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Runtime/Scripting/ApiRestrictions.h")]
	internal readonly struct DisableApiScope : IDisposable
	{
		public DisableApiScope(ApiRestrictions.ContextRestrictions api, Object context)
		{
			this.m_ContextApi = api;
			this.m_Context = context;
			ApiRestrictions.PushDisableApi(api, context);
			this.m_GlobalApi = ApiRestrictions.GlobalRestrictions.GLOBALCOUNT;
		}

		public DisableApiScope(ApiRestrictions.GlobalRestrictions api)
		{
			this.m_GlobalApi = api;
			this.m_Context = null;
			ApiRestrictions.PushDisableApi(api);
			this.m_ContextApi = ApiRestrictions.ContextRestrictions.CONTEXTCOUNT;
		}

		public void Dispose()
		{
			bool flag = this.m_Context != null;
			if (flag)
			{
				ApiRestrictions.PopDisableApi(this.m_ContextApi, this.m_Context);
			}
			else
			{
				ApiRestrictions.PopDisableApi(this.m_GlobalApi);
			}
		}

		private readonly ApiRestrictions.ContextRestrictions m_ContextApi;

		private readonly ApiRestrictions.GlobalRestrictions m_GlobalApi;

		private readonly Object m_Context;
	}
}
