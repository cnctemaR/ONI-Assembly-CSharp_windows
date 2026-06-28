using System;
using System.Collections.Generic;

public class FaceGraph : KMonoBehaviour
{
	public IEnumerator<Expression> GetEnumerator()
	{
		return this.expressions.GetEnumerator();
	}

	public Expression overrideExpression { get; private set; }

	public Expression currentExpression { get; private set; }

	public KCompBuildInstance headComp { get; private set; }

	public void AddExpression(Expression expression)
	{
		if (!this.expressions.Contains(expression))
		{
			this.expressions.Add(expression);
			this.UpdateFace();
		}
	}

	public void RemoveExpression(Expression expression)
	{
		if (this.expressions.Remove(expression))
		{
			this.UpdateFace();
		}
	}

	public void SetHeadComp(KCompBuildInstance head_comp)
	{
		this.headComp = head_comp;
		this.UpdateFace();
	}

	public void SetOverrideExpression(Expression expression)
	{
		if (expression != this.overrideExpression)
		{
			this.overrideExpression = expression;
			this.UpdateFace();
		}
	}

	private void UpdateFace()
	{
		if (this.headComp != null)
		{
			Expression expression = null;
			if (this.overrideExpression != null)
			{
				expression = this.overrideExpression;
			}
			else if (this.expressions.Count > 0)
			{
				this.expressions.Sort((Expression a, Expression b) => b.priority.CompareTo(a.priority));
				expression = this.expressions[0];
			}
			if (expression != this.currentExpression || expression == null)
			{
				if (expression != null)
				{
					this.headComp.SetAnimOverride(expression.face.hash);
				}
				else
				{
					this.headComp.ClearAnimOverride();
				}
				this.currentExpression = expression;
			}
		}
	}

	public Expression GetCurrentExpression()
	{
		return this.currentExpression;
	}

	public KCompBuildInstance GetHeadComp()
	{
		return this.headComp;
	}

	private List<Expression> expressions = new List<Expression>();
}
