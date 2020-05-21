<p align="center">
  <img src="./resources/cat.png" width="450" />
</p>
<h1 align="center">
  credential-cat
</h1>

<p align="center">
  Enumerating leaked credentials on several sources around the dark web (also in surface).
  <br/><br/>
  <a href="http://www.wtfpl.net/txt/copying/">
    <img alt="WTFPL License" src="https://img.shields.io/github/license/BizarreNULL/credential-cat" />
  </a>
  <img alt="GitHub code size in bytes" src="https://img.shields.io/github/languages/code-size/BizarreNULL/credential-cat">
</p>


> ### ⚠️ Work in progress!
>
> As I [@BizarreNULL](https://github.com/BizarreNULL) currently am the sole maintainer of this *repo* due to my job and mangá vicious, maybe the things seem to be a little bit slower. Interested in helping out to make this more useful? With triage, troubleshooting, or issue handling? Let me know!



## Get started

We have a working folder for `credential-cat`, by default is located on documents directory under `.credential_cat`, but can be changed by setting the `CREDENTIAL_CAT_HOME` environment variable under Windows, Linux or MacOS. You can check the current value of this variable by starting the CLI with `--env` flag.

Also is necessary to setup a *webproxy* for the requests, or use `--bypass-proxy` under `search` command for ignore this first setup or in any necessary condition. The `proxy` command have all the information that you need to get setup new ones, or import from a list.

The current available *sources* is:

- Pwndb;

You can check all the possibilities under `source` command, also we provide a `IEngine` interface for you create your very owns *sources* for `credential-cat`. To start searching around, you can choose several options under `search` command, like search by hash and/or password or user and/or email address, including use a wordlist for any kind of search. Some useful examples:

```bash
$ credential-cat search --user-list huge_user_wordlist.txt --case-sensitive --export json
```

You can also do the same with password:

```bash
$ credential-cat search --password "cats_are_cool" --bypass-proxy -iU
```

For every request the `credential-cat` holds a powerful cache for evading server blocks or desnecessary usage of the network. You can check the help page inside `cache` command for more information.



## Developing custom data sources

Application allows the users to extend the data sources options by implementing the `IEngine` interface, the binary need to match with the existent target version of the `CredentialCat.Shared` dotnet runtime. You can manage sources extensions by using the `source` command on the CLI. Above we have a *hello, world!* example:

```csharp
public class CustomEngine : IEngine 
{
    private readonly DatabaseContext _context;
    
    public CustomEngine(DatabaseContext databaseContext)
    {
        _context = databaseContext;
    }

    public Task<IQueryable<CacheEntity>> SearchByUserOrEmail(string value, bool ignoreCache, bool ignoreUpdate, bool forceUpdate, bool caseSensitive, int timeout, int limit, bool bypassProxy)
    {
        System.Console.WriteLine("Hello, world!");
        Environment.Exit(0);
    }
// ...
```

Also, is necessary to have this signature for `IEngine` implementation constructor, to interact with the cache database. On the future, we will provide a wrapper for data persistence.



## Building the *purr*

The `credential-cat` needs dotnet core SDK up to 3.1 for work properly on development build, currently we don't have any production-ready release. Any text-editor or IDE is good enough to work with this tool. I am using Visual Studio 2019 with ReSharper Ultimate 2020.1.3.

```bash
$ git clone git@github.com:BizarreNULL/credential-cat.git
```

Then navigate to `CredentialCat.Console` project:

```bash
$ cd credential-cat/src/CredentialCat.Console
```

We don't recommend to publish the application yet. Running in debug mode is the best approach for several reasons, first you can help me to find any issues with the application behavior, specially on design of the CLI and `Engines` implementations. In any case, you can run with:

```bash
$ dotnet run -- --help
```

Now you are ready to rock!



## Licenses

[credential-cat](https://github.com/BizarreNULL/credential-cat) logo and all other *repo* icons made by <a href="https://www.flaticon.com/authors/freepik" title="Freepik">Freepik</a> from <a href="https://www.flaticon.com/" title="Flaticon">www.flaticon.com.</a>

