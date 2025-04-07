﻿using Microsoft.AspNetCore.Identity;

namespace CarRentXpress.Data.Entities;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}