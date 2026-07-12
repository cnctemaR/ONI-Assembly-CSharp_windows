using System;

namespace System.Reflection
{
	internal sealed class SignatureGenericMethodParameterType : SignatureGenericParameterType
	{
		internal SignatureGenericMethodParameterType(int position)
			: base(position)
		{
		}

		public sealed override bool IsGenericTypeParameter
		{
			get
			{
				return false;
			}
		}

		public sealed override bool IsGenericMethodParameter
		{
			get
			{
				return true;
			}
		}

		public sealed override string Name
		{
			get
			{
				return "!!" + this.GenericParameterPosition.ToString();
			}
		}
	}
}
