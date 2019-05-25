namespace DropPlus.DAL.Interfaces
{
    public interface IUnitOfWork
    {
        void Save();

        void Dispose(bool disposing);
        void Dispose();
    }
}