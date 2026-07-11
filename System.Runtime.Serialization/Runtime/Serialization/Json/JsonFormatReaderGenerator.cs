using System;
using System.Runtime.Serialization.Diagnostics.Application;
using System.Security;
using System.Xml;

namespace System.Runtime.Serialization.Json
{
	internal sealed class JsonFormatReaderGenerator
	{
		[SecurityCritical]
		public JsonFormatReaderGenerator()
		{
			this.helper = new JsonFormatReaderGenerator.CriticalHelper();
		}

		[SecurityCritical]
		public JsonFormatClassReaderDelegate GenerateClassReader(ClassDataContract classContract)
		{
			JsonFormatClassReaderDelegate jsonFormatClassReaderDelegate;
			try
			{
				if (TD.DCJsonGenReaderStartIsEnabled())
				{
					TD.DCJsonGenReaderStart("Class", classContract.UnderlyingType.FullName);
				}
				jsonFormatClassReaderDelegate = this.helper.GenerateClassReader(classContract);
			}
			finally
			{
				if (TD.DCJsonGenReaderStopIsEnabled())
				{
					TD.DCJsonGenReaderStop();
				}
			}
			return jsonFormatClassReaderDelegate;
		}

		[SecurityCritical]
		public JsonFormatCollectionReaderDelegate GenerateCollectionReader(CollectionDataContract collectionContract)
		{
			JsonFormatCollectionReaderDelegate jsonFormatCollectionReaderDelegate;
			try
			{
				if (TD.DCJsonGenReaderStartIsEnabled())
				{
					TD.DCJsonGenReaderStart("Collection", collectionContract.StableName.Name);
				}
				jsonFormatCollectionReaderDelegate = this.helper.GenerateCollectionReader(collectionContract);
			}
			finally
			{
				if (TD.DCJsonGenReaderStopIsEnabled())
				{
					TD.DCJsonGenReaderStop();
				}
			}
			return jsonFormatCollectionReaderDelegate;
		}

		[SecurityCritical]
		public JsonFormatGetOnlyCollectionReaderDelegate GenerateGetOnlyCollectionReader(CollectionDataContract collectionContract)
		{
			JsonFormatGetOnlyCollectionReaderDelegate jsonFormatGetOnlyCollectionReaderDelegate;
			try
			{
				if (TD.DCJsonGenReaderStartIsEnabled())
				{
					TD.DCJsonGenReaderStart("GetOnlyCollection", collectionContract.UnderlyingType.FullName);
				}
				jsonFormatGetOnlyCollectionReaderDelegate = this.helper.GenerateGetOnlyCollectionReader(collectionContract);
			}
			finally
			{
				if (TD.DCJsonGenReaderStopIsEnabled())
				{
					TD.DCJsonGenReaderStop();
				}
			}
			return jsonFormatGetOnlyCollectionReaderDelegate;
		}

		[SecurityCritical]
		private JsonFormatReaderGenerator.CriticalHelper helper;

		private class CriticalHelper
		{
			internal JsonFormatClassReaderDelegate GenerateClassReader(ClassDataContract classContract)
			{
				return (XmlReaderDelegator xr, XmlObjectSerializerReadContextComplexJson ctx, XmlDictionaryString emptyDictionaryString, XmlDictionaryString[] memberNames) => new JsonFormatReaderInterpreter(classContract).ReadFromJson(xr, ctx, emptyDictionaryString, memberNames);
			}

			internal JsonFormatCollectionReaderDelegate GenerateCollectionReader(CollectionDataContract collectionContract)
			{
				return (XmlReaderDelegator xr, XmlObjectSerializerReadContextComplexJson ctx, XmlDictionaryString emptyDS, XmlDictionaryString inm, CollectionDataContract cc) => new JsonFormatReaderInterpreter(collectionContract, false).ReadCollectionFromJson(xr, ctx, emptyDS, inm, cc);
			}

			internal JsonFormatGetOnlyCollectionReaderDelegate GenerateGetOnlyCollectionReader(CollectionDataContract collectionContract)
			{
				return delegate(XmlReaderDelegator xr, XmlObjectSerializerReadContextComplexJson ctx, XmlDictionaryString emptyDS, XmlDictionaryString inm, CollectionDataContract cc)
				{
					new JsonFormatReaderInterpreter(collectionContract, true).ReadGetOnlyCollectionFromJson(xr, ctx, emptyDS, inm, cc);
				};
			}
		}
	}
}
