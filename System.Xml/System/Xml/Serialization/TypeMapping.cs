using System;

namespace System.Xml.Serialization
{
	internal abstract class TypeMapping : Mapping
	{
		internal bool ReferencedByTopLevelElement
		{
			get
			{
				return this.referencedByTopLevelElement;
			}
			set
			{
				this.referencedByTopLevelElement = value;
			}
		}

		internal bool ReferencedByElement
		{
			get
			{
				return this.referencedByElement || this.referencedByTopLevelElement;
			}
			set
			{
				this.referencedByElement = value;
			}
		}

		internal string Namespace
		{
			get
			{
				return this.typeNs;
			}
			set
			{
				this.typeNs = value;
			}
		}

		internal string TypeName
		{
			get
			{
				return this.typeName;
			}
			set
			{
				this.typeName = value;
			}
		}

		internal TypeDesc TypeDesc
		{
			get
			{
				return this.typeDesc;
			}
			set
			{
				this.typeDesc = value;
			}
		}

		internal bool IncludeInSchema
		{
			get
			{
				return this.includeInSchema;
			}
			set
			{
				this.includeInSchema = value;
			}
		}

		internal virtual bool IsList
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		internal bool IsReference
		{
			get
			{
				return this.reference;
			}
			set
			{
				this.reference = value;
			}
		}

		internal bool IsAnonymousType
		{
			get
			{
				return this.typeName == null || this.typeName.Length == 0;
			}
		}

		internal virtual string DefaultElementName
		{
			get
			{
				if (!this.IsAnonymousType)
				{
					return this.typeName;
				}
				return XmlConvert.EncodeLocalName(this.typeDesc.Name);
			}
		}

		private TypeDesc typeDesc;

		private string typeNs;

		private string typeName;

		private bool referencedByElement;

		private bool referencedByTopLevelElement;

		private bool includeInSchema = true;

		private bool reference;
	}
}
