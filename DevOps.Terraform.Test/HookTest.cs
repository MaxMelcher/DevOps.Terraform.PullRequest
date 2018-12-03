using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DevOps.Terraform.PullRequest.Models;
using Newtonsoft.Json;
using Xunit;

namespace DevOps.Terraform.Test
{
    public class HookTest
    {
        [Fact]
        public async Task InvokeHook()
        {
            string lines = System.IO.File.ReadAllText(@"hook.json");

            var client = new HttpClient();
            await client.PostAsync("https://localhost:44377/api/status", new StringContent(lines, Encoding.UTF8, "application/json"));
        }
    }
}
