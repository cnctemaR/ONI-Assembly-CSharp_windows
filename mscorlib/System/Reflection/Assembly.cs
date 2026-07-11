using System;
using System.Collections.Generic;
using System.Configuration.Assemblies;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;
using System.Threading;
using Mono;

namespace System.Reflection
{
	[ComVisible(true)]
	[ComDefaultInterface(typeof(_Assembly))]
	[ClassInterface(ClassInterfaceType.None)]
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public abstract class Assembly : ICustomAttributeProvider, _Assembly, IEvidenceFactory, ISerializable
	{
		protected Assembly()
		{
			this.resolve_event_holder = new Assembly.ResolveEventHolder();
		}

		public virtual event ModuleResolveEventHandler ModuleResolve
		{
			[SecurityPermission(SecurityAction.LinkDemand, ControlAppDomain = true)]
			add
			{
				this.resolve_event_holder.ModuleResolve += value;
			}
			[SecurityPermission(SecurityAction.LinkDemand, ControlAppDomain = true)]
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

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetAotId();

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
			[SecuritySafeCritical]
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
			[SecurityPermission(SecurityAction.Demand, ControlEvidence = true)]
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
		internal extern bool get_global_assembly_cache();

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

		[SecurityCritical]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new NotImplementedException();
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
				return EmptyArray<FileStream>.Value;
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
				Assembly assembly = AppDomain.CurrentDomain.DoResourceResolve(name, this);
				if (assembly != null && assembly != this)
				{
					return assembly.GetManifestResourceStream(name);
				}
				return null;
			}
			else
			{
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
					return new FileStream(Path.Combine(Path.GetDirectoryName(this.Location), manifestResourceInfo.FileName), FileMode.Open, FileAccess.Read);
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
					return new Assembly.UnmanagedMemoryStreamForModule((byte*)(void*)manifestResourceInternal, (long)num, module);
				}
			}
		}

		public virtual Stream GetManifestResourceStream(Type type, string name)
		{
			StackCrawlMark stackCrawlMark = StackCrawlMark.LookForMyCaller;
			return this.GetManifestResourceStream(type, name, false, ref stackCrawlMark);
		}

		internal Stream GetManifestResourceStream(Type type, string name, bool skipSecurityCheck, ref StackCrawlMark stackMark)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (type == null)
			{
				if (name == null)
				{
					throw new ArgumentNullException("type");
				}
			}
			else
			{
				string @namespace = type.Namespace;
				if (@namespace != null)
				{
					stringBuilder.Append(@namespace);
					if (name != null)
					{
						stringBuilder.Append(Type.Delimiter);
					}
				}
			}
			if (name != null)
			{
				stringBuilder.Append(name);
			}
			return this.GetManifestResourceStream(stringBuilder.ToString());
		}

		internal Stream GetManifestResourceStream(string name, ref StackCrawlMark stackMark, bool skipSecurityCheck)
		{
			return this.GetManifestResourceStream(null, name, skipSecurityCheck, ref stackMark);
		}

		internal string GetSimpleName()
		{
			return this.GetName(true).Name;
		}

		internal byte[] GetPublicKey()
		{
			return this.GetName(true).GetPublicKey();
		}

		internal Version GetVersion()
		{
			return this.GetName(true).Version;
		}

		private AssemblyNameFlags GetFlags()
		{
			return this.GetName(true).Flags;
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

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void InternalGetAssemblyName(string assemblyFile, out MonoAssemblyName aname, out string codebase);

		public virtual AssemblyName GetName(bool copiedName)
		{
			throw new NotImplementedException();
		}

		public virtual AssemblyName GetName()
		{
			return this.GetName(false);
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

		internal Assembly GetSatelliteAssembly(CultureInfo culture, Version version, bool throwOnError)
		{
			if (culture == null)
			{
				throw new ArgumentNullException("culture");
			}
			StackCrawlMark stackCrawlMark = StackCrawlMark.LookForMyCaller;
			string text = this.GetSimpleName() + ".resources";
			return this.InternalGetSatelliteAssembly(text, culture, version, true, ref stackCrawlMark);
		}

		internal RuntimeAssembly InternalGetSatelliteAssembly(string name, CultureInfo culture, Version version, bool throwOnFileNotFound, ref StackCrawlMark stackMark)
		{
			AssemblyName assemblyName = new AssemblyName();
			assemblyName.SetPublicKey(this.GetPublicKey());
			assemblyName.Flags = this.GetFlags() | AssemblyNameFlags.PublicKey;
			if (version == null)
			{
				assemblyName.Version = this.GetVersion();
			}
			else
			{
				assemblyName.Version = version;
			}
			assemblyName.CultureInfo = culture;
			assemblyName.Name = name;
			try
			{
				Assembly assembly = AppDomain.CurrentDomain.LoadSatellite(assemblyName, false);
				if (assembly != null)
				{
					return (RuntimeAssembly)assembly;
				}
			}
			catch (FileNotFoundException)
			{
			}
			if (string.IsNullOrEmpty(this.Location))
			{
				return null;
			}
			string text = Path.Combine(Path.GetDirectoryName(this.Location), Path.Combine(culture.Name, assemblyName.Name + ".dll"));
			RuntimeAssembly runtimeAssembly;
			try
			{
				runtimeAssembly = (RuntimeAssembly)Assembly.LoadFrom(text);
			}
			catch
			{
				if (throwOnFileNotFound || File.Exists(text))
				{
					throw;
				}
				runtimeAssembly = null;
			}
			return runtimeAssembly;
		}

		Type _Assembly.GetType()
		{
			return base.GetType();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Assembly LoadFrom(string assemblyFile, bool refonly);

		public static Assembly LoadFrom(string assemblyFile)
		{
			return Assembly.LoadFrom(assemblyFile, false);
		}

		[Obsolete]
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
		[Obsolete]
		public static Assembly LoadFrom(string assemblyFile, Evidence securityEvidence, byte[] hashValue, AssemblyHashAlgorithm hashAlgorithm)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public static Assembly LoadFrom(string assemblyFile, byte[] hashValue, AssemblyHashAlgorithm hashAlgorithm)
		{
			throw new NotImplementedException();
		}

		public static Assembly UnsafeLoadFrom(string assemblyFile)
		{
			return Assembly.LoadFrom(assemblyFile);
		}

		[Obsolete]
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

		[Obsolete]
		public static Assembly Load(string assemblyString, Evidence assemblySecurity)
		{
			return AppDomain.CurrentDomain.Load(assemblyString, assemblySecurity);
		}

		public static Assembly Load(AssemblyName assemblyRef)
		{
			return AppDomain.CurrentDomain.Load(assemblyRef);
		}

		[Obsolete]
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

		[Obsolete]
		public static Assembly Load(byte[] rawAssembly, byte[] rawSymbolStore, Evidence securityEvidence)
		{
			return AppDomain.CurrentDomain.Load(rawAssembly, rawSymbolStore, securityEvidence);
		}

		[MonoLimitation("Argument securityContextSource is ignored")]
		public static Assembly Load(byte[] rawAssembly, byte[] rawSymbolStore, SecurityContextSource securityContextSource)
		{
			return AppDomain.CurrentDomain.Load(rawAssembly, rawSymbolStore);
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

		[Obsolete("This method has been deprecated. Please use Assembly.Load() instead. http://go.microsoft.com/fwlink/?linkid=14202")]
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
		public virtual Module LoadModule(string moduleName, byte[] rawModule, byte[] rawSymbolStore)
		{
			throw new NotImplementedException();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Assembly load_with_partial_name(string name, Evidence e);

		[Obsolete("This method has been deprecated. Please use Assembly.Load() instead. http://go.microsoft.com/fwlink/?linkid=14202")]
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

		public virtual object CreateInstance(string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes)
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

		public Module[] GetModules()
		{
			return this.GetModules(false);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal virtual extern Module[] GetModulesInternal();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public virtual extern string[] GetManifestResourceNames();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Assembly GetExecutingAssembly();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Assembly GetCallingAssembly();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern IntPtr InternalGetReferencedAssemblies(Assembly module);

		internal unsafe static AssemblyName[] GetReferencedAssemblies(Assembly module)
		{
			AssemblyName[] array2;
			using (SafeGPtrArrayHandle safeGPtrArrayHandle = new SafeGPtrArrayHandle(Assembly.InternalGetReferencedAssemblies(module)))
			{
				int length = safeGPtrArrayHandle.Length;
				try
				{
					AssemblyName[] array = new AssemblyName[length];
					for (int i = 0; i < length; i++)
					{
						AssemblyName assemblyName = new AssemblyName();
						MonoAssemblyName* ptr = (MonoAssemblyName*)(void*)safeGPtrArrayHandle[i];
						assemblyName.FillName(ptr, null, true, false, true, true);
						array[i] = assemblyName;
					}
					array2 = array;
				}
				finally
				{
					for (int j = 0; j < length; j++)
					{
						MonoAssemblyName* ptr2 = (MonoAssemblyName*)(void*)safeGPtrArrayHandle[j];
						RuntimeMarshal.FreeAssemblyName(ref *ptr2, true);
					}
				}
			}
			return array2;
		}

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
			ManifestResourceInfo manifestResourceInfo = new ManifestResourceInfo(null, null, (ResourceLocation)0);
			if (this.GetManifestResourceInfoInternal(resourceName, manifestResourceInfo))
			{
				return manifestResourceInfo;
			}
			return null;
		}

		[MonoTODO("Currently it always returns zero")]
		[ComVisible(false)]
		public virtual long HostContext
		{
			get
			{
				return 0L;
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

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object o)
		{
			return this == o || (o != null && ((Assembly)o)._mono_assembly == this._mono_assembly);
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

		public virtual PermissionSet PermissionSet
		{
			get
			{
				return this.GrantedPermissionSet;
			}
		}

		public virtual SecurityRuleSet SecurityRuleSet
		{
			get
			{
				throw Assembly.CreateNIE();
			}
		}

		private static Exception CreateNIE()
		{
			return new NotImplementedException("Derived classes must implement it");
		}

		public virtual IList<CustomAttributeData> GetCustomAttributesData()
		{
			return CustomAttributeData.GetCustomAttributes(this);
		}

		[MonoTODO]
		public bool IsFullyTrusted
		{
			get
			{
				return true;
			}
		}

		public virtual Type GetType(string name, bool throwOnError, bool ignoreCase)
		{
			throw Assembly.CreateNIE();
		}

		public virtual Module GetModule(string name)
		{
			throw Assembly.CreateNIE();
		}

		public virtual AssemblyName[] GetReferencedAssemblies()
		{
			throw Assembly.CreateNIE();
		}

		public virtual Module[] GetModules(bool getResourceModules)
		{
			throw Assembly.CreateNIE();
		}

		[MonoTODO("Always returns the same as GetModules")]
		public virtual Module[] GetLoadedModules(bool getResourceModules)
		{
			throw Assembly.CreateNIE();
		}

		public virtual Assembly GetSatelliteAssembly(CultureInfo culture)
		{
			throw Assembly.CreateNIE();
		}

		public virtual Assembly GetSatelliteAssembly(CultureInfo culture, Version version)
		{
			throw Assembly.CreateNIE();
		}

		public virtual Module ManifestModule
		{
			get
			{
				throw Assembly.CreateNIE();
			}
		}

		public virtual bool GlobalAssemblyCache
		{
			get
			{
				throw Assembly.CreateNIE();
			}
		}

		public virtual bool IsDynamic
		{
			get
			{
				return false;
			}
		}

		public static bool operator ==(Assembly left, Assembly right)
		{
			return left == right || (!((left == null) ^ (right == null)) && left.Equals(right));
		}

		public static bool operator !=(Assembly left, Assembly right)
		{
			return left != right && (((left == null) ^ (right == null)) || !left.Equals(right));
		}

		public virtual IEnumerable<TypeInfo> DefinedTypes
		{
			get
			{
				foreach (Type type in this.GetTypes())
				{
					yield return type.GetTypeInfo();
				}
				Type[] array = null;
				yield break;
			}
		}

		public virtual IEnumerable<Type> ExportedTypes
		{
			get
			{
				return this.GetExportedTypes();
			}
		}

		public virtual IEnumerable<Module> Modules
		{
			get
			{
				return this.GetModules();
			}
		}

		public virtual IEnumerable<CustomAttributeData> CustomAttributes
		{
			get
			{
				return this.GetCustomAttributesData();
			}
		}

		internal IntPtr _mono_assembly;

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

		internal class UnmanagedMemoryStreamForModule : UnmanagedMemoryStream
		{
			public unsafe UnmanagedMemoryStreamForModule(byte* pointer, long length, Module module)
				: base(pointer, length)
			{
				this.module = module;
			}

			protected override void Dispose(bool disposing)
			{
				if (this._isOpen)
				{
					this.module = null;
				}
				base.Dispose(disposing);
			}

			private Module module;
		}
	}
}
