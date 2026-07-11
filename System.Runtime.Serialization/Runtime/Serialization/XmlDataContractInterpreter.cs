using System;
using System.Reflection;
using System.Xml.Serialization;

namespace System.Runtime.Serialization
{
	internal class XmlDataContractInterpreter
	{
		public XmlDataContractInterpreter(XmlDataContract contract)
		{
			this.contract = contract;
		}

		public IXmlSerializable CreateXmlSerializable()
		{
			Type underlyingType = this.contract.UnderlyingType;
			object obj;
			if (underlyingType.IsValueType)
			{
				obj = FormatterServices.GetUninitializedObject(underlyingType);
			}
			else
			{
				obj = this.GetConstructor().Invoke(new object[0]);
			}
			return (IXmlSerializable)obj;
		}

		private ConstructorInfo GetConstructor()
		{
			Type underlyingType = this.contract.UnderlyingType;
			if (underlyingType.IsValueType)
			{
				return null;
			}
			ConstructorInfo constructor = underlyingType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Globals.EmptyTypeArray, null);
			if (constructor == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("IXmlSerializable Type '{0}' must have default constructor.", new object[] { DataContract.GetClrTypeFullName(underlyingType) })));
			}
			return constructor;
		}

		private XmlDataContract contract;
	}
}
