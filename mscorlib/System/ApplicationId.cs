using System;
using System.Runtime.InteropServices;
using System.Text;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public sealed class ApplicationId
	{
		public ApplicationId(byte[] publicKeyToken, string name, Version version, string processorArchitecture, string culture)
		{
			if (publicKeyToken == null)
			{
				throw new ArgumentNullException("publicKeyToken");
			}
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (version == null)
			{
				throw new ArgumentNullException("version");
			}
			this._token = (byte[])publicKeyToken.Clone();
			this._name = name;
			this._version = version;
			this._proc = processorArchitecture;
			this._culture = culture;
		}

		public string Culture
		{
			get
			{
				return this._culture;
			}
		}

		public string Name
		{
			get
			{
				return this._name;
			}
		}

		public string ProcessorArchitecture
		{
			get
			{
				return this._proc;
			}
		}

		public byte[] PublicKeyToken
		{
			get
			{
				return (byte[])this._token.Clone();
			}
		}

		public Version Version
		{
			get
			{
				return this._version;
			}
		}

		public ApplicationId Copy()
		{
			return new ApplicationId(this._token, this._name, this._version, this._proc, this._culture);
		}

		public override bool Equals(object o)
		{
			if (o == null)
			{
				return false;
			}
			ApplicationId applicationId = o as ApplicationId;
			if (applicationId == null)
			{
				return false;
			}
			if (this._name != applicationId._name)
			{
				return false;
			}
			if (this._proc != applicationId._proc)
			{
				return false;
			}
			if (this._culture != applicationId._culture)
			{
				return false;
			}
			if (!this._version.Equals(applicationId._version))
			{
				return false;
			}
			if (this._token.Length != applicationId._token.Length)
			{
				return false;
			}
			for (int i = 0; i < this._token.Length; i++)
			{
				if (this._token[i] != applicationId._token[i])
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int num = this._name.GetHashCode() ^ this._version.GetHashCode();
			for (int i = 0; i < this._token.Length; i++)
			{
				num ^= (int)this._token[i];
			}
			return num;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this._name);
			if (this._culture != null)
			{
				stringBuilder.AppendFormat(", culture=\"{0}\"", this._culture);
			}
			stringBuilder.AppendFormat(", version=\"{0}\", publicKeyToken=\"", this._version);
			for (int i = 0; i < this._token.Length; i++)
			{
				stringBuilder.Append(this._token[i].ToString("X2"));
			}
			if (this._proc != null)
			{
				stringBuilder.AppendFormat("\", processorArchitecture =\"{0}\"", this._proc);
			}
			else
			{
				stringBuilder.Append("\"");
			}
			return stringBuilder.ToString();
		}

		private byte[] _token;

		private string _name;

		private Version _version;

		private string _proc;

		private string _culture;
	}
}
