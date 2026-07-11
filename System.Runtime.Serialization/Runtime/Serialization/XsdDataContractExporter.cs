using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;

namespace System.Runtime.Serialization
{
	public class XsdDataContractExporter
	{
		public XsdDataContractExporter()
		{
		}

		public XsdDataContractExporter(XmlSchemaSet schemas)
		{
			this.schemas = schemas;
		}

		public XmlSchemaSet Schemas
		{
			get
			{
				if (this.schemas == null)
				{
					this.schemas = new XmlSchemaSet();
					this.schemas.Add(XsdDataContractExporter.MSTypesSchema);
				}
				return this.schemas;
			}
		}

		public ExportOptions Options
		{
			get
			{
				return this.options;
			}
			set
			{
				this.options = value;
			}
		}

		public bool CanExport(ICollection<Type> types)
		{
			foreach (Type type in types)
			{
				if (!this.CanExport(type))
				{
					return false;
				}
			}
			return true;
		}

		public bool CanExport(ICollection<Assembly> assemblies)
		{
			foreach (Assembly assembly in assemblies)
			{
				foreach (Module module in assembly.GetModules())
				{
					foreach (Type type in module.GetTypes())
					{
						if (!this.CanExport(type))
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		public bool CanExport(Type type)
		{
			return !this.KnownTypes.GetQName(type).IsEmpty;
		}

		public void Export(ICollection<Type> types)
		{
			foreach (Type type in types)
			{
				this.Export(type);
			}
		}

		public void Export(ICollection<Assembly> assemblies)
		{
			foreach (Assembly assembly in assemblies)
			{
				foreach (Module module in assembly.GetModules())
				{
					foreach (Type type in module.GetTypes())
					{
						this.Export(type);
					}
				}
			}
		}

		[MonoTODO]
		public void Export(Type type)
		{
			this.KnownTypes.Add(type);
			SerializationMap serializationMap = this.KnownTypes.FindUserMap(type);
			if (serializationMap == null)
			{
				return;
			}
			serializationMap.GetSchemaType(this.Schemas, this.GeneratedTypes);
			this.Schemas.Compile();
		}

		[MonoTODO]
		public XmlQualifiedName GetRootElementName(Type type)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public XmlSchemaType GetSchemaType(Type type)
		{
			SerializationMap serializationMap = this.KnownTypes.FindUserMap(type);
			if (serializationMap == null)
			{
				return null;
			}
			return serializationMap.GetSchemaType(this.Schemas, this.GeneratedTypes);
		}

		public XmlQualifiedName GetSchemaTypeName(Type type)
		{
			XmlQualifiedName qname = this.KnownTypes.GetQName(type);
			if (qname.Namespace == "http://schemas.microsoft.com/2003/10/Serialization/")
			{
				return new XmlQualifiedName(qname.Name, "http://www.w3.org/2001/XMLSchema");
			}
			return qname;
		}

		private KnownTypeCollection KnownTypes
		{
			get
			{
				if (this.known_types == null)
				{
					this.known_types = new KnownTypeCollection();
				}
				return this.known_types;
			}
		}

		private Dictionary<XmlQualifiedName, XmlSchemaType> GeneratedTypes
		{
			get
			{
				if (this.generated_schema_types == null)
				{
					this.generated_schema_types = new Dictionary<XmlQualifiedName, XmlSchemaType>();
				}
				return this.generated_schema_types;
			}
		}

		private static XmlSchema MSTypesSchema
		{
			get
			{
				if (XsdDataContractExporter.mstypes_schema == null)
				{
					Assembly callingAssembly = Assembly.GetCallingAssembly();
					Stream manifestResourceStream = callingAssembly.GetManifestResourceStream("mstypes.schema");
					XsdDataContractExporter.mstypes_schema = XmlSchema.Read(manifestResourceStream, null);
				}
				return XsdDataContractExporter.mstypes_schema;
			}
		}

		private ExportOptions options;

		private KnownTypeCollection known_types;

		private XmlSchemaSet schemas;

		private Dictionary<XmlQualifiedName, XmlSchemaType> generated_schema_types;

		private static XmlSchema mstypes_schema;
	}
}
