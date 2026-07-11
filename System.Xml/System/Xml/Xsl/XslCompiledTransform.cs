using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Security;
using System.Security.Permissions;
using System.Xml.XmlConfiguration;
using System.Xml.XPath;
using System.Xml.Xsl.Qil;
using System.Xml.Xsl.Runtime;
using System.Xml.Xsl.Xslt;

namespace System.Xml.Xsl
{
	public sealed class XslCompiledTransform
	{
		static XslCompiledTransform()
		{
			XslCompiledTransform.MemberAccessPermissionSet.AddPermission(new ReflectionPermission(ReflectionPermissionFlag.MemberAccess));
			XslCompiledTransform.ReaderSettings = new XmlReaderSettings();
		}

		public XslCompiledTransform()
		{
		}

		public XslCompiledTransform(bool enableDebug)
		{
			this.enableDebug = enableDebug;
		}

		private void Reset()
		{
			this.compilerResults = null;
			this.outputSettings = null;
			this.qil = null;
			this.command = null;
		}

		internal CompilerErrorCollection Errors
		{
			get
			{
				if (this.compilerResults == null)
				{
					return null;
				}
				return this.compilerResults.Errors;
			}
		}

		public XmlWriterSettings OutputSettings
		{
			get
			{
				return this.outputSettings;
			}
		}

		public TempFileCollection TemporaryFiles
		{
			[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
			get
			{
				if (this.compilerResults == null)
				{
					return null;
				}
				return this.compilerResults.TempFiles;
			}
		}

		public void Load(XmlReader stylesheet)
		{
			this.Reset();
			this.LoadInternal(stylesheet, XsltSettings.Default, XsltConfigSection.CreateDefaultResolver());
		}

		public void Load(XmlReader stylesheet, XsltSettings settings, XmlResolver stylesheetResolver)
		{
			this.Reset();
			this.LoadInternal(stylesheet, settings, stylesheetResolver);
		}

		public void Load(IXPathNavigable stylesheet)
		{
			this.Reset();
			this.LoadInternal(stylesheet, XsltSettings.Default, XsltConfigSection.CreateDefaultResolver());
		}

		public void Load(IXPathNavigable stylesheet, XsltSettings settings, XmlResolver stylesheetResolver)
		{
			this.Reset();
			this.LoadInternal(stylesheet, settings, stylesheetResolver);
		}

		public void Load(string stylesheetUri)
		{
			this.Reset();
			if (stylesheetUri == null)
			{
				throw new ArgumentNullException("stylesheetUri");
			}
			this.LoadInternal(stylesheetUri, XsltSettings.Default, XsltConfigSection.CreateDefaultResolver());
		}

		public void Load(string stylesheetUri, XsltSettings settings, XmlResolver stylesheetResolver)
		{
			this.Reset();
			if (stylesheetUri == null)
			{
				throw new ArgumentNullException("stylesheetUri");
			}
			this.LoadInternal(stylesheetUri, settings, stylesheetResolver);
		}

		private CompilerResults LoadInternal(object stylesheet, XsltSettings settings, XmlResolver stylesheetResolver)
		{
			if (stylesheet == null)
			{
				throw new ArgumentNullException("stylesheet");
			}
			if (settings == null)
			{
				settings = XsltSettings.Default;
			}
			this.CompileXsltToQil(stylesheet, settings, stylesheetResolver);
			CompilerError firstError = this.GetFirstError();
			if (firstError != null)
			{
				throw new XslLoadException(firstError);
			}
			if (!settings.CheckOnly)
			{
				this.CompileQilToMsil(settings);
			}
			return this.compilerResults;
		}

		private void CompileXsltToQil(object stylesheet, XsltSettings settings, XmlResolver stylesheetResolver)
		{
			this.compilerResults = new Compiler(settings, this.enableDebug, null).Compile(stylesheet, stylesheetResolver, out this.qil);
		}

		private CompilerError GetFirstError()
		{
			foreach (object obj in this.compilerResults.Errors)
			{
				CompilerError compilerError = (CompilerError)obj;
				if (!compilerError.IsWarning)
				{
					return compilerError;
				}
			}
			return null;
		}

		private void CompileQilToMsil(XsltSettings settings)
		{
			this.command = new XmlILGenerator().Generate(this.qil, null);
			this.outputSettings = this.command.StaticData.DefaultWriterSettings;
			this.qil = null;
		}

		public static CompilerErrorCollection CompileToType(XmlReader stylesheet, XsltSettings settings, XmlResolver stylesheetResolver, bool debug, TypeBuilder typeBuilder, string scriptAssemblyPath)
		{
			if (stylesheet == null)
			{
				throw new ArgumentNullException("stylesheet");
			}
			if (typeBuilder == null)
			{
				throw new ArgumentNullException("typeBuilder");
			}
			if (settings == null)
			{
				settings = XsltSettings.Default;
			}
			if (settings.EnableScript && scriptAssemblyPath == null)
			{
				throw new ArgumentNullException("scriptAssemblyPath");
			}
			if (scriptAssemblyPath != null)
			{
				scriptAssemblyPath = Path.GetFullPath(scriptAssemblyPath);
			}
			QilExpression qilExpression;
			CompilerErrorCollection errors = new Compiler(settings, debug, scriptAssemblyPath).Compile(stylesheet, stylesheetResolver, out qilExpression).Errors;
			if (!errors.HasErrors)
			{
				if (XslCompiledTransform.GeneratedCodeCtor == null)
				{
					XslCompiledTransform.GeneratedCodeCtor = typeof(GeneratedCodeAttribute).GetConstructor(new Type[]
					{
						typeof(string),
						typeof(string)
					});
				}
				typeBuilder.SetCustomAttribute(new CustomAttributeBuilder(XslCompiledTransform.GeneratedCodeCtor, new object[]
				{
					typeof(XslCompiledTransform).FullName,
					"4.0.0.0"
				}));
				new XmlILGenerator().Generate(qilExpression, typeBuilder);
			}
			return errors;
		}

		public void Load(Type compiledStylesheet)
		{
			this.Reset();
			if (compiledStylesheet == null)
			{
				throw new ArgumentNullException("compiledStylesheet");
			}
			object[] customAttributes = compiledStylesheet.GetCustomAttributes(typeof(GeneratedCodeAttribute), false);
			GeneratedCodeAttribute generatedCodeAttribute = ((customAttributes.Length != 0) ? ((GeneratedCodeAttribute)customAttributes[0]) : null);
			if (generatedCodeAttribute != null && generatedCodeAttribute.Tool == typeof(XslCompiledTransform).FullName)
			{
				if (new Version("4.0.0.0").CompareTo(new Version(generatedCodeAttribute.Version)) < 0)
				{
					throw new ArgumentException(Res.GetString("Executing a stylesheet that was compiled using a later version of the framework is not supported. Stylesheet Version: {0}. Current Framework Version: {1}.", new object[] { generatedCodeAttribute.Version, "4.0.0.0" }), "compiledStylesheet");
				}
				FieldInfo field = compiledStylesheet.GetField("staticData", BindingFlags.Static | BindingFlags.NonPublic);
				FieldInfo field2 = compiledStylesheet.GetField("ebTypes", BindingFlags.Static | BindingFlags.NonPublic);
				if (field != null && field2 != null)
				{
					if (XsltConfigSection.EnableMemberAccessForXslCompiledTransform)
					{
						new ReflectionPermission(ReflectionPermissionFlag.MemberAccess).Assert();
					}
					byte[] array = field.GetValue(null) as byte[];
					if (array != null)
					{
						MethodInfo method = compiledStylesheet.GetMethod("Execute", BindingFlags.Static | BindingFlags.NonPublic);
						Type[] array2 = (Type[])field2.GetValue(null);
						this.Load(method, array, array2);
						return;
					}
				}
			}
			if (this.command == null)
			{
				throw new ArgumentException(Res.GetString("Type '{0}' is not a compiled stylesheet class.", new object[] { compiledStylesheet.FullName }), "compiledStylesheet");
			}
		}

		public void Load(MethodInfo executeMethod, byte[] queryData, Type[] earlyBoundTypes)
		{
			this.Reset();
			if (executeMethod == null)
			{
				throw new ArgumentNullException("executeMethod");
			}
			if (queryData == null)
			{
				throw new ArgumentNullException("queryData");
			}
			if (!XsltConfigSection.EnableMemberAccessForXslCompiledTransform && executeMethod.DeclaringType != null && !executeMethod.DeclaringType.IsVisible)
			{
				new ReflectionPermission(ReflectionPermissionFlag.MemberAccess).Demand();
			}
			DynamicMethod dynamicMethod = executeMethod as DynamicMethod;
			Delegate @delegate = ((dynamicMethod != null) ? dynamicMethod.CreateDelegate(typeof(ExecuteDelegate)) : Delegate.CreateDelegate(typeof(ExecuteDelegate), executeMethod));
			this.command = new XmlILCommand((ExecuteDelegate)@delegate, new XmlQueryStaticData(queryData, earlyBoundTypes));
			this.outputSettings = this.command.StaticData.DefaultWriterSettings;
		}

		public void Transform(IXPathNavigable input, XmlWriter results)
		{
			XslCompiledTransform.CheckArguments(input, results);
			this.Transform(input, null, results, XsltConfigSection.CreateDefaultResolver());
		}

		public void Transform(IXPathNavigable input, XsltArgumentList arguments, XmlWriter results)
		{
			XslCompiledTransform.CheckArguments(input, results);
			this.Transform(input, arguments, results, XsltConfigSection.CreateDefaultResolver());
		}

		public void Transform(IXPathNavigable input, XsltArgumentList arguments, TextWriter results)
		{
			XslCompiledTransform.CheckArguments(input, results);
			using (XmlWriter xmlWriter = XmlWriter.Create(results, this.OutputSettings))
			{
				this.Transform(input, arguments, xmlWriter, XsltConfigSection.CreateDefaultResolver());
				xmlWriter.Close();
			}
		}

		public void Transform(IXPathNavigable input, XsltArgumentList arguments, Stream results)
		{
			XslCompiledTransform.CheckArguments(input, results);
			using (XmlWriter xmlWriter = XmlWriter.Create(results, this.OutputSettings))
			{
				this.Transform(input, arguments, xmlWriter, XsltConfigSection.CreateDefaultResolver());
				xmlWriter.Close();
			}
		}

		public void Transform(XmlReader input, XmlWriter results)
		{
			XslCompiledTransform.CheckArguments(input, results);
			this.Transform(input, null, results, XsltConfigSection.CreateDefaultResolver());
		}

		public void Transform(XmlReader input, XsltArgumentList arguments, XmlWriter results)
		{
			XslCompiledTransform.CheckArguments(input, results);
			this.Transform(input, arguments, results, XsltConfigSection.CreateDefaultResolver());
		}

		public void Transform(XmlReader input, XsltArgumentList arguments, TextWriter results)
		{
			XslCompiledTransform.CheckArguments(input, results);
			using (XmlWriter xmlWriter = XmlWriter.Create(results, this.OutputSettings))
			{
				this.Transform(input, arguments, xmlWriter, XsltConfigSection.CreateDefaultResolver());
				xmlWriter.Close();
			}
		}

		public void Transform(XmlReader input, XsltArgumentList arguments, Stream results)
		{
			XslCompiledTransform.CheckArguments(input, results);
			using (XmlWriter xmlWriter = XmlWriter.Create(results, this.OutputSettings))
			{
				this.Transform(input, arguments, xmlWriter, XsltConfigSection.CreateDefaultResolver());
				xmlWriter.Close();
			}
		}

		public void Transform(string inputUri, XmlWriter results)
		{
			XslCompiledTransform.CheckArguments(inputUri, results);
			using (XmlReader xmlReader = XmlReader.Create(inputUri, XslCompiledTransform.ReaderSettings))
			{
				this.Transform(xmlReader, null, results, XsltConfigSection.CreateDefaultResolver());
			}
		}

		public void Transform(string inputUri, XsltArgumentList arguments, XmlWriter results)
		{
			XslCompiledTransform.CheckArguments(inputUri, results);
			using (XmlReader xmlReader = XmlReader.Create(inputUri, XslCompiledTransform.ReaderSettings))
			{
				this.Transform(xmlReader, arguments, results, XsltConfigSection.CreateDefaultResolver());
			}
		}

		public void Transform(string inputUri, XsltArgumentList arguments, TextWriter results)
		{
			XslCompiledTransform.CheckArguments(inputUri, results);
			using (XmlReader xmlReader = XmlReader.Create(inputUri, XslCompiledTransform.ReaderSettings))
			{
				using (XmlWriter xmlWriter = XmlWriter.Create(results, this.OutputSettings))
				{
					this.Transform(xmlReader, arguments, xmlWriter, XsltConfigSection.CreateDefaultResolver());
					xmlWriter.Close();
				}
			}
		}

		public void Transform(string inputUri, XsltArgumentList arguments, Stream results)
		{
			XslCompiledTransform.CheckArguments(inputUri, results);
			using (XmlReader xmlReader = XmlReader.Create(inputUri, XslCompiledTransform.ReaderSettings))
			{
				using (XmlWriter xmlWriter = XmlWriter.Create(results, this.OutputSettings))
				{
					this.Transform(xmlReader, arguments, xmlWriter, XsltConfigSection.CreateDefaultResolver());
					xmlWriter.Close();
				}
			}
		}

		public void Transform(string inputUri, string resultsFile)
		{
			if (inputUri == null)
			{
				throw new ArgumentNullException("inputUri");
			}
			if (resultsFile == null)
			{
				throw new ArgumentNullException("resultsFile");
			}
			using (XmlReader xmlReader = XmlReader.Create(inputUri, XslCompiledTransform.ReaderSettings))
			{
				using (XmlWriter xmlWriter = XmlWriter.Create(resultsFile, this.OutputSettings))
				{
					this.Transform(xmlReader, null, xmlWriter, XsltConfigSection.CreateDefaultResolver());
					xmlWriter.Close();
				}
			}
		}

		public void Transform(XmlReader input, XsltArgumentList arguments, XmlWriter results, XmlResolver documentResolver)
		{
			XslCompiledTransform.CheckArguments(input, results);
			this.CheckCommand();
			this.command.Execute(input, documentResolver, arguments, results);
		}

		public void Transform(IXPathNavigable input, XsltArgumentList arguments, XmlWriter results, XmlResolver documentResolver)
		{
			XslCompiledTransform.CheckArguments(input, results);
			this.CheckCommand();
			this.command.Execute(input.CreateNavigator(), documentResolver, arguments, results);
		}

		private static void CheckArguments(object input, object results)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			if (results == null)
			{
				throw new ArgumentNullException("results");
			}
		}

		private static void CheckArguments(string inputUri, object results)
		{
			if (inputUri == null)
			{
				throw new ArgumentNullException("inputUri");
			}
			if (results == null)
			{
				throw new ArgumentNullException("results");
			}
		}

		private void CheckCommand()
		{
			if (this.command == null)
			{
				throw new InvalidOperationException(Res.GetString("No stylesheet was loaded."));
			}
		}

		private QilExpression TestCompile(object stylesheet, XsltSettings settings, XmlResolver stylesheetResolver)
		{
			this.Reset();
			this.CompileXsltToQil(stylesheet, settings, stylesheetResolver);
			return this.qil;
		}

		private void TestGenerate(XsltSettings settings)
		{
			this.CompileQilToMsil(settings);
		}

		private void Transform(string inputUri, XsltArgumentList arguments, XmlWriter results, XmlResolver documentResolver)
		{
			this.command.Execute(inputUri, documentResolver, arguments, results);
		}

		internal static void PrintQil(object qil, XmlWriter xw, bool printComments, bool printTypes, bool printLineInfo)
		{
			QilExpression qilExpression = (QilExpression)qil;
			QilXmlWriter.Options options = QilXmlWriter.Options.None;
			if (printComments)
			{
				options |= QilXmlWriter.Options.Annotations;
			}
			if (printTypes)
			{
				options |= QilXmlWriter.Options.TypeInfo;
			}
			if (printLineInfo)
			{
				options |= QilXmlWriter.Options.LineInfo;
			}
			new QilXmlWriter(xw, options).ToXml(qilExpression);
			xw.Flush();
		}

		private static readonly XmlReaderSettings ReaderSettings;

		private static readonly PermissionSet MemberAccessPermissionSet = new PermissionSet(PermissionState.None);

		private const string Version = "4.0.0.0";

		private bool enableDebug;

		private CompilerResults compilerResults;

		private XmlWriterSettings outputSettings;

		private QilExpression qil;

		private XmlILCommand command;

		private static volatile ConstructorInfo GeneratedCodeCtor;
	}
}
