using System;
using System.IO;
using System.Reflection;

namespace Microsoft.SqlServer.Server
{
	internal sealed class BooleanNormalizer : Normalizer
	{
		internal override void Normalize(FieldInfo fi, object obj, Stream s)
		{
			s.WriteByte(((bool)base.GetValue(fi, obj)) ? 1 : 0);
		}

		internal override void DeNormalize(FieldInfo fi, object recvr, Stream s)
		{
			byte b = (byte)s.ReadByte();
			base.SetValue(fi, recvr, b == 1);
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
