using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace GraphInstaller
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        void WriteLog(string s)
        {
            richTextBox1.Text += s;
            richTextBox1.Update();
        }

        bool vs2010 = false;
        private void button1_Click(object sender, EventArgs e)
        {
            StreamReader reader = null;
            StreamWriter writer = null;
            try
            {
                richTextBox1.Text = "";
                WriteLog("Checking the installation....");
                string csprojFile;
                string ProjectsDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Visual Studio 2008\\Projects\\";
                if (!Directory.Exists(ProjectsDir))
                {
                    ProjectsDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Visual Studio 2010\\Projects\\";
                    if (!Directory.Exists(ProjectsDir))
                        throw new Exception("Can not find the projects directory.");
                    vs2010 = true;
                }
                bool install = true;
                DialogResult r;
                if (Directory.Exists(ProjectsDir + textBox2.Text))
                {
                    r = MessageBox.Show("The project name already exists!\nDo you want to install GraphDLL on it without creating a new one?", "Confirmation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                    if (r == DialogResult.Yes)
                        install = false;
                    else if (r == DialogResult.Cancel)
                    {
                        WriteLog("\n\n[Installation Canceled!]");
                        return;
                    }
                }
                if (radioButton1.Checked)
                {
                    if (install)
                        CreateProject(textBox2.Text, ProjectsDir + textBox2.Text + "\\");
                    csprojFile = ProjectsDir + textBox2.Text + "\\" + textBox2.Text + "\\" + textBox2.Text + ".csproj";
                }
                else
                    csprojFile = textBox1.Text;
                FileInfo csprojinfo = new FileInfo(csprojFile);
                FileInfo exeinfo = new FileInfo(Application.ExecutablePath);
                string ProgramFile = csprojinfo.Directory + "\\Program.cs";
                string GraphDLLFile = exeinfo.Directory + "\\GraphDLL.dll";
                string WMPLibFile = exeinfo.Directory + "\\Interop.WMPLib.dll";
                string GraphDLLVersion = FileVersionInfo.GetVersionInfo(GraphDLLFile).FileVersion;
                if (!install)
                    WriteLog("[Done]\n");
                WriteLog("Opening csproj file....");
                reader = new StreamReader(csprojFile);
                writer = new StreamWriter("temp.txt");
                WriteLog("[Done]\n");

                string lines;
                while (!reader.EndOfStream)
                {
                    lines = reader.ReadLine();
                    writer.WriteLine(lines);
                    if (lines == "  <ItemGroup>")
                        break;
                }
                if (reader.EndOfStream) throw new FileLoadException("The file is illegual for the installation.");

                WriteLog("Checking the references....\n");
                bool WinForm = false, Drawing = false, GraphDLL = false;
                while (!reader.EndOfStream)
                {
                    lines = reader.ReadLine();
                    if (lines == "    <Reference Include=" + '"' + "System.Drawing" + '"' + " />")
                    {
                        Drawing = true;
                        WriteLog("\tSystem.Drawing found!\n");
                    }
                    else if (lines == "    <Reference Include=" + '"' + "System.Windows.Forms" + '"' + " />")
                    {
                        WinForm = true;
                        WriteLog("\tSystem.Windows.Forms found!\n");
                    }
                    else if (lines == "    <Reference Include=" + '"' + "GraphDLL, Version=2.0.4.3, Culture=neutral, processorArchitecture=MSIL" + '"' + ">")
                    {
                        GraphDLL = true;
                        WriteLog("\tGraphDLL found!\n");
                    }
                    else if (lines == "  </ItemGroup>")
                        break;

                    writer.WriteLine(lines);
                }
                if (reader.EndOfStream) throw new FileLoadException("The file is illegual for the installation.");

                if (!GraphDLL)
                {
                    writer.WriteLine("    <Reference Include=" + '"' + "GraphDLL, Version=" + GraphDLLVersion + ", Culture=neutral, processorArchitecture=MSIL" + '"' + ">");
                    writer.WriteLine("      <SpecificVersion>False</SpecificVersion>");
                    writer.WriteLine("      <HintPath>" + GraphDLLFile + " \\GraphDLL.dll</HintPath>");
                    writer.WriteLine("    </Reference>");
                    WriteLog("\tGraphDLL added!\n");
                }
                if (!Drawing)
                {
                    writer.WriteLine("    <Reference Include=" + '"' + "System.Drawing" + '"' + " />");
                    WriteLog("\tSystem.Drawing added!\n");
                }
                if (!WinForm)
                {
                    writer.WriteLine("    <Reference Include=" + '"' + "System.Windows.Forms" + '"' + " />");
                    WriteLog("\tSystem.Windows.Forms added!\n");
                }
                writer.WriteLine("  </ItemGroup>");

                if (!WinForm || !Drawing || !GraphDLL)
                {
                    while (!reader.EndOfStream)
                    {
                        lines = reader.ReadLine();
                        writer.WriteLine(lines);
                    }
                    reader.Close();
                    writer.Close();
                    File.Copy(csprojFile, csprojFile + ".bak", true);
                    File.Copy("temp.txt", csprojFile, true);
                }
                else
                {
                    reader.Close();
                    writer.Close();
                }

                WriteLog("Opening Program.cs file....");
                reader = new StreamReader(ProgramFile);
                writer = new StreamWriter("temp.txt");
                WriteLog("[Done]\n");
                WriteLog("Adding using....");
                writer.WriteLine("using GraphDLL;");
                writer.WriteLine("using System.Drawing;");
                writer.WriteLine("using System.Windows.Forms;");
                while (!reader.EndOfStream)
                {
                    lines = reader.ReadLine();
                    if (lines != "using GraphDLL;"
                            && lines != "using System.Drawing;"
                            && lines != "using System.Windows.Forms;")
                        writer.WriteLine(lines);
                }
                reader.Close();
                writer.Close();
                File.Copy(ProgramFile, ProgramFile + ".bak", true);
                File.Copy("temp.txt", ProgramFile, true);
                WriteLog("[Done]\n");

                WriteLog("Copying GraphDLL.dll (v" + GraphDLLVersion + ")...");
                Directory.CreateDirectory(csprojinfo.Directory + "\\bin\\Debug");
                File.Copy(GraphDLLFile, csprojinfo.Directory + "\\bin\\Debug\\GraphDLL.dll", true);
                WriteLog("[Done]\n");

                WriteLog("Copying Interop.WMPLib.dll...");
                File.Copy(WMPLibFile, csprojinfo.Directory + "\\bin\\Debug\\Interop.WMPLib.dll", true);
                WriteLog("[Done]\n");
                File.Delete("temp.txt");

                WriteLog("\n[Installation Successfull]");

                r = MessageBox.Show("Installation was successfull!\nDo you want to open the project?", "Installation Successfull", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (r == DialogResult.Yes)
                {
                    Process.Start(csprojFile);
                    Close();
                }
            }
            catch (Exception ex)
            {
                WriteLog("\n\nError: " + ex.Message + "\nEmail me for help: matinlotfali@gmail.com");
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    writer.Close();
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
                button1.Enabled = File.Exists(textBox1.Text);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
                button1.Enabled = (textBox2.Text.Length > 0) && (textBox2.Text[0] != '<');
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Enabled = !radioButton1.Checked;
            button2.Enabled = !radioButton1.Checked;
            textBox2.Enabled = radioButton1.Checked;
            textBox2.Text = "<Project Name>";
            textBox1_TextChanged(null, null);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult r = openFileDialog1.ShowDialog();
            if (r == DialogResult.OK)
                textBox1.Text = openFileDialog1.FileName;
        }

        void CreateProject(string name, string Path)
        {
            if (!vs2010)
            {
                WriteLog("[Done]\nCreating a new project...");
                string file = Path + name + ".sln";
                Directory.CreateDirectory(Path);
                BinaryWriter binwriter = new BinaryWriter(new FileStream(file, FileMode.Create));
                binwriter.Write(Properties.Resources.sln2008);
                binwriter.Close();
                ReplaceInFile("ConsoleApplication5", name, file);
                WriteLog("\n\t" + name + ".sln installed!");


                Directory.CreateDirectory(Path + name);
                file = Path + name + "\\" + name + ".csproj";
                binwriter = new BinaryWriter(new FileStream(file, FileMode.Create));
                binwriter.Write(Properties.Resources.csproj2008);
                binwriter.Close();
                ReplaceInFile("ConsoleApplication5", name, file);
                WriteLog("\n\t" + name + ".csproj installed!");

                file = Path + name + "\\Program.cs";
                binwriter = new BinaryWriter(new FileStream(file, FileMode.Create));
                binwriter.Write(Properties.Resources.Program2008);
                binwriter.Close();
                ReplaceInFile("ConsoleApplication5", name, file);
                WriteLog("\n\tProgram.cs installed!");

                Directory.CreateDirectory(Path + name + "\\Properties");
                file = Path + name + "\\Properties\\AssemblyInfo.cs";
                binwriter = new BinaryWriter(new FileStream(file, FileMode.Create));
                binwriter.Write(Properties.Resources.AssemblyInfo2008);
                binwriter.Close();
                ReplaceInFile("ConsoleApplication5", name, file);
                WriteLog("\n\tAssemblyInfo.cs installed!\n");
            }
            else
            {
                WriteLog("[Done]\nCreating a new project...");
                string file = Path + name + ".sln";
                Directory.CreateDirectory(Path);
                BinaryWriter binwriter = new BinaryWriter(new FileStream(file, FileMode.Create));
                binwriter.Write(Properties.Resources.sln2010);
                binwriter.Close();
                ReplaceInFile("ConsoleApplication6", name, file);
                WriteLog("\n\t" + name + ".sln installed!");


                Directory.CreateDirectory(Path + name);
                file = Path + name + "\\" + name + ".csproj";
                binwriter = new BinaryWriter(new FileStream(file, FileMode.Create));
                binwriter.Write(Properties.Resources.csproj2010);
                binwriter.Close();
                ReplaceInFile("ConsoleApplication6", name, file);
                WriteLog("\n\t" + name + ".csproj installed!");

                file = Path + name + "\\Program.cs";
                binwriter = new BinaryWriter(new FileStream(file, FileMode.Create));
                binwriter.Write(Properties.Resources.Program2010);
                binwriter.Close();
                ReplaceInFile("ConsoleApplication6", name, file);
                WriteLog("\n\tProgram.cs installed!");

                Directory.CreateDirectory(Path + name + "\\Properties");
                file = Path + name + "\\Properties\\AssemblyInfo.cs";
                binwriter = new BinaryWriter(new FileStream(file, FileMode.Create));
                binwriter.Write(Properties.Resources.AssemblyInfo2010);
                binwriter.Close();
                ReplaceInFile("ConsoleApplication6", name, file);
                WriteLog("\n\tAssemblyInfo.cs installed!\n");
            }
        }

        void ReplaceInFile(string a, string b, string file)
        {
            StreamReader reader = new StreamReader(file);
            StreamWriter writer = new StreamWriter("temp.txt");

            string line = reader.ReadLine();
            if (line.Length > 2)
                line = line.Remove(0, 2);
            line = line.Replace(a, b);
            writer.WriteLine(line);
            while (!reader.EndOfStream)
            {
                line = reader.ReadLine();
                line = line.Replace(a, b);
                writer.WriteLine(line);
            }
            reader.Close();
            writer.Close();
            File.Copy("temp.txt", file, true);
        }
    }
}
