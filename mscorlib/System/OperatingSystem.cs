using System;
using System.Runtime.Serialization;
using System.Security;

namespace System
{
	[Serializable]
	public sealed class OperatingSystem : ISerializable, ICloneable
	{
		public OperatingSystem(PlatformID platform, Version version)
			: this(platform, version, null)
		{
		}

		internal OperatingSystem(PlatformID platform, Version version, string servicePack)
		{
			if (platform < PlatformID.Win32S || platform > PlatformID.MacOSX)
			{
				throw new ArgumentOutOfRangeException("platform", platform, SR.Format("Illegal enum value: {0}.", platform));
			}
			if (version == null)
			{
				throw new ArgumentNullException("version");
			}
			this._platform = platform;
			this._version = version;
			this._servicePack = servicePack;
		}

		[SecurityCritical]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new PlatformNotSupportedException();
		}

		public PlatformID Platform
		{
			get
			{
				return this._platform;
			}
		}

		public string ServicePack
		{
			get
			{
				return this._servicePack ?? string.Empty;
			}
		}

		public Version Version
		{
			get
			{
				return this._version;
			}
		}

		public object Clone()
		{
			return new OperatingSystem(this._platform, this._version, this._servicePack);
		}

		public override string ToString()
		{
			return this.VersionString;
		}

		public string VersionString
		{
			get
			{
				if (this._versionString == null)
				{
					string text;
					switch (this._platform)
					{
					case PlatformID.Win32S:
						text = "Microsoft Win32S ";
						break;
					case PlatformID.Win32Windows:
						text = ((this._version.Major > 4 || (this._version.Major == 4 && this._version.Minor > 0)) ? "Microsoft Windows 98 " : "Microsoft Windows 95 ");
						break;
					case PlatformID.Win32NT:
						text = "Microsoft Windows NT ";
						break;
					case PlatformID.WinCE:
						text = "Microsoft Windows CE ";
						break;
					case PlatformID.Unix:
						text = "Unix ";
						break;
					case PlatformID.Xbox:
						text = "Xbox ";
						break;
					case PlatformID.MacOSX:
						text = "Mac OS X ";
						break;
					default:
						text = "<unknown> ";
						break;
					}
					this._versionString = (string.IsNullOrEmpty(this._servicePack) ? (text + this._version.ToString()) : (text + this._version.ToString(3) + " " + this._servicePack));
				}
				return this._versionString;
			}
		}

		private readonly Version _version;

		private readonly PlatformID _platform;

		private readonly string _servicePack;

		private string _versionString;
	}
}
