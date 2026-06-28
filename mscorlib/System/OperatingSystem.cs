using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public sealed class OperatingSystem : ICloneable, ISerializable
	{
		public OperatingSystem(PlatformID platform, Version version)
		{
			if (version == null)
			{
				throw new ArgumentNullException("version");
			}
			this._platform = platform;
			this._version = version;
		}

		public PlatformID Platform
		{
			get
			{
				return this._platform;
			}
		}

		public Version Version
		{
			get
			{
				return this._version;
			}
		}

		public string ServicePack
		{
			get
			{
				return this._servicePack;
			}
		}

		public string VersionString
		{
			get
			{
				return this.ToString();
			}
		}

		public object Clone()
		{
			return new OperatingSystem(this._platform, this._version);
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("_platform", this._platform);
			info.AddValue("_version", this._version);
			info.AddValue("_servicePack", this._servicePack);
		}

		public override string ToString()
		{
			int platform = (int)this._platform;
			string text;
			switch (platform)
			{
			case 0:
				text = "Microsoft Win32S";
				goto IL_0096;
			case 1:
				text = "Microsoft Windows 98";
				goto IL_0096;
			case 2:
				text = "Microsoft Windows NT";
				goto IL_0096;
			case 3:
				text = "Microsoft Windows CE";
				goto IL_0096;
			case 4:
				break;
			case 5:
				text = "XBox";
				goto IL_0096;
			case 6:
				text = "OSX";
				goto IL_0096;
			default:
				if (platform != 128)
				{
					text = Locale.GetText("<unknown>");
					goto IL_0096;
				}
				break;
			}
			text = "Unix";
			IL_0096:
			return text + " " + this._version.ToString();
		}

		private PlatformID _platform;

		private Version _version;

		private string _servicePack = string.Empty;
	}
}
