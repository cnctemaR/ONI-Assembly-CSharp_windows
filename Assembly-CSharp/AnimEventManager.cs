using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimEventManager
{
	public int PlayAnim(KAnimControllerBase controller, KAnim.Anim anim, KAnim.PlayMode mode, float time, bool use_unscaled_time)
	{
		AnimEventManager.AnimData animData = default(AnimEventManager.AnimData);
		animData.frameRate = anim.frameRate;
		animData.totalTime = anim.totalTime;
		animData.numFrames = anim.numFrames;
		animData.useUnscaledTime = use_unscaled_time;
		AnimEventManager.EventPlayerData eventPlayerData = new AnimEventManager.EventPlayerData();
		eventPlayerData.elapsedTime = time;
		eventPlayerData.mode = mode;
		eventPlayerData.controller = controller as KBatchedAnimController;
		eventPlayerData.currentFrame = eventPlayerData.controller.GetFrameIdx(eventPlayerData.elapsedTime, false);
		eventPlayerData.previousFrame = -1;
		eventPlayerData.events = null;
		eventPlayerData.updatingEvents = null;
		KPrefabID component = controller.GetComponent<KPrefabID>();
		if (component != null)
		{
			eventPlayerData.events = GameAudioSheets.Get().GetEvents(anim.id);
		}
		if (eventPlayerData.events == null)
		{
			eventPlayerData.events = AnimEventManager.emptyEventList;
		}
		int num;
		if (this.freeIndices.Count > 0)
		{
			num = this.freeIndices[this.freeIndices.Count - 1];
			this.freeIndices.RemoveAt(this.freeIndices.Count - 1);
			this.eventData[num] = eventPlayerData;
			this.animData[num] = animData;
		}
		else
		{
			num = this.eventData.Count;
			this.eventData.Add(eventPlayerData);
			this.animData.Add(animData);
		}
		return num;
	}

	public int StopAnim(int handle)
	{
		if (handle == -1)
		{
			return -1;
		}
		AnimEventManager.EventPlayerData eventPlayerData = this.eventData[handle];
		if (eventPlayerData.controller == null)
		{
			return -1;
		}
		this.StopEvents(eventPlayerData);
		eventPlayerData.controller = null;
		eventPlayerData.events = null;
		this.eventData[handle] = eventPlayerData;
		this.freeIndices.Add(handle);
		return -1;
	}

	public float GetElapsedTime(int handle)
	{
		return this.eventData[handle].elapsedTime;
	}

	public void SetElapsedTime(int handle, float elapsedTime)
	{
		this.eventData[handle].SetElapsedTime(elapsedTime);
	}

	public void Update()
	{
		float deltaTime = Time.deltaTime;
		float unscaledDeltaTime = Time.unscaledDeltaTime;
		for (int i = 0; i < this.eventData.Count; i++)
		{
			AnimEventManager.EventPlayerData eventPlayerData = this.eventData[i];
			if (!(eventPlayerData.controller == null))
			{
				eventPlayerData.currentFrame = eventPlayerData.controller.GetFrameIdx(eventPlayerData.elapsedTime, false);
				this.PlayEvents(eventPlayerData);
				float num = ((!this.animData[i].useUnscaledTime) ? deltaTime : unscaledDeltaTime);
				eventPlayerData.previousFrame = eventPlayerData.currentFrame;
				eventPlayerData.elapsedTime += num * eventPlayerData.controller.GetPlaySpeed();
				if (num > 0f && eventPlayerData.mode != KAnim.PlayMode.Paused)
				{
					if (eventPlayerData.updatingEvents != null)
					{
						for (int j = 0; j < eventPlayerData.updatingEvents.Count; j++)
						{
							AnimEvent animEvent = eventPlayerData.updatingEvents[j];
							animEvent.OnUpdate(eventPlayerData);
						}
					}
					this.eventData[i] = eventPlayerData;
					if (eventPlayerData.mode != KAnim.PlayMode.Loop && eventPlayerData.currentFrame >= this.animData[i].numFrames - 1)
					{
						this.StopEvents(eventPlayerData);
						this.finishedCalls.Add(eventPlayerData.controller);
					}
				}
			}
		}
		for (int k = 0; k < this.finishedCalls.Count; k++)
		{
			KBatchedAnimController kbatchedAnimController = this.finishedCalls[k];
			kbatchedAnimController.TriggerStop();
		}
		this.finishedCalls.Clear();
	}

	private void PlayEvents(AnimEventManager.EventPlayerData data)
	{
		for (int i = 0; i < data.events.Count; i++)
		{
			AnimEvent animEvent = data.events[i];
			animEvent.Play(data);
		}
	}

	private void StopEvents(AnimEventManager.EventPlayerData data)
	{
		for (int i = 0; i < data.events.Count; i++)
		{
			AnimEvent animEvent = data.events[i];
			animEvent.Stop(data);
		}
		if (data.updatingEvents != null)
		{
			data.updatingEvents.Clear();
		}
	}

	private static readonly List<AnimEvent> emptyEventList = new List<AnimEvent>();

	private List<AnimEventManager.EventPlayerData> eventData = new List<AnimEventManager.EventPlayerData>();

	private List<AnimEventManager.AnimData> animData = new List<AnimEventManager.AnimData>();

	private List<int> freeIndices = new List<int>();

	private List<KBatchedAnimController> finishedCalls = new List<KBatchedAnimController>();

	private struct AnimData
	{
		public float frameRate;

		public float totalTime;

		public int numFrames;

		public bool useUnscaledTime;
	}

	public class EventPlayerData
	{
		public int currentFrame { get; set; }

		public int previousFrame { get; set; }

		public ComponentType GetComponent<ComponentType>()
		{
			return this.controller.GetComponent<ComponentType>();
		}

		public string name
		{
			get
			{
				return this.controller.name;
			}
		}

		public float normalizedTime
		{
			get
			{
				return this.elapsedTime / this.controller.CurrentAnim.totalTime;
			}
		}

		public string currentAnimFile
		{
			get
			{
				return this.controller.currentAnimFile;
			}
		}

		public KAnimHashedString currentAnimFileHash
		{
			get
			{
				return this.controller.currentAnimFileHash;
			}
		}

		public string currentAnim
		{
			get
			{
				return this.controller.currentAnim;
			}
		}

		public Vector3 position
		{
			get
			{
				return this.controller.transform.position;
			}
		}

		public void AddUpdatingEvent(AnimEvent ev)
		{
			if (this.updatingEvents == null)
			{
				this.updatingEvents = new List<AnimEvent>();
			}
			this.updatingEvents.Add(ev);
		}

		public void SetElapsedTime(float elapsedTime)
		{
			this.elapsedTime = elapsedTime;
		}

		public float elapsedTime;

		public KAnim.PlayMode mode;

		public List<AnimEvent> events;

		public List<AnimEvent> updatingEvents;

		public KBatchedAnimController controller;
	}
}
