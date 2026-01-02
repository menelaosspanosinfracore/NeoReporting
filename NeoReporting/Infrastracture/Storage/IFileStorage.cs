namespace ReportGenerator.Infrastructure.Storage
{
    public interface IFileStorage
    {
        void Save(string path, byte[] content);
    }
}
