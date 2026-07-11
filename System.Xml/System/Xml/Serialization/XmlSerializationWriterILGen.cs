using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Xml.Schema;

namespace System.Xml.Serialization
{
	internal class XmlSerializationWriterILGen : XmlSerializationILGen
	{
		internal XmlSerializationWriterILGen(TypeScope[] scopes, string access, string className)
			: base(scopes, access, className)
		{
		}

		internal void GenerateBegin()
		{
			this.typeBuilder = CodeGenerator.CreateTypeBuilder(base.ModuleBuilder, base.ClassName, base.TypeAttributes | TypeAttributes.BeforeFieldInit, typeof(XmlSerializationWriter), CodeGenerator.EmptyTypeArray);
			foreach (TypeScope typeScope in base.Scopes)
			{
				foreach (object obj in typeScope.TypeMappings)
				{
					TypeMapping typeMapping = (TypeMapping)obj;
					if (typeMapping is StructMapping || typeMapping is EnumMapping)
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
			}
		}

		internal Type GenerateEnd()
		{
			base.GenerateReferencedMethods();
			this.GenerateInitCallbacksMethod();
			this.typeBuilder.DefineDefaultConstructor(CodeGenerator.PublicMethodAttributes);
			return this.typeBuilder.CreateType();
		}

		internal string GenerateElement(XmlMapping xmlMapping)
		{
			if (!xmlMapping.IsWriteable)
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

		private void GenerateInitCallbacksMethod()
		{
			this.ilg = new CodeGenerator(this.typeBuilder);
			this.ilg.BeginMethod(typeof(void), "InitCallbacks", CodeGenerator.EmptyTypeArray, CodeGenerator.EmptyStringArray, CodeGenerator.ProtectedOverrideMethodAttributes);
			this.ilg.EndMethod();
		}

		private void WriteQualifiedNameElement(string name, string ns, object defaultValue, SourceInfo source, bool nullable, TypeMapping mapping)
		{
			bool flag = defaultValue != null && defaultValue != DBNull.Value;
			if (flag)
			{
				throw CodeGenerator.NotSupported("XmlQualifiedName DefaultValue not supported.  Fail in WriteValue()");
			}
			List<Type> list = new List<Type>();
			this.ilg.Ldarg(0);
			this.ilg.Ldstr(name);
			list.Add(typeof(string));
			if (ns != null)
			{
				this.ilg.Ldstr(ns);
				list.Add(typeof(string));
			}
			source.Load(mapping.TypeDesc.Type);
			list.Add(mapping.TypeDesc.Type);
			MethodInfo method = typeof(XmlSerializationWriter).GetMethod(nullable ? "WriteNullableQualifiedNameLiteral" : "WriteElementQualifiedName", CodeGenerator.InstanceBindingFlags, null, list.ToArray(), null);
			this.ilg.Call(method);
			if (flag)
			{
				throw CodeGenerator.NotSupported("XmlQualifiedName DefaultValue not supported.  Fail in WriteValue()");
			}
		}

		private void WriteEnumValue(EnumMapping mapping, SourceInfo source, out Type returnType)
		{
			string text = base.ReferenceMapping(mapping);
			MethodBuilder methodBuilder = base.EnsureMethodBuilder(this.typeBuilder, text, CodeGenerator.PrivateMethodAttributes, typeof(string), new Type[] { mapping.TypeDesc.Type });
			this.ilg.Ldarg(0);
			source.Load(mapping.TypeDesc.Type);
			this.ilg.Call(methodBuilder);
			returnType = typeof(string);
		}

		private void WritePrimitiveValue(TypeDesc typeDesc, SourceInfo source, out Type returnType)
		{
			if (typeDesc == base.StringTypeDesc || typeDesc.FormatterName == "String")
			{
				source.Load(typeDesc.Type);
				returnType = typeDesc.Type;
				return;
			}
			if (!typeDesc.HasCustomFormatter)
			{
				Type type = typeDesc.Type;
				MethodInfo method = typeof(XmlConvert).GetMethod("ToString", CodeGenerator.StaticBindingFlags, null, new Type[] { type }, null);
				source.Load(typeDesc.Type);
				this.ilg.Call(method);
				returnType = method.ReturnType;
				return;
			}
			BindingFlags bindingFlags = CodeGenerator.StaticBindingFlags;
			if (typeDesc.FormatterName == "XmlQualifiedName")
			{
				bindingFlags = CodeGenerator.InstanceBindingFlags;
				this.ilg.Ldarg(0);
			}
			MethodInfo method2 = typeof(XmlSerializationWriter).GetMethod("From" + typeDesc.FormatterName, bindingFlags, null, new Type[] { typeDesc.Type }, null);
			source.Load(typeDesc.Type);
			this.ilg.Call(method2);
			returnType = method2.ReturnType;
		}

		private void WritePrimitive(string method, string name, string ns, object defaultValue, SourceInfo source, TypeMapping mapping, bool writeXsiType, bool isElement, bool isNullable)
		{
			TypeDesc typeDesc = mapping.TypeDesc;
			bool flag = defaultValue != null && defaultValue != DBNull.Value && mapping.TypeDesc.HasDefaultSupport;
			if (flag)
			{
				if (mapping is EnumMapping)
				{
					source.Load(mapping.TypeDesc.Type);
					string text = null;
					if (((EnumMapping)mapping).IsFlags)
					{
						string[] array = ((string)defaultValue).Split(null);
						for (int i = 0; i < array.Length; i++)
						{
							if (array[i] != null && array[i].Length != 0)
							{
								if (i > 0)
								{
									text += ", ";
								}
								text += array[i];
							}
						}
					}
					else
					{
						text = (string)defaultValue;
					}
					this.ilg.Ldc(Enum.Parse(mapping.TypeDesc.Type, text, false));
					this.ilg.If(Cmp.NotEqualTo);
				}
				else
				{
					this.WriteCheckDefault(source, defaultValue, isNullable);
				}
			}
			List<Type> list = new List<Type>();
			this.ilg.Ldarg(0);
			list.Add(typeof(string));
			this.ilg.Ldstr(name);
			if (ns != null)
			{
				list.Add(typeof(string));
				this.ilg.Ldstr(ns);
			}
			if (mapping is EnumMapping)
			{
				Type type;
				this.WriteEnumValue((EnumMapping)mapping, source, out type);
				list.Add(type);
			}
			else
			{
				Type type;
				this.WritePrimitiveValue(typeDesc, source, out type);
				list.Add(type);
			}
			if (writeXsiType)
			{
				list.Add(typeof(XmlQualifiedName));
				ConstructorInfo constructor = typeof(XmlQualifiedName).GetConstructor(CodeGenerator.InstanceBindingFlags, null, new Type[]
				{
					typeof(string),
					typeof(string)
				}, null);
				this.ilg.Ldstr(mapping.TypeName);
				this.ilg.Ldstr(mapping.Namespace);
				this.ilg.New(constructor);
			}
			MethodInfo method2 = typeof(XmlSerializationWriter).GetMethod(method, CodeGenerator.InstanceBindingFlags, null, list.ToArray(), null);
			this.ilg.Call(method2);
			if (flag)
			{
				this.ilg.EndIf();
			}
		}

		private void WriteTag(string methodName, string name, string ns)
		{
			MethodInfo method = typeof(XmlSerializationWriter).GetMethod(methodName, CodeGenerator.InstanceBindingFlags, null, new Type[]
			{
				typeof(string),
				typeof(string)
			}, null);
			this.ilg.Ldarg(0);
			this.ilg.Ldstr(name);
			this.ilg.Ldstr(ns);
			this.ilg.Call(method);
		}

		private void WriteTag(string methodName, string name, string ns, bool writePrefixed)
		{
			MethodInfo method = typeof(XmlSerializationWriter).GetMethod(methodName, CodeGenerator.InstanceBindingFlags, null, new Type[]
			{
				typeof(string),
				typeof(string),
				typeof(object),
				typeof(bool)
			}, null);
			this.ilg.Ldarg(0);
			this.ilg.Ldstr(name);
			this.ilg.Ldstr(ns);
			this.ilg.Load(null);
			this.ilg.Ldc(writePrefixed);
			this.ilg.Call(method);
		}

		private void WriteStartElement(string name, string ns, bool writePrefixed)
		{
			this.WriteTag("WriteStartElement", name, ns, writePrefixed);
		}

		private void WriteEndElement()
		{
			MethodInfo method = typeof(XmlSerializationWriter).GetMethod("WriteEndElement", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
			this.ilg.Ldarg(0);
			this.ilg.Call(method);
		}

		private void WriteEndElement(string source)
		{
			MethodInfo method = typeof(XmlSerializationWriter).GetMethod("WriteEndElement", CodeGenerator.InstanceBindingFlags, null, new Type[] { typeof(object) }, null);
			object variable = this.ilg.GetVariable(source);
			this.ilg.Ldarg(0);
			this.ilg.Load(variable);
			this.ilg.ConvertValue(this.ilg.GetVariableType(variable), typeof(object));
			this.ilg.Call(method);
		}

		private void WriteLiteralNullTag(string name, string ns)
		{
			this.WriteTag("WriteNullTagLiteral", name, ns);
		}

		private void WriteEmptyTag(string name, string ns)
		{
			this.WriteTag("WriteEmptyTag", name, ns);
		}

		private string GenerateMembersElement(XmlMembersMapping xmlMembersMapping)
		{
			ElementAccessor accessor = xmlMembersMapping.Accessor;
			MembersMapping membersMapping = (MembersMapping)accessor.Mapping;
			bool hasWrapperElement = membersMapping.HasWrapperElement;
			bool writeAccessors = membersMapping.WriteAccessors;
			string text = this.NextMethodName(accessor.Name);
			this.ilg = new CodeGenerator(this.typeBuilder);
			this.ilg.BeginMethod(typeof(void), text, new Type[] { typeof(object[]) }, new string[] { "p" }, CodeGenerator.PublicMethodAttributes);
			MethodInfo method = typeof(XmlSerializationWriter).GetMethod("WriteStartDocument", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
			this.ilg.Ldarg(0);
			this.ilg.Call(method);
			MethodInfo method2 = typeof(XmlSerializationWriter).GetMethod("TopLevelElement", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
			this.ilg.Ldarg(0);
			this.ilg.Call(method2);
			LocalBuilder localBuilder = this.ilg.DeclareLocal(typeof(int), "pLength");
			this.ilg.Ldarg("p");
			this.ilg.Ldlen();
			this.ilg.Stloc(localBuilder);
			if (hasWrapperElement)
			{
				this.WriteStartElement(accessor.Name, (accessor.Form == XmlSchemaForm.Qualified) ? accessor.Namespace : "", false);
				int num = this.FindXmlnsIndex(membersMapping.Members);
				if (num >= 0)
				{
					MemberMapping memberMapping = membersMapping.Members[num];
					string text2 = string.Concat(new string[]
					{
						"((",
						typeof(XmlSerializerNamespaces).FullName,
						")p[",
						num.ToString(CultureInfo.InvariantCulture),
						"])"
					});
					this.ilg.Ldloc(localBuilder);
					this.ilg.Ldc(num);
					this.ilg.If(Cmp.GreaterThan);
					this.WriteNamespaces(text2);
					this.ilg.EndIf();
				}
				for (int i = 0; i < membersMapping.Members.Length; i++)
				{
					MemberMapping memberMapping2 = membersMapping.Members[i];
					if (memberMapping2.Attribute != null && !memberMapping2.Ignore)
					{
						SourceInfo sourceInfo = new SourceInfo("p[" + i.ToString(CultureInfo.InvariantCulture) + "]", null, null, localBuilder.LocalType.GetElementType(), this.ilg);
						SourceInfo sourceInfo2 = null;
						int num2 = 0;
						if (memberMapping2.CheckSpecified != SpecifiedAccessor.None)
						{
							string text3 = memberMapping2.Name + "Specified";
							for (int j = 0; j < membersMapping.Members.Length; j++)
							{
								if (membersMapping.Members[j].Name == text3)
								{
									sourceInfo2 = new SourceInfo("((bool)p[" + j.ToString(CultureInfo.InvariantCulture) + "])", null, null, typeof(bool), this.ilg);
									num2 = j;
									break;
								}
							}
						}
						this.ilg.Ldloc(localBuilder);
						this.ilg.Ldc(i);
						this.ilg.If(Cmp.GreaterThan);
						if (sourceInfo2 != null)
						{
							Label label = this.ilg.DefineLabel();
							Label label2 = this.ilg.DefineLabel();
							this.ilg.Ldloc(localBuilder);
							this.ilg.Ldc(num2);
							this.ilg.Ble(label);
							sourceInfo2.Load(typeof(bool));
							this.ilg.Br_S(label2);
							this.ilg.MarkLabel(label);
							this.ilg.Ldc(true);
							this.ilg.MarkLabel(label2);
							this.ilg.If();
						}
						this.WriteMember(sourceInfo, memberMapping2.Attribute, memberMapping2.TypeDesc, "p");
						if (sourceInfo2 != null)
						{
							this.ilg.EndIf();
						}
						this.ilg.EndIf();
					}
				}
			}
			for (int k = 0; k < membersMapping.Members.Length; k++)
			{
				MemberMapping memberMapping3 = membersMapping.Members[k];
				if (memberMapping3.Xmlns == null && !memberMapping3.Ignore)
				{
					SourceInfo sourceInfo3 = null;
					int num3 = 0;
					if (memberMapping3.CheckSpecified != SpecifiedAccessor.None)
					{
						string text4 = memberMapping3.Name + "Specified";
						for (int l = 0; l < membersMapping.Members.Length; l++)
						{
							if (membersMapping.Members[l].Name == text4)
							{
								sourceInfo3 = new SourceInfo("((bool)p[" + l.ToString(CultureInfo.InvariantCulture) + "])", null, null, typeof(bool), this.ilg);
								num3 = l;
								break;
							}
						}
					}
					this.ilg.Ldloc(localBuilder);
					this.ilg.Ldc(k);
					this.ilg.If(Cmp.GreaterThan);
					if (sourceInfo3 != null)
					{
						Label label3 = this.ilg.DefineLabel();
						Label label4 = this.ilg.DefineLabel();
						this.ilg.Ldloc(localBuilder);
						this.ilg.Ldc(num3);
						this.ilg.Ble(label3);
						sourceInfo3.Load(typeof(bool));
						this.ilg.Br_S(label4);
						this.ilg.MarkLabel(label3);
						this.ilg.Ldc(true);
						this.ilg.MarkLabel(label4);
						this.ilg.If();
					}
					string text5 = "p[" + k.ToString(CultureInfo.InvariantCulture) + "]";
					string text6 = null;
					if (memberMapping3.ChoiceIdentifier != null)
					{
						for (int m = 0; m < membersMapping.Members.Length; m++)
						{
							if (membersMapping.Members[m].Name == memberMapping3.ChoiceIdentifier.MemberName)
							{
								text6 = string.Concat(new string[]
								{
									"((",
									membersMapping.Members[m].TypeDesc.CSharpName,
									")p[",
									m.ToString(CultureInfo.InvariantCulture),
									"])"
								});
								break;
							}
						}
					}
					this.WriteMember(new SourceInfo(text5, text5, null, null, this.ilg), text6, memberMapping3.ElementsSortedByDerivation, memberMapping3.Text, memberMapping3.ChoiceIdentifier, memberMapping3.TypeDesc, writeAccessors || hasWrapperElement);
					if (sourceInfo3 != null)
					{
						this.ilg.EndIf();
					}
					this.ilg.EndIf();
				}
			}
			if (hasWrapperElement)
			{
				this.WriteEndElement();
			}
			this.ilg.EndMethod();
			return text;
		}

		private string GenerateTypeElement(XmlTypeMapping xmlTypeMapping)
		{
			ElementAccessor accessor = xmlTypeMapping.Accessor;
			TypeMapping mapping = accessor.Mapping;
			string text = this.NextMethodName(accessor.Name);
			this.ilg = new CodeGenerator(this.typeBuilder);
			this.ilg.BeginMethod(typeof(void), text, new Type[] { typeof(object) }, new string[] { "o" }, CodeGenerator.PublicMethodAttributes);
			MethodInfo method = typeof(XmlSerializationWriter).GetMethod("WriteStartDocument", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
			this.ilg.Ldarg(0);
			this.ilg.Call(method);
			this.ilg.If(this.ilg.GetArg("o"), Cmp.EqualTo, null);
			if (accessor.IsNullable)
			{
				this.WriteLiteralNullTag(accessor.Name, (accessor.Form == XmlSchemaForm.Qualified) ? accessor.Namespace : "");
			}
			else
			{
				this.WriteEmptyTag(accessor.Name, (accessor.Form == XmlSchemaForm.Qualified) ? accessor.Namespace : "");
			}
			this.ilg.GotoMethodEnd();
			this.ilg.EndIf();
			if (!mapping.TypeDesc.IsValueType && !mapping.TypeDesc.Type.IsPrimitive)
			{
				MethodInfo method2 = typeof(XmlSerializationWriter).GetMethod("TopLevelElement", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
				this.ilg.Ldarg(0);
				this.ilg.Call(method2);
			}
			this.WriteMember(new SourceInfo("o", "o", null, typeof(object), this.ilg), null, new ElementAccessor[] { accessor }, null, null, mapping.TypeDesc, true);
			this.ilg.EndMethod();
			return text;
		}

		private string NextMethodName(string name)
		{
			string text = "Write";
			int num = base.NextMethodNumber + 1;
			base.NextMethodNumber = num;
			return text + num.ToString(null, NumberFormatInfo.InvariantInfo) + "_" + CodeIdentifier.MakeValidInternal(name);
		}

		private void WriteEnumMethod(EnumMapping mapping)
		{
			string text = (string)base.MethodNames[mapping];
			List<Type> list = new List<Type>();
			List<string> list2 = new List<string>();
			list.Add(mapping.TypeDesc.Type);
			list2.Add("v");
			this.ilg = new CodeGenerator(this.typeBuilder);
			this.ilg.BeginMethod(typeof(string), base.GetMethodBuilder(text), list.ToArray(), list2.ToArray(), CodeGenerator.PrivateMethodAttributes);
			LocalBuilder localBuilder = this.ilg.DeclareLocal(typeof(string), "s");
			this.ilg.Load(null);
			this.ilg.Stloc(localBuilder);
			ConstantMapping[] constants = mapping.Constants;
			if (constants.Length != 0)
			{
				Hashtable hashtable = new Hashtable();
				List<Label> list3 = new List<Label>();
				List<string> list4 = new List<string>();
				Label label = this.ilg.DefineLabel();
				Label label2 = this.ilg.DefineLabel();
				LocalBuilder localBuilder2 = this.ilg.DeclareLocal(mapping.TypeDesc.Type, "localTmp");
				this.ilg.Ldarg("v");
				this.ilg.Stloc(localBuilder2);
				foreach (ConstantMapping constantMapping in constants)
				{
					if (hashtable[constantMapping.Value] == null)
					{
						Label label3 = this.ilg.DefineLabel();
						this.ilg.Ldloc(localBuilder2);
						this.ilg.Ldc(Enum.ToObject(mapping.TypeDesc.Type, constantMapping.Value));
						this.ilg.Beq(label3);
						list3.Add(label3);
						list4.Add(constantMapping.XmlName);
						hashtable.Add(constantMapping.Value, constantMapping.Value);
					}
				}
				if (mapping.IsFlags)
				{
					this.ilg.Br(label);
					for (int j = 0; j < list3.Count; j++)
					{
						this.ilg.MarkLabel(list3[j]);
						this.ilg.Ldc(list4[j]);
						this.ilg.Stloc(localBuilder);
						this.ilg.Br(label2);
					}
					this.ilg.MarkLabel(label);
					base.RaCodeGen.ILGenForEnumLongValue(this.ilg, "v");
					LocalBuilder localBuilder3 = this.ilg.DeclareLocal(typeof(string[]), "strArray");
					this.ilg.NewArray(typeof(string), constants.Length);
					this.ilg.Stloc(localBuilder3);
					for (int k = 0; k < constants.Length; k++)
					{
						ConstantMapping constantMapping2 = constants[k];
						this.ilg.Ldloc(localBuilder3);
						this.ilg.Ldc(k);
						this.ilg.Ldstr(constantMapping2.XmlName);
						this.ilg.Stelem(typeof(string));
					}
					this.ilg.Ldloc(localBuilder3);
					LocalBuilder localBuilder4 = this.ilg.DeclareLocal(typeof(long[]), "longArray");
					this.ilg.NewArray(typeof(long), constants.Length);
					this.ilg.Stloc(localBuilder4);
					for (int l = 0; l < constants.Length; l++)
					{
						ConstantMapping constantMapping3 = constants[l];
						this.ilg.Ldloc(localBuilder4);
						this.ilg.Ldc(l);
						this.ilg.Ldc(constantMapping3.Value);
						this.ilg.Stelem(typeof(long));
					}
					this.ilg.Ldloc(localBuilder4);
					this.ilg.Ldstr(mapping.TypeDesc.FullName);
					MethodInfo method = typeof(XmlSerializationWriter).GetMethod("FromEnum", CodeGenerator.StaticBindingFlags, null, new Type[]
					{
						typeof(long),
						typeof(string[]),
						typeof(long[]),
						typeof(string)
					}, null);
					this.ilg.Call(method);
					this.ilg.Stloc(localBuilder);
					this.ilg.Br(label2);
				}
				else
				{
					this.ilg.Br(label);
					for (int m = 0; m < list3.Count; m++)
					{
						this.ilg.MarkLabel(list3[m]);
						this.ilg.Ldc(list4[m]);
						this.ilg.Stloc(localBuilder);
						this.ilg.Br(label2);
					}
					MethodInfo method2 = typeof(CultureInfo).GetMethod("get_InvariantCulture", CodeGenerator.StaticBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
					MethodInfo method3 = typeof(long).GetMethod("ToString", CodeGenerator.InstanceBindingFlags, null, new Type[] { typeof(IFormatProvider) }, null);
					MethodInfo method4 = typeof(XmlSerializationWriter).GetMethod("CreateInvalidEnumValueException", CodeGenerator.InstanceBindingFlags, null, new Type[]
					{
						typeof(object),
						typeof(string)
					}, null);
					this.ilg.MarkLabel(label);
					this.ilg.Ldarg(0);
					this.ilg.Ldarg("v");
					this.ilg.ConvertValue(mapping.TypeDesc.Type, typeof(long));
					LocalBuilder localBuilder5 = this.ilg.DeclareLocal(typeof(long), "num");
					this.ilg.Stloc(localBuilder5);
					this.ilg.LdlocAddress(localBuilder5);
					this.ilg.Call(method2);
					this.ilg.Call(method3);
					this.ilg.Ldstr(mapping.TypeDesc.FullName);
					this.ilg.Call(method4);
					this.ilg.Throw();
				}
				this.ilg.MarkLabel(label2);
			}
			this.ilg.Ldloc(localBuilder);
			this.ilg.EndMethod();
		}

		private void WriteDerivedTypes(StructMapping mapping)
		{
			for (StructMapping structMapping = mapping.DerivedMappings; structMapping != null; structMapping = structMapping.NextDerivedMapping)
			{
				this.ilg.InitElseIf();
				this.WriteTypeCompare("t", structMapping.TypeDesc.Type);
				this.ilg.AndIf();
				string text = base.ReferenceMapping(structMapping);
				List<Type> list = new List<Type>();
				this.ilg.Ldarg(0);
				list.Add(typeof(string));
				this.ilg.Ldarg("n");
				list.Add(typeof(string));
				this.ilg.Ldarg("ns");
				object variable = this.ilg.GetVariable("o");
				Type variableType = this.ilg.GetVariableType(variable);
				this.ilg.Load(variable);
				this.ilg.ConvertValue(variableType, structMapping.TypeDesc.Type);
				list.Add(structMapping.TypeDesc.Type);
				if (structMapping.TypeDesc.IsNullable)
				{
					list.Add(typeof(bool));
					this.ilg.Ldarg("isNullable");
				}
				list.Add(typeof(bool));
				this.ilg.Ldc(true);
				MethodInfo methodInfo = base.EnsureMethodBuilder(this.typeBuilder, text, CodeGenerator.PrivateMethodAttributes, typeof(void), list.ToArray());
				this.ilg.Call(methodInfo);
				this.ilg.GotoMethodEnd();
				this.WriteDerivedTypes(structMapping);
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
						this.WriteTypeCompare("t", enumMapping.TypeDesc.Type);
						this.ilg.AndIf();
						string text = base.ReferenceMapping(enumMapping);
						MethodInfo method = typeof(XmlSerializationWriter).GetMethod("get_Writer", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
						MethodInfo method2 = typeof(XmlWriter).GetMethod("WriteStartElement", CodeGenerator.InstanceBindingFlags, null, new Type[]
						{
							typeof(string),
							typeof(string)
						}, null);
						this.ilg.Ldarg(0);
						this.ilg.Call(method);
						this.ilg.Ldarg("n");
						this.ilg.Ldarg("ns");
						this.ilg.Call(method2);
						MethodInfo method3 = typeof(XmlSerializationWriter).GetMethod("WriteXsiType", CodeGenerator.InstanceBindingFlags, null, new Type[]
						{
							typeof(string),
							typeof(string)
						}, null);
						this.ilg.Ldarg(0);
						this.ilg.Ldstr(enumMapping.TypeName);
						this.ilg.Ldstr(enumMapping.Namespace);
						this.ilg.Call(method3);
						MethodBuilder methodBuilder = base.EnsureMethodBuilder(this.typeBuilder, text, CodeGenerator.PrivateMethodAttributes, typeof(string), new Type[] { enumMapping.TypeDesc.Type });
						MethodInfo method4 = typeof(XmlWriter).GetMethod("WriteString", CodeGenerator.InstanceBindingFlags, null, new Type[] { typeof(string) }, null);
						this.ilg.Ldarg(0);
						this.ilg.Call(method);
						object variable = this.ilg.GetVariable("o");
						this.ilg.Ldarg(0);
						this.ilg.Load(variable);
						this.ilg.ConvertValue(this.ilg.GetVariableType(variable), enumMapping.TypeDesc.Type);
						this.ilg.Call(methodBuilder);
						this.ilg.Call(method4);
						MethodInfo method5 = typeof(XmlWriter).GetMethod("WriteEndElement", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
						this.ilg.Ldarg(0);
						this.ilg.Call(method);
						this.ilg.Call(method5);
						this.ilg.GotoMethodEnd();
					}
					else if (mapping is ArrayMapping)
					{
						ArrayMapping arrayMapping = mapping as ArrayMapping;
						if (arrayMapping != null)
						{
							this.ilg.InitElseIf();
							if (arrayMapping.TypeDesc.IsArray)
							{
								this.WriteArrayTypeCompare("t", arrayMapping.TypeDesc.Type);
							}
							else
							{
								this.WriteTypeCompare("t", arrayMapping.TypeDesc.Type);
							}
							this.ilg.AndIf();
							this.ilg.EnterScope();
							MethodInfo method6 = typeof(XmlSerializationWriter).GetMethod("get_Writer", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
							MethodInfo method7 = typeof(XmlWriter).GetMethod("WriteStartElement", CodeGenerator.InstanceBindingFlags, null, new Type[]
							{
								typeof(string),
								typeof(string)
							}, null);
							this.ilg.Ldarg(0);
							this.ilg.Call(method6);
							this.ilg.Ldarg("n");
							this.ilg.Ldarg("ns");
							this.ilg.Call(method7);
							MethodInfo method8 = typeof(XmlSerializationWriter).GetMethod("WriteXsiType", CodeGenerator.InstanceBindingFlags, null, new Type[]
							{
								typeof(string),
								typeof(string)
							}, null);
							this.ilg.Ldarg(0);
							this.ilg.Ldstr(arrayMapping.TypeName);
							this.ilg.Ldstr(arrayMapping.Namespace);
							this.ilg.Call(method8);
							this.WriteMember(new SourceInfo("o", "o", null, null, this.ilg), null, arrayMapping.ElementsSortedByDerivation, null, null, arrayMapping.TypeDesc, true);
							MethodInfo method9 = typeof(XmlWriter).GetMethod("WriteEndElement", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
							this.ilg.Ldarg(0);
							this.ilg.Call(method6);
							this.ilg.Call(method9);
							this.ilg.GotoMethodEnd();
							this.ilg.ExitScope();
						}
					}
				}
			}
		}

		private void WriteStructMethod(StructMapping mapping)
		{
			string text = (string)base.MethodNames[mapping];
			this.ilg = new CodeGenerator(this.typeBuilder);
			List<Type> list = new List<Type>(5);
			List<string> list2 = new List<string>(5);
			list.Add(typeof(string));
			list2.Add("n");
			list.Add(typeof(string));
			list2.Add("ns");
			list.Add(mapping.TypeDesc.Type);
			list2.Add("o");
			if (mapping.TypeDesc.IsNullable)
			{
				list.Add(typeof(bool));
				list2.Add("isNullable");
			}
			list.Add(typeof(bool));
			list2.Add("needType");
			this.ilg.BeginMethod(typeof(void), base.GetMethodBuilder(text), list.ToArray(), list2.ToArray(), CodeGenerator.PrivateMethodAttributes);
			if (mapping.TypeDesc.IsNullable)
			{
				this.ilg.If(this.ilg.GetArg("o"), Cmp.EqualTo, null);
				this.ilg.If(this.ilg.GetArg("isNullable"), Cmp.EqualTo, true);
				MethodInfo method = typeof(XmlSerializationWriter).GetMethod("WriteNullTagLiteral", CodeGenerator.InstanceBindingFlags, null, new Type[]
				{
					typeof(string),
					typeof(string)
				}, null);
				this.ilg.Ldarg(0);
				this.ilg.Ldarg("n");
				this.ilg.Ldarg("ns");
				this.ilg.Call(method);
				this.ilg.EndIf();
				this.ilg.GotoMethodEnd();
				this.ilg.EndIf();
			}
			this.ilg.If(this.ilg.GetArg("needType"), Cmp.NotEqualTo, true);
			LocalBuilder localBuilder = this.ilg.DeclareLocal(typeof(Type), "t");
			MethodInfo method2 = typeof(object).GetMethod("GetType", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
			ArgBuilder arg = this.ilg.GetArg("o");
			this.ilg.LdargAddress(arg);
			this.ilg.ConvertAddress(arg.ArgType, typeof(object));
			this.ilg.Call(method2);
			this.ilg.Stloc(localBuilder);
			this.WriteTypeCompare("t", mapping.TypeDesc.Type);
			this.ilg.If();
			this.WriteDerivedTypes(mapping);
			if (mapping.TypeDesc.IsRoot)
			{
				this.WriteEnumAndArrayTypes();
			}
			this.ilg.Else();
			if (mapping.TypeDesc.IsRoot)
			{
				MethodInfo method3 = typeof(XmlSerializationWriter).GetMethod("WriteTypedPrimitive", CodeGenerator.InstanceBindingFlags, null, new Type[]
				{
					typeof(string),
					typeof(string),
					typeof(object),
					typeof(bool)
				}, null);
				this.ilg.Ldarg(0);
				this.ilg.Ldarg("n");
				this.ilg.Ldarg("ns");
				this.ilg.Ldarg("o");
				this.ilg.Ldc(true);
				this.ilg.Call(method3);
				this.ilg.GotoMethodEnd();
			}
			else
			{
				MethodInfo method4 = typeof(XmlSerializationWriter).GetMethod("CreateUnknownTypeException", CodeGenerator.InstanceBindingFlags, null, new Type[] { typeof(object) }, null);
				this.ilg.Ldarg(0);
				this.ilg.Ldarg(arg);
				this.ilg.ConvertValue(arg.ArgType, typeof(object));
				this.ilg.Call(method4);
				this.ilg.Throw();
			}
			this.ilg.EndIf();
			this.ilg.EndIf();
			if (!mapping.TypeDesc.IsAbstract)
			{
				if (mapping.TypeDesc.Type != null && typeof(XmlSchemaObject).IsAssignableFrom(mapping.TypeDesc.Type))
				{
					MethodInfo method5 = typeof(XmlSerializationWriter).GetMethod("set_EscapeName", CodeGenerator.InstanceBindingFlags, null, new Type[] { typeof(bool) }, null);
					this.ilg.Ldarg(0);
					this.ilg.Ldc(false);
					this.ilg.Call(method5);
				}
				string text2 = null;
				MemberMapping[] allMembers = TypeScope.GetAllMembers(mapping, this.memberInfos);
				int num = this.FindXmlnsIndex(allMembers);
				if (num >= 0)
				{
					MemberMapping memberMapping = allMembers[num];
					CodeIdentifier.CheckValidIdentifier(memberMapping.Name);
					text2 = base.RaCodeGen.GetStringForMember("o", memberMapping.Name, mapping.TypeDesc);
				}
				this.ilg.Ldarg(0);
				this.ilg.Ldarg("n");
				this.ilg.Ldarg("ns");
				ArgBuilder arg2 = this.ilg.GetArg("o");
				this.ilg.Ldarg(arg2);
				this.ilg.ConvertValue(arg2.ArgType, typeof(object));
				this.ilg.Ldc(false);
				if (text2 == null)
				{
					this.ilg.Load(null);
				}
				else
				{
					base.ILGenLoad(text2);
				}
				MethodInfo method6 = typeof(XmlSerializationWriter).GetMethod("WriteStartElement", CodeGenerator.InstanceBindingFlags, null, new Type[]
				{
					typeof(string),
					typeof(string),
					typeof(object),
					typeof(bool),
					typeof(XmlSerializerNamespaces)
				}, null);
				this.ilg.Call(method6);
				if (!mapping.TypeDesc.IsRoot)
				{
					this.ilg.If(this.ilg.GetArg("needType"), Cmp.EqualTo, true);
					MethodInfo method7 = typeof(XmlSerializationWriter).GetMethod("WriteXsiType", CodeGenerator.InstanceBindingFlags, null, new Type[]
					{
						typeof(string),
						typeof(string)
					}, null);
					this.ilg.Ldarg(0);
					this.ilg.Ldstr(mapping.TypeName);
					this.ilg.Ldstr(mapping.Namespace);
					this.ilg.Call(method7);
					this.ilg.EndIf();
				}
				foreach (MemberMapping memberMapping2 in allMembers)
				{
					if (memberMapping2.Attribute != null)
					{
						CodeIdentifier.CheckValidIdentifier(memberMapping2.Name);
						if (memberMapping2.CheckShouldPersist)
						{
							this.ilg.LdargAddress(arg);
							this.ilg.Call(memberMapping2.CheckShouldPersistMethodInfo);
							this.ilg.If();
						}
						if (memberMapping2.CheckSpecified != SpecifiedAccessor.None)
						{
							string stringForMember = base.RaCodeGen.GetStringForMember("o", memberMapping2.Name + "Specified", mapping.TypeDesc);
							base.ILGenLoad(stringForMember);
							this.ilg.If();
						}
						this.WriteMember(base.RaCodeGen.GetSourceForMember("o", memberMapping2, mapping.TypeDesc, this.ilg), memberMapping2.Attribute, memberMapping2.TypeDesc, "o");
						if (memberMapping2.CheckSpecified != SpecifiedAccessor.None)
						{
							this.ilg.EndIf();
						}
						if (memberMapping2.CheckShouldPersist)
						{
							this.ilg.EndIf();
						}
					}
				}
				foreach (MemberMapping memberMapping3 in allMembers)
				{
					if (memberMapping3.Xmlns == null)
					{
						CodeIdentifier.CheckValidIdentifier(memberMapping3.Name);
						bool flag = memberMapping3.CheckShouldPersist && (memberMapping3.Elements.Length != 0 || memberMapping3.Text != null);
						if (flag)
						{
							this.ilg.LdargAddress(arg);
							this.ilg.Call(memberMapping3.CheckShouldPersistMethodInfo);
							this.ilg.If();
						}
						if (memberMapping3.CheckSpecified != SpecifiedAccessor.None)
						{
							string stringForMember2 = base.RaCodeGen.GetStringForMember("o", memberMapping3.Name + "Specified", mapping.TypeDesc);
							base.ILGenLoad(stringForMember2);
							this.ilg.If();
						}
						string text3 = null;
						if (memberMapping3.ChoiceIdentifier != null)
						{
							CodeIdentifier.CheckValidIdentifier(memberMapping3.ChoiceIdentifier.MemberName);
							text3 = base.RaCodeGen.GetStringForMember("o", memberMapping3.ChoiceIdentifier.MemberName, mapping.TypeDesc);
						}
						this.WriteMember(base.RaCodeGen.GetSourceForMember("o", memberMapping3, memberMapping3.MemberInfo, mapping.TypeDesc, this.ilg), text3, memberMapping3.ElementsSortedByDerivation, memberMapping3.Text, memberMapping3.ChoiceIdentifier, memberMapping3.TypeDesc, true);
						if (memberMapping3.CheckSpecified != SpecifiedAccessor.None)
						{
							this.ilg.EndIf();
						}
						if (flag)
						{
							this.ilg.EndIf();
						}
					}
				}
				this.WriteEndElement("o");
			}
			this.ilg.EndMethod();
		}

		private bool CanOptimizeWriteListSequence(TypeDesc listElementTypeDesc)
		{
			return listElementTypeDesc != null && listElementTypeDesc != base.QnameTypeDesc;
		}

		private void WriteMember(SourceInfo source, AttributeAccessor attribute, TypeDesc memberTypeDesc, string parent)
		{
			if (memberTypeDesc.IsAbstract)
			{
				return;
			}
			if (memberTypeDesc.IsArrayLike)
			{
				string text = "a" + memberTypeDesc.Name;
				string text2 = "ai" + memberTypeDesc.Name;
				string text3 = "i";
				string csharpName = memberTypeDesc.CSharpName;
				this.WriteArrayLocalDecl(csharpName, text, source, memberTypeDesc);
				if (memberTypeDesc.IsNullable)
				{
					this.ilg.Ldloc(memberTypeDesc.Type, text);
					this.ilg.Load(null);
					this.ilg.If(Cmp.NotEqualTo);
				}
				if (attribute.IsList)
				{
					if (this.CanOptimizeWriteListSequence(memberTypeDesc.ArrayElementTypeDesc))
					{
						string text4 = ((attribute.Form == XmlSchemaForm.Qualified) ? attribute.Namespace : string.Empty);
						MethodInfo method = typeof(XmlSerializationWriter).GetMethod("get_Writer", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
						MethodInfo method2 = typeof(XmlWriter).GetMethod("WriteStartAttribute", CodeGenerator.InstanceBindingFlags, null, new Type[]
						{
							typeof(string),
							typeof(string),
							typeof(string)
						}, null);
						this.ilg.Ldarg(0);
						this.ilg.Call(method);
						this.ilg.Load(null);
						this.ilg.Ldstr(attribute.Name);
						this.ilg.Ldstr(text4);
						this.ilg.Call(method2);
					}
					else
					{
						LocalBuilder localBuilder = this.ilg.DeclareOrGetLocal(typeof(StringBuilder), "sb");
						ConstructorInfo constructor = typeof(StringBuilder).GetConstructor(CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
						this.ilg.New(constructor);
						this.ilg.Stloc(localBuilder);
					}
				}
				TypeDesc arrayElementTypeDesc = memberTypeDesc.ArrayElementTypeDesc;
				if (memberTypeDesc.IsEnumerable)
				{
					throw CodeGenerator.NotSupported("CDF15337, DDB176069: Also fail in whidbey IEnumerable member with XmlAttributeAttribute");
				}
				if (memberTypeDesc.IsArray)
				{
					LocalBuilder localBuilder2 = this.ilg.DeclareOrGetLocal(typeof(int), text3);
					this.ilg.For(localBuilder2, 0, this.ilg.GetLocal(text));
				}
				else
				{
					LocalBuilder localBuilder3 = this.ilg.DeclareOrGetLocal(typeof(int), text3);
					this.ilg.For(localBuilder3, 0, this.ilg.GetLocal(text));
				}
				this.WriteLocalDecl(text2, base.RaCodeGen.GetStringForArrayMember(text, text3, memberTypeDesc), arrayElementTypeDesc.Type);
				if (attribute.IsList)
				{
					Type typeFromHandle = typeof(string);
					string text5;
					Type type;
					if (this.CanOptimizeWriteListSequence(memberTypeDesc.ArrayElementTypeDesc))
					{
						this.ilg.Ldloc(text3);
						this.ilg.Ldc(0);
						this.ilg.If(Cmp.NotEqualTo);
						MethodInfo method3 = typeof(XmlSerializationWriter).GetMethod("get_Writer", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
						MethodInfo method4 = typeof(XmlWriter).GetMethod("WriteString", CodeGenerator.InstanceBindingFlags, null, new Type[] { typeof(string) }, null);
						this.ilg.Ldarg(0);
						this.ilg.Call(method3);
						this.ilg.Ldstr(" ");
						this.ilg.Call(method4);
						this.ilg.EndIf();
						this.ilg.Ldarg(0);
						text5 = "WriteValue";
						type = typeof(XmlSerializationWriter);
					}
					else
					{
						MethodInfo method5 = typeof(StringBuilder).GetMethod("Append", CodeGenerator.InstanceBindingFlags, null, new Type[] { typeof(string) }, null);
						this.ilg.Ldloc(text3);
						this.ilg.Ldc(0);
						this.ilg.If(Cmp.NotEqualTo);
						this.ilg.Ldloc("sb");
						this.ilg.Ldstr(" ");
						this.ilg.Call(method5);
						this.ilg.Pop();
						this.ilg.EndIf();
						this.ilg.Ldloc("sb");
						text5 = "Append";
						type = typeof(StringBuilder);
					}
					if (attribute.Mapping is EnumMapping)
					{
						this.WriteEnumValue((EnumMapping)attribute.Mapping, new SourceInfo(text2, text2, null, arrayElementTypeDesc.Type, this.ilg), out typeFromHandle);
					}
					else
					{
						this.WritePrimitiveValue(arrayElementTypeDesc, new SourceInfo(text2, text2, null, arrayElementTypeDesc.Type, this.ilg), out typeFromHandle);
					}
					MethodInfo method6 = type.GetMethod(text5, CodeGenerator.InstanceBindingFlags, null, new Type[] { typeFromHandle }, null);
					this.ilg.Call(method6);
					if (method6.ReturnType != typeof(void))
					{
						this.ilg.Pop();
					}
				}
				else
				{
					this.WriteAttribute(new SourceInfo(text2, text2, null, null, this.ilg), attribute, parent);
				}
				if (memberTypeDesc.IsEnumerable)
				{
					throw CodeGenerator.NotSupported("CDF15337, DDB176069: Also fail in whidbey IEnumerable member with XmlAttributeAttribute");
				}
				this.ilg.EndFor();
				if (attribute.IsList)
				{
					if (this.CanOptimizeWriteListSequence(memberTypeDesc.ArrayElementTypeDesc))
					{
						MethodInfo method7 = typeof(XmlSerializationWriter).GetMethod("get_Writer", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
						MethodInfo method8 = typeof(XmlWriter).GetMethod("WriteEndAttribute", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
						this.ilg.Ldarg(0);
						this.ilg.Call(method7);
						this.ilg.Call(method8);
					}
					else
					{
						MethodInfo method9 = typeof(StringBuilder).GetMethod("get_Length", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
						this.ilg.Ldloc("sb");
						this.ilg.Call(method9);
						this.ilg.Ldc(0);
						this.ilg.If(Cmp.NotEqualTo);
						List<Type> list = new List<Type>();
						this.ilg.Ldarg(0);
						this.ilg.Ldstr(attribute.Name);
						list.Add(typeof(string));
						string text6 = ((attribute.Form == XmlSchemaForm.Qualified) ? attribute.Namespace : string.Empty);
						if (text6 != null)
						{
							this.ilg.Ldstr(text6);
							list.Add(typeof(string));
						}
						MethodInfo method10 = typeof(object).GetMethod("ToString", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
						this.ilg.Ldloc("sb");
						this.ilg.Call(method10);
						list.Add(typeof(string));
						MethodInfo method11 = typeof(XmlSerializationWriter).GetMethod("WriteAttribute", CodeGenerator.InstanceBindingFlags, null, list.ToArray(), null);
						this.ilg.Call(method11);
						this.ilg.EndIf();
					}
				}
				if (memberTypeDesc.IsNullable)
				{
					this.ilg.EndIf();
					return;
				}
			}
			else
			{
				this.WriteAttribute(source, attribute, parent);
			}
		}

		private void WriteAttribute(SourceInfo source, AttributeAccessor attribute, string parent)
		{
			if (!(attribute.Mapping is SpecialMapping))
			{
				TypeDesc typeDesc = attribute.Mapping.TypeDesc;
				source = source.CastTo(typeDesc);
				this.WritePrimitive("WriteAttribute", attribute.Name, (attribute.Form == XmlSchemaForm.Qualified) ? attribute.Namespace : "", XmlSerializationWriterILGen.GetConvertedDefaultValue(source.Type, attribute.Default), source, attribute.Mapping, false, false, false);
				return;
			}
			SpecialMapping specialMapping = (SpecialMapping)attribute.Mapping;
			if (specialMapping.TypeDesc.Kind == TypeKind.Attribute || specialMapping.TypeDesc.CanBeAttributeValue)
			{
				MethodInfo method = typeof(XmlSerializationWriter).GetMethod("WriteXmlAttribute", CodeGenerator.InstanceBindingFlags, null, new Type[]
				{
					typeof(XmlNode),
					typeof(object)
				}, null);
				this.ilg.Ldarg(0);
				this.ilg.Ldloc(source.Source);
				this.ilg.Ldarg(parent);
				this.ilg.ConvertValue(this.ilg.GetArg(parent).ArgType, typeof(object));
				this.ilg.Call(method);
				return;
			}
			throw new InvalidOperationException(Res.GetString("Internal error."));
		}

		private static object GetConvertedDefaultValue(Type targetType, object rawDefaultValue)
		{
			if (targetType == null)
			{
				return rawDefaultValue;
			}
			object obj;
			if (!targetType.TryConvertTo(rawDefaultValue, out obj))
			{
				return rawDefaultValue;
			}
			return obj;
		}

		private void WriteMember(SourceInfo source, string choiceSource, ElementAccessor[] elements, TextAccessor text, ChoiceIdentifierAccessor choice, TypeDesc memberTypeDesc, bool writeAccessors)
		{
			if (memberTypeDesc.IsArrayLike && (elements.Length != 1 || !(elements[0].Mapping is ArrayMapping)))
			{
				this.WriteArray(source, choiceSource, elements, text, choice, memberTypeDesc);
				return;
			}
			this.WriteElements(source, choiceSource, elements, text, choice, "a" + memberTypeDesc.Name, writeAccessors, memberTypeDesc.IsNullable);
		}

		private void WriteArray(SourceInfo source, string choiceSource, ElementAccessor[] elements, TextAccessor text, ChoiceIdentifierAccessor choice, TypeDesc arrayTypeDesc)
		{
			if (elements.Length == 0 && text == null)
			{
				return;
			}
			string csharpName = arrayTypeDesc.CSharpName;
			string text2 = "a" + arrayTypeDesc.Name;
			this.WriteArrayLocalDecl(csharpName, text2, source, arrayTypeDesc);
			LocalBuilder local = this.ilg.GetLocal(text2);
			if (arrayTypeDesc.IsNullable)
			{
				this.ilg.Ldloc(local);
				this.ilg.Load(null);
				this.ilg.If(Cmp.NotEqualTo);
			}
			string text3 = null;
			if (choice != null)
			{
				string csharpName2 = choice.Mapping.TypeDesc.CSharpName;
				SourceInfo sourceInfo = new SourceInfo(choiceSource, null, choice.MemberInfo, null, this.ilg);
				text3 = "c" + choice.Mapping.TypeDesc.Name;
				this.WriteArrayLocalDecl(csharpName2 + "[]", text3, sourceInfo, choice.Mapping.TypeDesc);
				Label label = this.ilg.DefineLabel();
				Label label2 = this.ilg.DefineLabel();
				LocalBuilder local2 = this.ilg.GetLocal(text3);
				this.ilg.Ldloc(local2);
				this.ilg.Load(null);
				this.ilg.Beq(label2);
				this.ilg.Ldloc(local2);
				this.ilg.Ldlen();
				this.ilg.Ldloc(local);
				this.ilg.Ldlen();
				this.ilg.Clt();
				this.ilg.Br(label);
				this.ilg.MarkLabel(label2);
				this.ilg.Ldc(true);
				this.ilg.MarkLabel(label);
				this.ilg.If();
				MethodInfo method = typeof(XmlSerializationWriter).GetMethod("CreateInvalidChoiceIdentifierValueException", CodeGenerator.InstanceBindingFlags, null, new Type[]
				{
					typeof(string),
					typeof(string)
				}, null);
				this.ilg.Ldarg(0);
				this.ilg.Ldstr(choice.Mapping.TypeDesc.FullName);
				this.ilg.Ldstr(choice.MemberName);
				this.ilg.Call(method);
				this.ilg.Throw();
				this.ilg.EndIf();
			}
			this.WriteArrayItems(elements, text, choice, arrayTypeDesc, text2, text3);
			if (arrayTypeDesc.IsNullable)
			{
				this.ilg.EndIf();
			}
		}

		private void WriteArrayItems(ElementAccessor[] elements, TextAccessor text, ChoiceIdentifierAccessor choice, TypeDesc arrayTypeDesc, string arrayName, string choiceName)
		{
			TypeDesc arrayElementTypeDesc = arrayTypeDesc.ArrayElementTypeDesc;
			if (arrayTypeDesc.IsEnumerable)
			{
				LocalBuilder localBuilder = this.ilg.DeclareLocal(typeof(IEnumerator), "e");
				MethodInfo methodInfo = arrayTypeDesc.Type.GetMethod("GetEnumerator", CodeGenerator.InstancePublicBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
				if (methodInfo != null && typeof(IEnumerator).IsAssignableFrom(methodInfo.ReturnType))
				{
					this.ilg.LoadAddress(this.ilg.GetVariable(arrayName));
				}
				else
				{
					this.ilg.Load(this.ilg.GetVariable(arrayName));
					Type type = (arrayTypeDesc.IsGenericInterface ? typeof(IEnumerable<>).MakeGenericType(new Type[] { arrayElementTypeDesc.Type }) : typeof(IEnumerable));
					methodInfo = type.GetMethod("GetEnumerator", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
					this.ilg.ConvertValue(arrayTypeDesc.Type, type);
				}
				this.ilg.Call(methodInfo);
				this.ilg.ConvertValue(methodInfo.ReturnType, typeof(IEnumerator));
				this.ilg.Stloc(localBuilder);
				this.ilg.Ldloc(localBuilder);
				this.ilg.Load(null);
				this.ilg.If(Cmp.NotEqualTo);
				this.ilg.WhileBegin();
				string text2 = arrayName.Replace(arrayTypeDesc.Name, "") + "a" + arrayElementTypeDesc.Name;
				string text3 = arrayName.Replace(arrayTypeDesc.Name, "") + "i" + arrayElementTypeDesc.Name;
				this.WriteLocalDecl(text3, "e.Current", arrayElementTypeDesc.Type);
				this.WriteElements(new SourceInfo(text3, null, null, arrayElementTypeDesc.Type, this.ilg), choiceName + "i", elements, text, choice, text2, true, true);
				this.ilg.WhileBeginCondition();
				MethodInfo method = typeof(IEnumerator).GetMethod("MoveNext", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
				this.ilg.Ldloc(localBuilder);
				this.ilg.Call(method);
				this.ilg.WhileEndCondition();
				this.ilg.WhileEnd();
				this.ilg.EndIf();
				return;
			}
			string text4 = "i" + arrayName.Replace(arrayTypeDesc.Name, "");
			string text5 = arrayName.Replace(arrayTypeDesc.Name, "") + "a" + arrayElementTypeDesc.Name;
			string text6 = arrayName.Replace(arrayTypeDesc.Name, "") + "i" + arrayElementTypeDesc.Name;
			LocalBuilder localBuilder2 = this.ilg.DeclareOrGetLocal(typeof(int), text4);
			this.ilg.For(localBuilder2, 0, this.ilg.GetLocal(arrayName));
			if (elements.Length + ((text == null) ? 0 : 1) > 1)
			{
				this.WriteLocalDecl(text6, base.RaCodeGen.GetStringForArrayMember(arrayName, text4, arrayTypeDesc), arrayElementTypeDesc.Type);
				if (choice != null)
				{
					this.WriteLocalDecl(choiceName + "i", base.RaCodeGen.GetStringForArrayMember(choiceName, text4, choice.Mapping.TypeDesc), choice.Mapping.TypeDesc.Type);
				}
				this.WriteElements(new SourceInfo(text6, null, null, arrayElementTypeDesc.Type, this.ilg), choiceName + "i", elements, text, choice, text5, true, arrayElementTypeDesc.IsNullable);
			}
			else
			{
				this.WriteElements(new SourceInfo(base.RaCodeGen.GetStringForArrayMember(arrayName, text4, arrayTypeDesc), null, null, arrayElementTypeDesc.Type, this.ilg), null, elements, text, choice, text5, true, arrayElementTypeDesc.IsNullable);
			}
			this.ilg.EndFor();
		}

		private void WriteElements(SourceInfo source, string enumSource, ElementAccessor[] elements, TextAccessor text, ChoiceIdentifierAccessor choice, string arrayName, bool writeAccessors, bool isNullable)
		{
			if (elements.Length == 0 && text == null)
			{
				return;
			}
			if (elements.Length == 1 && text == null)
			{
				TypeDesc typeDesc = (elements[0].IsUnbounded ? elements[0].Mapping.TypeDesc.CreateArrayTypeDesc() : elements[0].Mapping.TypeDesc);
				if (!elements[0].Any && !elements[0].Mapping.TypeDesc.IsOptionalValue)
				{
					source = source.CastTo(typeDesc);
				}
				this.WriteElement(source, elements[0], arrayName, writeAccessors);
				return;
			}
			bool flag = false;
			if (isNullable && choice == null)
			{
				source.Load(typeof(object));
				this.ilg.Load(null);
				this.ilg.If(Cmp.NotEqualTo);
				flag = true;
			}
			int num = 0;
			ArrayList arrayList = new ArrayList();
			ElementAccessor elementAccessor = null;
			bool flag2 = false;
			string text2 = ((choice == null) ? null : choice.Mapping.TypeDesc.FullName);
			foreach (ElementAccessor elementAccessor2 in elements)
			{
				if (elementAccessor2.Any)
				{
					num++;
					if (elementAccessor2.Name != null && elementAccessor2.Name.Length > 0)
					{
						arrayList.Add(elementAccessor2);
					}
					else if (elementAccessor == null)
					{
						elementAccessor = elementAccessor2;
					}
				}
				else if (choice != null)
				{
					string csharpName = elementAccessor2.Mapping.TypeDesc.CSharpName;
					object obj;
					string text3 = text2 + ".@" + this.FindChoiceEnumValue(elementAccessor2, (EnumMapping)choice.Mapping, out obj);
					if (flag2)
					{
						this.ilg.InitElseIf();
					}
					else
					{
						flag2 = true;
						this.ilg.InitIf();
					}
					base.ILGenLoad(enumSource, (choice == null) ? null : choice.Mapping.TypeDesc.Type);
					this.ilg.Load(obj);
					this.ilg.Ceq();
					if (isNullable && !elementAccessor2.IsNullable)
					{
						Label label = this.ilg.DefineLabel();
						Label label2 = this.ilg.DefineLabel();
						this.ilg.Brfalse(label);
						source.Load(typeof(object));
						this.ilg.Load(null);
						this.ilg.Cne();
						this.ilg.Br_S(label2);
						this.ilg.MarkLabel(label);
						this.ilg.Ldc(false);
						this.ilg.MarkLabel(label2);
					}
					this.ilg.AndIf();
					this.WriteChoiceTypeCheck(source, csharpName, choice, text3, elementAccessor2.Mapping.TypeDesc);
					SourceInfo sourceInfo = source.CastTo(elementAccessor2.Mapping.TypeDesc);
					this.WriteElement(elementAccessor2.Any ? source : sourceInfo, elementAccessor2, arrayName, writeAccessors);
				}
				else
				{
					TypeDesc typeDesc2 = (elementAccessor2.IsUnbounded ? elementAccessor2.Mapping.TypeDesc.CreateArrayTypeDesc() : elementAccessor2.Mapping.TypeDesc);
					string csharpName2 = typeDesc2.CSharpName;
					if (flag2)
					{
						this.ilg.InitElseIf();
					}
					else
					{
						flag2 = true;
						this.ilg.InitIf();
					}
					this.WriteInstanceOf(source, typeDesc2.Type);
					this.ilg.AndIf();
					SourceInfo sourceInfo2 = source.CastTo(typeDesc2);
					this.WriteElement(elementAccessor2.Any ? source : sourceInfo2, elementAccessor2, arrayName, writeAccessors);
				}
			}
			if (flag2 && num > 0 && elements.Length - num <= 0)
			{
				this.ilg.EndIf();
			}
			if (num > 0)
			{
				if (elements.Length - num > 0)
				{
					this.ilg.InitElseIf();
				}
				else
				{
					this.ilg.InitIf();
				}
				string fullName = typeof(XmlElement).FullName;
				source.Load(typeof(object));
				this.ilg.IsInst(typeof(XmlElement));
				this.ilg.Load(null);
				this.ilg.Cne();
				this.ilg.AndIf();
				LocalBuilder localBuilder = this.ilg.DeclareLocal(typeof(XmlElement), "elem");
				source.Load(typeof(XmlElement));
				this.ilg.Stloc(localBuilder);
				int num2 = 0;
				foreach (object obj2 in arrayList)
				{
					ElementAccessor elementAccessor3 = (ElementAccessor)obj2;
					if (num2++ > 0)
					{
						this.ilg.InitElseIf();
					}
					else
					{
						this.ilg.InitIf();
					}
					string text4 = null;
					Label label3;
					Label label4;
					if (choice != null)
					{
						object obj3;
						text4 = text2 + ".@" + this.FindChoiceEnumValue(elementAccessor3, (EnumMapping)choice.Mapping, out obj3);
						label3 = this.ilg.DefineLabel();
						label4 = this.ilg.DefineLabel();
						base.ILGenLoad(enumSource, (choice == null) ? null : choice.Mapping.TypeDesc.Type);
						this.ilg.Load(obj3);
						this.ilg.Bne(label3);
						if (isNullable && !elementAccessor3.IsNullable)
						{
							source.Load(typeof(object));
							this.ilg.Load(null);
							this.ilg.Cne();
						}
						else
						{
							this.ilg.Ldc(true);
						}
						this.ilg.Br(label4);
						this.ilg.MarkLabel(label3);
						this.ilg.Ldc(false);
						this.ilg.MarkLabel(label4);
						this.ilg.AndIf();
					}
					label3 = this.ilg.DefineLabel();
					label4 = this.ilg.DefineLabel();
					MethodInfo method = typeof(XmlNode).GetMethod("get_Name", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
					MethodInfo method2 = typeof(XmlNode).GetMethod("get_NamespaceURI", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
					this.ilg.Ldloc(localBuilder);
					this.ilg.Call(method);
					this.ilg.Ldstr(elementAccessor3.Name);
					MethodInfo method3 = typeof(string).GetMethod("op_Equality", CodeGenerator.StaticBindingFlags, null, new Type[]
					{
						typeof(string),
						typeof(string)
					}, null);
					this.ilg.Call(method3);
					this.ilg.Brfalse(label3);
					this.ilg.Ldloc(localBuilder);
					this.ilg.Call(method2);
					this.ilg.Ldstr(elementAccessor3.Namespace);
					this.ilg.Call(method3);
					this.ilg.Br(label4);
					this.ilg.MarkLabel(label3);
					this.ilg.Ldc(false);
					this.ilg.MarkLabel(label4);
					if (choice != null)
					{
						this.ilg.If();
					}
					else
					{
						this.ilg.AndIf();
					}
					this.WriteElement(new SourceInfo("elem", null, null, localBuilder.LocalType, this.ilg), elementAccessor3, arrayName, writeAccessors);
					if (choice != null)
					{
						this.ilg.Else();
						MethodInfo method4 = typeof(XmlSerializationWriter).GetMethod("CreateChoiceIdentifierValueException", CodeGenerator.InstanceBindingFlags, null, new Type[]
						{
							typeof(string),
							typeof(string),
							typeof(string),
							typeof(string)
						}, null);
						this.ilg.Ldarg(0);
						this.ilg.Ldstr(text4);
						this.ilg.Ldstr(choice.MemberName);
						this.ilg.Ldloc(localBuilder);
						this.ilg.Call(method);
						this.ilg.Ldloc(localBuilder);
						this.ilg.Call(method2);
						this.ilg.Call(method4);
						this.ilg.Throw();
						this.ilg.EndIf();
					}
				}
				if (num2 > 0)
				{
					this.ilg.Else();
				}
				if (elementAccessor != null)
				{
					this.WriteElement(new SourceInfo("elem", null, null, localBuilder.LocalType, this.ilg), elementAccessor, arrayName, writeAccessors);
				}
				else
				{
					MethodInfo method5 = typeof(XmlSerializationWriter).GetMethod("CreateUnknownAnyElementException", CodeGenerator.InstanceBindingFlags, null, new Type[]
					{
						typeof(string),
						typeof(string)
					}, null);
					this.ilg.Ldarg(0);
					this.ilg.Ldloc(localBuilder);
					MethodInfo method6 = typeof(XmlNode).GetMethod("get_Name", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
					MethodInfo method7 = typeof(XmlNode).GetMethod("get_NamespaceURI", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
					this.ilg.Call(method6);
					this.ilg.Ldloc(localBuilder);
					this.ilg.Call(method7);
					this.ilg.Call(method5);
					this.ilg.Throw();
				}
				if (num2 > 0)
				{
					this.ilg.EndIf();
				}
			}
			if (text != null)
			{
				string csharpName3 = text.Mapping.TypeDesc.CSharpName;
				if (elements.Length != 0)
				{
					this.ilg.InitElseIf();
					this.WriteInstanceOf(source, text.Mapping.TypeDesc.Type);
					this.ilg.AndIf();
					SourceInfo sourceInfo3 = source.CastTo(text.Mapping.TypeDesc);
					this.WriteText(sourceInfo3, text);
				}
				else
				{
					SourceInfo sourceInfo4 = source.CastTo(text.Mapping.TypeDesc);
					this.WriteText(sourceInfo4, text);
				}
			}
			if (elements.Length != 0)
			{
				if (isNullable)
				{
					this.ilg.InitElseIf();
					source.Load(null);
					this.ilg.Load(null);
					this.ilg.AndIf(Cmp.NotEqualTo);
				}
				else
				{
					this.ilg.Else();
				}
				MethodInfo method8 = typeof(XmlSerializationWriter).GetMethod("CreateUnknownTypeException", CodeGenerator.InstanceBindingFlags, null, new Type[] { typeof(object) }, null);
				this.ilg.Ldarg(0);
				source.Load(typeof(object));
				this.ilg.Call(method8);
				this.ilg.Throw();
				this.ilg.EndIf();
			}
			if (flag)
			{
				this.ilg.EndIf();
			}
		}

		private void WriteText(SourceInfo source, TextAccessor text)
		{
			if (text.Mapping is PrimitiveMapping)
			{
				PrimitiveMapping primitiveMapping = (PrimitiveMapping)text.Mapping;
				this.ilg.Ldarg(0);
				Type type;
				if (text.Mapping is EnumMapping)
				{
					this.WriteEnumValue((EnumMapping)text.Mapping, source, out type);
				}
				else
				{
					this.WritePrimitiveValue(primitiveMapping.TypeDesc, source, out type);
				}
				MethodInfo method = typeof(XmlSerializationWriter).GetMethod("WriteValue", CodeGenerator.InstanceBindingFlags, null, new Type[] { type }, null);
				this.ilg.Call(method);
				return;
			}
			if (!(text.Mapping is SpecialMapping))
			{
				return;
			}
			TypeKind kind = ((SpecialMapping)text.Mapping).TypeDesc.Kind;
			if (kind == TypeKind.Node)
			{
				MethodInfo method2 = source.Type.GetMethod("WriteTo", CodeGenerator.InstanceBindingFlags, null, new Type[] { typeof(XmlWriter) }, null);
				MethodInfo method3 = typeof(XmlSerializationWriter).GetMethod("get_Writer", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
				source.Load(source.Type);
				this.ilg.Ldarg(0);
				this.ilg.Call(method3);
				this.ilg.Call(method2);
				return;
			}
			throw new InvalidOperationException(Res.GetString("Internal error."));
		}

		private void WriteElement(SourceInfo source, ElementAccessor element, string arrayName, bool writeAccessor)
		{
			string text = (writeAccessor ? element.Name : element.Mapping.TypeName);
			string text2 = ((element.Any && element.Name.Length == 0) ? null : ((element.Form == XmlSchemaForm.Qualified) ? (writeAccessor ? element.Namespace : element.Mapping.Namespace) : ""));
			if (element.Mapping is NullableMapping)
			{
				if (source.Type == element.Mapping.TypeDesc.Type)
				{
					MethodInfo method = element.Mapping.TypeDesc.Type.GetMethod("get_HasValue", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
					source.LoadAddress(element.Mapping.TypeDesc.Type);
					this.ilg.Call(method);
				}
				else
				{
					source.Load(null);
					this.ilg.Load(null);
					this.ilg.Cne();
				}
				this.ilg.If();
				string csharpName = element.Mapping.TypeDesc.BaseTypeDesc.CSharpName;
				SourceInfo sourceInfo = source.CastTo(element.Mapping.TypeDesc.BaseTypeDesc);
				ElementAccessor elementAccessor = element.Clone();
				elementAccessor.Mapping = ((NullableMapping)element.Mapping).BaseMapping;
				this.WriteElement(elementAccessor.Any ? source : sourceInfo, elementAccessor, arrayName, writeAccessor);
				if (element.IsNullable)
				{
					this.ilg.Else();
					this.WriteLiteralNullTag(element.Name, (element.Form == XmlSchemaForm.Qualified) ? element.Namespace : "");
				}
				this.ilg.EndIf();
				return;
			}
			if (element.Mapping is ArrayMapping)
			{
				ArrayMapping arrayMapping = (ArrayMapping)element.Mapping;
				if (element.IsUnbounded)
				{
					throw CodeGenerator.NotSupported("Unreachable: IsUnbounded is never set true!");
				}
				this.ilg.EnterScope();
				string csharpName2 = arrayMapping.TypeDesc.CSharpName;
				this.WriteArrayLocalDecl(csharpName2, arrayName, source, arrayMapping.TypeDesc);
				if (element.IsNullable)
				{
					this.WriteNullCheckBegin(arrayName, element);
				}
				else if (arrayMapping.TypeDesc.IsNullable)
				{
					this.ilg.Ldloc(this.ilg.GetLocal(arrayName));
					this.ilg.Load(null);
					this.ilg.If(Cmp.NotEqualTo);
				}
				this.WriteStartElement(text, text2, false);
				this.WriteArrayItems(arrayMapping.ElementsSortedByDerivation, null, null, arrayMapping.TypeDesc, arrayName, null);
				this.WriteEndElement();
				if (element.IsNullable)
				{
					this.ilg.EndIf();
				}
				else if (arrayMapping.TypeDesc.IsNullable)
				{
					this.ilg.EndIf();
				}
				this.ilg.ExitScope();
				return;
			}
			else
			{
				if (element.Mapping is EnumMapping)
				{
					this.WritePrimitive("WriteElementString", text, text2, element.Default, source, element.Mapping, false, true, element.IsNullable);
					return;
				}
				if (element.Mapping is PrimitiveMapping)
				{
					PrimitiveMapping primitiveMapping = (PrimitiveMapping)element.Mapping;
					if (primitiveMapping.TypeDesc == base.QnameTypeDesc)
					{
						this.WriteQualifiedNameElement(text, text2, XmlSerializationWriterILGen.GetConvertedDefaultValue(source.Type, element.Default), source, element.IsNullable, primitiveMapping);
						return;
					}
					string text3 = (primitiveMapping.TypeDesc.XmlEncodingNotRequired ? "Raw" : "");
					this.WritePrimitive(element.IsNullable ? ("WriteNullableStringLiteral" + text3) : ("WriteElementString" + text3), text, text2, XmlSerializationWriterILGen.GetConvertedDefaultValue(source.Type, element.Default), source, primitiveMapping, false, true, element.IsNullable);
					return;
				}
				else
				{
					if (element.Mapping is StructMapping)
					{
						StructMapping structMapping = (StructMapping)element.Mapping;
						string text4 = base.ReferenceMapping(structMapping);
						List<Type> list = new List<Type>();
						this.ilg.Ldarg(0);
						this.ilg.Ldstr(text);
						list.Add(typeof(string));
						this.ilg.Ldstr(text2);
						list.Add(typeof(string));
						source.Load(structMapping.TypeDesc.Type);
						list.Add(structMapping.TypeDesc.Type);
						if (structMapping.TypeDesc.IsNullable)
						{
							this.ilg.Ldc(element.IsNullable);
							list.Add(typeof(bool));
						}
						this.ilg.Ldc(false);
						list.Add(typeof(bool));
						MethodBuilder methodBuilder = base.EnsureMethodBuilder(this.typeBuilder, text4, CodeGenerator.PrivateMethodAttributes, typeof(void), list.ToArray());
						this.ilg.Call(methodBuilder);
						return;
					}
					if (!(element.Mapping is SpecialMapping))
					{
						throw new InvalidOperationException(Res.GetString("Internal error."));
					}
					string csharpName3 = ((SpecialMapping)element.Mapping).TypeDesc.CSharpName;
					if (element.Mapping is SerializableMapping)
					{
						this.WriteElementCall("WriteSerializable", typeof(IXmlSerializable), source, text, text2, element.IsNullable, !element.Any);
						return;
					}
					Label label = this.ilg.DefineLabel();
					Label label2 = this.ilg.DefineLabel();
					source.Load(null);
					this.ilg.IsInst(typeof(XmlNode));
					this.ilg.Brtrue(label);
					source.Load(null);
					this.ilg.Load(null);
					this.ilg.Ceq();
					this.ilg.Br(label2);
					this.ilg.MarkLabel(label);
					this.ilg.Ldc(true);
					this.ilg.MarkLabel(label2);
					this.ilg.If();
					this.WriteElementCall("WriteElementLiteral", typeof(XmlNode), source, text, text2, element.IsNullable, element.Any);
					this.ilg.Else();
					MethodInfo method2 = typeof(XmlSerializationWriter).GetMethod("CreateInvalidAnyTypeException", CodeGenerator.InstanceBindingFlags, null, new Type[] { typeof(object) }, null);
					this.ilg.Ldarg(0);
					source.Load(null);
					this.ilg.Call(method2);
					this.ilg.Throw();
					this.ilg.EndIf();
					return;
				}
			}
		}

		private void WriteElementCall(string func, Type cast, SourceInfo source, string name, string ns, bool isNullable, bool isAny)
		{
			MethodInfo method = typeof(XmlSerializationWriter).GetMethod(func, CodeGenerator.InstanceBindingFlags, null, new Type[]
			{
				cast,
				typeof(string),
				typeof(string),
				typeof(bool),
				typeof(bool)
			}, null);
			this.ilg.Ldarg(0);
			source.Load(cast);
			this.ilg.Ldstr(name);
			this.ilg.Ldstr(ns);
			this.ilg.Ldc(isNullable);
			this.ilg.Ldc(isAny);
			this.ilg.Call(method);
		}

		private void WriteCheckDefault(SourceInfo source, object value, bool isNullable)
		{
			if (value is string && ((string)value).Length == 0)
			{
				Label label = this.ilg.DefineLabel();
				Label label2 = this.ilg.DefineLabel();
				Label label3 = this.ilg.DefineLabel();
				source.Load(typeof(string));
				if (isNullable)
				{
					this.ilg.Brfalse(label3);
				}
				else
				{
					this.ilg.Brfalse(label2);
				}
				MethodInfo method = typeof(string).GetMethod("get_Length", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
				source.Load(typeof(string));
				this.ilg.Call(method);
				this.ilg.Ldc(0);
				this.ilg.Cne();
				this.ilg.Br(label);
				if (isNullable)
				{
					this.ilg.MarkLabel(label3);
					this.ilg.Ldc(true);
				}
				else
				{
					this.ilg.MarkLabel(label2);
					this.ilg.Ldc(false);
				}
				this.ilg.MarkLabel(label);
				this.ilg.If();
				return;
			}
			if (value == null)
			{
				source.Load(typeof(object));
				this.ilg.Load(null);
				this.ilg.Cne();
			}
			else if (value.GetType().IsPrimitive)
			{
				source.Load(null);
				this.ilg.Ldc(Convert.ChangeType(value, source.Type, CultureInfo.InvariantCulture));
				this.ilg.Cne();
			}
			else
			{
				Type type = value.GetType();
				source.Load(type);
				this.ilg.Ldc(value);
				MethodInfo method2 = type.GetMethod("op_Inequality", CodeGenerator.StaticBindingFlags, null, new Type[] { type, type }, null);
				if (method2 != null)
				{
					this.ilg.Call(method2);
				}
				else
				{
					this.ilg.Cne();
				}
			}
			this.ilg.If();
		}

		private void WriteChoiceTypeCheck(SourceInfo source, string fullTypeName, ChoiceIdentifierAccessor choice, string enumName, TypeDesc typeDesc)
		{
			Label label = this.ilg.DefineLabel();
			Label label2 = this.ilg.DefineLabel();
			source.Load(typeof(object));
			this.ilg.Load(null);
			this.ilg.Beq(label);
			this.WriteInstanceOf(source, typeDesc.Type);
			this.ilg.Ldc(false);
			this.ilg.Ceq();
			this.ilg.Br(label2);
			this.ilg.MarkLabel(label);
			this.ilg.Ldc(false);
			this.ilg.MarkLabel(label2);
			this.ilg.If();
			MethodInfo method = typeof(XmlSerializationWriter).GetMethod("CreateMismatchChoiceException", CodeGenerator.InstanceBindingFlags, null, new Type[]
			{
				typeof(string),
				typeof(string),
				typeof(string)
			}, null);
			this.ilg.Ldarg(0);
			this.ilg.Ldstr(typeDesc.FullName);
			this.ilg.Ldstr(choice.MemberName);
			this.ilg.Ldstr(enumName);
			this.ilg.Call(method);
			this.ilg.Throw();
			this.ilg.EndIf();
		}

		private void WriteNullCheckBegin(string source, ElementAccessor element)
		{
			LocalBuilder local = this.ilg.GetLocal(source);
			this.ilg.Load(local);
			this.ilg.Load(null);
			this.ilg.If(Cmp.EqualTo);
			this.WriteLiteralNullTag(element.Name, (element.Form == XmlSchemaForm.Qualified) ? element.Namespace : "");
			this.ilg.Else();
		}

		private void WriteNamespaces(string source)
		{
			MethodInfo method = typeof(XmlSerializationWriter).GetMethod("WriteNamespaceDeclarations", CodeGenerator.InstanceBindingFlags, null, new Type[] { typeof(XmlSerializerNamespaces) }, null);
			this.ilg.Ldarg(0);
			base.ILGenLoad(source, typeof(XmlSerializerNamespaces));
			this.ilg.Call(method);
		}

		private int FindXmlnsIndex(MemberMapping[] members)
		{
			for (int i = 0; i < members.Length; i++)
			{
				if (members[i].Xmlns != null)
				{
					return i;
				}
			}
			return -1;
		}

		private void WriteLocalDecl(string variableName, string initValue, Type type)
		{
			base.RaCodeGen.WriteLocalDecl(variableName, new SourceInfo(initValue, initValue, null, type, this.ilg));
		}

		private void WriteArrayLocalDecl(string typeName, string variableName, SourceInfo initValue, TypeDesc arrayTypeDesc)
		{
			base.RaCodeGen.WriteArrayLocalDecl(typeName, variableName, initValue, arrayTypeDesc);
		}

		private void WriteTypeCompare(string variable, Type type)
		{
			base.RaCodeGen.WriteTypeCompare(variable, type, this.ilg);
		}

		private void WriteInstanceOf(SourceInfo source, Type type)
		{
			base.RaCodeGen.WriteInstanceOf(source, type, this.ilg);
		}

		private void WriteArrayTypeCompare(string variable, Type arrayType)
		{
			base.RaCodeGen.WriteArrayTypeCompare(variable, arrayType, this.ilg);
		}

		private string FindChoiceEnumValue(ElementAccessor element, EnumMapping choiceMapping, out object eValue)
		{
			string text = null;
			eValue = null;
			for (int i = 0; i < choiceMapping.Constants.Length; i++)
			{
				string xmlName = choiceMapping.Constants[i].XmlName;
				if (element.Any && element.Name.Length == 0)
				{
					if (xmlName == "##any:")
					{
						text = choiceMapping.Constants[i].Name;
						eValue = Enum.ToObject(choiceMapping.TypeDesc.Type, choiceMapping.Constants[i].Value);
						break;
					}
				}
				else
				{
					int num = xmlName.LastIndexOf(':');
					string text2 = ((num < 0) ? choiceMapping.Namespace : xmlName.Substring(0, num));
					string text3 = ((num < 0) ? xmlName : xmlName.Substring(num + 1));
					if (element.Name == text3 && ((element.Form == XmlSchemaForm.Unqualified && string.IsNullOrEmpty(text2)) || element.Namespace == text2))
					{
						text = choiceMapping.Constants[i].Name;
						eValue = Enum.ToObject(choiceMapping.TypeDesc.Type, choiceMapping.Constants[i].Value);
						break;
					}
				}
			}
			if (text != null && text.Length != 0)
			{
				CodeIdentifier.CheckValidIdentifier(text);
				return text;
			}
			if (element.Any && element.Name.Length == 0)
			{
				throw new InvalidOperationException(Res.GetString("Type {0} is missing enumeration value '##any:' corresponding to XmlAnyElementAttribute.", new object[] { choiceMapping.TypeDesc.FullName }));
			}
			throw new InvalidOperationException(Res.GetString("Type {0} is missing enumeration value '{1}' for element '{2}' from namespace '{3}'.", new object[]
			{
				choiceMapping.TypeDesc.FullName,
				element.Namespace + ":" + element.Name,
				element.Name,
				element.Namespace
			}));
		}
	}
}
