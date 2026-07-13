using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	public abstract class MouseManipulator : Manipulator
	{
		public List<ManipulatorActivationFilter> activators { get; private set; }

		protected MouseManipulator()
		{
			this.activators = new List<ManipulatorActivationFilter>();
		}

		protected bool CanStartManipulation(IMouseEvent e)
		{
			foreach (ManipulatorActivationFilter manipulatorActivationFilter in this.activators)
			{
				bool flag = manipulatorActivationFilter.Matches(e);
				if (flag)
				{
					this.m_currentActivator = manipulatorActivationFilter;
					return true;
				}
			}
			return false;
		}

		protected bool CanStopManipulation(IMouseEvent e)
		{
			bool flag = e == null;
			return !flag && e.button == (int)this.m_currentActivator.button;
		}

		private ManipulatorActivationFilter m_currentActivator;
	}
}
