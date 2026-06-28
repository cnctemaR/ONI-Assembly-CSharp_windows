using System;

namespace KSerialization
{
	public class TypeInfo
	{
		public void BuildGenericArgs()
		{
			if (this.type.IsGenericType)
			{
				Type genericTypeDefinition = this.type.GetGenericTypeDefinition();
				this.genericTypeArgs = this.type.GetGenericArguments();
				this.genericInstantiationType = genericTypeDefinition.MakeGenericType(this.genericTypeArgs);
			}
			if (this.subTypes != null)
			{
				for (int i = 0; i < this.subTypes.Length; i++)
				{
					this.subTypes[i].BuildGenericArgs();
				}
			}
		}

		public override bool Equals(object obj)
		{
			if (obj != null && obj is TypeInfo)
			{
				TypeInfo typeInfo = (TypeInfo)obj;
				return this.Equals(typeInfo);
			}
			return false;
		}

		public bool Equals(TypeInfo other)
		{
			if (this.info != other.info)
			{
				return false;
			}
			if (this.subTypes == null || other.subTypes == null)
			{
				return this.subTypes == null && other.subTypes == null && this.type == other.type;
			}
			if (this.subTypes.Length == other.subTypes.Length)
			{
				for (int i = 0; i < this.subTypes.Length; i++)
				{
					if (!this.subTypes[i].Equals(other.subTypes[i]))
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return this.type.GetHashCode();
		}

		public Type type;

		public SerializationTypeInfo info;

		public TypeInfo[] subTypes;

		public Type genericInstantiationType;

		public Type[] genericTypeArgs;
	}
}
