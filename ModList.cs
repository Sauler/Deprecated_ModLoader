
using System.Collections.Generic;
using CMS.Mods.API.MainMenu;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace CMS.Mods {
	internal class ModList : MonoBehaviour {
	
		public void Awake() {
			DontDestroyOnLoad(gameObject);
			SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
			Init();
		}
		
		private void SceneManager_activeSceneChanged(Scene from, Scene to) {
			if (to.name == "SceneLoader") 
				Destroy(gameObject);
		}
		
		public void Init() {
			CreateMainMenuCategory();
		}

		private void OnDestroy() {
			MainMenuCustomizer.Instance.RemoveMainMenuCategory("Mods");
		}

		private void CreateMainMenuCategory() {
			var list = new List<MainMenuOption>();
			var loadedMods = ModLoader.Instance().GetLoadedMods();

			for (var i = 0; i < loadedMods.Length; i++) {
				var mod = loadedMods[i];
				var item = new MainMenuOption {
					Name = mod.GetInfo().Name,
					URL = string.Empty
				};
				list.Add(item);
			}

			var mainMenuCategory = new MainMenuCategory {
				Name = "Mods",
				Options = list,
				WithBackOption = true
			};
			MainMenuCustomizer.Instance.AddMainMenuCategory(mainMenuCategory);
		}
	}
}