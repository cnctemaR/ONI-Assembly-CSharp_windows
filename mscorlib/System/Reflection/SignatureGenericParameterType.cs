using System;

namespace System.Reflection
{
	internal abstract class SignatureGenericParameterType : SignatureType
	{
		protected SignatureGenericParameterType(int position)
		{
			this._position = position;
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
			return false;
		}

		protected sealed override bool IsArrayImpl()
		{
			return false;
		}

		protected sealed override bool IsByRefImpl()
		{
			return false;
		}

		public sealed override bool IsByRefLike
		{
			get
			{
				return false;
			}
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
				return true;
			}
		}

		public abstract override bool IsGenericMethodParameter { get; }

		public sealed override bool ContainsGenericParameters
		{
			get
			{
				return true;
			}
		}

		internal sealed override SignatureType ElementType
		{
			get
			{
				return null;
			}
		}

		public sealed override int GetArrayRank()
		{
			throw new ArgumentException("Must be an array type.");
		}

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
				return this._position;
			}
		}

		public abstract override string Name { get; }

		public sealed override string Namespace
		{
			get
			{
				return null;
			}
		}

		public sealed override string ToString()
		{
			return this.Name;
		}

		private readonly int _position;
	}
}
