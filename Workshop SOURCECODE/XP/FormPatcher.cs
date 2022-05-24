using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Windows.Forms;
using Microsoft.Win32;
using Mono.Cecil;
using Mono.Cecil.Cil;
//using TLDPatcher.Properties;

namespace TLDPatcher
{
	// Token: 0x02000004 RID: 4
	public partial class Form1 : Form
	{
		// Token: 0x06000008 RID: 8 RVA: 0x0000240C File Offset: 0x0000060C
		public Form1()
		{


			//Log.logBox = this.logBox;
			this.FindTldPath();
			DownloadFiles();

			//this.tldPathLabel.Text = Form1.tldPath;
			try
			{
				this.tldLoaderVersion = FileVersionInfo.GetVersionInfo("TLDLoader.dll");
				string arg;
				if (this.tldLoaderVersion.FileBuildPart != 0)
				{
					arg = string.Format("{0}.{1}.{2}", this.tldLoaderVersion.FileMajorPart, this.tldLoaderVersion.FileMinorPart, this.tldLoaderVersion.FileBuildPart);
				}
				else
				{
					arg = string.Format("{0}.{1}", this.tldLoaderVersion.FileMajorPart, this.tldLoaderVersion.FileMinorPart);
				}
				Console.WriteLine(string.Format("{1}TLDLoader v{0} is up to date, no new version found.", arg, Environment.NewLine), false, false);
				//this.statusBarLabel.Text = "TLDPatcher Ready!";
			}
			catch (Exception ex)
			{
				Console.WriteLine(string.Format("Check for new version failed with error: {0}", ex.Message), false, false);
				//this.statusBarLabel.Text = "TLDPatcher Ready!";
			}
			Console.WriteLine("TLDPatcher ready!", true, true);
			if (Form1.tldPath != "(unknown)")
			{
				//this.tldPathLabel.Text = Form1.tldPath;
				this.AssemblyFullPath = Path.Combine(Form1.tldPath, this.AssemblyPath);
				this.CheckPatchStatus();
				//this.PathToMods.Text = "Mods Folder: " + this.mdPath;
			}
		}
		Uri urizip = new Uri("https://github.com/KolbenLP/WorkshopTLDMods/raw/main/Workshop/TLDPatcher.zip");
		// Token: 0x06000009 RID: 9 RVA: 0x000025EC File Offset: 0x000007EC

		public void Clean()
		{
			try
			{
				Patcher.DeleteIfExists("TLDFolder.txt");
				Patcher.DeleteIfExists("Mono.Cecil.dll");
				Patcher.DeleteIfExists("TLDLoader.dll");        //bitch wont work
				if (Directory.Exists("Extract"))
				{
					Directory.Delete("Extract", true);
				}
				Application.Exit();
			}
			catch
			{
				Application.Exit();
			} 
			
		}



		public void DownloadFiles()
		{
			Patcher.DeleteIfExists("TLDLoader.dll");
			WebClient client = new WebClient();

			client.Credentials = new NetworkCredential("admin", "1331");
			Uri uridllp = new Uri("https://github.com/KolbenLP/WorkshopTLDMods/raw/main/Workshop/TLDLoader.dll");
			Uri urimono = new Uri("https://github.com/KolbenLP/WorkshopTLDMods/raw/main/Workshop/Mono.Cecil.dll");

			client.DownloadFile(uridllp, "TLDLoader.dll");
			if(!File.Exists("Mono.Cecil.dll"))
			{
				client.DownloadFile(urimono, "Mono.Cecil.dll");
			}
			try
			{
				if (!File.Exists(tldPath + "\\TheLongDrive_Data\\Managed\\TLDLoader.dll"))
				{
					File.Copy("TLDLoader.dll", tldPath + "\\TheLongDrive_Data\\Managed\\TLDLoader.dll");
				}
			}
			catch
			{
				//silent
			}
			
		}

		public void FindTldPath()
		{
			if (File.Exists(Path.Combine(Form1.exePath, "TLDFolder.txt")))
			{
				Form1.tldPath = File.ReadAllText(Path.Combine(Form1.exePath, "TLDFolder.txt"));
				if (!Directory.Exists(Form1.tldPath))
				{
					Console.WriteLine(string.Format("Saved TLD Folder, doesn't exists: {0}", Form1.tldPath), false, false);
					Form1.tldPath = "(unknown)";
					try
					{
						File.Delete(Path.Combine(Form1.exePath, "TLDFolder.txt"));
						goto IL_99;
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message, false, false);
						goto IL_99;
					}
				}
				Console.WriteLine(string.Format("Loaded saved TLD Folder: {0}", Form1.tldPath), false, false);
				return;
			}
			IL_99:
			if (Directory.Exists("C:\\Program Files (x86)\\Steam\\steamapps\\common\\The Long Drive"))
			{
				Form1.tldPath = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\The Long Drive";
				File.WriteAllText(Path.Combine(Form1.exePath, "TLDFolder.txt"), Form1.tldPath);
				return;
			}
			string text = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\Valve\\Steam", "InstallPath", "C:\\Program Files (x86)\\Steam");
			if (text == null)
			{
				text = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Valve\\Steam", "InstallPath", "C:\\Program Files (x86)\\Steam");
			}
			if (Directory.Exists(text) && File.Exists(text + "\\steamapps\\libraryfolders.vdf"))
			{
				foreach (string text2 in File.ReadAllText(text + "\\steamapps\\libraryfolders.vdf").Split(new char[]
				{
					'"'
				}, StringSplitOptions.RemoveEmptyEntries))
				{
					if (Directory.Exists(text2) && Directory.Exists(text2 + "\\steamapps\\common\\The Long Drive"))
					{
						Form1.tldPath = text2 + "\\steamapps\\common\\The Long Drive";
						File.WriteAllText(Path.Combine(Form1.exePath, "TLDFolder.txt"), Form1.tldPath);
						return;
					}
				}
				return;
			}
			Form1.tldPath = "(unknown)";
			SelectFolder();
		}

		// Token: 0x0600000A RID: 10 RVA: 0x000027AC File Offset: 0x000009AC
		public void FindTldPath1()
		{
			if (File.Exists("TLDFolder.txt"))
			{
				Form1.tldPath = File.ReadAllText("TLDFolder.txt");
				if (!Directory.Exists(Form1.tldPath))
				{
					Console.WriteLine(string.Format("Saved TLD Folder, doesn't exists: {0}", Form1.tldPath), false, false);
					Form1.tldPath = "(unknown)";
					try
					{
						File.Delete("TLDFolder.txt");
						goto IL_78;
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message, false, false);
						goto IL_78;
					}
				}
				Console.WriteLine(string.Format("Loaded saved TLD Folder: {0}", Form1.tldPath), false, false);
				return;
			}
			IL_78:
			Form1.tldPath = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\The Long Drive";
			if (Directory.Exists(Form1.tldPath))
			{
				File.WriteAllText("TLDFolder.txt", Form1.tldPath);
				return;
			}
			string text = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\Valve\\Steam", "InstallPath", "C:\\Program Files (x86)\\Steam");
			if (text == null)
			{
				text = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Valve\\Steam", "InstallPath", "C:\\Program Files (x86)\\Steam");
			}
			if (Directory.Exists(text) && File.Exists(text + "\\steamapps\\libraryfolders.vdf"))
			{
				foreach (string text2 in File.ReadAllText(text + "\\steamapps\\libraryfolders.vdf").Split(new char[]
				{
					'"'
				}, StringSplitOptions.RemoveEmptyEntries))
				{
					if (Directory.Exists(text2) && Directory.Exists(text2 + "\\steamapps\\common\\The Long Drive"))
					{
						Form1.tldPath = text2 + "\\steamapps\\common\\The Long Drive";
						File.WriteAllText("TLDFolder.txt", Form1.tldPath);
						return;
					}
				}
				return;
			}
			Form1.tldPath = "(unknown)";
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002934 File Offset: 0x00000B34
		public void PatchStarter()
		{
			if (this.oldFilesFound)
			{
				Console.WriteLine("Cleaning old files!", true, true);
				Patcher.DeleteIfExists(Path.Combine(Form1.tldPath, "TheLongDrive_Data\\Managed\\Assembly-CSharp.original.dll"));
				Patcher.DeleteIfExists(Path.Combine(Form1.tldPath, "TheLongDrive_Data\\Managed\\Mono.Cecil.dll"));
				Patcher.DeleteIfExists(Path.Combine(Form1.tldPath, "TheLongDrive_Data\\Managed\\Mono.Cecil.Rocks.dll"));
				Patcher.DeleteIfExists(Path.Combine(Form1.tldPath, "TheLongDrive_Data\\Managed\\TLDLoader.dll"));
				Patcher.DeleteIfExists(Path.Combine(Form1.tldPath, "TheLongDrive_Data\\Managed\\TLDPatcher.exe"));
				Patcher.DeleteIfExists(Path.Combine(Form1.tldPath, "TheLongDrive_Data\\Managed\\System.Xml.dll"));
				this.StartPatching();
				return;
			}
			if (this.isgameUpdated)
			{
				Console.WriteLine("Removing old backup!", true, true);
				Patcher.DeleteIfExists(string.Format("{0}.backup", this.AssemblyFullPath));
				this.StartPatching();
				return;
			}
			if (this.oldPatchFound)
			{
				if (File.Exists(Path.Combine(Form1.tldPath, "TheLongDrive_Data\\Managed\\Assembly-CSharp.original.dll")))
				{
					if (File.Exists(this.AssemblyFullPath))
					{
						Console.WriteLine("Recovering backup file!", true, true);
						Patcher.DeleteIfExists(this.AssemblyFullPath);
						File.Move(Path.Combine(Form1.tldPath, "TheLongDrive_Data\\Managed\\Assembly-CSharp.original.dll"), this.AssemblyFullPath);
						Console.WriteLine("Recovering.....Assembly-CSharp.original.dll", false, false);
					}
					else
					{
						Console.WriteLine("Recovering backup file!", true, true);
						File.Move(Path.Combine(Form1.tldPath, "TheLongDrive_Data\\Managed\\Assembly-CSharp.original.dll"), this.AssemblyFullPath);
						Console.WriteLine("Recovering.....Assembly-CSharp.original.dll", false, false);
					}
					Console.WriteLine("Cleaning old files!", true, true);
					Patcher.DeleteIfExists(Path.Combine(Form1.tldPath, "TheLongDrive_Data\\Managed\\Mono.Cecil.dll"));
					Patcher.DeleteIfExists(Path.Combine(Form1.tldPath, "TheLongDrive_Data\\Managed\\Mono.Cecil.Rocks.dll"));
					Patcher.DeleteIfExists(Path.Combine(Form1.tldPath, "TheLongDrive_Data\\Managed\\TLDLoader.dll"));
					Patcher.DeleteIfExists(Path.Combine(Form1.tldPath, "TheLongDrive_Data\\Managed\\TLDPatcher.exe"));
					Patcher.DeleteIfExists(Path.Combine(Form1.tldPath, "TheLongDrive_Data\\Managed\\System.Xml.dll"));
					this.StartPatching();
					return;
				}
				MessageBox.Show(string.Format("0.1 backup file not found in:{1}{0}{1}Can't continue with upgrade{1}{1}Please check integrity files in steam, to recover original file.", Path.Combine(Form1.tldPath, "TheLongDrive_Data\\Managed\\Assembly-CSharp.original.dll"), Environment.NewLine), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
				//this.statusBarLabel.Text = "Error!";
				return;
			}
			else
			{
				if (this.tldloaderUpdate)
				{
					Console.WriteLine("TLDLoader.dll update!", true, true);
					Patcher.CopyReferences(Form1.tldPath);
					Patcher.CopyCoreAssets(this.modPath);
					Console.WriteLine("TLDLoader.dll update successful!", false, false);
					Console.WriteLine("", false, false);
					MessageBox.Show("Update successfull!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
					//this.statusBarLabel.Text = "Update successfull!";
					this.CheckPatchStatus();
					return;
				}
				this.StartPatching();
				return;
			}
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00002BD0 File Offset: 0x00000DD0
		public void StartPatching()
		{
			Console.WriteLine("Start patching game files!", true, true);
			try
			{
				Patcher.CopyReferences(Form1.tldPath);
				File.Copy(this.AssemblyFullPath, string.Format("{0}.backup", this.AssemblyFullPath));
				Console.WriteLine("Creating.....Assembly-CSharp.dll.backup", false, false);
				Console.WriteLine(string.Format("Patching.....{0}", Path.GetFileName(this.AssemblyFullPath)), false, false);
				this.PatchThis(Path.Combine(Form1.tldPath, "TheLongDrive_Data\\Managed\\"), "Assembly-CSharp.dll", "mainmenuscript", "Start", "TLDLoader.dll", "TLDLoader.ModLoader", this.InitMethod);
			}
			catch (Exception ex)
			{
				MessageBox.Show(string.Format("Error while patching loader: {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Console.WriteLine(string.Format("Error while patching loader: {0}", ex.Message), false, false);
			}
			try
			{
				this.PatchThisLast(Path.Combine(Form1.tldPath, "TheLongDrive_Data\\Managed\\"), "Assembly-CSharp.dll", "itemdatabase", "Awake", "TLDLoader.dll", "TLDLoader.ModLoader", "dbInit");
				Console.WriteLine("Finished patching!", false, false);
				Patcher.CopyCoreAssets(this.modPath);
				Console.WriteLine("Patching successfull!", false, false);
				Console.WriteLine("", false, false);
				MessageBox.Show("Patching successfull!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				//this.statusBarLabel.Text = "Patching successfull!";
			}
			catch (Exception ex2)
			{
				MessageBox.Show(string.Format("Error while patching itemdatabase: {0}", ex2.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Console.WriteLine(string.Format("Error while patching itemdtabase: {0}", ex2.Message), false, false);
			}
			this.CheckPatchStatus();
		}

		// Token: 0x0600000D RID: 13 RVA: 0x00002D88 File Offset: 0x00000F88
		private void PatchThisLast(string mainPath, string assemblyToPatch, string assemblyType, string assemblyMethod, string loaderAssembly, string loaderType, string loaderMethod)
		{
			DefaultAssemblyResolver defaultAssemblyResolver = new DefaultAssemblyResolver();
			defaultAssemblyResolver.AddSearchDirectory(mainPath);
			ModuleDefinition moduleDefinition = ModuleDefinition.ReadModule(mainPath + "/" + assemblyToPatch, new ReaderParameters
			{
				ReadWrite = true,
				AssemblyResolver = defaultAssemblyResolver
			});
			ModuleDefinition moduleDefinition2 = ModuleDefinition.ReadModule(mainPath + "/" + loaderAssembly);
			MethodDefinition method = moduleDefinition2.GetType(loaderType).Methods.Single((MethodDefinition x) => x.Name == loaderMethod);
			MethodDefinition methodDefinition = moduleDefinition.GetType(assemblyType).Methods.First((MethodDefinition x) => x.Name == assemblyMethod);
			Instruction instruction = Instruction.Create(OpCodes.Call, moduleDefinition.ImportReference(method));
			methodDefinition.Body.GetILProcessor().InsertAfter(methodDefinition.Body.Instructions[methodDefinition.Body.Instructions.Count - 2], instruction);
			moduleDefinition.Write();
			moduleDefinition.Dispose();
			moduleDefinition2.Dispose();
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00002E88 File Offset: 0x00001088
		public void Starter()
		{
			Patcher.DeleteIfExists(Path.Combine(Form1.tldPath, "TheLongDrive_Data\\Managed\\Assembly-CSharp.dll.backup"));
			Patcher.DeleteIfExists(tldPath + "\\Extract.zip");

			WebClient client = new WebClient();
			client.Credentials = new NetworkCredential("admin", "1331");

			client.DownloadFile(urizip, tldPath + "\\Extract.zip");
			if(!Directory.Exists("Extract"))
			{
				Directory.CreateDirectory("Extract");
			}
			else if(Directory.Exists("Extract"))
			{
				Directory.Delete("Extract", true);
				Directory.CreateDirectory("Extract");
			}
			ZipFile.ExtractToDirectory(tldPath + "\\Extract.zip", "Extract");

			//this.button1.Enabled = false;
			if (this.AssemblyFullPath != null)
			{
				this.InitMethod = "InitMainMenu";
				try
				{
					this.modPath = this.mdPath;
					this.PatchStarter();
					if (!Directory.Exists(this.mdPath))
					{
						Directory.CreateDirectory(this.mdPath);
					}
					return;
				}
				catch (Exception ex)
				{
					MessageBox.Show(string.Format("Error:{1}{0}{1}{1}Please restart patcher!", ex.Message, Environment.NewLine), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
			}
			MessageBox.Show("Select game path first", "Error path unknown", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		// Token: 0x0600000F RID: 15 RVA: 0x00002F2C File Offset: 0x0000112C
		public void SelectFolder()
		{
			OpenFileDialog selectPathToMSC = new OpenFileDialog();
			selectPathToMSC.Title = "Select TheLongDrive.exe";
			if (selectPathToMSC.ShowDialog() == DialogResult.OK)
			{
				Form1.tldPath = Path.GetDirectoryName(selectPathToMSC.FileName);
				//this.tldPathLabel.Text = Form1.tldPath;
				this.AssemblyFullPath = Path.Combine(Form1.tldPath, this.AssemblyPath);
				this.gfPath = Path.Combine(Form1.tldPath, "Mods");
				if (Directory.Exists(this.gfPath))
				{
					Console.WriteLine(string.Format("Found mods folder in: {0}", this.gfPath), false, false);
					this.modPath = this.gfPath;
				}
				Console.WriteLine(string.Format("Game folder set to: {0}", Form1.tldPath), false, false);
				File.WriteAllText("TLDFolder.txt", Form1.tldPath);
				Console.WriteLine(string.Format("Game folder is saved as: {0}{1}", Form1.tldPath, Environment.NewLine), false, false);
				this.CheckPatchStatus();
			}
		}

		public bool flag = false;
		// Token: 0x06000010 RID: 16 RVA: 0x00003010 File Offset: 0x00001210
		public void CheckPatchStatus()
		{
			tldloaderUpdate = false;
			isgameUpdated = false;
			oldPatchFound = false;
			oldFilesFound = false;
			//this.button1.Enabled = false;
			//this.button3.Enabled = false;
			flag = false;
			bool flag2 = false;
			try
			{
				flag = this.IsPatched(Path.Combine(Form1.tldPath, "TheLongDrive_Data\\Managed\\Assembly-CSharp.dll"), "mainmenuscript", "Start", "TLDLoader.dll", "TLDLoader.ModLoader", "InitMainMenu");
			}
			catch (Exception ex)
			{
				MessageBox.Show(string.Format("Patch11 checking error: {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Console.WriteLine(string.Format("Patch checking error: {0}", ex.Message), false, false);
			}
			if (!flag)
			{
				try
				{
					flag2 = false;
				}
				catch (Exception ex2)
				{
					MessageBox.Show(string.Format("Patch checking error: {0}", ex2.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					Console.WriteLine(string.Format("Patch checking error: {0}", ex2.Message), false, false);
				}
			}
			if (!flag && !flag2)
			{
				if (File.Exists(Path.Combine(Form1.tldPath, "TheLongDrive_Data\\Managed\\Assembly-CSharp.original.dll")))
				{
					Console.WriteLine("Patch not found, but TLDLoader 0.1 files found (probably there was game update)", false, false);
					//this.button1.Text = "Install TLDLoader";
					//this.button1.Enabled = true;
					this.oldFilesFound = true;
				}
				else if (File.Exists(Path.Combine(Form1.tldPath, "TheLongDrive_Data\\Managed\\TLDLoader.dll")) && File.Exists(Path.Combine(Form1.tldPath, "TheLongDrive_Data\\Managed\\Assembly-CSharp.dll.backup")))
				{
					//this.statusLabelText.Text = "Not patched, but TLDLoader found (probably there was game update)";
					Console.WriteLine("Patch not found, but TLDLoader files found (looks like there was game update)", false, false);
					//this.button1.Text = "Install TLDLoader";
					//this.button1.Enabled = true;
					this.isgameUpdated = true;
				}
				else
				{
					//this.statusLabelText.Text = "Not installed";
					Console.WriteLine("Patch not found, ready to install patch", false, false);
					//this.button1.Text = "Install TLDLoader";
					//this.button1.Enabled = true;
				}
				//this.statusLabelText.ForeColor = Color.Red;
				return;
			}
			if (!flag)
			{
				if (flag2)
				{
					//this.statusLabelText.Text = "0.1 patch found, upgrade available";
					Console.WriteLine("Old patch found, ready to upgrade", false, false);
					//this.button1.Text = "Upgrade TLDLoader";
					//this.button1.Enabled = true;
					//this.statusLabelText.ForeColor = Color.Orange;
					this.oldPatchFound = true;
				}
				return;
			}
			if (Form1.MD5HashFile(Path.Combine(Form1.tldPath, "TheLongDrive_Data\\Managed\\TLDLoader.dll")) == Form1.MD5HashFile(Path.GetFullPath(Path.Combine("TLDLoader.dll", ""))))
			{
				//this.statusLabelText.Text = "Installed, TLDLoader.dll is up to date.";
				Console.WriteLine("Newest patch found, no need to patch again", false, false);
				//this.button1.Enabled = false;
				//this.button3.Enabled = true;
				//this.statusLabelText.ForeColor = Color.Green;
				this.tldloaderUpdate = false;
				return;
			}
			//this.statusLabelText.Text = "Installed, but TLDLoader.dll mismatch, Update?";
			Console.WriteLine("Newest patch found, but TLDLoader.dll version mismatch, update TLDLoader?", false, false);
			//this.button1.Enabled = true;
			//this.button1.Text = "Update TLDLoader";
			//this.button3.Enabled = true;
			//this.statusLabelText.ForeColor = Color.Blue;
			this.tldloaderUpdate = true;
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00003360 File Offset: 0x00001560
		public static string MD5HashFile(string fn)
		{
			return BitConverter.ToString(MD5.Create().ComputeHash(File.ReadAllBytes(fn))).Replace("-", "");
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00003388 File Offset: 0x00001588
		private void button3_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("Do you want to remove TLDLoader from game?", "Remove?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				try
				{
					//this.button3.Enabled = false;
					Console.WriteLine("Removing TLDLoader from game", true, true);
					if (File.Exists(string.Format("{0}.backup", this.AssemblyFullPath)))
					{
						Patcher.DeleteIfExists(this.AssemblyFullPath);
						File.Move(string.Format("{0}.backup", this.AssemblyFullPath), this.AssemblyFullPath);
						Console.WriteLine("Recovering.....Assembly-CSharp.dll.backup", false, false);
						Patcher.DeleteReferences(Form1.tldPath);
						Console.WriteLine("", false, true);
						Console.WriteLine("TLDLoader removed successfully!", false, false);
						Console.WriteLine("", false, false);
						MessageBox.Show("TLDLoader removed successfully!", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
						//this.statusBarLabel.Text = "TLDLoader removed successfully!";
					}
					else
					{
						Console.WriteLine("Error! Backup file not found", false, false);
						MessageBox.Show(string.Format("Backup file not found in:{1}{0}{1}Can't continue{1}{1}Please check integrity files in steam, to recover original file.", string.Format("{0}.backup", this.AssemblyFullPath), Environment.NewLine), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
					this.CheckPatchStatus();
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
					//this.statusBarLabel.Text = "Error: " + ex.Message;
				}
			}
		}

		// Token: 0x06000013 RID: 19 RVA: 0x000034E8 File Offset: 0x000016E8
		private void LaunchMSCsruSteam()
		{
			try
			{
				Console.WriteLine("Starting game on steam", true, false);
				Process.Start("steam://rungameid/1017180");
				Environment.Exit(0);
			}
			catch (Exception ex)
			{
				MessageBox.Show(string.Format("Failed to run TLD, is steam installed correctly?{1}{1}Error details:{1}{0}", ex.Message, Environment.NewLine), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		// Token: 0x06000014 RID: 20 RVA: 0x0000354C File Offset: 0x0000174C
		private void button4_Click(object sender, EventArgs e)
		{
			this.LaunchMSCsruSteam();
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00003554 File Offset: 0x00001754
		private void linkDebug_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			try
			{
				Process.Start("https://github.com/piotrulos/MSCModLoader/wiki/Debugging-your-mods-in-Visual-Studio");
			}
			catch (Exception ex)
			{
				MessageBox.Show(string.Format("Failed to open url!{1}{1}Error details:{1}{0}", ex.Message, Environment.NewLine), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		// Token: 0x06000016 RID: 22 RVA: 0x000035A4 File Offset: 0x000017A4
		private void PatchThis(string mainPath, string assemblyToPatch, string assemblyType, string assemblyMethod, string loaderAssembly, string loaderType, string loaderMethod)
		{
			DefaultAssemblyResolver defaultAssemblyResolver = new DefaultAssemblyResolver();
			defaultAssemblyResolver.AddSearchDirectory(mainPath);
			ModuleDefinition moduleDefinition = ModuleDefinition.ReadModule(mainPath + "/" + assemblyToPatch, new ReaderParameters
			{
				ReadWrite = true,
				AssemblyResolver = defaultAssemblyResolver
			});
			ModuleDefinition moduleDefinition2 = ModuleDefinition.ReadModule(mainPath + "/" + loaderAssembly);
			MethodDefinition method = moduleDefinition2.GetType(loaderType).Methods.Single((MethodDefinition x) => x.Name == loaderMethod);
			MethodDefinition methodDefinition = moduleDefinition.GetType(assemblyType).Methods.First((MethodDefinition x) => x.Name == assemblyMethod);
			Instruction instruction = Instruction.Create(OpCodes.Call, moduleDefinition.ImportReference(method));
			methodDefinition.Body.GetILProcessor().InsertBefore(methodDefinition.Body.Instructions[0], instruction);
			moduleDefinition.Write();
			moduleDefinition.Dispose();
			moduleDefinition2.Dispose();
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00003694 File Offset: 0x00001894
		private bool IsPatched(string assemblyToPatch, string assemblyType, string assemblyMethod, string loaderAssembly, string loaderType, string loaderMethod)
		{
			ModuleDefinition moduleDefinition = ModuleDefinition.ReadModule(assemblyToPatch);
			ModuleDefinition moduleDefinition2 = ModuleDefinition.ReadModule(loaderAssembly);
			moduleDefinition2.GetType(loaderType).Methods.Single((MethodDefinition x) => x.Name == loaderMethod);
			foreach (Instruction instruction in moduleDefinition.GetType(assemblyType).Methods.First((MethodDefinition x) => x.Name == assemblyMethod).Body.Instructions)
			{
				if (instruction.OpCode.Equals(OpCodes.Call) && instruction.Operand.ToString().Equals(string.Concat(new string[]
				{
					"System.Void ",
					loaderType,
					"::",
					loaderMethod,
					"()"
				})))
				{
					moduleDefinition.Dispose();
					moduleDefinition2.Dispose();
					return true;
				}
			}
			moduleDefinition.Dispose();
			moduleDefinition2.Dispose();
			return false;
		}

		// Token: 0x06000018 RID: 24
		[DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

		// Token: 0x06000019 RID: 25
		[DllImport("user32.dll")]
		public static extern bool ReleaseCapture();

		// Token: 0x0600001A RID: 26 RVA: 0x000037C4 File Offset: 0x000019C4
		private void Move_Window(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				Form1.ReleaseCapture();
				Form1.SendMessage(base.Handle, 161, 2, 0);
			}
		}

		// Token: 0x0600001B RID: 27 RVA: 0x000037EC File Offset: 0x000019EC
		private void Minimise_Window(object sender, EventArgs e)
		{
			base.WindowState = FormWindowState.Minimized;
		}

		// Token: 0x0600001C RID: 28 RVA: 0x000037F8 File Offset: 0x000019F8
		private void Form1_Resize(object sender, EventArgs e)
		{
			Control[] array = new Control[]
			{
				//this.TitleBarLeft,
				//this.TitleBarRight,
				//this.ResizeBarBottom,
				//this.ResizeBarLeft,
				//this.ResizeBarRight
			};
			bool visible = base.WindowState != FormWindowState.Maximized;
			Control[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].Visible = visible;
			}
		}

		// Token: 0x0600001D RID: 29 RVA: 0x0000385D File Offset: 0x00001A5D
		private void Close_Window(object sender, EventArgs e)
		{
			Application.Exit();
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00003864 File Offset: 0x00001A64
		private void Resize_Window(object sender, MouseEventArgs e)
		{

		}

		// Token: 0x0600001F RID: 31 RVA: 0x000038C5 File Offset: 0x00001AC5
		private void TitleBarRight_Click(object sender, EventArgs e)
		{
		}

		// Token: 0x06000020 RID: 32 RVA: 0x000038C5 File Offset: 0x00001AC5
		private void groupBox1_Enter(object sender, EventArgs e)
		{
		}

		// Token: 0x04000003 RID: 3
		public static Form1 form1;

		// Token: 0x04000004 RID: 4
		public static string tldPath = "(unknown)";

		// Token: 0x04000005 RID: 5
		public static string exePath = "";

		// Token: 0x04000006 RID: 6
		private string AssemblyPath = "TheLongDrive_Data\\Managed\\Assembly-CSharp.dll";

		// Token: 0x04000007 RID: 7
		private string AssemblyFullPath = "";

		// Token: 0x04000008 RID: 8
		private string InitMethod = "InitMainMenu";

		// Token: 0x04000009 RID: 9
		private string mdPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TheLongDrive\\Mods");

		// Token: 0x0400000A RID: 10
		private string gfPath = "?";

		// Token: 0x0400000B RID: 11
		private string modPath = "";

		// Token: 0x0400000C RID: 12
		private bool is64bin;

		// Token: 0x0400000D RID: 13
		private bool isgameUpdated;

		// Token: 0x0400000E RID: 14
		public bool oldPatchFound;

		// Token: 0x0400000F RID: 15
		public bool oldFilesFound;

		// Token: 0x04000010 RID: 16
		public bool tldloaderUpdate;

		// Token: 0x04000011 RID: 17
		private FileVersionInfo tldLoaderVersion;

		// Token: 0x04000012 RID: 18
		public const int WM_NCLBUTTONDOWN = 161;

		// Token: 0x04000013 RID: 19
		public const int HT_CAPTION = 2;
	}
}
