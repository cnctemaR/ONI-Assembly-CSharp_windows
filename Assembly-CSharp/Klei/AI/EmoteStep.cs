using System;
using UnityEngine;

namespace Klei.AI
{
	public class EmoteStep
	{
		public int Id
		{
			get
			{
				return this.anim.HashValue;
			}
		}

		public HandleVector<EmoteStep.Callbacks>.Handle RegisterCallbacks(Action<GameObject> startedCb, Action<GameObject> finishedCb)
		{
			if (startedCb == null && finishedCb == null)
			{
				return HandleVector<EmoteStep.Callbacks>.InvalidHandle;
			}
			EmoteStep.Callbacks callbacks = new EmoteStep.Callbacks
			{
				StartedCb = startedCb,
				FinishedCb = finishedCb
			};
			return this.callbacks.Add(callbacks);
		}

		public void UnregisterCallbacks(HandleVector<EmoteStep.Callbacks>.Handle callbackHandle)
		{
			this.callbacks.Release(callbackHandle);
		}

		public void OnStepStarted(HandleVector<EmoteStep.Callbacks>.Handle callbackHandle, GameObject parameter)
		{
			if (callbackHandle == HandleVector<EmoteStep.Callbacks>.Handle.InvalidHandle)
			{
				return;
			}
			EmoteStep.Callbacks item = this.callbacks.GetItem(callbackHandle);
			if (item.StartedCb != null)
			{
				item.StartedCb(parameter);
			}
		}

		public void OnStepFinished(HandleVector<EmoteStep.Callbacks>.Handle callbackHandle, GameObject parameter)
		{
			if (callbackHandle == HandleVector<EmoteStep.Callbacks>.Handle.InvalidHandle)
			{
				return;
			}
			EmoteStep.Callbacks item = this.callbacks.GetItem(callbackHandle);
			if (item.FinishedCb != null)
			{
				item.FinishedCb(parameter);
			}
		}

		public HashedString anim = HashedString.Invalid;

		public KAnim.PlayMode mode = KAnim.PlayMode.Once;

		public float timeout = -1f;

		private HandleVector<EmoteStep.Callbacks> callbacks = new HandleVector<EmoteStep.Callbacks>(64);

		public struct Callbacks
		{
			public Action<GameObject> StartedCb;

			public Action<GameObject> FinishedCb;
		}
	}
}
