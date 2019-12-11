using System.IO;

namespace CMS.Mods {
	public static class Logger {
		public static void Log(string text) {
			using (var streamWriter = new StreamWriter("output.txt", true)) {
				streamWriter.WriteLine(text);
			}
		}
		
		public static void Log(string text, string path, bool append) {
			using (var streamWriter = new StreamWriter(path, append)) {
				streamWriter.WriteLine(text);
			}
		}

		public static void RemoveLogFile(string path = "output.txt") {
			if (File.Exists(path))
				File.Delete(path);
		}
	}
}