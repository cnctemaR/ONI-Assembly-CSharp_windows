using System;

namespace System.Xml.Serialization
{
	internal class MembersMapping : TypeMapping
	{
		internal MemberMapping[] Members
		{
			get
			{
				return this.members;
			}
			set
			{
				this.members = value;
			}
		}

		internal MemberMapping XmlnsMember
		{
			get
			{
				return this.xmlnsMember;
			}
			set
			{
				this.xmlnsMember = value;
			}
		}

		internal bool HasWrapperElement
		{
			get
			{
				return this.hasWrapperElement;
			}
			set
			{
				this.hasWrapperElement = value;
			}
		}

		internal bool ValidateRpcWrapperElement
		{
			get
			{
				return this.validateRpcWrapperElement;
			}
			set
			{
				this.validateRpcWrapperElement = value;
			}
		}

		internal bool WriteAccessors
		{
			get
			{
				return this.writeAccessors;
			}
			set
			{
				this.writeAccessors = value;
			}
		}

		private MemberMapping[] members;

		private bool hasWrapperElement = true;

		private bool validateRpcWrapperElement;

		private bool writeAccessors = true;

		private MemberMapping xmlnsMember;
	}
}
