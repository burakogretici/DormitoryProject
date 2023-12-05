using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace KvsProject.CS.Web.Components
{
    [HtmlTargetElement("js-validation")]
    public class JsValidation : TagHelper
    {
        public Type Type { get; set; }
        public string FormSelector { get; set; } = "form";
        public string Ignore { get; set; }
        public string SubmitHandler { get; set; }
        public string InvalidHandler { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var validationContext = new JsValidationContext();
            context.Items.Add("JsValidationContext", validationContext);

            output.TagName = "script";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.SetAttribute("type", "text/javascript");

            if (Type == null)
            {
                Type = ViewContext.ViewData.ModelMetadata.ModelType;
            }

            await output.GetChildContentAsync();

            var fieldNames = new List<string>();
            var rules = new List<string>();
            foreach (PropertyDescriptor item in TypeDescriptor.GetProperties(Type))
            {
                fieldNames.Add(item.Name);
                var list = new List<string>();
                var itemValidation = validationContext.Fields.FirstOrDefault(a => a.Item1 == item.Name);
                if (itemValidation != null)
                {
                    if (!string.IsNullOrEmpty(itemValidation.Item2))
                    {
                        list.Add(itemValidation.Item2.Trim().TrimEnd(','));
                    }
                }
                else
                {
                    var type = item.PropertyType;
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        type = Nullable.GetUnderlyingType(type);
                    }
                    if (type == typeof(bool))
                    {
                        continue;
                    }
                    foreach (var attribute in item.Attributes)
                    {
                        if (attribute is RequiredAttribute)
                        {
                            list.Add("required: true");
                        }
                        else if (attribute is RangeAttribute)
                        {
                            list.Add("range: [" + ((RangeAttribute)attribute).Minimum + ", " + ((RangeAttribute)attribute).Maximum + "]");
                        }
                        else if (attribute is StringLengthAttribute)
                        {
                            list.Add("rangelength: [" + ((StringLengthAttribute)attribute).MinimumLength + ", " + ((StringLengthAttribute)attribute).MaximumLength + "]");
                        }
                        else if (attribute is MaxLengthAttribute)
                        {
                            list.Add("maxlength: " + ((MaxLengthAttribute)attribute).Length);
                        }
                        else if (attribute is MinLengthAttribute)
                        {
                            list.Add("minlength: " + ((MinLengthAttribute)attribute).Length);
                        }
                        else if (attribute is MinLengthAttribute)
                        {
                            list.Add("minlength: " + ((MinLengthAttribute)attribute).Length);
                        }
                        else if (attribute is EmailAddressAttribute)
                        {
                            list.Add("email: true");
                        }
                        else if (attribute is UrlAttribute)
                        {
                            list.Add("url: true");
                        }
                     
                        else if (attribute is CompareAttribute)
                        {
                            list.Add("equalTo: \"#" + ((CompareAttribute)attribute).OtherProperty + "\"");
                        }
                    }
                }
                if (list.Any())
                {
                    rules.Add("\"" + item.Name + "\"" + ": { " + string.Join(",", list) + " }");

                }
            }

            foreach (var item in validationContext.Fields.Where(a => !fieldNames.Contains(a.Item1)))
            {
                rules.Add("\"" + item.Item1 + "\"" + ": {" + item.Item2 + "}");
            }

            if (rules.Any())
            {
                var sb = new StringBuilder();
                sb.AppendLine("");
                sb.AppendLine("$().ready(function(){");
                sb.Append("\t$('");
                sb.Append(FormSelector);
                sb.AppendLine("').validate({");
                if (!string.IsNullOrEmpty(Ignore))
                {
                    sb.Append("\t\tignore: ");
                    sb.Append(Ignore);
                    sb.AppendLine(",");
                }
                if (!string.IsNullOrEmpty(SubmitHandler))
                {
                    sb.Append("submitHandler: function() {");
                    sb.Append(SubmitHandler);
                    sb.AppendLine("},");
                }
                if (!string.IsNullOrEmpty(InvalidHandler))
                {
                    sb.Append("invalidHandler: function() {");
                    sb.Append(InvalidHandler);
                    sb.AppendLine("},");
                }
                sb.AppendLine("\t\trules: {");

                for (int i = 0; i < rules.Count; i++)
                {
                    sb.Append("\t\t\t");
                    sb.Append(rules[i]);
                    if (i < rules.Count - 1)
                    {
                        sb.AppendLine(",");
                    }
                    else
                    {
                        sb.AppendLine("");
                    }
                }

                sb.AppendLine("\t\t}");
                sb.AppendLine("\t});");
                sb.AppendLine("});");

                output.Content.AppendHtml(sb.ToString());
            }
            else
            {
                output.SuppressOutput();
            }
        }
    }

    [HtmlTargetElement("rule", ParentTag = "js-validation", TagStructure = TagStructure.WithoutEndTag)]
    public class JsValidationRule : TagHelper
    {
        public string Name { get; set; }
        public string Rules { get; set; }

        [HtmlAttributeName("for")]
        public ModelExpression FieldExpression { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var validationContext = context.Items["JsValidationContext"] as JsValidationContext;

            if (FieldExpression != null)
            {
                Name = FieldExpression.Name;
            }

            if (!string.IsNullOrEmpty(Name))
            {
                validationContext.Fields.Add(new Tuple<string, string>(Name, Rules));
            }
            output.SuppressOutput();
        }

    }

    public class JsValidationContext
    {
        public string Id { get; set; }
        public List<Tuple<string, string>> Fields { get; set; }

        public JsValidationContext()
        {
            Fields = new List<Tuple<string, string>>();
        }
    }
}
