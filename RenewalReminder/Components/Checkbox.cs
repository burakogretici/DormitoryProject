using System;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace RenewalReminder.CS.Web.Components
{
    [HtmlTargetElement("checkbox", TagStructure = TagStructure.WithoutEndTag)]
    public class Checkbox : TagHelper
    {
        public bool? Value { get; set; }
        public bool ShowLabel { get; set; } = true;
        public string Label { get; set; }
        public bool Disabled { get; set; }

        [HtmlAttributeName("asp-for")]
        public ModelExpression FieldExpression { get; set; }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        private Regex replaceRegex = new Regex("[^a-zA-Z0-9]");

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

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;

            var name = GetAttributeValue(output, "name", () => { return ""; });
            var id = GetAttributeValue(output, "id", () => { return ""; });
            if (FieldExpression != null)
            {
                name = GetAttributeValue(output, "name", () => { return Prefix + FieldExpression?.Name; });
                id = GetAttributeValue(output, "id", () => { return replaceRegex.Replace(Prefix + FieldExpression?.Name, "_"); });
                output.Attributes.RemoveAll("id");
                output.Attributes.RemoveAll("name");

                if (!Value.HasValue && FieldExpression.Model != null)
                {
                    var type = FieldExpression.Metadata.ModelType;
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        Value = FieldExpression.Model as bool?;
                    }
                    else
                    {
                        Value = (bool)FieldExpression.Model;
                    }
                }
                if (ShowLabel && string.IsNullOrEmpty(Label))
                {
                    Label = FieldExpression.Metadata.DisplayName;
                }
            }

            output.AddClass("custom-control", HtmlEncoder.Default);
            output.AddClass("custom-checkbox", HtmlEncoder.Default);

            var input = new TagBuilder("input");
            input.MergeAttribute("type", "checkbox");
            if (!string.IsNullOrEmpty(name))
            {
                input.MergeAttribute("name", name);
            }
            if (!string.IsNullOrEmpty(id))
            {
                input.MergeAttribute("id", id);
            }
            input.MergeAttribute("value", "true");
            if (Value.GetValueOrDefault())
            {
                input.MergeAttribute("checked", "checked");
            }
            input.AddCssClass("custom-control-input");

            var eventAttrs = new List<string>();
            foreach (var item in output.Attributes)
            {
                if (item.Name.StartsWith("on"))
                {
                    eventAttrs.Add(item.Name);
                }
            }
            foreach (var item in eventAttrs)
            {
                input.MergeAttribute(item, GetAttributeValue(output, item, () => { return ""; }));
                output.Attributes.RemoveAll(item);
            }

            if (Disabled)
            {
                input.MergeAttribute("disabled", "disabled");
            }

            output.Content.AppendHtml(input);

            var label = new TagBuilder("label");
            label.MergeAttribute("for", id);
            label.AddCssClass("custom-control-label");
            if (!string.IsNullOrEmpty(Label) && ShowLabel)
            {
                label.InnerHtml.AppendHtml(Label);
            }
            output.Content.AppendHtml(label);
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
    }
}
