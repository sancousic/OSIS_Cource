using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OSIS_Cource
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = Server.Tasks;

            DataGridViewColumn SourceColumn = new DataGridViewTextBoxColumn();
            SourceColumn.HeaderText = "Source";
            SourceColumn.DataPropertyName = "Source";
            SourceColumn.ReadOnly = true;
            DataGridViewColumn DestColumn = new DataGridViewTextBoxColumn();
            DestColumn.DataPropertyName = "Dest";
            DestColumn.HeaderText = "Dest";
            DestColumn.ReadOnly = true;
            DataGridViewColumn CompleteColumn = new DataGridViewCheckBoxColumn();
            CompleteColumn.DataPropertyName = "IsComplete";
            CompleteColumn.HeaderText = "Status";
            CompleteColumn.ReadOnly = true;
            CompleteColumn.Resizable = DataGridViewTriState.False;
            DataGridViewColumn TypeColumn = new DataGridViewTextBoxColumn();
            TypeColumn.DataPropertyName = "Type";
            TypeColumn.HeaderText = "Type";
            TypeColumn.ReadOnly = true;
            TypeColumn.Resizable = DataGridViewTriState.False;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { SourceColumn, DestColumn, TypeColumn, CompleteColumn });

            radioButton1.Checked = true;

            Server.Clients.ListChanged += Clients_ListChanged;
            listBox1.DisplayMember = "Client_str";
            listBox1.ValueMember = "Id";

            tabControl1.TabIndexChanged += TabControl1_TabIndexChanged;
            tabControl1.SelectedIndexChanged += TabControl1_TabIndexChanged;

            timer1.Start();
            Server.Listen.Start();

            ViewModel.Form1 = this;
        }
        public ListBox GetListBox1 => listBox1;
        private void Clients_ListChanged(object sender, ListChangedEventArgs e)
        {
            listBox1.Invoke((Action)delegate {
                listBox1.Items.Clear();
                foreach(var c in Server.Clients)
                {
                    listBox1.Items.Add(c);
                }
            });
        }
        private void TabControl1_TabIndexChanged(object sender, EventArgs e)
        {
        }

        private void ListBox1_BindingContextChanged(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            TaskView taskView = new TaskView();
            taskView.Source = openFileDialog1.FileName;
            if(saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            taskView.Dest = saveFileDialog1.FileName;
            taskView.Type = radioButton1.Checked ? "Encrypt" : "Decrypt";
            Server.Tasks.Add(taskView);
            Server.queue.Enqueue((taskView.Source, textBox1.Text, taskView.Dest,
                radioButton1.Checked ? MessageType.EncryptDes : MessageType.DecryptDes));
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            Server.Clients[0].isFree = !Server.Clients[0].isFree;
        }
    }
}
