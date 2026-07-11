using System;
using System.IO;
using System.Security.Policy;
using System.Xml.XPath;
using Mono.Xml.Xsl;

namespace System.Xml.Xsl
{
	public sealed class XslTransform
	{
		public XslTransform()
			: this(XslTransform.GetDefaultDebugger())
		{
		}

		internal XslTransform(object debugger)
		{
			this.debugger = debugger;
		}

		static XslTransform()
		{
			string environmentVariable = Environment.GetEnvironmentVariable("MONO_XSLT_STACK_FRAME");
			string text = environmentVariable;
			switch (text)
			{
			case "stdout":
				XslTransform.TemplateStackFrameOutput = Console.Out;
				break;
			case "stderr":
				XslTransform.TemplateStackFrameOutput = Console.Error;
				break;
			case "error":
				XslTransform.TemplateStackFrameError = true;
				break;
			}
		}

		private static object GetDefaultDebugger()
		{
			string text = null;
			try
			{
				text = Environment.GetEnvironmentVariable("MONO_XSLT_DEBUGGER");
			}
			catch (Exception)
			{
			}
			if (text == null)
			{
				return null;
			}
			if (text == "simple")
			{
				return new SimpleXsltDebugger();
			}
			return Activator.CreateInstance(Type.GetType(text));
		}

		[MonoTODO]
		public XmlResolver XmlResolver
		{
			set
			{
				this.xmlResolver = value;
			}
		}

		public XmlReader Transform(IXPathNavigable input, XsltArgumentList args)
		{
			return this.Transform(input.CreateNavigator(), args, this.xmlResolver);
		}

		public XmlReader Transform(IXPathNavigable input, XsltArgumentList args, XmlResolver resolver)
		{
			return this.Transform(input.CreateNavigator(), args, resolver);
		}

		public XmlReader Transform(XPathNavigator input, XsltArgumentList args)
		{
			return this.Transform(input, args, this.xmlResolver);
		}

		public XmlReader Transform(XPathNavigator input, XsltArgumentList args, XmlResolver resolver)
		{
			MemoryStream memoryStream = new MemoryStream();
			this.Transform(input, args, new XmlTextWriter(memoryStream, null), resolver);
			memoryStream.Position = 0L;
			return new XmlTextReader(memoryStream, XmlNodeType.Element, null);
		}

		public void Transform(IXPathNavigable input, XsltArgumentList args, TextWriter output)
		{
			this.Transform(input.CreateNavigator(), args, output, this.xmlResolver);
		}

		public void Transform(IXPathNavigable input, XsltArgumentList args, TextWriter output, XmlResolver resolver)
		{
			this.Transform(input.CreateNavigator(), args, output, resolver);
		}

		public void Transform(IXPathNavigable input, XsltArgumentList args, Stream output)
		{
			this.Transform(input.CreateNavigator(), args, output, this.xmlResolver);
		}

		public void Transform(IXPathNavigable input, XsltArgumentList args, Stream output, XmlResolver resolver)
		{
			this.Transform(input.CreateNavigator(), args, output, resolver);
		}

		public void Transform(IXPathNavigable input, XsltArgumentList args, XmlWriter output)
		{
			this.Transform(input.CreateNavigator(), args, output, this.xmlResolver);
		}

		public void Transform(IXPathNavigable input, XsltArgumentList args, XmlWriter output, XmlResolver resolver)
		{
			this.Transform(input.CreateNavigator(), args, output, resolver);
		}

		public void Transform(XPathNavigator input, XsltArgumentList args, XmlWriter output)
		{
			this.Transform(input, args, output, this.xmlResolver);
		}

		public void Transform(XPathNavigator input, XsltArgumentList args, XmlWriter output, XmlResolver resolver)
		{
			if (this.s == null)
			{
				throw new XsltException("No stylesheet was loaded.", null);
			}
			Outputter outputter = new GenericOutputter(output, this.s.Outputs, null);
			new XslTransformProcessor(this.s, this.debugger).Process(input, outputter, args, resolver);
			output.Flush();
		}

		public void Transform(XPathNavigator input, XsltArgumentList args, Stream output)
		{
			this.Transform(input, args, output, this.xmlResolver);
		}

		public void Transform(XPathNavigator input, XsltArgumentList args, Stream output, XmlResolver resolver)
		{
			XslOutput xslOutput = (XslOutput)this.s.Outputs[string.Empty];
			this.Transform(input, args, new StreamWriter(output, xslOutput.Encoding), resolver);
		}

		public void Transform(XPathNavigator input, XsltArgumentList args, TextWriter output)
		{
			this.Transform(input, args, output, this.xmlResolver);
		}

		public void Transform(XPathNavigator input, XsltArgumentList args, TextWriter output, XmlResolver resolver)
		{
			if (this.s == null)
			{
				throw new XsltException("No stylesheet was loaded.", null);
			}
			Outputter outputter = new GenericOutputter(output, this.s.Outputs, output.Encoding);
			new XslTransformProcessor(this.s, this.debugger).Process(input, outputter, args, resolver);
			outputter.Done();
			output.Flush();
		}

		public void Transform(string inputfile, string outputfile)
		{
			this.Transform(inputfile, outputfile, this.xmlResolver);
		}

		public void Transform(string inputfile, string outputfile, XmlResolver resolver)
		{
			using (Stream stream = new FileStream(outputfile, FileMode.Create, FileAccess.ReadWrite))
			{
				this.Transform(new XPathDocument(inputfile).CreateNavigator(), null, stream, resolver);
			}
		}

		public void Load(string url)
		{
			this.Load(url, null);
		}

		public void Load(string url, XmlResolver resolver)
		{
			XmlResolver xmlResolver = resolver;
			if (xmlResolver == null)
			{
				xmlResolver = new XmlUrlResolver();
			}
			Uri uri = xmlResolver.ResolveUri(null, url);
			using (Stream stream = xmlResolver.GetEntity(uri, null, typeof(Stream)) as Stream)
			{
				this.Load(new XPathDocument(new XmlValidatingReader(new XmlTextReader(uri.ToString(), stream)
				{
					XmlResolver = xmlResolver
				})
				{
					XmlResolver = xmlResolver,
					ValidationType = ValidationType.None
				}, XmlSpace.Preserve).CreateNavigator(), resolver, null);
			}
		}

		public void Load(XmlReader stylesheet)
		{
			this.Load(stylesheet, null, null);
		}

		public void Load(XmlReader stylesheet, XmlResolver resolver)
		{
			this.Load(stylesheet, resolver, null);
		}

		public void Load(XPathNavigator stylesheet)
		{
			this.Load(stylesheet, null, null);
		}

		public void Load(XPathNavigator stylesheet, XmlResolver resolver)
		{
			this.Load(stylesheet, resolver, null);
		}

		public void Load(IXPathNavigable stylesheet)
		{
			this.Load(stylesheet.CreateNavigator(), null);
		}

		public void Load(IXPathNavigable stylesheet, XmlResolver resolver)
		{
			this.Load(stylesheet.CreateNavigator(), resolver);
		}

		public void Load(IXPathNavigable stylesheet, XmlResolver resolver, Evidence evidence)
		{
			this.Load(stylesheet.CreateNavigator(), resolver, evidence);
		}

		public void Load(XPathNavigator stylesheet, XmlResolver resolver, Evidence evidence)
		{
			this.s = new Compiler(this.debugger).Compile(stylesheet, resolver, evidence);
		}

		public void Load(XmlReader stylesheet, XmlResolver resolver, Evidence evidence)
		{
			this.Load(new XPathDocument(stylesheet, XmlSpace.Preserve).CreateNavigator(), resolver, evidence);
		}

		internal static readonly bool TemplateStackFrameError;

		internal static readonly TextWriter TemplateStackFrameOutput;

		private object debugger;

		private CompiledStylesheet s;

		private XmlResolver xmlResolver = new XmlUrlResolver();
	}
}
