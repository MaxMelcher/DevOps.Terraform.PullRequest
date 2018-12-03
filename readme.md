The goal of the project is to have a better terraform experiene with Azure DevOps.

After a pull request is submitted, the following will happen:

* the app pulls the code
* the app starts 'terraform plan'
* the app will comment on the pull request with the plan output

## Seeing is believing!

//todo video

## Deployment

## Azure DevOps Configuration

## Simple Scenario: 1 Environment

## Advanced Scenario: Multiple Environments

## Limitations

* Parallel Pull Requests
* * If you submit two pull requests to the same branch they are executed one by one - conflicts can occur!
* * The goal would be to have only one plan that can be merged. If a second pull request is opened, it must wait until the first is merged or rejected
* Terraform Workspaces are not supported
* * Right now only the default workspace is used to generate a plan
* Terraform in the docker container is 0.11.10 linux version

![Flow](/docs/flow.png "terraform pull request flow")