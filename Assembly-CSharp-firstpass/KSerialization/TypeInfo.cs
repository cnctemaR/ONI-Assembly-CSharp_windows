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
			bool flag;
			if (obj != null && obj is TypeInfo)
			{
				TypeInfo typeInfo = (TypeInfo)obj;
				flag = this.Equals(typeInfo);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public bool Equals(TypeInfo other)
		{
			bool flag;
			if (this.info == other.info)
			{
				if (this.subTypes != null && other.subTypes != null)
				{
					if (this.subTypes.Length == other.subTypes.Length)
					{
						for (int i = 0; i < this.subTypes.Length; i++)
						{
							if (!this.subTypes[i].Equals(other.subTypes[i]))
							{
								return false;
							}
						}
						flag = true;
					}
					else
					{
						flag = false;
					}
				}
				else
				{
					flag = this.subTypes == null && other.subTypes == null && this.type == other.type;
				}
			}
			else
			{
				flag = false;
			}
			return flag;
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
