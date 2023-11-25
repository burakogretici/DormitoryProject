using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using KvsProject.CS.Web.Components;
using RenewalRemindr.Models;

namespace KvsProject.Components
{
    [HtmlTargetElement("filters")]
    public class SearchFilter : TagHelper
    {
        public bool ShowLabel { get; set; }
        public int ColumnCount { get; set; } = 3;
        public FilterButtonType ButtonType { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var renderId = Extensions.GenerateKeyword(8);
            var render = !context.Items.ContainsKey("RenderId");
            if (render)
            {
                context.Items.Add("RenderId", renderId);
                context.Items.Add("FilterContext_" + renderId, new FilterContext());
            }

            renderId = context.Items["RenderId"] as string;
            var filterContext = context.Items["FilterContext_" + renderId] as FilterContext;
            filterContext.ColumnSize = (int)Math.Ceiling(12M / ColumnCount);
            filterContext.ShowLabel = ShowLabel;

            output.GetChildContentAsync().Wait();

            var form = new TagBuilder("div");
            foreach (var item in output.Attributes)
            {
                form.MergeAttribute(item.Name, item.Value?.ToString(), true);
            }
            output.Attributes.Clear();
            form.AddCssClass("form-row");

            var id = form.Attributes.GetValueOrDefault("id");
            if (string.IsNullOrEmpty(id))
            {
                id = "filter" + Extensions.GenerateKeyword(5, true);
                form.MergeAttribute("id", id, true);
            }
            filterContext.Id = id;

            //var lgColumn = 0;
            //var mdColumn = 0;
            for (int i = 0; i < filterContext.Fields.Count; i++)
            {
                var div = filterContext.Fields[i].Item1 as TagBuilder;
                if (i == filterContext.Fields.Count - 1 && ButtonType == FilterButtonType.ICON)
                {
                    var searchButton = new TagBuilder("button");
                    searchButton.AddCssClass("btn btn-sm btn-warning ml-1 search-btn");
                    searchButton.MergeAttribute("type", "button");
                    searchButton.InnerHtml.AppendHtml(new HtmlString(@"<i class=""fas fa-search""></i>"));
                    if (filterContext.ShowLabel)
                    {
                        var outerDiv = new TagBuilder("div");
                        outerDiv.AddCssClass(div.Attributes.GetValueOrDefault("class"));
                        div.MergeAttribute("class", "flex-grow-1", true);
                        outerDiv.InnerHtml.AppendHtml(div);
                        searchButton.AddCssClass("ml-1 align-self-end");
                        div = outerDiv;
                    }
                    div.AddCssClass("d-flex");
                    div.InnerHtml.AppendHtml(searchButton);

                    if (filterContext.ButtonFields.Any())
                    {
                        foreach (var item in filterContext.ButtonFields)
                        {
                            div.InnerHtml.AppendHtml(item);
                        }
                    }
                }

                form.InnerHtml.AppendHtml(div);
            }

            if (!filterContext.Fields.Any())
            {
                if (filterContext.ButtonFields.Any())
                {
                    foreach (var item in filterContext.ButtonFields)
                    {
                        form.InnerHtml.AppendHtml(item);
                    }
                }
            }

            if (ButtonType == FilterButtonType.BUTTON)
            {
                var div = new TagBuilder("div");
                div.AddCssClass("text-right col-12");

                if (filterContext.ButtonFields.Any())
                {
                    foreach (var item in filterContext.ButtonFields)
                    {
                        div.InnerHtml.AppendHtml(item);
                    }
                }

                var searchButton = new TagBuilder("button");
                searchButton.AddCssClass("btn btn-sm btn-warning");
                searchButton.MergeAttribute("type", "submit");
                searchButton.InnerHtml.AppendHtml(new HtmlString(@"<i class=""fas fa-search""></i> "));
                searchButton.InnerHtml.Append(" Ara");
                div.InnerHtml.AppendHtml(searchButton);


                form.InnerHtml.AppendHtml(div);
            }

            foreach (var item in filterContext.HiddenFields)
            {
                form.InnerHtml.AppendHtml(item);
            }

            if (render)
            {
                output.TagName = "";
                output.TagMode = TagMode.StartTagAndEndTag;
                output.Content.AppendHtml(form);
            }
            else
            {
                filterContext.HtmlContent = form;
                output.SuppressOutput();
            }
        }
    }

    [HtmlTargetElement("field", ParentTag = "filters")]
    public class SearchFilterField : TagHelper
    {
        public string Names { get; set; }
        public FilterType Type { get; set; }
        public FilterOperant Operant { get; set; }
        public int LgSize { get; set; } = 1;
        public int MdSize { get; set; }
        public IEnumerable<SelectListItem> Items { get; set; }
        public string Label { get; set; }
        public string PlaceHolder { get; set; }
        public object Value { get; set; }

        [HtmlAttributeName("for")]
        public ModelExpression FieldExpression { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var renderId = context.Items["RenderId"] as string;
            var filterContext = context.Items["FilterContext_" + renderId] as FilterContext;

            if (FieldExpression != null)
            {
                Names = FieldExpression.Name;
                if (string.IsNullOrEmpty(Label))
                {
                    Label = FieldExpression.Metadata.DisplayName;
                }
            }

            if (string.IsNullOrEmpty(PlaceHolder) && Type != FilterType.HIDDEN)
            {
                PlaceHolder = Label;
                var formatString = "{0} Ara...";
                if (Type == FilterType.SELECT)
                {
                    formatString = "{0} Seçiniz";
                }
                PlaceHolder = string.Format(formatString, Label);
            }

            if (MdSize == 0)
            {
                MdSize = LgSize * 2;
            }

            var operant = GetOperantString();
            var lgSize = filterContext.ColumnSize * LgSize > 12 ? 12 : filterContext.ColumnSize * LgSize;
            var mdSize = filterContext.ColumnSize * MdSize > 12 ? 12 : filterContext.ColumnSize * MdSize;

            var filters = GetFilters(context);
            var value = filters.Any() ? filters.First().Value : string.Empty;
            if (Value != null)
            {
                value = Value.ToString();
            }

            ViewContext.ViewBag.Filters = filters;
            ViewContext.ViewBag.FilterValue = value;
            

            var childContent = output.GetChildContentAsync().Result;

            if (Type == FilterType.BUTTON)
            {
                var button = new TagBuilder("button");
                foreach (var item in output.Attributes)
                {
                    button.MergeAttribute(item.Name, item.Value?.ToString(), true);
                }
                button.MergeAttribute("type", "button", true);
                button.InnerHtml.AppendHtml(childContent.GetContent());
                filterContext.ButtonFields.Add(button);

                output.SuppressOutput();
                return;
            }

            if (!childContent.IsEmptyOrWhiteSpace)
            {
                var html = childContent.GetContent();
                if (Type == FilterType.HIDDEN)
                {
                    filterContext.HiddenFields.Add(new HtmlString(html));
                }
                else
                {
                    var div = new TagBuilder("div");
                    div.AddCssClass(string.Format("col-lg-{0} col-md-{1}", lgSize, mdSize));
                    div.InnerHtml.AppendHtml(html);
                    filterContext.Fields.Add(new Tuple<IHtmlContent, int, int>(div, mdSize, lgSize));
                }
            }
            else
            {
                

                var div = new TagBuilder("div");
                div.AddCssClass(string.Format("col-lg-{0} col-md-{1} mt-md-1", lgSize, mdSize));
                if (filterContext.ShowLabel)
                {
                    div.AddCssClass("form-group");
                    var label = new TagBuilder("label");
                    label.AddCssClass("font-weight-semibold");
                    label.InnerHtml.SetContent(Label);
                    div.InnerHtml.AppendHtml(label);
                }
                if (Type == FilterType.TEXT || Type == FilterType.DATE || Type == FilterType.DECIMAL || Type == FilterType.INT || Type == FilterType.HIDDEN)
                {
                    var input = new TagBuilder("input");
                    input.TagRenderMode = TagRenderMode.SelfClosing;
                    foreach (var item in output.Attributes)
                    {
                        input.MergeAttribute(item.Name, item.Value?.ToString(), true);
                    }
                    if (Type != FilterType.HIDDEN)
                    {
                        input.MergeAttribute("type", "text");
                        input.AddCssClass("form-control form-control-sm");
                        input.MergeAttribute("placeHolder", PlaceHolder, true);
                    }
                    else
                    {
                        input.MergeAttribute("type", "hidden");
                    }
                    input.MergeAttribute("autocomplete", "off", true);
                    input.MergeAttribute("data-field", Names, true);
                    input.MergeAttribute("value", value, true);
                    input.MergeAttribute("data-operant", operant, true);

                    if (Type == FilterType.DATE)
                    {
                        input.AddCssClass("date-picker");
                    }
                    else if (Type == FilterType.DECIMAL)
                    {
                        input.AddCssClass("float-text");
                    }
                    else if (Type == FilterType.INT)
                    {
                        input.AddCssClass("int-text");
                    }

                    if (Type != FilterType.HIDDEN)
                    {
                        div.InnerHtml.AppendHtml(input);
                    }
                    else
                    {
                        filterContext.HiddenFields.Add(input);
                    }
                }
                else if (Type == FilterType.SELECT)
                {
                    var input = new TagBuilder("select");
                    foreach (var item in output.Attributes)
                    {
                        input.MergeAttribute(item.Name, item.Value?.ToString(), true);
                    }
                    input.MergeAttribute("data-field", Names, true);
                    input.MergeAttribute("data-operant", operant, true);
                    input.AddCssClass("form-control form-control-sm");

                    var emptyOption = new TagBuilder("option");
                    emptyOption.MergeAttribute("value", "");
                    emptyOption.InnerHtml.SetContent(PlaceHolder);
                    input.InnerHtml.AppendHtml(emptyOption);

                    if (Items != null)
                    {
                        foreach (var item in Items)
                        {
                            var option = new TagBuilder("option");
                            option.MergeAttribute("value", item.Value);
                            option.InnerHtml.SetContent(item.Text);
                            if (item.Value == value)
                            {
                                option.MergeAttribute("selected", "selected");
                            }
                            input.InnerHtml.AppendHtml(option);
                        }
                    }

                    div.InnerHtml.AppendHtml(input);
                }

                if (Type != FilterType.HIDDEN)
                {
                    filterContext.Fields.Add(new Tuple<IHtmlContent, int, int>(div, mdSize, lgSize));
                }
            }

            output.SuppressOutput();
        }

        public List<GridFilter> GetFilters(TagHelperContext context)
        {
            var renderId = context.Items["RenderId"] as string;

            var values = new List<GridFilter>();
            
            if (context.Items.ContainsKey("GridContext_" + renderId))
            {
                var gridContext = context.Items["GridContext_" + renderId] as GridContext;
                if (gridContext.Request != null && gridContext.Request.Filters != null)
                {
                    foreach (var item in gridContext.Request.Filters.Where(a => a.Field == Names))
                    {
                        values.Add(item);
                    }
                }
            }

            return values;
        }

        private string GetOperantString()
        {
            var operant = "*";
            switch (Operant)
            {
                case FilterOperant.CONTAINS:
                    operant = "*";
                    break;
                case FilterOperant.ENDS_WITH:
                    operant = "-";
                    break;
                case FilterOperant.EQUAL:
                    operant = "=";
                    break;
                case FilterOperant.GREATER_THAN:
                    operant = ">";
                    break;
                case FilterOperant.GREATER_THANEQUAL:
                    operant = ">=";
                    break;
                case FilterOperant.LESS_THAN:
                    operant = "<";
                    break;
                case FilterOperant.LESS_THANEQUAL:
                    operant = "<=";
                    break;
                case FilterOperant.NOT_CONTAINS:
                    operant = "!*";
                    break;
                case FilterOperant.NOT_EQUAL:
                    operant = "!=";
                    break;
                case FilterOperant.STARTS_WITH:
                    operant = "+";
                    break;

                default:
                    break;
            }
            return operant;
        }
    }

    public class FilterContext
    {
        public TagBuilder HtmlContent { get; set; }
        public string Id { get; set; }
        public List<Tuple<IHtmlContent, int, int>> Fields { get; set; }
        public List<IHtmlContent> HiddenFields { get; set; }
        public List<IHtmlContent> ButtonFields { get; set; }
        public int ColumnSize { get; set; }
        public bool ShowLabel { get; set; }

        public FilterContext()
        {
            Fields = new List<Tuple<IHtmlContent, int, int>>();
            HiddenFields = new List<IHtmlContent>();
            ButtonFields = new List<IHtmlContent>();
        }
    }

    public enum FilterButtonType
    {
        ICON,
        BUTTON
    }

    public enum FilterType
    {
        TEXT,
        INT,
        DECIMAL,
        SELECT,
        DATE,
        HIDDEN,
        BUTTON
    }
    public enum FilterOperant
    {
        CONTAINS,
        EQUAL,
        GREATER_THAN,
        GREATER_THANEQUAL,
        LESS_THAN,
        LESS_THANEQUAL,
        STARTS_WITH,
        ENDS_WITH,
        NOT_CONTAINS,
        NOT_EQUAL
    }
}
