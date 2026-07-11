using System;
using System.CodeDom.Compiler;

namespace System.Xml.Serialization
{
	public class XmlMemberMapping
	{
		internal XmlMemberMapping(MemberMapping mapping)
		{
			this.mapping = mapping;
		}

		internal MemberMapping Mapping
		{
			get
			{
				return this.mapping;
			}
		}

		internal Accessor Accessor
		{
			get
			{
				return this.mapping.Accessor;
			}
		}

		public bool Any
		{
			get
			{
				return this.Accessor.Any;
			}
		}

		public string ElementName
		{
			get
			{
				return Accessor.UnescapeName(this.Accessor.Name);
			}
		}

		public string XsdElementName
		{
			get
			{
				return this.Accessor.Name;
			}
		}

		public string Namespace
		{
			get
			{
				return this.Accessor.Namespace;
			}
		}

		public string MemberName
		{
			get
			{
				return this.mapping.Name;
			}
		}

		public string TypeName
		{
			get
			{
				if (this.Accessor.Mapping == null)
				{
					return string.Empty;
				}
				return this.Accessor.Mapping.TypeName;
			}
		}

		public string TypeNamespace
		{
			get
			{
				if (this.Accessor.Mapping == null)
				{
					return null;
				}
				return this.Accessor.Mapping.Namespace;
			}
		}

		public string TypeFullName
		{
			get
			{
				return this.mapping.TypeDesc.FullName;
			}
		}

		public bool CheckSpecified
		{
			get
			{
				return this.mapping.CheckSpecified > SpecifiedAccessor.None;
			}
		}

		internal bool IsNullable
		{
			get
			{
				return this.mapping.IsNeedNullable;
			}
		}

		public string GenerateTypeName(CodeDomProvider codeProvider)
		{
			return this.mapping.GetTypeName(codeProvider);
		}

		private MemberMapping mapping;
	}
}
