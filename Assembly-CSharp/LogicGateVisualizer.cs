using System;
using System.Collections.Generic;
using UnityEngine;

[SkipSaveFileSerialization]
public class LogicGateVisualizer : LogicGateBase
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.Register();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		this.Unregister();
	}

	private void Register()
	{
		this.Unregister();
		this.visChildren.Add(new LogicGateVisualizer.IOVisualizer(base.OutputCell, false));
		this.visChildren.Add(new LogicGateVisualizer.IOVisualizer(base.InputCellOne, true));
		if (base.RequiresTwoInputs)
		{
			this.visChildren.Add(new LogicGateVisualizer.IOVisualizer(base.InputCellTwo, true));
		}
		LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
		foreach (LogicGateVisualizer.IOVisualizer iovisualizer in this.visChildren)
		{
			logicCircuitManager.AddVisElem(iovisualizer);
		}
	}

	private void Unregister()
	{
		LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
		foreach (LogicGateVisualizer.IOVisualizer iovisualizer in this.visChildren)
		{
			logicCircuitManager.RemoveVisElem(iovisualizer);
		}
		this.visChildren.Clear();
	}

	private List<LogicGateVisualizer.IOVisualizer> visChildren = new List<LogicGateVisualizer.IOVisualizer>();

	private class IOVisualizer : ILogicUIElement, IUniformGridObject
	{
		public IOVisualizer(int cell, bool input)
		{
			this.cell = cell;
			this.input = input;
		}

		public int GetLogicUICell()
		{
			return this.cell;
		}

		public bool IsLogicInput()
		{
			return this.input;
		}

		public Vector2 PosMin()
		{
			return Grid.CellToPos2D(this.cell);
		}

		public Vector2 PosMax()
		{
			return this.PosMin();
		}

		private int cell;

		private bool input;
	}
}
