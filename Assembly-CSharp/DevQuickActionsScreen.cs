using System;
using System.Collections.Generic;
using UnityEngine;

public class DevQuickActionsScreen : MonoBehaviour
{
	public static void DestroyInstance()
	{
		DevQuickActionsScreen.Instance = null;
	}

	private void Awake()
	{
		DevQuickActionsScreen.Instance = this;
		this.Pointer.SetVisibleState(false);
		DevQuickActionTargetFollower pointer = this.Pointer;
		pointer.OnToggleChanged = (Action<bool>)Delegate.Combine(pointer.OnToggleChanged, new Action<bool>(this.OnPointerToggleClicked));
		this.originalEndNode.gameObject.SetActive(false);
		this.originalCategoryDevNode.gameObject.SetActive(false);
	}

	private void OnPointerToggleClicked(bool val)
	{
		if (this.Target != null && this.RootNode != null)
		{
			if (val)
			{
				this.RootNode.Expand();
				return;
			}
			this.RootNode.Collapse();
		}
	}

	public void Toggle(GameObject target)
	{
		if (target == null)
		{
			this.Close();
			return;
		}
		if (this.Target != target)
		{
			this.Open(target);
			return;
		}
		this.Close();
	}

	public void Open(GameObject target)
	{
		if (this.Target != null && this.Target != target)
		{
			this.Close();
		}
		this.Target = target;
		if (target == null)
		{
			return;
		}
		Vector3 vector = CameraController.Instance.overlayCamera.WorldToScreenPoint(target.transform.position);
		this.RootNode = this.GetUnsedCategoryNode();
		this.RootNode.Setup(target.GetProperName(), null);
		this.RootNode.transform.SetPosition(vector);
		this.RootNode.SetChildrenSeparationSpace(50f);
		this.Target.Subscribe(1502190696, new Action<object>(this.OnTargetLost));
		List<IDevQuickAction> list = new List<IDevQuickAction>(this.Target.GetComponents<IDevQuickAction>());
		list.AddRange(this.Target.GetAllSMI<IDevQuickAction>());
		foreach (IDevQuickAction devQuickAction in list)
		{
			foreach (DevQuickActionInstruction devQuickActionInstruction in devQuickAction.GetDevInstructions())
			{
				string[] array = devQuickActionInstruction.Address.Split('/', StringSplitOptions.None);
				DevQuickActionCategoryNode devQuickActionCategoryNode = this.RootNode;
				for (int i = 0; i < array.Length; i++)
				{
					string text = array[i];
					if (i < array.Length - 1)
					{
						DevQuickActionCategoryNode devQuickActionCategoryNode2 = null;
						if (!this.registeredCategoryNodes.TryGetValue(text, out devQuickActionCategoryNode2))
						{
							devQuickActionCategoryNode2 = this.GetUnsedCategoryNode();
							devQuickActionCategoryNode2.Setup(text, devQuickActionCategoryNode);
							this.registeredCategoryNodes.Add(text, devQuickActionCategoryNode2);
							devQuickActionCategoryNode.AddChildren(devQuickActionCategoryNode2);
						}
						devQuickActionCategoryNode = devQuickActionCategoryNode2;
					}
					else
					{
						DevQuickActionEndNode unsedEndNode = this.GetUnsedEndNode();
						unsedEndNode.Setup(text, devQuickActionCategoryNode, devQuickActionInstruction.Action);
						devQuickActionCategoryNode.AddChildren(unsedEndNode);
						unsedEndNode.gameObject.SetActive(false);
					}
				}
			}
		}
		this.RootNode.Collapse();
		if (this.Pointer.IsToggleOn)
		{
			this.RootNode.Expand();
		}
		this.RootNode.gameObject.SetActive(false);
		this.Pointer.transform.position = this.RootNode.transform.position;
		this.Pointer.SetTarget(this.Target);
		this.Pointer.SetVisibleState(true);
	}

	public void Close()
	{
		if (this.Target != null)
		{
			this.Target.Unsubscribe(1502190696, new Action<object>(this.OnTargetLost));
		}
		this.Target = null;
		if (this.RootNode != null)
		{
			this.RootNode.Recycle();
			this.RootNode = null;
		}
		this.registeredCategoryNodes.Clear();
		this.Pointer.SetTarget(null);
		this.Pointer.SetVisibleState(false);
	}

	private void OnTargetLost(object o)
	{
		this.Close();
	}

	private DevQuickActionEndNode GetUnsedEndNode()
	{
		DevQuickActionEndNode devQuickActionEndNode = null;
		if (!this.recycledEndNodes.TryPop(out devQuickActionEndNode))
		{
			devQuickActionEndNode = Util.KInstantiateUI(this.originalEndNode.gameObject, this.originalEndNode.transform.parent.gameObject, false).GetComponent<DevQuickActionEndNode>();
		}
		this.SetupUnusedNodeForUse(devQuickActionEndNode);
		return devQuickActionEndNode;
	}

	private DevQuickActionCategoryNode GetUnsedCategoryNode()
	{
		DevQuickActionCategoryNode devQuickActionCategoryNode = null;
		if (!this.recycledCategoriesNodes.TryPop(out devQuickActionCategoryNode))
		{
			devQuickActionCategoryNode = Util.KInstantiateUI(this.originalCategoryDevNode.gameObject, this.originalCategoryDevNode.transform.parent.gameObject, false).GetComponent<DevQuickActionCategoryNode>();
		}
		this.SetupUnusedNodeForUse(devQuickActionCategoryNode);
		return devQuickActionCategoryNode;
	}

	private void SetupUnusedNodeForUse(DevQuickActionNode node)
	{
		node.OnRecycle = new Action<DevQuickActionNode>(this.OnNodeRecycled);
		node.SetChildrenSeparationSpace(60f);
		node.gameObject.SetActive(true);
	}

	private void OnNodeRecycled(DevQuickActionNode node)
	{
		if (node is DevQuickActionCategoryNode)
		{
			this.recycledCategoriesNodes.Push(node as DevQuickActionCategoryNode);
			return;
		}
		if (node is DevQuickActionEndNode)
		{
			this.recycledEndNodes.Push(node as DevQuickActionEndNode);
		}
	}

	public const float DEFAULT_SPACE = 60f;

	public const float ROOT_SPACE = 50f;

	public const char CATEGORY_DIVIDER = '/';

	public DevQuickActionNode originalCategoryDevNode;

	public DevQuickActionNode originalEndNode;

	public DevQuickActionTargetFollower Pointer;

	public Stack<DevQuickActionEndNode> recycledEndNodes = new Stack<DevQuickActionEndNode>();

	public Stack<DevQuickActionCategoryNode> recycledCategoriesNodes = new Stack<DevQuickActionCategoryNode>();

	private Dictionary<string, DevQuickActionCategoryNode> registeredCategoryNodes = new Dictionary<string, DevQuickActionCategoryNode>();

	private GameObject Target;

	private DevQuickActionCategoryNode RootNode;

	public static DevQuickActionsScreen Instance;
}
