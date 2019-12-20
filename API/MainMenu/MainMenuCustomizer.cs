using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CMS.Mods.API.MainMenu {
	public class MainMenuCustomizer {
		private readonly SettingsDrawer settingsDrawer;
		private readonly List<MainMenuCategory> mainMenuCategories;
		private readonly Transform buttonToCopy;
		private const string CopyButton = "Canvas/MainMenu/DownMenu/Background/ButtonsBackground/Buttons/ButtonProfilesManage";

		private static MainMenuCustomizer instance;
		public static MainMenuCustomizer Instance {
			get {
				if (instance == null)
					instance = new MainMenuCustomizer();

				return instance;
			}
		}

		public MainMenuCustomizer() {
			settingsDrawer = Object.FindObjectOfType<SettingsDrawer>();
			buttonToCopy = Object.FindObjectOfType<MainMenuManager>().transform.parent
				.Find(CopyButton);
			mainMenuCategories = new List<MainMenuCategory>();
		}

		public void DrawFor(string categoryName) {
			settingsDrawer.DrawFor(categoryName);
		}

		public void AddMainMenuCategory(MainMenuCategory mainMenuCategory, bool createButton = true) {
			if (settingsDrawer == null || mainMenuCategory == null)
				return;

			var optionsCount = mainMenuCategory.Options.Count;

			string key;
			string value;
			for (var i = 0; i < optionsCount; i++) {
				var option = mainMenuCategory.Options[i];
				key = mainMenuCategory.Name + ".item" + (i + 1) + "id";
				value = option.Name;

				settingsDrawer.ini.elements.Add(key, value);
			}

			if (mainMenuCategory.WithBackOption) {
				key = mainMenuCategory.Name + ".item" + (optionsCount + 1) + "id";
				value = "MM_Back";
				settingsDrawer.ini.elements.Add(key, value);
			}

			for (var i = 0; i < optionsCount; i++) {
				var option = mainMenuCategory.Options[i];
				key = mainMenuCategory.Name + ".item" + (i + 1) + "option";
				value = option.URL;

				settingsDrawer.ini.elements.Add(key, value);
			}

			if (mainMenuCategory.WithBackOption) {
				key = mainMenuCategory.Name + ".item" + (optionsCount + 1) + "url";
				value = "ExitFromSettings";
				settingsDrawer.ini.elements.Add(key, value);
			}

			mainMenuCategories.Add(mainMenuCategory);
			if (createButton)
				CreateButton(mainMenuCategory.Name);
		}
		
		public void RemoveMainMenuCategory(MainMenuCategory mainMenuCategory, bool removeFromList = true) {
			if (settingsDrawer == null || mainMenuCategory == null)
				return;

			var optionsCount = mainMenuCategory.Options.Count;

			for (var i = 0; i < optionsCount; i++) {
				var key = mainMenuCategory.Name + ".item" + (i + 1) + "id";
				settingsDrawer.ini.elements.Remove(key);
			}
			
			for (var i = 0; i < optionsCount; i++) {
				var key = mainMenuCategory.Name + ".item" + (i + 1) + "option";
				settingsDrawer.ini.elements.Remove(key);
			}

			if (mainMenuCategory.WithBackOption) {
				var key1 = mainMenuCategory.Name + ".item" + (optionsCount + 1) + "id";
				settingsDrawer.ini.elements.Remove(key1);

				key1 = mainMenuCategory.Name + ".item" + (optionsCount + 1) + "url";
				settingsDrawer.ini.elements.Remove(key1);
			}

			if (removeFromList)
				mainMenuCategories.Remove(mainMenuCategory);
		}

		public void RemoveMainMenuCategory(string categoryName, bool removeFromList = true) {
			if (string.IsNullOrEmpty(categoryName))
				return;

			var mainMenuCategory = GetCategory(categoryName);
			if (mainMenuCategory != null)
				RemoveMainMenuCategory(mainMenuCategory, removeFromList);
		}

		private MainMenuCategory GetCategory(string name) {
			for (var i = 0; i < mainMenuCategories.Count; i++) {
				var mainMenuCategory = mainMenuCategories[i];
				if (mainMenuCategory.Name != name)
					continue;

				return mainMenuCategory;
			}

			return null;
		}

		public void RemoveAllCategories() {
			for (var i = 0; i < mainMenuCategories.Count; i++) {
				RemoveMainMenuCategory(mainMenuCategories[i]);
			}
			
			mainMenuCategories.Clear();
		}
		
		private void CreateButton(string name) {
			var newButton = Object.Instantiate(buttonToCopy, buttonToCopy.parent, true);
			newButton.name = name;
			newButton.SetSiblingIndex(2);
			newButton.GetComponentInChildren<Text>().text = name;
			var buttonComponent = newButton.GetComponent<Button>();
			buttonComponent.onClick = new Button.ButtonClickedEvent();
			buttonComponent.onClick.AddListener(delegate{OnModsButtonClick(name);});
			var horizontalItemsMove = settingsDrawer.horizontalItemsMove;
			var list = new List<GameObject>(horizontalItemsMove.items);
			list.Insert(2, newButton.gameObject);
			settingsDrawer.horizontalItemsMove.items = list.ToArray();
		}

		private void OnModsButtonClick(string name) {
			CreateOptions(name);
            settingsDrawer.horizontalItemsMove.DisableMove();
            DrawFor(name);
		}

		private void CreateOptions(string name) {
			var category = GetCategory(name);
			if (category == null)
				return;

			RemoveMainMenuCategory(name);
			AddMainMenuCategory(category, false);
		}
	}
}