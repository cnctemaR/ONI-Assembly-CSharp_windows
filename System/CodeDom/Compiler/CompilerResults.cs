using System;
using System.Collections.Specialized;
using System.Reflection;
using System.Security.Permissions;
using System.Security.Policy;

namespace System.CodeDom.Compiler
{
	[PermissionSet((SecurityAction)15, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
	[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
	[Serializable]
	public class CompilerResults
	{
		public CompilerResults(TempFileCollection tempFiles)
		{
			this.tempFiles = tempFiles;
		}

		public Assembly CompiledAssembly
		{
			get
			{
				if (this.compiledAssembly == null && this.pathToAssembly != null)
				{
					this.compiledAssembly = Assembly.LoadFrom(this.pathToAssembly);
				}
				return this.compiledAssembly;
			}
			set
			{
				this.compiledAssembly = value;
			}
		}

		public CompilerErrorCollection Errors
		{
			get
			{
				if (this.errors == null)
				{
					this.errors = new CompilerErrorCollection();
				}
				return this.errors;
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

		public int NativeCompilerReturnValue
		{
			get
			{
				return this.nativeCompilerReturnValue;
			}
			set
			{
				this.nativeCompilerReturnValue = value;
			}
		}

		public global::System.Collections.Specialized.StringCollection Output
		{
			get
			{
				if (this.output == null)
				{
					this.output = new global::System.Collections.Specialized.StringCollection();
				}
				return this.output;
			}
			internal set
			{
				this.output = value;
			}
		}

		public string PathToAssembly
		{
			get
			{
				return this.pathToAssembly;
			}
			set
			{
				this.pathToAssembly = value;
			}
		}

		public TempFileCollection TempFiles
		{
			get
			{
				return this.tempFiles;
			}
			set
			{
				this.tempFiles = value;
			}
		}

		private Assembly compiledAssembly;

		private CompilerErrorCollection errors = new CompilerErrorCollection();

		private Evidence evidence;

		private int nativeCompilerReturnValue;

		private global::System.Collections.Specialized.StringCollection output = new global::System.Collections.Specialized.StringCollection();

		private string pathToAssembly;

		private TempFileCollection tempFiles;
	}
}
