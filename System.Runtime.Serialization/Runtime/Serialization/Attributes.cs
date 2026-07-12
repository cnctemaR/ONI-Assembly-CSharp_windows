using System;
using System.Security;
using System.Xml;

namespace System.Runtime.Serialization
{
	internal class Attributes
	{
		[SecuritySafeCritical]
		internal void Read(XmlReaderDelegator reader)
		{
			this.Reset();
			while (reader.MoveToNextAttribute())
			{
				switch (reader.IndexOfLocalName(Attributes.serializationLocalNames, DictionaryGlobals.SerializationNamespace))
				{
				case 0:
					this.ReadId(reader);
					break;
				case 1:
					this.ReadArraySize(reader);
					break;
				case 2:
					this.ReadRef(reader);
					break;
				case 3:
					this.ClrType = reader.Value;
					break;
				case 4:
					this.ClrAssembly = reader.Value;
					break;
				case 5:
					this.ReadFactoryType(reader);
					break;
				default:
				{
					int num = reader.IndexOfLocalName(Attributes.schemaInstanceLocalNames, DictionaryGlobals.SchemaInstanceNamespace);
					if (num != 0)
					{
						if (num != 1)
						{
							if (!reader.IsNamespaceUri(DictionaryGlobals.XmlnsNamespace))
							{
								this.UnrecognizedAttributesFound = true;
							}
						}
						else
						{
							this.ReadXsiType(reader);
						}
					}
					else
					{
						this.ReadXsiNil(reader);
					}
					break;
				}
				}
			}
			reader.MoveToElement();
		}

		internal void Reset()
		{
			this.Id = Globals.NewObjectId;
			this.Ref = Globals.NewObjectId;
			this.XsiTypeName = null;
			this.XsiTypeNamespace = null;
			this.XsiTypePrefix = null;
			this.XsiNil = false;
			this.ClrAssembly = null;
			this.ClrType = null;
			this.ArraySZSize = -1;
			this.FactoryTypeName = null;
			this.FactoryTypeNamespace = null;
			this.FactoryTypePrefix = null;
			this.UnrecognizedAttributesFound = false;
		}

		private void ReadId(XmlReaderDelegator reader)
		{
			this.Id = reader.ReadContentAsString();
			if (string.IsNullOrEmpty(this.Id))
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("Invalid Id '{0}'. Must not be null or empty.", new object[] { this.Id })));
			}
		}

		private void ReadRef(XmlReaderDelegator reader)
		{
			this.Ref = reader.ReadContentAsString();
			if (string.IsNullOrEmpty(this.Ref))
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("Invalid Ref '{0}'. Must not be null or empty.", new object[] { this.Ref })));
			}
		}

		private void ReadXsiNil(XmlReaderDelegator reader)
		{
			this.XsiNil = reader.ReadContentAsBoolean();
		}

		private void ReadArraySize(XmlReaderDelegator reader)
		{
			this.ArraySZSize = reader.ReadContentAsInt();
			if (this.ArraySZSize < 0)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("Invalid Size '{0}'. Must be non-negative integer.", new object[] { this.ArraySZSize })));
			}
		}

		private void ReadXsiType(XmlReaderDelegator reader)
		{
			string value = reader.Value;
			if (value != null && value.Length > 0)
			{
				XmlObjectSerializerReadContext.ParseQualifiedName(value, reader, out this.XsiTypeName, out this.XsiTypeNamespace, out this.XsiTypePrefix);
			}
		}

		private void ReadFactoryType(XmlReaderDelegator reader)
		{
			string value = reader.Value;
			if (value != null && value.Length > 0)
			{
				XmlObjectSerializerReadContext.ParseQualifiedName(value, reader, out this.FactoryTypeName, out this.FactoryTypeNamespace, out this.FactoryTypePrefix);
			}
		}

		[SecurityCritical]
		private static XmlDictionaryString[] serializationLocalNames = new XmlDictionaryString[]
		{
			DictionaryGlobals.IdLocalName,
			DictionaryGlobals.ArraySizeLocalName,
			DictionaryGlobals.RefLocalName,
			DictionaryGlobals.ClrTypeLocalName,
			DictionaryGlobals.ClrAssemblyLocalName,
			DictionaryGlobals.ISerializableFactoryTypeLocalName
		};

		[SecurityCritical]
		private static XmlDictionaryString[] schemaInstanceLocalNames = new XmlDictionaryString[]
		{
			DictionaryGlobals.XsiNilLocalName,
			DictionaryGlobals.XsiTypeLocalName
		};

		internal string Id;

		internal string Ref;

		internal string XsiTypeName;

		internal string XsiTypeNamespace;

		internal string XsiTypePrefix;

		internal bool XsiNil;

		internal string ClrAssembly;

		internal string ClrType;

		internal int ArraySZSize;

		internal string FactoryTypeName;

		internal string FactoryTypeNamespace;

		internal string FactoryTypePrefix;

		internal bool UnrecognizedAttributesFound;
	}
}
