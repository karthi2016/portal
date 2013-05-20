using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Manifests.Command;
using MemberSuite.SDK.Manifests.Command.Views;
using MemberSuite.SDK.Types;

public partial class orders_EditOrderLineItem : PortalPage 
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected override bool IsPublic
    {
        get
        {
            return true;
        }
    }

    protected override void InitializeTargetObject()
    {
        if (MultiStepWizards.PlaceAnOrder.EditOrderLineItem == null)
            GoHome();

        base.InitializeTargetObject();
        // important - custom fields need to know who their context is
        CustomFieldSet1.MemberSuiteObject = MultiStepWizards.PlaceAnOrder.EditOrderLineItem; ;
    }

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        ClassMetadata cm = new ClassMetadata();
        cm.Fields = MultiStepWizards.PlaceAnOrder.EditOrderLineItemProductDemographics;
        // setup the metadata
        CustomFieldSet1.Metadata = cm;

        DataEntryViewMetadata vm = new DataEntryViewMetadata();
        vm.Sections = new List<MemberSuite.SDK.Manifests.Command.ViewMetadata.ControlSection>();
        ViewMetadata.ControlSection cs = new ViewMetadata.ControlSection();

        foreach( var f in cm.Fields )
        {
            ControlMetadata cm2 = ControlMetadata.FromFieldMetadata( f );
            cm2.IsRequired = null;   // important, we don't want this overrideen
            cs.LeftControls.Add(  cm2 );
        }
        vm.Sections.Add(cs);
        CustomFieldSet1.PageLayout = vm;

        CustomFieldSet1.Render();
    }

    protected void btnContinue_Click(object sender, EventArgs e)
    {
        if (!IsValid)
            return;

        CustomFieldSet1.Harvest();

        GoTo(MultiStepWizards.PlaceAnOrder.EditOrderLineItemRedirectUrl);
 
    }

    protected void lbCancel_Click(object sender, EventArgs e)
    {
        GoTo(MultiStepWizards.PlaceAnOrder.EditOrderLineItemRedirectUrl);
    }
}