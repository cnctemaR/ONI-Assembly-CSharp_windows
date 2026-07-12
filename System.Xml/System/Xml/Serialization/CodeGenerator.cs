using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml.Serialization.Configuration;

namespace System.Xml.Serialization
{
	internal class CodeGenerator
	{
		internal static bool IsValidLanguageIndependentIdentifier(string ident)
		{
			return CodeGenerator.IsValidLanguageIndependentIdentifier(ident);
		}

		internal static void ValidateIdentifiers(CodeObject e)
		{
			CodeGenerator.ValidateIdentifiers(e);
		}

		internal CodeGenerator(TypeBuilder typeBuilder)
		{
			this.typeBuilder = typeBuilder;
		}

		internal static bool IsNullableGenericType(Type type)
		{
			return type.Name == "Nullable`1";
		}

		internal static void AssertHasInterface(Type type, Type iType)
		{
		}

		internal void BeginMethod(Type returnType, string methodName, Type[] argTypes, string[] argNames, MethodAttributes methodAttributes)
		{
			this.methodBuilder = this.typeBuilder.DefineMethod(methodName, methodAttributes, returnType, argTypes);
			this.ilGen = this.methodBuilder.GetILGenerator();
			this.InitILGeneration(argTypes, argNames, (this.methodBuilder.Attributes & MethodAttributes.Static) == MethodAttributes.Static);
		}

		internal void BeginMethod(Type returnType, MethodBuilderInfo methodBuilderInfo, Type[] argTypes, string[] argNames, MethodAttributes methodAttributes)
		{
			this.methodBuilder = methodBuilderInfo.MethodBuilder;
			this.ilGen = this.methodBuilder.GetILGenerator();
			this.InitILGeneration(argTypes, argNames, (this.methodBuilder.Attributes & MethodAttributes.Static) == MethodAttributes.Static);
		}

		private void InitILGeneration(Type[] argTypes, string[] argNames, bool isStatic)
		{
			this.methodEndLabel = this.ilGen.DefineLabel();
			this.retLabel = this.ilGen.DefineLabel();
			this.blockStack = new Stack();
			this.whileStack = new Stack();
			this.currentScope = new LocalScope();
			this.freeLocals = new Dictionary<Tuple<Type, string>, Queue<LocalBuilder>>();
			this.argList = new Dictionary<string, ArgBuilder>();
			if (!isStatic)
			{
				this.argList.Add("this", new ArgBuilder("this", 0, this.typeBuilder.BaseType));
			}
			for (int i = 0; i < argTypes.Length; i++)
			{
				ArgBuilder argBuilder = new ArgBuilder(argNames[i], this.argList.Count, argTypes[i]);
				this.argList.Add(argBuilder.Name, argBuilder);
				this.methodBuilder.DefineParameter(argBuilder.Index, ParameterAttributes.None, argBuilder.Name);
			}
		}

		internal MethodBuilder EndMethod()
		{
			this.MarkLabel(this.methodEndLabel);
			this.Ret();
			MethodBuilder methodBuilder = this.methodBuilder;
			this.methodBuilder = null;
			this.ilGen = null;
			this.freeLocals = null;
			this.blockStack = null;
			this.whileStack = null;
			this.argList = null;
			this.currentScope = null;
			this.retLocal = null;
			return methodBuilder;
		}

		internal MethodBuilder MethodBuilder
		{
			get
			{
				return this.methodBuilder;
			}
		}

		internal static Exception NotSupported(string msg)
		{
			return new NotSupportedException(msg);
		}

		internal ArgBuilder GetArg(string name)
		{
			return this.argList[name];
		}

		internal LocalBuilder GetLocal(string name)
		{
			return this.currentScope[name];
		}

		internal LocalBuilder ReturnLocal
		{
			get
			{
				if (this.retLocal == null)
				{
					this.retLocal = this.DeclareLocal(this.methodBuilder.ReturnType, "_ret");
				}
				return this.retLocal;
			}
		}

		internal Label ReturnLabel
		{
			get
			{
				return this.retLabel;
			}
		}

		internal LocalBuilder GetTempLocal(Type type)
		{
			LocalBuilder localBuilder;
			if (!this.TmpLocals.TryGetValue(type, out localBuilder))
			{
				localBuilder = this.DeclareLocal(type, "_tmp" + this.TmpLocals.Count.ToString());
				this.TmpLocals.Add(type, localBuilder);
			}
			return localBuilder;
		}

		internal Type GetVariableType(object var)
		{
			if (var is ArgBuilder)
			{
				return ((ArgBuilder)var).ArgType;
			}
			if (var is LocalBuilder)
			{
				return ((LocalBuilder)var).LocalType;
			}
			return var.GetType();
		}

		internal object GetVariable(string name)
		{
			object obj;
			if (this.TryGetVariable(name, out obj))
			{
				return obj;
			}
			return null;
		}

		internal bool TryGetVariable(string name, out object variable)
		{
			LocalBuilder localBuilder;
			if (this.currentScope != null && this.currentScope.TryGetValue(name, out localBuilder))
			{
				variable = localBuilder;
				return true;
			}
			ArgBuilder argBuilder;
			if (this.argList != null && this.argList.TryGetValue(name, out argBuilder))
			{
				variable = argBuilder;
				return true;
			}
			int num;
			if (int.TryParse(name, out num))
			{
				variable = num;
				return true;
			}
			variable = null;
			return false;
		}

		internal void EnterScope()
		{
			LocalScope localScope = new LocalScope(this.currentScope);
			this.currentScope = localScope;
		}

		internal void ExitScope()
		{
			this.currentScope.AddToFreeLocals(this.freeLocals);
			this.currentScope = this.currentScope.parent;
		}

		private bool TryDequeueLocal(Type type, string name, out LocalBuilder local)
		{
			Tuple<Type, string> tuple = new Tuple<Type, string>(type, name);
			Queue<LocalBuilder> queue;
			if (this.freeLocals.TryGetValue(tuple, out queue))
			{
				local = queue.Dequeue();
				if (queue.Count == 0)
				{
					this.freeLocals.Remove(tuple);
				}
				return true;
			}
			local = null;
			return false;
		}

		internal LocalBuilder DeclareLocal(Type type, string name)
		{
			LocalBuilder localBuilder;
			if (!this.TryDequeueLocal(type, name, out localBuilder))
			{
				localBuilder = this.ilGen.DeclareLocal(type, false);
				if (DiagnosticsSwitches.KeepTempFiles.Enabled)
				{
					localBuilder.SetLocalSymInfo(name);
				}
			}
			this.currentScope[name] = localBuilder;
			return localBuilder;
		}

		internal LocalBuilder DeclareOrGetLocal(Type type, string name)
		{
			LocalBuilder localBuilder;
			if (!this.currentScope.TryGetValue(name, out localBuilder))
			{
				localBuilder = this.DeclareLocal(type, name);
			}
			return localBuilder;
		}

		internal object For(LocalBuilder local, object start, object end)
		{
			ForState forState = new ForState(local, this.DefineLabel(), this.DefineLabel(), end);
			if (forState.Index != null)
			{
				this.Load(start);
				this.Stloc(forState.Index);
				this.Br(forState.TestLabel);
			}
			this.MarkLabel(forState.BeginLabel);
			this.blockStack.Push(forState);
			return forState;
		}

		internal void EndFor()
		{
			ForState forState = this.blockStack.Pop() as ForState;
			if (forState.Index != null)
			{
				this.Ldloc(forState.Index);
				this.Ldc(1);
				this.Add();
				this.Stloc(forState.Index);
				this.MarkLabel(forState.TestLabel);
				this.Ldloc(forState.Index);
				this.Load(forState.End);
				if (this.GetVariableType(forState.End).IsArray)
				{
					this.Ldlen();
				}
				else
				{
					MethodInfo method = typeof(ICollection).GetMethod("get_Count", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
					this.Call(method);
				}
				this.Blt(forState.BeginLabel);
				return;
			}
			this.Br(forState.BeginLabel);
		}

		internal void If()
		{
			this.InternalIf(false);
		}

		internal void IfNot()
		{
			this.InternalIf(true);
		}

		private OpCode GetBranchCode(Cmp cmp)
		{
			return CodeGenerator.BranchCodes[(int)cmp];
		}

		internal void If(Cmp cmpOp)
		{
			IfState ifState = new IfState();
			ifState.EndIf = this.DefineLabel();
			ifState.ElseBegin = this.DefineLabel();
			this.ilGen.Emit(this.GetBranchCode(cmpOp), ifState.ElseBegin);
			this.blockStack.Push(ifState);
		}

		internal void If(object value1, Cmp cmpOp, object value2)
		{
			this.Load(value1);
			this.Load(value2);
			this.If(cmpOp);
		}

		internal void Else()
		{
			IfState ifState = this.PopIfState();
			this.Br(ifState.EndIf);
			this.MarkLabel(ifState.ElseBegin);
			ifState.ElseBegin = ifState.EndIf;
			this.blockStack.Push(ifState);
		}

		internal void EndIf()
		{
			IfState ifState = this.PopIfState();
			if (!ifState.ElseBegin.Equals(ifState.EndIf))
			{
				this.MarkLabel(ifState.ElseBegin);
			}
			this.MarkLabel(ifState.EndIf);
		}

		internal void BeginExceptionBlock()
		{
			this.leaveLabels.Push(this.DefineLabel());
			this.ilGen.BeginExceptionBlock();
		}

		internal void BeginCatchBlock(Type exception)
		{
			this.ilGen.BeginCatchBlock(exception);
		}

		internal void EndExceptionBlock()
		{
			this.ilGen.EndExceptionBlock();
			this.ilGen.MarkLabel((Label)this.leaveLabels.Pop());
		}

		internal void Leave()
		{
			this.ilGen.Emit(OpCodes.Leave, (Label)this.leaveLabels.Peek());
		}

		internal void Call(MethodInfo methodInfo)
		{
			if (methodInfo.IsVirtual && !methodInfo.DeclaringType.IsValueType)
			{
				this.ilGen.Emit(OpCodes.Callvirt, methodInfo);
				return;
			}
			this.ilGen.Emit(OpCodes.Call, methodInfo);
		}

		internal void Call(ConstructorInfo ctor)
		{
			this.ilGen.Emit(OpCodes.Call, ctor);
		}

		internal void New(ConstructorInfo constructorInfo)
		{
			this.ilGen.Emit(OpCodes.Newobj, constructorInfo);
		}

		internal void InitObj(Type valueType)
		{
			this.ilGen.Emit(OpCodes.Initobj, valueType);
		}

		internal void NewArray(Type elementType, object len)
		{
			this.Load(len);
			this.ilGen.Emit(OpCodes.Newarr, elementType);
		}

		internal void LoadArrayElement(object obj, object arrayIndex)
		{
			Type elementType = this.GetVariableType(obj).GetElementType();
			this.Load(obj);
			this.Load(arrayIndex);
			if (CodeGenerator.IsStruct(elementType))
			{
				this.Ldelema(elementType);
				this.Ldobj(elementType);
				return;
			}
			this.Ldelem(elementType);
		}

		internal void StoreArrayElement(object obj, object arrayIndex, object value)
		{
			Type variableType = this.GetVariableType(obj);
			if (variableType == typeof(Array))
			{
				this.Load(obj);
				this.Call(typeof(Array).GetMethod("SetValue", new Type[]
				{
					typeof(object),
					typeof(int)
				}));
				return;
			}
			Type elementType = variableType.GetElementType();
			this.Load(obj);
			this.Load(arrayIndex);
			if (CodeGenerator.IsStruct(elementType))
			{
				this.Ldelema(elementType);
			}
			this.Load(value);
			this.ConvertValue(this.GetVariableType(value), elementType);
			if (CodeGenerator.IsStruct(elementType))
			{
				this.Stobj(elementType);
				return;
			}
			this.Stelem(elementType);
		}

		private static bool IsStruct(Type objType)
		{
			return objType.IsValueType && !objType.IsPrimitive;
		}

		internal Type LoadMember(object obj, MemberInfo memberInfo)
		{
			if (this.GetVariableType(obj).IsValueType)
			{
				this.LoadAddress(obj);
			}
			else
			{
				this.Load(obj);
			}
			return this.LoadMember(memberInfo);
		}

		private static MethodInfo GetPropertyMethodFromBaseType(PropertyInfo propertyInfo, bool isGetter)
		{
			Type type = propertyInfo.DeclaringType.BaseType;
			string name = propertyInfo.Name;
			MethodInfo methodInfo = null;
			while (type != null)
			{
				PropertyInfo property = type.GetProperty(name);
				if (property != null)
				{
					if (isGetter)
					{
						methodInfo = property.GetGetMethod(true);
					}
					else
					{
						methodInfo = property.GetSetMethod(true);
					}
					if (methodInfo != null)
					{
						break;
					}
				}
				type = type.BaseType;
			}
			return methodInfo;
		}

		internal Type LoadMember(MemberInfo memberInfo)
		{
			Type type;
			if (memberInfo.MemberType == MemberTypes.Field)
			{
				FieldInfo fieldInfo = (FieldInfo)memberInfo;
				type = fieldInfo.FieldType;
				if (fieldInfo.IsStatic)
				{
					this.ilGen.Emit(OpCodes.Ldsfld, fieldInfo);
				}
				else
				{
					this.ilGen.Emit(OpCodes.Ldfld, fieldInfo);
				}
			}
			else
			{
				PropertyInfo propertyInfo = (PropertyInfo)memberInfo;
				type = propertyInfo.PropertyType;
				if (propertyInfo != null)
				{
					MethodInfo methodInfo = propertyInfo.GetGetMethod(true);
					if (methodInfo == null)
					{
						methodInfo = CodeGenerator.GetPropertyMethodFromBaseType(propertyInfo, true);
					}
					this.Call(methodInfo);
				}
			}
			return type;
		}

		internal Type LoadMemberAddress(MemberInfo memberInfo)
		{
			Type type;
			if (memberInfo.MemberType == MemberTypes.Field)
			{
				FieldInfo fieldInfo = (FieldInfo)memberInfo;
				type = fieldInfo.FieldType;
				if (fieldInfo.IsStatic)
				{
					this.ilGen.Emit(OpCodes.Ldsflda, fieldInfo);
				}
				else
				{
					this.ilGen.Emit(OpCodes.Ldflda, fieldInfo);
				}
			}
			else
			{
				PropertyInfo propertyInfo = (PropertyInfo)memberInfo;
				type = propertyInfo.PropertyType;
				if (propertyInfo != null)
				{
					MethodInfo methodInfo = propertyInfo.GetGetMethod(true);
					if (methodInfo == null)
					{
						methodInfo = CodeGenerator.GetPropertyMethodFromBaseType(propertyInfo, true);
					}
					this.Call(methodInfo);
					LocalBuilder tempLocal = this.GetTempLocal(type);
					this.Stloc(tempLocal);
					this.Ldloca(tempLocal);
				}
			}
			return type;
		}

		internal void StoreMember(MemberInfo memberInfo)
		{
			if (memberInfo.MemberType != MemberTypes.Field)
			{
				PropertyInfo propertyInfo = (PropertyInfo)memberInfo;
				if (propertyInfo != null)
				{
					MethodInfo methodInfo = propertyInfo.GetSetMethod(true);
					if (methodInfo == null)
					{
						methodInfo = CodeGenerator.GetPropertyMethodFromBaseType(propertyInfo, false);
					}
					this.Call(methodInfo);
				}
				return;
			}
			FieldInfo fieldInfo = (FieldInfo)memberInfo;
			if (fieldInfo.IsStatic)
			{
				this.ilGen.Emit(OpCodes.Stsfld, fieldInfo);
				return;
			}
			this.ilGen.Emit(OpCodes.Stfld, fieldInfo);
		}

		internal void Load(object obj)
		{
			if (obj == null)
			{
				this.ilGen.Emit(OpCodes.Ldnull);
				return;
			}
			if (obj is ArgBuilder)
			{
				this.Ldarg((ArgBuilder)obj);
				return;
			}
			if (obj is LocalBuilder)
			{
				this.Ldloc((LocalBuilder)obj);
				return;
			}
			this.Ldc(obj);
		}

		internal void LoadAddress(object obj)
		{
			if (obj is ArgBuilder)
			{
				this.LdargAddress((ArgBuilder)obj);
				return;
			}
			if (obj is LocalBuilder)
			{
				this.LdlocAddress((LocalBuilder)obj);
				return;
			}
			this.Load(obj);
		}

		internal void ConvertAddress(Type source, Type target)
		{
			this.InternalConvert(source, target, true);
		}

		internal void ConvertValue(Type source, Type target)
		{
			this.InternalConvert(source, target, false);
		}

		internal void Castclass(Type target)
		{
			this.ilGen.Emit(OpCodes.Castclass, target);
		}

		internal void Box(Type type)
		{
			this.ilGen.Emit(OpCodes.Box, type);
		}

		internal void Unbox(Type type)
		{
			this.ilGen.Emit(OpCodes.Unbox, type);
		}

		private OpCode GetLdindOpCode(TypeCode typeCode)
		{
			return CodeGenerator.LdindOpCodes[(int)typeCode];
		}

		internal void Ldobj(Type type)
		{
			OpCode ldindOpCode = this.GetLdindOpCode(Type.GetTypeCode(type));
			if (!ldindOpCode.Equals(OpCodes.Nop))
			{
				this.ilGen.Emit(ldindOpCode);
				return;
			}
			this.ilGen.Emit(OpCodes.Ldobj, type);
		}

		internal void Stobj(Type type)
		{
			this.ilGen.Emit(OpCodes.Stobj, type);
		}

		internal void Ceq()
		{
			this.ilGen.Emit(OpCodes.Ceq);
		}

		internal void Clt()
		{
			this.ilGen.Emit(OpCodes.Clt);
		}

		internal void Cne()
		{
			this.Ceq();
			this.Ldc(0);
			this.Ceq();
		}

		internal void Ble(Label label)
		{
			this.ilGen.Emit(OpCodes.Ble, label);
		}

		internal void Throw()
		{
			this.ilGen.Emit(OpCodes.Throw);
		}

		internal void Ldtoken(Type t)
		{
			this.ilGen.Emit(OpCodes.Ldtoken, t);
		}

		internal void Ldc(object o)
		{
			Type type = o.GetType();
			if (o is Type)
			{
				this.Ldtoken((Type)o);
				this.Call(typeof(Type).GetMethod("GetTypeFromHandle", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(RuntimeTypeHandle) }, null));
				return;
			}
			if (type.IsEnum)
			{
				this.Ldc(((IConvertible)o).ToType(Enum.GetUnderlyingType(type), null));
				return;
			}
			switch (Type.GetTypeCode(type))
			{
			case TypeCode.Boolean:
				this.Ldc((bool)o);
				return;
			case TypeCode.Char:
				throw new NotSupportedException("Char is not a valid schema primitive and should be treated as int in DataContract");
			case TypeCode.SByte:
			case TypeCode.Byte:
			case TypeCode.Int16:
			case TypeCode.UInt16:
				this.Ldc(((IConvertible)o).ToInt32(CultureInfo.InvariantCulture));
				return;
			case TypeCode.Int32:
				this.Ldc((int)o);
				return;
			case TypeCode.UInt32:
				this.Ldc((int)((uint)o));
				return;
			case TypeCode.Int64:
				this.Ldc((long)o);
				return;
			case TypeCode.UInt64:
				this.Ldc((long)((ulong)o));
				return;
			case TypeCode.Single:
				this.Ldc((float)o);
				return;
			case TypeCode.Double:
				this.Ldc((double)o);
				return;
			case TypeCode.Decimal:
			{
				ConstructorInfo constructor = typeof(decimal).GetConstructor(CodeGenerator.InstanceBindingFlags, null, new Type[]
				{
					typeof(int),
					typeof(int),
					typeof(int),
					typeof(bool),
					typeof(byte)
				}, null);
				int[] bits = decimal.GetBits((decimal)o);
				this.Ldc(bits[0]);
				this.Ldc(bits[1]);
				this.Ldc(bits[2]);
				this.Ldc(((long)bits[3] & (long)((ulong)int.MinValue)) == (long)((ulong)int.MinValue));
				this.Ldc((int)((byte)((bits[3] >> 16) & 255)));
				this.New(constructor);
				return;
			}
			case TypeCode.DateTime:
			{
				ConstructorInfo constructor2 = typeof(DateTime).GetConstructor(CodeGenerator.InstanceBindingFlags, null, new Type[] { typeof(long) }, null);
				this.Ldc(((DateTime)o).Ticks);
				this.New(constructor2);
				return;
			}
			case TypeCode.String:
				this.Ldstr((string)o);
				return;
			}
			if (type == typeof(TimeSpan) && LocalAppContextSwitches.EnableTimeSpanSerialization)
			{
				ConstructorInfo constructor3 = typeof(TimeSpan).GetConstructor(CodeGenerator.InstanceBindingFlags, null, new Type[] { typeof(long) }, null);
				this.Ldc(((TimeSpan)o).Ticks);
				this.New(constructor3);
				return;
			}
			throw new NotSupportedException("UnknownConstantType");
		}

		internal void Ldc(bool boolVar)
		{
			if (boolVar)
			{
				this.ilGen.Emit(OpCodes.Ldc_I4_1);
				return;
			}
			this.ilGen.Emit(OpCodes.Ldc_I4_0);
		}

		internal void Ldc(int intVar)
		{
			switch (intVar)
			{
			case -1:
				this.ilGen.Emit(OpCodes.Ldc_I4_M1);
				return;
			case 0:
				this.ilGen.Emit(OpCodes.Ldc_I4_0);
				return;
			case 1:
				this.ilGen.Emit(OpCodes.Ldc_I4_1);
				return;
			case 2:
				this.ilGen.Emit(OpCodes.Ldc_I4_2);
				return;
			case 3:
				this.ilGen.Emit(OpCodes.Ldc_I4_3);
				return;
			case 4:
				this.ilGen.Emit(OpCodes.Ldc_I4_4);
				return;
			case 5:
				this.ilGen.Emit(OpCodes.Ldc_I4_5);
				return;
			case 6:
				this.ilGen.Emit(OpCodes.Ldc_I4_6);
				return;
			case 7:
				this.ilGen.Emit(OpCodes.Ldc_I4_7);
				return;
			case 8:
				this.ilGen.Emit(OpCodes.Ldc_I4_8);
				return;
			default:
				this.ilGen.Emit(OpCodes.Ldc_I4, intVar);
				return;
			}
		}

		internal void Ldc(long l)
		{
			this.ilGen.Emit(OpCodes.Ldc_I8, l);
		}

		internal void Ldc(float f)
		{
			this.ilGen.Emit(OpCodes.Ldc_R4, f);
		}

		internal void Ldc(double d)
		{
			this.ilGen.Emit(OpCodes.Ldc_R8, d);
		}

		internal void Ldstr(string strVar)
		{
			if (strVar == null)
			{
				this.ilGen.Emit(OpCodes.Ldnull);
				return;
			}
			this.ilGen.Emit(OpCodes.Ldstr, strVar);
		}

		internal void LdlocAddress(LocalBuilder localBuilder)
		{
			if (localBuilder.LocalType.IsValueType)
			{
				this.Ldloca(localBuilder);
				return;
			}
			this.Ldloc(localBuilder);
		}

		internal void Ldloc(LocalBuilder localBuilder)
		{
			this.ilGen.Emit(OpCodes.Ldloc, localBuilder);
		}

		internal void Ldloc(string name)
		{
			LocalBuilder localBuilder = this.currentScope[name];
			this.Ldloc(localBuilder);
		}

		internal void Stloc(Type type, string name)
		{
			LocalBuilder localBuilder = null;
			if (!this.currentScope.TryGetValue(name, out localBuilder))
			{
				localBuilder = this.DeclareLocal(type, name);
			}
			this.Stloc(localBuilder);
		}

		internal void Stloc(LocalBuilder local)
		{
			this.ilGen.Emit(OpCodes.Stloc, local);
		}

		internal void Ldloc(Type type, string name)
		{
			LocalBuilder localBuilder = this.currentScope[name];
			this.Ldloc(localBuilder);
		}

		internal void Ldloca(LocalBuilder localBuilder)
		{
			this.ilGen.Emit(OpCodes.Ldloca, localBuilder);
		}

		internal void LdargAddress(ArgBuilder argBuilder)
		{
			if (argBuilder.ArgType.IsValueType)
			{
				this.Ldarga(argBuilder);
				return;
			}
			this.Ldarg(argBuilder);
		}

		internal void Ldarg(string arg)
		{
			this.Ldarg(this.GetArg(arg));
		}

		internal void Ldarg(ArgBuilder arg)
		{
			this.Ldarg(arg.Index);
		}

		internal void Ldarg(int slot)
		{
			switch (slot)
			{
			case 0:
				this.ilGen.Emit(OpCodes.Ldarg_0);
				return;
			case 1:
				this.ilGen.Emit(OpCodes.Ldarg_1);
				return;
			case 2:
				this.ilGen.Emit(OpCodes.Ldarg_2);
				return;
			case 3:
				this.ilGen.Emit(OpCodes.Ldarg_3);
				return;
			default:
				if (slot <= 255)
				{
					this.ilGen.Emit(OpCodes.Ldarg_S, slot);
					return;
				}
				this.ilGen.Emit(OpCodes.Ldarg, slot);
				return;
			}
		}

		internal void Ldarga(ArgBuilder argBuilder)
		{
			this.Ldarga(argBuilder.Index);
		}

		internal void Ldarga(int slot)
		{
			if (slot <= 255)
			{
				this.ilGen.Emit(OpCodes.Ldarga_S, slot);
				return;
			}
			this.ilGen.Emit(OpCodes.Ldarga, slot);
		}

		internal void Ldlen()
		{
			this.ilGen.Emit(OpCodes.Ldlen);
			this.ilGen.Emit(OpCodes.Conv_I4);
		}

		private OpCode GetLdelemOpCode(TypeCode typeCode)
		{
			return CodeGenerator.LdelemOpCodes[(int)typeCode];
		}

		internal void Ldelem(Type arrayElementType)
		{
			if (arrayElementType.IsEnum)
			{
				this.Ldelem(Enum.GetUnderlyingType(arrayElementType));
				return;
			}
			OpCode ldelemOpCode = this.GetLdelemOpCode(Type.GetTypeCode(arrayElementType));
			if (ldelemOpCode.Equals(OpCodes.Nop))
			{
				throw new InvalidOperationException("ArrayTypeIsNotSupported");
			}
			this.ilGen.Emit(ldelemOpCode);
		}

		internal void Ldelema(Type arrayElementType)
		{
			OpCode ldelema = OpCodes.Ldelema;
			this.ilGen.Emit(ldelema, arrayElementType);
		}

		private OpCode GetStelemOpCode(TypeCode typeCode)
		{
			return CodeGenerator.StelemOpCodes[(int)typeCode];
		}

		internal void Stelem(Type arrayElementType)
		{
			if (arrayElementType.IsEnum)
			{
				this.Stelem(Enum.GetUnderlyingType(arrayElementType));
				return;
			}
			OpCode stelemOpCode = this.GetStelemOpCode(Type.GetTypeCode(arrayElementType));
			if (stelemOpCode.Equals(OpCodes.Nop))
			{
				throw new InvalidOperationException("ArrayTypeIsNotSupported");
			}
			this.ilGen.Emit(stelemOpCode);
		}

		internal Label DefineLabel()
		{
			return this.ilGen.DefineLabel();
		}

		internal void MarkLabel(Label label)
		{
			this.ilGen.MarkLabel(label);
		}

		internal void Nop()
		{
			this.ilGen.Emit(OpCodes.Nop);
		}

		internal void Add()
		{
			this.ilGen.Emit(OpCodes.Add);
		}

		internal void Ret()
		{
			this.ilGen.Emit(OpCodes.Ret);
		}

		internal void Br(Label label)
		{
			this.ilGen.Emit(OpCodes.Br, label);
		}

		internal void Br_S(Label label)
		{
			this.ilGen.Emit(OpCodes.Br_S, label);
		}

		internal void Blt(Label label)
		{
			this.ilGen.Emit(OpCodes.Blt, label);
		}

		internal void Brfalse(Label label)
		{
			this.ilGen.Emit(OpCodes.Brfalse, label);
		}

		internal void Brtrue(Label label)
		{
			this.ilGen.Emit(OpCodes.Brtrue, label);
		}

		internal void Pop()
		{
			this.ilGen.Emit(OpCodes.Pop);
		}

		internal void Dup()
		{
			this.ilGen.Emit(OpCodes.Dup);
		}

		internal void Ldftn(MethodInfo methodInfo)
		{
			this.ilGen.Emit(OpCodes.Ldftn, methodInfo);
		}

		private void InternalIf(bool negate)
		{
			IfState ifState = new IfState();
			ifState.EndIf = this.DefineLabel();
			ifState.ElseBegin = this.DefineLabel();
			if (negate)
			{
				this.Brtrue(ifState.ElseBegin);
			}
			else
			{
				this.Brfalse(ifState.ElseBegin);
			}
			this.blockStack.Push(ifState);
		}

		private OpCode GetConvOpCode(TypeCode typeCode)
		{
			return CodeGenerator.ConvOpCodes[(int)typeCode];
		}

		private void InternalConvert(Type source, Type target, bool isAddress)
		{
			if (target == source)
			{
				return;
			}
			if (target.IsValueType)
			{
				if (source.IsValueType)
				{
					OpCode convOpCode = this.GetConvOpCode(Type.GetTypeCode(target));
					if (convOpCode.Equals(OpCodes.Nop))
					{
						throw new CodeGeneratorConversionException(source, target, isAddress, "NoConversionPossibleTo");
					}
					this.ilGen.Emit(convOpCode);
					return;
				}
				else
				{
					if (!source.IsAssignableFrom(target))
					{
						throw new CodeGeneratorConversionException(source, target, isAddress, "IsNotAssignableFrom");
					}
					this.Unbox(target);
					if (!isAddress)
					{
						this.Ldobj(target);
						return;
					}
				}
			}
			else if (target.IsAssignableFrom(source))
			{
				if (source.IsValueType)
				{
					if (isAddress)
					{
						this.Ldobj(source);
					}
					this.Box(source);
					return;
				}
			}
			else
			{
				if (source.IsAssignableFrom(target))
				{
					this.Castclass(target);
					return;
				}
				if (target.IsInterface || source.IsInterface)
				{
					this.Castclass(target);
					return;
				}
				throw new CodeGeneratorConversionException(source, target, isAddress, "IsNotAssignableFrom");
			}
		}

		private IfState PopIfState()
		{
			return this.blockStack.Pop() as IfState;
		}

		internal static AssemblyBuilder CreateAssemblyBuilder(AppDomain appDomain, string name)
		{
			AssemblyName assemblyName = new AssemblyName();
			assemblyName.Name = name;
			assemblyName.Version = new Version(1, 0, 0, 0);
			if (DiagnosticsSwitches.KeepTempFiles.Enabled)
			{
				return appDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave, CodeGenerator.TempFilesLocation);
			}
			return appDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
		}

		internal static string TempFilesLocation
		{
			get
			{
				if (CodeGenerator.tempFilesLocation == null)
				{
					object section = ConfigurationManager.GetSection(ConfigurationStrings.XmlSerializerSectionPath);
					string text = null;
					if (section != null)
					{
						XmlSerializerSection xmlSerializerSection = section as XmlSerializerSection;
						if (xmlSerializerSection != null)
						{
							text = xmlSerializerSection.TempFilesLocation;
						}
					}
					if (text != null)
					{
						CodeGenerator.tempFilesLocation = text.Trim();
					}
					else
					{
						CodeGenerator.tempFilesLocation = Path.GetTempPath();
					}
				}
				return CodeGenerator.tempFilesLocation;
			}
			set
			{
				CodeGenerator.tempFilesLocation = value;
			}
		}

		internal static ModuleBuilder CreateModuleBuilder(AssemblyBuilder assemblyBuilder, string name)
		{
			if (DiagnosticsSwitches.KeepTempFiles.Enabled)
			{
				return assemblyBuilder.DefineDynamicModule(name, name + ".dll", true);
			}
			return assemblyBuilder.DefineDynamicModule(name);
		}

		internal static TypeBuilder CreateTypeBuilder(ModuleBuilder moduleBuilder, string name, TypeAttributes attributes, Type parent, Type[] interfaces)
		{
			return moduleBuilder.DefineType("Microsoft.Xml.Serialization.GeneratedAssembly." + name, attributes, parent, interfaces);
		}

		internal void InitElseIf()
		{
			this.elseIfState = (IfState)this.blockStack.Pop();
			this.initElseIfStack = this.blockStack.Count;
			this.Br(this.elseIfState.EndIf);
			this.MarkLabel(this.elseIfState.ElseBegin);
		}

		internal void InitIf()
		{
			this.initIfStack = this.blockStack.Count;
		}

		internal void AndIf(Cmp cmpOp)
		{
			if (this.initIfStack == this.blockStack.Count)
			{
				this.initIfStack = -1;
				this.If(cmpOp);
				return;
			}
			if (this.initElseIfStack == this.blockStack.Count)
			{
				this.initElseIfStack = -1;
				this.elseIfState.ElseBegin = this.DefineLabel();
				this.ilGen.Emit(this.GetBranchCode(cmpOp), this.elseIfState.ElseBegin);
				this.blockStack.Push(this.elseIfState);
				return;
			}
			IfState ifState = (IfState)this.blockStack.Peek();
			this.ilGen.Emit(this.GetBranchCode(cmpOp), ifState.ElseBegin);
		}

		internal void AndIf()
		{
			if (this.initIfStack == this.blockStack.Count)
			{
				this.initIfStack = -1;
				this.If();
				return;
			}
			if (this.initElseIfStack == this.blockStack.Count)
			{
				this.initElseIfStack = -1;
				this.elseIfState.ElseBegin = this.DefineLabel();
				this.Brfalse(this.elseIfState.ElseBegin);
				this.blockStack.Push(this.elseIfState);
				return;
			}
			IfState ifState = (IfState)this.blockStack.Peek();
			this.Brfalse(ifState.ElseBegin);
		}

		internal void IsInst(Type type)
		{
			this.ilGen.Emit(OpCodes.Isinst, type);
		}

		internal void Beq(Label label)
		{
			this.ilGen.Emit(OpCodes.Beq, label);
		}

		internal void Bne(Label label)
		{
			this.ilGen.Emit(OpCodes.Bne_Un, label);
		}

		internal void GotoMethodEnd()
		{
			this.Br(this.methodEndLabel);
		}

		internal void WhileBegin()
		{
			CodeGenerator.WhileState whileState = new CodeGenerator.WhileState(this);
			this.Br(whileState.CondLabel);
			this.MarkLabel(whileState.StartLabel);
			this.whileStack.Push(whileState);
		}

		internal void WhileEnd()
		{
			CodeGenerator.WhileState whileState = (CodeGenerator.WhileState)this.whileStack.Pop();
			this.MarkLabel(whileState.EndLabel);
		}

		internal void WhileBreak()
		{
			CodeGenerator.WhileState whileState = (CodeGenerator.WhileState)this.whileStack.Peek();
			this.Br(whileState.EndLabel);
		}

		internal void WhileContinue()
		{
			CodeGenerator.WhileState whileState = (CodeGenerator.WhileState)this.whileStack.Peek();
			this.Br(whileState.CondLabel);
		}

		internal void WhileBeginCondition()
		{
			CodeGenerator.WhileState whileState = (CodeGenerator.WhileState)this.whileStack.Peek();
			this.Nop();
			this.MarkLabel(whileState.CondLabel);
		}

		internal void WhileEndCondition()
		{
			CodeGenerator.WhileState whileState = (CodeGenerator.WhileState)this.whileStack.Peek();
			this.Brtrue(whileState.StartLabel);
		}

		internal static BindingFlags InstancePublicBindingFlags = BindingFlags.Instance | BindingFlags.Public;

		internal static BindingFlags InstanceBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		internal static BindingFlags StaticBindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

		internal static MethodAttributes PublicMethodAttributes = MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.HideBySig;

		internal static MethodAttributes PublicOverrideMethodAttributes = MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Virtual | MethodAttributes.HideBySig;

		internal static MethodAttributes ProtectedOverrideMethodAttributes = MethodAttributes.Family | MethodAttributes.Virtual | MethodAttributes.HideBySig;

		internal static MethodAttributes PrivateMethodAttributes = MethodAttributes.Private | MethodAttributes.HideBySig;

		internal static Type[] EmptyTypeArray = new Type[0];

		internal static string[] EmptyStringArray = new string[0];

		private TypeBuilder typeBuilder;

		private MethodBuilder methodBuilder;

		private ILGenerator ilGen;

		private Dictionary<string, ArgBuilder> argList;

		private LocalScope currentScope;

		private Dictionary<Tuple<Type, string>, Queue<LocalBuilder>> freeLocals;

		private Stack blockStack;

		private Label methodEndLabel;

		internal LocalBuilder retLocal;

		internal Label retLabel;

		private Dictionary<Type, LocalBuilder> TmpLocals = new Dictionary<Type, LocalBuilder>();

		private static OpCode[] BranchCodes = new OpCode[]
		{
			OpCodes.Bge,
			OpCodes.Bne_Un,
			OpCodes.Bgt,
			OpCodes.Ble,
			OpCodes.Beq,
			OpCodes.Blt
		};

		private Stack leaveLabels = new Stack();

		private static OpCode[] LdindOpCodes = new OpCode[]
		{
			OpCodes.Nop,
			OpCodes.Nop,
			OpCodes.Nop,
			OpCodes.Ldind_I1,
			OpCodes.Ldind_I2,
			OpCodes.Ldind_I1,
			OpCodes.Ldind_U1,
			OpCodes.Ldind_I2,
			OpCodes.Ldind_U2,
			OpCodes.Ldind_I4,
			OpCodes.Ldind_U4,
			OpCodes.Ldind_I8,
			OpCodes.Ldind_I8,
			OpCodes.Ldind_R4,
			OpCodes.Ldind_R8,
			OpCodes.Nop,
			OpCodes.Nop,
			OpCodes.Nop,
			OpCodes.Ldind_Ref
		};

		private static OpCode[] LdelemOpCodes = new OpCode[]
		{
			OpCodes.Nop,
			OpCodes.Ldelem_Ref,
			OpCodes.Ldelem_Ref,
			OpCodes.Ldelem_I1,
			OpCodes.Ldelem_I2,
			OpCodes.Ldelem_I1,
			OpCodes.Ldelem_U1,
			OpCodes.Ldelem_I2,
			OpCodes.Ldelem_U2,
			OpCodes.Ldelem_I4,
			OpCodes.Ldelem_U4,
			OpCodes.Ldelem_I8,
			OpCodes.Ldelem_I8,
			OpCodes.Ldelem_R4,
			OpCodes.Ldelem_R8,
			OpCodes.Nop,
			OpCodes.Nop,
			OpCodes.Nop,
			OpCodes.Ldelem_Ref
		};

		private static OpCode[] StelemOpCodes = new OpCode[]
		{
			OpCodes.Nop,
			OpCodes.Stelem_Ref,
			OpCodes.Stelem_Ref,
			OpCodes.Stelem_I1,
			OpCodes.Stelem_I2,
			OpCodes.Stelem_I1,
			OpCodes.Stelem_I1,
			OpCodes.Stelem_I2,
			OpCodes.Stelem_I2,
			OpCodes.Stelem_I4,
			OpCodes.Stelem_I4,
			OpCodes.Stelem_I8,
			OpCodes.Stelem_I8,
			OpCodes.Stelem_R4,
			OpCodes.Stelem_R8,
			OpCodes.Nop,
			OpCodes.Nop,
			OpCodes.Nop,
			OpCodes.Stelem_Ref
		};

		private static OpCode[] ConvOpCodes = new OpCode[]
		{
			OpCodes.Nop,
			OpCodes.Nop,
			OpCodes.Nop,
			OpCodes.Conv_I1,
			OpCodes.Conv_I2,
			OpCodes.Conv_I1,
			OpCodes.Conv_U1,
			OpCodes.Conv_I2,
			OpCodes.Conv_U2,
			OpCodes.Conv_I4,
			OpCodes.Conv_U4,
			OpCodes.Conv_I8,
			OpCodes.Conv_U8,
			OpCodes.Conv_R4,
			OpCodes.Conv_R8,
			OpCodes.Nop,
			OpCodes.Nop,
			OpCodes.Nop,
			OpCodes.Nop
		};

		private static string tempFilesLocation = null;

		private int initElseIfStack = -1;

		private IfState elseIfState;

		private int initIfStack = -1;

		private Stack whileStack;

		internal class WhileState
		{
			public WhileState(CodeGenerator ilg)
			{
				this.StartLabel = ilg.DefineLabel();
				this.CondLabel = ilg.DefineLabel();
				this.EndLabel = ilg.DefineLabel();
			}

			public Label StartLabel;

			public Label CondLabel;

			public Label EndLabel;
		}
	}
}
