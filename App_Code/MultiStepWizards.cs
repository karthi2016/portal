using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using MemberSuite.SDK.Manifests;
using MemberSuite.SDK.Manifests.Searching;
using MemberSuite.SDK.Searching;
using MemberSuite.SDK.Types;

/// <summary>
/// This is a reference class for multi-steps wizards throughout the portal.
/// They can use this class to store transient values that need to be held.
/// </summary>
public static class MultiStepWizards
{
    public static class MakePayment
    {
        public static void Clear()
        {
            Payment = null;
        }

        public static msPayment Payment
        {
            get { return SessionManager.Get<msPayment>("MemberSuite:MakePayment.Payment"); }
            set { SessionManager.Set("MemberSuite:MakePayment.Payment", value); }
        }

       
    }


    public static class RenewMembership 
    {
        public static msMembership Membership
        {
            get { return SessionManager.Get<msMembership>("MemberSuite:RenewMembership.Membership"); }
            set { SessionManager.Set("MemberSuite:RenewMembership.Membership",value); }
        }

        public static msEntity Entity
        {
            get { return SessionManager.Get<msEntity>("MemberSuite:RenewMembership.Entity"); }
            set { SessionManager.Set("MemberSuite:RenewMembership.Entity",value); }   
        }

        public static void Clear()
        {
            MultiStepWizards.RenewMembership.Membership = null;
            Entity = null;
        }
    }

    public static class CreateAccount
    {
        public static void Clear()
        {
            Request = null;
            TargetIndividual = null;
            TargetPortalUser = null;
            TargetOrganization = null;
            TargetOrganizationRelationship = null;
            CompleteUrl = null;
        }
        public static NewUserRequest Request
        {
            get { return SessionManager.Get<NewUserRequest>("MemberSuite:CreateAccount.Request"); }
            set { SessionManager.Set("MemberSuite:CreateAccount.Request",value); }
        }

        public static msIndividual TargetIndividual
        {
            get { return SessionManager.Get<msIndividual>("MemberSuite:CreateAccount.TargetIndividual"); }
            set { SessionManager.Set("MemberSuite:CreateAccount.TargetIndividual",value); }
        }

        public static msPortalUser TargetPortalUser
        {
            get { return SessionManager.Get<msPortalUser>("MemberSuite:CreateAccount.TargetPortalUser"); }
            set { SessionManager.Set("MemberSuite:CreateAccount.TargetPortalUser",value); }
        }

        public static msOrganization TargetOrganization
        {
            get { return SessionManager.Get<msOrganization>("MemberSuite:CreateAccount.TargetOrganization"); }
            set { SessionManager.Set("MemberSuite:CreateAccount.TargetOrganization",value); }
        }

        public static msRelationship TargetOrganizationRelationship
        {
            get { return SessionManager.Get<msRelationship>("MemberSuite:CreateAccount.TargetRelationship"); }
            set { SessionManager.Set("MemberSuite:CreateAccount.TargetRelationship",value); }
        }

        public static string CompleteUrl
        {
            get { return SessionManager.Get<string>("MemberSuite:CreateAccount.CompleteUrl"); }
            set { SessionManager.Set("MemberSuite:CreateAccount.CompleteUrl",value); }
        }

        public static bool InitiatedByLeader
        {
            get
            {
                return SessionManager.Get<bool>("MemberSuite:CreateAccount.InitiatedByLeader");
                    
            }
            set { SessionManager.Set("MemberSuite:CreateAccount.InitiatedByLeader",value); }
        }
    }

    public static class RegisterForEvent
    {

        public static void Clear()
        {
            Group = null;
            Order = null;
            RegistrationFee = null;
            AdditionalLineItems = null;

        }

        public static msOrganization Group
        {
            get { return SessionManager.Get<msOrganization>("MemberSuite:RegisterForEvent.Group"); }
            set { SessionManager.Set("MemberSuite:RegisterForEvent.Group",value); }
        }
        public static msOrder Order
        {
            get { return SessionManager.Get<msOrder>("MemberSuite:RegisterForEvent.Order"); }
            set { SessionManager.Set("MemberSuite:RegisterForEvent.Order",value); }
        }

        public static msRegistrationFee RegistrationFee
        {
            get { return SessionManager.Get<msRegistrationFee>("MemberSuite:RegisterForEvent.RegistrationFee"); }
            set { SessionManager.Set("MemberSuite:RegisterForEvent.RegistrationFee",value); }
        }

        public static List<msOrderLineItem> AdditionalLineItems
        {
            get { return SessionManager.Get<List<msOrderLineItem>>("MemberSuite:RegisterForEvent.AdditionalLineItems"); }
            set { SessionManager.Set("MemberSuite:RegisterForEvent.AdditionalLineItems",value); }
        }

    }

    public static class GroupRegistration
    {
        public static void Clear()
        {
            Order = null;
            Group = null;
            RegistrantID = null;
            Event = null;
            
        }

        public static msOrder Order
        {
            get { return SessionManager.Get<msOrder>("MemberSuite:GroupRegistration.Order"); }
            set { SessionManager.Set("MemberSuite:GroupRegistration.Order",value); }
        }

        public static msOrganization Group
        {
            get { return SessionManager.Get<msOrganization>("MemberSuite:GroupRegistration.Group"); }
            set { SessionManager.Set("MemberSuite:GroupRegistration.Group",value); }
        }

        public static string RegistrantID
        {
            get { return SessionManager.Get<string>("MemberSuite:GroupRegistration.RegistrantID"); }
            set { SessionManager.Set("MemberSuite:GroupRegistration.RegistrantID",value); }
        }

        public static msEvent Event
        {
            get { return SessionManager.Get<msEvent>("MemberSuite:GroupRegistration.Event"); }
            set { SessionManager.Set("MemberSuite:GroupRegistration.Event",value); }
        }

        public static void NavigateBackToGroupRegistrationIfApplicable( string eventID )
        {
            var o = Group;
            if (o == null || string.IsNullOrWhiteSpace( o.ID )) return;

            Order = null;
            Group = null;
            Event = null;
            HttpContext.Current.Response.Redirect(string.Format("ManageGroupRegistration.aspx?contextID={0}&organizationID={1}",
                eventID, o.ID));

        }
    }


    public static class EnterCompetition
    {
        public static void Clear()
        {
            EntryFee = null;
        }

        public static msCompetitionEntryFee EntryFee
        {
            get { return SessionManager.Get<msCompetitionEntryFee>("MemberSuite:EnterCompetition.EntryFee"); }
            set { SessionManager.Set("MemberSuite:EnterCompetition.EntryFee",value); }
        }
    }

    public static class PostAJob
    {
        public static void Clear()
        {
            JobPosting = null;
        }

        public static msJobPosting JobPosting
        {
            get { return SessionManager.Get<msJobPosting>("MemberSuite:PostAJob.JobPosting"); }
            set { SessionManager.Set("MemberSuite:PostAJob.JobPosting",value); }
        }
    }

    public static class PlaceAnOrder
    {
        public static void Clear()
        {
            ShoppingCart = null;
            Payload = null;
 
        }
        /// <summary>
        /// Gets or sets the shopping cart, which is carried around by the user
        /// through the portal
        /// </summary>
        /// <value>The shopping cart.</value>
        public static msOrder ShoppingCart
        {
            get { return SessionManager.Get<msOrder>("MemberSuite:PlaceAnOrder.ShoppingCart"); }
            set { SessionManager.Set("MemberSuite:PlaceAnOrder.ShoppingCart",value); }
        }

        ///// <summary>
        ///// Gets or sets key that prevents duplicate orders from being submitted
        ///// </summary>
        ///// <value>The shopping cart.</value>
        //public static string AntiDuplicationKey
        //{
        //    get { return SessionManager.Get<string>("MemberSuite:PlaceAnOrder.AntiDuplicationKey"); }
        //    set { SessionManager.Set("MemberSuite:PlaceAnOrder.AntiDuplicationKey",value); }
        //}

        

        /// <summary>
        /// Gets or sets the transient shopping cart.
        /// </summary>
        /// <value>The transient shopping cart.</value>
        /// <remarks>Since the portal is designed with a centralized order process, all orders are
        /// sent to the ConfirmOrder.aspx page for processing. Sometimes, processes need to generate
        /// orders outside of the shopping cart - for instance, a membership registration or an event registration,
        /// and for that, they place an item in the Transient cart and send it to the ConfirmOrder. When the
        /// page is instructed to use a transient shopping cart, it removes the option for the user to
        /// "keep shopping" and forces them to complete their order.</remarks>
        public static msOrder TransientShoppingCart
        {
            get { return SessionManager.Get<msOrder>("MemberSuite:PlaceAnOrder.TransientShoppingCart"); }
            set { SessionManager.Set("MemberSuite:PlaceAnOrder.TransientShoppingCart",value); }
        }

        public static OrderPayload Payload
        {
            get { return SessionManager.Get<OrderPayload>("MemberSuite:PlaceAnOrder.Payload"); }
            set { SessionManager.Set("MemberSuite:PlaceAnOrder.Payload", value); }
        }

        public static string ContinueShoppingUrl
        {
            get { return SessionManager.Get<string>("MemberSuite:PlaceAnOrder.ContinueShoppingUrl"); }
            set { SessionManager.Set("MemberSuite:PlaceAnOrder.ContinueShoppingUrl",value); }
        }

          public static List<DataRow> RecentlyAddedItems
        {

            get
            {
                var dt = SessionManager.Get<DataTable>("MemberSuite:PlaceAnOrder.RecentlyAddedItems");

                // we need to do this for MS-3903, because DataRow isn't serializable and we don't feel like
                // changing code in 20 places
                if (dt == null) return null;
                List<DataRow> rows = new List<DataRow>();
                foreach (DataRow dr in dt.Rows)
                    rows.Add(dr);

                return rows;

            }
            set
            {
                DataTable dt = null;

                if (value != null)
                    if (value.Count == 0) // blank table
                        dt = new DataTable();
                    else
                    {
                        dt = value[0].Table.Clone(); // clone the parent table
                        foreach (var row in value)
                            dt.ImportRow(row);
                    }


                SessionManager.Set("MemberSuite:PlaceAnOrder.RecentlyAddedItems",dt);
            }
        }

        public static string OrderCompleteUrl
        {
            get { return SessionManager.Get<string>("MemberSuite:PlaceAnOrder.OrderCompleteUrl"); }
            set { SessionManager.Set("MemberSuite:PlaceAnOrder.OrderCompleteUrl",value); }
        }

        public static bool ReloadEntityOnOrderComplete
        {
            get
            {
                return SessionManager.Get<bool>("MemberSuite:PlaceAnOrder.ReloadEntityOnOrderComplete");
            
            }
            set { SessionManager.Set("MemberSuite:PlaceAnOrder.ReloadEntityOnOrderComplete",value); }
        }

        public static msOrderLineItem EditOrderLineItem
        {
            get { return SessionManager.Get<msOrderLineItem>("MemberSuite:PlaceAnOrder.EditOrderLineItem"); }
            set { SessionManager.Set("MemberSuite:PlaceAnOrder.EditOrderLineItem",value); }
        }

        public static string EditOrderLineItemProductName
        {
            get { return SessionManager.Get<string>("MemberSuite:PlaceAnOrder.EditOrderLineItemProductName"); }
            set { SessionManager.Set("MemberSuite:PlaceAnOrder.EditOrderLineItemProductName",value); }
        }

        public static List<FieldMetadata> EditOrderLineItemProductDemographics
        {
            get { return SessionManager.Get<List<FieldMetadata>>("MemberSuite:PlaceAnOrder.EditOrderLineItemProductDemographics"); }
            set { SessionManager.Set("MemberSuite:PlaceAnOrder.EditOrderLineItemProductDemographics",value); }
        }

        public static string EditOrderLineItemRedirectUrl
        {
            get { return SessionManager.Get<string>("MemberSuite:PlaceAnOrder.EditOrderLineItemRedirectUrl"); }
            set { SessionManager.Set("MemberSuite:PlaceAnOrder.EditOrderLineItemRedirectUrl",value); }
        }

        public static List<msOrderLineItem> CrossSellItems
        {
            get { return SessionManager.Get<List<msOrderLineItem>>("MemberSuite:PlaceAnOrder.CrossSellItems"); }
            set { SessionManager.Set("MemberSuite:PlaceAnOrder.CrossSellItems",value); }
        }

        public static void InitializeShoppingCart()
        {
            if (MultiStepWizards.PlaceAnOrder.ShoppingCart == null)
            {
                var mso = new msOrder();
                if (ConciergeAPI.CurrentEntity != null)
                    mso.BillTo = mso.ShipTo = ConciergeAPI.CurrentEntity.ID;
                MultiStepWizards.PlaceAnOrder.ShoppingCart = mso;
                CrossSellItems = null;
            }
        }

        public static msOrderLineItem AddItemToShoppingCart(decimal qty, DataRow selectedProduct)
        {
            string productID =  selectedProduct["ID"].ToString();
            decimal unitPrice = (decimal)selectedProduct["Price"];
            //Add the line item to the shopping cart
            var lineItem = new msOrderLineItem
                               {
                                   Quantity = qty,
                                   Product = productID,
                                   UnitPrice = unitPrice,
                                   Total = unitPrice * qty,
                                   OrderLineItemID = Guid.NewGuid().ToString()
                               };

            MultiStepWizards.PlaceAnOrder.ShoppingCart.LineItems.Add(lineItem);

            //Add this as a recently added item
            if (MultiStepWizards.PlaceAnOrder.RecentlyAddedItems == null)
                MultiStepWizards.PlaceAnOrder.RecentlyAddedItems = new List<DataRow>();

            if (MultiStepWizards.PlaceAnOrder.RecentlyAddedItems.Count >= NumberOfRecentItemsToDisplay)
                MultiStepWizards.PlaceAnOrder.RecentlyAddedItems.RemoveAt(0);

            selectedProduct["Quantity"] = 1;
            MultiStepWizards.PlaceAnOrder.RecentlyAddedItems.Add(selectedProduct);
            return lineItem;
        }

        public const int NumberOfRecentItemsToDisplay = 3;

        public static OrderConfirmationPacket OrderConfirmaionPacket
        {
            get { return SessionManager.Get<OrderConfirmationPacket>("MemberSuite:PlaceAnOrder.OrderConfirmaionPacket"); }
            set { SessionManager.Set("MemberSuite:PlaceAnOrder.OrderConfirmaionPacket",value); }
        }

        public static void InitiateOrderProcess(msOrder targetOrder)
        {
            InitiateOrderProcess(targetOrder, null);
        }

        public static void InitiateOrderProcess(msOrder targetOrder, OrderPayload payload)
        {
            if (targetOrder != null)
            {
                TransientShoppingCart = targetOrder;
                Payload = payload;
                HttpContext.Current.Response.Redirect("/orders/InitiateOrder.aspx?useTransient=true");
            }
            else
                HttpContext.Current.Response.Redirect("/orders/InitiateOrder.aspx");
        }
        
    }

    public static class ViewChapterMembers
    {
        public static  void Clear()
        {
            SearchManifest = null;
            SearchBuilder = null;
        }

        public static SearchManifest SearchManifest
        {
            get { return SessionManager.Get<SearchManifest>("MemberSuite:ViewChapterMembers.SearchManifest"); }
            set { SessionManager.Set("MemberSuite:ViewChapterMembers.SearchManifest",value); }
        }

        public static SearchBuilder SearchBuilder
        {
            get { return SessionManager.Get<SearchBuilder>("MemberSuite:ViewChapterMembers.SearchBuilder"); }
            set { SessionManager.Set("MemberSuite:ViewChapterMembers.SearchBuilder",value); }
        }
    }

    public static class ViewSectionMembers
    {
        public static void Clear()
        {
            SearchManifest = null;
            SearchBuilder = null;
        }

        public static SearchManifest SearchManifest
        {
            get { return SessionManager.Get<SearchManifest>("MemberSuite:ViewSectionMembers.SearchManifest"); }
            set { SessionManager.Set("MemberSuite:ViewSectionMembers.SearchManifest",value); }
        }

        public static SearchBuilder SearchBuilder
        {
            get { return SessionManager.Get<SearchBuilder>("MemberSuite:ViewSectionMembers.SearchBuilder"); }
            set { SessionManager.Set("MemberSuite:ViewSectionMembers.SearchBuilder",value); }
        }
    }

    public static class ViewOrganizationalLayerMembers
    {
        public static void Clear()
        {
            SearchManifest = null;
            SearchBuilder = null;
        }
        public static SearchManifest SearchManifest
        {
            get { return SessionManager.Get<SearchManifest>("MemberSuite:ViewOrganizationalLayerMembers.SearchManifest"); }
            set { SessionManager.Set("MemberSuite:ViewOrganizationalLayerMembers.SearchManifest",value); }
        }

        public static SearchBuilder SearchBuilder
        {
            get { return SessionManager.Get<SearchBuilder>("MemberSuite:ViewOrganizationalLayerMembers.SearchBuilder"); }
            set { SessionManager.Set("MemberSuite:ViewOrganizationalLayerMembers.SearchBuilder",value); }
        }
    }

    public static class SearchDirectory
    {
        public static void Clear()
        {
            SearchManifest = null;
            SearchBuilder = null;
        }
        public static SearchManifest SearchManifest
        {
            get { return SessionManager.Get<SearchManifest>("MemberSuite:SearchDirectory.SearchManifest"); }
            set { SessionManager.Set("MemberSuite:SearchDirectory.SearchManifest",value); }
        }

        public static SearchBuilder SearchBuilder
        {
            get { return SessionManager.Get<SearchBuilder>("MemberSuite:SearchDirectory.SearchBuilder"); }
            set { SessionManager.Set("MemberSuite:SearchDirectory.SearchBuilder",value); }
        }
    }

    public static class SearchJobPostings
    {
        public static void Clear()
        {
            SearchManifest = null;
            SearchBuilder = null;
        }
        public static SearchManifest SearchManifest
        {
            get { return SessionManager.Get<SearchManifest>("MemberSuite:SearchJobPostings.SearchManifest"); }
            set { SessionManager.Set("MemberSuite:SearchJobPostings.SearchManifest",value); }
        }

        public static SearchBuilder SearchBuilder
        {
            get { return SessionManager.Get<SearchBuilder>("MemberSuite:SearchJobPostings.SearchBuilder"); }
            set { SessionManager.Set("MemberSuite:SearchJobPostings.SearchBuilder",value); }
        }
    }

    public static class SearchEventRegistrations
    {
        public static void Clear()
        {
            SearchManifest = null;
            SearchBuilder = null;
        }
        public static SearchManifest SearchManifest
        {
            get { return SessionManager.Get<SearchManifest>("MemberSuite:SearchEventRegistrations.SearchManifest"); }
            set { SessionManager.Set("MemberSuite:SearchEventRegistrations.SearchManifest",value); }
        }

        public static SearchBuilder SearchBuilder
        {
            get { return SessionManager.Get<SearchBuilder>("MemberSuite:SearchEventRegistrations.SearchBuilder"); }
            set { SessionManager.Set("MemberSuite:SearchEventRegistrations.SearchBuilder",value); }
        }
    }

    public static class SearchResumeBank
    {
        public static void Clear()
        {
            SearchManifest = null;
            SearchBuilder = null;
        }
        public static SearchManifest SearchManifest
        {
            get { return SessionManager.Get<SearchManifest>("MemberSuite:SearchResumeBank.SearchManifest"); }
            set { SessionManager.Set("MemberSuite:SearchResumeBank.SearchManifest",value); }
        }

        public static SearchBuilder SearchBuilder
        {
            get { return SessionManager.Get<SearchBuilder>("MemberSuite:SearchResumeBank.SearchBuilder"); }
            set { SessionManager.Set("MemberSuite:SearchResumeBank.SearchBuilder",value); }
        }
    }

    public static class AddContact
    {
        public static void Clear()
        {
            SendInvitation = false;
            EmailAddress = null;
            Individual = null;
            RelationshipType = null;
        }


        public static bool SendInvitation
        {
            get
            {
                return SessionManager.Get<bool>("MemberSuite:AddContact.SendInvitation");
            }
            set { SessionManager.Set("MemberSuite:AddContact.SendInvitation",value); }
        }

        public static string EmailAddress
        {
            get { return SessionManager.Get<string>("MemberSuite:AddContact.EmailAddress"); }
            set { SessionManager.Set("MemberSuite:AddContact.EmailAddress",value); }
        }

        public static msIndividual Individual
        {
            get { return SessionManager.Get<msIndividual>("MemberSuite:AddContact.Individual"); }
            set { SessionManager.Set("MemberSuite:AddContact.Individual",value); }
        }

        public static msRelationshipType RelationshipType
        {
            get { return SessionManager.Get<msRelationshipType>("MemberSuite:AddContact.RelationshipType"); }
            set { SessionManager.Set("MemberSuite:AddContact.RelationshipType",value); }
        }

      
    }

    public static class CustomizePage
    {
         

        public static List<msPortalControlPropertyOverride> ControlsEligibleForQuickOverride
        {
            get { return SessionManager.Get<List<msPortalControlPropertyOverride>>("CustomizePage:Controls"); }
            set { SessionManager.Set("CustomizePage:Controls",value); }
        }

        public static string Referrer
        {
            get { return SessionManager.Get<string>("CustomizePage:Referrer"); }
            set { SessionManager.Set("CustomizePage:Referrer",value); }
        }
        public static string PageName
        {
            get { return SessionManager.Get<string>("CustomizePage:PageName"); }
            set { SessionManager.Set("CustomizePage:PageName",value); }
        }

        public static List<string> AllEligibleControls
        {
            get { return SessionManager.Get<List<string>>("CustomizePage:AllControls"); }
            set { SessionManager.Set("CustomizePage:AllControls",value); }
        }

        public static string EditModeControlName
        {
            get { return SessionManager.Get< string>("CustomizePage:EditModeControlName"); }
            set { SessionManager.Set("CustomizePage:EditModeControlName",value); }
        }

        public static string EditModeControlPropertyName
        {
            get { return SessionManager.Get<string>("CustomizePage:EditModeControlPropertyName"); }
            set { SessionManager.Set("CustomizePage:EditModeControlPropertyName",value); }
        }


        public static string EditModeControlPropertyValue
        {
            get { return SessionManager.Get<string>("CustomizePage:EditModeControlPropertyValue"); }
            set { SessionManager.Set("CustomizePage:EditModeControlPropertyValue",value); }
        }

        public static string EditModeControlPropertyDescription 
        {
            get { return SessionManager.Get<string>("CustomizePage:EditModeControlPropertyDescription"); }
            set { SessionManager.Set("CustomizePage:EditModeControlPropertyDescription",value); }
        }

        public static void Clear()
        {
            ControlsEligibleForQuickOverride = null;
            PageName = null;
        }
 
    }

    public static void ClearAll()
    {
        SearchResumeBank.SearchManifest = null;
        SearchResumeBank.SearchBuilder = null;

        SearchJobPostings.SearchManifest = null;
        SearchJobPostings.SearchBuilder = null;

        PlaceAnOrder.ShoppingCart = null;
        RenewMembership.Membership = null;

        CustomizePage.Clear();


        RenewMembership.Clear();
        CreateAccount.Clear();
        RegisterForEvent.Clear();
        GroupRegistration.Clear();
        EnterCompetition.Clear();
        PostAJob.Clear();
        PlaceAnOrder.Clear();
        ViewChapterMembers.Clear();
        ViewSectionMembers.Clear();
        ViewOrganizationalLayerMembers.Clear();
        SearchDirectory.Clear();
        SearchJobPostings.Clear();
        SearchEventRegistrations.Clear();
        SearchResumeBank.Clear();
        AddContact.Clear();
        CustomizePage.Clear();
        // just wipe all keys
        // throw new NotSupportedException();
        /*
        List<string> keys = new List<string>();
        foreach (string key in HttpContext.Current.Session.Keys)
            if (key.StartsWith("MemberSuite:"))
                keys.Add(key);

        foreach( var key in keys )
                HttpContext.Current.Session[key] = null;
         * */
    }


}