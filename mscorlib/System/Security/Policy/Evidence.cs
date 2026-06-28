using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using Mono.Security.Authenticode;

namespace System.Security.Policy
{
	[MonoTODO("Serialization format not compatible with .NET")]
	[ComVisible(true)]
	[Serializable]
	public sealed class Evidence : IEnumerable, ICollection
	{
		public Evidence()
		{
		}

		public Evidence(Evidence evidence)
		{
			if (evidence != null)
			{
				this.Merge(evidence);
			}
		}

		public Evidence(object[] hostEvidence, object[] assemblyEvidence)
		{
			if (hostEvidence != null)
			{
				this.HostEvidenceList.AddRange(hostEvidence);
			}
			if (assemblyEvidence != null)
			{
				this.AssemblyEvidenceList.AddRange(assemblyEvidence);
			}
		}

		public int Count
		{
			get
			{
				int num = 0;
				if (this.hostEvidenceList != null)
				{
					num += this.hostEvidenceList.Count;
				}
				if (this.assemblyEvidenceList != null)
				{
					num += this.assemblyEvidenceList.Count;
				}
				return num;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public bool Locked
		{
			get
			{
				return this._locked;
			}
			[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"ControlEvidence\"/>\n</PermissionSet>\n")]
			set
			{
				this._locked = value;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this;
			}
		}

		internal ArrayList HostEvidenceList
		{
			get
			{
				if (this.hostEvidenceList == null)
				{
					this.hostEvidenceList = ArrayList.Synchronized(new ArrayList());
				}
				return this.hostEvidenceList;
			}
		}

		internal ArrayList AssemblyEvidenceList
		{
			get
			{
				if (this.assemblyEvidenceList == null)
				{
					this.assemblyEvidenceList = ArrayList.Synchronized(new ArrayList());
				}
				return this.assemblyEvidenceList;
			}
		}

		public void AddAssembly(object id)
		{
			this.AssemblyEvidenceList.Add(id);
			this._hashCode = 0;
		}

		public void AddHost(object id)
		{
			if (this._locked && SecurityManager.SecurityEnabled)
			{
				new SecurityPermission(SecurityPermissionFlag.ControlEvidence).Demand();
			}
			this.HostEvidenceList.Add(id);
			this._hashCode = 0;
		}

		[ComVisible(false)]
		public void Clear()
		{
			if (this.hostEvidenceList != null)
			{
				this.hostEvidenceList.Clear();
			}
			if (this.assemblyEvidenceList != null)
			{
				this.assemblyEvidenceList.Clear();
			}
			this._hashCode = 0;
		}

		public void CopyTo(Array array, int index)
		{
			int num = 0;
			if (this.hostEvidenceList != null)
			{
				num = this.hostEvidenceList.Count;
				if (num > 0)
				{
					this.hostEvidenceList.CopyTo(array, index);
				}
			}
			if (this.assemblyEvidenceList != null && this.assemblyEvidenceList.Count > 0)
			{
				this.assemblyEvidenceList.CopyTo(array, index + num);
			}
		}

		[ComVisible(false)]
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			Evidence evidence = obj as Evidence;
			if (evidence == null)
			{
				return false;
			}
			if (this.HostEvidenceList.Count != evidence.HostEvidenceList.Count)
			{
				return false;
			}
			if (this.AssemblyEvidenceList.Count != evidence.AssemblyEvidenceList.Count)
			{
				return false;
			}
			for (int i = 0; i < this.hostEvidenceList.Count; i++)
			{
				bool flag = false;
				int j = 0;
				while (j < evidence.hostEvidenceList.Count)
				{
					if (this.hostEvidenceList[i].Equals(evidence.hostEvidenceList[j]))
					{
						flag = true;
						break;
					}
					i++;
				}
				if (!flag)
				{
					return false;
				}
			}
			for (int k = 0; k < this.assemblyEvidenceList.Count; k++)
			{
				bool flag2 = false;
				int l = 0;
				while (l < evidence.assemblyEvidenceList.Count)
				{
					if (this.assemblyEvidenceList[k].Equals(evidence.assemblyEvidenceList[l]))
					{
						flag2 = true;
						break;
					}
					k++;
				}
				if (!flag2)
				{
					return false;
				}
			}
			return true;
		}

		public IEnumerator GetEnumerator()
		{
			IEnumerator enumerator = null;
			if (this.hostEvidenceList != null)
			{
				enumerator = this.hostEvidenceList.GetEnumerator();
			}
			IEnumerator enumerator2 = null;
			if (this.assemblyEvidenceList != null)
			{
				enumerator2 = this.assemblyEvidenceList.GetEnumerator();
			}
			return new Evidence.EvidenceEnumerator(enumerator, enumerator2);
		}

		public IEnumerator GetAssemblyEnumerator()
		{
			return this.AssemblyEvidenceList.GetEnumerator();
		}

		[ComVisible(false)]
		public override int GetHashCode()
		{
			if (this._hashCode == 0)
			{
				if (this.hostEvidenceList != null)
				{
					for (int i = 0; i < this.hostEvidenceList.Count; i++)
					{
						this._hashCode ^= this.hostEvidenceList[i].GetHashCode();
					}
				}
				if (this.assemblyEvidenceList != null)
				{
					for (int j = 0; j < this.assemblyEvidenceList.Count; j++)
					{
						this._hashCode ^= this.assemblyEvidenceList[j].GetHashCode();
					}
				}
			}
			return this._hashCode;
		}

		public IEnumerator GetHostEnumerator()
		{
			return this.HostEvidenceList.GetEnumerator();
		}

		public void Merge(Evidence evidence)
		{
			if (evidence != null && evidence.Count > 0)
			{
				if (evidence.hostEvidenceList != null)
				{
					foreach (object obj in evidence.hostEvidenceList)
					{
						this.AddHost(obj);
					}
				}
				if (evidence.assemblyEvidenceList != null)
				{
					foreach (object obj2 in evidence.assemblyEvidenceList)
					{
						this.AddAssembly(obj2);
					}
				}
				this._hashCode = 0;
			}
		}

		[ComVisible(false)]
		public void RemoveType(Type t)
		{
			for (int i = this.hostEvidenceList.Count; i >= 0; i--)
			{
				if (this.hostEvidenceList.GetType() == t)
				{
					this.hostEvidenceList.RemoveAt(i);
					this._hashCode = 0;
				}
			}
			for (int j = this.assemblyEvidenceList.Count; j >= 0; j--)
			{
				if (this.assemblyEvidenceList.GetType() == t)
				{
					this.assemblyEvidenceList.RemoveAt(j);
					this._hashCode = 0;
				}
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsAuthenticodePresent(Assembly a);

		[PermissionSet(SecurityAction.Assert, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Unrestricted=\"true\"/>\n</PermissionSet>\n")]
		internal static Evidence GetDefaultHostEvidence(Assembly a)
		{
			Evidence evidence = new Evidence();
			string escapedCodeBase = a.EscapedCodeBase;
			evidence.AddHost(Zone.CreateFromUrl(escapedCodeBase));
			evidence.AddHost(new Url(escapedCodeBase));
			evidence.AddHost(new Hash(a));
			if (string.Compare("FILE://", 0, escapedCodeBase, 0, 7, true, CultureInfo.InvariantCulture) != 0)
			{
				evidence.AddHost(Site.CreateFromUrl(escapedCodeBase));
			}
			AssemblyName assemblyName = a.UnprotectedGetName();
			byte[] publicKey = assemblyName.GetPublicKey();
			if (publicKey != null && publicKey.Length > 0)
			{
				StrongNamePublicKeyBlob strongNamePublicKeyBlob = new StrongNamePublicKeyBlob(publicKey);
				evidence.AddHost(new StrongName(strongNamePublicKeyBlob, assemblyName.Name, assemblyName.Version));
			}
			if (Evidence.IsAuthenticodePresent(a))
			{
				AuthenticodeDeformatter authenticodeDeformatter = new AuthenticodeDeformatter(a.Location);
				if (authenticodeDeformatter.SigningCertificate != null)
				{
					X509Certificate x509Certificate = new X509Certificate(authenticodeDeformatter.SigningCertificate.RawData);
					if (x509Certificate.GetHashCode() != 0)
					{
						evidence.AddHost(new Publisher(x509Certificate));
					}
				}
			}
			if (a.GlobalAssemblyCache)
			{
				evidence.AddHost(new GacInstalled());
			}
			AppDomainManager domainManager = AppDomain.CurrentDomain.DomainManager;
			if (domainManager != null && (domainManager.HostSecurityManager.Flags & HostSecurityManagerOptions.HostAssemblyEvidence) == HostSecurityManagerOptions.HostAssemblyEvidence)
			{
				evidence = domainManager.HostSecurityManager.ProvideAssemblyEvidence(a, evidence);
			}
			return evidence;
		}

		private bool _locked;

		private ArrayList hostEvidenceList;

		private ArrayList assemblyEvidenceList;

		private int _hashCode;

		private class EvidenceEnumerator : IEnumerator
		{
			public EvidenceEnumerator(IEnumerator hostenum, IEnumerator assemblyenum)
			{
				this.hostEnum = hostenum;
				this.assemblyEnum = assemblyenum;
				this.currentEnum = this.hostEnum;
			}

			public bool MoveNext()
			{
				if (this.currentEnum == null)
				{
					return false;
				}
				bool flag = this.currentEnum.MoveNext();
				if (!flag && this.hostEnum == this.currentEnum && this.assemblyEnum != null)
				{
					this.currentEnum = this.assemblyEnum;
					flag = this.assemblyEnum.MoveNext();
				}
				return flag;
			}

			public void Reset()
			{
				if (this.hostEnum != null)
				{
					this.hostEnum.Reset();
					this.currentEnum = this.hostEnum;
				}
				else
				{
					this.currentEnum = this.assemblyEnum;
				}
				if (this.assemblyEnum != null)
				{
					this.assemblyEnum.Reset();
				}
			}

			public object Current
			{
				get
				{
					return this.currentEnum.Current;
				}
			}

			private IEnumerator currentEnum;

			private IEnumerator hostEnum;

			private IEnumerator assemblyEnum;
		}
	}
}
