using System;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using Mono.CompilerServices.SymbolWriter;

namespace Mono.Cecil.Mdb
{
	internal sealed class MdbReader : ISymbolReader, IDisposable
	{
		public MdbReader(ModuleDefinition module, MonoSymbolFile symFile)
		{
			this.module = module;
			this.symbol_file = symFile;
			this.documents = new Dictionary<string, Document>();
		}

		public ISymbolWriterProvider GetWriterProvider()
		{
			return new MdbWriterProvider();
		}

		public bool ProcessDebugHeader(ImageDebugHeader header)
		{
			return this.symbol_file.Guid == this.module.Mvid;
		}

		public MethodDebugInformation Read(MethodDefinition method)
		{
			MetadataToken metadataToken = method.MetadataToken;
			MethodEntry methodByToken = this.symbol_file.GetMethodByToken(metadataToken.ToInt32());
			if (methodByToken == null)
			{
				return null;
			}
			MethodDebugInformation methodDebugInformation = new MethodDebugInformation(method);
			methodDebugInformation.code_size = MdbReader.ReadCodeSize(method);
			ScopeDebugInformation[] array = MdbReader.ReadScopes(methodByToken, methodDebugInformation);
			this.ReadLineNumbers(methodByToken, methodDebugInformation);
			MdbReader.ReadLocalVariables(methodByToken, array);
			return methodDebugInformation;
		}

		private static int ReadCodeSize(MethodDefinition method)
		{
			return method.Module.Read<MethodDefinition, int>(method, (MethodDefinition m, MetadataReader reader) => reader.ReadCodeSize(m));
		}

		private static void ReadLocalVariables(MethodEntry entry, ScopeDebugInformation[] scopes)
		{
			foreach (LocalVariableEntry localVariableEntry in entry.GetLocals())
			{
				VariableDebugInformation variableDebugInformation = new VariableDebugInformation(localVariableEntry.Index, localVariableEntry.Name);
				int blockIndex = localVariableEntry.BlockIndex;
				if (blockIndex >= 0 && blockIndex < scopes.Length)
				{
					ScopeDebugInformation scopeDebugInformation = scopes[blockIndex];
					if (scopeDebugInformation != null)
					{
						scopeDebugInformation.Variables.Add(variableDebugInformation);
					}
				}
			}
		}

		private void ReadLineNumbers(MethodEntry entry, MethodDebugInformation info)
		{
			LineNumberTable lineNumberTable = entry.GetLineNumberTable();
			info.sequence_points = new Collection<SequencePoint>(lineNumberTable.LineNumbers.Length);
			for (int i = 0; i < lineNumberTable.LineNumbers.Length; i++)
			{
				LineNumberEntry lineNumberEntry = lineNumberTable.LineNumbers[i];
				if (i <= 0 || lineNumberTable.LineNumbers[i - 1].Offset != lineNumberEntry.Offset)
				{
					info.sequence_points.Add(this.LineToSequencePoint(lineNumberEntry));
				}
			}
		}

		private Document GetDocument(SourceFileEntry file)
		{
			string fileName = file.FileName;
			Document document;
			if (this.documents.TryGetValue(fileName, out document))
			{
				return document;
			}
			document = new Document(fileName)
			{
				Hash = file.Checksum
			};
			this.documents.Add(fileName, document);
			return document;
		}

		private static ScopeDebugInformation[] ReadScopes(MethodEntry entry, MethodDebugInformation info)
		{
			CodeBlockEntry[] codeBlocks = entry.GetCodeBlocks();
			ScopeDebugInformation[] array = new ScopeDebugInformation[codeBlocks.Length + 1];
			ScopeDebugInformation[] array2 = array;
			int num = 0;
			ScopeDebugInformation scopeDebugInformation = new ScopeDebugInformation();
			scopeDebugInformation.Start = new InstructionOffset(0);
			scopeDebugInformation.End = new InstructionOffset(info.code_size);
			ScopeDebugInformation scopeDebugInformation2 = scopeDebugInformation;
			array2[num] = scopeDebugInformation;
			info.scope = scopeDebugInformation2;
			foreach (CodeBlockEntry codeBlockEntry in codeBlocks)
			{
				if (codeBlockEntry.BlockType == CodeBlockEntry.Type.Lexical || codeBlockEntry.BlockType == CodeBlockEntry.Type.CompilerGenerated)
				{
					ScopeDebugInformation scopeDebugInformation3 = new ScopeDebugInformation();
					scopeDebugInformation3.Start = new InstructionOffset(codeBlockEntry.StartOffset);
					scopeDebugInformation3.End = new InstructionOffset(codeBlockEntry.EndOffset);
					array[codeBlockEntry.Index + 1] = scopeDebugInformation3;
					if (!MdbReader.AddScope(info.scope.Scopes, scopeDebugInformation3))
					{
						info.scope.Scopes.Add(scopeDebugInformation3);
					}
				}
			}
			return array;
		}

		private static bool AddScope(Collection<ScopeDebugInformation> scopes, ScopeDebugInformation scope)
		{
			foreach (ScopeDebugInformation scopeDebugInformation in scopes)
			{
				if (scopeDebugInformation.HasScopes && MdbReader.AddScope(scopeDebugInformation.Scopes, scope))
				{
					return true;
				}
				if (scope.Start.Offset >= scopeDebugInformation.Start.Offset && scope.End.Offset <= scopeDebugInformation.End.Offset)
				{
					scopeDebugInformation.Scopes.Add(scope);
					return true;
				}
			}
			return false;
		}

		private SequencePoint LineToSequencePoint(LineNumberEntry line)
		{
			SourceFileEntry sourceFile = this.symbol_file.GetSourceFile(line.File);
			return new SequencePoint(line.Offset, this.GetDocument(sourceFile))
			{
				StartLine = line.Row,
				EndLine = line.EndRow,
				StartColumn = line.Column,
				EndColumn = line.EndColumn
			};
		}

		public Collection<CustomDebugInformation> Read(ICustomDebugInformationProvider provider)
		{
			return new Collection<CustomDebugInformation>();
		}

		public void Dispose()
		{
			this.symbol_file.Dispose();
		}

		private readonly ModuleDefinition module;

		private readonly MonoSymbolFile symbol_file;

		private readonly Dictionary<string, Document> documents;
	}
}
