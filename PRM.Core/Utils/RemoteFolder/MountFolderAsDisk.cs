using System.Runtime.InteropServices;
using System.Text;

namespace PRM.Core.Utils.RemoteFolder
{
    static class MountFolderAsDisk
    {
        public static bool MountDiskFromFolder(char diskLetter, string folderPath)
        {
            return DefineDosDevice(0, diskLetter.ToString() + ":", folderPath);
        }

        public static bool UnMountDisk(char diskLetter)
        {
            return DefineDosDevice(2, diskLetter.ToString() + ":", null);
        }

        #region extenal implementations

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool DefineDosDevice(int flags, string devname, string path);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int QueryDosDevice(string devname, StringBuilder buffer, int bufSize);

        #endregion
    }
}
