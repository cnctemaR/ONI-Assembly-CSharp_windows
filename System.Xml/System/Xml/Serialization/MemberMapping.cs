using System;
using System.CodeDom.Compiler;
using System.Reflection;

namespace System.Xml.Serialization
{
	internal class MemberMapping : AccessorMapping
	{
		internal MemberMapping()
		{
		}

		private MemberMapping(MemberMapping mapping)
			: base(mapping)
		{
			this.name = mapping.name;
			this.checkShouldPersist = mapping.checkShouldPersist;
			this.checkSpecified = mapping.checkSpecified;
			this.isReturnValue = mapping.isReturnValue;
			this.readOnly = mapping.readOnly;
			this.sequenceId = mapping.sequenceId;
			this.memberInfo = mapping.memberInfo;
			this.checkSpecifiedMemberInfo = mapping.checkSpecifiedMemberInfo;
			this.checkShouldPersistMethodInfo = mapping.checkShouldPersistMethodInfo;
		}

		internal bool CheckShouldPersist
		{
			get
			{
				return this.checkShouldPersist;
			}
			set
			{
				this.checkShouldPersist = value;
			}
		}

		internal SpecifiedAccessor CheckSpecified
		{
			get
			{
				return this.checkSpecified;
			}
			set
			{
				this.checkSpecified = value;
			}
		}

		internal string Name
		{
			get
			{
				if (this.name != null)
				{
					return this.name;
				}
				return string.Empty;
			}
			set
			{
				this.name = value;
			}
		}

		internal MemberInfo MemberInfo
		{
			get
			{
				return this.memberInfo;
			}
			set
			{
				this.memberInfo = value;
			}
		}

		internal MemberInfo CheckSpecifiedMemberInfo
		{
			get
			{
				return this.checkSpecifiedMemberInfo;
			}
			set
			{
				this.checkSpecifiedMemberInfo = value;
			}
		}

		internal MethodInfo CheckShouldPersistMethodInfo
		{
			get
			{
				return this.checkShouldPersistMethodInfo;
			}
			set
			{
				this.checkShouldPersistMethodInfo = value;
			}
		}

		internal bool IsReturnValue
		{
			get
			{
				return this.isReturnValue;
			}
			set
			{
				this.isReturnValue = value;
			}
		}

		internal bool ReadOnly
		{
			get
			{
				return this.readOnly;
			}
			set
			{
				this.readOnly = value;
			}
		}

		internal bool IsSequence
		{
			get
			{
				return this.sequenceId >= 0;
			}
		}

		internal int SequenceId
		{
			get
			{
				return this.sequenceId;
			}
			set
			{
				this.sequenceId = value;
			}
		}

		private string GetNullableType(TypeDesc td)
		{
			if (td.IsMappedType || (!td.IsValueType && (base.Elements[0].IsSoap || td.ArrayElementTypeDesc == null)))
			{
				return td.FullName;
			}
			if (td.ArrayElementTypeDesc != null)
			{
				return this.GetNullableType(td.ArrayElementTypeDesc) + "[]";
			}
			return "System.Nullable`1[" + td.FullName + "]";
		}

		internal MemberMapping Clone()
		{
			return new MemberMapping(this);
		}

		internal string GetTypeName(CodeDomProvider codeProvider)
		{
			if (base.IsNeedNullable && codeProvider.Supports(GeneratorSupport.GenericTypeReference))
			{
				return this.GetNullableType(base.TypeDesc);
			}
			return base.TypeDesc.FullName;
		}

		private string name;

		private bool checkShouldPersist;

		private SpecifiedAccessor checkSpecified;

		private bool isReturnValue;

		private bool readOnly;

		private int sequenceId = -1;

		private MemberInfo memberInfo;

		private MemberInfo checkSpecifiedMemberInfo;

		private MethodInfo checkShouldPersistMethodInfo;
	}
}
