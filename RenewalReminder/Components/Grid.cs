using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using RenewalReminder.Components;
using RenewalReminder.Services.Abstract;
using RenewalRemindr.Models;

namespace RenewalReminder.CS.Web.Components
{
    [HtmlTargetElement("grid")]
    public class Grid : TagHelper
    {
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public string Url { get; set; }
        public int PageSize { get; set; }
        public int Page { get; set; }
        public bool PreventFirstLoad { get; set; }
        public bool NoStore { get; set; }
        public string FilterSelector { get; set; }

        public Grid()
        {
            PageSize = 20;
            Page = 1;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var renderId = Extensions.GenerateKeyword(8);
            var render = !context.Items.ContainsKey("RenderId");
            if (render)
            {
                context.Items.Add("RenderId", renderId);
                context.Items.Add("GridContext_" + renderId, new GridContext());
                context.Items.Add("FilterContext_" + renderId, new FilterContext());
            }
            renderId = context.Items["RenderId"] as string;
            var gridContext = context.Items["GridContext_" + renderId] as GridContext;
            var filterContext = context.Items["FilterContext_" + renderId] as FilterContext;

            var table = new TagBuilder("table");
            foreach (var item in output.Attributes)
            {
                table.MergeAttribute(item.Name, item.Value?.ToString(), true);
            }
            output.Attributes.Clear();

            var hashedId = (ViewContext.RouteData.Values["controller"] + "-" + ViewContext.RouteData.Values["action"]).GetHashCode();
            var id = table.Attributes.GetValueOrDefault("id");
            if (string.IsNullOrEmpty(id))
            {
                id = "grid" + hashedId;
                if (!NoStore)
                {
                    table.MergeAttribute("data-grid-id", id, true);
                }
                table.MergeAttribute("id", id, true);
            }
            else
            {
                table.MergeAttribute("data-grid-id", id + hashedId, true);
            }
            gridContext.Id = id;

            var gridRequest = ViewContext.HttpContext.RequestServices.GetService<IUserAccessor>().Get<GridRequest>("GridRequest");
            if (gridRequest != null && gridRequest.SessionKey == table.Attributes.GetValueOrDefault("data-grid-id"))
            {
                gridContext.Request = gridRequest;
            }
            else
            {
                gridRequest = null;
            }

            await output.GetChildContentAsync();

            if (filterContext.HtmlContent != null)
            {
                FilterSelector = "#" + filterContext.Id;
                gridContext.HtmlContents.Add(filterContext.HtmlContent);
            }

            table.AddCssClass("grid dataTable");
            table.MergeAttribute("data-url", Url, true);
            table.MergeAttribute("data-page-size", gridRequest != null ? gridRequest.PageSize.ToString() : PageSize.ToString(), true);
            table.MergeAttribute("data-page", gridRequest != null ? gridRequest.Page.ToString() : Page.ToString(), true);
            table.MergeAttribute("data-prevent-load", PreventFirstLoad ? "1" : "0", true);
            if (!string.IsNullOrEmpty(FilterSelector))
            {
                table.Attributes["data-filter"] = FilterSelector;
            }

            var thead = new TagBuilder("thead");
            var tr = new TagBuilder("tr");
            foreach (var column in gridContext.Columns)
            {
                tr.InnerHtml.AppendHtml(column);
            }

            thead.InnerHtml.AppendHtml(tr);
            table.InnerHtml.AppendHtml(thead);
            table.InnerHtml.AppendHtml(new TagBuilder("tbody"));

            var div = new TagBuilder("div");
            div.AddCssClass("table-responsive flex-grow-1");
            div.InnerHtml.AppendHtml(table);
            gridContext.HtmlContents.Add(div);

            foreach (var template in gridContext.JsTemplates)
            {
                gridContext.HtmlContents.Add((TagBuilder)template);
            }
            if (render)
            {
                output.TagName = "";
                output.TagMode = TagMode.StartTagAndEndTag;
                foreach (var item in gridContext.HtmlContents)
                {
                    output.Content.AppendHtml(item);
                }
            }
            else
            {
                output.SuppressOutput();
            }
        }
    }

    [HtmlTargetElement("column", ParentTag = "grid")]
    public class GridColumn : TagHelper
    {
        public string Field { get; set; }
        public string Title { get; set; }
        public bool Orderable { get; set; }
        public string OrderType { get; set; }
        public string Template { get; set; }
        public bool Hidden { get; set; }

        [HtmlAttributeName("for")]
        public ModelExpression FieldExpression { get; set; }

        public GridColumn()
        {
            Orderable = true;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var renderId = context.Items["RenderId"] as string;
            var gridContext = context.Items["GridContext_" + renderId] as GridContext;
            if (FieldExpression != null)
            {
                if (string.IsNullOrEmpty(Field))
                {
                    Field = FieldExpression.Name;
                }
                if (string.IsNullOrEmpty(Title))
                {
                    Title = FieldExpression.Metadata.DisplayName;
                }
            }

            var childContent = output.GetChildContentAsync().Result;
            if (string.IsNullOrEmpty(Template) && !childContent.IsEmptyOrWhiteSpace)
            {
                var template = childContent.GetContent();
                if (!string.IsNullOrEmpty(template))
                {
                    var match = Regex.Match(template, @"<script(.*?)>(.*?)</script>");
                    if (match.Success)
                    {
                        Template = Field + "_" + Extensions.GenerateKeyword(5, true);
                        var js = new TagBuilder("script");
                        js.InnerHtml.Append(@"function " + Template + "(data, field, td){ ");
                        js.InnerHtml.Append(match.Groups[2].Value);
                        js.InnerHtml.Append("}");
                        gridContext.JsTemplates.Add(js);
                    }
                }
            }

            var column = new TagBuilder("th");
            column.InnerHtml.AppendHtml(Title);
            foreach (var item in output.Attributes)
            {
                column.MergeAttribute(item.Name, item.Value?.ToString(), true);
            }
            column.MergeAttribute("data-field", Field);
            column.MergeAttribute("data-orderable", Orderable ? "1" : "0");
            column.MergeAttribute("data-hidden", Hidden ? "1" : "0");
            if (Hidden)
            {
                column.AddCssClass("d-none");
            }
            if (gridContext.Request != null)
            {
                if (!string.IsNullOrEmpty(gridContext.Request.Sorting))
                {
                    var sorting = gridContext.Request.Sorting.Split(":");
                    if (sorting.First() == Field)
                    {
                        column.MergeAttribute("data-order", sorting.Last(), true);
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(OrderType))
                {
                    column.MergeAttribute("data-order", OrderType, true);
                }
            }
            if (!string.IsNullOrEmpty(Template))
            {
                column.MergeAttribute("data-template", Template, true);
            }

            gridContext.Columns.Add(column);

            output.SuppressOutput();
        }
    }

    public class GridContext
    {
        public List<TagBuilder> HtmlContents { get; set; }
        public List<IHtmlContent> Columns { get; set; }
        public List<IHtmlContent> JsTemplates { get; set; }
        public string Id { get; set; }
        public GridRequest Request { get; set; }

        public GridContext()
        {
            HtmlContents = new List<TagBuilder>();
            Columns = new List<IHtmlContent>();
            JsTemplates = new List<IHtmlContent>();
        }
    }
}
