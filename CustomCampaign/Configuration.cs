﻿using PhoenixPoint.Modding;

namespace CustomCampaign
{
	public class Configuration : ModConfig
	{
		public const int MaxStartingSoldiers = 25;


		[ConfigField(text: "Starting Resources (in %)",
			description: "Multiplier (in %) to player starting resources.")]
		public int StartingResources = 100;

		[ConfigField(description:
			@"Additional modifier towards tactical game difficulty. This simply upgrades/downgrades difficulty for tactical by given amount (Rookie + 3 = Legend).
Note this affects alien evloution speed (to keep the same difficulty feeling).")]
		public int TacticalDifficultyModifier = 0;

		[ConfigField(description:
			@"Additional modifier towards mission threat level. This simply upgrades/downgrades threat level by given amount (Low + 3 = Extreme).")]
		public int MissionThreatLevelModifier = 0;

		[ConfigField(description:"Additional modifier towards number of starting soldiers. Min: 1; Max: 25")]
		public int StartingSoldiersModifier = 0;

		[ConfigField(text: "Human Population Lost (in %)",
			description: "Multiplier (in %) to how much population is lost from various events. Set to 0 to never lose population.")]
		public int HumanPopulationLost = 100;

		[ConfigField(text: "Manufacture Speed (in %)",
			description: "Multiplier (in %) to how fast items are manufactured (highter is faster). Set to 0 for instant manufacturing.")]
		public int ItemManufactureSpeed = 100;

		[ConfigField(description: "Ammo cannot be manufactured & weapons don't consume ammo (still they will need to be reloaded in tactical).")]
		public bool NoAmmo = false;

		[ConfigField(text:"Ambush Chance (in %)",
			description: "Chance for ambush to appear if no exploration site did not roll encounter (default is 100).")]
		public int AmbushChance = 100;

		[ConfigField(text: "Haven Defense Frequency (in %)",
			description: "How often Pandorans will attack havens. Set to 0 to disable haven attacks.")]
		public int HavenDefenseFrequency = 100;

	}
}
