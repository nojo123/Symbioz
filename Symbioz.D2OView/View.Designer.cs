namespace Symbioz.D2OView
{
    partial class View
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.genesisTheme1 = new genesisTheme();
            this.txtalea = new genesisLabel();
            this.genesisLabel2 = new genesisLabel();
            this.genesisLabel1 = new genesisLabel();
            this.dbpass = new genesisTextBox();
            this.dbname = new genesisTextBox();
            this.dbhost = new genesisTextBox();
            this.textBox1 = new genesisLabel();
            this.dbuser = new genesisTextBox();
            this.genesisClose1 = new genesisClose();
            this.genesisButton1 = new genesisButton();
            this.genesisTextBox1 = new genesisTextBox();
            this.genesisTheme1.SuspendLayout();
            this.SuspendLayout();
            // 
            // genesisTheme1
            // 
            this.genesisTheme1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.genesisTheme1.BorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.genesisTheme1.Controls.Add(this.genesisTextBox1);
            this.genesisTheme1.Controls.Add(this.txtalea);
            this.genesisTheme1.Controls.Add(this.genesisLabel2);
            this.genesisTheme1.Controls.Add(this.genesisLabel1);
            this.genesisTheme1.Controls.Add(this.dbpass);
            this.genesisTheme1.Controls.Add(this.dbname);
            this.genesisTheme1.Controls.Add(this.dbhost);
            this.genesisTheme1.Controls.Add(this.textBox1);
            this.genesisTheme1.Controls.Add(this.dbuser);
            this.genesisTheme1.Controls.Add(this.genesisClose1);
            this.genesisTheme1.Controls.Add(this.genesisButton1);
            this.genesisTheme1.CurrentColor = HatchColors.White;
            this.genesisTheme1.CustomIcon = null;
            this.genesisTheme1.Customization = "+vr6//7+/v/z8/P/8PDw//7+/v/IyMj/+vr6//Dw8P////9G////AA==";
            this.genesisTheme1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.genesisTheme1.Font = new System.Drawing.Font("Verdana", 8F);
            this.genesisTheme1.HatchONOff = IsHatched.Yes;
            this.genesisTheme1.Image = null;
            this.genesisTheme1.Location = new System.Drawing.Point(0, 0);
            this.genesisTheme1.Movable = true;
            this.genesisTheme1.Name = "genesisTheme1";
            this.genesisTheme1.NoRounding = false;
            this.genesisTheme1.Sizable = true;
            this.genesisTheme1.Size = new System.Drawing.Size(866, 286);
            this.genesisTheme1.SmartBounds = true;
            this.genesisTheme1.StartPosition = System.Windows.Forms.FormStartPosition.WindowsDefaultLocation;
            this.genesisTheme1.TabIndex = 0;
            this.genesisTheme1.Text = "Symbioz.DBSync";
            this.genesisTheme1.TextColor = MultiColor.Black;
            this.genesisTheme1.TextPlacement = genesisTheme.TextLocation.Left;
            this.genesisTheme1.TransparencyKey = System.Drawing.Color.Empty;
            this.genesisTheme1.Transparent = false;
            this.genesisTheme1.UseAnIcon = genesisTheme.UseIcon.No;
            // 
            // txtalea
            // 
            this.txtalea.CurrentColor = MultiColor.Black;
            this.txtalea.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.txtalea.Location = new System.Drawing.Point(294, 108);
            this.txtalea.Name = "txtalea";
            this.txtalea.Size = new System.Drawing.Size(71, 13);
            this.txtalea.TabIndex = 9;
            this.txtalea.Text = "DbPass:";
            // 
            // genesisLabel2
            // 
            this.genesisLabel2.CurrentColor = MultiColor.Black;
            this.genesisLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.genesisLabel2.Location = new System.Drawing.Point(175, 108);
            this.genesisLabel2.Name = "genesisLabel2";
            this.genesisLabel2.Size = new System.Drawing.Size(71, 13);
            this.genesisLabel2.TabIndex = 8;
            this.genesisLabel2.Text = "DbUser:";
            // 
            // genesisLabel1
            // 
            this.genesisLabel1.CurrentColor = MultiColor.Black;
            this.genesisLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.genesisLabel1.Location = new System.Drawing.Point(294, 59);
            this.genesisLabel1.Name = "genesisLabel1";
            this.genesisLabel1.Size = new System.Drawing.Size(71, 13);
            this.genesisLabel1.TabIndex = 7;
            this.genesisLabel1.Text = "DbName:";
            // 
            // dbpass
            // 
            this.dbpass.Customization = "AAAA//X19f/c3Nz/";
            this.dbpass.Font = new System.Drawing.Font("Verdana", 8F);
            this.dbpass.Image = null;
            this.dbpass.Location = new System.Drawing.Point(271, 124);
            this.dbpass.MaxLength = 32767;
            this.dbpass.Multiline = false;
            this.dbpass.Name = "dbpass";
            this.dbpass.NoRounding = false;
            this.dbpass.ReadOnly = false;
            this.dbpass.Size = new System.Drawing.Size(116, 24);
            this.dbpass.TabIndex = 6;
            this.dbpass.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.dbpass.Transparent = false;
            this.dbpass.UseSystemPasswordChar = false;
            // 
            // dbname
            // 
            this.dbname.Customization = "AAAA//X19f/c3Nz/";
            this.dbname.Font = new System.Drawing.Font("Verdana", 8F);
            this.dbname.Image = null;
            this.dbname.Location = new System.Drawing.Point(271, 75);
            this.dbname.MaxLength = 32767;
            this.dbname.Multiline = false;
            this.dbname.Name = "dbname";
            this.dbname.NoRounding = false;
            this.dbname.ReadOnly = false;
            this.dbname.Size = new System.Drawing.Size(116, 24);
            this.dbname.TabIndex = 5;
            this.dbname.Text = "Symbioz";
            this.dbname.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.dbname.Transparent = false;
            this.dbname.UseSystemPasswordChar = false;
            // 
            // dbhost
            // 
            this.dbhost.Customization = "AAAA//X19f/c3Nz/";
            this.dbhost.Font = new System.Drawing.Font("Verdana", 8F);
            this.dbhost.Image = null;
            this.dbhost.Location = new System.Drawing.Point(149, 75);
            this.dbhost.MaxLength = 32767;
            this.dbhost.Multiline = false;
            this.dbhost.Name = "dbhost";
            this.dbhost.NoRounding = false;
            this.dbhost.ReadOnly = false;
            this.dbhost.Size = new System.Drawing.Size(116, 24);
            this.dbhost.TabIndex = 4;
            this.dbhost.Text = "127.0.0.1";
            this.dbhost.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.dbhost.Transparent = false;
            this.dbhost.UseSystemPasswordChar = false;
            // 
            // textBox1
            // 
            this.textBox1.CurrentColor = MultiColor.Black;
            this.textBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.textBox1.Location = new System.Drawing.Point(175, 59);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(53, 13);
            this.textBox1.TabIndex = 3;
            this.textBox1.Text = "DbHost:";
            // 
            // dbuser
            // 
            this.dbuser.Customization = "AAAA//X19f/c3Nz/";
            this.dbuser.Font = new System.Drawing.Font("Verdana", 8F);
            this.dbuser.Image = null;
            this.dbuser.Location = new System.Drawing.Point(149, 124);
            this.dbuser.MaxLength = 32767;
            this.dbuser.Multiline = false;
            this.dbuser.Name = "dbuser";
            this.dbuser.NoRounding = false;
            this.dbuser.ReadOnly = false;
            this.dbuser.Size = new System.Drawing.Size(116, 24);
            this.dbuser.TabIndex = 2;
            this.dbuser.Text = "root";
            this.dbuser.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.dbuser.Transparent = false;
            this.dbuser.UseSystemPasswordChar = false;
            // 
            // genesisClose1
            // 
            this.genesisClose1.Customization = "6+vr//X19f8YGBj//v7+/+Hh4f/r6+v/";
            this.genesisClose1.Font = new System.Drawing.Font("Verdana", 8F);
            this.genesisClose1.HoverColor = HoverOptions.Green;
            this.genesisClose1.Image = null;
            this.genesisClose1.Location = new System.Drawing.Point(557, 3);
            this.genesisClose1.Name = "genesisClose1";
            this.genesisClose1.NoRounding = false;
            this.genesisClose1.Size = new System.Drawing.Size(17, 17);
            this.genesisClose1.TabIndex = 1;
            this.genesisClose1.Text = "genesisClose1";
            this.genesisClose1.Transparent = false;
            this.genesisClose1.WindowState = System.Windows.Forms.FormWindowState.Normal;
            // 
            // genesisButton1
            // 
            this.genesisButton1.CurrentColor = MultiColor.Black;
            this.genesisButton1.Customization = "/Pz8/+7u7v/w8PD//v7+/+7u7v/8/Pz/IyMjHuHh4f/r6+v/";
            this.genesisButton1.Font = new System.Drawing.Font("Verdana", 8F);
            this.genesisButton1.Image = null;
            this.genesisButton1.Location = new System.Drawing.Point(12, 176);
            this.genesisButton1.Name = "genesisButton1";
            this.genesisButton1.NoRounding = false;
            this.genesisButton1.Size = new System.Drawing.Size(553, 30);
            this.genesisButton1.TabIndex = 0;
            this.genesisButton1.Text = "SyncDatabase";
            this.genesisButton1.Transparent = false;
            this.genesisButton1.Click += new System.EventHandler(this.genesisButton1_Click);
            // 
            // genesisTextBox1
            // 
            this.genesisTextBox1.Customization = "AAAA//X19f/c3Nz/";
            this.genesisTextBox1.Font = new System.Drawing.Font("Verdana", 8F);
            this.genesisTextBox1.Image = null;
            this.genesisTextBox1.Location = new System.Drawing.Point(570, 48);
            this.genesisTextBox1.MaxLength = 32767;
            this.genesisTextBox1.Multiline = true;
            this.genesisTextBox1.Name = "genesisTextBox1";
            this.genesisTextBox1.NoRounding = false;
            this.genesisTextBox1.ReadOnly = false;
            this.genesisTextBox1.Size = new System.Drawing.Size(259, 178);
            this.genesisTextBox1.TabIndex = 10;
            this.genesisTextBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.genesisTextBox1.Transparent = false;
            this.genesisTextBox1.UseSystemPasswordChar = false;
            // 
            // View
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(866, 286);
            this.Controls.Add(this.genesisTheme1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "View";
            this.Text = "Symbioz.D2OView";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.genesisTheme1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private genesisTheme genesisTheme1;
        private genesisLabel txtalea;
        private genesisLabel genesisLabel2;
        private genesisLabel genesisLabel1;
        private genesisTextBox dbpass;
        private genesisTextBox dbname;
        private genesisTextBox dbhost;
        private genesisLabel textBox1;
        private genesisTextBox dbuser;
        private genesisClose genesisClose1;
        private genesisButton genesisButton1;
        private genesisTextBox genesisTextBox1;


    }
}

