using System;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Runtime/Scripting/ApiRestrictions.h")]
	internal readonly struct EnableApiScope : IDisposable
	{
		public EnableApiScope(ApiRestrictions.ContextRestrictions api, Object context)
		{
			this.m_ContextApi = api;
			this.m_Context = context;
			ApiRestrictions.PopDisableApi(api, context);
			this.m_GlobalApi = ApiRestrictions.GlobalRestrictions.GLOBALCOUNT;
		}

		public EnableApiScope(ApiRestrictions.GlobalRestrictions api)
		{
			this.m_GlobalApi = api;
			this.m_Context = null;
			ApiRestrictions.PopDisableApi(api);
			this.m_ContextApi = ApiRestrictions.ContextRestrictions.CONTEXTCOUNT;
		}

		public void Dispose()
		{
			bool flag = this.m_Context != null;
			if (flag)
			{
				ApiRestrictions.PushDisableApi(this.m_ContextApi, this.m_Context);
			}
			else
			{
				ApiRestrictions.PushDisableApi(this.m_GlobalApi);
			}
		}

		private readonly ApiRestrictions.ContextRestrictions m_ContextApi;

		private readonly ApiRestrictions.GlobalRestrictions m_GlobalApi;

		private readonly Object m_Context;
	}
}
