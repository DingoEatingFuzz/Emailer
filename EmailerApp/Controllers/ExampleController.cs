using EmailerTemplate;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmailerApp.Controllers
{
    public class ExampleController : Controller
    {
        public string Index()
        {
            var file = Server.MapPath("~/example.html");
            var emailer = new Emailer(file);
            var d = emailer.data;

            d.title = "Example use of Emailer";
            d.hasSubtitle = true;
            d.subtitle = "This wouldn't show up if hasSubtitle was false.";
            d.items = new List<ExpandoObject>();

            dynamic properties = new ExpandoObject();
            properties.title = "Properties";
            properties.description = "Replace templated property names with their matching values in the data object.";
            properties.colors = new List<ExpandoObject>();
            Array.ForEach(new string[] { "#dde", "#ded", "#edd", "#ede" }, x =>
            {
                dynamic color = new ExpandoObject();
                color.color = x;
                properties.colors.Add(color);
            });

            dynamic conditionals = new ExpandoObject();
            conditionals.title = "Conditionals";
            conditionals.description = "Allow optional content to only be shown when a property matching the tag is true.";
            conditionals.colors = new List<ExpandoObject>();
            Array.ForEach(new string[] { "#f66", "#6f6", "#66f" }, x =>
            {
                dynamic color = new ExpandoObject();
                color.color = x;
                conditionals.colors.Add(color);
            });

            dynamic blocks = new ExpandoObject();
            blocks.title = "Blocks";
            blocks.description = 
                @"Render the entire block using the context of the dynamic object that matches the name of the block tag. 
                As demonstrated by the color squares, blocks can be nested and the scope changes as expected.";
            blocks.colors = new List<ExpandoObject>();
            Array.ForEach(new string[] { "#fff", "#ccc", "#999", "#666", "#333", "#000" }, x =>
            {
                dynamic color = new ExpandoObject();
                color.color = x;
                blocks.colors.Add(color);
            });

            d.items.Add(properties);
            d.items.Add(conditionals);
            d.items.Add(blocks);

            return emailer.render();
        }

    }
}
