using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLogic.Model
{
	public class PlayerPropertiesModel
	{
        public class PlayerProperty : ICloneable
        {

			public PlayerProperty() {
				playerName = "New Name";
				propertyName = "New Property";
				propertyValue = "New Value";
			}
			public PlayerProperty(string _name, string _propertyName, string _propertyValue) {
				playerName = _name;
				propertyName = _propertyName;
				propertyValue = _propertyValue;
			}

			public string? playerName;
			public string? propertyName;
			public string? propertyValue;

			 // Overrides for Equals, GetHashCode and ToString are important for MudSelect
            public override bool Equals(object? o)
            {
                var other = o as PlayerProperty;
                return other?.playerName == playerName && other?.propertyName == propertyName && other?.propertyValue == propertyValue;
            }
            public override int GetHashCode() {
				unchecked // Allow overflow without throwing an exception
				{
					int hash = 17;
					hash = hash * 31 + (playerName != null ? playerName.GetHashCode() : 0);
					hash = hash * 31 + (propertyName != null ? propertyName.GetHashCode() : 0);
					return hash;
				}
			}

            public override string ToString()
            {
				return $"{playerName}: {propertyName} ({propertyValue})";
            }

            public object Clone() {
				return this.MemberwiseClone();
            }
        }

		//The "Global" valueless properties that are used to populate the Guis and invoke for stuff like all players, joining player, etc.
		private struct ContextualProps {
			public ContextualProps(PlayerProperty playerProperty) {
				allPlayers = (PlayerProperty)playerProperty.Clone();
				allPlayers.playerName = Utils.Constants.ALL_PLAYERS_ID;
				allPlayers.propertyValue = "";
				allPlayersInScene = (PlayerProperty)playerProperty.Clone();
				allPlayersInScene.playerName = Utils.Constants.ALL_PLAYERS_IN_SCENE_ID;
				allPlayersInScene.propertyValue = "";
				invokingPlayer = (PlayerProperty)playerProperty.Clone();
				invokingPlayer.playerName = Utils.Constants.TRIGGERING_PLAYER_ID;
				invokingPlayer.propertyValue = "";
			}

			public static List<string> CONTEXTUAL_IDS = new List<string>{Utils.Constants.ALL_PLAYERS_ID, Utils.Constants.ALL_PLAYERS_IN_SCENE_ID, Utils.Constants.TRIGGERING_PLAYER_ID};

			public static bool IsContextualProperty(PlayerProperty property) {
				if (property.playerName == null){return false;}
				return CONTEXTUAL_IDS.Contains(property.playerName);
			}

			public PlayerProperty allPlayers {get; private set;}
			public PlayerProperty allPlayersInScene {get; private set;}
			public PlayerProperty invokingPlayer {get; private set;}
		}

		public void AddPlayerProperty(PlayerProperty newPlayerProperty) {
			//We want to make sure for each new property, the implicit "All Players", "All Players in Scene", "Joining Player" and "Invoking Player" are added for each property so they can be added to GUI selectors.

			if (ContextualProps.CONTEXTUAL_IDS.Contains(newPlayerProperty.playerName ?? "")) {
				throw new Exception($"Illegal player name {newPlayerProperty.playerName}");
			}

			//TODO, guard against the special player keywords as player names
			bool added = playerProperties.Add(newPlayerProperty);

			if (added) {
				ContextualProps props = new ContextualProps(newPlayerProperty);

				playerProperties.Add(props.allPlayers);
				playerProperties.Add(props.allPlayersInScene);
				playerProperties.Add(props.invokingPlayer);
			}
		}

		public void RemovePlayerProperty(PlayerProperty playerProperty) {
			bool removed = playerProperties.Remove(playerProperty);

			if (removed) {

				// If there is no more real properties of the type of the one we just removed, we want to remove all the contextual properties of that type
				if (playerProperties.Any(x => {
					return x.propertyName == playerProperty.propertyName && !ContextualProps.IsContextualProperty(x);
					// If any have the same value and are not contextual props, then there's a real prop still around
				})){
					Console.Write("Huh?!");
					/* Do nothing */
				}
				else {
					ContextualProps props = new ContextualProps(playerProperty);
					playerProperties.Remove(props.allPlayers);
					playerProperties.Remove(props.allPlayersInScene);
					playerProperties.Remove(props.invokingPlayer);
				}

			}
		}

		public HashSet<PlayerProperty> PlayerPropertiesWithoutImplicitPropeties() {
			HashSet<PlayerProperty> filteredPlayerProperties = new HashSet<PlayerProperty>();

			foreach(PlayerProperty prop in playerProperties) {
				if ((prop.propertyName != null) && (!ContextualProps.CONTEXTUAL_IDS.Contains(prop.playerName))) {
					filteredPlayerProperties.Add(prop);
				}
			}

			return filteredPlayerProperties;
		}

		public HashSet<PlayerProperty> PlayerProperties() {
			return playerProperties;
		}

		public HashSet<PlayerProperty> playerProperties {get; private set;} = new HashSet<PlayerProperty>();
		public bool treatAsWhitelist {get; set;} = false;
	}
}
