namespace API.Entities
{
    public class UserLike
    {
        public AppUser SourceUser { get; set; }
        public int SourceUserID { get; set; }
       // public int SourceUserId { get; internal set; }
        public AppUser LikedUser { get; set; }
        public int LikedUserId { get; set; }
    }
}
