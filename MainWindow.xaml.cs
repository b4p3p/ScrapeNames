using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ScrapeName
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string NAME_FILE = "names.csv";
        LinkedList<String> names_male = new LinkedList<string>();
        LinkedList<String> names_female = new LinkedList<string>();

        public MainWindow()
        {
            InitializeComponent();
            //cmdScrape_Click(null, null);
        }

        private void cmdScrape_Click(object sender, RoutedEventArgs e)
        {
            names_male = new LinkedList<string>();
            names_female = new LinkedList<string>();
            string URL = txtURL.Text;

            //prendo i nomi
            GetNames(URL);
            lstName_male.ItemsSource = names_male;
            lstName_female.ItemsSource = names_female;

            //creo il CSV
            CreateCSV();

        }

        

        private void GetNames ( string URL )
        {
            int page = 1;
            int result = 0;

            do
            {
                result = GetNames(URL , page);
                page++;
            } while (result > 0);

        }

        private int GetNames(string url, int page)
        {
            string URL = url + "/" + page;
            int cont = 0;

            HtmlWeb sites = new HtmlWeb();
            HtmlDocument doc = sites.Load(URL);

            //string test = website.DocumentNode.InnerHtml;

            string XPATH = "//div[contains(@class,'browsename')]";
            HtmlNodeCollection names = doc.DocumentNode.SelectNodes(XPATH);

            if (names == null) return 0;
            
            cont = names.Count;
            foreach (HtmlNode node in names)
            {
                HtmlNode node_name = node.SelectSingleNode("b/a");

                //nome
                string name = node_name.InnerText.Trim();
                if (name.Contains("(") || name.Contains(")") ||
                    name.Contains("&") || name.Contains(";")) continue;
                name = name.ToLower();
                name = char.ToUpper(name[0]) + name.Substring(1).ToLower();

                //genere
                HtmlNode node_gender = node.SelectSingleNode("span");
                string gender = node_gender.InnerText;

                if ( gender == "f")
                {
                    names_female.AddLast(name);
                }
                if (gender == "m")
                {
                    names_male.AddLast(name);
                }
            }

            return cont;
        }

        private void CreateCSV()
        {
            var csv = new StringBuilder();

            foreach (String name in names_male)
            {
                string newLine = string.Format("{0},{1}{2}", name, "m", Environment.NewLine);
                csv.Append(newLine); 
            }
            foreach (String name in names_female)
            {
                string newLine = string.Format("{0},{1}{2}", name, "f", Environment.NewLine);
                csv.Append(newLine);
            }
            File.WriteAllText(NAME_FILE, csv.ToString());
            MessageBox.Show("Scritto il file!");
        }
    }
}
