namespace SharedLogic.Model
{
	public class GameModel
	{
		public GameModel(string gameName, string GMPassword)
		{
			this.id = gameName;
			this.GMPassword = GMPassword;
		}

        //GameName (needs to be id because thats always the primary database key)
        public string id { get; set; }
		public string GMPassword { get; set; }
		public List<SceneModel> scenes { get; set; } = new List<SceneModel>();
		public List<CharacterModel> characters {get; set;} = new List<CharacterModel>();
		public GameSettingsModel gameSettings { get; set; } = new GameSettingsModel();
		public Guid storageID {get; set; } = Guid.NewGuid();
	}
}
