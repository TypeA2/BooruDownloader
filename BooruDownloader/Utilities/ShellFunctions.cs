using System;
using System.Runtime.InteropServices;

namespace BooruDownloader {
    

    public static class ShellFunctions {
        public enum UserFolder {
            Contacts,
            Desktop,
            Documents,
            Downloads,
            Favorites,
            Links,
            Music,
            Pictures,
            SavedGames,
            SavedSearches,
            Videos
        }

        private static readonly string[] _user_folder_guids =  {
            "{56784854-C6CB-462B-8169-88E350ACB882}", // Contacts
            "{B4BFCC3A-DB2C-424C-B029-7FE99A87C641}", // Desktop
            "{FDD39AD0-238F-46AF-ADB4-6C85480369C7}", // Documents
            "{374DE290-123F-4565-9164-39C4925E467B}", // Downloads
            "{1777F761-68AD-4D8A-87BD-30B759FA33DD}", // Favorites
            "{BFB9D5E0-C6A9-404C-B2B2-AE6DB6AF4968}", // Links
            "{4BD8D571-6D19-48D3-BE97-422220080E43}", // Music
            "{33E28130-4E1E-4676-835A-98395C3BC3BB}", // Pictures
            "{4C5C32FF-BB9D-43B0-B5B4-2D72E54EAAA4}", // SavedGames
            "{7D1D3A04-DEBB-4115-95CF-2F29DA2920DA}", // SavedSearches
            "{18989B1D-99B5-455B-841C-AB7C74E4DDFC}", // Videos
        };

        public static string GetUserFolder(UserFolder folder) {
            int result = SHGetKnownFolderPath(new Guid(_user_folder_guids[(int) folder]), 0, new IntPtr(0),
                out IntPtr out_path);

            if (result >= 0) {
                string path = Marshal.PtrToStringUni(out_path);
                Marshal.FreeCoTaskMem(out_path);
                return path;
            }

            return null;
        }

        [DllImport("shell32.dll")]
        private static extern int SHGetKnownFolderPath(
            [MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags, IntPtr hToken, out IntPtr ppszPath);

    }
}
