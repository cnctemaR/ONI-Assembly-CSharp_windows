using System;
using System.Collections.Generic;
using UnityEngine;

public class ContextMenuScreen : KScreen
{
	public int TargetCell
	{
		get
		{
			return this.targetCell;
		}
	}

	public void SetPortrait(MinionIdentity identity)
	{
		if (this.portrait == null)
		{
			this.portrait = this.portraitContainer.GetComponent<CrewPortrait>();
			if (this.portrait == null)
			{
				GameObject gameObject = Util.KInstantiateUI(this.portraitPrefab, this.buttonContainer.gameObject, true);
				gameObject.transform.SetAsFirstSibling();
				this.portrait = gameObject.GetComponent<CrewPortrait>();
			}
		}
		this.portrait.SetCrewMember(identity, true);
	}

	public void SetTargets(KSelectable[] targets, Worker worker)
	{
		this.worker = worker;
		foreach (ContextMenuButton contextMenuButton in this.buttons)
		{
			global::UnityEngine.Object.Destroy(contextMenuButton.gameObject);
		}
		this.buttons.Clear();
		ChoreConsumer component = worker.GetComponent<ChoreConsumer>();
		PriorityCommandCollector priorityCommandCollector = new PriorityCommandCollector(component);
		List<Chore.Precondition.Context> list = new List<Chore.Precondition.Context>();
		foreach (KSelectable kselectable in targets)
		{
			foreach (Chore chore in GlobalChoreProvider.Instance)
			{
				if (chore.gameObject == kselectable.gameObject && chore.allowInContextMenu)
				{
					chore.CollectChores(component, list, true);
				}
			}
			this.targetWorldPosition = kselectable.transform.position;
			this.targetCell = Grid.PosToCell(this.targetWorldPosition);
			kselectable.Trigger(809822742, priorityCommandCollector);
		}
		foreach (Chore.Precondition.Context context in list)
		{
			priorityCommandCollector.commands.Add(new PriorityChoreCommand(context));
		}
		foreach (PriorityCommand priorityCommand in priorityCommandCollector.commands)
		{
			ContextMenuButton component2 = Util.KInstantiate(this.buttonPrefab, this.buttonContainer.gameObject, null).GetComponent<ContextMenuButton>();
			component2.gameObject.SetActive(true);
			this.choresAvailable = true;
			component2.SetCommand(this, priorityCommand);
			component2.SetOnClick(new global::System.Action(component2.ExecuteTask));
			this.buttons.Add(component2);
		}
		ManualControlMonitor.Instance smi = worker.GetSMI<ManualControlMonitor.Instance>();
		ContextMenuButton component3 = this.MoveHereButton.GetComponent<ContextMenuButton>();
		if (smi.IsControlled())
		{
			if (smi.CanReachPosition(Grid.CellToPos(this.targetCell)))
			{
				this.choresAvailable = true;
				component3.SetMenuScreen(this);
				component3.SetLabel("Move here");
				this.MoveHereButton.SetActive(true);
				this.MoveHereButton.transform.SetAsLastSibling();
				component3.SetOnClick(new global::System.Action(component3.MoveAction));
			}
			else
			{
				component3.SetMenuScreen(this);
				component3.SetLabel("Unreachable");
				component3.SetOnClick(delegate
				{
					KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Negative", false));
				});
				this.MoveHereButton.SetActive(true);
				this.MoveHereButton.transform.SetAsLastSibling();
			}
		}
		else
		{
			this.MoveHereButton.SetActive(false);
		}
		this.UpdatePosition();
		if (!this.choresAvailable)
		{
			this.NoTaskButton.GetComponent<ContextMenuButton>().SetMenuScreen(this);
			this.NoTaskButton.SetActive(true);
			this.NoTaskButton.GetComponentInChildren<LocText>().text = "No tasks available";
		}
		else
		{
			this.NoTaskButton.SetActive(false);
		}
	}

	private void UpdatePosition()
	{
		Vector2 vector = Camera.main.WorldToScreenPoint(this.targetWorldPosition);
		this.transform.position = vector;
	}

	private void Update()
	{
		this.UpdatePosition();
	}

	public void Clear()
	{
		if (this.portrait != null)
		{
			global::UnityEngine.Object.Destroy(this.portrait.gameObject);
			this.portrait = null;
		}
		foreach (ContextMenuButton contextMenuButton in this.buttons)
		{
			global::UnityEngine.Object.Destroy(contextMenuButton.gameObject);
		}
		this.buttons.Clear();
		base.Show(false);
	}

	public void OnClick(PriorityCommand command)
	{
		PrioritizedChoreMonitor.Instance smi = this.worker.GetSMI<PrioritizedChoreMonitor.Instance>();
		smi.SetCommand(command);
		BrainScheduler.Get().Prioritize(this.worker.GetComponent<Brain>());
		this.Deactivate();
	}

	public void OrderMove()
	{
		Vector3 vector = Grid.CellToPos(this.targetCell);
		ManualControlMonitor.Instance smi = this.worker.GetSMI<ManualControlMonitor.Instance>();
		if (smi != null && smi.IsControlled() && smi.CanReachPosition(vector))
		{
			smi.SetDestination(Grid.CellToPos(this.targetCell));
		}
		this.Deactivate();
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.IsAction(global::Action.MouseRight) || e.TryConsume(global::Action.Escape))
		{
			this.Clear();
		}
		else
		{
			base.OnKeyDown(e);
		}
	}

	public Transform buttonContainer;

	public ContextMenuButton buttonPrefab;

	public GameObject NoTaskButton;

	public GameObject MoveHereButton;

	public Transform portraitContainer;

	public CrewPortrait portrait;

	public GameObject portraitPrefab;

	private List<ContextMenuButton> buttons = new List<ContextMenuButton>();

	private Vector3 targetWorldPosition;

	private int targetCell;

	private Worker worker;

	private bool choresAvailable;
}
