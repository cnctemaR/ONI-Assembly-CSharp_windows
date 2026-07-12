using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequiredByNativeCode(GenerateProxy = true)]
	[NativeHeader("Runtime/Mono/AssemblyFullName.h")]
	internal struct AssemblyVersion
	{
		public AssemblyVersion(ushort major, ushort minor, ushort build, ushort revision)
		{
			this.major = major;
			this.minor = minor;
			this.build = build;
			this.revision = revision;
		}

		public static bool operator ==(AssemblyVersion lhs, AssemblyVersion rhs)
		{
			return lhs.major == rhs.major && lhs.minor == rhs.minor && lhs.build == rhs.build && lhs.revision == rhs.revision;
		}

		public static bool operator !=(AssemblyVersion lhs, AssemblyVersion rhs)
		{
			return !(lhs == rhs);
		}

		public static bool operator <(AssemblyVersion lhs, AssemblyVersion rhs)
		{
			bool flag = lhs.major != rhs.major;
			bool flag2;
			if (flag)
			{
				flag2 = lhs.major < rhs.major;
			}
			else
			{
				bool flag3 = lhs.minor != rhs.minor;
				if (flag3)
				{
					flag2 = lhs.minor < rhs.minor;
				}
				else
				{
					bool flag4 = lhs.build != rhs.build;
					if (flag4)
					{
						flag2 = lhs.build < rhs.build;
					}
					else
					{
						bool flag5 = lhs.revision != rhs.revision;
						flag2 = flag5 && lhs.revision < rhs.revision;
					}
				}
			}
			return flag2;
		}

		public static bool operator >(AssemblyVersion lhs, AssemblyVersion rhs)
		{
			bool flag = lhs.major != rhs.major;
			bool flag2;
			if (flag)
			{
				flag2 = lhs.major > rhs.major;
			}
			else
			{
				bool flag3 = lhs.minor != rhs.minor;
				if (flag3)
				{
					flag2 = lhs.minor > rhs.minor;
				}
				else
				{
					bool flag4 = lhs.build != rhs.build;
					if (flag4)
					{
						flag2 = lhs.build > rhs.build;
					}
					else
					{
						bool flag5 = lhs.revision != rhs.revision;
						flag2 = flag5 && lhs.revision > rhs.revision;
					}
				}
			}
			return flag2;
		}

		public override string ToString()
		{
			return string.Format("{0}.{1}.{2}.{3}", new object[] { this.major, this.minor, this.build, this.revision });
		}

		public override bool Equals(object other)
		{
			if (other is AssemblyVersion)
			{
				AssemblyVersion assemblyVersion = (AssemblyVersion)other;
				if (this.major == assemblyVersion.major && this.minor == assemblyVersion.minor && this.build == assemblyVersion.build)
				{
					return this.revision == assemblyVersion.revision;
				}
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine<ushort, ushort, ushort, ushort>(this.major, this.minor, this.build, this.revision);
		}

		public ushort major;

		public ushort minor;

		public ushort build;

		public ushort revision;
	}
}
