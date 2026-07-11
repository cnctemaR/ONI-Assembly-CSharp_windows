using System;
using System.Reflection;

namespace System.Xml.Serialization
{
	internal class StructModel : TypeModel
	{
		internal StructModel(Type type, TypeDesc typeDesc, ModelScope scope)
			: base(type, typeDesc, scope)
		{
		}

		internal MemberInfo[] GetMemberInfos()
		{
			MemberInfo[] members = base.Type.GetMembers(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
			MemberInfo[] array = new MemberInfo[members.Length];
			int num = 0;
			for (int i = 0; i < members.Length; i++)
			{
				if ((members[i].MemberType & MemberTypes.Property) == (MemberTypes)0)
				{
					array[num++] = members[i];
				}
			}
			for (int j = 0; j < members.Length; j++)
			{
				if ((members[j].MemberType & MemberTypes.Property) != (MemberTypes)0)
				{
					array[num++] = members[j];
				}
			}
			return array;
		}

		internal FieldModel GetFieldModel(MemberInfo memberInfo)
		{
			FieldModel fieldModel = null;
			if (memberInfo is FieldInfo)
			{
				fieldModel = this.GetFieldModel((FieldInfo)memberInfo);
			}
			else if (memberInfo is PropertyInfo)
			{
				fieldModel = this.GetPropertyModel((PropertyInfo)memberInfo);
			}
			if (fieldModel != null && fieldModel.ReadOnly && fieldModel.FieldTypeDesc.Kind != TypeKind.Collection && fieldModel.FieldTypeDesc.Kind != TypeKind.Enumerable)
			{
				return null;
			}
			return fieldModel;
		}

		private void CheckSupportedMember(TypeDesc typeDesc, MemberInfo member, Type type)
		{
			if (typeDesc == null)
			{
				return;
			}
			if (typeDesc.IsUnsupported)
			{
				if (typeDesc.Exception == null)
				{
					typeDesc.Exception = new NotSupportedException(Res.GetString("{0} is an unsupported type. Please use [XmlIgnore] attribute to exclude members of this type from serialization graph.", new object[] { typeDesc.FullName }));
				}
				throw new InvalidOperationException(Res.GetString("Cannot serialize member '{0}' of type '{1}', see inner exception for more details.", new object[]
				{
					member.DeclaringType.FullName + "." + member.Name,
					type.FullName
				}), typeDesc.Exception);
			}
			this.CheckSupportedMember(typeDesc.BaseTypeDesc, member, type);
			this.CheckSupportedMember(typeDesc.ArrayElementTypeDesc, member, type);
		}

		private FieldModel GetFieldModel(FieldInfo fieldInfo)
		{
			if (fieldInfo.IsStatic)
			{
				return null;
			}
			if (fieldInfo.DeclaringType != base.Type)
			{
				return null;
			}
			TypeDesc typeDesc = base.ModelScope.TypeScope.GetTypeDesc(fieldInfo.FieldType, fieldInfo, true, false);
			if (fieldInfo.IsInitOnly && typeDesc.Kind != TypeKind.Collection && typeDesc.Kind != TypeKind.Enumerable)
			{
				return null;
			}
			this.CheckSupportedMember(typeDesc, fieldInfo, fieldInfo.FieldType);
			return new FieldModel(fieldInfo, fieldInfo.FieldType, typeDesc);
		}

		private FieldModel GetPropertyModel(PropertyInfo propertyInfo)
		{
			if (propertyInfo.DeclaringType != base.Type)
			{
				return null;
			}
			if (!StructModel.CheckPropertyRead(propertyInfo))
			{
				return null;
			}
			TypeDesc typeDesc = base.ModelScope.TypeScope.GetTypeDesc(propertyInfo.PropertyType, propertyInfo, true, false);
			if (!propertyInfo.CanWrite && typeDesc.Kind != TypeKind.Collection && typeDesc.Kind != TypeKind.Enumerable)
			{
				return null;
			}
			this.CheckSupportedMember(typeDesc, propertyInfo, propertyInfo.PropertyType);
			return new FieldModel(propertyInfo, propertyInfo.PropertyType, typeDesc);
		}

		internal static bool CheckPropertyRead(PropertyInfo propertyInfo)
		{
			if (!propertyInfo.CanRead)
			{
				return false;
			}
			MethodInfo getMethod = propertyInfo.GetGetMethod();
			return !getMethod.IsStatic && getMethod.GetParameters().Length == 0;
		}
	}
}
