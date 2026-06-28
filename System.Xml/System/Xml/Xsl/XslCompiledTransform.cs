using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Xml.XPath;
using Mono.Xml.Xsl;

namespace System.Xml.Xsl
{
	[MonoTODO]
	public sealed class XslCompiledTransform
	{
		public XslCompiledTransform()
			: this(false)
		{
		}

		public XslCompiledTransform(bool enableDebug)
		{
			this.enable_debug = enableDebug;
			if (this.enable_debug)
			{
				this.debugger = new NoOperationDebugger();
			}
			this.output_settings.ConformanceLevel = ConformanceLevel.Fragment;
		}

		[MonoTODO]
		public XmlWriterSettings OutputSettings
		{
			get
			{
				return this.output_settings;
			}
		}

		[MonoTODO]
		public TempFileCollection TemporaryFiles
		{
			get
			{
				return null;
			}
		}

		public void Transform(string inputfile, string outputfile)
		{
			using (Stream stream = File.Create(outputfile))
			{
				this.Transform(new XPathDocument(inputfile, XmlSpace.Preserve), null, stream);
			}
		}

		public void Transform(string inputfile, XmlWriter output)
		{
			this.Transform(inputfile, null, output);
		}

		public void Transform(string inputfile, XsltArgumentList args, Stream output)
		{
			this.Transform(new XPathDocument(inputfile, XmlSpace.Preserve), args, output);
		}

		public void Transform(string inputfile, XsltArgumentList args, TextWriter output)
		{
			this.Transform(new XPathDocument(inputfile, XmlSpace.Preserve), args, output);
		}

		public void Transform(string inputfile, XsltArgumentList args, XmlWriter output)
		{
			this.Transform(new XPathDocument(inputfile, XmlSpace.Preserve), args, output);
		}

		public void Transform(XmlReader reader, XmlWriter output)
		{
			this.Transform(reader, null, output);
		}

		public void Transform(XmlReader reader, XsltArgumentList args, Stream output)
		{
			this.Transform(new XPathDocument(reader, XmlSpace.Preserve), args, output);
		}

		public void Transform(XmlReader reader, XsltArgumentList args, TextWriter output)
		{
			this.Transform(new XPathDocument(reader, XmlSpace.Preserve), args, output);
		}

		public void Transform(XmlReader reader, XsltArgumentList args, XmlWriter output)
		{
			this.Transform(reader, args, output, null);
		}

		public void Transform(IXPathNavigable input, XsltArgumentList args, TextWriter output)
		{
			this.Transform(input.CreateNavigator(), args, output);
		}

		public void Transform(IXPathNavigable input, XsltArgumentList args, Stream output)
		{
			this.Transform(input.CreateNavigator(), args, output);
		}

		public void Transform(IXPathNavigable input, XmlWriter output)
		{
			this.Transform(input, null, output);
		}

		public void Transform(IXPathNavigable input, XsltArgumentList args, XmlWriter output)
		{
			this.Transform(input.CreateNavigator(), args, output, null);
		}

		public void Transform(XmlReader input, XsltArgumentList args, XmlWriter output, XmlResolver resolver)
		{
			this.Transform(new XPathDocument(input, XmlSpace.Preserve).CreateNavigator(), args, output, resolver);
		}

		private void Transform(XPathNavigator input, XsltArgumentList args, XmlWriter output, XmlResolver resolver)
		{
			if (this.s == null)
			{
				throw new XsltException("No stylesheet was loaded.", null);
			}
			Outputter outputter = new GenericOutputter(output, this.s.Outputs, null);
			new XslTransformProcessor(this.s, this.debugger).Process(input, outputter, args, resolver);
			output.Flush();
		}

		private void Transform(XPathNavigator input, XsltArgumentList args, Stream output)
		{
			XslOutput xslOutput = (XslOutput)this.s.Outputs[string.Empty];
			this.Transform(input, args, new StreamWriter(output, xslOutput.Encoding));
		}

		private void Transform(XPathNavigator input, XsltArgumentList args, TextWriter output)
		{
			if (this.s == null)
			{
				throw new XsltException("No stylesheet was loaded.", null);
			}
			Outputter outputter = new GenericOutputter(output, this.s.Outputs, output.Encoding);
			new XslTransformProcessor(this.s, this.debugger).Process(input, outputter, args, null);
			outputter.Done();
			output.Flush();
		}

		private XmlReader GetXmlReader(string url)
		{
			XmlResolver xmlResolver = new XmlUrlResolver();
			Uri uri = xmlResolver.ResolveUri(null, url);
			Stream stream = xmlResolver.GetEntity(uri, null, typeof(Stream)) as Stream;
			return new XmlValidatingReader(new XmlTextReader(uri.ToString(), stream)
			{
				XmlResolver = xmlResolver
			})
			{
				XmlResolver = xmlResolver,
				ValidationType = ValidationType.None
			};
		}

		public void Load(string url)
		{
			using (XmlReader xmlReader = this.GetXmlReader(url))
			{
				this.Load(xmlReader);
			}
		}

		public void Load(XmlReader stylesheet)
		{
			this.Load(stylesheet, null, null);
		}

		public void Load(IXPathNavigable stylesheet)
		{
			this.Load(stylesheet.CreateNavigator(), null, null);
		}

		public void Load(IXPathNavigable stylesheet, XsltSettings settings, XmlResolver resolver)
		{
			this.Load(stylesheet.CreateNavigator(), settings, resolver);
		}

		public void Load(XmlReader stylesheet, XsltSettings settings, XmlResolver resolver)
		{
			this.Load(new XPathDocument(stylesheet, XmlSpace.Preserve).CreateNavigator(), settings, resolver);
		}

		public void Load(string stylesheet, XsltSettings settings, XmlResolver resolver)
		{
			this.Load(new XPathDocument(stylesheet, XmlSpace.Preserve).CreateNavigator(), settings, resolver);
		}

		private void Load(XPathNavigator stylesheet, XsltSettings settings, XmlResolver resolver)
		{
			this.s = new Compiler(this.debugger).Compile(stylesheet, resolver, null);
		}

		private bool enable_debug;

		private object debugger;

		private CompiledStylesheet s;

		private XmlWriterSettings output_settings = new XmlWriterSettings();
	}
}
