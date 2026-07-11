using System;
using System.Diagnostics;
using System.IO;
using Klei;
using Newtonsoft.Json;

namespace KMod
{
	[JsonObject(MemberSerialization.Fields)]
	[DebuggerDisplay("{title}")]
	public struct Label
	{
		[JsonIgnore]
		private string distribution_platform_name
		{
			get
			{
				return this.distribution_platform.ToString();
			}
		}

		[JsonIgnore]
		public string install_path
		{
			get
			{
				return FSUtil.Normalize(Path.Combine(Path.Combine(Manager.GetDirectory(), this.distribution_platform_name), this.id));
			}
		}

		public override string ToString()
		{
			return this.title;
		}

		public bool Match(Label rhs)
		{
			return this.id == rhs.id && this.distribution_platform == rhs.distribution_platform;
		}

		public Label.DistributionPlatform distribution_platform;

		public string id;

		public ulong version;

		public string title;

		public enum DistributionPlatform
		{
			Local,
			Steam,
			Epic,
			Rail,
			Dev
		}
	}
}
