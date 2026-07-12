using System;

namespace UnityEngine.Android
{
	public class AndroidAssetPackState
	{
		internal AndroidAssetPackState(string name, AndroidAssetPackStatus status, AndroidAssetPackError error)
		{
			this.name = name;
			this.status = status;
			this.error = error;
		}

		public string name { get; }

		public AndroidAssetPackStatus status { get; }

		public AndroidAssetPackError error { get; }
	}
}
