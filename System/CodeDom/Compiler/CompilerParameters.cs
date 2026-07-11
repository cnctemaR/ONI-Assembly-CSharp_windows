using System;
using System.Collections.Specialized;
using System.Security.Policy;

namespace System.CodeDom.Compiler
{
	[Serializable]
	public class CompilerParameters
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

		public CompilerParameters()
			: this(null, null)
		{
		}

		public CompilerParameters(string[] assemblyNames)
			: this(assemblyNames, null, false)
		{
		}

		public CompilerParameters(string[] assemblyNames, string outputName)
			: this(assemblyNames, outputName, false)
		{
		}

		public CompilerParameters(string[] assemblyNames, string outputName, bool includeDebugInformation)
		{
			if (assemblyNames != null)
			{
				this.ReferencedAssemblies.AddRange(assemblyNames);
			}
			this.OutputAssembly = outputName;
			this.IncludeDebugInformation = includeDebugInformation;
		}

		public string CoreAssemblyFileName { get; set; } = string.Empty;

		public bool GenerateExecutable { get; set; }

		public bool GenerateInMemory { get; set; }

		public StringCollection ReferencedAssemblies
		{
			get
			{
				return this._assemblyNames;
			}
		}

		public string MainClass { get; set; }

		public string OutputAssembly { get; set; }

		public TempFileCollection TempFiles
		{
			get
			{
				TempFileCollection tempFileCollection;
				if ((tempFileCollection = this._tempFiles) == null)
				{
					tempFileCollection = (this._tempFiles = new TempFileCollection());
				}
				return tempFileCollection;
			}
			set
			{
				this._tempFiles = value;
			}
		}

		public bool IncludeDebugInformation { get; set; }

		public bool TreatWarningsAsErrors { get; set; }

		public int WarningLevel { get; set; } = -1;

		public string CompilerOptions { get; set; }

		public string Win32Resource { get; set; }

		public StringCollection EmbeddedResources
		{
			get
			{
				return this._embeddedResources;
			}
		}

		public StringCollection LinkedResources
		{
			get
			{
				return this._linkedResources;
			}
		}

		public IntPtr UserToken { get; set; }

		private Evidence _evidence;

		private readonly StringCollection _assemblyNames = new StringCollection();

		private readonly StringCollection _embeddedResources = new StringCollection();

		private readonly StringCollection _linkedResources = new StringCollection();

		private TempFileCollection _tempFiles;
	}
}
