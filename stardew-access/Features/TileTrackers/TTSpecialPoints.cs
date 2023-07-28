using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static stardew_access.Utils.JsonLoader;

namespace stardew_access.Features.Tracker
{
	internal class TTSpecialPoints : TileTrackerBase
	{
		public TTSpecialPoints(object? arg = null) : base(arg)
		{
			
		}

		public override void FindObjects(object? arg)
		{
			// Load the JSON file using JsonLoader
			string specialPointsFile = "SpecialPoints.json";
			JsonElement specialPointsJson = LoadJsonFile(specialPointsFile);
			if (specialPointsJson.ValueKind == JsonValueKind.Null)
			{
				MainClass.ErrorLog($"Unable to load {specialPointsFile}.");
				return;
			}

			// Deserialize the JsonElement into your dictionary
			var specialPoints = JsonSerializer.Deserialize<Dictionary<string, List<object>>>(specialPointsJson.ToString() ?? "{}");

			Farmer player = Game1.player;

			string category = "special";

			//get objects from current location
			if(specialPoints is not null && specialPoints.ContainsKey(player.currentLocation.Name)) {
				foreach(object obj in specialPoints[player.currentLocation.Name]) {

					SpecialPoint? sPoint = JsonSerializer.Deserialize<SpecialPoint>(obj.ToString() ?? "{}");
					if (sPoint == null) continue;

					string object_category = sPoint.CategoryOverride ?? category;

					if (sPoint.RequiresQuest != null) {
						if (!player.hasQuest(sPoint.RequiresQuest.Value)) {
							continue;
						}
					}

					if (sPoint.ExtraChecksHook != null) {
						switch (sPoint.ExtraChecksHook) {
							case 1:
								//Shadow Guy's Hiding Bush
								if (!player.hasMagnifyingGlass) continue;
								break;
							case 2:
								//Haley's Bracelet
								if (!(player.currentLocation.currentEvent != null && player.currentLocation.currentEvent.playerControlTargetTile == new Point(53, 8))) continue;
								break;
						}
					}

					if (sPoint.Name is not null)
						AddFocusableObject(object_category, sPoint.Name, new(sPoint.XPos, sPoint.YPos)!);

				}
			};

			base.FindObjects(arg);
		}
	}

	public class SpecialPoint {
		public string? Name { get; set; }
		public string? CategoryOverride { get; set; } = null;
		public int XPos { get; set; }
		public int YPos { get; set; }
		public int? RequiresQuest { get; set; } = null;
		public int? ExtraChecksHook { get; set; } = null;
	}
}
