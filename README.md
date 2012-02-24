# Razor Client Templates #

The Razor Client Templates library allows you to reuse your Partial Razor Views as client side (JavaScript) templates. 
In addition to ensuring consistency between the HTML generated on the client with what's generated server-side, it also means that you don't have to maintain two different sets of markup - just write once and run anywhere!



## Installation ##

Install via NuGet!

<code style="background-color: #202020;border: 4px solid silver;border-radius: 5px;-moz-border-radius: 5px;-webkit-border-radius: 5px;box-shadow: 2px 2px 3px #6e6e6e;color: #E2E2E2;display: block;font: 1.5em 'andale mono', 'lucida console', monospace;line-height: 1.5em;overflow: auto;padding: 15px;">
PM&gt;  Install-Package RazorClientTemplates
</code>



## Usage ##

It's easy to get started using Razor templates on the client-side.
Just follow a few simple steps:


### Step 1: Create your Razor template

First, you'll need a Razor template that you're using as a partial view.
In fact, you've probably already completed this step.

For example, take the following server-side Razor view:

	<div id="movies">
		@foreach(var movie in Model) {
			@Html.Partial("Movie", movie")
		}
	</div>

...which uses the Partial view *Movie.cshtml:*
	
	<div class="movie">	    
	    <h2>@Model.Title</h2>	    
        <p>Release Date: @Model.ReleaseDate</p>
        <p>Running Time: @Model.RunningTime</p>
	    
	    <h3>Actors</h3>
	    <ul class="actors">
	    @foreach(var actor in Model.Actors) {
	        <li>@actor.Name</li>
	    }
	    </ul>
	</div>

This is all fine and good when you want to render everything on the server, but what happens when you make AJAX calls to retrieve movie data on the client?
Just use **Razor Client Templates** to transform *Movie.cshtml* to a JavaScript function that you can call on the client!

### Step 2: Convert Razor markup to a Client Template

A client template is actually nothing more than a JavaScript function that produces rendered markup.
There are two ways to convert your Razor markup to a JavaScript client template function: render the function inline, or include it as an external JavaScript include.

**Inline JavaScript function**

You can use the `Html.ClientTemplate` helper method to render the script inline, directly to your page, like so:

	var movieTemplate = @Html.ClientTemplate("Movie")

...which will render the following to the client:

	var movieTemplate = function(Model) {
	    var _buf = [];
	    _buf.push('<div class=\"movie\">	    \r\n	    <h2>');
	    _buf.push(Model.Title);
	    _buf.push('</h2>	    \r\n        <p>Release Date: ');
	    _buf.push(Model.ReleaseDate);
	    _buf.push('</p>\r\n        <p>Running Time: ');
	    _buf.push(Model.RunningTime);
	    _buf.push('</p>\r\n	    \r\n	    <h3>Actors</h3>\r\n	    <ul class=\"actors\">\r\n');
	    for (var __i = 0; __i < Model.Actors.length; __i++) {
	        var actor = Model.Actors[__i];
	        _buf.push('	        <li>');
	        _buf.push(actor.Name);
	        _buf.push('</li>\r\n');
	    }
	    _buf.push('	    </ul>\r\n	</div>');
	    return _buf.join('');
	};

**External JavaScript Include**

Or, you can use the custom script handler to generate an external script include to keep that JavaScript out of your HTML:
	
	<script type="text/javascript" src="/ClientTemplate.axd?template=~/Views/Shared/Movie.cshtml&name=movieTemplate"></script>


### Step 3: Use the client template in the browser!


However you get it to the browser, you've now got a JavaScript function that you can call to render markup on the client! Just call the function and pass it a model and it will spit out rendered HTML that you can use however you like:

	var renderedMovie = movieTemplate({ 
            Title: "The Big Lebowski",
            ReleaseDate: "1998",
            RunningTime: "117 mins",
            Actors: [ 
                { Name: "Jeff Bridges" },
                { Name: "John Goodman" },
                { Name: "Steve Buscemi" }
            ]
        });

	$('#movies').append(renderedMovie);



## Supported Scenarios / Known Limitations

Look, in addition to the fact that this project is quite young, Razor markup wasn't designed to be converted into JavaScript functions, so naturally there are going to be some things that work, some things that don't work quite right (yet) and some things that may never work correctly.
You are, after all, translating from a statically-typed language to a dynamically-typed language - some things are simply going to be lost in translation!

That being said, here is what is known to be working, not working, and things we plan to get working.

### What works:

- Basic C# code constructs (`if/then`, `foreach`, etc.)
- Basic model property accessors work fine, but of course you are not going to have access to .NET methods and properties on the client (nor will you have JavaScript nuances on the server)
- Basic `foreach` loops work, as long as you stick to the basic model property accessors (e.g. `foreach(var movie in Model.Movies)`)
- Basic `if/else` checks work against simple boolean properties
- Comments are "supported" (in that they are ignored...)

### Planned to work:

- Support for Razor Helpers (the `@helper MyFunc()` syntax) should work soon


### What doesn't (and probably never will) work:

- VB.  C# rocks way too much to worry about VB support.
- JavaScript methods and properties on the server (this should be a given... right? this tool converts from *server* code to *client* code, not the other way around!)
- .NET methods and properties on the client -- stick to basic DTO models!
- ... You tell us!  If you find something that you think should work but doesn't, please submit an issue via our GitHub Issues page!
