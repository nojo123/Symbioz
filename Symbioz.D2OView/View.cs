using Symbioz.D2O;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Symbioz.D2O.Classes;
using System.Reflection;
using System.Diagnostics;
using Symbioz.Enums;
using Symbioz.D2O.InternalClasses;
using Symbioz.DofusProtocol.D2O;
using System.Collections;

namespace Symbioz.D2OView
{
    public partial class View : Form
    {
        public SqlConnection SqlConnection { get; set; }
        public static string D2OPath = Environment.CurrentDirectory + "/D2O/";
        public View()
        {
            InitializeComponent();
            D2OActivator.OpenD2Os(D2OPath, true);
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void dRepertory_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        void ExecuteD2OClassSync(D2OSyncQueries sync)
        {
            Console.Title = "Processing: " + sync.D2OName + "s... ";

            int count = 0;
            foreach (var query in sync.Queries)
            {

                count++;
                bool succes = SqlConnection.Execute(query);
                if (!succes)
                {
                    Console.WriteLine("Error");
                    Console.ReadKey();
                }
                else
                {
                    var stay = (sync.Queries.Count - count);
                    Console.WriteLine(" Stays: " + stay);

                }
            }
        }
        void Process()
        {
            string queries = string.Empty;
            foreach (var pet in Pet.Pets)
            {
                ushort itemId = (ushort)pet.id;
                string str = string.Empty;

                foreach (var effect in pet.possibleEffects)
                {
                    str += effect.effectId + "#" + effect.diceNum + "#" + effect.diceSide + "#" + effect.value + "|";
                }
                queries += "UPDATE Items SET Effects = '" + str + "' where Id = " + itemId + ";";
                
            }
            genesisTextBox1.Text = queries;

        }
        private void genesisButton1_Click(object sender, EventArgs e)
        {
          
            SqlConnection = new SqlConnection(dbhost.Text, dbname.Text, dbuser.Text, dbpass.Text);
            if (SqlConnection.Open())
            {
              
                Process();
            }
            else
            {
                MessageBox.Show("Sql Connection Cannot be oppened");
            }
        }
    }
}
