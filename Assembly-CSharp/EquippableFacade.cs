using System;
using KSerialization;

public class EquippableFacade : KMonoBehaviour
{
	public static void AddFacadeToEquippable(Equippable equippable, string facadeID)
	{
		EquippableFacade equippableFacade = equippable.gameObject.AddOrGet<EquippableFacade>();
		equippableFacade.FacadeID = facadeID;
		equippableFacade.BuildOverride = Db.Get().EquippableFacades.Get(facadeID).BuildOverride;
		equippableFacade.ApplyAnimOverride();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.OverrideName();
		this.ApplyAnimOverride();
	}

	public string FacadeID
	{
		get
		{
			return this._facadeID;
		}
		private set
		{
			this._facadeID = value;
			this.OverrideName();
		}
	}

	public void ApplyAnimOverride()
	{
		if (this.FacadeID.IsNullOrWhiteSpace())
		{
			return;
		}
		base.GetComponent<KBatchedAnimController>().SwapAnims(new KAnimFile[] { Db.Get().EquippableFacades.Get(this.FacadeID).AnimFile });
	}

	private void OverrideName()
	{
		base.GetComponent<KSelectable>().SetName(EquippableFacade.GetNameOverride(base.GetComponent<Equippable>().def.Id, this.FacadeID));
	}

	public static string GetNameOverride(string defID, string facadeID)
	{
		if (facadeID.IsNullOrWhiteSpace())
		{
			return Strings.Get("STRINGS.EQUIPMENT.PREFABS." + defID.ToUpper() + ".NAME");
		}
		return Strings.Get("STRINGS.EQUIPMENT.PREFABS.CUSTOMCLOTHING.FACADES." + facadeID.ToUpper());
	}

	[Serialize]
	private string _facadeID;

	[Serialize]
	public string BuildOverride;
}
