using System;

namespace UnityEngine.Rendering
{
	public struct ScopedRenderPass : IDisposable
	{
		internal ScopedRenderPass(ScriptableRenderContext context)
		{
			this.m_Context = context;
		}

		public void Dispose()
		{
			try
			{
				this.m_Context.EndRenderPass();
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException("The ScopedRenderPass instance is not valid. This can happen if it was constructed using the default constructor.", ex);
			}
		}

		private ScriptableRenderContext m_Context;
	}
}
