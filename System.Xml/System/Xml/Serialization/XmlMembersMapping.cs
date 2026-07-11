using System;

namespace System.Xml.Serialization
{
	public class XmlMembersMapping : XmlMapping
	{
		internal XmlMembersMapping()
		{
		}

		internal XmlMembersMapping(XmlMemberMapping[] mapping)
			: this(string.Empty, null, false, false, mapping)
		{
		}

		internal XmlMembersMapping(string elementName, string ns, XmlMemberMapping[] mapping)
			: this(elementName, ns, true, false, mapping)
		{
		}

		internal XmlMembersMapping(string elementName, string ns, bool hasWrapperElement, bool writeAccessors, XmlMemberMapping[] mapping)
			: base(elementName, ns)
		{
			this._hasWrapperElement = hasWrapperElement;
			this._mapping = mapping;
			ClassMap classMap = new ClassMap();
			classMap.IgnoreMemberNamespace = writeAccessors;
			foreach (XmlMemberMapping xmlMemberMapping in mapping)
			{
				classMap.AddMember(xmlMemberMapping.TypeMapMember);
			}
			base.ObjectMap = classMap;
		}

		public int Count
		{
			get
			{
				return this._mapping.Length;
			}
		}

		public XmlMemberMapping this[int index]
		{
			get
			{
				return this._mapping[index];
			}
		}

		public string TypeName
		{
			[MonoTODO]
			get
			{
				throw new NotImplementedException();
			}
		}

		public string TypeNamespace
		{
			[MonoTODO]
			get
			{
				throw new NotImplementedException();
			}
		}

		internal bool HasWrapperElement
		{
			get
			{
				return this._hasWrapperElement;
			}
		}

		private bool _hasWrapperElement;

		private XmlMemberMapping[] _mapping;
	}
}
