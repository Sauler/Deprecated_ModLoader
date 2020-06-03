using System;
using System.IO;
using System.Reflection;
using CMS.Mods.Modules;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CMS.Mods {
	public class ModLoader {
		private string modsDirectory;
		private bool haveAnyModsToLoad;
		private static ModLoader instance;
		private int currentModIndex;
		private Mod[] mods;
		public int LoadedModsCount;
		
		private ModLoader() {
			Logger.RemoveLogFile(Constants.LogFile);

			LoadedModsCount = 0;
			GetModsDirectory();
			BuildModsList();
			ActivateMods();
			RegisterEvents();
		}

		~ModLoader() {
			UnloadMods();
			UnregisterEvents();
		}

		private void UnloadMods() {
			if (!haveAnyModsToLoad) 
				return;
			
			for (var i = 0; i < mods.Length; i++) {
				var mod = mods[i];
				if (mod != null) {
					mod.Deactivate();
					LoadedModsCount--;
				}
			}
		}

		public Mod[] GetLoadedMods() {
			return mods;
		}

		private void GetModsDirectory() {
			modsDirectory = $"{Application.dataPath}/../Mods";
		}
		
		private void BuildModsList() {
			if (!Directory.Exists(modsDirectory)) {
				haveAnyModsToLoad = false;
				Directory.CreateDirectory(modsDirectory);
				return;
			}

			var files = Directory.GetFiles(modsDirectory, "*.dll");
			if (files.Length == 0) {
				haveAnyModsToLoad = false;
				return;
			}

			haveAnyModsToLoad = true;
			currentModIndex = 0;
			mods = new Mod[files.Length];
			
			Logger.Log("Found " + files.Length + " mods", Constants.LogFile, true);
			
			for (var i = 0; i < files.Length; i++) {
				var types = Assembly.LoadFile(files[i]).GetTypes();
				LoadMods(types);
			}
		}

		public static ModLoader Instance() {
			if (instance == null) 
				instance = new ModLoader();

			return instance;
		}
		
		private void LoadMods(Type[] types) {
			foreach (var type in types) {
				if (type.IsClass && type.BaseType == typeof(Mod)) {
					var mod = Activator.CreateInstance(type) as Mod;
					if (mod == null)
						return;
					
					mods[currentModIndex] = mod;
					currentModIndex++;
					Logger.Log("Mod name: " + mod.GetInfo().Name, Constants.LogFile, true);
					return;
				}
			}
		}
		
		private void ActivateMods() {
			if (!haveAnyModsToLoad)
				return;

			for (var i = 0; i < mods.Length; i++) {
				var mod = mods[i];
				if (mod != null) {
					mod.Activate();
					LoadedModsCount++;
				}
			}
		}

		private void RegisterEvents() {
			SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;
		}

		private void UnregisterEvents() {
			SceneManager.sceneLoaded -= SceneManagerOnSceneLoaded;
		}
		
		private void SceneManagerOnSceneLoaded(Scene scene, LoadSceneMode mode) {
			if (scene.name == "Menu") {
				var loadedModsInfo = new GameObject("LoadedModsInfo").AddComponent<LoadedModsInfo>();
				loadedModsInfo.SetLoadedModsCount(LoadedModsCount);
			}
		}
		
	}
}