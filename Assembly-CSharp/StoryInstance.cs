using System;
using Database;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class StoryInstance : ISaveLoadable
{
	public StoryInstance.State CurrentState
	{
		get
		{
			return this.state;
		}
		set
		{
			this.state = value;
			this.Telemetry.LogStateChange(this.state, GameClock.Instance.GetTimeInCycles());
		}
	}

	public StoryManager.StoryTelemetry Telemetry
	{
		get
		{
			if (this.telemetry == null)
			{
				this.telemetry = new StoryManager.StoryTelemetry();
			}
			return this.telemetry;
		}
	}

	public Story GetStory()
	{
		if (this._story == null)
		{
			this._story = Db.Get().Stories.Get(this.storyId);
		}
		return this._story;
	}

	public StoryInstance(Story story, int worldId)
	{
		this._story = story;
		this.storyId = story.Id;
		this.worldId = worldId;
	}

	[Serialize]
	public readonly string storyId;

	[Serialize]
	public int worldId;

	[Serialize]
	private StoryInstance.State state;

	[Serialize]
	private StoryManager.StoryTelemetry telemetry;

	public EventInfoData eventInfo;

	public StoryCompleteData completionData;

	public Action<StoryInstance> sequenceCompleteCallback;

	private Story _story;

	public enum State
	{
		RETROFITTED = -1,
		NOT_STARTED,
		DISCOVERED,
		IN_PROGRESS,
		COMPLETE
	}
}
