using UnityEngine;

namespace CMS.Mods.Modules {
	public class LoadedModsInfo : MonoBehaviour {
		private GUIStyle style;
		private bool canDrawInfo;
		private int loadedModsCount;
		
		private void Awake() {
			canDrawInfo = false;
			
			style = new GUIStyle {
				normal = {
					textColor = Color.red
				},
				fontSize = 16
			};
		}

		public void SetLoadedModsCount(int count) {
			loadedModsCount = count;
			canDrawInfo = true;
		}

		private void OnGUI() {
			if (!canDrawInfo)
				return;
			
			GUILayout.Label($"Mod Loader {Constants.ModLoaderVersion} by Sauler", style, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
			style.fontSize = 13;
			GUILayout.Label("Loaded mods: " + loadedModsCount, style, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
		}
	}
}