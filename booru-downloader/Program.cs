using System;
using System.Windows.Forms;

namespace booru_downloader {
    static class Program {

        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(
                new Dialog( new DisplaySource[] {
                    new DisplaySource("Danbooru", BooruAPI.Source.Danbooru),
                    new DisplaySource("Konachan", BooruAPI.Source.Konachan),
                    new DisplaySource("Yande.re", BooruAPI.Source.Yandere)
                } ));
        }
    }
}
