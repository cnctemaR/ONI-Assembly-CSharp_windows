using System;

namespace System.Reflection
{
	internal sealed class SignaturePointerType : SignatureHasElementType
	{
		internal SignaturePointerType(SignatureType elementType)
			: base(elementType)
		{
		}

		protected sealed override bool IsArrayImpl()
		{
			return false;
		}

		protected sealed override bool IsByRefImpl()
		{
			return false;
		}

		protected sealed override bool IsPointerImpl()
		{
			return true;
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
				return "*";
			}
		}
	}
}
