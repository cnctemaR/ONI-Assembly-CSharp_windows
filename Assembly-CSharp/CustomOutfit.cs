using System;

public class CustomOutfit : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		this.accessorizer = base.GetComponent<Accessorizer>();
	}

	public void Update()
	{
		if (!this.accessorizer.bodyData.legs.IsValid)
		{
			return;
		}
		KCompBuilder.BodyData bodyData = this.accessorizer.bodyData;
		int num = Hash.SDBMLower("Jorge");
		bodyData.neck = string.Format("neck_{0}", num);
		bodyData.legs = string.Format("leg_{0}", num);
		bodyData.belt = string.Format("belt_{0}", num);
		bodyData.pelvis = string.Format("pelvis_{0}", num);
		bodyData.foot = string.Format("foot_{0}", num);
		bodyData.hand = string.Format("hand_paint_{0}", num);
		bodyData.cuff = string.Format("cuff_{0}", num);
		this.accessorizer.bodyData = bodyData;
		base.enabled = false;
	}

	private Accessorizer accessorizer;
}
