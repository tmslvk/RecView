namespace webapi.Model
{
    public interface ILikeable
    {
        Task Like(int objectId, int userId);
        Task Dislike(int objectId, int userId);
    }
}
