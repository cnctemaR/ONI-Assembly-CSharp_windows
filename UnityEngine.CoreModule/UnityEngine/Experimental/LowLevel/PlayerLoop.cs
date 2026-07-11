using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Experimental.LowLevel
{
	public class PlayerLoop
	{
		public static PlayerLoopSystem GetDefaultPlayerLoop()
		{
			PlayerLoopSystemInternal[] defaultPlayerLoopInternal = PlayerLoop.GetDefaultPlayerLoopInternal();
			int num = 0;
			return PlayerLoop.InternalToPlayerLoopSystem(defaultPlayerLoopInternal, ref num);
		}

		public static void SetPlayerLoop(PlayerLoopSystem loop)
		{
			List<PlayerLoopSystemInternal> list = new List<PlayerLoopSystemInternal>();
			PlayerLoop.PlayerLoopSystemToInternal(loop, ref list);
			PlayerLoop.SetPlayerLoopInternal(list.ToArray());
		}

		private static int PlayerLoopSystemToInternal(PlayerLoopSystem sys, ref List<PlayerLoopSystemInternal> internalSys)
		{
			int count = internalSys.Count;
			PlayerLoopSystemInternal playerLoopSystemInternal = new PlayerLoopSystemInternal
			{
				type = sys.type,
				updateDelegate = sys.updateDelegate,
				updateFunction = sys.updateFunction,
				loopConditionFunction = sys.loopConditionFunction,
				numSubSystems = 0
			};
			internalSys.Add(playerLoopSystemInternal);
			if (sys.subSystemList != null)
			{
				for (int i = 0; i < sys.subSystemList.Length; i++)
				{
					playerLoopSystemInternal.numSubSystems += PlayerLoop.PlayerLoopSystemToInternal(sys.subSystemList[i], ref internalSys);
				}
			}
			internalSys[count] = playerLoopSystemInternal;
			return playerLoopSystemInternal.numSubSystems + 1;
		}

		private static PlayerLoopSystem InternalToPlayerLoopSystem(PlayerLoopSystemInternal[] internalSys, ref int offset)
		{
			PlayerLoopSystem playerLoopSystem = new PlayerLoopSystem
			{
				type = internalSys[offset].type,
				updateDelegate = internalSys[offset].updateDelegate,
				updateFunction = internalSys[offset].updateFunction,
				loopConditionFunction = internalSys[offset].loopConditionFunction,
				subSystemList = null
			};
			int num = offset++;
			if (internalSys[num].numSubSystems > 0)
			{
				List<PlayerLoopSystem> list = new List<PlayerLoopSystem>();
				while (offset <= num + internalSys[num].numSubSystems)
				{
					list.Add(PlayerLoop.InternalToPlayerLoopSystem(internalSys, ref offset));
				}
				playerLoopSystem.subSystemList = list.ToArray();
			}
			return playerLoopSystem;
		}

		[NativeMethod(IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern PlayerLoopSystemInternal[] GetDefaultPlayerLoopInternal();

		[NativeMethod(IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetPlayerLoopInternal(PlayerLoopSystemInternal[] loop);
	}
}
