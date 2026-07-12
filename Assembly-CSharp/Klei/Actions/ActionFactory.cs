using System;
using System.Collections.Generic;

namespace Klei.Actions
{
	public class ActionFactory<ActionFactoryType, ActionType, EnumType> where ActionFactoryType : ActionFactory<ActionFactoryType, ActionType, EnumType>
	{
		public static ActionType GetOrCreateAction(EnumType actionType)
		{
			ActionType actionType2;
			if (!ActionFactory<ActionFactoryType, ActionType, EnumType>.actionInstances.TryGetValue(actionType, out actionType2))
			{
				ActionFactory<ActionFactoryType, ActionType, EnumType>.EnsureFactoryInstance();
				actionType2 = (ActionFactory<ActionFactoryType, ActionType, EnumType>.actionInstances[actionType] = ActionFactory<ActionFactoryType, ActionType, EnumType>.actionFactory.CreateAction(actionType));
			}
			return actionType2;
		}

		private static void EnsureFactoryInstance()
		{
			if (ActionFactory<ActionFactoryType, ActionType, EnumType>.actionFactory != null)
			{
				return;
			}
			ActionFactory<ActionFactoryType, ActionType, EnumType>.actionFactory = Activator.CreateInstance(typeof(ActionFactoryType)) as ActionFactoryType;
		}

		protected virtual ActionType CreateAction(EnumType actionType)
		{
			throw new InvalidOperationException("Can not call InterfaceToolActionFactory<T1, T2>.CreateAction()! This function must be called from a deriving class!");
		}

		private static Dictionary<EnumType, ActionType> actionInstances = new Dictionary<EnumType, ActionType>();

		private static ActionFactoryType actionFactory = default(ActionFactoryType);
	}
}
