using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiveTorrents
{
    class ATConfig
    {
        // The timeout will be determined by TORRENT_PARALLEL_LIMIT * MAIN_LOOP_INTERVAL as torrents get removed on a FIFO logic basis

        public readonly String TORR_EXT_WILDCARD = "*.torrent";
        public readonly String TORR_ARCHIVE_DIR = @"x:\torr_archived\arc\";
        public readonly String TORR_ARCHIVE_DIR_OLD = @"x:\torr_archived\arc\old\";
        public readonly String TORR_ARCHIVE_REG = @"..\!!!!_torrs.lst";
        public readonly String TORR_ARCHIVE_FILES_REG = @"..\!!!!_torrs_files.lst";
        public readonly String TORR_INCOMING_DIR = @"x:\torr_incoming\";

        public readonly String TORR_INPUT_DIR = Environment.GetFolderPath (Environment.SpecialFolder.UserProfile) + @"\Downloads";

        public readonly string SDB_DLD_URL = ConfigurationManager.AppSettings["SDB_DLD_URL"];

    }
}
