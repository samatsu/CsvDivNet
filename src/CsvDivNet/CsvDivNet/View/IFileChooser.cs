using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsvDivNet.View
{
    interface IFileChooser
    {
        string SelectFile();

        string SelectDirectory();
    }
}
