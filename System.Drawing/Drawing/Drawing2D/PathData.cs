using System;

namespace System.Drawing.Drawing2D
{
	public sealed class PathData
	{
		public PointF[] Points
		{
			get
			{
				return this.points;
			}
			set
			{
				this.points = value;
			}
		}

		public byte[] Types
		{
			get
			{
				return this.types;
			}
			set
			{
				this.types = value;
			}
		}

		private PointF[] points;

		private byte[] types;
	}
}
