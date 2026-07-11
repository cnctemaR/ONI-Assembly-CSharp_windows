using System;
using UnityEngine.Playables;

public class FMODEventMixerBehaviour : PlayableBehaviour
{
	public override void ProcessFrame(Playable playable, FrameData info, object playerData)
	{
		int inputCount = playable.GetInputCount<Playable>();
		float num = (float)playable.GetGraph<Playable>().GetRootPlayable(0).GetTime<Playable>();
		for (int i = 0; i < inputCount; i++)
		{
			((ScriptPlayable<T>)playable.GetInput(i)).GetBehaviour().UpdateBehaviour(num);
		}
	}
}
