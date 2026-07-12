using System;
using System.IO;

namespace Microsoft.SqlServer.Server
{
	internal sealed class NormalizedSerializer : Serializer
	{
		internal NormalizedSerializer(Type t)
			: base(t)
		{
			SqlUserDefinedTypeAttribute udtAttribute = SerializationHelperSql9.GetUdtAttribute(t);
			this._normalizer = new BinaryOrderedUdtNormalizer(t, true);
			this._isFixedSize = udtAttribute.IsFixedLength;
			this._maxSize = this._normalizer.Size;
		}

		public override void Serialize(Stream s, object o)
		{
			this._normalizer.NormalizeTopObject(o, s);
		}

		public override object Deserialize(Stream s)
		{
			return this._normalizer.DeNormalizeTopObject(this._type, s);
		}

		private BinaryOrderedUdtNormalizer _normalizer;

		private bool _isFixedSize;

		private int _maxSize;
	}
}
