using System;

public class HEPBridgeTileVisualizer : KMonoBehaviour, IHighEnergyParticleDirection
{
	protected override void OnSpawn()
	{
		base.Subscribe<HEPBridgeTileVisualizer>(-1643076535, HEPBridgeTileVisualizer.OnRotateDelegate);
		this.OnRotate();
	}

	public void OnRotate()
	{
		Game.Instance.ForceOverlayUpdate(true);
	}

	public EightDirection Direction
	{
		get
		{
			EightDirection eightDirection = EightDirection.Right;
			Rotatable component = base.GetComponent<Rotatable>();
			if (component != null)
			{
				switch (component.Orientation)
				{
				case Orientation.Neutral:
					eightDirection = EightDirection.Left;
					break;
				case Orientation.R90:
					eightDirection = EightDirection.Up;
					break;
				case Orientation.R180:
					eightDirection = EightDirection.Right;
					break;
				case Orientation.R270:
					eightDirection = EightDirection.Down;
					break;
				}
			}
			return eightDirection;
		}
		set
		{
		}
	}

	private static readonly EventSystem.IntraObjectHandler<HEPBridgeTileVisualizer> OnRotateDelegate = new EventSystem.IntraObjectHandler<HEPBridgeTileVisualizer>(delegate(HEPBridgeTileVisualizer component, object data)
	{
		component.OnRotate();
	});
}
