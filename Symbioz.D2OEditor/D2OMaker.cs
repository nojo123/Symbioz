using Stump.DofusProtocol.D2oClasses.Tools.D2o;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Symbioz.D2OEditor
{
    public class D2OMaker
    {
        private D2OReader m_reader { get; set; }

        public Type D2OType { get { return m_currentObjects.Values.First().GetType(); } }

        private Dictionary<int, object> m_currentObjects { get; set; }
        public D2OMaker(string fileName)
        {
            m_reader = new D2OReader(fileName);
            this.m_currentObjects = m_reader.ReadObjects();
        }


        public FieldInfo[] GetFields()
        {
            return D2OType.GetFields();
        }
        public string D2OName { get { return m_reader.FileName; } }

        void Reload()
        {
            m_reader = new D2OReader(m_reader.FilePath);
        }

        public void Synchronize(DataGridView dataGrid)
        {
            var writer = new D2OWriter(m_reader.FilePath);
       
            // Delete old values
            foreach (var obj in m_currentObjects) 
            {
                writer.Delete(obj.Key);
            }
    
            // Row is an D2O Object

            List<string[]> values = new List<string[]>();


            foreach (DataGridViewRow row in dataGrid.Rows)
            {
                List<string> subValues = new List<string>();
                // cell is a field value
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Value != null)
                    subValues.Add(cell.Value.ToString());
                }
                values.Add(subValues.ToArray());
            }


            foreach (var objValues in values)
            {
                var fields = GetFields();
                if (objValues.Length == fields.Length)
                {
                    object D2oObject = Activator.CreateInstance(D2OType);
                    for (int i = 0; i < fields.Length; i++)
                    {

                        FieldInfo field = fields[i];
                        var objValue = objValues[i];

                        var realValue = Convert.ChangeType(objValue, field.FieldType);
                        field.SetValue(D2oObject, realValue);
                    }
                    writer.Write(D2oObject);
                }
            }

            writer.Dispose();

        }
        public void Load(DataGridView dataGrid)
        {
            dataGrid.Columns.Clear();
            dataGrid.Rows.Clear();

            string[] fields = GetFields().ToList().ConvertAll<string>(x => x.Name).ToArray();
            List<string[]> values = GetStringValues();

            foreach (var field in fields)
            {
                dataGrid.Columns.Add(field, field);
            }

            foreach (var value in values)
            {
                dataGrid.Rows.Add(value);
            }

        }
        public List<string[]> GetStringValues()
        {

            List<string[]> values = new List<string[]>();
            List<string> subValues = new List<string>();
            foreach (var obj in m_currentObjects.Values)
            {
                subValues.Clear();
                foreach (var data in GetFields())
                {
                    subValues.Add(data.GetValue(obj).ToString());
                }
                values.Add(subValues.ToArray());
            }
            return values;

        }
    }
}
