using System;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;

namespace System.Xml.Serialization
{
	internal class SourceInfo
	{
		public SourceInfo(string source, string arg, MemberInfo memberInfo, Type type, CodeGenerator ilg)
		{
			this.Source = source;
			this.Arg = arg ?? source;
			this.MemberInfo = memberInfo;
			this.Type = type;
			this.ILG = ilg;
		}

		public SourceInfo CastTo(TypeDesc td)
		{
			return new SourceInfo(string.Concat(new string[] { "((", td.CSharpName, ")", this.Source, ")" }), this.Arg, this.MemberInfo, td.Type, this.ILG);
		}

		public void LoadAddress(Type elementType)
		{
			this.InternalLoad(elementType, true);
		}

		public void Load(Type elementType)
		{
			this.InternalLoad(elementType, false);
		}

		private void InternalLoad(Type elementType, bool asAddress = false)
		{
			Match match = SourceInfo.regex.Match(this.Arg);
			if (match.Success)
			{
				object variable = this.ILG.GetVariable(match.Groups["a"].Value);
				Type variableType = this.ILG.GetVariableType(variable);
				object variable2 = this.ILG.GetVariable(match.Groups["ia"].Value);
				if (variableType.IsArray)
				{
					this.ILG.Load(variable);
					this.ILG.Load(variable2);
					Type elementType2 = variableType.GetElementType();
					if (CodeGenerator.IsNullableGenericType(elementType2))
					{
						this.ILG.Ldelema(elementType2);
						this.ConvertNullableValue(elementType2, elementType);
						return;
					}
					if (elementType2.IsValueType)
					{
						this.ILG.Ldelema(elementType2);
						if (!asAddress)
						{
							this.ILG.Ldobj(elementType2);
						}
					}
					else
					{
						this.ILG.Ldelem(elementType2);
					}
					if (elementType != null)
					{
						this.ILG.ConvertValue(elementType2, elementType);
						return;
					}
				}
				else
				{
					this.ILG.Load(variable);
					this.ILG.Load(variable2);
					MethodInfo methodInfo = variableType.GetMethod("get_Item", CodeGenerator.InstanceBindingFlags, null, new Type[] { typeof(int) }, null);
					if (methodInfo == null && typeof(IList).IsAssignableFrom(variableType))
					{
						methodInfo = SourceInfo.iListGetItemMethod.Value;
					}
					this.ILG.Call(methodInfo);
					Type returnType = methodInfo.ReturnType;
					if (CodeGenerator.IsNullableGenericType(returnType))
					{
						LocalBuilder tempLocal = this.ILG.GetTempLocal(returnType);
						this.ILG.Stloc(tempLocal);
						this.ILG.Ldloca(tempLocal);
						this.ConvertNullableValue(returnType, elementType);
						return;
					}
					if (elementType != null && !returnType.IsAssignableFrom(elementType) && !elementType.IsAssignableFrom(returnType))
					{
						throw new CodeGeneratorConversionException(returnType, elementType, asAddress, "IsNotAssignableFrom");
					}
					this.Convert(returnType, elementType, asAddress);
					return;
				}
			}
			else
			{
				if (this.Source == "null")
				{
					this.ILG.Load(null);
					return;
				}
				Type type;
				if (this.Arg.StartsWith("o.@", StringComparison.Ordinal) || this.MemberInfo != null)
				{
					object obj = this.ILG.GetVariable(this.Arg.StartsWith("o.@", StringComparison.Ordinal) ? "o" : this.Arg);
					type = this.ILG.GetVariableType(obj);
					if (type.IsValueType)
					{
						this.ILG.LoadAddress(obj);
					}
					else
					{
						this.ILG.Load(obj);
					}
				}
				else
				{
					object obj = this.ILG.GetVariable(this.Arg);
					type = this.ILG.GetVariableType(obj);
					if (CodeGenerator.IsNullableGenericType(type) && type.GetGenericArguments()[0] == elementType)
					{
						this.ILG.LoadAddress(obj);
						this.ConvertNullableValue(type, elementType);
					}
					else if (asAddress)
					{
						this.ILG.LoadAddress(obj);
					}
					else
					{
						this.ILG.Load(obj);
					}
				}
				if (this.MemberInfo != null)
				{
					Type type2 = ((this.MemberInfo is FieldInfo) ? ((FieldInfo)this.MemberInfo).FieldType : ((PropertyInfo)this.MemberInfo).PropertyType);
					if (CodeGenerator.IsNullableGenericType(type2))
					{
						this.ILG.LoadMemberAddress(this.MemberInfo);
						this.ConvertNullableValue(type2, elementType);
						return;
					}
					this.ILG.LoadMember(this.MemberInfo);
					this.Convert(type2, elementType, asAddress);
					return;
				}
				else
				{
					match = SourceInfo.regex2.Match(this.Source);
					if (match.Success)
					{
						if (asAddress)
						{
							this.ILG.ConvertAddress(type, this.Type);
						}
						else
						{
							this.ILG.ConvertValue(type, this.Type);
						}
						type = this.Type;
					}
					this.Convert(type, elementType, asAddress);
				}
			}
		}

		private void Convert(Type sourceType, Type targetType, bool asAddress)
		{
			if (targetType != null)
			{
				if (asAddress)
				{
					this.ILG.ConvertAddress(sourceType, targetType);
					return;
				}
				this.ILG.ConvertValue(sourceType, targetType);
			}
		}

		private void ConvertNullableValue(Type nullableType, Type targetType)
		{
			if (targetType != nullableType)
			{
				MethodInfo method = nullableType.GetMethod("get_Value", CodeGenerator.InstanceBindingFlags, null, CodeGenerator.EmptyTypeArray, null);
				this.ILG.Call(method);
				if (targetType != null)
				{
					this.ILG.ConvertValue(method.ReturnType, targetType);
				}
			}
		}

		public static implicit operator string(SourceInfo source)
		{
			return source.Source;
		}

		public static bool operator !=(SourceInfo a, SourceInfo b)
		{
			if (a != null)
			{
				return !a.Equals(b);
			}
			return b != null;
		}

		public static bool operator ==(SourceInfo a, SourceInfo b)
		{
			if (a != null)
			{
				return a.Equals(b);
			}
			return b == null;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return this.Source == null;
			}
			SourceInfo sourceInfo = obj as SourceInfo;
			return sourceInfo != null && this.Source == sourceInfo.Source;
		}

		public override int GetHashCode()
		{
			if (this.Source != null)
			{
				return this.Source.GetHashCode();
			}
			return 0;
		}

		private static Regex regex = new Regex("([(][(](?<t>[^)]+)[)])?(?<a>[^[]+)[[](?<ia>.+)[]][)]?");

		private static Regex regex2 = new Regex("[(][(](?<cast>[^)]+)[)](?<arg>[^)]+)[)]");

		private static readonly Lazy<MethodInfo> iListGetItemMethod = new Lazy<MethodInfo>(() => typeof(IList).GetMethod("get_Item", CodeGenerator.InstanceBindingFlags, null, new Type[] { typeof(int) }, null));

		public string Source;

		public readonly string Arg;

		public readonly MemberInfo MemberInfo;

		public readonly Type Type;

		public readonly CodeGenerator ILG;
	}
}
