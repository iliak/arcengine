#region Licence
//
//This file is part of ArcEngine.
//Copyright (C)2008-2009 Adrien H�mery ( iliak@mimicprod.net )
//
//ArcEngine is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//any later version.
//
//ArcEngine is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.
//
//You should have received a copy of the GNU General Public License
//along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
//
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using ArcEngine.Forms;
using ArcEngine.Storage;
using WeifenLuo.WinFormsUI.Docking;

namespace ArcEngine.Editor
{
	/// <summary>
	/// Main Editor form
	/// </summary>
	public partial class EditorMainForm : Form
	{


		/// <summary>
		/// Constructor
		/// </summary>
		public EditorMainForm(StorageBase storage)
		{
			Trace.WriteDebugLine("[Editor] Creating");

			// Form initialize
			InitializeComponent();

			Storage = storage;
			if (Storage != null)
				Text = "ArcEngine Editor : " + Storage;

			//Config.Load();


			// Log panel
			LogPanel = new LogForm(this);
			LogPanel.Show(dockPanel, DockState.DockBottomAutoHide);

			// Resource panel
			ResourcePanel = new ResourceForm();
			ResourcePanel.Show(dockPanel, DockState.DockLeft);
			ResourcePanel.Width = 200;
			ResourcePanel.RebuildResourceTree();


		}


		/// <summary>
		/// Opens a client form
		/// </summary>
		/// <param name="form">Form handle</param>
		public void OpenClientForm(EditorFormBase form)
		{
			if (form == null)
				return;

			form.Show(dockPanel, DockState.Document);
		}



		#region Menus envents

		/// <summary>
		/// Website
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void WebSiteMenu_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start("http://www.mimicprod.net/");

		}


		/// <summary>
		/// Collapse resource tree
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CollapseTreeMenu_Click(object sender, EventArgs e)
		{
			ResourcePanel.CollapseTree();
		}


		/// <summary>
		/// Quit the application
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			GC.Collect();
			Application.Exit();
		}


		//
		// About - About
		//
		private void MenuAboutAbout_Click(object sender, EventArgs e)
		{
			new AboutForm().ShowDialog();
		}




		/// <summary>
		///  Adds resource to the manager
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MenuTool_AddExistingResource_Click(object sender, EventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = "Resource bank (*.bnk)|*.bnk|All Files (*.*)|*.*";
			dlg.Title = "Select resource to open...";
			dlg.DefaultExt = ".bnk";

			DialogResult res = dlg.ShowDialog();
			if (res != DialogResult.OK)
				return;

			// Stay back in the good directory
			Environment.CurrentDirectory = Application.StartupPath;


			ResourceManager.AddStorage(new BankStorage(dlg.FileName));

			ResourcePanel.RebuildResourceTree();

		}


		/// <summary>
		/// Closes the selected TabPage from the menu File/Close
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CloseCurrentTab_OnClick(object sender, EventArgs e)
		{

		}


		/// <summary>
		/// Closes all open TabPage 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CloseAllTabs_OnClick(object sender, EventArgs e)
		{
			List<DockContent> windows = new List<DockContent>();

			foreach (DockContent window in dockPanel.Contents)
				windows.Add(window);

			foreach (DockContent window in windows)
			{
				if (window is AssetEditorBase)
					window.Close();
			}
		}


		/// <summary>
		/// Import an animation from a GIF image
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ImportAnimationFromGIF(object sender, EventArgs e)
		{
			new ImportGIFForm(Storage).ShowDialog();
			ResourcePanel.RebuildResourceTree();

		}


		/// <summary>
		/// Saves the resource under a new name from menu File/Save As
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FileSaveAs_OnClick(object sender, EventArgs e)
		{
			using (SaveFileDialog dlg = new SaveFileDialog())
			{
				dlg.Filter = "Resource bank (*.bnk)|*.bnk|All Files (*.*)|*.*";
				dlg.Title = "Save resource as...";
				dlg.DefaultExt = ".bnk";
				dlg.RestoreDirectory = true;
				dlg.AddExtension = true;
				dlg.CheckPathExists = true;
				dlg.OverwritePrompt = true;
				dlg.ValidateNames = true;

				DialogResult res = dlg.ShowDialog();
				if (res != DialogResult.OK)
					return;

				if (Storage != null)
					Storage.Dispose();

				Storage = new BankStorage(dlg.FileName);
			}

			ResourceManager.SaveAssetsToStorage(Storage);

			ResourcePanel.RebuildResourceTree();

		}


		#endregion


		#region Form Events


		/// <summary>
		/// Expand resource tree 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ExpandTreeMenu_Click(object sender, EventArgs e)
		{
			ResourcePanel.ExpandTree();
		}





		/// <summary>
		/// Loads resources bank from a file
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OpenBank(object sender, EventArgs e)
		{
			using (OpenFileDialog dlg = new OpenFileDialog())
			{
				dlg.Filter = "Resource bank (*.bnk)|*.bnk|All Files (*.*)|*.*";
				dlg.Title = "Select resource to open...";
				dlg.DefaultExt = ".bnk";
				dlg.RestoreDirectory = true;
				dlg.CheckFileExists = true;
				dlg.CheckPathExists = true;
				dlg.Multiselect = false;


				DialogResult res = dlg.ShowDialog();
				if (res != DialogResult.OK)
					return;

				// Erases all resources
				CloseAllTabs_OnClick(null, null);
				ResourceManager.ClearAssets();

				// Load bank
				Directory.SetCurrentDirectory(@"D:\Dev\ArcEngine\Games\DungeonEye");
				Storage = new BankStorage(dlg.FileName);
				ResourceManager.AddStorage(Storage);
			}

			ResourcePanel.RebuildResourceTree();

			Text = "ArcEngine Editor : " + Storage;

		}


		/// <summary>
		/// Save all resources to a storage
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SaveResourcesToStorage(object sender, EventArgs e)
		{
			// Save all opened asset
			foreach (DockContent window in dockPanel.Contents)
			{
				if (window is AssetEditorBase)
				{
					AssetEditorBase asseteditor = window as AssetEditorBase;
					asseteditor.Save();
				}
			}


			// Select a storage
			if (Storage == null)
			{
				SaveFileDialog dlg = new SaveFileDialog();
				dlg.Filter = "Resource bank (*.bnk)|*.bnk|All Files (*.*)|*.*";
				dlg.Title = "Save resource as...";
				dlg.DefaultExt = ".bnk";
				dlg.RestoreDirectory = true;
				dlg.AddExtension = true;
				dlg.CheckPathExists = true;
				dlg.OverwritePrompt = true;
				dlg.ValidateNames = true;

				DialogResult res = dlg.ShowDialog();
				if (res != DialogResult.OK)
					return;

				Storage = new BankStorage(dlg.FileName);
			}


			Text = "ArcEngine Editor " + Storage;
			ResourceManager.SaveAssetsToStorage(Storage);
		}




		/// <summary>
		/// Creates an empty resource bank
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CreateNewBank(object sender, EventArgs e)
		{
			DialogResult res = MessageBox.Show("Erase all resources ?", "", MessageBoxButtons.YesNo);

			if (res != DialogResult.Yes)
				return;

			CloseAllTabs_OnClick(null, null);
			ResourceManager.ClearAssets();

			// New bank name
			if (Storage != null)
				Storage.Dispose();
			Storage = null;

			//
			ResourcePanel.RebuildResourceTree();
		}



		/// <summary>
		/// Shows preference dialog box
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PreferenceMenu_Click(object sender, EventArgs e)
		{
			PreferencesForm pref = new PreferencesForm();
			if (pref.ShowDialog() == DialogResult.Cancel)
				return;

		}


		/// <summary>
		/// Adds a binary to the bank
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AddBinary_OnClick(object sender, EventArgs e)
		{
			if (Storage == null)
				return;

			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = "All Files (*.*)|*.*";
			dlg.Title = "Select a binary file...";
			dlg.DefaultExt = ".*";
			dlg.Multiselect = true;
			dlg.RestoreDirectory = true;

			DialogResult res = dlg.ShowDialog();
			if (res != DialogResult.OK)
				return;

			for (int i = 0; i < dlg.FileNames.Length; i++)
			{
				// Open storage file
				using (Stream to = Storage.OpenFile(dlg.FileNames[i], FileAccess.Write))
				{
					// Temp memory stream
					MemoryStream mem = new MemoryStream();

					// Open file to copy
					using (Stream from = new FileStream(dlg.FileNames[i], FileMode.Open))
					{
						mem.SetLength(from.Length);
						from.Read(mem.GetBuffer(), 0, (int)from.Length);
						mem.Flush();
					}

					mem.WriteTo(to);

					mem.Close();
				}
			}

			ResourcePanel.RebuildResourceTree();
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void EditorForm_FormClosed(object sender, FormClosedEventArgs e)
		{

			CloseAllTabs_OnClick(null, null);

			//foreach (DockContent window in list)
			//{
			//   if (window is AssetEditor)
			//      window.Close();
			//}
			Trace.WriteDebugLine("[Editor] Closed");
		}


		/// <summary>
		/// Import bank
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnImport(object sender, EventArgs e)
		{

		}


		/// <summary>
		/// Export bank
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnExport(object sender, EventArgs e)
		{
			ExportWizard wizard = new ExportWizard();
			wizard.ShowDialog();
		}


		#endregion


		#region Add new asset

		/// <summary>
		/// Adds a binary
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AddBinaryMenu_Click(object sender, EventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = "All Files (*.*)|*.*";
			dlg.Title = "Select a binary file...";
			dlg.DefaultExt = ".*";
			dlg.Multiselect = true;
			dlg.RestoreDirectory = true;

			DialogResult res = dlg.ShowDialog();
			if (res != DialogResult.OK)
				return;

			// for each selected file, add it to the bank file
			for (int i = 0; i < dlg.FileNames.Length; i++)
				ResourceManager.Load(dlg.SafeFileNames[i]);

			ResourcePanel.RebuildResourceTree();
		}


		/// <summary>
		/// Adds a bank to the resourcemanager
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void InsertBankMenu_Click(object sender, EventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = "Resource bank (*.bnk)|*.bnk|All Files (*.*)|*.*";
			dlg.Title = "Select resource to open...";
			dlg.DefaultExt = ".bnk";

			if (dlg.ShowDialog() != DialogResult.OK)
				return;

			ResourceManager.AddStorage(new BankStorage(dlg.FileName));

			ResourcePanel.RebuildResourceTree();
		}



		/// <summary>
		/// Close the bank
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MenuFileClose_Click(object sender, EventArgs e)
		{

		}


		/// <summary>
		/// Creates a new asset
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void NewAssetBox_Click(object sender, EventArgs e)
		{
			new WizardForm(dockPanel, null).ShowDialog();
			ResourcePanel.RebuildResourceTree();

		}



		#endregion


		#region Properties

		/// <summary>
		/// Resource panel
		/// </summary>
		ResourceForm ResourcePanel;


		/// <summary>
		/// Log panel
		/// </summary>
		LogForm LogPanel;


		/// <summary>
		/// DockPanel for asset forms
		/// </summary>
		public DockPanel AssetPanel
		{
			get
			{
				return dockPanel;
			}
		}


		/// <summary>
		/// MenuStrip handle
		/// </summary>
		public MenuStrip MenuHandle
		{
			get
			{
				return MainMenu;
			}
		}


		/// <summary>
		/// Tool bar handle
		/// </summary>
		public ToolStrip ToolBarHandle
		{
			get
			{
				return MainToolStrip;
			}
		}


		/// <summary>
		/// Storage 
		/// </summary>
		StorageBase Storage;

		#endregion


	}


	/// <summary>
	/// Editor event arguments
	/// </summary>
	public class EditorEventArgs : EventArgs
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="form">Editor's form handle</param>
		public EditorEventArgs(EditorMainForm form)
		{
			Form = form;
		}


		#region Properties


		/// <summary>
		/// Editor's form handle
		/// </summary>
		public EditorMainForm Form
		{
			get;
			private set;
		}

		#endregion
	}
}