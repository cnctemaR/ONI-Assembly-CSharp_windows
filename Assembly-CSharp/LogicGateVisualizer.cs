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
		this.visChildren.Add(new LogicGateVisualizer.IOVisualizer(base.OutputCellOne, false));
		if (base.RequiresFourOutputs)
		{
			this.visChildren.Add(new LogicGateVisualizer.IOVisualizer(base.OutputCellTwo, false));
			this.visChildren.Add(new LogicGateVisualizer.IOVisualizer(base.OutputCellThree, false));
			this.visChildren.Add(new LogicGateVisualizer.IOVisualizer(base.OutputCellFour, false));
		}
		this.visChildren.Add(new LogicGateVisualizer.IOVisualizer(base.InputCellOne, true));
		if (base.RequiresTwoInputs)
		{
			this.visChildren.Add(new LogicGateVisualizer.IOVisualizer(base.InputCellTwo, true));
		}
		else if (base.RequiresFourInputs)
		{
			this.visChildren.Add(new LogicGateVisualizer.IOVisualizer(base.InputCellTwo, true));
			this.visChildren.Add(new LogicGateVisualizer.IOVisualizer(base.InputCellThree, true));
			this.visChildren.Add(new LogicGateVisualizer.IOVisualizer(base.InputCellFour, true));
		}
		if (base.RequiresControlInputs)
		{
			this.visChildren.Add(new LogicGateVisualizer.IOVisualizer(base.ControlCellOne, true));
			this.visChildren.Add(new LogicGateVisualizer.IOVisualizer(base.ControlCellTwo, true));
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

		public LogicPortSpriteType GetLogicPortSpriteType()
		{
			if (!this.input)
			{
				return LogicPortSpriteType.Output;
			}
			return LogicPortSpriteType.Input;
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
