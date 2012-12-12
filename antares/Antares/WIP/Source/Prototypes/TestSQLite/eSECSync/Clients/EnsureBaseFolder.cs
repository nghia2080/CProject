using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography.DataProtection;
using Windows.Storage;

namespace eSECSync.Clients
{
    public class EnsureBaseFolder
    {
        public static EnsureBaseFolder Instance
        {
            get
            {
                return Nested._instance;
            }
        }

        private class Nested
        {
            internal static readonly EnsureBaseFolder _instance = new EnsureBaseFolder();
        }


        public StorageFolder PersonalFolder { get; set; }
        public StorageFolder UsersFolder { get; set; }

        public async Task InitializeFolder(string userID)
        {
            UsersFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Users", CreationCollisionOption.OpenIfExists);
            PersonalFolder = await UsersFolder.CreateFolderAsync(userID, CreationCollisionOption.OpenIfExists);
        }
    }
}
