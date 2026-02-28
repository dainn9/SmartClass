namespace Backend.Entities
{
    public class User : BaseEntity
    {
        public string GoogleId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public ICollection<RoomMember> RoomMembers { get; set; } = new List<RoomMember>();

    }
}
