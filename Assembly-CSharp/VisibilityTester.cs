using System;
using UnityEngine;

public class VisibilityTester : KMonoBehaviour
{
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
		string text = string.Empty;
		string text2 = text;
		text = string.Concat(new object[] { text2, "Source Cell: ", num, "\n" });
		text2 = text;
		text = string.Concat(new object[] { text2, "Target Cell: ", mouseCell, "\n" });
		text = text + "Visible: " + Grid.VisibilityTest(num, mouseCell);
		for (int i = 0; i < 10000; i++)
		{
			Grid.VisibilityTest(num, mouseCell);
		}
		DebugText.Instance.Draw(text, Grid.CellToPosCCC(mouseCell, Grid.SceneLayer.Move), Color.white);
	}

	public static VisibilityTester Instance;

	public bool enableTesting;
}
