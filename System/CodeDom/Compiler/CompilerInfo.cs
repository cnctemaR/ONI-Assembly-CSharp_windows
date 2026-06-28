using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Security.Permissions;

namespace System.CodeDom.Compiler
{
	[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"\nUnrestricted=\"true\"/>\n")]
	public sealed class CompilerInfo
	{
		internal CompilerInfo()
		{
		}

		internal void Init()
		{
			if (this.inited)
			{
				return;
			}
			this.inited = true;
			this.type = Type.GetType(this.TypeName);
			if (this.type == null)
			{
				return;
			}
			if (!typeof(CodeDomProvider).IsAssignableFrom(this.type))
			{
				this.type = null;
			}
		}

		public Type CodeDomProviderType
		{
			get
			{
				if (this.type == null)
				{
					this.type = Type.GetType(this.TypeName, false);
					if (this.type == null)
					{
						throw new ConfigurationErrorsException("Unable to locate compiler type '" + this.TypeName + "'");
					}
				}
				return this.type;
			}
		}

		public bool IsCodeDomProviderTypeValid
		{
			get
			{
				return this.type != null;
			}
		}

		public CompilerParameters CreateDefaultCompilerParameters()
		{
			CompilerParameters compilerParameters = new CompilerParameters();
			if (this.CompilerOptions == null)
			{
				compilerParameters.CompilerOptions = string.Empty;
			}
			else
			{
				compilerParameters.CompilerOptions = this.CompilerOptions;
			}
			compilerParameters.WarningLevel = this.WarningLevel;
			return compilerParameters;
		}

		public CodeDomProvider CreateProvider()
		{
			Type codeDomProviderType = this.CodeDomProviderType;
			if (this.ProviderOptions != null && this.ProviderOptions.Count > 0)
			{
				ConstructorInfo constructor = codeDomProviderType.GetConstructor(new Type[] { typeof(Dictionary<string, string>) });
				if (constructor != null)
				{
					return (CodeDomProvider)constructor.Invoke(new object[] { this.ProviderOptions });
				}
			}
			return (CodeDomProvider)Activator.CreateInstance(codeDomProviderType);
		}

		public override bool Equals(object o)
		{
			if (!(o is CompilerInfo))
			{
				return false;
			}
			CompilerInfo compilerInfo = (CompilerInfo)o;
			return compilerInfo.TypeName == this.TypeName;
		}

		public override int GetHashCode()
		{
			return this.TypeName.GetHashCode();
		}

		public string[] GetExtensions()
		{
			return this.Extensions.Split(new char[] { ';' });
		}

		public string[] GetLanguages()
		{
			return this.Languages.Split(new char[] { ';' });
		}

		internal string Languages;

		internal string Extensions;

		internal string TypeName;

		internal int WarningLevel;

		internal string CompilerOptions;

		internal Dictionary<string, string> ProviderOptions;

		private bool inited;

		private Type type;
	}
}
