using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Security;
using System.Text.RegularExpressions;
using System.Xml.Schema;

namespace System.Xml.Serialization
{
	internal class XmlSerializationReaderILGen : XmlSerializationILGen
	{
		internal Hashtable Enums
		{
			get
			{
				if (this.enums == null)
				{
					this.enums = new Hashtable();
				}
				return this.enums;
			}
		}

		internal XmlSerializationReaderILGen(TypeScope[] scopes, string access, string className)
			: base(scopes, access, className)
		{
		}

		internal void GenerateBegin()
		{
			this.typeBuilder = CodeGenerator.CreateTypeBuilder(base.ModuleBuilder, base.ClassName, base.TypeAttributes | TypeAttributes.BeforeFieldInit, typeof(XmlSerializationReader), CodeGenerator.EmptyTypeArray);
			foreach (TypeScope typeScope in base.Scopes)
			{
				foreach (object obj in typeScope.TypeMappings)
				{
					TypeMapping typeMapping = (TypeMapping)obj;
					if (typeMapping is StructMapping || typeMapping is EnumMapping || typeMapping is NullableMapping)
					{
						base.MethodNames.Add(typeMapping, this.NextMethodName(typeMapping.TypeDesc.Name));
					}
				}
				base.RaCodeGen.WriteReflectionInit(typeScope);
			}
		}

		internal override void GenerateMethod(TypeMapping mapping)
		{
			if (base.GeneratedMethods.Contains(mapping))
			{
				return;
			}
			base.GeneratedMethods[mapping] = mapping;
			if (mapping is StructMapping)
			{
				this.WriteStructMethod((StructMapping)mapping);
				return;
			}
			if (mapping is EnumMapping)
			{
				this.WriteEnumMethod((EnumMapping)mapping);
				return;
			}
			if (mapping is NullableMapping)
			{
				this.WriteNullableMethod((NullableMapping)mapping);
			}
		}

		internal void GenerateEnd(string[] methods, XmlMapping[] xmlMappings, Type[] types)
		{
			base.GenerateReferencedMethods();
			this.GenerateInitCallbacksMethod();
			this.ilg = new CodeGenerator(this.typeBuilder);
			this.ilg.BeginMethod(typeof(void), "InitIDs", CodeGenerator.EmptyTypeArray, CodeGenerator.EmptyStringArray, CodeGenerator.ProtectedOverrideMethodAttributes);
			MethodInfo method = typeof(XmlSerializationReader).GetMethod("get_Reader", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
			MethodInfo method2 = typeof(XmlReader).GetMethod("get_NameTable", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
			MethodInfo method3 = typeof(XmlNameTable).GetMethod("Add", CodeGenerator.InstanceBindingFlags, null, new Type[] { typeof(string) }, null);
			foreach (object obj in this.idNames.Keys)
			{
				string text = (string)obj;
				this.ilg.Ldarg(0);
				this.ilg.Ldarg(0);
				this.ilg.Call(method);
				this.ilg.Call(method2);
				this.ilg.Ldstr(text);
				this.ilg.Call(method3);
				this.ilg.StoreMember(this.idNameFields[text]);
			}
			this.ilg.EndMethod();
			this.typeBuilder.DefineDefaultConstructor(CodeGenerator.PublicMethodAttributes);
			Type type = this.typeBuilder.CreateType();
			this.CreatedTypes.Add(type.Name, type);
		}

		internal string GenerateElement(XmlMapping xmlMapping)
		{
			if (!xmlMapping.IsReadable)
			{
				return null;
			}
			if (!xmlMapping.GenerateSerializer)
			{
				throw new ArgumentException(Res.GetString("Internal error."), "xmlMapping");
			}
			if (xmlMapping is XmlTypeMapping)
			{
				return this.GenerateTypeElement((XmlTypeMapping)xmlMapping);
			}
			if (xmlMapping is XmlMembersMapping)
			{
				return this.GenerateMembersElement((XmlMembersMapping)xmlMapping);
			}
			throw new ArgumentException(Res.GetString("Internal error."), "xmlMapping");
		}

		private void WriteIsStartTag(string name, string ns)
		{
			this.WriteID(name);
			this.WriteID(ns);
			MethodInfo method = typeof(XmlSerializationReader).GetMethod("get_Reader", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
			MethodInfo method2 = typeof(XmlReader).GetMethod("IsStartElement", CodeGenerator.InstanceBindingFlags, null, new Type[]
			{
				typeof(string),
				typeof(string)
			}, null);
			this.ilg.Ldarg(0);
			this.ilg.Call(method);
			this.ilg.Ldarg(0);
			this.ilg.LoadMember(this.idNameFields[name ?? string.Empty]);
			this.ilg.Ldarg(0);
			this.ilg.LoadMember(this.idNameFields[ns ?? string.Empty]);
			this.ilg.Call(method2);
			this.ilg.If();
		}

		private void WriteUnknownNode(string func, string node, ElementAccessor e, bool anyIfs)
		{
			if (anyIfs)
			{
				this.ilg.Else();
			}
			List<Type> list = new List<Type>();
			this.ilg.Ldarg(0);
			if (node == "null")
			{
				this.ilg.Load(null);
			}
			else
			{
				object variable = this.ilg.GetVariable("p");
				this.ilg.Load(variable);
				this.ilg.ConvertValue(this.ilg.GetVariableType(variable), typeof(object));
			}
			list.Add(typeof(object));
			if (e != null)
			{
				string text = ((e.Form == XmlSchemaForm.Qualified) ? e.Namespace : "");
				text += ":";
				text += e.Name;
				this.ilg.Ldstr(ReflectionAwareILGen.GetCSharpString(text));
				list.Add(typeof(string));
			}
			MethodInfo method = typeof(XmlSerializationReader).GetMethod(func, CodeGenerator.InstanceBindingFlags, null, list.ToArray(), null);
			this.ilg.Call(method);
			if (anyIfs)
			{
				this.ilg.EndIf();
			}
		}

		private void GenerateInitCallbacksMethod()
		{
			this.ilg = new CodeGenerator(this.typeBuilder);
			this.ilg.BeginMethod(typeof(void), "InitCallbacks", CodeGenerator.EmptyTypeArray, CodeGenerator.EmptyStringArray, CodeGenerator.ProtectedOverrideMethodAttributes);
			string text = this.NextMethodName("Array");
			bool flag = false;
			this.ilg.EndMethod();
			if (flag)
			{
				this.ilg.BeginMethod(typeof(object), base.GetMethodBuilder(text), CodeGenerator.EmptyTypeArray, CodeGenerator.EmptyStringArray, CodeGenerator.PrivateMethodAttributes);
				MethodInfo method = typeof(XmlSerializationReader).GetMethod("UnknownNode", CodeGenerator.InstanceBindingFlags, null, new Type[] { typeof(object) }, null);
				this.ilg.Ldarg(0);
				this.ilg.Load(null);
				this.ilg.Call(method);
				this.ilg.Load(null);
				this.ilg.EndMethod();
			}
		}

		private string GenerateMembersElement(XmlMembersMapping xmlMembersMapping)
		{
			return this.GenerateLiteralMembersElement(xmlMembersMapping);
		}

		private string GetChoiceIdentifierSource(MemberMapping[] mappings, MemberMapping member)
		{
			string text = null;
			if (member.ChoiceIdentifier != null)
			{
				for (int i = 0; i < mappings.Length; i++)
				{
					if (mappings[i].Name == member.ChoiceIdentifier.MemberName)
					{
						text = "p[" + i.ToString(CultureInfo.InvariantCulture) + "]";
						break;
					}
				}
			}
			return text;
		}

		private string GetChoiceIdentifierSource(MemberMapping mapping, string parent, TypeDesc parentTypeDesc)
		{
			if (mapping.ChoiceIdentifier == null)
			{
				return "";
			}
			CodeIdentifier.CheckValidIdentifier(mapping.ChoiceIdentifier.MemberName);
			return base.RaCodeGen.GetStringForMember(parent, mapping.ChoiceIdentifier.MemberName, parentTypeDesc);
		}

		private string GenerateLiteralMembersElement(XmlMembersMapping xmlMembersMapping)
		{
			ElementAccessor accessor = xmlMembersMapping.Accessor;
			MemberMapping[] members = ((MembersMapping)accessor.Mapping).Members;
			bool hasWrapperElement = ((MembersMapping)accessor.Mapping).HasWrapperElement;
			string text = this.NextMethodName(accessor.Name);
			this.ilg = new CodeGenerator(this.typeBuilder);
			this.ilg.BeginMethod(typeof(object[]), text, CodeGenerator.EmptyTypeArray, CodeGenerator.EmptyStringArray, CodeGenerator.PublicMethodAttributes);
			this.ilg.Load(null);
			this.ilg.Stloc(this.ilg.ReturnLocal);
			MethodInfo method = typeof(XmlSerializationReader).GetMethod("get_Reader", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
			MethodInfo method2 = typeof(XmlReader).GetMethod("MoveToContent", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
			this.ilg.Ldarg(0);
			this.ilg.Call(method);
			this.ilg.Call(method2);
			this.ilg.Pop();
			LocalBuilder localBuilder = this.ilg.DeclareLocal(typeof(object[]), "p");
			this.ilg.NewArray(typeof(object), members.Length);
			this.ilg.Stloc(localBuilder);
			this.InitializeValueTypes("p", members);
			int num = 0;
			if (hasWrapperElement)
			{
				num = this.WriteWhileNotLoopStart();
				this.WriteIsStartTag(accessor.Name, (accessor.Form == XmlSchemaForm.Qualified) ? accessor.Namespace : "");
			}
			XmlSerializationReaderILGen.Member member = null;
			XmlSerializationReaderILGen.Member member2 = null;
			XmlSerializationReaderILGen.Member member3 = null;
			ArrayList arrayList = new ArrayList();
			ArrayList arrayList2 = new ArrayList();
			ArrayList arrayList3 = new ArrayList();
			for (int i = 0; i < members.Length; i++)
			{
				MemberMapping memberMapping = members[i];
				string text2 = "p[" + i.ToString(CultureInfo.InvariantCulture) + "]";
				string text3 = text2;
				if (memberMapping.Xmlns != null)
				{
					text3 = string.Concat(new string[]
					{
						"((",
						memberMapping.TypeDesc.CSharpName,
						")",
						text2,
						")"
					});
				}
				string choiceIdentifierSource = this.GetChoiceIdentifierSource(members, memberMapping);
				XmlSerializationReaderILGen.Member member4 = new XmlSerializationReaderILGen.Member(this, text2, text3, "a", i, memberMapping, choiceIdentifierSource);
				XmlSerializationReaderILGen.Member member5 = new XmlSerializationReaderILGen.Member(this, text2, null, "a", i, memberMapping, choiceIdentifierSource);
				if (!memberMapping.IsSequence)
				{
					member4.ParamsReadSource = "paramsRead[" + i.ToString(CultureInfo.InvariantCulture) + "]";
				}
				if (memberMapping.CheckSpecified == SpecifiedAccessor.ReadWrite)
				{
					string text4 = memberMapping.Name + "Specified";
					for (int j = 0; j < members.Length; j++)
					{
						if (members[j].Name == text4)
						{
							member4.CheckSpecifiedSource = "p[" + j.ToString(CultureInfo.InvariantCulture) + "]";
							break;
						}
					}
				}
				bool flag = false;
				if (memberMapping.Text != null)
				{
					member = member5;
				}
				if (memberMapping.Attribute != null && memberMapping.Attribute.Any)
				{
					member3 = member5;
				}
				if (memberMapping.Attribute != null || memberMapping.Xmlns != null)
				{
					arrayList3.Add(member4);
				}
				else if (memberMapping.Text != null)
				{
					arrayList2.Add(member4);
				}
				if (!memberMapping.IsSequence)
				{
					for (int k = 0; k < memberMapping.Elements.Length; k++)
					{
						if (memberMapping.Elements[k].Any && memberMapping.Elements[k].Name.Length == 0)
						{
							member2 = member5;
							if (memberMapping.Attribute == null && memberMapping.Text == null)
							{
								arrayList2.Add(member5);
							}
							flag = true;
							break;
						}
					}
				}
				if (memberMapping.Attribute != null || memberMapping.Text != null || flag)
				{
					arrayList.Add(member5);
				}
				else if (memberMapping.TypeDesc.IsArrayLike && (memberMapping.Elements.Length != 1 || !(memberMapping.Elements[0].Mapping is ArrayMapping)))
				{
					arrayList.Add(member5);
					arrayList2.Add(member5);
				}
				else
				{
					if (memberMapping.TypeDesc.IsArrayLike && !memberMapping.TypeDesc.IsArray)
					{
						member4.ParamsReadSource = null;
					}
					arrayList.Add(member4);
				}
			}
			XmlSerializationReaderILGen.Member[] array = (XmlSerializationReaderILGen.Member[])arrayList.ToArray(typeof(XmlSerializationReaderILGen.Member));
			XmlSerializationReaderILGen.Member[] array2 = (XmlSerializationReaderILGen.Member[])arrayList2.ToArray(typeof(XmlSerializationReaderILGen.Member));
			if (array.Length != 0 && array[0].Mapping.IsReturnValue)
			{
				MethodInfo method3 = typeof(XmlSerializationReader).GetMethod("set_IsReturnValue", CodeGenerator.InstanceBindingFlags, null, new Type[] { typeof(bool) }, null);
				this.ilg.Ldarg(0);
				this.ilg.Ldc(true);
				this.ilg.Call(method3);
			}
			this.WriteParamsRead(members.Length);
			if (arrayList3.Count > 0)
			{
				XmlSerializationReaderILGen.Member[] array3 = (XmlSerializationReaderILGen.Member[])arrayList3.ToArray(typeof(XmlSerializationReaderILGen.Member));
				this.WriteMemberBegin(array3);
				this.WriteAttributes(array3, member3, "UnknownNode", localBuilder);
				this.WriteMemberEnd(array3);
				MethodInfo method4 = typeof(XmlReader).GetMethod("MoveToElement", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
				this.ilg.Ldarg(0);
				this.ilg.Call(method);
				this.ilg.Call(method4);
				this.ilg.Pop();
			}
			this.WriteMemberBegin(array2);
			if (hasWrapperElement)
			{
				MethodInfo method5 = typeof(XmlReader).GetMethod("get_IsEmptyElement", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
				this.ilg.Ldarg(0);
				this.ilg.Call(method);
				this.ilg.Call(method5);
				this.ilg.If();
				MethodInfo method6 = typeof(XmlReader).GetMethod("Skip", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
				this.ilg.Ldarg(0);
				this.ilg.Call(method);
				this.ilg.Call(method6);
				this.ilg.Ldarg(0);
				this.ilg.Call(method);
				this.ilg.Call(method2);
				this.ilg.Pop();
				this.ilg.WhileContinue();
				this.ilg.EndIf();
				MethodInfo method7 = typeof(XmlReader).GetMethod("ReadStartElement", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
				this.ilg.Ldarg(0);
				this.ilg.Call(method);
				this.ilg.Call(method7);
			}
			if (this.IsSequence(array))
			{
				this.ilg.Ldc(0);
				this.ilg.Stloc(typeof(int), "state");
			}
			int num2 = this.WriteWhileNotLoopStart();
			string text5 = "UnknownNode((object)p, " + this.ExpectedElements(array) + ");";
			this.WriteMemberElements(array, text5, text5, member2, member);
			this.ilg.Ldarg(0);
			this.ilg.Call(method);
			this.ilg.Call(method2);
			this.ilg.Pop();
			this.WriteWhileLoopEnd(num2);
			this.WriteMemberEnd(array2);
			if (hasWrapperElement)
			{
				MethodInfo method8 = typeof(XmlSerializationReader).GetMethod("ReadEndElement", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
				this.ilg.Ldarg(0);
				this.ilg.Call(method8);
				this.WriteUnknownNode("UnknownNode", "null", accessor, true);
				this.ilg.Ldarg(0);
				this.ilg.Call(method);
				this.ilg.Call(method2);
				this.ilg.Pop();
				this.WriteWhileLoopEnd(num);
			}
			this.ilg.Ldloc(this.ilg.GetLocal("p"));
			this.ilg.EndMethod();
			return text;
		}

		private void InitializeValueTypes(string arrayName, MemberMapping[] mappings)
		{
			for (int i = 0; i < mappings.Length; i++)
			{
				if (mappings[i].TypeDesc.IsValueType)
				{
					LocalBuilder local = this.ilg.GetLocal(arrayName);
					this.ilg.Ldloc(local);
					this.ilg.Ldc(i);
					base.RaCodeGen.ILGenForCreateInstance(this.ilg, mappings[i].TypeDesc.Type, false, false);
					this.ilg.ConvertValue(mappings[i].TypeDesc.Type, typeof(object));
					this.ilg.Stelem(local.LocalType.GetElementType());
				}
			}
		}

		private string GenerateTypeElement(XmlTypeMapping xmlTypeMapping)
		{
			ElementAccessor accessor = xmlTypeMapping.Accessor;
			TypeMapping mapping = accessor.Mapping;
			string text = this.NextMethodName(accessor.Name);
			this.ilg = new CodeGenerator(this.typeBuilder);
			this.ilg.BeginMethod(typeof(object), text, CodeGenerator.EmptyTypeArray, CodeGenerator.EmptyStringArray, CodeGenerator.PublicMethodAttributes);
			LocalBuilder localBuilder = this.ilg.DeclareLocal(typeof(object), "o");
			this.ilg.Load(null);
			this.ilg.Stloc(localBuilder);
			XmlSerializationReaderILGen.Member[] array = new XmlSerializationReaderILGen.Member[]
			{
				new XmlSerializationReaderILGen.Member(this, "o", "o", "a", 0, new MemberMapping
				{
					TypeDesc = mapping.TypeDesc,
					Elements = new ElementAccessor[] { accessor }
				})
			};
			MethodInfo method = typeof(XmlSerializationReader).GetMethod("get_Reader", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
			MethodInfo method2 = typeof(XmlReader).GetMethod("MoveToContent", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
			this.ilg.Ldarg(0);
			this.ilg.Call(method);
			this.ilg.Call(method2);
			this.ilg.Pop();
			string text2 = "UnknownNode(null, " + this.ExpectedElements(array) + ");";
			this.WriteMemberElements(array, "throw CreateUnknownNodeException();", text2, accessor.Any ? array[0] : null, null);
			this.ilg.Ldloc(localBuilder);
			this.ilg.Stloc(this.ilg.ReturnLocal);
			this.ilg.Ldloc(this.ilg.ReturnLocal);
			this.ilg.EndMethod();
			return text;
		}

		private string NextMethodName(string name)
		{
			string text = "Read";
			int num = base.NextMethodNumber + 1;
			base.NextMethodNumber = num;
			return text + num.ToString(CultureInfo.InvariantCulture) + "_" + CodeIdentifier.MakeValidInternal(name);
		}

		private string NextIdName(string name)
		{
			string text = "id";
			int num = this.nextIdNumber + 1;
			this.nextIdNumber = num;
			return text + num.ToString(CultureInfo.InvariantCulture) + "_" + CodeIdentifier.MakeValidInternal(name);
		}

		private void WritePrimitive(TypeMapping mapping, string source)
		{
			if (mapping is EnumMapping)
			{
				string text = base.ReferenceMapping(mapping);
				if (text == null)
				{
					throw new InvalidOperationException(Res.GetString("The method for enum {0} is missing.", new object[] { mapping.TypeDesc.Name }));
				}
				MethodBuilder methodBuilder = base.EnsureMethodBuilder(this.typeBuilder, text, CodeGenerator.PrivateMethodAttributes, mapping.TypeDesc.Type, new Type[] { typeof(string) });
				this.ilg.Ldarg(0);
				if (source == "Reader.ReadElementString()" || source == "Reader.ReadString()")
				{
					MethodInfo method = typeof(XmlSerializationReader).GetMethod("get_Reader", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
					MethodInfo method2 = typeof(XmlReader).GetMethod((source == "Reader.ReadElementString()") ? "ReadElementString" : "ReadString", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
					this.ilg.Ldarg(0);
					this.ilg.Call(method);
					this.ilg.Call(method2);
				}
				else if (source == "Reader.Value")
				{
					MethodInfo method3 = typeof(XmlSerializationReader).GetMethod("get_Reader", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
					MethodInfo method4 = typeof(XmlReader).GetMethod("get_Value", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
					this.ilg.Ldarg(0);
					this.ilg.Call(method3);
					this.ilg.Call(method4);
				}
				else if (source == "vals[i]")
				{
					LocalBuilder local = this.ilg.GetLocal("vals");
					LocalBuilder local2 = this.ilg.GetLocal("i");
					this.ilg.LoadArrayElement(local, local2);
				}
				else
				{
					if (!(source == "false"))
					{
						throw CodeGenerator.NotSupported("Unexpected: " + source);
					}
					this.ilg.Ldc(false);
				}
				this.ilg.Call(methodBuilder);
				return;
			}
			else
			{
				if (mapping.TypeDesc != base.StringTypeDesc)
				{
					if (mapping.TypeDesc.FormatterName == "String")
					{
						if (source == "vals[i]")
						{
							if (mapping.TypeDesc.CollapseWhitespace)
							{
								this.ilg.Ldarg(0);
							}
							LocalBuilder local3 = this.ilg.GetLocal("vals");
							LocalBuilder local4 = this.ilg.GetLocal("i");
							this.ilg.LoadArrayElement(local3, local4);
							if (mapping.TypeDesc.CollapseWhitespace)
							{
								MethodInfo method5 = typeof(XmlSerializationReader).GetMethod("CollapseWhitespace", CodeGenerator.InstanceBindingFlags, null, new Type[] { typeof(string) }, null);
								this.ilg.Call(method5);
								return;
							}
						}
						else
						{
							MethodInfo method6 = typeof(XmlSerializationReader).GetMethod("get_Reader", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
							MethodInfo method7 = typeof(XmlReader).GetMethod((source == "Reader.Value") ? "get_Value" : "ReadElementString", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
							if (mapping.TypeDesc.CollapseWhitespace)
							{
								this.ilg.Ldarg(0);
							}
							this.ilg.Ldarg(0);
							this.ilg.Call(method6);
							this.ilg.Call(method7);
							if (mapping.TypeDesc.CollapseWhitespace)
							{
								MethodInfo method8 = typeof(XmlSerializationReader).GetMethod("CollapseWhitespace", CodeGenerator.InstanceBindingFlags, null, new Type[] { typeof(string) }, null);
								this.ilg.Call(method8);
								return;
							}
						}
					}
					else
					{
						Type type = ((source == "false") ? typeof(bool) : typeof(string));
						MethodInfo methodInfo;
						if (mapping.TypeDesc.HasCustomFormatter)
						{
							BindingFlags bindingFlags = CodeGenerator.StaticBindingFlags;
							if ((mapping.TypeDesc.FormatterName == "ByteArrayBase64" && source == "false") || (mapping.TypeDesc.FormatterName == "ByteArrayHex" && source == "false") || mapping.TypeDesc.FormatterName == "XmlQualifiedName")
							{
								bindingFlags = CodeGenerator.InstanceBindingFlags;
								this.ilg.Ldarg(0);
							}
							methodInfo = typeof(XmlSerializationReader).GetMethod("To" + mapping.TypeDesc.FormatterName, bindingFlags, null, new Type[] { type }, null);
						}
						else
						{
							methodInfo = typeof(XmlConvert).GetMethod("To" + mapping.TypeDesc.FormatterName, CodeGenerator.StaticBindingFlags, null, new Type[] { type }, null);
						}
						if (source == "Reader.ReadElementString()" || source == "Reader.ReadString()")
						{
							MethodInfo method9 = typeof(XmlSerializationReader).GetMethod("get_Reader", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
							MethodInfo method10 = typeof(XmlReader).GetMethod((source == "Reader.ReadElementString()") ? "ReadElementString" : "ReadString", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
							this.ilg.Ldarg(0);
							this.ilg.Call(method9);
							this.ilg.Call(method10);
						}
						else if (source == "Reader.Value")
						{
							MethodInfo method11 = typeof(XmlSerializationReader).GetMethod("get_Reader", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
							MethodInfo method12 = typeof(XmlReader).GetMethod("get_Value", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
							this.ilg.Ldarg(0);
							this.ilg.Call(method11);
							this.ilg.Call(method12);
						}
						else if (source == "vals[i]")
						{
							LocalBuilder local5 = this.ilg.GetLocal("vals");
							LocalBuilder local6 = this.ilg.GetLocal("i");
							this.ilg.LoadArrayElement(local5, local6);
						}
						else
						{
							this.ilg.Ldc(false);
						}
						this.ilg.Call(methodInfo);
					}
					return;
				}
				if (source == "Reader.ReadElementString()" || source == "Reader.ReadString()")
				{
					MethodInfo method13 = typeof(XmlSerializationReader).GetMethod("get_Reader", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
					MethodInfo method14 = typeof(XmlReader).GetMethod((source == "Reader.ReadElementString()") ? "ReadElementString" : "ReadString", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
					this.ilg.Ldarg(0);
					this.ilg.Call(method13);
					this.ilg.Call(method14);
					return;
				}
				if (source == "Reader.Value")
				{
					MethodInfo method15 = typeof(XmlSerializationReader).GetMethod("get_Reader", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
					MethodInfo method16 = typeof(XmlReader).GetMethod("get_Value", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
					this.ilg.Ldarg(0);
					this.ilg.Call(method15);
					this.ilg.Call(method16);
					return;
				}
				if (source == "vals[i]")
				{
					LocalBuilder local7 = this.ilg.GetLocal("vals");
					LocalBuilder local8 = this.ilg.GetLocal("i");
					this.ilg.LoadArrayElement(local7, local8);
					return;
				}
				throw CodeGenerator.NotSupported("Unexpected: " + source);
			}
		}

		private string MakeUnique(EnumMapping mapping, string name)
		{
			string text = name;
			object obj = this.Enums[text];
			if (obj != null)
			{
				if (obj == mapping)
				{
					return null;
				}
				int num = 0;
				while (obj != null)
				{
					num++;
					text = name + num.ToString(CultureInfo.InvariantCulture);
					obj = this.Enums[text];
				}
			}
			this.Enums.Add(text, mapping);
			return text;
		}

		private string WriteHashtable(EnumMapping mapping, string typeName, out MethodBuilder get_TableName)
		{
			get_TableName = null;
			CodeIdentifier.CheckValidIdentifier(typeName);
			string text = this.MakeUnique(mapping, typeName + "Values");
			if (text == null)
			{
				return CodeIdentifier.GetCSharpName(typeName);
			}
			string text2 = this.MakeUnique(mapping, "_" + text);
			text = CodeIdentifier.GetCSharpName(text);
			FieldBuilder fieldBuilder = this.typeBuilder.DefineField(text2, typeof(Hashtable), FieldAttributes.Private);
			PropertyBuilder propertyBuilder = this.typeBuilder.DefineProperty(text, PropertyAttributes.None, CallingConventions.HasThis, typeof(Hashtable), null, null, null, null, null);
			this.ilg = new CodeGenerator(this.typeBuilder);
			this.ilg.BeginMethod(typeof(Hashtable), "get_" + text, CodeGenerator.EmptyTypeArray, CodeGenerator.EmptyStringArray, MethodAttributes.Private | MethodAttributes.FamANDAssem | MethodAttributes.HideBySig | MethodAttributes.SpecialName);
			this.ilg.Ldarg(0);
			this.ilg.LoadMember(fieldBuilder);
			this.ilg.Load(null);
			this.ilg.If(Cmp.EqualTo);
			ConstructorInfo constructor = typeof(Hashtable).GetConstructor(CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
			LocalBuilder localBuilder = this.ilg.DeclareLocal(typeof(Hashtable), "h");
			this.ilg.New(constructor);
			this.ilg.Stloc(localBuilder);
			ConstantMapping[] constants = mapping.Constants;
			MethodInfo method = typeof(Hashtable).GetMethod("Add", CodeGenerator.InstanceBindingFlags, null, new Type[]
			{
				typeof(object),
				typeof(object)
			}, null);
			for (int i = 0; i < constants.Length; i++)
			{
				this.ilg.Ldloc(localBuilder);
				this.ilg.Ldstr(constants[i].XmlName);
				this.ilg.Ldc(Enum.ToObject(mapping.TypeDesc.Type, constants[i].Value));
				this.ilg.ConvertValue(mapping.TypeDesc.Type, typeof(long));
				this.ilg.ConvertValue(typeof(long), typeof(object));
				this.ilg.Call(method);
			}
			this.ilg.Ldarg(0);
			this.ilg.Ldloc(localBuilder);
			this.ilg.StoreMember(fieldBuilder);
			this.ilg.EndIf();
			this.ilg.Ldarg(0);
			this.ilg.LoadMember(fieldBuilder);
			get_TableName = this.ilg.EndMethod();
			propertyBuilder.SetGetMethod(get_TableName);
			return text;
		}

		private void WriteEnumMethod(EnumMapping mapping)
		{
			MethodBuilder methodBuilder = null;
			if (mapping.IsFlags)
			{
				this.WriteHashtable(mapping, mapping.TypeDesc.Name, out methodBuilder);
			}
			string text = (string)base.MethodNames[mapping];
			string csharpName = mapping.TypeDesc.CSharpName;
			List<Type> list = new List<Type>();
			List<string> list2 = new List<string>();
			Type type = mapping.TypeDesc.Type;
			Type underlyingType = Enum.GetUnderlyingType(type);
			list.Add(typeof(string));
			list2.Add("s");
			this.ilg = new CodeGenerator(this.typeBuilder);
			this.ilg.BeginMethod(type, base.GetMethodBuilder(text), list.ToArray(), list2.ToArray(), CodeGenerator.PrivateMethodAttributes);
			ConstantMapping[] constants = mapping.Constants;
			if (mapping.IsFlags)
			{
				MethodInfo method = typeof(XmlSerializationReader).GetMethod("ToEnum", CodeGenerator.StaticBindingFlags, null, new Type[]
				{
					typeof(string),
					typeof(Hashtable),
					typeof(string)
				}, null);
				this.ilg.Ldarg("s");
				this.ilg.Ldarg(0);
				this.ilg.Call(methodBuilder);
				this.ilg.Ldstr(csharpName);
				this.ilg.Call(method);
				if (underlyingType != typeof(long))
				{
					this.ilg.ConvertValue(typeof(long), underlyingType);
				}
				this.ilg.Stloc(this.ilg.ReturnLocal);
				this.ilg.Br(this.ilg.ReturnLabel);
			}
			else
			{
				List<Label> list3 = new List<Label>();
				List<object> list4 = new List<object>();
				Label label = this.ilg.DefineLabel();
				Label label2 = this.ilg.DefineLabel();
				LocalBuilder tempLocal = this.ilg.GetTempLocal(typeof(string));
				this.ilg.Ldarg("s");
				this.ilg.Stloc(tempLocal);
				this.ilg.Ldloc(tempLocal);
				this.ilg.Brfalse(label);
				Hashtable hashtable = new Hashtable();
				foreach (ConstantMapping constantMapping in constants)
				{
					CodeIdentifier.CheckValidIdentifier(constantMapping.Name);
					if (hashtable[constantMapping.XmlName] == null)
					{
						hashtable[constantMapping.XmlName] = constantMapping.XmlName;
						Label label3 = this.ilg.DefineLabel();
						this.ilg.Ldloc(tempLocal);
						this.ilg.Ldstr(constantMapping.XmlName);
						MethodInfo method2 = typeof(string).GetMethod("op_Equality", CodeGenerator.StaticBindingFlags, null, new Type[]
						{
							typeof(string),
							typeof(string)
						}, null);
						this.ilg.Call(method2);
						this.ilg.Brtrue(label3);
						list3.Add(label3);
						list4.Add(Enum.ToObject(mapping.TypeDesc.Type, constantMapping.Value));
					}
				}
				this.ilg.Br(label);
				for (int j = 0; j < list3.Count; j++)
				{
					this.ilg.MarkLabel(list3[j]);
					this.ilg.Ldc(list4[j]);
					this.ilg.Stloc(this.ilg.ReturnLocal);
					this.ilg.Br(this.ilg.ReturnLabel);
				}
				MethodInfo method3 = typeof(XmlSerializationReader).GetMethod("CreateUnknownConstantException", CodeGenerator.InstanceBindingFlags, null, new Type[]
				{
					typeof(string),
					typeof(Type)
				}, null);
				this.ilg.MarkLabel(label);
				this.ilg.Ldarg(0);
				this.ilg.Ldarg("s");
				this.ilg.Ldc(mapping.TypeDesc.Type);
				this.ilg.Call(method3);
				this.ilg.Throw();
				this.ilg.MarkLabel(label2);
			}
			this.ilg.MarkLabel(this.ilg.ReturnLabel);
			this.ilg.Ldloc(this.ilg.ReturnLocal);
			this.ilg.EndMethod();
		}

		private void WriteDerivedTypes(StructMapping mapping, bool isTypedReturn, string returnTypeName)
		{
			for (StructMapping structMapping = mapping.DerivedMappings; structMapping != null; structMapping = structMapping.NextDerivedMapping)
			{
				this.ilg.InitElseIf();
				this.WriteQNameEqual("xsiType", structMapping.TypeName, structMapping.Namespace);
				this.ilg.AndIf();
				string text = base.ReferenceMapping(structMapping);
				List<Type> list = new List<Type>();
				this.ilg.Ldarg(0);
				if (structMapping.TypeDesc.IsNullable)
				{
					this.ilg.Ldarg("isNullable");
					list.Add(typeof(bool));
				}
				this.ilg.Ldc(false);
				list.Add(typeof(bool));
				MethodBuilder methodBuilder = base.EnsureMethodBuilder(this.typeBuilder, text, CodeGenerator.PrivateMethodAttributes, structMapping.TypeDesc.Type, list.ToArray());
				this.ilg.Call(methodBuilder);
				this.ilg.ConvertValue(methodBuilder.ReturnType, this.ilg.ReturnLocal.LocalType);
				this.ilg.Stloc(this.ilg.ReturnLocal);
				this.ilg.Br(this.ilg.ReturnLabel);
				this.WriteDerivedTypes(structMapping, isTypedReturn, returnTypeName);
			}
		}

		private void WriteEnumAndArrayTypes()
		{
			TypeScope[] scopes = base.Scopes;
			for (int i = 0; i < scopes.Length; i++)
			{
				foreach (object obj in scopes[i].TypeMappings)
				{
					Mapping mapping = (Mapping)obj;
					if (mapping is EnumMapping)
					{
						EnumMapping enumMapping = (EnumMapping)mapping;
						this.ilg.InitElseIf();
						this.WriteQNameEqual("xsiType", enumMapping.TypeName, enumMapping.Namespace);
						this.ilg.AndIf();
						MethodInfo method = typeof(XmlSerializationReader).GetMethod("get_Reader", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
						MethodInfo method2 = typeof(XmlReader).GetMethod("ReadStartElement", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
						this.ilg.Ldarg(0);
						this.ilg.Call(method);
						this.ilg.Call(method2);
						string text = base.ReferenceMapping(enumMapping);
						LocalBuilder localBuilder = this.ilg.DeclareOrGetLocal(typeof(object), "e");
						MethodBuilder methodBuilder = base.EnsureMethodBuilder(this.typeBuilder, text, CodeGenerator.PrivateMethodAttributes, enumMapping.TypeDesc.Type, new Type[] { typeof(string) });
						MethodInfo method3 = typeof(XmlSerializationReader).GetMethod("CollapseWhitespace", CodeGenerator.InstanceBindingFlags, null, new Type[] { typeof(string) }, null);
						MethodInfo method4 = typeof(XmlReader).GetMethod("ReadString", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
						this.ilg.Ldarg(0);
						this.ilg.Ldarg(0);
						this.ilg.Ldarg(0);
						this.ilg.Call(method);
						this.ilg.Call(method4);
						this.ilg.Call(method3);
						this.ilg.Call(methodBuilder);
						this.ilg.ConvertValue(methodBuilder.ReturnType, localBuilder.LocalType);
						this.ilg.Stloc(localBuilder);
						MethodInfo method5 = typeof(XmlSerializationReader).GetMethod("ReadEndElement", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
						this.ilg.Ldarg(0);
						this.ilg.Call(method5);
						this.ilg.Ldloc(localBuilder);
						this.ilg.Stloc(this.ilg.ReturnLocal);
						this.ilg.Br(this.ilg.ReturnLabel);
					}
					else if (mapping is ArrayMapping)
					{
						ArrayMapping arrayMapping = (ArrayMapping)mapping;
						if (arrayMapping.TypeDesc.HasDefaultConstructor)
						{
							this.ilg.InitElseIf();
							this.WriteQNameEqual("xsiType", arrayMapping.TypeName, arrayMapping.Namespace);
							this.ilg.AndIf();
							this.ilg.EnterScope();
							MemberMapping memberMapping = new MemberMapping();
							memberMapping.TypeDesc = arrayMapping.TypeDesc;
							memberMapping.Elements = arrayMapping.Elements;
							string text2 = "a";
							string text3 = "z";
							XmlSerializationReaderILGen.Member member = new XmlSerializationReaderILGen.Member(this, text2, text3, 0, memberMapping);
							TypeDesc typeDesc = arrayMapping.TypeDesc;
							LocalBuilder localBuilder2 = this.ilg.DeclareLocal(arrayMapping.TypeDesc.Type, text2);
							if (arrayMapping.TypeDesc.IsValueType)
							{
								base.RaCodeGen.ILGenForCreateInstance(this.ilg, typeDesc.Type, false, false);
							}
							else
							{
								this.ilg.Load(null);
							}
							this.ilg.Stloc(localBuilder2);
							this.WriteArray(member.Source, member.ArrayName, arrayMapping, false, false, -1, 0);
							this.ilg.Ldloc(localBuilder2);
							this.ilg.Stloc(this.ilg.ReturnLocal);
							this.ilg.Br(this.ilg.ReturnLabel);
							this.ilg.ExitScope();
						}
					}
				}
			}
		}

		private void WriteNullableMethod(NullableMapping nullableMapping)
		{
			string text = (string)base.MethodNames[nullableMapping];
			this.ilg = new CodeGenerator(this.typeBuilder);
			this.ilg.BeginMethod(nullableMapping.TypeDesc.Type, base.GetMethodBuilder(text), new Type[] { typeof(bool) }, new string[] { "checkType" }, CodeGenerator.PrivateMethodAttributes);
			LocalBuilder localBuilder = this.ilg.DeclareLocal(nullableMapping.TypeDesc.Type, "o");
			this.ilg.LoadAddress(localBuilder);
			this.ilg.InitObj(nullableMapping.TypeDesc.Type);
			MethodInfo method = typeof(XmlSerializationReader).GetMethod("ReadNull", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
			this.ilg.Ldarg(0);
			this.ilg.Call(method);
			this.ilg.If();
			this.ilg.Ldloc(localBuilder);
			this.ilg.Stloc(this.ilg.ReturnLocal);
			this.ilg.Br(this.ilg.ReturnLabel);
			this.ilg.EndIf();
			this.WriteElement("o", null, null, new ElementAccessor
			{
				Mapping = nullableMapping.BaseMapping,
				Any = false,
				IsNullable = nullableMapping.BaseMapping.TypeDesc.IsNullable
			}, null, null, false, false, -1, -1);
			this.ilg.Ldloc(localBuilder);
			this.ilg.Stloc(this.ilg.ReturnLocal);
			this.ilg.Br(this.ilg.ReturnLabel);
			this.ilg.MarkLabel(this.ilg.ReturnLabel);
			this.ilg.Ldloc(this.ilg.ReturnLocal);
			this.ilg.EndMethod();
		}

		private void WriteStructMethod(StructMapping structMapping)
		{
			this.WriteLiteralStructMethod(structMapping);
		}

		private void WriteLiteralStructMethod(StructMapping structMapping)
		{
			string text = (string)base.MethodNames[structMapping];
			string csharpName = structMapping.TypeDesc.CSharpName;
			this.ilg = new CodeGenerator(this.typeBuilder);
			List<Type> list = new List<Type>();
			List<string> list2 = new List<string>();
			if (structMapping.TypeDesc.IsNullable)
			{
				list.Add(typeof(bool));
				list2.Add("isNullable");
			}
			list.Add(typeof(bool));
			list2.Add("checkType");
			this.ilg.BeginMethod(structMapping.TypeDesc.Type, base.GetMethodBuilder(text), list.ToArray(), list2.ToArray(), CodeGenerator.PrivateMethodAttributes);
			LocalBuilder localBuilder = this.ilg.DeclareLocal(typeof(XmlQualifiedName), "xsiType");
			LocalBuilder localBuilder2 = this.ilg.DeclareLocal(typeof(bool), "isNull");
			MethodInfo method = typeof(XmlSerializationReader).GetMethod("GetXsiType", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
			MethodInfo method2 = typeof(XmlSerializationReader).GetMethod("ReadNull", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
			Label label = this.ilg.DefineLabel();
			Label label2 = this.ilg.DefineLabel();
			this.ilg.Ldarg("checkType");
			this.ilg.Brtrue(label);
			this.ilg.Load(null);
			this.ilg.Br_S(label2);
			this.ilg.MarkLabel(label);
			this.ilg.Ldarg(0);
			this.ilg.Call(method);
			this.ilg.MarkLabel(label2);
			this.ilg.Stloc(localBuilder);
			this.ilg.Ldc(false);
			this.ilg.Stloc(localBuilder2);
			if (structMapping.TypeDesc.IsNullable)
			{
				this.ilg.Ldarg("isNullable");
				this.ilg.If();
				this.ilg.Ldarg(0);
				this.ilg.Call(method2);
				this.ilg.Stloc(localBuilder2);
				this.ilg.EndIf();
			}
			this.ilg.Ldarg("checkType");
			this.ilg.If();
			if (structMapping.TypeDesc.IsRoot)
			{
				this.ilg.Ldloc(localBuilder2);
				this.ilg.If();
				this.ilg.Ldloc(localBuilder);
				this.ilg.Load(null);
				this.ilg.If(Cmp.NotEqualTo);
				MethodInfo method3 = typeof(XmlSerializationReader).GetMethod("ReadTypedNull", CodeGenerator.InstanceBindingFlags, null, new Type[] { localBuilder.LocalType }, null);
				this.ilg.Ldarg(0);
				this.ilg.Ldloc(localBuilder);
				this.ilg.Call(method3);
				this.ilg.Stloc(this.ilg.ReturnLocal);
				this.ilg.Br(this.ilg.ReturnLabel);
				this.ilg.Else();
				if (structMapping.TypeDesc.IsValueType)
				{
					throw CodeGenerator.NotSupported("Arg_NeverValueType");
				}
				this.ilg.Load(null);
				this.ilg.Stloc(this.ilg.ReturnLocal);
				this.ilg.Br(this.ilg.ReturnLabel);
				this.ilg.EndIf();
				this.ilg.EndIf();
			}
			this.ilg.Ldloc(typeof(XmlQualifiedName), "xsiType");
			this.ilg.Load(null);
			this.ilg.Ceq();
			if (!structMapping.TypeDesc.IsRoot)
			{
				label = this.ilg.DefineLabel();
				label2 = this.ilg.DefineLabel();
				this.ilg.Brtrue(label);
				this.WriteQNameEqual("xsiType", structMapping.TypeName, structMapping.Namespace);
				this.ilg.Br_S(label2);
				this.ilg.MarkLabel(label);
				this.ilg.Ldc(true);
				this.ilg.MarkLabel(label2);
			}
			this.ilg.If();
			if (structMapping.TypeDesc.IsRoot)
			{
				ConstructorInfo constructor = typeof(XmlQualifiedName).GetConstructor(CodeGenerator.InstanceBindingFlags, null, new Type[]
				{
					typeof(string),
					typeof(string)
				}, null);
				MethodInfo method4 = typeof(XmlSerializationReader).GetMethod("ReadTypedPrimitive", CodeGenerator.InstanceBindingFlags, null, new Type[] { typeof(XmlQualifiedName) }, null);
				this.ilg.Ldarg(0);
				this.ilg.Ldstr("anyType");
				this.ilg.Ldstr("http://www.w3.org/2001/XMLSchema");
				this.ilg.New(constructor);
				this.ilg.Call(method4);
				this.ilg.Stloc(this.ilg.ReturnLocal);
				this.ilg.Br(this.ilg.ReturnLabel);
			}
			this.WriteDerivedTypes(structMapping, !structMapping.TypeDesc.IsRoot, csharpName);
			if (structMapping.TypeDesc.IsRoot)
			{
				this.WriteEnumAndArrayTypes();
			}
			this.ilg.Else();
			if (structMapping.TypeDesc.IsRoot)
			{
				MethodInfo method5 = typeof(XmlSerializationReader).GetMethod("ReadTypedPrimitive", CodeGenerator.InstanceBindingFlags, null, new Type[] { localBuilder.LocalType }, null);
				this.ilg.Ldarg(0);
				this.ilg.Ldloc(localBuilder);
				this.ilg.Call(method5);
				this.ilg.Stloc(this.ilg.ReturnLocal);
				this.ilg.Br(this.ilg.ReturnLabel);
			}
			else
			{
				MethodInfo method6 = typeof(XmlSerializationReader).GetMethod("CreateUnknownTypeException", CodeGenerator.InstanceBindingFlags, null, new Type[] { typeof(XmlQualifiedName) }, null);
				this.ilg.Ldarg(0);
				this.ilg.Ldloc(localBuilder);
				this.ilg.Call(method6);
				this.ilg.Throw();
			}
			this.ilg.EndIf();
			this.ilg.EndIf();
			if (structMapping.TypeDesc.IsNullable)
			{
				this.ilg.Ldloc(typeof(bool), "isNull");
				this.ilg.If();
				this.ilg.Load(null);
				this.ilg.Stloc(this.ilg.ReturnLocal);
				this.ilg.Br(this.ilg.ReturnLabel);
				this.ilg.EndIf();
			}
			if (structMapping.TypeDesc.IsAbstract)
			{
				MethodInfo method7 = typeof(XmlSerializationReader).GetMethod("CreateAbstractTypeException", CodeGenerator.InstanceBindingFlags, null, new Type[]
				{
					typeof(string),
					typeof(string)
				}, null);
				this.ilg.Ldarg(0);
				this.ilg.Ldstr(structMapping.TypeName);
				this.ilg.Ldstr(structMapping.Namespace);
				this.ilg.Call(method7);
				this.ilg.Throw();
			}
			else
			{
				if (structMapping.TypeDesc.Type != null && typeof(XmlSchemaObject).IsAssignableFrom(structMapping.TypeDesc.Type))
				{
					MethodInfo method8 = typeof(XmlSerializationReader).GetMethod("set_DecodeName", CodeGenerator.InstanceBindingFlags, null, new Type[] { typeof(bool) }, null);
					this.ilg.Ldarg(0);
					this.ilg.Ldc(false);
					this.ilg.Call(method8);
				}
				this.WriteCreateMapping(structMapping, "o");
				LocalBuilder local = this.ilg.GetLocal("o");
				MemberMapping[] settableMembers = TypeScope.GetSettableMembers(structMapping, this.memberInfos);
				XmlSerializationReaderILGen.Member member = null;
				XmlSerializationReaderILGen.Member member2 = null;
				XmlSerializationReaderILGen.Member member3 = null;
				bool flag = structMapping.HasExplicitSequence();
				ArrayList arrayList = new ArrayList(settableMembers.Length);
				ArrayList arrayList2 = new ArrayList(settableMembers.Length);
				ArrayList arrayList3 = new ArrayList(settableMembers.Length);
				for (int i = 0; i < settableMembers.Length; i++)
				{
					MemberMapping memberMapping = settableMembers[i];
					CodeIdentifier.CheckValidIdentifier(memberMapping.Name);
					string stringForMember = base.RaCodeGen.GetStringForMember("o", memberMapping.Name, structMapping.TypeDesc);
					XmlSerializationReaderILGen.Member member4 = new XmlSerializationReaderILGen.Member(this, stringForMember, "a", i, memberMapping, this.GetChoiceIdentifierSource(memberMapping, "o", structMapping.TypeDesc));
					if (!memberMapping.IsSequence)
					{
						member4.ParamsReadSource = "paramsRead[" + i.ToString(CultureInfo.InvariantCulture) + "]";
					}
					member4.IsNullable = memberMapping.TypeDesc.IsNullable;
					if (memberMapping.CheckSpecified == SpecifiedAccessor.ReadWrite)
					{
						member4.CheckSpecifiedSource = base.RaCodeGen.GetStringForMember("o", memberMapping.Name + "Specified", structMapping.TypeDesc);
					}
					if (memberMapping.Text != null)
					{
						member = member4;
					}
					if (memberMapping.Attribute != null && memberMapping.Attribute.Any)
					{
						member3 = member4;
					}
					if (!flag)
					{
						for (int j = 0; j < memberMapping.Elements.Length; j++)
						{
							if (memberMapping.Elements[j].Any && (memberMapping.Elements[j].Name == null || memberMapping.Elements[j].Name.Length == 0))
							{
								member2 = member4;
								break;
							}
						}
					}
					else if (memberMapping.IsParticle && !memberMapping.IsSequence)
					{
						StructMapping structMapping2;
						structMapping.FindDeclaringMapping(memberMapping, out structMapping2, structMapping.TypeName);
						throw new InvalidOperationException(Res.GetString("There was an error processing type '{0}'. Type member '{1}' declared in '{2}' is missing required '{3}' property. If one class in the class hierarchy uses explicit sequencing feature ({3}), then its base class and all derived classes have to do the same.", new object[]
						{
							structMapping.TypeDesc.FullName,
							memberMapping.Name,
							structMapping2.TypeDesc.FullName,
							"Order"
						}));
					}
					if (memberMapping.Attribute == null && memberMapping.Elements.Length == 1 && memberMapping.Elements[0].Mapping is ArrayMapping)
					{
						arrayList3.Add(new XmlSerializationReaderILGen.Member(this, stringForMember, stringForMember, "a", i, memberMapping, this.GetChoiceIdentifierSource(memberMapping, "o", structMapping.TypeDesc))
						{
							CheckSpecifiedSource = member4.CheckSpecifiedSource
						});
					}
					else
					{
						arrayList3.Add(member4);
					}
					if (memberMapping.TypeDesc.IsArrayLike)
					{
						arrayList.Add(member4);
						if (memberMapping.TypeDesc.IsArrayLike && (memberMapping.Elements.Length != 1 || !(memberMapping.Elements[0].Mapping is ArrayMapping)))
						{
							member4.ParamsReadSource = null;
							if (member4 != member && member4 != member2)
							{
								arrayList2.Add(member4);
							}
						}
						else if (!memberMapping.TypeDesc.IsArray)
						{
							member4.ParamsReadSource = null;
						}
					}
				}
				if (member2 != null)
				{
					arrayList2.Add(member2);
				}
				if (member != null && member != member2)
				{
					arrayList2.Add(member);
				}
				XmlSerializationReaderILGen.Member[] array = (XmlSerializationReaderILGen.Member[])arrayList.ToArray(typeof(XmlSerializationReaderILGen.Member));
				XmlSerializationReaderILGen.Member[] array2 = (XmlSerializationReaderILGen.Member[])arrayList2.ToArray(typeof(XmlSerializationReaderILGen.Member));
				XmlSerializationReaderILGen.Member[] array3 = (XmlSerializationReaderILGen.Member[])arrayList3.ToArray(typeof(XmlSerializationReaderILGen.Member));
				this.WriteMemberBegin(array);
				this.WriteParamsRead(settableMembers.Length);
				this.WriteAttributes(array3, member3, "UnknownNode", local);
				if (member3 != null)
				{
					this.WriteMemberEnd(array);
				}
				MethodInfo method9 = typeof(XmlSerializationReader).GetMethod("get_Reader", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
				MethodInfo method10 = typeof(XmlReader).GetMethod("MoveToElement", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
				this.ilg.Ldarg(0);
				this.ilg.Call(method9);
				this.ilg.Call(method10);
				this.ilg.Pop();
				MethodInfo method11 = typeof(XmlReader).GetMethod("get_IsEmptyElement", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
				this.ilg.Ldarg(0);
				this.ilg.Call(method9);
				this.ilg.Call(method11);
				this.ilg.If();
				MethodInfo method12 = typeof(XmlReader).GetMethod("Skip", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
				this.ilg.Ldarg(0);
				this.ilg.Call(method9);
				this.ilg.Call(method12);
				this.WriteMemberEnd(array2);
				this.ilg.Ldloc(local);
				this.ilg.Stloc(this.ilg.ReturnLocal);
				this.ilg.Br(this.ilg.ReturnLabel);
				this.ilg.EndIf();
				MethodInfo method13 = typeof(XmlReader).GetMethod("ReadStartElement", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
				this.ilg.Ldarg(0);
				this.ilg.Call(method9);
				this.ilg.Call(method13);
				if (this.IsSequence(array3))
				{
					this.ilg.Ldc(0);
					this.ilg.Stloc(typeof(int), "state");
				}
				int num = this.WriteWhileNotLoopStart();
				string text2 = "UnknownNode((object)o, " + this.ExpectedElements(array3) + ");";
				this.WriteMemberElements(array3, text2, text2, member2, member);
				MethodInfo method14 = typeof(XmlReader).GetMethod("MoveToContent", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
				this.ilg.Ldarg(0);
				this.ilg.Call(method9);
				this.ilg.Call(method14);
				this.ilg.Pop();
				this.WriteWhileLoopEnd(num);
				this.WriteMemberEnd(array2);
				MethodInfo method15 = typeof(XmlSerializationReader).GetMethod("ReadEndElement", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
				this.ilg.Ldarg(0);
				this.ilg.Call(method15);
				this.ilg.Ldloc(structMapping.TypeDesc.Type, "o");
				this.ilg.Stloc(this.ilg.ReturnLocal);
			}
			this.ilg.MarkLabel(this.ilg.ReturnLabel);
			this.ilg.Ldloc(this.ilg.ReturnLocal);
			this.ilg.EndMethod();
		}

		private void WriteQNameEqual(string source, string name, string ns)
		{
			this.WriteID(name);
			this.WriteID(ns);
			MethodInfo method = typeof(XmlQualifiedName).GetMethod("get_Name", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
			MethodInfo method2 = typeof(XmlQualifiedName).GetMethod("get_Namespace", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
			Label label = this.ilg.DefineLabel();
			Label label2 = this.ilg.DefineLabel();
			LocalBuilder local = this.ilg.GetLocal(source);
			this.ilg.Ldloc(local);
			this.ilg.Call(method);
			this.ilg.Ldarg(0);
			this.ilg.LoadMember(this.idNameFields[name ?? string.Empty]);
			this.ilg.Bne(label2);
			this.ilg.Ldloc(local);
			this.ilg.Call(method2);
			this.ilg.Ldarg(0);
			this.ilg.LoadMember(this.idNameFields[ns ?? string.Empty]);
			this.ilg.Ceq();
			this.ilg.Br_S(label);
			this.ilg.MarkLabel(label2);
			this.ilg.Ldc(false);
			this.ilg.MarkLabel(label);
		}

		private void WriteXmlNodeEqual(string source, string name, string ns)
		{
			this.WriteXmlNodeEqual(source, name, ns, true);
		}

		private void WriteXmlNodeEqual(string source, string name, string ns, bool doAndIf)
		{
			bool flag = string.IsNullOrEmpty(name);
			if (!flag)
			{
				this.WriteID(name);
			}
			this.WriteID(ns);
			MethodInfo method = typeof(XmlSerializationReader).GetMethod("get_" + source, CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
			MethodInfo method2 = typeof(XmlReader).GetMethod("get_LocalName", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
			MethodInfo method3 = typeof(XmlReader).GetMethod("get_NamespaceURI", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
			Label label = this.ilg.DefineLabel();
			Label label2 = this.ilg.DefineLabel();
			if (!flag)
			{
				this.ilg.Ldarg(0);
				this.ilg.Call(method);
				this.ilg.Call(method2);
				this.ilg.Ldarg(0);
				this.ilg.LoadMember(this.idNameFields[name ?? string.Empty]);
				this.ilg.Bne(label);
			}
			this.ilg.Ldarg(0);
			this.ilg.Call(method);
			this.ilg.Call(method3);
			this.ilg.Ldarg(0);
			this.ilg.LoadMember(this.idNameFields[ns ?? string.Empty]);
			this.ilg.Ceq();
			if (!flag)
			{
				this.ilg.Br_S(label2);
				this.ilg.MarkLabel(label);
				this.ilg.Ldc(false);
				this.ilg.MarkLabel(label2);
			}
			if (doAndIf)
			{
				this.ilg.AndIf();
			}
		}

		private void WriteID(string name)
		{
			if (name == null)
			{
				name = "";
			}
			if ((string)this.idNames[name] == null)
			{
				string text = this.NextIdName(name);
				this.idNames.Add(name, text);
				this.idNameFields.Add(name, this.typeBuilder.DefineField(text, typeof(string), FieldAttributes.Private));
			}
		}

		private void WriteAttributes(XmlSerializationReaderILGen.Member[] members, XmlSerializationReaderILGen.Member anyAttribute, string elseCall, LocalBuilder firstParam)
		{
			int num = 0;
			XmlSerializationReaderILGen.Member member = null;
			ArrayList arrayList = new ArrayList();
			MethodInfo method = typeof(XmlSerializationReader).GetMethod("get_Reader", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
			MethodInfo method2 = typeof(XmlReader).GetMethod("MoveToNextAttribute", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
			this.ilg.WhileBegin();
			foreach (XmlSerializationReaderILGen.Member member2 in members)
			{
				if (member2.Mapping.Xmlns != null)
				{
					member = member2;
				}
				else if (!member2.Mapping.Ignore)
				{
					AttributeAccessor attribute = member2.Mapping.Attribute;
					if (attribute != null && !attribute.Any)
					{
						arrayList.Add(attribute);
						if (num++ > 0)
						{
							this.ilg.InitElseIf();
						}
						else
						{
							this.ilg.InitIf();
						}
						if (member2.ParamsReadSource != null)
						{
							this.ILGenParamsReadSource(member2.ParamsReadSource);
							this.ilg.Ldc(false);
							this.ilg.AndIf(Cmp.EqualTo);
						}
						if (attribute.IsSpecialXmlNamespace)
						{
							this.WriteXmlNodeEqual("Reader", attribute.Name, "http://www.w3.org/XML/1998/namespace");
						}
						else
						{
							this.WriteXmlNodeEqual("Reader", attribute.Name, (attribute.Form == XmlSchemaForm.Qualified) ? attribute.Namespace : "");
						}
						this.WriteAttribute(member2);
					}
				}
			}
			if (num > 0)
			{
				this.ilg.InitElseIf();
			}
			else
			{
				this.ilg.InitIf();
			}
			if (member != null)
			{
				MethodInfo method3 = typeof(XmlSerializationReader).GetMethod("IsXmlnsAttribute", CodeGenerator.InstanceBindingFlags, null, new Type[] { typeof(string) }, null);
				MethodInfo method4 = typeof(XmlReader).GetMethod("get_Name", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
				MethodInfo method5 = typeof(XmlReader).GetMethod("get_LocalName", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
				MethodInfo method6 = typeof(XmlReader).GetMethod("get_Value", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
				this.ilg.Ldarg(0);
				this.ilg.Ldarg(0);
				this.ilg.Call(method);
				this.ilg.Call(method4);
				this.ilg.Call(method3);
				this.ilg.Ldc(true);
				this.ilg.AndIf(Cmp.EqualTo);
				base.ILGenLoad(member.Source);
				this.ilg.Load(null);
				this.ilg.If(Cmp.EqualTo);
				this.WriteSourceBegin(member.Source);
				ConstructorInfo constructor = member.Mapping.TypeDesc.Type.GetConstructor(CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
				this.ilg.New(constructor);
				this.WriteSourceEnd(member.Source, member.Mapping.TypeDesc.Type);
				this.ilg.EndIf();
				Label label = this.ilg.DefineLabel();
				Label label2 = this.ilg.DefineLabel();
				MethodInfo method7 = member.Mapping.TypeDesc.Type.GetMethod("Add", CodeGenerator.InstanceBindingFlags, null, new Type[]
				{
					typeof(string),
					typeof(string)
				}, null);
				MethodInfo method8 = typeof(string).GetMethod("get_Length", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
				base.ILGenLoad(member.ArraySource, member.Mapping.TypeDesc.Type);
				this.ilg.Ldarg(0);
				this.ilg.Call(method);
				this.ilg.Call(method4);
				this.ilg.Call(method8);
				this.ilg.Ldc(5);
				this.ilg.Beq(label);
				this.ilg.Ldarg(0);
				this.ilg.Call(method);
				this.ilg.Call(method5);
				this.ilg.Br(label2);
				this.ilg.MarkLabel(label);
				this.ilg.Ldstr(string.Empty);
				this.ilg.MarkLabel(label2);
				this.ilg.Ldarg(0);
				this.ilg.Call(method);
				this.ilg.Call(method6);
				this.ilg.Call(method7);
				this.ilg.Else();
			}
			else
			{
				MethodInfo method9 = typeof(XmlSerializationReader).GetMethod("IsXmlnsAttribute", CodeGenerator.InstanceBindingFlags, null, new Type[] { typeof(string) }, null);
				MethodInfo method10 = typeof(XmlReader).GetMethod("get_Name", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
				this.ilg.Ldarg(0);
				this.ilg.Ldarg(0);
				this.ilg.Call(method);
				this.ilg.Call(method10);
				this.ilg.Call(method9);
				this.ilg.Ldc(false);
				this.ilg.AndIf(Cmp.EqualTo);
			}
			if (anyAttribute != null)
			{
				LocalBuilder localBuilder = this.ilg.DeclareOrGetLocal(typeof(XmlAttribute), "attr");
				MethodInfo method11 = typeof(XmlSerializationReader).GetMethod("get_Document", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
				MethodInfo method12 = typeof(XmlDocument).GetMethod("ReadNode", CodeGenerator.InstanceBindingFlags, null, new Type[] { typeof(XmlReader) }, null);
				this.ilg.Ldarg(0);
				this.ilg.Call(method11);
				this.ilg.Ldarg(0);
				this.ilg.Call(method);
				this.ilg.Call(method12);
				this.ilg.ConvertValue(method12.ReturnType, localBuilder.LocalType);
				this.ilg.Stloc(localBuilder);
				MethodInfo method13 = typeof(XmlSerializationReader).GetMethod("ParseWsdlArrayType", CodeGenerator.InstanceBindingFlags, null, new Type[] { localBuilder.LocalType }, null);
				this.ilg.Ldarg(0);
				this.ilg.Ldloc(localBuilder);
				this.ilg.Call(method13);
				this.WriteAttribute(anyAttribute);
			}
			else
			{
				List<Type> list = new List<Type>();
				this.ilg.Ldarg(0);
				list.Add(typeof(object));
				this.ilg.Ldloc(firstParam);
				this.ilg.ConvertValue(firstParam.LocalType, typeof(object));
				if (arrayList.Count > 0)
				{
					string text = "";
					for (int j = 0; j < arrayList.Count; j++)
					{
						AttributeAccessor attributeAccessor = (AttributeAccessor)arrayList[j];
						if (j > 0)
						{
							text += ", ";
						}
						text += (attributeAccessor.IsSpecialXmlNamespace ? "http://www.w3.org/XML/1998/namespace" : (((attributeAccessor.Form == XmlSchemaForm.Qualified) ? attributeAccessor.Namespace : "") + ":" + attributeAccessor.Name));
					}
					list.Add(typeof(string));
					this.ilg.Ldstr(text);
				}
				MethodInfo method14 = typeof(XmlSerializationReader).GetMethod(elseCall, CodeGenerator.InstanceBindingFlags, null, list.ToArray(), null);
				this.ilg.Call(method14);
			}
			this.ilg.EndIf();
			this.ilg.WhileBeginCondition();
			this.ilg.Ldarg(0);
			this.ilg.Call(method);
			this.ilg.Call(method2);
			this.ilg.WhileEndCondition();
			this.ilg.WhileEnd();
		}

		private void WriteAttribute(XmlSerializationReaderILGen.Member member)
		{
			AttributeAccessor attribute = member.Mapping.Attribute;
			if (attribute.Mapping is SpecialMapping)
			{
				SpecialMapping specialMapping = (SpecialMapping)attribute.Mapping;
				if (specialMapping.TypeDesc.Kind == TypeKind.Attribute)
				{
					this.WriteSourceBegin(member.ArraySource);
					this.ilg.Ldloc("attr");
					this.WriteSourceEnd(member.ArraySource, member.Mapping.TypeDesc.IsArrayLike ? member.Mapping.TypeDesc.ArrayElementTypeDesc.Type : member.Mapping.TypeDesc.Type);
				}
				else
				{
					if (!specialMapping.TypeDesc.CanBeAttributeValue)
					{
						throw new InvalidOperationException(Res.GetString("Internal error."));
					}
					LocalBuilder local = this.ilg.GetLocal("attr");
					this.ilg.Ldloc(local);
					if (local.LocalType == typeof(XmlAttribute))
					{
						this.ilg.Load(null);
						this.ilg.Cne();
					}
					else
					{
						this.ilg.IsInst(typeof(XmlAttribute));
					}
					this.ilg.If();
					this.WriteSourceBegin(member.ArraySource);
					this.ilg.Ldloc(local);
					this.ilg.ConvertValue(local.LocalType, typeof(XmlAttribute));
					this.WriteSourceEnd(member.ArraySource, member.Mapping.TypeDesc.IsArrayLike ? member.Mapping.TypeDesc.ArrayElementTypeDesc.Type : member.Mapping.TypeDesc.Type);
					this.ilg.EndIf();
				}
			}
			else if (attribute.IsList)
			{
				LocalBuilder localBuilder = this.ilg.DeclareOrGetLocal(typeof(string), "listValues");
				LocalBuilder localBuilder2 = this.ilg.DeclareOrGetLocal(typeof(string[]), "vals");
				MethodInfo method = typeof(string).GetMethod("Split", CodeGenerator.InstanceBindingFlags, null, new Type[] { typeof(char[]) }, null);
				MethodInfo method2 = typeof(XmlSerializationReader).GetMethod("get_Reader", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
				MethodInfo method3 = typeof(XmlReader).GetMethod("get_Value", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
				this.ilg.Ldarg(0);
				this.ilg.Call(method2);
				this.ilg.Call(method3);
				this.ilg.Stloc(localBuilder);
				this.ilg.Ldloc(localBuilder);
				this.ilg.Load(null);
				this.ilg.Call(method);
				this.ilg.Stloc(localBuilder2);
				LocalBuilder localBuilder3 = this.ilg.DeclareOrGetLocal(typeof(int), "i");
				this.ilg.For(localBuilder3, 0, localBuilder2);
				string arraySource = this.GetArraySource(member.Mapping.TypeDesc, member.ArrayName);
				this.WriteSourceBegin(arraySource);
				this.WritePrimitive(attribute.Mapping, "vals[i]");
				this.WriteSourceEnd(arraySource, member.Mapping.TypeDesc.ArrayElementTypeDesc.Type);
				this.ilg.EndFor();
			}
			else
			{
				this.WriteSourceBegin(member.ArraySource);
				this.WritePrimitive(attribute.Mapping, attribute.IsList ? "vals[i]" : "Reader.Value");
				this.WriteSourceEnd(member.ArraySource, member.Mapping.TypeDesc.IsArrayLike ? member.Mapping.TypeDesc.ArrayElementTypeDesc.Type : member.Mapping.TypeDesc.Type);
			}
			if (member.Mapping.CheckSpecified == SpecifiedAccessor.ReadWrite && member.CheckSpecifiedSource != null && member.CheckSpecifiedSource.Length > 0)
			{
				this.ILGenSet(member.CheckSpecifiedSource, true);
			}
			if (member.ParamsReadSource != null)
			{
				this.ILGenParamsReadSource(member.ParamsReadSource, true);
			}
		}

		private void WriteMemberBegin(XmlSerializationReaderILGen.Member[] members)
		{
			foreach (XmlSerializationReaderILGen.Member member in members)
			{
				if (member.IsArrayLike)
				{
					string arrayName = member.ArrayName;
					string text = "c" + arrayName;
					TypeDesc typeDesc = member.Mapping.TypeDesc;
					if (member.Mapping.TypeDesc.IsArray)
					{
						this.WriteArrayLocalDecl(typeDesc.CSharpName, arrayName, "null", typeDesc);
						this.ilg.Ldc(0);
						this.ilg.Stloc(typeof(int), text);
						if (member.Mapping.ChoiceIdentifier != null)
						{
							this.WriteArrayLocalDecl(member.Mapping.ChoiceIdentifier.Mapping.TypeDesc.CSharpName + "[]", member.ChoiceArrayName, "null", member.Mapping.ChoiceIdentifier.Mapping.TypeDesc);
							this.ilg.Ldc(0);
							this.ilg.Stloc(typeof(int), "c" + member.ChoiceArrayName);
						}
					}
					else if (member.Source[member.Source.Length - 1] == '(' || member.Source[member.Source.Length - 1] == '{')
					{
						this.WriteCreateInstance(arrayName, typeDesc.CannotNew, typeDesc.Type);
						this.WriteSourceBegin(member.Source);
						this.ilg.Ldloc(this.ilg.GetLocal(arrayName));
						this.WriteSourceEnd(member.Source, typeDesc.Type);
					}
					else
					{
						if (member.IsList && !member.Mapping.ReadOnly && member.Mapping.TypeDesc.IsNullable)
						{
							base.ILGenLoad(member.Source, typeof(object));
							this.ilg.Load(null);
							this.ilg.If(Cmp.EqualTo);
							if (!member.Mapping.TypeDesc.HasDefaultConstructor)
							{
								MethodInfo method = typeof(XmlSerializationReader).GetMethod("CreateReadOnlyCollectionException", CodeGenerator.InstanceBindingFlags, null, new Type[] { typeof(string) }, null);
								this.ilg.Ldarg(0);
								this.ilg.Ldstr(member.Mapping.TypeDesc.CSharpName);
								this.ilg.Call(method);
								this.ilg.Throw();
							}
							else
							{
								this.WriteSourceBegin(member.Source);
								base.RaCodeGen.ILGenForCreateInstance(this.ilg, member.Mapping.TypeDesc.Type, typeDesc.CannotNew, true);
								this.WriteSourceEnd(member.Source, member.Mapping.TypeDesc.Type);
							}
							this.ilg.EndIf();
						}
						this.WriteLocalDecl(arrayName, new SourceInfo(member.Source, member.Source, member.Mapping.MemberInfo, member.Mapping.TypeDesc.Type, this.ilg));
					}
				}
			}
		}

		private string ExpectedElements(XmlSerializationReaderILGen.Member[] members)
		{
			if (this.IsSequence(members))
			{
				return "null";
			}
			string text = string.Empty;
			bool flag = true;
			foreach (XmlSerializationReaderILGen.Member member in members)
			{
				if (member.Mapping.Xmlns == null && !member.Mapping.Ignore && !member.Mapping.IsText && !member.Mapping.IsAttribute)
				{
					foreach (ElementAccessor elementAccessor in member.Mapping.Elements)
					{
						string text2 = ((elementAccessor.Form == XmlSchemaForm.Qualified) ? elementAccessor.Namespace : "");
						if (!elementAccessor.Any || (elementAccessor.Name != null && elementAccessor.Name.Length != 0))
						{
							if (!flag)
							{
								text += ", ";
							}
							text = text + text2 + ":" + elementAccessor.Name;
							flag = false;
						}
					}
				}
			}
			return ReflectionAwareILGen.GetQuotedCSharpString(null, text);
		}

		private void WriteMemberElements(XmlSerializationReaderILGen.Member[] members, string elementElseString, string elseString, XmlSerializationReaderILGen.Member anyElement, XmlSerializationReaderILGen.Member anyText)
		{
			if (anyText != null)
			{
				this.ilg.Load(null);
				this.ilg.Stloc(typeof(string), "tmp");
			}
			MethodInfo method = typeof(XmlReader).GetMethod("get_NodeType", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
			MethodInfo method2 = typeof(XmlSerializationReader).GetMethod("get_Reader", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
			int num = 1;
			this.ilg.Ldarg(0);
			this.ilg.Call(method2);
			this.ilg.Call(method);
			this.ilg.Ldc(num);
			this.ilg.If(Cmp.EqualTo);
			this.WriteMemberElementsIf(members, anyElement, elementElseString);
			if (anyText != null)
			{
				this.WriteMemberText(anyText, elseString);
			}
			this.ilg.Else();
			this.ILGenElseString(elseString);
			this.ilg.EndIf();
		}

		private void WriteMemberText(XmlSerializationReaderILGen.Member anyText, string elseString)
		{
			this.ilg.InitElseIf();
			Label label = this.ilg.DefineLabel();
			Label label2 = this.ilg.DefineLabel();
			MethodInfo method = typeof(XmlSerializationReader).GetMethod("get_Reader", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
			MethodInfo method2 = typeof(XmlReader).GetMethod("get_NodeType", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
			this.ilg.Ldarg(0);
			this.ilg.Call(method);
			this.ilg.Call(method2);
			this.ilg.Ldc(XmlNodeType.Text);
			this.ilg.Ceq();
			this.ilg.Brtrue(label);
			this.ilg.Ldarg(0);
			this.ilg.Call(method);
			this.ilg.Call(method2);
			this.ilg.Ldc(XmlNodeType.CDATA);
			this.ilg.Ceq();
			this.ilg.Brtrue(label);
			this.ilg.Ldarg(0);
			this.ilg.Call(method);
			this.ilg.Call(method2);
			this.ilg.Ldc(XmlNodeType.Whitespace);
			this.ilg.Ceq();
			this.ilg.Brtrue(label);
			this.ilg.Ldarg(0);
			this.ilg.Call(method);
			this.ilg.Call(method2);
			this.ilg.Ldc(XmlNodeType.SignificantWhitespace);
			this.ilg.Ceq();
			this.ilg.Br(label2);
			this.ilg.MarkLabel(label);
			this.ilg.Ldc(true);
			this.ilg.MarkLabel(label2);
			this.ilg.AndIf();
			if (anyText != null)
			{
				this.WriteText(anyText);
			}
		}

		private void WriteText(XmlSerializationReaderILGen.Member member)
		{
			TextAccessor text = member.Mapping.Text;
			if (!(text.Mapping is SpecialMapping))
			{
				if (member.IsArrayLike)
				{
					this.WriteSourceBegin(member.ArraySource);
					if (text.Mapping.TypeDesc.CollapseWhitespace)
					{
						this.ilg.Ldarg(0);
					}
					MethodInfo method = typeof(XmlSerializationReader).GetMethod("get_Reader", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
					MethodInfo method2 = typeof(XmlReader).GetMethod("ReadString", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
					this.ilg.Ldarg(0);
					this.ilg.Call(method);
					this.ilg.Call(method2);
					if (text.Mapping.TypeDesc.CollapseWhitespace)
					{
						MethodInfo method3 = typeof(XmlSerializationReader).GetMethod("CollapseWhitespace", CodeGenerator.InstanceBindingFlags, null, new Type[] { typeof(string) }, null);
						this.ilg.Call(method3);
					}
				}
				else if (text.Mapping.TypeDesc == base.StringTypeDesc || text.Mapping.TypeDesc.FormatterName == "String")
				{
					LocalBuilder local = this.ilg.GetLocal("tmp");
					MethodInfo method4 = typeof(XmlSerializationReader).GetMethod("ReadString", CodeGenerator.InstanceBindingFlags, null, new Type[]
					{
						typeof(string),
						typeof(bool)
					}, null);
					this.ilg.Ldarg(0);
					this.ilg.Ldloc(local);
					this.ilg.Ldc(text.Mapping.TypeDesc.CollapseWhitespace);
					this.ilg.Call(method4);
					this.ilg.Stloc(local);
					this.WriteSourceBegin(member.ArraySource);
					this.ilg.Ldloc(local);
				}
				else
				{
					this.WriteSourceBegin(member.ArraySource);
					this.WritePrimitive(text.Mapping, "Reader.ReadString()");
				}
				this.WriteSourceEnd(member.ArraySource, text.Mapping.TypeDesc.Type);
				return;
			}
			SpecialMapping specialMapping = (SpecialMapping)text.Mapping;
			this.WriteSourceBeginTyped(member.ArraySource, specialMapping.TypeDesc);
			if (specialMapping.TypeDesc.Kind == TypeKind.Node)
			{
				MethodInfo method5 = typeof(XmlSerializationReader).GetMethod("get_Reader", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
				MethodInfo method6 = typeof(XmlReader).GetMethod("ReadString", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
				MethodInfo method7 = typeof(XmlSerializationReader).GetMethod("get_Document", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
				MethodInfo method8 = typeof(XmlDocument).GetMethod("CreateTextNode", CodeGenerator.InstanceBindingFlags, null, new Type[] { typeof(string) }, null);
				this.ilg.Ldarg(0);
				this.ilg.Call(method7);
				this.ilg.Ldarg(0);
				this.ilg.Call(method5);
				this.ilg.Call(method6);
				this.ilg.Call(method8);
				this.WriteSourceEnd(member.ArraySource, specialMapping.TypeDesc.Type);
				return;
			}
			throw new InvalidOperationException(Res.GetString("Internal error."));
		}

		private void WriteMemberElementsElse(XmlSerializationReaderILGen.Member anyElement, string elementElseString)
		{
			if (anyElement != null)
			{
				ElementAccessor[] elements = anyElement.Mapping.Elements;
				for (int i = 0; i < elements.Length; i++)
				{
					ElementAccessor elementAccessor = elements[i];
					if (elementAccessor.Any && elementAccessor.Name.Length == 0)
					{
						this.WriteElement(anyElement.ArraySource, anyElement.ArrayName, anyElement.ChoiceArraySource, elementAccessor, anyElement.Mapping.ChoiceIdentifier, (anyElement.Mapping.CheckSpecified == SpecifiedAccessor.ReadWrite) ? anyElement.CheckSpecifiedSource : null, false, false, -1, i);
						return;
					}
				}
				return;
			}
			this.ILGenElementElseString(elementElseString);
		}

		private bool IsSequence(XmlSerializationReaderILGen.Member[] members)
		{
			for (int i = 0; i < members.Length; i++)
			{
				if (members[i].Mapping.IsParticle && members[i].Mapping.IsSequence)
				{
					return true;
				}
			}
			return false;
		}

		private void WriteMemberElementsIf(XmlSerializationReaderILGen.Member[] members, XmlSerializationReaderILGen.Member anyElement, string elementElseString)
		{
			int num = 0;
			bool flag = this.IsSequence(members);
			int num2 = 0;
			foreach (XmlSerializationReaderILGen.Member member in members)
			{
				if (member.Mapping.Xmlns == null && !member.Mapping.Ignore && (!flag || (!member.Mapping.IsText && !member.Mapping.IsAttribute)))
				{
					bool flag2 = true;
					ChoiceIdentifierAccessor choiceIdentifier = member.Mapping.ChoiceIdentifier;
					ElementAccessor[] elements = member.Mapping.Elements;
					for (int j = 0; j < elements.Length; j++)
					{
						ElementAccessor elementAccessor = elements[j];
						string text = ((elementAccessor.Form == XmlSchemaForm.Qualified) ? elementAccessor.Namespace : "");
						if (flag || !elementAccessor.Any || (elementAccessor.Name != null && elementAccessor.Name.Length != 0))
						{
							if (!flag2 || (!flag && num > 0))
							{
								this.ilg.InitElseIf();
							}
							else if (flag)
							{
								if (num2 > 0)
								{
									this.ilg.InitElseIf();
								}
								else
								{
									this.ilg.InitIf();
								}
								this.ilg.Ldloc("state");
								this.ilg.Ldc(num2);
								this.ilg.AndIf(Cmp.EqualTo);
								this.ilg.InitIf();
							}
							else
							{
								this.ilg.InitIf();
							}
							num++;
							flag2 = false;
							if (member.ParamsReadSource != null)
							{
								this.ILGenParamsReadSource(member.ParamsReadSource);
								this.ilg.Ldc(false);
								this.ilg.AndIf(Cmp.EqualTo);
							}
							Label label = this.ilg.DefineLabel();
							Label label2 = this.ilg.DefineLabel();
							if (member.Mapping.IsReturnValue)
							{
								MethodInfo method = typeof(XmlSerializationReader).GetMethod("get_IsReturnValue", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
								this.ilg.Ldarg(0);
								this.ilg.Call(method);
								this.ilg.Brtrue(label);
							}
							if (flag && elementAccessor.Any && elementAccessor.AnyNamespaces == null)
							{
								this.ilg.Ldc(true);
							}
							else
							{
								this.WriteXmlNodeEqual("Reader", elementAccessor.Name, text, false);
							}
							if (member.Mapping.IsReturnValue)
							{
								this.ilg.Br_S(label2);
								this.ilg.MarkLabel(label);
								this.ilg.Ldc(true);
								this.ilg.MarkLabel(label2);
							}
							this.ilg.AndIf();
							this.WriteElement(member.ArraySource, member.ArrayName, member.ChoiceArraySource, elementAccessor, choiceIdentifier, (member.Mapping.CheckSpecified == SpecifiedAccessor.ReadWrite) ? member.CheckSpecifiedSource : null, member.IsList && member.Mapping.TypeDesc.IsNullable, member.Mapping.ReadOnly, member.FixupIndex, j);
							if (member.Mapping.IsReturnValue)
							{
								MethodInfo method2 = typeof(XmlSerializationReader).GetMethod("set_IsReturnValue", CodeGenerator.InstanceBindingFlags, null, new Type[] { typeof(bool) }, null);
								this.ilg.Ldarg(0);
								this.ilg.Ldc(false);
								this.ilg.Call(method2);
							}
							if (member.ParamsReadSource != null)
							{
								this.ILGenParamsReadSource(member.ParamsReadSource, true);
							}
						}
					}
					if (flag)
					{
						if (member.IsArrayLike)
						{
							this.ilg.Else();
						}
						else
						{
							this.ilg.EndIf();
						}
						num2++;
						this.ilg.Ldc(num2);
						this.ilg.Stloc(this.ilg.GetLocal("state"));
						if (member.IsArrayLike)
						{
							this.ilg.EndIf();
						}
					}
				}
			}
			if (num > 0)
			{
				this.ilg.Else();
			}
			this.WriteMemberElementsElse(anyElement, elementElseString);
			if (num > 0)
			{
				this.ilg.EndIf();
			}
		}

		private string GetArraySource(TypeDesc typeDesc, string arrayName)
		{
			return this.GetArraySource(typeDesc, arrayName, false);
		}

		private string GetArraySource(TypeDesc typeDesc, string arrayName, bool multiRef)
		{
			string text = "c" + arrayName;
			string text2 = "";
			if (multiRef)
			{
				text2 = "soap = (System.Object[])EnsureArrayIndex(soap, " + text + "+2, typeof(System.Object)); ";
			}
			if (typeDesc.IsArray)
			{
				string csharpName = typeDesc.ArrayElementTypeDesc.CSharpName;
				string text3 = "(" + csharpName + "[])";
				text2 = string.Concat(new string[]
				{
					text2,
					arrayName,
					" = ",
					text3,
					"EnsureArrayIndex(",
					arrayName,
					", ",
					text,
					", ",
					base.RaCodeGen.GetStringForTypeof(csharpName),
					");"
				});
				string stringForArrayMember = base.RaCodeGen.GetStringForArrayMember(arrayName, text + "++", typeDesc);
				if (multiRef)
				{
					text2 = text2 + " soap[1] = " + arrayName + ";";
					text2 = string.Concat(new string[] { text2, " if (ReadReference(out soap[", text, "+2])) ", stringForArrayMember, " = null; else " });
				}
				return text2 + stringForArrayMember;
			}
			return base.RaCodeGen.GetStringForMethod(arrayName, typeDesc.CSharpName, "Add");
		}

		private void WriteMemberEnd(XmlSerializationReaderILGen.Member[] members)
		{
			this.WriteMemberEnd(members, false);
		}

		private void WriteMemberEnd(XmlSerializationReaderILGen.Member[] members, bool soapRefs)
		{
			foreach (XmlSerializationReaderILGen.Member member in members)
			{
				if (member.IsArrayLike)
				{
					TypeDesc typeDesc = member.Mapping.TypeDesc;
					if (typeDesc.IsArray)
					{
						this.WriteSourceBegin(member.Source);
						string text = member.ArrayName;
						string text2 = "c" + text;
						MethodInfo method = typeof(XmlSerializationReader).GetMethod("ShrinkArray", CodeGenerator.InstanceBindingFlags, null, new Type[]
						{
							typeof(Array),
							typeof(int),
							typeof(Type),
							typeof(bool)
						}, null);
						this.ilg.Ldarg(0);
						this.ilg.Ldloc(this.ilg.GetLocal(text));
						this.ilg.Ldloc(this.ilg.GetLocal(text2));
						this.ilg.Ldc(typeDesc.ArrayElementTypeDesc.Type);
						this.ilg.Ldc(member.IsNullable);
						this.ilg.Call(method);
						this.ilg.ConvertValue(method.ReturnType, typeDesc.Type);
						this.WriteSourceEnd(member.Source, typeDesc.Type);
						if (member.Mapping.ChoiceIdentifier != null)
						{
							this.WriteSourceBegin(member.ChoiceSource);
							text = member.ChoiceArrayName;
							text2 = "c" + text;
							this.ilg.Ldarg(0);
							this.ilg.Ldloc(this.ilg.GetLocal(text));
							this.ilg.Ldloc(this.ilg.GetLocal(text2));
							this.ilg.Ldc(member.Mapping.ChoiceIdentifier.Mapping.TypeDesc.Type);
							this.ilg.Ldc(member.IsNullable);
							this.ilg.Call(method);
							this.ilg.ConvertValue(method.ReturnType, member.Mapping.ChoiceIdentifier.Mapping.TypeDesc.Type.MakeArrayType());
							this.WriteSourceEnd(member.ChoiceSource, member.Mapping.ChoiceIdentifier.Mapping.TypeDesc.Type.MakeArrayType());
						}
					}
					else if (typeDesc.IsValueType)
					{
						LocalBuilder local = this.ilg.GetLocal(member.ArrayName);
						this.WriteSourceBegin(member.Source);
						this.ilg.Ldloc(local);
						this.WriteSourceEnd(member.Source, local.LocalType);
					}
				}
			}
		}

		private void WriteSourceBeginTyped(string source, TypeDesc typeDesc)
		{
			this.WriteSourceBegin(source);
		}

		private void WriteSourceBegin(string source)
		{
			object obj;
			if (this.ilg.TryGetVariable(source, out obj))
			{
				if (CodeGenerator.IsNullableGenericType(this.ilg.GetVariableType(obj)))
				{
					this.ilg.LoadAddress(obj);
				}
				return;
			}
			if (source.StartsWith("o.@", StringComparison.Ordinal))
			{
				this.ilg.LdlocAddress(this.ilg.GetLocal("o"));
				return;
			}
			Match match = XmlSerializationILGen.NewRegex("(?<locA1>[^ ]+) = .+EnsureArrayIndex[(](?<locA2>[^,]+), (?<locI1>[^,]+),[^;]+;(?<locA3>[^[]+)[[](?<locI2>[^+]+)[+][+][]]").Match(source);
			if (match.Success)
			{
				LocalBuilder local = this.ilg.GetLocal(match.Groups["locA1"].Value);
				LocalBuilder local2 = this.ilg.GetLocal(match.Groups["locI1"].Value);
				Type elementType = local.LocalType.GetElementType();
				MethodInfo method = typeof(XmlSerializationReader).GetMethod("EnsureArrayIndex", CodeGenerator.InstanceBindingFlags, null, new Type[]
				{
					typeof(Array),
					typeof(int),
					typeof(Type)
				}, null);
				this.ilg.Ldarg(0);
				this.ilg.Ldloc(local);
				this.ilg.Ldloc(local2);
				this.ilg.Ldc(elementType);
				this.ilg.Call(method);
				this.ilg.Castclass(local.LocalType);
				this.ilg.Stloc(local);
				this.ilg.Ldloc(local);
				this.ilg.Ldloc(local2);
				this.ilg.Dup();
				this.ilg.Ldc(1);
				this.ilg.Add();
				this.ilg.Stloc(local2);
				if (CodeGenerator.IsNullableGenericType(elementType) || elementType.IsValueType)
				{
					this.ilg.Ldelema(elementType);
				}
				return;
			}
			if (source.EndsWith(".Add(", StringComparison.Ordinal))
			{
				int num = source.LastIndexOf(".Add(", StringComparison.Ordinal);
				LocalBuilder local3 = this.ilg.GetLocal(source.Substring(0, num));
				this.ilg.LdlocAddress(local3);
				return;
			}
			match = XmlSerializationILGen.NewRegex("(?<a>[^[]+)[[](?<ia>.+)[]]").Match(source);
			if (match.Success)
			{
				this.ilg.Load(this.ilg.GetVariable(match.Groups["a"].Value));
				this.ilg.Load(this.ilg.GetVariable(match.Groups["ia"].Value));
				return;
			}
			throw CodeGenerator.NotSupported("Unexpected: " + source);
		}

		private void WriteSourceEnd(string source, Type elementType)
		{
			this.WriteSourceEnd(source, elementType, elementType);
		}

		private void WriteSourceEnd(string source, Type elementType, Type stackType)
		{
			object obj;
			if (this.ilg.TryGetVariable(source, out obj))
			{
				Type variableType = this.ilg.GetVariableType(obj);
				if (CodeGenerator.IsNullableGenericType(variableType))
				{
					this.ilg.Call(variableType.GetConstructor(variableType.GetGenericArguments()));
					return;
				}
				this.ilg.ConvertValue(stackType, elementType);
				this.ilg.ConvertValue(elementType, variableType);
				this.ilg.Stloc((LocalBuilder)obj);
				return;
			}
			else
			{
				if (source.StartsWith("o.@", StringComparison.Ordinal))
				{
					MemberInfo memberInfo = this.memberInfos[source.Substring(3)];
					this.ilg.ConvertValue(stackType, (memberInfo.MemberType == MemberTypes.Field) ? ((FieldInfo)memberInfo).FieldType : ((PropertyInfo)memberInfo).PropertyType);
					this.ilg.StoreMember(memberInfo);
					return;
				}
				Match match = XmlSerializationILGen.NewRegex("(?<locA1>[^ ]+) = .+EnsureArrayIndex[(](?<locA2>[^,]+), (?<locI1>[^,]+),[^;]+;(?<locA3>[^[]+)[[](?<locI2>[^+]+)[+][+][]]").Match(source);
				if (match.Success)
				{
					object variable = this.ilg.GetVariable(match.Groups["locA1"].Value);
					Type elementType2 = this.ilg.GetVariableType(variable).GetElementType();
					this.ilg.ConvertValue(elementType, elementType2);
					if (CodeGenerator.IsNullableGenericType(elementType2) || elementType2.IsValueType)
					{
						this.ilg.Stobj(elementType2);
						return;
					}
					this.ilg.Stelem(elementType2);
					return;
				}
				else
				{
					if (source.EndsWith(".Add(", StringComparison.Ordinal))
					{
						int num = source.LastIndexOf(".Add(", StringComparison.Ordinal);
						MethodInfo method = this.ilg.GetLocal(source.Substring(0, num)).LocalType.GetMethod("Add", CodeGenerator.InstanceBindingFlags, null, new Type[] { elementType }, null);
						Type parameterType = method.GetParameters()[0].ParameterType;
						this.ilg.ConvertValue(stackType, parameterType);
						this.ilg.Call(method);
						if (method.ReturnType != typeof(void))
						{
							this.ilg.Pop();
						}
						return;
					}
					match = XmlSerializationILGen.NewRegex("(?<a>[^[]+)[[](?<ia>.+)[]]").Match(source);
					if (match.Success)
					{
						Type elementType3 = this.ilg.GetVariableType(this.ilg.GetVariable(match.Groups["a"].Value)).GetElementType();
						this.ilg.ConvertValue(stackType, elementType3);
						this.ilg.Stelem(elementType3);
						return;
					}
					throw CodeGenerator.NotSupported("Unexpected: " + source);
				}
			}
		}

		private void WriteArray(string source, string arrayName, ArrayMapping arrayMapping, bool readOnly, bool isNullable, int fixupIndex, int elementIndex)
		{
			MethodInfo method = typeof(XmlSerializationReader).GetMethod("ReadNull", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
			this.ilg.Ldarg(0);
			this.ilg.Call(method);
			this.ilg.IfNot();
			MemberMapping memberMapping = new MemberMapping();
			memberMapping.Elements = arrayMapping.Elements;
			memberMapping.TypeDesc = arrayMapping.TypeDesc;
			memberMapping.ReadOnly = readOnly;
			if (source.StartsWith("o.@", StringComparison.Ordinal))
			{
				memberMapping.MemberInfo = this.memberInfos[source.Substring(3)];
			}
			XmlSerializationReaderILGen.Member member = new XmlSerializationReaderILGen.Member(this, source, arrayName, elementIndex, memberMapping, false);
			member.IsNullable = false;
			XmlSerializationReaderILGen.Member[] array = new XmlSerializationReaderILGen.Member[] { member };
			this.WriteMemberBegin(array);
			Label label = this.ilg.DefineLabel();
			Label label2 = this.ilg.DefineLabel();
			if (readOnly)
			{
				this.ilg.Load(this.ilg.GetVariable(member.ArrayName));
				this.ilg.Load(null);
				this.ilg.Beq(label);
			}
			MethodInfo method2 = typeof(XmlSerializationReader).GetMethod("get_Reader", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
			MethodInfo method3 = typeof(XmlReader).GetMethod("get_IsEmptyElement", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
			this.ilg.Ldarg(0);
			this.ilg.Call(method2);
			this.ilg.Call(method3);
			if (readOnly)
			{
				this.ilg.Br_S(label2);
				this.ilg.MarkLabel(label);
				this.ilg.Ldc(true);
				this.ilg.MarkLabel(label2);
			}
			this.ilg.If();
			MethodInfo method4 = typeof(XmlReader).GetMethod("Skip", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
			this.ilg.Ldarg(0);
			this.ilg.Call(method2);
			this.ilg.Call(method4);
			this.ilg.Else();
			MethodInfo method5 = typeof(XmlReader).GetMethod("ReadStartElement", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
			this.ilg.Ldarg(0);
			this.ilg.Call(method2);
			this.ilg.Call(method5);
			int num = this.WriteWhileNotLoopStart();
			string text = "UnknownNode(null, " + this.ExpectedElements(array) + ");";
			this.WriteMemberElements(array, text, text, null, null);
			MethodInfo method6 = typeof(XmlReader).GetMethod("MoveToContent", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
			this.ilg.Ldarg(0);
			this.ilg.Call(method2);
			this.ilg.Call(method6);
			this.ilg.Pop();
			this.WriteWhileLoopEnd(num);
			MethodInfo method7 = typeof(XmlSerializationReader).GetMethod("ReadEndElement", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
			this.ilg.Ldarg(0);
			this.ilg.Call(method7);
			this.ilg.EndIf();
			this.WriteMemberEnd(array, false);
			if (isNullable)
			{
				this.ilg.Else();
				member.IsNullable = true;
				this.WriteMemberBegin(array);
				this.WriteMemberEnd(array);
			}
			this.ilg.EndIf();
		}

		private void WriteElement(string source, string arrayName, string choiceSource, ElementAccessor element, ChoiceIdentifierAccessor choice, string checkSpecified, bool checkForNull, bool readOnly, int fixupIndex, int elementIndex)
		{
			if (checkSpecified != null && checkSpecified.Length > 0)
			{
				this.ILGenSet(checkSpecified, true);
			}
			if (element.Mapping is ArrayMapping)
			{
				this.WriteArray(source, arrayName, (ArrayMapping)element.Mapping, readOnly, element.IsNullable, fixupIndex, elementIndex);
			}
			else if (element.Mapping is NullableMapping)
			{
				string text = base.ReferenceMapping(element.Mapping);
				this.WriteSourceBegin(source);
				this.ilg.Ldarg(0);
				this.ilg.Ldc(true);
				MethodBuilder methodBuilder = base.EnsureMethodBuilder(this.typeBuilder, text, CodeGenerator.PrivateMethodAttributes, element.Mapping.TypeDesc.Type, new Type[] { typeof(bool) });
				this.ilg.Call(methodBuilder);
				this.WriteSourceEnd(source, element.Mapping.TypeDesc.Type);
			}
			else if (element.Mapping is PrimitiveMapping)
			{
				bool flag = false;
				if (element.IsNullable)
				{
					MethodInfo method = typeof(XmlSerializationReader).GetMethod("ReadNull", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
					this.ilg.Ldarg(0);
					this.ilg.Call(method);
					this.ilg.If();
					this.WriteSourceBegin(source);
					if (element.Mapping.TypeDesc.IsValueType)
					{
						throw CodeGenerator.NotSupported("No such condition.  PrimitiveMapping && IsNullable = String, XmlQualifiedName and never IsValueType");
					}
					this.ilg.Load(null);
					this.WriteSourceEnd(source, element.Mapping.TypeDesc.Type);
					this.ilg.Else();
					flag = true;
				}
				if (element.Default != null && element.Default != DBNull.Value && element.Mapping.TypeDesc.IsValueType)
				{
					MethodInfo method2 = typeof(XmlSerializationReader).GetMethod("get_Reader", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
					MethodInfo method3 = typeof(XmlReader).GetMethod("get_IsEmptyElement", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
					this.ilg.Ldarg(0);
					this.ilg.Call(method2);
					this.ilg.Call(method3);
					this.ilg.If();
					MethodInfo method4 = typeof(XmlReader).GetMethod("Skip", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
					this.ilg.Ldarg(0);
					this.ilg.Call(method2);
					this.ilg.Call(method4);
					this.ilg.Else();
					flag = true;
				}
				if (LocalAppContextSwitches.EnableTimeSpanSerialization && element.Mapping.TypeDesc.Type == typeof(TimeSpan))
				{
					MethodInfo method5 = typeof(XmlSerializationReader).GetMethod("get_Reader", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
					MethodInfo method6 = typeof(XmlReader).GetMethod("get_IsEmptyElement", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
					this.ilg.Ldarg(0);
					this.ilg.Call(method5);
					this.ilg.Call(method6);
					this.ilg.If();
					this.WriteSourceBegin(source);
					MethodInfo method7 = typeof(XmlReader).GetMethod("Skip", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
					this.ilg.Ldarg(0);
					this.ilg.Call(method5);
					this.ilg.Call(method7);
					ConstructorInfo constructor = typeof(TimeSpan).GetConstructor(CodeGenerator.InstanceBindingFlags, null, new Type[] { typeof(long) }, null);
					this.ilg.Ldc(default(TimeSpan).Ticks);
					this.ilg.New(constructor);
					this.WriteSourceEnd(source, element.Mapping.TypeDesc.Type);
					this.ilg.Else();
					this.WriteSourceBegin(source);
					this.WritePrimitive(element.Mapping, "Reader.ReadElementString()");
					this.WriteSourceEnd(source, element.Mapping.TypeDesc.Type);
					this.ilg.EndIf();
				}
				else
				{
					this.WriteSourceBegin(source);
					if (element.Mapping.TypeDesc == base.QnameTypeDesc)
					{
						MethodInfo method8 = typeof(XmlSerializationReader).GetMethod("ReadElementQualifiedName", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
						this.ilg.Ldarg(0);
						this.ilg.Call(method8);
					}
					else
					{
						string formatterName = element.Mapping.TypeDesc.FormatterName;
						string text2;
						if (formatterName == "ByteArrayBase64" || formatterName == "ByteArrayHex")
						{
							text2 = "false";
						}
						else
						{
							text2 = "Reader.ReadElementString()";
						}
						this.WritePrimitive(element.Mapping, text2);
					}
					this.WriteSourceEnd(source, element.Mapping.TypeDesc.Type);
				}
				if (flag)
				{
					this.ilg.EndIf();
				}
			}
			else if (element.Mapping is StructMapping)
			{
				TypeMapping mapping = element.Mapping;
				string text3 = base.ReferenceMapping(mapping);
				if (checkForNull)
				{
					MethodInfo method9 = typeof(XmlSerializationReader).GetMethod("get_Reader", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
					MethodInfo method10 = typeof(XmlReader).GetMethod("Skip", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
					this.ilg.Ldloc(arrayName);
					this.ilg.Load(null);
					this.ilg.If(Cmp.EqualTo);
					this.ilg.Ldarg(0);
					this.ilg.Call(method9);
					this.ilg.Call(method10);
					this.ilg.Else();
				}
				this.WriteSourceBegin(source);
				List<Type> list = new List<Type>();
				this.ilg.Ldarg(0);
				if (mapping.TypeDesc.IsNullable)
				{
					this.ilg.Load(element.IsNullable);
					list.Add(typeof(bool));
				}
				this.ilg.Ldc(true);
				list.Add(typeof(bool));
				MethodBuilder methodBuilder2 = base.EnsureMethodBuilder(this.typeBuilder, text3, CodeGenerator.PrivateMethodAttributes, mapping.TypeDesc.Type, list.ToArray());
				this.ilg.Call(methodBuilder2);
				this.WriteSourceEnd(source, mapping.TypeDesc.Type);
				if (checkForNull)
				{
					this.ilg.EndIf();
				}
			}
			else
			{
				if (!(element.Mapping is SpecialMapping))
				{
					throw new InvalidOperationException(Res.GetString("Internal error."));
				}
				SpecialMapping specialMapping = (SpecialMapping)element.Mapping;
				TypeKind kind = specialMapping.TypeDesc.Kind;
				if (kind != TypeKind.Node)
				{
					if (kind != TypeKind.Serializable)
					{
						throw new InvalidOperationException(Res.GetString("Internal error."));
					}
					SerializableMapping serializableMapping = (SerializableMapping)element.Mapping;
					if (serializableMapping.DerivedMappings != null)
					{
						MethodInfo method11 = typeof(XmlSerializationReader).GetMethod("GetXsiType", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
						Label label = this.ilg.DefineLabel();
						Label label2 = this.ilg.DefineLabel();
						LocalBuilder localBuilder = this.ilg.DeclareOrGetLocal(typeof(XmlQualifiedName), "tser");
						this.ilg.Ldarg(0);
						this.ilg.Call(method11);
						this.ilg.Stloc(localBuilder);
						this.ilg.Ldloc(localBuilder);
						this.ilg.Load(null);
						this.ilg.Ceq();
						this.ilg.Brtrue(label);
						this.WriteQNameEqual("tser", serializableMapping.XsiType.Name, serializableMapping.XsiType.Namespace);
						this.ilg.Br_S(label2);
						this.ilg.MarkLabel(label);
						this.ilg.Ldc(true);
						this.ilg.MarkLabel(label2);
						this.ilg.If();
					}
					this.WriteSourceBeginTyped(source, serializableMapping.TypeDesc);
					bool flag2 = !element.Any && XmlSerializationILGen.IsWildcard(serializableMapping);
					Type typeFromHandle = typeof(XmlSerializationReader);
					string text4 = "ReadSerializable";
					BindingFlags instanceBindingFlags = CodeGenerator.InstanceBindingFlags;
					Binder binder = null;
					Type[] array;
					if (!flag2)
					{
						(array = new Type[1])[0] = typeof(IXmlSerializable);
					}
					else
					{
						Type[] array2 = new Type[2];
						array2[0] = typeof(IXmlSerializable);
						array = array2;
						array2[1] = typeof(bool);
					}
					MethodInfo method12 = typeFromHandle.GetMethod(text4, instanceBindingFlags, binder, array, null);
					this.ilg.Ldarg(0);
					base.RaCodeGen.ILGenForCreateInstance(this.ilg, serializableMapping.TypeDesc.Type, serializableMapping.TypeDesc.CannotNew, false);
					if (serializableMapping.TypeDesc.CannotNew)
					{
						this.ilg.ConvertValue(typeof(object), typeof(IXmlSerializable));
					}
					if (flag2)
					{
						this.ilg.Ldc(true);
					}
					this.ilg.Call(method12);
					if (serializableMapping.TypeDesc != null)
					{
						this.ilg.ConvertValue(typeof(IXmlSerializable), serializableMapping.TypeDesc.Type);
					}
					this.WriteSourceEnd(source, serializableMapping.TypeDesc.Type);
					if (serializableMapping.DerivedMappings != null)
					{
						this.WriteDerivedSerializable(serializableMapping, serializableMapping, source, flag2);
						this.WriteUnknownNode("UnknownNode", "null", null, true);
					}
				}
				else
				{
					bool flag3 = specialMapping.TypeDesc.FullName == typeof(XmlDocument).FullName;
					this.WriteSourceBeginTyped(source, specialMapping.TypeDesc);
					MethodInfo method13 = typeof(XmlSerializationReader).GetMethod(flag3 ? "ReadXmlDocument" : "ReadXmlNode", CodeGenerator.InstanceBindingFlags, null, new Type[] { typeof(bool) }, null);
					this.ilg.Ldarg(0);
					this.ilg.Ldc(!element.Any);
					this.ilg.Call(method13);
					if (specialMapping.TypeDesc != null)
					{
						this.ilg.Castclass(specialMapping.TypeDesc.Type);
					}
					this.WriteSourceEnd(source, specialMapping.TypeDesc.Type);
				}
			}
			if (choice != null)
			{
				this.WriteSourceBegin(choiceSource);
				CodeIdentifier.CheckValidIdentifier(choice.MemberIds[elementIndex]);
				base.RaCodeGen.ILGenForEnumMember(this.ilg, choice.Mapping.TypeDesc.Type, choice.MemberIds[elementIndex]);
				this.WriteSourceEnd(choiceSource, choice.Mapping.TypeDesc.Type);
			}
		}

		private void WriteDerivedSerializable(SerializableMapping head, SerializableMapping mapping, string source, bool isWrappedAny)
		{
			if (mapping == null)
			{
				return;
			}
			for (SerializableMapping serializableMapping = mapping.DerivedMappings; serializableMapping != null; serializableMapping = serializableMapping.NextDerivedMapping)
			{
				Label label = this.ilg.DefineLabel();
				Label label2 = this.ilg.DefineLabel();
				LocalBuilder local = this.ilg.GetLocal("tser");
				this.ilg.InitElseIf();
				this.ilg.Ldloc(local);
				this.ilg.Load(null);
				this.ilg.Ceq();
				this.ilg.Brtrue(label);
				this.WriteQNameEqual("tser", serializableMapping.XsiType.Name, serializableMapping.XsiType.Namespace);
				this.ilg.Br_S(label2);
				this.ilg.MarkLabel(label);
				this.ilg.Ldc(true);
				this.ilg.MarkLabel(label2);
				this.ilg.AndIf();
				if (serializableMapping.Type != null)
				{
					if (head.Type.IsAssignableFrom(serializableMapping.Type))
					{
						this.WriteSourceBeginTyped(source, head.TypeDesc);
						Type typeFromHandle = typeof(XmlSerializationReader);
						string text = "ReadSerializable";
						BindingFlags instanceBindingFlags = CodeGenerator.InstanceBindingFlags;
						Binder binder = null;
						Type[] array;
						if (!isWrappedAny)
						{
							(array = new Type[1])[0] = typeof(IXmlSerializable);
						}
						else
						{
							Type[] array2 = new Type[2];
							array2[0] = typeof(IXmlSerializable);
							array = array2;
							array2[1] = typeof(bool);
						}
						MethodInfo method = typeFromHandle.GetMethod(text, instanceBindingFlags, binder, array, null);
						this.ilg.Ldarg(0);
						base.RaCodeGen.ILGenForCreateInstance(this.ilg, serializableMapping.TypeDesc.Type, serializableMapping.TypeDesc.CannotNew, false);
						if (serializableMapping.TypeDesc.CannotNew)
						{
							this.ilg.ConvertValue(typeof(object), typeof(IXmlSerializable));
						}
						if (isWrappedAny)
						{
							this.ilg.Ldc(true);
						}
						this.ilg.Call(method);
						if (head.TypeDesc != null)
						{
							this.ilg.ConvertValue(typeof(IXmlSerializable), head.TypeDesc.Type);
						}
						this.WriteSourceEnd(source, head.TypeDesc.Type);
					}
					else
					{
						MethodInfo method2 = typeof(XmlSerializationReader).GetMethod("CreateBadDerivationException", CodeGenerator.InstanceBindingFlags, null, new Type[]
						{
							typeof(string),
							typeof(string),
							typeof(string),
							typeof(string),
							typeof(string),
							typeof(string)
						}, null);
						this.ilg.Ldarg(0);
						this.ilg.Ldstr(serializableMapping.XsiType.Name);
						this.ilg.Ldstr(serializableMapping.XsiType.Namespace);
						this.ilg.Ldstr(head.XsiType.Name);
						this.ilg.Ldstr(head.XsiType.Namespace);
						this.ilg.Ldstr(serializableMapping.Type.FullName);
						this.ilg.Ldstr(head.Type.FullName);
						this.ilg.Call(method2);
						this.ilg.Throw();
					}
				}
				else
				{
					MethodInfo method3 = typeof(XmlSerializationReader).GetMethod("CreateMissingIXmlSerializableType", CodeGenerator.InstanceBindingFlags, null, new Type[]
					{
						typeof(string),
						typeof(string),
						typeof(string)
					}, null);
					this.ilg.Ldarg(0);
					this.ilg.Ldstr(serializableMapping.XsiType.Name);
					this.ilg.Ldstr(serializableMapping.XsiType.Namespace);
					this.ilg.Ldstr(head.Type.FullName);
					this.ilg.Call(method3);
					this.ilg.Throw();
				}
				this.WriteDerivedSerializable(head, serializableMapping, source, isWrappedAny);
			}
		}

		private int WriteWhileNotLoopStart()
		{
			MethodInfo method = typeof(XmlSerializationReader).GetMethod("get_Reader", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
			MethodInfo method2 = typeof(XmlReader).GetMethod("MoveToContent", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
			this.ilg.Ldarg(0);
			this.ilg.Call(method);
			this.ilg.Call(method2);
			this.ilg.Pop();
			int num = this.WriteWhileLoopStartCheck();
			this.ilg.WhileBegin();
			return num;
		}

		private void WriteWhileLoopEnd(int loopIndex)
		{
			this.WriteWhileLoopEndCheck(loopIndex);
			this.ilg.WhileBeginCondition();
			int num = 0;
			int num2 = 15;
			MethodInfo method = typeof(XmlSerializationReader).GetMethod("get_Reader", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
			MethodInfo method2 = typeof(XmlReader).GetMethod("get_NodeType", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
			Label label = this.ilg.DefineLabel();
			Label label2 = this.ilg.DefineLabel();
			this.ilg.Ldarg(0);
			this.ilg.Call(method);
			this.ilg.Call(method2);
			this.ilg.Ldc(num2);
			this.ilg.Beq(label);
			this.ilg.Ldarg(0);
			this.ilg.Call(method);
			this.ilg.Call(method2);
			this.ilg.Ldc(num);
			this.ilg.Cne();
			this.ilg.Br_S(label2);
			this.ilg.MarkLabel(label);
			this.ilg.Ldc(false);
			this.ilg.MarkLabel(label2);
			this.ilg.WhileEndCondition();
			this.ilg.WhileEnd();
		}

		private int WriteWhileLoopStartCheck()
		{
			MethodInfo method = typeof(XmlSerializationReader).GetMethod("get_ReaderCount", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
			this.ilg.Ldc(0);
			this.ilg.Stloc(typeof(int), string.Format(CultureInfo.InvariantCulture, "whileIterations{0}", this.nextWhileLoopIndex));
			this.ilg.Ldarg(0);
			this.ilg.Call(method);
			this.ilg.Stloc(typeof(int), string.Format(CultureInfo.InvariantCulture, "readerCount{0}", this.nextWhileLoopIndex));
			int num = this.nextWhileLoopIndex;
			this.nextWhileLoopIndex = num + 1;
			return num;
		}

		private void WriteWhileLoopEndCheck(int loopIndex)
		{
			Type type = Type.GetType("System.Int32&");
			MethodInfo method = typeof(XmlSerializationReader).GetMethod("CheckReaderCount", CodeGenerator.InstanceBindingFlags, null, new Type[] { type, type }, null);
			this.ilg.Ldarg(0);
			this.ilg.Ldloca(this.ilg.GetLocal(string.Format(CultureInfo.InvariantCulture, "whileIterations{0}", loopIndex)));
			this.ilg.Ldloca(this.ilg.GetLocal(string.Format(CultureInfo.InvariantCulture, "readerCount{0}", loopIndex)));
			this.ilg.Call(method);
		}

		private void WriteParamsRead(int length)
		{
			LocalBuilder localBuilder = this.ilg.DeclareLocal(typeof(bool[]), "paramsRead");
			this.ilg.NewArray(typeof(bool), length);
			this.ilg.Stloc(localBuilder);
		}

		private void WriteCreateMapping(TypeMapping mapping, string local)
		{
			string csharpName = mapping.TypeDesc.CSharpName;
			bool cannotNew = mapping.TypeDesc.CannotNew;
			LocalBuilder localBuilder = this.ilg.DeclareLocal(mapping.TypeDesc.Type, local);
			if (cannotNew)
			{
				this.ilg.BeginExceptionBlock();
			}
			base.RaCodeGen.ILGenForCreateInstance(this.ilg, mapping.TypeDesc.Type, mapping.TypeDesc.CannotNew, true);
			this.ilg.Stloc(localBuilder);
			if (cannotNew)
			{
				this.ilg.Leave();
				this.WriteCatchException(typeof(MissingMethodException));
				MethodInfo method = typeof(XmlSerializationReader).GetMethod("CreateInaccessibleConstructorException", CodeGenerator.InstanceBindingFlags, null, new Type[] { typeof(string) }, null);
				this.ilg.Ldarg(0);
				this.ilg.Ldstr(csharpName);
				this.ilg.Call(method);
				this.ilg.Throw();
				this.WriteCatchException(typeof(SecurityException));
				MethodInfo method2 = typeof(XmlSerializationReader).GetMethod("CreateCtorHasSecurityException", CodeGenerator.InstanceBindingFlags, null, new Type[] { typeof(string) }, null);
				this.ilg.Ldarg(0);
				this.ilg.Ldstr(csharpName);
				this.ilg.Call(method2);
				this.ilg.Throw();
				this.ilg.EndExceptionBlock();
			}
		}

		private void WriteCatchException(Type exceptionType)
		{
			this.ilg.BeginCatchBlock(exceptionType);
			this.ilg.Pop();
		}

		private void WriteCatchCastException(TypeDesc typeDesc, string source, string id)
		{
			this.WriteCatchException(typeof(InvalidCastException));
			MethodInfo method = typeof(XmlSerializationReader).GetMethod("CreateInvalidCastException", CodeGenerator.InstanceBindingFlags, null, new Type[]
			{
				typeof(Type),
				typeof(object),
				typeof(string)
			}, null);
			this.ilg.Ldarg(0);
			this.ilg.Ldc(typeDesc.Type);
			if (source.StartsWith("GetTarget(ids[", StringComparison.Ordinal))
			{
				MethodInfo method2 = typeof(XmlSerializationReader).GetMethod("GetTarget", CodeGenerator.InstanceBindingFlags, null, new Type[] { typeof(string) }, null);
				object variable = this.ilg.GetVariable("ids");
				this.ilg.Ldarg(0);
				this.ilg.LoadArrayElement(variable, int.Parse(source.Substring(14, source.Length - 16), CultureInfo.InvariantCulture));
				this.ilg.Call(method2);
			}
			else
			{
				this.ilg.Load(this.ilg.GetVariable(source));
			}
			if (id == null)
			{
				this.ilg.Load(null);
			}
			else if (id.StartsWith("ids[", StringComparison.Ordinal))
			{
				object variable2 = this.ilg.GetVariable("ids");
				this.ilg.LoadArrayElement(variable2, int.Parse(id.Substring(4, id.Length - 5), CultureInfo.InvariantCulture));
			}
			else
			{
				object variable3 = this.ilg.GetVariable(id);
				this.ilg.Load(variable3);
				this.ilg.ConvertValue(this.ilg.GetVariableType(variable3), typeof(string));
			}
			this.ilg.Call(method);
			this.ilg.Throw();
		}

		private void WriteArrayLocalDecl(string typeName, string variableName, string initValue, TypeDesc arrayTypeDesc)
		{
			base.RaCodeGen.WriteArrayLocalDecl(typeName, variableName, new SourceInfo(initValue, initValue, null, arrayTypeDesc.Type, this.ilg), arrayTypeDesc);
		}

		private void WriteCreateInstance(string source, bool ctorInaccessible, Type type)
		{
			base.RaCodeGen.WriteCreateInstance(source, ctorInaccessible, type, this.ilg);
		}

		private void WriteLocalDecl(string variableName, SourceInfo initValue)
		{
			base.RaCodeGen.WriteLocalDecl(variableName, initValue);
		}

		private void ILGenElseString(string elseString)
		{
			MethodInfo method = typeof(XmlSerializationReader).GetMethod("UnknownNode", CodeGenerator.InstanceBindingFlags, null, new Type[] { typeof(object) }, null);
			MethodInfo method2 = typeof(XmlSerializationReader).GetMethod("UnknownNode", CodeGenerator.InstanceBindingFlags, null, new Type[]
			{
				typeof(object),
				typeof(string)
			}, null);
			Match match = XmlSerializationILGen.NewRegex("UnknownNode[(]null, @[\"](?<qnames>[^\"]*)[\"][)];").Match(elseString);
			if (match.Success)
			{
				this.ilg.Ldarg(0);
				this.ilg.Load(null);
				this.ilg.Ldstr(match.Groups["qnames"].Value);
				this.ilg.Call(method2);
				return;
			}
			match = XmlSerializationILGen.NewRegex("UnknownNode[(][(]object[)](?<o>[^,]+), @[\"](?<qnames>[^\"]*)[\"][)];").Match(elseString);
			if (match.Success)
			{
				this.ilg.Ldarg(0);
				LocalBuilder local = this.ilg.GetLocal(match.Groups["o"].Value);
				this.ilg.Ldloc(local);
				this.ilg.ConvertValue(local.LocalType, typeof(object));
				this.ilg.Ldstr(match.Groups["qnames"].Value);
				this.ilg.Call(method2);
				return;
			}
			match = XmlSerializationILGen.NewRegex("UnknownNode[(][(]object[)](?<o>[^,]+), null[)];").Match(elseString);
			if (match.Success)
			{
				this.ilg.Ldarg(0);
				LocalBuilder local2 = this.ilg.GetLocal(match.Groups["o"].Value);
				this.ilg.Ldloc(local2);
				this.ilg.ConvertValue(local2.LocalType, typeof(object));
				this.ilg.Load(null);
				this.ilg.Call(method2);
				return;
			}
			match = XmlSerializationILGen.NewRegex("UnknownNode[(][(]object[)](?<o>[^)]+)[)];").Match(elseString);
			if (match.Success)
			{
				this.ilg.Ldarg(0);
				LocalBuilder local3 = this.ilg.GetLocal(match.Groups["o"].Value);
				this.ilg.Ldloc(local3);
				this.ilg.ConvertValue(local3.LocalType, typeof(object));
				this.ilg.Call(method);
				return;
			}
			throw CodeGenerator.NotSupported("Unexpected: " + elseString);
		}

		private void ILGenParamsReadSource(string paramsReadSource)
		{
			Match match = XmlSerializationILGen.NewRegex("paramsRead\\[(?<index>[0-9]+)\\]").Match(paramsReadSource);
			if (match.Success)
			{
				this.ilg.LoadArrayElement(this.ilg.GetLocal("paramsRead"), int.Parse(match.Groups["index"].Value, CultureInfo.InvariantCulture));
				return;
			}
			throw CodeGenerator.NotSupported("Unexpected: " + paramsReadSource);
		}

		private void ILGenParamsReadSource(string paramsReadSource, bool value)
		{
			Match match = XmlSerializationILGen.NewRegex("paramsRead\\[(?<index>[0-9]+)\\]").Match(paramsReadSource);
			if (match.Success)
			{
				this.ilg.StoreArrayElement(this.ilg.GetLocal("paramsRead"), int.Parse(match.Groups["index"].Value, CultureInfo.InvariantCulture), value);
				return;
			}
			throw CodeGenerator.NotSupported("Unexpected: " + paramsReadSource);
		}

		private void ILGenElementElseString(string elementElseString)
		{
			if (elementElseString == "throw CreateUnknownNodeException();")
			{
				MethodInfo method = typeof(XmlSerializationReader).GetMethod("CreateUnknownNodeException", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
				this.ilg.Ldarg(0);
				this.ilg.Call(method);
				this.ilg.Throw();
				return;
			}
			if (elementElseString.StartsWith("UnknownNode(", StringComparison.Ordinal))
			{
				this.ILGenElseString(elementElseString);
				return;
			}
			throw CodeGenerator.NotSupported("Unexpected: " + elementElseString);
		}

		private void ILGenSet(string source, object value)
		{
			this.WriteSourceBegin(source);
			this.ilg.Load(value);
			this.WriteSourceEnd(source, (value == null) ? typeof(object) : value.GetType());
		}

		private Hashtable idNames = new Hashtable();

		private Dictionary<string, FieldBuilder> idNameFields = new Dictionary<string, FieldBuilder>();

		private Hashtable enums;

		private int nextIdNumber;

		private int nextWhileLoopIndex;

		private class CreateCollectionInfo
		{
			internal CreateCollectionInfo(string name, TypeDesc td)
			{
				this.name = name;
				this.td = td;
			}

			internal string Name
			{
				get
				{
					return this.name;
				}
			}

			internal TypeDesc TypeDesc
			{
				get
				{
					return this.td;
				}
			}

			private string name;

			private TypeDesc td;
		}

		private class Member
		{
			internal Member(XmlSerializationReaderILGen outerClass, string source, string arrayName, int i, MemberMapping mapping)
				: this(outerClass, source, null, arrayName, i, mapping, false, null)
			{
			}

			internal Member(XmlSerializationReaderILGen outerClass, string source, string arrayName, int i, MemberMapping mapping, string choiceSource)
				: this(outerClass, source, null, arrayName, i, mapping, false, choiceSource)
			{
			}

			internal Member(XmlSerializationReaderILGen outerClass, string source, string arraySource, string arrayName, int i, MemberMapping mapping)
				: this(outerClass, source, arraySource, arrayName, i, mapping, false, null)
			{
			}

			internal Member(XmlSerializationReaderILGen outerClass, string source, string arraySource, string arrayName, int i, MemberMapping mapping, string choiceSource)
				: this(outerClass, source, arraySource, arrayName, i, mapping, false, choiceSource)
			{
			}

			internal Member(XmlSerializationReaderILGen outerClass, string source, string arrayName, int i, MemberMapping mapping, bool multiRef)
				: this(outerClass, source, null, arrayName, i, mapping, multiRef, null)
			{
			}

			internal Member(XmlSerializationReaderILGen outerClass, string source, string arraySource, string arrayName, int i, MemberMapping mapping, bool multiRef, string choiceSource)
			{
				this.source = source;
				this.arrayName = arrayName + "_" + i.ToString(CultureInfo.InvariantCulture);
				this.choiceArrayName = "choice_" + this.arrayName;
				this.choiceSource = choiceSource;
				if (mapping.TypeDesc.IsArrayLike)
				{
					if (arraySource != null)
					{
						this.arraySource = arraySource;
					}
					else
					{
						this.arraySource = outerClass.GetArraySource(mapping.TypeDesc, this.arrayName, multiRef);
					}
					this.isArray = mapping.TypeDesc.IsArray;
					this.isList = !this.isArray;
					if (mapping.ChoiceIdentifier != null)
					{
						this.choiceArraySource = outerClass.GetArraySource(mapping.TypeDesc, this.choiceArrayName, multiRef);
						string text = this.choiceArrayName;
						string text2 = "c" + text;
						string csharpName = mapping.ChoiceIdentifier.Mapping.TypeDesc.CSharpName;
						string text3 = "(" + csharpName + "[])";
						string text4 = string.Concat(new string[]
						{
							text,
							" = ",
							text3,
							"EnsureArrayIndex(",
							text,
							", ",
							text2,
							", ",
							outerClass.RaCodeGen.GetStringForTypeof(csharpName),
							");"
						});
						this.choiceArraySource = text4 + outerClass.RaCodeGen.GetStringForArrayMember(text, text2 + "++", mapping.ChoiceIdentifier.Mapping.TypeDesc);
					}
					else
					{
						this.choiceArraySource = this.choiceSource;
					}
				}
				else
				{
					this.arraySource = ((arraySource == null) ? source : arraySource);
					this.choiceArraySource = this.choiceSource;
				}
				this.mapping = mapping;
			}

			internal MemberMapping Mapping
			{
				get
				{
					return this.mapping;
				}
			}

			internal string Source
			{
				get
				{
					return this.source;
				}
			}

			internal string ArrayName
			{
				get
				{
					return this.arrayName;
				}
			}

			internal string ArraySource
			{
				get
				{
					return this.arraySource;
				}
			}

			internal bool IsList
			{
				get
				{
					return this.isList;
				}
			}

			internal bool IsArrayLike
			{
				get
				{
					return this.isArray || this.isList;
				}
			}

			internal bool IsNullable
			{
				get
				{
					return this.isNullable;
				}
				set
				{
					this.isNullable = value;
				}
			}

			internal bool MultiRef
			{
				get
				{
					return this.multiRef;
				}
				set
				{
					this.multiRef = value;
				}
			}

			internal int FixupIndex
			{
				get
				{
					return this.fixupIndex;
				}
				set
				{
					this.fixupIndex = value;
				}
			}

			internal string ParamsReadSource
			{
				get
				{
					return this.paramsReadSource;
				}
				set
				{
					this.paramsReadSource = value;
				}
			}

			internal string CheckSpecifiedSource
			{
				get
				{
					return this.checkSpecifiedSource;
				}
				set
				{
					this.checkSpecifiedSource = value;
				}
			}

			internal string ChoiceSource
			{
				get
				{
					return this.choiceSource;
				}
			}

			internal string ChoiceArrayName
			{
				get
				{
					return this.choiceArrayName;
				}
			}

			internal string ChoiceArraySource
			{
				get
				{
					return this.choiceArraySource;
				}
			}

			private string source;

			private string arrayName;

			private string arraySource;

			private string choiceArrayName;

			private string choiceSource;

			private string choiceArraySource;

			private MemberMapping mapping;

			private bool isArray;

			private bool isList;

			private bool isNullable;

			private bool multiRef;

			private int fixupIndex = -1;

			private string paramsReadSource;

			private string checkSpecifiedSource;
		}
	}
}
