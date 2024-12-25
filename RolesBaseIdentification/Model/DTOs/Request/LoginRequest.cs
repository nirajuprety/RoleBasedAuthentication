﻿namespace RolesBaseIdentification.Model.DTOs.Request
{
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string OtpCode{ get; set; }
        public bool IsEmailedOtp { get; set; }
    }
}
