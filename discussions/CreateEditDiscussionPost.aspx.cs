using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MemberSuite.SDK.Concierge;
using MemberSuite.SDK.Types;

public partial class discussions_CreateEditDiscussionPost : DiscussionsPage
{
    #region Fields

    protected msDiscussionPost replyToPost;

    #endregion

    #region Properties

    protected string ReplyToId
    {
        get { return Request.QueryString["ReplyToID"]; }
    }

    protected string Action
    {
        get { return Request.QueryString["action"]; }
    }

    #endregion

    #region Initialization

    protected override void InitializeTargetObject()
    {
        base.InitializeTargetObject();

        MemberSuiteObject targetObject = LoadObjectFromAPI(ContextID);

        if (targetObject == null)
        {
            GoToMissingRecordPage();
            return;
        }

        switch (targetObject.ClassType)
        {
            case msDiscussionTopic.CLASS_NAME:
                targetDiscussionTopic = targetObject.ConvertTo<msDiscussionTopic>();
                targetForum = LoadObjectFromAPI<msForum>(targetDiscussionTopic.Forum);
                targetDiscussionPost = CreateNewObject<msDiscussionPost>();
                break;
            case msDiscussionPost.CLASS_NAME:
                targetDiscussionPost = targetObject.ConvertTo<msDiscussionPost>();
                targetDiscussionTopic = LoadObjectFromAPI<msDiscussionTopic>(targetDiscussionPost.Topic);
                targetForum = LoadObjectFromAPI<msForum>(targetDiscussionTopic.Forum);
                editMode = true;
                break;
            default:
                QueueBannerError("Unexpected context type: " + targetObject.ClassType);
                GoHome();
                break;
        }

        if (targetForum == null || targetDiscussionTopic == null)
        {
            GoToMissingRecordPage();
            return;
        }

        TargetDiscussionBoard = LoadObjectFromAPI<msDiscussionBoard>(targetForum.DiscussionBoard);

        if (!string.IsNullOrWhiteSpace(ReplyToId))
            replyToPost = LoadObjectFromAPI<msDiscussionPost>(ReplyToId);

        loadSubscription();
    }

    protected override void InitializePage()
    {
        base.InitializePage();

        bind();

        if (string.Equals(Action, "remove", StringComparison.CurrentCultureIgnoreCase) && editMode)
        {
            targetDiscussionPost.Status = DiscussionPostStatus.Rejected;
            btnPost_Click(btnPost, null);
        }

        chkSubscribe.Checked = !editMode || drSubscription != null;

    }

    protected override bool CheckSecurity()
    {
        if (!base.CheckSecurity())
            return false;

        if (editMode)
            return targetDiscussionPost.PostedBy == ConciergeAPI.CurrentEntity.ID;

        return true;
    }

    #endregion

    #region Methods

    protected void bind()
    {
        tbName.Text = editMode ? targetDiscussionPost.Name : string.Format("RE: {0}", replyToPost == null ? targetDiscussionTopic.Name : replyToPost.Name);
        tbMessage.Text = targetDiscussionPost.Post;
    }

    protected void unbind()
    {
        targetDiscussionPost.Topic = targetDiscussionTopic.ID;
        targetDiscussionPost.Name = tbName.Text;
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

        targetDiscussionPost = SaveObject(targetDiscussionPost);

        string message = null;

        if (targetDiscussionPost.Status == DiscussionPostStatus.Approved)
        {
            message = "Your message has been posted";

            if(!editMode)
                using(IConciergeAPIService proxy = GetConciegeAPIProxy())
                {
                    proxy.SendEmailsToSubscribedEntities(targetDiscussionPost.ID);
                }
        }

        if (targetDiscussionPost.Status == DiscussionPostStatus.Pending)
            message = "Your message has been saved. It will be posted once approved by a moderator.";

        if (chkSubscribe.Checked && drSubscription == null)
        {
            msDiscussionTopicSubscription subscription = CreateNewObject<msDiscussionTopicSubscription>();
            subscription.Topic = targetDiscussionTopic.ID;
            subscription.Subscriber = ConciergeAPI.CurrentEntity.ID;

            SaveObject(subscription);
        }

        GoTo(string.Format(@"~\discussions\ViewDiscussionTopic.aspx?contextID={0}", targetDiscussionTopic.ID), message);
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        GoHome();
    }

    #endregion
}