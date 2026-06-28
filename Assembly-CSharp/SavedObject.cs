using System;
using KSerialization;

public class SavedObject : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.Subscribe(856640610, new EventSystem.EventHandler(this.OnStore));
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (!this.inStorage && !this.registered)
		{
			SaveLoader.Instance.saveManager.Register(this.root);
			this.registered = true;
		}
	}

	protected override void OnCleanUp()
	{
		this.registered = false;
	}

	private void OnStore(object data)
	{
		if (data != null)
		{
			SaveLoader.Instance.saveManager.Unregister(this.root);
			this.inStorage = true;
		}
		else
		{
			SaveLoader.Instance.saveManager.Register(this.root);
			this.inStorage = false;
			this.registered = true;
		}
	}

	[MyCmpGet]
	private SaveLoadRoot root;

	[Serialize]
	public bool inStorage;

	private bool registered;
}
