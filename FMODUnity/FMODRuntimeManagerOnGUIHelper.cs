using System;
using UnityEngine;

namespace FMODUnity
{
	public class FMODRuntimeManagerOnGUIHelper : MonoBehaviour
	{
		private void OnGUI()
		{
			if (this.TargetRuntimeManager)
			{
				this.TargetRuntimeManager.ExecuteOnGUI();
			}
		}

		public RuntimeManager TargetRuntimeManager;
	}
}
