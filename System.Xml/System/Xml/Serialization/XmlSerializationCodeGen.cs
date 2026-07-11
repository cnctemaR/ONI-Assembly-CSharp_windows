using System;
using System.Collections;

namespace System.Xml.Serialization
{
	internal class XmlSerializationCodeGen
	{
		internal XmlSerializationCodeGen(IndentedWriter writer, TypeScope[] scopes, string access, string className)
		{
			this.writer = writer;
			this.scopes = scopes;
			if (scopes.Length != 0)
			{
				this.stringTypeDesc = scopes[0].GetTypeDesc(typeof(string));
				this.qnameTypeDesc = scopes[0].GetTypeDesc(typeof(XmlQualifiedName));
			}
			this.raCodeGen = new ReflectionAwareCodeGen(writer);
			this.className = className;
			this.access = access;
		}

		internal IndentedWriter Writer
		{
			get
			{
				return this.writer;
			}
		}

		internal int NextMethodNumber
		{
			get
			{
				return this.nextMethodNumber;
			}
			set
			{
				this.nextMethodNumber = value;
			}
		}

		internal ReflectionAwareCodeGen RaCodeGen
		{
			get
			{
				return this.raCodeGen;
			}
		}

		internal TypeDesc StringTypeDesc
		{
			get
			{
				return this.stringTypeDesc;
			}
		}

		internal TypeDesc QnameTypeDesc
		{
			get
			{
				return this.qnameTypeDesc;
			}
		}

		internal string ClassName
		{
			get
			{
				return this.className;
			}
		}

		internal string Access
		{
			get
			{
				return this.access;
			}
		}

		internal TypeScope[] Scopes
		{
			get
			{
				return this.scopes;
			}
		}

		internal Hashtable MethodNames
		{
			get
			{
				return this.methodNames;
			}
		}

		internal Hashtable GeneratedMethods
		{
			get
			{
				return this.generatedMethods;
			}
		}

		internal virtual void GenerateMethod(TypeMapping mapping)
		{
		}

		internal void GenerateReferencedMethods()
		{
			while (this.references > 0)
			{
				TypeMapping[] array = this.referencedMethods;
				int num = this.references - 1;
				this.references = num;
				TypeMapping typeMapping = array[num];
				this.GenerateMethod(typeMapping);
			}
		}

		internal string ReferenceMapping(TypeMapping mapping)
		{
			if (!mapping.IsSoap && this.generatedMethods[mapping] == null)
			{
				this.referencedMethods = this.EnsureArrayIndex(this.referencedMethods, this.references);
				TypeMapping[] array = this.referencedMethods;
				int num = this.references;
				this.references = num + 1;
				array[num] = mapping;
			}
			return (string)this.methodNames[mapping];
		}

		private TypeMapping[] EnsureArrayIndex(TypeMapping[] a, int index)
		{
			if (a == null)
			{
				return new TypeMapping[32];
			}
			if (index < a.Length)
			{
				return a;
			}
			TypeMapping[] array = new TypeMapping[a.Length + 32];
			Array.Copy(a, array, index);
			return array;
		}

		internal void WriteQuotedCSharpString(string value)
		{
			this.raCodeGen.WriteQuotedCSharpString(value);
		}

		internal void GenerateHashtableGetBegin(string privateName, string publicName)
		{
			this.writer.Write(typeof(Hashtable).FullName);
			this.writer.Write(" ");
			this.writer.Write(privateName);
			this.writer.WriteLine(" = null;");
			this.writer.Write("public override ");
			this.writer.Write(typeof(Hashtable).FullName);
			this.writer.Write(" ");
			this.writer.Write(publicName);
			this.writer.WriteLine(" {");
			IndentedWriter indentedWriter = this.writer;
			int num = indentedWriter.Indent;
			indentedWriter.Indent = num + 1;
			this.writer.WriteLine("get {");
			IndentedWriter indentedWriter2 = this.writer;
			num = indentedWriter2.Indent;
			indentedWriter2.Indent = num + 1;
			this.writer.Write("if (");
			this.writer.Write(privateName);
			this.writer.WriteLine(" == null) {");
			IndentedWriter indentedWriter3 = this.writer;
			num = indentedWriter3.Indent;
			indentedWriter3.Indent = num + 1;
			this.writer.Write(typeof(Hashtable).FullName);
			this.writer.Write(" _tmp = new ");
			this.writer.Write(typeof(Hashtable).FullName);
			this.writer.WriteLine("();");
		}

		internal void GenerateHashtableGetEnd(string privateName)
		{
			this.writer.Write("if (");
			this.writer.Write(privateName);
			this.writer.Write(" == null) ");
			this.writer.Write(privateName);
			this.writer.WriteLine(" = _tmp;");
			IndentedWriter indentedWriter = this.writer;
			int num = indentedWriter.Indent;
			indentedWriter.Indent = num - 1;
			this.writer.WriteLine("}");
			this.writer.Write("return ");
			this.writer.Write(privateName);
			this.writer.WriteLine(";");
			IndentedWriter indentedWriter2 = this.writer;
			num = indentedWriter2.Indent;
			indentedWriter2.Indent = num - 1;
			this.writer.WriteLine("}");
			IndentedWriter indentedWriter3 = this.writer;
			num = indentedWriter3.Indent;
			indentedWriter3.Indent = num - 1;
			this.writer.WriteLine("}");
		}

		internal void GeneratePublicMethods(string privateName, string publicName, string[] methods, XmlMapping[] xmlMappings)
		{
			this.GenerateHashtableGetBegin(privateName, publicName);
			if (methods != null && methods.Length != 0 && xmlMappings != null && xmlMappings.Length == methods.Length)
			{
				for (int i = 0; i < methods.Length; i++)
				{
					if (methods[i] != null)
					{
						this.writer.Write("_tmp[");
						this.WriteQuotedCSharpString(xmlMappings[i].Key);
						this.writer.Write("] = ");
						this.WriteQuotedCSharpString(methods[i]);
						this.writer.WriteLine(";");
					}
				}
			}
			this.GenerateHashtableGetEnd(privateName);
		}

		internal void GenerateSupportedTypes(Type[] types)
		{
			this.writer.Write("public override ");
			this.writer.Write(typeof(bool).FullName);
			this.writer.Write(" CanSerialize(");
			this.writer.Write(typeof(Type).FullName);
			this.writer.WriteLine(" type) {");
			IndentedWriter indentedWriter = this.writer;
			int num = indentedWriter.Indent;
			indentedWriter.Indent = num + 1;
			Hashtable hashtable = new Hashtable();
			foreach (Type type in types)
			{
				if (!(type == null) && (type.IsPublic || type.IsNestedPublic) && hashtable[type] == null && !DynamicAssemblies.IsTypeDynamic(type) && !type.IsGenericType && (!type.ContainsGenericParameters || !DynamicAssemblies.IsTypeDynamic(type.GetGenericArguments())))
				{
					hashtable[type] = type;
					this.writer.Write("if (type == typeof(");
					this.writer.Write(CodeIdentifier.GetCSharpName(type));
					this.writer.WriteLine(")) return true;");
				}
			}
			this.writer.WriteLine("return false;");
			IndentedWriter indentedWriter2 = this.writer;
			num = indentedWriter2.Indent;
			indentedWriter2.Indent = num - 1;
			this.writer.WriteLine("}");
		}

		internal string GenerateBaseSerializer(string baseSerializer, string readerClass, string writerClass, CodeIdentifiers classes)
		{
			baseSerializer = CodeIdentifier.MakeValid(baseSerializer);
			baseSerializer = classes.AddUnique(baseSerializer, baseSerializer);
			this.writer.WriteLine();
			this.writer.Write("public abstract class ");
			this.writer.Write(CodeIdentifier.GetCSharpName(baseSerializer));
			this.writer.Write(" : ");
			this.writer.Write(typeof(XmlSerializer).FullName);
			this.writer.WriteLine(" {");
			IndentedWriter indentedWriter = this.writer;
			int num = indentedWriter.Indent;
			indentedWriter.Indent = num + 1;
			this.writer.Write("protected override ");
			this.writer.Write(typeof(XmlSerializationReader).FullName);
			this.writer.WriteLine(" CreateReader() {");
			IndentedWriter indentedWriter2 = this.writer;
			num = indentedWriter2.Indent;
			indentedWriter2.Indent = num + 1;
			this.writer.Write("return new ");
			this.writer.Write(readerClass);
			this.writer.WriteLine("();");
			IndentedWriter indentedWriter3 = this.writer;
			num = indentedWriter3.Indent;
			indentedWriter3.Indent = num - 1;
			this.writer.WriteLine("}");
			this.writer.Write("protected override ");
			this.writer.Write(typeof(XmlSerializationWriter).FullName);
			this.writer.WriteLine(" CreateWriter() {");
			IndentedWriter indentedWriter4 = this.writer;
			num = indentedWriter4.Indent;
			indentedWriter4.Indent = num + 1;
			this.writer.Write("return new ");
			this.writer.Write(writerClass);
			this.writer.WriteLine("();");
			IndentedWriter indentedWriter5 = this.writer;
			num = indentedWriter5.Indent;
			indentedWriter5.Indent = num - 1;
			this.writer.WriteLine("}");
			IndentedWriter indentedWriter6 = this.writer;
			num = indentedWriter6.Indent;
			indentedWriter6.Indent = num - 1;
			this.writer.WriteLine("}");
			return baseSerializer;
		}

		internal string GenerateTypedSerializer(string readMethod, string writeMethod, XmlMapping mapping, CodeIdentifiers classes, string baseSerializer, string readerClass, string writerClass)
		{
			string text = CodeIdentifier.MakeValid(Accessor.UnescapeName(mapping.Accessor.Mapping.TypeDesc.Name));
			text = classes.AddUnique(text + "Serializer", mapping);
			this.writer.WriteLine();
			this.writer.Write("public sealed class ");
			this.writer.Write(CodeIdentifier.GetCSharpName(text));
			this.writer.Write(" : ");
			this.writer.Write(baseSerializer);
			this.writer.WriteLine(" {");
			IndentedWriter indentedWriter = this.writer;
			int num = indentedWriter.Indent;
			indentedWriter.Indent = num + 1;
			this.writer.WriteLine();
			this.writer.Write("public override ");
			this.writer.Write(typeof(bool).FullName);
			this.writer.Write(" CanDeserialize(");
			this.writer.Write(typeof(XmlReader).FullName);
			this.writer.WriteLine(" xmlReader) {");
			IndentedWriter indentedWriter2 = this.writer;
			num = indentedWriter2.Indent;
			indentedWriter2.Indent = num + 1;
			if (mapping.Accessor.Any)
			{
				this.writer.WriteLine("return true;");
			}
			else
			{
				this.writer.Write("return xmlReader.IsStartElement(");
				this.WriteQuotedCSharpString(mapping.Accessor.Name);
				this.writer.Write(", ");
				this.WriteQuotedCSharpString(mapping.Accessor.Namespace);
				this.writer.WriteLine(");");
			}
			IndentedWriter indentedWriter3 = this.writer;
			num = indentedWriter3.Indent;
			indentedWriter3.Indent = num - 1;
			this.writer.WriteLine("}");
			if (writeMethod != null)
			{
				this.writer.WriteLine();
				this.writer.Write("protected override void Serialize(object objectToSerialize, ");
				this.writer.Write(typeof(XmlSerializationWriter).FullName);
				this.writer.WriteLine(" writer) {");
				IndentedWriter indentedWriter4 = this.writer;
				num = indentedWriter4.Indent;
				indentedWriter4.Indent = num + 1;
				this.writer.Write("((");
				this.writer.Write(writerClass);
				this.writer.Write(")writer).");
				this.writer.Write(writeMethod);
				this.writer.Write("(");
				if (mapping is XmlMembersMapping)
				{
					this.writer.Write("(object[])");
				}
				this.writer.WriteLine("objectToSerialize);");
				IndentedWriter indentedWriter5 = this.writer;
				num = indentedWriter5.Indent;
				indentedWriter5.Indent = num - 1;
				this.writer.WriteLine("}");
			}
			if (readMethod != null)
			{
				this.writer.WriteLine();
				this.writer.Write("protected override object Deserialize(");
				this.writer.Write(typeof(XmlSerializationReader).FullName);
				this.writer.WriteLine(" reader) {");
				IndentedWriter indentedWriter6 = this.writer;
				num = indentedWriter6.Indent;
				indentedWriter6.Indent = num + 1;
				this.writer.Write("return ((");
				this.writer.Write(readerClass);
				this.writer.Write(")reader).");
				this.writer.Write(readMethod);
				this.writer.WriteLine("();");
				IndentedWriter indentedWriter7 = this.writer;
				num = indentedWriter7.Indent;
				indentedWriter7.Indent = num - 1;
				this.writer.WriteLine("}");
			}
			IndentedWriter indentedWriter8 = this.writer;
			num = indentedWriter8.Indent;
			indentedWriter8.Indent = num - 1;
			this.writer.WriteLine("}");
			return text;
		}

		private void GenerateTypedSerializers(Hashtable serializers)
		{
			string text = "typedSerializers";
			this.GenerateHashtableGetBegin(text, "TypedSerializers");
			foreach (object obj in serializers.Keys)
			{
				string text2 = (string)obj;
				this.writer.Write("_tmp.Add(");
				this.WriteQuotedCSharpString(text2);
				this.writer.Write(", new ");
				this.writer.Write((string)serializers[text2]);
				this.writer.WriteLine("());");
			}
			this.GenerateHashtableGetEnd("typedSerializers");
		}

		private void GenerateGetSerializer(Hashtable serializers, XmlMapping[] xmlMappings)
		{
			this.writer.Write("public override ");
			this.writer.Write(typeof(XmlSerializer).FullName);
			this.writer.Write(" GetSerializer(");
			this.writer.Write(typeof(Type).FullName);
			this.writer.WriteLine(" type) {");
			IndentedWriter indentedWriter = this.writer;
			int num = indentedWriter.Indent;
			indentedWriter.Indent = num + 1;
			for (int i = 0; i < xmlMappings.Length; i++)
			{
				if (xmlMappings[i] is XmlTypeMapping)
				{
					Type type = xmlMappings[i].Accessor.Mapping.TypeDesc.Type;
					if (!(type == null) && (type.IsPublic || type.IsNestedPublic) && !DynamicAssemblies.IsTypeDynamic(type) && !type.IsGenericType && (!type.ContainsGenericParameters || !DynamicAssemblies.IsTypeDynamic(type.GetGenericArguments())))
					{
						this.writer.Write("if (type == typeof(");
						this.writer.Write(CodeIdentifier.GetCSharpName(type));
						this.writer.Write(")) return new ");
						this.writer.Write((string)serializers[xmlMappings[i].Key]);
						this.writer.WriteLine("();");
					}
				}
			}
			this.writer.WriteLine("return null;");
			IndentedWriter indentedWriter2 = this.writer;
			num = indentedWriter2.Indent;
			indentedWriter2.Indent = num - 1;
			this.writer.WriteLine("}");
		}

		internal void GenerateSerializerContract(string className, XmlMapping[] xmlMappings, Type[] types, string readerType, string[] readMethods, string writerType, string[] writerMethods, Hashtable serializers)
		{
			this.writer.WriteLine();
			this.writer.Write("public class XmlSerializerContract : global::");
			this.writer.Write(typeof(XmlSerializerImplementation).FullName);
			this.writer.WriteLine(" {");
			IndentedWriter indentedWriter = this.writer;
			int num = indentedWriter.Indent;
			indentedWriter.Indent = num + 1;
			this.writer.Write("public override global::");
			this.writer.Write(typeof(XmlSerializationReader).FullName);
			this.writer.Write(" Reader { get { return new ");
			this.writer.Write(readerType);
			this.writer.WriteLine("(); } }");
			this.writer.Write("public override global::");
			this.writer.Write(typeof(XmlSerializationWriter).FullName);
			this.writer.Write(" Writer { get { return new ");
			this.writer.Write(writerType);
			this.writer.WriteLine("(); } }");
			this.GeneratePublicMethods("readMethods", "ReadMethods", readMethods, xmlMappings);
			this.GeneratePublicMethods("writeMethods", "WriteMethods", writerMethods, xmlMappings);
			this.GenerateTypedSerializers(serializers);
			this.GenerateSupportedTypes(types);
			this.GenerateGetSerializer(serializers, xmlMappings);
			IndentedWriter indentedWriter2 = this.writer;
			num = indentedWriter2.Indent;
			indentedWriter2.Indent = num - 1;
			this.writer.WriteLine("}");
		}

		internal static bool IsWildcard(SpecialMapping mapping)
		{
			if (mapping is SerializableMapping)
			{
				return ((SerializableMapping)mapping).IsAny;
			}
			return mapping.TypeDesc.CanBeElementValue;
		}

		private IndentedWriter writer;

		private int nextMethodNumber;

		private Hashtable methodNames = new Hashtable();

		private ReflectionAwareCodeGen raCodeGen;

		private TypeScope[] scopes;

		private TypeDesc stringTypeDesc;

		private TypeDesc qnameTypeDesc;

		private string access;

		private string className;

		private TypeMapping[] referencedMethods;

		private int references;

		private Hashtable generatedMethods = new Hashtable();
	}
}
