using System;
using System.Collections.Generic;
using Klei.Actions;
using UnityEngine;

namespace Klei.Input
{
	[CreateAssetMenu(fileName = "InterfaceToolConfig", menuName = "Klei/Interface Tools/Config")]
	public class InterfaceToolConfig : ScriptableObject
	{
		public DigAction DigAction
		{
			get
			{
				return ActionFactory<DigToolActionFactory, DigAction, DigToolActionFactory.Actions>.GetOrCreateAction(this.digAction);
			}
		}

		public int Priority
		{
			get
			{
				return this.priority;
			}
		}

		public global::Action InputAction
		{
			get
			{
				return this.inputAction;
			}
		}

		[SerializeField]
		private DigToolActionFactory.Actions digAction;

		public static InterfaceToolConfig.Comparer ConfigComparer = new InterfaceToolConfig.Comparer();

		[SerializeField]
		[Tooltip("Defines which config will take priority should multiple configs be activated\n0 is the lower bound for this value.")]
		private int priority;

		[SerializeField]
		[Tooltip("This will serve as a key for activating different configs. Currently, these Actionsare how we indicate that different input modes are desired.\nAssigning Action.Invalid to this field will indicate that this is the \"default\" config")]
		private global::Action inputAction;

		public class Comparer : IComparer<InterfaceToolConfig>
		{
			public int Compare(InterfaceToolConfig lhs, InterfaceToolConfig rhs)
			{
				if (lhs.Priority == rhs.Priority)
				{
					return 0;
				}
				if (lhs.Priority <= rhs.Priority)
				{
					return -1;
				}
				return 1;
			}
		}
	}
}
