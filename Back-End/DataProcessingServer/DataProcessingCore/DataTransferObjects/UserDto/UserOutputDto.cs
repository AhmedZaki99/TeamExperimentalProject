namespace DataProcessingCore
{
    public class UserOutputDto : OutputDtoBase
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public DateTime? BirthDate { get; set; }
    }
}
