using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Container for information required to create a new user for use during the create account multi-step wizard
/// </summary>
[Serializable]
public class NewUserRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public string PostalCode { get; set; }
    public string Name { get; set; }
}