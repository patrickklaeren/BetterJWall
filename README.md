# BetterJWall

### A lightweight and clutter free alternative to the built in wallboards for JIRA server instances.

After a couple of years of working in a  team environment, constantly losing oversight and dragging people to update physical boards in the office, I grew increasingly frustrated with what JIRA refers to as a "wallboard" on their server instance. It is a blackscreen with so much clutter that it pushes anything relevant off the board. Queue: BetterJWall. A website built on the ASP.NET Core framework, utilising Atlassian's JIRA SDK and powered by what developers (especially scrum masters) *actually* want to see when glancing at a wallboard.

This isn't a hosted service, *yet*. If you want this, you'll need to build from source and host it somewhere, defining a user that has access to your JIRA instance in the `appsettings.json` file.

This is still a pretty basic application, and it will be built upon. However, it achieves its core aim of being a lightweight, generic, JIRA Server wallboard.

### How it's built

* Built with .NET Standard 2.0 in mind, on ASP.NET Core
* Dependency injection, with [AutoFac](https://github.com/markdown-it/markdown-it) as the container
* Atlassian's JIRA SDK (modified*)
* [RESTSharp](https://github.com/markdown-it/markdown-it) (a dependency for Atlassian's SDK)
* Newtonsoft [JSON.NET](https://github.com/markdown-it/markdown-it) for JSON (de)serialisation

There is no public release of Atlassian's SDK for .NET Standard 2.0 as of yet - hence I am rehosting the SDK here with alterations made to the source to make it fit for .NET Standard 2.0.

### Contribute any way you want

You can steal, hack or take this code for your own purposes. This was created solely for an end to my frustrations. If you do, however, encounter any issues with the source or have questions, submit an issue. Otherwise, if you're happy enough to contribute, feel free to submit a pull request.