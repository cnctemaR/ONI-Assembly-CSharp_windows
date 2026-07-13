using System;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using Mono.CompilerServices.SymbolWriter;

namespace Mono.Cecil.Mdb
{
	internal sealed class MdbWriter : ISymbolWriter, IDisposable
	{
		public MdbWriter(ModuleDefinition module, string assembly)
		{
			this.module = module;
			this.writer = new MonoSymbolWriter(assembly);
			this.source_files = new Dictionary<string, MdbWriter.SourceFile>();
		}

		public ISymbolReaderProvider GetReaderProvider()
		{
			return new MdbReaderProvider();
		}

		private MdbWriter.SourceFile GetSourceFile(Document document)
		{
			string url = document.Url;
			MdbWriter.SourceFile sourceFile;
			if (this.source_files.TryGetValue(url, out sourceFile))
			{
				return sourceFile;
			}
			SourceFileEntry sourceFileEntry = this.writer.DefineDocument(url, null, (document.Hash != null && document.Hash.Length == 16) ? document.Hash : null);
			sourceFile = new MdbWriter.SourceFile(this.writer.DefineCompilationUnit(sourceFileEntry), sourceFileEntry);
			this.source_files.Add(url, sourceFile);
			return sourceFile;
		}

		private void Populate(Collection<SequencePoint> sequencePoints, int[] offsets, int[] startRows, int[] endRows, int[] startCols, int[] endCols, out MdbWriter.SourceFile file)
		{
			MdbWriter.SourceFile sourceFile = null;
			for (int i = 0; i < sequencePoints.Count; i++)
			{
				SequencePoint sequencePoint = sequencePoints[i];
				offsets[i] = sequencePoint.Offset;
				if (sourceFile == null)
				{
					sourceFile = this.GetSourceFile(sequencePoint.Document);
				}
				startRows[i] = sequencePoint.StartLine;
				endRows[i] = sequencePoint.EndLine;
				startCols[i] = sequencePoint.StartColumn;
				endCols[i] = sequencePoint.EndColumn;
			}
			file = sourceFile;
		}

		public void Write(MethodDebugInformation info)
		{
			MdbWriter.SourceMethod sourceMethod = new MdbWriter.SourceMethod(info.method);
			Collection<SequencePoint> sequencePoints = info.SequencePoints;
			int count = sequencePoints.Count;
			if (count == 0)
			{
				return;
			}
			int[] array = new int[count];
			int[] array2 = new int[count];
			int[] array3 = new int[count];
			int[] array4 = new int[count];
			int[] array5 = new int[count];
			MdbWriter.SourceFile sourceFile;
			this.Populate(sequencePoints, array, array2, array3, array4, array5, out sourceFile);
			SourceMethodBuilder sourceMethodBuilder = this.writer.OpenMethod(sourceFile.CompilationUnit, 0, sourceMethod);
			for (int i = 0; i < count; i++)
			{
				sourceMethodBuilder.MarkSequencePoint(array[i], sourceFile.CompilationUnit.SourceFile, array2[i], array4[i], array3[i], array5[i], false);
			}
			if (info.scope != null)
			{
				this.WriteRootScope(info.scope, info);
			}
			this.writer.CloseMethod();
		}

		private void WriteRootScope(ScopeDebugInformation scope, MethodDebugInformation info)
		{
			this.WriteScopeVariables(scope);
			if (scope.HasScopes)
			{
				this.WriteScopes(scope.Scopes, info);
			}
		}

		private void WriteScope(ScopeDebugInformation scope, MethodDebugInformation info)
		{
			this.writer.OpenScope(scope.Start.Offset);
			this.WriteScopeVariables(scope);
			if (scope.HasScopes)
			{
				this.WriteScopes(scope.Scopes, info);
			}
			this.writer.CloseScope(scope.End.IsEndOfMethod ? info.code_size : scope.End.Offset);
		}

		private void WriteScopes(Collection<ScopeDebugInformation> scopes, MethodDebugInformation info)
		{
			for (int i = 0; i < scopes.Count; i++)
			{
				this.WriteScope(scopes[i], info);
			}
		}

		private void WriteScopeVariables(ScopeDebugInformation scope)
		{
			if (!scope.HasVariables)
			{
				return;
			}
			foreach (VariableDebugInformation variableDebugInformation in scope.variables)
			{
				if (!string.IsNullOrEmpty(variableDebugInformation.Name))
				{
					this.writer.DefineLocalVariable(variableDebugInformation.Index, variableDebugInformation.Name);
				}
			}
		}

		public ImageDebugHeader GetDebugHeader()
		{
			return new ImageDebugHeader();
		}

		public void Write()
		{
		}

		public void Write(ICustomDebugInformationProvider provider)
		{
		}

		public void Dispose()
		{
			this.writer.WriteSymbolFile(this.module.Mvid);
		}

		private readonly ModuleDefinition module;

		private readonly MonoSymbolWriter writer;

		private readonly Dictionary<string, MdbWriter.SourceFile> source_files;

		private class SourceFile : ISourceFile
		{
			public SourceFileEntry Entry
			{
				get
				{
					return this.entry;
				}
			}

			public CompileUnitEntry CompilationUnit
			{
				get
				{
					return this.compilation_unit;
				}
			}

			public SourceFile(CompileUnitEntry comp_unit, SourceFileEntry entry)
			{
				this.compilation_unit = comp_unit;
				this.entry = entry;
			}

			private readonly CompileUnitEntry compilation_unit;

			private readonly SourceFileEntry entry;
		}

		private class SourceMethod : IMethodDef
		{
			public string Name
			{
				get
				{
					return this.method.Name;
				}
			}

			public int Token
			{
				get
				{
					return this.method.MetadataToken.ToInt32();
				}
			}

			public SourceMethod(MethodDefinition method)
			{
				this.method = method;
			}

			private readonly MethodDefinition method;
		}
	}
}
