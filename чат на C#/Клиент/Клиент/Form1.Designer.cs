namespace Клиент
{
	partial class Form1
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
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
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			label1 = new Label();
			label2 = new Label();
			label3 = new Label();
			Заглавка = new Label();
			textBox1 = new TextBox();
			textBox2 = new TextBox();
			button1 = new Button();
			textBox3 = new TextBox();
			SuspendLayout();
			// 
			// label1
			// 
			resources.ApplyResources(label1, "label1");
			label1.FlatStyle = FlatStyle.Popup;
			label1.ForeColor = SystemColors.ControlLightLight;
			label1.Name = "label1";
			label1.Click += label1_Click;
			label1.MouseEnter += label1_MouseEnter;
			label1.MouseLeave += label1_MouseLeave;
			// 
			// label2
			// 
			resources.ApplyResources(label2, "label2");
			label2.FlatStyle = FlatStyle.Popup;
			label2.ForeColor = SystemColors.ControlLightLight;
			label2.Name = "label2";
			label2.Click += label2_Click;
			label2.MouseEnter += label2_MouseEnter;
			label2.MouseLeave += label2_MouseLeave;
			// 
			// label3
			// 
			resources.ApplyResources(label3, "label3");
			label3.FlatStyle = FlatStyle.Popup;
			label3.ForeColor = SystemColors.ControlLightLight;
			label3.Name = "label3";
			label3.Click += label3_Click;
			label3.MouseEnter += label3_MouseEnter;
			label3.MouseLeave += label3_MouseLeave;
			// 
			// Заглавка
			// 
			resources.ApplyResources(Заглавка, "Заглавка");
			Заглавка.FlatStyle = FlatStyle.Popup;
			Заглавка.ForeColor = SystemColors.ControlLightLight;
			Заглавка.Name = "Заглавка";
			// 
			// textBox1
			// 
			resources.ApplyResources(textBox1, "textBox1");
			textBox1.AllowDrop = true;
			textBox1.BackColor = Color.GhostWhite;
			textBox1.Name = "textBox1";
			textBox1.ReadOnly = true;
			// 
			// textBox2
			// 
			resources.ApplyResources(textBox2, "textBox2");
			textBox2.AllowDrop = true;
			textBox2.BackColor = Color.GhostWhite;
			textBox2.Name = "textBox2";
			// 
			// button1
			// 
			resources.ApplyResources(button1, "button1");
			button1.Name = "button1";
			button1.UseVisualStyleBackColor = true;
			button1.Click += button1_Click;
			// 
			// textBox3
			// 
			resources.ApplyResources(textBox3, "textBox3");
			textBox3.AllowDrop = true;
			textBox3.BackColor = Color.GhostWhite;
			textBox3.Name = "textBox3";
			// 
			// Form1
			// 
			resources.ApplyResources(this, "$this");
			AutoScaleMode = AutoScaleMode.Font;
			BackColor = Color.MediumPurple;
			Controls.Add(textBox3);
			Controls.Add(button1);
			Controls.Add(textBox2);
			Controls.Add(textBox1);
			Controls.Add(Заглавка);
			Controls.Add(label3);
			Controls.Add(label2);
			Controls.Add(label1);
			FormBorderStyle = FormBorderStyle.None;
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "Form1";
			FormClosing += Form_FormClosing;
			Load += Form_Load;
			MouseDown += Form1_MouseDown;
			MouseMove += Form1_MouseMove;
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion
		private Label label1;
		private Label label2;
		private Label label3;
		private Label Заглавка;
		private TextBox textBox2;
		private Button button1;
		private TextBox textBox3;
		public TextBox textBox1;
	}
}