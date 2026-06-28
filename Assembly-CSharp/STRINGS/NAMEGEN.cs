using System;

namespace STRINGS
{
	public class NAMEGEN
	{
		public class COLONY
		{
			public static string[] FORMATS = new string[] { "The {adjective} {noun}", "{adjective} {noun}", "{noun}", "The Not-So {adjective} {noun}" };

			public static string[] ADJECTIVE = new string[]
			{
				"Cool", "Deadly", "Space", "Data", "Future", "Grimy", "Idyllic", "Antigravity", "Rocky", "Duplicant",
				"Clone", "Subterranean", "Underground", "Messy", "Burgeoning", "Hopeful", "Lucky", "Super", "Ultra", "Perfect",
				"Swanky", "Luxury", "Incredible", "Delicate", "Radical", "Hip", "Impenetrable", "Ironic", "Suave", "Death",
				"Uncomfortable", "Outrageous", "Dorky", "Leaky", "Super Duper", "Galactic", "Lunar", "Intergalactic", "Transdimensional", "Temporal",
				"Dangerous", "Yucky", "Cute", "Happy", "Bustling", "Prosperous", "Doomed", "Merry", "Optimistic", "Systematic",
				"Beautiful", "Spacefaring", "Inescapable", "Golden", "Serene", "Chaotic", "Invisible", "Starving", "Rosy", "Great",
				"Rustic", "Scientific", "Besieged"
			};

			public static string[] NOUN = new string[]
			{
				"Base", "Colony", "Home", "Facility", "Hovel", "Pit", "Paradise", "Utopia", "Dystopia", "Spacestation",
				"Antfarm", "Settlement", "Asteroid", "Moonbase", "Comet", "Spacerock", "Spaceprison", "Pit", "Crater", "Citadel",
				"Shack", "Abode", "Fortress", "System", "Spacepad", "Space Hut", "Abyss", "Panopticon", "Spacecamp", "Pigpen",
				"Crashpad", "Homestead", "Acropolis", "Beyond", "Galaxy", "Dump", "Dwelling", "Rat's Nest", "Burrow", "Planet",
				"Cosmos", "Dreamland", "Tunnel", "Trainwreck", "Shelter", "Bunker", "Supernova", "Cantaloupe", "Factory"
			};
		}

		public class DUPLICANT
		{
			public class PREFIX
			{
				public static string[] MALE = new string[] { "Mr.", "King", "Prince", "Ol' Man" };

				public static string[] FEMALE = new string[] { "Ms.", "Queen", "Princess", "Ol' Woman" };

				public static string[] UNISEX = new string[]
				{
					"Mx.", "Dr.", "Captain", "Colonel", "Professor", "Slickity", "Big", "Tiny", "Peppy", "Old",
					"Good Ol'", "Wunder", "Evil"
				};
			}

			public class SUFFIX
			{
				public static string[] MALE = new string[] { "'The Dude'" };

				public static string[] FEMALE = new string[] { "'The Dudette'" };

				public static string[] UNISEX = new string[] { "Jr.", "Sr.", "The Evil", "The Good", "The Bad", "The Ugly", "The Great", "Strong", "McCool", "The Chosen One" };
			}

			public class NAME
			{
				public static string[] MALE = new string[]
				{
					"Hugo", "Jambell", "Jamie", "Jeff", "Alia", "Yog", "Ju-Lian", "Jarrett", "Graham", "Vince",
					"Matt", "Mike", "Alex", "Kevin", "Ian", "Vito", "Corey", "Jan", "Mark", "Charlie",
					"Sloth", "Peter", "Jason", "Dany", "Scott", "May", "Joe", "Lou", "Travaldo", "Wilson",
					"Wolfgang", "Webber", "Maxwell", "Warly", "Woodie", "Wes", "Lawry", "Stenlay", "Marty", "Hink",
					"Kernle"
				};

				public static string[] FEMALE = new string[]
				{
					"Kelly", "Toni", "Nana", "Dana", "Nabie-Ah", "Priscilla", "Leira", "Geneva", "Anita", "Jenna",
					"Jade", "Sarah", "Kelsey", "Jenny", "Emily", "Ashley", "Lannie", "Nasim", "Lorna", "Emma",
					"Simone", "Catie", "Laura", "Vivian", "Rachel", "Ariodne", "Kiersten", "Willow", "Wendy", "Walani",
					"Charlie", "Lucy", "Abigail", "Wickerbottom", "Wigfrid", "Lilah", "Crandice", "Elasebete", "Rinaldine", "Giselda",
					"Grunktilda"
				};

				public static string[] UNISEX = new string[]
				{
					"Kris", "Dumpling", "Slab", "Muscle", "Axel", "Ace", "Crudmuffin", "Greasy", "Buddy", "Babyface",
					"Avery", "Bailey", "Paris", "Scout", "Brook", "Kerry", "Jude", "Mason", "Orion", "Jupiter",
					"Snort", "Sunshine", "Dancer", "Campbell", "Lee", "Reese", "Pat", "Logan", "Star", "Anderson",
					"Pearson", "Tucker", "Bao", "Bobbie", "Chan", "Dar", "Farah", "Georgie", "Truck", "Crumb",
					"Wemp", "Lunk", "Cromulus", "Blump"
				};
			}
		}

		public class GRAVE
		{
			public static string[] EPITAPHS = new string[] { "Gone, but not forgotten.", "To dig, to mush, no more.", "May they know peace.", "Never another like them. Until we print one.", "Died doing what they loved.", "Ashes to ashes, stardust to stardust.", "Now we'll never know where they buried the treasure." };
		}
	}
}
