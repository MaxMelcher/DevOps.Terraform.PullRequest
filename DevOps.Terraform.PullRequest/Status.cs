using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DevOps.Terraform.PullRequest.Models;
using LibGit2Sharp;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DevOps.Terraform.PullRequest
{
    [Route("api/[controller]")]
    public class Status : Controller
    {

        public IActionResult Hook([FromBody]Content content)
        {

            if (content.EventType.Equals("git.pullrequest.created", StringComparison.CurrentCultureIgnoreCase))
            {

                //set status
                SetPRStatus(content);


                //pull code

                /*
                 * git config uploadpack.allowReachableSHA1InWant true
                   # Now it works.
                   cd ../local
                   git fetch origin "$SHA3"
                   git checkout "$SHA3"
                 */
                Checkout();


                //start terraform plan

                //add plan to PR

                return new StatusCodeResult(200);
            }
            else
            {
                return new StatusCodeResult(500);
            }
        }

        private async Task SetPRStatus(Content content)
        {
            //POST https://dev.azure.com/{organization}/{project}/_apis/git/repositories/{repositoryId}/pullRequests/{pullRequestId}/statuses?api-version=4.1-preview.1
            var client = new HttpClient();

            var organization = "mmelcher";
            var project = content.Resource.Repository.Project.Name;
            var repositoryId = content.Resource.Repository.Id;
            var pullRequestId = content.Resource.PullRequestId;

            var body = new
            {
                context = new
                {
                    genre = "",
                    name = "pending",
                },
                state = "pending"
            };

            var response = await client.PostAsync($"https://dev.azure.com/{organization}/{project}/_apis/git/repositories/{repositoryId}/pullRequests/{pullRequestId}/statuses?api-version=4.1-preview.1", new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json"));
            Console.WriteLine($"{response.StatusCode} - {response.Content}");
        }

        private void Checkout()
        {
            string logMessage = "";
            string rootedPath = LibGit2Sharp.Repository.Init(@"/tmp/repo");
            using (var repo = new LibGit2Sharp.Repository("/tmp/repo"))
            {
                if (repo.Network.Remotes.All(x => x.Name != "origin"))
                {
                    repo.Network.Remotes.Add("origin", "https://mmelcher@dev.azure.com/mmelcher/DevOps.Terraform.PullRequest/_git/DevOps.Terraform.PullRequest"); 
                }

                var remote = repo.Network.Remotes["origin"];
                var refSpecs = remote.FetchRefSpecs.Select(x => x.Specification);
                Commands.Fetch(repo, remote.Name, refSpecs, null, logMessage);

                Commands.Checkout(repo, "d99bf56e");
            }
            Console.WriteLine(logMessage);
        }
    }
}