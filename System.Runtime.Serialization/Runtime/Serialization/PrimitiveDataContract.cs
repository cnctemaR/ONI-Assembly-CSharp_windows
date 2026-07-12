using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Xml;

namespace System.Runtime.Serialization
{
	internal abstract class PrimitiveDataContract : DataContract
	{
		[SecuritySafeCritical]
		protected PrimitiveDataContract(Type type, XmlDictionaryString name, XmlDictionaryString ns)
			: base(new PrimitiveDataContract.PrimitiveDataContractCriticalHelper(type, name, ns))
		{
			this.helper = base.Helper as PrimitiveDataContract.PrimitiveDataContractCriticalHelper;
		}

		internal static PrimitiveDataContract GetPrimitiveDataContract(Type type)
		{
			return DataContract.GetBuiltInDataContract(type) as PrimitiveDataContract;
		}

		internal static PrimitiveDataContract GetPrimitiveDataContract(string name, string ns)
		{
			return DataContract.GetBuiltInDataContract(name, ns) as PrimitiveDataContract;
		}

		internal abstract string WriteMethodName { get; }

		internal abstract string ReadMethodName { get; }

		internal override XmlDictionaryString TopLevelElementNamespace
		{
			get
			{
				return DictionaryGlobals.SerializationNamespace;
			}
			set
			{
			}
		}

		internal override bool CanContainReferences
		{
			get
			{
				return false;
			}
		}

		internal override bool IsPrimitive
		{
			get
			{
				return true;
			}
		}

		internal override bool IsBuiltInDataContract
		{
			get
			{
				return true;
			}
		}

		internal MethodInfo XmlFormatWriterMethod
		{
			[PreserveDependency("WriteDouble", "System.Runtime.Serialization.XmlWriterDelegator")]
			[PreserveDependency("WriteFloat", "System.Runtime.Serialization.XmlWriterDelegator")]
			[PreserveDependency("WriteBoolean", "System.Runtime.Serialization.XmlWriterDelegator")]
			[PreserveDependency("WriteAnyType", "System.Runtime.Serialization.XmlObjectSerializerWriteContext")]
			[PreserveDependency("WriteBase64", "System.Runtime.Serialization.XmlObjectSerializerWriteContext")]
			[PreserveDependency("WriteString", "System.Runtime.Serialization.XmlObjectSerializerWriteContext")]
			[PreserveDependency("WriteQName", "System.Runtime.Serialization.XmlWriterDelegator")]
			[PreserveDependency("WriteGuid", "System.Runtime.Serialization.XmlWriterDelegator")]
			[PreserveDependency("WriteTimeSpan", "System.Runtime.Serialization.XmlWriterDelegator")]
			[PreserveDependency("WriteUri", "System.Runtime.Serialization.XmlObjectSerializerWriteContext")]
			[PreserveDependency("WriteLong", "System.Runtime.Serialization.XmlWriterDelegator")]
			[PreserveDependency("WriteAnyType", "System.Runtime.Serialization.XmlWriterDelegator")]
			[PreserveDependency("WriteBase64", "System.Runtime.Serialization.XmlWriterDelegator")]
			[PreserveDependency("WriteString", "System.Runtime.Serialization.XmlWriterDelegator")]
			[PreserveDependency("WriteDateTime", "System.Runtime.Serialization.XmlWriterDelegator")]
			[PreserveDependency("WriteDecimal", "System.Runtime.Serialization.XmlWriterDelegator")]
			[PreserveDependency("WriteChar", "System.Runtime.Serialization.XmlWriterDelegator")]
			[PreserveDependency("WriteUnsignedLong", "System.Runtime.Serialization.XmlWriterDelegator")]
			[PreserveDependency("WriteQName", "System.Runtime.Serialization.XmlObjectSerializerWriteContext")]
			[PreserveDependency("WriteUri", "System.Runtime.Serialization.XmlWriterDelegator")]
			[PreserveDependency("WriteInt", "System.Runtime.Serialization.XmlWriterDelegator")]
			[PreserveDependency("WriteUnsignedInt", "System.Runtime.Serialization.XmlWriterDelegator")]
			[PreserveDependency("WriteShort", "System.Runtime.Serialization.XmlWriterDelegator")]
			[PreserveDependency("WriteUnsignedByte", "System.Runtime.Serialization.XmlWriterDelegator")]
			[PreserveDependency("WriteSignedByte", "System.Runtime.Serialization.XmlWriterDelegator")]
			[SecuritySafeCritical]
			[PreserveDependency("WriteUnsignedShort", "System.Runtime.Serialization.XmlWriterDelegator")]
			get
			{
				if (this.helper.XmlFormatWriterMethod == null)
				{
					if (base.UnderlyingType.IsValueType)
					{
						this.helper.XmlFormatWriterMethod = typeof(XmlWriterDelegator).GetMethod(this.WriteMethodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[]
						{
							base.UnderlyingType,
							typeof(XmlDictionaryString),
							typeof(XmlDictionaryString)
						}, null);
					}
					else
					{
						this.helper.XmlFormatWriterMethod = typeof(XmlObjectSerializerWriteContext).GetMethod(this.WriteMethodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[]
						{
							typeof(XmlWriterDelegator),
							base.UnderlyingType,
							typeof(XmlDictionaryString),
							typeof(XmlDictionaryString)
						}, null);
					}
				}
				return this.helper.XmlFormatWriterMethod;
			}
		}

		internal MethodInfo XmlFormatContentWriterMethod
		{
			[PreserveDependency("WriteTimeSpan", "System.Runtime.Serialization.XmlWriterDelegator")]
			[PreserveDependency("WriteUri", "System.Runtime.Serialization.XmlWriterDelegator")]
			[PreserveDependency("WriteChar", "System.Runtime.Serialization.XmlWriterDelegator")]
			[PreserveDependency("WriteQName", "System.Runtime.Serialization.XmlWriterDelegator")]
			[PreserveDependency("WriteBase64", "System.Runtime.Serialization.XmlWriterDelegator")]
			[PreserveDependency("WriteString", "System.Runtime.Serialization.XmlObjectSerializerWriteContext")]
			[PreserveDependency("WriteBase64", "System.Runtime.Serialization.XmlObjectSerializerWriteContext")]
			[PreserveDependency("WriteAnyType", "System.Runtime.Serialization.XmlObjectSerializerWriteContext")]
			[PreserveDependency("WriteGuid", "System.Runtime.Serialization.XmlWriterDelegator")]
			[PreserveDependency("WriteAnyType", "System.Runtime.Serialization.XmlWriterDelegator")]
			[PreserveDependency("WriteDouble", "System.Runtime.Serialization.XmlWriterDelegator")]
			[PreserveDependency("WriteDateTime", "System.Runtime.Serialization.XmlWriterDelegator")]
			[PreserveDependency("WriteDecimal", "System.Runtime.Serialization.XmlWriterDelegator")]
			[PreserveDependency("WriteSignedByte", "System.Runtime.Serialization.XmlWriterDelegator")]
			[PreserveDependency("WriteUnsignedByte", "System.Runtime.Serialization.XmlWriterDelegator")]
			[PreserveDependency("WriteShort", "System.Runtime.Serialization.XmlWriterDelegator")]
			[PreserveDependency("WriteUnsignedShort", "System.Runtime.Serialization.XmlWriterDelegator")]
			[PreserveDependency("WriteUri", "System.Runtime.Serialization.XmlObjectSerializerWriteContext")]
			[PreserveDependency("WriteInt", "System.Runtime.Serialization.XmlWriterDelegator")]
			[PreserveDependency("WriteUnsignedInt", "System.Runtime.Serialization.XmlWriterDelegator")]
			[PreserveDependency("WriteLong", "System.Runtime.Serialization.XmlWriterDelegator")]
			[PreserveDependency("WriteUnsignedLong", "System.Runtime.Serialization.XmlWriterDelegator")]
			[PreserveDependency("WriteFloat", "System.Runtime.Serialization.XmlWriterDelegator")]
			[PreserveDependency("WriteString", "System.Runtime.Serialization.XmlWriterDelegator")]
			[PreserveDependency("WriteQName", "System.Runtime.Serialization.XmlObjectSerializerWriteContext")]
			[PreserveDependency("WriteBoolean", "System.Runtime.Serialization.XmlWriterDelegator")]
			[SecuritySafeCritical]
			get
			{
				if (this.helper.XmlFormatContentWriterMethod == null)
				{
					if (base.UnderlyingType.IsValueType)
					{
						this.helper.XmlFormatContentWriterMethod = typeof(XmlWriterDelegator).GetMethod(this.WriteMethodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] { base.UnderlyingType }, null);
					}
					else
					{
						this.helper.XmlFormatContentWriterMethod = typeof(XmlObjectSerializerWriteContext).GetMethod(this.WriteMethodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[]
						{
							typeof(XmlWriterDelegator),
							base.UnderlyingType
						}, null);
					}
				}
				return this.helper.XmlFormatContentWriterMethod;
			}
		}

		internal MethodInfo XmlFormatReaderMethod
		{
			[PreserveDependency("ReadElementContentAsChar", "System.Runtime.Serialization.XmlReaderDelegator")]
			[PreserveDependency("ReadElementContentAsDecimal", "System.Runtime.Serialization.XmlReaderDelegator")]
			[PreserveDependency("ReadElementContentAsDateTime", "System.Runtime.Serialization.XmlReaderDelegator")]
			[PreserveDependency("ReadElementContentAsString", "System.Runtime.Serialization.XmlReaderDelegator")]
			[PreserveDependency("ReadElementContentAsBase64", "System.Runtime.Serialization.XmlReaderDelegator")]
			[PreserveDependency("ReadElementContentAsAnyType", "System.Runtime.Serialization.XmlReaderDelegator")]
			[PreserveDependency("ReadElementContentAsTimeSpan", "System.Runtime.Serialization.XmlReaderDelegator")]
			[PreserveDependency("ReadElementContentAsGuid", "System.Runtime.Serialization.XmlReaderDelegator")]
			[PreserveDependency("ReadElementContentAsUri", "System.Runtime.Serialization.XmlReaderDelegator")]
			[PreserveDependency("ReadElementContentAsQName", "System.Runtime.Serialization.XmlReaderDelegator")]
			[SecuritySafeCritical]
			[PreserveDependency("ReadElementContentAsFloat", "System.Runtime.Serialization.XmlReaderDelegator")]
			[PreserveDependency("ReadElementContentAsDouble", "System.Runtime.Serialization.XmlReaderDelegator")]
			[PreserveDependency("ReadElementContentAsUnsignedLong", "System.Runtime.Serialization.XmlReaderDelegator")]
			[PreserveDependency("ReadElementContentAsLong", "System.Runtime.Serialization.XmlReaderDelegator")]
			[PreserveDependency("ReadElementContentAsUnsignedInt", "System.Runtime.Serialization.XmlReaderDelegator")]
			[PreserveDependency("ReadElementContentAsInt", "System.Runtime.Serialization.XmlReaderDelegator")]
			[PreserveDependency("ReadElementContentAsUnsignedShort", "System.Runtime.Serialization.XmlReaderDelegator")]
			[PreserveDependency("ReadElementContentAsShort", "System.Runtime.Serialization.XmlReaderDelegator")]
			[PreserveDependency("ReadElementContentAsUnsignedByte", "System.Runtime.Serialization.XmlReaderDelegator")]
			[PreserveDependency("ReadElementContentAsSignedByte", "System.Runtime.Serialization.XmlReaderDelegator")]
			[PreserveDependency("ReadElementContentAsBoolean", "System.Runtime.Serialization.XmlReaderDelegator")]
			get
			{
				if (this.helper.XmlFormatReaderMethod == null)
				{
					this.helper.XmlFormatReaderMethod = typeof(XmlReaderDelegator).GetMethod(this.ReadMethodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				}
				return this.helper.XmlFormatReaderMethod;
			}
		}

		public override void WriteXmlValue(XmlWriterDelegator xmlWriter, object obj, XmlObjectSerializerWriteContext context)
		{
			xmlWriter.WriteAnyType(obj);
		}

		protected object HandleReadValue(object obj, XmlObjectSerializerReadContext context)
		{
			context.AddNewObject(obj);
			return obj;
		}

		protected bool TryReadNullAtTopLevel(XmlReaderDelegator reader)
		{
			Attributes attributes = new Attributes();
			attributes.Read(reader);
			if (attributes.Ref != Globals.NewObjectId)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(global::System.Runtime.Serialization.SR.GetString("Cannot deserialize since root element references unrecognized object with id '{0}'.", new object[] { attributes.Ref })));
			}
			if (attributes.XsiNil)
			{
				reader.Skip();
				return true;
			}
			return false;
		}

		internal override bool Equals(object other, Dictionary<DataContractPairKey, object> checkedContracts)
		{
			if (other is PrimitiveDataContract)
			{
				Type type = base.GetType();
				Type type2 = other.GetType();
				return type.Equals(type2) || type.IsSubclassOf(type2) || type2.IsSubclassOf(type);
			}
			return false;
		}

		[SecurityCritical]
		private PrimitiveDataContract.PrimitiveDataContractCriticalHelper helper;

		[SecurityCritical(SecurityCriticalScope.Everything)]
		private class PrimitiveDataContractCriticalHelper : DataContract.DataContractCriticalHelper
		{
			internal PrimitiveDataContractCriticalHelper(Type type, XmlDictionaryString name, XmlDictionaryString ns)
				: base(type)
			{
				base.SetDataContractName(name, ns);
			}

			internal MethodInfo XmlFormatWriterMethod
			{
				get
				{
					return this.xmlFormatWriterMethod;
				}
				set
				{
					this.xmlFormatWriterMethod = value;
				}
			}

			internal MethodInfo XmlFormatContentWriterMethod
			{
				get
				{
					return this.xmlFormatContentWriterMethod;
				}
				set
				{
					this.xmlFormatContentWriterMethod = value;
				}
			}

			internal MethodInfo XmlFormatReaderMethod
			{
				get
				{
					return this.xmlFormatReaderMethod;
				}
				set
				{
					this.xmlFormatReaderMethod = value;
				}
			}

			private MethodInfo xmlFormatWriterMethod;

			private MethodInfo xmlFormatContentWriterMethod;

			private MethodInfo xmlFormatReaderMethod;
		}
	}
}
