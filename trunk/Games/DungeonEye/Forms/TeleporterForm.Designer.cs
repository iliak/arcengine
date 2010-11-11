﻿namespace DungeonEye.Forms
{
	partial class TeleporterForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TeleporterForm));
			this.DoneBox = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.TeamBox = new System.Windows.Forms.CheckBox();
			this.MonsterBox = new System.Windows.Forms.CheckBox();
			this.ItemsBox = new System.Windows.Forms.CheckBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.VisibleBox = new System.Windows.Forms.CheckBox();
			this.ReusableBox = new System.Windows.Forms.CheckBox();
			this.ActiveBox = new System.Windows.Forms.CheckBox();
			this.SoundNameBox = new System.Windows.Forms.TextBox();
			this.LoadSoundBox = new System.Windows.Forms.Button();
			this.PlaySoundBox = new System.Windows.Forms.Button();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.UseSoundBox = new System.Windows.Forms.CheckBox();
			this.targetControl1 = new DungeonEye.Forms.TargetControl();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// DoneBox
			// 
			this.DoneBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.DoneBox.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.DoneBox.Location = new System.Drawing.Point(301, 203);
			this.DoneBox.Name = "DoneBox";
			this.DoneBox.Size = new System.Drawing.Size(75, 23);
			this.DoneBox.TabIndex = 0;
			this.DoneBox.Text = "Done";
			this.DoneBox.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.ItemsBox);
			this.groupBox1.Controls.Add(this.MonsterBox);
			this.groupBox1.Controls.Add(this.TeamBox);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(80, 105);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Usable by :";
			// 
			// TeamBox
			// 
			this.TeamBox.AutoSize = true;
			this.TeamBox.Location = new System.Drawing.Point(6, 19);
			this.TeamBox.Name = "TeamBox";
			this.TeamBox.Size = new System.Drawing.Size(53, 17);
			this.TeamBox.TabIndex = 0;
			this.TeamBox.Text = "Team";
			this.TeamBox.UseVisualStyleBackColor = true;
			this.TeamBox.CheckedChanged += new System.EventHandler(this.TeamBox_CheckedChanged);
			// 
			// MonsterBox
			// 
			this.MonsterBox.AutoSize = true;
			this.MonsterBox.Location = new System.Drawing.Point(6, 42);
			this.MonsterBox.Name = "MonsterBox";
			this.MonsterBox.Size = new System.Drawing.Size(69, 17);
			this.MonsterBox.TabIndex = 0;
			this.MonsterBox.Text = "Monsters";
			this.MonsterBox.UseVisualStyleBackColor = true;
			this.MonsterBox.CheckedChanged += new System.EventHandler(this.MonsterBox_CheckedChanged);
			// 
			// ItemsBox
			// 
			this.ItemsBox.AutoSize = true;
			this.ItemsBox.Location = new System.Drawing.Point(6, 65);
			this.ItemsBox.Name = "ItemsBox";
			this.ItemsBox.Size = new System.Drawing.Size(51, 17);
			this.ItemsBox.TabIndex = 0;
			this.ItemsBox.Text = "Items";
			this.ItemsBox.UseVisualStyleBackColor = true;
			this.ItemsBox.CheckedChanged += new System.EventHandler(this.ItemsBox_CheckedChanged);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.ActiveBox);
			this.groupBox2.Controls.Add(this.ReusableBox);
			this.groupBox2.Controls.Add(this.VisibleBox);
			this.groupBox2.Location = new System.Drawing.Point(98, 12);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(96, 105);
			this.groupBox2.TabIndex = 2;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Properties :";
			// 
			// VisibleBox
			// 
			this.VisibleBox.AutoSize = true;
			this.VisibleBox.Location = new System.Drawing.Point(7, 20);
			this.VisibleBox.Name = "VisibleBox";
			this.VisibleBox.Size = new System.Drawing.Size(56, 17);
			this.VisibleBox.TabIndex = 0;
			this.VisibleBox.Text = "Visible";
			this.VisibleBox.UseVisualStyleBackColor = true;
			this.VisibleBox.CheckedChanged += new System.EventHandler(this.VisibleBox_CheckedChanged);
			// 
			// ReusableBox
			// 
			this.ReusableBox.AutoSize = true;
			this.ReusableBox.Location = new System.Drawing.Point(7, 42);
			this.ReusableBox.Name = "ReusableBox";
			this.ReusableBox.Size = new System.Drawing.Size(71, 17);
			this.ReusableBox.TabIndex = 0;
			this.ReusableBox.Text = "Resuable";
			this.ReusableBox.UseVisualStyleBackColor = true;
			this.ReusableBox.CheckedChanged += new System.EventHandler(this.ReusableBox_CheckedChanged);
			// 
			// ActiveBox
			// 
			this.ActiveBox.AutoSize = true;
			this.ActiveBox.Location = new System.Drawing.Point(7, 65);
			this.ActiveBox.Name = "ActiveBox";
			this.ActiveBox.Size = new System.Drawing.Size(56, 17);
			this.ActiveBox.TabIndex = 0;
			this.ActiveBox.Text = "Active";
			this.ActiveBox.UseVisualStyleBackColor = true;
			this.ActiveBox.CheckedChanged += new System.EventHandler(this.ActiveBox_CheckedChanged);
			// 
			// SoundNameBox
			// 
			this.SoundNameBox.Location = new System.Drawing.Point(6, 45);
			this.SoundNameBox.Name = "SoundNameBox";
			this.SoundNameBox.ReadOnly = true;
			this.SoundNameBox.Size = new System.Drawing.Size(215, 20);
			this.SoundNameBox.TabIndex = 1;
			this.SoundNameBox.TextChanged += new System.EventHandler(this.SoundNameBox_TextChanged);
			// 
			// LoadSoundBox
			// 
			this.LoadSoundBox.AutoSize = true;
			this.LoadSoundBox.Image = ((System.Drawing.Image)(resources.GetObject("LoadSoundBox.Image")));
			this.LoadSoundBox.Location = new System.Drawing.Point(182, 71);
			this.LoadSoundBox.Name = "LoadSoundBox";
			this.LoadSoundBox.Size = new System.Drawing.Size(39, 22);
			this.LoadSoundBox.TabIndex = 3;
			this.LoadSoundBox.UseVisualStyleBackColor = true;
			this.LoadSoundBox.Click += new System.EventHandler(this.LoadSoundBox_Click);
			// 
			// PlaySoundBox
			// 
			this.PlaySoundBox.Image = ((System.Drawing.Image)(resources.GetObject("PlaySoundBox.Image")));
			this.PlaySoundBox.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.PlaySoundBox.Location = new System.Drawing.Point(6, 71);
			this.PlaySoundBox.Name = "PlaySoundBox";
			this.PlaySoundBox.Size = new System.Drawing.Size(87, 23);
			this.PlaySoundBox.TabIndex = 4;
			this.PlaySoundBox.Text = " Play sound";
			this.PlaySoundBox.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.PlaySoundBox.UseVisualStyleBackColor = true;
			this.PlaySoundBox.Click += new System.EventHandler(this.PlaySoundBox_Click);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.UseSoundBox);
			this.groupBox3.Controls.Add(this.PlaySoundBox);
			this.groupBox3.Controls.Add(this.LoadSoundBox);
			this.groupBox3.Controls.Add(this.SoundNameBox);
			this.groupBox3.Location = new System.Drawing.Point(12, 126);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(227, 100);
			this.groupBox3.TabIndex = 4;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Sound :";
			// 
			// UseSoundBox
			// 
			this.UseSoundBox.AutoSize = true;
			this.UseSoundBox.Location = new System.Drawing.Point(6, 19);
			this.UseSoundBox.Name = "UseSoundBox";
			this.UseSoundBox.Size = new System.Drawing.Size(77, 17);
			this.UseSoundBox.TabIndex = 5;
			this.UseSoundBox.Text = "Use sound";
			this.UseSoundBox.UseVisualStyleBackColor = true;
			this.UseSoundBox.CheckedChanged += new System.EventHandler(this.UseSoundBox_CheckedChanged);
			// 
			// targetControl1
			// 
			this.targetControl1.Dungeon = null;
			this.targetControl1.Location = new System.Drawing.Point(200, 12);
			this.targetControl1.Name = "targetControl1";
			this.targetControl1.Size = new System.Drawing.Size(182, 105);
			this.targetControl1.TabIndex = 3;
			// 
			// TeleporterForm
			// 
			this.AcceptButton = this.DoneBox;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(388, 238);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.targetControl1);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.DoneBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TeleporterForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Teleporter wizard";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button DoneBox;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox ItemsBox;
		private System.Windows.Forms.CheckBox MonsterBox;
		private System.Windows.Forms.CheckBox TeamBox;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.CheckBox ActiveBox;
		private System.Windows.Forms.CheckBox ReusableBox;
		private System.Windows.Forms.CheckBox VisibleBox;
		private TargetControl targetControl1;
		private System.Windows.Forms.TextBox SoundNameBox;
		private System.Windows.Forms.Button LoadSoundBox;
		private System.Windows.Forms.Button PlaySoundBox;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.CheckBox UseSoundBox;
	}
}