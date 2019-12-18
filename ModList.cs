using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CMS.Mods {
	internal class ModList : MonoBehaviour {
		private const string CopyButton =
			"Canvas/MainMenu/DownMenu/Background/ButtonsBackground/Buttons/ButtonProfilesManage";

		private SettingsDrawer settingsDrawer;
		private Type settingsDrawerType;
		private FieldInfo canBack;
		private bool isModListActive;

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
			settingsDrawer = FindObjectOfType<SettingsDrawer>();
			settingsDrawerType = settingsDrawer.GetType();
			canBack = settingsDrawerType.GetField("canBack", BindingFlags.NonPublic | BindingFlags.Instance);
			
			AddModListButton();
		}

		private void AddModListButton() {
			var mainMenuManager = FindObjectOfType<MainMenuManager>();
			var buttonToClone = mainMenuManager.transform.parent.Find(CopyButton);
			var modListButton = Instantiate(buttonToClone, buttonToClone.parent, true);
			modListButton.SetSiblingIndex(2);
			modListButton.GetComponentInChildren<Text>().text = "Mods";
			var button = modListButton.GetComponent<Button>();
			button.onClick.RemoveAllListeners();
			button.onClick.AddListener(OnModsButtonClick);


			var horizontalItemsMove = FindObjectOfType<HorizontalItemsMove>();
			var tempList = new List<GameObject>(horizontalItemsMove.items);
			tempList.Insert(2, modListButton.gameObject);
			horizontalItemsMove.items = tempList.ToArray();
		}

		private void OnModsButtonClick() {
			if (!settingsDrawer.CanOpen())
				return;

			settingsDrawer.SettingsIsDrawer = true;

			var isReady = settingsDrawerType.GetField("isReady", BindingFlags.NonPublic | BindingFlags.Instance);
			isReady.SetValue(settingsDrawer, false);

			settingsDrawer.horizontalMoveBlocker.SetActive(true);
			
			canBack.SetValue(settingsDrawer, true);

			var original = Resources.Load<GameObject>("UI/MainMenuItem");
			settingsDrawer.verticalItemsMove.Disable();
			settingsDrawer.verticalItemsMove.ResetItems(true);

			var currentCategoryName = settingsDrawerType.GetField("currentCategoryName",
				BindingFlags.NonPublic | BindingFlags.Instance);
			currentCategoryName.SetValue(settingsDrawer, "Mods");

			var placeToDraw = (Transform) settingsDrawerType.GetField("PlaceToDraw",
					BindingFlags.NonPublic | BindingFlags.Instance)
				.GetValue(settingsDrawer);
			settingsDrawer.CleanElement(placeToDraw);

			LeanTween.cancel(placeToDraw.parent.gameObject);
			settingsDrawer.patchInfo.SetActive(false);

			LeanTween.value(placeToDraw.parent.gameObject,
					placeToDraw.parent.GetComponent<RectTransform>().sizeDelta.y,
					(float) (ModLoader.Instance().LoadedModsCount * 2 / 2 * 27.2000007629395 + 30.0),
					0.5f)
				.setOnUpdate(val =>
					placeToDraw.parent.GetComponent<RectTransform>().sizeDelta =
						placeToDraw.parent.GetComponent<RectTransform>().sizeDelta.SetY(val))
				.setEase(LeanTweenType.easeInOutSine);

			var mods = ModLoader.Instance().GetLoadedMods();
			for (var i = 0; i < mods.Length; ++i) {
				var gameObject = Instantiate(original, placeToDraw);
//				if (i == mods.Length) {
//					ExitFromSettings
//					gameObject.transform.Find("Text").GetComponent<TextLocalize>().SetText("MM_Back");
//					gameObject.transform.Find("Option").gameObject.SetActive(false);
//					gameObject.name = "ExitFromSettings";
//					gameObject.GetComponent<SettingsItemVertical>().Menu = settingsDrawer;
//					gameObject.GetComponent<SettingsItemVertical>().CategoryName = "Mods";
//					gameObject.GetComponent<SettingsItemVertical>().Index = i + 1;
//					gameObject.GetComponent<SettingsItemVertical>().SetListener(settingsDrawer.verticalItemsMove);
//					settingsDrawer.verticalItemsMove.AddItem(gameObject);
//				}
				var modName = mods[i].GetInfo().Name;
				gameObject.transform.Find("Text").GetComponent<TextLocalize>().SetText(modName);
				gameObject.transform.Find("Option").gameObject.SetActive(false);
				gameObject.name = modName;
				gameObject.GetComponent<SettingsItemVertical>().Menu = settingsDrawer;
				gameObject.GetComponent<SettingsItemVertical>().CategoryName = "Mods";
				gameObject.GetComponent<SettingsItemVertical>().Index = i + 1;
				gameObject.GetComponent<SettingsItemVertical>().SetListener(settingsDrawer.verticalItemsMove);
				settingsDrawer.verticalItemsMove.AddItem(gameObject);
				
			}
			
			isReady.SetValue(settingsDrawer, true);
			settingsDrawer.verticalItemsMove.Enable();
			isModListActive = true;
		}

		private void Update() {
			if (!isModListActive || settingsDrawer == null)
				return;

			if ((bool) canBack.GetValue(settingsDrawer) && InputManager.Instance.UICancelButtonDown() &&
			    settingsDrawer.CanOpen()) {
				settingsDrawer.CloseSettings();
				isModListActive = false;
			}
		}
	}
}