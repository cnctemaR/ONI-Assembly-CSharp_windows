using System;

namespace UnityEngine
{
	public struct QueryParameters
	{
		public QueryParameters(int layerMask = -5, bool hitMultipleFaces = false, QueryTriggerInteraction hitTriggers = QueryTriggerInteraction.UseGlobal, bool hitBackfaces = false)
		{
			this.layerMask = layerMask;
			this.hitMultipleFaces = hitMultipleFaces;
			this.hitTriggers = hitTriggers;
			this.hitBackfaces = hitBackfaces;
		}

		public static QueryParameters Default
		{
			get
			{
				return new QueryParameters(-5, false, QueryTriggerInteraction.UseGlobal, false);
			}
		}

		public int layerMask;

		public bool hitMultipleFaces;

		public QueryTriggerInteraction hitTriggers;

		public bool hitBackfaces;
	}
}
