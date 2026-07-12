using System;

namespace System.Reflection
{
	internal abstract class SignatureHasElementType : SignatureType
	{
		protected SignatureHasElementType(SignatureType elementType)
		{
			this._elementType = elementType;
		}

		public sealed override bool IsTypeDefinition
		{
			get
			{
				return false;
			}
		}

		public sealed override bool IsGenericTypeDefinition
		{
			get
			{
				return false;
			}
		}

		protected sealed override bool HasElementTypeImpl()
		{
			return true;
		}

		protected abstract override bool IsArrayImpl();

		protected abstract override bool IsByRefImpl();

		public sealed override bool IsByRefLike
		{
			get
			{
				return false;
			}
		}

		protected abstract override bool IsPointerImpl();

		public abstract override bool IsSZArray { get; }

		public abstract override bool IsVariableBoundArray { get; }

		public sealed override bool IsConstructedGenericType
		{
			get
			{
				return false;
			}
		}

		public sealed override bool IsGenericParameter
		{
			get
			{
				return false;
			}
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
				return false;
			}
		}

		public sealed override bool ContainsGenericParameters
		{
			get
			{
				return this._elementType.ContainsGenericParameters;
			}
		}

		internal sealed override SignatureType ElementType
		{
			get
			{
				return this._elementType;
			}
		}

		public abstract override int GetArrayRank();

		public sealed override Type GetGenericTypeDefinition()
		{
			throw new InvalidOperationException("This operation is only valid on generic types.");
		}

		public sealed override Type[] GetGenericArguments()
		{
			return Array.Empty<Type>();
		}

		public sealed override Type[] GenericTypeArguments
		{
			get
			{
				return Array.Empty<Type>();
			}
		}

		public sealed override int GenericParameterPosition
		{
			get
			{
				throw new InvalidOperationException("Method may only be called on a Type for which Type.IsGenericParameter is true.");
			}
		}

		public sealed override string Name
		{
			get
			{
				return this._elementType.Name + this.Suffix;
			}
		}

		public sealed override string Namespace
		{
			get
			{
				return this._elementType.Namespace;
			}
		}

		public sealed override string ToString()
		{
			return this._elementType.ToString() + this.Suffix;
		}

		protected abstract string Suffix { get; }

		private readonly SignatureType _elementType;
	}
}
