using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;

namespace System.Reflection
{
	[ClassInterface(ClassInterfaceType.None)]
	[ComDefaultInterface(typeof(_Module))]
	[ComVisible(true)]
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	internal class RuntimeModule : Module
	{
		public override Assembly Assembly
		{
			get
			{
				return this.assembly;
			}
		}

		public override string Name
		{
			get
			{
				return this.name;
			}
		}

		public override string ScopeName
		{
			get
			{
				return this.scopename;
			}
		}

		public override int MDStreamVersion
		{
			get
			{
				if (this._impl == IntPtr.Zero)
				{
					throw new NotSupportedException();
				}
				return RuntimeModule.GetMDStreamVersion(this._impl);
			}
		}

		public override Guid ModuleVersionId
		{
			get
			{
				return this.GetModuleVersionId();
			}
		}

		public override string FullyQualifiedName
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

		public override bool IsResource()
		{
			return this.is_resource;
		}

		public override Type[] FindTypes(TypeFilter filter, object filterCriteria)
		{
			List<Type> list = new List<Type>();
			foreach (Type type in this.GetTypes())
			{
				if (filter(type, filterCriteria))
				{
					list.Add(type);
				}
			}
			return list.ToArray();
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			return MonoCustomAttrs.GetCustomAttributes(this, inherit);
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			return MonoCustomAttrs.GetCustomAttributes(this, attributeType, inherit);
		}

		public override FieldInfo GetField(string name, BindingFlags bindingAttr)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (this.IsResource())
			{
				return null;
			}
			Type globalType = RuntimeModule.GetGlobalType(this._impl);
			if (!(globalType != null))
			{
				return null;
			}
			return globalType.GetField(name, bindingAttr);
		}

		public override FieldInfo[] GetFields(BindingFlags bindingFlags)
		{
			if (this.IsResource())
			{
				return new FieldInfo[0];
			}
			Type globalType = RuntimeModule.GetGlobalType(this._impl);
			if (!(globalType != null))
			{
				return new FieldInfo[0];
			}
			return globalType.GetFields(bindingFlags);
		}

		public override int MetadataToken
		{
			get
			{
				return RuntimeModule.get_MetadataToken(this);
			}
		}

		protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			if (this.IsResource())
			{
				return null;
			}
			Type globalType = RuntimeModule.GetGlobalType(this._impl);
			if (globalType == null)
			{
				return null;
			}
			if (types == null)
			{
				return globalType.GetMethod(name);
			}
			return globalType.GetMethod(name, bindingAttr, binder, callConvention, types, modifiers);
		}

		public override MethodInfo[] GetMethods(BindingFlags bindingFlags)
		{
			if (this.IsResource())
			{
				return new MethodInfo[0];
			}
			Type globalType = RuntimeModule.GetGlobalType(this._impl);
			if (!(globalType != null))
			{
				return new MethodInfo[0];
			}
			return globalType.GetMethods(bindingFlags);
		}

		internal override ModuleHandle GetModuleHandleImpl()
		{
			return new ModuleHandle(this._impl);
		}

		public override void GetPEKind(out PortableExecutableKinds peKind, out ImageFileMachine machine)
		{
			RuntimeModule.GetPEKind(this._impl, out peKind, out machine);
		}

		public override Type GetType(string className, bool throwOnError, bool ignoreCase)
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

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			return MonoCustomAttrs.IsDefined(this, attributeType, inherit);
		}

		public override FieldInfo ResolveField(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			return RuntimeModule.ResolveField(this, this._impl, metadataToken, genericTypeArguments, genericMethodArguments);
		}

		internal static FieldInfo ResolveField(Module module, IntPtr monoModule, int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			ResolveTokenError resolveTokenError;
			IntPtr intPtr = RuntimeModule.ResolveFieldToken(monoModule, metadataToken, RuntimeModule.ptrs_from_types(genericTypeArguments), RuntimeModule.ptrs_from_types(genericMethodArguments), out resolveTokenError);
			if (intPtr == IntPtr.Zero)
			{
				throw RuntimeModule.resolve_token_exception(module.Name, metadataToken, resolveTokenError, "Field");
			}
			return FieldInfo.GetFieldFromHandle(new RuntimeFieldHandle(intPtr));
		}

		public override MemberInfo ResolveMember(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			return RuntimeModule.ResolveMember(this, this._impl, metadataToken, genericTypeArguments, genericMethodArguments);
		}

		internal static MemberInfo ResolveMember(Module module, IntPtr monoModule, int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			ResolveTokenError resolveTokenError;
			MemberInfo memberInfo = RuntimeModule.ResolveMemberToken(monoModule, metadataToken, RuntimeModule.ptrs_from_types(genericTypeArguments), RuntimeModule.ptrs_from_types(genericMethodArguments), out resolveTokenError);
			if (memberInfo == null)
			{
				throw RuntimeModule.resolve_token_exception(module.Name, metadataToken, resolveTokenError, "MemberInfo");
			}
			return memberInfo;
		}

		public override MethodBase ResolveMethod(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			return RuntimeModule.ResolveMethod(this, this._impl, metadataToken, genericTypeArguments, genericMethodArguments);
		}

		internal static MethodBase ResolveMethod(Module module, IntPtr monoModule, int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			ResolveTokenError resolveTokenError;
			IntPtr intPtr = RuntimeModule.ResolveMethodToken(monoModule, metadataToken, RuntimeModule.ptrs_from_types(genericTypeArguments), RuntimeModule.ptrs_from_types(genericMethodArguments), out resolveTokenError);
			if (intPtr == IntPtr.Zero)
			{
				throw RuntimeModule.resolve_token_exception(module.Name, metadataToken, resolveTokenError, "MethodBase");
			}
			return RuntimeMethodInfo.GetMethodFromHandleNoGenericCheck(new RuntimeMethodHandle(intPtr));
		}

		public override string ResolveString(int metadataToken)
		{
			return RuntimeModule.ResolveString(this, this._impl, metadataToken);
		}

		internal static string ResolveString(Module module, IntPtr monoModule, int metadataToken)
		{
			ResolveTokenError resolveTokenError;
			string text = RuntimeModule.ResolveStringToken(monoModule, metadataToken, out resolveTokenError);
			if (text == null)
			{
				throw RuntimeModule.resolve_token_exception(module.Name, metadataToken, resolveTokenError, "string");
			}
			return text;
		}

		public override Type ResolveType(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			return RuntimeModule.ResolveType(this, this._impl, metadataToken, genericTypeArguments, genericMethodArguments);
		}

		internal static Type ResolveType(Module module, IntPtr monoModule, int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			ResolveTokenError resolveTokenError;
			IntPtr intPtr = RuntimeModule.ResolveTypeToken(monoModule, metadataToken, RuntimeModule.ptrs_from_types(genericTypeArguments), RuntimeModule.ptrs_from_types(genericMethodArguments), out resolveTokenError);
			if (intPtr == IntPtr.Zero)
			{
				throw RuntimeModule.resolve_token_exception(module.Name, metadataToken, resolveTokenError, "Type");
			}
			return Type.GetTypeFromHandle(new RuntimeTypeHandle(intPtr));
		}

		public override byte[] ResolveSignature(int metadataToken)
		{
			return RuntimeModule.ResolveSignature(this, this._impl, metadataToken);
		}

		internal static byte[] ResolveSignature(Module module, IntPtr monoModule, int metadataToken)
		{
			ResolveTokenError resolveTokenError;
			byte[] array = RuntimeModule.ResolveSignature(monoModule, metadataToken, out resolveTokenError);
			if (array == null)
			{
				throw RuntimeModule.resolve_token_exception(module.Name, metadataToken, resolveTokenError, "signature");
			}
			return array;
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			UnitySerializationHolder.GetUnitySerializationInfo(info, 5, this.ScopeName, this.GetRuntimeAssembly());
		}

		public override X509Certificate GetSignerCertificate()
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

		public override Type[] GetTypes()
		{
			return RuntimeModule.InternalGetTypes(this._impl);
		}

		public override IList<CustomAttributeData> GetCustomAttributesData()
		{
			return CustomAttributeData.GetCustomAttributes(this);
		}

		internal RuntimeAssembly GetRuntimeAssembly()
		{
			return (RuntimeAssembly)this.assembly;
		}

		internal IntPtr MonoModule
		{
			get
			{
				return this._impl;
			}
		}

		internal override Guid GetModuleVersionId()
		{
			byte[] array = new byte[16];
			RuntimeModule.GetGuidInternal(this._impl, array);
			return new Guid(array);
		}

		internal static Exception resolve_token_exception(string name, int metadataToken, ResolveTokenError error, string tokenType)
		{
			if (error == ResolveTokenError.OutOfRange)
			{
				return new ArgumentOutOfRangeException("metadataToken", string.Format("Token 0x{0:x} is not valid in the scope of module {1}", metadataToken, name));
			}
			return new ArgumentException(string.Format("Token 0x{0:x} is not a valid {1} token in the scope of module {2}", metadataToken, tokenType, name), "metadataToken");
		}

		internal static IntPtr[] ptrs_from_types(Type[] types)
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

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int get_MetadataToken(Module module);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetMDStreamVersion(IntPtr module);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Type[] InternalGetTypes(IntPtr module);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern IntPtr GetHINSTANCE(IntPtr module);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetGuidInternal(IntPtr module, byte[] guid);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Type GetGlobalType(IntPtr module);

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

		internal IntPtr _impl;

		internal Assembly assembly;

		internal string fqname;

		internal string name;

		internal string scopename;

		internal bool is_resource;

		internal int token;
	}
}
