using System;

namespace System.Xml.Serialization
{
	internal class PrimitiveModel : TypeModel
	{
		internal PrimitiveModel(Type type, TypeDesc typeDesc, ModelScope scope)
			: base(type, typeDesc, scope)
		{
		}
	}
}
