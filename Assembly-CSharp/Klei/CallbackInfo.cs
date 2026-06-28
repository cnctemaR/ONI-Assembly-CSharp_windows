using System;

namespace Klei
{
	public struct CallbackInfo
	{
		public CallbackInfo(HandleVector<Game.CallbackInfo>.Handle h)
		{
			this.handle = h;
		}

		public void Release()
		{
			if (this.handle.IsValid())
			{
				Game.CallbackInfo callbackInfo = Game.Instance.callbackManager.Get(this.handle);
				global::System.Action cb = callbackInfo.cb;
				if (!callbackInfo.manuallyRelease)
				{
					Game.Instance.callbackManager.Release(this.handle);
				}
				cb();
			}
		}

		private HandleVector<Game.CallbackInfo>.Handle handle;
	}
}
