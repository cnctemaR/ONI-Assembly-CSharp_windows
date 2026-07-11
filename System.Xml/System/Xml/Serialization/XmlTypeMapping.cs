using System;

namespace System.Xml.Serialization
{
	public class XmlTypeMapping : XmlMapping
	{
		internal XmlTypeMapping(TypeScope scope, ElementAccessor accessor)
			: base(scope, accessor)
		{
		}

		internal TypeMapping Mapping
		{
			get
			{
				return base.Accessor.Mapping;
			}
		}

		public string TypeName
		{
			get
			{
				return this.Mapping.TypeDesc.Name;
			}
		}

		public string TypeFullName
		{
			get
			{
				return this.Mapping.TypeDesc.FullName;
			}
		}

		public string XsdTypeName
		{
			get
			{
				return this.Mapping.TypeName;
			}
		}

		public string XsdTypeNamespace
		{
			get
			{
				return this.Mapping.Namespace;
			}
		}
	}
}
