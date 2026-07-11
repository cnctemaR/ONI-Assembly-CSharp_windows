using System;

namespace System.Xml.Serialization
{
	internal class CodeGeneratorConversionException : Exception
	{
		public CodeGeneratorConversionException(Type sourceType, Type targetType, bool isAddress, string reason)
		{
			this.sourceType = sourceType;
			this.targetType = targetType;
			this.isAddress = isAddress;
			this.reason = reason;
		}

		private Type sourceType;

		private Type targetType;

		private bool isAddress;

		private string reason;
	}
}
