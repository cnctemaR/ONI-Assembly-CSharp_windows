using System;
using System.Collections.Generic;
using System.Reflection;

namespace System.CodeDom.Compiler
{
	public sealed class CompilerInfo
	{
		private CompilerInfo()
		{
		}

		public string[] GetLanguages()
		{
			return this.CloneCompilerLanguages();
		}

		public string[] GetExtensions()
		{
			return this.CloneCompilerExtensions();
		}

		public Type CodeDomProviderType
		{
			get
			{
				if (this._type == null)
				{
					lock (this)
					{
						if (this._type == null)
						{
							this._type = Type.GetType(this._codeDomProviderTypeName);
						}
					}
				}
				return this._type;
			}
		}

		public bool IsCodeDomProviderTypeValid
		{
			get
			{
				return Type.GetType(this._codeDomProviderTypeName) != null;
			}
		}

		public CodeDomProvider CreateProvider()
		{
			if (this._providerOptions.Count > 0)
			{
				ConstructorInfo constructor = this.CodeDomProviderType.GetConstructor(new Type[] { typeof(IDictionary<string, string>) });
				if (constructor != null)
				{
					return (CodeDomProvider)constructor.Invoke(new object[] { this._providerOptions });
				}
			}
			return (CodeDomProvider)Activator.CreateInstance(this.CodeDomProviderType);
		}

		public CodeDomProvider CreateProvider(IDictionary<string, string> providerOptions)
		{
			if (providerOptions == null)
			{
				throw new ArgumentNullException("providerOptions");
			}
			ConstructorInfo constructor = this.CodeDomProviderType.GetConstructor(new Type[] { typeof(IDictionary<string, string>) });
			if (constructor != null)
			{
				return (CodeDomProvider)constructor.Invoke(new object[] { providerOptions });
			}
			throw new InvalidOperationException(global::SR.Format("This CodeDomProvider type does not have a constructor that takes providerOptions - \"{0}\"", this.CodeDomProviderType.ToString()));
		}

		public CompilerParameters CreateDefaultCompilerParameters()
		{
			return this.CloneCompilerParameters();
		}

		internal CompilerInfo(CompilerParameters compilerParams, string codeDomProviderTypeName, string[] compilerLanguages, string[] compilerExtensions)
		{
			this._compilerLanguages = compilerLanguages;
			this._compilerExtensions = compilerExtensions;
			this._codeDomProviderTypeName = codeDomProviderTypeName;
			this._compilerParams = compilerParams ?? new CompilerParameters();
		}

		internal CompilerInfo(CompilerParameters compilerParams, string codeDomProviderTypeName)
		{
			this._codeDomProviderTypeName = codeDomProviderTypeName;
			this._compilerParams = compilerParams ?? new CompilerParameters();
		}

		public override int GetHashCode()
		{
			return this._codeDomProviderTypeName.GetHashCode();
		}

		public override bool Equals(object o)
		{
			CompilerInfo compilerInfo = o as CompilerInfo;
			return compilerInfo != null && (this.CodeDomProviderType == compilerInfo.CodeDomProviderType && this.CompilerParams.WarningLevel == compilerInfo.CompilerParams.WarningLevel && this.CompilerParams.IncludeDebugInformation == compilerInfo.CompilerParams.IncludeDebugInformation) && this.CompilerParams.CompilerOptions == compilerInfo.CompilerParams.CompilerOptions;
		}

		private CompilerParameters CloneCompilerParameters()
		{
			return new CompilerParameters
			{
				IncludeDebugInformation = this._compilerParams.IncludeDebugInformation,
				TreatWarningsAsErrors = this._compilerParams.TreatWarningsAsErrors,
				WarningLevel = this._compilerParams.WarningLevel,
				CompilerOptions = this._compilerParams.CompilerOptions
			};
		}

		private string[] CloneCompilerLanguages()
		{
			return (string[])this._compilerLanguages.Clone();
		}

		private string[] CloneCompilerExtensions()
		{
			return (string[])this._compilerExtensions.Clone();
		}

		internal CompilerParameters CompilerParams
		{
			get
			{
				return this._compilerParams;
			}
		}

		internal IDictionary<string, string> ProviderOptions
		{
			get
			{
				return this._providerOptions;
			}
		}

		internal readonly IDictionary<string, string> _providerOptions = new Dictionary<string, string>();

		internal string _codeDomProviderTypeName;

		internal CompilerParameters _compilerParams;

		internal string[] _compilerLanguages;

		internal string[] _compilerExtensions;

		private Type _type;
	}
}
