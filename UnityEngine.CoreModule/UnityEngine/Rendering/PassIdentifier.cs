using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering
{
	[NativeHeader("Runtime/Shaders/PassIdentifier.h")]
	[UsedByNativeCode]
	public readonly struct PassIdentifier : IEquatable<PassIdentifier>
	{
		public uint SubshaderIndex
		{
			get
			{
				return this.m_SubShaderIndex;
			}
		}

		public uint PassIndex
		{
			get
			{
				return this.m_PassIndex;
			}
		}

		public PassIdentifier(uint subshaderIndex, uint passIndex)
		{
			this.m_SubShaderIndex = subshaderIndex;
			this.m_PassIndex = passIndex;
		}

		public override bool Equals(object o)
		{
			bool flag;
			if (o is PassIdentifier)
			{
				PassIdentifier passIdentifier = (PassIdentifier)o;
				flag = this.Equals(passIdentifier);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public bool Equals(PassIdentifier rhs)
		{
			return this.m_SubShaderIndex == rhs.m_SubShaderIndex && this.m_PassIndex == rhs.m_PassIndex;
		}

		public static bool operator ==(PassIdentifier lhs, PassIdentifier rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(PassIdentifier lhs, PassIdentifier rhs)
		{
			return !(lhs == rhs);
		}

		public override int GetHashCode()
		{
			return this.m_SubShaderIndex.GetHashCode() ^ this.m_PassIndex.GetHashCode();
		}

		internal readonly uint m_SubShaderIndex;

		internal readonly uint m_PassIndex;
	}
}
