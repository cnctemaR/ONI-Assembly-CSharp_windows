using System;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;
using System.Threading;
using Mono.Security.Cryptography;

namespace System.IO.IsolatedStorage
{
	[ComVisible(true)]
	[PermissionSet(SecurityAction.Assert, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Unrestricted=\"true\"/>\n</PermissionSet>\n")]
	public sealed class IsolatedStorageFile : IsolatedStorage, IDisposable
	{
		private IsolatedStorageFile(IsolatedStorageScope scope)
		{
			this.storage_scope = scope;
		}

		internal IsolatedStorageFile(IsolatedStorageScope scope, string location)
		{
			this.storage_scope = scope;
			this.directory = new DirectoryInfo(location);
			if (!this.directory.Exists)
			{
				string text = Locale.GetText("Invalid storage.");
				throw new IsolatedStorageException(text);
			}
		}

		public static IEnumerator GetEnumerator(IsolatedStorageScope scope)
		{
			IsolatedStorageFile.Demand(scope);
			if (scope != IsolatedStorageScope.User && scope != (IsolatedStorageScope.User | IsolatedStorageScope.Roaming) && scope != IsolatedStorageScope.Machine)
			{
				string text = Locale.GetText("Invalid scope, only User, User|Roaming and Machine are valid");
				throw new ArgumentException(text);
			}
			return new IsolatedStorageFileEnumerator(scope, IsolatedStorageFile.GetIsolatedStorageRoot(scope));
		}

		public static IsolatedStorageFile GetStore(IsolatedStorageScope scope, Evidence domainEvidence, Type domainEvidenceType, Evidence assemblyEvidence, Type assemblyEvidenceType)
		{
			IsolatedStorageFile.Demand(scope);
			bool flag = (scope & IsolatedStorageScope.Domain) != IsolatedStorageScope.None;
			if (flag && domainEvidence == null)
			{
				throw new ArgumentNullException("domainEvidence");
			}
			bool flag2 = (scope & IsolatedStorageScope.Assembly) != IsolatedStorageScope.None;
			if (flag2 && assemblyEvidence == null)
			{
				throw new ArgumentNullException("assemblyEvidence");
			}
			IsolatedStorageFile isolatedStorageFile = new IsolatedStorageFile(scope);
			if (flag)
			{
				if (domainEvidenceType == null)
				{
					isolatedStorageFile._domainIdentity = IsolatedStorageFile.GetDomainIdentityFromEvidence(domainEvidence);
				}
				else
				{
					isolatedStorageFile._domainIdentity = IsolatedStorageFile.GetTypeFromEvidence(domainEvidence, domainEvidenceType);
				}
				if (isolatedStorageFile._domainIdentity == null)
				{
					throw new IsolatedStorageException(Locale.GetText("Couldn't find domain identity."));
				}
			}
			if (flag2)
			{
				if (assemblyEvidenceType == null)
				{
					isolatedStorageFile._assemblyIdentity = IsolatedStorageFile.GetAssemblyIdentityFromEvidence(assemblyEvidence);
				}
				else
				{
					isolatedStorageFile._assemblyIdentity = IsolatedStorageFile.GetTypeFromEvidence(assemblyEvidence, assemblyEvidenceType);
				}
				if (isolatedStorageFile._assemblyIdentity == null)
				{
					throw new IsolatedStorageException(Locale.GetText("Couldn't find assembly identity."));
				}
			}
			isolatedStorageFile.PostInit();
			return isolatedStorageFile;
		}

		public static IsolatedStorageFile GetStore(IsolatedStorageScope scope, object domainIdentity, object assemblyIdentity)
		{
			IsolatedStorageFile.Demand(scope);
			if ((scope & IsolatedStorageScope.Domain) != IsolatedStorageScope.None && domainIdentity == null)
			{
				throw new ArgumentNullException("domainIdentity");
			}
			bool flag = (scope & IsolatedStorageScope.Assembly) != IsolatedStorageScope.None;
			if (flag && assemblyIdentity == null)
			{
				throw new ArgumentNullException("assemblyIdentity");
			}
			IsolatedStorageFile isolatedStorageFile = new IsolatedStorageFile(scope);
			if (flag)
			{
				isolatedStorageFile._fullEvidences = Assembly.GetCallingAssembly().UnprotectedGetEvidence();
			}
			isolatedStorageFile._domainIdentity = domainIdentity;
			isolatedStorageFile._assemblyIdentity = assemblyIdentity;
			isolatedStorageFile.PostInit();
			return isolatedStorageFile;
		}

		public static IsolatedStorageFile GetStore(IsolatedStorageScope scope, Type domainEvidenceType, Type assemblyEvidenceType)
		{
			IsolatedStorageFile.Demand(scope);
			IsolatedStorageFile isolatedStorageFile = new IsolatedStorageFile(scope);
			if ((scope & IsolatedStorageScope.Domain) != IsolatedStorageScope.None)
			{
				if (domainEvidenceType == null)
				{
					domainEvidenceType = typeof(Url);
				}
				isolatedStorageFile._domainIdentity = IsolatedStorageFile.GetTypeFromEvidence(AppDomain.CurrentDomain.Evidence, domainEvidenceType);
			}
			if ((scope & IsolatedStorageScope.Assembly) != IsolatedStorageScope.None)
			{
				Evidence evidence = Assembly.GetCallingAssembly().UnprotectedGetEvidence();
				isolatedStorageFile._fullEvidences = evidence;
				if ((scope & IsolatedStorageScope.Domain) != IsolatedStorageScope.None)
				{
					if (assemblyEvidenceType == null)
					{
						assemblyEvidenceType = typeof(Url);
					}
					isolatedStorageFile._assemblyIdentity = IsolatedStorageFile.GetTypeFromEvidence(evidence, assemblyEvidenceType);
				}
				else
				{
					isolatedStorageFile._assemblyIdentity = IsolatedStorageFile.GetAssemblyIdentityFromEvidence(evidence);
				}
			}
			isolatedStorageFile.PostInit();
			return isolatedStorageFile;
		}

		public static IsolatedStorageFile GetStore(IsolatedStorageScope scope, object applicationIdentity)
		{
			IsolatedStorageFile.Demand(scope);
			if (applicationIdentity == null)
			{
				throw new ArgumentNullException("applicationIdentity");
			}
			IsolatedStorageFile isolatedStorageFile = new IsolatedStorageFile(scope);
			isolatedStorageFile._applicationIdentity = applicationIdentity;
			isolatedStorageFile._fullEvidences = Assembly.GetCallingAssembly().UnprotectedGetEvidence();
			isolatedStorageFile.PostInit();
			return isolatedStorageFile;
		}

		public static IsolatedStorageFile GetStore(IsolatedStorageScope scope, Type applicationEvidenceType)
		{
			IsolatedStorageFile.Demand(scope);
			IsolatedStorageFile isolatedStorageFile = new IsolatedStorageFile(scope);
			isolatedStorageFile.InitStore(scope, applicationEvidenceType);
			isolatedStorageFile._fullEvidences = Assembly.GetCallingAssembly().UnprotectedGetEvidence();
			isolatedStorageFile.PostInit();
			return isolatedStorageFile;
		}

		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.IsolatedStorageFilePermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Allowed=\"ApplicationIsolationByMachine\"/>\n</PermissionSet>\n")]
		public static IsolatedStorageFile GetMachineStoreForApplication()
		{
			IsolatedStorageScope isolatedStorageScope = IsolatedStorageScope.Machine | IsolatedStorageScope.Application;
			IsolatedStorageFile isolatedStorageFile = new IsolatedStorageFile(isolatedStorageScope);
			isolatedStorageFile.InitStore(isolatedStorageScope, null);
			isolatedStorageFile._fullEvidences = Assembly.GetCallingAssembly().UnprotectedGetEvidence();
			isolatedStorageFile.PostInit();
			return isolatedStorageFile;
		}

		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.IsolatedStorageFilePermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Allowed=\"AssemblyIsolationByMachine\"/>\n</PermissionSet>\n")]
		public static IsolatedStorageFile GetMachineStoreForAssembly()
		{
			IsolatedStorageScope isolatedStorageScope = IsolatedStorageScope.Assembly | IsolatedStorageScope.Machine;
			IsolatedStorageFile isolatedStorageFile = new IsolatedStorageFile(isolatedStorageScope);
			Evidence evidence = Assembly.GetCallingAssembly().UnprotectedGetEvidence();
			isolatedStorageFile._fullEvidences = evidence;
			isolatedStorageFile._assemblyIdentity = IsolatedStorageFile.GetAssemblyIdentityFromEvidence(evidence);
			isolatedStorageFile.PostInit();
			return isolatedStorageFile;
		}

		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.IsolatedStorageFilePermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Allowed=\"DomainIsolationByMachine\"/>\n</PermissionSet>\n")]
		public static IsolatedStorageFile GetMachineStoreForDomain()
		{
			IsolatedStorageScope isolatedStorageScope = IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly | IsolatedStorageScope.Machine;
			IsolatedStorageFile isolatedStorageFile = new IsolatedStorageFile(isolatedStorageScope);
			isolatedStorageFile._domainIdentity = IsolatedStorageFile.GetDomainIdentityFromEvidence(AppDomain.CurrentDomain.Evidence);
			Evidence evidence = Assembly.GetCallingAssembly().UnprotectedGetEvidence();
			isolatedStorageFile._fullEvidences = evidence;
			isolatedStorageFile._assemblyIdentity = IsolatedStorageFile.GetAssemblyIdentityFromEvidence(evidence);
			isolatedStorageFile.PostInit();
			return isolatedStorageFile;
		}

		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.IsolatedStorageFilePermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Allowed=\"ApplicationIsolationByUser\"/>\n</PermissionSet>\n")]
		public static IsolatedStorageFile GetUserStoreForApplication()
		{
			IsolatedStorageScope isolatedStorageScope = IsolatedStorageScope.User | IsolatedStorageScope.Application;
			IsolatedStorageFile isolatedStorageFile = new IsolatedStorageFile(isolatedStorageScope);
			isolatedStorageFile.InitStore(isolatedStorageScope, null);
			isolatedStorageFile._fullEvidences = Assembly.GetCallingAssembly().UnprotectedGetEvidence();
			isolatedStorageFile.PostInit();
			return isolatedStorageFile;
		}

		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.IsolatedStorageFilePermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Allowed=\"AssemblyIsolationByUser\"/>\n</PermissionSet>\n")]
		public static IsolatedStorageFile GetUserStoreForAssembly()
		{
			IsolatedStorageScope isolatedStorageScope = IsolatedStorageScope.User | IsolatedStorageScope.Assembly;
			IsolatedStorageFile isolatedStorageFile = new IsolatedStorageFile(isolatedStorageScope);
			Evidence evidence = Assembly.GetCallingAssembly().UnprotectedGetEvidence();
			isolatedStorageFile._fullEvidences = evidence;
			isolatedStorageFile._assemblyIdentity = IsolatedStorageFile.GetAssemblyIdentityFromEvidence(evidence);
			isolatedStorageFile.PostInit();
			return isolatedStorageFile;
		}

		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.IsolatedStorageFilePermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Allowed=\"DomainIsolationByUser\"/>\n</PermissionSet>\n")]
		public static IsolatedStorageFile GetUserStoreForDomain()
		{
			IsolatedStorageScope isolatedStorageScope = IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly;
			IsolatedStorageFile isolatedStorageFile = new IsolatedStorageFile(isolatedStorageScope);
			isolatedStorageFile._domainIdentity = IsolatedStorageFile.GetDomainIdentityFromEvidence(AppDomain.CurrentDomain.Evidence);
			Evidence evidence = Assembly.GetCallingAssembly().UnprotectedGetEvidence();
			isolatedStorageFile._fullEvidences = evidence;
			isolatedStorageFile._assemblyIdentity = IsolatedStorageFile.GetAssemblyIdentityFromEvidence(evidence);
			isolatedStorageFile.PostInit();
			return isolatedStorageFile;
		}

		public static void Remove(IsolatedStorageScope scope)
		{
			string isolatedStorageRoot = IsolatedStorageFile.GetIsolatedStorageRoot(scope);
			Directory.Delete(isolatedStorageRoot, true);
		}

		internal static string GetIsolatedStorageRoot(IsolatedStorageScope scope)
		{
			string text = null;
			if ((scope & IsolatedStorageScope.User) != IsolatedStorageScope.None)
			{
				if ((scope & IsolatedStorageScope.Roaming) != IsolatedStorageScope.None)
				{
					text = Environment.InternalGetFolderPath(Environment.SpecialFolder.LocalApplicationData);
				}
				else
				{
					text = Environment.InternalGetFolderPath(Environment.SpecialFolder.ApplicationData);
				}
			}
			else if ((scope & IsolatedStorageScope.Machine) != IsolatedStorageScope.None)
			{
				text = Environment.InternalGetFolderPath(Environment.SpecialFolder.CommonApplicationData);
			}
			if (text == null)
			{
				string text2 = Locale.GetText("Couldn't access storage location for '{0}'.");
				throw new IsolatedStorageException(string.Format(text2, scope));
			}
			return Path.Combine(text, ".isolated-storage");
		}

		private static void Demand(IsolatedStorageScope scope)
		{
			if (SecurityManager.SecurityEnabled)
			{
				new IsolatedStorageFilePermission(PermissionState.None)
				{
					UsageAllowed = IsolatedStorageFile.ScopeToContainment(scope)
				}.Demand();
			}
		}

		private static IsolatedStorageContainment ScopeToContainment(IsolatedStorageScope scope)
		{
			switch (scope)
			{
			case IsolatedStorageScope.User | IsolatedStorageScope.Assembly:
				return IsolatedStorageContainment.AssemblyIsolationByUser;
			default:
				switch (scope)
				{
				case IsolatedStorageScope.User | IsolatedStorageScope.Assembly | IsolatedStorageScope.Roaming:
					return IsolatedStorageContainment.AssemblyIsolationByRoamingUser;
				default:
					switch (scope)
					{
					case IsolatedStorageScope.Assembly | IsolatedStorageScope.Machine:
						return IsolatedStorageContainment.AssemblyIsolationByMachine;
					default:
						if (scope == (IsolatedStorageScope.User | IsolatedStorageScope.Application))
						{
							return IsolatedStorageContainment.ApplicationIsolationByUser;
						}
						if (scope == (IsolatedStorageScope.User | IsolatedStorageScope.Roaming | IsolatedStorageScope.Application))
						{
							return IsolatedStorageContainment.ApplicationIsolationByRoamingUser;
						}
						if (scope != (IsolatedStorageScope.Machine | IsolatedStorageScope.Application))
						{
							return IsolatedStorageContainment.UnrestrictedIsolatedStorage;
						}
						return IsolatedStorageContainment.ApplicationIsolationByMachine;
					case IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly | IsolatedStorageScope.Machine:
						return IsolatedStorageContainment.DomainIsolationByMachine;
					}
					break;
				case IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly | IsolatedStorageScope.Roaming:
					return IsolatedStorageContainment.DomainIsolationByRoamingUser;
				}
				break;
			case IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly:
				return IsolatedStorageContainment.DomainIsolationByUser;
			}
		}

		internal static ulong GetDirectorySize(DirectoryInfo di)
		{
			ulong num = 0UL;
			foreach (FileInfo fileInfo in di.GetFiles())
			{
				num += (ulong)fileInfo.Length;
			}
			foreach (DirectoryInfo directoryInfo in di.GetDirectories())
			{
				num += IsolatedStorageFile.GetDirectorySize(directoryInfo);
			}
			return num;
		}

		~IsolatedStorageFile()
		{
		}

		private void PostInit()
		{
			string text = IsolatedStorageFile.GetIsolatedStorageRoot(base.Scope);
			string text2;
			if (this._applicationIdentity != null)
			{
				text2 = string.Format("a{0}{1}", this.SeparatorInternal, this.GetNameFromIdentity(this._applicationIdentity));
			}
			else if (this._domainIdentity != null)
			{
				text2 = string.Format("d{0}{1}{0}{2}", this.SeparatorInternal, this.GetNameFromIdentity(this._domainIdentity), this.GetNameFromIdentity(this._assemblyIdentity));
			}
			else
			{
				if (this._assemblyIdentity == null)
				{
					throw new IsolatedStorageException(Locale.GetText("No code identity available."));
				}
				text2 = string.Format("d{0}none{0}{1}", this.SeparatorInternal, this.GetNameFromIdentity(this._assemblyIdentity));
			}
			text = Path.Combine(text, text2);
			this.directory = new DirectoryInfo(text);
			if (!this.directory.Exists)
			{
				try
				{
					this.directory.Create();
					this.SaveIdentities(text);
				}
				catch (IOException)
				{
				}
			}
		}

		[CLSCompliant(false)]
		public override ulong CurrentSize
		{
			get
			{
				return IsolatedStorageFile.GetDirectorySize(this.directory);
			}
		}

		[CLSCompliant(false)]
		public override ulong MaximumSize
		{
			get
			{
				if (!SecurityManager.SecurityEnabled)
				{
					return 9223372036854775807UL;
				}
				if (this._resolved)
				{
					return this._maxSize;
				}
				Evidence evidence;
				if (this._fullEvidences != null)
				{
					evidence = this._fullEvidences;
				}
				else
				{
					evidence = new Evidence();
					if (this._assemblyIdentity != null)
					{
						evidence.AddHost(this._assemblyIdentity);
					}
				}
				if (evidence.Count < 1)
				{
					throw new InvalidOperationException(Locale.GetText("Couldn't get the quota from the available evidences."));
				}
				PermissionSet permissionSet = null;
				PermissionSet permissionSet2 = SecurityManager.ResolvePolicy(evidence, null, null, null, out permissionSet);
				IsolatedStoragePermission permission = this.GetPermission(permissionSet2);
				if (permission == null)
				{
					if (!permissionSet2.IsUnrestricted())
					{
						throw new InvalidOperationException(Locale.GetText("No quota from the available evidences."));
					}
					this._maxSize = 9223372036854775807UL;
				}
				else
				{
					this._maxSize = (ulong)permission.UserQuota;
				}
				this._resolved = true;
				return this._maxSize;
			}
		}

		internal string Root
		{
			get
			{
				return this.directory.FullName;
			}
		}

		public void Close()
		{
		}

		public void CreateDirectory(string dir)
		{
			if (dir == null)
			{
				throw new ArgumentNullException("dir");
			}
			if (dir.IndexOfAny(Path.PathSeparatorChars) < 0)
			{
				if (this.directory.GetFiles(dir).Length > 0)
				{
					throw new IOException(Locale.GetText("Directory name already exists as a file."));
				}
				this.directory.CreateSubdirectory(dir);
			}
			else
			{
				string[] array = dir.Split(Path.PathSeparatorChars);
				DirectoryInfo directoryInfo = this.directory;
				for (int i = 0; i < array.Length; i++)
				{
					if (directoryInfo.GetFiles(array[i]).Length > 0)
					{
						throw new IOException(Locale.GetText("Part of the directory name already exists as a file."));
					}
					directoryInfo = directoryInfo.CreateSubdirectory(array[i]);
				}
			}
		}

		public void DeleteDirectory(string dir)
		{
			try
			{
				DirectoryInfo directoryInfo = this.directory.CreateSubdirectory(dir);
				directoryInfo.Delete();
			}
			catch
			{
				throw new IsolatedStorageException(Locale.GetText("Could not delete directory '{0}'", new object[] { dir }));
			}
		}

		public void DeleteFile(string file)
		{
			File.Delete(Path.Combine(this.directory.FullName, file));
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}

		public string[] GetDirectoryNames(string searchPattern)
		{
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			string directoryName = Path.GetDirectoryName(searchPattern);
			string fileName = Path.GetFileName(searchPattern);
			DirectoryInfo[] array;
			if (directoryName == null || directoryName.Length == 0)
			{
				array = this.directory.GetDirectories(searchPattern);
			}
			else
			{
				DirectoryInfo[] directories = this.directory.GetDirectories(directoryName);
				if (directories.Length != 1 || !(directories[0].Name == directoryName) || directories[0].FullName.IndexOf(this.directory.FullName) < 0)
				{
					throw new SecurityException();
				}
				array = directories[0].GetDirectories(fileName);
			}
			return this.GetNames(array);
		}

		private string[] GetNames(FileSystemInfo[] afsi)
		{
			string[] array = new string[afsi.Length];
			for (int num = 0; num != afsi.Length; num++)
			{
				array[num] = afsi[num].Name;
			}
			return array;
		}

		public string[] GetFileNames(string searchPattern)
		{
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			string directoryName = Path.GetDirectoryName(searchPattern);
			string fileName = Path.GetFileName(searchPattern);
			FileInfo[] array;
			if (directoryName == null || directoryName.Length == 0)
			{
				array = this.directory.GetFiles(searchPattern);
			}
			else
			{
				DirectoryInfo[] directories = this.directory.GetDirectories(directoryName);
				if (directories.Length != 1 || !(directories[0].Name == directoryName) || directories[0].FullName.IndexOf(this.directory.FullName) < 0)
				{
					throw new SecurityException();
				}
				array = directories[0].GetFiles(fileName);
			}
			return this.GetNames(array);
		}

		public override void Remove()
		{
			this.directory.Delete(true);
		}

		protected override IsolatedStoragePermission GetPermission(PermissionSet ps)
		{
			if (ps == null)
			{
				return null;
			}
			return (IsolatedStoragePermission)ps.GetPermission(typeof(IsolatedStorageFilePermission));
		}

		private string GetNameFromIdentity(object identity)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(identity.ToString());
			SHA1 sha = SHA1.Create();
			byte[] array = sha.ComputeHash(bytes, 0, bytes.Length);
			byte[] array2 = new byte[10];
			Buffer.BlockCopy(array, 0, array2, 0, array2.Length);
			return CryptoConvert.ToHex(array2);
		}

		private static object GetTypeFromEvidence(Evidence e, Type t)
		{
			foreach (object obj in e)
			{
				if (obj.GetType() == t)
				{
					return obj;
				}
			}
			return null;
		}

		internal static object GetAssemblyIdentityFromEvidence(Evidence e)
		{
			object obj = IsolatedStorageFile.GetTypeFromEvidence(e, typeof(Publisher));
			if (obj != null)
			{
				return obj;
			}
			obj = IsolatedStorageFile.GetTypeFromEvidence(e, typeof(StrongName));
			if (obj != null)
			{
				return obj;
			}
			return IsolatedStorageFile.GetTypeFromEvidence(e, typeof(Url));
		}

		internal static object GetDomainIdentityFromEvidence(Evidence e)
		{
			object typeFromEvidence = IsolatedStorageFile.GetTypeFromEvidence(e, typeof(ApplicationDirectory));
			if (typeFromEvidence != null)
			{
				return typeFromEvidence;
			}
			return IsolatedStorageFile.GetTypeFromEvidence(e, typeof(Url));
		}

		[PermissionSet(SecurityAction.Assert, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"SerializationFormatter\"/>\n</PermissionSet>\n")]
		private void SaveIdentities(string root)
		{
			IsolatedStorageFile.Identities identities = new IsolatedStorageFile.Identities(this._applicationIdentity, this._assemblyIdentity, this._domainIdentity);
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			IsolatedStorageFile.mutex.WaitOne();
			try
			{
				using (FileStream fileStream = File.Create(root + ".storage"))
				{
					binaryFormatter.Serialize(fileStream, identities);
				}
			}
			finally
			{
				IsolatedStorageFile.mutex.ReleaseMutex();
			}
		}

		private bool _resolved;

		private ulong _maxSize;

		private Evidence _fullEvidences;

		private static Mutex mutex = new Mutex();

		private DirectoryInfo directory;

		[Serializable]
		private struct Identities
		{
			public Identities(object application, object assembly, object domain)
			{
				this.Application = application;
				this.Assembly = assembly;
				this.Domain = domain;
			}

			public object Application;

			public object Assembly;

			public object Domain;
		}
	}
}
