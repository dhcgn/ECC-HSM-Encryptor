namespace Storage
{
    public interface IStorageProvider
    {
        DiskStorage Load(string storagePath, string password);

        void Save(DiskStorage diskStorage, string storagePath, string password);
    }
}