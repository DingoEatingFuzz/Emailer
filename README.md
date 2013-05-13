# Emailer - A simple and pointless template system for .NET

**Disclaimer:** I wrote this before I knew Mustache existed. There isn't a whole lot going on here that mustache doesn't also do and do better.

## Overview
Emailer is a class that takes advantage of the fun stuff introduced in .NET 4&mdash;mainly `dynamic` and `ExpandoObject`. These two tools allow for an infinitely deep object evaluated at runtime (which is exactly what we want). This infinitely deep object is the `data` property of an `Emailer` object.

The `Emailer` already has the `data` property defined, but it has no stuff in it. So using this engine is as straight-forward as this:

1. Create an `Emailer` object with a template filename as an argument
2. Add stuff to its `data` property
3. Call `Emailer.render()`

Is this weird? Yeah, a little bit. You are probably used to passing some form of data object along with some sort of template string when constructing a template renderer. You have to keep in mind that I wrote this for a very specific need and only used it a couple times. It's called `Emailer` for crying out loud; I wasn't planning for long-run. As weird as the API is, it still works.

The last thing you should know before using this tool (yeah right, as if anyone is going to use this) is that in a very unfancy way, Emailer iterates over the data object to perform replacements to the template string. This means parsing is non-existent and missing properties results in template artifacts in your rendered string, (e.g., [[thisWasPointless]] will show up as is in your rendered string of the property `thisWasPointless` isn't in the data object. Again, very specific need.

Anyway, on with the features.

## Properties
What templating engine is complete without properties? In this syntax, properties are referenced by name between double square brackets `[[property]]`. Properties that are mentioned in a template, but don't exist in the data object stay in the template (as mentioned above).

## Conditionals
This was the last feature added to Emailer. Conditionals are created with square brackets and percent signs: `[%condition Arbitrary Content including line-breaks %]`. There is no "else" to this "if". If you want that, you need to do so with another property on the data object. After all, what is an else but an opposite if? To exercise this template feature, the data object needs to have a property with the name used in the opening tag with a `Boolean` value.

## Blocks
Probably the coolest and most important feature of Emailer is blocks. This allows for loops and context-changing. Blocks are created with square brackets and curly braces: `[{block Arbitrary Content including line-breaks }]`. Blocks expect a `List<ExpandoObject>` in the data object. It will extract the entire block from the template and render it for each item in the list with the context of the element in the list. This means If you have a `books` property on data, and a `title` property on each book in that list, the template will still use the `[[title]]` syntax in the block since it is now looking at the book object in the list. Pretty neat, or at least I thought so.

## Under the Hood
1. A full template *is* a block. This means blocks can be nested
2. Blocks can be placed in conditionals
3. Conditionals can be placed in blocks
4. Variables can be placed in variables, but careful of the evaluation order

## The end
I hope you like it, but I'm not counting on it.
