using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using static Crayon.Output;

namespace ArchiveTorrents
{
    class Program
    {
        private const String TORR_EXT_WILDCARD = "*.torrent";
        private const String TORR_ARCHIVE_DIR = @"x:\torr_archived\";
        private const String TORR_INCOMING_DIR = @"x:\torr_incoming\";

        private static String TORR_INPUT_DIR = Environment.GetFolderPath (Environment.SpecialFolder.UserProfile) + @"\Downloads";

        static void Main (String[] args)
        {
            Console.WriteLine ($"Processing Input Directory [{ Green (TORR_INPUT_DIR)}], looking for duplicates..");

            // find files that have been downloaded multiple times, and delete them like "abc (1).torrent"
            var dupTorrFiles = Directory.GetFiles (TORR_INPUT_DIR, "* (?).torrent");

            for (var i = 0; i < dupTorrFiles.Length; i++) {
                var torrFile = new FileInfo (dupTorrFiles[i]);

                Console.WriteLine ($"Found multiple-download file {Red (torrFile.Name)}");

                File.Delete (torrFile.FullName);
            }

            Console.WriteLine ();
            Console.WriteLine ($"Processing Input Directory [{ Green (TORR_INPUT_DIR)}], processing torrent files..");

            // find actual torrent files
            var torrFiles = Directory.GetFiles (TORR_INPUT_DIR, TORR_EXT_WILDCARD);

            for (var i = 0; i < torrFiles.Length; i++) {
                var torrFile = new FileInfo (torrFiles[i]);
                var normalizedName = Path.GetFileNameWithoutExtension (normalizeFileName (torrFile.Name));

                Console.WriteLine ($"Found file        [{ Magenta (torrFile.Name) }], normalized [{ normalizedName }]");

                if (Directory.GetFiles (TORR_ARCHIVE_DIR, normalizedName + TORR_EXT_WILDCARD).Length > 0) {
                    Console.WriteLine ($"Duplicate found   [{ Red (torrFile.Name) }], removing..");
                } else {
                    Console.WriteLine ($"Archiving torrent [{ Green (torrFile.Name) }]");

                    // archive as copy
                    File.Copy (
                        torrFile.FullName,
                        TORR_ARCHIVE_DIR + torrFile.Name
                        );

                    // copy to incoming folder of torrent client to pick up
                    File.Copy (
                        torrFile.FullName,
                        TORR_INCOMING_DIR + torrFile.Name
                        );
                }

                // delete anyways
                File.Delete (torrFile.FullName);
            }

            Console.WriteLine ();
            Console.WriteLine ($"{Green ("It's all good man")}");
        }

        private static String normalizeFileName (String originalName)
        {
            return originalName
                .Replace ("-[rarbg.to]", "")
                .Replace ("[rarbg]", "");
        }
    }
}
