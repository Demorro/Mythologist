namespace SharedLogic.Model
{
    public class SceneModel
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
	}
}
