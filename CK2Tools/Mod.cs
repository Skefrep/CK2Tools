using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace CK2Tools
{
    public class Mod
    {
        public Mod() { }

        ~Mod() { }

        public void DecodeFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Mod file does not exist.", filePath);
            }

            var stream = new StreamReader(filePath);
            string line;

            while ((line = stream.ReadLine()) != null)
            {
                line = line.Split('#')[0]; // Ignore comments

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                line = line.Trim();

                var values = line.Split('=');
                
                switch (values[0].ToLower())
                {
                    case "name":
                        Name = values[1].Trim().Trim('"');
                        break;

                    case "path":
                        Path = values[1].Trim().Trim('"');
                        break;

                    case "user_dir":
                        UserDirectory = values[1].Trim().Trim('"');
                        break;

                    case "replace_path":
                        ReplacePath.Add(values[1].Trim().Trim('"'));
                        break;

                    default:
                        throw new Exception("Unknown keyword in mod file: " + values[0]);
                }
            }
            stream.Close();

            ((MainWindow)(Application.Current.MainWindow)).FillFields();
        }

        public void WriteFile()
        {
            FileInfo fi = null;
            try
            {
                fi = new System.IO.FileInfo(ModFile);
            }
            catch (ArgumentException) { }
            catch (PathTooLongException) { }
            catch (NotSupportedException) { }

            if (ReferenceEquals(fi, null))
                throw new Exception("No mod file has been defined.");

            if (string.IsNullOrEmpty(Name))
                throw new Exception("The mod must have a name.");

            if (string.IsNullOrEmpty(Path))
                throw new Exception("The mod must have a path.");

            if (new FileInfo(ModFile).Length == 0)
                writeNewFile();
            else
                writeExistingFile();
                
        }

        private void writeNewFile()
        {
            var stream = new StreamWriter(ModFile);
            // Name
            stream.WriteLine("\tname=\"" + Name + "\"");
            stream.WriteLine("\tpath=\"" + Path + "\"");

            if (!string.IsNullOrEmpty(UserDirectory))
                stream.WriteLine("\tuser_dir=\"" + UserDirectory + "\"");

            if (ReplacePath != null)
            { 
                foreach (var rpath in ReplacePath)
                {
                    if (!string.IsNullOrEmpty(rpath))
                        stream.WriteLine("\treplace_path=\"" + rpath + "\"");
                }
            }
            stream.Close();
        }

        private void writeExistingFile()
        {
            File.Copy(ModFile, ModFile + ".bkp");
            var reader = new StreamReader(ModFile + ".bkp");
            var writer = new StreamWriter(ModFile);

            bool bNameWritten = false;
            bool bPathWritten = false;
            bool bUserDirWritten = false;

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var rgx = new Regex("^([^#]*\\s*[nN][aA][mM][eE]\\s*=\\s*\")(.*)(\".*)$");
                if (rgx.IsMatch(line))
                {
                    if (bNameWritten)
                        throw new Exception("mod file mismatch: two names");
                    
                    line = Regex.Replace(line, rgx.ToString(), "$1" + Name + "$3");
                    bNameWritten = true;
                }

                rgx = new Regex("^([^#]*\\s*[pP][aA][tT][hH]\\s*=\\s*\")(.*)(\".*)$");
                if (rgx.IsMatch(line))
                {
                    if (bPathWritten)
                        throw new Exception("mod file mismatch: two paths");

                    line = Regex.Replace(line, rgx.ToString(), "$1" + Path + "$3");
                    bPathWritten = true;
                }

                rgx = new Regex("^([^#]*\\s*[uU][sS][eE][rR]_[dD][iI][rR]\\s*=\\s*\")(.*)(\".*)$");
                if (rgx.IsMatch(line))
                {
                    if (bUserDirWritten)
                        throw new Exception("mod file mismatch: two user dirs");

                    line = Regex.Replace(line, rgx.ToString(), "$1" + UserDirectory + "$3");
                    bUserDirWritten = true;
                }

                writer.WriteLine(line);
            }

            if (!bNameWritten)
                writer.WriteLine("\tname=\"" + Name + "\"");

            if (!bPathWritten)
                writer.WriteLine("\tname=\"" + Path + "\"");

            if (!bUserDirWritten)
                writer.WriteLine("\tname=\"" + UserDirectory + "\"");
        }

        public string ModFile { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string UserDirectory { get; set; }
        public List<string> ReplacePath { get; set; }
    }
}
