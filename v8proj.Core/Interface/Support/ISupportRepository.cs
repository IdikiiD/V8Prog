using v8proj.Core.Entities;

namespace v8proj.Core.Interface.Support
{
    public interface ISupportRepository
    {
        void Add(Entities.Support support);
        void SaveChanges();
    }
}