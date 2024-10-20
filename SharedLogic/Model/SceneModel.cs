using MudBlazor.ThemeManager.Extensions;
using SharedLogic.Events;

namespace SharedLogic.Model
{
    public class SceneModel : ICloneable
    {
        public SceneModel(string id)
        {
            this.id = id;
        }

        public string id { get; set; }
        public Uri? backgroundImageUri { get; set; }
        public Uri? backgroundMusicUri { get; set; }

		//The characters that are in this scene
		public List<string> charactersIdsInScene { get; set; }

		public string sceneNotes {get; set; } = "";

		public List<Event> onActorJoinEvents {get; set; } = new List<Event>();
		public List<Event> triggerableEvents {get; set; } = new List<Event>();

		// Overrides for Equals, GetHashCode and ToString are important for MudSelect
		public override bool Equals(object o)
		{
			var other = o as SceneModel;
			return other?.id == id;
		}
		public override int GetHashCode() => id?.GetHashCode() ?? 0;

		public override string ToString()
		{
			return id;
		}
	
        public object Clone() {
			SceneModel cloneScene = new SceneModel(this.id);

			cloneScene.backgroundImageUri = this.backgroundImageUri;
			cloneScene.backgroundMusicUri = this.backgroundMusicUri;

			cloneScene.charactersIdsInScene = new List<string>();
			foreach(var characterId in this.charactersIdsInScene) {
				cloneScene.charactersIdsInScene.Add(characterId);
			}
			
			cloneScene.sceneNotes = this.sceneNotes;

			cloneScene.onActorJoinEvents = new List<Event>();
			foreach(var actorJoinEvent in this.onActorJoinEvents) {
				cloneScene.onActorJoinEvents.Add((Event)actorJoinEvent.Clone());
			}

			cloneScene.triggerableEvents = new List<Event>();
			foreach(var triggerableEvent in this.triggerableEvents) {
				cloneScene.triggerableEvents.Add((Event)triggerableEvent.Clone());
			}

			return cloneScene;
        }
    }
}
