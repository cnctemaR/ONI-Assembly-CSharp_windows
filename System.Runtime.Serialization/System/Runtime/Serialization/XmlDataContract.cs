using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security;
using System.Threading;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Runtime.Serialization
{
	internal sealed class XmlDataContract : DataContract
	{
		[SecuritySafeCritical]
		internal XmlDataContract()
			: base(new XmlDataContract.XmlDataContractCriticalHelper())
		{
			this.helper = base.Helper as XmlDataContract.XmlDataContractCriticalHelper;
		}

		[SecuritySafeCritical]
		internal XmlDataContract(Type type)
			: base(new XmlDataContract.XmlDataContractCriticalHelper(type))
		{
			this.helper = base.Helper as XmlDataContract.XmlDataContractCriticalHelper;
		}

		internal override Dictionary<XmlQualifiedName, DataContract> KnownDataContracts
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.KnownDataContracts;
			}
			[SecurityCritical]
			set
			{
				this.helper.KnownDataContracts = value;
			}
		}

		internal XmlSchemaType XsdType
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.XsdType;
			}
			[SecurityCritical]
			set
			{
				this.helper.XsdType = value;
			}
		}

		internal bool IsAnonymous
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.IsAnonymous;
			}
		}

		internal override bool HasRoot
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.HasRoot;
			}
			[SecurityCritical]
			set
			{
				this.helper.HasRoot = value;
			}
		}

		internal override XmlDictionaryString TopLevelElementName
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.TopLevelElementName;
			}
			[SecurityCritical]
			set
			{
				this.helper.TopLevelElementName = value;
			}
		}

		internal override XmlDictionaryString TopLevelElementNamespace
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.TopLevelElementNamespace;
			}
			[SecurityCritical]
			set
			{
				this.helper.TopLevelElementNamespace = value;
			}
		}

		internal bool IsTopLevelElementNullable
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.IsTopLevelElementNullable;
			}
			[SecurityCritical]
			set
			{
				this.helper.IsTopLevelElementNullable = value;
			}
		}

		internal bool IsTypeDefinedOnImport
		{
			[SecuritySafeCritical]
			get
			{
				return this.helper.IsTypeDefinedOnImport;
			}
			[SecurityCritical]
			set
			{
				this.helper.IsTypeDefinedOnImport = value;
			}
		}

		internal CreateXmlSerializableDelegate CreateXmlSerializableDelegate
		{
			[SecuritySafeCritical]
			get
			{
				if (this.helper.CreateXmlSerializableDelegate == null)
				{
					lock (this)
					{
						if (this.helper.CreateXmlSerializableDelegate == null)
						{
							CreateXmlSerializableDelegate createXmlSerializableDelegate = this.GenerateCreateXmlSerializableDelegate();
							Thread.MemoryBarrier();
							this.helper.CreateXmlSerializableDelegate = createXmlSerializableDelegate;
						}
					}
				}
				return this.helper.CreateXmlSerializableDelegate;
			}
		}

		internal override bool CanContainReferences
		{
			get
			{
				return false;
			}
		}

		internal override bool IsBuiltInDataContract
		{
			get
			{
				return base.UnderlyingType == Globals.TypeOfXmlElement || base.UnderlyingType == Globals.TypeOfXmlNodeArray;
			}
		}

		private ConstructorInfo GetConstructor()
		{
			Type underlyingType = base.UnderlyingType;
			if (underlyingType.IsValueType)
			{
				return null;
			}
			ConstructorInfo constructor = underlyingType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Globals.EmptyTypeArray, null);
			if (constructor == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("IXmlSerializable Type '{0}' must have default constructor.", new object[] { DataContract.GetClrTypeFullName(underlyingType) })));
			}
			return constructor;
		}

		[SecurityCritical]
		internal void SetTopLevelElementName(XmlQualifiedName elementName)
		{
			if (elementName != null)
			{
				XmlDictionary xmlDictionary = new XmlDictionary();
				this.TopLevelElementName = xmlDictionary.Add(elementName.Name);
				this.TopLevelElementNamespace = xmlDictionary.Add(elementName.Namespace);
			}
		}

		internal override bool Equals(object other, Dictionary<DataContractPairKey, object> checkedContracts)
		{
			if (base.IsEqualOrChecked(other, checkedContracts))
			{
				return true;
			}
			XmlDataContract xmlDataContract = other as XmlDataContract;
			if (xmlDataContract == null)
			{
				return false;
			}
			if (this.HasRoot != xmlDataContract.HasRoot)
			{
				return false;
			}
			if (this.IsAnonymous)
			{
				return xmlDataContract.IsAnonymous;
			}
			return base.StableName.Name == xmlDataContract.StableName.Name && base.StableName.Namespace == xmlDataContract.StableName.Namespace;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override void WriteXmlValue(XmlWriterDelegator xmlWriter, object obj, XmlObjectSerializerWriteContext context)
		{
			if (context == null)
			{
				XmlObjectSerializerWriteContext.WriteRootIXmlSerializable(xmlWriter, obj);
				return;
			}
			context.WriteIXmlSerializable(xmlWriter, obj);
		}

		public override object ReadXmlValue(XmlReaderDelegator xmlReader, XmlObjectSerializerReadContext context)
		{
			object obj;
			if (context == null)
			{
				obj = XmlObjectSerializerReadContext.ReadRootIXmlSerializable(xmlReader, this, true);
			}
			else
			{
				obj = context.ReadIXmlSerializable(xmlReader, this, true);
				context.AddNewObject(obj);
			}
			xmlReader.ReadEndElement();
			return obj;
		}

		internal CreateXmlSerializableDelegate GenerateCreateXmlSerializableDelegate()
		{
			return () => new XmlDataContractInterpreter(this).CreateXmlSerializable();
		}

		[SecurityCritical]
		private XmlDataContract.XmlDataContractCriticalHelper helper;

		[SecurityCritical(SecurityCriticalScope.Everything)]
		private class XmlDataContractCriticalHelper : DataContract.DataContractCriticalHelper
		{
			internal XmlDataContractCriticalHelper()
			{
			}

			internal XmlDataContractCriticalHelper(Type type)
				: base(type)
			{
				if (type.IsDefined(Globals.TypeOfDataContractAttribute, false))
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Type '{0}' cannot be IXmlSerializable and have DataContractAttribute attribute.", new object[] { DataContract.GetClrTypeFullName(type) })));
				}
				if (type.IsDefined(Globals.TypeOfCollectionDataContractAttribute, false))
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Type '{0}' cannot be IXmlSerializable and have CollectionDataContractAttribute attribute.", new object[] { DataContract.GetClrTypeFullName(type) })));
				}
				XmlQualifiedName xmlQualifiedName;
				XmlSchemaType xmlSchemaType;
				bool flag;
				SchemaExporter.GetXmlTypeInfo(type, out xmlQualifiedName, out xmlSchemaType, out flag);
				base.StableName = xmlQualifiedName;
				this.XsdType = xmlSchemaType;
				this.HasRoot = flag;
				XmlDictionary xmlDictionary = new XmlDictionary();
				base.Name = xmlDictionary.Add(base.StableName.Name);
				base.Namespace = xmlDictionary.Add(base.StableName.Namespace);
				object[] array = ((base.UnderlyingType == null) ? null : base.UnderlyingType.GetCustomAttributes(Globals.TypeOfXmlRootAttribute, false));
				if (array == null || array.Length == 0)
				{
					if (flag)
					{
						this.topLevelElementName = base.Name;
						this.topLevelElementNamespace = ((base.StableName.Namespace == "http://www.w3.org/2001/XMLSchema") ? DictionaryGlobals.EmptyString : base.Namespace);
						this.isTopLevelElementNullable = true;
						return;
					}
					return;
				}
				else
				{
					if (flag)
					{
						XmlRootAttribute xmlRootAttribute = (XmlRootAttribute)array[0];
						this.isTopLevelElementNullable = xmlRootAttribute.IsNullable;
						string elementName = xmlRootAttribute.ElementName;
						this.topLevelElementName = ((elementName == null || elementName.Length == 0) ? base.Name : xmlDictionary.Add(DataContract.EncodeLocalName(elementName)));
						string @namespace = xmlRootAttribute.Namespace;
						this.topLevelElementNamespace = ((@namespace == null || @namespace.Length == 0) ? DictionaryGlobals.EmptyString : xmlDictionary.Add(@namespace));
						return;
					}
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Type '{0}' cannot specify an XmlRootAttribute attribute because its IsAny setting is 'true'. This type must write all its contents including the root element. Verify that the IXmlSerializable implementation is correct.", new object[] { DataContract.GetClrTypeFullName(base.UnderlyingType) })));
				}
			}

			internal override Dictionary<XmlQualifiedName, DataContract> KnownDataContracts
			{
				get
				{
					if (!this.isKnownTypeAttributeChecked && base.UnderlyingType != null)
					{
						lock (this)
						{
							if (!this.isKnownTypeAttributeChecked)
							{
								this.knownDataContracts = DataContract.ImportKnownTypeAttributes(base.UnderlyingType);
								Thread.MemoryBarrier();
								this.isKnownTypeAttributeChecked = true;
							}
						}
					}
					return this.knownDataContracts;
				}
				set
				{
					this.knownDataContracts = value;
				}
			}

			internal XmlSchemaType XsdType
			{
				get
				{
					return this.xsdType;
				}
				set
				{
					this.xsdType = value;
				}
			}

			internal bool IsAnonymous
			{
				get
				{
					return this.xsdType != null;
				}
			}

			internal override bool HasRoot
			{
				get
				{
					return this.hasRoot;
				}
				set
				{
					this.hasRoot = value;
				}
			}

			internal override XmlDictionaryString TopLevelElementName
			{
				get
				{
					return this.topLevelElementName;
				}
				set
				{
					this.topLevelElementName = value;
				}
			}

			internal override XmlDictionaryString TopLevelElementNamespace
			{
				get
				{
					return this.topLevelElementNamespace;
				}
				set
				{
					this.topLevelElementNamespace = value;
				}
			}

			internal bool IsTopLevelElementNullable
			{
				get
				{
					return this.isTopLevelElementNullable;
				}
				set
				{
					this.isTopLevelElementNullable = value;
				}
			}

			internal bool IsTypeDefinedOnImport
			{
				get
				{
					return this.isTypeDefinedOnImport;
				}
				set
				{
					this.isTypeDefinedOnImport = value;
				}
			}

			internal CreateXmlSerializableDelegate CreateXmlSerializableDelegate
			{
				get
				{
					return this.createXmlSerializable;
				}
				set
				{
					this.createXmlSerializable = value;
				}
			}

			private Dictionary<XmlQualifiedName, DataContract> knownDataContracts;

			private bool isKnownTypeAttributeChecked;

			private XmlDictionaryString topLevelElementName;

			private XmlDictionaryString topLevelElementNamespace;

			private bool isTopLevelElementNullable;

			private bool isTypeDefinedOnImport;

			private XmlSchemaType xsdType;

			private bool hasRoot;

			private CreateXmlSerializableDelegate createXmlSerializable;
		}
	}
}
