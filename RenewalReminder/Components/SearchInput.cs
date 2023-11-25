using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using KvsProject;
using KvsProject.Components;

namespace KvsProject.CS.Web.Components
{
    [HtmlTargetElement("search-input")]
    public class SearchInput : TagHelper
    {
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        private string Prefix
        {
            get
            {
                if (ViewContext.ViewData.TemplateInfo != null && !string.IsNullOrEmpty(ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix))
                {
                    return ViewContext.ViewData.TemplateInfo.HtmlFieldPrefix + ".";
                }
                return string.Empty;
            }
        }

        public string Url { get; set; }
        public int PageSize { get; set; } = 20;
        public string PlaceHolder { get; set; }
        public object Value { get; set; }
        public bool ClearIfEmpty { get; set; } = true;
        public int StartSearch { get; set; } = 2;
        public bool Disabled { get; set; }
        public string EmptyValue { get; set; }

        private Regex replaceRegex = new Regex("[^a-zA-Z0-9]");

        [HtmlAttributeName("for")]
        public ModelExpression FieldExpression { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var renderId = Extensions.GenerateKeyword(8);
            var render = !context.Items.ContainsKey("RenderId");
            if (render)
            {
                context.Items.Add("RenderId", renderId);
                context.Items.Add("FilterContext_" + renderId, new FilterContext());
                context.Items.Add("SearchInputContext_" + renderId, new SearchInputContext());
            }
            renderId = context.Items["RenderId"] as string;
            var searchContext = context.Items["SearchInputContext_" + renderId] as SearchInputContext;

            var div = new TagBuilder("div");
            div.MergeAttribute("class", "input-group search-input");

            var value = GetAttributeValue(output, "value", () => { return FieldExpression?.Model?.ToString(); });
            if (value == null && context.AllAttributes.Any(a => a.Name.ToString() == "value"))
            {
                value = context.AllAttributes.FirstOrDefault(a => a.Name.ToString() == "value").Value?.ToString();
            }
            var name = GetAttributeValue(output, "name", () => { return Prefix + FieldExpression?.Name; });
            var id = GetAttributeValue(output, "id", () => { return default; });
            if (string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(name))
            {
                id = replaceRegex.Replace(name, "_");
            }

            var input = new TagBuilder("input");
            input.TagRenderMode = TagRenderMode.SelfClosing;
            foreach (var item in output.Attributes)
            {
                input.MergeAttribute(item.Name, item.Value?.ToString(), true);
            }
            output.Attributes.Clear();
            input.MergeAttribute("id", id + "Text");
            input.MergeAttribute("name", name + "Text");
            input.MergeAttribute("data-display", "1");
            input.MergeAttribute("autocomplete", "off");
            if (Disabled)
            {
                input.MergeAttribute("disabled", "disabled");
            }
            div.InnerHtml.AppendHtml(input);

            await output.GetChildContentAsync();

            if (string.IsNullOrEmpty(EmptyValue) && !(FieldExpression?.Metadata.IsNullableValueType).GetValueOrDefault())
            {
                EmptyValue = "0";
            }

            var hidden = new TagBuilder("input");
            hidden.TagRenderMode = TagRenderMode.SelfClosing;
            hidden.MergeAttribute("type", "hidden");
            hidden.MergeAttribute("value", value);
            hidden.MergeAttribute("id", id);
            hidden.MergeAttribute("name", name);
            hidden.MergeAttribute("data-url", Url, true);
            hidden.MergeAttribute("data-page-size", PageSize.ToString(), true);
            hidden.MergeAttribute("data-id", "1");
            hidden.MergeAttribute("data-clear-if-empty", ClearIfEmpty ? "1" : "0");
            hidden.MergeAttribute("data-empty-value", EmptyValue);
            hidden.MergeAttribute("data-start-search", StartSearch.ToString());

            div.InnerHtml.AppendHtml(hidden);

            searchContext.HtmlContents.Add(div);

            var configDiv = new TagBuilder("div");
            configDiv.AddCssClass("d-none");

            var colDiv = new TagBuilder("div");
            colDiv.MergeAttribute("data-columns", "1");
            foreach (var item in searchContext.Columns)
            {
                colDiv.InnerHtml.AppendHtml(item);
            }
            configDiv.InnerHtml.AppendHtml(colDiv);

            searchContext.HtmlContents.Add(configDiv);

            if (render)
            {
                output.TagName = "";
                output.TagMode = TagMode.StartTagAndEndTag;
                foreach (var item in searchContext.HtmlContents)
                {
                    output.Content.AppendHtml(item);
                }
            }
        }

        private string GetAttributeValue(TagHelperOutput output, string name, Func<string> fn)
        {
            var value = string.Empty;
            if (output.Attributes.TryGetAttribute(name, out TagHelperAttribute attribute))
            {
                value = attribute.Value?.ToString();
                output.Attributes.RemoveAll(name);
            }
            else if (fn != null)
            {
                value = fn.Invoke();
            }
            return value;
        }

        private string GetInnerHtmlString(TagBuilder content)
        {
            using (var writer = new System.IO.StringWriter())
            {
                content.InnerHtml.WriteTo(writer, HtmlEncoder.Default);
                return writer.ToString();
            }
        }
    }

    [HtmlTargetElement("column", ParentTag = "search-input")]
    public class SearchInputColumn : TagHelper
    {
        public string Field { get; set; }
        public string Title { get; set; }
        public bool Orderable { get; set; }
        public string OrderType { get; set; }
        public string Template { get; set; }
        public bool SearchIn { get; set; }
        public bool Display { get; set; }
        public FieldColumnType ColumnType { get; set; }

        [HtmlAttributeName("for")]
        public ModelExpression FieldExpression { get; set; }

        public SearchInputColumn()
        {
            Orderable = true;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var renderId = context.Items["RenderId"] as string;
            var searchContext = context.Items["SearchInputContext_" + renderId] as SearchInputContext;
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
                        searchContext.JsTemplates.Add(js);
                    }
                }
            }

            var column = new TagBuilder("div");
            column.InnerHtml.AppendHtml(Title);
            foreach (var item in output.Attributes)
            {
                column.MergeAttribute(item.Name, item.Value?.ToString(), true);
            }
            column.MergeAttribute("data-field", Field);
            column.MergeAttribute("data-orderable", Orderable ? "1" : "0");
            if (!string.IsNullOrEmpty(OrderType))
            {
                column.MergeAttribute("data-order", OrderType, true);
            }
            if (!string.IsNullOrEmpty(Template))
            {
                column.MergeAttribute("data-template", Template, true);
            }
            column.MergeAttribute("data-col-type", ((int)ColumnType).ToString(), true);
            column.MergeAttribute("data-search-in", SearchIn ? "1" : "0");
            column.MergeAttribute("data-display", Display ? "1" : "0");

            searchContext.Columns.Add(column);

            output.SuppressOutput();
        }
    }

    public class SearchInputContext
    {
        public List<TagBuilder> HtmlContents { get; set; }
        public List<IHtmlContent> Columns { get; set; }
        public List<IHtmlContent> JsTemplates { get; set; }
        public string Id { get; set; }

        public SearchInputContext()
        {
            HtmlContents = new List<TagBuilder>();
            Columns = new List<IHtmlContent>();
            JsTemplates = new List<IHtmlContent>();
        }
    }
    public enum FieldColumnType : int
    {
        ID = 0,
        NAME = 1,
        HIDDEN = 3
    }
}
