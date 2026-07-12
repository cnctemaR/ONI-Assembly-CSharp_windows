using System;

namespace System.Reflection
{
	internal sealed class SignatureByRefType : SignatureHasElementType
	{
		internal SignatureByRefType(SignatureType elementType)
			: base(elementType)
		{
		}

		protected sealed override bool IsArrayImpl()
		{
			return false;
		}

		protected sealed override bool IsByRefImpl()
		{
			return true;
		}

		protected sealed override bool IsPointerImpl()
		{
			return false;
		}

		public sealed override bool IsSZArray
		{
			get
			{
				return false;
			}
		}

		public sealed override bool IsVariableBoundArray
		{
			get
			{
				return false;
			}
		}

		public sealed override int GetArrayRank()
		{
			throw new ArgumentException("Must be an array type.");
		}

		protected sealed override string Suffix
		{
			get
			{
				return "&";
			}
		}
	}
}
