using UnityEngine;
   
   namespace CMS.Mods {
   	public static class Utils {
   		public static string GetGameObjectPath(GameObject obj) {
   			var path = "/" + obj.name;
   			while (obj.transform.parent != null) {
   				obj = obj.transform.parent.gameObject;
   				path = "/" + obj.name + path;
   			}
   			return path;
   		}
   
   		public static string[] GetChildObjectsPaths(GameObject obj) {
   			var childCount = obj.transform.childCount;
   			if (childCount == 0)
   				return null;
   
   			var result = new string[childCount];
   			for (var i = 0; i < childCount; i++) {
   				result[i] = GetGameObjectPath(obj.transform.GetChild(i).gameObject);
   			}
   
   			return result;
   		}
   	}
   }