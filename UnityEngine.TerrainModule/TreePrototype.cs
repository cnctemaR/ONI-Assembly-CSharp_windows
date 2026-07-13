using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode]
	[NativeAsStruct]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class TreePrototype
	{
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

		public int navMeshLod
		{
			get
			{
				return this.m_NavMeshLod;
			}
			set
			{
				this.m_NavMeshLod = value;
			}
		}

		public TreePrototype()
		{
		}

		public TreePrototype(TreePrototype other)
		{
			this.prefab = other.prefab;
			this.bendFactor = other.bendFactor;
			this.navMeshLod = other.navMeshLod;
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
			bool flag = other == null;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = other == this;
				if (flag3)
				{
					flag2 = true;
				}
				else
				{
					bool flag4 = base.GetType() != other.GetType();
					if (flag4)
					{
						flag2 = false;
					}
					else
					{
						bool flag5 = this.prefab == other.prefab && this.bendFactor == other.bendFactor && this.navMeshLod == other.navMeshLod;
						flag2 = flag5;
					}
				}
			}
			return flag2;
		}

		internal bool Validate(out string errorMessage)
		{
			return TreePrototype.ValidateTreePrototype(this, out errorMessage);
		}

		[FreeFunction("TerrainDataScriptingInterface::ValidateTreePrototype")]
		internal static bool ValidateTreePrototype([NotNull] TreePrototype prototype, out string errorMessage)
		{
			if (prototype == null)
			{
				ThrowHelper.ThrowArgumentNullException(prototype, "prototype");
			}
			bool flag;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				flag = TreePrototype.ValidateTreePrototype_Injected(prototype, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				errorMessage = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return flag;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ValidateTreePrototype_Injected(TreePrototype prototype, out ManagedSpanWrapper errorMessage);

		[NativeName("prefab")]
		internal GameObject m_Prefab;

		[NativeName("bendFactor")]
		internal float m_BendFactor;

		[NativeName("navMeshLod")]
		internal int m_NavMeshLod;
	}
}
