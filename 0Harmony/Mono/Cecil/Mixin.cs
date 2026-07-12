using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Mono.Cecil.Cil;
using Mono.Cecil.Metadata;
using Mono.Collections.Generic;
using Mono.Security.Cryptography;

namespace Mono.Cecil
{
	internal static class Mixin
	{
		public static bool IsNullOrEmpty<T>(this T[] self)
		{
			return self == null || self.Length == 0;
		}

		public static bool IsNullOrEmpty<T>(this Collection<T> self)
		{
			return self == null || self.size == 0;
		}

		public static T[] Resize<T>(this T[] self, int length)
		{
			Array.Resize<T>(ref self, length);
			return self;
		}

		public static T[] Add<T>(this T[] self, T item)
		{
			if (self == null)
			{
				self = new T[] { item };
				return self;
			}
			self = self.Resize<T>(self.Length + 1);
			self[self.Length - 1] = item;
			return self;
		}

		public static Version CheckVersion(Version version)
		{
			if (version == null)
			{
				return Mixin.ZeroVersion;
			}
			if (version.Build == -1)
			{
				return new Version(version.Major, version.Minor, 0, 0);
			}
			if (version.Revision == -1)
			{
				return new Version(version.Major, version.Minor, version.Build, 0);
			}
			return version;
		}

		public static bool TryGetUniqueDocument(this MethodDebugInformation info, out Document document)
		{
			document = info.SequencePoints[0].Document;
			for (int i = 1; i < info.SequencePoints.Count; i++)
			{
				if (info.SequencePoints[i].Document != document)
				{
					return false;
				}
			}
			return true;
		}

		public static void ResolveConstant(this IConstantProvider self, ref object constant, ModuleDefinition module)
		{
			if (module == null)
			{
				constant = Mixin.NoValue;
				return;
			}
			object syncRoot = module.SyncRoot;
			lock (syncRoot)
			{
				if (constant == Mixin.NotResolved)
				{
					if (module.HasImage())
					{
						constant = module.Read<IConstantProvider, object>(self, (IConstantProvider provider, MetadataReader reader) => reader.ReadConstant(provider));
					}
					else
					{
						constant = Mixin.NoValue;
					}
				}
			}
		}

		public static bool GetHasCustomAttributes(this ICustomAttributeProvider self, ModuleDefinition module)
		{
			if (module.HasImage())
			{
				return module.Read<ICustomAttributeProvider, bool>(self, (ICustomAttributeProvider provider, MetadataReader reader) => reader.HasCustomAttributes(provider));
			}
			return false;
		}

		public static Collection<CustomAttribute> GetCustomAttributes(this ICustomAttributeProvider self, ref Collection<CustomAttribute> variable, ModuleDefinition module)
		{
			if (module.HasImage())
			{
				return module.Read<ICustomAttributeProvider, Collection<CustomAttribute>>(ref variable, self, (ICustomAttributeProvider provider, MetadataReader reader) => reader.ReadCustomAttributes(provider));
			}
			Interlocked.CompareExchange<Collection<CustomAttribute>>(ref variable, new Collection<CustomAttribute>(), null);
			return variable;
		}

		public static bool ContainsGenericParameter(this IGenericInstance self)
		{
			Collection<TypeReference> genericArguments = self.GenericArguments;
			for (int i = 0; i < genericArguments.Count; i++)
			{
				if (genericArguments[i].ContainsGenericParameter)
				{
					return true;
				}
			}
			return false;
		}

		public static void GenericInstanceFullName(this IGenericInstance self, StringBuilder builder)
		{
			builder.Append("<");
			Collection<TypeReference> genericArguments = self.GenericArguments;
			for (int i = 0; i < genericArguments.Count; i++)
			{
				if (i > 0)
				{
					builder.Append(",");
				}
				builder.Append(genericArguments[i].FullName);
			}
			builder.Append(">");
		}

		public static bool GetHasGenericParameters(this IGenericParameterProvider self, ModuleDefinition module)
		{
			if (module.HasImage())
			{
				return module.Read<IGenericParameterProvider, bool>(self, (IGenericParameterProvider provider, MetadataReader reader) => reader.HasGenericParameters(provider));
			}
			return false;
		}

		public static Collection<GenericParameter> GetGenericParameters(this IGenericParameterProvider self, ref Collection<GenericParameter> collection, ModuleDefinition module)
		{
			if (module.HasImage())
			{
				return module.Read<IGenericParameterProvider, Collection<GenericParameter>>(ref collection, self, (IGenericParameterProvider provider, MetadataReader reader) => reader.ReadGenericParameters(provider));
			}
			Interlocked.CompareExchange<Collection<GenericParameter>>(ref collection, new GenericParameterCollection(self), null);
			return collection;
		}

		public static bool GetHasMarshalInfo(this IMarshalInfoProvider self, ModuleDefinition module)
		{
			if (module.HasImage())
			{
				return module.Read<IMarshalInfoProvider, bool>(self, (IMarshalInfoProvider provider, MetadataReader reader) => reader.HasMarshalInfo(provider));
			}
			return false;
		}

		public static MarshalInfo GetMarshalInfo(this IMarshalInfoProvider self, ref MarshalInfo variable, ModuleDefinition module)
		{
			if (!module.HasImage())
			{
				return null;
			}
			return module.Read<IMarshalInfoProvider, MarshalInfo>(ref variable, self, (IMarshalInfoProvider provider, MetadataReader reader) => reader.ReadMarshalInfo(provider));
		}

		public static bool GetAttributes(this uint self, uint attributes)
		{
			return (self & attributes) > 0U;
		}

		public static uint SetAttributes(this uint self, uint attributes, bool value)
		{
			if (value)
			{
				return self | attributes;
			}
			return self & ~attributes;
		}

		public static bool GetMaskedAttributes(this uint self, uint mask, uint attributes)
		{
			return (self & mask) == attributes;
		}

		public static uint SetMaskedAttributes(this uint self, uint mask, uint attributes, bool value)
		{
			if (value)
			{
				self &= ~mask;
				return self | attributes;
			}
			return self & ~(mask & attributes);
		}

		public static bool GetAttributes(this ushort self, ushort attributes)
		{
			return (self & attributes) > 0;
		}

		public static ushort SetAttributes(this ushort self, ushort attributes, bool value)
		{
			if (value)
			{
				return self | attributes;
			}
			return self & ~attributes;
		}

		public static bool GetMaskedAttributes(this ushort self, ushort mask, uint attributes)
		{
			return (long)(self & mask) == (long)((ulong)attributes);
		}

		public static ushort SetMaskedAttributes(this ushort self, ushort mask, uint attributes, bool value)
		{
			if (value)
			{
				self &= ~mask;
				return (ushort)((uint)self | attributes);
			}
			return (ushort)((uint)self & ~((uint)mask & attributes));
		}

		public static bool HasImplicitThis(this IMethodSignature self)
		{
			return self.HasThis && !self.ExplicitThis;
		}

		public static void MethodSignatureFullName(this IMethodSignature self, StringBuilder builder)
		{
			builder.Append("(");
			if (self.HasParameters)
			{
				Collection<ParameterDefinition> parameters = self.Parameters;
				for (int i = 0; i < parameters.Count; i++)
				{
					ParameterDefinition parameterDefinition = parameters[i];
					if (i > 0)
					{
						builder.Append(",");
					}
					if (parameterDefinition.ParameterType.IsSentinel)
					{
						builder.Append("...,");
					}
					builder.Append(parameterDefinition.ParameterType.FullName);
				}
			}
			builder.Append(")");
		}

		public static void CheckModule(ModuleDefinition module)
		{
			if (module == null)
			{
				throw new ArgumentNullException(Mixin.Argument.module.ToString());
			}
		}

		public static bool TryGetAssemblyNameReference(this ModuleDefinition module, AssemblyNameReference name_reference, out AssemblyNameReference assembly_reference)
		{
			Collection<AssemblyNameReference> assemblyReferences = module.AssemblyReferences;
			for (int i = 0; i < assemblyReferences.Count; i++)
			{
				AssemblyNameReference assemblyNameReference = assemblyReferences[i];
				if (Mixin.Equals(name_reference, assemblyNameReference))
				{
					assembly_reference = assemblyNameReference;
					return true;
				}
			}
			assembly_reference = null;
			return false;
		}

		private static bool Equals(byte[] a, byte[] b)
		{
			if (a == b)
			{
				return true;
			}
			if (a == null)
			{
				return false;
			}
			if (a.Length != b.Length)
			{
				return false;
			}
			for (int i = 0; i < a.Length; i++)
			{
				if (a[i] != b[i])
				{
					return false;
				}
			}
			return true;
		}

		private static bool Equals<T>(T a, T b) where T : class, IEquatable<T>
		{
			return a == b || (a != null && a.Equals(b));
		}

		private static bool Equals(AssemblyNameReference a, AssemblyNameReference b)
		{
			return a == b || (!(a.Name != b.Name) && Mixin.Equals<Version>(a.Version, b.Version) && !(a.Culture != b.Culture) && Mixin.Equals(a.PublicKeyToken, b.PublicKeyToken));
		}

		public static ParameterDefinition GetParameter(this Mono.Cecil.Cil.MethodBody self, int index)
		{
			MethodDefinition method = self.method;
			if (method.HasThis)
			{
				if (index == 0)
				{
					return self.ThisParameter;
				}
				index--;
			}
			Collection<ParameterDefinition> parameters = method.Parameters;
			if (index < 0 || index >= parameters.size)
			{
				return null;
			}
			return parameters[index];
		}

		public static VariableDefinition GetVariable(this Mono.Cecil.Cil.MethodBody self, int index)
		{
			Collection<VariableDefinition> variables = self.Variables;
			if (index < 0 || index >= variables.size)
			{
				return null;
			}
			return variables[index];
		}

		public static bool GetSemantics(this MethodDefinition self, MethodSemanticsAttributes semantics)
		{
			return (self.SemanticsAttributes & semantics) > MethodSemanticsAttributes.None;
		}

		public static void SetSemantics(this MethodDefinition self, MethodSemanticsAttributes semantics, bool value)
		{
			if (value)
			{
				self.SemanticsAttributes |= semantics;
				return;
			}
			self.SemanticsAttributes &= ~semantics;
		}

		public static bool IsVarArg(this IMethodSignature self)
		{
			return self.CallingConvention == MethodCallingConvention.VarArg;
		}

		public static int GetSentinelPosition(this IMethodSignature self)
		{
			if (!self.HasParameters)
			{
				return -1;
			}
			Collection<ParameterDefinition> parameters = self.Parameters;
			for (int i = 0; i < parameters.Count; i++)
			{
				if (parameters[i].ParameterType.IsSentinel)
				{
					return i;
				}
			}
			return -1;
		}

		public static void CheckName(object name)
		{
			if (name == null)
			{
				throw new ArgumentNullException(Mixin.Argument.name.ToString());
			}
		}

		public static void CheckName(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullOrEmptyException(Mixin.Argument.name.ToString());
			}
		}

		public static void CheckFileName(string fileName)
		{
			if (string.IsNullOrEmpty(fileName))
			{
				throw new ArgumentNullOrEmptyException(Mixin.Argument.fileName.ToString());
			}
		}

		public static void CheckFullName(string fullName)
		{
			if (string.IsNullOrEmpty(fullName))
			{
				throw new ArgumentNullOrEmptyException(Mixin.Argument.fullName.ToString());
			}
		}

		public static void CheckStream(object stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException(Mixin.Argument.stream.ToString());
			}
		}

		public static void CheckWriteSeek(Stream stream)
		{
			if (!stream.CanWrite || !stream.CanSeek)
			{
				throw new ArgumentException("Stream must be writable and seekable.");
			}
		}

		public static void CheckReadSeek(Stream stream)
		{
			if (!stream.CanRead || !stream.CanSeek)
			{
				throw new ArgumentException("Stream must be readable and seekable.");
			}
		}

		public static void CheckType(object type)
		{
			if (type == null)
			{
				throw new ArgumentNullException(Mixin.Argument.type.ToString());
			}
		}

		public static void CheckType(object type, Mixin.Argument argument)
		{
			if (type == null)
			{
				throw new ArgumentNullException(argument.ToString());
			}
		}

		public static void CheckField(object field)
		{
			if (field == null)
			{
				throw new ArgumentNullException(Mixin.Argument.field.ToString());
			}
		}

		public static void CheckMethod(object method)
		{
			if (method == null)
			{
				throw new ArgumentNullException(Mixin.Argument.method.ToString());
			}
		}

		public static void CheckParameters(object parameters)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(Mixin.Argument.parameters.ToString());
			}
		}

		public static uint GetTimestamp()
		{
			return (uint)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
		}

		public static bool HasImage(this ModuleDefinition self)
		{
			return self != null && self.HasImage;
		}

		public static string GetFileName(this Stream self)
		{
			FileStream fileStream = self as FileStream;
			if (fileStream == null)
			{
				return string.Empty;
			}
			return Path.GetFullPath(fileStream.Name);
		}

		public static TargetRuntime ParseRuntime(this string self)
		{
			if (string.IsNullOrEmpty(self))
			{
				return TargetRuntime.Net_4_0;
			}
			switch (self[1])
			{
			case '1':
				if (self[3] != '0')
				{
					return TargetRuntime.Net_1_1;
				}
				return TargetRuntime.Net_1_0;
			case '2':
				return TargetRuntime.Net_2_0;
			}
			return TargetRuntime.Net_4_0;
		}

		public static string RuntimeVersionString(this TargetRuntime runtime)
		{
			switch (runtime)
			{
			case TargetRuntime.Net_1_0:
				return "v1.0.3705";
			case TargetRuntime.Net_1_1:
				return "v1.1.4322";
			case TargetRuntime.Net_2_0:
				return "v2.0.50727";
			}
			return "v4.0.30319";
		}

		public static bool IsWindowsMetadata(this ModuleDefinition module)
		{
			return module.MetadataKind > MetadataKind.Ecma335;
		}

		public static byte[] ReadAll(this Stream self)
		{
			MemoryStream memoryStream = new MemoryStream((int)self.Length);
			byte[] array = new byte[1024];
			int num;
			while ((num = self.Read(array, 0, array.Length)) != 0)
			{
				memoryStream.Write(array, 0, num);
			}
			return memoryStream.ToArray();
		}

		public static void Read(object o)
		{
		}

		public static bool GetHasSecurityDeclarations(this ISecurityDeclarationProvider self, ModuleDefinition module)
		{
			if (module.HasImage())
			{
				return module.Read<ISecurityDeclarationProvider, bool>(self, (ISecurityDeclarationProvider provider, MetadataReader reader) => reader.HasSecurityDeclarations(provider));
			}
			return false;
		}

		public static Collection<SecurityDeclaration> GetSecurityDeclarations(this ISecurityDeclarationProvider self, ref Collection<SecurityDeclaration> variable, ModuleDefinition module)
		{
			if (module.HasImage)
			{
				return module.Read<ISecurityDeclarationProvider, Collection<SecurityDeclaration>>(ref variable, self, (ISecurityDeclarationProvider provider, MetadataReader reader) => reader.ReadSecurityDeclarations(provider));
			}
			Interlocked.CompareExchange<Collection<SecurityDeclaration>>(ref variable, new Collection<SecurityDeclaration>(), null);
			return variable;
		}

		public static TypeReference GetEnumUnderlyingType(this TypeDefinition self)
		{
			Collection<FieldDefinition> fields = self.Fields;
			for (int i = 0; i < fields.Count; i++)
			{
				FieldDefinition fieldDefinition = fields[i];
				if (!fieldDefinition.IsStatic)
				{
					return fieldDefinition.FieldType;
				}
			}
			throw new ArgumentException();
		}

		public static TypeDefinition GetNestedType(this TypeDefinition self, string fullname)
		{
			if (!self.HasNestedTypes)
			{
				return null;
			}
			Collection<TypeDefinition> nestedTypes = self.NestedTypes;
			for (int i = 0; i < nestedTypes.Count; i++)
			{
				TypeDefinition typeDefinition = nestedTypes[i];
				if (typeDefinition.TypeFullName() == fullname)
				{
					return typeDefinition;
				}
			}
			return null;
		}

		public static bool IsPrimitive(this ElementType self)
		{
			return self - ElementType.Boolean <= 11 || self - ElementType.I <= 1;
		}

		public static string TypeFullName(this TypeReference self)
		{
			if (!string.IsNullOrEmpty(self.Namespace))
			{
				return self.Namespace + "." + self.Name;
			}
			return self.Name;
		}

		public static bool IsTypeOf(this TypeReference self, string @namespace, string name)
		{
			return self.Name == name && self.Namespace == @namespace;
		}

		public static bool IsTypeSpecification(this TypeReference type)
		{
			ElementType etype = type.etype;
			switch (etype)
			{
			case ElementType.Ptr:
			case ElementType.ByRef:
			case ElementType.Var:
			case ElementType.Array:
			case ElementType.GenericInst:
			case ElementType.FnPtr:
			case ElementType.SzArray:
			case ElementType.MVar:
			case ElementType.CModReqD:
			case ElementType.CModOpt:
				break;
			case ElementType.ValueType:
			case ElementType.Class:
			case ElementType.TypedByRef:
			case (ElementType)23:
			case ElementType.I:
			case ElementType.U:
			case (ElementType)26:
			case ElementType.Object:
				return false;
			default:
				if (etype != ElementType.Sentinel && etype != ElementType.Pinned)
				{
					return false;
				}
				break;
			}
			return true;
		}

		public static TypeDefinition CheckedResolve(this TypeReference self)
		{
			TypeDefinition typeDefinition = self.Resolve();
			if (typeDefinition == null)
			{
				throw new ResolutionException(self);
			}
			return typeDefinition;
		}

		public static bool TryGetCoreLibraryReference(this ModuleDefinition module, out AssemblyNameReference reference)
		{
			Collection<AssemblyNameReference> assemblyReferences = module.AssemblyReferences;
			for (int i = 0; i < assemblyReferences.Count; i++)
			{
				reference = assemblyReferences[i];
				if (Mixin.IsCoreLibrary(reference))
				{
					return true;
				}
			}
			reference = null;
			return false;
		}

		public static bool IsCoreLibrary(this ModuleDefinition module)
		{
			if (module.Assembly == null)
			{
				return false;
			}
			if (!Mixin.IsCoreLibrary(module.Assembly.Name))
			{
				return false;
			}
			if (module.HasImage)
			{
				if (module.Read<ModuleDefinition, bool>(module, (ModuleDefinition m, MetadataReader reader) => reader.image.GetTableLength(Table.AssemblyRef) > 0))
				{
					return false;
				}
			}
			return true;
		}

		public static void KnownValueType(this TypeReference type)
		{
			if (!type.IsDefinition)
			{
				type.IsValueType = true;
			}
		}

		private static bool IsCoreLibrary(AssemblyNameReference reference)
		{
			string name = reference.Name;
			return name == "mscorlib" || name == "System.Runtime" || name == "System.Private.CoreLib" || name == "netstandard";
		}

		public static ImageDebugHeaderEntry GetCodeViewEntry(this ImageDebugHeader header)
		{
			return header.GetEntry(ImageDebugType.CodeView);
		}

		public static ImageDebugHeaderEntry GetDeterministicEntry(this ImageDebugHeader header)
		{
			return header.GetEntry(ImageDebugType.Deterministic);
		}

		public static ImageDebugHeader AddDeterministicEntry(this ImageDebugHeader header)
		{
			ImageDebugHeaderEntry imageDebugHeaderEntry = new ImageDebugHeaderEntry(new ImageDebugDirectory
			{
				Type = ImageDebugType.Deterministic
			}, Empty<byte>.Array);
			if (header == null)
			{
				return new ImageDebugHeader(imageDebugHeaderEntry);
			}
			ImageDebugHeaderEntry[] array = new ImageDebugHeaderEntry[header.Entries.Length + 1];
			Array.Copy(header.Entries, array, header.Entries.Length);
			array[array.Length - 1] = imageDebugHeaderEntry;
			return new ImageDebugHeader(array);
		}

		public static ImageDebugHeaderEntry GetEmbeddedPortablePdbEntry(this ImageDebugHeader header)
		{
			return header.GetEntry(ImageDebugType.EmbeddedPortablePdb);
		}

		private static ImageDebugHeaderEntry GetEntry(this ImageDebugHeader header, ImageDebugType type)
		{
			if (!header.HasEntries)
			{
				return null;
			}
			for (int i = 0; i < header.Entries.Length; i++)
			{
				ImageDebugHeaderEntry imageDebugHeaderEntry = header.Entries[i];
				if (imageDebugHeaderEntry.Directory.Type == type)
				{
					return imageDebugHeaderEntry;
				}
			}
			return null;
		}

		public static string GetPdbFileName(string assemblyFileName)
		{
			return Path.ChangeExtension(assemblyFileName, ".pdb");
		}

		public static string GetMdbFileName(string assemblyFileName)
		{
			return assemblyFileName + ".mdb";
		}

		public static bool IsPortablePdb(string fileName)
		{
			bool flag;
			using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				flag = Mixin.IsPortablePdb(fileStream);
			}
			return flag;
		}

		public static bool IsPortablePdb(Stream stream)
		{
			if (stream.Length < 4L)
			{
				return false;
			}
			long position = stream.Position;
			bool flag;
			try
			{
				flag = new BinaryReader(stream).ReadUInt32() == 1112167234U;
			}
			finally
			{
				stream.Position = position;
			}
			return flag;
		}

		public static uint ReadCompressedUInt32(this byte[] data, ref int position)
		{
			uint num;
			if ((data[position] & 128) == 0)
			{
				num = (uint)data[position];
				position++;
			}
			else if ((data[position] & 64) == 0)
			{
				num = ((uint)data[position] & 4294967167U) << 8;
				num |= (uint)data[position + 1];
				position += 2;
			}
			else
			{
				num = ((uint)data[position] & 4294967103U) << 24;
				num |= (uint)((uint)data[position + 1] << 16);
				num |= (uint)((uint)data[position + 2] << 8);
				num |= (uint)data[position + 3];
				position += 4;
			}
			return num;
		}

		public static MetadataToken GetMetadataToken(this CodedIndex self, uint data)
		{
			uint num;
			TokenType tokenType;
			switch (self)
			{
			case CodedIndex.TypeDefOrRef:
				num = data >> 2;
				switch (data & 3U)
				{
				case 0U:
					tokenType = TokenType.TypeDef;
					break;
				case 1U:
					tokenType = TokenType.TypeRef;
					break;
				case 2U:
					tokenType = TokenType.TypeSpec;
					break;
				default:
					goto IL_05BB;
				}
				break;
			case CodedIndex.HasConstant:
				num = data >> 2;
				switch (data & 3U)
				{
				case 0U:
					tokenType = TokenType.Field;
					break;
				case 1U:
					tokenType = TokenType.Param;
					break;
				case 2U:
					tokenType = TokenType.Property;
					break;
				default:
					goto IL_05BB;
				}
				break;
			case CodedIndex.HasCustomAttribute:
				num = data >> 5;
				switch (data & 31U)
				{
				case 0U:
					tokenType = TokenType.Method;
					break;
				case 1U:
					tokenType = TokenType.Field;
					break;
				case 2U:
					tokenType = TokenType.TypeRef;
					break;
				case 3U:
					tokenType = TokenType.TypeDef;
					break;
				case 4U:
					tokenType = TokenType.Param;
					break;
				case 5U:
					tokenType = TokenType.InterfaceImpl;
					break;
				case 6U:
					tokenType = TokenType.MemberRef;
					break;
				case 7U:
					tokenType = TokenType.Module;
					break;
				case 8U:
					tokenType = TokenType.Permission;
					break;
				case 9U:
					tokenType = TokenType.Property;
					break;
				case 10U:
					tokenType = TokenType.Event;
					break;
				case 11U:
					tokenType = TokenType.Signature;
					break;
				case 12U:
					tokenType = TokenType.ModuleRef;
					break;
				case 13U:
					tokenType = TokenType.TypeSpec;
					break;
				case 14U:
					tokenType = TokenType.Assembly;
					break;
				case 15U:
					tokenType = TokenType.AssemblyRef;
					break;
				case 16U:
					tokenType = TokenType.File;
					break;
				case 17U:
					tokenType = TokenType.ExportedType;
					break;
				case 18U:
					tokenType = TokenType.ManifestResource;
					break;
				case 19U:
					tokenType = TokenType.GenericParam;
					break;
				case 20U:
					tokenType = TokenType.GenericParamConstraint;
					break;
				case 21U:
					tokenType = TokenType.MethodSpec;
					break;
				default:
					goto IL_05BB;
				}
				break;
			case CodedIndex.HasFieldMarshal:
			{
				num = data >> 1;
				uint num2 = data & 1U;
				if (num2 != 0U)
				{
					if (num2 != 1U)
					{
						goto IL_05BB;
					}
					tokenType = TokenType.Param;
				}
				else
				{
					tokenType = TokenType.Field;
				}
				break;
			}
			case CodedIndex.HasDeclSecurity:
				num = data >> 2;
				switch (data & 3U)
				{
				case 0U:
					tokenType = TokenType.TypeDef;
					break;
				case 1U:
					tokenType = TokenType.Method;
					break;
				case 2U:
					tokenType = TokenType.Assembly;
					break;
				default:
					goto IL_05BB;
				}
				break;
			case CodedIndex.MemberRefParent:
				num = data >> 3;
				switch (data & 7U)
				{
				case 0U:
					tokenType = TokenType.TypeDef;
					break;
				case 1U:
					tokenType = TokenType.TypeRef;
					break;
				case 2U:
					tokenType = TokenType.ModuleRef;
					break;
				case 3U:
					tokenType = TokenType.Method;
					break;
				case 4U:
					tokenType = TokenType.TypeSpec;
					break;
				default:
					goto IL_05BB;
				}
				break;
			case CodedIndex.HasSemantics:
			{
				num = data >> 1;
				uint num2 = data & 1U;
				if (num2 != 0U)
				{
					if (num2 != 1U)
					{
						goto IL_05BB;
					}
					tokenType = TokenType.Property;
				}
				else
				{
					tokenType = TokenType.Event;
				}
				break;
			}
			case CodedIndex.MethodDefOrRef:
			{
				num = data >> 1;
				uint num2 = data & 1U;
				if (num2 != 0U)
				{
					if (num2 != 1U)
					{
						goto IL_05BB;
					}
					tokenType = TokenType.MemberRef;
				}
				else
				{
					tokenType = TokenType.Method;
				}
				break;
			}
			case CodedIndex.MemberForwarded:
			{
				num = data >> 1;
				uint num2 = data & 1U;
				if (num2 != 0U)
				{
					if (num2 != 1U)
					{
						goto IL_05BB;
					}
					tokenType = TokenType.Method;
				}
				else
				{
					tokenType = TokenType.Field;
				}
				break;
			}
			case CodedIndex.Implementation:
				num = data >> 2;
				switch (data & 3U)
				{
				case 0U:
					tokenType = TokenType.File;
					break;
				case 1U:
					tokenType = TokenType.AssemblyRef;
					break;
				case 2U:
					tokenType = TokenType.ExportedType;
					break;
				default:
					goto IL_05BB;
				}
				break;
			case CodedIndex.CustomAttributeType:
			{
				num = data >> 3;
				uint num2 = data & 7U;
				if (num2 != 2U)
				{
					if (num2 != 3U)
					{
						goto IL_05BB;
					}
					tokenType = TokenType.MemberRef;
				}
				else
				{
					tokenType = TokenType.Method;
				}
				break;
			}
			case CodedIndex.ResolutionScope:
				num = data >> 2;
				switch (data & 3U)
				{
				case 0U:
					tokenType = TokenType.Module;
					break;
				case 1U:
					tokenType = TokenType.ModuleRef;
					break;
				case 2U:
					tokenType = TokenType.AssemblyRef;
					break;
				case 3U:
					tokenType = TokenType.TypeRef;
					break;
				default:
					goto IL_05BB;
				}
				break;
			case CodedIndex.TypeOrMethodDef:
			{
				num = data >> 1;
				uint num2 = data & 1U;
				if (num2 != 0U)
				{
					if (num2 != 1U)
					{
						goto IL_05BB;
					}
					tokenType = TokenType.Method;
				}
				else
				{
					tokenType = TokenType.TypeDef;
				}
				break;
			}
			case CodedIndex.HasCustomDebugInformation:
				num = data >> 5;
				switch (data & 31U)
				{
				case 0U:
					tokenType = TokenType.Method;
					break;
				case 1U:
					tokenType = TokenType.Field;
					break;
				case 2U:
					tokenType = TokenType.TypeRef;
					break;
				case 3U:
					tokenType = TokenType.TypeDef;
					break;
				case 4U:
					tokenType = TokenType.Param;
					break;
				case 5U:
					tokenType = TokenType.InterfaceImpl;
					break;
				case 6U:
					tokenType = TokenType.MemberRef;
					break;
				case 7U:
					tokenType = TokenType.Module;
					break;
				case 8U:
					tokenType = TokenType.Permission;
					break;
				case 9U:
					tokenType = TokenType.Property;
					break;
				case 10U:
					tokenType = TokenType.Event;
					break;
				case 11U:
					tokenType = TokenType.Signature;
					break;
				case 12U:
					tokenType = TokenType.ModuleRef;
					break;
				case 13U:
					tokenType = TokenType.TypeSpec;
					break;
				case 14U:
					tokenType = TokenType.Assembly;
					break;
				case 15U:
					tokenType = TokenType.AssemblyRef;
					break;
				case 16U:
					tokenType = TokenType.File;
					break;
				case 17U:
					tokenType = TokenType.ExportedType;
					break;
				case 18U:
					tokenType = TokenType.ManifestResource;
					break;
				case 19U:
					tokenType = TokenType.GenericParam;
					break;
				case 20U:
					tokenType = TokenType.GenericParamConstraint;
					break;
				case 21U:
					tokenType = TokenType.MethodSpec;
					break;
				case 22U:
					tokenType = TokenType.Document;
					break;
				case 23U:
					tokenType = TokenType.LocalScope;
					break;
				case 24U:
					tokenType = TokenType.LocalVariable;
					break;
				case 25U:
					tokenType = TokenType.LocalConstant;
					break;
				case 26U:
					tokenType = TokenType.ImportScope;
					break;
				default:
					goto IL_05BB;
				}
				break;
			default:
				goto IL_05BB;
			}
			return new MetadataToken(tokenType, num);
			IL_05BB:
			return MetadataToken.Zero;
		}

		public static uint CompressMetadataToken(this CodedIndex self, MetadataToken token)
		{
			uint num = 0U;
			if (token.RID == 0U)
			{
				return num;
			}
			switch (self)
			{
			case CodedIndex.TypeDefOrRef:
			{
				num = token.RID << 2;
				TokenType tokenType = token.TokenType;
				if (tokenType == TokenType.TypeRef)
				{
					return num | 1U;
				}
				if (tokenType == TokenType.TypeDef)
				{
					return num | 0U;
				}
				if (tokenType == TokenType.TypeSpec)
				{
					return num | 2U;
				}
				break;
			}
			case CodedIndex.HasConstant:
			{
				num = token.RID << 2;
				TokenType tokenType = token.TokenType;
				if (tokenType == TokenType.Field)
				{
					return num | 0U;
				}
				if (tokenType == TokenType.Param)
				{
					return num | 1U;
				}
				if (tokenType == TokenType.Property)
				{
					return num | 2U;
				}
				break;
			}
			case CodedIndex.HasCustomAttribute:
			{
				num = token.RID << 5;
				TokenType tokenType = token.TokenType;
				if (tokenType <= TokenType.Event)
				{
					if (tokenType <= TokenType.Method)
					{
						if (tokenType <= TokenType.TypeRef)
						{
							if (tokenType == TokenType.Module)
							{
								return num | 7U;
							}
							if (tokenType == TokenType.TypeRef)
							{
								return num | 2U;
							}
						}
						else
						{
							if (tokenType == TokenType.TypeDef)
							{
								return num | 3U;
							}
							if (tokenType == TokenType.Field)
							{
								return num | 1U;
							}
							if (tokenType == TokenType.Method)
							{
								return num | 0U;
							}
						}
					}
					else if (tokenType <= TokenType.MemberRef)
					{
						if (tokenType == TokenType.Param)
						{
							return num | 4U;
						}
						if (tokenType == TokenType.InterfaceImpl)
						{
							return num | 5U;
						}
						if (tokenType == TokenType.MemberRef)
						{
							return num | 6U;
						}
					}
					else
					{
						if (tokenType == TokenType.Permission)
						{
							return num | 8U;
						}
						if (tokenType == TokenType.Signature)
						{
							return num | 11U;
						}
						if (tokenType == TokenType.Event)
						{
							return num | 10U;
						}
					}
				}
				else if (tokenType <= TokenType.AssemblyRef)
				{
					if (tokenType <= TokenType.ModuleRef)
					{
						if (tokenType == TokenType.Property)
						{
							return num | 9U;
						}
						if (tokenType == TokenType.ModuleRef)
						{
							return num | 12U;
						}
					}
					else
					{
						if (tokenType == TokenType.TypeSpec)
						{
							return num | 13U;
						}
						if (tokenType == TokenType.Assembly)
						{
							return num | 14U;
						}
						if (tokenType == TokenType.AssemblyRef)
						{
							return num | 15U;
						}
					}
				}
				else if (tokenType <= TokenType.ManifestResource)
				{
					if (tokenType == TokenType.File)
					{
						return num | 16U;
					}
					if (tokenType == TokenType.ExportedType)
					{
						return num | 17U;
					}
					if (tokenType == TokenType.ManifestResource)
					{
						return num | 18U;
					}
				}
				else
				{
					if (tokenType == TokenType.GenericParam)
					{
						return num | 19U;
					}
					if (tokenType == TokenType.MethodSpec)
					{
						return num | 21U;
					}
					if (tokenType == TokenType.GenericParamConstraint)
					{
						return num | 20U;
					}
				}
				break;
			}
			case CodedIndex.HasFieldMarshal:
			{
				num = token.RID << 1;
				TokenType tokenType = token.TokenType;
				if (tokenType == TokenType.Field)
				{
					return num | 0U;
				}
				if (tokenType == TokenType.Param)
				{
					return num | 1U;
				}
				break;
			}
			case CodedIndex.HasDeclSecurity:
			{
				num = token.RID << 2;
				TokenType tokenType = token.TokenType;
				if (tokenType == TokenType.TypeDef)
				{
					return num | 0U;
				}
				if (tokenType == TokenType.Method)
				{
					return num | 1U;
				}
				if (tokenType == TokenType.Assembly)
				{
					return num | 2U;
				}
				break;
			}
			case CodedIndex.MemberRefParent:
			{
				num = token.RID << 3;
				TokenType tokenType = token.TokenType;
				if (tokenType <= TokenType.TypeDef)
				{
					if (tokenType == TokenType.TypeRef)
					{
						return num | 1U;
					}
					if (tokenType == TokenType.TypeDef)
					{
						return num | 0U;
					}
				}
				else
				{
					if (tokenType == TokenType.Method)
					{
						return num | 3U;
					}
					if (tokenType == TokenType.ModuleRef)
					{
						return num | 2U;
					}
					if (tokenType == TokenType.TypeSpec)
					{
						return num | 4U;
					}
				}
				break;
			}
			case CodedIndex.HasSemantics:
			{
				num = token.RID << 1;
				TokenType tokenType = token.TokenType;
				if (tokenType == TokenType.Event)
				{
					return num | 0U;
				}
				if (tokenType == TokenType.Property)
				{
					return num | 1U;
				}
				break;
			}
			case CodedIndex.MethodDefOrRef:
			{
				num = token.RID << 1;
				TokenType tokenType = token.TokenType;
				if (tokenType == TokenType.Method)
				{
					return num | 0U;
				}
				if (tokenType == TokenType.MemberRef)
				{
					return num | 1U;
				}
				break;
			}
			case CodedIndex.MemberForwarded:
			{
				num = token.RID << 1;
				TokenType tokenType = token.TokenType;
				if (tokenType == TokenType.Field)
				{
					return num | 0U;
				}
				if (tokenType == TokenType.Method)
				{
					return num | 1U;
				}
				break;
			}
			case CodedIndex.Implementation:
			{
				num = token.RID << 2;
				TokenType tokenType = token.TokenType;
				if (tokenType == TokenType.AssemblyRef)
				{
					return num | 1U;
				}
				if (tokenType == TokenType.File)
				{
					return num | 0U;
				}
				if (tokenType == TokenType.ExportedType)
				{
					return num | 2U;
				}
				break;
			}
			case CodedIndex.CustomAttributeType:
			{
				num = token.RID << 3;
				TokenType tokenType = token.TokenType;
				if (tokenType == TokenType.Method)
				{
					return num | 2U;
				}
				if (tokenType == TokenType.MemberRef)
				{
					return num | 3U;
				}
				break;
			}
			case CodedIndex.ResolutionScope:
			{
				num = token.RID << 2;
				TokenType tokenType = token.TokenType;
				if (tokenType <= TokenType.TypeRef)
				{
					if (tokenType == TokenType.Module)
					{
						return num | 0U;
					}
					if (tokenType == TokenType.TypeRef)
					{
						return num | 3U;
					}
				}
				else
				{
					if (tokenType == TokenType.ModuleRef)
					{
						return num | 1U;
					}
					if (tokenType == TokenType.AssemblyRef)
					{
						return num | 2U;
					}
				}
				break;
			}
			case CodedIndex.TypeOrMethodDef:
			{
				num = token.RID << 1;
				TokenType tokenType = token.TokenType;
				if (tokenType == TokenType.TypeDef)
				{
					return num | 0U;
				}
				if (tokenType == TokenType.Method)
				{
					return num | 1U;
				}
				break;
			}
			case CodedIndex.HasCustomDebugInformation:
			{
				num = token.RID << 5;
				TokenType tokenType = token.TokenType;
				if (tokenType <= TokenType.ModuleRef)
				{
					if (tokenType <= TokenType.Param)
					{
						if (tokenType <= TokenType.TypeDef)
						{
							if (tokenType == TokenType.Module)
							{
								return num | 7U;
							}
							if (tokenType == TokenType.TypeRef)
							{
								return num | 2U;
							}
							if (tokenType == TokenType.TypeDef)
							{
								return num | 3U;
							}
						}
						else
						{
							if (tokenType == TokenType.Field)
							{
								return num | 1U;
							}
							if (tokenType == TokenType.Method)
							{
								return num | 0U;
							}
							if (tokenType == TokenType.Param)
							{
								return num | 4U;
							}
						}
					}
					else if (tokenType <= TokenType.Permission)
					{
						if (tokenType == TokenType.InterfaceImpl)
						{
							return num | 5U;
						}
						if (tokenType == TokenType.MemberRef)
						{
							return num | 6U;
						}
						if (tokenType == TokenType.Permission)
						{
							return num | 8U;
						}
					}
					else if (tokenType <= TokenType.Event)
					{
						if (tokenType == TokenType.Signature)
						{
							return num | 11U;
						}
						if (tokenType == TokenType.Event)
						{
							return num | 10U;
						}
					}
					else
					{
						if (tokenType == TokenType.Property)
						{
							return num | 9U;
						}
						if (tokenType == TokenType.ModuleRef)
						{
							return num | 12U;
						}
					}
				}
				else if (tokenType <= TokenType.GenericParam)
				{
					if (tokenType <= TokenType.AssemblyRef)
					{
						if (tokenType == TokenType.TypeSpec)
						{
							return num | 13U;
						}
						if (tokenType == TokenType.Assembly)
						{
							return num | 14U;
						}
						if (tokenType == TokenType.AssemblyRef)
						{
							return num | 15U;
						}
					}
					else if (tokenType <= TokenType.ExportedType)
					{
						if (tokenType == TokenType.File)
						{
							return num | 16U;
						}
						if (tokenType == TokenType.ExportedType)
						{
							return num | 17U;
						}
					}
					else
					{
						if (tokenType == TokenType.ManifestResource)
						{
							return num | 18U;
						}
						if (tokenType == TokenType.GenericParam)
						{
							return num | 19U;
						}
					}
				}
				else if (tokenType <= TokenType.Document)
				{
					if (tokenType == TokenType.MethodSpec)
					{
						return num | 21U;
					}
					if (tokenType == TokenType.GenericParamConstraint)
					{
						return num | 20U;
					}
					if (tokenType == TokenType.Document)
					{
						return num | 22U;
					}
				}
				else if (tokenType <= TokenType.LocalVariable)
				{
					if (tokenType == TokenType.LocalScope)
					{
						return num | 23U;
					}
					if (tokenType == TokenType.LocalVariable)
					{
						return num | 24U;
					}
				}
				else
				{
					if (tokenType == TokenType.LocalConstant)
					{
						return num | 25U;
					}
					if (tokenType == TokenType.ImportScope)
					{
						return num | 26U;
					}
				}
				break;
			}
			}
			throw new ArgumentException();
		}

		public static int GetSize(this CodedIndex self, Func<Table, int> counter)
		{
			int num;
			Table[] array;
			switch (self)
			{
			case CodedIndex.TypeDefOrRef:
				num = 2;
				array = new Table[]
				{
					Table.TypeDef,
					Table.TypeRef,
					Table.TypeSpec
				};
				break;
			case CodedIndex.HasConstant:
				num = 2;
				array = new Table[]
				{
					Table.Field,
					Table.Param,
					Table.Property
				};
				break;
			case CodedIndex.HasCustomAttribute:
				num = 5;
				array = new Table[]
				{
					Table.Method,
					Table.Field,
					Table.TypeRef,
					Table.TypeDef,
					Table.Param,
					Table.InterfaceImpl,
					Table.MemberRef,
					Table.Module,
					Table.DeclSecurity,
					Table.Property,
					Table.Event,
					Table.StandAloneSig,
					Table.ModuleRef,
					Table.TypeSpec,
					Table.Assembly,
					Table.AssemblyRef,
					Table.File,
					Table.ExportedType,
					Table.ManifestResource,
					Table.GenericParam,
					Table.GenericParamConstraint,
					Table.MethodSpec
				};
				break;
			case CodedIndex.HasFieldMarshal:
				num = 1;
				array = new Table[]
				{
					Table.Field,
					Table.Param
				};
				break;
			case CodedIndex.HasDeclSecurity:
				num = 2;
				array = new Table[]
				{
					Table.TypeDef,
					Table.Method,
					Table.Assembly
				};
				break;
			case CodedIndex.MemberRefParent:
				num = 3;
				array = new Table[]
				{
					Table.TypeDef,
					Table.TypeRef,
					Table.ModuleRef,
					Table.Method,
					Table.TypeSpec
				};
				break;
			case CodedIndex.HasSemantics:
				num = 1;
				array = new Table[]
				{
					Table.Event,
					Table.Property
				};
				break;
			case CodedIndex.MethodDefOrRef:
				num = 1;
				array = new Table[]
				{
					Table.Method,
					Table.MemberRef
				};
				break;
			case CodedIndex.MemberForwarded:
				num = 1;
				array = new Table[]
				{
					Table.Field,
					Table.Method
				};
				break;
			case CodedIndex.Implementation:
				num = 2;
				array = new Table[]
				{
					Table.File,
					Table.AssemblyRef,
					Table.ExportedType
				};
				break;
			case CodedIndex.CustomAttributeType:
				num = 3;
				array = new Table[]
				{
					Table.Method,
					Table.MemberRef
				};
				break;
			case CodedIndex.ResolutionScope:
				num = 2;
				array = new Table[]
				{
					Table.Module,
					Table.ModuleRef,
					Table.AssemblyRef,
					Table.TypeRef
				};
				break;
			case CodedIndex.TypeOrMethodDef:
				num = 1;
				array = new Table[]
				{
					Table.TypeDef,
					Table.Method
				};
				break;
			case CodedIndex.HasCustomDebugInformation:
				num = 5;
				array = new Table[]
				{
					Table.Method,
					Table.Field,
					Table.TypeRef,
					Table.TypeDef,
					Table.Param,
					Table.InterfaceImpl,
					Table.MemberRef,
					Table.Module,
					Table.DeclSecurity,
					Table.Property,
					Table.Event,
					Table.StandAloneSig,
					Table.ModuleRef,
					Table.TypeSpec,
					Table.Assembly,
					Table.AssemblyRef,
					Table.File,
					Table.ExportedType,
					Table.ManifestResource,
					Table.GenericParam,
					Table.GenericParamConstraint,
					Table.MethodSpec,
					Table.Document,
					Table.LocalScope,
					Table.LocalVariable,
					Table.LocalConstant,
					Table.ImportScope
				};
				break;
			default:
				throw new ArgumentException();
			}
			int num2 = 0;
			for (int i = 0; i < array.Length; i++)
			{
				num2 = Math.Max(counter(array[i]), num2);
			}
			if (num2 >= 1 << 16 - num)
			{
				return 4;
			}
			return 2;
		}

		public static RSA CreateRSA(this WriterParameters writer_parameters)
		{
			if (writer_parameters.StrongNameKeyBlob != null)
			{
				return CryptoConvert.FromCapiKeyBlob(writer_parameters.StrongNameKeyBlob);
			}
			string strongNameKeyContainer;
			byte[] array;
			if (writer_parameters.StrongNameKeyContainer != null)
			{
				strongNameKeyContainer = writer_parameters.StrongNameKeyContainer;
			}
			else if (!Mixin.TryGetKeyContainer(writer_parameters.StrongNameKeyPair, out array, out strongNameKeyContainer))
			{
				return CryptoConvert.FromCapiKeyBlob(array);
			}
			return new RSACryptoServiceProvider(new CspParameters
			{
				Flags = CspProviderFlags.UseMachineKeyStore,
				KeyContainerName = strongNameKeyContainer,
				KeyNumber = 2
			});
		}

		private static bool TryGetKeyContainer(ISerializable key_pair, out byte[] key, out string key_container)
		{
			SerializationInfo serializationInfo = new SerializationInfo(typeof(StrongNameKeyPair), new FormatterConverter());
			key_pair.GetObjectData(serializationInfo, default(StreamingContext));
			key = (byte[])serializationInfo.GetValue("_keyPairArray", typeof(byte[]));
			key_container = serializationInfo.GetString("_keyPairContainer");
			return key_container != null;
		}

		public static Version ZeroVersion = new Version(0, 0, 0, 0);

		public const int NotResolvedMarker = -2;

		public const int NoDataMarker = -1;

		internal static object NoValue = new object();

		internal static object NotResolved = new object();

		public const string mscorlib = "mscorlib";

		public const string system_runtime = "System.Runtime";

		public const string system_private_corelib = "System.Private.CoreLib";

		public const string netstandard = "netstandard";

		public const int TableCount = 58;

		public const int CodedIndexCount = 14;

		public enum Argument
		{
			name,
			fileName,
			fullName,
			stream,
			type,
			method,
			field,
			parameters,
			module,
			modifierType,
			eventType,
			fieldType,
			declaringType,
			returnType,
			propertyType,
			interfaceType,
			constraintType
		}
	}
}
