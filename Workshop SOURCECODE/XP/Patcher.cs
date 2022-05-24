using System;
using System.IO;

namespace TLDPatcher
{
	// Token: 0x02000008 RID: 8
	internal class Patcher
	{
		// Token: 0x0600002B RID: 43 RVA: 0x00004FEA File Offset: 0x000031EA
		public static void DeleteIfExists(string filename)
		{
			if (File.Exists(filename))
			{
				File.Delete(filename);
				Console.WriteLine(string.Format("Removing.....{0}", Path.GetFileName(filename)), false, false);
			}
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00005014 File Offset: 0x00003214
		public static void DeleteReferences(string tldPath)
		{
			Patcher.DeleteIfExists(Path.Combine(tldPath, "TheLongDrive_Data\\Managed\\TLDLoader.dll"));
			Patcher.DeleteIfExists(Path.Combine(tldPath, "TheLongDrive_Data\\Managed\\TLDLoader.dll.mdb"));
			Patcher.DeleteIfExists(Path.Combine(tldPath, "TheLongDrive_Data\\Managed\\TLDLoader.pdb"));
			Patcher.DeleteIfExists(Path.Combine(tldPath, "TheLongDrive_Data\\Managed\\uAudio.dll"));
			Patcher.DeleteIfExists(Path.Combine(tldPath, "TheLongDrive_Data\\Managed\\Ionic.Zip.dll"));
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00005074 File Offset: 0x00003274
		public static void CopyReferences(string tldPath)
		{
			Patcher.DeleteIfExists(Path.Combine(tldPath, "TheLongDrive_Data\\Managed\\TLDLoader.dll"));
			Patcher.DeleteIfExists(Path.Combine(tldPath, "TheLongDrive_Data\\Managed\\TLDLoader.dll.mdb"));
			Patcher.DeleteIfExists(Path.Combine(tldPath, "TheLongDrive_Data\\Managed\\TLDLoader.pdb"));
			Patcher.DeleteIfExists(Path.Combine(tldPath, "TheLongDrive_Data\\Managed\\uAudio.dll"));
			Patcher.DeleteIfExists(Path.Combine(tldPath, "TheLongDrive_Data\\Managed\\Ionic.Zip.dll"));
			if (File.Exists(Path.GetFullPath(Path.Combine("TLDLoader.dll", ""))))
			{
				File.Copy(Path.Combine(Form1.exePath, "TLDLoader.dll"), Path.Combine(tldPath, "TheLongDrive_Data\\Managed\\TLDLoader.dll"));
				Console.WriteLine("Copying new file.....TLDLoader.dll", false, false);
			}
			if (File.Exists(Path.GetFullPath(Path.Combine("TLDLoader.dll.mdb", ""))))
			{
				File.Copy(Path.GetFullPath(Path.Combine("TLDLoader.dll.mdb", "")), Path.Combine(tldPath, "TheLongDrive_Data\\Managed\\TLDLoader.dll.mdb"));
				Console.WriteLine("Copying new file.....TLDLoader.dll.mdb", false, false);
			}
			if (File.Exists(Path.GetFullPath(Path.Combine("TLDLoader.pdb", ""))))
			{
				File.Copy(Path.GetFullPath(Path.Combine("TLDLoader.pdb", "")), Path.Combine(tldPath, "TheLongDrive_Data\\Managed\\TLDLoader.pdb"));
				Console.WriteLine("Copying new file.....TLDLoader.pdb", false, false);
			}
			if (File.Exists(Path.GetFullPath(Path.Combine("Ionic.Zip.dll", ""))))
			{
				File.Copy(Path.GetFullPath(Path.Combine("Ionic.Zip.dll", "")), Path.Combine(tldPath, "TheLongDrive_Data\\Managed\\Ionic.Zip.dll"));
				Console.WriteLine("Copying new file.....Ionic.Zip.dll", false, false);
			}
		}

		// Token: 0x0600002E RID: 46 RVA: 0x000051F8 File Offset: 0x000033F8
		public static void CopyCoreAssets(string modPath)
		{
			Console.WriteLine("Copying Core Assets.....TLDLoader_Core", false, false);
			if (!Directory.Exists(Path.Combine(modPath, "Assets\\TLDLoader_Core")))
			{
				Directory.CreateDirectory(Path.Combine(modPath, "Assets\\TLDLoader_Core"));
			}
			else
			{
				File.Delete(Path.Combine(modPath, "Assets\\TLDLoader_Core\\core.unity3d"));
			}
			File.Copy(Path.Combine(Form1.exePath, "Extract\\TLDLoader_Core\\core.unity3d"), Path.Combine(modPath, "Assets\\TLDLoader_Core\\core.unity3d"));
			Console.WriteLine("Copying Core Assets.....TLDLoader_Settings", false, false);
			if (!Directory.Exists(Path.Combine(modPath, "Assets\\TLDLoader_Settings")))
			{
				Directory.CreateDirectory(Path.Combine(modPath, "Assets\\TLDLoader_Settings"));
			}
			else
			{
				File.Delete(Path.Combine(modPath, "Assets\\TLDLoader_Settings\\settingsui.unity3d"));
			}
			File.Copy(Path.Combine(Form1.exePath, "Extract\\TLDLoader_Settings\\settingsui.unity3d"), Path.Combine(modPath, "Assets\\TLDLoader_Settings\\settingsui.unity3d"));
			Console.WriteLine("Copying Core Assets Completed!", false, true);
		}
	}
}
