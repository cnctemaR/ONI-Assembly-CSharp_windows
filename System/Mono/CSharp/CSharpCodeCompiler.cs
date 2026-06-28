using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Mono.CSharp
{
	internal class CSharpCodeCompiler : CSharpCodeGenerator, global::System.CodeDom.Compiler.ICodeCompiler
	{
		public CSharpCodeCompiler()
		{
		}

		public CSharpCodeCompiler(IDictionary<string, string> providerOptions)
			: base(providerOptions)
		{
		}

		static CSharpCodeCompiler()
		{
			if (Path.DirectorySeparatorChar == '\\')
			{
				PropertyInfo property = typeof(Environment).GetProperty("GacPath", BindingFlags.Static | BindingFlags.NonPublic);
				MethodInfo getMethod = property.GetGetMethod(true);
				string directoryName = Path.GetDirectoryName((string)getMethod.Invoke(null, null));
				CSharpCodeCompiler.windowsMonoPath = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(directoryName)), "bin\\mono.bat");
				if (!File.Exists(CSharpCodeCompiler.windowsMonoPath))
				{
					CSharpCodeCompiler.windowsMonoPath = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(directoryName)), "bin\\mono.exe");
				}
				if (!File.Exists(CSharpCodeCompiler.windowsMonoPath))
				{
					CSharpCodeCompiler.windowsMonoPath = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(directoryName))), "mono\\mono\\mini\\mono.exe");
				}
				if (!File.Exists(CSharpCodeCompiler.windowsMonoPath))
				{
					throw new FileNotFoundException("Windows mono path not found: " + CSharpCodeCompiler.windowsMonoPath);
				}
				CSharpCodeCompiler.windowsMcsPath = Path.Combine(directoryName, "2.0\\gmcs.exe");
				if (!File.Exists(CSharpCodeCompiler.windowsMcsPath))
				{
					CSharpCodeCompiler.windowsMcsPath = Path.Combine(Path.GetDirectoryName(directoryName), "lib\\net_2_0\\gmcs.exe");
				}
				if (!File.Exists(CSharpCodeCompiler.windowsMcsPath))
				{
					throw new FileNotFoundException("Windows mcs path not found: " + CSharpCodeCompiler.windowsMcsPath);
				}
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
			if (Path.DirectorySeparatorChar == '\\')
			{
				process.StartInfo.FileName = CSharpCodeCompiler.windowsMonoPath;
				process.StartInfo.Arguments = "\"" + CSharpCodeCompiler.windowsMcsPath + "\" " + CSharpCodeCompiler.BuildArgs(options, fileNames, base.ProviderOptions);
			}
			else
			{
				process.StartInfo.FileName = "gmcs";
				process.StartInfo.Arguments = CSharpCodeCompiler.BuildArgs(options, fileNames, base.ProviderOptions);
			}
			this.mcsOutput = new global::System.Collections.Specialized.StringCollection();
			this.mcsOutMutex = new Mutex();
			string text = Environment.GetEnvironmentVariable("MONO_PATH");
			if (text == null)
			{
				text = string.Empty;
			}
			string privateBinPath = AppDomain.CurrentDomain.SetupInformation.PrivateBinPath;
			if (privateBinPath != null && privateBinPath.Length > 0)
			{
				text = string.Format("{0}:{1}", privateBinPath, text);
			}
			if (text.Length > 0)
			{
				global::System.Collections.Specialized.StringDictionary environmentVariables = process.StartInfo.EnvironmentVariables;
				if (environmentVariables.ContainsKey("MONO_PATH"))
				{
					environmentVariables["MONO_PATH"] = text;
				}
				else
				{
					environmentVariables.Add("MONO_PATH", text);
				}
			}
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.ErrorDataReceived += this.McsStderrDataReceived;
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
				process.BeginOutputReadLine();
				process.BeginErrorReadLine();
				process.WaitForExit();
				compilerResults.NativeCompilerReturnValue = process.ExitCode;
			}
			finally
			{
				process.CancelErrorRead();
				process.CancelOutputRead();
				process.Close();
			}
			global::System.Collections.Specialized.StringCollection stringCollection = this.mcsOutput;
			bool flag = true;
			foreach (string text2 in this.mcsOutput)
			{
				global::System.CodeDom.Compiler.CompilerError compilerError = CSharpCodeCompiler.CreateErrorFromString(text2);
				if (compilerError != null)
				{
					compilerResults.Errors.Add(compilerError);
					if (!compilerError.IsWarning)
					{
						flag = false;
					}
				}
			}
			if (stringCollection.Count > 0)
			{
				stringCollection.Insert(0, process.StartInfo.FileName + " " + process.StartInfo.Arguments + Environment.NewLine);
				compilerResults.Output = stringCollection;
			}
			if (flag)
			{
				if (!File.Exists(options.OutputAssembly))
				{
					StringBuilder stringBuilder = new StringBuilder();
					foreach (string text3 in stringCollection)
					{
						stringBuilder.Append(text3 + Environment.NewLine);
					}
					throw new Exception("Compiler failed to produce the assembly. Output: '" + stringBuilder.ToString() + "'");
				}
				if (options.GenerateInMemory)
				{
					using (FileStream fileStream = File.OpenRead(options.OutputAssembly))
					{
						byte[] array = new byte[fileStream.Length];
						fileStream.Read(array, 0, array.Length);
						compilerResults.CompiledAssembly = Assembly.Load(array, null, options.Evidence);
						fileStream.Close();
					}
				}
				else
				{
					compilerResults.PathToAssembly = options.OutputAssembly;
				}
			}
			else
			{
				compilerResults.CompiledAssembly = null;
			}
			return compilerResults;
		}

		private void McsStderrDataReceived(object sender, global::System.Diagnostics.DataReceivedEventArgs args)
		{
			if (args.Data != null)
			{
				this.mcsOutMutex.WaitOne();
				this.mcsOutput.Add(args.Data);
				this.mcsOutMutex.ReleaseMutex();
			}
		}

		private static string BuildArgs(global::System.CodeDom.Compiler.CompilerParameters options, string[] fileNames, IDictionary<string, string> providerOptions)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (options.GenerateExecutable)
			{
				stringBuilder.Append("/target:exe ");
			}
			else
			{
				stringBuilder.Append("/target:library ");
			}
			string privateBinPath = AppDomain.CurrentDomain.SetupInformation.PrivateBinPath;
			if (privateBinPath != null && privateBinPath.Length > 0)
			{
				stringBuilder.AppendFormat("/lib:\"{0}\" ", privateBinPath);
			}
			if (options.Win32Resource != null)
			{
				stringBuilder.AppendFormat("/win32res:\"{0}\" ", options.Win32Resource);
			}
			if (options.IncludeDebugInformation)
			{
				stringBuilder.Append("/debug+ /optimize- ");
			}
			else
			{
				stringBuilder.Append("/debug- /optimize+ ");
			}
			if (options.TreatWarningsAsErrors)
			{
				stringBuilder.Append("/warnaserror ");
			}
			if (options.WarningLevel >= 0)
			{
				stringBuilder.AppendFormat("/warn:{0} ", options.WarningLevel);
			}
			if (options.OutputAssembly == null || options.OutputAssembly.Length == 0)
			{
				string text = ((!options.GenerateExecutable) ? "dll" : "exe");
				options.OutputAssembly = CSharpCodeCompiler.GetTempFileNameWithExtension(options.TempFiles, text, !options.GenerateInMemory);
			}
			stringBuilder.AppendFormat("/out:\"{0}\" ", options.OutputAssembly);
			foreach (string text2 in options.ReferencedAssemblies)
			{
				if (text2 != null && text2.Length != 0)
				{
					stringBuilder.AppendFormat("/r:\"{0}\" ", text2);
				}
			}
			if (options.CompilerOptions != null)
			{
				stringBuilder.Append(options.CompilerOptions);
				stringBuilder.Append(" ");
			}
			foreach (string text3 in options.EmbeddedResources)
			{
				stringBuilder.AppendFormat("/resource:\"{0}\" ", text3);
			}
			foreach (string text4 in options.LinkedResources)
			{
				stringBuilder.AppendFormat("/linkresource:\"{0}\" ", text4);
			}
			if (providerOptions != null && providerOptions.Count > 0)
			{
				string text5;
				if (!providerOptions.TryGetValue("CompilerVersion", out text5))
				{
					text5 = "2.0";
				}
				if (text5.Length >= 1 && text5[0] == 'v')
				{
					text5 = text5.Substring(1);
				}
				string text6 = text5;
				if (text6 != null)
				{
					if (CSharpCodeCompiler.<>f__switch$map1 == null)
					{
						CSharpCodeCompiler.<>f__switch$map1 = new Dictionary<string, int>(2)
						{
							{ "2.0", 0 },
							{ "3.5", 1 }
						};
					}
					int num;
					if (CSharpCodeCompiler.<>f__switch$map1.TryGetValue(text6, out num))
					{
						if (num != 0)
						{
							if (num != 1)
							{
							}
						}
						else
						{
							stringBuilder.Append("/langversion:ISO-2");
						}
					}
				}
			}
			stringBuilder.Append(" -- ");
			foreach (string text7 in fileNames)
			{
				stringBuilder.AppendFormat("\"{0}\" ", text7);
			}
			return stringBuilder.ToString();
		}

		private static global::System.CodeDom.Compiler.CompilerError CreateErrorFromString(string error_string)
		{
			if (error_string.StartsWith("BETA"))
			{
				return null;
			}
			if (error_string == null || error_string == string.Empty)
			{
				return null;
			}
			global::System.CodeDom.Compiler.CompilerError compilerError = new global::System.CodeDom.Compiler.CompilerError();
			global::System.Text.RegularExpressions.Regex regex = new global::System.Text.RegularExpressions.Regex("^(\\s*(?<file>.*)\\((?<line>\\d*)(,(?<column>\\d*))?\\)(:)?\\s+)*(?<level>\\w+)\\s*(?<number>.*):\\s(?<message>.*)", global::System.Text.RegularExpressions.RegexOptions.ExplicitCapture | global::System.Text.RegularExpressions.RegexOptions.Compiled);
			global::System.Text.RegularExpressions.Match match = regex.Match(error_string);
			if (!match.Success)
			{
				compilerError.ErrorText = error_string;
				compilerError.IsWarning = false;
				compilerError.ErrorNumber = string.Empty;
				return compilerError;
			}
			if (string.Empty != match.Result("${file}"))
			{
				compilerError.FileName = match.Result("${file}");
			}
			if (string.Empty != match.Result("${line}"))
			{
				compilerError.Line = int.Parse(match.Result("${line}"));
			}
			if (string.Empty != match.Result("${column}"))
			{
				compilerError.Column = int.Parse(match.Result("${column}"));
			}
			string text = match.Result("${level}");
			if (text == "warning")
			{
				compilerError.IsWarning = true;
			}
			else if (text != "error")
			{
				return null;
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
				array[i] = CSharpCodeCompiler.GetTempFileNameWithExtension(options.TempFiles, i + ".cs");
				FileStream fileStream = new FileStream(array[i], FileMode.OpenOrCreate);
				StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8);
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
				array[i] = CSharpCodeCompiler.GetTempFileNameWithExtension(options.TempFiles, i + ".cs");
				FileStream fileStream = new FileStream(array[i], FileMode.OpenOrCreate);
				using (StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8))
				{
					streamWriter.Write(sources[i]);
					streamWriter.Close();
				}
				fileStream.Close();
			}
			return this.CompileFromFileBatch(options, array);
		}

		private static string windowsMcsPath;

		private static string windowsMonoPath;

		private Mutex mcsOutMutex;

		private global::System.Collections.Specialized.StringCollection mcsOutput;
	}
}
