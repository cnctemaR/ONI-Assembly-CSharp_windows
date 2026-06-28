using System;
using Klei;
using KSerialization;
using UnityEngine;

namespace Generated
{
	[SerializationConfig(MemberSerialization.OptOut)]
	public struct Neighbors
	{
		public Neighbors(TerrainCell a, TerrainCell b)
		{
			Debug.Assert(a != null && b != null, "NULL Neighbor");
			this.n0 = a;
			this.n1 = b;
		}

		public TerrainCell n0;

		public TerrainCell n1;
	}
}
