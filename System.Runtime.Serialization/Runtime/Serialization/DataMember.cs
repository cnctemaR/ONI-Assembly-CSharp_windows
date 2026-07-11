using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security;

namespace System.Runtime.Serialization
{
	internal class DataMember
	{
		[SecuritySafeCritical]
		internal DataMember()
		{
			this.helper = new DataMember.CriticalHelper();
		}

		[SecuritySafeCritical]
		internal DataMember(MemberInfo memberInfo)
		{
			this.helper = new DataMember.CriticalHelper(memberInfo);
		}

		[SecuritySafeCritical]
		internal DataMember(string name)
		{
			this.helper = new DataMember.CriticalHelper(name);
		}

		[SecuritySafeCritical]
		internal DataMember(DataContract memberTypeContract, string name, bool isNullable, bool isRequired, bool emitDefaultValue, int order)
		{
			this.helper = new DataMember.CriticalHelper(memberTypeContract, name, isNullable, isRequired, emitDefaultValue, order);
		}

		internal MemberInfo MemberInfo
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.MemberInfo;
			}
		}

		internal string Name
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.Name;
			}
			[SecurityCritical]
			set
			{
				this.helper.Name = value;
			}
		}

		internal int Order
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.Order;
			}
			[SecurityCritical]
			set
			{
				this.helper.Order = value;
			}
		}

		internal bool IsRequired
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.IsRequired;
			}
			[SecurityCritical]
			set
			{
				this.helper.IsRequired = value;
			}
		}

		internal bool EmitDefaultValue
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.EmitDefaultValue;
			}
			[SecurityCritical]
			set
			{
				this.helper.EmitDefaultValue = value;
			}
		}

		internal bool IsNullable
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.IsNullable;
			}
			[SecurityCritical]
			set
			{
				this.helper.IsNullable = value;
			}
		}

		internal bool IsGetOnlyCollection
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.IsGetOnlyCollection;
			}
			[SecurityCritical]
			set
			{
				this.helper.IsGetOnlyCollection = value;
			}
		}

		internal Type MemberType
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.MemberType;
			}
		}

		internal DataContract MemberTypeContract
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.MemberTypeContract;
			}
			[SecurityCritical]
			set
			{
				this.helper.MemberTypeContract = value;
			}
		}

		internal bool HasConflictingNameAndType
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.HasConflictingNameAndType;
			}
			[SecurityCritical]
			set
			{
				this.helper.HasConflictingNameAndType = value;
			}
		}

		internal DataMember ConflictingMember
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.ConflictingMember;
			}
			[SecurityCritical]
			set
			{
				this.helper.ConflictingMember = value;
			}
		}

		internal DataMember BindGenericParameters(DataContract[] paramContracts, Dictionary<DataContract, DataContract> boundContracts)
		{
			DataContract dataContract = this.MemberTypeContract.BindGenericParameters(paramContracts, boundContracts);
			return new DataMember(dataContract, this.Name, !dataContract.IsValueType, this.IsRequired, this.EmitDefaultValue, this.Order);
		}

		internal bool Equals(object other, Dictionary<DataContractPairKey, object> checkedContracts)
		{
			if (this == other)
			{
				return true;
			}
			DataMember dataMember = other as DataMember;
			if (dataMember != null)
			{
				bool flag = this.MemberTypeContract != null && !this.MemberTypeContract.IsValueType;
				bool flag2 = dataMember.MemberTypeContract != null && !dataMember.MemberTypeContract.IsValueType;
				return this.Name == dataMember.Name && (this.IsNullable || flag) == (dataMember.IsNullable || flag2) && this.IsRequired == dataMember.IsRequired && this.EmitDefaultValue == dataMember.EmitDefaultValue && this.MemberTypeContract.Equals(dataMember.MemberTypeContract, checkedContracts);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		[SecurityCritical]
		private DataMember.CriticalHelper helper;

		[SecurityCritical(SecurityCriticalScope.Everything)]
		private class CriticalHelper
		{
			internal CriticalHelper()
			{
				this.emitDefaultValue = true;
			}

			internal CriticalHelper(MemberInfo memberInfo)
			{
				this.emitDefaultValue = true;
				this.memberInfo = memberInfo;
			}

			internal CriticalHelper(string name)
			{
				this.Name = name;
			}

			internal CriticalHelper(DataContract memberTypeContract, string name, bool isNullable, bool isRequired, bool emitDefaultValue, int order)
			{
				this.MemberTypeContract = memberTypeContract;
				this.Name = name;
				this.IsNullable = isNullable;
				this.IsRequired = isRequired;
				this.EmitDefaultValue = emitDefaultValue;
				this.Order = order;
			}

			internal MemberInfo MemberInfo
			{
				get
				{
					return this.memberInfo;
				}
			}

			internal string Name
			{
				get
				{
					return this.name;
				}
				set
				{
					this.name = value;
				}
			}

			internal int Order
			{
				get
				{
					return this.order;
				}
				set
				{
					this.order = value;
				}
			}

			internal bool IsRequired
			{
				get
				{
					return this.isRequired;
				}
				set
				{
					this.isRequired = value;
				}
			}

			internal bool EmitDefaultValue
			{
				get
				{
					return this.emitDefaultValue;
				}
				set
				{
					this.emitDefaultValue = value;
				}
			}

			internal bool IsNullable
			{
				get
				{
					return this.isNullable;
				}
				set
				{
					this.isNullable = value;
				}
			}

			internal bool IsGetOnlyCollection
			{
				get
				{
					return this.isGetOnlyCollection;
				}
				set
				{
					this.isGetOnlyCollection = value;
				}
			}

			internal Type MemberType
			{
				get
				{
					FieldInfo fieldInfo = this.MemberInfo as FieldInfo;
					if (fieldInfo != null)
					{
						return fieldInfo.FieldType;
					}
					return ((PropertyInfo)this.MemberInfo).PropertyType;
				}
			}

			internal DataContract MemberTypeContract
			{
				get
				{
					if (this.memberTypeContract == null && this.MemberInfo != null)
					{
						if (this.IsGetOnlyCollection)
						{
							this.memberTypeContract = DataContract.GetGetOnlyCollectionDataContract(DataContract.GetId(this.MemberType.TypeHandle), this.MemberType.TypeHandle, this.MemberType, SerializationMode.SharedContract);
						}
						else
						{
							this.memberTypeContract = DataContract.GetDataContract(this.MemberType);
						}
					}
					return this.memberTypeContract;
				}
				set
				{
					this.memberTypeContract = value;
				}
			}

			internal bool HasConflictingNameAndType
			{
				get
				{
					return this.hasConflictingNameAndType;
				}
				set
				{
					this.hasConflictingNameAndType = value;
				}
			}

			internal DataMember ConflictingMember
			{
				get
				{
					return this.conflictingMember;
				}
				set
				{
					this.conflictingMember = value;
				}
			}

			private DataContract memberTypeContract;

			private string name;

			private int order;

			private bool isRequired;

			private bool emitDefaultValue;

			private bool isNullable;

			private bool isGetOnlyCollection;

			private MemberInfo memberInfo;

			private bool hasConflictingNameAndType;

			private DataMember conflictingMember;
		}
	}
}
