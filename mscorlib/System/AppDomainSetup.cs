using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Hosting;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Security.Policy;
using Unity;

namespace System
{
	[ClassInterface(ClassInterfaceType.None)]
	[ComVisible(true)]
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class AppDomainSetup : IAppDomainSetup
	{
		public AppDomainSetup()
		{
		}

		internal AppDomainSetup(AppDomainSetup setup)
		{
			this.application_base = setup.application_base;
			this.application_name = setup.application_name;
			this.cache_path = setup.cache_path;
			this.configuration_file = setup.configuration_file;
			this.dynamic_base = setup.dynamic_base;
			this.license_file = setup.license_file;
			this.private_bin_path = setup.private_bin_path;
			this.private_bin_path_probe = setup.private_bin_path_probe;
			this.shadow_copy_directories = setup.shadow_copy_directories;
			this.shadow_copy_files = setup.shadow_copy_files;
			this.publisher_policy = setup.publisher_policy;
			this.path_changed = setup.path_changed;
			this.loader_optimization = setup.loader_optimization;
			this.disallow_binding_redirects = setup.disallow_binding_redirects;
			this.disallow_code_downloads = setup.disallow_code_downloads;
			this._activationArguments = setup._activationArguments;
			this.domain_initializer = setup.domain_initializer;
			this.application_trust = setup.application_trust;
			this.domain_initializer_args = setup.domain_initializer_args;
			this.disallow_appbase_probe = setup.disallow_appbase_probe;
			this.configuration_bytes = setup.configuration_bytes;
		}

		public AppDomainSetup(ActivationArguments activationArguments)
		{
			this._activationArguments = activationArguments;
		}

		public AppDomainSetup(ActivationContext activationContext)
		{
			this._activationArguments = new ActivationArguments(activationContext);
		}

		private static string GetAppBase(string appBase)
		{
			if (appBase == null)
			{
				return null;
			}
			if (appBase.Length >= 8 && appBase.ToLower().StartsWith("file://"))
			{
				appBase = appBase.Substring(7);
				if (Path.DirectorySeparatorChar != '/')
				{
					appBase = appBase.Replace('/', Path.DirectorySeparatorChar);
				}
			}
			appBase = Path.GetFullPath(appBase);
			if (Path.DirectorySeparatorChar != '/')
			{
				bool flag = appBase.StartsWith("\\\\?\\", StringComparison.Ordinal);
				if (appBase.IndexOf(':', flag ? 6 : 2) != -1)
				{
					throw new NotSupportedException("The given path's format is not supported.");
				}
			}
			string directoryName = Path.GetDirectoryName(appBase);
			if (directoryName != null && directoryName.LastIndexOfAny(Path.GetInvalidPathChars()) >= 0)
			{
				throw new ArgumentException(string.Format(Locale.GetText("Invalid path characters in path: '{0}'"), appBase), "appBase");
			}
			string fileName = Path.GetFileName(appBase);
			if (fileName != null && fileName.LastIndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
			{
				throw new ArgumentException(string.Format(Locale.GetText("Invalid filename characters in path: '{0}'"), appBase), "appBase");
			}
			return appBase;
		}

		public string ApplicationBase
		{
			[SecuritySafeCritical]
			get
			{
				return AppDomainSetup.GetAppBase(this.application_base);
			}
			set
			{
				this.application_base = value;
			}
		}

		public string ApplicationName
		{
			get
			{
				return this.application_name;
			}
			set
			{
				this.application_name = value;
			}
		}

		public string CachePath
		{
			[SecuritySafeCritical]
			get
			{
				return this.cache_path;
			}
			set
			{
				this.cache_path = value;
			}
		}

		public string ConfigurationFile
		{
			[SecuritySafeCritical]
			get
			{
				if (this.configuration_file == null)
				{
					return null;
				}
				if (Path.IsPathRooted(this.configuration_file))
				{
					return this.configuration_file;
				}
				if (this.ApplicationBase == null)
				{
					throw new MemberAccessException("The ApplicationBase must be set before retrieving this property.");
				}
				return Path.Combine(this.ApplicationBase, this.configuration_file);
			}
			set
			{
				this.configuration_file = value;
			}
		}

		public bool DisallowPublisherPolicy
		{
			get
			{
				return this.publisher_policy;
			}
			set
			{
				this.publisher_policy = value;
			}
		}

		public string DynamicBase
		{
			[SecuritySafeCritical]
			get
			{
				if (this.dynamic_base == null)
				{
					return null;
				}
				if (Path.IsPathRooted(this.dynamic_base))
				{
					return this.dynamic_base;
				}
				if (this.ApplicationBase == null)
				{
					throw new MemberAccessException("The ApplicationBase must be set before retrieving this property.");
				}
				return Path.Combine(this.ApplicationBase, this.dynamic_base);
			}
			[SecuritySafeCritical]
			set
			{
				if (this.application_name == null)
				{
					throw new MemberAccessException("ApplicationName must be set before the DynamicBase can be set.");
				}
				this.dynamic_base = Path.Combine(value, ((uint)this.application_name.GetHashCode()).ToString("x"));
			}
		}

		public string LicenseFile
		{
			[SecuritySafeCritical]
			get
			{
				return this.license_file;
			}
			set
			{
				this.license_file = value;
			}
		}

		[MonoLimitation("In Mono this is controlled by the --share-code flag")]
		public LoaderOptimization LoaderOptimization
		{
			get
			{
				return this.loader_optimization;
			}
			set
			{
				this.loader_optimization = value;
			}
		}

		public string PrivateBinPath
		{
			[SecuritySafeCritical]
			get
			{
				return this.private_bin_path;
			}
			set
			{
				this.private_bin_path = value;
				this.path_changed = true;
			}
		}

		public string PrivateBinPathProbe
		{
			get
			{
				return this.private_bin_path_probe;
			}
			set
			{
				this.private_bin_path_probe = value;
				this.path_changed = true;
			}
		}

		public string ShadowCopyDirectories
		{
			[SecuritySafeCritical]
			get
			{
				return this.shadow_copy_directories;
			}
			set
			{
				this.shadow_copy_directories = value;
			}
		}

		public string ShadowCopyFiles
		{
			get
			{
				return this.shadow_copy_files;
			}
			set
			{
				this.shadow_copy_files = value;
			}
		}

		public bool DisallowBindingRedirects
		{
			get
			{
				return this.disallow_binding_redirects;
			}
			set
			{
				this.disallow_binding_redirects = value;
			}
		}

		public bool DisallowCodeDownload
		{
			get
			{
				return this.disallow_code_downloads;
			}
			set
			{
				this.disallow_code_downloads = value;
			}
		}

		public string TargetFrameworkName { get; set; }

		public ActivationArguments ActivationArguments
		{
			get
			{
				if (this._activationArguments != null)
				{
					return this._activationArguments;
				}
				this.DeserializeNonPrimitives();
				return this._activationArguments;
			}
			set
			{
				this._activationArguments = value;
			}
		}

		[MonoLimitation("it needs to be invoked within the created domain")]
		public AppDomainInitializer AppDomainInitializer
		{
			get
			{
				if (this.domain_initializer != null)
				{
					return this.domain_initializer;
				}
				this.DeserializeNonPrimitives();
				return this.domain_initializer;
			}
			set
			{
				this.domain_initializer = value;
			}
		}

		[MonoLimitation("it needs to be used to invoke the initializer within the created domain")]
		public string[] AppDomainInitializerArguments
		{
			get
			{
				return this.domain_initializer_args;
			}
			set
			{
				this.domain_initializer_args = value;
			}
		}

		[MonoNotSupported("This property exists but not considered.")]
		public ApplicationTrust ApplicationTrust
		{
			get
			{
				if (this.application_trust != null)
				{
					return this.application_trust;
				}
				this.DeserializeNonPrimitives();
				if (this.application_trust == null)
				{
					this.application_trust = new ApplicationTrust();
				}
				return this.application_trust;
			}
			set
			{
				this.application_trust = value;
			}
		}

		[MonoNotSupported("This property exists but not considered.")]
		public bool DisallowApplicationBaseProbing
		{
			get
			{
				return this.disallow_appbase_probe;
			}
			set
			{
				this.disallow_appbase_probe = value;
			}
		}

		[MonoNotSupported("This method exists but not considered.")]
		public byte[] GetConfigurationBytes()
		{
			if (this.configuration_bytes == null)
			{
				return null;
			}
			return this.configuration_bytes.Clone() as byte[];
		}

		[MonoNotSupported("This method exists but not considered.")]
		public void SetConfigurationBytes(byte[] value)
		{
			this.configuration_bytes = value;
		}

		private void DeserializeNonPrimitives()
		{
			lock (this)
			{
				if (this.serialized_non_primitives != null)
				{
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					MemoryStream memoryStream = new MemoryStream(this.serialized_non_primitives);
					object[] array = (object[])binaryFormatter.Deserialize(memoryStream);
					this._activationArguments = (ActivationArguments)array[0];
					this.domain_initializer = (AppDomainInitializer)array[1];
					this.application_trust = (ApplicationTrust)array[2];
					this.serialized_non_primitives = null;
				}
			}
		}

		internal void SerializeNonPrimitives()
		{
			object[] array = new object[] { this._activationArguments, this.domain_initializer, this.application_trust };
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			MemoryStream memoryStream = new MemoryStream();
			binaryFormatter.Serialize(memoryStream, array);
			this.serialized_non_primitives = memoryStream.ToArray();
		}

		[MonoTODO("not implemented, does not throw because it's used in testing moonlight")]
		public void SetCompatibilitySwitches(IEnumerable<string> switches)
		{
		}

		public string AppDomainManagerAssembly
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
			set
			{
				ThrowStub.ThrowNotSupportedException();
			}
		}

		public string AppDomainManagerType
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
			set
			{
				ThrowStub.ThrowNotSupportedException();
			}
		}

		public string[] PartialTrustVisibleAssemblies
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
			set
			{
				ThrowStub.ThrowNotSupportedException();
			}
		}

		public bool SandboxInterop
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return default(bool);
			}
			set
			{
				ThrowStub.ThrowNotSupportedException();
			}
		}

		[SecurityCritical]
		public void SetNativeFunction(string functionName, int functionVersion, IntPtr functionPointer)
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private string application_base;

		private string application_name;

		private string cache_path;

		private string configuration_file;

		private string dynamic_base;

		private string license_file;

		private string private_bin_path;

		private string private_bin_path_probe;

		private string shadow_copy_directories;

		private string shadow_copy_files;

		private bool publisher_policy;

		private bool path_changed;

		private LoaderOptimization loader_optimization;

		private bool disallow_binding_redirects;

		private bool disallow_code_downloads;

		private ActivationArguments _activationArguments;

		private AppDomainInitializer domain_initializer;

		[NonSerialized]
		private ApplicationTrust application_trust;

		private string[] domain_initializer_args;

		private bool disallow_appbase_probe;

		private byte[] configuration_bytes;

		private byte[] serialized_non_primitives;
	}
}
