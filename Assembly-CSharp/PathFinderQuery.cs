using System;

public class PathFinderQuery
{
	public virtual bool IsMatch(int cell, int parent_cell, int cost)
	{
		return true;
	}

	public virtual bool CanTraverse(int cell, int from_cell, int cost, int underwater_cost)
	{
		return true;
	}

	public void SetResult(int cell, int cost, NavType nav_type)
	{
		this.resultCell = cell;
		this.resultNavType = nav_type;
	}

	public void ClearResult()
	{
		this.resultCell = -1;
	}

	public virtual int GetResultCell()
	{
		return this.resultCell;
	}

	public NavType GetResultNavType()
	{
		return this.resultNavType;
	}

	protected int resultCell;

	private NavType resultNavType;
}
