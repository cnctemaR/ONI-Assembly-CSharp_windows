using System;
using System.Text;

namespace System.Xml.Serialization
{
	public class XmlMembersMapping : XmlMapping
	{
		internal XmlMembersMapping(TypeScope scope, ElementAccessor accessor, XmlMappingAccess access)
			: base(scope, accessor, access)
		{
			MembersMapping membersMapping = (MembersMapping)accessor.Mapping;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(":");
			this.mappings = new XmlMemberMapping[membersMapping.Members.Length];
			for (int i = 0; i < this.mappings.Length; i++)
			{
				if (membersMapping.Members[i].TypeDesc.Type != null)
				{
					stringBuilder.Append(XmlMapping.GenerateKey(membersMapping.Members[i].TypeDesc.Type, null, null));
					stringBuilder.Append(":");
				}
				this.mappings[i] = new XmlMemberMapping(membersMapping.Members[i]);
			}
			base.SetKeyInternal(stringBuilder.ToString());
		}

		public string TypeName
		{
			get
			{
				return base.Accessor.Mapping.TypeName;
			}
		}

		public string TypeNamespace
		{
			get
			{
				return base.Accessor.Mapping.Namespace;
			}
		}

		public XmlMemberMapping this[int index]
		{
			get
			{
				return this.mappings[index];
			}
		}

		public int Count
		{
			get
			{
				return this.mappings.Length;
			}
		}

		private XmlMemberMapping[] mappings;
	}
}
