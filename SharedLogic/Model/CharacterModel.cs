namespace SharedLogic.Model
{
    public class CharacterModel
    {
        public CharacterModel(string id)
        {
            this.id = id;
			markdownValue = "";
        }

        public string id { get; set; }

		public Uri? portraitImageUri { get; set; }

		public string markdownValue {get; set; }

		// Overrides for Equals, GetHashCode and ToString are important for MudSelect
		public override bool Equals(object o)
		{
			var other = o as CharacterModel;
			return other?.id == id;
		}
		public override int GetHashCode() => id?.GetHashCode() ?? 0;

		public override string ToString()
		{
			return id;
		}
	}
}
