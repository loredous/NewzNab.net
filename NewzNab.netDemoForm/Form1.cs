using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NewzNab.net;


namespace NewzNab.netDemoForm
{
    public partial class Form1 : Form
    {
        NewzNabSource FormSource;
        BindingSource Src;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnUseConfig_Click(object sender, EventArgs e)
        {
            FormSource = new NewzNabSource(txtURL.Text, chkSSL.Checked, txtAPI.Text);
            if (FormSource.GetCapabilities())
            {
                clsCategories.Items.Clear();
                clsCategories.DisplayMember = "Value";
                clsCategories.ValueMember = "Key";
                foreach (KeyValuePair<int,string> Category in FormSource.Capabilities.Categories)
                {
                    clsCategories.Items.Add(Category);
                }

                clsGenres.Items.Clear();
                foreach (NewzNabGenre Genre in FormSource.Capabilities.Genres)
                {
                    clsGenres.Items.Add(Genre);
                }

                clsGroups.Items.Clear();
                foreach (UsenetGroup Group in FormSource.Capabilities.Groups)
                {
                    clsGroups.Items.Add(Group);
                }

                cmbSearchType.Items.Clear();
                cmbSearchType.Items.Add("General");
                cmbSearchType.SelectedIndex = 0;
                if (!FormSource.Capabilities.SearchAvail)
                {
                    btnSearch.Enabled = false;
                    cmbSearchType.Enabled = false;
                }
                if (FormSource.Capabilities.TVSearchAvail) { cmbSearchType.Items.Add("TV Search"); }
                if (FormSource.Capabilities.MovieSearchAvail) { cmbSearchType.Items.Add("Movie Search"); }
                if (FormSource.Capabilities.AudioSearchAvail) { cmbSearchType.Items.Add("Audio Search"); }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            NewzNabQuery Query = new NewzNabQuery();
            Query.Query = txtSearchTerms.Text;
            foreach (KeyValuePair<int, string> Category in clsCategories.CheckedItems)
            {
                Query.Categories.Add(Category.Key);
            }
            foreach (NewzNabGenre Genre in clsGenres.CheckedItems)
            {
                Query.Categories.Add(Genre.CategoryID);
            }
            foreach (UsenetGroup Group in clsGroups.CheckedItems)
            {
                Query.Groups.Add(Group.Name);
            }
            switch (cmbSearchType.SelectedItem.ToString())
            {
                case "General":
                    Query.RequestedFunction = Functions.SEARCH;
                    break;
                case "TV Search":
                    Query.RequestedFunction = Functions.TV_SEARCH;
                    break;
                case "Movie Search":
                    Query.RequestedFunction = Functions.MOVIE_SEARCH;
                    break;
                case "Audio Search":
                    Query.RequestedFunction = Functions.MUSIC_SEARCH;
                    break;
            }
            
            Src = new BindingSource(new BindingList<NewzNabSearchResult>(FormSource.Search(Query)), null);
            dataGridView1.DataSource = Src;

            lblResultCount.Text = Src.Count.ToString();


            
            
        }

        private void PrepareDataGridView()
        {
            dataGridView1.AutoGenerateColumns = false;

            DataGridViewColumn column = new DataGridViewTextBoxColumn();
            column.DataPropertyName = "Title";
            column.Name = "Title";
            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            column.SortMode = DataGridViewColumnSortMode.Programmatic;
            dataGridView1.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.DataPropertyName = "Category";
            column.Name = "Category";
            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            column.SortMode = DataGridViewColumnSortMode.Programmatic;
            dataGridView1.Columns.Add(column);

            //column = new DataGridViewTextBoxColumn();
            //column.DataPropertyName = "Description";
            //column.Name = "Description";
            //column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //dataGridView1.Columns.Add(column);

            column = new DataGridViewTextBoxColumn();
            column.DataPropertyName = "PublishDate";
            column.Name = "Publish Date";
            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            column.SortMode = DataGridViewColumnSortMode.Programmatic;
            dataGridView1.Columns.Add(column);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            PrepareDataGridView();
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value != null)
            {
                DataGridViewCell Cell = this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                Cell.ToolTipText = ((NewzNabSearchResult)(dataGridView1.Rows[e.RowIndex].DataBoundItem)).Description;
            }
        }
    }
}
