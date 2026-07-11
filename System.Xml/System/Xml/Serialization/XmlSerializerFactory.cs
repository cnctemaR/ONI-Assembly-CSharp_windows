using System;
using System.Security.Permissions;
using System.Security.Policy;

namespace System.Xml.Serialization
{
	public class XmlSerializerFactory
	{
		public XmlSerializer CreateSerializer(Type type, XmlAttributeOverrides overrides, Type[] extraTypes, XmlRootAttribute root, string defaultNamespace)
		{
			return this.CreateSerializer(type, overrides, extraTypes, root, defaultNamespace, null);
		}

		public XmlSerializer CreateSerializer(Type type, XmlRootAttribute root)
		{
			return this.CreateSerializer(type, null, new Type[0], root, null, null);
		}

		public XmlSerializer CreateSerializer(Type type, Type[] extraTypes)
		{
			return this.CreateSerializer(type, null, extraTypes, null, null, null);
		}

		public XmlSerializer CreateSerializer(Type type, XmlAttributeOverrides overrides)
		{
			return this.CreateSerializer(type, overrides, new Type[0], null, null, null);
		}

		public XmlSerializer CreateSerializer(XmlTypeMapping xmlTypeMapping)
		{
			return (XmlSerializer)XmlSerializer.GenerateTempAssembly(xmlTypeMapping).Contract.TypedSerializers[xmlTypeMapping.Key];
		}

		public XmlSerializer CreateSerializer(Type type)
		{
			return this.CreateSerializer(type, null);
		}

		public XmlSerializer CreateSerializer(Type type, string defaultNamespace)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			TempAssembly tempAssembly = XmlSerializerFactory.cache[defaultNamespace, type];
			XmlTypeMapping xmlTypeMapping = null;
			if (tempAssembly == null)
			{
				TempAssemblyCache tempAssemblyCache = XmlSerializerFactory.cache;
				lock (tempAssemblyCache)
				{
					tempAssembly = XmlSerializerFactory.cache[defaultNamespace, type];
					if (tempAssembly == null)
					{
						XmlSerializerImplementation xmlSerializerImplementation;
						if (TempAssembly.LoadGeneratedAssembly(type, defaultNamespace, out xmlSerializerImplementation) == null)
						{
							xmlTypeMapping = new XmlReflectionImporter(defaultNamespace).ImportTypeMapping(type, null, defaultNamespace);
							tempAssembly = XmlSerializer.GenerateTempAssembly(xmlTypeMapping, type, defaultNamespace);
						}
						else
						{
							tempAssembly = new TempAssembly(xmlSerializerImplementation);
						}
						XmlSerializerFactory.cache.Add(defaultNamespace, type, tempAssembly);
					}
				}
			}
			if (xmlTypeMapping == null)
			{
				xmlTypeMapping = XmlReflectionImporter.GetTopLevelMapping(type, defaultNamespace);
			}
			return tempAssembly.Contract.GetSerializer(type);
		}

		public XmlSerializer CreateSerializer(Type type, XmlAttributeOverrides overrides, Type[] extraTypes, XmlRootAttribute root, string defaultNamespace, string location)
		{
			return this.CreateSerializer(type, overrides, extraTypes, root, defaultNamespace, location, null);
		}

		[Obsolete("This method is obsolete and will be removed in a future release of the .NET Framework. Please use an overload of CreateSerializer which does not take an Evidence parameter. See http://go2.microsoft.com/fwlink/?LinkId=131738 for more information.")]
		public XmlSerializer CreateSerializer(Type type, XmlAttributeOverrides overrides, Type[] extraTypes, XmlRootAttribute root, string defaultNamespace, string location, Evidence evidence)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (location != null || evidence != null)
			{
				this.DemandForUserLocationOrEvidence();
			}
			XmlReflectionImporter xmlReflectionImporter = new XmlReflectionImporter(overrides, defaultNamespace);
			for (int i = 0; i < extraTypes.Length; i++)
			{
				xmlReflectionImporter.IncludeType(extraTypes[i]);
			}
			XmlTypeMapping xmlTypeMapping = xmlReflectionImporter.ImportTypeMapping(type, root, defaultNamespace);
			return (XmlSerializer)XmlSerializer.GenerateTempAssembly(xmlTypeMapping, type, defaultNamespace, location, evidence).Contract.TypedSerializers[xmlTypeMapping.Key];
		}

		[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
		private void DemandForUserLocationOrEvidence()
		{
		}

		private static TempAssemblyCache cache = new TempAssemblyCache();
	}
}
