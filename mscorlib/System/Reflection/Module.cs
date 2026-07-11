using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;

namespace System.Reflection
{
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.None)]
	[ComDefaultInterface(typeof(_Module))]
	[Serializable]
	public class Module : ISerializable, ICustomAttributeProvider, _Module
	{
		internal Module()
		{
		}

		void _Module.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
		{
			throw new NotImplementedException();
		}

		void _Module.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
		{
			throw new NotImplementedException();
		}

		void _Module.GetTypeInfoCount(out uint pcTInfo)
		{
			throw new NotImplementedException();
		}

		void _Module.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		{
			throw new NotImplementedException();
		}

		public Assembly Assembly
		{
			get
			{
				return this.assembly;
			}
		}

		public virtual string FullyQualifiedName
		{
			get
			{
				if (SecurityManager.SecurityEnabled)
				{
					new FileIOPermission(FileIOPermissionAccess.PathDiscovery, this.fqname).Demand();
				}
				return this.fqname;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string ScopeName
		{
			get
			{
				return this.scopename;
			}
		}

		public ModuleHandle ModuleHandle
		{
			get
			{
				return new ModuleHandle(this._impl);
			}
		}

		public extern int MetadataToken
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public int MDStreamVersion
		{
			get
			{
				if (this._impl == IntPtr.Zero)
				{
					throw new NotSupportedException();
				}
				return Module.GetMDStreamVersion(this._impl);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetMDStreamVersion(IntPtr module_handle);

		public virtual Type[] FindTypes(TypeFilter filter, object filterCriteria)
		{
			ArrayList arrayList = new ArrayList();
			Type[] types = this.GetTypes();
			foreach (Type type in types)
			{
				if (filter(type, filterCriteria))
				{
					arrayList.Add(type);
				}
			}
			return (Type[])arrayList.ToArray(typeof(Type));
		}

		public virtual object[] GetCustomAttributes(bool inherit)
		{
			return MonoCustomAttrs.GetCustomAttributes(this, inherit);
		}

		public virtual object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			return MonoCustomAttrs.GetCustomAttributes(this, attributeType, inherit);
		}

		public FieldInfo GetField(string name)
		{
			if (this.IsResource())
			{
				return null;
			}
			Type globalType = this.GetGlobalType();
			return (globalType == null) ? null : globalType.GetField(name, BindingFlags.Static | BindingFlags.Public);
		}

		public FieldInfo GetField(string name, BindingFlags bindingAttr)
		{
			if (this.IsResource())
			{
				return null;
			}
			Type globalType = this.GetGlobalType();
			return (globalType == null) ? null : globalType.GetField(name, bindingAttr);
		}

		public FieldInfo[] GetFields()
		{
			if (this.IsResource())
			{
				return new FieldInfo[0];
			}
			Type globalType = this.GetGlobalType();
			return (globalType == null) ? new FieldInfo[0] : globalType.GetFields(BindingFlags.Static | BindingFlags.Public);
		}

		public MethodInfo GetMethod(string name)
		{
			if (this.IsResource())
			{
				return null;
			}
			Type globalType = this.GetGlobalType();
			return (globalType == null) ? null : globalType.GetMethod(name);
		}

		public MethodInfo GetMethod(string name, Type[] types)
		{
			return this.GetMethodImpl(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, CallingConventions.Any, types, null);
		}

		public MethodInfo GetMethod(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			return this.GetMethodImpl(name, bindingAttr, binder, callConvention, types, modifiers);
		}

		protected virtual MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			if (this.IsResource())
			{
				return null;
			}
			Type globalType = this.GetGlobalType();
			return (globalType == null) ? null : globalType.GetMethod(name, bindingAttr, binder, callConvention, types, modifiers);
		}

		public MethodInfo[] GetMethods()
		{
			if (this.IsResource())
			{
				return new MethodInfo[0];
			}
			Type globalType = this.GetGlobalType();
			return (globalType == null) ? new MethodInfo[0] : globalType.GetMethods();
		}

		public MethodInfo[] GetMethods(BindingFlags bindingFlags)
		{
			if (this.IsResource())
			{
				return new MethodInfo[0];
			}
			Type globalType = this.GetGlobalType();
			return (globalType == null) ? new MethodInfo[0] : globalType.GetMethods(bindingFlags);
		}

		public FieldInfo[] GetFields(BindingFlags bindingFlags)
		{
			if (this.IsResource())
			{
				return new FieldInfo[0];
			}
			Type globalType = this.GetGlobalType();
			return (globalType == null) ? new FieldInfo[0] : globalType.GetFields(bindingFlags);
		}

		[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"SerializationFormatter\"/>\n</PermissionSet>\n")]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			UnitySerializationHolder.GetModuleData(this, info, context);
		}

		public X509Certificate GetSignerCertificate()
		{
			X509Certificate x509Certificate;
			try
			{
				x509Certificate = X509Certificate.CreateFromSignedFile(this.assembly.Location);
			}
			catch
			{
				x509Certificate = null;
			}
			return x509Certificate;
		}

		[ComVisible(true)]
		public virtual Type GetType(string className)
		{
			return this.GetType(className, false, false);
		}

		[ComVisible(true)]
		public virtual Type GetType(string className, bool ignoreCase)
		{
			return this.GetType(className, false, ignoreCase);
		}

		[ComVisible(true)]
		public virtual Type GetType(string className, bool throwOnError, bool ignoreCase)
		{
			if (className == null)
			{
				throw new ArgumentNullException("className");
			}
			if (className == string.Empty)
			{
				throw new ArgumentException("Type name can't be empty");
			}
			return this.assembly.InternalGetType(this, className, throwOnError, ignoreCase);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Type[] InternalGetTypes();

		public virtual Type[] GetTypes()
		{
			return this.InternalGetTypes();
		}

		public virtual bool IsDefined(Type attributeType, bool inherit)
		{
			return MonoCustomAttrs.IsDefined(this, attributeType, inherit);
		}

		public bool IsResource()
		{
			return this.is_resource;
		}

		public override string ToString()
		{
			return this.name;
		}

		internal Guid MvId
		{
			get
			{
				return this.GetModuleVersionId();
			}
		}

		public Guid ModuleVersionId
		{
			get
			{
				return this.GetModuleVersionId();
			}
		}

		public void GetPEKind(out PortableExecutableKinds peKind, out ImageFileMachine machine)
		{
			this.ModuleHandle.GetPEKind(out peKind, out machine);
		}

		private Exception resolve_token_exception(int metadataToken, ResolveTokenError error, string tokenType)
		{
			if (error == ResolveTokenError.OutOfRange)
			{
				return new ArgumentOutOfRangeException("metadataToken", string.Format("Token 0x{0:x} is not valid in the scope of module {1}", metadataToken, this.name));
			}
			return new ArgumentException(string.Format("Token 0x{0:x} is not a valid {1} token in the scope of module {2}", metadataToken, tokenType, this.name), "metadataToken");
		}

		private IntPtr[] ptrs_from_types(Type[] types)
		{
			if (types == null)
			{
				return null;
			}
			IntPtr[] array = new IntPtr[types.Length];
			for (int i = 0; i < types.Length; i++)
			{
				if (types[i] == null)
				{
					throw new ArgumentException();
				}
				array[i] = types[i].TypeHandle.Value;
			}
			return array;
		}

		public FieldInfo ResolveField(int metadataToken)
		{
			return this.ResolveField(metadataToken, null, null);
		}

		public FieldInfo ResolveField(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			ResolveTokenError resolveTokenError;
			IntPtr intPtr = Module.ResolveFieldToken(this._impl, metadataToken, this.ptrs_from_types(genericTypeArguments), this.ptrs_from_types(genericMethodArguments), out resolveTokenError);
			if (intPtr == IntPtr.Zero)
			{
				throw this.resolve_token_exception(metadataToken, resolveTokenError, "Field");
			}
			return FieldInfo.GetFieldFromHandle(new RuntimeFieldHandle(intPtr));
		}

		public MemberInfo ResolveMember(int metadataToken)
		{
			return this.ResolveMember(metadataToken, null, null);
		}

		public MemberInfo ResolveMember(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			ResolveTokenError resolveTokenError;
			MemberInfo memberInfo = Module.ResolveMemberToken(this._impl, metadataToken, this.ptrs_from_types(genericTypeArguments), this.ptrs_from_types(genericMethodArguments), out resolveTokenError);
			if (memberInfo == null)
			{
				throw this.resolve_token_exception(metadataToken, resolveTokenError, "MemberInfo");
			}
			return memberInfo;
		}

		public MethodBase ResolveMethod(int metadataToken)
		{
			return this.ResolveMethod(metadataToken, null, null);
		}

		public MethodBase ResolveMethod(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			ResolveTokenError resolveTokenError;
			IntPtr intPtr = Module.ResolveMethodToken(this._impl, metadataToken, this.ptrs_from_types(genericTypeArguments), this.ptrs_from_types(genericMethodArguments), out resolveTokenError);
			if (intPtr == IntPtr.Zero)
			{
				throw this.resolve_token_exception(metadataToken, resolveTokenError, "MethodBase");
			}
			return MethodBase.GetMethodFromHandleNoGenericCheck(new RuntimeMethodHandle(intPtr));
		}

		public string ResolveString(int metadataToken)
		{
			ResolveTokenError resolveTokenError;
			string text = Module.ResolveStringToken(this._impl, metadataToken, out resolveTokenError);
			if (text == null)
			{
				throw this.resolve_token_exception(metadataToken, resolveTokenError, "string");
			}
			return text;
		}

		public Type ResolveType(int metadataToken)
		{
			return this.ResolveType(metadataToken, null, null);
		}

		public Type ResolveType(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			ResolveTokenError resolveTokenError;
			IntPtr intPtr = Module.ResolveTypeToken(this._impl, metadataToken, this.ptrs_from_types(genericTypeArguments), this.ptrs_from_types(genericMethodArguments), out resolveTokenError);
			if (intPtr == IntPtr.Zero)
			{
				throw this.resolve_token_exception(metadataToken, resolveTokenError, "Type");
			}
			return Type.GetTypeFromHandle(new RuntimeTypeHandle(intPtr));
		}

		public byte[] ResolveSignature(int metadataToken)
		{
			ResolveTokenError resolveTokenError;
			byte[] array = Module.ResolveSignature(this._impl, metadataToken, out resolveTokenError);
			if (array == null)
			{
				throw this.resolve_token_exception(metadataToken, resolveTokenError, "signature");
			}
			return array;
		}

		internal static Type MonoDebugger_ResolveType(Module module, int token)
		{
			ResolveTokenError resolveTokenError;
			IntPtr intPtr = Module.ResolveTypeToken(module._impl, token, null, null, out resolveTokenError);
			if (intPtr == IntPtr.Zero)
			{
				return null;
			}
			return Type.GetTypeFromHandle(new RuntimeTypeHandle(intPtr));
		}

		internal static Guid Mono_GetGuid(Module module)
		{
			return module.GetModuleVersionId();
		}

		internal virtual Guid GetModuleVersionId()
		{
			return new Guid(this.GetGuidInternal());
		}

		private static bool filter_by_type_name(Type m, object filterCriteria)
		{
			string text = (string)filterCriteria;
			if (text.EndsWith("*"))
			{
				return m.Name.StartsWith(text.Substring(0, text.Length - 1));
			}
			return m.Name == text;
		}

		private static bool filter_by_type_name_ignore_case(Type m, object filterCriteria)
		{
			string text = (string)filterCriteria;
			if (text.EndsWith("*"))
			{
				return m.Name.ToLower().StartsWith(text.Substring(0, text.Length - 1).ToLower());
			}
			return string.Compare(m.Name, text, true) == 0;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern IntPtr GetHINSTANCE();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern string GetGuidInternal();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Type GetGlobalType();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern IntPtr ResolveTypeToken(IntPtr module, int token, IntPtr[] type_args, IntPtr[] method_args, out ResolveTokenError error);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern IntPtr ResolveMethodToken(IntPtr module, int token, IntPtr[] type_args, IntPtr[] method_args, out ResolveTokenError error);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern IntPtr ResolveFieldToken(IntPtr module, int token, IntPtr[] type_args, IntPtr[] method_args, out ResolveTokenError error);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string ResolveStringToken(IntPtr module, int token, out ResolveTokenError error);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern MemberInfo ResolveMemberToken(IntPtr module, int token, IntPtr[] type_args, IntPtr[] method_args, out ResolveTokenError error);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern byte[] ResolveSignature(IntPtr module, int metadataToken, out ResolveTokenError error);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void GetPEKind(IntPtr module, out PortableExecutableKinds peKind, out ImageFileMachine machine);

		private const BindingFlags defaultBindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;

		public static readonly TypeFilter FilterTypeName = new TypeFilter(Module.filter_by_type_name);

		public static readonly TypeFilter FilterTypeNameIgnoreCase = new TypeFilter(Module.filter_by_type_name_ignore_case);

		private IntPtr _impl;

		internal Assembly assembly;

		internal string fqname;

		internal string name;

		internal string scopename;

		internal bool is_resource;

		internal int token;
	}
}
