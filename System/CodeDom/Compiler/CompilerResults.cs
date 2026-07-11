using System;
using System.Collections.Specialized;
using System.Reflection;
using System.Security.Policy;

namespace System.CodeDom.Compiler
{
	[Serializable]
	public class CompilerResults
	{
		[Obsolete("CAS policy is obsolete and will be removed in a future release of the .NET Framework. Please see http://go2.microsoft.com/fwlink/?LinkId=131738 for more information.")]
		public Evidence Evidence
		{
			get
			{
				Evidence evidence = this._evidence;
				if (evidence == null)
				{
					return null;
				}
				return evidence.Clone();
			}
			set
			{
				this._evidence = ((value != null) ? value.Clone() : null);
			}
		}

		public CompilerResults(TempFileCollection tempFiles)
		{
			this._tempFiles = tempFiles;
		}

		public TempFileCollection TempFiles
		{
			get
			{
				return this._tempFiles;
			}
			set
			{
				this._tempFiles = value;
			}
		}

		public Assembly CompiledAssembly
		{
			get
			{
				if (this._compiledAssembly == null && this.PathToAssembly != null)
				{
					this._compiledAssembly = Assembly.Load(new AssemblyName
					{
						CodeBase = this.PathToAssembly
					});
				}
				return this._compiledAssembly;
			}
			set
			{
				this._compiledAssembly = value;
			}
		}

		public CompilerErrorCollection Errors
		{
			get
			{
				return this._errors;
			}
		}

		public StringCollection Output
		{
			get
			{
				return this._output;
			}
		}

		public string PathToAssembly { get; set; }

		public int NativeCompilerReturnValue { get; set; }

		private Evidence _evidence;

		private readonly CompilerErrorCollection _errors = new CompilerErrorCollection();

		private readonly StringCollection _output = new StringCollection();

		private Assembly _compiledAssembly;

		private TempFileCollection _tempFiles;
	}
}
