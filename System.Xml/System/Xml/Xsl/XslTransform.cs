using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Policy;
using System.Xml.XmlConfiguration;
using System.Xml.XPath;
using System.Xml.Xsl.XsltOld;
using System.Xml.Xsl.XsltOld.Debugger;

namespace System.Xml.Xsl
{
	[Obsolete("This class has been deprecated. Please use System.Xml.Xsl.XslCompiledTransform instead. http://go.microsoft.com/fwlink/?linkid=14202")]
	public sealed class XslTransform
	{
		private XmlResolver _DocumentResolver
		{
			get
			{
				if (this.isDocumentResolverSet)
				{
					return this._documentResolver;
				}
				return XsltConfigSection.CreateDefaultResolver();
			}
		}

		public XslTransform()
		{
		}

		public XmlResolver XmlResolver
		{
			set
			{
				this._documentResolver = value;
				this.isDocumentResolverSet = true;
			}
		}

		public void Load(XmlReader stylesheet)
		{
			this.Load(stylesheet, XsltConfigSection.CreateDefaultResolver());
		}

		public void Load(XmlReader stylesheet, XmlResolver resolver)
		{
			this.Load(new XPathDocument(stylesheet, XmlSpace.Preserve), resolver);
		}

		public void Load(IXPathNavigable stylesheet)
		{
			this.Load(stylesheet, XsltConfigSection.CreateDefaultResolver());
		}

		public void Load(IXPathNavigable stylesheet, XmlResolver resolver)
		{
			if (stylesheet == null)
			{
				throw new ArgumentNullException("stylesheet");
			}
			this.Load(stylesheet.CreateNavigator(), resolver);
		}

		public void Load(XPathNavigator stylesheet)
		{
			if (stylesheet == null)
			{
				throw new ArgumentNullException("stylesheet");
			}
			this.Load(stylesheet, XsltConfigSection.CreateDefaultResolver());
		}

		public void Load(XPathNavigator stylesheet, XmlResolver resolver)
		{
			if (stylesheet == null)
			{
				throw new ArgumentNullException("stylesheet");
			}
			this.Compile(stylesheet, resolver, null);
		}

		public void Load(string url)
		{
			XmlTextReaderImpl xmlTextReaderImpl = new XmlTextReaderImpl(url);
			Evidence evidence = XmlSecureResolver.CreateEvidenceForUrl(xmlTextReaderImpl.BaseURI);
			this.Compile(Compiler.LoadDocument(xmlTextReaderImpl).CreateNavigator(), XsltConfigSection.CreateDefaultResolver(), evidence);
		}

		public void Load(string url, XmlResolver resolver)
		{
			XmlTextReaderImpl xmlTextReaderImpl = new XmlTextReaderImpl(url);
			xmlTextReaderImpl.XmlResolver = resolver;
			Evidence evidence = XmlSecureResolver.CreateEvidenceForUrl(xmlTextReaderImpl.BaseURI);
			this.Compile(Compiler.LoadDocument(xmlTextReaderImpl).CreateNavigator(), resolver, evidence);
		}

		public void Load(IXPathNavigable stylesheet, XmlResolver resolver, Evidence evidence)
		{
			if (stylesheet == null)
			{
				throw new ArgumentNullException("stylesheet");
			}
			this.Load(stylesheet.CreateNavigator(), resolver, evidence);
		}

		public void Load(XmlReader stylesheet, XmlResolver resolver, Evidence evidence)
		{
			if (stylesheet == null)
			{
				throw new ArgumentNullException("stylesheet");
			}
			this.Load(new XPathDocument(stylesheet, XmlSpace.Preserve), resolver, evidence);
		}

		public void Load(XPathNavigator stylesheet, XmlResolver resolver, Evidence evidence)
		{
			if (stylesheet == null)
			{
				throw new ArgumentNullException("stylesheet");
			}
			this.Compile(stylesheet, resolver, evidence);
		}

		private void CheckCommand()
		{
			if (this._CompiledStylesheet == null)
			{
				throw new InvalidOperationException(Res.GetString("No stylesheet was loaded."));
			}
		}

		public XmlReader Transform(XPathNavigator input, XsltArgumentList args, XmlResolver resolver)
		{
			this.CheckCommand();
			return new Processor(input, args, resolver, this._CompiledStylesheet, this._QueryStore, this._RootAction, this.debugger).StartReader();
		}

		public XmlReader Transform(XPathNavigator input, XsltArgumentList args)
		{
			return this.Transform(input, args, this._DocumentResolver);
		}

		public void Transform(XPathNavigator input, XsltArgumentList args, XmlWriter output, XmlResolver resolver)
		{
			this.CheckCommand();
			new Processor(input, args, resolver, this._CompiledStylesheet, this._QueryStore, this._RootAction, this.debugger).Execute(output);
		}

		public void Transform(XPathNavigator input, XsltArgumentList args, XmlWriter output)
		{
			this.Transform(input, args, output, this._DocumentResolver);
		}

		public void Transform(XPathNavigator input, XsltArgumentList args, Stream output, XmlResolver resolver)
		{
			this.CheckCommand();
			new Processor(input, args, resolver, this._CompiledStylesheet, this._QueryStore, this._RootAction, this.debugger).Execute(output);
		}

		public void Transform(XPathNavigator input, XsltArgumentList args, Stream output)
		{
			this.Transform(input, args, output, this._DocumentResolver);
		}

		public void Transform(XPathNavigator input, XsltArgumentList args, TextWriter output, XmlResolver resolver)
		{
			this.CheckCommand();
			new Processor(input, args, resolver, this._CompiledStylesheet, this._QueryStore, this._RootAction, this.debugger).Execute(output);
		}

		public void Transform(XPathNavigator input, XsltArgumentList args, TextWriter output)
		{
			this.CheckCommand();
			new Processor(input, args, this._DocumentResolver, this._CompiledStylesheet, this._QueryStore, this._RootAction, this.debugger).Execute(output);
		}

		public XmlReader Transform(IXPathNavigable input, XsltArgumentList args, XmlResolver resolver)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			return this.Transform(input.CreateNavigator(), args, resolver);
		}

		public XmlReader Transform(IXPathNavigable input, XsltArgumentList args)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			return this.Transform(input.CreateNavigator(), args, this._DocumentResolver);
		}

		public void Transform(IXPathNavigable input, XsltArgumentList args, TextWriter output, XmlResolver resolver)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			this.Transform(input.CreateNavigator(), args, output, resolver);
		}

		public void Transform(IXPathNavigable input, XsltArgumentList args, TextWriter output)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			this.Transform(input.CreateNavigator(), args, output, this._DocumentResolver);
		}

		public void Transform(IXPathNavigable input, XsltArgumentList args, Stream output, XmlResolver resolver)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			this.Transform(input.CreateNavigator(), args, output, resolver);
		}

		public void Transform(IXPathNavigable input, XsltArgumentList args, Stream output)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			this.Transform(input.CreateNavigator(), args, output, this._DocumentResolver);
		}

		public void Transform(IXPathNavigable input, XsltArgumentList args, XmlWriter output, XmlResolver resolver)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			this.Transform(input.CreateNavigator(), args, output, resolver);
		}

		public void Transform(IXPathNavigable input, XsltArgumentList args, XmlWriter output)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			this.Transform(input.CreateNavigator(), args, output, this._DocumentResolver);
		}

		public void Transform(string inputfile, string outputfile, XmlResolver resolver)
		{
			FileStream fileStream = null;
			try
			{
				XPathDocument xpathDocument = new XPathDocument(inputfile);
				fileStream = new FileStream(outputfile, FileMode.Create, FileAccess.ReadWrite);
				this.Transform(xpathDocument, null, fileStream, resolver);
			}
			finally
			{
				if (fileStream != null)
				{
					fileStream.Close();
				}
			}
		}

		public void Transform(string inputfile, string outputfile)
		{
			this.Transform(inputfile, outputfile, this._DocumentResolver);
		}

		private void Compile(XPathNavigator stylesheet, XmlResolver resolver, Evidence evidence)
		{
			Compiler compiler = ((this.Debugger == null) ? new Compiler() : new DbgCompiler(this.Debugger));
			NavigatorInput navigatorInput = new NavigatorInput(stylesheet);
			compiler.Compile(navigatorInput, resolver ?? XmlNullResolver.Singleton, evidence);
			this._CompiledStylesheet = compiler.CompiledStylesheet;
			this._QueryStore = compiler.QueryStore;
			this._RootAction = compiler.RootAction;
		}

		internal IXsltDebugger Debugger
		{
			get
			{
				return this.debugger;
			}
		}

		internal XslTransform(object debugger)
		{
			if (debugger != null)
			{
				this.debugger = new XslTransform.DebuggerAddapter(debugger);
			}
		}

		private XmlResolver _documentResolver;

		private bool isDocumentResolverSet;

		private Stylesheet _CompiledStylesheet;

		private List<TheQuery> _QueryStore;

		private RootAction _RootAction;

		private IXsltDebugger debugger;

		private class DebuggerAddapter : IXsltDebugger
		{
			public DebuggerAddapter(object unknownDebugger)
			{
				this.unknownDebugger = unknownDebugger;
				BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
				Type type = unknownDebugger.GetType();
				this.getBltIn = type.GetMethod("GetBuiltInTemplatesUri", bindingFlags);
				this.onCompile = type.GetMethod("OnInstructionCompile", bindingFlags);
				this.onExecute = type.GetMethod("OnInstructionExecute", bindingFlags);
			}

			public string GetBuiltInTemplatesUri()
			{
				if (this.getBltIn == null)
				{
					return null;
				}
				return (string)this.getBltIn.Invoke(this.unknownDebugger, new object[0]);
			}

			public void OnInstructionCompile(XPathNavigator styleSheetNavigator)
			{
				if (this.onCompile != null)
				{
					this.onCompile.Invoke(this.unknownDebugger, new object[] { styleSheetNavigator });
				}
			}

			public void OnInstructionExecute(IXsltProcessor xsltProcessor)
			{
				if (this.onExecute != null)
				{
					this.onExecute.Invoke(this.unknownDebugger, new object[] { xsltProcessor });
				}
			}

			private object unknownDebugger;

			private MethodInfo getBltIn;

			private MethodInfo onCompile;

			private MethodInfo onExecute;
		}
	}
}
