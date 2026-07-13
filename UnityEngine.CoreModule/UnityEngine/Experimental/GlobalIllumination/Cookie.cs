using System;

namespace UnityEngine.Experimental.GlobalIllumination
{
	public struct Cookie
	{
		[Obsolete("Please use entityId instead.", false)]
		public int instanceID
		{
			get
			{
				return this.entityId;
			}
			set
			{
				this.entityId = value;
			}
		}

		public static Cookie Defaults()
		{
			Cookie cookie;
			cookie.entityId = EntityId.None;
			cookie.scale = 1f;
			cookie.sizes = new Vector2(1f, 1f);
			return cookie;
		}

		public EntityId entityId;

		public float scale;

		public Vector2 sizes;
	}
}
