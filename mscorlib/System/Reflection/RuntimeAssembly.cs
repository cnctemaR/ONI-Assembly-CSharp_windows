using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Threading;

namespace System.Reflection
{
	[ComDefaultInterface(typeof(_Assembly))]
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.None)]
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	internal class RuntimeAssembly : Assembly
	{
		protected RuntimeAssembly()
		{
			this.resolve_event_holder = new Assembly.ResolveEventHolder();
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			UnitySerializationHolder.GetUnitySerializationInfo(info, 6, this.FullName, this);
		}

		internal static RuntimeAssembly GetExecutingAssembly(ref StackCrawlMark stackMark)
		{
			throw new NotSupportedException();
		}

		[SecurityCritical]
		internal static AssemblyName CreateAssemblyName(string assemblyString, bool forIntrospection, out RuntimeAssembly assemblyFromResolveEvent)
		{
			if (assemblyString == null)
			{
				throw new ArgumentNullException("assemblyString");
			}
			if (assemblyString.Length == 0 || assemblyString[0] == '\0')
			{
				throw new ArgumentException(Environment.GetResourceString("String cannot have zero length."));
			}
			if (forIntrospection)
			{
				AppDomain.CheckReflectionOnlyLoadSupported();
			}
			AssemblyName assemblyName = new AssemblyName();
			assemblyName.Name = assemblyString;
			assemblyFromResolveEvent = null;
			return assemblyName;
		}

		internal static RuntimeAssembly InternalLoadAssemblyName(AssemblyName assemblyRef, Evidence assemblySecurity, RuntimeAssembly reqAssembly, ref StackCrawlMark stackMark, bool throwOnFileNotFound, bool forIntrospection, bool suppressSecurityChecks)
		{
			if (assemblyRef == null)
			{
				throw new ArgumentNullException("assemblyRef");
			}
			if (assemblyRef.CodeBase != null)
			{
				AppDomain.CheckLoadFromSupported();
			}
			assemblyRef = (AssemblyName)assemblyRef.Clone();
			if (assemblySecurity != null)
			{
			}
			return (RuntimeAssembly)Assembly.Load(assemblyRef);
		}

		internal static RuntimeAssembly LoadWithPartialNameInternal(string partialName, Evidence securityEvidence, ref StackCrawlMark stackMark)
		{
			return (RuntimeAssembly)Assembly.LoadWithPartialName(partialName, securityEvidence);
		}

		internal static RuntimeAssembly LoadWithPartialNameInternal(AssemblyName an, Evidence securityEvidence, ref StackCrawlMark stackMark)
		{
			return RuntimeAssembly.LoadWithPartialNameInternal(an.ToString(), securityEvidence, ref stackMark);
		}

		public override AssemblyName GetName(bool copiedName)
		{
			if (SecurityManager.SecurityEnabled)
			{
				string codeBase = this.CodeBase;
			}
			return AssemblyName.Create(this, true);
		}

		public override Type GetType(string name, bool throwOnError, bool ignoreCase)
		{
			if (name == null)
			{
				throw new ArgumentNullException(name);
			}
			if (name.Length == 0)
			{
				throw new ArgumentException("name", "Name cannot be empty");
			}
			return base.InternalGetType(null, name, throwOnError, ignoreCase);
		}

		public override Module GetModule(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name.Length == 0)
			{
				throw new ArgumentException("Name can't be empty");
			}
			foreach (Module module in this.GetModules(true))
			{
				if (module.ScopeName == name)
				{
					return module;
				}
			}
			return null;
		}

		public override AssemblyName[] GetReferencedAssemblies()
		{
			return Assembly.GetReferencedAssemblies(this);
		}

		public override Module[] GetModules(bool getResourceModules)
		{
			Module[] modulesInternal = this.GetModulesInternal();
			if (!getResourceModules)
			{
				List<Module> list = new List<Module>(modulesInternal.Length);
				foreach (Module module in modulesInternal)
				{
					if (!module.IsResource())
					{
						list.Add(module);
					}
				}
				return list.ToArray();
			}
			return modulesInternal;
		}

		[MonoTODO("Always returns the same as GetModules")]
		public override Module[] GetLoadedModules(bool getResourceModules)
		{
			return this.GetModules(getResourceModules);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override Assembly GetSatelliteAssembly(CultureInfo culture)
		{
			StackCrawlMark stackCrawlMark = StackCrawlMark.LookForMyCaller;
			return base.GetSatelliteAssembly(culture, null, true, ref stackCrawlMark);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override Assembly GetSatelliteAssembly(CultureInfo culture, Version version)
		{
			StackCrawlMark stackCrawlMark = StackCrawlMark.LookForMyCaller;
			return base.GetSatelliteAssembly(culture, version, true, ref stackCrawlMark);
		}

		[ComVisible(false)]
		public override Module ManifestModule
		{
			get
			{
				return this.GetManifestModule();
			}
		}

		public override bool GlobalAssemblyCache
		{
			get
			{
				return this.get_global_assembly_cache();
			}
		}

		public override Type[] GetExportedTypes()
		{
			return this.GetTypes(true);
		}

		internal static byte[] GetAotId()
		{
			byte[] array = new byte[16];
			if (RuntimeAssembly.GetAotIdInternal(array))
			{
				return array;
			}
			return null;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string get_code_base(Assembly a, bool escaped);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern string get_location();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string get_fullname(Assembly a);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool GetAotIdInternal(byte[] aotid);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string InternalImageRuntimeVersion(Assembly a);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern bool get_global_assembly_cache();

		public override extern MethodInfo EntryPoint
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[ComVisible(false)]
		public override extern bool ReflectionOnly
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal static string GetCodeBase(Assembly a, bool escaped)
		{
			string text = RuntimeAssembly.get_code_base(a, escaped);
			if (SecurityManager.SecurityEnabled && string.Compare("FILE://", 0, text, 0, 7, true, CultureInfo.InvariantCulture) == 0)
			{
				string text2 = text.Substring(7);
				new FileIOPermission(FileIOPermissionAccess.PathDiscovery, text2).Demand();
			}
			return text;
		}

		public override string CodeBase
		{
			get
			{
				return RuntimeAssembly.GetCodeBase(this, false);
			}
		}

		public override string EscapedCodeBase
		{
			get
			{
				return RuntimeAssembly.GetCodeBase(this, true);
			}
		}

		public override string FullName
		{
			get
			{
				return RuntimeAssembly.get_fullname(this);
			}
		}

		[ComVisible(false)]
		public override string ImageRuntimeVersion
		{
			get
			{
				return RuntimeAssembly.InternalImageRuntimeVersion(this);
			}
		}

		internal override IntPtr MonoAssembly
		{
			get
			{
				return this._mono_assembly;
			}
		}

		internal override bool FromByteArray
		{
			set
			{
				this.fromByteArray = value;
			}
		}

		public override string Location
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

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool GetManifestResourceInfoInternal(string name, ManifestResourceInfo info);

		public override ManifestResourceInfo GetManifestResourceInfo(string resourceName)
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

		[MethodImpl(MethodImplOptions.InternalCall)]
		public override extern string[] GetManifestResourceNames();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern IntPtr GetManifestResourceInternal(string name, out int size, out Module module);

		public unsafe override Stream GetManifestResourceStream(string name)
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
					return new RuntimeAssembly.UnmanagedMemoryStreamForModule((byte*)(void*)manifestResourceInternal, (long)num, module);
				}
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public override Stream GetManifestResourceStream(Type type, string name)
		{
			StackCrawlMark stackCrawlMark = StackCrawlMark.LookForMyCaller;
			return base.GetManifestResourceStream(type, name, false, ref stackCrawlMark);
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			return MonoCustomAttrs.IsDefined(this, attributeType, inherit);
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			return MonoCustomAttrs.GetCustomAttributes(this, inherit);
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			return MonoCustomAttrs.GetCustomAttributes(this, attributeType, inherit);
		}

		public override IList<CustomAttributeData> GetCustomAttributesData()
		{
			return CustomAttributeData.GetCustomAttributes(this);
		}

		public override event ModuleResolveEventHandler ModuleResolve
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

		internal override Module GetManifestModule()
		{
			return this.GetManifestModuleInternal();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern Module GetManifestModuleInternal();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal override extern Module[] GetModulesInternal();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern object GetFilesInternal(string name, bool getResourceModules);

		public override FileStream[] GetFiles(bool getResourceModules)
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

		public override FileStream GetFile(string name)
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

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object o)
		{
			return this == o || (o != null && o is RuntimeAssembly && ((RuntimeAssembly)o)._mono_assembly == this._mono_assembly);
		}

		public override string ToString()
		{
			if (this.assemblyName != null)
			{
				return this.assemblyName;
			}
			this.assemblyName = this.FullName;
			return this.assemblyName;
		}

		public override Evidence Evidence
		{
			[SecurityPermission(SecurityAction.Demand, ControlEvidence = true)]
			get
			{
				return this.UnprotectedGetEvidence();
			}
		}

		internal override Evidence UnprotectedGetEvidence()
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

		internal override PermissionSet GrantedPermissionSet
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

		internal override PermissionSet DeniedPermissionSet
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
			if (RuntimeAssembly.LoadPermissions(this, ref zero, ref num, ref zero2, ref num2, ref zero3, ref num3))
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

		public override PermissionSet PermissionSet
		{
			get
			{
				return this.GrantedPermissionSet;
			}
		}

		internal IntPtr _mono_assembly;

		internal Evidence _evidence;

		internal Assembly.ResolveEventHolder resolve_event_holder;

		internal PermissionSet _minimum;

		internal PermissionSet _optional;

		internal PermissionSet _refuse;

		internal PermissionSet _granted;

		internal PermissionSet _denied;

		internal bool fromByteArray;

		internal string assemblyName;

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
