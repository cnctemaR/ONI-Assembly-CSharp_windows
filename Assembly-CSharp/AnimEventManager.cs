using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class AnimEventManager : Singleton<AnimEventManager>
{
	public void FreeResources()
	{
	}

	public HandleVector<int>.Handle PlayAnim(KAnimControllerBase controller, KAnim.Anim anim, KAnim.PlayMode mode, float time, bool use_unscaled_time)
	{
		AnimEventManager.AnimData animData = default(AnimEventManager.AnimData);
		animData.frameRate = anim.frameRate;
		animData.totalTime = anim.totalTime;
		animData.numFrames = anim.numFrames;
		animData.useUnscaledTime = use_unscaled_time;
		AnimEventManager.EventPlayerData eventPlayerData = default(AnimEventManager.EventPlayerData);
		eventPlayerData.elapsedTime = time;
		eventPlayerData.mode = mode;
		eventPlayerData.controller = controller as KBatchedAnimController;
		eventPlayerData.currentFrame = eventPlayerData.controller.GetFrameIdx(eventPlayerData.elapsedTime, false);
		eventPlayerData.previousFrame = -1;
		eventPlayerData.events = null;
		eventPlayerData.updatingEvents = null;
		eventPlayerData.events = GameAudioSheets.Get().GetEvents(anim.id);
		if (eventPlayerData.events == null)
		{
			eventPlayerData.events = AnimEventManager.emptyEventList;
		}
		HandleVector<int>.Handle handle3;
		if (animData.useUnscaledTime)
		{
			HandleVector<int>.Handle handle = this.uiAnimData.Allocate(animData);
			HandleVector<int>.Handle handle2 = this.uiEventData.Allocate(eventPlayerData);
			handle3 = this.indirectionData.Allocate(new AnimEventManager.IndirectionData(handle, handle2, true));
		}
		else
		{
			HandleVector<int>.Handle handle4 = this.animData.Allocate(animData);
			HandleVector<int>.Handle handle5 = this.eventData.Allocate(eventPlayerData);
			handle3 = this.indirectionData.Allocate(new AnimEventManager.IndirectionData(handle4, handle5, false));
		}
		return handle3;
	}

	public void SetMode(HandleVector<int>.Handle handle, KAnim.PlayMode mode)
	{
		if (!handle.IsValid())
		{
			return;
		}
		AnimEventManager.IndirectionData data = this.indirectionData.GetData(handle);
		KCompactedVector<AnimEventManager.EventPlayerData> kcompactedVector = (data.isUIData ? this.uiEventData : this.eventData);
		AnimEventManager.EventPlayerData data2 = kcompactedVector.GetData(data.eventDataHandle);
		data2.mode = mode;
		kcompactedVector.SetData(data.eventDataHandle, data2);
	}

	public void StopAnim(HandleVector<int>.Handle handle)
	{
		if (!handle.IsValid())
		{
			return;
		}
		AnimEventManager.IndirectionData data = this.indirectionData.GetData(handle);
		KCompactedVector<AnimEventManager.AnimData> kcompactedVector = (data.isUIData ? this.uiAnimData : this.animData);
		KCompactedVector<AnimEventManager.EventPlayerData> kcompactedVector2 = (data.isUIData ? this.uiEventData : this.eventData);
		AnimEventManager.EventPlayerData data2 = kcompactedVector2.GetData(data.eventDataHandle);
		this.StopEvents(data2);
		kcompactedVector.Free(data.animDataHandle);
		kcompactedVector2.Free(data.eventDataHandle);
		this.indirectionData.Free(handle);
	}

	public float GetElapsedTime(HandleVector<int>.Handle handle)
	{
		AnimEventManager.IndirectionData data = this.indirectionData.GetData(handle);
		return (data.isUIData ? this.uiEventData : this.eventData).GetData(data.eventDataHandle).elapsedTime;
	}

	public void SetElapsedTime(HandleVector<int>.Handle handle, float elapsed_time)
	{
		AnimEventManager.IndirectionData data = this.indirectionData.GetData(handle);
		KCompactedVector<AnimEventManager.EventPlayerData> kcompactedVector = (data.isUIData ? this.uiEventData : this.eventData);
		AnimEventManager.EventPlayerData data2 = kcompactedVector.GetData(data.eventDataHandle);
		data2.elapsedTime = elapsed_time;
		kcompactedVector.SetData(data.eventDataHandle, data2);
	}

	public void Update()
	{
		float deltaTime = Time.deltaTime;
		float unscaledDeltaTime = Time.unscaledDeltaTime;
		this.Update(deltaTime, this.animData.GetDataList(), this.eventData.GetDataList());
		this.Update(unscaledDeltaTime, this.uiAnimData.GetDataList(), this.uiEventData.GetDataList());
		for (int i = 0; i < this.finishedCalls.Count; i++)
		{
			this.finishedCalls[i].TriggerStop();
		}
		this.finishedCalls.Clear();
	}

	private void Update(float dt, List<AnimEventManager.AnimData> anim_data, List<AnimEventManager.EventPlayerData> event_data)
	{
		if (dt <= 0f)
		{
			return;
		}
		for (int i = 0; i < event_data.Count; i++)
		{
			AnimEventManager.EventPlayerData eventPlayerData = event_data[i];
			if (!(eventPlayerData.controller == null) && eventPlayerData.mode != KAnim.PlayMode.Paused)
			{
				eventPlayerData.currentFrame = eventPlayerData.controller.GetFrameIdx(eventPlayerData.elapsedTime, false);
				event_data[i] = eventPlayerData;
				this.PlayEvents(eventPlayerData);
				eventPlayerData.previousFrame = eventPlayerData.currentFrame;
				eventPlayerData.elapsedTime += dt * eventPlayerData.controller.GetPlaySpeed();
				event_data[i] = eventPlayerData;
				if (eventPlayerData.updatingEvents != null)
				{
					for (int j = 0; j < eventPlayerData.updatingEvents.Count; j++)
					{
						eventPlayerData.updatingEvents[j].OnUpdate(eventPlayerData);
					}
				}
				event_data[i] = eventPlayerData;
				if (eventPlayerData.mode != KAnim.PlayMode.Loop && eventPlayerData.currentFrame >= anim_data[i].numFrames - 1)
				{
					this.StopEvents(eventPlayerData);
					this.finishedCalls.Add(eventPlayerData.controller);
				}
			}
		}
	}

	private void PlayEvents(AnimEventManager.EventPlayerData data)
	{
		for (int i = 0; i < data.events.Count; i++)
		{
			data.events[i].Play(data);
		}
	}

	private void StopEvents(AnimEventManager.EventPlayerData data)
	{
		for (int i = 0; i < data.events.Count; i++)
		{
			data.events[i].Stop(data);
		}
		if (data.updatingEvents != null)
		{
			data.updatingEvents.Clear();
		}
	}

	public AnimEventManager.DevTools_DebugInfo DevTools_GetDebugInfo()
	{
		return new AnimEventManager.DevTools_DebugInfo(this, this.animData, this.eventData, this.uiAnimData, this.uiEventData);
	}

	private static readonly List<AnimEvent> emptyEventList = new List<AnimEvent>();

	private const int INITIAL_VECTOR_SIZE = 256;

	private KCompactedVector<AnimEventManager.AnimData> animData = new KCompactedVector<AnimEventManager.AnimData>(256);

	private KCompactedVector<AnimEventManager.EventPlayerData> eventData = new KCompactedVector<AnimEventManager.EventPlayerData>(256);

	private KCompactedVector<AnimEventManager.AnimData> uiAnimData = new KCompactedVector<AnimEventManager.AnimData>(256);

	private KCompactedVector<AnimEventManager.EventPlayerData> uiEventData = new KCompactedVector<AnimEventManager.EventPlayerData>(256);

	private KCompactedVector<AnimEventManager.IndirectionData> indirectionData = new KCompactedVector<AnimEventManager.IndirectionData>(0);

	private List<KBatchedAnimController> finishedCalls = new List<KBatchedAnimController>();

	public struct AnimData
	{
		public float frameRate;

		public float totalTime;

		public int numFrames;

		public bool useUnscaledTime;
	}

	[DebuggerDisplay("{controller.name}, Anim={currentAnim}, Frame={currentFrame}, Mode={mode}")]
	public struct EventPlayerData
	{
		public int currentFrame { readonly get; set; }

		public int previousFrame { readonly get; set; }

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

		public Vector3 position
		{
			get
			{
				return this.controller.transform.GetPosition();
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

		public void FreeResources()
		{
			this.elapsedTime = 0f;
			this.mode = KAnim.PlayMode.Once;
			this.currentFrame = 0;
			this.previousFrame = 0;
			this.events = null;
			this.updatingEvents = null;
			this.controller = null;
		}

		public float elapsedTime;

		public KAnim.PlayMode mode;

		public List<AnimEvent> events;

		public List<AnimEvent> updatingEvents;

		public KBatchedAnimController controller;
	}

	private struct IndirectionData
	{
		public IndirectionData(HandleVector<int>.Handle anim_data_handle, HandleVector<int>.Handle event_data_handle, bool is_ui_data)
		{
			this.isUIData = is_ui_data;
			this.animDataHandle = anim_data_handle;
			this.eventDataHandle = event_data_handle;
		}

		public bool isUIData;

		public HandleVector<int>.Handle animDataHandle;

		public HandleVector<int>.Handle eventDataHandle;
	}

	public readonly struct DevTools_DebugInfo
	{
		public DevTools_DebugInfo(AnimEventManager eventManager, KCompactedVector<AnimEventManager.AnimData> animData, KCompactedVector<AnimEventManager.EventPlayerData> eventData, KCompactedVector<AnimEventManager.AnimData> uiAnimData, KCompactedVector<AnimEventManager.EventPlayerData> uiEventData)
		{
			this.eventManager = eventManager;
			this.animData = animData;
			this.eventData = eventData;
			this.uiAnimData = uiAnimData;
			this.uiEventData = uiEventData;
		}

		public readonly AnimEventManager eventManager;

		public readonly KCompactedVector<AnimEventManager.AnimData> animData;

		public readonly KCompactedVector<AnimEventManager.EventPlayerData> eventData;

		public readonly KCompactedVector<AnimEventManager.AnimData> uiAnimData;

		public readonly KCompactedVector<AnimEventManager.EventPlayerData> uiEventData;
	}
}
