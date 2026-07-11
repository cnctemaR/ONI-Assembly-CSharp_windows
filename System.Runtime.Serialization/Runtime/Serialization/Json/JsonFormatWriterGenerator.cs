using System;
using System.Runtime.Serialization.Diagnostics.Application;
using System.Security;
using System.Xml;

namespace System.Runtime.Serialization.Json
{
	internal class JsonFormatWriterGenerator
	{
		[SecurityCritical]
		public JsonFormatWriterGenerator()
		{
			this.helper = new JsonFormatWriterGenerator.CriticalHelper();
		}

		[SecurityCritical]
		internal JsonFormatClassWriterDelegate GenerateClassWriter(ClassDataContract classContract)
		{
			JsonFormatClassWriterDelegate jsonFormatClassWriterDelegate;
			try
			{
				if (TD.DCJsonGenWriterStartIsEnabled())
				{
					TD.DCJsonGenWriterStart("Class", classContract.UnderlyingType.FullName);
				}
				jsonFormatClassWriterDelegate = this.helper.GenerateClassWriter(classContract);
			}
			finally
			{
				if (TD.DCJsonGenWriterStopIsEnabled())
				{
					TD.DCJsonGenWriterStop();
				}
			}
			return jsonFormatClassWriterDelegate;
		}

		[SecurityCritical]
		internal JsonFormatCollectionWriterDelegate GenerateCollectionWriter(CollectionDataContract collectionContract)
		{
			JsonFormatCollectionWriterDelegate jsonFormatCollectionWriterDelegate;
			try
			{
				if (TD.DCJsonGenWriterStartIsEnabled())
				{
					TD.DCJsonGenWriterStart("Collection", collectionContract.UnderlyingType.FullName);
				}
				jsonFormatCollectionWriterDelegate = this.helper.GenerateCollectionWriter(collectionContract);
			}
			finally
			{
				if (TD.DCJsonGenWriterStopIsEnabled())
				{
					TD.DCJsonGenWriterStop();
				}
			}
			return jsonFormatCollectionWriterDelegate;
		}

		[SecurityCritical]
		private JsonFormatWriterGenerator.CriticalHelper helper;

		private class CriticalHelper
		{
			internal JsonFormatClassWriterDelegate GenerateClassWriter(ClassDataContract classContract)
			{
				return delegate(XmlWriterDelegator xmlWriter, object obj, XmlObjectSerializerWriteContextComplexJson context, ClassDataContract dataContract, XmlDictionaryString[] memberNames)
				{
					new JsonFormatWriterInterpreter(classContract).WriteToJson(xmlWriter, obj, context, dataContract, memberNames);
				};
			}

			internal JsonFormatCollectionWriterDelegate GenerateCollectionWriter(CollectionDataContract collectionContract)
			{
				return delegate(XmlWriterDelegator xmlWriter, object obj, XmlObjectSerializerWriteContextComplexJson context, CollectionDataContract dataContract)
				{
					new JsonFormatWriterInterpreter(collectionContract).WriteCollectionToJson(xmlWriter, obj, context, dataContract);
				};
			}
		}
	}
}
