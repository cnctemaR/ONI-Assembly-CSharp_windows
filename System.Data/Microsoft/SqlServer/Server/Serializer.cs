using System;
using System.IO;

namespace Microsoft.SqlServer.Server
{
	internal abstract class Serializer
	{
		public abstract object Deserialize(Stream s);

		public abstract void Serialize(Stream s, object o);

		protected Serializer(Type t)
		{
			this._type = t;
		}

		protected Type _type;
	}
}
