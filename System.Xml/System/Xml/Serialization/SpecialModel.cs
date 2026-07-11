using System;

namespace System.Xml.Serialization
{
	internal class SpecialModel : TypeModel
	{
		internal SpecialModel(Type type, TypeDesc typeDesc, ModelScope scope)
			: base(type, typeDesc, scope)
		{
		}
	}
}
