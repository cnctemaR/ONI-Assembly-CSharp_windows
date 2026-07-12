using System;

namespace System.Reflection
{
	internal sealed class SignatureArrayType : SignatureHasElementType
	{
		internal SignatureArrayType(SignatureType elementType, int rank, bool isMultiDim)
			: base(elementType)
		{
			this._rank = rank;
			this._isMultiDim = isMultiDim;
		}

		protected sealed override bool IsArrayImpl()
		{
			return true;
		}

		protected sealed override bool IsByRefImpl()
		{
			return false;
		}

		protected sealed override bool IsPointerImpl()
		{
			return false;
		}

		public sealed override bool IsSZArray
		{
			get
			{
				return !this._isMultiDim;
			}
		}

		public sealed override bool IsVariableBoundArray
		{
			get
			{
				return this._isMultiDim;
			}
		}

		public sealed override int GetArrayRank()
		{
			return this._rank;
		}

		protected sealed override string Suffix
		{
			get
			{
				if (!this._isMultiDim)
				{
					return "[]";
				}
				if (this._rank == 1)
				{
					return "[*]";
				}
				return "[" + new string(',', this._rank - 1) + "]";
			}
		}

		private readonly int _rank;

		private readonly bool _isMultiDim;
	}
}
