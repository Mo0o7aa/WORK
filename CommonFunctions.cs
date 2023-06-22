using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ntra_missions
{
    public class CommonFunctions
    {

        public CommonFunctions()
        {

        }

        public FileInfo[] ListFilesInFolder(String path)
        {
            
            DirectoryInfo directory = new DirectoryInfo(path);

            FileInfo[] filesList = directory.GetFiles();

            return filesList;
        }

        public bool CheckDirectory(String path)
        {
            if (!Directory.Exists(path))
            {
                return false;
            }
            else
                return true;
        }

        public bool CheckFile(String path)
        {
            if (!File.Exists(path))
            {
                return false;
            }
            else
                return true;
        }

        public void CreateDirectory(String path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

    }
}
