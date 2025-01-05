namespace Api_Tutorial.Dtos
{
    partial class LoginConfirmationDto
    {
        byte[] PasswordHash { get; set; } = [0];
        byte[] PasswordSalt { get; set; } = [0];
    }
}
