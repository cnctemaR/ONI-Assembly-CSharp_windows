using System;
using KSerialization;

namespace ProcGenGame
{
	[SerializationConfig(MemberSerialization.OptOut)]
	public struct Neighbors
	{
		public Neighbors(TerrainCell a, TerrainCell b)
		{
			this.n0 = a;
			this.n1 = b;
		}

		public TerrainCell n0;

		public TerrainCell n1;
	}
}
