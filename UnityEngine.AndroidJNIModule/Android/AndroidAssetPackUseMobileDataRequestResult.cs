using System;

namespace UnityEngine.Android
{
	public class AndroidAssetPackUseMobileDataRequestResult
	{
		internal AndroidAssetPackUseMobileDataRequestResult(bool allowed)
		{
			this.allowed = allowed;
		}

		public bool allowed { get; }
	}
}
