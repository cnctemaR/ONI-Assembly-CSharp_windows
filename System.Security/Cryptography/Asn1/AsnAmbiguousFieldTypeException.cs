using System;
using System.Reflection;

namespace System.Security.Cryptography.Asn1
{
	internal class AsnAmbiguousFieldTypeException : AsnSerializationConstraintException
	{
		public AsnAmbiguousFieldTypeException(FieldInfo fieldInfo, Type ambiguousType)
			: base(SR.Format("Field '{0}' of type '{1}' has ambiguous type '{2}', an attribute derived from AsnTypeAttribute is required.", fieldInfo.Name, fieldInfo.DeclaringType.FullName, ambiguousType.Namespace))
		{
		}
	}
}
