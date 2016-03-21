using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Types;

public partial class discussions_CreateEditDiscussionTopic : DiscussionsPage
{
    #region Fields

    #endregion

    #region Initialization

    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();

        var targetObject = APIExtensions.LoadObjectFromAPI(ContextID);

        if(targetObject == null)
        {
            GoToMissingRecordPage();
            return;
        }

        switch(targetObject.ClassType)
        {
            case msForum.CLASS_NAME:
                targetForum = targetObject.ConvertTo<msForum>();
                targetDiscussionTopic = CreateNewObject<msDiscussionTopic>();
                break;
            case msDiscussionTopic.CLASS_NAME:
                targetDiscussionTopic = targetObject.ConvertTo<msDiscussionTopic>();
                targetForum = LoadObjectFromAPI<msForum>(targetDiscussionTopic.Forum);
                editMode = true;
                break;
            default:
                QueueBannerError("Unexpected context type: " + targetObject.ClassType);
                GoHome();
                break;
        }

        if(targetForum == null || targetDiscussionTopic == null)
        {
            GoToMissingRecordPage();
            return;
        }

        TargetDiscussionBoard = LoadObjectFromAPI<msDiscussionBoard>(targetForum.DiscussionBoard);

        //You can never edit the initial post from this page - that is done by editing the post directly
        targetDiscussionPost = CreateNewObject<msDiscussionPost>();
    }

    protected override void InitializePage()
    {
        base.InitializePage();

        trMessage.Visible = !editMode;

        bind();

        chkSubscribe.Checked = !editMode || drSubscription != null;

        if (editMode)
            CustomTitle.Text = string.Format("{0}: Edit Topic", targetForum.Name);
        else
            CustomTitle.Text = string.Format("{0}: New Topic", targetForum.Name);
    }

    protected override bool CheckSecurity()
    {
        if (!base.CheckSecurity())
            return false;

        if (editMode)
            return targetDiscussionTopic.PostedBy == ConciergeAPI.CurrentEntity.ID;

        return true;
    }

    #endregion

    #region Methods

    protected void bind()
    {
        tbName.Text = targetDiscussionTopic.Name;
    }

    protected void unbind()
    {
        targetDiscussionTopic.Name = tbName.Text;
        targetDiscussionTopic.Forum = targetForum.ID;
        
        targetDiscussionPost.Post = tbMessage.Text;
        targetDiscussionPost.Status = targetForum.Moderated && !isModerator()
                                          ? DiscussionPostStatus.Pending
                                          : DiscussionPostStatus.Approved;

        targetDiscussionTopic.PostedBy = targetDiscussionPost.PostedBy = ConciergeAPI.CurrentEntity.ID;
    }

    #endregion

    #region Event Handlers

    protected void btnPost_Click(object sender, EventArgs e)
    {
        unbind();

        targetDiscussionTopic = SaveObject(targetDiscussionTopic);

        string message = "Your Topic has been posted.";

        if(!editMode)
        {
            targetDiscussionPost.Topic = targetDiscussionTopic.ID;
            targetDiscussionPost = SaveObject(targetDiscussionPost);

            if (targetDiscussionPost.Status == DiscussionPostStatus.Approved)
            {
                message += " Your new message has been posted";
                using (IConciergeAPIService proxy = GetConciegeAPIProxy())
                {
                    proxy.SendEmailsToSubscribedEntities(targetDiscussionPost.ID);
                }
            }

            if (targetDiscussionPost.Status == DiscussionPostStatus.Pending)
                message += " Your new message has been saved. It will be posted once approved by a moderator.";
        }

        if (chkSubscribe.Checked && drSubscription == null)
        {
            msDiscussionTopicSubscription subscription = CreateNewObject<msDiscussionTopicSubscription>();
            subscription.Topic = targetDiscussionTopic.ID;
            subscription.Subscriber = ConciergeAPI.CurrentEntity.ID;

            SaveObject(subscription);
        }

        GoTo(string.Format(@"~\discussions\ViewForum.aspx?contextID={0}", targetForum.ID), message);
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        GoHome();
    }

    #endregion
}