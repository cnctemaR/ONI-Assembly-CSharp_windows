using System;

namespace UnityEngine.Experimental.GlobalIllumination
{
	public struct Cookie
	{
		public static Cookie Defaults()
		{
			Cookie cookie;
			cookie.instanceID = 0;
			cookie.scale = 1f;
			cookie.sizes = new Vector2(1f, 1f);
			return cookie;
		}

		public int instanceID;

		public float scale;

		public Vector2 sizes;
	}
}
