using System;
using System.Collections.Generic;
using System.Reflection;
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
			[SecuritySafeCritical]
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
			[SecuritySafeCritical]
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
