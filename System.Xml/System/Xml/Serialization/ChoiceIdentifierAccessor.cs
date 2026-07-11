using System;
using System.Reflection;

namespace System.Xml.Serialization
{
	internal class ChoiceIdentifierAccessor : Accessor
	{
		internal string MemberName
		{
			get
			{
				return this.memberName;
			}
			set
			{
				this.memberName = value;
			}
		}

		internal string[] MemberIds
		{
			get
			{
				return this.memberIds;
			}
			set
			{
				this.memberIds = value;
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

		private string memberName;

		private string[] memberIds;

		private MemberInfo memberInfo;
	}
}
