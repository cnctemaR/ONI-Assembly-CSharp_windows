using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace System
{
	internal class TypeSpec
	{
		internal bool HasModifiers
		{
			get
			{
				return this.modifier_spec != null;
			}
		}

		internal bool IsNested
		{
			get
			{
				return this.nested != null && this.nested.Count > 0;
			}
		}

		internal bool IsByRef
		{
			get
			{
				return this.is_byref;
			}
		}

		internal TypeName Name
		{
			get
			{
				return this.name;
			}
		}

		internal IEnumerable<TypeName> Nested
		{
			get
			{
				if (this.nested != null)
				{
					return this.nested;
				}
				return EmptyArray<TypeName>.Value;
			}
		}

		internal IEnumerable<ModifierSpec> Modifiers
		{
			get
			{
				if (this.modifier_spec != null)
				{
					return this.modifier_spec;
				}
				return EmptyArray<ModifierSpec>.Value;
			}
		}

		private string GetDisplayFullName(TypeSpec.DisplayNameFormat flags)
		{
			bool flag = (flags & TypeSpec.DisplayNameFormat.WANT_ASSEMBLY) > TypeSpec.DisplayNameFormat.Default;
			bool flag2 = (flags & TypeSpec.DisplayNameFormat.NO_MODIFIERS) == TypeSpec.DisplayNameFormat.Default;
			StringBuilder stringBuilder = new StringBuilder(this.name.DisplayName);
			if (this.nested != null)
			{
				foreach (TypeIdentifier typeIdentifier in this.nested)
				{
					stringBuilder.Append('+').Append(typeIdentifier.DisplayName);
				}
			}
			if (this.generic_params != null)
			{
				stringBuilder.Append('[');
				for (int i = 0; i < this.generic_params.Count; i++)
				{
					if (i > 0)
					{
						stringBuilder.Append(", ");
					}
					if (this.generic_params[i].assembly_name != null)
					{
						stringBuilder.Append('[').Append(this.generic_params[i].DisplayFullName).Append(']');
					}
					else
					{
						stringBuilder.Append(this.generic_params[i].DisplayFullName);
					}
				}
				stringBuilder.Append(']');
			}
			if (flag2)
			{
				this.GetModifierString(stringBuilder);
			}
			if (this.assembly_name != null && flag)
			{
				stringBuilder.Append(", ").Append(this.assembly_name);
			}
			return stringBuilder.ToString();
		}

		internal string ModifierString()
		{
			return this.GetModifierString(new StringBuilder()).ToString();
		}

		private StringBuilder GetModifierString(StringBuilder sb)
		{
			if (this.modifier_spec != null)
			{
				foreach (ModifierSpec modifierSpec in this.modifier_spec)
				{
					modifierSpec.Append(sb);
				}
			}
			if (this.is_byref)
			{
				sb.Append('&');
			}
			return sb;
		}

		internal string DisplayFullName
		{
			get
			{
				if (this.display_fullname == null)
				{
					this.display_fullname = this.GetDisplayFullName(TypeSpec.DisplayNameFormat.Default);
				}
				return this.display_fullname;
			}
		}

		internal static TypeSpec Parse(string typeName)
		{
			int num = 0;
			if (typeName == null)
			{
				throw new ArgumentNullException("typeName");
			}
			TypeSpec typeSpec = TypeSpec.Parse(typeName, ref num, false, true);
			if (num < typeName.Length)
			{
				throw new ArgumentException("Count not parse the whole type name", "typeName");
			}
			return typeSpec;
		}

		internal static string EscapeDisplayName(string internalName)
		{
			StringBuilder stringBuilder = new StringBuilder(internalName.Length);
			int i = 0;
			while (i < internalName.Length)
			{
				char c = internalName[i];
				switch (c)
				{
				case '&':
				case '*':
				case '+':
				case ',':
					goto IL_0056;
				case '\'':
				case '(':
				case ')':
					goto IL_0067;
				default:
					switch (c)
					{
					case '[':
					case '\\':
					case ']':
						goto IL_0056;
					default:
						goto IL_0067;
					}
					break;
				}
				IL_006F:
				i++;
				continue;
				IL_0056:
				stringBuilder.Append('\\').Append(c);
				goto IL_006F;
				IL_0067:
				stringBuilder.Append(c);
				goto IL_006F;
			}
			return stringBuilder.ToString();
		}

		internal static string UnescapeInternalName(string displayName)
		{
			StringBuilder stringBuilder = new StringBuilder(displayName.Length);
			for (int i = 0; i < displayName.Length; i++)
			{
				char c = displayName[i];
				if (c == '\\' && ++i < displayName.Length)
				{
					c = displayName[i];
				}
				stringBuilder.Append(c);
			}
			return stringBuilder.ToString();
		}

		internal static bool NeedsEscaping(string internalName)
		{
			foreach (char c in internalName)
			{
				switch (c)
				{
				case '&':
				case '*':
				case '+':
				case ',':
					return true;
				case '\'':
				case '(':
				case ')':
					break;
				default:
					switch (c)
					{
					case '[':
					case '\\':
					case ']':
						return true;
					}
					break;
				}
			}
			return false;
		}

		internal Type Resolve(Func<AssemblyName, Assembly> assemblyResolver, Func<Assembly, string, bool, Type> typeResolver, bool throwOnError, bool ignoreCase)
		{
			Assembly assembly = null;
			if (assemblyResolver == null && typeResolver == null)
			{
				return Type.GetType(this.DisplayFullName, throwOnError, ignoreCase);
			}
			if (this.assembly_name != null)
			{
				if (assemblyResolver != null)
				{
					assembly = assemblyResolver(new AssemblyName(this.assembly_name));
				}
				else
				{
					assembly = Assembly.Load(this.assembly_name);
				}
				if (assembly == null)
				{
					if (throwOnError)
					{
						throw new FileNotFoundException("Could not resolve assembly '" + this.assembly_name + "'");
					}
					return null;
				}
			}
			Type type = null;
			if (typeResolver != null)
			{
				type = typeResolver(assembly, this.name.DisplayName, ignoreCase);
			}
			else
			{
				type = assembly.GetType(this.name.DisplayName, false, ignoreCase);
			}
			if (!(type == null))
			{
				if (this.nested != null)
				{
					foreach (TypeIdentifier typeIdentifier in this.nested)
					{
						Type nestedType = type.GetNestedType(typeIdentifier.DisplayName, BindingFlags.Public | BindingFlags.NonPublic);
						if (nestedType == null)
						{
							if (throwOnError)
							{
								throw new TypeLoadException("Could not resolve type '" + typeIdentifier + "'");
							}
							return null;
						}
						else
						{
							type = nestedType;
						}
					}
				}
				if (this.generic_params != null)
				{
					Type[] array = new Type[this.generic_params.Count];
					int i = 0;
					while (i < array.Length)
					{
						Type type2 = this.generic_params[i].Resolve(assemblyResolver, typeResolver, throwOnError, ignoreCase);
						if (type2 == null)
						{
							if (throwOnError)
							{
								throw new TypeLoadException("Could not resolve type '" + this.generic_params[i].name + "'");
							}
							return null;
						}
						else
						{
							array[i] = type2;
							i++;
						}
					}
					type = type.MakeGenericType(array);
				}
				if (this.modifier_spec != null)
				{
					foreach (ModifierSpec modifierSpec in this.modifier_spec)
					{
						type = modifierSpec.Resolve(type);
					}
				}
				if (this.is_byref)
				{
					type = type.MakeByRefType();
				}
				return type;
			}
			if (throwOnError)
			{
				throw new TypeLoadException("Could not resolve type '" + this.name + "'");
			}
			return null;
		}

		private void AddName(string type_name)
		{
			if (this.name == null)
			{
				this.name = TypeSpec.ParsedTypeIdentifier(type_name);
				return;
			}
			if (this.nested == null)
			{
				this.nested = new List<TypeIdentifier>();
			}
			this.nested.Add(TypeSpec.ParsedTypeIdentifier(type_name));
		}

		private void AddModifier(ModifierSpec md)
		{
			if (this.modifier_spec == null)
			{
				this.modifier_spec = new List<ModifierSpec>();
			}
			this.modifier_spec.Add(md);
		}

		private static void SkipSpace(string name, ref int pos)
		{
			int num = pos;
			while (num < name.Length && char.IsWhiteSpace(name[num]))
			{
				num++;
			}
			pos = num;
		}

		private static void BoundCheck(int idx, string s)
		{
			if (idx >= s.Length)
			{
				throw new ArgumentException("Invalid generic arguments spec", "typeName");
			}
		}

		private static TypeIdentifier ParsedTypeIdentifier(string displayName)
		{
			return TypeIdentifiers.FromDisplay(displayName);
		}

		private static TypeSpec Parse(string name, ref int p, bool is_recurse, bool allow_aqn)
		{
			int i = p;
			bool flag = false;
			TypeSpec typeSpec = new TypeSpec();
			TypeSpec.SkipSpace(name, ref i);
			int num = i;
			while (i < name.Length)
			{
				char c = name[i];
				switch (c)
				{
				case '&':
				case '*':
					goto IL_0098;
				case '\'':
				case '(':
				case ')':
					break;
				case '+':
					typeSpec.AddName(name.Substring(num, i - num));
					num = i + 1;
					break;
				case ',':
					goto IL_0077;
				default:
					switch (c)
					{
					case '[':
						goto IL_0098;
					case '\\':
						i++;
						break;
					case ']':
						goto IL_0077;
					}
					break;
				}
				IL_00D6:
				if (!flag)
				{
					i++;
					continue;
				}
				break;
				IL_0077:
				typeSpec.AddName(name.Substring(num, i - num));
				num = i + 1;
				flag = true;
				if (is_recurse && !allow_aqn)
				{
					p = i;
					return typeSpec;
				}
				goto IL_00D6;
				IL_0098:
				if (name[i] != '[' && is_recurse)
				{
					throw new ArgumentException("Generic argument can't be byref or pointer type", "typeName");
				}
				typeSpec.AddName(name.Substring(num, i - num));
				num = i + 1;
				flag = true;
				goto IL_00D6;
			}
			if (num < i)
			{
				typeSpec.AddName(name.Substring(num, i - num));
			}
			else if (num == i)
			{
				typeSpec.AddName(string.Empty);
			}
			if (flag)
			{
				while (i < name.Length)
				{
					char c = name[i];
					if (c <= '*')
					{
						if (c != '&')
						{
							if (c != '*')
							{
								goto IL_04BE;
							}
							if (typeSpec.is_byref)
							{
								throw new ArgumentException("Can't have a pointer to a byref type", "typeName");
							}
							int num2 = 1;
							while (i + 1 < name.Length && name[i + 1] == '*')
							{
								i++;
								num2++;
							}
							typeSpec.AddModifier(new PointerSpec(num2));
						}
						else
						{
							if (typeSpec.is_byref)
							{
								throw new ArgumentException("Can't have a byref of a byref", "typeName");
							}
							typeSpec.is_byref = true;
						}
					}
					else if (c != ',')
					{
						if (c != '[')
						{
							if (c != ']')
							{
								goto IL_04BE;
							}
							if (is_recurse)
							{
								p = i;
								return typeSpec;
							}
							throw new ArgumentException("Unmatched ']'", "typeName");
						}
						else
						{
							if (typeSpec.is_byref)
							{
								throw new ArgumentException("Byref qualifier must be the last one of a type", "typeName");
							}
							i++;
							if (i >= name.Length)
							{
								throw new ArgumentException("Invalid array/generic spec", "typeName");
							}
							TypeSpec.SkipSpace(name, ref i);
							if (name[i] != ',' && name[i] != '*' && name[i] != ']')
							{
								List<TypeSpec> list = new List<TypeSpec>();
								if (typeSpec.HasModifiers)
								{
									throw new ArgumentException("generic args after array spec or pointer type", "typeName");
								}
								while (i < name.Length)
								{
									TypeSpec.SkipSpace(name, ref i);
									bool flag2 = name[i] == '[';
									if (flag2)
									{
										i++;
									}
									list.Add(TypeSpec.Parse(name, ref i, true, flag2));
									TypeSpec.BoundCheck(i, name);
									if (flag2)
									{
										if (name[i] != ']')
										{
											throw new ArgumentException("Unclosed assembly-qualified type name at " + name[i].ToString(), "typeName");
										}
										i++;
										TypeSpec.BoundCheck(i, name);
									}
									if (name[i] == ']')
									{
										break;
									}
									if (name[i] != ',')
									{
										throw new ArgumentException("Invalid generic arguments separator " + name[i].ToString(), "typeName");
									}
									i++;
								}
								if (i >= name.Length || name[i] != ']')
								{
									throw new ArgumentException("Error parsing generic params spec", "typeName");
								}
								typeSpec.generic_params = list;
							}
							else
							{
								int num3 = 1;
								bool flag3 = false;
								while (i < name.Length && name[i] != ']')
								{
									if (name[i] == '*')
									{
										if (flag3)
										{
											throw new ArgumentException("Array spec cannot have 2 bound dimensions", "typeName");
										}
										flag3 = true;
									}
									else
									{
										if (name[i] != ',')
										{
											throw new ArgumentException("Invalid character in array spec " + name[i].ToString(), "typeName");
										}
										num3++;
									}
									i++;
									TypeSpec.SkipSpace(name, ref i);
								}
								if (i >= name.Length || name[i] != ']')
								{
									throw new ArgumentException("Error parsing array spec", "typeName");
								}
								if (num3 > 1 && flag3)
								{
									throw new ArgumentException("Invalid array spec, multi-dimensional array cannot be bound", "typeName");
								}
								typeSpec.AddModifier(new ArraySpec(num3, flag3));
							}
						}
					}
					else if (is_recurse && allow_aqn)
					{
						int num4 = i;
						while (num4 < name.Length && name[num4] != ']')
						{
							num4++;
						}
						if (num4 >= name.Length)
						{
							throw new ArgumentException("Unmatched ']' while parsing generic argument assembly name");
						}
						typeSpec.assembly_name = name.Substring(i + 1, num4 - i - 1).Trim();
						p = num4;
						return typeSpec;
					}
					else
					{
						if (is_recurse)
						{
							p = i;
							return typeSpec;
						}
						if (allow_aqn)
						{
							typeSpec.assembly_name = name.Substring(i + 1).Trim();
							i = name.Length;
						}
					}
					i++;
					continue;
					IL_04BE:
					throw new ArgumentException(string.Concat(new object[]
					{
						"Bad type def, can't handle '",
						name[i].ToString(),
						"' at ",
						i
					}), "typeName");
				}
			}
			p = i;
			return typeSpec;
		}

		internal TypeName TypeNameWithoutModifiers()
		{
			return new TypeSpec.TypeSpecTypeName(this, false);
		}

		internal TypeName TypeName
		{
			get
			{
				return new TypeSpec.TypeSpecTypeName(this, true);
			}
		}

		private TypeIdentifier name;

		private string assembly_name;

		private List<TypeIdentifier> nested;

		private List<TypeSpec> generic_params;

		private List<ModifierSpec> modifier_spec;

		private bool is_byref;

		private string display_fullname;

		[Flags]
		internal enum DisplayNameFormat
		{
			Default = 0,
			WANT_ASSEMBLY = 1,
			NO_MODIFIERS = 2
		}

		private class TypeSpecTypeName : TypeNames.ATypeName, TypeName, IEquatable<TypeName>
		{
			internal TypeSpecTypeName(TypeSpec ts, bool wantModifiers)
			{
				this.ts = ts;
				this.want_modifiers = wantModifiers;
			}

			public override string DisplayName
			{
				get
				{
					if (this.want_modifiers)
					{
						return this.ts.DisplayFullName;
					}
					return this.ts.GetDisplayFullName(TypeSpec.DisplayNameFormat.NO_MODIFIERS);
				}
			}

			public override TypeName NestedName(TypeIdentifier innerName)
			{
				return TypeNames.FromDisplay(this.DisplayName + "+" + innerName.DisplayName);
			}

			private TypeSpec ts;

			private bool want_modifiers;
		}
	}
}
