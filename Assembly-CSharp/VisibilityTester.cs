using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/VisibilityTester")]
public class VisibilityTester : KMonoBehaviour
{
	public static void DestroyInstance()
	{
		VisibilityTester.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		VisibilityTester.Instance = this;
	}

	private void Update()
	{
		if (SelectTool.Instance == null || SelectTool.Instance.selected == null || !this.enableTesting)
		{
			return;
		}
		int num = Grid.PosToCell(SelectTool.Instance.selected);
		int mouseCell = DebugHandler.GetMouseCell();
		string text = "";
		text = string.Concat(new object[] { text, "Source Cell: ", num, "\n" });
		text = string.Concat(new object[] { text, "Target Cell: ", mouseCell, "\n" });
		text = text + "Visible: " + Grid.VisibilityTest(num, mouseCell, false).ToString();
		for (int i = 0; i < 10000; i++)
		{
			Grid.VisibilityTest(num, mouseCell, false);
		}
		DebugText.Instance.Draw(text, Grid.CellToPosCCC(mouseCell, Grid.SceneLayer.Move), Color.white);
	}

	public static VisibilityTester Instance;

	public bool enableTesting;
}
