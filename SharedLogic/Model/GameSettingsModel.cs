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
	}
}
