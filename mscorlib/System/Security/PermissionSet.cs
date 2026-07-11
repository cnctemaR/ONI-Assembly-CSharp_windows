using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;
using System.Threading;

namespace System.Security
{
	[MonoTODO("CAS support is experimental (and unsupported).")]
	[ComVisible(true)]
	[PermissionSet(SecurityAction.InheritanceDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.StrongNameIdentityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                PublicKeyBlob=\"002400000480000094000000060200000024000052534131000400000100010007D1FA57C4AED9F0A32E84AA0FAEFD0DE9E8FD6AEC8F87FB03766C834C99921EB23BE79AD9D5DCC1DD9AD236132102900B723CF980957FC4E177108FC607774F29E8320E92EA05ECE4E821C0A5EFE8F1645C4C0C93C1AB99285D622CAA652C1DFAD63D745D6F2DE5F17E5EAF0FC4963D261C8A12436518206DC093344D5AD293\"/>\n</PermissionSet>\n")]
	[Serializable]
	public class PermissionSet : IEnumerable, ICollection, IDeserializationCallback, ISecurityEncodable, IStackWalk
	{
		internal PermissionSet()
		{
			this.list = new ArrayList();
		}

		public PermissionSet(PermissionState state)
			: this()
		{
			this.state = CodeAccessPermission.CheckPermissionState(state, true);
		}

		public PermissionSet(PermissionSet permSet)
			: this()
		{
			if (permSet != null)
			{
				this.state = permSet.state;
				foreach (object obj in permSet.list)
				{
					IPermission permission = (IPermission)obj;
					this.list.Add(permission);
				}
			}
		}

		internal PermissionSet(string xml)
			: this()
		{
			this.state = PermissionState.None;
			if (xml != null)
			{
				SecurityElement securityElement = SecurityElement.FromString(xml);
				this.FromXml(securityElement);
			}
		}

		internal PermissionSet(IPermission perm)
			: this()
		{
			if (perm != null)
			{
				this.list.Add(perm);
			}
		}

		[MonoTODO("may not be required")]
		void IDeserializationCallback.OnDeserialization(object sender)
		{
		}

		public IPermission AddPermission(IPermission perm)
		{
			if (perm == null || this._readOnly)
			{
				return perm;
			}
			if (this.state == PermissionState.Unrestricted)
			{
				return (IPermission)Activator.CreateInstance(perm.GetType(), PermissionSet.psUnrestricted);
			}
			IPermission permission = this.RemovePermission(perm.GetType());
			if (permission != null)
			{
				perm = perm.Union(permission);
			}
			this.list.Add(perm);
			return perm;
		}

		[MonoTODO("CAS support is experimental (and unsupported). Imperative mode is not implemented.")]
		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\n               version=\"1\">\n   <IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\n                version=\"1\"\n                Flags=\"Assertion\"/>\n</PermissionSet>\n")]
		public void Assert()
		{
			int num = this.Count;
			foreach (object obj in this.list)
			{
				IPermission permission = (IPermission)obj;
				if (permission is IStackWalk)
				{
					if (!SecurityManager.IsGranted(permission))
					{
						return;
					}
				}
				else
				{
					num--;
				}
			}
			if (SecurityManager.SecurityEnabled && num > 0)
			{
				throw new NotSupportedException("Currently only declarative Assert are supported.");
			}
		}

		internal void Clear()
		{
			this.list.Clear();
		}

		public virtual PermissionSet Copy()
		{
			return new PermissionSet(this);
		}

		public virtual void CopyTo(Array array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (this.list.Count > 0)
			{
				if (array.Rank > 1)
				{
					throw new ArgumentException(Locale.GetText("Array has more than one dimension"));
				}
				if (index < 0 || index >= array.Length)
				{
					throw new IndexOutOfRangeException("index");
				}
				this.list.CopyTo(array, index);
			}
		}

		public void Demand()
		{
		}

		internal void CasOnlyDemand(int skip)
		{
			Assembly assembly = null;
			AppDomain appDomain = null;
			if (this._ignored == null)
			{
				this._ignored = new bool[this.list.Count];
			}
			ArrayList stack = SecurityFrame.GetStack(skip);
			if (stack != null && stack.Count > 0)
			{
				SecurityFrame securityFrame = (SecurityFrame)stack[0];
				assembly = securityFrame.Assembly;
				appDomain = securityFrame.Domain;
				foreach (object obj in stack)
				{
					SecurityFrame securityFrame2 = (SecurityFrame)obj;
					if (this.ProcessFrame(securityFrame2, ref assembly, ref appDomain) && this.AllIgnored())
					{
						return;
					}
				}
				SecurityFrame securityFrame3 = (SecurityFrame)stack[stack.Count - 1];
				this.CheckAssembly(assembly, securityFrame3);
				this.CheckAppDomain(appDomain, securityFrame3);
			}
			CompressedStack compressedStack = Thread.CurrentThread.GetCompressedStack();
			if (compressedStack != null && !compressedStack.IsEmpty())
			{
				foreach (object obj2 in compressedStack.List)
				{
					SecurityFrame securityFrame4 = (SecurityFrame)obj2;
					if (this.ProcessFrame(securityFrame4, ref assembly, ref appDomain) && this.AllIgnored())
					{
						break;
					}
				}
			}
		}

		[MonoTODO("CAS support is experimental (and unsupported). Imperative mode is not implemented.")]
		public void Deny()
		{
			if (!SecurityManager.SecurityEnabled)
			{
				return;
			}
			foreach (object obj in this.list)
			{
				IPermission permission = (IPermission)obj;
				if (permission is IStackWalk)
				{
					throw new NotSupportedException("Currently only declarative Deny are supported.");
				}
			}
		}

		public virtual void FromXml(SecurityElement et)
		{
			if (et == null)
			{
				throw new ArgumentNullException("et");
			}
			if (et.Tag != "PermissionSet")
			{
				string text = string.Format("Invalid tag {0} expected {1}", et.Tag, "PermissionSet");
				throw new ArgumentException(text, "et");
			}
			this.list.Clear();
			if (CodeAccessPermission.IsUnrestricted(et))
			{
				this.state = PermissionState.Unrestricted;
				return;
			}
			this.state = PermissionState.None;
			if (et.Children != null)
			{
				foreach (object obj in et.Children)
				{
					SecurityElement securityElement = (SecurityElement)obj;
					string text2 = securityElement.Attribute("class");
					if (text2 == null)
					{
						throw new ArgumentException(Locale.GetText("No permission class is specified."));
					}
					if (this.Resolver != null)
					{
						text2 = this.Resolver.ResolveClassName(text2);
					}
					this.list.Add(PermissionBuilder.Create(text2, securityElement));
				}
			}
		}

		public IEnumerator GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		public bool IsSubsetOf(PermissionSet target)
		{
			if (target == null || target.IsEmpty())
			{
				return this.IsEmpty();
			}
			if (target.IsUnrestricted())
			{
				return true;
			}
			if (this.IsUnrestricted())
			{
				return false;
			}
			if (this.IsUnrestricted() && (target == null || !target.IsUnrestricted()))
			{
				return false;
			}
			foreach (object obj in this.list)
			{
				IPermission permission = (IPermission)obj;
				Type type = permission.GetType();
				IPermission permission2;
				if (target.IsUnrestricted() && permission is CodeAccessPermission && permission is IUnrestrictedPermission)
				{
					permission2 = (IPermission)Activator.CreateInstance(type, PermissionSet.psUnrestricted);
				}
				else
				{
					permission2 = target.GetPermission(type);
				}
				if (!permission.IsSubsetOf(permission2))
				{
					return false;
				}
			}
			return true;
		}

		[MonoTODO("CAS support is experimental (and unsupported). Imperative mode is not implemented.")]
		public void PermitOnly()
		{
			if (!SecurityManager.SecurityEnabled)
			{
				return;
			}
			foreach (object obj in this.list)
			{
				IPermission permission = (IPermission)obj;
				if (permission is IStackWalk)
				{
					throw new NotSupportedException("Currently only declarative Deny are supported.");
				}
			}
		}

		public bool ContainsNonCodeAccessPermissions()
		{
			if (this.list.Count > 0)
			{
				foreach (object obj in this.list)
				{
					IPermission permission = (IPermission)obj;
					if (!permission.GetType().IsSubclassOf(typeof(CodeAccessPermission)))
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		public static byte[] ConvertPermissionSet(string inFormat, byte[] inData, string outFormat)
		{
			if (inFormat == null)
			{
				throw new ArgumentNullException("inFormat");
			}
			if (outFormat == null)
			{
				throw new ArgumentNullException("outFormat");
			}
			if (inData == null)
			{
				return null;
			}
			if (inFormat == outFormat)
			{
				return inData;
			}
			PermissionSet permissionSet = null;
			if (inFormat == "BINARY")
			{
				if (outFormat.StartsWith("XML"))
				{
					using (MemoryStream memoryStream = new MemoryStream(inData))
					{
						BinaryFormatter binaryFormatter = new BinaryFormatter();
						permissionSet = (PermissionSet)binaryFormatter.Deserialize(memoryStream);
						memoryStream.Close();
					}
					string text = permissionSet.ToString();
					if (outFormat != null)
					{
						if (PermissionSet.<>f__switch$map2B == null)
						{
							PermissionSet.<>f__switch$map2B = new Dictionary<string, int>(3)
							{
								{ "XML", 0 },
								{ "XMLASCII", 0 },
								{ "XMLUNICODE", 1 }
							};
						}
						int num;
						if (PermissionSet.<>f__switch$map2B.TryGetValue(outFormat, out num))
						{
							if (num == 0)
							{
								return Encoding.ASCII.GetBytes(text);
							}
							if (num == 1)
							{
								return Encoding.Unicode.GetBytes(text);
							}
						}
					}
				}
			}
			else
			{
				if (!inFormat.StartsWith("XML"))
				{
					return null;
				}
				if (outFormat == "BINARY")
				{
					string text2 = null;
					if (inFormat != null)
					{
						if (PermissionSet.<>f__switch$map2C == null)
						{
							PermissionSet.<>f__switch$map2C = new Dictionary<string, int>(3)
							{
								{ "XML", 0 },
								{ "XMLASCII", 0 },
								{ "XMLUNICODE", 1 }
							};
						}
						int num;
						if (PermissionSet.<>f__switch$map2C.TryGetValue(inFormat, out num))
						{
							if (num != 0)
							{
								if (num == 1)
								{
									text2 = Encoding.Unicode.GetString(inData);
								}
							}
							else
							{
								text2 = Encoding.ASCII.GetString(inData);
							}
						}
					}
					if (text2 != null)
					{
						permissionSet = new PermissionSet(PermissionState.None);
						permissionSet.FromXml(SecurityElement.FromString(text2));
						MemoryStream memoryStream2 = new MemoryStream();
						BinaryFormatter binaryFormatter2 = new BinaryFormatter();
						binaryFormatter2.Serialize(memoryStream2, permissionSet);
						memoryStream2.Close();
						return memoryStream2.ToArray();
					}
				}
				else if (outFormat.StartsWith("XML"))
				{
					string text3 = string.Format(Locale.GetText("Can't convert from {0} to {1}"), inFormat, outFormat);
					throw new XmlSyntaxException(text3);
				}
			}
			throw new SerializationException(string.Format(Locale.GetText("Unknown output format {0}."), outFormat));
		}

		public IPermission GetPermission(Type permClass)
		{
			if (permClass == null || this.list.Count == 0)
			{
				return null;
			}
			foreach (object obj in this.list)
			{
				if (obj != null && obj.GetType().Equals(permClass))
				{
					return (IPermission)obj;
				}
			}
			return null;
		}

		public PermissionSet Intersect(PermissionSet other)
		{
			if (other == null || other.IsEmpty() || this.IsEmpty())
			{
				return null;
			}
			PermissionState permissionState = PermissionState.None;
			if (this.IsUnrestricted() && other.IsUnrestricted())
			{
				permissionState = PermissionState.Unrestricted;
			}
			PermissionSet permissionSet;
			if (permissionState == PermissionState.Unrestricted)
			{
				permissionSet = new PermissionSet(permissionState);
			}
			else if (this.IsUnrestricted())
			{
				permissionSet = other.Copy();
			}
			else if (other.IsUnrestricted())
			{
				permissionSet = this.Copy();
			}
			else
			{
				permissionSet = new PermissionSet(permissionState);
				this.InternalIntersect(permissionSet, this, other, false);
			}
			return permissionSet;
		}

		internal void InternalIntersect(PermissionSet intersect, PermissionSet a, PermissionSet b, bool unrestricted)
		{
			foreach (object obj in b.list)
			{
				IPermission permission = (IPermission)obj;
				IPermission permission2 = a.GetPermission(permission.GetType());
				if (permission2 != null)
				{
					intersect.AddPermission(permission.Intersect(permission2));
				}
				else if (unrestricted)
				{
					intersect.AddPermission(permission);
				}
			}
		}

		public bool IsEmpty()
		{
			if (this.state == PermissionState.Unrestricted)
			{
				return false;
			}
			if (this.list == null || this.list.Count == 0)
			{
				return true;
			}
			foreach (object obj in this.list)
			{
				IPermission permission = (IPermission)obj;
				if (!permission.IsSubsetOf(null))
				{
					return false;
				}
			}
			return true;
		}

		public bool IsUnrestricted()
		{
			return this.state == PermissionState.Unrestricted;
		}

		public IPermission RemovePermission(Type permClass)
		{
			if (permClass == null || this._readOnly)
			{
				return null;
			}
			foreach (object obj in this.list)
			{
				if (obj.GetType().Equals(permClass))
				{
					this.list.Remove(obj);
					return (IPermission)obj;
				}
			}
			return null;
		}

		public IPermission SetPermission(IPermission perm)
		{
			if (perm == null || this._readOnly)
			{
				return perm;
			}
			IUnrestrictedPermission unrestrictedPermission = perm as IUnrestrictedPermission;
			if (unrestrictedPermission == null)
			{
				this.state = PermissionState.None;
			}
			else
			{
				this.state = ((!unrestrictedPermission.IsUnrestricted()) ? PermissionState.None : this.state);
			}
			this.RemovePermission(perm.GetType());
			this.list.Add(perm);
			return perm;
		}

		public override string ToString()
		{
			return this.ToXml().ToString();
		}

		public virtual SecurityElement ToXml()
		{
			SecurityElement securityElement = new SecurityElement("PermissionSet");
			securityElement.AddAttribute("class", base.GetType().FullName);
			securityElement.AddAttribute("version", 1.ToString());
			if (this.state == PermissionState.Unrestricted)
			{
				securityElement.AddAttribute("Unrestricted", "true");
			}
			foreach (object obj in this.list)
			{
				IPermission permission = (IPermission)obj;
				securityElement.AddChild(permission.ToXml());
			}
			return securityElement;
		}

		public PermissionSet Union(PermissionSet other)
		{
			if (other == null)
			{
				return this.Copy();
			}
			PermissionSet permissionSet = null;
			if (this.IsUnrestricted() || other.IsUnrestricted())
			{
				return new PermissionSet(PermissionState.Unrestricted);
			}
			permissionSet = this.Copy();
			foreach (object obj in other.list)
			{
				IPermission permission = (IPermission)obj;
				permissionSet.AddPermission(permission);
			}
			return permissionSet;
		}

		public virtual int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		public virtual bool IsSynchronized
		{
			get
			{
				return this.list.IsSynchronized;
			}
		}

		public virtual bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public virtual object SyncRoot
		{
			get
			{
				return this;
			}
		}

		internal bool DeclarativeSecurity
		{
			get
			{
				return this._declsec;
			}
			set
			{
				this._declsec = value;
			}
		}

		[ComVisible(false)]
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			PermissionSet permissionSet = obj as PermissionSet;
			if (permissionSet == null)
			{
				return false;
			}
			if (this.state != permissionSet.state)
			{
				return false;
			}
			if (this.list.Count != permissionSet.Count)
			{
				return false;
			}
			for (int i = 0; i < this.list.Count; i++)
			{
				bool flag = false;
				int num = 0;
				while (i < permissionSet.list.Count)
				{
					if (this.list[i].Equals(permissionSet.list[num]))
					{
						flag = true;
						break;
					}
					num++;
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}

		[ComVisible(false)]
		public override int GetHashCode()
		{
			return (this.list.Count != 0) ? base.GetHashCode() : ((int)this.state);
		}

		public static void RevertAssert()
		{
			CodeAccessPermission.RevertAssert();
		}

		internal PolicyLevel Resolver
		{
			get
			{
				return this._policyLevel;
			}
			set
			{
				this._policyLevel = value;
			}
		}

		internal void SetReadOnly(bool value)
		{
			this._readOnly = value;
		}

		private bool AllIgnored()
		{
			if (this._ignored == null)
			{
				throw new NotSupportedException("bad bad bad");
			}
			for (int i = 0; i < this._ignored.Length; i++)
			{
				if (!this._ignored[i])
				{
					return false;
				}
			}
			return true;
		}

		internal bool ProcessFrame(SecurityFrame frame, ref Assembly current, ref AppDomain domain)
		{
			if (this.IsUnrestricted())
			{
				if (frame.Deny != null)
				{
					CodeAccessPermission.ThrowSecurityException(this, "Deny", frame, SecurityAction.Demand, null);
				}
				else if (frame.PermitOnly != null && !frame.PermitOnly.IsUnrestricted())
				{
					CodeAccessPermission.ThrowSecurityException(this, "PermitOnly", frame, SecurityAction.Demand, null);
				}
			}
			if (frame.HasStackModifiers)
			{
				for (int i = 0; i < this.list.Count; i++)
				{
					CodeAccessPermission codeAccessPermission = (CodeAccessPermission)this.list[i];
					if (codeAccessPermission.ProcessFrame(frame))
					{
						this._ignored[i] = true;
						if (this.AllIgnored())
						{
							return true;
						}
					}
				}
			}
			if (frame.Assembly != current)
			{
				this.CheckAssembly(current, frame);
				current = frame.Assembly;
			}
			if (frame.Domain != domain)
			{
				this.CheckAppDomain(domain, frame);
				domain = frame.Domain;
			}
			return false;
		}

		internal void CheckAssembly(Assembly a, SecurityFrame frame)
		{
			IPermission permission = SecurityManager.CheckPermissionSet(a, this, false);
			if (permission != null)
			{
				CodeAccessPermission.ThrowSecurityException(this, "Demand failed assembly permissions checks.", frame, SecurityAction.Demand, permission);
			}
		}

		internal void CheckAppDomain(AppDomain domain, SecurityFrame frame)
		{
			IPermission permission = SecurityManager.CheckPermissionSet(domain, this);
			if (permission != null)
			{
				CodeAccessPermission.ThrowSecurityException(this, "Demand failed appdomain permissions checks.", frame, SecurityAction.Demand, permission);
			}
		}

		internal static PermissionSet CreateFromBinaryFormat(byte[] data)
		{
			if (data == null || data[0] != 46 || data.Length < 2)
			{
				string text = Locale.GetText("Invalid data in 2.0 metadata format.");
				throw new SecurityException(text);
			}
			int num = 1;
			int num2 = PermissionSet.ReadEncodedInt(data, ref num);
			PermissionSet permissionSet = new PermissionSet(PermissionState.None);
			for (int i = 0; i < num2; i++)
			{
				IPermission permission = PermissionSet.ProcessAttribute(data, ref num);
				if (permission == null)
				{
					string text2 = Locale.GetText("Unsupported data found in 2.0 metadata format.");
					throw new SecurityException(text2);
				}
				permissionSet.AddPermission(permission);
			}
			return permissionSet;
		}

		internal static int ReadEncodedInt(byte[] data, ref int position)
		{
			int num;
			if ((data[position] & 128) == 0)
			{
				num = (int)data[position];
				position++;
			}
			else if ((data[position] & 64) == 0)
			{
				num = ((int)(data[position] & 63) << 8) | (int)data[position + 1];
				position += 2;
			}
			else
			{
				num = ((int)(data[position] & 31) << 24) | ((int)data[position + 1] << 16) | ((int)data[position + 2] << 8) | (int)data[position + 3];
				position += 4;
			}
			return num;
		}

		internal static IPermission ProcessAttribute(byte[] data, ref int position)
		{
			int num = PermissionSet.ReadEncodedInt(data, ref position);
			string @string = Encoding.UTF8.GetString(data, position, num);
			position += num;
			Type type = Type.GetType(@string);
			SecurityAttribute securityAttribute = Activator.CreateInstance(type, PermissionSet.action) as SecurityAttribute;
			if (securityAttribute == null)
			{
				return null;
			}
			PermissionSet.ReadEncodedInt(data, ref position);
			int num2 = PermissionSet.ReadEncodedInt(data, ref position);
			for (int i = 0; i < num2; i++)
			{
				byte b = data[position++];
				bool flag;
				if (b != 83)
				{
					if (b != 84)
					{
						return null;
					}
					flag = true;
				}
				else
				{
					flag = false;
				}
				bool flag2 = false;
				byte b2 = data[position++];
				if (b2 == 29)
				{
					flag2 = true;
					b2 = data[position++];
				}
				int num3 = PermissionSet.ReadEncodedInt(data, ref position);
				string string2 = Encoding.UTF8.GetString(data, position, num3);
				position += num3;
				int num4 = 1;
				if (flag2)
				{
					num4 = BitConverter.ToInt32(data, position);
					position += 4;
				}
				object[] array = null;
				for (int j = 0; j < num4; j++)
				{
					if (flag2)
					{
					}
					b = b2;
					object obj;
					switch (b)
					{
					case 2:
						obj = Convert.ToBoolean(data[position++]);
						break;
					case 3:
						obj = Convert.ToChar(data[position]);
						position += 2;
						break;
					case 4:
						obj = Convert.ToSByte(data[position++]);
						break;
					case 5:
						obj = Convert.ToByte(data[position++]);
						break;
					case 6:
						obj = Convert.ToInt16(data[position]);
						position += 2;
						break;
					case 7:
						obj = Convert.ToUInt16(data[position]);
						position += 2;
						break;
					case 8:
						obj = Convert.ToInt32(data[position]);
						position += 4;
						break;
					case 9:
						obj = Convert.ToUInt32(data[position]);
						position += 4;
						break;
					case 10:
						obj = Convert.ToInt64(data[position]);
						position += 8;
						break;
					case 11:
						obj = Convert.ToUInt64(data[position]);
						position += 8;
						break;
					case 12:
						obj = Convert.ToSingle(data[position]);
						position += 4;
						break;
					case 13:
						obj = Convert.ToDouble(data[position]);
						position += 8;
						break;
					case 14:
					{
						string text = null;
						if (data[position] != 255)
						{
							int num5 = PermissionSet.ReadEncodedInt(data, ref position);
							text = Encoding.UTF8.GetString(data, position, num5);
							position += num5;
						}
						else
						{
							position++;
						}
						obj = text;
						break;
					}
					default:
					{
						if (b != 80)
						{
							return null;
						}
						int num6 = PermissionSet.ReadEncodedInt(data, ref position);
						obj = Type.GetType(Encoding.UTF8.GetString(data, position, num6));
						position += num6;
						break;
					}
					}
					if (flag)
					{
						PropertyInfo property = type.GetProperty(string2);
						property.SetValue(securityAttribute, obj, array);
					}
					else
					{
						FieldInfo field = type.GetField(string2);
						field.SetValue(securityAttribute, obj);
					}
				}
			}
			return securityAttribute.CreatePermission();
		}

		private const string tagName = "PermissionSet";

		private const int version = 1;

		private static object[] psUnrestricted = new object[] { PermissionState.Unrestricted };

		private PermissionState state;

		private ArrayList list;

		private PolicyLevel _policyLevel;

		private bool _declsec;

		private bool _readOnly;

		private bool[] _ignored;

		private static object[] action = new object[] { (SecurityAction)0 };
	}
}
