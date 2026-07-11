using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements
{
	public abstract class MouseManipulator : Manipulator
	{
		protected MouseManipulator()
		{
			this.activators = new List<ManipulatorActivationFilter>();
		}

		public List<ManipulatorActivationFilter> activators { get; private set; }

		protected bool CanStartManipulation(IMouseEvent e)
		{
			foreach (ManipulatorActivationFilter manipulatorActivationFilter in this.activators)
			{
				if (manipulatorActivationFilter.Matches(e))
				{
					this.m_currentActivator = manipulatorActivationFilter;
					return true;
				}
			}
			return false;
		}

		protected bool CanStopManipulation(IMouseEvent e)
		{
			return e.button == (int)this.m_currentActivator.button;
		}

		private ManipulatorActivationFilter m_currentActivator;
	}
}
