namespace Backend.Entities
{
    public class Room : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public ICollection<RoomMember> RoomMembers { get; set; } = new List<RoomMember>();
    }
}
