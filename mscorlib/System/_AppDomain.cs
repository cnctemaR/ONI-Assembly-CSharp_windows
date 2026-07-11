using System;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Security.Principal;

namespace System
{
	[ComVisible(true)]
	[CLSCompliant(false)]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("05F696DC-2B29-3663-AD8B-C4389CF2A713")]
	public interface _AppDomain
	{
		event AssemblyLoadEventHandler AssemblyLoad;

		event ResolveEventHandler AssemblyResolve;

		event EventHandler DomainUnload;

		event EventHandler ProcessExit;

		event ResolveEventHandler ResourceResolve;

		event ResolveEventHandler TypeResolve;

		event UnhandledExceptionEventHandler UnhandledException;

		string BaseDirectory { get; }

		string DynamicDirectory { get; }

		Evidence Evidence { get; }

		string FriendlyName { get; }

		string RelativeSearchPath { get; }

		bool ShadowCopyFiles { get; }

		[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"ControlAppDomain\"/>\n</PermissionSet>\n")]
		void AppendPrivatePath(string path);

		[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"ControlAppDomain\"/>\n</PermissionSet>\n")]
		void ClearPrivatePath();

		[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"ControlAppDomain\"/>\n</PermissionSet>\n")]
		void ClearShadowCopyPath();

		ObjectHandle CreateInstance(string assemblyName, string typeName);

		ObjectHandle CreateInstance(string assemblyName, string typeName, object[] activationAttributes);

		ObjectHandle CreateInstance(string assemblyName, string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes, Evidence securityAttributes);

		ObjectHandle CreateInstanceFrom(string assemblyFile, string typeName);

		ObjectHandle CreateInstanceFrom(string assemblyFile, string typeName, object[] activationAttributes);

		ObjectHandle CreateInstanceFrom(string assemblyFile, string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes, Evidence securityAttributes);

		AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access);

		AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, Evidence evidence);

		AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, string dir);

		AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, string dir, Evidence evidence);

		AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, PermissionSet requiredPermissions, PermissionSet optionalPermissions, PermissionSet refusedPermissions);

		AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, Evidence evidence, PermissionSet requiredPermissions, PermissionSet optionalPermissions, PermissionSet refusedPermissions);

		AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, string dir, PermissionSet requiredPermissions, PermissionSet optionalPermissions, PermissionSet refusedPermissions);

		AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, string dir, Evidence evidence, PermissionSet requiredPermissions, PermissionSet optionalPermissions, PermissionSet refusedPermissions);

		AssemblyBuilder DefineDynamicAssembly(AssemblyName name, AssemblyBuilderAccess access, string dir, Evidence evidence, PermissionSet requiredPermissions, PermissionSet optionalPermissions, PermissionSet refusedPermissions, bool isSynchronized);

		void DoCallBack(CrossAppDomainDelegate theDelegate);

		bool Equals(object other);

		int ExecuteAssembly(string assemblyFile);

		int ExecuteAssembly(string assemblyFile, Evidence assemblySecurity);

		int ExecuteAssembly(string assemblyFile, Evidence assemblySecurity, string[] args);

		Assembly[] GetAssemblies();

		object GetData(string name);

		int GetHashCode();

		[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"Infrastructure\"/>\n</PermissionSet>\n")]
		object GetLifetimeService();

		Type GetType();

		[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"Infrastructure\"/>\n</PermissionSet>\n")]
		object InitializeLifetimeService();

		Assembly Load(AssemblyName assemblyRef);

		Assembly Load(byte[] rawAssembly);

		Assembly Load(string assemblyString);

		Assembly Load(AssemblyName assemblyRef, Evidence assemblySecurity);

		Assembly Load(byte[] rawAssembly, byte[] rawSymbolStore);

		Assembly Load(string assemblyString, Evidence assemblySecurity);

		Assembly Load(byte[] rawAssembly, byte[] rawSymbolStore, Evidence securityEvidence);

		[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"ControlAppDomain\"/>\n</PermissionSet>\n")]
		void SetAppDomainPolicy(PolicyLevel domainPolicy);

		[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"ControlAppDomain\"/>\n</PermissionSet>\n")]
		void SetCachePath(string s);

		[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"ControlAppDomain\"/>\n</PermissionSet>\n")]
		void SetData(string name, object data);

		void SetPrincipalPolicy(PrincipalPolicy policy);

		[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"ControlAppDomain\"/>\n</PermissionSet>\n")]
		void SetShadowCopyPath(string s);

		void SetThreadPrincipal(IPrincipal principal);

		string ToString();

		void GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId);

		void GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo);

		void GetTypeInfoCount(out uint pcTInfo);

		void Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr);
	}
}
