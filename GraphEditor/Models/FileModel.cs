using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace GraphEditor.Models
{
    class FileModel : MainWindow
    {
        private const string error = "error";
        private string fileName;

        SaveFileDialog saveFileDialog;
        OpenFileDialog openFileDialog;
        public FileModel()
        {
        }


        public string Load()
        {
            fileName = ChooseFolderToLoad();
            return fileName;
        }
        public void XmlSerialize(Type dataType, object data, string filePath)
        {
            XmlSerializer xmlSerialize = new XmlSerializer(dataType);
            if (File.Exists(filePath)) File.Delete(filePath);
            TextWriter writer = new StreamWriter(filePath);
            xmlSerialize.Serialize(writer, data);
            writer.Close();
        }

        public object XmlDeserialize(Type dataType, string filePath)
        {
            object obj = null;

            XmlSerializer xmlSerialize = new XmlSerializer(dataType);
            if (File.Exists(filePath))
            {
                TextReader textReader = new StreamReader(filePath);
                obj = xmlSerialize.Deserialize(textReader);
                textReader.Close();
            }


            return obj;
        }
        public string ChooseFolderToSave(Canvas myCanvas)
        {
            saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = ".text";
            saveFileDialog.Filter = "Text documents (.txt)|*.txt";

            if (saveFileDialog.ShowDialog() == true)
            {
                fileName = saveFileDialog.FileName;
            }
            else
                fileName = error;

            return fileName;
        }

        public string ChooseFolderToLoad()
        {
            openFileDialog = new OpenFileDialog();

            openFileDialog.DefaultExt = ".text";
            openFileDialog.Filter = "Text documents (.txt)|*.txt";

            if (openFileDialog.ShowDialog() == true)
            {
                fileName = openFileDialog.FileName;
            }
            else
                fileName = error;

            return fileName;
        }
    }
}
