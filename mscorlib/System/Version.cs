using System;
using System.Runtime.InteropServices;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public sealed class Version : IComparable, ICloneable, IComparable<Version>, IEquatable<Version>
	{
		public Version()
		{
			this.CheckedSet(2, 0, 0, -1, -1);
		}

		public Version(string version)
		{
			int num = -1;
			int num2 = -1;
			int num3 = -1;
			int num4 = -1;
			if (version == null)
			{
				throw new ArgumentNullException("version");
			}
			string[] array = version.Split(new char[] { '.' });
			int num5 = array.Length;
			if (num5 < 2 || num5 > 4)
			{
				throw new ArgumentException(Locale.GetText("There must be 2, 3 or 4 components in the version string."));
			}
			if (num5 > 0)
			{
				num = int.Parse(array[0]);
			}
			if (num5 > 1)
			{
				num2 = int.Parse(array[1]);
			}
			if (num5 > 2)
			{
				num3 = int.Parse(array[2]);
			}
			if (num5 > 3)
			{
				num4 = int.Parse(array[3]);
			}
			this.CheckedSet(num5, num, num2, num3, num4);
		}

		public Version(int major, int minor)
		{
			this.CheckedSet(2, major, minor, 0, 0);
		}

		public Version(int major, int minor, int build)
		{
			this.CheckedSet(3, major, minor, build, 0);
		}

		public Version(int major, int minor, int build, int revision)
		{
			this.CheckedSet(4, major, minor, build, revision);
		}

		private void CheckedSet(int defined, int major, int minor, int build, int revision)
		{
			if (major < 0)
			{
				throw new ArgumentOutOfRangeException("major");
			}
			this._Major = major;
			if (minor < 0)
			{
				throw new ArgumentOutOfRangeException("minor");
			}
			this._Minor = minor;
			if (defined == 2)
			{
				this._Build = -1;
				this._Revision = -1;
				return;
			}
			if (build < 0)
			{
				throw new ArgumentOutOfRangeException("build");
			}
			this._Build = build;
			if (defined == 3)
			{
				this._Revision = -1;
				return;
			}
			if (revision < 0)
			{
				throw new ArgumentOutOfRangeException("revision");
			}
			this._Revision = revision;
		}

		public int Build
		{
			get
			{
				return this._Build;
			}
		}

		public int Major
		{
			get
			{
				return this._Major;
			}
		}

		public int Minor
		{
			get
			{
				return this._Minor;
			}
		}

		public int Revision
		{
			get
			{
				return this._Revision;
			}
		}

		public short MajorRevision
		{
			get
			{
				return (short)(this._Revision >> 16);
			}
		}

		public short MinorRevision
		{
			get
			{
				return (short)this._Revision;
			}
		}

		public object Clone()
		{
			if (this._Build == -1)
			{
				return new Version(this._Major, this._Minor);
			}
			if (this._Revision == -1)
			{
				return new Version(this._Major, this._Minor, this._Build);
			}
			return new Version(this._Major, this._Minor, this._Build, this._Revision);
		}

		public int CompareTo(object version)
		{
			if (version == null)
			{
				return 1;
			}
			if (!(version is Version))
			{
				throw new ArgumentException(Locale.GetText("Argument to Version.CompareTo must be a Version."));
			}
			return this.CompareTo((Version)version);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as Version);
		}

		public int CompareTo(Version value)
		{
			if (value == null)
			{
				return 1;
			}
			if (this._Major > value._Major)
			{
				return 1;
			}
			if (this._Major < value._Major)
			{
				return -1;
			}
			if (this._Minor > value._Minor)
			{
				return 1;
			}
			if (this._Minor < value._Minor)
			{
				return -1;
			}
			if (this._Build > value._Build)
			{
				return 1;
			}
			if (this._Build < value._Build)
			{
				return -1;
			}
			if (this._Revision > value._Revision)
			{
				return 1;
			}
			if (this._Revision < value._Revision)
			{
				return -1;
			}
			return 0;
		}

		public bool Equals(Version obj)
		{
			return obj != null && obj._Major == this._Major && obj._Minor == this._Minor && obj._Build == this._Build && obj._Revision == this._Revision;
		}

		public override int GetHashCode()
		{
			return (this._Revision << 24) | (this._Build << 16) | (this._Minor << 8) | this._Major;
		}

		public override string ToString()
		{
			string text = this._Major.ToString() + "." + this._Minor.ToString();
			if (this._Build != -1)
			{
				text = text + "." + this._Build.ToString();
			}
			if (this._Revision != -1)
			{
				text = text + "." + this._Revision.ToString();
			}
			return text;
		}

		public string ToString(int fieldCount)
		{
			if (fieldCount == 0)
			{
				return string.Empty;
			}
			if (fieldCount == 1)
			{
				return this._Major.ToString();
			}
			if (fieldCount == 2)
			{
				return this._Major.ToString() + "." + this._Minor.ToString();
			}
			if (fieldCount == 3)
			{
				if (this._Build == -1)
				{
					throw new ArgumentException(Locale.GetText("fieldCount is larger than the number of components defined in this instance."));
				}
				return string.Concat(new string[]
				{
					this._Major.ToString(),
					".",
					this._Minor.ToString(),
					".",
					this._Build.ToString()
				});
			}
			else
			{
				if (fieldCount != 4)
				{
					throw new ArgumentException(Locale.GetText("Invalid fieldCount parameter: ") + fieldCount.ToString());
				}
				if (this._Build == -1 || this._Revision == -1)
				{
					throw new ArgumentException(Locale.GetText("fieldCount is larger than the number of components defined in this instance."));
				}
				return string.Concat(new string[]
				{
					this._Major.ToString(),
					".",
					this._Minor.ToString(),
					".",
					this._Build.ToString(),
					".",
					this._Revision.ToString()
				});
			}
		}

		internal static Version CreateFromString(string info)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 1;
			int num6 = -1;
			if (info == null)
			{
				return new Version(0, 0, 0, 0);
			}
			foreach (char c in info)
			{
				if (char.IsDigit(c))
				{
					if (num6 < 0)
					{
						num6 = (int)(c - '0');
					}
					else
					{
						num6 = num6 * 10 + (int)(c - '0');
					}
				}
				else if (num6 >= 0)
				{
					switch (num5)
					{
					case 1:
						num = num6;
						break;
					case 2:
						num2 = num6;
						break;
					case 3:
						num3 = num6;
						break;
					case 4:
						num4 = num6;
						break;
					}
					num6 = -1;
					num5++;
				}
				if (num5 == 5)
				{
					break;
				}
			}
			if (num6 >= 0)
			{
				switch (num5)
				{
				case 1:
					num = num6;
					break;
				case 2:
					num2 = num6;
					break;
				case 3:
					num3 = num6;
					break;
				case 4:
					num4 = num6;
					break;
				}
			}
			return new Version(num, num2, num3, num4);
		}

		public static bool operator ==(Version v1, Version v2)
		{
			return object.Equals(v1, v2);
		}

		public static bool operator !=(Version v1, Version v2)
		{
			return !object.Equals(v1, v2);
		}

		public static bool operator >(Version v1, Version v2)
		{
			return v1.CompareTo(v2) > 0;
		}

		public static bool operator >=(Version v1, Version v2)
		{
			return v1.CompareTo(v2) >= 0;
		}

		public static bool operator <(Version v1, Version v2)
		{
			return v1.CompareTo(v2) < 0;
		}

		public static bool operator <=(Version v1, Version v2)
		{
			return v1.CompareTo(v2) <= 0;
		}

		private const int UNDEFINED = -1;

		private int _Major;

		private int _Minor;

		private int _Build;

		private int _Revision;
	}
}
