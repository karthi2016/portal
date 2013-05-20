using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Types;

/// <summary>
/// Summary description for Utils
/// </summary>
public static class Utils
{
    /// <summary>
    /// Sets the list item to the selected value if the value exists
    /// </summary>
    /// <param name="lc">The lc.</param>
    /// <param name="valueToSet">The value to set.</param>
    /// <remarks>I created this b/c if you try to set the selected value of a list control and
    /// that doesn't exist, an exception is throw - we want an option to be more graceful. For instance,
    /// if a chapter no longer exists, we don't want the renewal process to error out.</remarks>
	 public static void SafeSetSelectedValue( this ListControl lc, string valueToSet )
     {
         if (lc == null) throw new ArgumentNullException("lc");

         var li = lc.Items.FindByValue(valueToSet);
         if (li != null)
         {
             lc.ClearSelection();
             li.Selected = true;
         }

     }

     public static msEntityAddress GetEntityPreferredAddress(msEntity e)
     {
         if (e.Addresses == null || e.Addresses.Count == 0)
             return null;

         // see if we can get the default
         var defaultAddress = e.Addresses.ToList().Find(x => x.Type == e.PreferredAddressType);

         if (defaultAddress == null)   // no default, let's use the first
             defaultAddress = e.Addresses[0];

         if (defaultAddress.Address == null) return null; // shouldn't happen, but this is defensive programming
         return defaultAddress;
     }
}