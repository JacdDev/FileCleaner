namespace FileCleaner
{
    public partial class Form1 : Form
    {
        Dictionary<string, List<string>> dicSameFiles = new Dictionary<string, List<string>>();

        public Form1()
        {
            InitializeComponent();
            label2.Text = "";
        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                textBox1.Text = folderBrowserDialog1.SelectedPath;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(textBox1.Text))
            {
                listBox1.Items.Clear();
                listBox2.Items.Clear();
                List<string> files = new List<string>();
                enumerateFiles(textBox1.Text, files);
                dicSameFiles = getSameFiles(files);

                foreach (string file in dicSameFiles.Keys)
                {
                    if (dicSameFiles[file].Count > 1)
                    {
                        listBox1.Items.Add(file);
                    }
                }

                if (listBox1.Items.Count > 0)
                {
                    label2.Text = "Archivos con duplicados:";
                }
                else
                {
                    label2.Text = "No se han encontrado duplicidades";
                }
            }
            else{
                MessageBox.Show("La carpeta no existe", "Error");
            }
        }

        private void enumerateFiles(string path, List<string> files)
        {

            IEnumerable<string> directories = Directory.EnumerateDirectories(path);
            foreach (string directory in directories)
            {
                enumerateFiles(directory,files);
            }

            files.AddRange(Directory.EnumerateFiles(path));
        }

        private Dictionary<string, List<string>> getSameFiles(List<string> files)
        {
            List<int> processedFiles = new List<int>();
            Dictionary<string,List<string>> dicSameFiles = new Dictionary<string,List<string>>();

            for(int i=0;i<files.Count;++i)
            {
                dicSameFiles.Add(files[i], new List<string>() { files[i] });
                processedFiles.Add(i);

                for (int j=i+1;j<files.Count;++j)
                {
                    if (!processedFiles.Contains(j)){

                        if (compareFiles(files[i], files[j]))
                        {
                            dicSameFiles[files[i]].Add(files[j]);
                            processedFiles.Add(j);
                        }

                    }
                }
            }

            return dicSameFiles;
        }

        private bool compareFiles(string filepath1, string filepath2)
        {
            using (var reader1 = new FileStream(filepath1, FileMode.Open, FileAccess.Read))
            {
                using (var reader2 = new FileStream(filepath2, FileMode.Open, FileAccess.Read))
                {
                    byte[]? hash1;
                    byte[]? hash2;

                    
                    using (var md51 = System.Security.Cryptography.MD5.Create())
                    {
                        md51.ComputeHash(reader1);
                        hash1 = md51.Hash;
                    }

                    using (var md52 = System.Security.Cryptography.MD5.Create())
                    {
                        md52.ComputeHash(reader2);
                        hash2 = md52.Hash;
                    }

                    bool sameLength = hash1?.Length == hash1?.Length;
                    if (sameLength)
                    {
                        for (int i = 0; i < hash1?.Length; ++i)
                        {
                            if (hash1[i] != hash2?[i])
                            {
                                return false;
                            }
                        }

                    }

                    return sameLength;
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
            if (listBox1.SelectedItem != null)
            {
                foreach (string file in dicSameFiles[(string)listBox1.SelectedItem])
                {
                    listBox2.Items.Add(file);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedItem != null)
            {
                string cmd = "explorer.exe";
                string arg = "/select, " + listBox2.SelectedItem;
                System.Diagnostics.Process.Start(cmd, arg);
            }
            else
            {
                MessageBox.Show("Seleccione un archivo en la lista inferior", "Error");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedItem != null)
            {
                var confirmResult = MessageBox.Show("¿Seguro que quieres borrar "+ listBox2.SelectedItem+"?","Confirmar",MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.Yes)
                {
                    File.Delete((string)listBox2.SelectedItem);
                    listBox2.Items.Remove(listBox2.SelectedItem);
                }
                
            }
            else
            {
                MessageBox.Show("Seleccione un archivo en la lista inferior", "Error");
            }
        }
    }
}