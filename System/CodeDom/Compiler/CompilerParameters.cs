using System;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Policy;

namespace System.CodeDom.Compiler
{
	[PermissionSet((SecurityAction)15, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
	[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
	[Serializable]
	public class CompilerParameters
	{
		public CompilerParameters()
		{
		}

		public CompilerParameters(string[] assemblyNames)
		{
			this.referencedAssemblies = new global::System.Collections.Specialized.StringCollection();
			this.referencedAssemblies.AddRange(assemblyNames);
		}

		public CompilerParameters(string[] assemblyNames, string output)
		{
			this.referencedAssemblies = new global::System.Collections.Specialized.StringCollection();
			this.referencedAssemblies.AddRange(assemblyNames);
			this.outputAssembly = output;
		}

		public CompilerParameters(string[] assemblyNames, string output, bool includeDebugInfo)
		{
			this.referencedAssemblies = new global::System.Collections.Specialized.StringCollection();
			this.referencedAssemblies.AddRange(assemblyNames);
			this.outputAssembly = output;
			this.includeDebugInformation = includeDebugInfo;
		}

		public string CompilerOptions
		{
			get
			{
				return this.compilerOptions;
			}
			set
			{
				this.compilerOptions = value;
			}
		}

		public Evidence Evidence
		{
			get
			{
				return this.evidence;
			}
			[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"ControlEvidence\"/>\n</PermissionSet>\n")]
			set
			{
				this.evidence = value;
			}
		}

		public bool GenerateExecutable
		{
			get
			{
				return this.generateExecutable;
			}
			set
			{
				this.generateExecutable = value;
			}
		}

		public bool GenerateInMemory
		{
			get
			{
				return this.generateInMemory;
			}
			set
			{
				this.generateInMemory = value;
			}
		}

		public bool IncludeDebugInformation
		{
			get
			{
				return this.includeDebugInformation;
			}
			set
			{
				this.includeDebugInformation = value;
			}
		}

		public string MainClass
		{
			get
			{
				return this.mainClass;
			}
			set
			{
				this.mainClass = value;
			}
		}

		public string OutputAssembly
		{
			get
			{
				return this.outputAssembly;
			}
			set
			{
				this.outputAssembly = value;
			}
		}

		public global::System.Collections.Specialized.StringCollection ReferencedAssemblies
		{
			get
			{
				if (this.referencedAssemblies == null)
				{
					this.referencedAssemblies = new global::System.Collections.Specialized.StringCollection();
				}
				return this.referencedAssemblies;
			}
		}

		public TempFileCollection TempFiles
		{
			get
			{
				if (this.tempFiles == null)
				{
					this.tempFiles = new TempFileCollection();
				}
				return this.tempFiles;
			}
			set
			{
				this.tempFiles = value;
			}
		}

		public bool TreatWarningsAsErrors
		{
			get
			{
				return this.treatWarningsAsErrors;
			}
			set
			{
				this.treatWarningsAsErrors = value;
			}
		}

		public IntPtr UserToken
		{
			get
			{
				return this.userToken;
			}
			set
			{
				this.userToken = value;
			}
		}

		public int WarningLevel
		{
			get
			{
				return this.warningLevel;
			}
			set
			{
				this.warningLevel = value;
			}
		}

		public string Win32Resource
		{
			get
			{
				return this.win32Resource;
			}
			set
			{
				this.win32Resource = value;
			}
		}

		[ComVisible(false)]
		public global::System.Collections.Specialized.StringCollection EmbeddedResources
		{
			get
			{
				if (this.embedded_resources == null)
				{
					this.embedded_resources = new global::System.Collections.Specialized.StringCollection();
				}
				return this.embedded_resources;
			}
		}

		[ComVisible(false)]
		public global::System.Collections.Specialized.StringCollection LinkedResources
		{
			get
			{
				if (this.linked_resources == null)
				{
					this.linked_resources = new global::System.Collections.Specialized.StringCollection();
				}
				return this.linked_resources;
			}
		}

		private string compilerOptions;

		private Evidence evidence;

		private bool generateExecutable;

		private bool generateInMemory;

		private bool includeDebugInformation;

		private string mainClass;

		private string outputAssembly;

		private global::System.Collections.Specialized.StringCollection referencedAssemblies;

		private TempFileCollection tempFiles;

		private bool treatWarningsAsErrors;

		private IntPtr userToken = IntPtr.Zero;

		private int warningLevel = -1;

		private string win32Resource;

		private global::System.Collections.Specialized.StringCollection embedded_resources;

		private global::System.Collections.Specialized.StringCollection linked_resources;
	}
}
