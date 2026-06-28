using System;

namespace Klei
{
	public struct CallbackInfo
	{
		public CallbackInfo(HandleVector<global::System.Action>.Handle onComplete)
		{
			this.onComplete = onComplete;
		}

		public HandleVector<global::System.Action>.Handle onComplete;
	}
}
