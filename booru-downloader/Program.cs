using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace booru_downloader {
    static class Program {

        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(
                new Dialog( new string [] { "All", "danbooru", "konachan", "yande.re" } )
            );
        }
    }
}
