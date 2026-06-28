using System;
using System.Collections;
using System.Configuration.Assemblies;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;

namespace System.Reflection
{
	[ComDefaultInterface(typeof(_Assembly))]
	[ClassInterface(ClassInterfaceType.None)]
	[ComVisible(true)]
	[Serializable]
	public class Assembly : ISerializable, ICustomAttributeProvider, _Assembly, IEvidenceFactory
	{
		internal Assembly()
		{
			this.resolve_event_holder = new Assembly.ResolveEventHolder();
		}

		public event ModuleResolveEventHandler ModuleResolve
		{
			[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"ControlAppDomain\"/>\n</PermissionSet>\n")]
			add
			{
				this.resolve_event_holder.ModuleResolve += value;
			}
			[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"ControlAppDomain\"/>\n</PermissionSet>\n")]
			remove
			{
				this.resolve_event_holder.ModuleResolve -= value;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern string get_code_base(bool escaped);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern string get_fullname();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern string get_location();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern string InternalImageRuntimeVersion();

		private string GetCodeBase(bool escaped)
		{
			string text = this.get_code_base(escaped);
			if (SecurityManager.SecurityEnabled && string.Compare("FILE://", 0, text, 0, 7, true, CultureInfo.InvariantCulture) == 0)
			{
				string text2 = text.Substring(7);
				new FileIOPermission(FileIOPermissionAccess.PathDiscovery, text2).Demand();
			}
			return text;
		}

		public virtual string CodeBase
		{
			get
			{
				return this.GetCodeBase(false);
			}
		}

		public virtual string EscapedCodeBase
		{
			get
			{
				return this.GetCodeBase(true);
			}
		}

		public virtual string FullName
		{
			get
			{
				return this.ToString();
			}
		}

		public virtual extern MethodInfo EntryPoint
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public virtual Evidence Evidence
		{
			[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"ControlEvidence\"/>\n</PermissionSet>\n")]
			get
			{
				return this.UnprotectedGetEvidence();
			}
		}

		internal Evidence UnprotectedGetEvidence()
		{
			if (this._evidence == null)
			{
				lock (this)
				{
					this._evidence = Evidence.GetDefaultHostEvidence(this);
				}
			}
			return this._evidence;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool get_global_assembly_cache();

		public bool GlobalAssemblyCache
		{
			get
			{
				return this.get_global_assembly_cache();
			}
		}

		internal bool FromByteArray
		{
			set
			{
				this.fromByteArray = value;
			}
		}

		public virtual string Location
		{
			get
			{
				if (this.fromByteArray)
				{
					return string.Empty;
				}
				string location = this.get_location();
				if (location != string.Empty && SecurityManager.SecurityEnabled)
				{
					new FileIOPermission(FileIOPermissionAccess.PathDiscovery, location).Demand();
				}
				return location;
			}
		}

		[ComVisible(false)]
		public virtual string ImageRuntimeVersion
		{
			get
			{
				return this.InternalImageRuntimeVersion();
			}
		}

		[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"SerializationFormatter\"/>\n</PermissionSet>\n")]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			UnitySerializationHolder.GetAssemblyData(this, info, context);
		}

		public virtual bool IsDefined(Type attributeType, bool inherit)
		{
			return MonoCustomAttrs.IsDefined(this, attributeType, inherit);
		}

		public virtual object[] GetCustomAttributes(bool inherit)
		{
			return MonoCustomAttrs.GetCustomAttributes(this, inherit);
		}

		public virtual object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			return MonoCustomAttrs.GetCustomAttributes(this, attributeType, inherit);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern object GetFilesInternal(string name, bool getResourceModules);

		public virtual FileStream[] GetFiles()
		{
			return this.GetFiles(false);
		}

		public virtual FileStream[] GetFiles(bool getResourceModules)
		{
			string[] array = (string[])this.GetFilesInternal(null, getResourceModules);
			if (array == null)
			{
				return new FileStream[0];
			}
			string location = this.Location;
			FileStream[] array2;
			if (location != string.Empty)
			{
				array2 = new FileStream[array.Length + 1];
				array2[0] = new FileStream(location, FileMode.Open, FileAccess.Read);
				for (int i = 0; i < array.Length; i++)
				{
					array2[i + 1] = new FileStream(array[i], FileMode.Open, FileAccess.Read);
				}
			}
			else
			{
				array2 = new FileStream[array.Length];
				for (int j = 0; j < array.Length; j++)
				{
					array2[j] = new FileStream(array[j], FileMode.Open, FileAccess.Read);
				}
			}
			return array2;
		}

		public virtual FileStream GetFile(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException(null, "Name cannot be null.");
			}
			if (name.Length == 0)
			{
				throw new ArgumentException("Empty name is not valid");
			}
			string text = (string)this.GetFilesInternal(name, true);
			if (text != null)
			{
				return new FileStream(text, FileMode.Open, FileAccess.Read);
			}
			return null;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern IntPtr GetManifestResourceInternal(string name, out int size, out Module module);

		public unsafe virtual Stream GetManifestResourceStream(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name.Length == 0)
			{
				throw new ArgumentException("String cannot have zero length.", "name");
			}
			ManifestResourceInfo manifestResourceInfo = this.GetManifestResourceInfo(name);
			if (manifestResourceInfo == null)
			{
				return null;
			}
			if (manifestResourceInfo.ReferencedAssembly != null)
			{
				return manifestResourceInfo.ReferencedAssembly.GetManifestResourceStream(name);
			}
			if (manifestResourceInfo.FileName != null && manifestResourceInfo.ResourceLocation == (ResourceLocation)0)
			{
				if (this.fromByteArray)
				{
					throw new FileNotFoundException(manifestResourceInfo.FileName);
				}
				string directoryName = Path.GetDirectoryName(this.Location);
				string text = Path.Combine(directoryName, manifestResourceInfo.FileName);
				return new FileStream(text, FileMode.Open, FileAccess.Read);
			}
			else
			{
				int num;
				Module module;
				IntPtr manifestResourceInternal = this.GetManifestResourceInternal(name, out num, out module);
				if (manifestResourceInternal == (IntPtr)0)
				{
					return null;
				}
				UnmanagedMemoryStream unmanagedMemoryStream = new UnmanagedMemoryStream((byte*)(void*)manifestResourceInternal, (long)num);
				unmanagedMemoryStream.Closed += new Assembly.ResourceCloseHandler(module).OnClose;
				return unmanagedMemoryStream;
			}
		}

		public virtual Stream GetManifestResourceStream(Type type, string name)
		{
			string text;
			if (type != null)
			{
				text = type.Namespace;
			}
			else
			{
				if (name == null)
				{
					throw new ArgumentNullException("type");
				}
				text = null;
			}
			if (text == null || text.Length == 0)
			{
				return this.GetManifestResourceStream(name);
			}
			return this.GetManifestResourceStream(text + "." + name);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal virtual extern Type[] GetTypes(bool exportedOnly);

		public virtual Type[] GetTypes()
		{
			return this.GetTypes(false);
		}

		public virtual Type[] GetExportedTypes()
		{
			return this.GetTypes(true);
		}

		public virtual Type GetType(string name, bool throwOnError)
		{
			return this.GetType(name, throwOnError, false);
		}

		public virtual Type GetType(string name)
		{
			return this.GetType(name, false, false);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern Type InternalGetType(Module module, string name, bool throwOnError, bool ignoreCase);

		public Type GetType(string name, bool throwOnError, bool ignoreCase)
		{
			if (name == null)
			{
				throw new ArgumentNullException(name);
			}
			if (name.Length == 0)
			{
				throw new ArgumentException("name", "Name cannot be empty");
			}
			return this.InternalGetType(null, name, throwOnError, ignoreCase);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void InternalGetAssemblyName(string assemblyFile, AssemblyName aname);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void FillName(Assembly ass, AssemblyName aname);

		[MonoTODO("copiedName == true is not supported")]
		public virtual AssemblyName GetName(bool copiedName)
		{
			if (SecurityManager.SecurityEnabled)
			{
				this.GetCodeBase(true);
			}
			return this.UnprotectedGetName();
		}

		public virtual AssemblyName GetName()
		{
			return this.GetName(false);
		}

		internal virtual AssemblyName UnprotectedGetName()
		{
			AssemblyName assemblyName = new AssemblyName();
			Assembly.FillName(this, assemblyName);
			return assemblyName;
		}

		public override string ToString()
		{
			if (this.assemblyName != null)
			{
				return this.assemblyName;
			}
			this.assemblyName = this.get_fullname();
			return this.assemblyName;
		}

		public static string CreateQualifiedName(string assemblyName, string typeName)
		{
			return typeName + ", " + assemblyName;
		}

		public static Assembly GetAssembly(Type type)
		{
			if (type != null)
			{
				return type.Assembly;
			}
			throw new ArgumentNullException("type");
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Assembly GetEntryAssembly();

		public Assembly GetSatelliteAssembly(CultureInfo culture)
		{
			return this.GetSatelliteAssembly(culture, null, true);
		}

		public Assembly GetSatelliteAssembly(CultureInfo culture, Version version)
		{
			return this.GetSatelliteAssembly(culture, version, true);
		}

		internal Assembly GetSatelliteAssemblyNoThrow(CultureInfo culture, Version version)
		{
			return this.GetSatelliteAssembly(culture, version, false);
		}

		private Assembly GetSatelliteAssembly(CultureInfo culture, Version version, bool throwOnError)
		{
			if (culture == null)
			{
				throw new ArgumentException("culture");
			}
			AssemblyName name = this.GetName(true);
			if (version != null)
			{
				name.Version = version;
			}
			name.CultureInfo = culture;
			name.Name += ".resources";
			try
			{
				Assembly assembly = AppDomain.CurrentDomain.LoadSatellite(name, false);
				if (assembly != null)
				{
					return assembly;
				}
			}
			catch (FileNotFoundException)
			{
			}
			string directoryName = Path.GetDirectoryName(this.Location);
			string text = Path.Combine(directoryName, Path.Combine(culture.Name, name.Name + ".dll"));
			if (!throwOnError && !File.Exists(text))
			{
				return null;
			}
			return Assembly.LoadFrom(text);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Assembly LoadFrom(string assemblyFile, bool refonly);

		public static Assembly LoadFrom(string assemblyFile)
		{
			return Assembly.LoadFrom(assemblyFile, false);
		}

		public static Assembly LoadFrom(string assemblyFile, Evidence securityEvidence)
		{
			Assembly assembly = Assembly.LoadFrom(assemblyFile, false);
			if (assembly != null && securityEvidence != null)
			{
				assembly.Evidence.Merge(securityEvidence);
			}
			return assembly;
		}

		[MonoTODO("This overload is not currently implemented")]
		public static Assembly LoadFrom(string assemblyFile, Evidence securityEvidence, byte[] hashValue, AssemblyHashAlgorithm hashAlgorithm)
		{
			if (assemblyFile == null)
			{
				throw new ArgumentNullException("assemblyFile");
			}
			if (assemblyFile == string.Empty)
			{
				throw new ArgumentException("Name can't be the empty string", "assemblyFile");
			}
			throw new NotImplementedException();
		}

		public static Assembly LoadFile(string path, Evidence securityEvidence)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path == string.Empty)
			{
				throw new ArgumentException("Path can't be empty", "path");
			}
			return Assembly.LoadFrom(path, securityEvidence);
		}

		public static Assembly LoadFile(string path)
		{
			return Assembly.LoadFile(path, null);
		}

		public static Assembly Load(string assemblyString)
		{
			return AppDomain.CurrentDomain.Load(assemblyString);
		}

		public static Assembly Load(string assemblyString, Evidence assemblySecurity)
		{
			return AppDomain.CurrentDomain.Load(assemblyString, assemblySecurity);
		}

		public static Assembly Load(AssemblyName assemblyRef)
		{
			return AppDomain.CurrentDomain.Load(assemblyRef);
		}

		public static Assembly Load(AssemblyName assemblyRef, Evidence assemblySecurity)
		{
			return AppDomain.CurrentDomain.Load(assemblyRef, assemblySecurity);
		}

		public static Assembly Load(byte[] rawAssembly)
		{
			return AppDomain.CurrentDomain.Load(rawAssembly);
		}

		public static Assembly Load(byte[] rawAssembly, byte[] rawSymbolStore)
		{
			return AppDomain.CurrentDomain.Load(rawAssembly, rawSymbolStore);
		}

		public static Assembly Load(byte[] rawAssembly, byte[] rawSymbolStore, Evidence securityEvidence)
		{
			return AppDomain.CurrentDomain.Load(rawAssembly, rawSymbolStore, securityEvidence);
		}

		public static Assembly ReflectionOnlyLoad(byte[] rawAssembly)
		{
			return AppDomain.CurrentDomain.Load(rawAssembly, null, null, true);
		}

		public static Assembly ReflectionOnlyLoad(string assemblyString)
		{
			return AppDomain.CurrentDomain.Load(assemblyString, null, true);
		}

		public static Assembly ReflectionOnlyLoadFrom(string assemblyFile)
		{
			if (assemblyFile == null)
			{
				throw new ArgumentNullException("assemblyFile");
			}
			return Assembly.LoadFrom(assemblyFile, true);
		}

		[Obsolete("")]
		public static Assembly LoadWithPartialName(string partialName)
		{
			return Assembly.LoadWithPartialName(partialName, null);
		}

		[MonoTODO("Not implemented")]
		public Module LoadModule(string moduleName, byte[] rawModule)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Not implemented")]
		public Module LoadModule(string moduleName, byte[] rawModule, byte[] rawSymbolStore)
		{
			throw new NotImplementedException();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Assembly load_with_partial_name(string name, Evidence e);

		[Obsolete("")]
		public static Assembly LoadWithPartialName(string partialName, Evidence securityEvidence)
		{
			return Assembly.LoadWithPartialName(partialName, securityEvidence, true);
		}

		internal static Assembly LoadWithPartialName(string partialName, Evidence securityEvidence, bool oldBehavior)
		{
			if (!oldBehavior)
			{
				throw new NotImplementedException();
			}
			if (partialName == null)
			{
				throw new NullReferenceException();
			}
			return Assembly.load_with_partial_name(partialName, securityEvidence);
		}

		public object CreateInstance(string typeName)
		{
			return this.CreateInstance(typeName, false);
		}

		public object CreateInstance(string typeName, bool ignoreCase)
		{
			Type type = this.GetType(typeName, false, ignoreCase);
			if (type == null)
			{
				return null;
			}
			object obj;
			try
			{
				obj = Activator.CreateInstance(type);
			}
			catch (InvalidOperationException)
			{
				throw new ArgumentException("It is illegal to invoke a method on a Type loaded via ReflectionOnly methods.");
			}
			return obj;
		}

		public object CreateInstance(string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes)
		{
			Type type = this.GetType(typeName, false, ignoreCase);
			if (type == null)
			{
				return null;
			}
			object obj;
			try
			{
				obj = Activator.CreateInstance(type, bindingAttr, binder, args, culture, activationAttributes);
			}
			catch (InvalidOperationException)
			{
				throw new ArgumentException("It is illegal to invoke a method on a Type loaded via ReflectionOnly methods.");
			}
			return obj;
		}

		public Module[] GetLoadedModules()
		{
			return this.GetLoadedModules(false);
		}

		public Module[] GetLoadedModules(bool getResourceModules)
		{
			return this.GetModules(getResourceModules);
		}

		public Module[] GetModules()
		{
			return this.GetModules(false);
		}

		public Module GetModule(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name.Length == 0)
			{
				throw new ArgumentException("Name can't be empty");
			}
			Module[] modules = this.GetModules(true);
			foreach (Module module in modules)
			{
				if (module.ScopeName == name)
				{
					return module;
				}
			}
			return null;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal virtual extern Module[] GetModulesInternal();

		public Module[] GetModules(bool getResourceModules)
		{
			Module[] modulesInternal = this.GetModulesInternal();
			if (!getResourceModules)
			{
				ArrayList arrayList = new ArrayList(modulesInternal.Length);
				foreach (Module module in modulesInternal)
				{
					if (!module.IsResource())
					{
						arrayList.Add(module);
					}
				}
				return (Module[])arrayList.ToArray(typeof(Module));
			}
			return modulesInternal;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern string[] GetNamespaces();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public virtual extern string[] GetManifestResourceNames();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Assembly GetExecutingAssembly();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Assembly GetCallingAssembly();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern AssemblyName[] GetReferencedAssemblies();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool GetManifestResourceInfoInternal(string name, ManifestResourceInfo info);

		public virtual ManifestResourceInfo GetManifestResourceInfo(string resourceName)
		{
			if (resourceName == null)
			{
				throw new ArgumentNullException("resourceName");
			}
			if (resourceName.Length == 0)
			{
				throw new ArgumentException("String cannot have zero length.");
			}
			ManifestResourceInfo manifestResourceInfo = new ManifestResourceInfo();
			bool manifestResourceInfoInternal = this.GetManifestResourceInfoInternal(resourceName, manifestResourceInfo);
			if (manifestResourceInfoInternal)
			{
				return manifestResourceInfo;
			}
			return null;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int MonoDebugger_GetMethodToken(MethodBase method);

		[ComVisible(false)]
		[MonoTODO("Always returns zero")]
		public long HostContext
		{
			get
			{
				return 0L;
			}
		}

		[ComVisible(false)]
		public Module ManifestModule
		{
			get
			{
				return this.GetManifestModule();
			}
		}

		internal virtual Module GetManifestModule()
		{
			return this.GetManifestModuleInternal();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern Module GetManifestModuleInternal();

		[ComVisible(false)]
		public virtual extern bool ReflectionOnly
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal void Resolve()
		{
			lock (this)
			{
				this.LoadAssemblyPermissions();
				Evidence evidence = new Evidence(this.UnprotectedGetEvidence());
				evidence.AddHost(new PermissionRequestEvidence(this._minimum, this._optional, this._refuse));
				this._granted = SecurityManager.ResolvePolicy(evidence, this._minimum, this._optional, this._refuse, out this._denied);
			}
		}

		internal PermissionSet GrantedPermissionSet
		{
			get
			{
				if (this._granted == null)
				{
					if (SecurityManager.ResolvingPolicyLevel != null)
					{
						if (SecurityManager.ResolvingPolicyLevel.IsFullTrustAssembly(this))
						{
							return DefaultPolicies.FullTrust;
						}
						return null;
					}
					else
					{
						this.Resolve();
					}
				}
				return this._granted;
			}
		}

		internal PermissionSet DeniedPermissionSet
		{
			get
			{
				if (this._granted == null)
				{
					if (SecurityManager.ResolvingPolicyLevel != null)
					{
						if (SecurityManager.ResolvingPolicyLevel.IsFullTrustAssembly(this))
						{
							return null;
						}
						return DefaultPolicies.FullTrust;
					}
					else
					{
						this.Resolve();
					}
				}
				return this._denied;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool LoadPermissions(Assembly a, ref IntPtr minimum, ref int minLength, ref IntPtr optional, ref int optLength, ref IntPtr refused, ref int refLength);

		private void LoadAssemblyPermissions()
		{
			IntPtr zero = IntPtr.Zero;
			IntPtr zero2 = IntPtr.Zero;
			IntPtr zero3 = IntPtr.Zero;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			if (Assembly.LoadPermissions(this, ref zero, ref num, ref zero2, ref num2, ref zero3, ref num3))
			{
				if (num > 0)
				{
					byte[] array = new byte[num];
					Marshal.Copy(zero, array, 0, num);
					this._minimum = SecurityManager.Decode(array);
				}
				if (num2 > 0)
				{
					byte[] array2 = new byte[num2];
					Marshal.Copy(zero2, array2, 0, num2);
					this._optional = SecurityManager.Decode(array2);
				}
				if (num3 > 0)
				{
					byte[] array3 = new byte[num3];
					Marshal.Copy(zero3, array3, 0, num3);
					this._refuse = SecurityManager.Decode(array3);
				}
			}
		}

		virtual Type System.Runtime.InteropServices._Assembly.GetType()
		{
			return base.GetType();
		}

		private IntPtr _mono_assembly;

		private Assembly.ResolveEventHolder resolve_event_holder;

		private Evidence _evidence;

		internal PermissionSet _minimum;

		internal PermissionSet _optional;

		internal PermissionSet _refuse;

		private PermissionSet _granted;

		private PermissionSet _denied;

		private bool fromByteArray;

		private string assemblyName;

		internal class ResolveEventHolder
		{
			public event ModuleResolveEventHandler ModuleResolve;
		}

		private class ResourceCloseHandler
		{
			public ResourceCloseHandler(Module module)
			{
				this.module = module;
			}

			public void OnClose(object sender, EventArgs e)
			{
				this.module = null;
			}

			private Module module;
		}
	}
}
