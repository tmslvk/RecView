using Microsoft.EntityFrameworkCore;
using webapi.Interfaces;
using webapi.Model;

namespace webapi.Services
{
    public class LikeService : ILikeable
    {
        private ApplicationContext db;

        public LikeService(ApplicationContext context)
        {
            db = context;
        }

        public async Task Like(int objectId, int userId)
        {
            var like = new Like()
            {
                UserId = userId,
                ObjectId = objectId,
                IsLiked = true,
            };
            await SaveLike(like);
        }
        public async Task Dislike(int objectId, int userId)
        {
            var like = new Like()
            {
                UserId = userId,
                ObjectId = objectId,
                IsLiked = false,
            };
            await SaveLike(like);
        }

        private async Task SaveLike(Like like)
        {
            var existingLike = await db.Likes
                .FirstOrDefaultAsync(l => l.UserId == like.UserId && l.ObjectId == like.ObjectId);

            if (existingLike != null)
            {
                existingLike.IsLiked = like.IsLiked;
                db.Likes.Update(existingLike);
            }
            else
            {
                db.Likes.Add(like);
            }

            await db.SaveChangesAsync();

            var post = await db.Publications.FindAsync(like.ObjectId);
            if (post != null)
            {
                post.LikesCount = db.Likes.Count(l => l.ObjectId == like.ObjectId && l.IsLiked);
                db.Publications.Update(post);
                await db.SaveChangesAsync();
            }
        }
    } 
}
