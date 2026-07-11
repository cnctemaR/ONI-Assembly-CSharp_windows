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
	[ComVisible(true)]
	[ComDefaultInterface(typeof(_Module))]
	[ClassInterface(ClassInterfaceType.None)]
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public abstract class Module : ISerializable, ICustomAttributeProvider, _Module
	{
		public ModuleHandle ModuleHandle
		{
			get
			{
				return new ModuleHandle(this._impl);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int get_MetadataToken(Module module);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetMDStreamVersion(IntPtr module_handle);

		public FieldInfo GetField(string name)
		{
			return this.GetField(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		}

		public FieldInfo[] GetFields()
		{
			return this.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		}

		public MethodInfo GetMethod(string name)
		{
			return this.GetMethodImpl(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, CallingConventions.Any, null, null);
		}

		public MethodInfo GetMethod(string name, Type[] types)
		{
			if (types == null)
			{
				throw new ArgumentNullException("types");
			}
			return this.GetMethodImpl(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, CallingConventions.Any, types, null);
		}

		public MethodInfo GetMethod(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			if (types == null)
			{
				throw new ArgumentNullException("types");
			}
			return this.GetMethodImpl(name, bindingAttr, binder, callConvention, types, modifiers);
		}

		public MethodInfo[] GetMethods()
		{
			return this.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		}

		[SecurityCritical]
		[SecurityPermission(SecurityAction.LinkDemand, SerializationFormatter = true)]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new NotImplementedException();
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

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern Type[] InternalGetTypes();

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

		internal Exception resolve_token_exception(int metadataToken, ResolveTokenError error, string tokenType)
		{
			if (error == ResolveTokenError.OutOfRange)
			{
				return new ArgumentOutOfRangeException("metadataToken", string.Format("Token 0x{0:x} is not valid in the scope of module {1}", metadataToken, this.name));
			}
			return new ArgumentException(string.Format("Token 0x{0:x} is not a valid {1} token in the scope of module {2}", metadataToken, tokenType, this.name), "metadataToken");
		}

		internal IntPtr[] ptrs_from_types(Type[] types)
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

		public MemberInfo ResolveMember(int metadataToken)
		{
			return this.ResolveMember(metadataToken, null, null);
		}

		public MethodBase ResolveMethod(int metadataToken)
		{
			return this.ResolveMethod(metadataToken, null, null);
		}

		public Type ResolveType(int metadataToken)
		{
			return this.ResolveType(metadataToken, null, null);
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
			if (text.Length > 0 && text[text.Length - 1] == '*')
			{
				return m.Name.StartsWithOrdinalUnchecked(text.Substring(0, text.Length - 1));
			}
			return m.Name == text;
		}

		private static bool filter_by_type_name_ignore_case(Type m, object filterCriteria)
		{
			string text = (string)filterCriteria;
			if (text.Length > 0 && text[text.Length - 1] == '*')
			{
				return m.Name.StartsWith(text.Substring(0, text.Length - 1), StringComparison.OrdinalIgnoreCase);
			}
			return string.Compare(m.Name, text, StringComparison.OrdinalIgnoreCase) == 0;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern IntPtr GetHINSTANCE();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern string GetGuidInternal();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern Type GetGlobalType();

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

		public override bool Equals(object o)
		{
			return o == this;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(Module left, Module right)
		{
			return left == right || (!((left == null) ^ (right == null)) && left.Equals(right));
		}

		public static bool operator !=(Module left, Module right)
		{
			return left != right && (((left == null) ^ (right == null)) || !left.Equals(right));
		}

		public virtual Assembly Assembly
		{
			get
			{
				throw Module.CreateNIE();
			}
		}

		public virtual string Name
		{
			get
			{
				throw Module.CreateNIE();
			}
		}

		public virtual string ScopeName
		{
			get
			{
				throw Module.CreateNIE();
			}
		}

		public virtual int MDStreamVersion
		{
			get
			{
				throw Module.CreateNIE();
			}
		}

		public virtual Guid ModuleVersionId
		{
			get
			{
				throw Module.CreateNIE();
			}
		}

		public virtual string FullyQualifiedName
		{
			get
			{
				throw Module.CreateNIE();
			}
		}

		public virtual int MetadataToken
		{
			get
			{
				throw Module.CreateNIE();
			}
		}

		private static Exception CreateNIE()
		{
			return new NotImplementedException("Derived classes must implement it");
		}

		public virtual bool IsResource()
		{
			throw Module.CreateNIE();
		}

		public virtual Type[] FindTypes(TypeFilter filter, object filterCriteria)
		{
			throw Module.CreateNIE();
		}

		public virtual object[] GetCustomAttributes(bool inherit)
		{
			throw Module.CreateNIE();
		}

		public virtual object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			throw Module.CreateNIE();
		}

		public virtual IList<CustomAttributeData> GetCustomAttributesData()
		{
			throw Module.CreateNIE();
		}

		public virtual FieldInfo GetField(string name, BindingFlags bindingAttr)
		{
			throw Module.CreateNIE();
		}

		public virtual FieldInfo[] GetFields(BindingFlags bindingFlags)
		{
			throw Module.CreateNIE();
		}

		protected virtual MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			throw Module.CreateNIE();
		}

		public virtual MethodInfo[] GetMethods(BindingFlags bindingFlags)
		{
			throw Module.CreateNIE();
		}

		public virtual void GetPEKind(out PortableExecutableKinds peKind, out ImageFileMachine machine)
		{
			throw Module.CreateNIE();
		}

		[ComVisible(true)]
		public virtual Type GetType(string className, bool throwOnError, bool ignoreCase)
		{
			throw Module.CreateNIE();
		}

		public virtual bool IsDefined(Type attributeType, bool inherit)
		{
			throw Module.CreateNIE();
		}

		public virtual FieldInfo ResolveField(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			throw Module.CreateNIE();
		}

		public virtual MemberInfo ResolveMember(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			throw Module.CreateNIE();
		}

		public virtual MethodBase ResolveMethod(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			throw Module.CreateNIE();
		}

		public virtual byte[] ResolveSignature(int metadataToken)
		{
			throw Module.CreateNIE();
		}

		public virtual string ResolveString(int metadataToken)
		{
			throw Module.CreateNIE();
		}

		public virtual Type ResolveType(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			throw Module.CreateNIE();
		}

		public virtual X509Certificate GetSignerCertificate()
		{
			throw Module.CreateNIE();
		}

		public virtual Type[] GetTypes()
		{
			throw Module.CreateNIE();
		}

		public virtual IEnumerable<CustomAttributeData> CustomAttributes
		{
			get
			{
				return this.GetCustomAttributesData();
			}
		}

		public static readonly TypeFilter FilterTypeName = new TypeFilter(Module.filter_by_type_name);

		public static readonly TypeFilter FilterTypeNameIgnoreCase = new TypeFilter(Module.filter_by_type_name_ignore_case);

		internal IntPtr _impl;

		internal Assembly assembly;

		internal string fqname;

		internal string name;

		internal string scopename;

		internal bool is_resource;

		internal int token;

		private const BindingFlags defaultBindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;
	}
}
