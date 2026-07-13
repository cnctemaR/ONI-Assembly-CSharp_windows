using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using Mono.Cecil.Cil;
using Mono.Cecil.Metadata;
using Mono.Cecil.PE;
using Mono.Collections.Generic;

namespace Mono.Cecil
{
	internal sealed class MetadataBuilder
	{
		public MetadataBuilder(ModuleDefinition module, string fq_name, uint timestamp, ISymbolWriterProvider symbol_writer_provider)
		{
			this.module = module;
			this.text_map = this.CreateTextMap();
			this.fq_name = fq_name;
			this.timestamp = timestamp;
			this.symbol_writer_provider = symbol_writer_provider;
			this.code = new CodeWriter(this);
			this.data = new DataBuffer();
			this.resources = new ResourceBuffer();
			this.string_heap = new StringHeapBuffer();
			this.guid_heap = new GuidHeapBuffer();
			this.user_string_heap = new UserStringHeapBuffer();
			this.blob_heap = new BlobHeapBuffer();
			this.table_heap = new TableHeapBuffer(module, this);
			this.type_ref_table = this.GetTable<TypeRefTable>(Table.TypeRef);
			this.type_def_table = this.GetTable<TypeDefTable>(Table.TypeDef);
			this.field_table = this.GetTable<FieldTable>(Table.Field);
			this.method_table = this.GetTable<MethodTable>(Table.Method);
			this.param_table = this.GetTable<ParamTable>(Table.Param);
			this.iface_impl_table = this.GetTable<InterfaceImplTable>(Table.InterfaceImpl);
			this.member_ref_table = this.GetTable<MemberRefTable>(Table.MemberRef);
			this.constant_table = this.GetTable<ConstantTable>(Table.Constant);
			this.custom_attribute_table = this.GetTable<CustomAttributeTable>(Table.CustomAttribute);
			this.declsec_table = this.GetTable<DeclSecurityTable>(Table.DeclSecurity);
			this.standalone_sig_table = this.GetTable<StandAloneSigTable>(Table.StandAloneSig);
			this.event_map_table = this.GetTable<EventMapTable>(Table.EventMap);
			this.event_table = this.GetTable<EventTable>(Table.Event);
			this.property_map_table = this.GetTable<PropertyMapTable>(Table.PropertyMap);
			this.property_table = this.GetTable<PropertyTable>(Table.Property);
			this.typespec_table = this.GetTable<TypeSpecTable>(Table.TypeSpec);
			this.method_spec_table = this.GetTable<MethodSpecTable>(Table.MethodSpec);
			RowEqualityComparer rowEqualityComparer = new RowEqualityComparer();
			this.type_ref_map = new Dictionary<Row<uint, uint, uint>, MetadataToken>(rowEqualityComparer);
			this.type_spec_map = new Dictionary<uint, MetadataToken>();
			this.member_ref_map = new Dictionary<Row<uint, uint, uint>, MetadataToken>(rowEqualityComparer);
			this.method_spec_map = new Dictionary<Row<uint, uint>, MetadataToken>(rowEqualityComparer);
			this.generic_parameters = new Collection<GenericParameter>();
			this.document_table = this.GetTable<DocumentTable>(Table.Document);
			this.method_debug_information_table = this.GetTable<MethodDebugInformationTable>(Table.MethodDebugInformation);
			this.local_scope_table = this.GetTable<LocalScopeTable>(Table.LocalScope);
			this.local_variable_table = this.GetTable<LocalVariableTable>(Table.LocalVariable);
			this.local_constant_table = this.GetTable<LocalConstantTable>(Table.LocalConstant);
			this.import_scope_table = this.GetTable<ImportScopeTable>(Table.ImportScope);
			this.state_machine_method_table = this.GetTable<StateMachineMethodTable>(Table.StateMachineMethod);
			this.custom_debug_information_table = this.GetTable<CustomDebugInformationTable>(Table.CustomDebugInformation);
			this.document_map = new Dictionary<string, MetadataToken>(StringComparer.Ordinal);
			this.import_scope_map = new Dictionary<Row<uint, uint>, MetadataToken>(rowEqualityComparer);
		}

		public MetadataBuilder(ModuleDefinition module, PortablePdbWriterProvider writer_provider)
		{
			this.module = module;
			this.text_map = new TextMap();
			this.symbol_writer_provider = writer_provider;
			this.string_heap = new StringHeapBuffer();
			this.guid_heap = new GuidHeapBuffer();
			this.user_string_heap = new UserStringHeapBuffer();
			this.blob_heap = new BlobHeapBuffer();
			this.table_heap = new TableHeapBuffer(module, this);
			this.pdb_heap = new PdbHeapBuffer();
			this.document_table = this.GetTable<DocumentTable>(Table.Document);
			this.method_debug_information_table = this.GetTable<MethodDebugInformationTable>(Table.MethodDebugInformation);
			this.local_scope_table = this.GetTable<LocalScopeTable>(Table.LocalScope);
			this.local_variable_table = this.GetTable<LocalVariableTable>(Table.LocalVariable);
			this.local_constant_table = this.GetTable<LocalConstantTable>(Table.LocalConstant);
			this.import_scope_table = this.GetTable<ImportScopeTable>(Table.ImportScope);
			this.state_machine_method_table = this.GetTable<StateMachineMethodTable>(Table.StateMachineMethod);
			this.custom_debug_information_table = this.GetTable<CustomDebugInformationTable>(Table.CustomDebugInformation);
			RowEqualityComparer rowEqualityComparer = new RowEqualityComparer();
			this.document_map = new Dictionary<string, MetadataToken>();
			this.import_scope_map = new Dictionary<Row<uint, uint>, MetadataToken>(rowEqualityComparer);
		}

		public void SetSymbolWriter(ISymbolWriter writer)
		{
			this.symbol_writer = writer;
			if (this.symbol_writer == null && this.module.HasImage && this.module.Image.HasDebugTables())
			{
				this.symbol_writer = new PortablePdbWriter(this, this.module);
			}
		}

		private TextMap CreateTextMap()
		{
			TextMap textMap = new TextMap();
			textMap.AddMap(TextSegment.ImportAddressTable, (this.module.Architecture == TargetArchitecture.I386) ? 8 : 0);
			textMap.AddMap(TextSegment.CLIHeader, 72, 8);
			bool flag = this.module.Architecture == TargetArchitecture.AMD64 || this.module.Architecture == TargetArchitecture.IA64 || this.module.Architecture == TargetArchitecture.ARM64;
			textMap.AddMap(TextSegment.Code, 0, (!flag) ? 4 : 16);
			return textMap;
		}

		private TTable GetTable<TTable>(Table table) where TTable : MetadataTable, new()
		{
			return this.table_heap.GetTable<TTable>(table);
		}

		private uint GetStringIndex(string @string)
		{
			if (string.IsNullOrEmpty(@string))
			{
				return 0U;
			}
			return this.string_heap.GetStringIndex(@string);
		}

		private uint GetGuidIndex(Guid guid)
		{
			return this.guid_heap.GetGuidIndex(guid);
		}

		private uint GetBlobIndex(ByteBuffer blob)
		{
			if (blob.length == 0)
			{
				return 0U;
			}
			return this.blob_heap.GetBlobIndex(blob);
		}

		private uint GetBlobIndex(byte[] blob)
		{
			if (blob.IsNullOrEmpty<byte>())
			{
				return 0U;
			}
			return this.GetBlobIndex(new ByteBuffer(blob));
		}

		public void BuildMetadata()
		{
			this.BuildModule();
			this.table_heap.string_offsets = this.string_heap.WriteStrings();
			this.table_heap.ComputeTableInformations();
			this.table_heap.WriteTableHeap();
		}

		private void BuildModule()
		{
			ModuleTable table = this.GetTable<ModuleTable>(Table.Module);
			table.row.Col1 = this.GetStringIndex(this.module.Name);
			table.row.Col2 = this.GetGuidIndex(this.module.Mvid);
			AssemblyDefinition assembly = this.module.Assembly;
			if (this.module.kind != ModuleKind.NetModule && assembly != null)
			{
				this.BuildAssembly();
			}
			if (this.module.HasAssemblyReferences)
			{
				this.AddAssemblyReferences();
			}
			if (this.module.HasModuleReferences)
			{
				this.AddModuleReferences();
			}
			if (this.module.HasResources)
			{
				this.AddResources();
			}
			if (this.module.HasExportedTypes)
			{
				this.AddExportedTypes();
			}
			this.BuildTypes();
			if (this.module.kind != ModuleKind.NetModule && assembly != null)
			{
				if (assembly.HasCustomAttributes)
				{
					this.AddCustomAttributes(assembly);
				}
				if (assembly.HasSecurityDeclarations)
				{
					this.AddSecurityDeclarations(assembly);
				}
			}
			if (this.module.HasCustomAttributes)
			{
				this.AddCustomAttributes(this.module);
			}
			if (this.module.EntryPoint != null)
			{
				this.entry_point = this.LookupToken(this.module.EntryPoint);
			}
		}

		private void BuildAssembly()
		{
			AssemblyDefinition assembly = this.module.Assembly;
			AssemblyNameDefinition name = assembly.Name;
			this.GetTable<AssemblyTable>(Table.Assembly).row = new Row<AssemblyHashAlgorithm, ushort, ushort, ushort, ushort, AssemblyAttributes, uint, uint, uint>(name.HashAlgorithm, (ushort)name.Version.Major, (ushort)name.Version.Minor, (ushort)name.Version.Build, (ushort)name.Version.Revision, name.Attributes, this.GetBlobIndex(name.PublicKey), this.GetStringIndex(name.Name), this.GetStringIndex(name.Culture));
			if (assembly.Modules.Count > 1)
			{
				this.BuildModules();
			}
		}

		private void BuildModules()
		{
			Collection<ModuleDefinition> modules = this.module.Assembly.Modules;
			FileTable table = this.GetTable<FileTable>(Table.File);
			for (int i = 0; i < modules.Count; i++)
			{
				ModuleDefinition moduleDefinition = modules[i];
				if (!moduleDefinition.IsMain)
				{
					WriterParameters writerParameters = new WriterParameters
					{
						SymbolWriterProvider = this.symbol_writer_provider
					};
					string moduleFileName = this.GetModuleFileName(moduleDefinition.Name);
					moduleDefinition.Write(moduleFileName, writerParameters);
					byte[] array = CryptoService.ComputeHash(moduleFileName);
					table.AddRow(new Row<FileAttributes, uint, uint>(FileAttributes.ContainsMetaData, this.GetStringIndex(moduleDefinition.Name), this.GetBlobIndex(array)));
				}
			}
		}

		private string GetModuleFileName(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new NotSupportedException();
			}
			return Path.Combine(Path.GetDirectoryName(this.fq_name), name);
		}

		private void AddAssemblyReferences()
		{
			Collection<AssemblyNameReference> assemblyReferences = this.module.AssemblyReferences;
			AssemblyRefTable table = this.GetTable<AssemblyRefTable>(Table.AssemblyRef);
			if (this.module.IsWindowsMetadata())
			{
				this.module.Projections.RemoveVirtualReferences(assemblyReferences);
			}
			for (int i = 0; i < assemblyReferences.Count; i++)
			{
				AssemblyNameReference assemblyNameReference = assemblyReferences[i];
				byte[] array = (assemblyNameReference.PublicKey.IsNullOrEmpty<byte>() ? assemblyNameReference.PublicKeyToken : assemblyNameReference.PublicKey);
				Version version = assemblyNameReference.Version;
				int num = table.AddRow(new Row<ushort, ushort, ushort, ushort, AssemblyAttributes, uint, uint, uint, uint>((ushort)version.Major, (ushort)version.Minor, (ushort)version.Build, (ushort)version.Revision, assemblyNameReference.Attributes, this.GetBlobIndex(array), this.GetStringIndex(assemblyNameReference.Name), this.GetStringIndex(assemblyNameReference.Culture), this.GetBlobIndex(assemblyNameReference.Hash)));
				assemblyNameReference.token = new MetadataToken(TokenType.AssemblyRef, num);
			}
			if (this.module.IsWindowsMetadata())
			{
				this.module.Projections.AddVirtualReferences(assemblyReferences);
			}
		}

		private void AddModuleReferences()
		{
			Collection<ModuleReference> moduleReferences = this.module.ModuleReferences;
			ModuleRefTable table = this.GetTable<ModuleRefTable>(Table.ModuleRef);
			for (int i = 0; i < moduleReferences.Count; i++)
			{
				ModuleReference moduleReference = moduleReferences[i];
				moduleReference.token = new MetadataToken(TokenType.ModuleRef, table.AddRow(this.GetStringIndex(moduleReference.Name)));
			}
		}

		private void AddResources()
		{
			Collection<Resource> collection = this.module.Resources;
			ManifestResourceTable table = this.GetTable<ManifestResourceTable>(Table.ManifestResource);
			for (int i = 0; i < collection.Count; i++)
			{
				Resource resource = collection[i];
				Row<uint, ManifestResourceAttributes, uint, uint> row = new Row<uint, ManifestResourceAttributes, uint, uint>(0U, resource.Attributes, this.GetStringIndex(resource.Name), 0U);
				switch (resource.ResourceType)
				{
				case ResourceType.Linked:
					row.Col4 = CodedIndex.Implementation.CompressMetadataToken(new MetadataToken(TokenType.File, this.AddLinkedResource((LinkedResource)resource)));
					break;
				case ResourceType.Embedded:
					row.Col1 = this.AddEmbeddedResource((EmbeddedResource)resource);
					break;
				case ResourceType.AssemblyLinked:
					row.Col4 = CodedIndex.Implementation.CompressMetadataToken(((AssemblyLinkedResource)resource).Assembly.MetadataToken);
					break;
				default:
					throw new NotSupportedException();
				}
				table.AddRow(row);
			}
		}

		private uint AddLinkedResource(LinkedResource resource)
		{
			MetadataTable<Row<FileAttributes, uint, uint>> table = this.GetTable<FileTable>(Table.File);
			byte[] array = resource.Hash;
			if (array.IsNullOrEmpty<byte>())
			{
				array = CryptoService.ComputeHash(resource.File);
			}
			return (uint)table.AddRow(new Row<FileAttributes, uint, uint>(FileAttributes.ContainsNoMetaData, this.GetStringIndex(resource.File), this.GetBlobIndex(array)));
		}

		private uint AddEmbeddedResource(EmbeddedResource resource)
		{
			return this.resources.AddResource(resource.GetResourceData());
		}

		private void AddExportedTypes()
		{
			Collection<ExportedType> exportedTypes = this.module.ExportedTypes;
			ExportedTypeTable table = this.GetTable<ExportedTypeTable>(Table.ExportedType);
			for (int i = 0; i < exportedTypes.Count; i++)
			{
				ExportedType exportedType = exportedTypes[i];
				int num = table.AddRow(new Row<TypeAttributes, uint, uint, uint, uint>(exportedType.Attributes, (uint)exportedType.Identifier, this.GetStringIndex(exportedType.Name), this.GetStringIndex(exportedType.Namespace), MetadataBuilder.MakeCodedRID(this.GetExportedTypeScope(exportedType), CodedIndex.Implementation)));
				exportedType.token = new MetadataToken(TokenType.ExportedType, num);
			}
		}

		private MetadataToken GetExportedTypeScope(ExportedType exported_type)
		{
			if (exported_type.DeclaringType != null)
			{
				return exported_type.DeclaringType.MetadataToken;
			}
			IMetadataScope scope = exported_type.Scope;
			TokenType tokenType = scope.MetadataToken.TokenType;
			if (tokenType != TokenType.ModuleRef)
			{
				if (tokenType == TokenType.AssemblyRef)
				{
					return scope.MetadataToken;
				}
			}
			else
			{
				FileTable table = this.GetTable<FileTable>(Table.File);
				for (int i = 0; i < table.length; i++)
				{
					if (table.rows[i].Col2 == this.GetStringIndex(scope.Name))
					{
						return new MetadataToken(TokenType.File, i + 1);
					}
				}
			}
			throw new NotSupportedException();
		}

		private void BuildTypes()
		{
			if (!this.module.HasTypes)
			{
				return;
			}
			this.AttachTokens();
			this.AddTypes();
			this.AddGenericParameters();
		}

		private void AttachTokens()
		{
			Collection<TypeDefinition> types = this.module.Types;
			for (int i = 0; i < types.Count; i++)
			{
				this.AttachTypeToken(types[i]);
			}
		}

		private void AttachTypeToken(TypeDefinition type)
		{
			TypeDefinitionProjection typeDefinitionProjection = WindowsRuntimeProjections.RemoveProjection(type);
			TokenType tokenType = TokenType.TypeDef;
			uint num = this.type_rid;
			this.type_rid = num + 1U;
			type.token = new MetadataToken(tokenType, num);
			type.fields_range.Start = this.field_rid;
			type.methods_range.Start = this.method_rid;
			if (type.HasFields)
			{
				this.AttachFieldsToken(type);
			}
			if (type.HasMethods)
			{
				this.AttachMethodsToken(type);
			}
			if (type.HasNestedTypes)
			{
				this.AttachNestedTypesToken(type);
			}
			WindowsRuntimeProjections.ApplyProjection(type, typeDefinitionProjection);
		}

		private void AttachNestedTypesToken(TypeDefinition type)
		{
			Collection<TypeDefinition> nestedTypes = type.NestedTypes;
			for (int i = 0; i < nestedTypes.Count; i++)
			{
				this.AttachTypeToken(nestedTypes[i]);
			}
		}

		private void AttachFieldsToken(TypeDefinition type)
		{
			Collection<FieldDefinition> fields = type.Fields;
			type.fields_range.Length = (uint)fields.Count;
			for (int i = 0; i < fields.Count; i++)
			{
				MemberReference memberReference = fields[i];
				TokenType tokenType = TokenType.Field;
				uint num = this.field_rid;
				this.field_rid = num + 1U;
				memberReference.token = new MetadataToken(tokenType, num);
			}
		}

		private void AttachMethodsToken(TypeDefinition type)
		{
			Collection<MethodDefinition> methods = type.Methods;
			type.methods_range.Length = (uint)methods.Count;
			for (int i = 0; i < methods.Count; i++)
			{
				MemberReference memberReference = methods[i];
				TokenType tokenType = TokenType.Method;
				uint num = this.method_rid;
				this.method_rid = num + 1U;
				memberReference.token = new MetadataToken(tokenType, num);
			}
		}

		private MetadataToken GetTypeToken(TypeReference type)
		{
			if (type == null)
			{
				return MetadataToken.Zero;
			}
			if (type.IsDefinition)
			{
				return type.token;
			}
			if (type.IsTypeSpecification())
			{
				return this.GetTypeSpecToken(type);
			}
			return this.GetTypeRefToken(type);
		}

		private MetadataToken GetTypeSpecToken(TypeReference type)
		{
			uint blobIndex = this.GetBlobIndex(this.GetTypeSpecSignature(type));
			MetadataToken metadataToken;
			if (this.type_spec_map.TryGetValue(blobIndex, out metadataToken))
			{
				return metadataToken;
			}
			return this.AddTypeSpecification(type, blobIndex);
		}

		private MetadataToken AddTypeSpecification(TypeReference type, uint row)
		{
			type.token = new MetadataToken(TokenType.TypeSpec, this.typespec_table.AddRow(row));
			MetadataToken token = type.token;
			this.type_spec_map.Add(row, token);
			return token;
		}

		private MetadataToken GetTypeRefToken(TypeReference type)
		{
			TypeReferenceProjection typeReferenceProjection = WindowsRuntimeProjections.RemoveProjection(type);
			Row<uint, uint, uint> row = this.CreateTypeRefRow(type);
			MetadataToken metadataToken;
			if (!this.type_ref_map.TryGetValue(row, out metadataToken))
			{
				metadataToken = this.AddTypeReference(type, row);
			}
			WindowsRuntimeProjections.ApplyProjection(type, typeReferenceProjection);
			return metadataToken;
		}

		private Row<uint, uint, uint> CreateTypeRefRow(TypeReference type)
		{
			return new Row<uint, uint, uint>(MetadataBuilder.MakeCodedRID(this.GetScopeToken(type), CodedIndex.ResolutionScope), this.GetStringIndex(type.Name), this.GetStringIndex(type.Namespace));
		}

		private MetadataToken GetScopeToken(TypeReference type)
		{
			if (type.IsNested)
			{
				return this.GetTypeRefToken(type.DeclaringType);
			}
			IMetadataScope scope = type.Scope;
			if (scope == null)
			{
				return MetadataToken.Zero;
			}
			return scope.MetadataToken;
		}

		private static uint MakeCodedRID(IMetadataTokenProvider provider, CodedIndex index)
		{
			return MetadataBuilder.MakeCodedRID(provider.MetadataToken, index);
		}

		private static uint MakeCodedRID(MetadataToken token, CodedIndex index)
		{
			return index.CompressMetadataToken(token);
		}

		private MetadataToken AddTypeReference(TypeReference type, Row<uint, uint, uint> row)
		{
			type.token = new MetadataToken(TokenType.TypeRef, this.type_ref_table.AddRow(row));
			MetadataToken token = type.token;
			this.type_ref_map.Add(row, token);
			return token;
		}

		private void AddTypes()
		{
			Collection<TypeDefinition> types = this.module.Types;
			for (int i = 0; i < types.Count; i++)
			{
				this.AddType(types[i]);
			}
		}

		private void AddType(TypeDefinition type)
		{
			TypeDefinitionProjection typeDefinitionProjection = WindowsRuntimeProjections.RemoveProjection(type);
			this.type_def_table.AddRow(new Row<TypeAttributes, uint, uint, uint, uint, uint>(type.Attributes, this.GetStringIndex(type.Name), this.GetStringIndex(type.Namespace), MetadataBuilder.MakeCodedRID(this.GetTypeToken(type.BaseType), CodedIndex.TypeDefOrRef), type.fields_range.Start, type.methods_range.Start));
			if (type.HasGenericParameters)
			{
				this.AddGenericParameters(type);
			}
			if (type.HasInterfaces)
			{
				this.AddInterfaces(type);
			}
			if (type.HasLayoutInfo)
			{
				this.AddLayoutInfo(type);
			}
			if (type.HasFields)
			{
				this.AddFields(type);
			}
			if (type.HasMethods)
			{
				this.AddMethods(type);
			}
			if (type.HasProperties)
			{
				this.AddProperties(type);
			}
			if (type.HasEvents)
			{
				this.AddEvents(type);
			}
			if (type.HasCustomAttributes)
			{
				this.AddCustomAttributes(type);
			}
			if (type.HasSecurityDeclarations)
			{
				this.AddSecurityDeclarations(type);
			}
			if (type.HasNestedTypes)
			{
				this.AddNestedTypes(type);
			}
			if (this.symbol_writer != null && type.HasCustomDebugInformations)
			{
				this.symbol_writer.Write(type);
			}
			WindowsRuntimeProjections.ApplyProjection(type, typeDefinitionProjection);
		}

		private void AddGenericParameters(IGenericParameterProvider owner)
		{
			Collection<GenericParameter> genericParameters = owner.GenericParameters;
			for (int i = 0; i < genericParameters.Count; i++)
			{
				this.generic_parameters.Add(genericParameters[i]);
			}
		}

		private void AddGenericParameters()
		{
			GenericParameter[] items = this.generic_parameters.items;
			int size = this.generic_parameters.size;
			Array.Sort<GenericParameter>(items, 0, size, new MetadataBuilder.GenericParameterComparer());
			GenericParamTable table = this.GetTable<GenericParamTable>(Table.GenericParam);
			GenericParamConstraintTable table2 = this.GetTable<GenericParamConstraintTable>(Table.GenericParamConstraint);
			for (int i = 0; i < size; i++)
			{
				GenericParameter genericParameter = items[i];
				int num = table.AddRow(new Row<ushort, GenericParameterAttributes, uint, uint>((ushort)genericParameter.Position, genericParameter.Attributes, MetadataBuilder.MakeCodedRID(genericParameter.Owner, CodedIndex.TypeOrMethodDef), this.GetStringIndex(genericParameter.Name)));
				genericParameter.token = new MetadataToken(TokenType.GenericParam, num);
				if (genericParameter.HasConstraints)
				{
					this.AddConstraints(genericParameter, table2);
				}
				if (genericParameter.HasCustomAttributes)
				{
					this.AddCustomAttributes(genericParameter);
				}
			}
		}

		private void AddConstraints(GenericParameter generic_parameter, GenericParamConstraintTable table)
		{
			Collection<GenericParameterConstraint> constraints = generic_parameter.Constraints;
			uint rid = generic_parameter.token.RID;
			for (int i = 0; i < constraints.Count; i++)
			{
				GenericParameterConstraint genericParameterConstraint = constraints[i];
				int num = table.AddRow(new Row<uint, uint>(rid, MetadataBuilder.MakeCodedRID(this.GetTypeToken(genericParameterConstraint.ConstraintType), CodedIndex.TypeDefOrRef)));
				genericParameterConstraint.token = new MetadataToken(TokenType.GenericParamConstraint, num);
				if (genericParameterConstraint.HasCustomAttributes)
				{
					this.AddCustomAttributes(genericParameterConstraint);
				}
			}
		}

		private void AddInterfaces(TypeDefinition type)
		{
			Collection<InterfaceImplementation> interfaces = type.Interfaces;
			uint rid = type.token.RID;
			for (int i = 0; i < interfaces.Count; i++)
			{
				InterfaceImplementation interfaceImplementation = interfaces[i];
				int num = this.iface_impl_table.AddRow(new Row<uint, uint>(rid, MetadataBuilder.MakeCodedRID(this.GetTypeToken(interfaceImplementation.InterfaceType), CodedIndex.TypeDefOrRef)));
				interfaceImplementation.token = new MetadataToken(TokenType.InterfaceImpl, num);
				if (interfaceImplementation.HasCustomAttributes)
				{
					this.AddCustomAttributes(interfaceImplementation);
				}
			}
		}

		private void AddLayoutInfo(TypeDefinition type)
		{
			this.GetTable<ClassLayoutTable>(Table.ClassLayout).AddRow(new Row<ushort, uint, uint>((ushort)type.PackingSize, (uint)type.ClassSize, type.token.RID));
		}

		private void AddNestedTypes(TypeDefinition type)
		{
			Collection<TypeDefinition> nestedTypes = type.NestedTypes;
			NestedClassTable table = this.GetTable<NestedClassTable>(Table.NestedClass);
			for (int i = 0; i < nestedTypes.Count; i++)
			{
				TypeDefinition typeDefinition = nestedTypes[i];
				this.AddType(typeDefinition);
				table.AddRow(new Row<uint, uint>(typeDefinition.token.RID, type.token.RID));
			}
		}

		private void AddFields(TypeDefinition type)
		{
			Collection<FieldDefinition> fields = type.Fields;
			for (int i = 0; i < fields.Count; i++)
			{
				this.AddField(fields[i]);
			}
		}

		private void AddField(FieldDefinition field)
		{
			FieldDefinitionProjection fieldDefinitionProjection = WindowsRuntimeProjections.RemoveProjection(field);
			this.field_table.AddRow(new Row<FieldAttributes, uint, uint>(field.Attributes, this.GetStringIndex(field.Name), this.GetBlobIndex(this.GetFieldSignature(field))));
			if (!field.InitialValue.IsNullOrEmpty<byte>())
			{
				this.AddFieldRVA(field);
			}
			if (field.HasLayoutInfo)
			{
				this.AddFieldLayout(field);
			}
			if (field.HasCustomAttributes)
			{
				this.AddCustomAttributes(field);
			}
			if (field.HasConstant)
			{
				this.AddConstant(field, field.FieldType);
			}
			if (field.HasMarshalInfo)
			{
				this.AddMarshalInfo(field);
			}
			WindowsRuntimeProjections.ApplyProjection(field, fieldDefinitionProjection);
		}

		private void AddFieldRVA(FieldDefinition field)
		{
			MetadataTable<Row<uint, uint>> table = this.GetTable<FieldRVATable>(Table.FieldRVA);
			int num = 1;
			if (field.FieldType.IsDefinition && !field.FieldType.IsGenericInstance)
			{
				TypeDefinition typeDefinition = field.FieldType.Resolve();
				if (typeDefinition.Module == this.module && typeDefinition.PackingSize > 1)
				{
					num = (int)typeDefinition.PackingSize;
				}
			}
			table.AddRow(new Row<uint, uint>(this.data.AddData(field.InitialValue, num), field.token.RID));
		}

		private void AddFieldLayout(FieldDefinition field)
		{
			this.GetTable<FieldLayoutTable>(Table.FieldLayout).AddRow(new Row<uint, uint>((uint)field.Offset, field.token.RID));
		}

		private void AddMethods(TypeDefinition type)
		{
			Collection<MethodDefinition> methods = type.Methods;
			for (int i = 0; i < methods.Count; i++)
			{
				this.AddMethod(methods[i]);
			}
		}

		private void AddMethod(MethodDefinition method)
		{
			MethodDefinitionProjection methodDefinitionProjection = WindowsRuntimeProjections.RemoveProjection(method);
			this.method_table.AddRow(new Row<uint, MethodImplAttributes, MethodAttributes, uint, uint, uint>(method.HasBody ? this.code.WriteMethodBody(method) : 0U, method.ImplAttributes, method.Attributes, this.GetStringIndex(method.Name), this.GetBlobIndex(this.GetMethodSignature(method)), this.param_rid));
			this.AddParameters(method);
			if (method.HasGenericParameters)
			{
				this.AddGenericParameters(method);
			}
			if (method.IsPInvokeImpl)
			{
				this.AddPInvokeInfo(method);
			}
			if (method.HasCustomAttributes)
			{
				this.AddCustomAttributes(method);
			}
			if (method.HasSecurityDeclarations)
			{
				this.AddSecurityDeclarations(method);
			}
			if (method.HasOverrides)
			{
				this.AddOverrides(method);
			}
			WindowsRuntimeProjections.ApplyProjection(method, methodDefinitionProjection);
		}

		private void AddParameters(MethodDefinition method)
		{
			ParameterDefinition parameter = method.MethodReturnType.parameter;
			if (parameter != null && MetadataBuilder.RequiresParameterRow(parameter))
			{
				this.AddParameter(0, parameter, this.param_table);
			}
			if (!method.HasParameters)
			{
				return;
			}
			Collection<ParameterDefinition> parameters = method.Parameters;
			for (int i = 0; i < parameters.Count; i++)
			{
				ParameterDefinition parameterDefinition = parameters[i];
				if (MetadataBuilder.RequiresParameterRow(parameterDefinition))
				{
					this.AddParameter((ushort)(i + 1), parameterDefinition, this.param_table);
				}
			}
		}

		private void AddPInvokeInfo(MethodDefinition method)
		{
			PInvokeInfo pinvokeInfo = method.PInvokeInfo;
			if (pinvokeInfo == null)
			{
				return;
			}
			this.GetTable<ImplMapTable>(Table.ImplMap).AddRow(new Row<PInvokeAttributes, uint, uint, uint>(pinvokeInfo.Attributes, MetadataBuilder.MakeCodedRID(method, CodedIndex.MemberForwarded), this.GetStringIndex(pinvokeInfo.EntryPoint), pinvokeInfo.Module.MetadataToken.RID));
		}

		private void AddOverrides(MethodDefinition method)
		{
			Collection<MethodReference> overrides = method.Overrides;
			MethodImplTable table = this.GetTable<MethodImplTable>(Table.MethodImpl);
			for (int i = 0; i < overrides.Count; i++)
			{
				table.AddRow(new Row<uint, uint, uint>(method.DeclaringType.token.RID, MetadataBuilder.MakeCodedRID(method, CodedIndex.MethodDefOrRef), MetadataBuilder.MakeCodedRID(this.LookupToken(overrides[i]), CodedIndex.MethodDefOrRef)));
			}
		}

		private static bool RequiresParameterRow(ParameterDefinition parameter)
		{
			return !string.IsNullOrEmpty(parameter.Name) || parameter.Attributes != ParameterAttributes.None || parameter.HasMarshalInfo || parameter.HasConstant || parameter.HasCustomAttributes;
		}

		private void AddParameter(ushort sequence, ParameterDefinition parameter, ParamTable table)
		{
			table.AddRow(new Row<ParameterAttributes, ushort, uint>(parameter.Attributes, sequence, this.GetStringIndex(parameter.Name)));
			TokenType tokenType = TokenType.Param;
			uint num = this.param_rid;
			this.param_rid = num + 1U;
			parameter.token = new MetadataToken(tokenType, num);
			if (parameter.HasCustomAttributes)
			{
				this.AddCustomAttributes(parameter);
			}
			if (parameter.HasConstant)
			{
				this.AddConstant(parameter, parameter.ParameterType);
			}
			if (parameter.HasMarshalInfo)
			{
				this.AddMarshalInfo(parameter);
			}
		}

		private void AddMarshalInfo(IMarshalInfoProvider owner)
		{
			this.GetTable<FieldMarshalTable>(Table.FieldMarshal).AddRow(new Row<uint, uint>(MetadataBuilder.MakeCodedRID(owner, CodedIndex.HasFieldMarshal), this.GetBlobIndex(this.GetMarshalInfoSignature(owner))));
		}

		private void AddProperties(TypeDefinition type)
		{
			Collection<PropertyDefinition> properties = type.Properties;
			this.property_map_table.AddRow(new Row<uint, uint>(type.token.RID, this.property_rid));
			for (int i = 0; i < properties.Count; i++)
			{
				this.AddProperty(properties[i]);
			}
		}

		private void AddProperty(PropertyDefinition property)
		{
			this.property_table.AddRow(new Row<PropertyAttributes, uint, uint>(property.Attributes, this.GetStringIndex(property.Name), this.GetBlobIndex(this.GetPropertySignature(property))));
			TokenType tokenType = TokenType.Property;
			uint num = this.property_rid;
			this.property_rid = num + 1U;
			property.token = new MetadataToken(tokenType, num);
			MethodDefinition methodDefinition = property.GetMethod;
			if (methodDefinition != null)
			{
				this.AddSemantic(MethodSemanticsAttributes.Getter, property, methodDefinition);
			}
			methodDefinition = property.SetMethod;
			if (methodDefinition != null)
			{
				this.AddSemantic(MethodSemanticsAttributes.Setter, property, methodDefinition);
			}
			if (property.HasOtherMethods)
			{
				this.AddOtherSemantic(property, property.OtherMethods);
			}
			if (property.HasCustomAttributes)
			{
				this.AddCustomAttributes(property);
			}
			if (property.HasConstant)
			{
				this.AddConstant(property, property.PropertyType);
			}
		}

		private void AddOtherSemantic(IMetadataTokenProvider owner, Collection<MethodDefinition> others)
		{
			for (int i = 0; i < others.Count; i++)
			{
				this.AddSemantic(MethodSemanticsAttributes.Other, owner, others[i]);
			}
		}

		private void AddEvents(TypeDefinition type)
		{
			Collection<EventDefinition> events = type.Events;
			this.event_map_table.AddRow(new Row<uint, uint>(type.token.RID, this.event_rid));
			for (int i = 0; i < events.Count; i++)
			{
				this.AddEvent(events[i]);
			}
		}

		private void AddEvent(EventDefinition @event)
		{
			this.event_table.AddRow(new Row<EventAttributes, uint, uint>(@event.Attributes, this.GetStringIndex(@event.Name), MetadataBuilder.MakeCodedRID(this.GetTypeToken(@event.EventType), CodedIndex.TypeDefOrRef)));
			TokenType tokenType = TokenType.Event;
			uint num = this.event_rid;
			this.event_rid = num + 1U;
			@event.token = new MetadataToken(tokenType, num);
			MethodDefinition methodDefinition = @event.AddMethod;
			if (methodDefinition != null)
			{
				this.AddSemantic(MethodSemanticsAttributes.AddOn, @event, methodDefinition);
			}
			methodDefinition = @event.InvokeMethod;
			if (methodDefinition != null)
			{
				this.AddSemantic(MethodSemanticsAttributes.Fire, @event, methodDefinition);
			}
			methodDefinition = @event.RemoveMethod;
			if (methodDefinition != null)
			{
				this.AddSemantic(MethodSemanticsAttributes.RemoveOn, @event, methodDefinition);
			}
			if (@event.HasOtherMethods)
			{
				this.AddOtherSemantic(@event, @event.OtherMethods);
			}
			if (@event.HasCustomAttributes)
			{
				this.AddCustomAttributes(@event);
			}
		}

		private void AddSemantic(MethodSemanticsAttributes semantics, IMetadataTokenProvider provider, MethodDefinition method)
		{
			method.SemanticsAttributes = semantics;
			this.GetTable<MethodSemanticsTable>(Table.MethodSemantics).AddRow(new Row<MethodSemanticsAttributes, uint, uint>(semantics, method.token.RID, MetadataBuilder.MakeCodedRID(provider, CodedIndex.HasSemantics)));
		}

		private void AddConstant(IConstantProvider owner, TypeReference type)
		{
			object constant = owner.Constant;
			ElementType constantType = MetadataBuilder.GetConstantType(type, constant);
			this.constant_table.AddRow(new Row<ElementType, uint, uint>(constantType, MetadataBuilder.MakeCodedRID(owner.MetadataToken, CodedIndex.HasConstant), this.GetBlobIndex(this.GetConstantSignature(constantType, constant))));
		}

		private static ElementType GetConstantType(TypeReference constant_type, object constant)
		{
			if (constant == null)
			{
				return ElementType.Class;
			}
			ElementType etype = constant_type.etype;
			switch (etype)
			{
			case ElementType.None:
			{
				TypeDefinition typeDefinition = constant_type.CheckedResolve();
				if (typeDefinition.IsEnum)
				{
					return MetadataBuilder.GetConstantType(typeDefinition.GetEnumUnderlyingType(), constant);
				}
				return ElementType.Class;
			}
			case ElementType.Void:
			case ElementType.Ptr:
			case ElementType.ValueType:
			case ElementType.Class:
			case ElementType.TypedByRef:
			case (ElementType)23:
			case (ElementType)26:
			case ElementType.FnPtr:
				return etype;
			case ElementType.Boolean:
			case ElementType.Char:
			case ElementType.I1:
			case ElementType.U1:
			case ElementType.I2:
			case ElementType.U2:
			case ElementType.I4:
			case ElementType.U4:
			case ElementType.I8:
			case ElementType.U8:
			case ElementType.R4:
			case ElementType.R8:
			case ElementType.I:
			case ElementType.U:
				return MetadataBuilder.GetConstantType(constant.GetType());
			case ElementType.String:
				return ElementType.String;
			case ElementType.ByRef:
			case ElementType.CModReqD:
			case ElementType.CModOpt:
				break;
			case ElementType.Var:
			case ElementType.Array:
			case ElementType.SzArray:
			case ElementType.MVar:
				return ElementType.Class;
			case ElementType.GenericInst:
			{
				GenericInstanceType genericInstanceType = (GenericInstanceType)constant_type;
				if (genericInstanceType.ElementType.IsTypeOf("System", "Nullable`1"))
				{
					return MetadataBuilder.GetConstantType(genericInstanceType.GenericArguments[0], constant);
				}
				return MetadataBuilder.GetConstantType(((TypeSpecification)constant_type).ElementType, constant);
			}
			case ElementType.Object:
				return MetadataBuilder.GetConstantType(constant.GetType());
			default:
				if (etype != ElementType.Sentinel)
				{
					return etype;
				}
				break;
			}
			return MetadataBuilder.GetConstantType(((TypeSpecification)constant_type).ElementType, constant);
		}

		private static ElementType GetConstantType(Type type)
		{
			switch (Type.GetTypeCode(type))
			{
			case TypeCode.Boolean:
				return ElementType.Boolean;
			case TypeCode.Char:
				return ElementType.Char;
			case TypeCode.SByte:
				return ElementType.I1;
			case TypeCode.Byte:
				return ElementType.U1;
			case TypeCode.Int16:
				return ElementType.I2;
			case TypeCode.UInt16:
				return ElementType.U2;
			case TypeCode.Int32:
				return ElementType.I4;
			case TypeCode.UInt32:
				return ElementType.U4;
			case TypeCode.Int64:
				return ElementType.I8;
			case TypeCode.UInt64:
				return ElementType.U8;
			case TypeCode.Single:
				return ElementType.R4;
			case TypeCode.Double:
				return ElementType.R8;
			case TypeCode.String:
				return ElementType.String;
			}
			throw new NotSupportedException(type.FullName);
		}

		private void AddCustomAttributes(ICustomAttributeProvider owner)
		{
			Collection<CustomAttribute> customAttributes = owner.CustomAttributes;
			for (int i = 0; i < customAttributes.Count; i++)
			{
				CustomAttribute customAttribute = customAttributes[i];
				CustomAttributeValueProjection customAttributeValueProjection = WindowsRuntimeProjections.RemoveProjection(customAttribute);
				this.custom_attribute_table.AddRow(new Row<uint, uint, uint>(MetadataBuilder.MakeCodedRID(owner, CodedIndex.HasCustomAttribute), MetadataBuilder.MakeCodedRID(this.LookupToken(customAttribute.Constructor), CodedIndex.CustomAttributeType), this.GetBlobIndex(this.GetCustomAttributeSignature(customAttribute))));
				WindowsRuntimeProjections.ApplyProjection(customAttribute, customAttributeValueProjection);
			}
		}

		private void AddSecurityDeclarations(ISecurityDeclarationProvider owner)
		{
			Collection<SecurityDeclaration> securityDeclarations = owner.SecurityDeclarations;
			for (int i = 0; i < securityDeclarations.Count; i++)
			{
				SecurityDeclaration securityDeclaration = securityDeclarations[i];
				this.declsec_table.AddRow(new Row<SecurityAction, uint, uint>(securityDeclaration.Action, MetadataBuilder.MakeCodedRID(owner, CodedIndex.HasDeclSecurity), this.GetBlobIndex(this.GetSecurityDeclarationSignature(securityDeclaration))));
			}
		}

		private MetadataToken GetMemberRefToken(MemberReference member)
		{
			Row<uint, uint, uint> row = this.CreateMemberRefRow(member);
			MetadataToken metadataToken;
			if (!this.member_ref_map.TryGetValue(row, out metadataToken))
			{
				metadataToken = this.AddMemberReference(member, row);
			}
			return metadataToken;
		}

		private Row<uint, uint, uint> CreateMemberRefRow(MemberReference member)
		{
			return new Row<uint, uint, uint>(MetadataBuilder.MakeCodedRID(this.GetTypeToken(member.DeclaringType), CodedIndex.MemberRefParent), this.GetStringIndex(member.Name), this.GetBlobIndex(this.GetMemberRefSignature(member)));
		}

		private MetadataToken AddMemberReference(MemberReference member, Row<uint, uint, uint> row)
		{
			member.token = new MetadataToken(TokenType.MemberRef, this.member_ref_table.AddRow(row));
			MetadataToken token = member.token;
			this.member_ref_map.Add(row, token);
			return token;
		}

		private MetadataToken GetMethodSpecToken(MethodSpecification method_spec)
		{
			Row<uint, uint> row = this.CreateMethodSpecRow(method_spec);
			MetadataToken metadataToken;
			if (this.method_spec_map.TryGetValue(row, out metadataToken))
			{
				return metadataToken;
			}
			this.AddMethodSpecification(method_spec, row);
			return method_spec.token;
		}

		private void AddMethodSpecification(MethodSpecification method_spec, Row<uint, uint> row)
		{
			method_spec.token = new MetadataToken(TokenType.MethodSpec, this.method_spec_table.AddRow(row));
			this.method_spec_map.Add(row, method_spec.token);
		}

		private Row<uint, uint> CreateMethodSpecRow(MethodSpecification method_spec)
		{
			return new Row<uint, uint>(MetadataBuilder.MakeCodedRID(this.LookupToken(method_spec.ElementMethod), CodedIndex.MethodDefOrRef), this.GetBlobIndex(this.GetMethodSpecSignature(method_spec)));
		}

		private SignatureWriter CreateSignatureWriter()
		{
			return new SignatureWriter(this);
		}

		private SignatureWriter GetMethodSpecSignature(MethodSpecification method_spec)
		{
			if (!method_spec.IsGenericInstance)
			{
				throw new NotSupportedException();
			}
			GenericInstanceMethod genericInstanceMethod = (GenericInstanceMethod)method_spec;
			SignatureWriter signatureWriter = this.CreateSignatureWriter();
			signatureWriter.WriteByte(10);
			signatureWriter.WriteGenericInstanceSignature(genericInstanceMethod);
			return signatureWriter;
		}

		public uint AddStandAloneSignature(uint signature)
		{
			return (uint)this.standalone_sig_table.AddRow(signature);
		}

		public uint GetLocalVariableBlobIndex(Collection<VariableDefinition> variables)
		{
			return this.GetBlobIndex(this.GetVariablesSignature(variables));
		}

		public uint GetCallSiteBlobIndex(CallSite call_site)
		{
			return this.GetBlobIndex(this.GetMethodSignature(call_site));
		}

		public uint GetConstantTypeBlobIndex(TypeReference constant_type)
		{
			return this.GetBlobIndex(this.GetConstantTypeSignature(constant_type));
		}

		private SignatureWriter GetVariablesSignature(Collection<VariableDefinition> variables)
		{
			SignatureWriter signatureWriter = this.CreateSignatureWriter();
			signatureWriter.WriteByte(7);
			signatureWriter.WriteCompressedUInt32((uint)variables.Count);
			for (int i = 0; i < variables.Count; i++)
			{
				signatureWriter.WriteTypeSignature(variables[i].VariableType);
			}
			return signatureWriter;
		}

		private SignatureWriter GetConstantTypeSignature(TypeReference constant_type)
		{
			SignatureWriter signatureWriter = this.CreateSignatureWriter();
			signatureWriter.WriteByte(6);
			signatureWriter.WriteTypeSignature(constant_type);
			return signatureWriter;
		}

		private SignatureWriter GetFieldSignature(FieldReference field)
		{
			SignatureWriter signatureWriter = this.CreateSignatureWriter();
			signatureWriter.WriteByte(6);
			signatureWriter.WriteTypeSignature(field.FieldType);
			return signatureWriter;
		}

		private SignatureWriter GetMethodSignature(IMethodSignature method)
		{
			SignatureWriter signatureWriter = this.CreateSignatureWriter();
			signatureWriter.WriteMethodSignature(method);
			return signatureWriter;
		}

		private SignatureWriter GetMemberRefSignature(MemberReference member)
		{
			FieldReference fieldReference = member as FieldReference;
			if (fieldReference != null)
			{
				return this.GetFieldSignature(fieldReference);
			}
			MethodReference methodReference = member as MethodReference;
			if (methodReference != null)
			{
				return this.GetMethodSignature(methodReference);
			}
			throw new NotSupportedException();
		}

		private SignatureWriter GetPropertySignature(PropertyDefinition property)
		{
			SignatureWriter signatureWriter = this.CreateSignatureWriter();
			byte b = 8;
			if (property.HasThis)
			{
				b |= 32;
			}
			uint num = 0U;
			Collection<ParameterDefinition> collection = null;
			if (property.HasParameters)
			{
				collection = property.Parameters;
				num = (uint)collection.Count;
			}
			signatureWriter.WriteByte(b);
			signatureWriter.WriteCompressedUInt32(num);
			signatureWriter.WriteTypeSignature(property.PropertyType);
			if (num == 0U)
			{
				return signatureWriter;
			}
			int num2 = 0;
			while ((long)num2 < (long)((ulong)num))
			{
				signatureWriter.WriteTypeSignature(collection[num2].ParameterType);
				num2++;
			}
			return signatureWriter;
		}

		private SignatureWriter GetTypeSpecSignature(TypeReference type)
		{
			SignatureWriter signatureWriter = this.CreateSignatureWriter();
			signatureWriter.WriteTypeSignature(type);
			return signatureWriter;
		}

		private SignatureWriter GetConstantSignature(ElementType type, object value)
		{
			SignatureWriter signatureWriter = this.CreateSignatureWriter();
			if (type <= ElementType.String)
			{
				if (type != ElementType.None)
				{
					if (type != ElementType.String)
					{
						goto IL_003B;
					}
					signatureWriter.WriteConstantString((string)value);
					return signatureWriter;
				}
			}
			else if (type - ElementType.Class > 3 && type - ElementType.Object > 2)
			{
				goto IL_003B;
			}
			signatureWriter.WriteInt32(0);
			return signatureWriter;
			IL_003B:
			signatureWriter.WriteConstantPrimitive(value);
			return signatureWriter;
		}

		private SignatureWriter GetCustomAttributeSignature(CustomAttribute attribute)
		{
			SignatureWriter signatureWriter = this.CreateSignatureWriter();
			if (!attribute.resolved)
			{
				signatureWriter.WriteBytes(attribute.GetBlob());
				return signatureWriter;
			}
			signatureWriter.WriteUInt16(1);
			signatureWriter.WriteCustomAttributeConstructorArguments(attribute);
			signatureWriter.WriteCustomAttributeNamedArguments(attribute);
			return signatureWriter;
		}

		private SignatureWriter GetSecurityDeclarationSignature(SecurityDeclaration declaration)
		{
			SignatureWriter signatureWriter = this.CreateSignatureWriter();
			if (!declaration.resolved)
			{
				signatureWriter.WriteBytes(declaration.GetBlob());
			}
			else if (this.module.Runtime < TargetRuntime.Net_2_0)
			{
				signatureWriter.WriteXmlSecurityDeclaration(declaration);
			}
			else
			{
				signatureWriter.WriteSecurityDeclaration(declaration);
			}
			return signatureWriter;
		}

		private SignatureWriter GetMarshalInfoSignature(IMarshalInfoProvider owner)
		{
			SignatureWriter signatureWriter = this.CreateSignatureWriter();
			signatureWriter.WriteMarshalInfo(owner.MarshalInfo);
			return signatureWriter;
		}

		private static Exception CreateForeignMemberException(MemberReference member)
		{
			return new ArgumentException(string.Format("Member '{0}' is declared in another module and needs to be imported", member));
		}

		public MetadataToken LookupToken(IMetadataTokenProvider provider)
		{
			if (provider == null)
			{
				throw new ArgumentNullException();
			}
			if (this.metadata_builder != null)
			{
				return this.metadata_builder.LookupToken(provider);
			}
			MemberReference memberReference = provider as MemberReference;
			if (memberReference == null || memberReference.Module != this.module)
			{
				throw MetadataBuilder.CreateForeignMemberException(memberReference);
			}
			MetadataToken metadataToken = provider.MetadataToken;
			TokenType tokenType = metadataToken.TokenType;
			if (tokenType <= TokenType.MemberRef)
			{
				if (tokenType <= TokenType.TypeDef)
				{
					if (tokenType == TokenType.TypeRef)
					{
						goto IL_00BE;
					}
					if (tokenType != TokenType.TypeDef)
					{
						goto IL_00E0;
					}
				}
				else if (tokenType != TokenType.Field && tokenType != TokenType.Method)
				{
					if (tokenType != TokenType.MemberRef)
					{
						goto IL_00E0;
					}
					return this.GetMemberRefToken(memberReference);
				}
			}
			else if (tokenType <= TokenType.Property)
			{
				if (tokenType != TokenType.Event && tokenType != TokenType.Property)
				{
					goto IL_00E0;
				}
			}
			else
			{
				if (tokenType == TokenType.TypeSpec || tokenType == TokenType.GenericParam)
				{
					goto IL_00BE;
				}
				if (tokenType != TokenType.MethodSpec)
				{
					goto IL_00E0;
				}
				return this.GetMethodSpecToken((MethodSpecification)provider);
			}
			return metadataToken;
			IL_00BE:
			return this.GetTypeToken((TypeReference)provider);
			IL_00E0:
			throw new NotSupportedException();
		}

		public void AddMethodDebugInformation(MethodDebugInformation method_info)
		{
			if (method_info.HasSequencePoints)
			{
				this.AddSequencePoints(method_info);
			}
			if (method_info.Scope != null)
			{
				this.AddLocalScope(method_info, method_info.Scope);
			}
			if (method_info.StateMachineKickOffMethod != null)
			{
				this.AddStateMachineMethod(method_info);
			}
			this.AddCustomDebugInformations(method_info.Method);
		}

		private void AddStateMachineMethod(MethodDebugInformation method_info)
		{
			this.state_machine_method_table.AddRow(new Row<uint, uint>(method_info.Method.MetadataToken.RID, method_info.StateMachineKickOffMethod.MetadataToken.RID));
		}

		private void AddLocalScope(MethodDebugInformation method_info, ScopeDebugInformation scope)
		{
			int num = this.local_scope_table.AddRow(new Row<uint, uint, uint, uint, uint, uint>(method_info.Method.MetadataToken.RID, (scope.import != null) ? this.AddImportScope(scope.import) : 0U, this.local_variable_rid, this.local_constant_rid, (uint)scope.Start.Offset, (uint)((scope.End.IsEndOfMethod ? method_info.code_size : scope.End.Offset) - scope.Start.Offset)));
			scope.token = new MetadataToken(TokenType.LocalScope, num);
			this.AddCustomDebugInformations(scope);
			if (scope.HasVariables)
			{
				this.AddLocalVariables(scope);
			}
			if (scope.HasConstants)
			{
				this.AddLocalConstants(scope);
			}
			for (int i = 0; i < scope.Scopes.Count; i++)
			{
				this.AddLocalScope(method_info, scope.Scopes[i]);
			}
		}

		private void AddLocalVariables(ScopeDebugInformation scope)
		{
			for (int i = 0; i < scope.Variables.Count; i++)
			{
				VariableDebugInformation variableDebugInformation = scope.Variables[i];
				this.local_variable_table.AddRow(new Row<VariableAttributes, ushort, uint>(variableDebugInformation.Attributes, (ushort)variableDebugInformation.Index, this.GetStringIndex(variableDebugInformation.Name)));
				variableDebugInformation.token = new MetadataToken(TokenType.LocalVariable, this.local_variable_rid);
				this.local_variable_rid += 1U;
				this.AddCustomDebugInformations(variableDebugInformation);
			}
		}

		private void AddLocalConstants(ScopeDebugInformation scope)
		{
			for (int i = 0; i < scope.Constants.Count; i++)
			{
				ConstantDebugInformation constantDebugInformation = scope.Constants[i];
				this.local_constant_table.AddRow(new Row<uint, uint>(this.GetStringIndex(constantDebugInformation.Name), this.GetBlobIndex(this.GetConstantSignature(constantDebugInformation))));
				constantDebugInformation.token = new MetadataToken(TokenType.LocalConstant, this.local_constant_rid);
				this.local_constant_rid += 1U;
			}
		}

		private SignatureWriter GetConstantSignature(ConstantDebugInformation constant)
		{
			TypeReference constantType = constant.ConstantType;
			SignatureWriter signatureWriter = this.CreateSignatureWriter();
			signatureWriter.WriteTypeSignature(constantType);
			if (constantType.IsTypeOf("System", "Decimal"))
			{
				int[] bits = decimal.GetBits((decimal)constant.Value);
				uint num = (uint)bits[0];
				uint num2 = (uint)bits[1];
				uint num3 = (uint)bits[2];
				byte b = (byte)(bits[3] >> 16);
				bool flag = ((long)bits[3] & (long)((ulong)int.MinValue)) != 0L;
				signatureWriter.WriteByte(b | (flag ? 128 : 0));
				signatureWriter.WriteUInt32(num);
				signatureWriter.WriteUInt32(num2);
				signatureWriter.WriteUInt32(num3);
				return signatureWriter;
			}
			if (constantType.IsTypeOf("System", "DateTime"))
			{
				signatureWriter.WriteInt64(((DateTime)constant.Value).Ticks);
				return signatureWriter;
			}
			signatureWriter.WriteBytes(this.GetConstantSignature(constantType.etype, constant.Value));
			return signatureWriter;
		}

		public void AddCustomDebugInformations(ICustomDebugInformationProvider provider)
		{
			if (!provider.HasCustomDebugInformations)
			{
				return;
			}
			Collection<CustomDebugInformation> customDebugInformations = provider.CustomDebugInformations;
			int i = 0;
			while (i < customDebugInformations.Count)
			{
				CustomDebugInformation customDebugInformation = customDebugInformations[i];
				switch (customDebugInformation.Kind)
				{
				case CustomDebugInformationKind.Binary:
				{
					BinaryCustomDebugInformation binaryCustomDebugInformation = (BinaryCustomDebugInformation)customDebugInformation;
					this.AddCustomDebugInformation(provider, binaryCustomDebugInformation, this.GetBlobIndex(binaryCustomDebugInformation.Data));
					break;
				}
				case CustomDebugInformationKind.StateMachineScope:
					this.AddStateMachineScopeDebugInformation(provider, (StateMachineScopeDebugInformation)customDebugInformation);
					break;
				case CustomDebugInformationKind.DynamicVariable:
				case CustomDebugInformationKind.DefaultNamespace:
					goto IL_00A5;
				case CustomDebugInformationKind.AsyncMethodBody:
					this.AddAsyncMethodBodyDebugInformation(provider, (AsyncMethodBodyDebugInformation)customDebugInformation);
					break;
				case CustomDebugInformationKind.EmbeddedSource:
					this.AddEmbeddedSourceDebugInformation(provider, (EmbeddedSourceDebugInformation)customDebugInformation);
					break;
				case CustomDebugInformationKind.SourceLink:
					this.AddSourceLinkDebugInformation(provider, (SourceLinkDebugInformation)customDebugInformation);
					break;
				default:
					goto IL_00A5;
				}
				i++;
				continue;
				IL_00A5:
				throw new NotImplementedException();
			}
		}

		private void AddStateMachineScopeDebugInformation(ICustomDebugInformationProvider provider, StateMachineScopeDebugInformation state_machine_scope)
		{
			MethodDebugInformation debugInformation = ((MethodDefinition)provider).DebugInformation;
			SignatureWriter signatureWriter = this.CreateSignatureWriter();
			Collection<StateMachineScope> scopes = state_machine_scope.Scopes;
			for (int i = 0; i < scopes.Count; i++)
			{
				StateMachineScope stateMachineScope = scopes[i];
				signatureWriter.WriteUInt32((uint)stateMachineScope.Start.Offset);
				int num = (stateMachineScope.End.IsEndOfMethod ? debugInformation.code_size : stateMachineScope.End.Offset);
				signatureWriter.WriteUInt32((uint)(num - stateMachineScope.Start.Offset));
			}
			this.AddCustomDebugInformation(provider, state_machine_scope, signatureWriter);
		}

		private void AddAsyncMethodBodyDebugInformation(ICustomDebugInformationProvider provider, AsyncMethodBodyDebugInformation async_method)
		{
			SignatureWriter signatureWriter = this.CreateSignatureWriter();
			signatureWriter.WriteUInt32((uint)(async_method.catch_handler.Offset + 1));
			if (!async_method.yields.IsNullOrEmpty<InstructionOffset>())
			{
				for (int i = 0; i < async_method.yields.Count; i++)
				{
					signatureWriter.WriteUInt32((uint)async_method.yields[i].Offset);
					signatureWriter.WriteUInt32((uint)async_method.resumes[i].Offset);
					signatureWriter.WriteCompressedUInt32(async_method.resume_methods[i].MetadataToken.RID);
				}
			}
			this.AddCustomDebugInformation(provider, async_method, signatureWriter);
		}

		private void AddEmbeddedSourceDebugInformation(ICustomDebugInformationProvider provider, EmbeddedSourceDebugInformation embedded_source)
		{
			SignatureWriter signatureWriter = this.CreateSignatureWriter();
			if (!embedded_source.resolved)
			{
				signatureWriter.WriteBytes(embedded_source.ReadRawEmbeddedSourceDebugInformation());
				this.AddCustomDebugInformation(provider, embedded_source, signatureWriter);
				return;
			}
			byte[] array = embedded_source.content ?? Empty<byte>.Array;
			if (embedded_source.compress)
			{
				signatureWriter.WriteInt32(array.Length);
				MemoryStream memoryStream = new MemoryStream(array);
				MemoryStream memoryStream2 = new MemoryStream();
				using (DeflateStream deflateStream = new DeflateStream(memoryStream2, CompressionMode.Compress, true))
				{
					memoryStream.CopyTo(deflateStream);
				}
				signatureWriter.WriteBytes(memoryStream2.ToArray());
			}
			else
			{
				signatureWriter.WriteInt32(0);
				signatureWriter.WriteBytes(array);
			}
			this.AddCustomDebugInformation(provider, embedded_source, signatureWriter);
		}

		private void AddSourceLinkDebugInformation(ICustomDebugInformationProvider provider, SourceLinkDebugInformation source_link)
		{
			SignatureWriter signatureWriter = this.CreateSignatureWriter();
			signatureWriter.WriteBytes(Encoding.UTF8.GetBytes(source_link.content));
			this.AddCustomDebugInformation(provider, source_link, signatureWriter);
		}

		private void AddCustomDebugInformation(ICustomDebugInformationProvider provider, CustomDebugInformation custom_info, SignatureWriter signature)
		{
			this.AddCustomDebugInformation(provider, custom_info, this.GetBlobIndex(signature));
		}

		private void AddCustomDebugInformation(ICustomDebugInformationProvider provider, CustomDebugInformation custom_info, uint blob_index)
		{
			int num = this.custom_debug_information_table.AddRow(new Row<uint, uint, uint>(MetadataBuilder.MakeCodedRID(provider.MetadataToken, CodedIndex.HasCustomDebugInformation), this.GetGuidIndex(custom_info.Identifier), blob_index));
			custom_info.token = new MetadataToken(TokenType.CustomDebugInformation, num);
		}

		private uint AddImportScope(ImportDebugInformation import)
		{
			uint num = 0U;
			if (import.Parent != null)
			{
				num = this.AddImportScope(import.Parent);
			}
			uint num2 = 0U;
			if (import.HasTargets)
			{
				SignatureWriter signatureWriter = this.CreateSignatureWriter();
				for (int i = 0; i < import.Targets.Count; i++)
				{
					this.AddImportTarget(import.Targets[i], signatureWriter);
				}
				num2 = this.GetBlobIndex(signatureWriter);
			}
			Row<uint, uint> row = new Row<uint, uint>(num, num2);
			MetadataToken metadataToken;
			if (this.import_scope_map.TryGetValue(row, out metadataToken))
			{
				return metadataToken.RID;
			}
			metadataToken = new MetadataToken(TokenType.ImportScope, this.import_scope_table.AddRow(row));
			this.import_scope_map.Add(row, metadataToken);
			return metadataToken.RID;
		}

		private void AddImportTarget(ImportTarget target, SignatureWriter signature)
		{
			signature.WriteCompressedUInt32((uint)target.kind);
			switch (target.kind)
			{
			case ImportTargetKind.ImportNamespace:
				signature.WriteCompressedUInt32(this.GetUTF8StringBlobIndex(target.@namespace));
				return;
			case ImportTargetKind.ImportNamespaceInAssembly:
				signature.WriteCompressedUInt32(target.reference.MetadataToken.RID);
				signature.WriteCompressedUInt32(this.GetUTF8StringBlobIndex(target.@namespace));
				return;
			case ImportTargetKind.ImportType:
				signature.WriteTypeToken(target.type);
				return;
			case ImportTargetKind.ImportXmlNamespaceWithAlias:
				signature.WriteCompressedUInt32(this.GetUTF8StringBlobIndex(target.alias));
				signature.WriteCompressedUInt32(this.GetUTF8StringBlobIndex(target.@namespace));
				return;
			case ImportTargetKind.ImportAlias:
				signature.WriteCompressedUInt32(this.GetUTF8StringBlobIndex(target.alias));
				return;
			case ImportTargetKind.DefineAssemblyAlias:
				signature.WriteCompressedUInt32(this.GetUTF8StringBlobIndex(target.alias));
				signature.WriteCompressedUInt32(target.reference.MetadataToken.RID);
				return;
			case ImportTargetKind.DefineNamespaceAlias:
				signature.WriteCompressedUInt32(this.GetUTF8StringBlobIndex(target.alias));
				signature.WriteCompressedUInt32(this.GetUTF8StringBlobIndex(target.@namespace));
				return;
			case ImportTargetKind.DefineNamespaceInAssemblyAlias:
				signature.WriteCompressedUInt32(this.GetUTF8StringBlobIndex(target.alias));
				signature.WriteCompressedUInt32(target.reference.MetadataToken.RID);
				signature.WriteCompressedUInt32(this.GetUTF8StringBlobIndex(target.@namespace));
				return;
			case ImportTargetKind.DefineTypeAlias:
				signature.WriteCompressedUInt32(this.GetUTF8StringBlobIndex(target.alias));
				signature.WriteTypeToken(target.type);
				return;
			default:
				return;
			}
		}

		private uint GetUTF8StringBlobIndex(string s)
		{
			return this.GetBlobIndex(Encoding.UTF8.GetBytes(s));
		}

		public MetadataToken GetDocumentToken(Document document)
		{
			MetadataToken metadataToken;
			if (this.document_map.TryGetValue(document.Url, out metadataToken))
			{
				return metadataToken;
			}
			metadataToken = new MetadataToken(TokenType.Document, this.document_table.AddRow(new Row<uint, uint, uint, uint>(this.GetBlobIndex(this.GetDocumentNameSignature(document)), this.GetGuidIndex(document.HashAlgorithm.ToGuid()), this.GetBlobIndex(document.Hash), this.GetGuidIndex(document.Language.ToGuid()))));
			document.token = metadataToken;
			this.AddCustomDebugInformations(document);
			this.document_map.Add(document.Url, metadataToken);
			return metadataToken;
		}

		private SignatureWriter GetDocumentNameSignature(Document document)
		{
			string url = document.Url;
			SignatureWriter signatureWriter = this.CreateSignatureWriter();
			char c;
			if (!MetadataBuilder.TryGetDocumentNameSeparator(url, out c))
			{
				signatureWriter.WriteByte(0);
				signatureWriter.WriteCompressedUInt32(this.GetUTF8StringBlobIndex(url));
				return signatureWriter;
			}
			signatureWriter.WriteByte((byte)c);
			string[] array = url.Split(new char[] { c });
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == string.Empty)
				{
					signatureWriter.WriteCompressedUInt32(0U);
				}
				else
				{
					signatureWriter.WriteCompressedUInt32(this.GetUTF8StringBlobIndex(array[i]));
				}
			}
			return signatureWriter;
		}

		private static bool TryGetDocumentNameSeparator(string path, out char separator)
		{
			separator = '\0';
			if (string.IsNullOrEmpty(path))
			{
				return false;
			}
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < path.Length; i++)
			{
				if (path[i] == '/')
				{
					num++;
				}
				else if (path[i] == '\\')
				{
					num2++;
				}
			}
			if (num == 0 && num2 == 0)
			{
				return false;
			}
			if (num >= num2)
			{
				separator = '/';
				return true;
			}
			separator = '\\';
			return true;
		}

		private void AddSequencePoints(MethodDebugInformation info)
		{
			uint rid = info.Method.MetadataToken.RID;
			Document document;
			if (info.TryGetUniqueDocument(out document))
			{
				this.method_debug_information_table.rows[(int)(rid - 1U)].Col1 = this.GetDocumentToken(document).RID;
			}
			SignatureWriter signatureWriter = this.CreateSignatureWriter();
			signatureWriter.WriteSequencePoints(info);
			this.method_debug_information_table.rows[(int)(rid - 1U)].Col2 = this.GetBlobIndex(signatureWriter);
		}

		internal readonly ModuleDefinition module;

		internal readonly ISymbolWriterProvider symbol_writer_provider;

		internal ISymbolWriter symbol_writer;

		internal readonly TextMap text_map;

		internal readonly string fq_name;

		internal readonly uint timestamp;

		private readonly Dictionary<Row<uint, uint, uint>, MetadataToken> type_ref_map;

		private readonly Dictionary<uint, MetadataToken> type_spec_map;

		private readonly Dictionary<Row<uint, uint, uint>, MetadataToken> member_ref_map;

		private readonly Dictionary<Row<uint, uint>, MetadataToken> method_spec_map;

		private readonly Collection<GenericParameter> generic_parameters;

		internal readonly CodeWriter code;

		internal readonly DataBuffer data;

		internal readonly ResourceBuffer resources;

		internal readonly StringHeapBuffer string_heap;

		internal readonly GuidHeapBuffer guid_heap;

		internal readonly UserStringHeapBuffer user_string_heap;

		internal readonly BlobHeapBuffer blob_heap;

		internal readonly TableHeapBuffer table_heap;

		internal readonly PdbHeapBuffer pdb_heap;

		internal MetadataToken entry_point;

		internal uint type_rid = 1U;

		internal uint field_rid = 1U;

		internal uint method_rid = 1U;

		internal uint param_rid = 1U;

		internal uint property_rid = 1U;

		internal uint event_rid = 1U;

		internal uint local_variable_rid = 1U;

		internal uint local_constant_rid = 1U;

		private readonly TypeRefTable type_ref_table;

		private readonly TypeDefTable type_def_table;

		private readonly FieldTable field_table;

		private readonly MethodTable method_table;

		private readonly ParamTable param_table;

		private readonly InterfaceImplTable iface_impl_table;

		private readonly MemberRefTable member_ref_table;

		private readonly ConstantTable constant_table;

		private readonly CustomAttributeTable custom_attribute_table;

		private readonly DeclSecurityTable declsec_table;

		private readonly StandAloneSigTable standalone_sig_table;

		private readonly EventMapTable event_map_table;

		private readonly EventTable event_table;

		private readonly PropertyMapTable property_map_table;

		private readonly PropertyTable property_table;

		private readonly TypeSpecTable typespec_table;

		private readonly MethodSpecTable method_spec_table;

		internal MetadataBuilder metadata_builder;

		private readonly DocumentTable document_table;

		private readonly MethodDebugInformationTable method_debug_information_table;

		private readonly LocalScopeTable local_scope_table;

		private readonly LocalVariableTable local_variable_table;

		private readonly LocalConstantTable local_constant_table;

		private readonly ImportScopeTable import_scope_table;

		private readonly StateMachineMethodTable state_machine_method_table;

		private readonly CustomDebugInformationTable custom_debug_information_table;

		private readonly Dictionary<Row<uint, uint>, MetadataToken> import_scope_map;

		private readonly Dictionary<string, MetadataToken> document_map;

		private sealed class GenericParameterComparer : IComparer<GenericParameter>
		{
			public int Compare(GenericParameter a, GenericParameter b)
			{
				uint num = MetadataBuilder.MakeCodedRID(a.Owner, CodedIndex.TypeOrMethodDef);
				uint num2 = MetadataBuilder.MakeCodedRID(b.Owner, CodedIndex.TypeOrMethodDef);
				if (num == num2)
				{
					int position = a.Position;
					int position2 = b.Position;
					if (position == position2)
					{
						return 0;
					}
					if (position <= position2)
					{
						return -1;
					}
					return 1;
				}
				else
				{
					if (num <= num2)
					{
						return -1;
					}
					return 1;
				}
			}
		}
	}
}
