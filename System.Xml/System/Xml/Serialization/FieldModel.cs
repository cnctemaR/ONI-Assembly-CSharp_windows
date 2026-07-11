using System;
using System.Reflection;

namespace System.Xml.Serialization
{
	internal class FieldModel
	{
		internal FieldModel(string name, Type fieldType, TypeDesc fieldTypeDesc, bool checkSpecified, bool checkShouldPersist)
			: this(name, fieldType, fieldTypeDesc, checkSpecified, checkShouldPersist, false)
		{
		}

		internal FieldModel(string name, Type fieldType, TypeDesc fieldTypeDesc, bool checkSpecified, bool checkShouldPersist, bool readOnly)
		{
			this.fieldTypeDesc = fieldTypeDesc;
			this.name = name;
			this.fieldType = fieldType;
			this.checkSpecified = (checkSpecified ? SpecifiedAccessor.ReadWrite : SpecifiedAccessor.None);
			this.checkShouldPersist = checkShouldPersist;
			this.readOnly = readOnly;
		}

		internal FieldModel(MemberInfo memberInfo, Type fieldType, TypeDesc fieldTypeDesc)
		{
			this.name = memberInfo.Name;
			this.fieldType = fieldType;
			this.fieldTypeDesc = fieldTypeDesc;
			this.memberInfo = memberInfo;
			this.checkShouldPersistMethodInfo = memberInfo.DeclaringType.GetMethod("ShouldSerialize" + memberInfo.Name, new Type[0]);
			this.checkShouldPersist = this.checkShouldPersistMethodInfo != null;
			FieldInfo field = memberInfo.DeclaringType.GetField(memberInfo.Name + "Specified");
			if (field != null)
			{
				if (field.FieldType != typeof(bool))
				{
					throw new InvalidOperationException(Res.GetString("Member '{0}' of type {1} cannot be serialized.  Members with names ending on 'Specified' suffix have special meaning to the XmlSerializer: they control serialization of optional ValueType members and have to be of type {2}.", new object[]
					{
						field.Name,
						field.FieldType.FullName,
						typeof(bool).FullName
					}));
				}
				this.checkSpecified = (field.IsInitOnly ? SpecifiedAccessor.ReadOnly : SpecifiedAccessor.ReadWrite);
				this.checkSpecifiedMemberInfo = field;
			}
			else
			{
				PropertyInfo property = memberInfo.DeclaringType.GetProperty(memberInfo.Name + "Specified");
				if (property != null)
				{
					if (StructModel.CheckPropertyRead(property))
					{
						this.checkSpecified = (property.CanWrite ? SpecifiedAccessor.ReadWrite : SpecifiedAccessor.ReadOnly);
						this.checkSpecifiedMemberInfo = property;
					}
					if (this.checkSpecified != SpecifiedAccessor.None && property.PropertyType != typeof(bool))
					{
						throw new InvalidOperationException(Res.GetString("Member '{0}' of type {1} cannot be serialized.  Members with names ending on 'Specified' suffix have special meaning to the XmlSerializer: they control serialization of optional ValueType members and have to be of type {2}.", new object[]
						{
							property.Name,
							property.PropertyType.FullName,
							typeof(bool).FullName
						}));
					}
				}
			}
			if (memberInfo is PropertyInfo)
			{
				this.readOnly = !((PropertyInfo)memberInfo).CanWrite;
				this.isProperty = true;
				return;
			}
			if (memberInfo is FieldInfo)
			{
				this.readOnly = ((FieldInfo)memberInfo).IsInitOnly;
			}
		}

		internal string Name
		{
			get
			{
				return this.name;
			}
		}

		internal Type FieldType
		{
			get
			{
				return this.fieldType;
			}
		}

		internal TypeDesc FieldTypeDesc
		{
			get
			{
				return this.fieldTypeDesc;
			}
		}

		internal bool CheckShouldPersist
		{
			get
			{
				return this.checkShouldPersist;
			}
		}

		internal SpecifiedAccessor CheckSpecified
		{
			get
			{
				return this.checkSpecified;
			}
		}

		internal MemberInfo MemberInfo
		{
			get
			{
				return this.memberInfo;
			}
		}

		internal MemberInfo CheckSpecifiedMemberInfo
		{
			get
			{
				return this.checkSpecifiedMemberInfo;
			}
		}

		internal MethodInfo CheckShouldPersistMethodInfo
		{
			get
			{
				return this.checkShouldPersistMethodInfo;
			}
		}

		internal bool ReadOnly
		{
			get
			{
				return this.readOnly;
			}
		}

		internal bool IsProperty
		{
			get
			{
				return this.isProperty;
			}
		}

		private SpecifiedAccessor checkSpecified;

		private MemberInfo memberInfo;

		private MemberInfo checkSpecifiedMemberInfo;

		private MethodInfo checkShouldPersistMethodInfo;

		private bool checkShouldPersist;

		private bool readOnly;

		private bool isProperty;

		private Type fieldType;

		private string name;

		private TypeDesc fieldTypeDesc;
	}
}
