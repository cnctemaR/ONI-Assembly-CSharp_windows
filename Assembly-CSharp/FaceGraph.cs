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

	public void AddExpression(Expression expression)
	{
		if (this.expressions.Contains(expression))
		{
			return;
		}
		this.expressions.Add(expression);
		this.UpdateFace();
	}

	public void RemoveExpression(Expression expression)
	{
		if (this.expressions.Remove(expression))
		{
			this.UpdateFace();
		}
	}

	public void SetOverrideExpression(Expression expression)
	{
		if (expression != this.overrideExpression)
		{
			this.overrideExpression = expression;
			this.UpdateFace();
		}
	}

	public void ApplyShape()
	{
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		Accessorizer component2 = base.GetComponent<Accessorizer>();
		KAnimFile anim = Assets.GetAnim("head_master_swap_kanim");
		this.ApplyShape(component2.GetAccessory(Db.Get().AccessorySlots.Eyes).symbol, component, anim, "snapto_eyes");
		this.ApplyShape(component2.GetAccessory(Db.Get().AccessorySlots.Mouth).symbol, component, anim, "snapto_mouth");
	}

	private void ApplyShape(KAnim.Build.Symbol variation_symbol, KBatchedAnimController controller, KAnimFile shapes_file, HashedString symbol_name_in_shape_file)
	{
		HashedString hashedString = "neutral";
		if (this.currentExpression != null)
		{
			hashedString = this.currentExpression.face.hash;
		}
		KAnim.Anim anim = null;
		KAnim.Anim.FrameElement frameElement = default(KAnim.Anim.FrameElement);
		bool flag = false;
		int num = 0;
		while (num < shapes_file.GetData().animCount && !flag)
		{
			KAnim.Anim anim2 = shapes_file.GetData().GetAnim(num);
			if (anim2.hash == hashedString)
			{
				anim = anim2;
				KAnim.Anim.Frame frame = anim.GetFrame(shapes_file.GetData().build.batchTag, 0);
				for (int i = 0; i < frame.numElements; i++)
				{
					KBatchGroupData batchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(shapes_file.GetData().animBatchTag);
					frameElement = batchGroupData.GetFrameElement(frame.firstElementIdx + i);
					if (!(frameElement.symbol != symbol_name_in_shape_file))
					{
						flag = true;
						break;
					}
				}
			}
			num++;
		}
		if (anim == null)
		{
			DebugUtil.Assert(false, "Could not find shape for expression: " + HashCache.Get().Get(hashedString));
		}
		if (!flag)
		{
			DebugUtil.Assert(false, "Could not find shape element for shape:" + HashCache.Get().Get(variation_symbol.hash));
		}
		KAnim.Build.Symbol symbol = KAnimBatchManager.Instance().GetBatchGroupData(controller.batchGroupID).GetSymbol(symbol_name_in_shape_file);
		KBatchGroupData batchGroupData2 = KAnimBatchManager.Instance().GetBatchGroupData(variation_symbol.build.batchTag);
		KAnim.Build.SymbolFrameInstance symbolFrameInstance = batchGroupData2.symbolFrameInstances[variation_symbol.firstFrameIdx + frameElement.frame];
		symbolFrameInstance.buildImageIdx = base.GetComponent<SymbolOverrideController>().GetAtlasIdx(variation_symbol.build.GetTexture(0));
		controller.SetSymbolOverride(symbol.firstFrameIdx, symbolFrameInstance);
	}

	private void UpdateFace()
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
			this.currentExpression = expression;
			base.GetComponent<SymbolOverrideController>().MarkDirty();
		}
	}

	public Expression GetCurrentExpression()
	{
		return this.currentExpression;
	}

	private List<Expression> expressions = new List<Expression>();
}
