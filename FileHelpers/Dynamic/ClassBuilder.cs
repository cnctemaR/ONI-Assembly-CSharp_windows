using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace FileHelpers.Dynamic
{
	public abstract class ClassBuilder
	{
		public static Type ClassFromString(string classStr)
		{
			return ClassBuilder.ClassFromString(classStr, string.Empty);
		}

		public static Type ClassFromString(string classStr, NetLanguage lang)
		{
			return ClassBuilder.ClassFromString(classStr, string.Empty, lang);
		}

		public static Type ClassFromString(string classStr, string className)
		{
			return ClassBuilder.ClassFromString(classStr, className, NetLanguage.CSharp);
		}

		public static Type ClassFromString(string classStr, string className, NetLanguage lang)
		{
			if (classStr.Length < 4)
			{
				throw new BadUsageException("There is not enough text to be a proper class, load your class and try again");
			}
			CompilerParameters compilerParameters = new CompilerParameters();
			bool flag = false;
			lock (ClassBuilder.mReferencesLock)
			{
				if (ClassBuilder.mReferences == null)
				{
					ArrayList arrayList = new ArrayList();
					foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
					{
						Module module = assembly.GetModules()[0];
						if (!(module.Name == "mscorlib.dll") && !(module.Name == "<Unknown>"))
						{
							if (module.Name == "System.Data.dll")
							{
								flag = true;
							}
							if (File.Exists(module.FullyQualifiedName))
							{
								arrayList.Add(module.FullyQualifiedName);
							}
						}
					}
					ClassBuilder.mReferences = (string[])arrayList.ToArray(typeof(string));
				}
			}
			compilerParameters.ReferencedAssemblies.AddRange(ClassBuilder.mReferences);
			compilerParameters.GenerateExecutable = false;
			compilerParameters.GenerateInMemory = true;
			compilerParameters.IncludeDebugInformation = false;
			StringBuilder stringBuilder = new StringBuilder();
			switch (lang)
			{
			case NetLanguage.CSharp:
				stringBuilder.Append("using System; using FileHelpers;");
				if (flag)
				{
					stringBuilder.Append(" using System.Data;");
				}
				break;
			case NetLanguage.VbNet:
				if (CultureInfo.CurrentCulture.CompareInfo.IndexOf(classStr, "Imports System", CompareOptions.IgnoreCase) == -1)
				{
					stringBuilder.Append("Imports System\n");
				}
				if (CultureInfo.CurrentCulture.CompareInfo.IndexOf(classStr, "Imports FileHelpers", CompareOptions.IgnoreCase) == -1)
				{
					stringBuilder.Append("Imports FileHelpers\n");
				}
				if (flag && CultureInfo.CurrentCulture.CompareInfo.IndexOf(classStr, "Imports System.Data", CompareOptions.IgnoreCase) == -1)
				{
					stringBuilder.Append("Imports System.Data\n");
				}
				break;
			}
			stringBuilder.Append(classStr);
			CodeDomProvider codeDomProvider = null;
			switch (lang)
			{
			case NetLanguage.CSharp:
				codeDomProvider = CodeDomProvider.CreateProvider("cs");
				break;
			case NetLanguage.VbNet:
				codeDomProvider = CodeDomProvider.CreateProvider("vb");
				break;
			}
			CompilerResults compilerResults = codeDomProvider.CompileAssemblyFromSource(compilerParameters, new string[] { stringBuilder.ToString() });
			if (compilerResults.Errors.HasErrors)
			{
				StringBuilder stringBuilder2 = new StringBuilder();
				stringBuilder2.Append("Error Compiling Expression: " + StringHelper.NewLine);
				foreach (object obj2 in compilerResults.Errors)
				{
					CompilerError compilerError = (CompilerError)obj2;
					stringBuilder2.AppendFormat("Line {0}: {1}\n", compilerError.Line, compilerError.ErrorText);
				}
				throw new RunTimeCompilationException(stringBuilder2.ToString(), classStr, compilerResults.Errors);
			}
			if (className != string.Empty)
			{
				return compilerResults.CompiledAssembly.GetType(className, true, true);
			}
			Type[] types = compilerResults.CompiledAssembly.GetTypes();
			if (types.Length > 0)
			{
				foreach (Type type in types)
				{
					if (!type.FullName.StartsWith("My.My") && type.IsDefined(typeof(TypedRecordAttribute), false))
					{
						return type;
					}
				}
			}
			throw new BadUsageException("The compiled assembly does not have any type inside.");
		}

		public static Type ClassFromSourceFile(string filename)
		{
			return ClassBuilder.ClassFromSourceFile(filename, string.Empty);
		}

		public static Type ClassFromSourceFile(string filename, NetLanguage lang)
		{
			return ClassBuilder.ClassFromSourceFile(filename, string.Empty, lang);
		}

		public static Type ClassFromSourceFile(string filename, string className)
		{
			return ClassBuilder.ClassFromSourceFile(filename, className, NetLanguage.CSharp);
		}

		public static Type ClassFromSourceFile(string filename, string className, NetLanguage lang)
		{
			StreamReader streamReader = new StreamReader(filename);
			string text = streamReader.ReadToEnd();
			streamReader.Close();
			return ClassBuilder.ClassFromString(text, className, lang);
		}

		public static Type ClassFromBinaryFile(string filename)
		{
			return ClassBuilder.ClassFromBinaryFile(filename, string.Empty, NetLanguage.CSharp);
		}

		public static Type ClassFromBinaryFile(string filename, NetLanguage lang)
		{
			return ClassBuilder.ClassFromBinaryFile(filename, string.Empty, lang);
		}

		public static Type ClassFromBinaryFile(string filename, string className, NetLanguage lang)
		{
			return ClassBuilder.ClassFromBinaryFile(filename, className, lang, "withthefilehelpers1.0.0youcancodewithoutproblems1.5.0");
		}

		public static Type ClassFromBinaryFile(string filename, string className, NetLanguage lang, string password)
		{
			StreamReader streamReader = new StreamReader(filename);
			string text = streamReader.ReadToEnd();
			streamReader.Close();
			text = ClassBuilder.Decrypt(text, password);
			return ClassBuilder.ClassFromString(text, className, lang);
		}

		public static Type ClassFromXmlFile(string filename)
		{
			ClassBuilder classBuilder = ClassBuilder.LoadFromXml(filename);
			return classBuilder.CreateRecordClass();
		}

		public static void ClassToBinaryFile(string filename, string classSource)
		{
			ClassBuilder.ClassToBinaryFile(filename, classSource, "withthefilehelpers1.0.0youcancodewithoutproblems1.5.0");
		}

		public static void ClassToBinaryFile(string filename, string classSource, string password)
		{
			classSource = ClassBuilder.Encrypt(classSource, password);
			StreamWriter streamWriter = new StreamWriter(filename);
			streamWriter.Write(classSource);
			streamWriter.Close();
		}

		public void SaveToSourceFile(string filename)
		{
			this.SaveToSourceFile(filename, NetLanguage.CSharp);
		}

		public void SaveToSourceFile(string filename, NetLanguage lang)
		{
			StreamWriter streamWriter = new StreamWriter(filename);
			streamWriter.Write(this.GetClassSourceCode(lang));
			streamWriter.Close();
		}

		public void SaveToBinaryFile(string filename)
		{
			this.SaveToBinaryFile(filename, NetLanguage.CSharp);
		}

		public void SaveToBinaryFile(string filename, NetLanguage lang)
		{
			StreamWriter streamWriter = new StreamWriter(filename);
			streamWriter.Write(this.GetClassBinaryCode(lang));
			streamWriter.Close();
		}

		internal ClassBuilder(string className)
		{
			className = className.Trim();
			if (!ValidIdentifierValidator.ValidIdentifier(className))
			{
				throw new FileHelpersException(Messages.Errors.InvalidIdentifier.Identifier(className).Text);
			}
			this.mClassName = className;
		}

		public Type CreateRecordClass()
		{
			string classSourceCode = this.GetClassSourceCode(NetLanguage.CSharp);
			return ClassBuilder.ClassFromString(classSourceCode, NetLanguage.CSharp);
		}

		public void ClearFields()
		{
			this.mFields.Clear();
		}

		internal void AddFieldInternal(FieldBuilder field)
		{
			field.mFieldIndex = this.mFields.Add(field);
			field.mClassBuilder = this;
		}

		public FieldBuilder[] Fields
		{
			get
			{
				return (FieldBuilder[])this.mFields.ToArray(typeof(FieldBuilder));
			}
		}

		public int FieldCount
		{
			get
			{
				return this.mFields.Count;
			}
		}

		public FieldBuilder FieldByIndex(int index)
		{
			return (FieldBuilder)this.mFields[index];
		}

		public string ClassName
		{
			get
			{
				return this.mClassName;
			}
			set
			{
				this.mClassName = value;
			}
		}

		public int IgnoreFirstLines
		{
			get
			{
				return this.mIgnoreFirstLines;
			}
			set
			{
				this.mIgnoreFirstLines = value;
			}
		}

		public int IgnoreLastLines
		{
			get
			{
				return this.mIgnoreLastLines;
			}
			set
			{
				this.mIgnoreLastLines = value;
			}
		}

		public bool IgnoreEmptyLines
		{
			get
			{
				return this.mIgnoreEmptyLines;
			}
			set
			{
				this.mIgnoreEmptyLines = value;
			}
		}

		public bool GenerateProperties
		{
			get
			{
				return this.mGenerateProperties;
			}
			set
			{
				this.mGenerateProperties = value;
			}
		}

		public string GetClassBinaryCode(NetLanguage lang)
		{
			return ClassBuilder.Encrypt(this.GetClassSourceCode(lang), "withthefilehelpers1.0.0youcancodewithoutproblems1.5.0");
		}

		public string GetClassSourceCode(NetLanguage lang)
		{
			this.ValidateClass();
			StringBuilder stringBuilder = new StringBuilder(100);
			this.BeginNamespace(lang, stringBuilder);
			AttributesBuilder attributesBuilder = new AttributesBuilder(lang);
			this.AddAttributesInternal(attributesBuilder);
			this.AddAttributesCode(attributesBuilder, lang);
			stringBuilder.Append(attributesBuilder.GetAttributesCode());
			switch (lang)
			{
			case NetLanguage.CSharp:
				stringBuilder.Append(ClassBuilder.GetVisibility(lang, this.mVisibility) + this.GetSealed(lang) + "class " + this.mClassName);
				stringBuilder.Append(StringHelper.NewLine);
				stringBuilder.Append("{");
				break;
			case NetLanguage.VbNet:
				stringBuilder.Append(ClassBuilder.GetVisibility(lang, this.mVisibility) + this.GetSealed(lang) + "Class " + this.mClassName);
				stringBuilder.Append(StringHelper.NewLine);
				break;
			}
			stringBuilder.Append(StringHelper.NewLine);
			stringBuilder.Append(StringHelper.NewLine);
			foreach (object obj in this.mFields)
			{
				FieldBuilder fieldBuilder = (FieldBuilder)obj;
				stringBuilder.Append(fieldBuilder.GetFieldCode(lang));
				stringBuilder.Append(StringHelper.NewLine);
			}
			stringBuilder.Append(StringHelper.NewLine);
			switch (lang)
			{
			case NetLanguage.CSharp:
				stringBuilder.Append("}");
				break;
			case NetLanguage.VbNet:
				stringBuilder.Append("End Class");
				break;
			}
			this.EndNamespace(lang, stringBuilder);
			return stringBuilder.ToString();
		}

		private void ValidateClass()
		{
			if (this.ClassName.Trim().Length == 0)
			{
				throw new FileHelpersException(Messages.Errors.EmptyClassName.Text);
			}
			for (int i = 0; i < this.mFields.Count; i++)
			{
				if (((FieldBuilder)this.mFields[i]).FieldName.Trim().Length == 0)
				{
					throw new FileHelpersException(Messages.Errors.EmptyFieldName.Position((i + 1).ToString()).Text);
				}
				if (((FieldBuilder)this.mFields[i]).FieldType.Trim().Length == 0)
				{
					throw new FileHelpersException(Messages.Errors.EmptyFieldType.Position((i + 1).ToString()).Text);
				}
			}
		}

		internal abstract void AddAttributesCode(AttributesBuilder attbs, NetLanguage lang);

		private void AddAttributesInternal(AttributesBuilder attbs)
		{
			if (this.mIgnoreFirstLines != 0)
			{
				attbs.AddAttribute("IgnoreFirst(" + this.mIgnoreFirstLines.ToString() + ")");
			}
			if (this.mIgnoreLastLines != 0)
			{
				attbs.AddAttribute("IgnoreLast(" + this.mIgnoreLastLines.ToString() + ")");
			}
			if (this.mIgnoreEmptyLines)
			{
				attbs.AddAttribute("IgnoreEmptyLines()");
			}
			if (this.mRecordConditionInfo.Condition != FileHelpers.RecordCondition.None)
			{
				attbs.AddAttribute(string.Concat(new string[]
				{
					"ConditionalRecord(RecordCondition.",
					this.mRecordConditionInfo.Condition.ToString(),
					", \"",
					this.mRecordConditionInfo.Selector,
					"\")"
				}));
			}
			if (!string.IsNullOrEmpty(this.mIgnoreCommentInfo.CommentMarker))
			{
				attbs.AddAttribute(string.Concat(new string[]
				{
					"IgnoreCommentedLines(\"",
					this.mIgnoreCommentInfo.CommentMarker,
					"\", ",
					this.mIgnoreCommentInfo.InAnyPlace.ToString().ToLower(),
					")"
				}));
			}
		}

		private static byte[] Encrypt(byte[] clearData, byte[] key, byte[] iv)
		{
			MemoryStream memoryStream = new MemoryStream();
			Rijndael rijndael = Rijndael.Create();
			rijndael.Key = key;
			rijndael.IV = iv;
			CryptoStream cryptoStream = new CryptoStream(memoryStream, rijndael.CreateEncryptor(), CryptoStreamMode.Write);
			cryptoStream.Write(clearData, 0, clearData.Length);
			cryptoStream.Close();
			return memoryStream.ToArray();
		}

		private static string Encrypt(string clearText, string Password)
		{
			byte[] bytes = Encoding.Unicode.GetBytes(clearText);
			PasswordDeriveBytes passwordDeriveBytes = new PasswordDeriveBytes(Password, new byte[]
			{
				73, 118, 97, 110, 32, 77, 101, 100, 118, 101,
				100, 101, 118
			});
			byte[] array = ClassBuilder.Encrypt(bytes, passwordDeriveBytes.GetBytes(32), passwordDeriveBytes.GetBytes(16));
			return Convert.ToBase64String(array);
		}

		private static byte[] Decrypt(byte[] cipherData, byte[] key, byte[] iv)
		{
			MemoryStream memoryStream = new MemoryStream();
			Rijndael rijndael = Rijndael.Create();
			rijndael.Key = key;
			rijndael.IV = iv;
			CryptoStream cryptoStream = new CryptoStream(memoryStream, rijndael.CreateDecryptor(), CryptoStreamMode.Write);
			cryptoStream.Write(cipherData, 0, cipherData.Length);
			cryptoStream.Close();
			return memoryStream.ToArray();
		}

		private static string Decrypt(string cipherText, string password)
		{
			byte[] array = Convert.FromBase64String(cipherText);
			PasswordDeriveBytes passwordDeriveBytes = new PasswordDeriveBytes(password, new byte[]
			{
				73, 118, 97, 110, 32, 77, 101, 100, 118, 101,
				100, 101, 118
			});
			byte[] array2 = ClassBuilder.Decrypt(array, passwordDeriveBytes.GetBytes(32), passwordDeriveBytes.GetBytes(16));
			return Encoding.Unicode.GetString(array2);
		}

		public string CommentText
		{
			get
			{
				return this.mCommentText;
			}
			set
			{
				this.mCommentText = value;
			}
		}

		public NetVisibility Visibility
		{
			get
			{
				return this.mVisibility;
			}
			set
			{
				this.mVisibility = value;
			}
		}

		public bool SealedClass
		{
			get
			{
				return this.mSealedClass;
			}
			set
			{
				this.mSealedClass = value;
			}
		}

		public string Namespace
		{
			get
			{
				return this.mNamespace;
			}
			set
			{
				this.mNamespace = value;
			}
		}

		internal static string GetVisibility(NetLanguage lang, NetVisibility visibility)
		{
			switch (lang)
			{
			case NetLanguage.CSharp:
				switch (visibility)
				{
				case NetVisibility.Public:
					return "public ";
				case NetVisibility.Internal:
					return "internal ";
				case NetVisibility.Protected:
					return "protected ";
				case NetVisibility.Private:
					return "private ";
				}
				break;
			case NetLanguage.VbNet:
				switch (visibility)
				{
				case NetVisibility.Public:
					return "Public ";
				case NetVisibility.Internal:
					return "Friend ";
				case NetVisibility.Protected:
					return "Protected ";
				case NetVisibility.Private:
					return "Private ";
				}
				break;
			}
			return string.Empty;
		}

		private string GetSealed(NetLanguage lang)
		{
			if (!this.mSealedClass)
			{
				return string.Empty;
			}
			switch (lang)
			{
			case NetLanguage.CSharp:
				return "sealed ";
			case NetLanguage.VbNet:
				return "NotInheritable ";
			default:
				return string.Empty;
			}
		}

		private void BeginNamespace(NetLanguage lang, StringBuilder sb)
		{
			if (this.mNamespace == string.Empty)
			{
				return;
			}
			switch (lang)
			{
			case NetLanguage.CSharp:
				sb.Append("namespace ");
				sb.Append(this.mNamespace);
				sb.Append(StringHelper.NewLine);
				sb.Append("{");
				break;
			case NetLanguage.VbNet:
				sb.Append("Namespace ");
				sb.Append(this.mNamespace);
				sb.Append(StringHelper.NewLine);
				break;
			}
			sb.Append(StringHelper.NewLine);
		}

		private void EndNamespace(NetLanguage lang, StringBuilder sb)
		{
			if (this.mNamespace == string.Empty)
			{
				return;
			}
			sb.Append(StringHelper.NewLine);
			switch (lang)
			{
			case NetLanguage.CSharp:
				sb.Append("}");
				return;
			case NetLanguage.VbNet:
				sb.Append("End Namespace");
				return;
			default:
				return;
			}
		}

		public static ClassBuilder LoadFromXmlString(string xml)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(new StringReader(xml));
			return ClassBuilder.LoadFromXml(xmlDocument);
		}

		public static ClassBuilder LoadFromXml(XmlDocument document)
		{
			ClassBuilder classBuilder = null;
			string localName = document.DocumentElement.LocalName;
			if (localName == "DelimitedClass")
			{
				classBuilder = DelimitedClassBuilder.LoadXmlInternal(document);
			}
			else
			{
				classBuilder = FixedLengthClassBuilder.LoadXmlInternal(document);
			}
			XmlNode xmlNode = document.DocumentElement["IgnoreLastLines"];
			if (xmlNode != null)
			{
				classBuilder.IgnoreLastLines = int.Parse(xmlNode.InnerText);
			}
			xmlNode = document.DocumentElement["IgnoreFirstLines"];
			if (xmlNode != null)
			{
				classBuilder.IgnoreFirstLines = int.Parse(xmlNode.InnerText);
			}
			xmlNode = document.DocumentElement["IgnoreEmptyLines"];
			if (xmlNode != null)
			{
				classBuilder.IgnoreEmptyLines = true;
			}
			xmlNode = document.DocumentElement["CommentMarker"];
			if (xmlNode != null)
			{
				classBuilder.IgnoreCommentedLines.CommentMarker = xmlNode.InnerText;
			}
			xmlNode = document.DocumentElement["CommentInAnyPlace"];
			if (xmlNode != null)
			{
				classBuilder.IgnoreCommentedLines.InAnyPlace = bool.Parse(xmlNode.InnerText.ToLower());
			}
			xmlNode = document.DocumentElement["SealedClass"];
			classBuilder.SealedClass = xmlNode != null;
			xmlNode = document.DocumentElement["Namespace"];
			if (xmlNode != null)
			{
				classBuilder.Namespace = xmlNode.InnerText;
			}
			xmlNode = document.DocumentElement["Visibility"];
			if (xmlNode != null)
			{
				classBuilder.Visibility = (NetVisibility)Enum.Parse(typeof(NetVisibility), xmlNode.InnerText);
			}
			xmlNode = document.DocumentElement["RecordCondition"];
			if (xmlNode != null)
			{
				classBuilder.RecordCondition.Condition = (RecordCondition)Enum.Parse(typeof(RecordCondition), xmlNode.InnerText);
			}
			xmlNode = document.DocumentElement["RecordConditionSelector"];
			if (xmlNode != null)
			{
				classBuilder.RecordCondition.Selector = xmlNode.InnerText;
			}
			xmlNode = document.DocumentElement["CommentText"];
			if (xmlNode != null)
			{
				classBuilder.CommentText = xmlNode.InnerText;
			}
			classBuilder.ReadClassElements(document);
			xmlNode = document.DocumentElement["Fields"];
			XmlNodeList xmlNodeList;
			if (localName == "DelimitedClass")
			{
				xmlNodeList = xmlNode.SelectNodes("/DelimitedClass/Fields/Field");
			}
			else
			{
				xmlNodeList = xmlNode.SelectNodes("/FixedLengthClass/Fields/Field");
			}
			foreach (object obj in xmlNodeList)
			{
				XmlNode xmlNode2 = (XmlNode)obj;
				classBuilder.ReadField(xmlNode2);
			}
			return classBuilder;
		}

		public static ClassBuilder LoadFromXml(string filename)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(filename);
			return ClassBuilder.LoadFromXml(xmlDocument);
		}

		public string SaveToXmlString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			using (StringWriter stringWriter = new StringWriter(stringBuilder))
			{
				this.SaveToXml(stringWriter);
			}
			return stringBuilder.ToString();
		}

		public void SaveToXml(string filename)
		{
			using (FileStream fileStream = new FileStream(filename, FileMode.Create))
			{
				this.SaveToXml(fileStream);
			}
		}

		public void SaveToXml(Stream stream)
		{
			using (TextWriter textWriter = new StreamWriter(stream))
			{
				this.SaveToXml(textWriter);
			}
		}

		public void SaveToXml(TextWriter writer)
		{
			XmlHelper xmlHelper = new XmlHelper();
			xmlHelper.BeginWriteStream(writer);
			this.WriteHeaderElement(xmlHelper);
			xmlHelper.WriteElement("ClassName", this.ClassName);
			xmlHelper.WriteElement("Namespace", this.Namespace, string.Empty);
			xmlHelper.WriteElement("SealedClass", this.SealedClass);
			xmlHelper.WriteElement("Visibility", this.Visibility.ToString(), "Public");
			xmlHelper.WriteElement("IgnoreEmptyLines", this.IgnoreEmptyLines);
			xmlHelper.WriteElement("IgnoreFirstLines", this.IgnoreFirstLines.ToString(), "0");
			xmlHelper.WriteElement("IgnoreLastLines", this.IgnoreLastLines.ToString(), "0");
			xmlHelper.WriteElement("CommentMarker", this.IgnoreCommentedLines.CommentMarker, string.Empty);
			xmlHelper.WriteElement("CommentInAnyPlace", this.IgnoreCommentedLines.InAnyPlace.ToString().ToLower(), true.ToString().ToLower());
			xmlHelper.WriteElement("RecordCondition", this.RecordCondition.Condition.ToString(), "None");
			xmlHelper.WriteElement("RecordConditionSelector", this.RecordCondition.Selector, string.Empty);
			this.WriteExtraElements(xmlHelper);
			xmlHelper.Writer.WriteStartElement("Fields");
			for (int i = 0; i < this.mFields.Count; i++)
			{
				((FieldBuilder)this.mFields[i]).SaveToXml(xmlHelper);
			}
			xmlHelper.Writer.WriteEndElement();
			xmlHelper.Writer.WriteEndElement();
			xmlHelper.EndWrite();
		}

		internal abstract void WriteHeaderElement(XmlHelper writer);

		internal abstract void WriteExtraElements(XmlHelper writer);

		internal abstract void ReadClassElements(XmlDocument document);

		internal abstract void ReadField(XmlNode node);

		public ClassBuilder.RecordConditionInfo RecordCondition
		{
			get
			{
				return this.mRecordConditionInfo;
			}
		}

		public ClassBuilder.IgnoreCommentInfo IgnoreCommentedLines
		{
			get
			{
				return this.mIgnoreCommentInfo;
			}
		}

		internal static string TypeToString(Type type)
		{
			if (type.IsGenericType)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(type.Name.Substring(0, type.Name.IndexOf("`", StringComparison.Ordinal)));
				stringBuilder.Append("<");
				Type[] genericArguments = type.GetGenericArguments();
				for (int i = 0; i < genericArguments.Length; i++)
				{
					if (i > 0)
					{
						stringBuilder.Append(",");
					}
					stringBuilder.Append(ClassBuilder.TypeToString(genericArguments[i]));
				}
				stringBuilder.Append(">");
				return stringBuilder.ToString();
			}
			return type.FullName;
		}

		private static string[] mReferences;

		private static readonly object mReferencesLock = new object();

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		internal ArrayList mFields = new ArrayList();

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string mClassName;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int mIgnoreFirstLines;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int mIgnoreLastLines;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool mIgnoreEmptyLines;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool mGenerateProperties;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string mCommentText;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private NetVisibility mVisibility;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool mSealedClass = true;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string mNamespace = string.Empty;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ClassBuilder.RecordConditionInfo mRecordConditionInfo = new ClassBuilder.RecordConditionInfo();

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ClassBuilder.IgnoreCommentInfo mIgnoreCommentInfo = new ClassBuilder.IgnoreCommentInfo();

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public sealed class RecordConditionInfo
		{
			internal RecordConditionInfo()
			{
			}

			public RecordCondition Condition
			{
				get
				{
					return this.mRecordCondition;
				}
				set
				{
					this.mRecordCondition = value;
				}
			}

			public string Selector
			{
				get
				{
					return this.mRecordConditionSelector;
				}
				set
				{
					this.mRecordConditionSelector = value;
				}
			}

			private RecordCondition mRecordCondition;

			private string mRecordConditionSelector = string.Empty;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public sealed class IgnoreCommentInfo
		{
			internal IgnoreCommentInfo()
			{
			}

			public string CommentMarker
			{
				get
				{
					return this.mMarker;
				}
				set
				{
					if (value != null)
					{
						value = value.Trim();
					}
					this.mMarker = value;
				}
			}

			public bool InAnyPlace
			{
				get
				{
					return this.mInAnyPlace;
				}
				set
				{
					this.mInAnyPlace = value;
				}
			}

			private string mMarker = string.Empty;

			private bool mInAnyPlace = true;
		}
	}
}
