using UnityEngine;
using UnityEngine.Events;

namespace CMS.Mods {
	public class UI : MonoBehaviour {
		public class DrawUIEvent : UnityEvent {
			
		};

		public DrawUIEvent DrawUI;
		private void OnGUI() {
			if (DrawUI != null)
				DrawUI.Invoke();
		}

		private void OnDestroy() {
			if (DrawUI != null)
				DrawUI.RemoveAllListeners();
		}
	}
}