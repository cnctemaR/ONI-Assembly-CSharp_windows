using System;

public class GrowthState : KMonoBehaviour, ISim200ms
{
	public int Maturity
	{
		get
		{
			return this.maturity;
		}
		protected set
		{
			this.maturity = value;
		}
	}

	public void Sim200ms(float dt)
	{
		if (this.CanGrow && this.GrowingEnabled)
		{
			this.Grow(dt);
		}
		this.SyncTemp();
	}

	private void SyncTemp()
	{
		int num = Grid.PosToCell(base.transform.GetPosition());
		float num2 = 0f;
		num2 += Grid.Temperature[Grid.CellDownLeft(num)] * (Grid.Cell[Grid.CellDownLeft(num)].mass / 280f);
		num2 += Grid.Temperature[Grid.CellDownRight(num)] * (Grid.Cell[Grid.CellDownRight(num)].mass / 280f);
		num2 += Grid.Temperature[Grid.CellUpLeft(num)] * (Grid.Cell[Grid.CellUpLeft(num)].mass / 280f);
		num2 += Grid.Temperature[Grid.CellUpRight(num)] * (Grid.Cell[Grid.CellUpRight(num)].mass / 280f);
		num2 += Grid.Temperature[Grid.CellRight(num)] * (Grid.Cell[Grid.CellRight(num)].mass / 280f);
		num2 += Grid.Temperature[Grid.CellLeft(num)] * (Grid.Cell[Grid.CellLeft(num)].mass / 280f);
		num2 += Grid.Temperature[Grid.CellBelow(num)] * (Grid.Cell[Grid.CellBelow(num)].mass / 280f);
		num2 += Grid.Temperature[Grid.CellAbove(num)] * (Grid.Cell[Grid.CellAbove(num)].mass / 280f);
		num2 /= 8f;
		this.temperature = num2;
	}

	public void ForceMaturity(int newMaturity)
	{
		this.maturity = newMaturity;
	}

	private void Grow(float dt)
	{
		this.timeInMaturityState += dt;
		if (this.timeInMaturityState >= this.maturityStateDuration)
		{
			if (this.maturity < this.maxMaturity)
			{
				this.maturity++;
			}
			this.timeInMaturityState = 0f;
			base.gameObject.Trigger(-2116516046, null);
		}
	}

	public void Regress(int numberOfStages)
	{
		this.maturity -= numberOfStages;
		if (this.maturity < 0)
		{
			this.maturity = 0;
		}
		this.timeInMaturityState = 0f;
	}

	public float temperature;

	public bool CanGrow = true;

	public bool GrowingEnabled = true;

	public int maxMaturity = 10;

	private int maturity;

	private bool isDead;

	private float timeInMaturityState;

	public float maturityStateDuration = 10f;
}
