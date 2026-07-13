using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using MonoMod.Logs;

namespace MonoMod.Utils
{
	[NullableContext(1)]
	[Nullable(0)]
	internal static class Extensions
	{
		[NullableContext(2)]
		public static TypeDefinition SafeResolve(this TypeReference r)
		{
			TypeDefinition typeDefinition;
			try
			{
				typeDefinition = ((r != null) ? r.Resolve() : null);
			}
			catch
			{
				typeDefinition = null;
			}
			return typeDefinition;
		}

		[NullableContext(2)]
		public static FieldDefinition SafeResolve(this FieldReference r)
		{
			FieldDefinition fieldDefinition;
			try
			{
				fieldDefinition = ((r != null) ? r.Resolve() : null);
			}
			catch
			{
				fieldDefinition = null;
			}
			return fieldDefinition;
		}

		[NullableContext(2)]
		public static MethodDefinition SafeResolve(this MethodReference r)
		{
			MethodDefinition methodDefinition;
			try
			{
				methodDefinition = ((r != null) ? r.Resolve() : null);
			}
			catch
			{
				methodDefinition = null;
			}
			return methodDefinition;
		}

		[NullableContext(2)]
		public static PropertyDefinition SafeResolve(this PropertyReference r)
		{
			PropertyDefinition propertyDefinition;
			try
			{
				propertyDefinition = ((r != null) ? r.Resolve() : null);
			}
			catch
			{
				propertyDefinition = null;
			}
			return propertyDefinition;
		}

		[return: Nullable(2)]
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
			Helpers.ThrowIfArgumentNull<Instruction>(instr, "instr");
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
			Helpers.ThrowIfArgumentNull<Instruction>(instr, "instr");
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

		public static bool IsBaseMethodCall(this Mono.Cecil.Cil.MethodBody body, [Nullable(2)] MethodReference called)
		{
			Helpers.ThrowIfArgumentNull<Mono.Cecil.Cil.MethodBody>(body, "body");
			MethodDefinition method = body.Method;
			if (called == null)
			{
				return false;
			}
			TypeReference typeReference = called.DeclaringType;
			for (;;)
			{
				TypeSpecification typeSpecification = typeReference as TypeSpecification;
				if (typeSpecification == null)
				{
					break;
				}
				typeReference = typeSpecification.ElementType;
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
						goto IL_0072;
					}
				}
				while (!(typeDefinition.GetPatchFullName() == patchFullName));
				flag = true;
				IL_0072:;
			}
			catch
			{
				flag = method.DeclaringType.GetPatchFullName() == patchFullName;
			}
			return flag;
		}

		public static bool IsCallvirt(this MethodReference method)
		{
			Helpers.ThrowIfArgumentNull<MethodReference>(method, "method");
			return method.HasThis && !method.DeclaringType.IsValueType;
		}

		public static bool IsStruct(this TypeReference type)
		{
			Helpers.ThrowIfArgumentNull<TypeReference>(type, "type");
			return type.IsValueType && !type.IsPrimitive;
		}

		public static Mono.Cecil.Cil.OpCode ToLongOp(this Mono.Cecil.Cil.OpCode op)
		{
			string name = Enum.GetName(Extensions.t_Code, op.Code);
			if (name == null || !name.EndsWith("_S", StringComparison.Ordinal))
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
					opCode2 = (toLongOp2[code] = ((Mono.Cecil.Cil.OpCode?)((field != null) ? field.GetValue(null) : null)).GetValueOrDefault(op));
				}
			}
			return opCode2;
		}

		public static Mono.Cecil.Cil.OpCode ToShortOp(this Mono.Cecil.Cil.OpCode op)
		{
			string name = Enum.GetName(Extensions.t_Code, op.Code);
			if (name == null || name.EndsWith("_S", StringComparison.Ordinal))
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
					opCode2 = (toShortOp2[code] = ((Mono.Cecil.Cil.OpCode?)((field != null) ? field.GetValue(null) : null)).GetValueOrDefault(op));
				}
			}
			return opCode2;
		}

		public static void RecalculateILOffsets(this MethodDefinition method)
		{
			Helpers.ThrowIfArgumentNull<MethodDefinition>(method, "method");
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
			Helpers.ThrowIfArgumentNull<MethodDefinition>(method, "method");
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

		[NullableContext(2)]
		public static bool Is(this MemberInfo minfo, MemberReference mref)
		{
			return mref.Is(minfo);
		}

		[NullableContext(2)]
		public static bool Is(this MemberReference mref, MemberInfo minfo)
		{
			if (mref == null)
			{
				return false;
			}
			if (minfo == null)
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
						string fullName = mref.FullName;
						string fullName2 = type3.FullName;
						return fullName == ((fullName2 != null) ? fullName2.Replace("+", "/", StringComparison.Ordinal) : null);
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
						Relinker relinker = delegate(IMetadataTokenProvider paramMemberRef, [Nullable(2)] IGenericParameterProvider ctx)
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
			Helpers.ThrowIfArgumentNull<ModuleDefinition>(mod, "mod");
			TypeReference typeReference = mtp as TypeReference;
			if (typeReference != null)
			{
				return mod.ImportReference(typeReference);
			}
			FieldReference fieldReference = mtp as FieldReference;
			if (fieldReference != null)
			{
				return mod.ImportReference(fieldReference);
			}
			MethodReference methodReference = mtp as MethodReference;
			if (methodReference != null)
			{
				return mod.ImportReference(methodReference);
			}
			Mono.Cecil.CallSite callSite = mtp as Mono.Cecil.CallSite;
			if (callSite != null)
			{
				return mod.ImportReference(callSite);
			}
			return mtp;
		}

		public static Mono.Cecil.CallSite ImportReference(this ModuleDefinition mod, Mono.Cecil.CallSite callsite)
		{
			Helpers.ThrowIfArgumentNull<ModuleDefinition>(mod, "mod");
			Helpers.ThrowIfArgumentNull<Mono.Cecil.CallSite>(callsite, "callsite");
			Mono.Cecil.CallSite callSite = new Mono.Cecil.CallSite(mod.ImportReference(callsite.ReturnType));
			callSite.CallingConvention = callsite.CallingConvention;
			callSite.ExplicitThis = callsite.ExplicitThis;
			callSite.HasThis = callsite.HasThis;
			foreach (ParameterDefinition parameterDefinition in callsite.Parameters)
			{
				ParameterDefinition parameterDefinition2 = new ParameterDefinition(mod.ImportReference(parameterDefinition.ParameterType))
				{
					Name = parameterDefinition.Name,
					Attributes = parameterDefinition.Attributes,
					Constant = parameterDefinition.Constant,
					MarshalInfo = parameterDefinition.MarshalInfo
				};
				callSite.Parameters.Add(parameterDefinition2);
			}
			return callSite;
		}

		public static void AddRange<[Nullable(2)] T>(this Collection<T> list, IEnumerable<T> other)
		{
			Helpers.ThrowIfArgumentNull<Collection<T>>(list, "list");
			foreach (T t in Helpers.ThrowIfNull<IEnumerable<T>>(other, "other"))
			{
				list.Add(t);
			}
		}

		public static void AddRange(this IDictionary dict, IDictionary other)
		{
			Helpers.ThrowIfArgumentNull<IDictionary>(dict, "dict");
			foreach (object obj in Helpers.ThrowIfNull<IDictionary>(other, "other"))
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
				dict.Add(dictionaryEntry.Key, dictionaryEntry.Value);
			}
		}

		public static void AddRange<[Nullable(2)] TKey, [Nullable(2)] TValue>(this IDictionary<TKey, TValue> dict, IDictionary<TKey, TValue> other)
		{
			Helpers.ThrowIfArgumentNull<IDictionary<TKey, TValue>>(dict, "dict");
			foreach (KeyValuePair<TKey, TValue> keyValuePair in Helpers.ThrowIfNull<IDictionary<TKey, TValue>>(other, "other"))
			{
				dict.Add(keyValuePair.Key, keyValuePair.Value);
			}
		}

		public static void AddRange<TKey, [Nullable(2)] TValue>(this Dictionary<TKey, TValue> dict, Dictionary<TKey, TValue> other)
		{
			Helpers.ThrowIfArgumentNull<Dictionary<TKey, TValue>>(dict, "dict");
			foreach (KeyValuePair<TKey, TValue> keyValuePair in Helpers.ThrowIfNull<Dictionary<TKey, TValue>>(other, "other"))
			{
				dict.Add(keyValuePair.Key, keyValuePair.Value);
			}
		}

		public static void InsertRange<[Nullable(2)] T>(this Collection<T> list, int index, IEnumerable<T> other)
		{
			Helpers.ThrowIfArgumentNull<Collection<T>>(list, "list");
			foreach (T t in Helpers.ThrowIfNull<IEnumerable<T>>(other, "other"))
			{
				list.Insert(index++, t);
			}
		}

		public static bool IsCompatible(this Type type, Type other)
		{
			return Helpers.ThrowIfNull<Type>(type, "type")._IsCompatible(Helpers.ThrowIfNull<Type>(other, "other")) || other._IsCompatible(type);
		}

		private static bool _IsCompatible(this Type type, Type other)
		{
			return type == other || ((!other.IsEnum || !(type == typeof(Enum))) && (!other.IsValueType || !(type == typeof(ValueType))) && (type.IsAssignableFrom(other) || (other.IsEnum && type.IsCompatible(Enum.GetUnderlyingType(other))) || ((other.IsPointer || other.IsByRef) && type == typeof(IntPtr)) || (type.IsPointer && other.IsPointer) || (type.IsByRef && other.IsPointer)));
		}

		public static T GetDeclaredMember<[Nullable(0)] T>(this T member) where T : MemberInfo
		{
			Helpers.ThrowIfArgumentNull<T>(member, "member");
			if (member.DeclaringType == member.ReflectedType)
			{
				return member;
			}
			if (member.DeclaringType != null)
			{
				int metadataToken = member.MetadataToken;
				foreach (MemberInfo memberInfo in member.DeclaringType.GetMembers((BindingFlags)(-1)))
				{
					if (memberInfo.MetadataToken == metadataToken)
					{
						return (T)((object)memberInfo);
					}
				}
			}
			return member;
		}

		public unsafe static void SetMonoCorlibInternal(this Assembly asm, bool value)
		{
			if (PlatformDetection.Runtime != RuntimeKind.Mono)
			{
				return;
			}
			Helpers.ThrowIfArgumentNull<Assembly>(asm, "asm");
			Type type = asm.GetType();
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
					FieldInfo fieldInfo2;
					if ((fieldInfo2 = type.GetField("_mono_assembly", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) == null && (fieldInfo2 = type.GetField("dynamic_assembly", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) == null)
					{
						throw new InvalidOperationException("Could not find assembly field for Mono");
					}
					fieldInfo = fieldInfo2;
					Extensions.fmap_mono_assembly[type] = fieldInfo;
				}
			}
			if (fieldInfo == null)
			{
				return;
			}
			AssemblyName name = asm.GetName();
			Dictionary<string, WeakReference> assemblyCache = ReflectionHelper.AssemblyCache;
			lock (assemblyCache)
			{
				WeakReference weakReference = new WeakReference(asm);
				ReflectionHelper.AssemblyCache[asm.GetRuntimeHashedFullName()] = weakReference;
				ReflectionHelper.AssemblyCache[name.FullName] = weakReference;
				if (name.Name != null)
				{
					ReflectionHelper.AssemblyCache[name.Name] = weakReference;
				}
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
			int num2 = IntPtr.Size + IntPtr.Size + IntPtr.Size + IntPtr.Size + IntPtr.Size + IntPtr.Size + 20 + 4 + 4 + 4 + ((!Extensions._MonoAssemblyNameHasArch) ? (ReflectionHelper.IsCoreBCL ? 16 : 8) : (ReflectionHelper.IsCoreBCL ? ((IntPtr.Size == 4) ? 20 : 24) : ((IntPtr.Size == 4) ? 12 : 16))) + IntPtr.Size + IntPtr.Size + 1 + 1 + 1;
			byte* ptr = num + num2;
			*ptr = ((value > false) ? 1 : 0);
		}

		public static bool IsDynamicMethod(this MethodBase method)
		{
			Helpers.ThrowIfArgumentNull<MethodBase>(method, "method");
			if (Extensions._RTDynamicMethod != null)
			{
				return method is DynamicMethod || method.GetType() == Extensions._RTDynamicMethod;
			}
			if (method is DynamicMethod)
			{
				return true;
			}
			if (method.MetadataToken != 0 || !method.IsStatic || !method.IsPublic || (method.Attributes & global::System.Reflection.MethodAttributes.PrivateScope) != global::System.Reflection.MethodAttributes.PrivateScope)
			{
				return false;
			}
			if (method.DeclaringType != null)
			{
				foreach (MethodInfo methodInfo in method.DeclaringType.GetMethods(BindingFlags.Static | BindingFlags.Public))
				{
					if (method == methodInfo)
					{
						return false;
					}
				}
			}
			return true;
		}

		[return: Nullable(2)]
		public static object SafeGetTarget(this WeakReference weak)
		{
			Helpers.ThrowIfArgumentNull<WeakReference>(weak, "weak");
			object obj;
			try
			{
				obj = weak.Target;
			}
			catch (InvalidOperationException)
			{
				obj = null;
			}
			return obj;
		}

		public static bool SafeGetIsAlive(this WeakReference weak)
		{
			Helpers.ThrowIfArgumentNull<WeakReference>(weak, "weak");
			bool flag;
			try
			{
				flag = weak.IsAlive;
			}
			catch (InvalidOperationException)
			{
				flag = false;
			}
			return flag;
		}

		public static T CreateDelegate<[Nullable(0)] T>(this MethodBase method) where T : Delegate
		{
			return (T)((object)method.CreateDelegate(typeof(T), null));
		}

		public static T CreateDelegate<[Nullable(0)] T>(this MethodBase method, [Nullable(2)] object target) where T : Delegate
		{
			return (T)((object)method.CreateDelegate(typeof(T), target));
		}

		public static Delegate CreateDelegate(this MethodBase method, Type delegateType)
		{
			return method.CreateDelegate(delegateType, null);
		}

		public static Delegate CreateDelegate(this MethodBase method, Type delegateType, [Nullable(2)] object target)
		{
			Helpers.ThrowIfArgumentNull<MethodBase>(method, "method");
			Helpers.ThrowIfArgumentNull<Type>(delegateType, "delegateType");
			if (!typeof(Delegate).IsAssignableFrom(delegateType))
			{
				throw new ArgumentException("Type argument must be a delegate type!");
			}
			DynamicMethod dynamicMethod = method as DynamicMethod;
			if (dynamicMethod != null)
			{
				return dynamicMethod.CreateDelegate(delegateType, target);
			}
			MethodInfo methodInfo = method as MethodInfo;
			if (methodInfo != null)
			{
				return Delegate.CreateDelegate(delegateType, target, methodInfo);
			}
			RuntimeMethodHandle methodHandle = method.MethodHandle;
			RuntimeHelpers.PrepareMethod(methodHandle);
			IntPtr functionPointer = methodHandle.GetFunctionPointer();
			return (Delegate)Activator.CreateInstance(delegateType, new object[] { target, functionPointer });
		}

		[NullableContext(2)]
		public static T TryCreateDelegate<[Nullable(0)] T>(this MethodInfo mi) where T : Delegate
		{
			T t;
			try
			{
				T t2;
				if (mi == null)
				{
					t = default(T);
					t2 = t;
				}
				else
				{
					t2 = mi.CreateDelegate<T>();
				}
				t = t2;
			}
			catch
			{
				t = default(T);
			}
			return t;
		}

		[return: Nullable(2)]
		public static MethodDefinition FindMethod(this TypeDefinition type, string id, bool simple = true)
		{
			Helpers.ThrowIfArgumentNull<TypeDefinition>(type, "type");
			Helpers.ThrowIfArgumentNull<string>(id, "id");
			if (simple && !id.Contains(' ', StringComparison.Ordinal))
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

		[return: Nullable(2)]
		public static MethodDefinition FindMethodDeep(this TypeDefinition type, string id, bool simple = true)
		{
			MethodDefinition methodDefinition;
			if ((methodDefinition = Helpers.ThrowIfNull<TypeDefinition>(type, "type").FindMethod(id, simple)) == null)
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

		[return: Nullable(2)]
		public static MethodInfo FindMethod(this Type type, string id, bool simple = true)
		{
			Helpers.ThrowIfArgumentNull<Type>(type, "type");
			Helpers.ThrowIfArgumentNull<string>(id, "id");
			MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (simple && !id.Contains(' ', StringComparison.Ordinal))
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

		[return: Nullable(2)]
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

		[return: Nullable(2)]
		public static PropertyDefinition FindProperty(this TypeDefinition type, string name)
		{
			Helpers.ThrowIfArgumentNull<TypeDefinition>(type, "type");
			foreach (PropertyDefinition propertyDefinition in type.Properties)
			{
				if (propertyDefinition.Name == name)
				{
					return propertyDefinition;
				}
			}
			return null;
		}

		[return: Nullable(2)]
		public static PropertyDefinition FindPropertyDeep(this TypeDefinition type, string name)
		{
			Helpers.ThrowIfArgumentNull<TypeDefinition>(type, "type");
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

		[return: Nullable(2)]
		public static FieldDefinition FindField(this TypeDefinition type, string name)
		{
			Helpers.ThrowIfArgumentNull<TypeDefinition>(type, "type");
			foreach (FieldDefinition fieldDefinition in type.Fields)
			{
				if (fieldDefinition.Name == name)
				{
					return fieldDefinition;
				}
			}
			return null;
		}

		[return: Nullable(2)]
		public static FieldDefinition FindFieldDeep(this TypeDefinition type, string name)
		{
			Helpers.ThrowIfArgumentNull<TypeDefinition>(type, "type");
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

		[return: Nullable(2)]
		public static EventDefinition FindEvent(this TypeDefinition type, string name)
		{
			Helpers.ThrowIfArgumentNull<TypeDefinition>(type, "type");
			foreach (EventDefinition eventDefinition in type.Events)
			{
				if (eventDefinition.Name == name)
				{
					return eventDefinition;
				}
			}
			return null;
		}

		[return: Nullable(2)]
		public static EventDefinition FindEventDeep(this TypeDefinition type, string name)
		{
			Helpers.ThrowIfArgumentNull<TypeDefinition>(type, "type");
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

		public static string GetID(this MethodReference method, [Nullable(2)] string name = null, [Nullable(2)] string type = null, bool withType = true, bool simple = false)
		{
			Helpers.ThrowIfArgumentNull<MethodReference>(method, "method");
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
			stringBuilder.Append(method.ReturnType.GetPatchFullName()).Append(' ');
			if (withType && (type != null || method.DeclaringType != null))
			{
				stringBuilder.Append(type ?? method.DeclaringType.GetPatchFullName()).Append("::");
			}
			stringBuilder.Append(name ?? method.Name);
			GenericInstanceMethod genericInstanceMethod = method as GenericInstanceMethod;
			if (genericInstanceMethod != null && genericInstanceMethod.GenericArguments.Count != 0)
			{
				stringBuilder.Append('<');
				Collection<TypeReference> genericArguments = genericInstanceMethod.GenericArguments;
				for (int i = 0; i < genericArguments.Count; i++)
				{
					if (i > 0)
					{
						stringBuilder.Append(',');
					}
					stringBuilder.Append(genericArguments[i].GetPatchFullName());
				}
				stringBuilder.Append('>');
			}
			else if (method.GenericParameters.Count != 0)
			{
				stringBuilder.Append('<');
				Collection<GenericParameter> genericParameters = method.GenericParameters;
				for (int j = 0; j < genericParameters.Count; j++)
				{
					if (j > 0)
					{
						stringBuilder.Append(',');
					}
					stringBuilder.Append(genericParameters[j].Name);
				}
				stringBuilder.Append('>');
			}
			stringBuilder.Append('(');
			if (method.HasParameters)
			{
				Collection<ParameterDefinition> parameters = method.Parameters;
				for (int k = 0; k < parameters.Count; k++)
				{
					ParameterDefinition parameterDefinition = parameters[k];
					if (k > 0)
					{
						stringBuilder.Append(',');
					}
					if (parameterDefinition.ParameterType.IsSentinel)
					{
						stringBuilder.Append("...,");
					}
					stringBuilder.Append(parameterDefinition.ParameterType.GetPatchFullName());
				}
			}
			stringBuilder.Append(')');
			return stringBuilder.ToString();
		}

		public static string GetID(this Mono.Cecil.CallSite method)
		{
			Helpers.ThrowIfArgumentNull<Mono.Cecil.CallSite>(method, "method");
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(method.ReturnType.GetPatchFullName()).Append(' ');
			stringBuilder.Append('(');
			if (method.HasParameters)
			{
				Collection<ParameterDefinition> parameters = method.Parameters;
				for (int i = 0; i < parameters.Count; i++)
				{
					ParameterDefinition parameterDefinition = parameters[i];
					if (i > 0)
					{
						stringBuilder.Append(',');
					}
					if (parameterDefinition.ParameterType.IsSentinel)
					{
						stringBuilder.Append("...,");
					}
					stringBuilder.Append(parameterDefinition.ParameterType.GetPatchFullName());
				}
			}
			stringBuilder.Append(')');
			return stringBuilder.ToString();
		}

		public static string GetID(this MethodBase method, [Nullable(2)] string name = null, [Nullable(2)] string type = null, bool withType = true, bool proxyMethod = false, bool simple = false)
		{
			Helpers.ThrowIfArgumentNull<MethodBase>(method, "method");
			for (;;)
			{
				MethodInfo methodInfo = method as MethodInfo;
				if (methodInfo == null || !method.IsGenericMethod || method.IsGenericMethodDefinition)
				{
					break;
				}
				method = methodInfo.GetGenericMethodDefinition();
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
			MethodInfo methodInfo2 = method as MethodInfo;
			string text;
			if (methodInfo2 == null)
			{
				text = null;
			}
			else
			{
				Type returnType = methodInfo2.ReturnType;
				text = ((returnType != null) ? returnType.FullName : null);
			}
			stringBuilder2.Append(text ?? "System.Void").Append(' ');
			if (withType && (type != null || method.DeclaringType != null))
			{
				StringBuilder stringBuilder3 = stringBuilder;
				string text2 = type;
				if (type == null)
				{
					string fullName = method.DeclaringType.FullName;
					text2 = ((fullName != null) ? fullName.Replace("+", "/", StringComparison.Ordinal) : null);
				}
				stringBuilder3.Append(text2).Append("::");
			}
			stringBuilder.Append(name ?? method.Name);
			if (method.ContainsGenericParameters)
			{
				stringBuilder.Append('<');
				Type[] genericArguments = method.GetGenericArguments();
				for (int i = 0; i < genericArguments.Length; i++)
				{
					if (i > 0)
					{
						stringBuilder.Append(',');
					}
					stringBuilder.Append(genericArguments[i].Name);
				}
				stringBuilder.Append('>');
			}
			stringBuilder.Append('(');
			ParameterInfo[] parameters = method.GetParameters();
			for (int j = ((proxyMethod > false) ? 1 : 0); j < parameters.Length; j++)
			{
				ParameterInfo parameterInfo = parameters[j];
				if (j > ((proxyMethod > false) ? 1 : 0))
				{
					stringBuilder.Append(',');
				}
				bool flag;
				try
				{
					flag = parameterInfo.GetCustomAttributes(Extensions.t_ParamArrayAttribute, false).Length != 0;
				}
				catch (NotSupportedException)
				{
					flag = false;
				}
				if (flag)
				{
					stringBuilder.Append("...,");
				}
				stringBuilder.Append(parameterInfo.ParameterType.FullName);
			}
			stringBuilder.Append(')');
			return stringBuilder.ToString();
		}

		public static string GetPatchName(this MemberReference mr)
		{
			Helpers.ThrowIfArgumentNull<MemberReference>(mr, "mr");
			Mono.Cecil.ICustomAttributeProvider customAttributeProvider = mr as Mono.Cecil.ICustomAttributeProvider;
			return ((customAttributeProvider != null) ? customAttributeProvider.GetPatchName() : null) ?? mr.Name;
		}

		public static string GetPatchFullName(this MemberReference mr)
		{
			Helpers.ThrowIfArgumentNull<MemberReference>(mr, "mr");
			Mono.Cecil.ICustomAttributeProvider customAttributeProvider = mr as Mono.Cecil.ICustomAttributeProvider;
			return ((customAttributeProvider != null) ? customAttributeProvider.GetPatchFullName(mr) : null) ?? mr.FullName;
		}

		private static string GetPatchName(this Mono.Cecil.ICustomAttributeProvider cap)
		{
			Helpers.ThrowIfArgumentNull<Mono.Cecil.ICustomAttributeProvider>(cap, "cap");
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
			if (!text.StartsWith("patch_", StringComparison.Ordinal))
			{
				return text;
			}
			return text.Substring(6);
		}

		private static string GetPatchFullName(this Mono.Cecil.ICustomAttributeProvider cap, MemberReference mr)
		{
			Helpers.ThrowIfArgumentNull<Mono.Cecil.ICustomAttributeProvider>(cap, "cap");
			Helpers.ThrowIfArgumentNull<MemberReference>(mr, "mr");
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
					text = (text.StartsWith("patch_", StringComparison.Ordinal) ? text.Substring(6) : text);
				}
				if (text.StartsWith("global::", StringComparison.Ordinal))
				{
					text = text.Substring(8);
				}
				else if (!text.Contains('.', StringComparison.Ordinal) && !text.Contains('/', StringComparison.Ordinal))
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
				TypeSpecification typeSpecification = mr as TypeSpecification;
				if (typeSpecification != null)
				{
					List<TypeSpecification> list = new List<TypeSpecification>();
					TypeSpecification typeSpecification2 = typeSpecification;
					do
					{
						list.Add(typeSpecification2);
					}
					while ((typeSpecification2 = typeSpecification2.ElementType as TypeSpecification) != null);
					StringBuilder stringBuilder = new StringBuilder(text.Length + list.Count * 4);
					stringBuilder.Append(text);
					for (int i = list.Count - 1; i > -1; i--)
					{
						typeSpecification2 = list[i];
						if (typeSpecification2.IsByReference)
						{
							stringBuilder.Append('&');
						}
						else if (typeSpecification2.IsPointer)
						{
							stringBuilder.Append('*');
						}
						else if (!typeSpecification2.IsPinned && !typeSpecification2.IsSentinel)
						{
							if (typeSpecification2.IsArray)
							{
								ArrayType arrayType = (ArrayType)typeSpecification2;
								if (arrayType.IsVector)
								{
									stringBuilder.Append("[]");
								}
								else
								{
									stringBuilder.Append('[');
									for (int j = 0; j < arrayType.Dimensions.Count; j++)
									{
										if (j > 0)
										{
											stringBuilder.Append(',');
										}
										stringBuilder.Append(arrayType.Dimensions[j].ToString());
									}
									stringBuilder.Append(']');
								}
							}
							else if (typeSpecification2.IsRequiredModifier)
							{
								stringBuilder.Append("modreq(").Append(((RequiredModifierType)typeSpecification2).ModifierType).Append(')');
							}
							else if (typeSpecification2.IsOptionalModifier)
							{
								stringBuilder.Append("modopt(").Append(((OptionalModifierType)typeSpecification2).ModifierType).Append(')');
							}
							else if (typeSpecification2.IsGenericInstance)
							{
								GenericInstanceType genericInstanceType = (GenericInstanceType)typeSpecification2;
								stringBuilder.Append('<');
								for (int k = 0; k < genericInstanceType.GenericArguments.Count; k++)
								{
									if (k > 0)
									{
										stringBuilder.Append(',');
									}
									stringBuilder.Append(genericInstanceType.GenericArguments[k].GetPatchFullName());
								}
								stringBuilder.Append('>');
							}
							else
							{
								if (!typeSpecification2.IsFunctionPointer)
								{
									DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(43, 2);
									defaultInterpolatedStringHandler.AppendLiteral("MonoMod can't handle TypeSpecification: ");
									defaultInterpolatedStringHandler.AppendFormatted(typeReference.FullName);
									defaultInterpolatedStringHandler.AppendLiteral(" (");
									defaultInterpolatedStringHandler.AppendFormatted<Type>(typeReference.GetType());
									defaultInterpolatedStringHandler.AppendLiteral(")");
									throw new NotSupportedException(defaultInterpolatedStringHandler.ToStringAndClear());
								}
								FunctionPointerType functionPointerType = (FunctionPointerType)typeSpecification2;
								stringBuilder.Append(' ').Append(functionPointerType.ReturnType.GetPatchFullName()).Append(" *(");
								if (functionPointerType.HasParameters)
								{
									for (int l = 0; l < functionPointerType.Parameters.Count; l++)
									{
										ParameterDefinition parameterDefinition = functionPointerType.Parameters[l];
										if (l > 0)
										{
											stringBuilder.Append(',');
										}
										if (parameterDefinition.ParameterType.IsSentinel)
										{
											stringBuilder.Append("...,");
										}
										stringBuilder.Append(parameterDefinition.ParameterType.FullName);
									}
								}
								stringBuilder.Append(')');
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
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(3, 3);
				defaultInterpolatedStringHandler2.AppendFormatted(fieldReference.FieldType.GetPatchFullName());
				defaultInterpolatedStringHandler2.AppendLiteral(" ");
				defaultInterpolatedStringHandler2.AppendFormatted(fieldReference.DeclaringType.GetPatchFullName());
				defaultInterpolatedStringHandler2.AppendLiteral("::");
				defaultInterpolatedStringHandler2.AppendFormatted(cap.GetPatchName());
				return defaultInterpolatedStringHandler2.ToStringAndClear();
			}
			if (cap is MethodReference)
			{
				throw new InvalidOperationException("GetPatchFullName not supported on MethodReferences - use GetID instead");
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(39, 1);
			defaultInterpolatedStringHandler3.AppendLiteral("GetPatchFullName not supported on type ");
			defaultInterpolatedStringHandler3.AppendFormatted<Type>(cap.GetType());
			throw new InvalidOperationException(defaultInterpolatedStringHandler3.ToStringAndClear());
		}

		[NullableContext(2)]
		[return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("o")]
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
			MethodDefinition methodDefinition = c;
			Mono.Cecil.Cil.MethodBody body = o.Body;
			methodDefinition.Body = ((body != null) ? body.Clone(c) : null);
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
				c.Parameters.Add(parameterDefinition.Clone());
			}
			foreach (CustomAttribute customAttribute in o.CustomAttributes)
			{
				c.CustomAttributes.Add(customAttribute.Clone());
			}
			foreach (MethodReference methodReference in o.Overrides)
			{
				c.Overrides.Add(methodReference);
			}
			if (c.Body != null)
			{
				foreach (Instruction instruction in c.Body.Instructions)
				{
					GenericParameter genericParameter2 = instruction.Operand as GenericParameter;
					int num;
					if (genericParameter2 != null && (num = o.GenericParameters.IndexOf(genericParameter2)) != -1)
					{
						instruction.Operand = c.GenericParameters[num];
					}
					else
					{
						ParameterDefinition parameterDefinition2 = instruction.Operand as ParameterDefinition;
						if (parameterDefinition2 != null && (num = o.Parameters.IndexOf(parameterDefinition2)) != -1)
						{
							instruction.Operand = c.Parameters[num];
						}
					}
				}
			}
			return c;
		}

		[NullableContext(2)]
		[return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("bo")]
		public static Mono.Cecil.Cil.MethodBody Clone(this Mono.Cecil.Cil.MethodBody bo, [Nullable(1)] MethodDefinition m)
		{
			Helpers.ThrowIfArgumentNull<MethodDefinition>(m, "m");
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
			Func<InstructionOffset, InstructionOffset> <>9__6;
			Func<InstructionOffset, InstructionOffset> <>9__7;
			Func<StateMachineScope, StateMachineScope> <>9__8;
			m.CustomDebugInformations.AddRange<CustomDebugInformation>(bo.Method.CustomDebugInformations.Select<CustomDebugInformation, CustomDebugInformation>(delegate(CustomDebugInformation o)
			{
				AsyncMethodBodyDebugInformation asyncMethodBodyDebugInformation = o as AsyncMethodBodyDebugInformation;
				if (asyncMethodBodyDebugInformation != null)
				{
					AsyncMethodBodyDebugInformation asyncMethodBodyDebugInformation2 = new AsyncMethodBodyDebugInformation();
					if (asyncMethodBodyDebugInformation.CatchHandler.Offset >= 0)
					{
						asyncMethodBodyDebugInformation2.CatchHandler = (asyncMethodBodyDebugInformation.CatchHandler.IsEndOfMethod ? default(InstructionOffset) : new InstructionOffset(base.<Clone>g__ResolveInstrOff|3(asyncMethodBodyDebugInformation.CatchHandler.Offset)));
					}
					Collection<InstructionOffset> yields = asyncMethodBodyDebugInformation2.Yields;
					IEnumerable<InstructionOffset> yields2 = asyncMethodBodyDebugInformation.Yields;
					Func<InstructionOffset, InstructionOffset> func2;
					if ((func2 = <>9__6) == null)
					{
						func2 = (<>9__6 = delegate(InstructionOffset off)
						{
							if (!off.IsEndOfMethod)
							{
								return new InstructionOffset(base.<Clone>g__ResolveInstrOff|3(off.Offset));
							}
							return default(InstructionOffset);
						});
					}
					yields.AddRange<InstructionOffset>(yields2.Select<InstructionOffset, InstructionOffset>(func2));
					Collection<InstructionOffset> resumes = asyncMethodBodyDebugInformation2.Resumes;
					IEnumerable<InstructionOffset> resumes2 = asyncMethodBodyDebugInformation.Resumes;
					Func<InstructionOffset, InstructionOffset> func3;
					if ((func3 = <>9__7) == null)
					{
						func3 = (<>9__7 = delegate(InstructionOffset off)
						{
							if (!off.IsEndOfMethod)
							{
								return new InstructionOffset(base.<Clone>g__ResolveInstrOff|3(off.Offset));
							}
							return default(InstructionOffset);
						});
					}
					resumes.AddRange<InstructionOffset>(resumes2.Select<InstructionOffset, InstructionOffset>(func3));
					asyncMethodBodyDebugInformation2.ResumeMethods.AddRange<MethodDefinition>(asyncMethodBodyDebugInformation.ResumeMethods);
					return asyncMethodBodyDebugInformation2;
				}
				StateMachineScopeDebugInformation stateMachineScopeDebugInformation = o as StateMachineScopeDebugInformation;
				if (stateMachineScopeDebugInformation != null)
				{
					StateMachineScopeDebugInformation stateMachineScopeDebugInformation2 = new StateMachineScopeDebugInformation();
					Collection<StateMachineScope> scopes = stateMachineScopeDebugInformation2.Scopes;
					IEnumerable<StateMachineScope> scopes2 = stateMachineScopeDebugInformation.Scopes;
					Func<StateMachineScope, StateMachineScope> func4;
					if ((func4 = <>9__8) == null)
					{
						func4 = (<>9__8 = (StateMachineScope s) => new StateMachineScope(base.<Clone>g__ResolveInstrOff|3(s.Start.Offset), s.End.IsEndOfMethod ? null : base.<Clone>g__ResolveInstrOff|3(s.End.Offset)));
					}
					scopes.AddRange<StateMachineScope>(scopes2.Select<StateMachineScope, StateMachineScope>(func4));
					return stateMachineScopeDebugInformation2;
				}
				return o;
			}));
			m.DebugInformation.SequencePoints.AddRange<SequencePoint>(bo.Method.DebugInformation.SequencePoints.Select<SequencePoint, SequencePoint>((SequencePoint o) => new SequencePoint(base.<Clone>g__ResolveInstrOff|3(o.Offset), o.Document)
			{
				StartLine = o.StartLine,
				StartColumn = o.StartColumn,
				EndLine = o.EndLine,
				EndColumn = o.EndColumn
			}));
			Func<Instruction, Instruction> <>9__9;
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
						if ((func = <>9__9) == null)
						{
							func = (<>9__9 = (Instruction i) => bc.Instructions[bo.Instructions.IndexOf(i)]);
						}
						instruction3.Operand = enumerable.Select<Instruction, Instruction>(func).ToArray<Instruction>();
					}
					else
					{
						VariableDefinition variableDefinition = instruction.Operand as VariableDefinition;
						if (variableDefinition != null)
						{
							instruction.Operand = bc.Variables[variableDefinition.Index];
						}
					}
				}
			}
			return bc;
		}

		public static GenericParameter Update(this GenericParameter param, int position, GenericParameterType type)
		{
			Extensions.f_GenericParameter_position.SetValue(param, position);
			Extensions.f_GenericParameter_type.SetValue(param, type);
			return param;
		}

		[return: Nullable(2)]
		public static GenericParameter ResolveGenericParameter(this IGenericParameterProvider provider, GenericParameter orig)
		{
			Helpers.ThrowIfArgumentNull<IGenericParameterProvider>(provider, "provider");
			Helpers.ThrowIfArgumentNull<GenericParameter>(orig, "orig");
			GenericParameter genericParameter = provider as GenericParameter;
			if (genericParameter != null && genericParameter.Name == orig.Name)
			{
				return genericParameter;
			}
			foreach (GenericParameter genericParameter2 in provider.GenericParameters)
			{
				if (genericParameter2.Name == orig.Name)
				{
					return genericParameter2;
				}
			}
			int position = orig.Position;
			if (provider is MethodReference && orig.DeclaringMethod != null)
			{
				if (position < provider.GenericParameters.Count)
				{
					return provider.GenericParameters[position];
				}
				return orig.Clone().Update(position, GenericParameterType.Method);
			}
			else
			{
				if (!(provider is TypeReference) || orig.DeclaringType == null)
				{
					TypeSpecification typeSpecification = provider as TypeSpecification;
					GenericParameter genericParameter3;
					if ((genericParameter3 = ((typeSpecification != null) ? typeSpecification.ElementType.ResolveGenericParameter(orig) : null)) == null)
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
						genericParameter3 = declaringType.ResolveGenericParameter(orig);
					}
					return genericParameter3;
				}
				if (position < provider.GenericParameters.Count)
				{
					return provider.GenericParameters[position];
				}
				return orig.Clone().Update(position, GenericParameterType.Type);
			}
			GenericParameter genericParameter4;
			return genericParameter4;
		}

		[return: Nullable(2)]
		[return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("mtp")]
		public static IMetadataTokenProvider Relink([Nullable(2)] this IMetadataTokenProvider mtp, Relinker relinker, IGenericParameterProvider context)
		{
			TypeReference typeReference = mtp as TypeReference;
			IMetadataTokenProvider metadataTokenProvider;
			if (typeReference == null)
			{
				GenericParameterConstraint genericParameterConstraint = mtp as GenericParameterConstraint;
				if (genericParameterConstraint == null)
				{
					MethodReference methodReference = mtp as MethodReference;
					if (methodReference == null)
					{
						FieldReference fieldReference = mtp as FieldReference;
						if (fieldReference == null)
						{
							ParameterDefinition parameterDefinition = mtp as ParameterDefinition;
							if (parameterDefinition == null)
							{
								Mono.Cecil.CallSite callSite = mtp as Mono.Cecil.CallSite;
								if (callSite == null)
								{
									if (mtp != null)
									{
										DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(58, 1);
										defaultInterpolatedStringHandler.AppendLiteral("MonoMod can't handle metadata token providers of the type ");
										defaultInterpolatedStringHandler.AppendFormatted<Type>(mtp.GetType());
										throw new InvalidOperationException(defaultInterpolatedStringHandler.ToStringAndClear());
									}
									metadataTokenProvider = null;
								}
								else
								{
									metadataTokenProvider = callSite.Relink(relinker, context);
								}
							}
							else
							{
								metadataTokenProvider = parameterDefinition.Relink(relinker, context);
							}
						}
						else
						{
							metadataTokenProvider = fieldReference.Relink(relinker, context);
						}
					}
					else
					{
						metadataTokenProvider = methodReference.Relink(relinker, context);
					}
				}
				else
				{
					metadataTokenProvider = genericParameterConstraint.Relink(relinker, context);
				}
			}
			else
			{
				metadataTokenProvider = typeReference.Relink(relinker, context);
			}
			return metadataTokenProvider;
		}

		[NullableContext(2)]
		[return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("type")]
		public static TypeReference Relink(this TypeReference type, [Nullable(1)] Relinker relinker, IGenericParameterProvider context)
		{
			if (type == null)
			{
				return null;
			}
			Helpers.ThrowIfArgumentNull<Relinker>(relinker, "relinker");
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
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(43, 2);
				defaultInterpolatedStringHandler.AppendLiteral("MonoMod can't handle TypeSpecification: ");
				defaultInterpolatedStringHandler.AppendFormatted(type.FullName);
				defaultInterpolatedStringHandler.AppendLiteral(" (");
				defaultInterpolatedStringHandler.AppendFormatted<Type>(type.GetType());
				defaultInterpolatedStringHandler.AppendLiteral(")");
				throw new NotSupportedException(defaultInterpolatedStringHandler.ToStringAndClear());
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
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(13, 3);
					defaultInterpolatedStringHandler2.AppendFormatted("MonoMod relinker failed finding");
					defaultInterpolatedStringHandler2.AppendLiteral(" ");
					defaultInterpolatedStringHandler2.AppendFormatted(type.FullName);
					defaultInterpolatedStringHandler2.AppendLiteral(" (context: ");
					defaultInterpolatedStringHandler2.AppendFormatted<IGenericParameterProvider>(context);
					defaultInterpolatedStringHandler2.AppendLiteral(")");
					throw new RelinkTargetNotFoundException(defaultInterpolatedStringHandler2.ToStringAndClear(), type, context);
				}
				GenericParameter genericParameter2 = genericParameter;
				for (int k = 0; k < genericParameter2.Constraints.Count; k++)
				{
					if (!genericParameter2.Constraints[k].GetConstraintType().IsGenericInstance)
					{
						genericParameter2.Constraints[k] = genericParameter2.Constraints[k].Relink(relinker, context);
					}
				}
				return genericParameter2;
			}
		}

		[return: Nullable(2)]
		[return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("constraint")]
		public static GenericParameterConstraint Relink([Nullable(2)] this GenericParameterConstraint constraint, Relinker relinker, IGenericParameterProvider context)
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
			Helpers.ThrowIfArgumentNull<MethodReference>(method, "method");
			Helpers.ThrowIfArgumentNull<Relinker>(relinker, "relinker");
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
				methodReference.GenericParameters.Add(genericParameter.Relink(relinker, context));
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
			Helpers.ThrowIfArgumentNull<Mono.Cecil.CallSite>(method, "method");
			Helpers.ThrowIfArgumentNull<Relinker>(relinker, "relinker");
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
			Helpers.ThrowIfArgumentNull<FieldReference>(field, "field");
			Helpers.ThrowIfArgumentNull<Relinker>(relinker, "relinker");
			TypeReference typeReference = field.DeclaringType.Relink(relinker, context);
			return relinker(new FieldReference(field.Name, field.FieldType.Relink(relinker, typeReference), typeReference), context);
		}

		public static ParameterDefinition Relink(this ParameterDefinition param, Relinker relinker, IGenericParameterProvider context)
		{
			Helpers.ThrowIfArgumentNull<ParameterDefinition>(param, "param");
			Helpers.ThrowIfArgumentNull<Relinker>(relinker, "relinker");
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
			Helpers.ThrowIfArgumentNull<ParameterDefinition>(param, "param");
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
			Helpers.ThrowIfArgumentNull<CustomAttribute>(attrib, "attrib");
			Helpers.ThrowIfArgumentNull<Relinker>(relinker, "relinker");
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
			Helpers.ThrowIfArgumentNull<CustomAttribute>(attrib, "attrib");
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
			Helpers.ThrowIfArgumentNull<GenericParameter>(param, "param");
			Helpers.ThrowIfArgumentNull<Relinker>(relinker, "relinker");
			GenericParameter genericParameter = new GenericParameter(param.Name, param.Owner)
			{
				Attributes = param.Attributes
			}.Update(param.Position, param.Type);
			foreach (CustomAttribute customAttribute in param.CustomAttributes)
			{
				genericParameter.CustomAttributes.Add(customAttribute.Relink(relinker, context));
			}
			foreach (GenericParameterConstraint genericParameterConstraint in param.Constraints)
			{
				genericParameter.Constraints.Add(genericParameterConstraint.Relink(relinker, context));
			}
			return genericParameter;
		}

		public static GenericParameter Clone(this GenericParameter param)
		{
			Helpers.ThrowIfArgumentNull<GenericParameter>(param, "param");
			GenericParameter genericParameter = new GenericParameter(param.Name, param.Owner)
			{
				Attributes = param.Attributes
			}.Update(param.Position, param.Type);
			foreach (CustomAttribute customAttribute in param.CustomAttributes)
			{
				genericParameter.CustomAttributes.Add(customAttribute.Clone());
			}
			foreach (GenericParameterConstraint genericParameterConstraint in param.Constraints)
			{
				genericParameter.Constraints.Add(genericParameterConstraint);
			}
			return genericParameter;
		}

		public static int GetManagedSize(this Type t)
		{
			if (!Helpers.ThrowIfNull<Type>(t, "t").IsByRef && !t.IsPointer)
			{
				ConcurrentDictionary<Type, int> getManagedSizeCache = Extensions._GetManagedSizeCache;
				Type type = Helpers.ThrowIfNull<Type>(t, "t");
				Func<Type, int> func;
				if ((func = Extensions.<>O.<0>__ComputeManagedSize) == null)
				{
					func = (Extensions.<>O.<0>__ComputeManagedSize = new Func<Type, int>(Extensions.ComputeManagedSize));
				}
				return getManagedSizeCache.GetOrAdd(type, func);
			}
			return IntPtr.Size;
		}

		private static int ComputeManagedSize(Type t)
		{
			MethodInfo methodInfo = Extensions._GetManagedSizeHelper;
			if (methodInfo == null)
			{
				methodInfo = (Extensions._GetManagedSizeHelper = typeof(Unsafe).GetMethod("SizeOf"));
			}
			if (t.IsByRef || t.IsPointer || t.IsByRefLike())
			{
				return Extensions.GenerateAndInvokeSizeofHelper(t);
			}
			return methodInfo.MakeGenericMethod(new Type[] { t }).CreateDelegate<Func<int>>()();
		}

		private static int GenerateAndInvokeSizeofHelper(Type t)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(8, 1);
			defaultInterpolatedStringHandler.AppendLiteral("SizeOf<");
			defaultInterpolatedStringHandler.AppendFormatted<Type>(t);
			defaultInterpolatedStringHandler.AppendLiteral(">");
			int num;
			using (DynamicMethodDefinition dynamicMethodDefinition = new DynamicMethodDefinition(defaultInterpolatedStringHandler.ToStringAndClear(), typeof(int), new Type[0]))
			{
				ILProcessor ilprocessor = dynamicMethodDefinition.GetILProcessor();
				ilprocessor.Emit(Mono.Cecil.Cil.OpCodes.Sizeof, ilprocessor.Import(t));
				ilprocessor.Emit(Mono.Cecil.Cil.OpCodes.Ret);
				num = (int)dynamicMethodDefinition.Generate().Invoke(null, null);
			}
			return num;
		}

		public static Type GetThisParamType(this MethodBase method)
		{
			Type type = Helpers.ThrowIfNull<MethodBase>(method, "method").DeclaringType;
			if (type.IsValueType)
			{
				type = type.MakeByRefType();
			}
			return type;
		}

		public static IntPtr GetLdftnPointer(this MethodBase m)
		{
			Helpers.ThrowIfArgumentNull<MethodBase>(m, "m");
			Func<IntPtr> func;
			if (Extensions._GetLdftnPointerCache.TryGetValue(m, out func))
			{
				return func();
			}
			FormatInterpolatedStringHandler formatInterpolatedStringHandler = new FormatInterpolatedStringHandler(17, 1);
			formatInterpolatedStringHandler.AppendLiteral("GetLdftnPointer<");
			formatInterpolatedStringHandler.AppendFormatted<MethodBase>(m);
			formatInterpolatedStringHandler.AppendLiteral(">");
			IntPtr intPtr;
			using (DynamicMethodDefinition dynamicMethodDefinition = new DynamicMethodDefinition(DebugFormatter.Format(ref formatInterpolatedStringHandler), typeof(IntPtr), Type.EmptyTypes))
			{
				ILProcessor ilprocessor = dynamicMethodDefinition.GetILProcessor();
				ilprocessor.Emit(Mono.Cecil.Cil.OpCodes.Ldftn, dynamicMethodDefinition.Definition.Module.ImportReference(m));
				ilprocessor.Emit(Mono.Cecil.Cil.OpCodes.Ret);
				Dictionary<MethodBase, Func<IntPtr>> getLdftnPointerCache = Extensions._GetLdftnPointerCache;
				lock (getLdftnPointerCache)
				{
					intPtr = (Extensions._GetLdftnPointerCache[m] = dynamicMethodDefinition.Generate().CreateDelegate<Func<IntPtr>>())();
				}
			}
			return intPtr;
		}

		public static string ToHexadecimalString(this byte[] data)
		{
			return BitConverter.ToString(data).Replace("-", string.Empty, StringComparison.Ordinal);
		}

		[return: Nullable(2)]
		public static T InvokePassing<[Nullable(2)] T>(this MulticastDelegate md, T val, [Nullable(new byte[] { 1, 2 })] params object[] args)
		{
			if (md == null)
			{
				return val;
			}
			Helpers.ThrowIfArgumentNull<object[]>(args, "args");
			object[] array = new object[args.Length + 1];
			array[0] = val;
			Array.Copy(args, 0, array, 1, args.Length);
			Delegate[] invocationList = md.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				array[0] = invocationList[i].DynamicInvoke(array);
			}
			return (T)((object)array[0]);
		}

		public static bool InvokeWhileTrue(this MulticastDelegate md, params object[] args)
		{
			if (md == null)
			{
				return true;
			}
			Helpers.ThrowIfArgumentNull<object[]>(args, "args");
			Delegate[] invocationList = md.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				if (!(bool)invocationList[i].DynamicInvoke(args))
				{
					return false;
				}
			}
			return true;
		}

		public static bool InvokeWhileFalse(this MulticastDelegate md, params object[] args)
		{
			if (md == null)
			{
				return false;
			}
			Helpers.ThrowIfArgumentNull<object[]>(args, "args");
			Delegate[] invocationList = md.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				if ((bool)invocationList[i].DynamicInvoke(args))
				{
					return true;
				}
			}
			return false;
		}

		[return: Nullable(2)]
		public static T InvokeWhileNull<T>([Nullable(2)] this MulticastDelegate md, params object[] args) where T : class
		{
			if (md == null)
			{
				return default(T);
			}
			Helpers.ThrowIfArgumentNull<object[]>(args, "args");
			Delegate[] invocationList = md.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				T t = (T)((object)invocationList[i].DynamicInvoke(args));
				if (t != null)
				{
					return t;
				}
			}
			return default(T);
		}

		public static string SpacedPascalCase(this string input)
		{
			Helpers.ThrowIfArgumentNull<string>(input, "input");
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < input.Length; i++)
			{
				char c = input[i];
				if (i > 0 && char.IsUpper(c))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append(c);
			}
			return stringBuilder.ToString();
		}

		public static string ReadNullTerminatedString(this BinaryReader stream)
		{
			Helpers.ThrowIfArgumentNull<BinaryReader>(stream, "stream");
			string text = "";
			char c;
			while ((c = stream.ReadChar()) != '\0')
			{
				text += c.ToString();
			}
			return text;
		}

		public static void WriteNullTerminatedString(this BinaryWriter stream, string text)
		{
			Helpers.ThrowIfArgumentNull<BinaryWriter>(stream, "stream");
			Helpers.ThrowIfArgumentNull<string>(text, "text");
			if (text != null)
			{
				foreach (char c in text)
				{
					stream.Write(c);
				}
			}
			stream.Write('\0');
		}

		private static MethodBase GetRealMethod(MethodBase method)
		{
			if (Extensions.RTDynamicMethod_m_owner != null && method.GetType() == Extensions.RTDynamicMethod)
			{
				return (MethodBase)Extensions.RTDynamicMethod_m_owner.GetValue(method);
			}
			return method;
		}

		public static T CastDelegate<[Nullable(0)] T>(this Delegate source) where T : Delegate
		{
			return (T)((object)Helpers.ThrowIfNull<Delegate>(source, "source").CastDelegate(typeof(T)));
		}

		[NullableContext(2)]
		[return: global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("source")]
		public static Delegate CastDelegate(this Delegate source, [Nullable(1)] Type type)
		{
			if (source == null)
			{
				return null;
			}
			Helpers.ThrowIfArgumentNull<Type>(type, "type");
			if (type.IsAssignableFrom(source.GetType()))
			{
				return source;
			}
			Delegate[] invocationList = source.GetInvocationList();
			if (invocationList.Length == 1)
			{
				return Extensions.GetRealMethod(invocationList[0].Method).CreateDelegate(type, invocationList[0].Target);
			}
			Delegate[] array = new Delegate[invocationList.Length];
			for (int i = 0; i < invocationList.Length; i++)
			{
				array[i] = Extensions.GetRealMethod(invocationList[i].Method).CreateDelegate(type, invocationList[i].Target);
			}
			return Delegate.Combine(array);
		}

		public static bool TryCastDelegate<[Nullable(0)] T>(this Delegate source, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out T result) where T : Delegate
		{
			if (source == null)
			{
				result = default(T);
				return false;
			}
			T t = source as T;
			if (t != null)
			{
				result = t;
				return true;
			}
			Delegate @delegate;
			bool flag = source.TryCastDelegate(typeof(T), out @delegate);
			result = (T)((object)@delegate);
			return flag;
		}

		public static bool TryCastDelegate(this Delegate source, Type type, [Nullable(2)] [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out Delegate result)
		{
			result = null;
			if (source == null)
			{
				return false;
			}
			bool flag;
			try
			{
				result = source.CastDelegate(type);
				flag = true;
			}
			catch (Exception ex)
			{
				bool flag2;
				MMDbgLog.DebugLogWarningStringHandler debugLogWarningStringHandler = new MMDbgLog.DebugLogWarningStringHandler(43, 3, out flag2);
				if (flag2)
				{
					debugLogWarningStringHandler.AppendLiteral("Exception thrown in TryCastDelegate(");
					debugLogWarningStringHandler.AppendFormatted<Type>(source.GetType());
					debugLogWarningStringHandler.AppendLiteral(" -> ");
					debugLogWarningStringHandler.AppendFormatted<Type>(type);
					debugLogWarningStringHandler.AppendLiteral("): ");
					debugLogWarningStringHandler.AppendFormatted<Exception>(ex);
				}
				MMDbgLog.Warning(ref debugLogWarningStringHandler);
				flag = false;
			}
			return flag;
		}

		[return: Nullable(2)]
		public static MethodInfo GetStateMachineTarget(this MethodInfo method)
		{
			if (Extensions.p_StateMachineType == null || Extensions.t_StateMachineAttribute == null)
			{
				return null;
			}
			Helpers.ThrowIfArgumentNull<MethodInfo>(method, "method");
			object[] customAttributes = method.GetCustomAttributes(false);
			int i = 0;
			while (i < customAttributes.Length)
			{
				Attribute attribute = (Attribute)customAttributes[i];
				if (Extensions.t_StateMachineAttribute.IsCompatible(attribute.GetType()))
				{
					Type type = Extensions.p_StateMachineType.GetValue(attribute, null) as Type;
					if (type == null)
					{
						return null;
					}
					return type.GetMethod("MoveNext", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				}
				else
				{
					i++;
				}
			}
			return null;
		}

		public static MethodBase GetActualGenericMethodDefinition(this MethodInfo method)
		{
			Helpers.ThrowIfArgumentNull<MethodInfo>(method, "method");
			return (method.IsGenericMethod ? method.GetGenericMethodDefinition() : method).GetUnfilledMethodOnGenericType();
		}

		public static MethodBase GetUnfilledMethodOnGenericType(this MethodBase method)
		{
			Helpers.ThrowIfArgumentNull<MethodBase>(method, "method");
			if (method.DeclaringType != null && method.DeclaringType.IsGenericType)
			{
				Type genericTypeDefinition = method.DeclaringType.GetGenericTypeDefinition();
				method = MethodBase.GetMethodFromHandle(method.MethodHandle, genericTypeDefinition.TypeHandle);
			}
			return method;
		}

		public static bool Is(this MemberReference member, string fullName)
		{
			Helpers.ThrowIfArgumentNull<string>(fullName, "fullName");
			return member != null && member.FullName.Replace("+", "/", StringComparison.Ordinal) == fullName.Replace("+", "/", StringComparison.Ordinal);
		}

		public static bool Is(this MemberReference member, string typeFullName, string name)
		{
			Helpers.ThrowIfArgumentNull<string>(typeFullName, "typeFullName");
			Helpers.ThrowIfArgumentNull<string>(name, "name");
			return member != null && member.DeclaringType.FullName.Replace("+", "/", StringComparison.Ordinal) == typeFullName.Replace("+", "/", StringComparison.Ordinal) && member.Name == name;
		}

		public static bool Is(this MemberReference member, Type type, string name)
		{
			Helpers.ThrowIfArgumentNull<Type>(type, "type");
			Helpers.ThrowIfArgumentNull<string>(name, "name");
			if (member == null)
			{
				return false;
			}
			string text = member.DeclaringType.FullName.Replace("+", "/", StringComparison.Ordinal);
			string fullName = type.FullName;
			return text == ((fullName != null) ? fullName.Replace("+", "/", StringComparison.Ordinal) : null) && member.Name == name;
		}

		public static bool Is(this MethodReference method, string fullName)
		{
			Helpers.ThrowIfArgumentNull<string>(fullName, "fullName");
			if (method == null)
			{
				return false;
			}
			if (fullName.Contains(' ', StringComparison.Ordinal))
			{
				if (method.GetID(null, null, true, true).Replace("+", "/", StringComparison.Ordinal) == fullName.Replace("+", "/", StringComparison.Ordinal))
				{
					return true;
				}
				if (method.GetID(null, null, true, false).Replace("+", "/", StringComparison.Ordinal) == fullName.Replace("+", "/", StringComparison.Ordinal))
				{
					return true;
				}
			}
			return method.FullName.Replace("+", "/", StringComparison.Ordinal) == fullName.Replace("+", "/", StringComparison.Ordinal);
		}

		public static bool Is(this MethodReference method, string typeFullName, string name)
		{
			Helpers.ThrowIfArgumentNull<string>(typeFullName, "typeFullName");
			Helpers.ThrowIfArgumentNull<string>(name, "name");
			return method != null && ((name.Contains(' ', StringComparison.Ordinal) && method.DeclaringType.FullName.Replace("+", "/", StringComparison.Ordinal) == typeFullName.Replace("+", "/", StringComparison.Ordinal) && method.GetID(null, null, false, false).Replace("+", "/", StringComparison.Ordinal) == name.Replace("+", "/", StringComparison.Ordinal)) || (method.DeclaringType.FullName.Replace("+", "/", StringComparison.Ordinal) == typeFullName.Replace("+", "/", StringComparison.Ordinal) && method.Name == name));
		}

		public static bool Is(this MethodReference method, Type type, string name)
		{
			Helpers.ThrowIfArgumentNull<Type>(type, "type");
			Helpers.ThrowIfArgumentNull<string>(name, "name");
			if (method == null)
			{
				return false;
			}
			if (name.Contains(' ', StringComparison.Ordinal))
			{
				string text = method.DeclaringType.FullName.Replace("+", "/", StringComparison.Ordinal);
				string fullName = type.FullName;
				if (text == ((fullName != null) ? fullName.Replace("+", "/", StringComparison.Ordinal) : null) && method.GetID(null, null, false, false).Replace("+", "/", StringComparison.Ordinal) == name.Replace("+", "/", StringComparison.Ordinal))
				{
					return true;
				}
			}
			string text2 = method.DeclaringType.FullName.Replace("+", "/", StringComparison.Ordinal);
			string fullName2 = type.FullName;
			return text2 == ((fullName2 != null) ? fullName2.Replace("+", "/", StringComparison.Ordinal) : null) && method.Name == name;
		}

		[NullableContext(2)]
		public static void ReplaceOperands([Nullable(1)] this ILProcessor il, object from, object to)
		{
			Helpers.ThrowIfArgumentNull<ILProcessor>(il, "il");
			foreach (Instruction instruction in il.Body.Instructions)
			{
				object operand = instruction.Operand;
				if ((operand != null) ? operand.Equals(from) : (from == null))
				{
					instruction.Operand = to;
				}
			}
		}

		public static FieldReference Import(this ILProcessor il, FieldInfo field)
		{
			return Helpers.ThrowIfNull<ILProcessor>(il, "il").Body.Method.Module.ImportReference(field);
		}

		public static MethodReference Import(this ILProcessor il, MethodBase method)
		{
			return Helpers.ThrowIfNull<ILProcessor>(il, "il").Body.Method.Module.ImportReference(method);
		}

		public static TypeReference Import(this ILProcessor il, Type type)
		{
			return Helpers.ThrowIfNull<ILProcessor>(il, "il").Body.Method.Module.ImportReference(type);
		}

		public static MemberReference Import(this ILProcessor il, MemberInfo member)
		{
			Helpers.ThrowIfArgumentNull<ILProcessor>(il, "il");
			Helpers.ThrowIfArgumentNull<MemberInfo>(member, "member");
			FieldInfo fieldInfo = member as FieldInfo;
			if (fieldInfo != null)
			{
				return il.Import(fieldInfo);
			}
			MethodBase methodBase = member as MethodBase;
			if (methodBase != null)
			{
				return il.Import(methodBase);
			}
			Type type = member as Type;
			if (type == null)
			{
				throw new NotSupportedException("Unsupported member type " + member.GetType().FullName);
			}
			return il.Import(type);
		}

		public static Instruction Create(this ILProcessor il, Mono.Cecil.Cil.OpCode opcode, FieldInfo field)
		{
			return Helpers.ThrowIfNull<ILProcessor>(il, "il").Create(opcode, il.Import(field));
		}

		public static Instruction Create(this ILProcessor il, Mono.Cecil.Cil.OpCode opcode, MethodBase method)
		{
			Helpers.ThrowIfArgumentNull<ILProcessor>(il, "il");
			return il.Create(opcode, il.Import(method));
		}

		public static Instruction Create(this ILProcessor il, Mono.Cecil.Cil.OpCode opcode, Type type)
		{
			return Helpers.ThrowIfNull<ILProcessor>(il, "il").Create(opcode, il.Import(type));
		}

		public static Instruction Create(this ILProcessor il, Mono.Cecil.Cil.OpCode opcode, object operand)
		{
			Instruction instruction = Helpers.ThrowIfNull<ILProcessor>(il, "il").Create(Mono.Cecil.Cil.OpCodes.Nop);
			instruction.OpCode = opcode;
			instruction.Operand = operand;
			return instruction;
		}

		public static Instruction Create(this ILProcessor il, Mono.Cecil.Cil.OpCode opcode, MemberInfo member)
		{
			Helpers.ThrowIfArgumentNull<ILProcessor>(il, "il");
			Helpers.ThrowIfArgumentNull<MemberInfo>(member, "member");
			FieldInfo fieldInfo = member as FieldInfo;
			if (fieldInfo != null)
			{
				return il.Create(opcode, fieldInfo);
			}
			MethodBase methodBase = member as MethodBase;
			if (methodBase != null)
			{
				return il.Create(opcode, methodBase);
			}
			Type type = member as Type;
			if (type == null)
			{
				throw new NotSupportedException("Unsupported member type " + member.GetType().FullName);
			}
			return il.Create(opcode, type);
		}

		public static void Emit(this ILProcessor il, Mono.Cecil.Cil.OpCode opcode, FieldInfo field)
		{
			Helpers.ThrowIfNull<ILProcessor>(il, "il").Emit(opcode, il.Import(field));
		}

		public static void Emit(this ILProcessor il, Mono.Cecil.Cil.OpCode opcode, MethodBase method)
		{
			Helpers.ThrowIfArgumentNull<ILProcessor>(il, "il");
			Helpers.ThrowIfArgumentNull<MethodBase>(method, "method");
			il.Emit(opcode, il.Import(method));
		}

		public static void Emit(this ILProcessor il, Mono.Cecil.Cil.OpCode opcode, Type type)
		{
			Helpers.ThrowIfNull<ILProcessor>(il, "il").Emit(opcode, il.Import(type));
		}

		public static void Emit(this ILProcessor il, Mono.Cecil.Cil.OpCode opcode, MemberInfo member)
		{
			Helpers.ThrowIfArgumentNull<ILProcessor>(il, "il");
			Helpers.ThrowIfArgumentNull<MemberInfo>(member, "member");
			FieldInfo fieldInfo = member as FieldInfo;
			if (fieldInfo != null)
			{
				il.Emit(opcode, fieldInfo);
				return;
			}
			MethodBase methodBase = member as MethodBase;
			if (methodBase != null)
			{
				il.Emit(opcode, methodBase);
				return;
			}
			Type type = member as Type;
			if (type == null)
			{
				throw new NotSupportedException("Unsupported member type " + member.GetType().FullName);
			}
			il.Emit(opcode, type);
		}

		public static void Emit(this ILProcessor il, Mono.Cecil.Cil.OpCode opcode, object operand)
		{
			Helpers.ThrowIfNull<ILProcessor>(il, "il").Append(il.Create(opcode, operand));
		}

		// Note: this type is marked as 'beforefieldinit'.
		static Extensions()
		{
			FieldInfo field = typeof(GenericParameter).GetField("position", BindingFlags.Instance | BindingFlags.NonPublic);
			if (field == null)
			{
				throw new InvalidOperationException("No field 'position' on GenericParameter");
			}
			Extensions.f_GenericParameter_position = field;
			FieldInfo field2 = typeof(GenericParameter).GetField("type", BindingFlags.Instance | BindingFlags.NonPublic);
			if (field2 == null)
			{
				throw new InvalidOperationException("No field 'type' on GenericParameter");
			}
			Extensions.f_GenericParameter_type = field2;
			Extensions._GetManagedSizeCache = new ConcurrentDictionary<Type, int>(new KeyValuePair<Type, int>[]
			{
				new KeyValuePair<Type, int>(typeof(void), 0)
			});
			Extensions._GetLdftnPointerCache = new Dictionary<MethodBase, Func<IntPtr>>();
			Extensions.RTDynamicMethod = typeof(DynamicMethod).GetNestedType("RTDynamicMethod", BindingFlags.NonPublic);
			Type rtdynamicMethod = Extensions.RTDynamicMethod;
			Extensions.RTDynamicMethod_m_owner = ((rtdynamicMethod != null) ? rtdynamicMethod.GetField("m_owner", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) : null);
			Extensions.t_StateMachineAttribute = typeof(object).Assembly.GetType("System.Runtime.CompilerServices.StateMachineAttribute");
			Type type = Extensions.t_StateMachineAttribute;
			Extensions.p_StateMachineType = ((type != null) ? type.GetProperty("StateMachineType") : null);
		}

		private static readonly Type t_Code = typeof(Code);

		private static readonly Type t_OpCodes = typeof(Mono.Cecil.Cil.OpCodes);

		private static readonly Dictionary<int, Mono.Cecil.Cil.OpCode> _ToLongOp = new Dictionary<int, Mono.Cecil.Cil.OpCode>();

		private static readonly Dictionary<int, Mono.Cecil.Cil.OpCode> _ToShortOp = new Dictionary<int, Mono.Cecil.Cil.OpCode>();

		private static readonly Dictionary<Type, FieldInfo> fmap_mono_assembly = new Dictionary<Type, FieldInfo>();

		private static readonly bool _MonoAssemblyNameHasArch = new AssemblyName("Dummy, ProcessorArchitecture=MSIL").ProcessorArchitecture == ProcessorArchitecture.MSIL;

		[Nullable(2)]
		private static readonly Type _RTDynamicMethod = typeof(DynamicMethod).GetNestedType("RTDynamicMethod", BindingFlags.Public | BindingFlags.NonPublic);

		private static readonly Type t_ParamArrayAttribute = typeof(ParamArrayAttribute);

		private static readonly FieldInfo f_GenericParameter_position;

		private static readonly FieldInfo f_GenericParameter_type;

		private static readonly ConcurrentDictionary<Type, int> _GetManagedSizeCache;

		[Nullable(2)]
		private static MethodInfo _GetManagedSizeHelper;

		private static readonly Dictionary<MethodBase, Func<IntPtr>> _GetLdftnPointerCache;

		[Nullable(2)]
		private static readonly Type RTDynamicMethod;

		[Nullable(2)]
		private static readonly FieldInfo RTDynamicMethod_m_owner;

		[Nullable(2)]
		private static readonly Type t_StateMachineAttribute;

		[Nullable(2)]
		private static readonly PropertyInfo p_StateMachineType;

		[CompilerGenerated]
		private static class <>O
		{
			[Nullable(new byte[] { 0, 1 })]
			public static Func<Type, int> <0>__ComputeManagedSize;
		}
	}
}
