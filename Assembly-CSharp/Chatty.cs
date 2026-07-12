using System;
using System.Collections.Generic;

public class Chatty : KMonoBehaviour, ISimEveryTick
{
	protected override void OnPrefabInit()
	{
		base.GetComponent<KPrefabID>().AddTag(GameTags.AlwaysConverse, false);
		base.Subscribe(-594200555, new Action<object>(this.OnStartedTalking));
		this.identity = base.GetComponent<MinionIdentity>();
	}

	private void OnStartedTalking(object data)
	{
		MinionIdentity minionIdentity = data as MinionIdentity;
		if (minionIdentity == null)
		{
			return;
		}
		this.conversationPartners.Add(minionIdentity);
	}

	public void SimEveryTick(float dt)
	{
		if (this.conversationPartners.Count == 0)
		{
			return;
		}
		for (int i = this.conversationPartners.Count - 1; i >= 0; i--)
		{
			MinionIdentity minionIdentity = this.conversationPartners[i];
			this.conversationPartners.RemoveAt(i);
			if (!(minionIdentity == this.identity))
			{
				minionIdentity.AddTag(GameTags.PleasantConversation);
			}
		}
		base.gameObject.AddTag(GameTags.PleasantConversation);
	}

	private MinionIdentity identity;

	private List<MinionIdentity> conversationPartners = new List<MinionIdentity>();
}
