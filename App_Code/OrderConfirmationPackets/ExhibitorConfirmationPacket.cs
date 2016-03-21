using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ExhibitorConfirmationPacket
/// </summary>
public class ExhibitorConfirmationPacket : OrderConfirmationPacket
{
	public ExhibitorConfirmationPacket()
	{
		 
	}

    public override bool ShouldShow()
    {
        if (BoothPreferences != null && BoothPreferences.Count > 0)
            return true;

        if (!string.IsNullOrWhiteSpace(SpecialRequests))
            return true;

        return false;
    }

    public List<string> BoothPreferences { get; set; }

    public string SpecialRequests { get; set; }

    public string ConfirmationInstructions { get; set; }
}