using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace CMS.Mods {
	public class ModLoader {
		private const string LogFile = "ModLoader.txt";
		
		private string modsDirectory;
		private bool haveAnyModsToLoad;
		private static ModLoader instance;
		private int currentModIndex;
		private Mod[] mods;
		
		private ModLoader() {
			Logger.RemoveLogFile(LogFile);
			
			GetModsDirectory();
			BuildModsList();
			ActivateMods();
		}

		~ModLoader() {
			UnloadMods();
		}

		private void UnloadMods() {
			if (!haveAnyModsToLoad) 
				return;
			
			for (var i = 0; i < mods.Length; i++) {
				var mod = mods[i];
				if (mod != null) 
					mod.Deactivate();
			}
		}

		private void GetModsDirectory() {
			modsDirectory = Application.dataPath + "/../Mods";
			Logger.Log("Mods directory: " + modsDirectory, LogFile, true);
		}
		
		private void BuildModsList() {
			if (!Directory.Exists(modsDirectory)) {
				haveAnyModsToLoad = false;
				Logger.Log("Mods directory does not exists!", LogFile, true);
				return;
			}

			var files = Directory.GetFiles(modsDirectory, "*.dll");
			if (files.Length == 0) {
				haveAnyModsToLoad = false;
				Logger.Log("No mods found in mods directory!", LogFile, true);
				return;
			}

			haveAnyModsToLoad = true;
			currentModIndex = 0;
			mods = new Mod[files.Length];
			
			Logger.Log("Found " + files.Length + " mods", LogFile, true);
			
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
					Logger.Log("Mod name: " + mod.GetInfo().Name, LogFile, true);
					return;
				}
			}
		}
		
		private void ActivateMods() {
			if (!haveAnyModsToLoad)
				return;

			for (var i = 0; i < mods.Length; i++) {
				var mod = mods[i];
				if (mod != null) 
					mod.Activate();
			}
		}
	}
}