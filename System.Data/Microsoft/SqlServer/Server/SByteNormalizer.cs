using System;
using System.IO;
using System.Reflection;

namespace Microsoft.SqlServer.Server
{
	internal sealed class SByteNormalizer : Normalizer
	{
		internal override void Normalize(FieldInfo fi, object obj, Stream s)
		{
			byte b = (byte)((sbyte)base.GetValue(fi, obj));
			if (!this._skipNormalize)
			{
				b ^= 128;
			}
			s.WriteByte(b);
		}

		internal override void DeNormalize(FieldInfo fi, object recvr, Stream s)
		{
			byte b = (byte)s.ReadByte();
			if (!this._skipNormalize)
			{
				b ^= 128;
			}
			sbyte b2 = (sbyte)b;
			base.SetValue(fi, recvr, b2);
		}

		internal override int Size
		{
			get
			{
				return 1;
			}
		}
	}
}
