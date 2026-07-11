using System;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class TreePrototype
	{
		public TreePrototype()
		{
		}

		public TreePrototype(TreePrototype other)
		{
			this.prefab = other.prefab;
			this.bendFactor = other.bendFactor;
		}

		public GameObject prefab
		{
			get
			{
				return this.m_Prefab;
			}
			set
			{
				this.m_Prefab = value;
			}
		}

		public float bendFactor
		{
			get
			{
				return this.m_BendFactor;
			}
			set
			{
				this.m_BendFactor = value;
			}
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as TreePrototype);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		private bool Equals(TreePrototype other)
		{
			bool flag;
			if (object.ReferenceEquals(other, null))
			{
				flag = false;
			}
			else if (object.ReferenceEquals(other, this))
			{
				flag = true;
			}
			else if (base.GetType() != other.GetType())
			{
				flag = false;
			}
			else
			{
				bool flag2 = this.prefab == other.prefab && this.bendFactor == other.bendFactor;
				flag = flag2;
			}
			return flag;
		}

		internal GameObject m_Prefab;

		internal float m_BendFactor;
	}
}
