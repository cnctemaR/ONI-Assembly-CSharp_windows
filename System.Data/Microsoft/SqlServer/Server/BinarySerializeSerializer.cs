using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Microsoft.SqlServer.Server
{
	internal sealed class BinarySerializeSerializer : Serializer
	{
		internal BinarySerializeSerializer(Type t)
			: base(t)
		{
		}

		public override void Serialize(Stream s, object o)
		{
			BinaryWriter binaryWriter = new BinaryWriter(s);
			((IBinarySerialize)o).Write(binaryWriter);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override object Deserialize(Stream s)
		{
			object obj = Activator.CreateInstance(this._type);
			BinaryReader binaryReader = new BinaryReader(s);
			((IBinarySerialize)obj).Read(binaryReader);
			return obj;
		}
	}
}
