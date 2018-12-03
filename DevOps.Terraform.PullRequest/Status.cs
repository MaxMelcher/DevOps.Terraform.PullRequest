using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DevOps.Terraform.PullRequest.Helpers;
using DevOps.Terraform.PullRequest.Models;
using LibGit2Sharp;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DevOps.Terraform.PullRequest
{
    [Route("api/Status")]
    public class Status : Controller
    {

        public enum PullRequestState
        {
            pending = 2,
            succeeded = 4
        }

        public async Task<IActionResult> Hook([FromBody]Content content)
        {

            if (content.EventType.Equals("git.pullrequest.created", StringComparison.CurrentCultureIgnoreCase))
            {

                //set status to pending
                await SetPRStatus(content, PullRequestState.pending);

                //pull code
                Checkout(content);

                //todo logging
                //todo check lock

                //start terraform plan
                var plan = Plan(content);

                //add plan to PR
                await Comment(plan, content);


                //set status to succeeded
                await SetPRStatus(content, PullRequestState.succeeded);

                return new StatusCodeResult(200);
            }
            else
            {
                return new StatusCodeResult(500);
            }
        }

        private async Task Comment((string std, string error) plan, Content content)
        {
            //https://dev.azure.com/mmelcher/35de4fef-8cb5-4980-8b30-10d199188ba6/_apis/git/repositories/dd86a3d3-e0d7-4b05-9ec4-87f1d37eacab/pullRequests/19/threads
            var client = new HttpClient();

            var pullRequestId = content.Resource.PullRequestId;

            var text = string.IsNullOrEmpty(plan.error) ? plan.std : plan.error;

            //todo if not all variables are supplied the error code is 'Error: Required variable not set:'           
            var body = new
            {
                comments = new[]
                {
                    new
                    {
                        content = text
                    }
                }
            };

            //token that has only status permissions
            //todo add PAT from config
            var personalaccesstoken = "ejyfdwegf6hpxlthjflgrtg2fokvq3qf3kvreasdc4c7ir7trlia";

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($":{personalaccesstoken}")));

            var response = await client.PostAsync($"{content.Resource.Repository.Url}/pullRequests/{pullRequestId}/threads?api-version=4.1-preview.1", new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json"));
            Console.WriteLine($"{response.StatusCode}");
        }

        private (string std, string error) Plan(Content content)
        {

            var repository = $"/tmp/repo/{content.Resource.Repository.Name}";
            var init = $"/usr/bin/terraform init -no-color -input=false {repository}".Bash();

            Console.WriteLine($"Init: {init.std}");
            Console.WriteLine($"Init Error: {init.error}");

            if (!string.IsNullOrEmpty(init.error))
            {
                return init;
            }

            //get the plan without colors.
            var plan = $"/usr/bin/terraform plan -no-color -input=false  {repository} ".Bash();

            //we need to escape the + or - or they will be indented by the markdown parser
            //we need to escape the $ sign or they with be interpreted as math formular
            plan.std = Regex.Replace(plan.std, "\\+", m => $"### \\+");
            //plan.std = Regex.Replace(plan.std, "\\$", m => $"\\$");
            plan.std = Regex.Replace(plan.std, "- ", m => $"### \\- ");

            plan.std = Regex.Replace(plan.std, "Plan: ", m => $"## Plan: ");

            Console.WriteLine($"Plan: {plan}");Console.WriteLine($"Plan Error: {plan.error}");

            return plan;
        }

        private async Task SetPRStatus(Content content, PullRequestState pullRequestState)
        {
            //POST https://dev.azure.com/{organization}/{project}/_apis/git/repositories/{repositoryId}/pullRequests/{pullRequestId}/statuses?api-version=4.1-preview.1
            var client = new HttpClient();

            var pullRequestId = content.Resource.PullRequestId;

            //todo maybe add a plan output that is linkable in the 'targetUrl' property

            var body = new
            {
                context = new
                {
                    name = pullRequestState.ToString(),
                    genre = "Terraform-PR",
                },
                description = pullRequestState == PullRequestState.pending ? "terraform plan pending" : "terraform plan executed",
                state = pullRequestState.ToString()
            };

            //token that has only status permissions
            //todo add PAT from config
            var personalaccesstoken = "ejyfdwegf6hpxlthjflgrtg2fokvq3qf3kvreasdc4c7ir7trlia";

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($":{personalaccesstoken}")));

            var response = await client.PostAsync($"{content.Resource.Repository.Url}/pullRequests/{pullRequestId}/statuses?api-version=4.1-preview.1", new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json"));
            Console.WriteLine($"{response.StatusCode}");
        }

        private void Checkout(Content content)
        {
            string logMessage = "";
            string rootedPath = LibGit2Sharp.Repository.Init($"/tmp/repo/{content.Resource.Repository.Name}");
            using (var repo = new LibGit2Sharp.Repository(rootedPath))
            {
                if (repo.Network.Remotes.All(x => x.Name != "origin"))
                {
                    repo.Network.Remotes.Add("origin", content.Resource.Repository.RemoteUrl.AbsoluteUri);
                }

                var remote = repo.Network.Remotes["origin"];
                var refSpecs = remote.FetchRefSpecs.Select(x => x.Specification);

                //fetch all commits without cloning the entire repository
                Commands.Fetch(repo, remote.Name, refSpecs, null, logMessage);

                //checkout the last commit from the PR
                Commands.Checkout(repo, content.Resource.LastMergeSourceCommit.CommitId);
            }
            Console.WriteLine(logMessage);
        }
    }
}