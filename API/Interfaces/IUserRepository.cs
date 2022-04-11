namespace API.Interfaces
{
    public interface IUserRepository
    {
        //void Update(AppUser user);
        Task<bool> SaveAllAsync();
       // Task<IEn>
    }
}
