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
		bool flag = this.ShouldUseSidewaysSymbol(component);
		BlinkMonitor.Instance smi = component2.GetSMI<BlinkMonitor.Instance>();
		if (smi.IsNullOrStopped() || !smi.IsBlinking())
		{
			KAnim.Build.Symbol symbol = component2.GetAccessory(Db.Get().AccessorySlots.Eyes).symbol;
			this.ApplyShape(symbol, component, anim, "snapto_eyes", flag);
		}
		SpeechMonitor.Instance smi2 = component2.GetSMI<SpeechMonitor.Instance>();
		if (smi2.IsNullOrStopped() || !smi2.IsPlayingSpeech())
		{
			KAnim.Build.Symbol symbol2 = component2.GetAccessory(Db.Get().AccessorySlots.Mouth).symbol;
			this.ApplyShape(symbol2, component, anim, "snapto_mouth", flag);
			return;
		}
		smi2.DrawMouth();
	}

	private bool ShouldUseSidewaysSymbol(KBatchedAnimController controller)
	{
		KAnim.Anim currentAnim = controller.GetCurrentAnim();
		if (currentAnim == null)
		{
			return false;
		}
		int currentFrameIndex = controller.GetCurrentFrameIndex();
		if (currentFrameIndex <= 0)
		{
			return false;
		}
		KBatchGroupData batchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(currentAnim.animFile.animBatchTag);
		KAnim.Anim.Frame frame = batchGroupData.GetFrame(currentFrameIndex);
		for (int i = 0; i < frame.numElements; i++)
		{
			KAnim.Anim.FrameElement frameElement = batchGroupData.GetFrameElement(frame.firstElementIdx + i);
			if (frameElement.symbol == FaceGraph.HASH_SNAPTO_EYES && frameElement.frame >= FaceGraph.FIRST_SIDEWAYS_FRAME)
			{
				return true;
			}
		}
		return false;
	}

	private void ApplyShape(KAnim.Build.Symbol variation_symbol, KBatchedAnimController controller, KAnimFile shapes_file, HashedString symbol_name_in_shape_file, bool should_use_sideways_symbol)
	{
		HashedString hashedString = FaceGraph.HASH_NEUTRAL;
		if (this.currentExpression != null)
		{
			hashedString = this.currentExpression.face.hash;
		}
		KAnim.Anim anim = null;
		KAnim.Anim.FrameElement frameElement = default(KAnim.Anim.FrameElement);
		bool flag = false;
		bool flag2 = false;
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
					frameElement = KAnimBatchManager.Instance().GetBatchGroupData(shapes_file.GetData().animBatchTag).GetFrameElement(frame.firstElementIdx + i);
					if (!(frameElement.symbol != symbol_name_in_shape_file))
					{
						if (flag2 || !should_use_sideways_symbol)
						{
							flag = true;
						}
						flag2 = true;
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
		if (!flag2)
		{
			DebugUtil.Assert(false, "Could not find shape element for shape:" + HashCache.Get().Get(variation_symbol.hash));
		}
		KAnim.Build.Symbol symbol = KAnimBatchManager.Instance().GetBatchGroupData(controller.batchGroupID).GetSymbol(symbol_name_in_shape_file);
		KAnim.Build.SymbolFrameInstance symbolFrameInstance = KAnimBatchManager.Instance().GetBatchGroupData(variation_symbol.build.batchTag).symbolFrameInstances[variation_symbol.firstFrameIdx + frameElement.frame];
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

	private static KAnimHashedString HASH_SNAPTO_EYES = "snapto_eyes";

	private static KAnimHashedString HASH_NEUTRAL = "neutral";

	private static int FIRST_SIDEWAYS_FRAME = 29;
}
