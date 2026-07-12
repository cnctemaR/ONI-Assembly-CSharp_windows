using System;
using System.Diagnostics;

namespace ProcGen
{
	[DebuggerDisplay("{world} ({x}, {y})")]
	public class WorldPlacement
	{
		public string world { get; set; }

		public MinMaxI allowedRings { get; set; }

		public int buffer { get; set; }

		public WorldPlacement.LocationType locationType { get; set; }

		public int x { get; private set; }

		public int y { get; private set; }

		public int width { get; private set; }

		public int height { get; private set; }

		public bool startWorld { get; set; }

		public WorldPlacement()
		{
			this.allowedRings = new MinMaxI(0, 9999);
			this.buffer = 2;
			this.locationType = WorldPlacement.LocationType.Cluster;
		}

		public void SetPosition(Vector2I pos)
		{
			this.x = pos.X;
			this.y = pos.Y;
		}

		public void SetSize(Vector2I size)
		{
			this.width = size.X;
			this.height = size.Y;
		}

		public enum LocationType
		{
			Cluster,
			Startworld,
			InnerCluster
		}
	}
}
