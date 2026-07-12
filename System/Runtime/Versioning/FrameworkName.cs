using System;
using System.Text;

namespace System.Runtime.Versioning
{
	[Serializable]
	public sealed class FrameworkName : IEquatable<FrameworkName>
	{
		public string Identifier
		{
			get
			{
				return this.m_identifier;
			}
		}

		public Version Version
		{
			get
			{
				return this.m_version;
			}
		}

		public string Profile
		{
			get
			{
				return this.m_profile;
			}
		}

		public string FullName
		{
			get
			{
				if (this.m_fullName == null)
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append(this.Identifier);
					stringBuilder.Append(',');
					stringBuilder.Append("Version").Append('=');
					stringBuilder.Append('v');
					stringBuilder.Append(this.Version);
					if (!string.IsNullOrEmpty(this.Profile))
					{
						stringBuilder.Append(',');
						stringBuilder.Append("Profile").Append('=');
						stringBuilder.Append(this.Profile);
					}
					this.m_fullName = stringBuilder.ToString();
				}
				return this.m_fullName;
			}
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as FrameworkName);
		}

		public bool Equals(FrameworkName other)
		{
			return other != null && (this.Identifier == other.Identifier && this.Version == other.Version) && this.Profile == other.Profile;
		}

		public override int GetHashCode()
		{
			return this.Identifier.GetHashCode() ^ this.Version.GetHashCode() ^ this.Profile.GetHashCode();
		}

		public override string ToString()
		{
			return this.FullName;
		}

		public FrameworkName(string identifier, Version version)
			: this(identifier, version, null)
		{
		}

		public FrameworkName(string identifier, Version version, string profile)
		{
			if (identifier == null)
			{
				throw new ArgumentNullException("identifier");
			}
			if (identifier.Trim().Length == 0)
			{
				throw new ArgumentException(SR.GetString("The parameter '{0}' cannot be an empty string.", new object[] { "identifier" }), "identifier");
			}
			if (version == null)
			{
				throw new ArgumentNullException("version");
			}
			this.m_identifier = identifier.Trim();
			this.m_version = (Version)version.Clone();
			this.m_profile = ((profile == null) ? string.Empty : profile.Trim());
		}

		public FrameworkName(string frameworkName)
		{
			if (frameworkName == null)
			{
				throw new ArgumentNullException("frameworkName");
			}
			if (frameworkName.Length == 0)
			{
				throw new ArgumentException(SR.GetString("The parameter '{0}' cannot be an empty string.", new object[] { "frameworkName" }), "frameworkName");
			}
			string[] array = frameworkName.Split(',', StringSplitOptions.None);
			if (array.Length < 2 || array.Length > 3)
			{
				throw new ArgumentException(SR.GetString("FrameworkName cannot have less than two components or more than three components."), "frameworkName");
			}
			this.m_identifier = array[0].Trim();
			if (this.m_identifier.Length == 0)
			{
				throw new ArgumentException(SR.GetString("FrameworkName is invalid."), "frameworkName");
			}
			bool flag = false;
			this.m_profile = string.Empty;
			int i = 1;
			while (i < array.Length)
			{
				string[] array2 = array[i].Split('=', StringSplitOptions.None);
				if (array2.Length != 2)
				{
					throw new ArgumentException(SR.GetString("FrameworkName is invalid."), "frameworkName");
				}
				string text = array2[0].Trim();
				string text2 = array2[1].Trim();
				if (text.Equals("Version", StringComparison.OrdinalIgnoreCase))
				{
					flag = true;
					if (text2.Length > 0 && (text2[0] == 'v' || text2[0] == 'V'))
					{
						text2 = text2.Substring(1);
					}
					try
					{
						this.m_version = new Version(text2);
						goto IL_0181;
					}
					catch (Exception ex)
					{
						throw new ArgumentException(SR.GetString("FrameworkName version component is invalid."), "frameworkName", ex);
					}
					goto IL_014B;
				}
				goto IL_014B;
				IL_0181:
				i++;
				continue;
				IL_014B:
				if (!text.Equals("Profile", StringComparison.OrdinalIgnoreCase))
				{
					throw new ArgumentException(SR.GetString("FrameworkName is invalid."), "frameworkName");
				}
				if (!string.IsNullOrEmpty(text2))
				{
					this.m_profile = text2;
					goto IL_0181;
				}
				goto IL_0181;
			}
			if (!flag)
			{
				throw new ArgumentException(SR.GetString("FrameworkName version component is missing."), "frameworkName");
			}
		}

		public static bool operator ==(FrameworkName left, FrameworkName right)
		{
			if (left == null)
			{
				return right == null;
			}
			return left.Equals(right);
		}

		public static bool operator !=(FrameworkName left, FrameworkName right)
		{
			return !(left == right);
		}

		private readonly string m_identifier;

		private readonly Version m_version;

		private readonly string m_profile;

		private string m_fullName;

		private const char c_componentSeparator = ',';

		private const char c_keyValueSeparator = '=';

		private const char c_versionValuePrefix = 'v';

		private const string c_versionKey = "Version";

		private const string c_profileKey = "Profile";
	}
}
