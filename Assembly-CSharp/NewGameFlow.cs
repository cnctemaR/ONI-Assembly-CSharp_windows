using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/NewGameFlow")]
public class NewGameFlow : KMonoBehaviour
{
	public void BeginFlow()
	{
		this.currentScreenIndex = -1;
		this.Next();
	}

	private void Next()
	{
		this.ClearCurrentScreen();
		this.currentScreenIndex++;
		this.ActivateCurrentScreen();
	}

	private void Previous()
	{
		this.ClearCurrentScreen();
		this.currentScreenIndex--;
		this.ActivateCurrentScreen();
	}

	private void ClearCurrentScreen()
	{
		if (this.currentScreen != null)
		{
			this.currentScreen.Deactivate();
			this.currentScreen = null;
		}
	}

	private void ActivateCurrentScreen()
	{
		if (this.currentScreenIndex >= 0 && this.currentScreenIndex < this.newGameFlowScreens.Count)
		{
			NewGameFlowScreen newGameFlowScreen = Util.KInstantiateUI<NewGameFlowScreen>(this.newGameFlowScreens[this.currentScreenIndex].gameObject, base.transform.parent.gameObject, true);
			newGameFlowScreen.OnNavigateForward += this.Next;
			newGameFlowScreen.OnNavigateBackward += this.Previous;
			if (!newGameFlowScreen.IsActive() && !newGameFlowScreen.activateOnSpawn)
			{
				newGameFlowScreen.Activate();
			}
			this.currentScreen = newGameFlowScreen;
		}
	}

	public List<NewGameFlowScreen> newGameFlowScreens;

	private int currentScreenIndex = -1;

	private NewGameFlowScreen currentScreen;
}
