﻿namespace ModsenLibGateWay.Models
{
    /// <summary>
    /// CustomerRequestModel Register model
    /// </summary>
    public class CustomerRegRequestModel
    {
        /// <summary>
        /// string? Email 
        /// </summary>
        public string? Email { get; set; }
        /// <summary>
        /// string? Keyword
        /// </summary>
        /// 
        public string? Keyword { get; set; }
        /// <summary>
        /// string? Password
        /// </summary>
        /// 
        public string? Password { get; set; }
        /// <summary>
        /// string? PhoneNumber 
        /// </summary>
        public string? PhoneNumber { get; set; }
        /// <summary>
        /// string? Name
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// string? Surname
        /// </summary>
        public string? Surname { get; set; }
        /// <summary>
        /// string? MidName
        /// </summary>
        public string? MidName { get; set; }
        /// <summary>
        /// DateTime? BirthDate
        /// </summary>
        public DateTime? BirthDate { get; set; }
        /// <summary>
        /// string? Gender 
        /// </summary>
        public string? Gender { get; set; }
        /// <summary>
        /// string? Address
        /// </summary>
        public string? Address { get; set; }
    }
}
