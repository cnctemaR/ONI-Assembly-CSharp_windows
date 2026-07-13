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
	[ComDefaultInterface(typeof(_Assembly))]
	[ClassInterface(ClassInterfaceType.None)]
	[ComVisible(true)]
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public abstract class Assembly : ICustomAttributeProvider, _Assembly, IEvidenceFactory, ISerializable
	{
		public virtual event ModuleResolveEventHandler ModuleResolve
		{
			[SecurityPermission(SecurityAction.LinkDemand, ControlAppDomain = true)]
			add
			{
				throw new NotImplementedException();
			}
			[SecurityPermission(SecurityAction.LinkDemand, ControlAppDomain = true)]
			remove
			{
				throw new NotImplementedException();
			}
		}

		public virtual string CodeBase
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual string EscapedCodeBase
		{
			[SecuritySafeCritical]
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual string FullName
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual MethodInfo EntryPoint
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual Evidence Evidence
		{
			[SecurityPermission(SecurityAction.Demand, ControlEvidence = true)]
			get
			{
				throw new NotImplementedException();
			}
		}

		internal virtual Evidence UnprotectedGetEvidence()
		{
			throw new NotImplementedException();
		}

		internal virtual IntPtr MonoAssembly
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		internal virtual bool FromByteArray
		{
			set
			{
				throw new NotImplementedException();
			}
		}

		public virtual string Location
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		[ComVisible(false)]
		public virtual string ImageRuntimeVersion
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		[SecurityCritical]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new NotImplementedException();
		}

		public virtual bool IsDefined(Type attributeType, bool inherit)
		{
			throw new NotImplementedException();
		}

		public virtual object[] GetCustomAttributes(bool inherit)
		{
			throw new NotImplementedException();
		}

		public virtual object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			throw new NotImplementedException();
		}

		public virtual FileStream[] GetFiles()
		{
			return this.GetFiles(false);
		}

		public virtual FileStream[] GetFiles(bool getResourceModules)
		{
			throw new NotImplementedException();
		}

		public virtual FileStream GetFile(string name)
		{
			throw new NotImplementedException();
		}

		public virtual Stream GetManifestResourceStream(string name)
		{
			throw new NotImplementedException();
		}

		public virtual Stream GetManifestResourceStream(Type type, string name)
		{
			throw new NotImplementedException();
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
			throw new NotImplementedException();
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
			return base.ToString();
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

		internal Assembly GetSatelliteAssembly(CultureInfo culture, Version version, bool throwOnError, ref StackCrawlMark stackMark)
		{
			if (culture == null)
			{
				throw new ArgumentNullException("culture");
			}
			string text = this.GetSimpleName() + ".resources";
			return this.InternalGetSatelliteAssembly(text, culture, version, true, ref stackMark);
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
				Assembly assembly = AppDomain.CurrentDomain.LoadSatellite(assemblyName, false, ref stackMark);
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
				runtimeAssembly = (RuntimeAssembly)Assembly.LoadFrom(text, false, ref stackMark);
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
		private static extern Assembly LoadFrom(string assemblyFile, bool refOnly, ref StackCrawlMark stackMark);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Assembly LoadFile_internal(string assemblyFile, ref StackCrawlMark stackMark);

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static Assembly LoadFrom(string assemblyFile)
		{
			StackCrawlMark stackCrawlMark = StackCrawlMark.LookForMyCaller;
			return Assembly.LoadFrom(assemblyFile, false, ref stackCrawlMark);
		}

		[Obsolete]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static Assembly LoadFrom(string assemblyFile, Evidence securityEvidence)
		{
			StackCrawlMark stackCrawlMark = StackCrawlMark.LookForMyCaller;
			Assembly assembly = Assembly.LoadFrom(assemblyFile, false, ref stackCrawlMark);
			if (assembly != null && securityEvidence != null)
			{
				assembly.Evidence.Merge(securityEvidence);
			}
			return assembly;
		}

		[Obsolete]
		[MonoTODO("This overload is not currently implemented")]
		public static Assembly LoadFrom(string assemblyFile, Evidence securityEvidence, byte[] hashValue, AssemblyHashAlgorithm hashAlgorithm)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public static Assembly LoadFrom(string assemblyFile, byte[] hashValue, AssemblyHashAlgorithm hashAlgorithm)
		{
			throw new NotImplementedException();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static Assembly UnsafeLoadFrom(string assemblyFile)
		{
			StackCrawlMark stackCrawlMark = StackCrawlMark.LookForMyCaller;
			return Assembly.LoadFrom(assemblyFile, false, ref stackCrawlMark);
		}

		[Obsolete]
		[MethodImpl(MethodImplOptions.NoInlining)]
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
			StackCrawlMark stackCrawlMark = StackCrawlMark.LookForMyCaller;
			Assembly assembly = Assembly.LoadFile_internal(path, ref stackCrawlMark);
			if (assembly != null && securityEvidence != null)
			{
				throw new NotImplementedException();
			}
			return assembly;
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

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static Assembly ReflectionOnlyLoad(string assemblyString)
		{
			StackCrawlMark stackCrawlMark = StackCrawlMark.LookForMyCaller;
			return AppDomain.CurrentDomain.Load(assemblyString, null, true, ref stackCrawlMark);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static Assembly ReflectionOnlyLoadFrom(string assemblyFile)
		{
			if (assemblyFile == null)
			{
				throw new ArgumentNullException("assemblyFile");
			}
			StackCrawlMark stackCrawlMark = StackCrawlMark.LookForMyCaller;
			return Assembly.LoadFrom(assemblyFile, true, ref stackCrawlMark);
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

		internal virtual Module[] GetModulesInternal()
		{
			throw new NotImplementedException();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Assembly GetExecutingAssembly();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Assembly GetCallingAssembly();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern IntPtr InternalGetReferencedAssemblies(Assembly module);

		public virtual string[] GetManifestResourceNames()
		{
			throw new NotImplementedException();
		}

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

		public virtual ManifestResourceInfo GetManifestResourceInfo(string resourceName)
		{
			throw new NotImplementedException();
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
			throw new NotImplementedException();
		}

		[ComVisible(false)]
		public virtual bool ReflectionOnly
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object o)
		{
			return base.Equals(o);
		}

		internal virtual PermissionSet GrantedPermissionSet
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		internal virtual PermissionSet DeniedPermissionSet
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual PermissionSet PermissionSet
		{
			get
			{
				throw new NotImplementedException();
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
			throw new NotImplementedException();
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

		public virtual Type[] GetForwardedTypes()
		{
			throw new PlatformNotSupportedException();
		}

		internal class ResolveEventHolder
		{
			public event ModuleResolveEventHandler ModuleResolve;
		}
	}
}
