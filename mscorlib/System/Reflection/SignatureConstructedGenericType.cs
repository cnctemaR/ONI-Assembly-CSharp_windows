using System;
using System.Text;

namespace System.Reflection
{
	internal sealed class SignatureConstructedGenericType : SignatureType
	{
		internal SignatureConstructedGenericType(Type genericTypeDefinition, Type[] typeArguments)
		{
			if (genericTypeDefinition == null)
			{
				throw new ArgumentNullException("genericTypeDefinition");
			}
			if (typeArguments == null)
			{
				throw new ArgumentNullException("typeArguments");
			}
			typeArguments = (Type[])typeArguments.Clone();
			for (int i = 0; i < typeArguments.Length; i++)
			{
				if (typeArguments[i] == null)
				{
					throw new ArgumentNullException("typeArguments");
				}
			}
			this._genericTypeDefinition = genericTypeDefinition;
			this._genericTypeArguments = typeArguments;
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
				return this._genericTypeDefinition.IsByRefLike;
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
				return true;
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
				for (int i = 0; i < this._genericTypeArguments.Length; i++)
				{
					if (this._genericTypeArguments[i].ContainsGenericParameters)
					{
						return true;
					}
				}
				return false;
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
			return this._genericTypeDefinition;
		}

		public sealed override Type[] GetGenericArguments()
		{
			return this.GenericTypeArguments;
		}

		public sealed override Type[] GenericTypeArguments
		{
			get
			{
				return (Type[])this._genericTypeArguments.Clone();
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
				return this._genericTypeDefinition.Name;
			}
		}

		public sealed override string Namespace
		{
			get
			{
				return this._genericTypeDefinition.Namespace;
			}
		}

		public sealed override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this._genericTypeDefinition.ToString());
			stringBuilder.Append('[');
			for (int i = 0; i < this._genericTypeArguments.Length; i++)
			{
				if (i != 0)
				{
					stringBuilder.Append(',');
				}
				stringBuilder.Append(this._genericTypeArguments[i].ToString());
			}
			stringBuilder.Append(']');
			return stringBuilder.ToString();
		}

		private readonly Type _genericTypeDefinition;

		private readonly Type[] _genericTypeArguments;
	}
}
