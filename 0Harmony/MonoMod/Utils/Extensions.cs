using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

namespace MonoMod.Utils
{
	public static class Extensions
	{
		public static TypeDefinition SafeResolve(this TypeReference r)
		{
			TypeDefinition typeDefinition;
			try
			{
				typeDefinition = r.Resolve();
			}
			catch
			{
				typeDefinition = null;
			}
			return typeDefinition;
		}

		public static FieldDefinition SafeResolve(this FieldReference r)
		{
			FieldDefinition fieldDefinition;
			try
			{
				fieldDefinition = r.Resolve();
			}
			catch
			{
				fieldDefinition = null;
			}
			return fieldDefinition;
		}

		public static MethodDefinition SafeResolve(this MethodReference r)
		{
			MethodDefinition methodDefinition;
			try
			{
				methodDefinition = r.Resolve();
			}
			catch
			{
				methodDefinition = null;
			}
			return methodDefinition;
		}

		public static PropertyDefinition SafeResolve(this PropertyReference r)
		{
			PropertyDefinition propertyDefinition;
			try
			{
				propertyDefinition = r.Resolve();
			}
			catch
			{
				propertyDefinition = null;
			}
			return propertyDefinition;
		}

		public static CustomAttribute GetCustomAttribute(this Mono.Cecil.ICustomAttributeProvider cap, string attribute)
		{
			if (cap == null || !cap.HasCustomAttributes)
			{
				return null;
			}
			foreach (CustomAttribute customAttribute in cap.CustomAttributes)
			{
				if (customAttribute.AttributeType.FullName == attribute)
				{
					return customAttribute;
				}
			}
			return null;
		}

		public static bool HasCustomAttribute(this Mono.Cecil.ICustomAttributeProvider cap, string attribute)
		{
			return cap.GetCustomAttribute(attribute) != null;
		}

		public static int GetInt(this Instruction instr)
		{
			Mono.Cecil.Cil.OpCode opCode = instr.OpCode;
			if (opCode == Mono.Cecil.Cil.OpCodes.Ldc_I4_M1)
			{
				return -1;
			}
			if (opCode == Mono.Cecil.Cil.OpCodes.Ldc_I4_0)
			{
				return 0;
			}
			if (opCode == Mono.Cecil.Cil.OpCodes.Ldc_I4_1)
			{
				return 1;
			}
			if (opCode == Mono.Cecil.Cil.OpCodes.Ldc_I4_2)
			{
				return 2;
			}
			if (opCode == Mono.Cecil.Cil.OpCodes.Ldc_I4_3)
			{
				return 3;
			}
			if (opCode == Mono.Cecil.Cil.OpCodes.Ldc_I4_4)
			{
				return 4;
			}
			if (opCode == Mono.Cecil.Cil.OpCodes.Ldc_I4_5)
			{
				return 5;
			}
			if (opCode == Mono.Cecil.Cil.OpCodes.Ldc_I4_6)
			{
				return 6;
			}
			if (opCode == Mono.Cecil.Cil.OpCodes.Ldc_I4_7)
			{
				return 7;
			}
			if (opCode == Mono.Cecil.Cil.OpCodes.Ldc_I4_8)
			{
				return 8;
			}
			if (opCode == Mono.Cecil.Cil.OpCodes.Ldc_I4_S)
			{
				return (int)((sbyte)instr.Operand);
			}
			return (int)instr.Operand;
		}

		public static int? GetIntOrNull(this Instruction instr)
		{
			Mono.Cecil.Cil.OpCode opCode = instr.OpCode;
			if (opCode == Mono.Cecil.Cil.OpCodes.Ldc_I4_M1)
			{
				return new int?(-1);
			}
			if (opCode == Mono.Cecil.Cil.OpCodes.Ldc_I4_0)
			{
				return new int?(0);
			}
			if (opCode == Mono.Cecil.Cil.OpCodes.Ldc_I4_1)
			{
				return new int?(1);
			}
			if (opCode == Mono.Cecil.Cil.OpCodes.Ldc_I4_2)
			{
				return new int?(2);
			}
			if (opCode == Mono.Cecil.Cil.OpCodes.Ldc_I4_3)
			{
				return new int?(3);
			}
			if (opCode == Mono.Cecil.Cil.OpCodes.Ldc_I4_4)
			{
				return new int?(4);
			}
			if (opCode == Mono.Cecil.Cil.OpCodes.Ldc_I4_5)
			{
				return new int?(5);
			}
			if (opCode == Mono.Cecil.Cil.OpCodes.Ldc_I4_6)
			{
				return new int?(6);
			}
			if (opCode == Mono.Cecil.Cil.OpCodes.Ldc_I4_7)
			{
				return new int?(7);
			}
			if (opCode == Mono.Cecil.Cil.OpCodes.Ldc_I4_8)
			{
				return new int?(8);
			}
			if (opCode == Mono.Cecil.Cil.OpCodes.Ldc_I4_S)
			{
				return new int?((int)((sbyte)instr.Operand));
			}
			if (opCode == Mono.Cecil.Cil.OpCodes.Ldc_I4)
			{
				return new int?((int)instr.Operand);
			}
			return null;
		}

		public static bool IsBaseMethodCall(this Mono.Cecil.Cil.MethodBody body, MethodReference called)
		{
			MethodDefinition method = body.Method;
			if (called == null)
			{
				return false;
			}
			TypeReference typeReference = called.DeclaringType;
			while (typeReference is TypeSpecification)
			{
				typeReference = ((TypeSpecification)typeReference).ElementType;
			}
			string patchFullName = typeReference.GetPatchFullName();
			bool flag = false;
			try
			{
				TypeDefinition typeDefinition = method.DeclaringType;
				do
				{
					TypeReference baseType = typeDefinition.BaseType;
					if ((typeDefinition = ((baseType != null) ? baseType.SafeResolve() : null)) == null)
					{
						goto IL_0067;
					}
				}
				while (!(typeDefinition.GetPatchFullName() == patchFullName));
				flag = true;
				IL_0067:;
			}
			catch
			{
				flag = method.DeclaringType.GetPatchFullName() == patchFullName;
			}
			return flag;
		}

		public static bool IsCallvirt(this MethodReference method)
		{
			return method.HasThis && !method.DeclaringType.IsValueType;
		}

		public static bool IsStruct(this TypeReference type)
		{
			return type.IsValueType && !type.IsPrimitive;
		}

		public static Mono.Cecil.Cil.OpCode ToLongOp(this Mono.Cecil.Cil.OpCode op)
		{
			string name = Enum.GetName(Extensions.t_Code, op.Code);
			if (!name.EndsWith("_S"))
			{
				return op;
			}
			Dictionary<int, Mono.Cecil.Cil.OpCode> toLongOp = Extensions._ToLongOp;
			Mono.Cecil.Cil.OpCode opCode2;
			lock (toLongOp)
			{
				Mono.Cecil.Cil.OpCode opCode;
				if (Extensions._ToLongOp.TryGetValue((int)op.Code, out opCode))
				{
					opCode2 = opCode;
				}
				else
				{
					Dictionary<int, Mono.Cecil.Cil.OpCode> toLongOp2 = Extensions._ToLongOp;
					int code = (int)op.Code;
					FieldInfo field = Extensions.t_OpCodes.GetField(name.Substring(0, name.Length - 2));
					opCode2 = (toLongOp2[code] = ((Mono.Cecil.Cil.OpCode?)((field != null) ? field.GetValue(null) : null)) ?? op);
				}
			}
			return opCode2;
		}

		public static Mono.Cecil.Cil.OpCode ToShortOp(this Mono.Cecil.Cil.OpCode op)
		{
			string name = Enum.GetName(Extensions.t_Code, op.Code);
			if (name.EndsWith("_S"))
			{
				return op;
			}
			Dictionary<int, Mono.Cecil.Cil.OpCode> toShortOp = Extensions._ToShortOp;
			Mono.Cecil.Cil.OpCode opCode2;
			lock (toShortOp)
			{
				Mono.Cecil.Cil.OpCode opCode;
				if (Extensions._ToShortOp.TryGetValue((int)op.Code, out opCode))
				{
					opCode2 = opCode;
				}
				else
				{
					Dictionary<int, Mono.Cecil.Cil.OpCode> toShortOp2 = Extensions._ToShortOp;
					int code = (int)op.Code;
					FieldInfo field = Extensions.t_OpCodes.GetField(name + "_S");
					opCode2 = (toShortOp2[code] = ((Mono.Cecil.Cil.OpCode?)((field != null) ? field.GetValue(null) : null)) ?? op);
				}
			}
			return opCode2;
		}

		public static void RecalculateILOffsets(this MethodDefinition method)
		{
			if (!method.HasBody)
			{
				return;
			}
			int num = 0;
			for (int i = 0; i < method.Body.Instructions.Count; i++)
			{
				Instruction instruction = method.Body.Instructions[i];
				instruction.Offset = num;
				num += instruction.GetSize();
			}
		}

		public static void FixShortLongOps(this MethodDefinition method)
		{
			if (!method.HasBody)
			{
				return;
			}
			for (int i = 0; i < method.Body.Instructions.Count; i++)
			{
				Instruction instruction = method.Body.Instructions[i];
				if (instruction.Operand is Instruction)
				{
					instruction.OpCode = instruction.OpCode.ToLongOp();
				}
			}
			method.RecalculateILOffsets();
			bool flag;
			do
			{
				flag = false;
				for (int j = 0; j < method.Body.Instructions.Count; j++)
				{
					Instruction instruction2 = method.Body.Instructions[j];
					Instruction instruction3 = instruction2.Operand as Instruction;
					if (instruction3 != null)
					{
						int num = instruction3.Offset - (instruction2.Offset + instruction2.GetSize());
						if (num == (int)((sbyte)num))
						{
							Mono.Cecil.Cil.OpCode opCode = instruction2.OpCode;
							instruction2.OpCode = instruction2.OpCode.ToShortOp();
							flag = opCode != instruction2.OpCode;
						}
					}
				}
			}
			while (flag);
		}

		public static bool Is(this MemberInfo minfo, MemberReference mref)
		{
			return mref.Is(minfo);
		}

		public static bool Is(this MemberReference mref, MemberInfo minfo)
		{
			if (mref == null)
			{
				return false;
			}
			TypeReference typeReference = mref.DeclaringType;
			if (((typeReference != null) ? typeReference.FullName : null) == "<Module>")
			{
				typeReference = null;
			}
			GenericParameter genericParameter = mref as GenericParameter;
			if (genericParameter != null)
			{
				Type type = minfo as Type;
				if (type == null)
				{
					return false;
				}
				if (!type.IsGenericParameter)
				{
					IGenericInstance genericInstance = genericParameter.Owner as IGenericInstance;
					return genericInstance != null && genericInstance.GenericArguments[genericParameter.Position].Is(type);
				}
				return genericParameter.Position == type.GenericParameterPosition;
			}
			else
			{
				if (minfo.DeclaringType != null)
				{
					if (typeReference == null)
					{
						return false;
					}
					Type type2 = minfo.DeclaringType;
					if (minfo is Type && type2.IsGenericType && !type2.IsGenericTypeDefinition)
					{
						type2 = type2.GetGenericTypeDefinition();
					}
					if (!typeReference.Is(type2))
					{
						return false;
					}
				}
				else if (typeReference != null)
				{
					return false;
				}
				if (!(mref is TypeSpecification) && mref.Name != minfo.Name)
				{
					return false;
				}
				TypeReference typeReference2 = mref as TypeReference;
				if (typeReference2 != null)
				{
					Type type3 = minfo as Type;
					if (type3 == null)
					{
						return false;
					}
					if (type3.IsGenericParameter)
					{
						return false;
					}
					GenericInstanceType genericInstanceType = mref as GenericInstanceType;
					if (genericInstanceType != null)
					{
						if (!type3.IsGenericType)
						{
							return false;
						}
						Collection<TypeReference> genericArguments = genericInstanceType.GenericArguments;
						Type[] genericArguments2 = type3.GetGenericArguments();
						if (genericArguments.Count != genericArguments2.Length)
						{
							return false;
						}
						for (int i = 0; i < genericArguments.Count; i++)
						{
							if (!genericArguments[i].Is(genericArguments2[i]))
							{
								return false;
							}
						}
						return genericInstanceType.ElementType.Is(type3.GetGenericTypeDefinition());
					}
					else
					{
						if (typeReference2.HasGenericParameters)
						{
							if (!type3.IsGenericType)
							{
								return false;
							}
							Collection<GenericParameter> genericParameters = typeReference2.GenericParameters;
							Type[] genericArguments3 = type3.GetGenericArguments();
							if (genericParameters.Count != genericArguments3.Length)
							{
								return false;
							}
							for (int j = 0; j < genericParameters.Count; j++)
							{
								if (!genericParameters[j].Is(genericArguments3[j]))
								{
									return false;
								}
							}
						}
						else if (type3.IsGenericType)
						{
							return false;
						}
						ArrayType arrayType = mref as ArrayType;
						if (arrayType != null)
						{
							return type3.IsArray && arrayType.Dimensions.Count == type3.GetArrayRank() && arrayType.ElementType.Is(type3.GetElementType());
						}
						ByReferenceType byReferenceType = mref as ByReferenceType;
						if (byReferenceType != null)
						{
							return type3.IsByRef && byReferenceType.ElementType.Is(type3.GetElementType());
						}
						PointerType pointerType = mref as PointerType;
						if (pointerType != null)
						{
							return type3.IsPointer && pointerType.ElementType.Is(type3.GetElementType());
						}
						TypeSpecification typeSpecification = mref as TypeSpecification;
						if (typeSpecification != null)
						{
							return typeSpecification.ElementType.Is(type3.HasElementType ? type3.GetElementType() : type3);
						}
						if (typeReference != null)
						{
							return mref.Name == type3.Name;
						}
						return mref.FullName == type3.FullName.Replace("+", "/");
					}
				}
				else
				{
					if (minfo is Type)
					{
						return false;
					}
					MethodReference methodRef = mref as MethodReference;
					if (methodRef == null)
					{
						return !(minfo is MethodInfo) && mref is FieldReference == minfo is FieldInfo && mref is PropertyReference == minfo is PropertyInfo && mref is EventReference == minfo is EventInfo;
					}
					MethodBase methodBase = minfo as MethodBase;
					if (methodBase == null)
					{
						return false;
					}
					Collection<ParameterDefinition> parameters = methodRef.Parameters;
					ParameterInfo[] parameters2 = methodBase.GetParameters();
					if (parameters.Count != parameters2.Length)
					{
						return false;
					}
					GenericInstanceMethod genericInstanceMethod = mref as GenericInstanceMethod;
					if (genericInstanceMethod == null)
					{
						if (methodRef.HasGenericParameters)
						{
							if (!methodBase.IsGenericMethod)
							{
								return false;
							}
							Collection<GenericParameter> genericParameters2 = methodRef.GenericParameters;
							Type[] genericArguments4 = methodBase.GetGenericArguments();
							if (genericParameters2.Count != genericArguments4.Length)
							{
								return false;
							}
							for (int k = 0; k < genericParameters2.Count; k++)
							{
								if (!genericParameters2[k].Is(genericArguments4[k]))
								{
									return false;
								}
							}
						}
						else if (methodBase.IsGenericMethod)
						{
							return false;
						}
						Relinker relinker = delegate(IMetadataTokenProvider paramMemberRef, IGenericParameterProvider ctx)
						{
							TypeReference typeReference3 = paramMemberRef as TypeReference;
							if (typeReference3 == null)
							{
								return paramMemberRef;
							}
							return base.<Is>g__ResolveParameter|1(typeReference3);
						};
						MemberReference memberReference = methodRef.ReturnType.Relink(relinker, null);
						MethodInfo methodInfo = methodBase as MethodInfo;
						if (!memberReference.Is(((methodInfo != null) ? methodInfo.ReturnType : null) ?? typeof(void)))
						{
							MemberReference returnType = methodRef.ReturnType;
							MethodInfo methodInfo2 = methodBase as MethodInfo;
							if (!returnType.Is(((methodInfo2 != null) ? methodInfo2.ReturnType : null) ?? typeof(void)))
							{
								return false;
							}
						}
						for (int l = 0; l < parameters.Count; l++)
						{
							if (!parameters[l].ParameterType.Relink(relinker, null).Is(parameters2[l].ParameterType) && !parameters[l].ParameterType.Is(parameters2[l].ParameterType))
							{
								return false;
							}
						}
						return true;
					}
					if (!methodBase.IsGenericMethod)
					{
						return false;
					}
					Collection<TypeReference> genericArguments5 = genericInstanceMethod.GenericArguments;
					Type[] genericArguments6 = methodBase.GetGenericArguments();
					if (genericArguments5.Count != genericArguments6.Length)
					{
						return false;
					}
					for (int m = 0; m < genericArguments5.Count; m++)
					{
						if (!genericArguments5[m].Is(genericArguments6[m]))
						{
							return false;
						}
					}
					MemberReference elementMethod = genericInstanceMethod.ElementMethod;
					MethodInfo methodInfo3 = methodBase as MethodInfo;
					return elementMethod.Is(((methodInfo3 != null) ? methodInfo3.GetGenericMethodDefinition() : null) ?? methodBase);
				}
			}
		}

		public static IMetadataTokenProvider ImportReference(this ModuleDefinition mod, IMetadataTokenProvider mtp)
		{
			if (mtp is TypeReference)
			{
				return mod.ImportReference((TypeReference)mtp);
			}
			if (mtp is FieldReference)
			{
				return mod.ImportReference((FieldReference)mtp);
			}
			if (mtp is MethodReference)
			{
				return mod.ImportReference((MethodReference)mtp);
			}
			return mtp;
		}

		public static void AddRange<T>(this Collection<T> list, IEnumerable<T> other)
		{
			foreach (T t in other)
			{
				list.Add(t);
			}
		}

		public static void AddRange(this IDictionary dict, IDictionary other)
		{
			foreach (object obj in other)
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
				dict.Add(dictionaryEntry.Key, dictionaryEntry.Value);
			}
		}

		public static void AddRange<K, V>(this IDictionary<K, V> dict, IDictionary<K, V> other)
		{
			foreach (KeyValuePair<K, V> keyValuePair in other)
			{
				dict.Add(keyValuePair.Key, keyValuePair.Value);
			}
		}

		public static void AddRange<K, V>(this Dictionary<K, V> dict, Dictionary<K, V> other)
		{
			foreach (KeyValuePair<K, V> keyValuePair in other)
			{
				dict.Add(keyValuePair.Key, keyValuePair.Value);
			}
		}

		public static void InsertRange<T>(this Collection<T> list, int index, IEnumerable<T> other)
		{
			foreach (T t in other)
			{
				list.Insert(index++, t);
			}
		}

		public static bool IsCompatible(this Type type, Type other)
		{
			return type._IsCompatible(other) || other._IsCompatible(type);
		}

		private static bool _IsCompatible(this Type type, Type other)
		{
			return type == other || type.IsAssignableFrom(other) || (other.IsEnum && type.IsCompatible(Enum.GetUnderlyingType(other)));
		}

		public static T GetDeclaredMember<T>(this T member) where T : MemberInfo
		{
			if (member.DeclaringType == member.ReflectedType)
			{
				return member;
			}
			int metadataToken = member.MetadataToken;
			foreach (MemberInfo memberInfo in member.DeclaringType.GetMembers((BindingFlags)(-1)))
			{
				if (memberInfo.MetadataToken == metadataToken)
				{
					return (T)((object)memberInfo);
				}
			}
			return member;
		}

		public unsafe static void SetMonoCorlibInternal(this Assembly asm, bool value)
		{
			if (Type.GetType("Mono.Runtime") == null)
			{
				return;
			}
			Type type = ((asm != null) ? asm.GetType() : null);
			if (type == null)
			{
				return;
			}
			Dictionary<Type, FieldInfo> dictionary = Extensions.fmap_mono_assembly;
			FieldInfo fieldInfo;
			lock (dictionary)
			{
				if (!Extensions.fmap_mono_assembly.TryGetValue(type, out fieldInfo))
				{
					fieldInfo = type.GetField("_mono_assembly", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) ?? type.GetField("dynamic_assembly", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					Extensions.fmap_mono_assembly[type] = fieldInfo;
				}
			}
			if (fieldInfo == null)
			{
				return;
			}
			AssemblyName assemblyName = new AssemblyName(asm.FullName);
			Dictionary<string, Assembly> assemblyCache = ReflectionHelper.AssemblyCache;
			lock (assemblyCache)
			{
				ReflectionHelper.AssemblyCache[assemblyName.FullName] = asm;
				ReflectionHelper.AssemblyCache[assemblyName.Name] = asm;
			}
			long num = 0L;
			object value2 = fieldInfo.GetValue(asm);
			if (value2 is IntPtr)
			{
				IntPtr intPtr = (IntPtr)value2;
				num = (long)intPtr;
			}
			else if (value2 is UIntPtr)
			{
				UIntPtr uintPtr = (UIntPtr)value2;
				num = (long)(ulong)uintPtr;
			}
			int num2 = IntPtr.Size + IntPtr.Size + IntPtr.Size + IntPtr.Size + IntPtr.Size + IntPtr.Size + 20 + 4 + 4 + 4 + ((!Extensions._MonoAssemblyNameHasArch) ? ((typeof(object).Assembly.GetName().Name == "System.Private.CoreLib") ? 16 : 8) : ((typeof(object).Assembly.GetName().Name == "System.Private.CoreLib") ? ((IntPtr.Size == 4) ? 20 : 24) : ((IntPtr.Size == 4) ? 12 : 16))) + IntPtr.Size + IntPtr.Size + 1 + 1 + 1;
			byte* ptr = num + num2;
			*ptr = (value ? 1 : 0);
		}

		public static T CreateDelegate<T>(this MethodBase method) where T : Delegate
		{
			return (T)((object)method.CreateDelegate(typeof(T), null));
		}

		public static T CreateDelegate<T>(this MethodBase method, object target) where T : Delegate
		{
			return (T)((object)method.CreateDelegate(typeof(T), target));
		}

		public static Delegate CreateDelegate(this MethodBase method, Type delegateType)
		{
			return method.CreateDelegate(delegateType, null);
		}

		public static Delegate CreateDelegate(this MethodBase method, Type delegateType, object target)
		{
			if (!typeof(Delegate).IsAssignableFrom(delegateType))
			{
				throw new ArgumentException("Type argument must be a delegate type!");
			}
			DynamicMethod dynamicMethod = method as DynamicMethod;
			if (dynamicMethod != null)
			{
				return dynamicMethod.CreateDelegate(delegateType, target);
			}
			RuntimeMethodHandle methodHandle = method.MethodHandle;
			RuntimeHelpers.PrepareMethod(methodHandle);
			IntPtr functionPointer = methodHandle.GetFunctionPointer();
			return (Delegate)Activator.CreateInstance(delegateType, new object[] { target, functionPointer });
		}

		public static MethodDefinition FindMethod(this TypeDefinition type, string id, bool simple = true)
		{
			if (simple && !id.Contains(" "))
			{
				foreach (MethodDefinition methodDefinition in type.Methods)
				{
					if (methodDefinition.GetID(null, null, true, true) == id)
					{
						return methodDefinition;
					}
				}
				foreach (MethodDefinition methodDefinition2 in type.Methods)
				{
					if (methodDefinition2.GetID(null, null, false, true) == id)
					{
						return methodDefinition2;
					}
				}
			}
			foreach (MethodDefinition methodDefinition3 in type.Methods)
			{
				if (methodDefinition3.GetID(null, null, true, false) == id)
				{
					return methodDefinition3;
				}
			}
			foreach (MethodDefinition methodDefinition4 in type.Methods)
			{
				if (methodDefinition4.GetID(null, null, false, false) == id)
				{
					return methodDefinition4;
				}
			}
			return null;
		}

		public static MethodDefinition FindMethodDeep(this TypeDefinition type, string id, bool simple = true)
		{
			MethodDefinition methodDefinition;
			if ((methodDefinition = type.FindMethod(id, simple)) == null)
			{
				TypeReference baseType = type.BaseType;
				if (baseType == null)
				{
					return null;
				}
				TypeDefinition typeDefinition = baseType.Resolve();
				if (typeDefinition == null)
				{
					return null;
				}
				methodDefinition = typeDefinition.FindMethodDeep(id, simple);
			}
			return methodDefinition;
		}

		public static MethodInfo FindMethod(this Type type, string id, bool simple = true)
		{
			MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (simple && !id.Contains(" "))
			{
				foreach (MethodInfo methodInfo in methods)
				{
					if (methodInfo.GetID(null, null, true, false, true) == id)
					{
						return methodInfo;
					}
				}
				foreach (MethodInfo methodInfo2 in methods)
				{
					if (methodInfo2.GetID(null, null, false, false, true) == id)
					{
						return methodInfo2;
					}
				}
			}
			foreach (MethodInfo methodInfo3 in methods)
			{
				if (methodInfo3.GetID(null, null, true, false, false) == id)
				{
					return methodInfo3;
				}
			}
			foreach (MethodInfo methodInfo4 in methods)
			{
				if (methodInfo4.GetID(null, null, false, false, false) == id)
				{
					return methodInfo4;
				}
			}
			return null;
		}

		public static MethodInfo FindMethodDeep(this Type type, string id, bool simple = true)
		{
			MethodInfo methodInfo;
			if ((methodInfo = type.FindMethod(id, simple)) == null)
			{
				Type baseType = type.BaseType;
				if (baseType == null)
				{
					return null;
				}
				methodInfo = baseType.FindMethodDeep(id, simple);
			}
			return methodInfo;
		}

		public static PropertyDefinition FindProperty(this TypeDefinition type, string name)
		{
			foreach (PropertyDefinition propertyDefinition in type.Properties)
			{
				if (propertyDefinition.Name == name)
				{
					return propertyDefinition;
				}
			}
			return null;
		}

		public static PropertyDefinition FindPropertyDeep(this TypeDefinition type, string name)
		{
			PropertyDefinition propertyDefinition;
			if ((propertyDefinition = type.FindProperty(name)) == null)
			{
				TypeReference baseType = type.BaseType;
				if (baseType == null)
				{
					return null;
				}
				TypeDefinition typeDefinition = baseType.Resolve();
				if (typeDefinition == null)
				{
					return null;
				}
				propertyDefinition = typeDefinition.FindPropertyDeep(name);
			}
			return propertyDefinition;
		}

		public static FieldDefinition FindField(this TypeDefinition type, string name)
		{
			foreach (FieldDefinition fieldDefinition in type.Fields)
			{
				if (fieldDefinition.Name == name)
				{
					return fieldDefinition;
				}
			}
			return null;
		}

		public static FieldDefinition FindFieldDeep(this TypeDefinition type, string name)
		{
			FieldDefinition fieldDefinition;
			if ((fieldDefinition = type.FindField(name)) == null)
			{
				TypeReference baseType = type.BaseType;
				if (baseType == null)
				{
					return null;
				}
				TypeDefinition typeDefinition = baseType.Resolve();
				if (typeDefinition == null)
				{
					return null;
				}
				fieldDefinition = typeDefinition.FindFieldDeep(name);
			}
			return fieldDefinition;
		}

		public static EventDefinition FindEvent(this TypeDefinition type, string name)
		{
			foreach (EventDefinition eventDefinition in type.Events)
			{
				if (eventDefinition.Name == name)
				{
					return eventDefinition;
				}
			}
			return null;
		}

		public static EventDefinition FindEventDeep(this TypeDefinition type, string name)
		{
			EventDefinition eventDefinition;
			if ((eventDefinition = type.FindEvent(name)) == null)
			{
				TypeReference baseType = type.BaseType;
				if (baseType == null)
				{
					return null;
				}
				TypeDefinition typeDefinition = baseType.Resolve();
				if (typeDefinition == null)
				{
					return null;
				}
				eventDefinition = typeDefinition.FindEventDeep(name);
			}
			return eventDefinition;
		}

		public static string GetID(this MethodReference method, string name = null, string type = null, bool withType = true, bool simple = false)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (simple)
			{
				if (withType && (type != null || method.DeclaringType != null))
				{
					stringBuilder.Append(type ?? method.DeclaringType.GetPatchFullName()).Append("::");
				}
				stringBuilder.Append(name ?? method.Name);
				return stringBuilder.ToString();
			}
			stringBuilder.Append(method.ReturnType.GetPatchFullName()).Append(" ");
			if (withType && (type != null || method.DeclaringType != null))
			{
				stringBuilder.Append(type ?? method.DeclaringType.GetPatchFullName()).Append("::");
			}
			stringBuilder.Append(name ?? method.Name);
			GenericInstanceMethod genericInstanceMethod = method as GenericInstanceMethod;
			if (genericInstanceMethod != null && genericInstanceMethod.GenericArguments.Count != 0)
			{
				stringBuilder.Append("<");
				Collection<TypeReference> genericArguments = genericInstanceMethod.GenericArguments;
				for (int i = 0; i < genericArguments.Count; i++)
				{
					if (i > 0)
					{
						stringBuilder.Append(",");
					}
					stringBuilder.Append(genericArguments[i].GetPatchFullName());
				}
				stringBuilder.Append(">");
			}
			else if (method.GenericParameters.Count != 0)
			{
				stringBuilder.Append("<");
				Collection<GenericParameter> genericParameters = method.GenericParameters;
				for (int j = 0; j < genericParameters.Count; j++)
				{
					if (j > 0)
					{
						stringBuilder.Append(",");
					}
					stringBuilder.Append(genericParameters[j].Name);
				}
				stringBuilder.Append(">");
			}
			stringBuilder.Append("(");
			if (method.HasParameters)
			{
				Collection<ParameterDefinition> parameters = method.Parameters;
				for (int k = 0; k < parameters.Count; k++)
				{
					ParameterDefinition parameterDefinition = parameters[k];
					if (k > 0)
					{
						stringBuilder.Append(",");
					}
					if (parameterDefinition.ParameterType.IsSentinel)
					{
						stringBuilder.Append("...,");
					}
					stringBuilder.Append(parameterDefinition.ParameterType.GetPatchFullName());
				}
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		public static string GetID(this Mono.Cecil.CallSite method)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(method.ReturnType.GetPatchFullName()).Append(" ");
			stringBuilder.Append("(");
			if (method.HasParameters)
			{
				Collection<ParameterDefinition> parameters = method.Parameters;
				for (int i = 0; i < parameters.Count; i++)
				{
					ParameterDefinition parameterDefinition = parameters[i];
					if (i > 0)
					{
						stringBuilder.Append(",");
					}
					if (parameterDefinition.ParameterType.IsSentinel)
					{
						stringBuilder.Append("...,");
					}
					stringBuilder.Append(parameterDefinition.ParameterType.GetPatchFullName());
				}
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		public static string GetID(this MethodBase method, string name = null, string type = null, bool withType = true, bool proxyMethod = false, bool simple = false)
		{
			while (method is MethodInfo && method.IsGenericMethod && !method.IsGenericMethodDefinition)
			{
				method = ((MethodInfo)method).GetGenericMethodDefinition();
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (simple)
			{
				if (withType && (type != null || method.DeclaringType != null))
				{
					stringBuilder.Append(type ?? method.DeclaringType.FullName).Append("::");
				}
				stringBuilder.Append(name ?? method.Name);
				return stringBuilder.ToString();
			}
			StringBuilder stringBuilder2 = stringBuilder;
			MethodInfo methodInfo = method as MethodInfo;
			string text;
			if (methodInfo == null)
			{
				text = null;
			}
			else
			{
				Type returnType = methodInfo.ReturnType;
				text = ((returnType != null) ? returnType.FullName : null);
			}
			stringBuilder2.Append(text ?? "System.Void").Append(" ");
			if (withType && (type != null || method.DeclaringType != null))
			{
				stringBuilder.Append(type ?? method.DeclaringType.FullName.Replace("+", "/")).Append("::");
			}
			stringBuilder.Append(name ?? method.Name);
			if (method.ContainsGenericParameters)
			{
				stringBuilder.Append("<");
				Type[] genericArguments = method.GetGenericArguments();
				for (int i = 0; i < genericArguments.Length; i++)
				{
					if (i > 0)
					{
						stringBuilder.Append(",");
					}
					stringBuilder.Append(genericArguments[i].Name);
				}
				stringBuilder.Append(">");
			}
			stringBuilder.Append("(");
			ParameterInfo[] parameters = method.GetParameters();
			for (int j = (proxyMethod ? 1 : 0); j < parameters.Length; j++)
			{
				ParameterInfo parameterInfo = parameters[j];
				if (j > (proxyMethod ? 1 : 0))
				{
					stringBuilder.Append(",");
				}
				object[] array;
				try
				{
					array = parameterInfo.GetCustomAttributes(Extensions.t_ParamArrayAttribute, false);
				}
				catch
				{
					array = null;
				}
				if (array != null && array.Length != 0)
				{
					stringBuilder.Append("...,");
				}
				stringBuilder.Append(parameterInfo.ParameterType.FullName);
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		public static string GetPatchName(this MemberReference mr)
		{
			Mono.Cecil.ICustomAttributeProvider customAttributeProvider = mr as Mono.Cecil.ICustomAttributeProvider;
			return ((customAttributeProvider != null) ? customAttributeProvider.GetPatchName() : null) ?? mr.Name;
		}

		public static string GetPatchFullName(this MemberReference mr)
		{
			Mono.Cecil.ICustomAttributeProvider customAttributeProvider = mr as Mono.Cecil.ICustomAttributeProvider;
			return ((customAttributeProvider != null) ? customAttributeProvider.GetPatchFullName(mr) : null) ?? mr.FullName;
		}

		private static string GetPatchName(this Mono.Cecil.ICustomAttributeProvider cap)
		{
			CustomAttribute customAttribute = cap.GetCustomAttribute("MonoMod.MonoModPatch");
			string text;
			if (customAttribute != null)
			{
				text = (string)customAttribute.ConstructorArguments[0].Value;
				int num = text.LastIndexOf('.');
				if (num != -1 && num != text.Length - 1)
				{
					text = text.Substring(num + 1);
				}
				return text;
			}
			text = ((MemberReference)cap).Name;
			if (!text.StartsWith("patch_"))
			{
				return text;
			}
			return text.Substring(6);
		}

		private static string GetPatchFullName(this Mono.Cecil.ICustomAttributeProvider cap, MemberReference mr)
		{
			TypeReference typeReference = cap as TypeReference;
			if (typeReference != null)
			{
				CustomAttribute customAttribute = cap.GetCustomAttribute("MonoMod.MonoModPatch");
				string text;
				if (customAttribute != null)
				{
					text = (string)customAttribute.ConstructorArguments[0].Value;
				}
				else
				{
					text = ((MemberReference)cap).Name;
					text = (text.StartsWith("patch_") ? text.Substring(6) : text);
				}
				if (text.StartsWith("global::"))
				{
					text = text.Substring(8);
				}
				else if (!text.Contains(".") && !text.Contains("/"))
				{
					if (!string.IsNullOrEmpty(typeReference.Namespace))
					{
						text = typeReference.Namespace + "." + text;
					}
					else if (typeReference.IsNested)
					{
						text = typeReference.DeclaringType.GetPatchFullName() + "/" + text;
					}
				}
				if (mr is TypeSpecification)
				{
					List<TypeSpecification> list = new List<TypeSpecification>();
					TypeSpecification typeSpecification = (TypeSpecification)mr;
					do
					{
						list.Add(typeSpecification);
					}
					while ((typeSpecification = typeSpecification.ElementType as TypeSpecification) != null);
					StringBuilder stringBuilder = new StringBuilder(text.Length + list.Count * 4);
					stringBuilder.Append(text);
					for (int i = list.Count - 1; i > -1; i--)
					{
						typeSpecification = list[i];
						if (typeSpecification.IsByReference)
						{
							stringBuilder.Append("&");
						}
						else if (typeSpecification.IsPointer)
						{
							stringBuilder.Append("*");
						}
						else if (!typeSpecification.IsPinned && !typeSpecification.IsSentinel)
						{
							if (typeSpecification.IsArray)
							{
								ArrayType arrayType = (ArrayType)typeSpecification;
								if (arrayType.IsVector)
								{
									stringBuilder.Append("[]");
								}
								else
								{
									stringBuilder.Append("[");
									for (int j = 0; j < arrayType.Dimensions.Count; j++)
									{
										if (j > 0)
										{
											stringBuilder.Append(",");
										}
										stringBuilder.Append(arrayType.Dimensions[j].ToString());
									}
									stringBuilder.Append("]");
								}
							}
							else if (typeSpecification.IsRequiredModifier)
							{
								stringBuilder.Append("modreq(").Append(((RequiredModifierType)typeSpecification).ModifierType).Append(")");
							}
							else if (typeSpecification.IsOptionalModifier)
							{
								stringBuilder.Append("modopt(").Append(((OptionalModifierType)typeSpecification).ModifierType).Append(")");
							}
							else if (typeSpecification.IsGenericInstance)
							{
								GenericInstanceType genericInstanceType = (GenericInstanceType)typeSpecification;
								stringBuilder.Append("<");
								for (int k = 0; k < genericInstanceType.GenericArguments.Count; k++)
								{
									if (k > 0)
									{
										stringBuilder.Append(",");
									}
									stringBuilder.Append(genericInstanceType.GenericArguments[k].GetPatchFullName());
								}
								stringBuilder.Append(">");
							}
							else
							{
								if (!typeSpecification.IsFunctionPointer)
								{
									throw new NotSupportedException(string.Format("MonoMod can't handle TypeSpecification: {0} ({1})", typeReference.FullName, typeReference.GetType()));
								}
								FunctionPointerType functionPointerType = (FunctionPointerType)typeSpecification;
								stringBuilder.Append(" ").Append(functionPointerType.ReturnType.GetPatchFullName()).Append(" *(");
								if (functionPointerType.HasParameters)
								{
									for (int l = 0; l < functionPointerType.Parameters.Count; l++)
									{
										ParameterDefinition parameterDefinition = functionPointerType.Parameters[l];
										if (l > 0)
										{
											stringBuilder.Append(",");
										}
										if (parameterDefinition.ParameterType.IsSentinel)
										{
											stringBuilder.Append("...,");
										}
										stringBuilder.Append(parameterDefinition.ParameterType.FullName);
									}
								}
								stringBuilder.Append(")");
							}
						}
					}
					text = stringBuilder.ToString();
				}
				return text;
			}
			FieldReference fieldReference = cap as FieldReference;
			if (fieldReference != null)
			{
				return string.Concat(new string[]
				{
					fieldReference.FieldType.GetPatchFullName(),
					" ",
					fieldReference.DeclaringType.GetPatchFullName(),
					"::",
					cap.GetPatchName()
				});
			}
			if (cap is MethodReference)
			{
				throw new InvalidOperationException("GetPatchFullName not supported on MethodReferences - use GetID instead");
			}
			throw new InvalidOperationException(string.Format("GetPatchFullName not supported on type {0}", cap.GetType()));
		}

		public static MethodDefinition Clone(this MethodDefinition o, MethodDefinition c = null)
		{
			if (o == null)
			{
				return null;
			}
			if (c == null)
			{
				c = new MethodDefinition(o.Name, o.Attributes, o.ReturnType);
			}
			c.Name = o.Name;
			c.Attributes = o.Attributes;
			c.ReturnType = o.ReturnType;
			c.DeclaringType = o.DeclaringType;
			c.MetadataToken = c.MetadataToken;
			c.Body = o.Body.Clone(c);
			c.Attributes = o.Attributes;
			c.ImplAttributes = o.ImplAttributes;
			c.PInvokeInfo = o.PInvokeInfo;
			c.IsPreserveSig = o.IsPreserveSig;
			c.IsPInvokeImpl = o.IsPInvokeImpl;
			foreach (GenericParameter genericParameter in o.GenericParameters)
			{
				c.GenericParameters.Add(genericParameter.Clone());
			}
			foreach (ParameterDefinition parameterDefinition in o.Parameters)
			{
				c.Parameters.Add(parameterDefinition);
			}
			foreach (CustomAttribute customAttribute in o.CustomAttributes)
			{
				c.CustomAttributes.Add(customAttribute.Clone());
			}
			foreach (MethodReference methodReference in o.Overrides)
			{
				c.Overrides.Add(methodReference);
			}
			return c;
		}

		public static Mono.Cecil.Cil.MethodBody Clone(this Mono.Cecil.Cil.MethodBody bo, MethodDefinition m)
		{
			if (bo == null)
			{
				return null;
			}
			Mono.Cecil.Cil.MethodBody bc = new Mono.Cecil.Cil.MethodBody(m);
			bc.MaxStackSize = bo.MaxStackSize;
			bc.InitLocals = bo.InitLocals;
			bc.LocalVarToken = bo.LocalVarToken;
			bc.Instructions.AddRange<Instruction>(bo.Instructions.Select<Instruction, Instruction>(delegate(Instruction o)
			{
				Instruction instruction4 = Instruction.Create(Mono.Cecil.Cil.OpCodes.Nop);
				instruction4.OpCode = o.OpCode;
				instruction4.Operand = o.Operand;
				instruction4.Offset = o.Offset;
				return instruction4;
			}));
			Func<Instruction, Instruction> <>9__4;
			foreach (Instruction instruction in bc.Instructions)
			{
				Instruction instruction2 = instruction.Operand as Instruction;
				if (instruction2 != null)
				{
					instruction.Operand = bc.Instructions[bo.Instructions.IndexOf(instruction2)];
				}
				else
				{
					Instruction[] array = instruction.Operand as Instruction[];
					if (array != null)
					{
						Instruction instruction3 = instruction;
						IEnumerable<Instruction> enumerable = array;
						Func<Instruction, Instruction> func;
						if ((func = <>9__4) == null)
						{
							func = (<>9__4 = (Instruction i) => bc.Instructions[bo.Instructions.IndexOf(i)]);
						}
						instruction3.Operand = enumerable.Select<Instruction, Instruction>(func).ToArray<Instruction>();
					}
				}
			}
			bc.ExceptionHandlers.AddRange<Mono.Cecil.Cil.ExceptionHandler>(bo.ExceptionHandlers.Select<Mono.Cecil.Cil.ExceptionHandler, Mono.Cecil.Cil.ExceptionHandler>((Mono.Cecil.Cil.ExceptionHandler o) => new Mono.Cecil.Cil.ExceptionHandler(o.HandlerType)
			{
				TryStart = ((o.TryStart == null) ? null : bc.Instructions[bo.Instructions.IndexOf(o.TryStart)]),
				TryEnd = ((o.TryEnd == null) ? null : bc.Instructions[bo.Instructions.IndexOf(o.TryEnd)]),
				FilterStart = ((o.FilterStart == null) ? null : bc.Instructions[bo.Instructions.IndexOf(o.FilterStart)]),
				HandlerStart = ((o.HandlerStart == null) ? null : bc.Instructions[bo.Instructions.IndexOf(o.HandlerStart)]),
				HandlerEnd = ((o.HandlerEnd == null) ? null : bc.Instructions[bo.Instructions.IndexOf(o.HandlerEnd)]),
				CatchType = o.CatchType
			}));
			bc.Variables.AddRange<VariableDefinition>(bo.Variables.Select<VariableDefinition, VariableDefinition>((VariableDefinition o) => new VariableDefinition(o.VariableType)));
			m.CustomDebugInformations.AddRange<CustomDebugInformation>(bo.Method.CustomDebugInformations);
			m.DebugInformation.SequencePoints.AddRange<SequencePoint>(bo.Method.DebugInformation.SequencePoints.Select<SequencePoint, SequencePoint>((SequencePoint o) => new SequencePoint(bc.Instructions.FirstOrDefault<Instruction>((Instruction i) => i.Offset == o.Offset), o.Document)
			{
				StartLine = o.StartLine,
				StartColumn = o.StartColumn,
				EndLine = o.EndLine,
				EndColumn = o.EndColumn
			}));
			return bc;
		}

		public static GenericParameter Update(this GenericParameter param, int position, GenericParameterType type)
		{
			Extensions.f_GenericParameter_position.SetValue(param, position);
			Extensions.f_GenericParameter_type.SetValue(param, type);
			return param;
		}

		public static GenericParameter ResolveGenericParameter(this IGenericParameterProvider provider, GenericParameter orig)
		{
			if (provider is GenericParameter && ((GenericParameter)provider).Name == orig.Name)
			{
				return (GenericParameter)provider;
			}
			foreach (GenericParameter genericParameter in provider.GenericParameters)
			{
				if (genericParameter.Name == orig.Name)
				{
					return genericParameter;
				}
			}
			int position = orig.Position;
			if (provider is MethodReference && orig.DeclaringMethod != null)
			{
				if (position < provider.GenericParameters.Count)
				{
					return provider.GenericParameters[position];
				}
				return new GenericParameter(orig.Name, provider).Update(position, GenericParameterType.Method);
			}
			else
			{
				if (!(provider is TypeReference) || orig.DeclaringType == null)
				{
					TypeSpecification typeSpecification = provider as TypeSpecification;
					GenericParameter genericParameter2;
					if ((genericParameter2 = ((typeSpecification != null) ? typeSpecification.ElementType.ResolveGenericParameter(orig) : null)) == null)
					{
						MemberReference memberReference = provider as MemberReference;
						if (memberReference == null)
						{
							return null;
						}
						TypeReference declaringType = memberReference.DeclaringType;
						if (declaringType == null)
						{
							return null;
						}
						genericParameter2 = declaringType.ResolveGenericParameter(orig);
					}
					return genericParameter2;
				}
				if (position < provider.GenericParameters.Count)
				{
					return provider.GenericParameters[position];
				}
				return new GenericParameter(orig.Name, provider).Update(position, GenericParameterType.Type);
			}
			GenericParameter genericParameter3;
			return genericParameter3;
		}

		public static IMetadataTokenProvider Relink(this IMetadataTokenProvider mtp, Relinker relinker, IGenericParameterProvider context)
		{
			if (mtp is TypeReference)
			{
				return ((TypeReference)mtp).Relink(relinker, context);
			}
			if (mtp is GenericParameterConstraint)
			{
				return ((GenericParameterConstraint)mtp).Relink(relinker, context);
			}
			if (mtp is MethodReference)
			{
				return ((MethodReference)mtp).Relink(relinker, context);
			}
			if (mtp is FieldReference)
			{
				return ((FieldReference)mtp).Relink(relinker, context);
			}
			if (mtp is ParameterDefinition)
			{
				return ((ParameterDefinition)mtp).Relink(relinker, context);
			}
			if (mtp is Mono.Cecil.CallSite)
			{
				return ((Mono.Cecil.CallSite)mtp).Relink(relinker, context);
			}
			throw new InvalidOperationException(string.Format("MonoMod can't handle metadata token providers of the type {0}", mtp.GetType()));
		}

		public static TypeReference Relink(this TypeReference type, Relinker relinker, IGenericParameterProvider context)
		{
			if (type == null)
			{
				return null;
			}
			TypeSpecification typeSpecification = type as TypeSpecification;
			if (typeSpecification != null)
			{
				TypeReference typeReference = typeSpecification.ElementType.Relink(relinker, context);
				if (type.IsSentinel)
				{
					return new SentinelType(typeReference);
				}
				if (type.IsByReference)
				{
					return new ByReferenceType(typeReference);
				}
				if (type.IsPointer)
				{
					return new PointerType(typeReference);
				}
				if (type.IsPinned)
				{
					return new PinnedType(typeReference);
				}
				if (type.IsArray)
				{
					ArrayType arrayType = new ArrayType(typeReference, ((ArrayType)type).Rank);
					for (int i = 0; i < arrayType.Rank; i++)
					{
						arrayType.Dimensions[i] = ((ArrayType)type).Dimensions[i];
					}
					return arrayType;
				}
				if (type.IsRequiredModifier)
				{
					return new RequiredModifierType(((RequiredModifierType)type).ModifierType.Relink(relinker, context), typeReference);
				}
				if (type.IsOptionalModifier)
				{
					return new OptionalModifierType(((OptionalModifierType)type).ModifierType.Relink(relinker, context), typeReference);
				}
				if (type.IsGenericInstance)
				{
					GenericInstanceType genericInstanceType = new GenericInstanceType(typeReference);
					foreach (TypeReference typeReference2 in ((GenericInstanceType)type).GenericArguments)
					{
						genericInstanceType.GenericArguments.Add((typeReference2 != null) ? typeReference2.Relink(relinker, context) : null);
					}
					return genericInstanceType;
				}
				if (type.IsFunctionPointer)
				{
					FunctionPointerType functionPointerType = (FunctionPointerType)type;
					functionPointerType.ReturnType = functionPointerType.ReturnType.Relink(relinker, context);
					for (int j = 0; j < functionPointerType.Parameters.Count; j++)
					{
						functionPointerType.Parameters[j].ParameterType = functionPointerType.Parameters[j].ParameterType.Relink(relinker, context);
					}
					return functionPointerType;
				}
				throw new NotSupportedException(string.Format("MonoMod can't handle TypeSpecification: {0} ({1})", type.FullName, type.GetType()));
			}
			else
			{
				if (!type.IsGenericParameter || context == null)
				{
					return (TypeReference)relinker(type, context);
				}
				GenericParameter genericParameter = context.ResolveGenericParameter((GenericParameter)type);
				if (genericParameter == null)
				{
					throw new RelinkTargetNotFoundException(string.Format("{0} {1} (context: {2})", "MonoMod relinker failed finding", type.FullName, context), type, context);
				}
				for (int k = 0; k < genericParameter.Constraints.Count; k++)
				{
					if (!genericParameter.Constraints[k].GetConstraintType().IsGenericInstance)
					{
						genericParameter.Constraints[k] = genericParameter.Constraints[k].Relink(relinker, context);
					}
				}
				return genericParameter;
			}
		}

		public static GenericParameterConstraint Relink(this GenericParameterConstraint constraint, Relinker relinker, IGenericParameterProvider context)
		{
			if (constraint == null)
			{
				return null;
			}
			GenericParameterConstraint genericParameterConstraint = new GenericParameterConstraint(constraint.ConstraintType.Relink(relinker, context));
			foreach (CustomAttribute customAttribute in constraint.CustomAttributes)
			{
				genericParameterConstraint.CustomAttributes.Add(customAttribute.Relink(relinker, context));
			}
			return genericParameterConstraint;
		}

		public static IMetadataTokenProvider Relink(this MethodReference method, Relinker relinker, IGenericParameterProvider context)
		{
			if (method.IsGenericInstance)
			{
				GenericInstanceMethod genericInstanceMethod = (GenericInstanceMethod)method;
				GenericInstanceMethod genericInstanceMethod2 = new GenericInstanceMethod((MethodReference)genericInstanceMethod.ElementMethod.Relink(relinker, context));
				foreach (TypeReference typeReference in genericInstanceMethod.GenericArguments)
				{
					genericInstanceMethod2.GenericArguments.Add(typeReference.Relink(relinker, context));
				}
				return (MethodReference)relinker(genericInstanceMethod2, context);
			}
			MethodReference methodReference = new MethodReference(method.Name, method.ReturnType, method.DeclaringType.Relink(relinker, context));
			methodReference.CallingConvention = method.CallingConvention;
			methodReference.ExplicitThis = method.ExplicitThis;
			methodReference.HasThis = method.HasThis;
			foreach (GenericParameter genericParameter in method.GenericParameters)
			{
				GenericParameter genericParameter2 = new GenericParameter(genericParameter.Name, genericParameter.Owner)
				{
					Attributes = genericParameter.Attributes
				}.Update(genericParameter.Position, genericParameter.Type);
				methodReference.GenericParameters.Add(genericParameter2);
				foreach (GenericParameterConstraint genericParameterConstraint in genericParameter.Constraints)
				{
					genericParameter2.Constraints.Add(genericParameterConstraint.Relink(relinker, methodReference));
				}
			}
			MethodReference methodReference2 = methodReference;
			TypeReference returnType = methodReference.ReturnType;
			methodReference2.ReturnType = ((returnType != null) ? returnType.Relink(relinker, methodReference) : null);
			foreach (ParameterDefinition parameterDefinition in method.Parameters)
			{
				parameterDefinition.ParameterType = parameterDefinition.ParameterType.Relink(relinker, method);
				methodReference.Parameters.Add(parameterDefinition);
			}
			return (MethodReference)relinker(methodReference, context);
		}

		public static Mono.Cecil.CallSite Relink(this Mono.Cecil.CallSite method, Relinker relinker, IGenericParameterProvider context)
		{
			Mono.Cecil.CallSite callSite = new Mono.Cecil.CallSite(method.ReturnType);
			callSite.CallingConvention = method.CallingConvention;
			callSite.ExplicitThis = method.ExplicitThis;
			callSite.HasThis = method.HasThis;
			Mono.Cecil.CallSite callSite2 = callSite;
			TypeReference returnType = callSite.ReturnType;
			callSite2.ReturnType = ((returnType != null) ? returnType.Relink(relinker, context) : null);
			foreach (ParameterDefinition parameterDefinition in method.Parameters)
			{
				parameterDefinition.ParameterType = parameterDefinition.ParameterType.Relink(relinker, context);
				callSite.Parameters.Add(parameterDefinition);
			}
			return (Mono.Cecil.CallSite)relinker(callSite, context);
		}

		public static IMetadataTokenProvider Relink(this FieldReference field, Relinker relinker, IGenericParameterProvider context)
		{
			TypeReference typeReference = field.DeclaringType.Relink(relinker, context);
			return relinker(new FieldReference(field.Name, field.FieldType.Relink(relinker, typeReference), typeReference), context);
		}

		public static ParameterDefinition Relink(this ParameterDefinition param, Relinker relinker, IGenericParameterProvider context)
		{
			MethodReference methodReference = param.Method as MethodReference;
			param = ((methodReference != null) ? methodReference.Parameters[param.Index] : null) ?? param;
			ParameterDefinition parameterDefinition = new ParameterDefinition(param.Name, param.Attributes, param.ParameterType.Relink(relinker, context))
			{
				IsIn = param.IsIn,
				IsLcid = param.IsLcid,
				IsOptional = param.IsOptional,
				IsOut = param.IsOut,
				IsReturnValue = param.IsReturnValue,
				MarshalInfo = param.MarshalInfo
			};
			if (param.HasConstant)
			{
				parameterDefinition.Constant = param.Constant;
			}
			return parameterDefinition;
		}

		public static ParameterDefinition Clone(this ParameterDefinition param)
		{
			ParameterDefinition parameterDefinition = new ParameterDefinition(param.Name, param.Attributes, param.ParameterType)
			{
				IsIn = param.IsIn,
				IsLcid = param.IsLcid,
				IsOptional = param.IsOptional,
				IsOut = param.IsOut,
				IsReturnValue = param.IsReturnValue,
				MarshalInfo = param.MarshalInfo
			};
			if (param.HasConstant)
			{
				parameterDefinition.Constant = param.Constant;
			}
			foreach (CustomAttribute customAttribute in param.CustomAttributes)
			{
				parameterDefinition.CustomAttributes.Add(customAttribute.Clone());
			}
			return parameterDefinition;
		}

		public static CustomAttribute Relink(this CustomAttribute attrib, Relinker relinker, IGenericParameterProvider context)
		{
			CustomAttribute customAttribute = new CustomAttribute((MethodReference)attrib.Constructor.Relink(relinker, context));
			foreach (CustomAttributeArgument customAttributeArgument in attrib.ConstructorArguments)
			{
				customAttribute.ConstructorArguments.Add(new CustomAttributeArgument(customAttributeArgument.Type.Relink(relinker, context), customAttributeArgument.Value));
			}
			foreach (Mono.Cecil.CustomAttributeNamedArgument customAttributeNamedArgument in attrib.Fields)
			{
				customAttribute.Fields.Add(new Mono.Cecil.CustomAttributeNamedArgument(customAttributeNamedArgument.Name, new CustomAttributeArgument(customAttributeNamedArgument.Argument.Type.Relink(relinker, context), customAttributeNamedArgument.Argument.Value)));
			}
			foreach (Mono.Cecil.CustomAttributeNamedArgument customAttributeNamedArgument2 in attrib.Properties)
			{
				customAttribute.Properties.Add(new Mono.Cecil.CustomAttributeNamedArgument(customAttributeNamedArgument2.Name, new CustomAttributeArgument(customAttributeNamedArgument2.Argument.Type.Relink(relinker, context), customAttributeNamedArgument2.Argument.Value)));
			}
			return customAttribute;
		}

		public static CustomAttribute Clone(this CustomAttribute attrib)
		{
			CustomAttribute customAttribute = new CustomAttribute(attrib.Constructor);
			foreach (CustomAttributeArgument customAttributeArgument in attrib.ConstructorArguments)
			{
				customAttribute.ConstructorArguments.Add(new CustomAttributeArgument(customAttributeArgument.Type, customAttributeArgument.Value));
			}
			foreach (Mono.Cecil.CustomAttributeNamedArgument customAttributeNamedArgument in attrib.Fields)
			{
				customAttribute.Fields.Add(new Mono.Cecil.CustomAttributeNamedArgument(customAttributeNamedArgument.Name, new CustomAttributeArgument(customAttributeNamedArgument.Argument.Type, customAttributeNamedArgument.Argument.Value)));
			}
			foreach (Mono.Cecil.CustomAttributeNamedArgument customAttributeNamedArgument2 in attrib.Properties)
			{
				customAttribute.Properties.Add(new Mono.Cecil.CustomAttributeNamedArgument(customAttributeNamedArgument2.Name, new CustomAttributeArgument(customAttributeNamedArgument2.Argument.Type, customAttributeNamedArgument2.Argument.Value)));
			}
			return customAttribute;
		}

		public static GenericParameter Relink(this GenericParameter param, Relinker relinker, IGenericParameterProvider context)
		{
			GenericParameter genericParameter = new GenericParameter(param.Name, param.Owner)
			{
				Attributes = param.Attributes
			}.Update(param.Position, param.Type);
			foreach (GenericParameterConstraint genericParameterConstraint in param.Constraints)
			{
				genericParameter.Constraints.Add(genericParameterConstraint.Relink(relinker, context));
			}
			return genericParameter;
		}

		public static GenericParameter Clone(this GenericParameter param)
		{
			GenericParameter genericParameter = new GenericParameter(param.Name, param.Owner)
			{
				Attributes = param.Attributes
			}.Update(param.Position, param.Type);
			foreach (GenericParameterConstraint genericParameterConstraint in param.Constraints)
			{
				genericParameter.Constraints.Add(genericParameterConstraint);
			}
			return genericParameter;
		}

		public static int GetManagedSize(this Type t)
		{
			int num;
			if (Extensions._GetManagedSizeCache.TryGetValue(t, out num))
			{
				return num;
			}
			if (Extensions._GetManagedSizeHelper == null)
			{
				Assembly assembly;
				using (ModuleDefinition moduleDefinition = ModuleDefinition.CreateModule("MonoMod.Utils.GetManagedSizeHelper", new ModuleParameters
				{
					Kind = ModuleKind.Dll
				}))
				{
					TypeDefinition typeDefinition = new TypeDefinition("MonoMod.Utils", "GetManagedSizeHelper", Mono.Cecil.TypeAttributes.Public | Mono.Cecil.TypeAttributes.Abstract | Mono.Cecil.TypeAttributes.Sealed)
					{
						BaseType = moduleDefinition.TypeSystem.Object
					};
					moduleDefinition.Types.Add(typeDefinition);
					MethodDefinition methodDefinition = new MethodDefinition("GetManagedSizeHelper", Mono.Cecil.MethodAttributes.FamANDAssem | Mono.Cecil.MethodAttributes.Family | Mono.Cecil.MethodAttributes.Static | Mono.Cecil.MethodAttributes.HideBySig, moduleDefinition.TypeSystem.Int32);
					GenericParameter genericParameter = new GenericParameter("T", methodDefinition);
					methodDefinition.GenericParameters.Add(genericParameter);
					typeDefinition.Methods.Add(methodDefinition);
					ILProcessor ilprocessor = methodDefinition.Body.GetILProcessor();
					ilprocessor.Emit(Mono.Cecil.Cil.OpCodes.Sizeof, genericParameter);
					ilprocessor.Emit(Mono.Cecil.Cil.OpCodes.Ret);
					assembly = ReflectionHelper.Load(moduleDefinition);
				}
				Extensions._GetManagedSizeHelper = assembly.GetType("MonoMod.Utils.GetManagedSizeHelper").GetMethod("GetManagedSizeHelper");
			}
			num = Extensions._GetManagedSizeHelper.MakeGenericMethod(new Type[] { t }).CreateDelegate<Func<int>>()();
			Dictionary<Type, int> getManagedSizeCache = Extensions._GetManagedSizeCache;
			int num2;
			lock (getManagedSizeCache)
			{
				num2 = (Extensions._GetManagedSizeCache[t] = num);
				num2 = num2;
			}
			return num2;
		}

		public static Type GetThisParamType(this MethodBase method)
		{
			Type type = method.DeclaringType;
			if (type.IsValueType)
			{
				type = type.MakeByRefType();
			}
			return type;
		}

		public static IntPtr GetLdftnPointer(this MethodBase m)
		{
			Func<IntPtr> func;
			if (Extensions._GetLdftnPointerCache.TryGetValue(m, out func))
			{
				return func();
			}
			DynamicMethodDefinition dynamicMethodDefinition = new DynamicMethodDefinition("GetLdftnPointer<" + m.GetID(null, null, true, false, true) + ">", typeof(IntPtr), Type.EmptyTypes);
			ILProcessor ilprocessor = dynamicMethodDefinition.GetILProcessor();
			ilprocessor.Emit(Mono.Cecil.Cil.OpCodes.Ldftn, dynamicMethodDefinition.Definition.Module.ImportReference(m));
			ilprocessor.Emit(Mono.Cecil.Cil.OpCodes.Ret);
			Dictionary<MethodBase, Func<IntPtr>> getLdftnPointerCache = Extensions._GetLdftnPointerCache;
			IntPtr intPtr;
			lock (getLdftnPointerCache)
			{
				intPtr = (Extensions._GetLdftnPointerCache[m] = dynamicMethodDefinition.Generate().CreateDelegate<Func<IntPtr>>())();
			}
			return intPtr;
		}

		private static readonly Type t_Code = typeof(Code);

		private static readonly Type t_OpCodes = typeof(Mono.Cecil.Cil.OpCodes);

		private static readonly Dictionary<int, Mono.Cecil.Cil.OpCode> _ToLongOp = new Dictionary<int, Mono.Cecil.Cil.OpCode>();

		private static readonly Dictionary<int, Mono.Cecil.Cil.OpCode> _ToShortOp = new Dictionary<int, Mono.Cecil.Cil.OpCode>();

		private static readonly object[] _NoArgs = new object[0];

		private static readonly Dictionary<Type, FieldInfo> fmap_mono_assembly = new Dictionary<Type, FieldInfo>();

		private static readonly bool _MonoAssemblyNameHasArch = new AssemblyName("Dummy, ProcessorArchitecture=MSIL").ProcessorArchitecture == ProcessorArchitecture.MSIL;

		private static readonly Type t_ParamArrayAttribute = typeof(ParamArrayAttribute);

		private static readonly FieldInfo f_GenericParameter_position = typeof(GenericParameter).GetField("position", BindingFlags.Instance | BindingFlags.NonPublic);

		private static readonly FieldInfo f_GenericParameter_type = typeof(GenericParameter).GetField("type", BindingFlags.Instance | BindingFlags.NonPublic);

		private static readonly Dictionary<Type, int> _GetManagedSizeCache = new Dictionary<Type, int> { 
		{
			typeof(void),
			0
		} };

		private static MethodInfo _GetManagedSizeHelper;

		private static readonly Dictionary<MethodBase, Func<IntPtr>> _GetLdftnPointerCache = new Dictionary<MethodBase, Func<IntPtr>>();
	}
}
