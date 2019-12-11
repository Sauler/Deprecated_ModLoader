namespace CMS.Mods {
	public abstract class Mod {
		public abstract void Activate();
		public abstract void Deactivate();
		public abstract ModInfo GetInfo();
	}
}