using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLogic.Model
{
	public class GameSettingsModel
	{
		public bool autoplayAudio { get; set; } = true;
		public bool loopAudio { get; set; } = true;
		public bool showSceneTitle { get; set; } = true;
		public bool playersCanMoveThemselvesBetweenScenes { get; set; } = false;
		public SceneModel? defaultScene { get; set; }


		//So we can have reference mutability, for use with mudblazor datagrid
		public class WhitelistName {
			public WhitelistName() { name = ""; }
			public WhitelistName(string? _name) { name = _name; }
			public string? name = "";

			public override int GetHashCode() => name?.GetHashCode() ?? 0;
			public override string ToString() => name ?? "";

            public override bool Equals(object? obj) {
				// Check for null and type compatibility
				if (obj == null || GetType() != obj.GetType())
					return false;

				// Cast the object to WhitelistName and compare the name
				var other = (WhitelistName)obj;
				return string.Equals(name, other.name, StringComparison.Ordinal);
            }
        }
		public List<WhitelistName> whitelistNames { get; set; } = new List<WhitelistName>();

	}
}
