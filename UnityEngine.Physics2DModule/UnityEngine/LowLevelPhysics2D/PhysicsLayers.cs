using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UnityEngine.LowLevelPhysics2D
{
	public readonly struct PhysicsLayers
	{
		public static PhysicsMask GetLayerMask(params string[] layerNames)
		{
			bool flag = layerNames.Length == 0;
			if (flag)
			{
				throw new ArgumentException("No Layer Names provided.", "layerNames");
			}
			PhysicsLayers.LayerNames layerNames2;
			bool flag2;
			if (PhysicsWorld.useFullLayers)
			{
				layerNames2 = PhysicsLowLevelScripting2D.PhysicsGlobal_GetPhysicsLayers() as PhysicsLayers.LayerNames;
				flag2 = layerNames2 != null;
			}
			else
			{
				flag2 = false;
			}
			bool flag3 = flag2;
			PhysicsMask physicsMask2;
			if (flag3)
			{
				PhysicsMask physicsMask = PhysicsMask.None;
				foreach (string text in layerNames)
				{
					PhysicsMask layerMask = layerNames2.GetLayerMask(text);
					bool flag4 = layerMask != PhysicsMask.None;
					if (flag4)
					{
						physicsMask |= layerMask;
					}
					else
					{
						Debug.LogWarning("The layer name '" + text + "' could not be found in the full 64-bit layers. Note that the name(s) provided are case-sensitive.");
					}
				}
				physicsMask2 = physicsMask;
			}
			else
			{
				PhysicsMask physicsMask3 = PhysicsMask.None;
				foreach (string text2 in layerNames)
				{
					int num = LayerMask.NameToLayer(text2);
					bool flag5 = num != -1;
					if (flag5)
					{
						physicsMask3 |= new PhysicsMask(new int[] { num });
					}
					else
					{
						Debug.LogWarning("The layer name '" + text2 + "' could not be found in the standard 32-bit layers. Note that the name(s) provided are case-sensitive.");
					}
				}
				physicsMask2 = physicsMask3;
			}
			return physicsMask2;
		}

		public static int GetLayerOrdinal(string layerName)
		{
			PhysicsLayers.LayerNames layerNames;
			bool flag;
			if (PhysicsWorld.useFullLayers)
			{
				layerNames = PhysicsLowLevelScripting2D.PhysicsGlobal_GetPhysicsLayers() as PhysicsLayers.LayerNames;
				flag = layerNames != null;
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			int num;
			if (flag2)
			{
				int layerOrdinal = layerNames.GetLayerOrdinal(layerName);
				bool flag3 = layerOrdinal != -1;
				if (flag3)
				{
					num = layerOrdinal;
				}
				else
				{
					Debug.LogWarning("The layer name '" + layerName + "' could not be found in the full 64-bit layers. Note that the name provided is case-sensitive.");
					num = -1;
				}
			}
			else
			{
				int num2 = LayerMask.NameToLayer(layerName);
				bool flag4 = num2 != -1;
				if (flag4)
				{
					num = num2;
				}
				else
				{
					Debug.LogWarning("The layer name '" + layerName + "' could not be found in the standard 32-bit layers. Note that the name provided is case-sensitive.");
					num = -1;
				}
			}
			return num;
		}

		public static string GetLayerName(int layerOrdinal)
		{
			PhysicsLayers.LayerNames layerNames;
			bool flag;
			if (PhysicsWorld.useFullLayers)
			{
				layerNames = PhysicsLowLevelScripting2D.PhysicsGlobal_GetPhysicsLayers() as PhysicsLayers.LayerNames;
				flag = layerNames != null;
			}
			else
			{
				flag = false;
			}
			bool flag2 = flag;
			string text;
			if (flag2)
			{
				bool flag3 = layerOrdinal < 0 || layerOrdinal > 63;
				if (flag3)
				{
					throw new ArgumentOutOfRangeException(string.Format("The layer ordinal `{0}' is out of the valid range [0, 63].", layerOrdinal));
				}
				text = layerNames.GetLayerName(layerOrdinal);
			}
			else
			{
				bool flag4 = layerOrdinal < 0 || layerOrdinal > 31;
				if (flag4)
				{
					throw new ArgumentOutOfRangeException(string.Format("The layer ordinal `{0}' is out of the valid range [0, 31].", layerOrdinal));
				}
				text = LayerMask.LayerToName(layerOrdinal);
			}
			return text;
		}

		internal static void GetLayerNamesAndMasks(List<string> layerNames, List<ulong> layerMasks)
		{
			layerNames.Clear();
			layerMasks.Clear();
			PhysicsLayers.LayerNames layerNames2 = PhysicsLowLevelScripting2D.PhysicsGlobal_GetPhysicsLayers() as PhysicsLayers.LayerNames;
			bool flag = layerNames2 != null;
			if (flag)
			{
				string[] names = layerNames2.m_Names;
				bool flag2 = names.Length == 64;
				if (flag2)
				{
					for (int i = 0; i < 64; i++)
					{
						string text = names[i];
						bool flag3 = string.IsNullOrEmpty(text);
						if (!flag3)
						{
							layerNames.Add(string.Format("{0} [{1}]", text, i));
							layerMasks.Add(1UL << i);
						}
					}
				}
			}
		}

		internal static void GetBitNamesAndMasks(List<string> layerNames, List<ulong> layerMasks)
		{
			layerNames.Clear();
			layerMasks.Clear();
			for (int i = 0; i < 64; i++)
			{
				layerNames.Add(string.Format("{0}", i));
				layerMasks.Add(1UL << i);
			}
		}

		public const int InvalidLayerOrdinal = -1;

		[Serializable]
		public class LayerNames : ISerializationCallbackReceiver
		{
			private string[] Names
			{
				get
				{
					bool flag = this.m_Names == null || this.m_Names.Length != 64;
					if (flag)
					{
						this.m_Names = new string[64];
					}
					return this.m_Names;
				}
			}

			private Dictionary<string, int> NameMap
			{
				get
				{
					bool flag = this.m_NameMap == null;
					if (flag)
					{
						this.m_NameMap = new Dictionary<string, int>(64);
					}
					return this.m_NameMap;
				}
			}

			public void OnBeforeSerialize()
			{
			}

			public void OnAfterDeserialize()
			{
				string[] names = this.Names;
				Dictionary<string, int> nameMap = this.NameMap;
				nameMap.Clear();
				for (int i = 0; i < 64; i++)
				{
					string text = names[i];
					bool flag = string.IsNullOrEmpty(text);
					if (!flag)
					{
						nameMap.TryAdd(text, i);
					}
				}
			}

			internal static PhysicsLayers.LayerNames DefaultLayerNames
			{
				get
				{
					PhysicsLayers.LayerNames layerNames = new PhysicsLayers.LayerNames();
					string[] names = layerNames.Names;
					Dictionary<string, int> nameMap = layerNames.NameMap;
					names[0] = "Default";
					nameMap.Add(names[0], 0);
					return layerNames;
				}
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal int GetLayerOrdinal(string layerName)
			{
				int num;
				bool flag = this.NameMap.TryGetValue(layerName, out num);
				int num2;
				if (flag)
				{
					num2 = num;
				}
				else
				{
					num2 = -1;
				}
				return num2;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal PhysicsMask GetLayerMask(string layerName)
			{
				int layerOrdinal = this.GetLayerOrdinal(layerName);
				bool flag = layerOrdinal != -1;
				PhysicsMask physicsMask;
				if (flag)
				{
					physicsMask = new PhysicsMask(new int[] { layerOrdinal });
				}
				else
				{
					physicsMask = default(PhysicsMask);
				}
				return physicsMask;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal string GetLayerName(int layerOrdinal)
			{
				bool flag = layerOrdinal < 0 || layerOrdinal > 63;
				if (flag)
				{
					throw new ArgumentOutOfRangeException(string.Format("The layer ordinal `{0}' is out of the valid range [0, 63].", layerOrdinal));
				}
				return this.Names[layerOrdinal];
			}

			[SerializeField]
			internal string[] m_Names;

			private Dictionary<string, int> m_NameMap;
		}
	}
}
