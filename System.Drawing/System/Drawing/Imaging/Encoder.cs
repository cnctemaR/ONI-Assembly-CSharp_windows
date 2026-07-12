using System;

namespace System.Drawing.Imaging
{
	public sealed class Encoder
	{
		public Encoder(Guid guid)
		{
			this._guid = guid;
		}

		public Guid Guid
		{
			get
			{
				return this._guid;
			}
		}

		public static readonly Encoder Compression = new Encoder(new Guid(-526552163, -13100, 17646, new byte[] { 142, 186, 63, 191, 139, 228, 252, 88 }));

		public static readonly Encoder ColorDepth = new Encoder(new Guid(1711829077, -21146, 19580, new byte[] { 154, 24, 56, 162, 49, 11, 131, 55 }));

		public static readonly Encoder ScanMethod = new Encoder(new Guid(978200161, 12553, 20054, new byte[] { 133, 54, 66, 193, 86, 231, 220, 250 }));

		public static readonly Encoder Version = new Encoder(new Guid(617712758, -32438, 16804, new byte[] { 191, 83, 28, 33, 156, 204, 247, 151 }));

		public static readonly Encoder RenderMethod = new Encoder(new Guid(1833092410, 8858, 18469, new byte[] { 139, 183, 92, 153, 226, 185, 168, 184 }));

		public static readonly Encoder Quality = new Encoder(new Guid(492561589, -1462, 17709, new byte[] { 156, 221, 93, 179, 81, 5, 231, 235 }));

		public static readonly Encoder Transformation = new Encoder(new Guid(-1928416559, -23154, 20136, new byte[] { 170, 20, 16, 128, 116, 183, 182, 249 }));

		public static readonly Encoder LuminanceTable = new Encoder(new Guid(-307020850, 614, 19063, new byte[] { 185, 4, 39, 33, 96, 153, 231, 23 }));

		public static readonly Encoder ChrominanceTable = new Encoder(new Guid(-219916836, 2483, 17174, new byte[] { 130, 96, 103, 106, 218, 50, 72, 28 }));

		public static readonly Encoder SaveFlag = new Encoder(new Guid(690120444, -21440, 18367, new byte[] { 140, 252, 168, 91, 137, 166, 85, 222 }));

		private Guid _guid;
	}
}
