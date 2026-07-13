using System;

[SkipSaveFileSerialization]
public class CancellableDig : Cancellable
{
	protected override void OnCancel(object data)
	{
		bool flag;
		if (data != null)
		{
			Boxed<bool> boxed = data as Boxed<bool>;
			if (boxed != null)
			{
				flag = boxed.value;
				goto IL_0016;
			}
		}
		flag = false;
		IL_0016:
		if (flag)
		{
			this.OnAnimationDone("ScaleDown");
			return;
		}
		EasingAnimations componentInChildren = base.GetComponentInChildren<EasingAnimations>();
		int num = Grid.PosToCell(this);
		if (componentInChildren.IsPlaying && Grid.Element[num].hardness == 255)
		{
			EasingAnimations easingAnimations = componentInChildren;
			easingAnimations.OnAnimationDone = (Action<string>)Delegate.Combine(easingAnimations.OnAnimationDone, new Action<string>(this.DoCancelAnim));
			return;
		}
		EasingAnimations easingAnimations2 = componentInChildren;
		easingAnimations2.OnAnimationDone = (Action<string>)Delegate.Combine(easingAnimations2.OnAnimationDone, new Action<string>(this.OnAnimationDone));
		componentInChildren.PlayAnimation("ScaleDown", 0.1f);
	}

	private void DoCancelAnim(string animName)
	{
		EasingAnimations componentInChildren = base.GetComponentInChildren<EasingAnimations>();
		componentInChildren.OnAnimationDone = (Action<string>)Delegate.Remove(componentInChildren.OnAnimationDone, new Action<string>(this.DoCancelAnim));
		componentInChildren.OnAnimationDone = (Action<string>)Delegate.Combine(componentInChildren.OnAnimationDone, new Action<string>(this.OnAnimationDone));
		componentInChildren.PlayAnimation("ScaleDown", 0.1f);
	}

	private void OnAnimationDone(string animationName)
	{
		if (animationName != "ScaleDown")
		{
			return;
		}
		EasingAnimations componentInChildren = base.GetComponentInChildren<EasingAnimations>();
		componentInChildren.OnAnimationDone = (Action<string>)Delegate.Remove(componentInChildren.OnAnimationDone, new Action<string>(this.OnAnimationDone));
		this.DeleteObject();
	}
}
