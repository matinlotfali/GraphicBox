using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace GraphInstaller
{
    enum VisualStudio
    {
        vs2008, vs2010, vs2012, vs2013
    }

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        void WriteLog(string s)
        {
            label1.Text += s;
            label1.Update();
        }

        VisualStudio vsVersion;
        private void button1_Click(object sender, EventArgs e)
        {
            StreamReader reader = null;
            StreamWriter writer = null;
            try
            {
                label1.Text = "";
                WriteLog("Checking the installation....");
                string csprojFile;

                string ProjectsDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Visual Studio 2013\\Projects\\";
                if (Directory.Exists(ProjectsDir))
                    vsVersion = VisualStudio.vs2013;
                else
                {
                    ProjectsDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Visual Studio 2012\\Projects\\";
                    if (Directory.Exists(ProjectsDir))
                        vsVersion = VisualStudio.vs2012;
                    else
                    {
                        ProjectsDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Visual Studio 2010\\Projects\\";
                        if (Directory.Exists(ProjectsDir))
                            vsVersion = VisualStudio.vs2010;
                        else
                        {
                            ProjectsDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Visual Studio 2008\\Projects\\";
                            if (Directory.Exists(ProjectsDir))
                                vsVersion = VisualStudio.vs2008;
                            else
                                throw new Exception("Can not find the projects directory.");
                        }
                    }
                }

                switch (vsVersion)
                {
                    case VisualStudio.vs2010:
                        WriteLog("\n    Visual Studio 2010 detected!"); break;
                    case VisualStudio.vs2008:
                        WriteLog("\n    Visual Studio 2008 detected!"); break;
                    case VisualStudio.vs2012:
                        WriteLog("\n    Visual Studio 2012 detected!"); break;
                    case VisualStudio.vs2013:
                        WriteLog("\n    Visual Studio 2013 detected!"); break;
                }


                bool kinect = checkBox1.Checked;
                bool install = true;
                DialogResult r;
                if (Directory.Exists(ProjectsDir + textBox2.Text))
                {
                    r = MessageBox.Show("The project name already exists!\nThis will install GraphDLL on it.\nConfirm?", "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if (r == DialogResult.OK)
                        install = false;
                    else
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
                string IconFile = csprojinfo.Directory + "\\GraphIcon.ico";
                string GraphDLLFile = exeinfo.Directory + "\\GraphDLL.dll";
                string WMPLibFile = exeinfo.Directory + "\\Interop.WMPLib.dll";
                string MIDIFile = exeinfo.Directory + "\\Toub.Sound.Midi.dll";
                string GraphKinectFile = exeinfo.Directory + "\\GraphKinect.dll";
                string GraphDLLVersion = FileVersionInfo.GetVersionInfo(GraphDLLFile).FileVersion;
                string GraphKinectDLLVersion = FileVersionInfo.GetVersionInfo(GraphKinectFile).FileVersion;
                if (!install)
                    WriteLog("[Done]\n");
                WriteLog("Opening csproj file....");
                reader = new StreamReader(csprojFile);
                writer = new StreamWriter("temp.txt");
                WriteLog("[Done]\n");

                #region Add icon
                WriteLog("Checking the properties....\n");
                string lines;
                bool Icon = false;
                while (!reader.EndOfStream)
                {
                    lines = reader.ReadLine();
                    if (lines == "    <Optimize>false</Optimize>")
                    {
                        writer.WriteLine("    <Optimize>true</Optimize>");
                        WriteLog("    Optimize activated!\n");
                    }
                    else if (lines == "  <PropertyGroup>")
                    {
                        writer.WriteLine(lines);
                        lines = reader.ReadLine();
                        if (lines == "    <ApplicationIcon>GraphIcon.ico</ApplicationIcon>")
                        {
                            Icon = true;
                            WriteLog("    Icon found!\n");
                        }
                        writer.WriteLine(lines);
                    }
                    else if (lines == "  <ItemGroup>")
                    {
                        if (!Icon)
                        {
                            writer.WriteLine("  <PropertyGroup>");
                            writer.WriteLine("    <ApplicationIcon>GraphIcon.ico</ApplicationIcon>");
                            writer.WriteLine("  </PropertyGroup>");
                            WriteLog("    GraphIcon added!\n");
                        }
                        writer.WriteLine(lines);
                        break;
                    }
                    else
                        writer.WriteLine(lines);
                }
                if (reader.EndOfStream) throw new FileLoadException("The file is illegual for the installation.");
                #endregion

                #region Add references
                WriteLog("Checking the references....\n");
                bool WinForm = false, GraphDLL = false, GKinect = false, SysDraw = false;
                while (!reader.EndOfStream)
                {
                    lines = reader.ReadLine();
                    if (lines == "    <Reference Include=" + '"' + "System.Windows.Forms" + '"' + " />")
                    {
                        WinForm = true;
                        WriteLog("    System.Windows.Forms found!\n");
                    }
                    else if (lines == "    <Reference Include=" + '"' + "System.Drawing" + '"' + " />")
                    {
                        SysDraw = true;
                        WriteLog("    System.Drawing found!\n");
                    } if (lines == "    <Reference Include=" + '"' + "GraphDLL" + '"' + ">")
                    {
                        GraphDLL = true;
                        WriteLog("    GraphDLL found!\n");
                    }
                    else if (lines == "    <Reference Include=" + '"' + "GraphKinect" + '"' + ">")
                    {
                        GKinect = true;
                        WriteLog("    GraphKinect found!\n");
                        if (!kinect)
                        {
                            lines = reader.ReadLine();
                            lines = reader.ReadLine();
                            lines = reader.ReadLine();
                            WriteLog("    GraphKinect deleted!\n");
                            continue;
                        }
                    }


                    else if (lines == "  </ItemGroup>")
                        break;

                    writer.WriteLine(lines);
                }
                if (reader.EndOfStream) throw new FileLoadException("The file is illegual for the installation.");

                if (!GraphDLL)
                {
                    writer.WriteLine("    <Reference Include=" + '"' + "GraphDLL" + '"' + ">");
                    writer.WriteLine("      <SpecificVersion>False</SpecificVersion>");
                    writer.WriteLine("      <HintPath>" + GraphDLLFile + "</HintPath>");
                    writer.WriteLine("    </Reference>");
                    WriteLog("    GraphDLL added!\n");
                }
                if (!GKinect && kinect)
                {
                    writer.WriteLine("    <Reference Include=" + '"' + "GraphKinect" + '"' + ">");
                    writer.WriteLine("      <SpecificVersion>False</SpecificVersion>");
                    writer.WriteLine("      <HintPath>" + GraphKinectFile + "</HintPath>");
                    writer.WriteLine("    </Reference>");
                    WriteLog("    GraphKinect added!\n");
                }
                if (!SysDraw)
                {
                    writer.WriteLine("    <Reference Include=" + '"' + "System.Drawing" + '"' + " />");
                    WriteLog("    System.Drawing added!\n");
                }
                if (!WinForm)
                {
                    writer.WriteLine("    <Reference Include=" + '"' + "System.Windows.Forms" + '"' + " />");
                    WriteLog("    System.Windows.Forms added!\n");
                }
                writer.WriteLine("  </ItemGroup>");

                if (!WinForm || !GraphDLL || !Icon || GKinect != kinect)
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
                #endregion

                #region Add using
                WriteLog("Opening Program.cs file....");
                reader = new StreamReader(ProgramFile);
                writer = new StreamWriter("temp.txt");
                WriteLog("[Done]\n");
                WriteLog("Adding using....");
                writer.WriteLine("using GraphDLL;");
                writer.WriteLine("using System.Windows.Forms;");
                if (kinect)
                    writer.WriteLine("using GraphKinectDLL;");
                while (!reader.EndOfStream)
                {
                    lines = reader.ReadLine();
                    if (lines != "using GraphDLL;"
                            && lines != "using System.Drawing;"
                            && lines != "using System.Windows.Forms;"
                            && lines != "using GraphKinectDLL;")
                        writer.WriteLine(lines);
                }
                reader.Close();
                writer.Close();
                File.Copy(ProgramFile, ProgramFile + ".bak", true);
                File.Copy("temp.txt", ProgramFile, true);
                File.Delete("temp.txt");
                WriteLog("[Done]\n");
                #endregion

                #region Copy DLL & Ico
                WriteLog("Copying GraphDLL.dll (v" + GraphDLLVersion + ")...");
                Directory.CreateDirectory(csprojinfo.Directory + "\\bin\\Debug");
                File.Copy(GraphDLLFile, csprojinfo.Directory + "\\bin\\Debug\\GraphDLL.dll", true);
                WriteLog("[Done]\n");

                if (kinect)
                {
                    WriteLog("Copying GraphKinect.dll (v" + GraphKinectDLLVersion + ")...");
                    File.Copy(WMPLibFile, csprojinfo.Directory + "\\bin\\Debug\\GraphKinect.dll", true);
                    WriteLog("[Done]\n");
                }

                //WriteLog("Copying Interop.WMPLib.dll...");
                //File.Copy(WMPLibFile, csprojinfo.Directory + "\\bin\\Debug\\Interop.WMPLib.dll", true);
                //WriteLog("[Done]\n");

                WriteLog("Copying Toub.Sound.Midi.dll...");
                File.Copy(MIDIFile, csprojinfo.Directory + "\\bin\\Debug\\Toub.Sound.Midi.dll", true);
                WriteLog("[Done]\n");

                WriteLog("Copying GraphIcon.ico...");
                FileStream ico = new FileStream(IconFile, FileMode.Create);
                Properties.Resources.Project1.Save(ico);
                ico.Close();
                WriteLog("[Done]\n");
                #endregion

                WriteLog("\n[Installation Successfull]");
                System.Threading.Thread.Sleep(1000);

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
            if (name.Contains(" "))
                throw new Exception("The project name can not contain space.");
            if (name[0] >= '0' && name[0] <= '9')
                throw new Exception("The project name can start with a number.");

            switch (vsVersion)
            {
                case VisualStudio.vs2008:
                    WriteLog("[Done]\nCreating a new project...");
                    string file = Path + name + ".sln";
                    Directory.CreateDirectory(Path);
                    BinaryWriter binwriter = new BinaryWriter(new FileStream(file, FileMode.Create));
                    binwriter.Write(Properties.Resources.sln2008);
                    binwriter.Close();
                    ReplaceInFile("ConsoleApplication5", name, file);
                    WriteLog("\n    " + name + ".sln installed!");


                    Directory.CreateDirectory(Path + name);
                    file = Path + name + "\\" + name + ".csproj";
                    binwriter = new BinaryWriter(new FileStream(file, FileMode.Create));
                    binwriter.Write(Properties.Resources.csproj2008);
                    binwriter.Close();
                    ReplaceInFile("ConsoleApplication5", name, file);
                    WriteLog("\n    " + name + ".csproj installed!");

                    file = Path + name + "\\Program.cs";
                    binwriter = new BinaryWriter(new FileStream(file, FileMode.Create));
                    binwriter.Write(Properties.Resources.Program2008);
                    binwriter.Close();
                    ReplaceInFile("ConsoleApplication5", name, file);
                    WriteLog("\n    Program.cs installed!");

                    Directory.CreateDirectory(Path + name + "\\Properties");
                    file = Path + name + "\\Properties\\AssemblyInfo.cs";
                    binwriter = new BinaryWriter(new FileStream(file, FileMode.Create));
                    binwriter.Write(Properties.Resources.AssemblyInfo2008);
                    binwriter.Close();
                    ReplaceInFile("ConsoleApplication5", name, file);
                    WriteLog("\n    AssemblyInfo.cs installed!\n");
                    break;
                case VisualStudio.vs2010:
                case VisualStudio.vs2012:
                case VisualStudio.vs2013:
                    WriteLog("[Done]\nCreating a new project...");
                    file = Path + name + ".sln";
                    Directory.CreateDirectory(Path);
                    binwriter = new BinaryWriter(new FileStream(file, FileMode.Create));
                    binwriter.Write(Properties.Resources.sln2010);
                    binwriter.Close();
                    ReplaceInFile("ConsoleApplication6", name, file);
                    WriteLog("\n    " + name + ".sln installed!");


                    Directory.CreateDirectory(Path + name);
                    file = Path + name + "\\" + name + ".csproj";
                    binwriter = new BinaryWriter(new FileStream(file, FileMode.Create));
                    binwriter.Write(Properties.Resources.csproj2010);
                    binwriter.Close();
                    ReplaceInFile("ConsoleApplication6", name, file);
                    WriteLog("\n    " + name + ".csproj installed!");

                    file = Path + name + "\\Program.cs";
                    binwriter = new BinaryWriter(new FileStream(file, FileMode.Create));
                    binwriter.Write(Properties.Resources.Program2010);
                    binwriter.Close();
                    ReplaceInFile("ConsoleApplication6", name, file);
                    WriteLog("\n    Program.cs installed!");

                    Directory.CreateDirectory(Path + name + "\\Properties");
                    file = Path + name + "\\Properties\\AssemblyInfo.cs";
                    binwriter = new BinaryWriter(new FileStream(file, FileMode.Create));
                    binwriter.Write(Properties.Resources.AssemblyInfo2010);
                    binwriter.Close();
                    ReplaceInFile("ConsoleApplication6", name, file);
                    WriteLog("\n    AssemblyInfo.cs installed!\n");
                    break;
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

        private void textBox2_Enter(object sender, EventArgs e)
        {
            if (textBox2.Text == "<Project Name>")
                textBox2.Text = "";
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
                textBox2.Text = "<Project Name>";
        }
    }
}
