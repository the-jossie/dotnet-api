namespace Api_Tutorial.Dtos
{
    public partial class LoginConfirmationDto
    {
        public byte[] PasswordHash { get; set; } = [0];
        public byte[] PasswordSalt { get; set; } = [0];
    }
}
