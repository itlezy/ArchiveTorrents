using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

using MonoTorrent;

using static Crayon.Output;

namespace ArchiveTorrents
{
    class Program
    {
        private const String TORR_EXT_WILDCARD = "*.torrent";
        private const String TORR_ARCHIVE_DIR = @"x:\torr_archived\arc\";
        private const String TORR_ARCHIVE_DIR_OLD = TORR_ARCHIVE_DIR + @"old\";
        private const String TORR_ARCHIVE_REG = @"..\!!!!_torrs.lst";
        private const String TORR_ARCHIVE_FILES_REG = @"..\!!!!_torrs_files.lst";
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
            Console.WriteLine ();

            // find actual torrent files
            var torrFiles = Directory.GetFiles (TORR_INPUT_DIR, TORR_EXT_WILDCARD);

            for (var i = 0; i < torrFiles.Length; i++) {
                var torrFile = new FileInfo (torrFiles[i]);

                //Console.WriteLine ($"Found file        [{ Magenta (torrFile.Name) }]");

                var torrTorr = Torrent.Load (torrFile.FullName);
                var torrLargestFile = torrTorr.Files.OrderByDescending (t => t.Length).First ();
                var torrHashId = torrTorr.InfoHashes.V1OrV2.ToHex ().ToLower ();
                var normalizedName = Path.GetFileNameWithoutExtension (normalizeFileName (torrFile.Name));

                Console.WriteLine ($"Found file        [{ Magenta (torrFile.Name) }], hashId { Green (torrHashId) }");
                Console.WriteLine ($"           >      [{ Magenta (torrLargestFile.Path) }], size { Green (torrLargestFile.Length.ToString ()) } ");

                if (File.ReadLines (TORR_ARCHIVE_DIR + TORR_ARCHIVE_REG).Contains (torrHashId)) {
                    // remove duplicate if the same hashId was already in the list
                    Console.WriteLine ($"Duplicate found L [{ Red (torrFile.Name) }], removing..");
                } else if (File.ReadLines (TORR_ARCHIVE_DIR + TORR_ARCHIVE_FILES_REG).Contains (torrLargestFile.Path + "|" + torrLargestFile.Length)) {
                    // remove duplicate if the same file with the same exact length was already in the list
                    Console.WriteLine ($"Duplicate found R [{ Red (torrFile.Name) }], removing..");
                } else if (
                    Directory.GetFiles (TORR_ARCHIVE_DIR, normalizedName + TORR_EXT_WILDCARD).Length > 0 ||
                    Directory.GetFiles (TORR_ARCHIVE_DIR_OLD, normalizedName + TORR_EXT_WILDCARD).Length > 0
                    ) {
                    // remove duplicate if the same torrent file exists
                    Console.WriteLine ($"Duplicate found F [{ Red (torrFile.Name) }], removing..");
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

                    // add the hashId to the list, so to be sure we can detect duplicates even if the file-name differs
                    File.AppendAllLines (TORR_ARCHIVE_DIR + TORR_ARCHIVE_REG, new String[] { torrHashId });
                    // add the largest file name and size to the list, so to be sure we can detect duplicates even if the file-name differs
                    File.AppendAllLines (TORR_ARCHIVE_DIR + TORR_ARCHIVE_FILES_REG, new String[] { torrLargestFile.Path + "|" + torrLargestFile.Length });
                }

                // delete original file at the end
                File.Delete (torrFile.FullName);
                Console.WriteLine ();
            }

            Console.WriteLine ();
            Console.WriteLine ($"{Green ("It's all good man")}");

            //Thread.Sleep (4000);
            Console.ReadLine ();
        }

        private static String normalizeFileName (String originalName)
        {
            return originalName
                .Replace ("-[rarbg.to]", "")
                .Replace ("[rarbg]", "");
        }
    }
}
