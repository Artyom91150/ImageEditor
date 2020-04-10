using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageEditor
{
    public partial class Form2 : Form
    {
        float[,] StructElementMatrix;
        Form1 form1;

        public Form2(float[,] _StructElementMatrix, Form1 _form1)
        {
            InitializeComponent();

            StructElementMatrix = _StructElementMatrix;
            form1 = _form1;

            numericUpDown1.Value = (int)Math.Sqrt(StructElementMatrix.Length);

            dataGridView1.RowCount = (int)Math.Sqrt(StructElementMatrix.Length);
            dataGridView1.ColumnCount = (int)Math.Sqrt(StructElementMatrix.Length);
            for(int i = 0; i < dataGridView1.RowCount; i++)
            {
                dataGridView1.Columns[i].Width = 50;
                dataGridView1.Rows[i].Height = 50;
            }

            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                for (int j = 0; j < dataGridView1.ColumnCount; j++)
                {
                    if (StructElementMatrix[i, j] == 1)
                    {
                        dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.Black;
                    }
                    else
                    {
                        dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.White;
                    }
                }
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }


        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {

            if (StructElementMatrix[e.RowIndex, e.ColumnIndex] == 1)
            {
                StructElementMatrix[e.RowIndex, e.ColumnIndex] -= 1;
            }
            else
            {
                StructElementMatrix[e.RowIndex, e.ColumnIndex] += 1;
            }

            if (StructElementMatrix[e.RowIndex, e.ColumnIndex] == 1)
            {
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.Black;
            }
            else
            {
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.White;
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            form1.StructElementMatrix = StructElementMatrix;
            this.Close();
        }


        private void numericUpDown1_MouseUp(object sender, MouseEventArgs e)
        {
            StructElementMatrix = new float[Convert.ToInt32(numericUpDown1.Value), Convert.ToInt32(numericUpDown1.Value)];

            dataGridView1.RowCount = Convert.ToInt32(numericUpDown1.Value);
            dataGridView1.ColumnCount = Convert.ToInt32(numericUpDown1.Value);
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                dataGridView1.Columns[i].Width = 50;
                dataGridView1.Rows[i].Height = 50;
            }

            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                for (int j = 0; j < dataGridView1.ColumnCount; j++)
                {
                    StructElementMatrix[i, j] = 1;
                    dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.Black;

                }
            }
        }
    }
}
