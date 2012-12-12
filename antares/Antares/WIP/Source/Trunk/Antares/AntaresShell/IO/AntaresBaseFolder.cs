using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;

namespace AntaresShell.IO
{
    public class AntaresBaseFolder
    {
        public async Task InitializeBaseFolder()
        {
            try
            {
                RoamingFolder = ApplicationData.Current.LocalFolder;
                InstalledFolder = Package.Current.InstalledLocation;

                TemporaryFolder =
                    await
                    ApplicationData.Current.LocalFolder.CreateFolderAsync("temp", CreationCollisionOption.OpenIfExists);
            }
            catch{}
        }

        public StorageFolder RoamingFolder { get; private set; }

        public StorageFolder InstalledFolder { get; private set; }

        public StorageFolder TemporaryFolder { get; private set; }

        public static AntaresBaseFolder Instance
        {
            get
            {
                return Nested._instance;
            }
        }

        private class Nested
        {
            internal static readonly AntaresBaseFolder _instance = new AntaresBaseFolder();
        }


    }
}
