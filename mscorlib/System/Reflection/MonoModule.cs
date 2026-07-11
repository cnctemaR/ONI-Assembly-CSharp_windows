using System;
using System.Collections.Generic;
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
	internal class MonoModule : RuntimeModule
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
				return Module.GetMDStreamVersion(this._impl);
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
			Type globalType = base.GetGlobalType();
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
			Type globalType = base.GetGlobalType();
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
				return Module.get_MetadataToken(this);
			}
		}

		protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			if (this.IsResource())
			{
				return null;
			}
			Type globalType = base.GetGlobalType();
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
			Type globalType = base.GetGlobalType();
			if (!(globalType != null))
			{
				return new MethodInfo[0];
			}
			return globalType.GetMethods(bindingFlags);
		}

		public override void GetPEKind(out PortableExecutableKinds peKind, out ImageFileMachine machine)
		{
			base.ModuleHandle.GetPEKind(out peKind, out machine);
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
			ResolveTokenError resolveTokenError;
			IntPtr intPtr = Module.ResolveFieldToken(this._impl, metadataToken, base.ptrs_from_types(genericTypeArguments), base.ptrs_from_types(genericMethodArguments), out resolveTokenError);
			if (intPtr == IntPtr.Zero)
			{
				throw base.resolve_token_exception(metadataToken, resolveTokenError, "Field");
			}
			return FieldInfo.GetFieldFromHandle(new RuntimeFieldHandle(intPtr));
		}

		public override MemberInfo ResolveMember(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			ResolveTokenError resolveTokenError;
			MemberInfo memberInfo = Module.ResolveMemberToken(this._impl, metadataToken, base.ptrs_from_types(genericTypeArguments), base.ptrs_from_types(genericMethodArguments), out resolveTokenError);
			if (memberInfo == null)
			{
				throw base.resolve_token_exception(metadataToken, resolveTokenError, "MemberInfo");
			}
			return memberInfo;
		}

		public override MethodBase ResolveMethod(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			ResolveTokenError resolveTokenError;
			IntPtr intPtr = Module.ResolveMethodToken(this._impl, metadataToken, base.ptrs_from_types(genericTypeArguments), base.ptrs_from_types(genericMethodArguments), out resolveTokenError);
			if (intPtr == IntPtr.Zero)
			{
				throw base.resolve_token_exception(metadataToken, resolveTokenError, "MethodBase");
			}
			return MethodBase.GetMethodFromHandleNoGenericCheck(new RuntimeMethodHandle(intPtr));
		}

		public override string ResolveString(int metadataToken)
		{
			ResolveTokenError resolveTokenError;
			string text = Module.ResolveStringToken(this._impl, metadataToken, out resolveTokenError);
			if (text == null)
			{
				throw base.resolve_token_exception(metadataToken, resolveTokenError, "string");
			}
			return text;
		}

		public override Type ResolveType(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			ResolveTokenError resolveTokenError;
			IntPtr intPtr = Module.ResolveTypeToken(this._impl, metadataToken, base.ptrs_from_types(genericTypeArguments), base.ptrs_from_types(genericMethodArguments), out resolveTokenError);
			if (intPtr == IntPtr.Zero)
			{
				throw base.resolve_token_exception(metadataToken, resolveTokenError, "Type");
			}
			return Type.GetTypeFromHandle(new RuntimeTypeHandle(intPtr));
		}

		public override byte[] ResolveSignature(int metadataToken)
		{
			ResolveTokenError resolveTokenError;
			byte[] array = Module.ResolveSignature(this._impl, metadataToken, out resolveTokenError);
			if (array == null)
			{
				throw base.resolve_token_exception(metadataToken, resolveTokenError, "signature");
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
			return base.InternalGetTypes();
		}

		public override IList<CustomAttributeData> GetCustomAttributesData()
		{
			return CustomAttributeData.GetCustomAttributes(this);
		}

		internal RuntimeAssembly GetRuntimeAssembly()
		{
			return (RuntimeAssembly)this.assembly;
		}
	}
}
