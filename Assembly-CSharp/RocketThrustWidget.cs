using System;
using STRINGS;
using TUNING;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/RocketThrustWidget")]
public class RocketThrustWidget : KMonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	protected override void OnPrefabInit()
	{
	}

	public void Draw(CommandModule commandModule)
	{
		if (this.rectTransform == null)
		{
			this.rectTransform = this.graphBar.gameObject.GetComponent<RectTransform>();
		}
		this.commandModule = commandModule;
		this.totalWidth = this.rectTransform.rect.width;
		this.UpdateGraphDotPos(commandModule);
	}

	private void UpdateGraphDotPos(CommandModule rocket)
	{
		this.totalWidth = this.rectTransform.rect.width;
		float num = Mathf.Lerp(0f, this.totalWidth, rocket.rocketStats.GetTotalMass() / this.maxMass);
		num = Mathf.Clamp(num, 0f, this.totalWidth);
		this.graphDot.rectTransform.SetLocalPosition(new Vector3(num, 0f, 0f));
		this.graphDotText.text = "-" + Util.FormatWholeNumber(rocket.rocketStats.GetTotalThrust() - rocket.rocketStats.GetRocketMaxDistance()) + "km";
	}

	private void Update()
	{
		if (this.mouseOver)
		{
			if (this.rectTransform == null)
			{
				this.rectTransform = this.graphBar.gameObject.GetComponent<RectTransform>();
			}
			Vector3 position = this.rectTransform.GetPosition();
			Vector2 size = this.rectTransform.rect.size;
			float num = KInputManager.GetMousePos().x - position.x + size.x / 2f;
			num = Mathf.Clamp(num, 0f, this.totalWidth);
			this.hoverMarker.rectTransform.SetLocalPosition(new Vector3(num, 0f, 0f));
			float num2 = Mathf.Lerp(0f, this.maxMass, num / this.totalWidth);
			float totalThrust = this.commandModule.rocketStats.GetTotalThrust();
			float rocketMaxDistance = this.commandModule.rocketStats.GetRocketMaxDistance();
			this.hoverTooltip.SetSimpleTooltip(string.Concat(new string[]
			{
				UI.STARMAP.ROCKETWEIGHT.MASS,
				GameUtil.GetFormattedMass(num2, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram, true, "{0:0.#}"),
				"\n",
				UI.STARMAP.ROCKETWEIGHT.MASSPENALTY,
				Util.FormatWholeNumber(ROCKETRY.CalculateMassWithPenalty(num2)),
				UI.UNITSUFFIXES.DISTANCE.KILOMETER,
				"\n\n",
				UI.STARMAP.ROCKETWEIGHT.CURRENTMASS,
				GameUtil.GetFormattedMass(this.commandModule.rocketStats.GetTotalMass(), GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Kilogram, true, "{0:0.#}"),
				"\n",
				UI.STARMAP.ROCKETWEIGHT.CURRENTMASSPENALTY,
				Util.FormatWholeNumber(totalThrust - rocketMaxDistance),
				UI.UNITSUFFIXES.DISTANCE.KILOMETER
			}));
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		this.mouseOver = true;
		this.hoverMarker.SetAlpha(1f);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		this.mouseOver = false;
		this.hoverMarker.SetAlpha(0f);
	}

	public Image graphBar;

	public Image graphDot;

	public LocText graphDotText;

	public Image hoverMarker;

	public ToolTip hoverTooltip;

	public RectTransform markersContainer;

	public Image markerTemplate;

	private RectTransform rectTransform;

	private float maxMass = 20000f;

	private float totalWidth = 5f;

	private bool mouseOver;

	public CommandModule commandModule;
}
