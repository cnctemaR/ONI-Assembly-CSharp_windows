using System;
using System.Collections;
using System.Xml.Xsl.Runtime;

namespace System.Xml.Xsl
{
	internal class XmlILCommand
	{
		public XmlILCommand(ExecuteDelegate delExec, XmlQueryStaticData staticData)
		{
			this.delExec = delExec;
			this.staticData = staticData;
		}

		public ExecuteDelegate ExecuteDelegate
		{
			get
			{
				return this.delExec;
			}
		}

		public XmlQueryStaticData StaticData
		{
			get
			{
				return this.staticData;
			}
		}

		public IList Evaluate(string contextDocumentUri, XmlResolver dataSources, XsltArgumentList argumentList)
		{
			XmlCachedSequenceWriter xmlCachedSequenceWriter = new XmlCachedSequenceWriter();
			this.Execute(contextDocumentUri, dataSources, argumentList, xmlCachedSequenceWriter);
			return xmlCachedSequenceWriter.ResultSequence;
		}

		public void Execute(object defaultDocument, XmlResolver dataSources, XsltArgumentList argumentList, XmlWriter writer)
		{
			try
			{
				if (writer is XmlAsyncCheckWriter)
				{
					writer = ((XmlAsyncCheckWriter)writer).CoreWriter;
				}
				XmlWellFormedWriter xmlWellFormedWriter = writer as XmlWellFormedWriter;
				if (xmlWellFormedWriter != null && xmlWellFormedWriter.RawWriter != null && xmlWellFormedWriter.WriteState == WriteState.Start && xmlWellFormedWriter.Settings.ConformanceLevel != ConformanceLevel.Document)
				{
					this.Execute(defaultDocument, dataSources, argumentList, new XmlMergeSequenceWriter(xmlWellFormedWriter.RawWriter));
				}
				else
				{
					this.Execute(defaultDocument, dataSources, argumentList, new XmlMergeSequenceWriter(new XmlRawWriterWrapper(writer)));
				}
			}
			finally
			{
				writer.Flush();
			}
		}

		private void Execute(object defaultDocument, XmlResolver dataSources, XsltArgumentList argumentList, XmlSequenceWriter results)
		{
			if (dataSources == null)
			{
				dataSources = XmlNullResolver.Singleton;
			}
			this.delExec(new XmlQueryRuntime(this.staticData, defaultDocument, dataSources, argumentList, results));
		}

		private ExecuteDelegate delExec;

		private XmlQueryStaticData staticData;
	}
}
