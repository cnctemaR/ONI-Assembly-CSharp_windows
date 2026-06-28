using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.VisualBasic
{
	internal class VBCodeCompiler : VBCodeGenerator, global::System.CodeDom.Compiler.ICodeCompiler
	{
		static VBCodeCompiler()
		{
			if (Path.DirectorySeparatorChar == '\\')
			{
				PropertyInfo property = typeof(Environment).GetProperty("GacPath", BindingFlags.Static | BindingFlags.NonPublic);
				MethodInfo getMethod = property.GetGetMethod(true);
				string directoryName = Path.GetDirectoryName((string)getMethod.Invoke(null, null));
				VBCodeCompiler.windowsMonoPath = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(directoryName)), "bin\\mono.bat");
				if (!File.Exists(VBCodeCompiler.windowsMonoPath))
				{
					VBCodeCompiler.windowsMonoPath = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(directoryName)), "bin\\mono.exe");
				}
				VBCodeCompiler.windowsvbncPath = Path.Combine(directoryName, "2.0\\vbnc.exe");
			}
		}

		public global::System.CodeDom.Compiler.CompilerResults CompileAssemblyFromDom(global::System.CodeDom.Compiler.CompilerParameters options, global::System.CodeDom.CodeCompileUnit e)
		{
			return this.CompileAssemblyFromDomBatch(options, new global::System.CodeDom.CodeCompileUnit[] { e });
		}

		public global::System.CodeDom.Compiler.CompilerResults CompileAssemblyFromDomBatch(global::System.CodeDom.Compiler.CompilerParameters options, global::System.CodeDom.CodeCompileUnit[] ea)
		{
			if (options == null)
			{
				throw new ArgumentNullException("options");
			}
			global::System.CodeDom.Compiler.CompilerResults compilerResults;
			try
			{
				compilerResults = this.CompileFromDomBatch(options, ea);
			}
			finally
			{
				options.TempFiles.Delete();
			}
			return compilerResults;
		}

		public global::System.CodeDom.Compiler.CompilerResults CompileAssemblyFromFile(global::System.CodeDom.Compiler.CompilerParameters options, string fileName)
		{
			return this.CompileAssemblyFromFileBatch(options, new string[] { fileName });
		}

		public global::System.CodeDom.Compiler.CompilerResults CompileAssemblyFromFileBatch(global::System.CodeDom.Compiler.CompilerParameters options, string[] fileNames)
		{
			if (options == null)
			{
				throw new ArgumentNullException("options");
			}
			global::System.CodeDom.Compiler.CompilerResults compilerResults;
			try
			{
				compilerResults = this.CompileFromFileBatch(options, fileNames);
			}
			finally
			{
				options.TempFiles.Delete();
			}
			return compilerResults;
		}

		public global::System.CodeDom.Compiler.CompilerResults CompileAssemblyFromSource(global::System.CodeDom.Compiler.CompilerParameters options, string source)
		{
			return this.CompileAssemblyFromSourceBatch(options, new string[] { source });
		}

		public global::System.CodeDom.Compiler.CompilerResults CompileAssemblyFromSourceBatch(global::System.CodeDom.Compiler.CompilerParameters options, string[] sources)
		{
			if (options == null)
			{
				throw new ArgumentNullException("options");
			}
			global::System.CodeDom.Compiler.CompilerResults compilerResults;
			try
			{
				compilerResults = this.CompileFromSourceBatch(options, sources);
			}
			finally
			{
				options.TempFiles.Delete();
			}
			return compilerResults;
		}

		private static string BuildArgs(global::System.CodeDom.Compiler.CompilerParameters options, string[] fileNames)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("/quiet ");
			if (options.GenerateExecutable)
			{
				stringBuilder.Append("/target:exe ");
			}
			else
			{
				stringBuilder.Append("/target:library ");
			}
			if (options.TreatWarningsAsErrors)
			{
				stringBuilder.Append("/warnaserror ");
			}
			if (options.OutputAssembly == null || options.OutputAssembly.Length == 0)
			{
				string text = ((!options.GenerateExecutable) ? "dll" : "exe");
				options.OutputAssembly = VBCodeCompiler.GetTempFileNameWithExtension(options.TempFiles, text, !options.GenerateInMemory);
			}
			stringBuilder.AppendFormat("/out:\"{0}\" ", options.OutputAssembly);
			bool flag = false;
			if (options.ReferencedAssemblies != null)
			{
				foreach (string text2 in options.ReferencedAssemblies)
				{
					if (string.Compare(text2, "Microsoft.VisualBasic", true, CultureInfo.InvariantCulture) == 0)
					{
						flag = true;
					}
					stringBuilder.AppendFormat("/r:\"{0}\" ", text2);
				}
			}
			if (!flag)
			{
				stringBuilder.Append("/r:\"Microsoft.VisualBasic.dll\" ");
			}
			if (options.CompilerOptions != null)
			{
				stringBuilder.Append(options.CompilerOptions);
				stringBuilder.Append(" ");
			}
			foreach (string text3 in fileNames)
			{
				stringBuilder.AppendFormat(" \"{0}\" ", text3);
			}
			return stringBuilder.ToString();
		}

		private static global::System.CodeDom.Compiler.CompilerError CreateErrorFromString(string error_string)
		{
			global::System.CodeDom.Compiler.CompilerError compilerError = new global::System.CodeDom.Compiler.CompilerError();
			global::System.Text.RegularExpressions.Regex regex = new global::System.Text.RegularExpressions.Regex("^(\\s*(?<file>.*)?\\((?<line>\\d*)(,(?<column>\\d*))?\\)\\s+)?:\\s*(?<level>Error|Warning)?\\s*(?<number>.*):\\s(?<message>.*)", global::System.Text.RegularExpressions.RegexOptions.ExplicitCapture | global::System.Text.RegularExpressions.RegexOptions.Compiled);
			global::System.Text.RegularExpressions.Match match = regex.Match(error_string);
			if (!match.Success)
			{
				return null;
			}
			if (string.Empty != match.Result("${file}"))
			{
				compilerError.FileName = match.Result("${file}").Trim();
			}
			if (string.Empty != match.Result("${line}"))
			{
				compilerError.Line = int.Parse(match.Result("${line}"));
			}
			if (string.Empty != match.Result("${column}"))
			{
				compilerError.Column = int.Parse(match.Result("${column}"));
			}
			if (match.Result("${level}").Trim() == "Warning")
			{
				compilerError.IsWarning = true;
			}
			compilerError.ErrorNumber = match.Result("${number}");
			compilerError.ErrorText = match.Result("${message}");
			return compilerError;
		}

		private static string GetTempFileNameWithExtension(global::System.CodeDom.Compiler.TempFileCollection temp_files, string extension, bool keepFile)
		{
			return temp_files.AddExtension(extension, keepFile);
		}

		private static string GetTempFileNameWithExtension(global::System.CodeDom.Compiler.TempFileCollection temp_files, string extension)
		{
			return temp_files.AddExtension(extension);
		}

		private global::System.CodeDom.Compiler.CompilerResults CompileFromFileBatch(global::System.CodeDom.Compiler.CompilerParameters options, string[] fileNames)
		{
			if (options == null)
			{
				throw new ArgumentNullException("options");
			}
			if (fileNames == null)
			{
				throw new ArgumentNullException("fileNames");
			}
			global::System.CodeDom.Compiler.CompilerResults compilerResults = new global::System.CodeDom.Compiler.CompilerResults(options.TempFiles);
			global::System.Diagnostics.Process process = new global::System.Diagnostics.Process();
			string text = string.Empty;
			if (Path.DirectorySeparatorChar == '\\')
			{
				process.StartInfo.FileName = VBCodeCompiler.windowsMonoPath;
				process.StartInfo.Arguments = VBCodeCompiler.windowsvbncPath + ' ' + VBCodeCompiler.BuildArgs(options, fileNames);
			}
			else
			{
				process.StartInfo.FileName = "vbnc";
				process.StartInfo.Arguments = VBCodeCompiler.BuildArgs(options, fileNames);
			}
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = true;
			try
			{
				process.Start();
			}
			catch (Exception ex)
			{
				global::System.ComponentModel.Win32Exception ex2 = ex as global::System.ComponentModel.Win32Exception;
				if (ex2 != null)
				{
					throw new SystemException(string.Format("Error running {0}: {1}", process.StartInfo.FileName, global::System.ComponentModel.Win32Exception.W32ErrorMessage(ex2.NativeErrorCode)));
				}
				throw;
			}
			try
			{
				text = process.StandardOutput.ReadToEnd();
				process.WaitForExit();
			}
			finally
			{
				compilerResults.NativeCompilerReturnValue = process.ExitCode;
				process.Close();
			}
			bool flag = true;
			if (compilerResults.NativeCompilerReturnValue == 1)
			{
				flag = false;
				string[] array = text.Split(Environment.NewLine.ToCharArray());
				foreach (string text2 in array)
				{
					global::System.CodeDom.Compiler.CompilerError compilerError = VBCodeCompiler.CreateErrorFromString(text2);
					if (compilerError != null)
					{
						compilerResults.Errors.Add(compilerError);
					}
				}
			}
			if ((!flag && !compilerResults.Errors.HasErrors) || (compilerResults.NativeCompilerReturnValue != 0 && compilerResults.NativeCompilerReturnValue != 1))
			{
				flag = false;
				global::System.CodeDom.Compiler.CompilerError compilerError2 = new global::System.CodeDom.Compiler.CompilerError(string.Empty, 0, 0, "VBNC_CRASH", text);
				compilerResults.Errors.Add(compilerError2);
			}
			if (flag)
			{
				if (options.GenerateInMemory)
				{
					using (FileStream fileStream = File.OpenRead(options.OutputAssembly))
					{
						byte[] array3 = new byte[fileStream.Length];
						fileStream.Read(array3, 0, array3.Length);
						compilerResults.CompiledAssembly = Assembly.Load(array3, null, options.Evidence);
						fileStream.Close();
					}
				}
				else
				{
					compilerResults.CompiledAssembly = Assembly.LoadFrom(options.OutputAssembly);
					compilerResults.PathToAssembly = options.OutputAssembly;
				}
			}
			else
			{
				compilerResults.CompiledAssembly = null;
			}
			return compilerResults;
		}

		private global::System.CodeDom.Compiler.CompilerResults CompileFromDomBatch(global::System.CodeDom.Compiler.CompilerParameters options, global::System.CodeDom.CodeCompileUnit[] ea)
		{
			if (options == null)
			{
				throw new ArgumentNullException("options");
			}
			if (ea == null)
			{
				throw new ArgumentNullException("ea");
			}
			string[] array = new string[ea.Length];
			global::System.Collections.Specialized.StringCollection referencedAssemblies = options.ReferencedAssemblies;
			for (int i = 0; i < ea.Length; i++)
			{
				global::System.CodeDom.CodeCompileUnit codeCompileUnit = ea[i];
				array[i] = VBCodeCompiler.GetTempFileNameWithExtension(options.TempFiles, i + ".vb");
				FileStream fileStream = new FileStream(array[i], FileMode.OpenOrCreate);
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
				((global::System.CodeDom.Compiler.ICodeGenerator)this).GenerateCodeFromCompileUnit(codeCompileUnit, streamWriter, new global::System.CodeDom.Compiler.CodeGeneratorOptions());
				streamWriter.Close();
				fileStream.Close();
			}
			return this.CompileAssemblyFromFileBatch(options, array);
		}

		private global::System.CodeDom.Compiler.CompilerResults CompileFromSourceBatch(global::System.CodeDom.Compiler.CompilerParameters options, string[] sources)
		{
			if (options == null)
			{
				throw new ArgumentNullException("options");
			}
			if (sources == null)
			{
				throw new ArgumentNullException("sources");
			}
			string[] array = new string[sources.Length];
			for (int i = 0; i < sources.Length; i++)
			{
				array[i] = VBCodeCompiler.GetTempFileNameWithExtension(options.TempFiles, i + ".vb");
				FileStream fileStream = new FileStream(array[i], FileMode.OpenOrCreate);
				using (StreamWriter streamWriter = new StreamWriter(fileStream))
				{
					streamWriter.Write(sources[i]);
					streamWriter.Close();
				}
				fileStream.Close();
			}
			return this.CompileFromFileBatch(options, array);
		}

		private static string windowsMonoPath;

		private static string windowsvbncPath;
	}
}
