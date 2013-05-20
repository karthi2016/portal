using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for OrderConfirmationPacket
/// </summary>
public abstract class OrderConfirmationPacket
{

    /// <summary>
    /// Determines whether or not the confirmation section should even be shown
    /// </summary>
    /// <returns></returns>
    public abstract bool ShouldShow();
}