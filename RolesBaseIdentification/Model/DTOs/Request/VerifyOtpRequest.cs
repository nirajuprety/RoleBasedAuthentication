namespace RolesBaseIdentification.Model.DTOs.Request
{
    public class VerifyOtpRequest
    {
        public string Email { get; set; }
        public string OtpCode { get; set; }
        public bool IsEmailedOtp { get; set; }
    }
}
