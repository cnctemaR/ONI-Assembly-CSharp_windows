using System;
using System.Runtime.Serialization.Diagnostics.Application;
using System.Security;

namespace System.Runtime.Serialization
{
	internal sealed class XmlFormatWriterGenerator
	{
		[SecurityCritical]
		public XmlFormatWriterGenerator()
		{
			this.helper = new XmlFormatWriterGenerator.CriticalHelper();
		}

		[SecurityCritical]
		internal XmlFormatClassWriterDelegate GenerateClassWriter(ClassDataContract classContract)
		{
			XmlFormatClassWriterDelegate xmlFormatClassWriterDelegate;
			try
			{
				if (TD.DCGenWriterStartIsEnabled())
				{
					TD.DCGenWriterStart("Class", classContract.UnderlyingType.FullName);
				}
				xmlFormatClassWriterDelegate = this.helper.GenerateClassWriter(classContract);
			}
			finally
			{
				if (TD.DCGenWriterStopIsEnabled())
				{
					TD.DCGenWriterStop();
				}
			}
			return xmlFormatClassWriterDelegate;
		}

		[SecurityCritical]
		internal XmlFormatCollectionWriterDelegate GenerateCollectionWriter(CollectionDataContract collectionContract)
		{
			XmlFormatCollectionWriterDelegate xmlFormatCollectionWriterDelegate;
			try
			{
				if (TD.DCGenWriterStartIsEnabled())
				{
					TD.DCGenWriterStart("Collection", collectionContract.UnderlyingType.FullName);
				}
				xmlFormatCollectionWriterDelegate = this.helper.GenerateCollectionWriter(collectionContract);
			}
			finally
			{
				if (TD.DCGenWriterStopIsEnabled())
				{
					TD.DCGenWriterStop();
				}
			}
			return xmlFormatCollectionWriterDelegate;
		}

		[SecurityCritical]
		private XmlFormatWriterGenerator.CriticalHelper helper;

		private class CriticalHelper
		{
			internal XmlFormatClassWriterDelegate GenerateClassWriter(ClassDataContract classContract)
			{
				return delegate(XmlWriterDelegator xw, object obj, XmlObjectSerializerWriteContext ctx, ClassDataContract ctr)
				{
					new XmlFormatWriterInterpreter(classContract).WriteToXml(xw, obj, ctx, ctr);
				};
			}

			internal XmlFormatCollectionWriterDelegate GenerateCollectionWriter(CollectionDataContract collectionContract)
			{
				return delegate(XmlWriterDelegator xw, object obj, XmlObjectSerializerWriteContext ctx, CollectionDataContract ctr)
				{
					new XmlFormatWriterInterpreter(collectionContract).WriteCollectionToXml(xw, obj, ctx, ctr);
				};
			}
		}
	}
}
