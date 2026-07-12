using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;

namespace System.Runtime.Serialization
{
	internal static class SchemaHelper
	{
		internal static bool NamespacesEqual(string ns1, string ns2)
		{
			if (ns1 == null || ns1.Length == 0)
			{
				return ns2 == null || ns2.Length == 0;
			}
			return ns1 == ns2;
		}

		internal static XmlSchemaType GetSchemaType(XmlSchemaSet schemas, XmlQualifiedName typeQName, out XmlSchema outSchema)
		{
			outSchema = null;
			IEnumerable enumerable = schemas.Schemas();
			string @namespace = typeQName.Namespace;
			foreach (object obj in enumerable)
			{
				XmlSchema xmlSchema = (XmlSchema)obj;
				if (SchemaHelper.NamespacesEqual(@namespace, xmlSchema.TargetNamespace))
				{
					outSchema = xmlSchema;
					foreach (XmlSchemaObject xmlSchemaObject in xmlSchema.Items)
					{
						XmlSchemaType xmlSchemaType = xmlSchemaObject as XmlSchemaType;
						if (xmlSchemaType != null && xmlSchemaType.Name == typeQName.Name)
						{
							return xmlSchemaType;
						}
					}
				}
			}
			return null;
		}

		internal static XmlSchemaType GetSchemaType(Dictionary<XmlQualifiedName, SchemaObjectInfo> schemaInfo, XmlQualifiedName typeName)
		{
			SchemaObjectInfo schemaObjectInfo;
			if (schemaInfo.TryGetValue(typeName, out schemaObjectInfo))
			{
				return schemaObjectInfo.type;
			}
			return null;
		}

		internal static XmlSchema GetSchemaWithType(Dictionary<XmlQualifiedName, SchemaObjectInfo> schemaInfo, XmlSchemaSet schemas, XmlQualifiedName typeName)
		{
			SchemaObjectInfo schemaObjectInfo;
			if (schemaInfo.TryGetValue(typeName, out schemaObjectInfo) && schemaObjectInfo.schema != null)
			{
				return schemaObjectInfo.schema;
			}
			IEnumerable enumerable = schemas.Schemas();
			string @namespace = typeName.Namespace;
			foreach (object obj in enumerable)
			{
				XmlSchema xmlSchema = (XmlSchema)obj;
				if (SchemaHelper.NamespacesEqual(@namespace, xmlSchema.TargetNamespace))
				{
					return xmlSchema;
				}
			}
			return null;
		}

		internal static XmlSchemaElement GetSchemaElement(XmlSchemaSet schemas, XmlQualifiedName elementQName, out XmlSchema outSchema)
		{
			outSchema = null;
			IEnumerable enumerable = schemas.Schemas();
			string @namespace = elementQName.Namespace;
			foreach (object obj in enumerable)
			{
				XmlSchema xmlSchema = (XmlSchema)obj;
				if (SchemaHelper.NamespacesEqual(@namespace, xmlSchema.TargetNamespace))
				{
					outSchema = xmlSchema;
					foreach (XmlSchemaObject xmlSchemaObject in xmlSchema.Items)
					{
						XmlSchemaElement xmlSchemaElement = xmlSchemaObject as XmlSchemaElement;
						if (xmlSchemaElement != null && xmlSchemaElement.Name == elementQName.Name)
						{
							return xmlSchemaElement;
						}
					}
				}
			}
			return null;
		}

		internal static XmlSchemaElement GetSchemaElement(Dictionary<XmlQualifiedName, SchemaObjectInfo> schemaInfo, XmlQualifiedName elementName)
		{
			SchemaObjectInfo schemaObjectInfo;
			if (schemaInfo.TryGetValue(elementName, out schemaObjectInfo))
			{
				return schemaObjectInfo.element;
			}
			return null;
		}

		internal static XmlSchema GetSchema(string ns, XmlSchemaSet schemas)
		{
			if (ns == null)
			{
				ns = string.Empty;
			}
			foreach (object obj in schemas.Schemas())
			{
				XmlSchema xmlSchema = (XmlSchema)obj;
				if ((xmlSchema.TargetNamespace == null && ns.Length == 0) || ns.Equals(xmlSchema.TargetNamespace))
				{
					return xmlSchema;
				}
			}
			return SchemaHelper.CreateSchema(ns, schemas);
		}

		private static XmlSchema CreateSchema(string ns, XmlSchemaSet schemas)
		{
			XmlSchema xmlSchema = new XmlSchema();
			xmlSchema.ElementFormDefault = XmlSchemaForm.Qualified;
			if (ns.Length > 0)
			{
				xmlSchema.TargetNamespace = ns;
				xmlSchema.Namespaces.Add("tns", ns);
			}
			schemas.Add(xmlSchema);
			return xmlSchema;
		}

		internal static void AddElementForm(XmlSchemaElement element, XmlSchema schema)
		{
			if (schema.ElementFormDefault != XmlSchemaForm.Qualified)
			{
				element.Form = XmlSchemaForm.Qualified;
			}
		}

		internal static void AddSchemaImport(string ns, XmlSchema schema)
		{
			if (SchemaHelper.NamespacesEqual(ns, schema.TargetNamespace) || SchemaHelper.NamespacesEqual(ns, "http://www.w3.org/2001/XMLSchema") || SchemaHelper.NamespacesEqual(ns, "http://www.w3.org/2001/XMLSchema-instance"))
			{
				return;
			}
			foreach (object obj in schema.Includes)
			{
				if (obj is XmlSchemaImport && SchemaHelper.NamespacesEqual(ns, ((XmlSchemaImport)obj).Namespace))
				{
					return;
				}
			}
			XmlSchemaImport xmlSchemaImport = new XmlSchemaImport();
			if (ns != null && ns.Length > 0)
			{
				xmlSchemaImport.Namespace = ns;
			}
			schema.Includes.Add(xmlSchemaImport);
		}

		internal static XmlSchema GetSchemaWithGlobalElementDeclaration(XmlSchemaElement element, XmlSchemaSet schemas)
		{
			foreach (object obj in schemas.Schemas())
			{
				XmlSchema xmlSchema = (XmlSchema)obj;
				foreach (XmlSchemaObject xmlSchemaObject in xmlSchema.Items)
				{
					XmlSchemaElement xmlSchemaElement = xmlSchemaObject as XmlSchemaElement;
					if (xmlSchemaElement != null && xmlSchemaElement == element)
					{
						return xmlSchema;
					}
				}
			}
			return null;
		}

		internal static XmlQualifiedName GetGlobalElementDeclaration(XmlSchemaSet schemas, XmlQualifiedName typeQName, out bool isNullable)
		{
			IEnumerable enumerable = schemas.Schemas();
			if (typeQName.Namespace == null)
			{
				string empty = string.Empty;
			}
			isNullable = false;
			foreach (object obj in enumerable)
			{
				XmlSchema xmlSchema = (XmlSchema)obj;
				foreach (XmlSchemaObject xmlSchemaObject in xmlSchema.Items)
				{
					XmlSchemaElement xmlSchemaElement = xmlSchemaObject as XmlSchemaElement;
					if (xmlSchemaElement != null && xmlSchemaElement.SchemaTypeName.Equals(typeQName))
					{
						isNullable = xmlSchemaElement.IsNillable;
						return new XmlQualifiedName(xmlSchemaElement.Name, xmlSchema.TargetNamespace);
					}
				}
			}
			return null;
		}
	}
}
