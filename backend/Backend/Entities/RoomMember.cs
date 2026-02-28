using Backend.Entities.Enums;

namespace Backend.Entities
{
    public class RoomMember
    {
        public Guid RoomId { get; set; }
        public Room Room { get; set; } = null!;

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public RoomRole Role { get; set; }
    }
}
