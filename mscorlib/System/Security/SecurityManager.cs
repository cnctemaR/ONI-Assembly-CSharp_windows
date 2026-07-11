using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;

namespace System.Security
{
	[ComVisible(true)]
	public static class SecurityManager
	{
		public static extern bool CheckExecutionRights
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"ControlPolicy\"/>\n</PermissionSet>\n")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("The security manager cannot be turned off on MS runtime")]
		public static extern bool SecurityEnabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"ControlPolicy\"/>\n</PermissionSet>\n")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[MonoTODO("CAS support is experimental (and unsupported). This method only works in FullTrust.")]
		[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.StrongNameIdentityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                PublicKeyBlob=\"2100000000000000000400000000000000\"/>\n</PermissionSet>\n")]
		public static void GetZoneAndOrigin(out ArrayList zone, out ArrayList origin)
		{
			zone = new ArrayList();
			origin = new ArrayList();
		}

		public static bool IsGranted(IPermission perm)
		{
			return perm == null || !SecurityManager.SecurityEnabled || SecurityManager.IsGranted(Assembly.GetCallingAssembly(), perm);
		}

		internal static bool IsGranted(Assembly a, IPermission perm)
		{
			PermissionSet grantedPermissionSet = a.GrantedPermissionSet;
			if (grantedPermissionSet != null && !grantedPermissionSet.IsUnrestricted())
			{
				CodeAccessPermission codeAccessPermission = (CodeAccessPermission)grantedPermissionSet.GetPermission(perm.GetType());
				if (!perm.IsSubsetOf(codeAccessPermission))
				{
					return false;
				}
			}
			PermissionSet deniedPermissionSet = a.DeniedPermissionSet;
			if (deniedPermissionSet != null && !deniedPermissionSet.IsEmpty())
			{
				if (deniedPermissionSet.IsUnrestricted())
				{
					return false;
				}
				CodeAccessPermission codeAccessPermission2 = (CodeAccessPermission)a.DeniedPermissionSet.GetPermission(perm.GetType());
				if (codeAccessPermission2 != null && perm.IsSubsetOf(codeAccessPermission2))
				{
					return false;
				}
			}
			return true;
		}

		internal static IPermission CheckPermissionSet(Assembly a, PermissionSet ps, bool noncas)
		{
			if (ps.IsEmpty())
			{
				return null;
			}
			foreach (object obj in ps)
			{
				IPermission permission = (IPermission)obj;
				if (!noncas && permission is CodeAccessPermission)
				{
					if (!SecurityManager.IsGranted(a, permission))
					{
						return permission;
					}
				}
				else
				{
					try
					{
						permission.Demand();
					}
					catch (SecurityException)
					{
						return permission;
					}
				}
			}
			return null;
		}

		internal static IPermission CheckPermissionSet(AppDomain ad, PermissionSet ps)
		{
			if (ps == null || ps.IsEmpty())
			{
				return null;
			}
			PermissionSet grantedPermissionSet = ad.GrantedPermissionSet;
			if (grantedPermissionSet == null)
			{
				return null;
			}
			if (grantedPermissionSet.IsUnrestricted())
			{
				return null;
			}
			if (ps.IsUnrestricted())
			{
				return new SecurityPermission(SecurityPermissionFlag.NoFlags);
			}
			foreach (object obj in ps)
			{
				IPermission permission = (IPermission)obj;
				if (permission is CodeAccessPermission)
				{
					CodeAccessPermission codeAccessPermission = (CodeAccessPermission)grantedPermissionSet.GetPermission(permission.GetType());
					if (codeAccessPermission == null)
					{
						if ((!grantedPermissionSet.IsUnrestricted() || !(permission is IUnrestrictedPermission)) && !permission.IsSubsetOf(null))
						{
							return permission;
						}
					}
					else if (!permission.IsSubsetOf(codeAccessPermission))
					{
						return permission;
					}
				}
				else
				{
					try
					{
						permission.Demand();
					}
					catch (SecurityException)
					{
						return permission;
					}
				}
			}
			return null;
		}

		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"ControlPolicy\"/>\n</PermissionSet>\n")]
		public static PolicyLevel LoadPolicyLevelFromFile(string path, PolicyLevelType type)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			PolicyLevel policyLevel = null;
			try
			{
				policyLevel = new PolicyLevel(type.ToString(), type);
				policyLevel.LoadFromFile(path);
			}
			catch (Exception ex)
			{
				throw new ArgumentException(Locale.GetText("Invalid policy XML"), ex);
			}
			return policyLevel;
		}

		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"ControlPolicy\"/>\n</PermissionSet>\n")]
		public static PolicyLevel LoadPolicyLevelFromString(string str, PolicyLevelType type)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			PolicyLevel policyLevel = null;
			try
			{
				policyLevel = new PolicyLevel(type.ToString(), type);
				policyLevel.LoadFromString(str);
			}
			catch (Exception ex)
			{
				throw new ArgumentException(Locale.GetText("Invalid policy XML"), ex);
			}
			return policyLevel;
		}

		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"ControlPolicy\"/>\n</PermissionSet>\n")]
		public static IEnumerator PolicyHierarchy()
		{
			return SecurityManager.Hierarchy;
		}

		public static PermissionSet ResolvePolicy(Evidence evidence)
		{
			if (evidence == null)
			{
				return new PermissionSet(PermissionState.None);
			}
			PermissionSet permissionSet = null;
			IEnumerator hierarchy = SecurityManager.Hierarchy;
			while (hierarchy.MoveNext())
			{
				object obj = hierarchy.Current;
				PolicyLevel policyLevel = (PolicyLevel)obj;
				if (SecurityManager.ResolvePolicyLevel(ref permissionSet, policyLevel, evidence))
				{
					break;
				}
			}
			SecurityManager.ResolveIdentityPermissions(permissionSet, evidence);
			return permissionSet;
		}

		[MonoTODO("(2.0) more tests are needed")]
		public static PermissionSet ResolvePolicy(Evidence[] evidences)
		{
			if (evidences == null || evidences.Length == 0 || (evidences.Length == 1 && evidences[0].Count == 0))
			{
				return new PermissionSet(PermissionState.None);
			}
			PermissionSet permissionSet = SecurityManager.ResolvePolicy(evidences[0]);
			for (int i = 1; i < evidences.Length; i++)
			{
				permissionSet = permissionSet.Intersect(SecurityManager.ResolvePolicy(evidences[i]));
			}
			return permissionSet;
		}

		public static PermissionSet ResolveSystemPolicy(Evidence evidence)
		{
			if (evidence == null)
			{
				return new PermissionSet(PermissionState.None);
			}
			PermissionSet permissionSet = null;
			IEnumerator hierarchy = SecurityManager.Hierarchy;
			while (hierarchy.MoveNext())
			{
				object obj = hierarchy.Current;
				PolicyLevel policyLevel = (PolicyLevel)obj;
				if (policyLevel.Type == PolicyLevelType.AppDomain)
				{
					break;
				}
				if (SecurityManager.ResolvePolicyLevel(ref permissionSet, policyLevel, evidence))
				{
					break;
				}
			}
			SecurityManager.ResolveIdentityPermissions(permissionSet, evidence);
			return permissionSet;
		}

		public static PermissionSet ResolvePolicy(Evidence evidence, PermissionSet reqdPset, PermissionSet optPset, PermissionSet denyPset, out PermissionSet denied)
		{
			PermissionSet permissionSet = SecurityManager.ResolvePolicy(evidence);
			if (reqdPset != null && !reqdPset.IsSubsetOf(permissionSet))
			{
				throw new PolicyException(Locale.GetText("Policy doesn't grant the minimal permissions required to execute the assembly."));
			}
			if (SecurityManager.CheckExecutionRights)
			{
				bool flag = false;
				if (permissionSet != null)
				{
					if (permissionSet.IsUnrestricted())
					{
						flag = true;
					}
					else
					{
						IPermission permission = permissionSet.GetPermission(typeof(SecurityPermission));
						flag = SecurityManager._execution.IsSubsetOf(permission);
					}
				}
				if (!flag)
				{
					throw new PolicyException(Locale.GetText("Policy doesn't grant the right to execute the assembly."));
				}
			}
			denied = denyPset;
			return permissionSet;
		}

		public static IEnumerator ResolvePolicyGroups(Evidence evidence)
		{
			if (evidence == null)
			{
				throw new ArgumentNullException("evidence");
			}
			ArrayList arrayList = new ArrayList();
			IEnumerator hierarchy = SecurityManager.Hierarchy;
			while (hierarchy.MoveNext())
			{
				object obj = hierarchy.Current;
				PolicyLevel policyLevel = (PolicyLevel)obj;
				CodeGroup codeGroup = policyLevel.ResolveMatchingCodeGroups(evidence);
				arrayList.Add(codeGroup);
			}
			return arrayList.GetEnumerator();
		}

		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"ControlPolicy\"/>\n</PermissionSet>\n")]
		public static void SavePolicy()
		{
			IEnumerator hierarchy = SecurityManager.Hierarchy;
			while (hierarchy.MoveNext())
			{
				object obj = hierarchy.Current;
				PolicyLevel policyLevel = obj as PolicyLevel;
				policyLevel.Save();
			}
		}

		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"ControlPolicy\"/>\n</PermissionSet>\n")]
		public static void SavePolicyLevel(PolicyLevel level)
		{
			level.Save();
		}

		private static IEnumerator Hierarchy
		{
			get
			{
				object lockObject = SecurityManager._lockObject;
				lock (lockObject)
				{
					if (SecurityManager._hierarchy == null)
					{
						SecurityManager.InitializePolicyHierarchy();
					}
				}
				return SecurityManager._hierarchy.GetEnumerator();
			}
		}

		private static void InitializePolicyHierarchy()
		{
			string directoryName = Path.GetDirectoryName(Environment.GetMachineConfigPath());
			string text = Path.Combine(Environment.InternalGetFolderPath(Environment.SpecialFolder.ApplicationData), "mono");
			PolicyLevel policyLevel = new PolicyLevel("Enterprise", PolicyLevelType.Enterprise);
			SecurityManager._level = policyLevel;
			policyLevel.LoadFromFile(Path.Combine(directoryName, "enterprisesec.config"));
			PolicyLevel policyLevel2 = new PolicyLevel("Machine", PolicyLevelType.Machine);
			SecurityManager._level = policyLevel2;
			policyLevel2.LoadFromFile(Path.Combine(directoryName, "security.config"));
			PolicyLevel policyLevel3 = new PolicyLevel("User", PolicyLevelType.User);
			SecurityManager._level = policyLevel3;
			policyLevel3.LoadFromFile(Path.Combine(text, "security.config"));
			SecurityManager._hierarchy = ArrayList.Synchronized(new ArrayList { policyLevel, policyLevel2, policyLevel3 });
			SecurityManager._level = null;
		}

		internal static bool ResolvePolicyLevel(ref PermissionSet ps, PolicyLevel pl, Evidence evidence)
		{
			PolicyStatement policyStatement = pl.Resolve(evidence);
			if (policyStatement != null)
			{
				if (ps == null)
				{
					ps = policyStatement.PermissionSet;
				}
				else
				{
					ps = ps.Intersect(policyStatement.PermissionSet);
					if (ps == null)
					{
						ps = new PermissionSet(PermissionState.None);
					}
				}
				if ((policyStatement.Attributes & PolicyStatementAttribute.LevelFinal) == PolicyStatementAttribute.LevelFinal)
				{
					return true;
				}
			}
			return false;
		}

		internal static void ResolveIdentityPermissions(PermissionSet ps, Evidence evidence)
		{
			if (ps.IsUnrestricted())
			{
				return;
			}
			IEnumerator hostEnumerator = evidence.GetHostEnumerator();
			while (hostEnumerator.MoveNext())
			{
				object obj = hostEnumerator.Current;
				IIdentityPermissionFactory identityPermissionFactory = obj as IIdentityPermissionFactory;
				if (identityPermissionFactory != null)
				{
					IPermission permission = identityPermissionFactory.CreateIdentityPermission(evidence);
					ps.AddPermission(permission);
				}
			}
		}

		internal static PolicyLevel ResolvingPolicyLevel
		{
			get
			{
				return SecurityManager._level;
			}
			set
			{
				SecurityManager._level = value;
			}
		}

		internal static PermissionSet Decode(IntPtr permissions, int length)
		{
			PermissionSet permissionSet = null;
			object lockObject = SecurityManager._lockObject;
			lock (lockObject)
			{
				if (SecurityManager._declsecCache == null)
				{
					SecurityManager._declsecCache = new Hashtable();
				}
				object obj = (int)permissions;
				permissionSet = (PermissionSet)SecurityManager._declsecCache[obj];
				if (permissionSet == null)
				{
					byte[] array = new byte[length];
					Marshal.Copy(permissions, array, 0, length);
					permissionSet = SecurityManager.Decode(array);
					permissionSet.DeclarativeSecurity = true;
					SecurityManager._declsecCache.Add(obj, permissionSet);
				}
			}
			return permissionSet;
		}

		internal static PermissionSet Decode(byte[] encodedPermissions)
		{
			if (encodedPermissions == null || encodedPermissions.Length < 1)
			{
				throw new SecurityException("Invalid metadata format.");
			}
			byte b = encodedPermissions[0];
			if (b == 46)
			{
				return PermissionSet.CreateFromBinaryFormat(encodedPermissions);
			}
			if (b != 60)
			{
				throw new SecurityException(Locale.GetText("Unknown metadata format."));
			}
			string @string = Encoding.Unicode.GetString(encodedPermissions);
			return new PermissionSet(@string);
		}

		private static IPermission UnmanagedCode
		{
			get
			{
				object lockObject = SecurityManager._lockObject;
				lock (lockObject)
				{
					if (SecurityManager._unmanagedCode == null)
					{
						SecurityManager._unmanagedCode = new SecurityPermission(SecurityPermissionFlag.UnmanagedCode);
					}
				}
				return SecurityManager._unmanagedCode;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern bool GetLinkDemandSecurity(MethodBase method, RuntimeDeclSecurityActions* cdecl, RuntimeDeclSecurityActions* mdecl);

		internal unsafe static void ReflectedLinkDemandInvoke(MethodBase mb)
		{
			RuntimeDeclSecurityActions runtimeDeclSecurityActions;
			RuntimeDeclSecurityActions runtimeDeclSecurityActions2;
			if (!SecurityManager.GetLinkDemandSecurity(mb, &runtimeDeclSecurityActions, &runtimeDeclSecurityActions2))
			{
				return;
			}
			PermissionSet permissionSet = null;
			if (runtimeDeclSecurityActions.cas.size > 0)
			{
				permissionSet = SecurityManager.Decode(runtimeDeclSecurityActions.cas.blob, runtimeDeclSecurityActions.cas.size);
			}
			if (runtimeDeclSecurityActions.noncas.size > 0)
			{
				PermissionSet permissionSet2 = SecurityManager.Decode(runtimeDeclSecurityActions.noncas.blob, runtimeDeclSecurityActions.noncas.size);
				permissionSet = ((permissionSet != null) ? permissionSet.Union(permissionSet2) : permissionSet2);
			}
			if (runtimeDeclSecurityActions2.cas.size > 0)
			{
				PermissionSet permissionSet3 = SecurityManager.Decode(runtimeDeclSecurityActions2.cas.blob, runtimeDeclSecurityActions2.cas.size);
				permissionSet = ((permissionSet != null) ? permissionSet.Union(permissionSet3) : permissionSet3);
			}
			if (runtimeDeclSecurityActions2.noncas.size > 0)
			{
				PermissionSet permissionSet4 = SecurityManager.Decode(runtimeDeclSecurityActions2.noncas.blob, runtimeDeclSecurityActions2.noncas.size);
				permissionSet = ((permissionSet != null) ? permissionSet.Union(permissionSet4) : permissionSet4);
			}
			if (permissionSet != null)
			{
				permissionSet.Demand();
			}
		}

		internal unsafe static bool ReflectedLinkDemandQuery(MethodBase mb)
		{
			RuntimeDeclSecurityActions runtimeDeclSecurityActions;
			RuntimeDeclSecurityActions runtimeDeclSecurityActions2;
			return !SecurityManager.GetLinkDemandSecurity(mb, &runtimeDeclSecurityActions, &runtimeDeclSecurityActions2) || SecurityManager.LinkDemand(mb.ReflectedType.Assembly, &runtimeDeclSecurityActions, &runtimeDeclSecurityActions2);
		}

		private unsafe static bool LinkDemand(Assembly a, RuntimeDeclSecurityActions* klass, RuntimeDeclSecurityActions* method)
		{
			bool flag2;
			try
			{
				bool flag = true;
				if (klass->cas.size > 0)
				{
					PermissionSet permissionSet = SecurityManager.Decode(klass->cas.blob, klass->cas.size);
					flag = SecurityManager.CheckPermissionSet(a, permissionSet, false) == null;
				}
				if (flag && klass->noncas.size > 0)
				{
					PermissionSet permissionSet = SecurityManager.Decode(klass->noncas.blob, klass->noncas.size);
					flag = SecurityManager.CheckPermissionSet(a, permissionSet, true) == null;
				}
				if (flag && method->cas.size > 0)
				{
					PermissionSet permissionSet = SecurityManager.Decode(method->cas.blob, method->cas.size);
					flag = SecurityManager.CheckPermissionSet(a, permissionSet, false) == null;
				}
				if (flag && method->noncas.size > 0)
				{
					PermissionSet permissionSet = SecurityManager.Decode(method->noncas.blob, method->noncas.size);
					flag = SecurityManager.CheckPermissionSet(a, permissionSet, true) == null;
				}
				flag2 = flag;
			}
			catch (SecurityException)
			{
				flag2 = false;
			}
			return flag2;
		}

		private static bool LinkDemandFullTrust(Assembly a)
		{
			PermissionSet grantedPermissionSet = a.GrantedPermissionSet;
			if (grantedPermissionSet != null && !grantedPermissionSet.IsUnrestricted())
			{
				return false;
			}
			PermissionSet deniedPermissionSet = a.DeniedPermissionSet;
			return deniedPermissionSet == null || deniedPermissionSet.IsEmpty();
		}

		private static bool LinkDemandUnmanaged(Assembly a)
		{
			return SecurityManager.IsGranted(a, SecurityManager.UnmanagedCode);
		}

		private static void LinkDemandSecurityException(int securityViolation, IntPtr methodHandle)
		{
			RuntimeMethodHandle runtimeMethodHandle = new RuntimeMethodHandle(methodHandle);
			MethodInfo methodInfo = (MethodInfo)MethodBase.GetMethodFromHandle(runtimeMethodHandle);
			Assembly assembly = methodInfo.DeclaringType.Assembly;
			AssemblyName assemblyName = null;
			PermissionSet permissionSet = null;
			PermissionSet permissionSet2 = null;
			object obj = null;
			IPermission permission = null;
			if (assembly != null)
			{
				assemblyName = assembly.UnprotectedGetName();
				permissionSet = assembly.GrantedPermissionSet;
				permissionSet2 = assembly.DeniedPermissionSet;
			}
			string text;
			switch (securityViolation)
			{
			case 1:
				text = Locale.GetText("Permissions refused to call this method.");
				goto IL_00E5;
			case 2:
				text = Locale.GetText("Partially trusted callers aren't allowed to call into this assembly.");
				obj = DefaultPolicies.FullTrust;
				goto IL_00E5;
			case 4:
				text = Locale.GetText("Calling internal calls is restricted to ECMA signed assemblies.");
				goto IL_00E5;
			case 8:
				text = Locale.GetText("Calling unmanaged code isn't allowed from this assembly.");
				obj = SecurityManager._unmanagedCode;
				permission = SecurityManager._unmanagedCode;
				goto IL_00E5;
			}
			text = Locale.GetText("JIT time LinkDemand failed.");
			IL_00E5:
			throw new SecurityException(text, assemblyName, permissionSet, permissionSet2, methodInfo, SecurityAction.LinkDemand, obj, permission, null);
		}

		private static void InheritanceDemandSecurityException(int securityViolation, Assembly a, Type t, MethodInfo method)
		{
			AssemblyName assemblyName = null;
			PermissionSet permissionSet = null;
			PermissionSet permissionSet2 = null;
			if (a != null)
			{
				assemblyName = a.UnprotectedGetName();
				permissionSet = a.GrantedPermissionSet;
				permissionSet2 = a.DeniedPermissionSet;
			}
			string text;
			if (securityViolation != 1)
			{
				if (securityViolation != 2)
				{
					text = Locale.GetText("Load time InheritDemand failed.");
				}
				else
				{
					text = Locale.GetText("Method override refused.");
				}
			}
			else
			{
				text = string.Format(Locale.GetText("Class inheritance refused for {0}."), t);
			}
			throw new SecurityException(text, assemblyName, permissionSet, permissionSet2, method, SecurityAction.InheritanceDemand, null, null, null);
		}

		private static void ThrowException(Exception ex)
		{
			throw ex;
		}

		private unsafe static bool InheritanceDemand(AppDomain ad, Assembly a, RuntimeDeclSecurityActions* actions)
		{
			bool flag2;
			try
			{
				bool flag = true;
				if (actions->cas.size > 0)
				{
					PermissionSet permissionSet = SecurityManager.Decode(actions->cas.blob, actions->cas.size);
					flag = SecurityManager.CheckPermissionSet(a, permissionSet, false) == null;
					if (flag)
					{
						flag = SecurityManager.CheckPermissionSet(ad, permissionSet) == null;
					}
				}
				if (actions->noncas.size > 0)
				{
					PermissionSet permissionSet = SecurityManager.Decode(actions->noncas.blob, actions->noncas.size);
					flag = SecurityManager.CheckPermissionSet(a, permissionSet, true) == null;
					if (flag)
					{
						flag = SecurityManager.CheckPermissionSet(ad, permissionSet) == null;
					}
				}
				flag2 = flag;
			}
			catch (SecurityException)
			{
				flag2 = false;
			}
			return flag2;
		}

		private static void DemandUnmanaged()
		{
			SecurityManager.UnmanagedCode.Demand();
		}

		private static void InternalDemand(IntPtr permissions, int length)
		{
			PermissionSet permissionSet = SecurityManager.Decode(permissions, length);
			permissionSet.Demand();
		}

		private static void InternalDemandChoice(IntPtr permissions, int length)
		{
			throw new SecurityException("SecurityAction.DemandChoice was removed from 2.0");
		}

		private static object _lockObject = new object();

		private static ArrayList _hierarchy;

		private static IPermission _unmanagedCode;

		private static Hashtable _declsecCache;

		private static PolicyLevel _level;

		private static SecurityPermission _execution = new SecurityPermission(SecurityPermissionFlag.Execution);
	}
}
