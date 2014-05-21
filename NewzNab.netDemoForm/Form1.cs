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
                    lstSearchResults.Items.Add("Searching is disabled!");
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
                Query.Groups.Add(Group.ID);
            }
        }
    }
}
