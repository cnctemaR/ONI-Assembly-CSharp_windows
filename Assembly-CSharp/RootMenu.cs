using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RootMenu : KScreen
{
	private PlanScreen planScreen
	{
		get
		{
			return UIRegistry.planScreen;
		}
	}

	public static RootMenu Instance { get; private set; }

	public override float GetSortKey()
	{
		return -1f;
	}

	protected override void OnPrefabInit()
	{
		RootMenu.Instance = this;
		UIRegistry.rootMenu = this;
		base.Subscribe(Game.Instance.gameObject, -1503271301, new Action<object>(this.OnSelectObject));
		base.Subscribe(Game.Instance.gameObject, 288942073, new Action<object>(this.OnUIClear));
		base.Subscribe(Game.Instance.gameObject, -809948329, new Action<object>(this.OnBuildingStatechanged));
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.detailsScreen = DetailsScreen.Instance;
		this.userMenuParent = this.detailsScreen.UserMenuPanel.gameObject;
		this.userMenu = Util.KInstantiateUI(this.userMenuPrefab.gameObject, this.userMenuParent, false).GetComponent<UserMenuScreen>();
		this.detailsScreen.gameObject.SetActive(false);
		this.userMenu.gameObject.SetActive(false);
	}

	private void OnClickCommon()
	{
		this.CloseSubMenus();
	}

	public void AddSubMenu(KScreen sub_menu)
	{
		if (sub_menu.activateOnSpawn)
		{
			sub_menu.Show(true);
		}
		this.subMenus.Add(sub_menu);
	}

	public void RemoveSubMenu(KScreen sub_menu)
	{
		this.subMenus.Remove(sub_menu);
	}

	private void CloseSubMenus()
	{
		foreach (KScreen kscreen in this.subMenus)
		{
			if (kscreen != null)
			{
				if (kscreen.activateOnSpawn)
				{
					kscreen.gameObject.SetActive(false);
				}
				else
				{
					kscreen.Deactivate();
				}
			}
		}
		this.subMenus.Clear();
	}

	private void OnSelectObject(object data)
	{
		GameObject gameObject = (GameObject)data;
		if (gameObject != this.selectedGO)
		{
			this.selectedGO = gameObject;
			this.CloseSubMenus();
			this.userMenu.SetSelected(gameObject);
			if (this.selectedGO != null)
			{
				this.AddSubMenu(this.detailsScreen);
				this.detailsScreen.Refresh(this.selectedGO);
				this.AddSubMenu(this.userMenu);
				this.userMenu.Refresh(this.selectedGO);
			}
		}
	}

	private void OnBuildingStatechanged(object data)
	{
		GameObject gameObject = (GameObject)data;
		if (gameObject == this.selectedGO)
		{
			this.OnSelectObject(gameObject);
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (!e.Consumed && e.TryConsume(global::Action.Escape) && SelectTool.Instance.enabled)
		{
			if (this.AreSubMenusOpen())
			{
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Back", false));
				this.CloseSubMenus();
				SelectTool.Instance.Select(null, false);
			}
			else if (e.IsAction(global::Action.Escape))
			{
				if (!SelectTool.Instance.enabled)
				{
					KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Close", false));
				}
				if (PlayerController.Instance.IsUsingDefaultTool())
				{
					if (SelectTool.Instance.selected != null)
					{
						SelectTool.Instance.Select(null, false);
					}
					else
					{
						CameraController.Instance.ForcePanningState(false);
						this.TogglePauseScreen();
					}
				}
				else
				{
					Game.Instance.Trigger(288942073, null);
				}
				ToolMenu.Instance.ClearSelection();
				SelectTool.Instance.Activate();
			}
		}
		base.OnKeyDown(e);
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		base.OnKeyUp(e);
		if (!e.Consumed && e.TryConsume(global::Action.AlternateView) && this.tileScreenInst != null)
		{
			this.tileScreenInst.Deactivate();
			this.tileScreenInst = null;
		}
	}

	public void TogglePauseScreen()
	{
		PauseScreen.Instance.Show(true);
		AudioMixer.instance.Start(AudioMixerSnapshots.Get().ESCPauseSnapshot);
		MusicManager.instance.OnEscapeMenu(true);
		MusicManager.instance.PlaySong("Music_ESC_Menu", false);
	}

	public void ExternalClose()
	{
		this.OnClickCommon();
	}

	private void OnUIClear(object data)
	{
		this.CloseSubMenus();
		if (global::UnityEngine.EventSystems.EventSystem.current != null)
		{
			global::UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
		}
		else
		{
			global::Debug.LogWarning("OnUIClear() Event system is null", null);
		}
	}

	protected override void OnActivate()
	{
		base.OnActivate();
	}

	private bool AreSubMenusOpen()
	{
		return this.subMenus.Count > 0;
	}

	private KToggleMenu.ToggleInfo[] GetFillers()
	{
		HashSet<Tag> hashSet = new HashSet<Tag>();
		List<KToggleMenu.ToggleInfo> list = new List<KToggleMenu.ToggleInfo>();
		foreach (Pickupable pickupable in Components.Pickupables)
		{
			KPrefabID kprefabID = pickupable.KPrefabID;
			if (kprefabID.HasTag(GameTags.Filler) && hashSet.Add(kprefabID.PrefabTag))
			{
				string text = kprefabID.GetComponent<PrimaryElement>().Element.id.ToString();
				list.Add(new KToggleMenu.ToggleInfo(text, null, global::Action.NumActions));
			}
		}
		return list.ToArray();
	}

	private DetailsScreen detailsScreen;

	private UserMenuScreen userMenu;

	[SerializeField]
	private UserMenuScreen userMenuPrefab;

	private GameObject userMenuParent;

	[SerializeField]
	private TileScreen tileScreen;

	public KScreen buildMenu;

	private List<KScreen> subMenus = new List<KScreen>();

	private TileScreen tileScreenInst;

	public GameObject selectedGO;
}
