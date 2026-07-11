using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;

namespace System.Xml.Serialization
{
	internal class XmlSerializationILGen
	{
		internal XmlSerializationILGen(TypeScope[] scopes, string access, string className)
		{
			this.scopes = scopes;
			if (scopes.Length != 0)
			{
				this.stringTypeDesc = scopes[0].GetTypeDesc(typeof(string));
				this.qnameTypeDesc = scopes[0].GetTypeDesc(typeof(XmlQualifiedName));
			}
			this.raCodeGen = new ReflectionAwareILGen();
			this.className = className;
			this.typeAttributes = TypeAttributes.Public;
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

		internal ReflectionAwareILGen RaCodeGen
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

		internal ModuleBuilder ModuleBuilder
		{
			get
			{
				return this.moduleBuilder;
			}
			set
			{
				this.moduleBuilder = value;
			}
		}

		internal TypeAttributes TypeAttributes
		{
			get
			{
				return this.typeAttributes;
			}
		}

		internal static Regex NewRegex(string pattern)
		{
			Dictionary<string, Regex> dictionary = XmlSerializationILGen.regexs;
			Regex regex;
			lock (dictionary)
			{
				if (!XmlSerializationILGen.regexs.TryGetValue(pattern, out regex))
				{
					regex = new Regex(pattern);
					XmlSerializationILGen.regexs.Add(pattern, regex);
				}
			}
			return regex;
		}

		internal MethodBuilder EnsureMethodBuilder(TypeBuilder typeBuilder, string methodName, MethodAttributes attributes, Type returnType, Type[] parameterTypes)
		{
			MethodBuilderInfo methodBuilderInfo;
			if (!this.methodBuilders.TryGetValue(methodName, out methodBuilderInfo))
			{
				methodBuilderInfo = new MethodBuilderInfo(typeBuilder.DefineMethod(methodName, attributes, returnType, parameterTypes), parameterTypes);
				this.methodBuilders.Add(methodName, methodBuilderInfo);
			}
			return methodBuilderInfo.MethodBuilder;
		}

		internal MethodBuilderInfo GetMethodBuilder(string methodName)
		{
			return this.methodBuilders[methodName];
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
			if (this.generatedMethods[mapping] == null)
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

		internal FieldBuilder GenerateHashtableGetBegin(string privateName, string publicName, TypeBuilder serializerContractTypeBuilder)
		{
			FieldBuilder fieldBuilder = serializerContractTypeBuilder.DefineField(privateName, typeof(Hashtable), FieldAttributes.Private);
			this.ilg = new CodeGenerator(serializerContractTypeBuilder);
			PropertyBuilder propertyBuilder = serializerContractTypeBuilder.DefineProperty(publicName, PropertyAttributes.None, CallingConventions.HasThis, typeof(Hashtable), null, null, null, null, null);
			this.ilg.BeginMethod(typeof(Hashtable), "get_" + publicName, CodeGenerator.EmptyTypeArray, CodeGenerator.EmptyStringArray, CodeGenerator.PublicOverrideMethodAttributes | MethodAttributes.SpecialName);
			propertyBuilder.SetGetMethod(this.ilg.MethodBuilder);
			this.ilg.Ldarg(0);
			this.ilg.LoadMember(fieldBuilder);
			this.ilg.Load(null);
			this.ilg.If(Cmp.EqualTo);
			ConstructorInfo constructor = typeof(Hashtable).GetConstructor(CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
			LocalBuilder localBuilder = this.ilg.DeclareLocal(typeof(Hashtable), "_tmp");
			this.ilg.New(constructor);
			this.ilg.Stloc(localBuilder);
			return fieldBuilder;
		}

		internal void GenerateHashtableGetEnd(FieldBuilder fieldBuilder)
		{
			this.ilg.Ldarg(0);
			this.ilg.LoadMember(fieldBuilder);
			this.ilg.Load(null);
			this.ilg.If(Cmp.EqualTo);
			this.ilg.Ldarg(0);
			this.ilg.Ldloc(typeof(Hashtable), "_tmp");
			this.ilg.StoreMember(fieldBuilder);
			this.ilg.EndIf();
			this.ilg.EndIf();
			this.ilg.Ldarg(0);
			this.ilg.LoadMember(fieldBuilder);
			this.ilg.GotoMethodEnd();
			this.ilg.EndMethod();
		}

		internal FieldBuilder GeneratePublicMethods(string privateName, string publicName, string[] methods, XmlMapping[] xmlMappings, TypeBuilder serializerContractTypeBuilder)
		{
			FieldBuilder fieldBuilder = this.GenerateHashtableGetBegin(privateName, publicName, serializerContractTypeBuilder);
			if (methods != null && methods.Length != 0 && xmlMappings != null && xmlMappings.Length == methods.Length)
			{
				MethodInfo method = typeof(Hashtable).GetMethod("set_Item", CodeGenerator.InstanceBindingFlags, null, new Type[]
				{
					typeof(object),
					typeof(object)
				}, null);
				for (int i = 0; i < methods.Length; i++)
				{
					if (methods[i] != null)
					{
						this.ilg.Ldloc(typeof(Hashtable), "_tmp");
						this.ilg.Ldstr(xmlMappings[i].Key);
						this.ilg.Ldstr(methods[i]);
						this.ilg.Call(method);
					}
				}
			}
			this.GenerateHashtableGetEnd(fieldBuilder);
			return fieldBuilder;
		}

		internal void GenerateSupportedTypes(Type[] types, TypeBuilder serializerContractTypeBuilder)
		{
			this.ilg = new CodeGenerator(serializerContractTypeBuilder);
			this.ilg.BeginMethod(typeof(bool), "CanSerialize", new Type[] { typeof(Type) }, new string[] { "type" }, CodeGenerator.PublicOverrideMethodAttributes);
			Hashtable hashtable = new Hashtable();
			foreach (Type type in types)
			{
				if (!(type == null) && (type.IsPublic || type.IsNestedPublic) && hashtable[type] == null && !type.IsGenericType && !type.ContainsGenericParameters)
				{
					hashtable[type] = type;
					this.ilg.Ldarg("type");
					this.ilg.Ldc(type);
					this.ilg.If(Cmp.EqualTo);
					this.ilg.Ldc(true);
					this.ilg.GotoMethodEnd();
					this.ilg.EndIf();
				}
			}
			this.ilg.Ldc(false);
			this.ilg.GotoMethodEnd();
			this.ilg.EndMethod();
		}

		internal string GenerateBaseSerializer(string baseSerializer, string readerClass, string writerClass, CodeIdentifiers classes)
		{
			baseSerializer = CodeIdentifier.MakeValid(baseSerializer);
			baseSerializer = classes.AddUnique(baseSerializer, baseSerializer);
			TypeBuilder typeBuilder = CodeGenerator.CreateTypeBuilder(this.moduleBuilder, CodeIdentifier.GetCSharpName(baseSerializer), TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.BeforeFieldInit, typeof(XmlSerializer), CodeGenerator.EmptyTypeArray);
			ConstructorInfo constructor = this.CreatedTypes[readerClass].GetConstructor(CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
			this.ilg = new CodeGenerator(typeBuilder);
			this.ilg.BeginMethod(typeof(XmlSerializationReader), "CreateReader", CodeGenerator.EmptyTypeArray, CodeGenerator.EmptyStringArray, CodeGenerator.ProtectedOverrideMethodAttributes);
			this.ilg.New(constructor);
			this.ilg.EndMethod();
			ConstructorInfo constructor2 = this.CreatedTypes[writerClass].GetConstructor(CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
			this.ilg.BeginMethod(typeof(XmlSerializationWriter), "CreateWriter", CodeGenerator.EmptyTypeArray, CodeGenerator.EmptyStringArray, CodeGenerator.ProtectedOverrideMethodAttributes);
			this.ilg.New(constructor2);
			this.ilg.EndMethod();
			typeBuilder.DefineDefaultConstructor(MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
			Type type = typeBuilder.CreateType();
			this.CreatedTypes.Add(type.Name, type);
			return baseSerializer;
		}

		internal string GenerateTypedSerializer(string readMethod, string writeMethod, XmlMapping mapping, CodeIdentifiers classes, string baseSerializer, string readerClass, string writerClass)
		{
			string text = CodeIdentifier.MakeValid(Accessor.UnescapeName(mapping.Accessor.Mapping.TypeDesc.Name));
			text = classes.AddUnique(text + "Serializer", mapping);
			TypeBuilder typeBuilder = CodeGenerator.CreateTypeBuilder(this.moduleBuilder, CodeIdentifier.GetCSharpName(text), TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit, this.CreatedTypes[baseSerializer], CodeGenerator.EmptyTypeArray);
			this.ilg = new CodeGenerator(typeBuilder);
			this.ilg.BeginMethod(typeof(bool), "CanDeserialize", new Type[] { typeof(XmlReader) }, new string[] { "xmlReader" }, CodeGenerator.PublicOverrideMethodAttributes);
			if (mapping.Accessor.Any)
			{
				this.ilg.Ldc(true);
				this.ilg.Stloc(this.ilg.ReturnLocal);
				this.ilg.Br(this.ilg.ReturnLabel);
			}
			else
			{
				MethodInfo method = typeof(XmlReader).GetMethod("IsStartElement", CodeGenerator.InstanceBindingFlags, null, new Type[]
				{
					typeof(string),
					typeof(string)
				}, null);
				this.ilg.Ldarg(this.ilg.GetArg("xmlReader"));
				this.ilg.Ldstr(mapping.Accessor.Name);
				this.ilg.Ldstr(mapping.Accessor.Namespace);
				this.ilg.Call(method);
				this.ilg.Stloc(this.ilg.ReturnLocal);
				this.ilg.Br(this.ilg.ReturnLabel);
			}
			this.ilg.MarkLabel(this.ilg.ReturnLabel);
			this.ilg.Ldloc(this.ilg.ReturnLocal);
			this.ilg.EndMethod();
			if (writeMethod != null)
			{
				this.ilg = new CodeGenerator(typeBuilder);
				this.ilg.BeginMethod(typeof(void), "Serialize", new Type[]
				{
					typeof(object),
					typeof(XmlSerializationWriter)
				}, new string[] { "objectToSerialize", "writer" }, CodeGenerator.ProtectedOverrideMethodAttributes);
				MethodInfo method2 = this.CreatedTypes[writerClass].GetMethod(writeMethod, CodeGenerator.InstanceBindingFlags, null, new Type[] { (mapping is XmlMembersMapping) ? typeof(object[]) : typeof(object) }, null);
				this.ilg.Ldarg("writer");
				this.ilg.Castclass(this.CreatedTypes[writerClass]);
				this.ilg.Ldarg("objectToSerialize");
				if (mapping is XmlMembersMapping)
				{
					this.ilg.ConvertValue(typeof(object), typeof(object[]));
				}
				this.ilg.Call(method2);
				this.ilg.EndMethod();
			}
			if (readMethod != null)
			{
				this.ilg = new CodeGenerator(typeBuilder);
				this.ilg.BeginMethod(typeof(object), "Deserialize", new Type[] { typeof(XmlSerializationReader) }, new string[] { "reader" }, CodeGenerator.ProtectedOverrideMethodAttributes);
				MethodInfo method3 = this.CreatedTypes[readerClass].GetMethod(readMethod, CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
				this.ilg.Ldarg("reader");
				this.ilg.Castclass(this.CreatedTypes[readerClass]);
				this.ilg.Call(method3);
				this.ilg.EndMethod();
			}
			typeBuilder.DefineDefaultConstructor(CodeGenerator.PublicMethodAttributes);
			Type type = typeBuilder.CreateType();
			this.CreatedTypes.Add(type.Name, type);
			return type.Name;
		}

		private FieldBuilder GenerateTypedSerializers(Hashtable serializers, TypeBuilder serializerContractTypeBuilder)
		{
			string text = "typedSerializers";
			FieldBuilder fieldBuilder = this.GenerateHashtableGetBegin(text, "TypedSerializers", serializerContractTypeBuilder);
			MethodInfo method = typeof(Hashtable).GetMethod("Add", CodeGenerator.InstanceBindingFlags, null, new Type[]
			{
				typeof(object),
				typeof(object)
			}, null);
			foreach (object obj in serializers.Keys)
			{
				string text2 = (string)obj;
				ConstructorInfo constructor = this.CreatedTypes[(string)serializers[text2]].GetConstructor(CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
				this.ilg.Ldloc(typeof(Hashtable), "_tmp");
				this.ilg.Ldstr(text2);
				this.ilg.New(constructor);
				this.ilg.Call(method);
			}
			this.GenerateHashtableGetEnd(fieldBuilder);
			return fieldBuilder;
		}

		private void GenerateGetSerializer(Hashtable serializers, XmlMapping[] xmlMappings, TypeBuilder serializerContractTypeBuilder)
		{
			this.ilg = new CodeGenerator(serializerContractTypeBuilder);
			this.ilg.BeginMethod(typeof(XmlSerializer), "GetSerializer", new Type[] { typeof(Type) }, new string[] { "type" }, CodeGenerator.PublicOverrideMethodAttributes);
			for (int i = 0; i < xmlMappings.Length; i++)
			{
				if (xmlMappings[i] is XmlTypeMapping)
				{
					Type type = xmlMappings[i].Accessor.Mapping.TypeDesc.Type;
					if (!(type == null) && (type.IsPublic || type.IsNestedPublic) && !type.IsGenericType && !type.ContainsGenericParameters)
					{
						this.ilg.Ldarg("type");
						this.ilg.Ldc(type);
						this.ilg.If(Cmp.EqualTo);
						ConstructorInfo constructor = this.CreatedTypes[(string)serializers[xmlMappings[i].Key]].GetConstructor(CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
						this.ilg.New(constructor);
						this.ilg.Stloc(this.ilg.ReturnLocal);
						this.ilg.Br(this.ilg.ReturnLabel);
						this.ilg.EndIf();
					}
				}
			}
			this.ilg.Load(null);
			this.ilg.Stloc(this.ilg.ReturnLocal);
			this.ilg.Br(this.ilg.ReturnLabel);
			this.ilg.MarkLabel(this.ilg.ReturnLabel);
			this.ilg.Ldloc(this.ilg.ReturnLocal);
			this.ilg.EndMethod();
		}

		internal void GenerateSerializerContract(string className, XmlMapping[] xmlMappings, Type[] types, string readerType, string[] readMethods, string writerType, string[] writerMethods, Hashtable serializers)
		{
			TypeBuilder typeBuilder = CodeGenerator.CreateTypeBuilder(this.moduleBuilder, "XmlSerializerContract", TypeAttributes.Public | TypeAttributes.BeforeFieldInit, typeof(XmlSerializerImplementation), CodeGenerator.EmptyTypeArray);
			this.ilg = new CodeGenerator(typeBuilder);
			PropertyBuilder propertyBuilder = typeBuilder.DefineProperty("Reader", PropertyAttributes.None, CallingConventions.HasThis, typeof(XmlSerializationReader), null, null, null, null, null);
			this.ilg.BeginMethod(typeof(XmlSerializationReader), "get_Reader", CodeGenerator.EmptyTypeArray, CodeGenerator.EmptyStringArray, CodeGenerator.PublicOverrideMethodAttributes | MethodAttributes.SpecialName);
			propertyBuilder.SetGetMethod(this.ilg.MethodBuilder);
			ConstructorInfo constructorInfo = this.CreatedTypes[readerType].GetConstructor(CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
			this.ilg.New(constructorInfo);
			this.ilg.EndMethod();
			this.ilg = new CodeGenerator(typeBuilder);
			PropertyBuilder propertyBuilder2 = typeBuilder.DefineProperty("Writer", PropertyAttributes.None, CallingConventions.HasThis, typeof(XmlSerializationWriter), null, null, null, null, null);
			this.ilg.BeginMethod(typeof(XmlSerializationWriter), "get_Writer", CodeGenerator.EmptyTypeArray, CodeGenerator.EmptyStringArray, CodeGenerator.PublicOverrideMethodAttributes | MethodAttributes.SpecialName);
			propertyBuilder2.SetGetMethod(this.ilg.MethodBuilder);
			constructorInfo = this.CreatedTypes[writerType].GetConstructor(CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
			this.ilg.New(constructorInfo);
			this.ilg.EndMethod();
			FieldBuilder fieldBuilder = this.GeneratePublicMethods("readMethods", "ReadMethods", readMethods, xmlMappings, typeBuilder);
			FieldBuilder fieldBuilder2 = this.GeneratePublicMethods("writeMethods", "WriteMethods", writerMethods, xmlMappings, typeBuilder);
			FieldBuilder fieldBuilder3 = this.GenerateTypedSerializers(serializers, typeBuilder);
			this.GenerateSupportedTypes(types, typeBuilder);
			this.GenerateGetSerializer(serializers, xmlMappings, typeBuilder);
			ConstructorInfo constructor = typeof(XmlSerializerImplementation).GetConstructor(CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
			this.ilg = new CodeGenerator(typeBuilder);
			this.ilg.BeginMethod(typeof(void), ".ctor", CodeGenerator.EmptyTypeArray, CodeGenerator.EmptyStringArray, CodeGenerator.PublicMethodAttributes | MethodAttributes.RTSpecialName | MethodAttributes.SpecialName);
			this.ilg.Ldarg(0);
			this.ilg.Load(null);
			this.ilg.StoreMember(fieldBuilder);
			this.ilg.Ldarg(0);
			this.ilg.Load(null);
			this.ilg.StoreMember(fieldBuilder2);
			this.ilg.Ldarg(0);
			this.ilg.Load(null);
			this.ilg.StoreMember(fieldBuilder3);
			this.ilg.Ldarg(0);
			this.ilg.Call(constructor);
			this.ilg.EndMethod();
			Type type = typeBuilder.CreateType();
			this.CreatedTypes.Add(type.Name, type);
		}

		internal static bool IsWildcard(SpecialMapping mapping)
		{
			if (mapping is SerializableMapping)
			{
				return ((SerializableMapping)mapping).IsAny;
			}
			return mapping.TypeDesc.CanBeElementValue;
		}

		internal void ILGenLoad(string source)
		{
			this.ILGenLoad(source, null);
		}

		internal void ILGenLoad(string source, Type type)
		{
			if (source.StartsWith("o.@", StringComparison.Ordinal))
			{
				MemberInfo memberInfo = this.memberInfos[source.Substring(3)];
				this.ilg.LoadMember(this.ilg.GetVariable("o"), memberInfo);
				if (type != null)
				{
					Type type2 = ((memberInfo.MemberType == MemberTypes.Field) ? ((FieldInfo)memberInfo).FieldType : ((PropertyInfo)memberInfo).PropertyType);
					this.ilg.ConvertValue(type2, type);
					return;
				}
			}
			else
			{
				new SourceInfo(source, null, null, null, this.ilg).Load(type);
			}
		}

		private int nextMethodNumber;

		private Hashtable methodNames = new Hashtable();

		private Dictionary<string, MethodBuilderInfo> methodBuilders = new Dictionary<string, MethodBuilderInfo>();

		internal Dictionary<string, Type> CreatedTypes = new Dictionary<string, Type>();

		internal Dictionary<string, MemberInfo> memberInfos = new Dictionary<string, MemberInfo>();

		private ReflectionAwareILGen raCodeGen;

		private TypeScope[] scopes;

		private TypeDesc stringTypeDesc;

		private TypeDesc qnameTypeDesc;

		private string className;

		private TypeMapping[] referencedMethods;

		private int references;

		private Hashtable generatedMethods = new Hashtable();

		private ModuleBuilder moduleBuilder;

		private TypeAttributes typeAttributes;

		protected TypeBuilder typeBuilder;

		protected CodeGenerator ilg;

		private static Dictionary<string, Regex> regexs = new Dictionary<string, Regex>();
	}
}
