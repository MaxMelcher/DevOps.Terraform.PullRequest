using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DevOps.Terraform.PullRequest.Models
{
    public partial class Content
    {
        [JsonProperty("subscriptionId")]
        public Guid SubscriptionId { get; set; }

        [JsonProperty("notificationId")]
        public long NotificationId { get; set; }

        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("eventType")]
        public string EventType { get; set; }

        [JsonProperty("publisherId")]
        public string PublisherId { get; set; }

        [JsonProperty("message")]
        public Message Message { get; set; }

        [JsonProperty("detailedMessage")]
        public Message DetailedMessage { get; set; }

        [JsonProperty("resource")]
        public Resource Resource { get; set; }

        [JsonProperty("resourceVersion")]
        public string ResourceVersion { get; set; }

        [JsonProperty("resourceContainers")]
        public ResourceContainers ResourceContainers { get; set; }

        [JsonProperty("createdDate")]
        public DateTimeOffset CreatedDate { get; set; }
    }

    public partial class Message
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("html")]
        public string Html { get; set; }

        [JsonProperty("markdown")]
        public string Markdown { get; set; }
    }

    public partial class Resource
    {
        [JsonProperty("repository")]
        public Repository Repository { get; set; }

        [JsonProperty("pullRequestId")]
        public long PullRequestId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("createdBy")]
        public CreatedBy CreatedBy { get; set; }

        [JsonProperty("creationDate")]
        public DateTimeOffset CreationDate { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("sourceRefName")]
        public string SourceRefName { get; set; }

        [JsonProperty("targetRefName")]
        public string TargetRefName { get; set; }

        [JsonProperty("mergeStatus")]
        public string MergeStatus { get; set; }

        [JsonProperty("mergeId")]
        public Guid MergeId { get; set; }

        [JsonProperty("lastMergeSourceCommit")]
        public Commit LastMergeSourceCommit { get; set; }

        [JsonProperty("lastMergeTargetCommit")]
        public Commit LastMergeTargetCommit { get; set; }

        [JsonProperty("lastMergeCommit")]
        public Commit LastMergeCommit { get; set; }

        [JsonProperty("reviewers")]
        public List<Reviewer> Reviewers { get; set; }

        [JsonProperty("commits")]
        public List<Commit> Commits { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("_links")]
        public Links Links { get; set; }
    }

    public partial class Commit
    {
        [JsonProperty("commitId")]
        public string CommitId { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }
    }

    public partial class CreatedBy
    {
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("uniqueName")]
        public string UniqueName { get; set; }

        [JsonProperty("imageUrl")]
        public Uri ImageUrl { get; set; }
    }

    public partial class Links
    {
        [JsonProperty("web")]
        public Statuses Web { get; set; }

        [JsonProperty("statuses")]
        public Statuses Statuses { get; set; }
    }

    public partial class Statuses
    {
        [JsonProperty("href")]
        public Uri Href { get; set; }
    }

    public partial class Repository
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("project")]
        public Project Project { get; set; }

        [JsonProperty("defaultBranch")]
        public string DefaultBranch { get; set; }

        [JsonProperty("remoteUrl")]
        public Uri RemoteUrl { get; set; }
    }

    public partial class Project
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("visibility")]
        public string Visibility { get; set; }

        [JsonProperty("defaultTeamImageUrl")]
        public object DefaultTeamImageUrl { get; set; }
    }

    public partial class Reviewer
    {
        [JsonProperty("reviewerUrl")]
        public object ReviewerUrl { get; set; }

        [JsonProperty("vote")]
        public long Vote { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("uniqueName")]
        public string UniqueName { get; set; }

        [JsonProperty("imageUrl")]
        public Uri ImageUrl { get; set; }

        [JsonProperty("isContainer")]
        public bool IsContainer { get; set; }
    }

    public partial class ResourceContainers
    {
        [JsonProperty("collection")]
        public Account Collection { get; set; }

        [JsonProperty("account")]
        public Account Account { get; set; }

        [JsonProperty("project")]
        public Account Project { get; set; }
    }

    public partial class Account
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
    }

    public partial class Content
    {
        public static Content FromJson(string json) => JsonConvert.DeserializeObject<Content>(json, DevOps.Terraform.PullRequest.Models.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Content self) => JsonConvert.SerializeObject(self, DevOps.Terraform.PullRequest.Models.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
