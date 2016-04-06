using Stump.DofusProtocol.D2oClasses;
using Stump.DofusProtocol.D2oClasses.Tools.D2o;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Symbioz.D2OEditor
{
    public partial class EditorForm : Form
    {
        D2OMaker D2OMaker { get; set; }
        public EditorForm()
        {
            InitializeComponent();
            openFileDialog1.Title = "Open File";
            openFileDialog1.Filter = "D2O File | *.d2o";
            openFileDialog1.FileName = string.Empty;
            openFileDialog1.InitialDirectory = Environment.CurrentDirectory + @"\D2O\";
            dataGridView1.SelectionMode = DataGridViewSelectionMode.CellSelect;

            TEST();
        }
        void TEST()
        {
            D2OWriter writer = new D2OWriter(Environment.CurrentDirectory+"/D2O/breeds.D2O");
            writer.Write<Breed>(new Breed()
            {
                BreedRoles = new List<BreedRoleByBreed>(),
                BreedSpellsId = new List<uint>() { 100 },
                CreatureBonesId = 101,
                DescriptionId = 1000,
                FemaleArtwork = 101,
                LongNameId = 349,
                FemaleColors = new List<uint>() { 0, 0, 0, 0, 0 },
                MaleColors = new List<uint>() { 0, 0, 0, 0, 0 },
                FemaleLook = "{1003}",
                MaleLook = "{1003}",
                Id = 17,
                GameplayDescriptionId = 38484,

                StatsPointsForWisdom = new List<List<uint>>(),
                StatsPointsForVitality = new List<List<uint>>(),
                StatsPointsForStrength = new List<List<uint>>(),
                StatsPointsForIntelligence = new List<List<uint>>(),
                StatsPointsForChance = new List<List<uint>>(),
                StatsPointsForAgility = new List<List<uint>>(),
                SpawnMap = 0,
                ShortNameId = 348,
                MaleArtwork = 1
            });
            writer.Dispose();
        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName == string.Empty)
                return;
            else
            {
                D2OMaker = new D2OMaker(openFileDialog1.FileName);
                D2OMaker.Load(dataGridView1);
            }


        }

        
        private void button2_Click(object sender, EventArgs e)
        {
            this.dataGridView1.Rows.Add();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.Rows.Count > 1)
              this.dataGridView1.Rows.RemoveAt(dataGridView1.CurrentCellAddress.Y);

          

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {

            this.D2OMaker.Synchronize(dataGridView1);
            MessageBox.Show("D2O File Saved!");
        }

        private void EditorForm_Load(object sender, EventArgs e)
        {

        }
    }
}
