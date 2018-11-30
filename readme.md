The goal of the project is to have a better terraform experiene with Azure DevOps.

After a pull request is submitted, the following will happen:

* the app pulls the code
* the app starts 'terraform plan'
* the app will comment on the pull request with the plan output

![Flow](/docs/flow.png "terraform pull request flow")