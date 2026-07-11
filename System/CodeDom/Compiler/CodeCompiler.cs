using System;
using System.Collections.Specialized;
using System.IO;
using System.Security.Permissions;
using System.Text;

namespace System.CodeDom.Compiler
{
	public abstract class CodeCompiler : CodeGenerator, ICodeCompiler
	{
		CompilerResults ICodeCompiler.CompileAssemblyFromDom(CompilerParameters options, CodeCompileUnit e)
		{
			return this.FromDom(options, e);
		}

		CompilerResults ICodeCompiler.CompileAssemblyFromDomBatch(CompilerParameters options, CodeCompileUnit[] ea)
		{
			return this.FromDomBatch(options, ea);
		}

		CompilerResults ICodeCompiler.CompileAssemblyFromFile(CompilerParameters options, string fileName)
		{
			return this.FromFile(options, fileName);
		}

		CompilerResults ICodeCompiler.CompileAssemblyFromFileBatch(CompilerParameters options, string[] fileNames)
		{
			return this.FromFileBatch(options, fileNames);
		}

		CompilerResults ICodeCompiler.CompileAssemblyFromSource(CompilerParameters options, string source)
		{
			return this.FromSource(options, source);
		}

		CompilerResults ICodeCompiler.CompileAssemblyFromSourceBatch(CompilerParameters options, string[] sources)
		{
			return this.FromSourceBatch(options, sources);
		}

		protected abstract string CompilerName { get; }

		protected abstract string FileExtension { get; }

		protected abstract string CmdArgsFromParameters(CompilerParameters options);

		protected virtual CompilerResults FromDom(CompilerParameters options, CodeCompileUnit e)
		{
			return this.FromDomBatch(options, new CodeCompileUnit[] { e });
		}

		protected virtual CompilerResults FromDomBatch(CompilerParameters options, CodeCompileUnit[] ea)
		{
			string[] array = new string[ea.Length];
			int num = 0;
			if (options == null)
			{
				options = new CompilerParameters();
			}
			global::System.Collections.Specialized.StringCollection referencedAssemblies = options.ReferencedAssemblies;
			foreach (CodeCompileUnit codeCompileUnit in ea)
			{
				array[num] = Path.ChangeExtension(Path.GetTempFileName(), this.FileExtension);
				FileStream fileStream = new FileStream(array[num], FileMode.OpenOrCreate);
				StreamWriter streamWriter = new StreamWriter(fileStream);
				if (codeCompileUnit.ReferencedAssemblies != null)
				{
					foreach (string text in codeCompileUnit.ReferencedAssemblies)
					{
						if (!referencedAssemblies.Contains(text))
						{
							referencedAssemblies.Add(text);
						}
					}
				}
				((ICodeGenerator)this).GenerateCodeFromCompileUnit(codeCompileUnit, streamWriter, new CodeGeneratorOptions());
				streamWriter.Close();
				fileStream.Close();
				num++;
			}
			return this.Compile(options, array, false);
		}

		protected virtual CompilerResults FromFile(CompilerParameters options, string fileName)
		{
			return this.FromFileBatch(options, new string[] { fileName });
		}

		protected virtual CompilerResults FromFileBatch(CompilerParameters options, string[] fileNames)
		{
			return this.Compile(options, fileNames, true);
		}

		protected virtual CompilerResults FromSource(CompilerParameters options, string source)
		{
			return this.FromSourceBatch(options, new string[] { source });
		}

		protected virtual CompilerResults FromSourceBatch(CompilerParameters options, string[] sources)
		{
			string[] array = new string[sources.Length];
			int num = 0;
			foreach (string text in sources)
			{
				array[num] = Path.ChangeExtension(Path.GetTempFileName(), this.FileExtension);
				FileStream fileStream = new FileStream(array[num], FileMode.OpenOrCreate);
				StreamWriter streamWriter = new StreamWriter(fileStream);
				streamWriter.Write(text);
				streamWriter.Close();
				fileStream.Close();
				num++;
			}
			return this.Compile(options, array, false);
		}

		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
		private CompilerResults Compile(CompilerParameters options, string[] fileNames, bool keepFiles)
		{
			if (options == null)
			{
				throw new ArgumentNullException("options");
			}
			if (fileNames == null)
			{
				throw new ArgumentNullException("fileNames");
			}
			options.TempFiles = new TempFileCollection();
			foreach (string text in fileNames)
			{
				options.TempFiles.AddFile(text, keepFiles);
			}
			options.TempFiles.KeepFiles = keepFiles;
			string empty = string.Empty;
			string empty2 = string.Empty;
			string text2 = this.CompilerName + " " + this.CmdArgsFromParameters(options);
			CompilerResults compilerResults = new CompilerResults(new TempFileCollection());
			compilerResults.NativeCompilerReturnValue = Executor.ExecWaitWithCapture(text2, options.TempFiles, ref empty, ref empty2);
			string[] array = empty.Split(Environment.NewLine.ToCharArray());
			foreach (string text3 in array)
			{
				this.ProcessCompilerOutputLine(compilerResults, text3);
			}
			if (compilerResults.Errors.Count == 0)
			{
				compilerResults.PathToAssembly = options.OutputAssembly;
			}
			return compilerResults;
		}

		[global::System.MonoTODO]
		protected virtual string GetResponseFileCmdArgs(CompilerParameters options, string cmdArgs)
		{
			throw new NotImplementedException();
		}

		protected static string JoinStringArray(string[] sa, string separator)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = sa.Length;
			if (num > 1)
			{
				for (int i = 0; i < num - 1; i++)
				{
					stringBuilder.Append("\"");
					stringBuilder.Append(sa[i]);
					stringBuilder.Append("\"");
					stringBuilder.Append(separator);
				}
			}
			if (num > 0)
			{
				stringBuilder.Append("\"");
				stringBuilder.Append(sa[num - 1]);
				stringBuilder.Append("\"");
			}
			return stringBuilder.ToString();
		}

		protected abstract void ProcessCompilerOutputLine(CompilerResults results, string line);
	}
}
