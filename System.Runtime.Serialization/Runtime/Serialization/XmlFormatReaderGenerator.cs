using System;
using System.Runtime.Serialization.Diagnostics.Application;
using System.Security;
using System.Xml;

namespace System.Runtime.Serialization
{
	internal sealed class XmlFormatReaderGenerator
	{
		[SecurityCritical]
		public XmlFormatReaderGenerator()
		{
			this.helper = new XmlFormatReaderGenerator.CriticalHelper();
		}

		[SecurityCritical]
		public XmlFormatClassReaderDelegate GenerateClassReader(ClassDataContract classContract)
		{
			XmlFormatClassReaderDelegate xmlFormatClassReaderDelegate;
			try
			{
				if (TD.DCGenReaderStartIsEnabled())
				{
					TD.DCGenReaderStart("Class", classContract.UnderlyingType.FullName);
				}
				xmlFormatClassReaderDelegate = this.helper.GenerateClassReader(classContract);
			}
			finally
			{
				if (TD.DCGenReaderStopIsEnabled())
				{
					TD.DCGenReaderStop();
				}
			}
			return xmlFormatClassReaderDelegate;
		}

		[SecurityCritical]
		public XmlFormatCollectionReaderDelegate GenerateCollectionReader(CollectionDataContract collectionContract)
		{
			XmlFormatCollectionReaderDelegate xmlFormatCollectionReaderDelegate;
			try
			{
				if (TD.DCGenReaderStartIsEnabled())
				{
					TD.DCGenReaderStart("Collection", collectionContract.UnderlyingType.FullName);
				}
				xmlFormatCollectionReaderDelegate = this.helper.GenerateCollectionReader(collectionContract);
			}
			finally
			{
				if (TD.DCGenReaderStopIsEnabled())
				{
					TD.DCGenReaderStop();
				}
			}
			return xmlFormatCollectionReaderDelegate;
		}

		[SecurityCritical]
		public XmlFormatGetOnlyCollectionReaderDelegate GenerateGetOnlyCollectionReader(CollectionDataContract collectionContract)
		{
			XmlFormatGetOnlyCollectionReaderDelegate xmlFormatGetOnlyCollectionReaderDelegate;
			try
			{
				if (TD.DCGenReaderStartIsEnabled())
				{
					TD.DCGenReaderStart("GetOnlyCollection", collectionContract.UnderlyingType.FullName);
				}
				xmlFormatGetOnlyCollectionReaderDelegate = this.helper.GenerateGetOnlyCollectionReader(collectionContract);
			}
			finally
			{
				if (TD.DCGenReaderStopIsEnabled())
				{
					TD.DCGenReaderStop();
				}
			}
			return xmlFormatGetOnlyCollectionReaderDelegate;
		}

		[SecuritySafeCritical]
		internal static object UnsafeGetUninitializedObject(int id)
		{
			return FormatterServices.GetUninitializedObject(DataContract.GetDataContractForInitialization(id).TypeForInitialization);
		}

		[SecurityCritical]
		private XmlFormatReaderGenerator.CriticalHelper helper;

		private class CriticalHelper
		{
			internal XmlFormatClassReaderDelegate GenerateClassReader(ClassDataContract classContract)
			{
				return (XmlReaderDelegator xr, XmlObjectSerializerReadContext ctx, XmlDictionaryString[] memberNames, XmlDictionaryString[] memberNamespaces) => new XmlFormatReaderInterpreter(classContract).ReadFromXml(xr, ctx, memberNames, memberNamespaces);
			}

			internal XmlFormatCollectionReaderDelegate GenerateCollectionReader(CollectionDataContract collectionContract)
			{
				return (XmlReaderDelegator xr, XmlObjectSerializerReadContext ctx, XmlDictionaryString inm, XmlDictionaryString ins, CollectionDataContract cc) => new XmlFormatReaderInterpreter(collectionContract, false).ReadCollectionFromXml(xr, ctx, inm, ins, cc);
			}

			internal XmlFormatGetOnlyCollectionReaderDelegate GenerateGetOnlyCollectionReader(CollectionDataContract collectionContract)
			{
				return delegate(XmlReaderDelegator xr, XmlObjectSerializerReadContext ctx, XmlDictionaryString inm, XmlDictionaryString ins, CollectionDataContract cc)
				{
					new XmlFormatReaderInterpreter(collectionContract, true).ReadGetOnlyCollectionFromXml(xr, ctx, inm, ins, cc);
				};
			}
		}
	}
}
